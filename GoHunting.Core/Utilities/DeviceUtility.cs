using System;
using DeviceInfo.Plugin;

namespace GoHunting.Core
{
	public class DeviceUtility
	{
		public static string DeviceId
		{
			get { return CrossDeviceInfo.Current.Id; }
		}
	}
}

