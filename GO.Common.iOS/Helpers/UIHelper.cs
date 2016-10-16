using System;

using UIKit;

namespace GO.Common.iOS.Helpers
{
   public static class UIHelper
   {
      public static float ScreenWidth
      {
         get
         {
            return (float)UIScreen.MainScreen.Bounds.Width;
         }
      }

      public static float ScreenHeight
      {
         get
         {
            return (float)UIScreen.MainScreen.Bounds.Height;
         }
      }
   }
}

