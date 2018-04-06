using Android.App;
using Android.Content;
using Android.OS;
using GO.Common.Droid.Activities;
using GO.Core;
using GO.Hunting.Droid.Services;

namespace GO.Hunting.Droid
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
         base.InitAppSettings();

         AppSettings.TrackingId = "UA-65892866-1";

         // GoHunting // packageName: com.go.goandfindme
         AppSettings.BaseHost = "http://gohunting.greyorder.su/";
         AppSettings.ApplicationName = @"GOhunting";
         AppSettings.PackageName = "com.go.goandfindme";
      }

      protected override void GoToHomeScreen()
      {
         var intent = new Intent(this, typeof(DrawerActivityBase));
         intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
         StartActivity(intent);
      }
   }
}