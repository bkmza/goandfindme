using System;
using Android.App;
using Android.Content;
using Android.Widget;
using GoHunting.Core.Services;

namespace DroidMapping.Services
{
	public class ToastService : IToastService
	{
		public void ShowMessage (string message)
		{
			Context context = Application.Context;

			Toast toast = Toast.MakeText (context, message, ToastLength.Short);
			toast.Show ();
		}
	}
}

