using Android.App;
using Android.Content;
using Android.OS;
using GO.Common.Droid.Activities;
using GO.Core;
using GO.Paranoia.Droid.Services;

namespace GO.Paranoia.Droid
{
   [Activity(MainLauncher = true)]
   public class MainActivity : MainActivityBase
   {
      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

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

      protected override void GoToHomeScreen()
      {
         var intent = new Intent(this, typeof(DrawerActivityBase));
         intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
         StartActivity(intent);
      }
   }
}