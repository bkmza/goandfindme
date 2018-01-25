using Foundation;
using GO.Common.iOS.Services;
using GO.Common.iOS.Utilities;
using GO.Common.iOS.ViewControllers;
using GoHunting.Core;
using GoHunting.Core.Services;
using GoHunting.Core.Utilities;
using Google.Maps;
using MvvmCross.Platform;
using SQLite.Net.Interop;
using SQLite.Net.Platform.XamarinIOS;
using UIKit;

namespace GO.Paranoia.iOS
{
   [Register("AppDelegate")]
   public class AppDelegate : UIApplicationDelegate
   {
      public static AppDelegate Shared;

      private UIWindow _window;

      private const string MapsApiKey = "AIzaSyA30bwNoT0erSJKRZCvHexg0TO0K9acfcw";

      public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
      {
         MapServices.ProvideAPIKey(MapsApiKey);

         Shared = this;

         AppSettings.TrackingId = "UA-65892866-1";
         AppSettings.RegisterTypes();
         AppSettings.RegisterMapper();

         // Paranoia // packageName: com.go.paranoia
         AppSettings.BaseHost = "http://goandpay.greyorder.su/";
         AppSettings.ApplicationName = @"Paranoia";
         AppSettings.PackageName = "com.go.paranoia";

         Logger.Instance = new IOSLogger();

         Mvx.RegisterType<IToastService, ToastService>();
         Mvx.RegisterType<IAnalyticsService, AnalyticsService>();
         Mvx.RegisterType<ISQLitePlatform, SQLitePlatformIOS>();
         Mvx.RegisterType<ISQLite, SQLiteIOS>();
         Mvx.RegisterType<IDBService, DBService>();
         Mvx.RegisterType<IUserActionService, UserActionService>();
         Mvx.RegisterType<IMapSettingsService, MapSettingsService>();

         _window = new UIWindow(UIScreen.MainScreen.Bounds);

         _window.RootViewController = new StartViewController();

         _window.MakeKeyAndVisible();

         return true;
      }

      public override void OnResignActivation(UIApplication application)
      {

      }

      public override void DidEnterBackground(UIApplication application)
      {

      }

      public override void WillEnterForeground(UIApplication application)
      {

      }

      public override void OnActivated(UIApplication application)
      {
      }

      public override void WillTerminate(UIApplication application)
      {
      }

      public void SetRootViewController(UIViewController controller)
      {
         _window.RootViewController = controller;
      }
   }
}