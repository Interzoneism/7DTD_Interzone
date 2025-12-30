using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Platform;
using UnityEngine;

// Token: 0x02001207 RID: 4615
public class SaveInfoProvider
{
	// Token: 0x17000EE8 RID: 3816
	// (get) Token: 0x06009004 RID: 36868 RVA: 0x0039764C File Offset: 0x0039584C
	public static SaveInfoProvider Instance
	{
		get
		{
			if (SaveInfoProvider.instance == null)
			{
				SaveInfoProvider.instance = new SaveInfoProvider();
			}
			return SaveInfoProvider.instance;
		}
	}

	// Token: 0x17000EE9 RID: 3817
	// (get) Token: 0x06009005 RID: 36869 RVA: 0x00397664 File Offset: 0x00395864
	public static bool DataLimitEnabled
	{
		get
		{
			return SaveDataUtils.SaveDataManager.ShouldLimitSize();
		}
	}

	// Token: 0x06009006 RID: 36870 RVA: 0x00397670 File Offset: 0x00395870
	public static string GetWorldEntryKey(string worldName, string worldType)
	{
		return (worldName + worldType).ToLowerInvariant();
	}

	// Token: 0x06009007 RID: 36871 RVA: 0x0039767E File Offset: 0x0039587E
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetSaveEntryKey(string worldKey, string saveName)
	{
		return (worldKey + "/" + saveName).ToLowerInvariant();
	}

	// Token: 0x17000EEA RID: 3818
	// (get) Token: 0x06009008 RID: 36872 RVA: 0x00397691 File Offset: 0x00395891
	public ReadOnlyCollection<SaveInfoProvider.WorldEntryInfo> WorldEntryInfos
	{
		get
		{
			this.RefreshIfDirty();
			return this.worldEntryInfos.AsReadOnly();
		}
	}

	// Token: 0x17000EEB RID: 3819
	// (get) Token: 0x06009009 RID: 36873 RVA: 0x003976A4 File Offset: 0x003958A4
	public ReadOnlyCollection<SaveInfoProvider.SaveEntryInfo> SaveEntryInfos
	{
		get
		{
			this.RefreshIfDirty();
			return this.saveEntryInfos.AsReadOnly();
		}
	}

	// Token: 0x17000EEC RID: 3820
	// (get) Token: 0x0600900A RID: 36874 RVA: 0x003976B7 File Offset: 0x003958B7
	public ReadOnlyCollection<SaveInfoProvider.PlayerEntryInfo> PlayerEntryInfos
	{
		get
		{
			this.RefreshIfDirty();
			return this.playerEntryInfos.AsReadOnly();
		}
	}

	// Token: 0x17000EED RID: 3821
	// (get) Token: 0x0600900B RID: 36875 RVA: 0x003976CA File Offset: 0x003958CA
	public long TotalUsedBytes
	{
		get
		{
			this.RefreshIfDirty();
			return this.totalUsedBytes;
		}
	}

	// Token: 0x17000EEE RID: 3822
	// (get) Token: 0x0600900C RID: 36876 RVA: 0x003976D8 File Offset: 0x003958D8
	public long TotalAllowanceBytes
	{
		get
		{
			this.RefreshIfDirty();
			return this.totalAllowanceBytes;
		}
	}

	// Token: 0x17000EEF RID: 3823
	// (get) Token: 0x0600900D RID: 36877 RVA: 0x003976E6 File Offset: 0x003958E6
	public long TotalAvailableBytes
	{
		get
		{
			this.RefreshIfDirty();
			return this.totalAllowanceBytes - this.totalUsedBytes;
		}
	}

	// Token: 0x0600900E RID: 36878 RVA: 0x003976FB File Offset: 0x003958FB
	public void SetDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x0600900F RID: 36879 RVA: 0x00397704 File Offset: 0x00395904
	[PublicizedFrom(EAccessModifier.Private)]
	public static long GetPlatformReservedSizeBytes(SaveDataSizes sizes)
	{
		return 5242880L;
	}

