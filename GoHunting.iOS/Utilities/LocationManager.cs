using System;

using CoreLocation;
using UIKit;

namespace GoHunting.iOS.Utilities
{
   public class LocationManager
   {
      protected CLLocationManager locMgr;

      public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

      public LocationManager()
      {
         this.locMgr = new CLLocationManager();
         this.locMgr.PausesLocationUpdatesAutomatically = false;

         if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
         {
            locMgr.RequestAlwaysAuthorization();
         }

         if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
         {
            locMgr.AllowsBackgroundLocationUpdates = true;
         }
      }

      public CLLocationManager LocMgr
      {
         get { return this.locMgr; }
      }

      public void StartLocationUpdates()
      {
         if (CLLocationManager.LocationServicesEnabled)
         {
            //set the desired accuracy, in meters
            LocMgr.DesiredAccuracy = 1;
            LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
            {
               // fire our custom Location Updated event
               LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
            };
            LocMgr.StartUpdatingLocation();
         }
      }
   }

   public class LocationUpdatedEventArgs : EventArgs
   {
      CLLocation location;

      public LocationUpdatedEventArgs(CLLocation location)
      {
         this.location = location;
      }

      public CLLocation Location
      {
         get { return location; }
      }
   }
}

