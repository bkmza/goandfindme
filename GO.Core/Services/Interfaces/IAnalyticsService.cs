using System;

namespace GO.Core.Services
{
   public interface IAnalyticsService
   {
      void TrackState(string category, string action, string message);

      void TackException(Exception exception, bool isFatal, string additionalMessage = "");

      void TrackScreenView(string label);
   }
}