using System;
using System.Collections.Generic;
using System.Linq;
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
using Newtonsoft.Json;
using GoHunting.Core.Utilities;
using System.Threading.Tasks;

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

      Location _currentLocation;
      GoogleMap map;
      MapFragment mapFragment;
      List<MarkerOptions> _markers;

      LayoutInflater _layoutInflater;

      double _distanceToNearestPoint;
      string _nameOfNearestPoint;

      MapItemType? _mapItemFilterType;

      public CMapFragment ()
      {
         _dbService = Mvx.Resolve<IDBService> ();
      }

      public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
      {
         _layoutInflater = inflater;

         if (_view == null) {
            _view = inflater.Inflate (Resource.Layout.fragment_cmap, container, false);
         }

         return _view;
      }

      public override void OnStart ()
      {
         base.OnStart ();

         mapFragment = Activity.FragmentManager.FindFragmentById (Resource.Id.map) as MapFragment;
         if (mapFragment != null) {
            mapFragment.GetMapAsync (this);
         }
      }

      public override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);

         _apiService = Mvx.Resolve<IApiService> ();
         _toastService = Mvx.Resolve<IToastService> ();
         _userActionService = Mvx.Resolve<IUserActionService> ();

         try {
            AppLocation.Current.LocationService.LocationChanged += HandleLocationChanged;
         } catch (Exception ex) {
//            ShowAlert(string.Format("LocationService: {0}", ex.Message));
         }

         _markers = new List<MarkerOptions> ();

         SetHasOptionsMenu (true);
      }

      public override void OnStop ()
      {
         try {
            AppLocation.Current.LocationService.LocationChanged -= HandleLocationChanged;
         } catch (Exception ex) {
//            ShowAlert(string.Format("LocationService: {0}", ex.Message));
         }

         base.OnStop ();
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
         map.MapType = GoogleMap.MapTypeNormal;
         map.MyLocationEnabled = true;
         map.UiSettings.MyLocationButtonEnabled = true;
         map.UiSettings.ZoomControlsEnabled = true;
         map.SetInfoWindowAdapter (new CustomInfoWindowAdapter (_layoutInflater));

         CameraUpdate update = CameraUpdateFactory.NewLatLngZoom (Location_Minsk, 11);
         map.MoveCamera (update);

         UpdateMarkers ();
      }

      async void UpdateMarkers ()
      {
         if (!CheckInternetConnection ()) {
            IsLoading = false;
            return;
         }

         if (map == null) {
            IsLoading = false;
            return;
         }

         map.Clear ();

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
                     map.AddMarker (marker);
                  }
               } catch (Exception ex) {
                  Logger.Instance.Error (string.Format ("CMapFragmer.UpdateMarkers exception: {0}", ex.Message));
               }
            }
         }

         IsLoading = false;
      }

      void UpdateNearestPointInformation ()
      {
         _distanceToNearestPoint = float.MaxValue;
         MarkerOptions nearestMarker = new MarkerOptions ();
         if (_currentLocation != null) {
            foreach (var marker in _markers) {
               float[] results = new float[] { 0 };
               Location.DistanceBetween (_currentLocation.Latitude, _currentLocation.Longitude, marker.Position.Latitude, marker.Position.Longitude, results);
               if (_distanceToNearestPoint > results [0]) {
                  _distanceToNearestPoint = results [0];
                  nearestMarker = marker;
               }
            }
         }
         _distanceToNearestPoint = Math.Round (_distanceToNearestPoint, 2);
         _nameOfNearestPoint = nearestMarker.Title;
      }

      public override void OnCreateOptionsMenu (IMenu menu, MenuInflater inflater)
      {
         menu.Add (0, 0, 0, Resource.String.ConquerMenuTitle);
         menu.Add (0, 1, 1, Resource.String.QuestMenuTitle);
         menu.Add (0, 2, 2, Resource.String.RefreshMenuTitle);
         menu.Add (0, 3, 3, Resource.String.ConquerFilterMenuTitle);
         menu.Add (0, 4, 4, Resource.String.QuestFilterMenuTitle);
         menu.Add (0, 5, 5, Resource.String.LogoutMenuTitle);
      }

      public override bool OnOptionsItemSelected (IMenuItem item)
      {
         IsLoading = true;
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
            UpdateMarkers ();
            return true;
         case 3:
            _mapItemFilterType = MapItemType.Point;
            UpdateMarkers ();
            return true;
         case 4:
            _mapItemFilterType = MapItemType.Quest;
            UpdateMarkers ();
            return true;
         case 5:
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

      async void ConquerHandler ()
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
               UpdateMarkers ();
            }

            _userActionService.Add (new UserAction {
               Type = (int)MapItemType.Point,
               Title = "Point title receiving from server", // TODO get Title from response when it will implemented on back-end side
               Description = description,
               Date = DateTime.Now
            });
         }

         IsLoading = false;
         this.ShowAlert (description);
      }

      async void QuestHandler ()
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
               UpdateMarkers ();
            }

            _userActionService.Add (new UserAction {
               Type = (int)MapItemType.Quest,
               Title = "Point title receiving from server", // TODO get Title from response when it will implemented on back-end side
               Description = string.Empty,
               Date = DateTime.Now
            });
         }

         IsLoading = false;
         this.ShowAlert (description);
      }

      public override string FragmentTitle {
         get {
            return Resources.GetString (Resource.String.DrawerMap);
         }
      }
   }
}