	// Token: 0x06009010 RID: 36880 RVA: 0x0039770C File Offset: 0x0039590C
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshIfDirty()
	{
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		this.localSavesSum = 0L;
		this.remoteSavesSum = 0L;
		this.worldsSum = 0L;
		this.worldEntryInfosByWorldKey.Clear();
		this.saveEntryInfosBySaveKey.Clear();
		this.remoteSaveEntryInfosByGuid.Clear();
		this.worldEntryInfos.Clear();
		this.saveEntryInfos.Clear();
		this.playerEntryInfos.Clear();
		this.ProcessLocalWorlds();
		this.ProcessLocalWorldSaves();
		this.ProcessRemoteWorldSaves();
		this.worldEntryInfos.AddRange(this.worldEntryInfosByWorldKey.Values);
		this.totalUsedBytes = this.localSavesSum + this.remoteSavesSum + this.worldsSum;
		long num = 0L;
		this.worldEntryInfos.Sort();
		foreach (SaveInfoProvider.WorldEntryInfo worldEntryInfo in this.worldEntryInfos)
		{
			worldEntryInfo.BarStartOffset = num;
			if (worldEntryInfo.Deletable)
			{
				num += worldEntryInfo.WorldDataSize;
			}
			worldEntryInfo.SaveEntryInfos.Sort();
			foreach (SaveInfoProvider.SaveEntryInfo saveEntryInfo in worldEntryInfo.SaveEntryInfos)
			{
				saveEntryInfo.BarStartOffset = num;
				num += saveEntryInfo.SizeInfo.ReportedSize;
				long num2 = num;
				saveEntryInfo.PlayerEntryInfos.Sort();
				for (int i = saveEntryInfo.PlayerEntryInfos.Count - 1; i >= 0; i--)
				{
					SaveInfoProvider.PlayerEntryInfo playerEntryInfo = saveEntryInfo.PlayerEntryInfos[i];
					num2 -= playerEntryInfo.Size;
					playerEntryInfo.BarStartOffset = num2;
				}
			}
		}
		if (SaveDataUtils.SaveDataManager.ShouldLimitSize())
		{
			SaveDataUtils.SaveDataManager.UpdateSizes();
			SaveDataSizes sizes = SaveDataUtils.SaveDataManager.GetSizes();
			long platformReservedSizeBytes = SaveInfoProvider.GetPlatformReservedSizeBytes(sizes);
			this.totalAllowanceBytes = sizes.Total - platformReservedSizeBytes;
			return;
		}
		this.totalAllowanceBytes = -1L;
	}

	// Token: 0x06009011 RID: 36881 RVA: 0x00397924 File Offset: 0x00395B24
	public void ClearResources()
	{
		this.worldEntryInfosByWorldKey.Clear();
		this.saveEntryInfosBySaveKey.Clear();
		this.remoteSaveEntryInfosByGuid.Clear();
		this.worldEntryInfos.Clear();
		this.saveEntryInfos.Clear();
		this.playerEntryInfos.Clear();
		this.protectedDirectories.Clear();
		this.isDirty = true;
	}

	// Token: 0x06009012 RID: 36882 RVA: 0x00397988 File Offset: 0x00395B88
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessLocalWorlds()
	{
		foreach (PathAbstractions.AbstractedLocation abstractedLocation in PathAbstractions.WorldsSearchPaths.GetAvailablePathsList(null, null, null, false))
		{
			PathAbstractions.EAbstractedLocationType type = abstractedLocation.Type;
			string text;
			if (type != PathAbstractions.EAbstractedLocationType.Mods)
			{
				if (type != PathAbstractions.EAbstractedLocationType.GameData)
				{
					text = Localization.Get("xuiDmGenerated", false);
				}
				else
				{
					text = Localization.Get("xuiDmBuiltIn", false);
				}
			}
			else
			{
				text = Localization.Get("xuiDmMod", false) + ": " + abstractedLocation.ContainingMod.Name;
			}
			string type2 = text;
			bool flag = GameIO.IsWorldGenerated(abstractedLocation.Name);
			SaveInfoProvider.WorldEntryInfo worldEntryInfo = new SaveInfoProvider.WorldEntryInfo
			{
				WorldKey = SaveInfoProvider.GetWorldEntryKey(abstractedLocation.Name, "Local"),
				Name = abstractedLocation.Name,
				Type = type2,
				Location = abstractedLocation,
				Deletable = flag,
				WorldDataSize = GameIO.GetDirectorySize(abstractedLocation.FullPath, true),
				Version = null,
				HideIfEmpty = (!flag || SaveInfoProvider.HideableWorlds.ContainsCaseInsensitive(abstractedLocation.Name))
			};
			if (worldEntryInfo.Deletable)
			{
				this.worldsSum += worldEntryInfo.WorldDataSize;
			}
			GameUtils.WorldInfo worldInfo = GameUtils.WorldInfo.LoadWorldInfo(abstractedLocation);
			if (worldInfo != null)
			{
				worldEntryInfo.Version = worldInfo.GameVersionCreated;
			}
			this.worldEntryInfosByWorldKey[worldEntryInfo.WorldKey] = worldEntryInfo;
		}
	}

