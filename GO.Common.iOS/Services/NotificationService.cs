using System;
using System.Runtime.InteropServices;
using Foundation;
using GO.Core.Services.Interfaces;
using UIKit;

namespace GO.Common.iOS.Services
{
   public class NotificationService : INotificationService
   {
      public void FailedToRegisterForRemoteNotifications(string errorDescription)
         => Console.WriteLine("Error registering push notifications", errorDescription, null, "OK", null);

      public void RegisteredForRemoteNotificationsHandler(string deviceToken)
      {
         if (!string.IsNullOrWhiteSpace(deviceToken))
         {
            deviceToken = deviceToken.Trim('<').Trim('>');
         }

         // Get previous device token
         var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

         // Has the token changed?
         if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(deviceToken))
         {
            //TODO: Put your own logic here to notify your server that the device token has changed/been created!
         }

         // Save new device token
         NSUserDefaults.StandardUserDefaults.SetString(deviceToken, "PushDeviceToken");
      }

      public void RegisterForPushNotifications()
      {
         if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
         {
            var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                               UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                               new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
         }
         else
         {
            UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
            UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
         }
      }
   }
}
