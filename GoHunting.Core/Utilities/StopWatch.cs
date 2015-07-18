using System;
using Cirrious.CrossCore;
using GoHunting.Core.Services;

namespace GoHunting.Core.Utilities
{
   public class StopWatch
   {
      static IStopWatchWrapper _stopWatch;

      static StopWatch ()
      {
         _stopWatch = Mvx.Resolve<IStopWatchWrapper> ();
      }

      public static void Start (string key)
      {
         _stopWatch.Start (key);
      }

      public static void Stop (string key)
      {
         _stopWatch.Stop (key);
      }
   }
}

