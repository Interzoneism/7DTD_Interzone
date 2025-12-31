using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Backtrace.Unity;
using Backtrace.Unity.Model;
using Backtrace.Unity.Types;
using Platform;
using UnityEngine;

// Token: 0x0200112C RID: 4396
public static class BacktraceUtils
{
	// Token: 0x17000E6C RID: 3692
	// (get) Token: 0x06008A29 RID: 35369 RVA: 0x0037DC45 File Offset: 0x0037BE45
	public static bool Initialized
	{
		get
		{
			return BacktraceUtils.s_Configuration != null;
		}
	}

	// Token: 0x17000E6D RID: 3693
	// (get) Token: 0x06008A2A RID: 35370 RVA: 0x0037DC52 File Offset: 0x0037BE52
	public static bool Enabled
	{
		get
		{
			return BacktraceUtils.s_BacktraceEnabled && BacktraceUtils.s_VersionEnabled;
		}
	}

	// Token: 0x17000E6E RID: 3694
	// (get) Token: 0x06008A2B RID: 35371 RVA: 0x0037DC62 File Offset: 0x0037BE62
	public static bool BugReportFeature
	{
		get
		{
			return BacktraceUtils.Enabled && BacktraceUtils.s_BugReportFeature;
		}
	}

	// Token: 0x17000E6F RID: 3695
	// (get) Token: 0x06008A2C RID: 35372 RVA: 0x0037DC72 File Offset: 0x0037BE72
	public static bool BugReportAttachSaveFeature
	{
		get
		{
			return BacktraceUtils.Enabled && BacktraceUtils.s_BugReportFeature && BacktraceUtils.s_BugReportAttachSaveFeature;
		}
	}

	// Token: 0x17000E70 RID: 3696
	// (get) Token: 0x06008A2D RID: 35373 RVA: 0x0037DC89 File Offset: 0x0037BE89
	public static bool BugReportAttachWholeWorldFeature
	{
		get
		{
			return BacktraceUtils.s_BugReportFeature && BacktraceUtils.s_BugReportAttachSaveFeature && BacktraceUtils.s_BugReportAttachWholeWorldFeature;
		}
	}

	// Token: 0x06008A2E RID: 35374 RVA: 0x0037DCA0 File Offset: 0x0037BEA0
	public static void InitializeBacktrace()
	{
		BacktraceUtils.InitializeConfiguration();
		BacktraceUtils.InitializeBacktraceClient();
		Log.Out("Backtrace Initialized");
	}

	// Token: 0x06008A2F RID: 35375 RVA: 0x0037DCB8 File Offset: 0x0037BEB8
	public static void BacktraceUserLoggedIn(IPlatform platform)
	{
		if (BacktraceUtils.s_BacktraceClient == null)
		{
			return;
		}
		Log.Out(string.Format("[BACKTRACE] Attempting to get User ID from platform: {0}", (platform != null) ? new EPlatformIdentifier?(platform.PlatformIdentifier) : null));
		bool flag;
		if (platform == null)
		{
			flag = (null != null);
		}
		else
		{
			IUserClient user = platform.User;
			flag = (((user != null) ? user.PlatformUserId : null) != null);
		}
		if (!flag)
		{
			Log.Out(string.Format("[BACKTRACE] {0} PlatformUserId missing at this time", (platform != null) ? new EPlatformIdentifier?(platform.PlatformIdentifier) : null));
			return;
		}
		string text = platform.User.PlatformUserId.PlatformIdentifierString + "-" + platform.User.PlatformUserId.ReadablePlatformUserIdentifier;
		BacktraceUtils.s_BacktraceClient.SetAttributes(new Dictionary<string, string>
		{
			{
				"gamestats.platformuserid",
				text
			}
		});
		Log.Out("[BACKTRACE] Platform ID set to: \"" + text + "\"");
	}

	// Token: 0x06008A30 RID: 35376 RVA: 0x0037DDA4 File Offset: 0x0037BFA4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Reset()
	{
		BacktraceUtils.s_BacktraceEnabled = false;
		BacktraceUtils.s_VersionEnabled = false;
		BacktraceUtils.s_Configuration.HandleUnhandledExceptions = false;
		BacktraceUtils.s_Configuration.Sampling = 0.01;
		BacktraceUtils.s_Configuration.CaptureNativeCrashes = true;
		BacktraceUtils.s_Configuration.MinidumpType = MiniDumpType.Normal;
		BacktraceUtils.s_Configuration.DeduplicationStrategy = DeduplicationStrategy.Default;
	}

