using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02001192 RID: 4498
public static class GameIO
{
	// Token: 0x06008C86 RID: 35974 RVA: 0x00388880 File Offset: 0x00386A80
	[PublicizedFrom(EAccessModifier.Private)]
	static GameIO()
	{
		GameIO.m_UnityDataPath = Application.dataPath;
		GameIO.m_UnityRuntimePlatform = Application.platform;
	}

	// Token: 0x06008C87 RID: 35975 RVA: 0x003888E5 File Offset: 0x00386AE5
	public static SdFileInfo[] GetDirectory(string _path, string _pattern)
	{
		if (!SdDirectory.Exists(_path))
		{
			return Array.Empty<SdFileInfo>();
		}
		return new SdDirectoryInfo(_path).GetFiles(_pattern);
	}

	// Token: 0x06008C88 RID: 35976 RVA: 0x00388904 File Offset: 0x00386B04
	public static long FileSize(string _filePath)
	{
		SdFileInfo sdFileInfo = new SdFileInfo(_filePath);
		if (!sdFileInfo.Exists)
		{
			return -1L;
		}
		return sdFileInfo.Length;
	}

	// Token: 0x06008C89 RID: 35977 RVA: 0x00388929 File Offset: 0x00386B29
	public static string GetNormalizedPath(string _path)
	{
		return Path.GetFullPath(_path).TrimEnd(GameIO.pathTrimCharacters);
	}

