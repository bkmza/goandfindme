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
using Cirrious.CrossCore;
using DroidMapping.Adapters;
using GoHunting.Core;
using GoHunting.Core.Data;
using GoHunting.Core.Entities;
using GoHunting.Core.Enums;
using GoHunting.Core.Helpers;
using GoHunting.Core.Services;
using GoHunting.Core.Utilities;
using Newtonsoft.Json;

namespace DroidMapping.Fragments
{
   public class CMapFragment : FragmentBase, IOnMapReadyCallback
   {
      static readonly LatLng Location_Minsk = new LatLng (53.900819, 27.558823);

      View _view;

      IApiService _apiService;
      IToastService _toastService;
      IDBService _dbService;
      IUserActionService _userActionService;
      IMapSettingsService _mapSettingsService;

      Location _currentLocation;
      GoogleMap map;
      MapView mapFragment;
      List<MarkerOptions> _markers;

      LayoutInflater _layoutInflater;

      double _distanceToNearestPoint;
      string _nameOfNearestPoint;

      private int UpdateFrequency;
      private MapType MapType;
      private DateTime LastUpdated;
      private CancellationTokenSource _cancellationMapAutoUpdate;

      MapItemType? _mapItemFilterType;

      public CMapFragment ()
      {
         _dbService = Mvx.Resolve<IDBService> ();
         _apiService = Mvx.Resolve<IApiService> ();
         _toastService = Mvx.Resolve<IToastService> ();
         _userActionService = Mvx.Resolve<IUserActionService> ();
         _mapSettingsService = Mvx.Resolve<IMapSettingsService> ();
      }

      public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         if (container == null) {
            return null;
         }

         _layoutInflater = inflater;

         if (_view == null) {
            _view = inflater.Inflate (Resource.Layout.fragment_cmap, container, false);
         }

         mapFragment = _view.FindViewById<MapView> (Resource.Id.map);
         if (mapFragment != null) {
            mapFragment.OnCreate (savedInstanceState);
            mapFragment.OnResume ();
            mapFragment.GetMapAsync (this);
         }

         return _view;
      }

