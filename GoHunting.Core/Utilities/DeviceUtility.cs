using System;
using Acr.DeviceInfo;

namespace GoHunting.Core
{
   public class DeviceUtility
   {
      public static string TestId { get; set; }

      public static string DeviceId {
         get { return TestId ?? DeviceInfo.Hardware.DeviceId; }
      }
   }
}


