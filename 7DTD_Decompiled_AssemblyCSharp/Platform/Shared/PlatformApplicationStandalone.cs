using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Platform.Shared
{
	// Token: 0x020018DA RID: 6362
	public class PlatformApplicationStandalone : IPlatformApplication
	{
		// Token: 0x1700156F RID: 5487
		// (get) Token: 0x0600BBEC RID: 48108 RVA: 0x00476E00 File Offset: 0x00475000
		public Resolution[] SupportedResolutions
		{
			get
			{
				bool flag = false;
				if (this.lastResolutions != null)
				{
					Resolution[] resolutions = Screen.resolutions;
					if (this.lastResolutions.Length == resolutions.Length)
					{
						for (int i = 0; i < resolutions.Length; i++)
						{
							Resolution resolution = this.lastResolutions[i];
							Resolution resolution2 = resolutions[i];
							if (!resolution.Equals(resolution2))
							{
								flag = true;
								this.lastResolutions = resolutions;
								break;
							}
						}
					}
					else
					{
						this.lastResolutions = resolutions;
						flag = true;
					}
				}
				else
				{
					this.lastResolutions = Screen.resolutions;
					flag = true;
				}
				if (flag)
				{
					this.supportedResolutions = (from res in this.lastResolutions
					where res.width >= 640 && res.height >= 480
					select res).ToArray<Resolution>();
				}
				return this.supportedResolutions;
			}
		}

		// Token: 0x17001570 RID: 5488
		// (get) Token: 0x0600BBED RID: 48109 RVA: 0x00476EC8 File Offset: 0x004750C8
		[TupleElementNames(new string[]
		{
			"width",
			"height",
			"fullScreenMode"
		})]
		public ValueTuple<int, int, FullScreenMode> ScreenOptions
		{
			[return: TupleElementNames(new string[]
			{
				"width",
				"height",
				"fullScreenMode"
			})]
			get
			{
				FullScreenMode @int = (FullScreenMode)SdPlayerPrefs.GetInt("Screenmanager Fullscreen mode", 3);
				if (SdPlayerPrefs.HasKey("Screenmanager Resolution Width") && SdPlayerPrefs.HasKey("Screenmanager Resolution Height"))
				{
					int int2 = SdPlayerPrefs.GetInt("Screenmanager Resolution Width");
					int int3 = SdPlayerPrefs.GetInt("Screenmanager Resolution Height");
					return new ValueTuple<int, int, FullScreenMode>(int2, int3, @int);
				}
				Resolution[] array = this.SupportedResolutions;
				if (array.Length > 1)
				{
					Resolution resolution = array[array.Length - 2];
					return new ValueTuple<int, int, FullScreenMode>(resolution.width, resolution.height, @int);
				}
				return new ValueTuple<int, int, FullScreenMode>(Screen.width, Screen.height, FullScreenMode.Windowed);
			}
		}

		// Token: 0x0600BBEE RID: 48110 RVA: 0x00476F54 File Offset: 0x00475154
		public void SetResolution(int width, int height, FullScreenMode fullscreen)
		{
			if (width < 640 || height < 480 || width <= height)
			{
				fullscreen = FullScreenMode.Windowed;
				SdPlayerPrefs.SetInt("UnitySelectMonitor", 0);
			}
			if (height > width)
			{
				height = width;
			}
			SdPlayerPrefs.SetInt("Screenmanager Resolution Width", width);
			SdPlayerPrefs.SetInt("Screenmanager Resolution Height", height);
			SdPlayerPrefs.SetInt("Screenmanager Fullscreen mode", (int)fullscreen);
			Screen.SetResolution(width, height, fullscreen);
		}

		// Token: 0x17001571 RID: 5489
		// (get) Token: 0x0600BBEF RID: 48111 RVA: 0x00476FB3 File Offset: 0x004751B3
		public string temporaryCachePath
		{
			get
			{
				return Application.temporaryCachePath;
			}
		}

		// Token: 0x0600BBF0 RID: 48112 RVA: 0x000424BD File Offset: 0x000406BD
		public void RestartProcess(params string[] argv)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040092D2 RID: 37586
		[PublicizedFrom(EAccessModifier.Private)]
		public const string prefResolutionWidth = "Screenmanager Resolution Width";

		// Token: 0x040092D3 RID: 37587
		[PublicizedFrom(EAccessModifier.Private)]
		public const string prefResolutionHeight = "Screenmanager Resolution Height";

		// Token: 0x040092D4 RID: 37588
		[PublicizedFrom(EAccessModifier.Private)]
		public const string prefFullscreen = "Screenmanager Fullscreen mode";

		// Token: 0x040092D5 RID: 37589
		[PublicizedFrom(EAccessModifier.Private)]
		public const int minResWidth = 640;

		// Token: 0x040092D6 RID: 37590
		[PublicizedFrom(EAccessModifier.Private)]
		public const int minResHeight = 480;

		// Token: 0x040092D7 RID: 37591
		[PublicizedFrom(EAccessModifier.Private)]
		public Resolution[] lastResolutions;

		// Token: 0x040092D8 RID: 37592
		[PublicizedFrom(EAccessModifier.Private)]
		public Resolution[] supportedResolutions;
	}
}
