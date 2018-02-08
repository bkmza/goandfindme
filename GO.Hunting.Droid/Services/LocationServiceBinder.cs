using System;
using Android.OS;

namespace GO.Hunting.Droid.Services
{
	public class LocationServiceBinder : Binder
	{
		public LocationService Service
		{
			get { return this.service; }
		} protected LocationService service;

		public bool IsBound { get; set; }

		public LocationServiceBinder (LocationService service)
		{
			this.service = service;
		}
	}
}

