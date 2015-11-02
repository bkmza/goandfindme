using System;
using GoHunting.Core.Services;
//using Android.Gms.Analytics;
using GoHunting.Core;

namespace DroidMapping
{
   public class AnalyticsService : IAnalyticsService
   {
      public AnalyticsService ()
      {
      }

      public void TrackState (string category, string action, string label)
      {
         GAService.LogEvent (category, action, DeviceUtility.DeviceId, label);
      }

      public void TackException (Exception exception, bool isFatal, string additionalMessage = "")
      {
      }

      public void TrackScreenView (string title)
      {
         GAService.LogScreen (title, DeviceUtility.DeviceId);
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

