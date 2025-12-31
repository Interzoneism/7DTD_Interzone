using System;

namespace mumblelib
{
	// Token: 0x020013D6 RID: 5078
	public static class LinkFileManager
	{
		// Token: 0x06009EA8 RID: 40616 RVA: 0x003F0881 File Offset: 0x003EEA81
		public static ILinkFile Open()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				Log.Out("[MumbleLF] Loading Windows Mumble Link");
				return new WindowsLinkFile();
			}
			Log.Out("[MumbleLF] Loading Unix Mumble Link");
			return new UnixLinkFile();
		}
	}
}