	// Token: 0x06008A31 RID: 35377 RVA: 0x0037DDFC File Offset: 0x0037BFFC
	public static void SendBugReport(string message, string screenshotPath = null, SaveInfoProvider.SaveEntryInfo saveEntry = null, bool sendSave = false, Action<BacktraceResult> callback = null)
	{
		if (!BacktraceUtils.BugReportFeature)
		{
			Log.Out("[BACKTRACE] Backtrace bug reporting disabled by platform");
			return;
		}
		if (BacktraceUtils.s_BacktraceClient == null)
		{
			BacktraceUtils.DebugEnableBacktrace();
		}
		List<string> list = new List<string>();
		if (!string.IsNullOrEmpty(screenshotPath))
		{
			list.Add(screenshotPath);
		}
		if (saveEntry != null)
		{
			if (BacktraceUtils.BugReportAttachSaveFeature && sendSave)
			{
				bool flag = false;
				string text;
				BacktraceUtils.SaveArchiveResult saveArchiveResult = BacktraceUtils.TryCreateSaveArchive(saveEntry, out text);
				if (saveArchiveResult == BacktraceUtils.SaveArchiveResult.Success)
				{
					Log.Out("[BACKTRACE] Save file path: " + text);
					list.Add(text);
				}
				else if (saveArchiveResult == BacktraceUtils.SaveArchiveResult.MissingRegions)
				{
					list.Add(text);
					List<string> list2;
					if (BacktraceUtils.TryCreateRegionArchives(saveEntry, out list2))
					{
						list.AddRange(list2);
						Log.Out("[BACKTRACE] region file paths: " + string.Join(", ", list2));
						saveArchiveResult = BacktraceUtils.SaveArchiveResult.Success;
						flag = true;
					}
				}
				string text2;
				if (saveArchiveResult != BacktraceUtils.SaveArchiveResult.FailureToArchive && BacktraceUtils.TryCreateWorldArchive(saveEntry.WorldEntry, out text2) && text2 != null)
				{
					list.Add(text2);
					Log.Out("[BACKTRACE] World file path: " + text2);
					saveArchiveResult = BacktraceUtils.SaveArchiveResult.Success;
					flag = true;
				}
				if (saveArchiveResult == BacktraceUtils.SaveArchiveResult.Success && flag)
				{
					list = BacktraceUtils.CondenseZipFiles(saveEntry, list);
				}
				Log.Out("[BACKTRACE] File Sizes:");
				foreach (string text3 in list)
				{
					Log.Out(string.Format("[BACKTRACE] {0}: {1:N3}MB", text3, (double)new SdFileInfo(text3).Length / 1024.0 / 1024.0));
				}
			}
			string text4 = Path.Combine(saveEntry.WorldEntry.Location.FullPath, "checksums.txt");
			if (SdFile.Exists(text4))
			{
				string text5 = Path.Combine(PlatformApplicationManager.Application.temporaryCachePath, "backtraceTemp", "checksums.txt");
				Log.Out("[BACKTRACE] World file path: " + saveEntry.WorldEntry.Location.FullPath);
				if (!SdDirectory.Exists(text5))
				{
					SdDirectory.CreateDirectory(Path.GetDirectoryName(text5));
				}
				SdFile.Copy(text4, text5, true);
				list.Add(text5);
			}
			string text6 = Path.Combine(saveEntry.WorldEntry.Location.FullPath, "map_info.xml");
			if (SdFile.Exists(text6))
			{
				string text7 = Path.Combine(PlatformApplicationManager.Application.temporaryCachePath, "backtraceTemp", "map_info.xml");
				if (!SdDirectory.Exists(text7))
				{
					SdDirectory.CreateDirectory(Path.GetDirectoryName(text7));
				}
				SdFile.Copy(text6, text7, true);
				list.Add(text7);
			}
		}
		if (callback != null)
		{
			callback = (Action<BacktraceResult>)Delegate.Combine(callback, new Action<BacktraceResult>(BacktraceUtils.ClearMessageTypeAttribute));
		}
		else
		{
			callback = new Action<BacktraceResult>(BacktraceUtils.ClearMessageTypeAttribute);
		}
		BacktraceUtils.SetAttribute("messagetype", "bugreport");
		BacktraceClient backtraceClient = BacktraceUtils.s_BacktraceClient;
		if (backtraceClient == null)
		{
			return;
		}
		backtraceClient.Send(message, callback, list, null);
	}

	// Token: 0x06008A32 RID: 35378 RVA: 0x0037E0C0 File Offset: 0x0037C2C0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ClearMessageTypeAttribute(BacktraceResult _result)
	{
		BacktraceUtils.SetAttribute("messagetype", null);
	}

