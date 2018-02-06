using System;
using Android.OS;

namespace GO.Paranoia.Droid.Services
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }
	}
}

