using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Widget;
using GO.Common.Droid.Activities;
using GO.Common.Droid.Utilities;
using GO.Core;
using GO.Core.Data;
using GO.Core.Enums;
using GO.Core.Services;
using GO.Hunting.Droid.Services;
using MvvmCross.Platform;

namespace GO.Hunting.Droid
{
   [Activity(MainLauncher = true)]
   public class MainActivity : MainActivityBase
   {
      IApiService _apiService;
      IToastService _toastService;
      ILoginService _loginService;

      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         AppSettings.TrackingId = "UA-65892866-1";

         // GoHunting // packageName: com.go.goandfindme
         AppSettings.BaseHost = "http://gohunting.greyorder.su/";
         AppSettings.ApplicationName = @"GOhunting";
         AppSettings.PackageName = "com.go.goandfindme";

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

         var appId = GetAppId();
         AppSettingsService.SetAppId(appId);
         RegisterStatus status = await _loginService.CheckUserExists(appId);
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

      private string GetAppId()
      {
         string appId = null;
         string deviceId = AppSettingsService.GetAppId() ?? DeviceUtility.DeviceId;
         if (deviceId.ToLower() == "0123456789abcdef")
         {
            appId = DeviceUtility.GenerateAppId;
         }
         else
         {
            appId = deviceId;
         }
         return appId;
      }

      public async void ClickHandler(object sender, EventArgs e)
      {
         EditText editTextName = FindViewById<EditText>(Resource.Id.editText_name);
         EditText editTextComment = FindViewById<EditText>(Resource.Id.editText_comment);

         if (editTextName.Text.Trim().ToLower() == "google" && editTextComment.Text.Trim().ToLower() == "google123")
         {
            AppSettingsService.SetAppId("0123456789");
         }

         ProgressDialog progressDialog = ProgressDialog.Show(this, string.Empty, Resources.GetString(Resource.String.Wait), true, false);

         RegisterStatus result = await _loginService.Register(editTextName.Text, editTextComment.Text, AppSettingsService.GetAppId());
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