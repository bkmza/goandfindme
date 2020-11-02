namespace GO.Core.Services.Interfaces
{
    public interface INotificationService
   {
      void RegisterForPushNotifications();
      void RegisteredForRemoteNotificationsHandler(string deviceToken);
      void FailedToRegisterForRemoteNotifications(string errorDescription);
   }
}
