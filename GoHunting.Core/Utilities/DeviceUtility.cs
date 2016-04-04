using System;
using Acr.DeviceInfo;

namespace GoHunting.Core
{
   public class DeviceUtility
   {
      public static string DeviceId {
         get { return /*"HJA3XE3T";  }*/ DeviceInfo.Hardware.DeviceId; }
      }
   }
}


