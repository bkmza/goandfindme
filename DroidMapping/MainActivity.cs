using System;
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

namespace DroidMapping
{
	[Activity (Label = "Searching GPS...", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener
	{
		static readonly LatLng Location_Minsk = new LatLng (53.900819, 27.558823);
		public LatLng SelectedPoint;

		IApiService _apiService;
		IToastService _toastService;

		GoogleMap map;
		MapFragment mapFragment;
		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;
		ConnectivityManager _connectivityManager;
		List<MarkerOptions> _markers;

		private DrawerLayout _drawer;
		private MyActionBarDrawerToggle _drawerToggle;
		private ListView _drawerList;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.Main);

			MvxSimpleIoCContainer.Initialize ();
			Mvx.RegisterType<IApiService, ApiService> ();
			Mvx.RegisterType<IToastService, ToastService> ();

			_apiService = Mvx.Resolve<IApiService> ();
			_toastService = Mvx.Resolve<IToastService> ();

			InitDrawer ();

			if (null == savedInstanceState)
				SelectItem (0);

			// This event fires when the ServiceConnection lets the client (our App class) know that
			// the Service is connected. We use this event to start updating the UI with location
			// updates from the Service
			App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
//				Log.Debug (logTag, "ServiceConnected Event Raised");
//				// notifies us of location changes from the system
//				App.Current.LocationService.LocationChanged += HandleLocationChanged;
//				//notifies us of user changes to the location provider (ie the user disables or enables GPS)
//				App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
//				App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
//				// notifies us of the changing status of a provider (ie GPS no longer available)
//				App.Current.LocationService.StatusChanged += HandleStatusChanged;
			};
			App.StartLocationService();

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
			menu.Add (0, 0, 0, Resource.String.ConquerMenuTitle);
			menu.Add (0, 1, 1, Resource.String.QuestMenuTitle);
			menu.Add (0, 2, 2, Resource.String.RefreshMenuTitle);
			return true;
		}

		bool CheckInternetConnection ()
		{
			var activeConnection = _connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected) {
				return true;
			} else {
				_toastService.ShowMessage (this.Resources.GetString(Resource.String.NeedInternetConnect));
				return false;
			}
		}

		async void ConquerHandler ()
		{
			if (!CheckInternetConnection ()) {
				return;
			}

			string description = this.Resources.GetString(Resource.String.GPSNotDefined);
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

			string description = this.Resources.GetString(Resource.String.GPSNotDefined);
			if (_currentLocation != null) {
				Conquer result = await _apiService.Quest (DeviceUtility.DeviceId, _currentLocation.Latitude.ProcessCoordinate (), _currentLocation.Longitude.ProcessCoordinate ());
				description = result.GetDescription;
				if (result.IsSuccess) {
					UpdateMarkers ();
				}
			}

			_toastService.ShowMessage (description);
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


