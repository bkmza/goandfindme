using System;
using GO.Core;
using GO.Core.Services;
//using Android.Gms.Analytics;

namespace GO.Hunting.Droid.Services
{
   public class AnalyticsService : IAnalyticsService
   {
      private IAppSettingsService _appSettingsService;

      public AnalyticsService(IAppSettingsService appSettingsService)
      {
         _appSettingsService = appSettingsService;
      }

      public void TrackState (string category, string action, string label)
      {
         GAService.LogEvent (category, action, _appSettingsService.GetAppId(), label);
      }

      public void TackException (Exception exception, bool isFatal, string additionalMessage = "")
      {
      }

      public void TrackScreenView (string title)
      {
         GAService.LogScreen (title, _appSettingsService.GetAppId());
      }
   }

   class GAService
   {
      public static void LogScreen (string screenName, string userName)
      {
         string trackingId = AppSettings.TrackingId;

//         var _tracker = GoogleAnalytics.GetInstance (Android.App.Application.Context).NewTracker (trackingId);
//         _tracker.SetScreenName (screenName);
//
//         _tracker.Send (new Android.Gms.Analytics.HitBuilders.ScreenViewBuilder ().Build ());
      }

      public static void LogEvent (string category, string action, string userName, string label)
      {
         string trackingId = AppSettings.TrackingId;

//         var _tracker = GoogleAnalytics.GetInstance (Android.App.Application.Context).NewTracker (trackingId);
//         _tracker.SetScreenName (null);

//         _tracker.Send (new Android.Gms.Analytics.HitBuilders.EventBuilder ().SetCategory (category).SetAction (action).SetLabel (label).Build ());
      }
   }
}

