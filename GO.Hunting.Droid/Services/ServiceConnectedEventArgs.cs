using System;
using Android.OS;

namespace GO.Hunting.Droid.Services
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }
	}
}