	// Token: 0x06009013 RID: 36883 RVA: 0x00397B10 File Offset: 0x00395D10
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessLocalWorldSaves()
	{
		string saveGameRootDir = GameIO.GetSaveGameRootDir();
		if (SdDirectory.Exists(saveGameRootDir))
		{
			SdFileSystemInfo[] array = new SdDirectoryInfo(saveGameRootDir).GetDirectories();
			foreach (SdDirectoryInfo sdDirectoryInfo in array)
			{
				if (SdDirectory.Exists(sdDirectoryInfo.FullName))
				{
					SdDirectoryInfo sdDirectoryInfo2 = new SdDirectoryInfo(sdDirectoryInfo.FullName);
					SdFileSystemInfo[] array2 = sdDirectoryInfo2.GetDirectories();
					SdFileSystemInfo[] array3 = array2;
					if (array3.Length == 0 && sdDirectoryInfo2.GetFiles().Length == 0)
					{
						SdDirectory.Delete(sdDirectoryInfo.FullName);
					}
					else
					{
						SaveInfoProvider.WorldEntryInfo worldEntryInfo;
						if (!this.worldEntryInfosByWorldKey.TryGetValue(SaveInfoProvider.GetWorldEntryKey(sdDirectoryInfo.Name, "Local"), out worldEntryInfo))
						{
							worldEntryInfo = new SaveInfoProvider.WorldEntryInfo
							{
								WorldKey = SaveInfoProvider.GetWorldEntryKey(sdDirectoryInfo.Name, SaveInfoProvider.DeletedWorldsType),
								Name = sdDirectoryInfo.Name,
								Type = SaveInfoProvider.DeletedWorldsType,
								Location = PathAbstractions.AbstractedLocation.None,
								Deletable = false,
								WorldDataSize = 0L,
								Version = null,
								HideIfEmpty = true
							};
							this.worldEntryInfosByWorldKey[worldEntryInfo.WorldKey] = worldEntryInfo;
						}
						foreach (SdDirectoryInfo curSaveFolder in array3)
						{
							this.ProcessSaveEntry(worldEntryInfo, curSaveFolder);
						}
					}
				}
			}
		}
	}

	// Token: 0x06009014 RID: 36884 RVA: 0x00397C60 File Offset: 0x00395E60
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessSaveEntry(SaveInfoProvider.WorldEntryInfo worldEntryInfo, SdDirectoryInfo curSaveFolder)
	{
		string text = curSaveFolder.FullName + "/main.ttw";
		SaveInfoProvider.SaveEntryInfo saveEntryInfo = new SaveInfoProvider.SaveEntryInfo
		{
			Name = curSaveFolder.Name,
			WorldEntry = worldEntryInfo,
			SaveDir = curSaveFolder.FullName,
			Version = null
		};
		saveEntryInfo.SizeInfo.BytesOnDisk = GameIO.GetDirectorySize(curSaveFolder, true);
		saveEntryInfo.SizeInfo.IsArchived = (SaveInfoProvider.DataLimitEnabled && SdFile.Exists(Path.Combine(curSaveFolder.FullName, "archived.flag")));
		if (SdFile.Exists(text))
		{
			saveEntryInfo.LastSaved = SdFile.GetLastWriteTime(text);
			try
			{
				WorldState worldState = new WorldState();
				worldState.Load(text, false, false, false);
				saveEntryInfo.Version = worldState.gameVersion;
				if (SaveInfoProvider.DataLimitEnabled && worldState.saveDataLimit != -1L)
				{
					saveEntryInfo.SizeInfo.BytesReserved = worldState.saveDataLimit;
					if (saveEntryInfo.SizeInfo.BytesOnDisk > saveEntryInfo.SizeInfo.BytesReserved)
					{
						Debug.LogError(string.Format("Directory size of save \"{0}\" exceeds serialized save data limit of {1}.", curSaveFolder.FullName, worldState.saveDataLimit));
					}
				}
				else
				{
					saveEntryInfo.SizeInfo.BytesReserved = -1L;
				}
				goto IL_178;
			}
			catch (Exception ex)
			{
				Log.Warning("Error reading header of level '" + text + "'. Msg: " + ex.Message);
				goto IL_178;
			}
		}
		if (curSaveFolder.Name != "WorldEditor" && curSaveFolder.Name != "PrefabEditor")
		{
			Log.Warning(string.Format("Could not find main ttw file for save in directory: {0}", curSaveFolder));
		}
		saveEntryInfo.LastSaved = curSaveFolder.LastWriteTime;
		IL_178:
		this.saveEntryInfos.Add(saveEntryInfo);
		string saveEntryKey = SaveInfoProvider.GetSaveEntryKey(saveEntryInfo.WorldEntry.WorldKey, saveEntryInfo.Name);
		this.saveEntryInfosBySaveKey[saveEntryKey] = saveEntryInfo;
		worldEntryInfo.SaveEntryInfos.Add(saveEntryInfo);
		worldEntryInfo.SaveDataCount++;
		worldEntryInfo.SaveDataSize += saveEntryInfo.SizeInfo.ReportedSize;
		this.localSavesSum += saveEntryInfo.SizeInfo.ReportedSize;
		this.ProcessPlayerEntries(saveEntryInfo);
	}

