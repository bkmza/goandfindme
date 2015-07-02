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

namespace DroidMapping
{
	[Activity (Label = "Go Hunting!", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, IOnMapReadyCallback, ILocationListener
	{
		static readonly LatLng Location_Minsk = new LatLng (53.900819, 27.558823);

		GoogleMap map;
		MapFragment mapFragment;
		Location _currentLocation;
		LocationManager _locationManager;
		string _locationProvider;
		string _locationText;
		IApiService _apiService;
		bool _locationDetermined;

		void InitializeLocationManager()
		{
			_locationDetermined = false;

			_locationManager = (LocationManager)GetSystemService(LocationService);
			Criteria criteriaForLocationService = new Criteria
			{
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

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
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
				return base.OnOptionsItemSelected(item);
			}
		}

		async void ConquerHandler()
		{
			string description = "Unable to determine the address.";
			if (_currentLocation != null) {
				Conquer result = await _apiService.Conquer (DeviceUtility.DeviceId, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString());
				description = result.GetDescription;
			}

			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetTitle (description);
			RunOnUiThread (() => {
				alert.Show ();
			});
		}

		async void QuestHandler()
		{
			string description = "Unable to determine the address.";
			if (_currentLocation != null) {
				Conquer result = await _apiService.Quest (DeviceUtility.DeviceId, _currentLocation.Latitude.ToString(), _currentLocation.Longitude.ToString());
				description = result.GetDescription;
			}

			AlertDialog.Builder alert = new AlertDialog.Builder (this);
			alert.SetTitle (description);
			RunOnUiThread (() => {
				alert.Show ();
			});
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			MvxSimpleIoCContainer.Initialize ();
			Mvx.RegisterType<IApiService, ApiService> ();

			_apiService = Mvx.Resolve<IApiService> ();

			SetContentView (Resource.Layout.Main);

			mapFragment = FragmentManager.FindFragmentById (Resource.Id.map) as MapFragment;
			mapFragment.GetMapAsync (this);

			InitializeLocationManager();
		}

		private async void UpdateMarkers()
		{
			if (map == null)
				return;

			map.Clear ();

			var points = await _apiService.GetAll (DeviceUtility.DeviceId);
			foreach (var point in points) {
				if (point.IsValid ()) {
					var marker = new MarkerOptions ()
						.SetPosition (new LatLng (point.GetLatitude, point.GetLongitude))
						.SetSnippet(point.GetId.ToString())
						.SetTitle (point.GetContent)
						.InvokeIcon (BitmapDescriptorFactory.DefaultMarker (point.GetColorHue));
					
					map.AddMarker(marker);
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

		public void OnLocationChanged(Location location)
		{
			_currentLocation = location;

			if (_currentLocation == null)
			{
				_locationText = "Unable to determine your location.";
			}
			else
			{
				_locationText = string.Format("{0},{1}", _currentLocation.Latitude, _currentLocation.Longitude);
				if (!_locationDetermined) {
					AlertDialog.Builder alert = new AlertDialog.Builder (this);
					alert.SetTitle ("Current location is: " + _locationText);
					RunOnUiThread (() => {
						alert.Show ();
					});

					_locationDetermined = true;
				}
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
			base.OnResume();
			if(_locationManager.IsProviderEnabled(_locationProvider))
			{
				_locationManager.RequestLocationUpdates (_locationProvider, 2000, 1, this);
			}
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_locationManager.RemoveUpdates(this);
		}
	}
}


