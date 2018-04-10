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
using GO.Core.Data;
using GO.Core.Entities;
using GO.Core.Enums;
using GO.Core.Helpers;
using GO.Core.Services;
using GO.Core.Utilities;
using MvvmCross.Platform;
using Newtonsoft.Json;

namespace GO.Common.Droid.Fragments
{
   public class CMapFragmentBase : FragmentBase
   {
      public static readonly LatLng Location_Minsk = new LatLng(53.900819, 27.558823);

      protected IApiService ApiService;
      protected IToastService ToastService;
      protected IDBService DbService;
      protected IUserActionService UserActionService;
      protected IMapSettingsService MapSettingsService;
      protected IAppSettingsService AppSettingsService;

      private object _lockObject = new object();

      protected View MapView;
      protected IMenu MapMenu;
      protected MenuInflater MapMenuInflater;
      protected LayoutInflater MapLayoutInflater;
      protected Location MapCurrentLocation;
      protected double DistanceToNearestPoint;
      protected string NameOfNearestPoint;
      protected double UpdateFrequency;
      protected MapType MapType;
      protected DateTime LastUpdated;
      protected MapItemType? MapItemFilterType;
      protected GoogleMap GMap;
      protected MapView MapFragmentView;
      protected List<MarkerOptions> MapMarkers;
      protected CancellationTokenSource CancellationMapAutoUpdate;

      public CMapFragmentBase()
      {
         DbService = Mvx.Resolve<IDBService>();
         ApiService = Mvx.Resolve<IApiService>();
         ToastService = Mvx.Resolve<IToastService>();
         UserActionService = Mvx.Resolve<IUserActionService>();
         MapSettingsService = Mvx.Resolve<IMapSettingsService>();
         AppSettingsService = Mvx.Resolve<IAppSettingsService>();
      }

      public override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

         UpdateFrequency = MapSettingsService.GetUpdateFrequency();
         MapType = (MapType)MapSettingsService.GetMapType();
         MapMarkers = new List<MarkerOptions>();

         SetHasOptionsMenu(true);
      }

      public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
      {
         MapMenu = menu;
         MapMenuInflater = inflater;

         MapMenu.Add(0, 0, 0, Resource.String.ConquerMenuTitle);
         MapMenu.Add(0, 1, 1, Resource.String.QuestMenuTitle);
         MapMenu.Add(0, 2, 2, Resource.String.TrapMenuTitle);
         MapMenu.Add(0, 3, 3, Resource.String.PlaceMenuTitle);
         MapMenu.Add(0, 4, 4, Resource.String.RazeMenuTitle);
         MapMenu.Add(0, 5, 5, Resource.String.AttackMenuTitle);
         MapMenu.Add(0, 6, 6, Resource.String.RefreshMenuTitle);
         MapMenu.Add(0, 7, 7, Resource.String.OnlyPointsMenuTitle);
         MapMenu.Add(0, 8, 8, Resource.String.OnlyQuestsMenuTitle);
         MapMenu.Add(0, 9, 9, Resource.String.AllObjectsMenuTitle);
         MapMenu.Add(0, 10, 10, Resource.String.LogoutMenuTitle);
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

      protected async void StartAutoMapUpdate()
      {
         LastUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
         CancellationMapAutoUpdate = new CancellationTokenSource();

         while (!CancellationMapAutoUpdate.Token.IsCancellationRequested)
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

      protected void StopAutoMapUpdate()
      {
         if (CancellationMapAutoUpdate == null)
            return;

         if (CancellationMapAutoUpdate.Token.CanBeCanceled && !CancellationMapAutoUpdate.Token.IsCancellationRequested)
         {
            CancellationMapAutoUpdate.Cancel();
         }
      }

      protected async Task UpdateMarkersAsync()
      {
         IsLoading = true;

         StopAutoMapUpdate();
         await UpdateMarkers();
         StartAutoMapUpdate();

         IsLoading = false;
      }

      protected async Task UpdateMarkers()
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         if (GMap == null)
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
               MapMarkers = new List<MarkerOptions>();
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
                        MapMarkers.Add(marker);

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

      private void ClearMap()
      {
         Activity.RunOnUiThread(() =>
         {
            try
            {
               GMap.Clear();
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
               GMap.AddMarker(marker);
            }
            catch (Exception ex)
            {
               Logger.Instance.Error(ex.Message);
            }
         });
      }

      protected void UpdateNearestPointInformation()
      {
         DistanceToNearestPoint = float.MaxValue;
         MarkerOptions nearestMarker = new MarkerOptions();
         if (MapCurrentLocation != null)
         {
            lock (_lockObject)
            {
               foreach (var marker in MapMarkers)
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

      private async void Logout()
      {
         AppLocation.StopLocationService();
         Process.KillProcess(Process.MyPid());
      }
   }
}