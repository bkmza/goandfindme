using System;
using Android.App;
using Android.Content;
using Android.Widget;
using GO.Core.Services;

namespace GO.Hunting.Droid.Services
{
   public class ToastService : IToastService
	{
      public void ShowMessage (string message)
      {
         Context context = Application.Context;

         Toast toast = Toast.MakeText (context, message, ToastLength.Short);
         toast.Show ();
      }

      public void ShowMessageLongPeriod(string message)
      {
         Context context = Application.Context;

         Toast toast = Toast.MakeText (context, message, ToastLength.Long);
         toast.Show ();
      }
	}
}

