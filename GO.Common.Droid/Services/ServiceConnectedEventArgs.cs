using System;
using Android.OS;

namespace GO.Common.Droid.Services
{
   public class ServiceConnectedEventArgs : EventArgs
   {
      public IBinder Binder { get; set; }
   }
}