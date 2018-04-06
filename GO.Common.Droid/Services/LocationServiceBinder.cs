using System;
using Android.OS;

namespace GO.Common.Droid.Services
{
   public class LocationServiceBinder : Binder
   {
      public LocationService Service => this.service;

      protected LocationService service;

      public bool IsBound { get; set; }

      public LocationServiceBinder(LocationService service) => this.service = service;
   }
}