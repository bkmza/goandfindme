using Plugin.DeviceInfo;

namespace GO.Core
{
   public class DeviceUtility
   {
      public static string TestId { get; set; }

      public static string DeviceId => TestId ?? CrossDeviceInfo.Current.Id;

      public static string GenerateAppId => CrossDeviceInfo.Current.GenerateAppId();
   }
}