      public override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);

         UpdateFrequency = _mapSettingsService.GetUpdateFrequency ();
         MapType = (MapType)_mapSettingsService.GetMapType ();
         _markers = new List<MarkerOptions> ();

         SetHasOptionsMenu (true);
      }

      private async void StartAutoMapUpdate ()
      {
         LastUpdated = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Utc);
         _cancellationMapAutoUpdate = new CancellationTokenSource ();

         while (!_cancellationMapAutoUpdate.Token.IsCancellationRequested) {
            await Task.Delay (2000);
            var pastMinutes = (DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Utc) - LastUpdated).TotalMinutes;
            if (pastMinutes > UpdateFrequency) {
               await UpdateMarkersAsync ();
               LastUpdated = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Utc);
            }
         }
      }

      private void StopAutoMapUpdate ()
      {
         if (_cancellationMapAutoUpdate == null)
            return;

         if (_cancellationMapAutoUpdate.Token.CanBeCanceled && !_cancellationMapAutoUpdate.Token.IsCancellationRequested) {
            _cancellationMapAutoUpdate.Cancel ();
         }
      }

      public override void OnStart ()
      {
         base.OnStart ();
         try {
            AppLocation.Current.LocationService.LocationChanged += HandleLocationChanged;
            AppLocation.Current.LocationService.ProviderEnabled += HandleLocationEnabled;
            AppLocation.Current.LocationService.ProviderDisabled += HandleLocationDisabled;
         } catch (Exception ex) {
            ShowAlert (string.Format ("LocationService: {0}", ex.Message));
         }
         StartAutoMapUpdate ();
      }

      public override void OnStop ()
      {
         try {
            AppLocation.Current.LocationService.LocationChanged -= HandleLocationChanged;
            AppLocation.Current.LocationService.ProviderEnabled -= HandleLocationEnabled;
            AppLocation.Current.LocationService.ProviderDisabled -= HandleLocationDisabled;
         } catch (Exception ex) {
            ShowAlert (string.Format ("LocationService: {0}", ex.Message));
         }

         StopAutoMapUpdate ();

         base.OnStop ();
      }

      public void HandleLocationEnabled (object sender, ProviderEnabledEventArgs e)
      {
         Activity.RunOnUiThread (() => {
            _currentLocation = null;
            AppLocation.Current.LocationService.LocationChanged += HandleLocationChanged;
            _toastService.ShowMessageLongPeriod (string.Format ("Started processing your location. Looks like you turned on GPS."));
         });
      }

      public void HandleLocationDisabled (object sender, ProviderDisabledEventArgs e)
      {
         Activity.RunOnUiThread (() => {
            _currentLocation = null;
            AppLocation.Current.LocationService.LocationChanged -= HandleLocationChanged;
            _toastService.ShowMessageLongPeriod (string.Format ("Stopped processing your location. Looks like you turned off GPS."));
            Activity.Title = FragmentTitle;
         });
      }

      public void HandleLocationChanged (object sender, LocationChangedEventArgs e)
      {
         _currentLocation = e.Location;
         if (_currentLocation != null) {
            UpdateNearestPointInformation ();
            Activity.Title = string.Format ("{0} м: {1}", _distanceToNearestPoint.ToString ("0.00"), _nameOfNearestPoint);
         }
      }

      public void OnMapReady (GoogleMap googleMap)
      {
         map = googleMap;
         switch (MapType) {
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
         map.SetInfoWindowAdapter (new CustomInfoWindowAdapter (_layoutInflater, _toastService));

         CameraUpdate update = CameraUpdateFactory.NewLatLngZoom (Location_Minsk, 11);
         map.MoveCamera (update);

         Task.Run (async () => {
            await UpdateMarkersAsync ();
         });
      }

      private async Task UpdateMarkersAsync ()
      {
         IsLoading = true;

         StopAutoMapUpdate ();
         await UpdateMarkers ();
         StartAutoMapUpdate ();

         IsLoading = false;
      }

      async Task UpdateMarkers ()
      {
         if (!CheckInternetConnection ()) {
            IsLoading = false;
            return;
         }

         if (map == null) {
            IsLoading = false;
            return;
         }

         ClearMap ();

         ErrorInfo errorInfo = await _apiService.CheckUserAccess (DeviceUtility.DeviceId);
         if (errorInfo.status == "blocked") {
            ShowAlert (errorInfo.message);
         } else {
            var points = await _apiService.GetAll (DeviceUtility.DeviceId);

            if (points == null) {
               Logger.Instance.Debug ("CMapFragment.UpdateMarkers: points list is null");
               return;
            }

            if (_mapItemFilterType.HasValue) {
               points = points.Where (x => x.GetMapItemType == _mapItemFilterType);
            }

            IsLoading = false;

            lock (_lockObject) {
               _markers = new List<MarkerOptions> ();
               foreach (var point in points) {
                  try {
                     if (point.IsValid ()) {
                        var iconName = point.GetIconName;
                        BitmapDescriptor icon = BitmapDescriptorFactory.FromResource (Resources.GetIdentifier (iconName, "drawable", this.Activity.PackageName));
                        var marker = new MarkerOptions ()
                        .SetPosition (new LatLng (point.GetLatitude, point.GetLongitude))
                        .SetSnippet (JsonConvert.SerializeObject (point))
                        .SetTitle (point.GetContent)
                        .SetIcon (icon);
                        _markers.Add (marker);

                        AddMarkerOnMap (marker);
                     }
                  } catch (Exception ex) {
                     Logger.Instance.Error (string.Format ("CMapFragmer.UpdateMarkers exception: {0}", ex.Message));
                  }
               }
            }
         }

         IsLoading = false;
      }

      private object _lockObject = new object ();

      void UpdateNearestPointInformation ()
      {
         _distanceToNearestPoint = float.MaxValue;
         MarkerOptions nearestMarker = new MarkerOptions ();
         if (_currentLocation != null) {
            lock (_lockObject) {
               foreach (var marker in _markers) {
                  float[] results = { 0 };
                  Location.DistanceBetween (_currentLocation.Latitude, _currentLocation.Longitude, marker.Position.Latitude, marker.Position.Longitude, results);
                  if (_distanceToNearestPoint > results [0]) {
                     _distanceToNearestPoint = results [0];
                     nearestMarker = marker;
                  }
               }
            }
         }
         _distanceToNearestPoint = Math.Round (_distanceToNearestPoint, 2);
         _nameOfNearestPoint = nearestMarker.Title;
      }

      private IMenu _menu;
      private MenuInflater _menuInflater;

      public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
      {
         _menu = menu;
         _menuInflater = inflater;

         _menu.Add (0, 0, 0, Resource.String.ConquerMenuTitle);
         _menu.Add (0, 1, 1, Resource.String.QuestMenuTitle);
         _menu.Add (0, 2, 2, Resource.String.RefreshMenuTitle);
         _menu.Add (0, 3, 3, Resource.String.OnlyPointsMenuTitle);
         _menu.Add (0, 4, 4, Resource.String.OnlyQuestsMenuTitle);
         _menu.Add (0, 5, 5, Resource.String.AllObjectsMenuTitle);
         _menu.Add (0, 6, 6, Resource.String.LogoutMenuTitle);
      }

      public override bool OnOptionsItemSelected (IMenuItem item)
      {
         switch (item.ItemId) {
         case 0:
            base.AnalyticsService.TrackState ("Conquer", "Hit on Conquer button", string.Format ("User {0} is tryuing to conquer point {1}", DeviceUtility.DeviceId, _nameOfNearestPoint));
            ConquerHandler ();
            return true;
         case 1:
            QuestHandler ();
            return true;
         case 2:
            _mapItemFilterType = null;
            UpdateMarkersAsync ();
            return true;
         case 3:
            _mapItemFilterType = MapItemType.Point;
            _menu.Clear ();
            Activity.InvalidateOptionsMenu ();
            UpdateMarkersAsync ();
            return true;
         case 4:
            _mapItemFilterType = MapItemType.Quest;
            _menu.Clear ();
            Activity.InvalidateOptionsMenu ();
            UpdateMarkersAsync ();
            return true;
         case 5:
            _mapItemFilterType = null;
            _menu.Clear ();
            Activity.InvalidateOptionsMenu ();
            UpdateMarkersAsync ();
            return true;
         case 6:
            AnalyticsService.TrackState ("Conquer", "Hit on Logout button", string.Format ("User {0} is logout", DeviceUtility.DeviceId));
            Logout ();
            return true;
         default:
            return base.OnOptionsItemSelected (item);
         }
      }

      async void Logout ()
      {
         AppLocation.StopLocationService ();
         Process.KillProcess (Process.MyPid ());
      }

      private async void ConquerHandler ()
      {
         if (!CheckInternetConnection ()) {
            IsLoading = false;
            return;
         }

         string description = this.Resources.GetString (Resource.String.GPSNotDefined);
         if (_currentLocation != null) {
            Conquer result = await _apiService.Conquer (DeviceUtility.DeviceId, _currentLocation.Latitude.ProcessCoordinate (), _currentLocation.Longitude.ProcessCoordinate ());
            description = result.GetDescription;
            if (result.IsSuccess) {
               await UpdateMarkersAsync ();

               _userActionService.Add (new UserAction {
                  Type = (int)MapItemType.Point,
                  Title = result.title,
                  Number = result.number,
                  Description = result.GetDescription,
                  Date = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Utc)
               });
            }
         }

         IsLoading = false;
         this.ShowAlert (description);
      }

      private async void QuestHandler ()
      {
         if (!CheckInternetConnection ()) {
            IsLoading = false;
            return;
         }

         string description = this.Resources.GetString (Resource.String.GPSNotDefined);
         if (_currentLocation != null) {
            Conquer result = await _apiService.Quest (DeviceUtility.DeviceId, _currentLocation.Latitude.ProcessCoordinate (), _currentLocation.Longitude.ProcessCoordinate ());
            description = result.GetDescription;
            if (result.IsSuccess) {
               await UpdateMarkersAsync ();

               _userActionService.Add (new UserAction {
                  Type = (int)MapItemType.Quest,
                  Title = result.title,
                  Number = result.number,
                  Description = result.GetDescription,
                  Date = DateTime.SpecifyKind (DateTime.Now, DateTimeKind.Utc)
               });
            }
         }

         IsLoading = false;
         this.ShowAlert (description);
      }

      public override string FragmentTitle {
         get {
            return Resources.GetString (Resource.String.DrawerMap);
         }
      }

      private void ClearMap ()
      {
         Activity.RunOnUiThread (() => {
            try {
               map.Clear ();
            } catch (Exception ex) {
               Logger.Instance.Error (ex.Message);
            }
         });
      }

      private void AddMarkerOnMap (MarkerOptions marker)
      {
         Activity.RunOnUiThread (() => {
            try {
               map.AddMarker (marker);
            } catch (Exception ex) {
               Logger.Instance.Error (ex.Message);
            }
         });
      }
   }
}

