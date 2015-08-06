using System;
using Acr.DeviceInfo;

namespace GoHunting.Core
{
	public class DeviceUtility
	{
		public static string DeviceId
		{
         get { return DeviceInfo.Hardware.DeviceId; }
		}
	}
}

