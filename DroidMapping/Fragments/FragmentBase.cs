
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
using Android.Net;
using GoHunting.Core.Services;
using Cirrious.CrossCore;

namespace DroidMapping
{
   public class FragmentBase : Fragment
   {
      ConnectivityManager _connectivityManager;
      IToastService _toastService;
      ProgressDialog _progressDialog;
      protected IAnalyticsService AnalyticsService;

      public override void OnCreate (Bundle savedInstanceState)
      {
         base.OnCreate (savedInstanceState);

         _toastService = Mvx.Resolve<IToastService> ();
         AnalyticsService = Mvx.Resolve<IAnalyticsService> ();

         _connectivityManager = (ConnectivityManager)Activity.GetSystemService (Activity.ConnectivityService);
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
               _progressDialog = ProgressDialog.Show (this.Activity, string.Empty, Resources.GetString (Resource.String.Wait), true, false);
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
         this.Activity.RunOnUiThread (() => {
            var alert = new AlertDialog.Builder (this.Activity);
            alert.SetMessage (message);
            alert.SetCancelable (true);
            alert.Show ();
         });
      }
   }
}

