using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Views;
using GO.Common.Droid;
using GO.Common.Droid.Fragments;
using GO.Core.Data;
using GO.Core.Entities;
using GO.Core.Enums;
using GO.Core.Helpers;
using GO.Core.Utilities;
using GO.Hunting.Droid.Adapters;
using Newtonsoft.Json;

namespace GO.Hunting.Droid.Fragments
{
   public class CMapFragment : CMapFragmentBase, IOnMapReadyCallback
   {
      static readonly LatLng Location_Minsk = new LatLng(53.900819, 27.558823);

      GoogleMap map;
      MapView mapFragment;
      List<MarkerOptions> _markers;

      private CancellationTokenSource _cancellationMapAutoUpdate;

      public CMapFragment() { }

      public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         if (container == null)
         {
            return null;
         }

         MapLayoutInflater = inflater;

         if (MapView == null)
         {
            MapView = inflater.Inflate(Resource.Layout.fragment_cmap, container, false);
         }

         mapFragment = MapView.FindViewById<MapView>(Resource.Id.map);
         if (mapFragment != null)
         {
            mapFragment.OnCreate(savedInstanceState);
            mapFragment.OnResume();
            mapFragment.GetMapAsync(this);
         }

         return MapView;
      }

      public override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

         UpdateFrequency = MapSettingsService.GetUpdateFrequency();
         MapType = (MapType)MapSettingsService.GetMapType();
         _markers = new List<MarkerOptions>();

