using System;

namespace Platform
{
	// Token: 0x020017C7 RID: 6087
	public static class DeviceName
	{
		// Token: 0x0600B5E5 RID: 46565 RVA: 0x0046647C File Offset: 0x0046467C
		public static string GetDeviceName(this DeviceFlag _deviceId)
		{
			if (_deviceId <= DeviceFlag.XBoxSeriesS)
			{
				switch (_deviceId)
				{
				case DeviceFlag.StandaloneWindows:
					return "Windows";
				case DeviceFlag.StandaloneLinux:
					return "Linux";
				case DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux:
					break;
				case DeviceFlag.StandaloneOSX:
					return "OSX";
				default:
					if (_deviceId == DeviceFlag.XBoxSeriesS)
					{
						return "XBoxSeriesS";
					}
					break;
				}
			}
			else
			{
				if (_deviceId == DeviceFlag.XBoxSeriesX)
				{
					return "XBoxSeriesX";
				}
				if (_deviceId == DeviceFlag.PS5)
				{
					return "PS5";
				}
			}
			Log.Warning(string.Format("Device name for flag '{0}' is unknown", _deviceId));
			return string.Empty;
		}

		// Token: 0x04008F3C RID: 36668
		public const string StandaloneWindows = "Windows";

		// Token: 0x04008F3D RID: 36669
		public const string StandaloneLinux = "Linux";

		// Token: 0x04008F3E RID: 36670
		public const string StandaloneOSX = "OSX";

		// Token: 0x04008F3F RID: 36671
		public const string PS5 = "PS5";

		// Token: 0x04008F40 RID: 36672
		public const string XBoxSeriesS = "XBoxSeriesS";

		// Token: 0x04008F41 RID: 36673
		public const string XBoxSeriesX = "XBoxSeriesX";
	}
}
