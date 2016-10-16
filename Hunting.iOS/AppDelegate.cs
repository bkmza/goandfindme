using Foundation;
using UIKit;

using Google.Maps;
using Cirrious.CrossCore;

using GoHunting.iOS.ViewControllers;
using GoHunting.Core.Services;
using GoHunting.Core;
using GoHunting.Core.Utilities;
using GO.Common.iOS.Utilities;
using GoHunting.iOS.Services;

namespace Hunting.iOS
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
         Logger.Instance = new IOSLogger();

         Mvx.RegisterType<IToastService, ToastService>();
         Mvx.RegisterType<IAnalyticsService, AnalyticsService>();

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


