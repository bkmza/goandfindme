using System;
using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using DroidMapping.Services;
using GoHunting.Core.Services;

namespace DroidMapping
{
	[Activity (Label = "Searching GPS...", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class BaseActivity : Activity
	{
		ConnectivityManager _connectivityManager;
		IToastService _toastService;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			MvxSimpleIoCContainer.Initialize ();

			Mvx.RegisterType<IApiService, ApiService> ();
			Mvx.RegisterType<IToastService, ToastService> ();
			Mvx.RegisterType<ILoginService, LoginService> ();

			_toastService = Mvx.Resolve<IToastService> ();
			_connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
		}

		public bool CheckInternetConnection ()
		{
			var activeConnection = _connectivityManager.ActiveNetworkInfo;
			if ((activeConnection != null) && activeConnection.IsConnected) {
				return true;
			} else {
				_toastService.ShowMessage (this.Resources.GetString (Resource.String.NeedInternetConnect));
				return false;
			}
		}
	}
}

