using System.Collections.Generic;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Views;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using GoHunting.Core;
using GoHunting.Core.Data;
using GoHunting.Core.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using DroidMapping.Adapters;
using System;
using Android.Net;
using Android.Content.PM;
using GoHunting.Core.Helpers;
using Android.Support.V4.Widget;
using Android.Widget;

namespace DroidMapping
{
	[Activity (Label = "Searching GPS...", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener
	{
		static readonly LatLng Location_Minsk = new LatLng (53.900819, 27.558823);
		public LatLng SelectedPoint;

		GoogleMap map;
		MapFragment mapFragment;
		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;
		IApiService _apiService;
		ConnectivityManager _connectivityManager;
		List<MarkerOptions> _markers;

		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Main);
			InitDrawer ();

			if (null == savedInstanceState)
				SelectItem (0);

			MvxSimpleIoCContainer.Initialize ();
			Mvx.RegisterType<IApiService, ApiService> ();

			_apiService = Mvx.Resolve<IApiService> ();
			_markers = new List<MarkerOptions> ();

			mapFragment = FragmentManager.FindFragmentById (Resource.Id.map) as MapFragment;
			mapFragment.GetMapAsync (this);

			_connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
			InitializeLocationManager ();
		}

		private void InitDrawer()
		{
			_drawer = FindViewById<DrawerLayout> (Resource.Id.drawer_layout);
			_drawerList = FindViewById<ListView> (Resource.Id.left_drawer);

			_drawerList.Adapter = new ArrayAdapter<string> (this,
				Resource.Layout.DrawerListItem, Resources.GetStringArray (Resource.Array.DrawerItemsArray));
			_drawerList.ItemClick += (sender, args) => SelectItem (args.Position);

			ActionBar.SetDisplayHomeAsUpEnabled (true);
			ActionBar.SetHomeButtonEnabled (true);

			_drawerToggle = new MyActionBarDrawerToggle (this, _drawer,
				Resource.Drawable.ic_drawer_light,
				Resource.String.DrawerOpen,
				Resource.String.DrawerClose);

			_drawerToggle.DrawerClosed += delegate {
				InvalidateOptionsMenu ();
			};

			_drawerToggle.DrawerOpened += delegate {
				InvalidateOptionsMenu ();
			};

			_drawer.SetDrawerListener (_drawerToggle);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			if (_drawerToggle.OnOptionsItemSelected (item))
				return true;

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

		private void SelectItem (int position)
		{
//			var fragment = new PlanetFragment();
//			var arguments = new Bundle();
//			arguments.PutInt(PlanetFragment.ArgPlanetNumber, position);
//			fragment.Arguments = arguments;

//			FragmentManager.BeginTransaction()
//				.Replace(Resource.Id.content_frame, fragment)
//				.Commit();

			_drawerList.SetItemChecked (position, true);
			_drawer.CloseDrawer (_drawerList);
		}

		void InitializeLocationManager ()
		{
			_locationManager = (LocationManager)GetSystemService (LocationService);
			Criteria criteriaForLocationService = new Criteria {
				Accuracy = Accuracy.Fine
			};
			_locationProvider = LocationManager.GpsProvider;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			menu.Add (0, 0, 0, "Захват");
			menu.Add (0, 1, 1, "Квест");
			menu.Add (0, 2, 2, "Обновить");
			return true;
		}

		bool CheckInternetConnection ()
		{
			var activeConnection = _connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected) {
				return true;
			} else {
				AlertDialog.Builder alert = new AlertDialog.Builder (this);
				alert.SetTitle ("Необходимо подключение к интернету");
				RunOnUiThread (() => {
					alert.Show ();
				});
				return false;
			}
		}

		//string ProcessCoordinate(

		async void ConquerHandler ()
		{
			if (!CheckInternetConnection ()) {
				return;
			}

			string description = "GPS-координаты не определены, повторите попытку позже";
			if (_currentLocation != null) {
				Conquer result = await _apiService.Conquer (DeviceUtility.DeviceId, _currentLocation.Latitude.ProcessCoordinate (), _currentLocation.Longitude.ProcessCoordinate ());
				description = result.GetDescription;
				if (result.IsSuccess) {
					UpdateMarkers ();
				}
			} else {
				// Test
//				Conquer res = await _apiService.Conquer (DeviceUtility.DeviceId, "53.903972", "27.590428");
//				description = res.GetDescription;
//				if (res.IsSuccess) {
//					UpdateMarkers ();
//				}
			}

			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetTitle (description);
			RunOnUiThread (() => {
				alert.Show ();
			});
		}

		async void QuestHandler ()
		{
			if (!CheckInternetConnection ()) {
				return;
			}

			string description = "GPS-координаты не определены, повторите попытку позже";
			if (_currentLocation != null) {
				Conquer result = await _apiService.Quest (DeviceUtility.DeviceId, _currentLocation.Latitude.ProcessCoordinate (), _currentLocation.Longitude.ProcessCoordinate ());
				description = result.GetDescription;
				if (result.IsSuccess) {
					UpdateMarkers ();
				}
			} else {
				// Test
//				Conquer res = await _apiService.Quest (DeviceUtility.DeviceId, "53.903972", "27.590428");
//				description = res.GetDescription;
//				if (res.IsSuccess) {
//					UpdateMarkers ();
//				}
			}

			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetTitle (description);
			RunOnUiThread (() => {
				alert.Show ();
			});
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
			nearestMarker.InvokeIcon (BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueGreen));
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

		protected override void OnResume ()
		{
			base.OnResume ();
			if (_locationManager.IsProviderEnabled (_locationProvider)) {
				_locationManager.RequestLocationUpdates (_locationProvider, 2000, 5, this);
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_locationManager.RemoveUpdates (this);
		}
	}
}


