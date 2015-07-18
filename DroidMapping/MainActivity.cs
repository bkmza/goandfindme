using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
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
using GoHunting.Core.Utilities;
using Newtonsoft.Json;

namespace DroidMapping
{
	[Activity (Label = "Searching GPS...", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : BaseActivity
	{
		IApiService _apiService;
		IToastService _toastService;
		ILoginService _loginService;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			_apiService = Mvx.Resolve<IApiService> ();
			_toastService = Mvx.Resolve<IToastService> ();
			_loginService = Mvx.Resolve<ILoginService> ();

			Button button = FindViewById<Button> (Resource.Id.button_register);
			EditText editTextName = FindViewById<EditText> (Resource.Id.editText_name);
			EditText editTextComment = FindViewById<EditText> (Resource.Id.editText_comment);

			button.Click += async delegate {
				// ToDo
				// Show loading indicator
				bool result = await _loginService.Login(editTextName.Text, editTextComment.Text, DeviceUtility.DeviceId);

				// Hide loading indicator

				var intent = new Intent (this, typeof(MapActivity));
				StartActivity (intent);
			};

			// This event fires when the ServiceConnection lets the client (our App class) know that
			// the Service is connected. We use this event to start updating the UI with location
			// updates from the Service
//			App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
////				Log.Debug (logTag, "ServiceConnected Event Raised");
////				// notifies us of location changes from the system
////				App.Current.LocationService.LocationChanged += HandleLocationChanged;
////				//notifies us of user changes to the location provider (ie the user disables or enables GPS)
////				App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
////				App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
////				// notifies us of the changing status of a provider (ie GPS no longer available)
////				App.Current.LocationService.StatusChanged += HandleStatusChanged;
//			};
//			App.StartLocationService ();
		}
	}
}


