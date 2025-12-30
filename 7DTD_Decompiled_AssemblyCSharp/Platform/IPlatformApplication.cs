using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Platform.Shared;
using UnityEngine;

namespace Platform
{
	// Token: 0x02001840 RID: 6208
	public interface IPlatformApplication
	{
		// Token: 0x170014DA RID: 5338
		// (get) Token: 0x0600B82D RID: 47149
		Resolution[] SupportedResolutions { get; }

		// Token: 0x170014DB RID: 5339
		// (get) Token: 0x0600B82E RID: 47150
		[TupleElementNames(new string[]
		{
			"width",
			"height",
			"fullScreenMode"
		})]
		ValueTuple<int, int, FullScreenMode> ScreenOptions { [return: TupleElementNames(new string[]
		{
			"width",
			"height",
			"fullScreenMode"
		})] get; }

		// Token: 0x0600B82F RID: 47151
		void SetResolution(int width, int height, FullScreenMode fullscreen);

		// Token: 0x170014DC RID: 5340
		// (get) Token: 0x0600B830 RID: 47152
		string temporaryCachePath { get; }

		// Token: 0x0600B831 RID: 47153
		void RestartProcess(params string[] argv);

		// Token: 0x0600B832 RID: 47154 RVA: 0x00468D1C File Offset: 0x00466F1C
		public static IPlatformApplication Create()
		{
			return new PlatformApplicationStandalone();
		}

		// Token: 0x0600B833 RID: 47155 RVA: 0x00468D23 File Offset: 0x00466F23
		public static string JoinAndEscapeArgv(params string[] args)
		{
			if (args != null)
			{
				return string.Join<string>(' ', args.Select(new Func<string, string>(IPlatformApplication.EscapeArg)));
			}
			return null;
		}

		// Token: 0x0600B834 RID: 47156 RVA: 0x00468D44 File Offset: 0x00466F44
		public static string EscapeArg(string arg)
		{
			if (arg.Length > 0 && arg.AsSpan().IndexOfAny(" \t\n\v\"") < 0)
			{
				return arg;
			}
			if (arg.Length <= 0)
			{
				return "\"\"";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('"');
			int num = 0;
			int num2;
			for (;;)
			{
				num2 = 0;
				while (num < arg.Length && arg[num] == '\\')
				{
					num++;
					num2++;
				}
				if (num >= arg.Length)
				{
					break;
				}
				if (arg[num] == '"')
				{
					stringBuilder.Append('\\', num2 * 2 + 1);
					stringBuilder.Append(arg[num]);
				}
				else
				{
					stringBuilder.Append('\\', num2);
					stringBuilder.Append(arg[num]);
				}
				num++;
			}
			stringBuilder.Append('\\', num2 * 2);
			stringBuilder.Append('"');
			return stringBuilder.ToString();
		}

		// Token: 0x0600B835 RID: 47157 RVA: 0x00468E24 File Offset: 0x00467024
		RefreshRate GetCurrentRefreshRate()
		{
			return Screen.currentResolution.refreshRateRatio;
		}

		// Token: 0x0600B836 RID: 47158 RVA: 0x00468E3E File Offset: 0x0046703E
		Resolution GetCurrentResolution()
		{
			return Screen.currentResolution;
		}

		// Token: 0x170014DD RID: 5341
		// (get) Token: 0x0600B837 RID: 47159 RVA: 0x00468E45 File Offset: 0x00467045
		EnumGamePrefs VSyncCountPref
		{
			get
			{
				return EnumGamePrefs.OptionsGfxVsync;
			}
		}
	}
}
