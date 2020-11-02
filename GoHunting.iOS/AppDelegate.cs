using System;
using System.Runtime.InteropServices;
using Foundation;
using GO.Common.iOS.Services;
using GO.Common.iOS.Utilities;
using GO.Common.iOS.ViewControllers;
using GO.Core;
using GO.Core.Services;
using GO.Core.Services.Interfaces;
using GO.Core.Utilities;
using Google.Maps;
using MvvmCross.Platform;
using UIKit;

namespace GO.Paranoia.iOS
{
    [Register("AppDelegate")]
   public class AppDelegate : UIApplicationDelegate
   {
      public static AppDelegate Shared;

      private UIWindow _window;

      private INotificationService _notificationService;

      private const string MapsApiKey = "AIzaSyA30bwNoT0erSJKRZCvHexg0TO0K9acfcw";

      public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
      {
         MapServices.ProvideAPIKey(MapsApiKey);

         Shared = this;

         AppSettings.TrackingId = "UA-65892866-1";
         AppSettings.RegisterTypes();

         // Paranoia // packageName: com.go.paranoia
         AppSettings.BaseHost = "http://goandpay.greyorder.su/";
         AppSettings.ApplicationName = @"Paranoia";
         AppSettings.PackageName = "com.go.paranoia";

         Logger.Instance = new IOSLogger();

         Mvx.RegisterType<IToastService, ToastService>();
         Mvx.RegisterType<IAnalyticsService, AnalyticsService>();
         Mvx.RegisterSingleton<IDBService>(new DBService());
         Mvx.RegisterType<IUserActionService, UserActionService>();
         Mvx.RegisterType<IMapSettingsService, MapSettingsService>();
         _notificationService = new NotificationService();
         Mvx.RegisterSingleton(_notificationService);

         _window = new UIWindow(UIScreen.MainScreen.Bounds)
         {
            RootViewController = new StartViewController()
         };
         _window.MakeKeyAndVisible();

         RegisterForPushNotifications();

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

      public void SetRootViewController(UIViewController controller) => _window.RootViewController = controller;

      private void RegisterForPushNotifications() => _notificationService.RegisterForPushNotifications();

      public override void RegisteredForRemoteNotifications(UIApplication _, NSData deviceToken)
      {
         var currentDeviceToken = deviceToken.Description;

         byte[] result = new byte[deviceToken.Length];
         Marshal.Copy(deviceToken.Bytes, result, 0, (int)deviceToken.Length);
         var tokenValue = BitConverter.ToString(result).Replace("-", "");
         Console.WriteLine(tokenValue);

         _notificationService.RegisteredForRemoteNotificationsHandler(currentDeviceToken);
      }

      public override void FailedToRegisterForRemoteNotifications(UIApplication _, NSError error)
         => _notificationService.FailedToRegisterForRemoteNotifications(error.LocalizedDescription);
   }
}