	// Token: 0x06008A33 RID: 35379 RVA: 0x0037E0D0 File Offset: 0x0037C2D0
	public static void SendErrorReport(string messageType, string message, List<string> files = null)
	{
		if (BacktraceUtils.ErrorReportingDisabledMessageTypes.Contains(messageType))
		{
			Log.Out("Opted out of reporting error of type " + messageType);
			return;
		}
		if (BacktraceUtils.s_BacktraceClient == null)
		{
			BacktraceUtils.DebugEnableBacktrace();
		}
		BacktraceUtils.SetAttribute("messagetype", messageType);
		if (files != null)
		{
			BacktraceClient backtraceClient = BacktraceUtils.s_BacktraceClient;
			if (backtraceClient == null)
			{
				return;
			}
			backtraceClient.Send(message, new Action<BacktraceResult>(BacktraceUtils.ClearMessageTypeAttribute), files, null);
			return;
		}
		else
		{
			BacktraceClient backtraceClient2 = BacktraceUtils.s_BacktraceClient;
			if (backtraceClient2 == null)
			{
				return;
			}
			backtraceClient2.Send(message, new Action<BacktraceResult>(BacktraceUtils.ClearMessageTypeAttribute), null, null);
			return;
		}
	}

	// Token: 0x06008A34 RID: 35380 RVA: 0x0037E158 File Offset: 0x0037C358
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<string> CondenseZipFiles(SaveInfoProvider.SaveEntryInfo saveEntry, List<string> attachmentPaths)
	{
		string text = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, string.Concat(new string[]
		{
			"/combined_",
			saveEntry.WorldEntry.Name,
			"_",
			saveEntry.Name,
			"/"
		}));
		string text2 = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, string.Concat(new string[]
		{
			"/combined_",
			saveEntry.WorldEntry.Name,
			"_",
			saveEntry.Name,
			".zip"
		}));
		if (SdDirectory.Exists(text))
		{
			SdDirectory.Delete(text, true);
		}
		SdDirectory.CreateDirectory(text);
		List<string> list = new List<string>();
		List<string> list2 = new List<string>(attachmentPaths);
		foreach (string text3 in attachmentPaths)
		{
			if (Path.GetFileName(text3).ContainsCaseInsensitive("zip"))
			{
				string text4 = Path.Join(text, Path.GetFileNameWithoutExtension(text3)) + Path.AltDirectorySeparatorChar.ToString();
				SdDirectory.CreateDirectory(text4);
				ZipFile.ExtractToDirectory(text3, text4);
				list.Add(text3);
				list2.Remove(text3);
			}
		}
		using (Stream stream = SdFile.Open(text2, FileMode.Create, FileAccess.Write))
		{
			using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Create))
			{
				zipArchive.CreateFromDirectory(text);
			}
		}
		SdDirectory.Delete(text, true);
		if ((double)new SdFileInfo(text2).Length / 1024.0 / 1024.0 <= 30.0)
		{
			Log.Out("[BACKTRACE] Created combined archive: " + text2);
			Log.Out("[BACKTRACE] Remove the following: " + string.Join(", ", list));
			attachmentPaths = list2;
			attachmentPaths.Add(text2);
			using (List<string>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string path = enumerator.Current;
					SdFile.Delete(path);
				}
				return attachmentPaths;
			}
		}
		Log.Out("[BACKTRACE] Combined archive is too large to upload: " + text2);
		SdFile.Delete(text2);
		return attachmentPaths;
	}

	// Token: 0x06008A35 RID: 35381 RVA: 0x0037E3D4 File Offset: 0x0037C5D4
	public static BacktraceUtils.SaveArchiveResult TryCreateSaveArchive(SaveInfoProvider.SaveEntryInfo saveEntry, out string archivePath)
	{
		BacktraceUtils.SaveArchiveResult result;
		try
		{
			string saveGameDir = GameIO.GetSaveGameDir(saveEntry.WorldEntry.Name, saveEntry.Name);
			archivePath = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, string.Concat(new string[]
			{
				"Save_",
				saveEntry.WorldEntry.Name,
				"_",
				saveEntry.Name,
				".zip"
			}));
			if (SdFile.Exists(archivePath))
			{
				Log.Out("[BACKTRACE] Old save archive path: {0} exists, deleting...", new object[]
				{
					archivePath
				});
				SdFile.Delete(archivePath);
			}
			using (Stream stream = SdFile.OpenWrite(archivePath))
			{
				using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, false, null))
				{
					zipArchive.CreateFromDirectory(saveGameDir);
				}
			}
			double num = (double)new SdFileInfo(archivePath).Length / 1024.0 / 1024.0;
			if (SdFile.Exists(archivePath) && num <= 30.0)
			{
				if (new SdFileInfo(archivePath).Length != 0L)
				{
					Log.Out("[BACKTRACE] Save archive path: {0} exists, size is {1}MB, success!", new object[]
					{
						archivePath,
						num.ToString("N3")
					});
					return BacktraceUtils.SaveArchiveResult.Success;
				}
				Log.Warning("[BACKTRACE] Save archive exists: {0}, but is empty, retry", new object[]
				{
					archivePath
				});
				SdFile.Delete(archivePath);
			}
			else if (SdFile.Exists(archivePath))
			{
				Log.Out("[BACKTRACE] Save archive path: {0} exists, size is too big, {1}MB, deleting...", new object[]
				{
					archivePath,
					num.ToString("N3")
				});
				SdFile.Delete(archivePath);
			}
			using (ZipArchive zipArchive2 = ZipFile.Open(archivePath, ZipArchiveMode.Create))
			{
				SdDirectoryInfo directoryInfo = new SdDirectoryInfo(saveGameDir);
				zipArchive2.AddSearchPattern(directoryInfo, "*.txt", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.sdf", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.ttw", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.xml", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.7dt", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.nim", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.dat", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.ttp", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.ttp.meta", SearchOption.AllDirectories);
				zipArchive2.AddSearchPattern(directoryInfo, "*.7rm", SearchOption.AllDirectories);
			}
			if (SdFile.Exists(archivePath))
			{
				result = BacktraceUtils.SaveArchiveResult.MissingRegions;
			}
			else
			{
				result = BacktraceUtils.SaveArchiveResult.FailureToArchive;
			}
		}
		catch (Exception ex)
		{
			Log.Error("[BACKTRACE]  Exception: Could not create save archive: {0}", new object[]
			{
				ex.Message
			});
			archivePath = null;
			result = BacktraceUtils.SaveArchiveResult.FailureToArchive;
		}
		return result;
	}

	// Token: 0x06008A36 RID: 35382 RVA: 0x0037E6BC File Offset: 0x0037C8BC
	public static bool TryCreateRegionArchives(SaveInfoProvider.SaveEntryInfo saveEntry, out List<string> archivePaths)
	{
		bool result;
		try
		{
			string saveGameDir = GameIO.GetSaveGameDir(saveEntry.WorldEntry.Name, saveEntry.Name);
			archivePaths = new List<string>();
			foreach (SdFileSystemInfo sdFileSystemInfo in from info in new SdDirectoryInfo(PlatformApplicationManager.Application.temporaryCachePath).EnumerateFileSystemInfos()
			where info.Name.EndsWith("_Region.zip", StringComparison.InvariantCulture)
			select info)
			{
				try
				{
					SdFile.Delete(sdFileSystemInfo.FullName);
				}
				catch (Exception arg)
				{
					Log.Warning(string.Format("[BACKTRACE] Failed To Delete {0}, Reason {1}", sdFileSystemInfo.FullName, arg));
				}
			}
			int num = 0;
			string text = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, string.Format("Save_{0}_{1}_{2}_Region.zip", saveEntry.WorldEntry.Name, saveEntry.Name, num));
			Log.Out("[BACKTRACE] World archive path: {0}", new object[]
			{
				text
			});
			SdDirectoryInfo sdDirectoryInfo = new SdDirectoryInfo(saveGameDir);
			IEnumerable<SdFileSystemInfo> enumerable = (from fileSystemInfo in sdDirectoryInfo.EnumerateFileSystemInfos("*.7rg", SearchOption.AllDirectories)
			orderby fileSystemInfo.LastWriteTime.ToFileTimeUtc()
			select fileSystemInfo).Reverse<SdFileSystemInfo>();
			if (!(from fileSystemInfo in sdDirectoryInfo.EnumerateFileSystemInfos("*.7rg", SearchOption.AllDirectories)
			orderby fileSystemInfo.LastWriteTime.ToFileTimeUtc()
			select fileSystemInfo).Reverse<SdFileSystemInfo>().Any<SdFileSystemInfo>())
			{
				enumerable = sdDirectoryInfo.EnumerateFileSystemInfos("*.7rg", SearchOption.AllDirectories);
			}
			Queue<SdFileSystemInfo> queue = new Queue<SdFileSystemInfo>(enumerable);
			while (queue.Any<SdFileSystemInfo>())
			{
				enumerable = queue;
				foreach (SdFileSystemInfo sdFileSystemInfo2 in enumerable)
				{
					SdFileInfo sdFileInfo = new SdFileInfo(sdFileSystemInfo2.FullName);
					Log.Out(string.Format("{0} File size: {1} MiB", sdFileInfo.FullName, (float)sdFileInfo.Length / 1024f * 1024f));
					if (sdFileInfo.Length > 15728640L)
					{
						Log.Warning("File too big! " + text);
					}
					else
					{
						using (Stream stream = SdFile.Open(text, FileMode.OpenOrCreate, FileAccess.ReadWrite))
						{
							using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Update, false, null))
							{
								zipArchive.CreateEntryFromFile(sdFileSystemInfo2, sdFileSystemInfo2.Name, System.IO.Compression.CompressionLevel.Optimal);
							}
						}
						if (new SdFileInfo(text).Length > 31457280L)
						{
							Log.Warning("Archive too big! " + text);
							archivePaths.Add(text);
							break;
						}
						queue.Dequeue();
					}
				}
				archivePaths.Add(text);
				num++;
				text = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, string.Format("Save_{0}_{1}_{2}_Region.zip", saveEntry.WorldEntry.Name, saveEntry.Name, num));
			}
			archivePaths = new List<string>(archivePaths.Distinct<string>());
			result = true;
		}
		catch (Exception ex)
		{
			Log.Error("[BACKTRACE] Exception: Could not create save region archive: {0}", new object[]
			{
				ex.Message
			});
			archivePaths = new List<string>();
			result = false;
		}
		return result;
	}

	// Token: 0x06008A37 RID: 35383 RVA: 0x0037EA8C File Offset: 0x0037CC8C
	public static bool TryCreateWorldArchive(SaveInfoProvider.WorldEntryInfo worldEntry, out string archivePath)
	{
		bool result;
		try
		{
			string fullPath = Path.GetFullPath(GameIO.GetWorldDir(worldEntry.Name));
			if (PathAbstractions.WorldsSearchPaths.GetLocation(fullPath, null, null).Type == PathAbstractions.EAbstractedLocationType.GameData)
			{
				Log.Out("[BACKTRACE] World {0} was found in the GameData, do not archive.", new object[]
				{
					worldEntry.Name
				});
				archivePath = string.Empty;
				result = false;
			}
			else
			{
				string fullPath2 = Path.GetFullPath(GameIO.GetUserGameDataDir());
				if (!fullPath.Contains(fullPath2))
				{
					Log.Out("[BACKTRACE] Not creating archive for world {0} not in the User Game Data Directory", new object[]
					{
						worldEntry.Name
					});
					archivePath = null;
					result = false;
				}
				else
				{
					Log.Out("[BACKTRACE] Creating archive for GeneratedWorld {0}", new object[]
					{
						worldEntry.Name
					});
					archivePath = Path.Join(PlatformApplicationManager.Application.temporaryCachePath, "/world_" + worldEntry.Name + ".zip");
					if (SdFile.Exists(archivePath))
					{
						Log.Out("[BACKTRACE] Old World archive path: {0} exists, deleting...", new object[]
						{
							archivePath
						});
					}
					Log.Out("[BACKTRACE] World archive path: {0}. Creating archive...", new object[]
					{
						archivePath
					});
					if (BacktraceUtils.BugReportAttachWholeWorldFeature)
					{
						using (Stream stream = SdFile.Open(archivePath, FileMode.Create, FileAccess.ReadWrite))
						{
							using (ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, false, null))
							{
								zipArchive.CreateFromDirectory(fullPath);
							}
						}
						double num = (double)new SdFileInfo(archivePath).Length / 1024.0 / 1024.0;
						if (SdFile.Exists(archivePath) && (BacktraceUtils.BugReportAttachWholeWorldFeature || num <= 30.0))
						{
							if (new SdFileInfo(archivePath).Length != 0L)
							{
								Log.Out("[BACKTRACE] World archive path: {0} exists, size is {1}MB, success!", new object[]
								{
									archivePath,
									num.ToString("N3")
								});
								return true;
							}
							Log.Warning("[BACKTRACE] World archive exists: {0}, but is empty, retry", new object[]
							{
								archivePath
							});
							SdFile.Delete(archivePath);
						}
						else if (SdFile.Exists(archivePath))
						{
							Log.Out("[BACKTRACE] World archive path: {0} exists, size is too big, {1}MB, deleting...", new object[]
							{
								archivePath,
								num.ToString("N3")
							});
							SdFile.Delete(archivePath);
						}
					}
					Log.Out("[BACKTRACE] Attempting to archive only required elements...", new object[]
					{
						archivePath
					});
					using (Stream stream2 = SdFile.Open(archivePath, FileMode.Create, FileAccess.ReadWrite))
					{
						using (ZipArchive zipArchive2 = new ZipArchive(stream2, ZipArchiveMode.Update, false, null))
						{
							SdDirectoryInfo directoryInfo = new SdDirectoryInfo(fullPath);
							zipArchive2.AddSearchPattern(directoryInfo, "*.ttw", SearchOption.TopDirectoryOnly);
							zipArchive2.AddSearchPattern(directoryInfo, "*.xml", SearchOption.TopDirectoryOnly);
							zipArchive2.AddSearchPattern(directoryInfo, "*.txt", SearchOption.TopDirectoryOnly);
						}
					}
					if (SdFile.Exists(archivePath))
					{
						double num2 = (double)new SdFileInfo(archivePath).Length / 1024.0 / 1024.0;
						Log.Out("[BACKTRACE] World archive path: {0}", new object[]
						{
							archivePath
						});
						Log.Out("[BACKTRACE] World archive File size: {0}MB", new object[]
						{
							num2.ToString("N3")
						});
						if (!BacktraceUtils.BugReportAttachWholeWorldFeature && num2 > 30.0)
						{
							SdFile.Delete(archivePath);
							archivePath = null;
							Log.Error("[BACKTRACE] Exception: World archive too big, deleted");
							return false;
						}
					}
					result = true;
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("[BACKTRACE] Exception: Could not create world archive: {0}", new object[]
			{
				ex.Message
			});
			archivePath = null;
			result = false;
		}
		return result;
	}

	// Token: 0x06008A38 RID: 35384 RVA: 0x0037EE58 File Offset: 0x0037D058
	public static void DebugEnableBacktrace()
	{
		BacktraceUtils.s_BacktraceEnabled = true;
		BacktraceUtils.s_VersionEnabled = true;
		BacktraceUtils.InitializeBacktraceClient();
	}

	// Token: 0x06008A39 RID: 35385 RVA: 0x0037EE6C File Offset: 0x0037D06C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void InitializeBacktraceClient()
	{
		if (BacktraceUtils.Enabled)
		{
			if (BacktraceUtils.s_Configuration == null)
			{
				BacktraceUtils.InitializeConfiguration();
			}
			BacktraceUtils.s_BacktraceClient = BacktraceClient.Initialize(BacktraceUtils.s_Configuration, null, "BacktraceClient");
			BacktraceUtils.SetAttributes(new Dictionary<string, string>
			{
				{
					"svn.commit",
					BacktraceUtils.s_svncommit
				},
				{
					"game.version",
					Constants.cVersionInformation.SerializableString
				}
			});
			BacktraceClient backtraceClient = BacktraceUtils.s_BacktraceClient;
			backtraceClient.OnServerError = (Action<Exception>)Delegate.Combine(backtraceClient.OnServerError, new Action<Exception>(delegate(Exception e)
			{
				Log.Error("[BACKTRACE] Error response: " + e.Message);
			}));
			BacktraceClient backtraceClient2 = BacktraceUtils.s_BacktraceClient;
			backtraceClient2.OnClientReportLimitReached = (Action<BacktraceReport>)Delegate.Combine(backtraceClient2.OnClientReportLimitReached, new Action<BacktraceReport>(delegate(BacktraceReport e)
			{
				Log.Error("[BACKTRACE] Report Limit Reached Error: " + e.Message);
			}));
			if (PlatformManager.CrossplatformPlatform != null && PlatformManager.CrossplatformPlatform.User != null)
			{
				PlatformManager.CrossplatformPlatform.User.UserLoggedIn += BacktraceUtils.BacktraceUserLoggedIn;
				return;
			}
			if (PlatformManager.NativePlatform.User != null)
			{
				PlatformManager.NativePlatform.User.UserLoggedIn += BacktraceUtils.BacktraceUserLoggedIn;
				return;
			}
		}
		else
		{
			BacktraceUtils.s_BacktraceClient = null;
		}
	}

	// Token: 0x06008A3A RID: 35386 RVA: 0x0037EFA8 File Offset: 0x0037D1A8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void InitializeConfiguration()
	{
		BacktraceUtils.s_Configuration = ScriptableObject.CreateInstance<BacktraceConfiguration>();
		BacktraceUtils.s_Configuration.ServerUrl = "https://thefunpimps.sp.backtrace.io:6098/post?format=json&token=4deafd275ace1a865cc35c882f48a2d1f848c59fabea40227c1bfd84d9c794d9";
		BacktraceUtils.s_Configuration.Enabled = true;
		BacktraceUtils.s_platformString = DeviceFlag.StandaloneWindows.ToString().ToUpper();
		BacktraceUtils.Reset();
		object @lock = BacktraceUtils._lock;
		lock (@lock)
		{
			BacktraceUtils.s_PlayerLogAttachmentPath = Path.Join(Application.temporaryCachePath, "Player.log");
			Log.AddOutputPath(BacktraceUtils.s_PlayerLogAttachmentPath);
			BacktraceUtils.s_Configuration.AttachmentPaths = new string[]
			{
				BacktraceUtils.s_PlayerLogAttachmentPath
			};
		}
	}

	// Token: 0x06008A3B RID: 35387 RVA: 0x0037F064 File Offset: 0x0037D264
	public static void StartStatisticsUpdate()
	{
		DebugGameStats.StartStatisticsUpdate(new DebugGameStats.StatisticsUpdatedCallback(BacktraceUtils.SetAttributes));
	}

	// Token: 0x06008A3C RID: 35388 RVA: 0x0037F077 File Offset: 0x0037D277
	public static void SetAttributes(Dictionary<string, string> _attributesDictionary)
	{
		BacktraceClient backtraceClient = BacktraceUtils.s_BacktraceClient;
		if (backtraceClient == null)
		{
			return;
		}
		backtraceClient.SetAttributes(_attributesDictionary);
	}

	// Token: 0x06008A3D RID: 35389 RVA: 0x0037F089 File Offset: 0x0037D289
	public static void SetAttribute(string _attributeName, string _attributeValue)
	{
		BacktraceClient backtraceClient = BacktraceUtils.s_BacktraceClient;
		if (backtraceClient == null)
		{
			return;
		}
		backtraceClient.SetAttributes(new Dictionary<string, string>
		{
			{
				_attributeName,
				_attributeValue
			}
		});
	}

	// Token: 0x06008A3E RID: 35390 RVA: 0x0037F0A8 File Offset: 0x0037D2A8
	public static void UpdateConfig(XmlFile _xmlFile)
	{
		if (BacktraceUtils.s_Configuration == null)
		{
			return;
		}
		BacktraceUtils.Reset();
		XElement root = _xmlFile.XmlDoc.Root;
		if (root == null)
		{
			Log.Out("Could not load Backtrace Config from file " + _xmlFile.Filename + ".");
			return;
		}
		BacktraceUtils.s_VersionEnabled = true;
		foreach (XElement element in root.Elements("platform"))
		{
			BacktraceUtils.ParsePlatform(element);
		}
		BacktraceUtils.InitializeBacktraceClient();
		Log.Out(string.Format("Backtrace Configuration refreshed from XML: Enabled {0}", BacktraceUtils.Enabled));
		Log.Out("[BACKTRACE] Bug reporting: " + (BacktraceUtils.s_BugReportFeature ? "Enabled" : "Disabled"));
		Log.Out("[BACKTRACE] Bug reporting attach save feature: " + (BacktraceUtils.s_BugReportAttachSaveFeature ? "Enabled" : "Disabled"));
		Log.Out("[BACKTRACE] Bug reporting attach entire world feature: " + (BacktraceUtils.s_BugReportAttachWholeWorldFeature ? "Enabled" : "Disabled"));
	}

	// Token: 0x06008A3F RID: 35391 RVA: 0x0037F1C4 File Offset: 0x0037D3C4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParsePlatform(XElement _element)
	{
		string text;
		if (!_element.TryGetAttribute("name", out text))
		{
			throw new XmlLoadException("BacktraceConfig", _element, "Platform node attribute 'name' missing");
		}
		if (text.ToUpper() == "DEFAULT" || text.ToUpper() == BacktraceUtils.s_platformString)
		{
			foreach (XElement element in _element.Elements())
			{
				BacktraceUtils.ParsePlatformElement(element);
			}
		}
	}

	// Token: 0x06008A40 RID: 35392 RVA: 0x0037F258 File Offset: 0x0037D458
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParsePlatformElement(XElement _element)
	{
		if (_element.Name == BacktraceUtils.enabled)
		{
			string text;
			BacktraceUtils.s_BacktraceEnabled = (_element.TryGetAttribute("value", out text) && text.Equals("true"));
			return;
		}
		if (_element.Name == BacktraceUtils.excludedversions)
		{
			string text2;
			_element.TryGetAttribute("value", out text2);
			if (BacktraceUtils.s_VersionEnabled)
			{
				foreach (string text3 in text2.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					BacktraceUtils.s_VersionEnabled = !Constants.cVersionInformation.SerializableString.ContainsCaseInsensitive(text3);
					if (!BacktraceUtils.s_VersionEnabled)
					{
						Log.Out("[BACKTRACE] version " + text3 + " is Excluded, and matches current version, backtrace disabled");
						return;
					}
				}
				return;
			}
		}
		else if (_element.Name == BacktraceUtils.sampling)
		{
			string s;
			_element.TryGetAttribute("value", out s);
			double num;
			if (double.TryParse(s, out num))
			{
				BacktraceUtils.s_Configuration.Sampling = num;
				return;
			}
		}
		else if (_element.Name == BacktraceUtils.deduplicationStrategy)
		{
			string value;
			_element.TryGetAttribute("value", out value);
			DeduplicationStrategy deduplicationStrategy;
			if (Enum.TryParse<DeduplicationStrategy>(value, out deduplicationStrategy))
			{
				BacktraceUtils.s_Configuration.DeduplicationStrategy = deduplicationStrategy;
				return;
			}
		}
		else if (_element.Name == BacktraceUtils.minidumptype)
		{
			string value2;
			_element.TryGetAttribute("value", out value2);
			MiniDumpType minidumpType;
			if (Enum.TryParse<MiniDumpType>(value2, out minidumpType))
			{
				BacktraceUtils.s_Configuration.MinidumpType = minidumpType;
				return;
			}
		}
		else
		{
			if (_element.Name == BacktraceUtils.enablemetricssupport)
			{
				string text4;
				BacktraceUtils.s_Configuration.EnableMetricsSupport = (_element.TryGetAttribute("value", out text4) && text4.Equals("true"));
				return;
			}
			if (_element.Name == BacktraceUtils.bugreporting)
			{
				string text5;
				BacktraceUtils.s_BugReportFeature = (_element.TryGetAttribute("value", out text5) && text5.Equals("true"));
				Log.Out("[BACKTRACE] Bug reporting " + (BacktraceUtils.s_BugReportFeature ? "Enabled" : "Disabled") + " with save uploading: " + (BacktraceUtils.s_BugReportAttachSaveFeature ? "Enabled" : "Disabled"));
				return;
			}
			if (_element.Name == BacktraceUtils.attachsaves)
			{
				string text6;
				BacktraceUtils.s_BugReportAttachSaveFeature = (_element.TryGetAttribute("value", out text6) && text6.Equals("true"));
				Log.Out("[BACKTRACE] Save Attaching Feature: " + (BacktraceUtils.s_BugReportAttachSaveFeature ? "Enabled" : "Disabled"));
				return;
			}
			if (_element.Name == BacktraceUtils.attachentireworld)
			{
				string text7;
				BacktraceUtils.s_BugReportAttachWholeWorldFeature = (_element.TryGetAttribute("value", out text7) && text7.Equals("true"));
				Log.Out("[BACKTRACE] Entire World Attaching Feature: " + (BacktraceUtils.s_BugReportAttachWholeWorldFeature ? "Enabled" : "Disabled"));
				return;
			}
			if (_element.Name == BacktraceUtils.errorreportdisabledmessagetypes)
			{
				string text8;
				_element.TryGetAttribute("value", out text8);
				string[] array2 = text8.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				Log.Out("[BACKTRACE] Disabling Error Report Message Types: " + string.Join(", ", array2));
				BacktraceUtils.ErrorReportingDisabledMessageTypes.AddRange(array2);
			}
		}
	}

	// Token: 0x04006C1C RID: 27676
	[PublicizedFrom(EAccessModifier.Private)]
	public const long BacktraceFileSizeLimitMebibyte = 30L;

	// Token: 0x04006C1D RID: 27677
	[PublicizedFrom(EAccessModifier.Private)]
	public static BacktraceClient s_BacktraceClient;

	// Token: 0x04006C1E RID: 27678
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object _lock = new object();

	// Token: 0x04006C1F RID: 27679
	[PublicizedFrom(EAccessModifier.Private)]
	public static string s_PlayerLogAttachmentPath;

	// Token: 0x04006C20 RID: 27680
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string s_svncommit = "79285";

	// Token: 0x04006C21 RID: 27681
	[PublicizedFrom(EAccessModifier.Private)]
	public static string s_platformString;

	// Token: 0x04006C22 RID: 27682
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName enabled = "enabled";

	// Token: 0x04006C23 RID: 27683
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName excludedversions = "excludedversions";

	// Token: 0x04006C24 RID: 27684
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName sampling = "sampling";

	// Token: 0x04006C25 RID: 27685
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName deduplicationStrategy = "deduplicationstrategy";

	// Token: 0x04006C26 RID: 27686
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName minidumptype = "minidumptype";

	// Token: 0x04006C27 RID: 27687
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName enablemetricssupport = "enablemetricssupport";

	// Token: 0x04006C28 RID: 27688
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName bugreporting = "bugreportfeature";

	// Token: 0x04006C29 RID: 27689
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName attachsaves = "bugreportattachsaves";

	// Token: 0x04006C2A RID: 27690
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName attachentireworld = "bugreportattachentireworld";

	// Token: 0x04006C2B RID: 27691
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly XName errorreportdisabledmessagetypes = "errorreportdisabledmessagetypes";

	// Token: 0x04006C2C RID: 27692
	[PublicizedFrom(EAccessModifier.Private)]
	public static BacktraceConfiguration s_Configuration;

	// Token: 0x04006C2D RID: 27693
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_BacktraceEnabled = false;

	// Token: 0x04006C2E RID: 27694
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_VersionEnabled = false;

	// Token: 0x04006C2F RID: 27695
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_BugReportFeature = false;

	// Token: 0x04006C30 RID: 27696
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_BugReportAttachSaveFeature = false;

	// Token: 0x04006C31 RID: 27697
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_BugReportAttachWholeWorldFeature = false;

	// Token: 0x04006C32 RID: 27698
	public static List<string> ErrorReportingDisabledMessageTypes = new List<string>();

	// Token: 0x0200112D RID: 4397
	public enum SaveArchiveResult
	{
		// Token: 0x04006C34 RID: 27700
		FailureToArchive,
		// Token: 0x04006C35 RID: 27701
		Success,
		// Token: 0x04006C36 RID: 27702
		MissingRegions
	}
}
