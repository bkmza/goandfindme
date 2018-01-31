using GO.Core.Services;
using MvvmCross.Platform;

namespace GO.Core.Utilities
{
   public class StopWatch
   {
      static IStopWatchWrapper _stopWatch;

      static StopWatch()
      {
         _stopWatch = Mvx.Resolve<IStopWatchWrapper>();
      }

      public static void Start(string key)
      {
         _stopWatch.Start(key);
      }

      public static void Stop(string key)
      {
         _stopWatch.Stop(key);
      }
   }
}