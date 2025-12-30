using System;
using System.IO;
using System.Text;
using System.Threading;
using InControl;
using UnityEngine;

namespace Platform.Shared
{
	// Token: 0x020018DD RID: 6365
	public class Utils : IUtils
	{
		// Token: 0x0600BBF8 RID: 48120 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600BBF9 RID: 48121 RVA: 0x00477140 File Offset: 0x00475340
		public virtual bool OpenBrowser(string _url)
		{
			return Utils.OpenSystemBrowser(_url);
		}

		// Token: 0x0600BBFA RID: 48122 RVA: 0x00002914 File Offset: 0x00000B14
		public void ControllerDisconnected(InputDevice inputDevice)
		{
		}

		// Token: 0x0600BBFB RID: 48123 RVA: 0x00477148 File Offset: 0x00475348
		public virtual string GetPlatformLanguage()
		{
			if (this.platformLanguageCache == null)
			{
				string text = Application.systemLanguage.ToStringCached<SystemLanguage>().ToLower();
				string text2;
				if (!(text == "chinesesimplified"))
				{
					if (!(text == "chinesetraditional"))
					{
						if (!(text == "korean"))
						{
							text2 = text;
						}
						else
						{
							text2 = "koreana";
						}
					}
					else
					{
						text2 = "tchinese";
					}
				}
				else
				{
					text2 = "schinese";
				}
				text = text2;
				this.platformLanguageCache = text;
			}
			return this.platformLanguageCache;
		}

		// Token: 0x0600BBFC RID: 48124 RVA: 0x004771BF File Offset: 0x004753BF
		public virtual string GetAppLanguage()
		{
			return this.GetPlatformLanguage();
		}

		// Token: 0x0600BBFD RID: 48125 RVA: 0x004771C7 File Offset: 0x004753C7
		public virtual string GetCountry()
		{
			return "??";
		}

		// Token: 0x0600BBFE RID: 48126 RVA: 0x004766EB File Offset: 0x004748EB
		public virtual void ClearTempFiles()
		{
			Utils.TryDeleteTempCacheContents();
		}

		// Token: 0x0600BBFF RID: 48127 RVA: 0x004766F2 File Offset: 0x004748F2
		public virtual string GetTempFileName(string prefix = "", string suffix = "")
		{
			return Utils.GetRandomTempCacheFileName(prefix, suffix);
		}

		// Token: 0x0600BC00 RID: 48128 RVA: 0x004771CE File Offset: 0x004753CE
		[PublicizedFrom(EAccessModifier.Internal)]
		public static void TryDeleteTempCacheContents()
		{
			Utils.TryDeleteTempDirectoryContentsExceptCrashes(PlatformApplicationManager.Application.temporaryCachePath);
		}

		// Token: 0x0600BC01 RID: 48129 RVA: 0x004771E0 File Offset: 0x004753E0
		[PublicizedFrom(EAccessModifier.Private)]
		public static void TryDeleteTempDirectoryContentsExceptCrashes(string path)
		{
			try
			{
				if (Directory.Exists(path))
				{
					foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(path).EnumerateFileSystemInfos())
					{
						try
						{
							if (!fileSystemInfo.Name.EqualsCaseInsensitive("Crashes"))
							{
								DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
								if (directoryInfo != null)
								{
									directoryInfo.Delete(true);
								}
								else
								{
									fileSystemInfo.Delete();
								}
							}
						}
						catch (Exception ex)
						{
							Log.Warning(string.Concat(new string[]
							{
								"[Platform.Shared.Utils] Could not delete '",
								fileSystemInfo.Name,
								"' from temp cache. ",
								ex.GetType().FullName,
								": ",
								ex.Message
							}));
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Warning("[Platform.Shared.Utils] Could not delete contents of temp cache. " + ex2.GetType().FullName + ": " + ex2.Message);
			}
		}

		// Token: 0x0600BC02 RID: 48130 RVA: 0x00477300 File Offset: 0x00475500
		[PublicizedFrom(EAccessModifier.Internal)]
		public static string GetRandomTempCacheFileName(string prefix, string suffix)
		{
			return Utils.GetRandomFileName(PlatformApplicationManager.Application.temporaryCachePath, prefix, suffix);
		}

		// Token: 0x0600BC03 RID: 48131 RVA: 0x00477314 File Offset: 0x00475514
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetRandomFileName(string parentDir, string prefix, string suffix)
		{
			for (int i = 0; i < 100; i++)
			{
				string randomName = Utils.GetRandomName(prefix, suffix);
				string text = Path.Join(parentDir, randomName);
				if (!File.Exists(text))
				{
					using (File.Open(text, FileMode.OpenOrCreate))
					{
						return text;
					}
				}
			}
			throw new IOException(string.Format("Failed to create a temporary file after {0} attempts.", 100));
		}

		// Token: 0x0600BC04 RID: 48132 RVA: 0x00477390 File Offset: 0x00475590
		[PublicizedFrom(EAccessModifier.Private)]
		public static string GetRandomName(string prefix, string suffix)
		{
			System.Random value = Utils.RandLocal.Value;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(prefix);
			for (int i = 0; i < 16; i++)
			{
				stringBuilder.Append("0123456789ABCDEFGHIJKLMNOPabcdefghijklmnop"[value.Next("0123456789ABCDEFGHIJKLMNOPabcdefghijklmnop".Length)]);
			}
			stringBuilder.Append(suffix);
			return stringBuilder.ToString();
		}

		// Token: 0x0600BC05 RID: 48133 RVA: 0x00047178 File Offset: 0x00045378
		public virtual string GetCrossplayPlayerIcon(EPlayGroup _playGroup, bool _fetchGenericIcons, EPlatformIdentifier _nativePlatform)
		{
			return string.Empty;
		}

		// Token: 0x040092DC RID: 37596
		[PublicizedFrom(EAccessModifier.Private)]
		public static int Seed = Environment.TickCount;

		// Token: 0x040092DD RID: 37597
		public static readonly ThreadLocal<System.Random> RandLocal = new ThreadLocal<System.Random>(() => new System.Random(Interlocked.Increment(ref Utils.Seed)));

		// Token: 0x040092DE RID: 37598
		[PublicizedFrom(EAccessModifier.Private)]
		public string platformLanguageCache;
	}
}
