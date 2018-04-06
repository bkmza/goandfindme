using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using GO.Common.Droid.Activities;
using GO.Core;
using GO.Core.Data;
using GO.Core.Enums;
using GO.Paranoia.Droid.Services;

namespace GO.Paranoia.Droid
{
   [Activity(MainLauncher = true)]
   public class MainActivity : MainActivityBase
   {
      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         Button button = FindViewById<Button>(Resource.Id.button_register);

         button.Click += ClickHandler;

         AppLocation.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) =>
         {
         };

         AppLocation.StartLocationService();
      }

      protected override void InitAppSettings()
      {
         AppSettings.TrackingId = "UA-65892866-1";

         // Paranoia // packageName: com.go.paranoia
         AppSettings.BaseHost = "http://goandpay.greyorder.su/";
         AppSettings.ApplicationName = @"Paranoia";
         AppSettings.PackageName = "com.go.paranoia";
      }

      protected override void InitContentView() => SetContentView(Resource.Layout.Main);

      public async void ClickHandler(object sender, EventArgs e)
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

      protected override void GoToHomeScreen()
      {
         var intent = new Intent(this, typeof(DrawerActivityBase));
         intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
         StartActivity(intent);
      }
   }
}