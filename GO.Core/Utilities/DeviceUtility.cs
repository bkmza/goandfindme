using Plugin.DeviceInfo;

namespace GO.Core
{
   public class DeviceUtility
   {
      public static string TestId { get; set; }

      public static string DeviceId => TestId ?? CrossDevice.Hardware.DeviceId;
   }
}