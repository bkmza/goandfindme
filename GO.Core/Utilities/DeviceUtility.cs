namespace GO.Core
{
   public class DeviceUtility
   {
      public static string TestId { get; set; }

      public static string DeviceId => TestId ?? Plugin.DeviceInfo.CrossDeviceInfo.Current.Id;
   }
}