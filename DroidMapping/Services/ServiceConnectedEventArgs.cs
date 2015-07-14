using System;
using Android.OS;

namespace DroidMapping
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }
	}
}

