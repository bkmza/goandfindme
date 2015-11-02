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
using Android.Views;

namespace DroidMapping
{
   [Activity (/*Icon = "@drawable/Icon", */ScreenOrientation = ScreenOrientation.Portrait)]
   public class BaseActivity : Activity
   {
      ConnectivityManager _connectivityManager;
      IToastService _toastService;
      ProgressDialog _progressDialog;
      protected IAnalyticsService AnalyticsService;

      protected override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);
         RequestWindowFeature(WindowFeatures.NoTitle);
//         _toastService = Mvx.Resolve<IToastService> ();
//         AnalyticsService = Mvx.Resolve<IAnalyticsService> ();
//
//         _connectivityManager = (ConnectivityManager)GetSystemService (ConnectivityService);
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

      protected bool IsLoading {
         get {
            return _isLoading;
         }
         set {
            _isLoading = value;
            if (_isLoading) {
               _progressDialog = ProgressDialog.Show (this, string.Empty, Resources.GetString (Resource.String.Wait), true, false);
            } else {
               if (_progressDialog != null && _progressDialog.IsShowing) {
                  _progressDialog.Dismiss ();
               }
            }
         }
      }

      private bool _isLoading;

      protected void ShowAlert (string message)
      {
         this.RunOnUiThread (() => {
            var alert = new AlertDialog.Builder (this);
            alert.SetMessage (message);
            alert.SetCancelable(true);
            alert.Show ();
         });
      }
   }
}

