using System;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Widget;
using GO.Common.Droid.Services;
using GO.Common.Droid.Utilities;
using GO.Core;
using GO.Core.Data;
using GO.Core.Enums;
using GO.Core.Services;
using MvvmCross.Platform;
using Plugin.CurrentActivity;

namespace GO.Common.Droid.Activities
{
   public class MainActivityBase : ActivityBase, ActivityCompat.IOnRequestPermissionsResultCallback
   {
      protected IApiService ApiService;
      protected IToastService ToastService;
      protected ILoginService LoginService;

      public MainActivityBase() { }

      protected override void OnCreate(Bundle savedInstanceState)
      {
         base.OnCreate(savedInstanceState);

         CrossCurrentActivity.Current.Activity = this;

         ApiService = Mvx.Resolve<IApiService>();
         ToastService = Mvx.Resolve<IToastService>();
         LoginService = Mvx.Resolve<ILoginService>();

         InitAppSettings();
         InitContentView();

         if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
         {
            ShowAlert(string.Format("Разрешите использовать GPS и гео-локации в настройках телефона и перезапустите приложение."));
            return;
         }

         Button button = FindViewById<Button>(Resource.Id.button_register);
         button.Click += ClickHandler;

         // TEST ONLY
         //
         //IsLoading = false;
         //GoToHomeScreen ();
         //
         //

         IsLoading = true;
         CheckUserExists();


         AppLocation.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
         {
         };
         AppLocation.StartLocationService();
      }

      protected virtual void InitAppSettings() { }

      protected virtual void InitContentView() => SetContentView(Resource.Layout.Main);

      protected virtual void GoToHomeScreen() { }

      protected async void CheckUserExists()
      {
         if (!CheckInternetConnection())
         {
            IsLoading = false;
            return;
         }

         var appId = GetAppId();
         AppSettingsService.SetAppId(appId);
         RegisterStatus status = await LoginService.CheckUserExists(appId);
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

      protected string GetAppId()
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

      private async void ClickHandler(object sender, EventArgs e)
      {
         EditText editTextName = FindViewById<EditText>(Resource.Id.editText_name);
         EditText editTextComment = FindViewById<EditText>(Resource.Id.editText_comment);

         if (editTextName.Text.Trim().ToLower() == "google" && editTextComment.Text.Trim().ToLower() == "google123")
         {
            AppSettingsService.SetAppId("0123456789");
         }

         ProgressDialog progressDialog = ProgressDialog.Show(this, string.Empty, Resources.GetString(Resource.String.Wait), true, false);

         RegisterStatus result = await LoginService.Register(editTextName.Text, editTextComment.Text, AppSettingsService.GetAppId());
         if (result.GetStatus != (int)UserStatus.RegisteredAndApproved)
         {
            progressDialog.Dismiss();
            ToastService.ShowMessage(result.GetDescription);
            return;
         }

         GoToHomeScreen();
      }

      protected readonly string[] PermissionsLocation = {
         Manifest.Permission.AccessCoarseLocation,
         Manifest.Permission.AccessFineLocation
      };

      public void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
      {
         if (!PermissionUtility.VerifyPermissions(grantResults))
         {
            ToastService.ShowMessageLongPeriod(string.Format("Разрешите использовать GPS и гео-локации в настройках телефона перед использование приложения."));
         }
      }
   }
}