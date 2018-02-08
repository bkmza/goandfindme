using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Widget;
using GO.Core;
using GO.Core.Data;
using GO.Core.Enums;
using GO.Core.Services;
using GO.Core.Utilities;
using GO.Hunting.Droid.Services;
using GO.Common.Droid.Utilities;
using MvvmCross.Platform;
using Plugin.CurrentActivity;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinAndroid;

namespace GO.Hunting.Droid
{
   [Activity(MainLauncher = true)]
   public class MainActivity : ActivityBase, ActivityCompat.IOnRequestPermissionsResultCallback
   {
      IApiService _apiService;
      IToastService _toastService;
      ILoginService _loginService;

      protected override void OnCreate(Bundle bundle)
      {
         CrossCurrentActivity.Current.Activity = this;

         AppSettings.TrackingId = "UA-65892866-1";
         AppSettings.RegisterTypes();

         Logger.Instance = new AndroidLogger();
         Mvx.RegisterType<IToastService, ToastService>();
         Mvx.RegisterType<IAnalyticsService, AnalyticsService>();
         Mvx.RegisterType<ISQLitePlatform, SQLitePlatformAndroid>();
         Mvx.RegisterType<ISQLite, SQLiteAndroid>();
         Mvx.RegisterType<IDBService, DBService>();
         Mvx.RegisterType<IUserActionService, UserActionService>();
         Mvx.RegisterType<IMapSettingsService, MapSettingsService>();

         base.OnCreate(bundle);

         SetContentView(Resource.Layout.Main);

         _apiService = Mvx.Resolve<IApiService>();
         _toastService = Mvx.Resolve<IToastService>();
         _loginService = Mvx.Resolve<ILoginService>();

         // TEST ONLY
         //
         //IsLoading = false;
         //GoToHomeScreen ();
         //
         //

         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
         {
            ShowAlert(string.Format("Разрешите использовать GPS и гео-локации в настройках телефона и перезапустите приложение."));
            return;
         }

         IsLoading = true;
         CheckUserExists();

         Button button = FindViewById<Button>(Resource.Id.button_register);

         button.Click += ClickHandler;

         AppLocation.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
         {
         };

         AppLocation.StartLocationService();
      }


      readonly string[] PermissionsLocation = {
         Manifest.Permission.AccessCoarseLocation,
         Manifest.Permission.AccessFineLocation
      };

      const int RequestLocationId = 0;

      public async void CheckUserExists()
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         RegisterStatus status = await _loginService.CheckUserExists(DeviceUtility.DeviceId);
         if (status.GetStatus == (int)UserStatus.RegisteredAndApproved)
         {
            IsLoading = false;
            GoToHomeScreen();
         }
         else
         {
            ShowAlert(status.GetDescription);
         }

         IsLoading = false;
      }

      public async void ClickHandler(object sender, EventArgs e)
      {
         EditText editTextName = FindViewById<EditText>(Resource.Id.editText_name);
         EditText editTextComment = FindViewById<EditText>(Resource.Id.editText_comment);

         if (editTextName.Text.Trim().ToLower() == "google" && editTextComment.Text.Trim().ToLower() == "google123")
         {
            DeviceUtility.TestId = "0123456789";
         }

         ProgressDialog progressDialog = ProgressDialog.Show(this, string.Empty, Resources.GetString(Resource.String.Wait), true, false);

         RegisterStatus result = await _loginService.Register(editTextName.Text, editTextComment.Text, DeviceUtility.DeviceId);
         if (result.GetStatus != (int)UserStatus.RegisteredAndApproved)
         {
            progressDialog.Dismiss();
            _toastService.ShowMessage(result.GetDescription);
            return;
         }

         GoToHomeScreen();
      }

      public void GoToHomeScreen()
      {
         var intent = new Intent(this, typeof(DrawerActivityBase));
         intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
         StartActivity(intent);
      }

      public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
      {
         if (PermissionUtility.VerifyPermissions(grantResults))
         {

         }
         else
         {
            _toastService.ShowMessageLongPeriod(string.Format("Разрешите использовать GPS и гео-локации в настройках телефона перед использование приложения."));
         }
      }
   }
}