	// Token: 0x06009015 RID: 36885 RVA: 0x00397E78 File Offset: 0x00396078
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessPlayerEntries(SaveInfoProvider.SaveEntryInfo saveEntryInfo)
	{
		string text = saveEntryInfo.SaveDir + "/Player";
		if (!SdDirectory.Exists(text))
		{
			return;
		}
		this.fileNameKeysToPlayerInfos.Clear();
		foreach (SdFileInfo sdFileInfo in new SdDirectoryInfo(text).GetFiles())
		{
			int length;
			string text2;
			string a;
			if ((length = sdFileInfo.Name.IndexOf('.')) != -1)
			{
				text2 = sdFileInfo.Name.Substring(0, length);
				int num = sdFileInfo.Name.LastIndexOf('.') + 1;
				a = sdFileInfo.Name.Substring(num, sdFileInfo.Name.Length - num);
			}
			else
			{
				Debug.LogError("Encountered player save file with no extension.");
				text2 = sdFileInfo.Name;
				a = string.Empty;
			}
			DateTime lastWriteTime = SdFile.GetLastWriteTime(sdFileInfo.FullName);
			SaveInfoProvider.PlayerEntryInfo playerEntryInfo;
			if (!this.fileNameKeysToPlayerInfos.TryGetValue(text2, out playerEntryInfo))
			{
				playerEntryInfo = new SaveInfoProvider.PlayerEntryInfo
				{
					Id = text2,
					LastPlayed = lastWriteTime,
					SaveEntry = saveEntryInfo
				};
				PlatformUserIdentifierAbs platformUserIdentifierAbs;
				if (PlatformUserIdentifierAbs.TryFromCombinedString(text2, out platformUserIdentifierAbs))
				{
					playerEntryInfo.PrimaryUserId = platformUserIdentifierAbs;
					playerEntryInfo.CachedName = platformUserIdentifierAbs.ReadablePlatformUserIdentifier;
				}
				else
				{
					Log.Error("Could not associate player save file \"" + sdFileInfo.FullName + "\" with a player id. Combined id string: " + text2);
					playerEntryInfo.CachedName = text2;
				}
				saveEntryInfo.PlayerEntryInfos.Add(playerEntryInfo);
			}
			else if (lastWriteTime > playerEntryInfo.LastPlayed)
			{
				playerEntryInfo.LastPlayed = lastWriteTime;
			}
			playerEntryInfo.Size += sdFileInfo.Length;
			PlayerMetaInfo playerMetaInfo;
			if (string.Equals(a, "meta", StringComparison.InvariantCultureIgnoreCase) && PlayerMetaInfo.TryRead(sdFileInfo.FullName, out playerMetaInfo))
			{
				if (playerMetaInfo.nativeId != null)
				{
					playerEntryInfo.NativeUserId = playerMetaInfo.nativeId;
				}
				if (playerMetaInfo.name != null)
				{
					playerEntryInfo.CachedName = playerMetaInfo.name;
				}
				playerEntryInfo.PlayerLevel = playerMetaInfo.level;
				playerEntryInfo.DistanceWalked = playerMetaInfo.distanceWalked;
			}
			this.fileNameKeysToPlayerInfos[text2] = playerEntryInfo;
		}
		foreach (KeyValuePair<string, SaveInfoProvider.PlayerEntryInfo> keyValuePair in this.fileNameKeysToPlayerInfos)
		{
			long directorySize = GameIO.GetDirectorySize(new SdDirectoryInfo(Path.Combine(text, keyValuePair.Key)), true);
			keyValuePair.Value.Size += directorySize;
			this.playerEntryInfos.Add(keyValuePair.Value);
		}
		this.fileNameKeysToPlayerInfos.Clear();
	}

