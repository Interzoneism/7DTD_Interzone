using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x020017C6 RID: 6086
	public static class DeviceFlags
	{
		// Token: 0x0600B5E4 RID: 46564 RVA: 0x0002E2E1 File Offset: 0x0002C4E1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsCurrent(this DeviceFlag flags)
		{
			return (flags & DeviceFlag.StandaloneWindows) > DeviceFlag.None;
		}

		// Token: 0x04008F35 RID: 36661
		public const DeviceFlag Standalone = DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX;

		// Token: 0x04008F36 RID: 36662
		public const DeviceFlag XBoxSeries = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX;

		// Token: 0x04008F37 RID: 36663
		public const DeviceFlag PS5 = DeviceFlag.PS5;

		// Token: 0x04008F38 RID: 36664
		public const DeviceFlag Console = DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

		// Token: 0x04008F39 RID: 36665
		public const DeviceFlag All = DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;

		// Token: 0x04008F3A RID: 36666
		public const DeviceFlag None = DeviceFlag.None;

		// Token: 0x04008F3B RID: 36667
		public const DeviceFlag Current = DeviceFlag.StandaloneWindows;
	}
}
