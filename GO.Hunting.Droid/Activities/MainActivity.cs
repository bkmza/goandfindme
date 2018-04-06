using Android.App;
using Android.Content;
using GO.Common.Droid.Activities;
using GO.Core;

namespace GO.Hunting.Droid
{
   [Activity(MainLauncher = true)]
   public class MainActivity : MainActivityBase
   {
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