	// Token: 0x06008C8A RID: 35978 RVA: 0x0038893B File Offset: 0x00386B3B
	public static bool PathsEquals(string _path1, string _path2, bool _ignoreCase)
	{
		return string.Equals(GameIO.GetNormalizedPath(_path1), GameIO.GetNormalizedPath(_path2), _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
	}

	// Token: 0x06008C8B RID: 35979 RVA: 0x00388958 File Offset: 0x00386B58
	public static string GetFileExtension(string _filename)
	{
		int startIndex;
		if (_filename.Length > 4 && (startIndex = _filename.LastIndexOf('.')) > 0)
		{
			return _filename.Substring(startIndex);
		}
		return _filename;
	}

	// Token: 0x06008C8C RID: 35980 RVA: 0x00388984 File Offset: 0x00386B84
	public static string RemoveFileExtension(string _filename)
	{
		int length;
		if (_filename.Length > 4 && (length = _filename.LastIndexOf('.')) > 0)
		{
			return _filename.Substring(0, length);
		}
		return _filename;
	}

	// Token: 0x06008C8D RID: 35981 RVA: 0x003889B1 File Offset: 0x00386BB1
	public static string RemoveExtension(string _filename, string _extension)
	{
		if (_filename.Length > _extension.Length && _filename.EndsWith(_extension, StringComparison.InvariantCultureIgnoreCase))
		{
			return _filename.Substring(0, _filename.Length - _extension.Length);
		}
		return _filename;
	}

	// Token: 0x06008C8E RID: 35982 RVA: 0x003889E4 File Offset: 0x00386BE4
	public static string GetFilenameFromPath(string _filepath)
	{
		int num = _filepath.LastIndexOfAny(GameIO.ResourcePathSeparators);
		if (num >= 0 && num < _filepath.Length)
		{
			_filepath = _filepath.Substring(num + 1);
		}
		return _filepath;
	}

	// Token: 0x06008C8F RID: 35983 RVA: 0x00388A18 File Offset: 0x00386C18
	public static string GetFilenameFromPathWithoutExtension(string _filepath)
	{
		int num = _filepath.LastIndexOfAny(GameIO.ResourcePathSeparators);
		int num2 = _filepath.LastIndexOf('.');
		if (num >= 0 && num2 < num)
		{
			num2 = -1;
		}
		if (num >= 0 && num2 >= 0)
		{
			return _filepath.Substring(num + 1, num2 - num - 1);
		}
		if (num >= 0)
		{
			return _filepath.Substring(num + 1);
		}
		if (num2 >= 0)
		{
			return _filepath.Substring(0, num2);
		}
		return _filepath;
	}

	// Token: 0x06008C90 RID: 35984 RVA: 0x00388A78 File Offset: 0x00386C78
	public static string GetDirectoryFromPath(string _filepath)
	{
		int num = _filepath.LastIndexOf('/');
		if (num > 0 && num < _filepath.Length)
		{
			_filepath = _filepath.Substring(0, num);
		}
		return _filepath;
	}

	// Token: 0x06008C91 RID: 35985 RVA: 0x00388AA6 File Offset: 0x00386CA6
	public static long GetDirectorySize(string _filepath, bool recursive = true)
	{
		return GameIO.GetDirectorySize(new SdDirectoryInfo(_filepath), recursive);
	}

	// Token: 0x06008C92 RID: 35986 RVA: 0x00388AB4 File Offset: 0x00386CB4
	public static long GetDirectorySize(SdDirectoryInfo directoryInfo, bool recursive = true)
	{
		long num = 0L;
		if (directoryInfo == null || !directoryInfo.Exists)
		{
			return num;
		}
		foreach (SdFileInfo sdFileInfo in directoryInfo.GetFiles())
		{
			num += sdFileInfo.Length;
		}
		if (recursive)
		{
			foreach (SdDirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
			{
				num += GameIO.GetDirectorySize(directoryInfo2, recursive);
			}
		}
		return num;
	}

	// Token: 0x06008C93 RID: 35987 RVA: 0x00388B1F File Offset: 0x00386D1F
	public static string GetGameDir(string _relDir)
	{
		return GameIO.GetApplicationPath() + "/" + _relDir;
	}

	// Token: 0x06008C94 RID: 35988 RVA: 0x00388B34 File Offset: 0x00386D34
	public static string GetApplicationPath()
	{
		if (GameIO.m_ApplicationPath == null)
		{
			string text = GameIO.m_UnityDataPath;
			RuntimePlatform unityRuntimePlatform = GameIO.m_UnityRuntimePlatform;
			if (unityRuntimePlatform <= RuntimePlatform.PS4)
			{
				if (unityRuntimePlatform - RuntimePlatform.OSXPlayer > 1)
				{
					if (unityRuntimePlatform != RuntimePlatform.PS4)
					{
						goto IL_3F;
					}
					goto IL_4B;
				}
			}
			else
			{
				if (unityRuntimePlatform == RuntimePlatform.PS5)
				{
					goto IL_4B;
				}
				if (unityRuntimePlatform - RuntimePlatform.WindowsServer > 1)
				{
					goto IL_3F;
				}
			}
			text += "/..";
			goto IL_4B;
			IL_3F:
			text += "/..";
			IL_4B:
			GameIO.m_ApplicationPath = text;
		}
		return GameIO.m_ApplicationPath;
	}

	// Token: 0x06008C95 RID: 35989 RVA: 0x00388B97 File Offset: 0x00386D97
	public static string GetGamePath()
	{
		if (GameIO.m_UnityRuntimePlatform != RuntimePlatform.OSXPlayer && GameIO.m_UnityRuntimePlatform != RuntimePlatform.OSXServer)
		{
			return GameIO.m_UnityDataPath + "/..";
		}
		return GameIO.m_UnityDataPath + "/../..";
	}

	// Token: 0x06008C96 RID: 35990 RVA: 0x00388BC9 File Offset: 0x00386DC9
	public static string GetGameExecutablePath()
	{
		return GameIO.GetGamePath() + "/" + GameIO.GetGameExecutableName();
	}

	// Token: 0x06008C97 RID: 35991 RVA: 0x00388BE0 File Offset: 0x00386DE0
	public static string GetGameExecutableName()
	{
		RuntimePlatform unityRuntimePlatform = GameIO.m_UnityRuntimePlatform;
		if (unityRuntimePlatform <= RuntimePlatform.LinuxPlayer)
		{
			switch (unityRuntimePlatform)
			{
			case RuntimePlatform.OSXEditor:
				return "7DaysToDie.app";
			case RuntimePlatform.OSXPlayer:
				return "7DaysToDie.app";
			case RuntimePlatform.WindowsPlayer:
				return "7DaysToDie.exe";
			default:
				if (unityRuntimePlatform == RuntimePlatform.WindowsEditor)
				{
					return "7DaysToDie.exe";
				}
				if (unityRuntimePlatform == RuntimePlatform.LinuxPlayer)
				{
					return "7DaysToDie.x86_64";
				}
				break;
			}
		}
		else
		{
			if (unityRuntimePlatform == RuntimePlatform.LinuxEditor)
			{
				return "7DaysToDie.x86_64";
			}
			if (unityRuntimePlatform == RuntimePlatform.XboxOne)
			{
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			}
			switch (unityRuntimePlatform)
			{
			case RuntimePlatform.GameCoreXboxSeries:
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			case RuntimePlatform.GameCoreXboxOne:
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			case RuntimePlatform.PS5:
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			}
		}
		throw new ArgumentOutOfRangeException("m_UnityRuntimePlatform");
	}

	// Token: 0x06008C98 RID: 35992 RVA: 0x00388D1A File Offset: 0x00386F1A
	public static string GetLauncherExecutablePath()
	{
		return GameIO.GetGamePath() + "/" + GameIO.GetLauncherExecutableName();
	}

	// Token: 0x06008C99 RID: 35993 RVA: 0x00388D30 File Offset: 0x00386F30
	public static string GetLauncherExecutableName()
	{
		RuntimePlatform unityRuntimePlatform = GameIO.m_UnityRuntimePlatform;
		if (unityRuntimePlatform <= RuntimePlatform.LinuxPlayer)
		{
			switch (unityRuntimePlatform)
			{
			case RuntimePlatform.OSXEditor:
				return "7dLauncher.app";
			case RuntimePlatform.OSXPlayer:
				return "7dLauncher.app";
			case RuntimePlatform.WindowsPlayer:
				return "7dLauncher.exe";
			default:
				if (unityRuntimePlatform == RuntimePlatform.WindowsEditor)
				{
					return "7dLauncher.exe";
				}
				if (unityRuntimePlatform == RuntimePlatform.LinuxPlayer)
				{
					return "7DaysToDie.sh";
				}
				break;
			}
		}
		else
		{
			if (unityRuntimePlatform == RuntimePlatform.LinuxEditor)
			{
				return "7DaysToDie.sh";
			}
			if (unityRuntimePlatform == RuntimePlatform.XboxOne)
			{
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			}
			switch (unityRuntimePlatform)
			{
			case RuntimePlatform.GameCoreXboxSeries:
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			case RuntimePlatform.GameCoreXboxOne:
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			case RuntimePlatform.PS5:
				throw new ArgumentException("Platform " + GameIO.m_UnityRuntimePlatform.ToStringCached<RuntimePlatform>() + " currently not supported", "m_UnityRuntimePlatform");
			}
		}
		throw new ArgumentOutOfRangeException("m_UnityRuntimePlatform");
	}

	// Token: 0x06008C9A RID: 35994 RVA: 0x00388E6C File Offset: 0x0038706C
	public static string GetApplicationScratchPath()
	{
		if (GameIO.m_ApplicationScratchPath == null)
		{
			RuntimePlatform unityRuntimePlatform = GameIO.m_UnityRuntimePlatform;
			if (unityRuntimePlatform <= RuntimePlatform.XboxOne)
			{
				if (unityRuntimePlatform != RuntimePlatform.PS4)
				{
					if (unityRuntimePlatform != RuntimePlatform.XboxOne)
					{
						goto IL_50;
					}
					GameIO.m_ApplicationScratchPath = "D:";
					goto IL_5A;
				}
			}
			else
			{
				if (unityRuntimePlatform - RuntimePlatform.GameCoreXboxSeries <= 1)
				{
					GameIO.m_ApplicationScratchPath = "D:";
					goto IL_5A;
				}
				if (unityRuntimePlatform != RuntimePlatform.PS5)
				{
					goto IL_50;
				}
			}
			GameIO.m_ApplicationScratchPath = "/hostapp";
			goto IL_5A;
			IL_50:
			GameIO.m_ApplicationScratchPath = GameIO.GetApplicationPath();
		}
		IL_5A:
		return GameIO.m_ApplicationScratchPath;
	}

	// Token: 0x06008C9B RID: 35995 RVA: 0x00388ED8 File Offset: 0x003870D8
	public static string GetApplicationTempPath()
	{
		if (GameIO.m_ApplicationTempPath == null)
		{
			RuntimePlatform unityRuntimePlatform = GameIO.m_UnityRuntimePlatform;
			if (unityRuntimePlatform != RuntimePlatform.PS4)
			{
				if (unityRuntimePlatform == RuntimePlatform.XboxOne)
				{
					GameIO.m_ApplicationTempPath = "T:\\";
					goto IL_68;
				}
				switch (unityRuntimePlatform)
				{
				case RuntimePlatform.GameCoreXboxSeries:
					GameIO.m_ApplicationTempPath = "T:\\";
					goto IL_68;
				case RuntimePlatform.GameCoreXboxOne:
					GameIO.m_ApplicationTempPath = "T:\\";
					goto IL_68;
				case RuntimePlatform.PS5:
					break;
				default:
					GameIO.m_ApplicationTempPath = GameIO.GetApplicationPath();
					goto IL_68;
				}
			}
			GameIO.m_ApplicationTempPath = "/temp0";
		}
		IL_68:
		return GameIO.m_ApplicationTempPath;
	}

	// Token: 0x06008C9C RID: 35996 RVA: 0x00388F52 File Offset: 0x00387152
	public static IEnumerator PrecacheFile(string _path, int _doYieldEveryMs = -1, Action<float, long, long> _statusUpdateHandler = null)
	{
		if (!SdFile.Exists(_path))
		{
			Log.Error("File does not exist: " + _path);
			yield break;
		}
		Stream fs;
		try
		{
			fs = SdFile.OpenRead(_path);
		}
		catch (Exception e)
		{
			Log.Error("Precaching file failed");
			Log.Exception(e);
			yield break;
		}
		byte[] buf = new byte[16384];
		MicroStopwatch msw = new MicroStopwatch();
		int num;
		do
		{
			if (_doYieldEveryMs > 0 && msw.ElapsedMilliseconds >= (long)_doYieldEveryMs)
			{
				if (_statusUpdateHandler != null)
				{
					_statusUpdateHandler((float)fs.Position / (float)fs.Length, fs.Position, fs.Length);
				}
				yield return null;
				msw.ResetAndRestart();
			}
			try
			{
				num = fs.Read(buf, 0, buf.Length);
			}
			catch (Exception e2)
			{
				Log.Error("Precaching file failed");
				Log.Exception(e2);
				try
				{
					fs.Dispose();
				}
				catch (Exception e3)
				{
					Log.Error("Failed disposing filestream");
					Log.Exception(e3);
				}
				yield break;
			}
		}
		while (num > 0);
		fs.Dispose();
		yield break;
	}

	// Token: 0x06008C9D RID: 35997 RVA: 0x00388F70 File Offset: 0x00387170
	public static string GetDocumentPath()
	{
		RuntimePlatform unityRuntimePlatform = GameIO.m_UnityRuntimePlatform;
		if (unityRuntimePlatform <= RuntimePlatform.LinuxPlayer)
		{
			if (unityRuntimePlatform <= RuntimePlatform.WindowsPlayer)
			{
				if (unityRuntimePlatform <= RuntimePlatform.OSXPlayer)
				{
					goto IL_83;
				}
				if (unityRuntimePlatform != RuntimePlatform.WindowsPlayer)
				{
					goto IL_B3;
				}
				goto IL_7B;
			}
			else
			{
				if (unityRuntimePlatform == RuntimePlatform.WindowsEditor)
				{
					goto IL_7B;
				}
				if (unityRuntimePlatform != RuntimePlatform.LinuxPlayer)
				{
					goto IL_B3;
				}
			}
		}
		else if (unityRuntimePlatform <= RuntimePlatform.PS4)
		{
			if (unityRuntimePlatform != RuntimePlatform.LinuxEditor)
			{
				if (unityRuntimePlatform != RuntimePlatform.PS4)
				{
					goto IL_B3;
				}
				return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
		}
		else
		{
			if (unityRuntimePlatform == RuntimePlatform.XboxOne)
			{
				UnityEngine.Debug.LogWarning("XboxOne: Platform Document Path is currently not set");
				return null;
			}
			switch (unityRuntimePlatform)
			{
			case RuntimePlatform.GameCoreXboxSeries:
			case RuntimePlatform.GameCoreXboxOne:
				return Application.persistentDataPath;
			case RuntimePlatform.PS5:
				return "/download0";
			case RuntimePlatform.EmbeddedLinuxArm64:
			case RuntimePlatform.EmbeddedLinuxArm32:
			case RuntimePlatform.EmbeddedLinuxX64:
			case RuntimePlatform.EmbeddedLinuxX86:
				goto IL_B3;
			case RuntimePlatform.LinuxServer:
				break;
			case RuntimePlatform.WindowsServer:
				goto IL_7B;
			case RuntimePlatform.OSXServer:
				goto IL_83;
			default:
				goto IL_B3;
			}
		}
		return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		IL_7B:
		return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		IL_83:
		return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Library/Application Support";
		IL_B3:
		return null;
	}

	// Token: 0x06008C9E RID: 35998 RVA: 0x00389031 File Offset: 0x00387231
	public static string GetDefaultUserGameDataDir()
	{
		return GameIO.GetDocumentPath() + "/" + "7 Days To Die".Replace(" ", "");
	}

	// Token: 0x06008C9F RID: 35999 RVA: 0x00389056 File Offset: 0x00387256
	public static string GetUserGameDataDir()
	{
		return LaunchPrefs.UserDataFolder.Value;
	}

	// Token: 0x06008CA0 RID: 36000 RVA: 0x00389064 File Offset: 0x00387264
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdateUserDataFolderDependentPaths()
	{
		string userGameDataDir = GameIO.GetUserGameDataDir();
		if (GameIO.m_LastUserDataFolder == userGameDataDir)
		{
			return;
		}
		object cachedUserDataFolderDependentLock = GameIO.m_CachedUserDataFolderDependentLock;
		lock (cachedUserDataFolderDependentLock)
		{
			if (!(GameIO.m_LastUserDataFolder == userGameDataDir))
			{
				GameIO.m_CachedSaveGameRootDir = Path.Combine(userGameDataDir, "Saves");
				GameIO.m_CachedSaveGameLocalRootDir = Path.Combine(userGameDataDir, "SavesLocal");
				GameIO.m_LastUserDataFolder = userGameDataDir;
			}
		}
	}

	// Token: 0x06008CA1 RID: 36001 RVA: 0x003890E8 File Offset: 0x003872E8
	public static string GetSaveGameRootDir()
	{
		GameIO.UpdateUserDataFolderDependentPaths();
		return GameIO.m_CachedSaveGameRootDir;
	}

	// Token: 0x06008CA2 RID: 36002 RVA: 0x003890F4 File Offset: 0x003872F4
	public static string GetSaveGameDir(string _worldName)
	{
		return GameIO.GetSaveGameRootDir() + "/" + _worldName;
	}

	// Token: 0x06008CA3 RID: 36003 RVA: 0x00389106 File Offset: 0x00387306
	public static string GetSaveGameDir(string _worldName, string _gameName)
	{
		return GameIO.GetSaveGameDir(_worldName) + "/" + _gameName;
	}

	// Token: 0x06008CA4 RID: 36004 RVA: 0x00389119 File Offset: 0x00387319
	public static string GetSaveGameDir()
	{
		return GameIO.GetSaveGameDir(GamePrefs.GetString(EnumGamePrefs.GameWorld), GamePrefs.GetString(EnumGamePrefs.GameName));
	}

	// Token: 0x06008CA5 RID: 36005 RVA: 0x0038912E File Offset: 0x0038732E
	public static string GetSaveGameLocalRootDir()
	{
		GameIO.UpdateUserDataFolderDependentPaths();
		return GameIO.m_CachedSaveGameLocalRootDir;
	}

	// Token: 0x06008CA6 RID: 36006 RVA: 0x0038913C File Offset: 0x0038733C
	public static string GetSaveGameLocalDir()
	{
		string @string = GamePrefs.GetString(EnumGamePrefs.GameGuidClient);
		if (string.IsNullOrEmpty(@string))
		{
			throw new Exception("Accessing GetSaveGameLocalDir while GameGuidClient is not yet set!");
		}
		return GameIO.GetSaveGameLocalRootDir() + "/" + @string;
	}

	// Token: 0x06008CA7 RID: 36007 RVA: 0x00389177 File Offset: 0x00387377
	public static string GetPlayerDataDir()
	{
		return Path.Combine(GameIO.GetSaveGameDir(), "Player");
	}

	// Token: 0x06008CA8 RID: 36008 RVA: 0x00389188 File Offset: 0x00387388
	public static string GetPlayerDataLocalDir()
	{
		return Path.Combine(GameIO.GetSaveGameLocalDir(), "Player");
	}

	// Token: 0x06008CA9 RID: 36009 RVA: 0x0038919C File Offset: 0x0038739C
	public static int GetPlayerSaves(GameIO.FoundSave _foundSave = null, bool includeArchived = false)
	{
		int num = 0;
		string saveGameRootDir = GameIO.GetSaveGameRootDir();
		if (!SdDirectory.Exists(saveGameRootDir))
		{
			return 0;
		}
		SdFileSystemInfo[] array = new SdDirectoryInfo(saveGameRootDir).GetDirectories();
		foreach (SdDirectoryInfo sdDirectoryInfo in array)
		{
			string fullName = sdDirectoryInfo.FullName;
			if (SdDirectory.Exists(fullName))
			{
				SdFileSystemInfo[] array2 = new SdDirectoryInfo(fullName).GetDirectories();
				foreach (SdDirectoryInfo sdDirectoryInfo2 in array2)
				{
					if (!sdDirectoryInfo2.Name.Contains("#"))
					{
						bool flag = SdFile.Exists(Path.Combine(sdDirectoryInfo2.FullName, "archived.flag"));
						if (includeArchived || !flag)
						{
							string text = sdDirectoryInfo2.FullName + "/main.ttw";
							if (SdFile.Exists(text))
							{
								try
								{
									WorldState worldState = new WorldState();
									worldState.Load(text, false, false, false);
									if (worldState.gameVersion != null)
									{
										if (_foundSave != null)
										{
											_foundSave(sdDirectoryInfo2.Name, sdDirectoryInfo.Name, SdFile.GetLastWriteTime(text), worldState, flag);
										}
										num++;
									}
								}
								catch (Exception ex)
								{
									Log.Warning("Error reading header of level '" + text + "'. Ignoring. Msg: " + ex.Message);
								}
							}
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06008CAA RID: 36010 RVA: 0x00389300 File Offset: 0x00387500
	public static string GetSaveGameRegionDir()
	{
		return Path.Combine(GameIO.GetSaveGameDir(), "Region");
	}

	// Token: 0x06008CAB RID: 36011 RVA: 0x00389314 File Offset: 0x00387514
	public static string GetSaveGameRegionDirDefault(string _levelName)
	{
		return PathAbstractions.WorldsSearchPaths.GetLocation(_levelName, null, null).FullPath + "/Region";
	}

	// Token: 0x06008CAC RID: 36012 RVA: 0x00389340 File Offset: 0x00387540
	public static string GetSaveGameRegionDirDefault()
	{
		return GameIO.GetSaveGameRegionDirDefault(GamePrefs.GetString(EnumGamePrefs.GameWorld));
	}

	// Token: 0x06008CAD RID: 36013 RVA: 0x00389350 File Offset: 0x00387550
	public static string GetWorldDir()
	{
		return PathAbstractions.WorldsSearchPaths.GetLocation(GamePrefs.GetString(EnumGamePrefs.GameWorld), null, null).FullPath;
	}

	// Token: 0x06008CAE RID: 36014 RVA: 0x00389378 File Offset: 0x00387578
	public static string GetWorldDir(string _worldName)
	{
		PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(_worldName, null, null);
		if (location.Type != PathAbstractions.EAbstractedLocationType.None)
		{
			return location.FullPath;
		}
		return null;
	}

	// Token: 0x06008CAF RID: 36015 RVA: 0x003893A5 File Offset: 0x003875A5
	public static bool DoesWorldExist(string _worldName)
	{
		return GameIO.GetWorldDir(_worldName) != null;
	}

	// Token: 0x06008CB0 RID: 36016 RVA: 0x003893B0 File Offset: 0x003875B0
	public static bool IsWorldGenerated(string _worldName)
	{
		return SdDirectory.Exists(Path.Combine(GameIO.GetUserGameDataDir(), "GeneratedWorlds", _worldName));
	}

	// Token: 0x06008CB1 RID: 36017 RVA: 0x003893C8 File Offset: 0x003875C8
	public static bool IsAbsolutePath(string _path)
	{
		RuntimePlatform unityRuntimePlatform = GameIO.m_UnityRuntimePlatform;
		switch (unityRuntimePlatform)
		{
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.IPhonePlayer:
			break;
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			goto IL_E8;
		case RuntimePlatform.OSXWebPlayer:
		case RuntimePlatform.OSXDashboardPlayer:
		case RuntimePlatform.WindowsWebPlayer:
		case (RuntimePlatform)6:
			goto IL_13F;
		default:
			switch (unityRuntimePlatform)
			{
			case RuntimePlatform.Android:
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				break;
			case RuntimePlatform.NaCl:
			case (RuntimePlatform)14:
			case RuntimePlatform.FlashPlayer:
			case RuntimePlatform.WebGLPlayer:
				goto IL_13F;
			default:
				switch (unityRuntimePlatform)
				{
				case RuntimePlatform.PS4:
				case RuntimePlatform.PS5:
				case RuntimePlatform.LinuxServer:
				case RuntimePlatform.OSXServer:
					break;
				case RuntimePlatform.PSM:
				case RuntimePlatform.SamsungTVPlayer:
				case (RuntimePlatform)29:
				case RuntimePlatform.WiiU:
				case RuntimePlatform.tvOS:
				case RuntimePlatform.Switch:
				case RuntimePlatform.Lumin:
				case RuntimePlatform.Stadia:
				case RuntimePlatform.CloudRendering:
				case RuntimePlatform.EmbeddedLinuxArm64:
				case RuntimePlatform.EmbeddedLinuxArm32:
				case RuntimePlatform.EmbeddedLinuxX64:
				case RuntimePlatform.EmbeddedLinuxX86:
					goto IL_13F;
				case RuntimePlatform.XboxOne:
				case RuntimePlatform.GameCoreXboxSeries:
				case RuntimePlatform.GameCoreXboxOne:
				case RuntimePlatform.WindowsServer:
					goto IL_E8;
				default:
					goto IL_13F;
				}
				break;
			}
			break;
		}
		return _path[0] == '/' || _path[0] == '\\' || _path.StartsWith("~/") || _path.StartsWith("~\\");
		IL_E8:
		return _path[1] == ':' && (_path[2] == '/' || _path[2] == '\\') && ((_path[0] >= 'A' && _path[0] <= 'Z') || (_path[0] >= 'a' && _path[0] <= 'z'));
		IL_13F:
		throw new ArgumentOutOfRangeException("_path", _path, "Unsupported platform");
	}

	// Token: 0x06008CB2 RID: 36018 RVA: 0x00389524 File Offset: 0x00387724
	public static string MakeAbsolutePath(string _path)
	{
		if (GameIO.IsAbsolutePath(_path))
		{
			return _path;
		}
		return GameIO.GetGamePath() + "/" + _path;
	}

	// Token: 0x06008CB3 RID: 36019 RVA: 0x00389540 File Offset: 0x00387740
	public static string GetOsStylePath(string _path)
	{
		if (GameIO.m_UnityRuntimePlatform != RuntimePlatform.WindowsPlayer && GameIO.m_UnityRuntimePlatform != RuntimePlatform.WindowsServer)
		{
			return _path.Replace("\\", "/");
		}
		return _path.Replace("/", "\\");
	}

	// Token: 0x06008CB4 RID: 36020 RVA: 0x00389574 File Offset: 0x00387774
	public static void CopyDirectory(string _sourceDirectory, string _targetDirectory)
	{
		SdDirectoryInfo source = new SdDirectoryInfo(_sourceDirectory);
		SdDirectoryInfo target = new SdDirectoryInfo(_targetDirectory);
		GameIO.CopyAll(source, target);
	}

	// Token: 0x06008CB5 RID: 36021 RVA: 0x00389594 File Offset: 0x00387794
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CopyAll(SdDirectoryInfo _source, SdDirectoryInfo _target)
	{
		SdDirectory.CreateDirectory(_target.FullName);
		foreach (SdFileInfo sdFileInfo in _source.GetFiles())
		{
			sdFileInfo.CopyTo(Path.Combine(_target.FullName, sdFileInfo.Name), true);
		}
		foreach (SdDirectoryInfo sdDirectoryInfo in _source.GetDirectories())
		{
			SdDirectoryInfo target = _target.CreateSubdirectory(sdDirectoryInfo.Name);
			GameIO.CopyAll(sdDirectoryInfo, target);
		}
	}

	// Token: 0x06008CB6 RID: 36022 RVA: 0x00389614 File Offset: 0x00387814
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OpenExplorerOnLinux(string _path)
	{
		_path = _path.Replace("\\", "/");
		if (SdFile.Exists(_path))
		{
			_path = Path.GetDirectoryName(_path);
		}
		if (_path.IndexOf(' ') >= 0)
		{
			_path = "\"" + _path + "\"";
		}
		try
		{
			Process.Start("xdg-open", _path);
		}
		catch (Exception e)
		{
			Log.Error("Failed opening file browser:");
			Log.Exception(e);
		}
	}

	// Token: 0x06008CB7 RID: 36023 RVA: 0x00389690 File Offset: 0x00387890
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OpenExplorerOnMac(string _path)
	{
		_path = _path.Replace("\\", "/");
		bool flag = SdDirectory.Exists(_path);
		if (!_path.StartsWith("\""))
		{
			_path = "\"" + _path;
		}
		if (!_path.EndsWith("\""))
		{
			_path += "\"";
		}
		try
		{
			Process.Start("open", (flag ? "" : "-R ") + _path);
		}
		catch (Exception e)
		{
			Log.Error("Failed opening Finder:");
			Log.Exception(e);
		}
	}

	// Token: 0x06008CB8 RID: 36024 RVA: 0x00389730 File Offset: 0x00387930
	[PublicizedFrom(EAccessModifier.Private)]
	public static void OpenExplorerOnWin(string _path)
	{
		_path = _path.Replace("/", "\\");
		bool flag = SdDirectory.Exists(_path);
		try
		{
			Process.Start("explorer.exe", (flag ? "/root,\"" : "/select,\"") + _path + "\"");
		}
		catch (Exception e)
		{
			Log.Error("Failed opening Explorer:");
			Log.Exception(e);
		}
	}

	// Token: 0x06008CB9 RID: 36025 RVA: 0x003897A0 File Offset: 0x003879A0
	public static void OpenExplorer(string _path)
	{
		RuntimePlatform platform = Application.platform;
		if (platform > RuntimePlatform.WindowsEditor)
		{
			if (platform != RuntimePlatform.LinuxPlayer && platform != RuntimePlatform.LinuxEditor)
			{
				switch (platform)
				{
				case RuntimePlatform.LinuxServer:
					break;
				case RuntimePlatform.WindowsServer:
					goto IL_39;
				case RuntimePlatform.OSXServer:
					goto IL_40;
				default:
					goto IL_4E;
				}
			}
			GameIO.OpenExplorerOnLinux(_path);
			return;
		}
		if (platform <= RuntimePlatform.OSXPlayer)
		{
			goto IL_40;
		}
		if (platform != RuntimePlatform.WindowsPlayer && platform != RuntimePlatform.WindowsEditor)
		{
			goto IL_4E;
		}
		IL_39:
		GameIO.OpenExplorerOnWin(_path);
		return;
		IL_40:
		GameIO.OpenExplorerOnMac(_path);
		return;
		IL_4E:
		Log.Error("Failed opening file browser: Unsupported OS");
	}

	// Token: 0x06008CBA RID: 36026 RVA: 0x00389808 File Offset: 0x00387A08
	public static string IsRunningAsSnap()
	{
		if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Linux)
		{
			return null;
		}
		string environmentVariable = Environment.GetEnvironmentVariable("SNAP_NAME");
		if (environmentVariable == null)
		{
			Log.Out("Snap detection: Not running as Snap (no SNAP_NAME environment variable)");
			return null;
		}
		Log.Out("Snap detection: Running as Snap (Snap package: '" + environmentVariable + "')");
		return environmentVariable;
	}

	// Token: 0x06008CBB RID: 36027 RVA: 0x00389850 File Offset: 0x00387A50
	public static bool IsRunningInSteamRuntime()
	{
		if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Linux)
		{
			return false;
		}
		string text = "/etc/os-release";
		if (!SdFile.Exists(text))
		{
			Log.Out("SteamRuntime detection: Linux OS file " + text + " does not exist");
			return false;
		}
		foreach (string input in SdFile.ReadAllLines(text))
		{
			Match match = GameIO.linuxOsReleaseMatcher.Match(input);
			if (match.Success)
			{
				string value = match.Groups[1].Value;
				bool flag = value.EqualsCaseInsensitive("steamrt");
				Log.Out(string.Format("SteamRuntime detection: OS ID='{0}', is SteamRT={1}", value, flag));
				return flag;
			}
		}
		Log.Out("SteamRuntime detection: No ID line matched");
		return false;
	}

	// Token: 0x04006D52 RID: 27986
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_ApplicationScratchPath;

	// Token: 0x04006D53 RID: 27987
	[PublicizedFrom(EAccessModifier.Private)]
	public const string XB1ScratchPath = "D:";

	// Token: 0x04006D54 RID: 27988
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PS4ScratchPath = "/hostapp";

	// Token: 0x04006D55 RID: 27989
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_ApplicationTempPath;

	// Token: 0x04006D56 RID: 27990
	[PublicizedFrom(EAccessModifier.Private)]
	public const string XB1TempPath = "T:\\";

	// Token: 0x04006D57 RID: 27991
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PS4TempPath = "/temp0";

	// Token: 0x04006D58 RID: 27992
	[PublicizedFrom(EAccessModifier.Private)]
	public static RuntimePlatform m_UnityRuntimePlatform;

	// Token: 0x04006D59 RID: 27993
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_UnityDataPath;

	// Token: 0x04006D5A RID: 27994
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_ApplicationPath;

	// Token: 0x04006D5B RID: 27995
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_LastUserDataFolder;

	// Token: 0x04006D5C RID: 27996
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object m_CachedUserDataFolderDependentLock = new object();

	// Token: 0x04006D5D RID: 27997
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_CachedSaveGameRootDir;

	// Token: 0x04006D5E RID: 27998
	[PublicizedFrom(EAccessModifier.Private)]
	public static string m_CachedSaveGameLocalRootDir;

	// Token: 0x04006D5F RID: 27999
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] pathTrimCharacters = new char[]
	{
		'/',
		'\\'
	};

	// Token: 0x04006D60 RID: 28000
	public static readonly char[] ResourcePathSeparators = new char[]
	{
		'/',
		'\\',
		'?'
	};

	// Token: 0x04006D61 RID: 28001
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex linuxOsReleaseMatcher = new Regex("^ID=['\"]?([^'\"]+)['\"]?$");

	// Token: 0x02001193 RID: 4499
	// (Invoke) Token: 0x06008CBD RID: 36029
	public delegate void FoundSave(string saveName, string worldName, DateTime lastSaved, WorldState worldState, bool isArchived);
}
