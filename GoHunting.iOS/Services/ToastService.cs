using System;
using GoHunting.Core.Services;
using UIKit;

namespace GoHunting.iOS.Services
{
   public class ToastService : IToastService
   {
      public void ShowMessage(string message)
      {
         UIAlertView alert = new UIAlertView(null, message, null, "Ok", null);

         alert.Show();
      }

      public void ShowMessageLongPeriod(string message)
      {
         UIAlertView alert = new UIAlertView(null, message, null, "Ok", null);

         alert.Show();
      }
   }
}

