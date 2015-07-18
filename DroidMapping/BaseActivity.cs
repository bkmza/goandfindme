using System;
using Android.App;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using DroidMapping.Services;
using GoHunting.Core.Services;
using GoHunting.Core.Utilities;
using DroidMapping.Utilities;

namespace DroidMapping
{
   [Activity (ScreenOrientation = ScreenOrientation.Portrait)]
   public class BaseActivity : Activity
   {
      ConnectivityManager _connectivityManager;
      IToastService _toastService;

      protected override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);

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