	// Token: 0x06009016 RID: 36886 RVA: 0x00398100 File Offset: 0x00396300
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcessRemoteWorldSaves()
	{
		string saveGameLocalRootDir = GameIO.GetSaveGameLocalRootDir();
		if (!SdDirectory.Exists(saveGameLocalRootDir))
		{
			return;
		}
		SdFileSystemInfo[] array = new SdDirectoryInfo(saveGameLocalRootDir).GetDirectories();
		foreach (SdDirectoryInfo sdDirectoryInfo in array)
		{
			string text = sdDirectoryInfo.FullName + "/RemoteWorldInfo.xml";
			SaveInfoProvider.SaveEntryInfo saveEntryInfo = new SaveInfoProvider.SaveEntryInfo
			{
				SaveDir = sdDirectoryInfo.FullName
			};
			saveEntryInfo.SizeInfo.BytesOnDisk = GameIO.GetDirectorySize(sdDirectoryInfo, true);
			saveEntryInfo.SizeInfo.IsArchived = (SaveInfoProvider.DataLimitEnabled && SdFile.Exists(Path.Combine(sdDirectoryInfo.FullName, "archived.flag")));
			RemoteWorldInfo remoteWorldInfo;
			SaveInfoProvider.WorldEntryInfo worldEntryInfo;
			if (RemoteWorldInfo.TryRead(text, out remoteWorldInfo))
			{
				string worldEntryKey = SaveInfoProvider.GetWorldEntryKey(remoteWorldInfo.worldName, SaveInfoProvider.RemoteWorldsType);
				if (!this.worldEntryInfosByWorldKey.TryGetValue(worldEntryKey, out worldEntryInfo))
				{
					worldEntryInfo = new SaveInfoProvider.WorldEntryInfo
					{
						WorldKey = worldEntryKey,
						Name = remoteWorldInfo.worldName,
						Type = SaveInfoProvider.RemoteWorldsType,
						Location = PathAbstractions.AbstractedLocation.None,
						Deletable = false,
						WorldDataSize = 0L,
						Version = remoteWorldInfo.gameVersion
					};
					this.worldEntryInfosByWorldKey[worldEntryKey] = worldEntryInfo;
				}
				if (SaveInfoProvider.DataLimitEnabled && remoteWorldInfo.saveSize != -1L)
				{
					saveEntryInfo.SizeInfo.BytesReserved = remoteWorldInfo.saveSize;
					if (saveEntryInfo.SizeInfo.BytesOnDisk > saveEntryInfo.SizeInfo.BytesReserved)
					{
						Debug.LogError(string.Format("Directory size of save \"{0}\" exceeds serialized save data size of {1}.", sdDirectoryInfo.FullName, remoteWorldInfo.saveSize));
					}
				}
				else
				{
					saveEntryInfo.SizeInfo.BytesReserved = -1L;
				}
				saveEntryInfo.Name = remoteWorldInfo.gameName;
				saveEntryInfo.WorldEntry = worldEntryInfo;
				saveEntryInfo.LastSaved = SdFile.GetLastWriteTime(text);
				saveEntryInfo.Version = remoteWorldInfo.gameVersion;
			}
			else
			{
				string worldEntryKey2 = SaveInfoProvider.GetWorldEntryKey(SaveInfoProvider.RemoteWorldsLabel, SaveInfoProvider.RemoteWorldsType);
				if (!this.worldEntryInfosByWorldKey.TryGetValue(worldEntryKey2, out worldEntryInfo))
				{
					worldEntryInfo = new SaveInfoProvider.WorldEntryInfo
					{
						WorldKey = worldEntryKey2,
						Name = SaveInfoProvider.RemoteWorldsLabel,
						Type = SaveInfoProvider.RemoteWorldsType,
						Location = PathAbstractions.AbstractedLocation.None,
						Deletable = false,
						WorldDataSize = 0L,
						Version = null
					};
					this.worldEntryInfosByWorldKey[worldEntryKey2] = worldEntryInfo;
				}
				saveEntryInfo.Name = sdDirectoryInfo.Name;
				saveEntryInfo.WorldEntry = worldEntryInfo;
				saveEntryInfo.LastSaved = sdDirectoryInfo.LastWriteTime;
				saveEntryInfo.Version = null;
			}
			this.saveEntryInfos.Add(saveEntryInfo);
			string saveEntryKey = SaveInfoProvider.GetSaveEntryKey(saveEntryInfo.WorldEntry.WorldKey, saveEntryInfo.Name);
			this.saveEntryInfosBySaveKey[saveEntryKey] = saveEntryInfo;
			this.remoteSaveEntryInfosByGuid[sdDirectoryInfo.Name] = saveEntryInfo;
			worldEntryInfo.SaveEntryInfos.Add(saveEntryInfo);
			worldEntryInfo.SaveDataCount++;
			worldEntryInfo.SaveDataSize += saveEntryInfo.SizeInfo.ReportedSize;
			this.remoteSavesSum += saveEntryInfo.SizeInfo.ReportedSize;
		}
	}

	// Token: 0x06009017 RID: 36887 RVA: 0x0039841C File Offset: 0x0039661C
	public bool TryGetLocalSaveEntry(string worldName, string saveName, out SaveInfoProvider.SaveEntryInfo saveEntryInfo)
	{
		this.RefreshIfDirty();
		string saveEntryKey = SaveInfoProvider.GetSaveEntryKey(SaveInfoProvider.GetWorldEntryKey(worldName, "Local"), saveName);
		return this.saveEntryInfosBySaveKey.TryGetValue(saveEntryKey, out saveEntryInfo);
	}

	// Token: 0x06009018 RID: 36888 RVA: 0x0039844E File Offset: 0x0039664E
	public bool TryGetRemoteSaveEntry(string guid, out SaveInfoProvider.SaveEntryInfo saveEntryInfo)
	{
		this.RefreshIfDirty();
		return this.remoteSaveEntryInfosByGuid.TryGetValue(guid, out saveEntryInfo);
	}

