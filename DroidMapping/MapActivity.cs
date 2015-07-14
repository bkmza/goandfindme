﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using DroidMapping.Adapters;
using DroidMapping.Services;
using GoHunting.Core;
using GoHunting.Core.Data;
using GoHunting.Core.Helpers;
using GoHunting.Core.Services;
using Newtonsoft.Json;
using Android.Content;

namespace DroidMapping
{
	[Activity (Label = "Searching GPS...", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MapActivity : BaseActivity, IOnMapReadyCallback, ILocationListener
	{
		IApiService _apiService;
		IToastService _toastService;

		static readonly LatLng Location_Minsk = new LatLng (53.900819, 27.558823);

		Location _currentLocation;
		GoogleMap map;
		MapFragment mapFragment;
		LocationManager _locationManager;
		string _locationProvider;
		List<MarkerOptions> _markers;

		public MapActivity ()
		{
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_apiService = Mvx.Resolve<IApiService> ();
			_toastService = Mvx.Resolve<IToastService> ();

			SetContentView (Resource.Layout.MapLayout);

			_markers = new List<MarkerOptions> ();

			mapFragment = FragmentManager.FindFragmentById (Resource.Id.map) as MapFragment;
			mapFragment.GetMapAsync (this);
			InitializeLocationManager ();
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_locationManager.RemoveUpdates (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			if (_locationManager.IsProviderEnabled (_locationProvider)) {
				_locationManager.RequestLocationUpdates (_locationProvider, 2000, 5, this);
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add (0, 0, 0, Resource.String.ConquerMenuTitle);
			menu.Add (0, 1, 1, Resource.String.QuestMenuTitle);
			menu.Add (0, 2, 2, Resource.String.RefreshMenuTitle);
			return true;
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) {
			case 0:
				ConquerHandler ();
				return true;
			case 1:
				QuestHandler ();
				return true;
			case 2:
				UpdateMarkers ();
				return true;
			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		void InitializeLocationManager ()
		{
			_locationManager = (LocationManager)GetSystemService (LocationService);
			Criteria criteriaForLocationService = new Criteria {
				Accuracy = Accuracy.Fine
			};
			_locationProvider = LocationManager.GpsProvider;
		}

		async void ConquerHandler ()
		{
			if (!CheckInternetConnection ()) {
				return;
			}

			string description = this.Resources.GetString (Resource.String.GPSNotDefined);
			if (_currentLocation != null) {
				Conquer result = await _apiService.Conquer (DeviceUtility.DeviceId, _currentLocation.Latitude.ProcessCoordinate (), _currentLocation.Longitude.ProcessCoordinate ());
				description = result.GetDescription;
				if (result.IsSuccess) {
					UpdateMarkers ();
				}
			}

			_toastService.ShowMessage (description);
		}

		async void QuestHandler ()
		{
			if (!CheckInternetConnection ()) {
				return;
			}

			string description = this.Resources.GetString (Resource.String.GPSNotDefined);
			if (_currentLocation != null) {
				Conquer result = await _apiService.Quest (DeviceUtility.DeviceId, _currentLocation.Latitude.ProcessCoordinate (), _currentLocation.Longitude.ProcessCoordinate ());
				description = result.GetDescription;
				if (result.IsSuccess) {
					UpdateMarkers ();
				}
			}

			_toastService.ShowMessage (description);
		}

		double GetMinDistanceToPoint ()
		{
			float minDistance = float.MaxValue;
			MarkerOptions nearestMarker = new MarkerOptions ();
			if (_currentLocation != null) {
				foreach (var marker in _markers) {
					float[] results = new float[] { 0 };
					Location.DistanceBetween (_currentLocation.Latitude, _currentLocation.Longitude, marker.Position.Latitude, marker.Position.Longitude, results);
					if (minDistance > results [0]) {
						minDistance = results [0];
						nearestMarker = marker;
					}
				}
			}
			return Math.Round (minDistance, 2);
		}

		public void OnLocationChanged (Location location)
		{
			_currentLocation = location;
			if (_currentLocation != null) {
				this.Window.SetTitle (string.Format ("До точки: {0} метров", GetMinDistanceToPoint ()));
			}
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}

		public void OnMapReady (GoogleMap googleMap)
		{
			map = googleMap;
			map.MapType = GoogleMap.MapTypeNormal;
			map.MyLocationEnabled = true;
			map.UiSettings.MyLocationButtonEnabled = true;
			map.UiSettings.ZoomControlsEnabled = true;
			map.SetInfoWindowAdapter (new CustomInfoWindowAdapter (LayoutInflater));

			CameraUpdate update = CameraUpdateFactory.NewLatLngZoom (Location_Minsk, 11);
			map.MoveCamera (update);

			UpdateMarkers ();
		}

		private async void UpdateMarkers ()
		{
			if (!CheckInternetConnection ()) {
				return;
			}

			if (map == null)
				return;

			map.Clear ();

			var points = await _apiService.GetAll (DeviceUtility.DeviceId);
			foreach (var point in points) {
				if (point.IsValid ()) {
					var marker = new MarkerOptions ()
						.SetPosition (new LatLng (point.GetLatitude, point.GetLongitude))
						.SetSnippet (point.GetId.ToString ())
						.SetTitle (point.GetContent)
						.InvokeIcon (BitmapDescriptorFactory.DefaultMarker (point.GetColorHue));
					_markers.Add (marker);
					map.AddMarker (marker);
				}
			}
		}
	}
}
