
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GoHunting.Core.Services;
using Android.Locations;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Cirrious.CrossCore;
using DroidMapping.Adapters;
using GoHunting.Core.Data;
using GoHunting.Core;
using Newtonsoft.Json;
using GoHunting.Core.Helpers;
using GoHunting.Core.Entities;
using GoHunting.Core.Enums;

namespace DroidMapping
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

         mapFragment = FragmentManager.FindFragmentById (Resource.Id.map) as MapFragment;
         mapFragment.GetMapAsync (this);

         return _view;
      }

      public override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);

         _apiService = Mvx.Resolve<IApiService> ();
         _toastService = Mvx.Resolve<IToastService> ();
         _userActionService = Mvx.Resolve<IUserActionService> ();

         AppLocation.Current.LocationService.LocationChanged += HandleLocationChanged;

         _markers = new List<MarkerOptions> ();

         SetHasOptionsMenu (true);
      }

      public override void OnStop ()
      {
         AppLocation.Current.LocationService.LocationChanged -= HandleLocationChanged;

         base.OnStop ();
      }

      public void HandleLocationChanged (object sender, LocationChangedEventArgs e)
      {
         _currentLocation = e.Location;
         if (_currentLocation != null) {
            UpdateNearestPointInformation ();
            this.Activity.Title = string.Format ("{0} м: {1}", _distanceToNearestPoint.ToString("0.00"), _nameOfNearestPoint);
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

      private async void UpdateMarkers ()
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
            foreach (var point in points) {
               if (point.IsValid ()) {
                  BitmapDescriptor icon = BitmapDescriptorFactory.FromResource (Resources.GetIdentifier (point.GetIconName, "drawable", this.Activity.PackageName));
                  var marker = new MarkerOptions ()
                     .SetPosition (new LatLng (point.GetLatitude, point.GetLongitude))
                     .SetSnippet (JsonConvert.SerializeObject (point))
                     .SetTitle (point.GetContent)
                     .SetIcon (icon);
                  _markers.Add (marker);
                  map.AddMarker (marker);
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
         menu.Add (0, 3, 3, Resource.String.LogoutMenuTitle);
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
            UpdateMarkers ();
            return true;
         case 3:
            base.AnalyticsService.TrackState ("Conquer", "Hit on Logout button", string.Format ("User {0} is logout", DeviceUtility.DeviceId));
            Logout ();
            return true;
         default:
            return base.OnOptionsItemSelected (item);
         }
      }

      async void Logout ()
      {
         AppLocation.StopLocationService ();
         Android.OS.Process.KillProcess (Android.OS.Process.MyPid ());
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
               _userActionService.Add (new UserAction {
                  Type = (int)MapItemType.Point,
                  Title = "", // TODO get Title from response when it will implemented on back-end side
                  Description = result.GetDescription,
                  Date = DateTime.Now
               });
            }
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
               _userActionService.Add (new UserAction {
                  Type = (int)MapItemType.Quest,
                  Title = "", // TODO get Title from response when it will implemented on back-end side
                  Description = result.GetDescription,
                  Date = DateTime.Now
               });
            }
         }

         IsLoading = false;
         this.ShowAlert (description);
      }

      public override string Titile {
         get {
            return "Map";
         }
      }
   }
}