	// Token: 0x06009019 RID: 36889 RVA: 0x00398463 File Offset: 0x00396663
	[PublicizedFrom(EAccessModifier.Private)]
	public string NormalizePath(string path)
	{
		return Path.GetFullPath(path);
	}

	// Token: 0x0600901A RID: 36890 RVA: 0x0039846B File Offset: 0x0039666B
	public void SetDirectoryProtected(string path, bool isProtected)
	{
		if (isProtected)
		{
			this.protectedDirectories.Add(this.NormalizePath(path));
			return;
		}
		this.protectedDirectories.Remove(this.NormalizePath(path));
	}

	// Token: 0x0600901B RID: 36891 RVA: 0x00398497 File Offset: 0x00396697
	public bool IsDirectoryProtected(string path)
	{
		return this.protectedDirectories.Contains(this.NormalizePath(path));
	}

	// Token: 0x04006EED RID: 28397
	[PublicizedFrom(EAccessModifier.Private)]
	public const long BytesPerMB = 1048576L;

	// Token: 0x04006EEE RID: 28398
	public const string cLocalWorldsKey = "Local";

	// Token: 0x04006EEF RID: 28399
	public static readonly string RemoteWorldsLabel = "[" + Localization.Get("xuiDmRemoteWorlds", false) + "] ";

	// Token: 0x04006EF0 RID: 28400
	public static readonly string RemoteWorldsType = Localization.Get("xuiDmRemote", false);

	// Token: 0x04006EF1 RID: 28401
	public static readonly string DeletedWorldsType = Localization.Get("xuiDmDeleted", false);

	// Token: 0x04006EF2 RID: 28402
	public static readonly List<string> HideableWorlds = new List<string>
	{
		"Empty",
		"Playtesting"
	};

	// Token: 0x04006EF3 RID: 28403
	[PublicizedFrom(EAccessModifier.Private)]
	public static SaveInfoProvider instance;

	// Token: 0x04006EF4 RID: 28404
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, SaveInfoProvider.WorldEntryInfo> worldEntryInfosByWorldKey = new Dictionary<string, SaveInfoProvider.WorldEntryInfo>();

	// Token: 0x04006EF5 RID: 28405
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, SaveInfoProvider.SaveEntryInfo> saveEntryInfosBySaveKey = new Dictionary<string, SaveInfoProvider.SaveEntryInfo>();

	// Token: 0x04006EF6 RID: 28406
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, SaveInfoProvider.SaveEntryInfo> remoteSaveEntryInfosByGuid = new Dictionary<string, SaveInfoProvider.SaveEntryInfo>();

	// Token: 0x04006EF7 RID: 28407
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SaveInfoProvider.WorldEntryInfo> worldEntryInfos = new List<SaveInfoProvider.WorldEntryInfo>();

	// Token: 0x04006EF8 RID: 28408
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SaveInfoProvider.SaveEntryInfo> saveEntryInfos = new List<SaveInfoProvider.SaveEntryInfo>();

	// Token: 0x04006EF9 RID: 28409
	[PublicizedFrom(EAccessModifier.Private)]
	public List<SaveInfoProvider.PlayerEntryInfo> playerEntryInfos = new List<SaveInfoProvider.PlayerEntryInfo>();

	// Token: 0x04006EFA RID: 28410
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<string> protectedDirectories = new HashSet<string>();

	// Token: 0x04006EFB RID: 28411
	[PublicizedFrom(EAccessModifier.Private)]
	public long localSavesSum;

	// Token: 0x04006EFC RID: 28412
	[PublicizedFrom(EAccessModifier.Private)]
	public long remoteSavesSum;

	// Token: 0x04006EFD RID: 28413
	[PublicizedFrom(EAccessModifier.Private)]
	public long worldsSum;

	// Token: 0x04006EFE RID: 28414
	[PublicizedFrom(EAccessModifier.Private)]
	public long totalUsedBytes;

	// Token: 0x04006EFF RID: 28415
	[PublicizedFrom(EAccessModifier.Private)]
	public long totalAllowanceBytes;

	// Token: 0x04006F00 RID: 28416
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty = true;

	// Token: 0x04006F01 RID: 28417
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, SaveInfoProvider.PlayerEntryInfo> fileNameKeysToPlayerInfos = new Dictionary<string, SaveInfoProvider.PlayerEntryInfo>();

	// Token: 0x02001208 RID: 4616
	public struct SaveSizeInfo
	{
		// Token: 0x17000EF0 RID: 3824
		// (get) Token: 0x0600901E RID: 36894 RVA: 0x0039858C File Offset: 0x0039678C
		public long ReportedSize
		{
			get
			{
				if (!this.IsArchived)
				{
					return this.MaxSize;
				}
				return this.BytesOnDisk;
			}
		}

		// Token: 0x17000EF1 RID: 3825
		// (get) Token: 0x0600901F RID: 36895 RVA: 0x003985A3 File Offset: 0x003967A3
		public long MaxSize
		{
			get
			{
				return Math.Max(this.BytesOnDisk, this.BytesReserved);
			}
		}