         SetHasOptionsMenu(true);
      }

      private async void StartAutoMapUpdate()
      {
         LastUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
         _cancellationMapAutoUpdate = new CancellationTokenSource();

         while (!_cancellationMapAutoUpdate.Token.IsCancellationRequested)
         {
            await Task.Delay(2000);
            var pastMinutes = (DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc) - LastUpdated).TotalMinutes;
            if (pastMinutes > UpdateFrequency)
            {
               await UpdateMarkersAsync();
               LastUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
               break;
            }
         }
      }

      private void StopAutoMapUpdate()
      {
         if (_cancellationMapAutoUpdate == null)
            return;

         if (_cancellationMapAutoUpdate.Token.CanBeCanceled && !_cancellationMapAutoUpdate.Token.IsCancellationRequested)
         {
            _cancellationMapAutoUpdate.Cancel();
         }
      }

      public override void OnStart()
      {
         base.OnStart();
         try
         {
            AppLocation.Current.LocationService.LocationChanged += HandleLocationChanged;
            AppLocation.Current.LocationService.ProviderEnabled += HandleLocationEnabled;
            AppLocation.Current.LocationService.ProviderDisabled += HandleLocationDisabled;
         }
         catch (Exception ex)
         {
            ShowAlert(string.Format("LocationService: {0}", ex.Message));
         }
         StartAutoMapUpdate();
      }

      public override void OnStop()
      {
         try
         {
            AppLocation.Current.LocationService.LocationChanged -= HandleLocationChanged;
            AppLocation.Current.LocationService.ProviderEnabled -= HandleLocationEnabled;
            AppLocation.Current.LocationService.ProviderDisabled -= HandleLocationDisabled;
         }
         catch (Exception ex)
         {
            ShowAlert(string.Format("LocationService: {0}", ex.Message));
         }

         StopAutoMapUpdate();

         base.OnStop();
      }

      public void HandleLocationEnabled(object sender, ProviderEnabledEventArgs e)
      {
         Activity.RunOnUiThread(() =>
         {
            MapCurrentLocation = null;
            AppLocation.Current.LocationService.LocationChanged += HandleLocationChanged;
            ToastService.ShowMessageLongPeriod(string.Format("Started processing your location. Looks like you turned on GPS."));
         });
      }

      public void HandleLocationDisabled(object sender, ProviderDisabledEventArgs e)
      {
         Activity.RunOnUiThread(() =>
         {
            MapCurrentLocation = null;
            AppLocation.Current.LocationService.LocationChanged -= HandleLocationChanged;
            ToastService.ShowMessageLongPeriod(string.Format("Stopped processing your location. Looks like you turned off GPS."));
            Activity.Title = FragmentTitle;
         });
      }

      public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
      {
         MapCurrentLocation = e.Location;
         if (MapCurrentLocation != null)
         {
            UpdateNearestPointInformation();
            Activity.Title = string.Format("{0} м: {1}", DistanceToNearestPoint.ToString("0.00"), NameOfNearestPoint);
         }
      }

      public void OnMapReady(GoogleMap googleMap)
      {
         map = googleMap;
         switch (MapType)
         {
            case MapType.Normal:
               map.MapType = GoogleMap.MapTypeNormal;
               break;
            case MapType.Hybrid:
               map.MapType = GoogleMap.MapTypeHybrid;
               break;
            case MapType.Terrain:
               map.MapType = GoogleMap.MapTypeTerrain;
               break;
            default:
               map.MapType = GoogleMap.MapTypeNormal;
               break;
         }
         map.MyLocationEnabled = true;
         map.UiSettings.MyLocationButtonEnabled = true;
         map.UiSettings.ZoomControlsEnabled = true;
         map.SetInfoWindowAdapter(new CustomInfoWindowAdapter(MapLayoutInflater, ToastService, AppSettingsService));

         CameraUpdate update = CameraUpdateFactory.NewLatLngZoom(Location_Minsk, 11);
         map.MoveCamera(update);

         Task.Run(async () =>
         {
            await UpdateMarkersAsync();
         });
      }

      private async Task UpdateMarkersAsync()
      {
         IsLoading = true;

         StopAutoMapUpdate();
         await UpdateMarkers();
         StartAutoMapUpdate();

         IsLoading = false;
      }

      async Task UpdateMarkers()
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         if (map == null)
         {
            IsLoading = false;
            return;
         }

         ClearMap();

         ErrorInfo errorInfo = await ApiService.CheckUserAccess(AppSettingsService.GetAppId());
         if (errorInfo.status == "blocked")
         {
            ShowAlert(errorInfo.message);
         }
         else
         {
            var points = await ApiService.GetAll(AppSettingsService.GetAppId());

            if (points == null)
            {
               Logger.Instance.Debug("CMapFragment.UpdateMarkers: points list is null");
               return;
            }

            if (MapItemFilterType.HasValue)
            {
               points = points.Where(x => x.GetMapItemType == MapItemFilterType);
            }

            IsLoading = false;

            lock (_lockObject)
            {
               _markers = new List<MarkerOptions>();
               foreach (var point in points)
               {
                  try
                  {
                     if (point.IsValid())
                     {
                        var iconName = point.GetIconName;
                        BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resources.GetIdentifier(iconName, "drawable", this.Activity.PackageName));
                        var marker = new MarkerOptions()
                        .SetPosition(new LatLng(point.GetLatitude, point.GetLongitude))
                        .SetSnippet(JsonConvert.SerializeObject(point))
                        .SetTitle(point.GetContent)
                        .SetIcon(icon);
                        _markers.Add(marker);

                        AddMarkerOnMap(marker);
                     }
                  }
                  catch (Exception ex)
                  {
                     Logger.Instance.Error(string.Format("CMapFragmer.UpdateMarkers exception: {0}", ex.Message));
                  }
               }
            }
         }

         IsLoading = false;
      }

      private object _lockObject = new object();

      void UpdateNearestPointInformation()
      {
         DistanceToNearestPoint = float.MaxValue;
         MarkerOptions nearestMarker = new MarkerOptions();
         if (MapCurrentLocation != null)
         {
            lock (_lockObject)
            {
               foreach (var marker in _markers)
               {
                  float[] results = { 0 };
                  Location.DistanceBetween(MapCurrentLocation.Latitude, MapCurrentLocation.Longitude, marker.Position.Latitude, marker.Position.Longitude, results);
                  if (DistanceToNearestPoint > results[0])
                  {
                     DistanceToNearestPoint = results[0];
                     nearestMarker = marker;
                  }
               }
            }
         }
         DistanceToNearestPoint = Math.Round(DistanceToNearestPoint, 2);
         NameOfNearestPoint = nearestMarker.Title;
      }

      public override bool OnOptionsItemSelected(IMenuItem item)
      {
         switch (item.ItemId)
         {
            case 0:
               base.AnalyticsService.TrackState("Conquer", "Hit on Conquer button", string.Format("User {0} is tryuing to conquer point {1}", AppSettingsService.GetAppId(), NameOfNearestPoint));
               ActionHandler(ActionType.Point);
               return true;
            case 1:
               ActionHandler(ActionType.Quest);
               return true;
            case 2:
               ActionHandler(ActionType.Trap);
               return true;
            case 3:
               ActionHandler(ActionType.Place);
               return true;
            case 4:
               ActionHandler(ActionType.Raze);
               return true;
            case 5:
               ActionHandler(ActionType.Attack);
               return true;
            case 6:
               MapItemFilterType = null;
               UpdateMarkersAsync();
               return true;
            case 7:
               MapItemFilterType = MapItemType.Point;
               MapMenu.Clear();
               Activity.InvalidateOptionsMenu();
               UpdateMarkersAsync();
               return true;
            case 8:
               MapItemFilterType = MapItemType.Quest;
               MapMenu.Clear();
               Activity.InvalidateOptionsMenu();
               UpdateMarkersAsync();
               return true;
            case 9:
               MapItemFilterType = null;
               MapMenu.Clear();
               Activity.InvalidateOptionsMenu();
               UpdateMarkersAsync();
               return true;
            case 10:
               AnalyticsService.TrackState("Conquer", "Hit on Logout button", string.Format("User {0} is logout", AppSettingsService.GetAppId()));
               Logout();
               return true;
            default:
               return base.OnOptionsItemSelected(item);
         }
      }

      async void Logout()
      {
         AppLocation.StopLocationService();
         Process.KillProcess(Process.MyPid());
      }

      private async void ActionHandler(ActionType type)
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         string description = this.Resources.GetString(Resource.String.GPSNotDefined);
         if (MapCurrentLocation != null)
         {
            if (MapCurrentLocation.IsFromMockProvider)
            {
               description = this.Resources.GetString(Resource.String.AllowMockLocationsShouldBeDisabled);
            }
            else
            {
               ActionResponseBase result = await UserActionService.MakeAction(type, AppSettingsService.GetAppId(), MapCurrentLocation.Latitude.ProcessCoordinate(), MapCurrentLocation.Longitude.ProcessCoordinate());
               description = result.GetDescription;
               if (result.IsSuccess)
               {
                  await UpdateMarkersAsync();

                  UserActionService.Add(new UserAction
                  {
                     Type = (int)MapItemType.Quest,
                     Title = result.title,
                     Number = result.number,
                     Description = result.GetDescription,
                     Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                  });
               }
            }
         }

         IsLoading = false;
         this.ShowAlert(description);
      }

      public override string FragmentTitle => Resources.GetString(Resource.String.DrawerMap);

      private void ClearMap()
      {
         Activity.RunOnUiThread(() =>
         {
            try
            {
               map.Clear();
            }
            catch (Exception ex)
            {
               Logger.Instance.Error(ex.Message);
            }
         });
      }

      private void AddMarkerOnMap(MarkerOptions marker)
      {
         Activity.RunOnUiThread(() =>
         {
            try
            {
               map.AddMarker(marker);
            }
            catch (Exception ex)
            {
               Logger.Instance.Error(ex.Message);
            }
         });
      }
   }
}