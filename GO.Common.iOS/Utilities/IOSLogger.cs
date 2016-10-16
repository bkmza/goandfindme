using System;
using GoHunting.Core.Utilities;

namespace GO.Common.iOS.Utilities
{
   public class IOSLogger : ILogger
   {
      public void Debug(string message)
      {
         #if DEBUG
         Console.WriteLine(string.Format("LTF.Debug: {0}", message));
         #endif
      }
      public void Warn(string message)
      {
         Console.WriteLine(string.Format("LTF.Warn: {0}", message));
      }
      public void Error(string message)
      {
         Console.WriteLine(string.Format("LTF.Error: {0}", message));
      }
   }
}

