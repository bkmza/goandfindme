using Foundation;
using GO.Common.iOS.Services;
using GO.Common.iOS.Utilities;
using GO.Common.iOS.ViewControllers;
using GO.Core;
using GO.Core.Services;
using GO.Core.Utilities;
using Google.Maps;
using MvvmCross.Platform;
using UIKit;

namespace GO.Hunting.iOS
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

         // GoHunting // packageName: com.go.goandfindme
         AppSettings.BaseHost = "http://gohunting.greyorder.su/";
         AppSettings.ApplicationName = @"GOhunting";
         AppSettings.PackageName = "com.go.goandfindme";

         Logger.Instance = new IOSLogger();

         Mvx.RegisterType<IToastService, ToastService>();
         Mvx.RegisterType<IAnalyticsService, AnalyticsService>();
         Mvx.RegisterType<IDBService, DBService>();
         Mvx.RegisterType<IUserActionService, UserActionService>();
         Mvx.RegisterType<IMapSettingsService, MapSettingsService>();

         _window = new UIWindow(UIScreen.MainScreen.Bounds)
         {
            RootViewController = new StartViewController()
         };
         _window.MakeKeyAndVisible();

         UINavigationBar.Appearance.BarTintColor = UIColor.White;
         UITabBar.Appearance.BarTintColor = UIColor.White;

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