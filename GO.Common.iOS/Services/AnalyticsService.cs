using System;
using GO.Core.Services;

namespace GO.Common.iOS.Services
{
   public class AnalyticsService : IAnalyticsService
   {
      public void TackException(Exception exception, bool isFatal, string additionalMessage = "")
      {
         // TODO
         // implement analytics tracking
      }

      public void TrackScreenView(string label)
      {
         // TODO
         // implement analytics tracking
      }

      public void TrackState(string category, string action, string message)
      {
         // TODO
         // implement analytics tracking
      }
   }
}

