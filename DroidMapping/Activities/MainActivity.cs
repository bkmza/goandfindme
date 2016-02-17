using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Cirrious.CrossCore;
using DroidMapping.Services;
using DroidMapping.Utilities;
using GoHunting.Core;
using GoHunting.Core.Data;
using GoHunting.Core.Enums;
using GoHunting.Core.Services;
using GoHunting.Core.Utilities;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;

namespace DroidMapping
{
   [Activity (MainLauncher = true)]
   public class MainActivity : ActivityBase
   {
      IApiService _apiService;
      IToastService _toastService;
      ILoginService _loginService;

      protected override void OnCreate (Bundle bundle)
      {
         AppSettings.TrackingId = "UA-65892866-1";
         AppSettings.RegisterTypes ();
         AppSettings.RegisterMapper ();

         Logger.Instance = new AndroidLogger ();
         Mvx.RegisterType<IToastService, ToastService> ();
         Mvx.RegisterType<IAnalyticsService, AnalyticsService> ();
         Mvx.RegisterType<ISQLitePlatform, SQLitePlatformAndroid> ();
         Mvx.RegisterType<ISQLite, SQLiteAndroid> ();
         Mvx.RegisterType<IDBService, DBService> ();
         Mvx.RegisterType<IUserActionService, UserActionService> ();

         base.OnCreate (bundle);

         SetContentView (Resource.Layout.Main);

         _apiService = Mvx.Resolve<IApiService> ();
         _toastService = Mvx.Resolve<IToastService> ();
         _loginService = Mvx.Resolve<ILoginService> ();

         IsLoading = true;
         CheckUserExists ();

         Button button = FindViewById<Button> (Resource.Id.button_register);

         button.Click += ClickHandler;

         AppLocation.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
         };
         AppLocation.StartLocationService ();
      }

      public async void CheckUserExists ()
      {
         // TODO
         //
         IsLoading = false;
         GoToHomeScreen ();
         //
         //

         if (!CheckInternetConnection ()) {
            IsLoading = false;
            return;
         }

         RegisterStatus status = await _loginService.CheckUserExists (DeviceUtility.DeviceId);
         if (status.GetStatus == (int)UserStatus.RegisteredAndApproved) {
            IsLoading = false;
            GoToHomeScreen ();
         } else {
            ShowAlert (status.GetDescription);
         }

         IsLoading = false;
      }

      public async void ClickHandler (object sender, EventArgs e)
      {
         EditText editTextName = FindViewById<EditText> (Resource.Id.editText_name);
         EditText editTextComment = FindViewById<EditText> (Resource.Id.editText_comment);

         ProgressDialog progressDialog = ProgressDialog.Show (this, string.Empty, Resources.GetString (Resource.String.Wait), true, false);

         RegisterStatus result = await _loginService.Register (editTextName.Text, editTextComment.Text, DeviceUtility.DeviceId);
         if (result.GetStatus != (int)UserStatus.RegisteredAndApproved) {
            progressDialog.Dismiss ();
            _toastService.ShowMessage (result.GetDescription);
            return;
         }

         GoToHomeScreen ();
      }

      public void GoToHomeScreen ()
      {
         var intent = new Intent (this, typeof(DrawerActivityBase));
         intent.SetFlags (ActivityFlags.NewTask | ActivityFlags.ClearTask);
         StartActivity (intent);
      }
   }
}


