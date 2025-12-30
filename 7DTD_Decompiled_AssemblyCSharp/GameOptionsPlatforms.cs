using System;
using Platform;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000F80 RID: 3968
public static class GameOptionsPlatforms
{
	// Token: 0x06007EAB RID: 32427 RVA: 0x003357E5 File Offset: 0x003339E5
	public static float GetStreamingMipmapBudget()
	{
		return (float)SystemInfo.graphicsMemorySize * 0.9f;
	}

	// Token: 0x06007EAC RID: 32428 RVA: 0x003357F4 File Offset: 0x003339F4
	public static string GetItemIconFilterString()
	{
		string result = "mip0";
		if ((float)SystemInfo.graphicsMemorySize <= 3200f || SystemInfo.systemMemorySize < 6800)
		{
			result = "mip1";
		}
		return result;
	}

	// Token: 0x06007EAD RID: 32429 RVA: 0x00335828 File Offset: 0x00333A28
	public static int CalcTextureQualityMin()
	{
		int systemMemorySize = SystemInfo.systemMemorySize;
		if (SystemInfo.graphicsMemorySize < 2400 || systemMemorySize < 4900)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06007EAE RID: 32430 RVA: 0x00335854 File Offset: 0x00333A54
	public static int CalcDefaultGfxPreset()
	{
		if ((DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX).IsCurrent())
		{
			int result = 2;
			float num = (float)SystemInfo.systemMemorySize;
			float num2 = (float)SystemInfo.graphicsMemorySize;
			if (!SystemInfo.operatingSystem.Contains(" Steam ") && (num2 < 2400f || num < 4800f))
			{
				result = 1;
			}
			if (num2 > 7500f && num > 5200f)
			{
				string text = SystemInfo.graphicsDeviceVendor.ToLower();
				if (text.Contains("nvidia"))
				{
					if (!GameOptionsPlatforms.FindGfxName(" 1070, 305"))
					{
						result = 3;
						if (GameOptionsPlatforms.FindGfxName(" 208, 307, 308, 309, 407, 408, 409, 507, 508, 509"))
						{
							result = 4;
						}
					}
				}
				else if ((text == "amd" || text == "ati") && !GameOptionsPlatforms.FindGfxName("RX 570,RX 580,RX 590,RX 5500,RX 65,RX 66"))
				{
					result = 3;
					if (GameOptionsPlatforms.FindGfxName(" 680, 690, 695, 770, 780, 790, 907, 908, 909"))
					{
						result = 4;
					}
				}
			}
			return result;
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			return 6;
		}
		return 2;
	}

	// Token: 0x17000D3B RID: 3387
	// (get) Token: 0x06007EAF RID: 32431 RVA: 0x00335929 File Offset: 0x00333B29
	public static int DefaultUpscalerMode
	{
		get
		{
			if (!FSR3.FSR3Supported())
			{
				return 4;
			}
			return 2;
		}
	}

	// Token: 0x06007EB0 RID: 32432 RVA: 0x00335938 File Offset: 0x00333B38
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool FindGfxName(string names)
	{
		string text = SystemInfo.graphicsDeviceName.ToLower();
		if (text.Contains("laptop"))
		{
			return false;
		}
		string[] array = names.Split(',', StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (text.Contains(array[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06007EB1 RID: 32433 RVA: 0x00335985 File Offset: 0x00333B85
	public static int ApplyTextureFilterLimit(int filter)
	{
		if ((float)SystemInfo.graphicsMemorySize < 3200f)
		{
			filter = 0;
		}
		return filter;
	}

	// Token: 0x02000F81 RID: 3969
	public static class GfxPreset
	{
		// Token: 0x04006083 RID: 24707
		public const int Lowest = 0;

		// Token: 0x04006084 RID: 24708
		public const int Low = 1;

		// Token: 0x04006085 RID: 24709
		public const int Medium = 2;

		// Token: 0x04006086 RID: 24710
		public const int High = 3;

		// Token: 0x04006087 RID: 24711
		public const int Ultra = 4;

		// Token: 0x04006088 RID: 24712
		public const int Custom = 5;

		// Token: 0x04006089 RID: 24713
		public const int ConsolePerformance = 6;

		// Token: 0x0400608A RID: 24714
		public const int LEGACY_ConsolePerformanceFSR = 7;

		// Token: 0x0400608B RID: 24715
		public const int ConsoleQuality = 8;

		// Token: 0x0400608C RID: 24716
		public const int LEGACY_ConsoleQualityFSR = 9;

		// Token: 0x0400608D RID: 24717
		public const int Simplified = 100;
	}

	// Token: 0x02000F82 RID: 3970
	public static class UpscalerMode
	{
		// Token: 0x06007EB2 RID: 32434 RVA: 0x00335998 File Offset: 0x00333B98
		public static string ToString(int upscalerSettingValue)
		{
			switch (upscalerSettingValue)
			{
			case 0:
				return "Off";
			case 1:
				return "FSR2";
			case 2:
				return "FSR3";
			case 3:
				return "Dynamic Resolution";
			case 4:
				return "Scale";
			case 5:
				return "DLSS";
			default:
				return "Unknown";
			}
		}

		// Token: 0x0400608E RID: 24718
		public const int Off = 0;

		// Token: 0x0400608F RID: 24719
		public const int FSR2 = 1;

		// Token: 0x04006090 RID: 24720
		public const int FSR3 = 2;

		// Token: 0x04006091 RID: 24721
		public const int DynamicResolution = 3;

		// Token: 0x04006092 RID: 24722
		public const int Scale = 4;

		// Token: 0x04006093 RID: 24723
		public const int DLSS = 5;
	}
}