		// Token: 0x17000EF2 RID: 3826
		// (get) Token: 0x06009020 RID: 36896 RVA: 0x003985B6 File Offset: 0x003967B6
		public bool Archivable
		{
			get
			{
				return this.BytesReserved >= this.BytesOnDisk;
			}
		}

		// Token: 0x04006F02 RID: 28418
		public long BytesOnDisk;

		// Token: 0x04006F03 RID: 28419
		public long BytesReserved;

		// Token: 0x04006F04 RID: 28420
		public bool IsArchived;
	}

	// Token: 0x02001209 RID: 4617
	public class WorldEntryInfo : IComparable
	{
		// Token: 0x06009021 RID: 36897 RVA: 0x003985CC File Offset: 0x003967CC
		public int CompareTo(object obj)
		{
			SaveInfoProvider.WorldEntryInfo worldEntryInfo = obj as SaveInfoProvider.WorldEntryInfo;
			if (worldEntryInfo != null)
			{
				return string.Compare(this.WorldKey, worldEntryInfo.WorldKey, StringComparison.OrdinalIgnoreCase);
			}
			return 1;
		}

		// Token: 0x04006F05 RID: 28421
		public string WorldKey;

		// Token: 0x04006F06 RID: 28422
		public string Name;

		// Token: 0x04006F07 RID: 28423
		public string Type;

		// Token: 0x04006F08 RID: 28424
		public PathAbstractions.AbstractedLocation Location;

		// Token: 0x04006F09 RID: 28425
		public bool Deletable;

		// Token: 0x04006F0A RID: 28426
		public long WorldDataSize;

		// Token: 0x04006F0B RID: 28427
		public VersionInformation Version;

		// Token: 0x04006F0C RID: 28428
		public long SaveDataSize;

		// Token: 0x04006F0D RID: 28429
		public int SaveDataCount;

		// Token: 0x04006F0E RID: 28430
		public long BarStartOffset;

		// Token: 0x04006F0F RID: 28431
		public bool HideIfEmpty;

		// Token: 0x04006F10 RID: 28432
		public readonly List<SaveInfoProvider.SaveEntryInfo> SaveEntryInfos = new List<SaveInfoProvider.SaveEntryInfo>();
	}

	// Token: 0x0200120A RID: 4618
	public class SaveEntryInfo : IComparable
	{
		// Token: 0x06009023 RID: 36899 RVA: 0x0039860C File Offset: 0x0039680C
		public int CompareTo(object obj)
		{
			SaveInfoProvider.SaveEntryInfo saveEntryInfo = obj as SaveInfoProvider.SaveEntryInfo;
			if (saveEntryInfo == null)
			{
				return 1;
			}
			int num = saveEntryInfo.LastSaved.CompareTo(this.LastSaved);
			if (num == 0)
			{
				return saveEntryInfo.Name.CompareTo(this.Name);
			}
			return num;
		}

		// Token: 0x04006F11 RID: 28433
		public string Name;

		// Token: 0x04006F12 RID: 28434
		public string SaveDir;

		// Token: 0x04006F13 RID: 28435
		public long Size;

		// Token: 0x04006F14 RID: 28436
		public SaveInfoProvider.SaveSizeInfo SizeInfo;

		// Token: 0x04006F15 RID: 28437
		public long BarStartOffset;

		// Token: 0x04006F16 RID: 28438
		public SaveInfoProvider.WorldEntryInfo WorldEntry;

		// Token: 0x04006F17 RID: 28439
		public DateTime LastSaved;

		// Token: 0x04006F18 RID: 28440
		public VersionInformation Version;

		// Token: 0x04006F19 RID: 28441
		public readonly List<SaveInfoProvider.PlayerEntryInfo> PlayerEntryInfos = new List<SaveInfoProvider.PlayerEntryInfo>();
	}

	// Token: 0x0200120B RID: 4619
	public class PlayerEntryInfo : IComparable
	{
		// Token: 0x17000EF3 RID: 3827
		// (get) Token: 0x06009025 RID: 36901 RVA: 0x00398660 File Offset: 0x00396860
		public string PlatformName
		{
			get
			{
				PlatformUserIdentifierAbs platformUserIdentifierAbs = this.NativeUserId ?? this.PrimaryUserId;
				return ((platformUserIdentifierAbs != null) ? platformUserIdentifierAbs.PlatformIdentifierString : null) ?? "-";
			}
		}

		// Token: 0x17000EF4 RID: 3828
		// (get) Token: 0x06009026 RID: 36902 RVA: 0x00398688 File Offset: 0x00396888
		public IPlatformUserData PlatformUserData
		{
			get
			{
				if (this.platformUserData == null && this.PrimaryUserId != null)
				{
					IPlatformUserData orCreate = PlatformUserManager.GetOrCreate(this.PrimaryUserId);
					if (this.NativeUserId != null)
					{
						orCreate.NativeId = this.NativeUserId;
					}
					this.platformUserData = orCreate;
				}
				return this.platformUserData;
			}
		}

		// Token: 0x06009027 RID: 36903 RVA: 0x003986D4 File Offset: 0x003968D4
		public int CompareTo(object obj)
		{
			SaveInfoProvider.PlayerEntryInfo playerEntryInfo = obj as SaveInfoProvider.PlayerEntryInfo;
			if (playerEntryInfo == null)
			{
				return 1;
			}
			int num = playerEntryInfo.LastPlayed.CompareTo(this.LastPlayed);
			if (num == 0)
			{
				return playerEntryInfo.CachedName.CompareTo(this.CachedName);
			}
			return num;
		}

		// Token: 0x04006F1A RID: 28442
		public string Id;

		// Token: 0x04006F1B RID: 28443
		public string CachedName;

		// Token: 0x04006F1C RID: 28444
		public PlatformUserIdentifierAbs PrimaryUserId;

		// Token: 0x04006F1D RID: 28445
		public PlatformUserIdentifierAbs NativeUserId;

		// Token: 0x04006F1E RID: 28446
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatformUserData platformUserData;

		// Token: 0x04006F1F RID: 28447
		public long Size;

		// Token: 0x04006F20 RID: 28448
		public long BarStartOffset;

		// Token: 0x04006F21 RID: 28449
		public SaveInfoProvider.SaveEntryInfo SaveEntry;

		// Token: 0x04006F22 RID: 28450
		public DateTime LastPlayed;

		// Token: 0x04006F23 RID: 28451
		public int PlayerLevel;

		// Token: 0x04006F24 RID: 28452
		public float DistanceWalked;
	}

	// Token: 0x0200120C RID: 4620
	public class PlayerEntryInfoPlatformDataResolver
	{
		// Token: 0x17000EF5 RID: 3829
		// (get) Token: 0x06009029 RID: 36905 RVA: 0x00398715 File Offset: 0x00396915
		// (set) Token: 0x0600902A RID: 36906 RVA: 0x0039871D File Offset: 0x0039691D
		public bool IsComplete { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600902B RID: 36907 RVA: 0x00398726 File Offset: 0x00396926
		[PublicizedFrom(EAccessModifier.Private)]
		public PlayerEntryInfoPlatformDataResolver(List<SaveInfoProvider.PlayerEntryInfo> playerEntries)
		{
			this.pendingPlayerEntries = playerEntries;
		}

		// Token: 0x0600902C RID: 36908 RVA: 0x00398738 File Offset: 0x00396938
		public static SaveInfoProvider.PlayerEntryInfoPlatformDataResolver StartNew(IEnumerable<SaveInfoProvider.PlayerEntryInfo> playerEntries)
		{
			List<SaveInfoProvider.PlayerEntryInfo> list = new List<SaveInfoProvider.PlayerEntryInfo>();
			List<IPlatformUserData> list2 = null;
			foreach (SaveInfoProvider.PlayerEntryInfo playerEntryInfo in playerEntries)
			{
				list.Add(playerEntryInfo);
				IPlatformUserData platformUserData = playerEntryInfo.PlatformUserData;
				if (platformUserData != null)
				{
					if (list2 == null)
					{
						list2 = new List<IPlatformUserData>();
					}
					list2.Add(platformUserData);
				}
			}
			SaveInfoProvider.PlayerEntryInfoPlatformDataResolver playerEntryInfoPlatformDataResolver = new SaveInfoProvider.PlayerEntryInfoPlatformDataResolver(list);
			if (list2 == null)
			{
				playerEntryInfoPlatformDataResolver.IsComplete = true;
				return playerEntryInfoPlatformDataResolver;
			}
			if (!PlatformUserManager.AreUsersPendingResolve(list2))
			{
				playerEntryInfoPlatformDataResolver.IsComplete = true;
				return playerEntryInfoPlatformDataResolver;
			}
			ThreadManager.StartCoroutine(playerEntryInfoPlatformDataResolver.ResolveUserData(list2));
			return playerEntryInfoPlatformDataResolver;
		}

		// Token: 0x0600902D RID: 36909 RVA: 0x003987DC File Offset: 0x003969DC
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator ResolveUserData(List<IPlatformUserData> resolvingPlatformData)
		{
			yield return PlatformUserManager.ResolveUsersDetailsCoroutine(resolvingPlatformData);
			yield return PlatformUserManager.ResolveUserBlocksCoroutine(resolvingPlatformData);
			this.IsComplete = true;
			yield break;
		}

		// Token: 0x04006F25 RID: 28453
		public readonly List<SaveInfoProvider.PlayerEntryInfo> pendingPlayerEntries;
	}
}
