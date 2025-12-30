using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

// Token: 0x0200094D RID: 2381
public static class SaveDataUtils
{
	// Token: 0x17000780 RID: 1920
	// (get) Token: 0x060047AB RID: 18347 RVA: 0x001C1D0B File Offset: 0x001BFF0B
	// (set) Token: 0x060047AC RID: 18348 RVA: 0x001C1D12 File Offset: 0x001BFF12
	public static ISaveDataManager SaveDataManager { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000781 RID: 1921
	// (get) Token: 0x060047AD RID: 18349 RVA: 0x001C1D1A File Offset: 0x001BFF1A
	// (set) Token: 0x060047AE RID: 18350 RVA: 0x001C1D21 File Offset: 0x001BFF21
	public static ISaveDataPrefs SaveDataPrefs { get; [PublicizedFrom(EAccessModifier.Private)] set; } = SaveDataPrefsUninitialized.INSTANCE;

	// Token: 0x060047AF RID: 18351 RVA: 0x001C1D29 File Offset: 0x001BFF29
	public static IEnumerator InitStaticCoroutine()
	{
		if (SaveDataUtils.s_initStatic)
		{
			yield break;
		}
		SaveDataUtils.s_initStatic = true;
		Log.Out("[SaveDataUtils] InitStatic Begin");
		SaveDataUtils.UpdatePaths();
		SaveDataUtils.s_saveDataManager = SaveDataManager_Placeholder.Instance;
		SaveDataUtils.SaveDataManager = SaveDataUtils.s_saveDataManager;
		SaveDataUtils.SaveDataManager.Init();
		SdDirectory.CreateDirectory(GameIO.GetUserGameDataDir());
		if (LaunchPrefs.PlayerPrefsFile.Value)
		{
			Log.Out("[SaveDataUtils] SdPlayerPrefs -> SdFile");
			SaveDataUtils.SaveDataPrefs = (SaveDataUtils.s_saveDataPrefs = SaveDataPrefsFile.INSTANCE);
		}
		else
		{
			Log.Out("[SaveDataUtils] SdPlayerPrefs -> Unity");
			SaveDataUtils.SaveDataPrefs = (SaveDataUtils.s_saveDataPrefs = SaveDataPrefsUnity.INSTANCE);
		}
		Log.Out("[SaveDataUtils] InitStatic Complete");
		yield break;
	}

	// Token: 0x060047B0 RID: 18352 RVA: 0x001C1D31 File Offset: 0x001BFF31
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void SetSaveDataManagerOverride(ISaveDataManager saveDataManagerOverride)
	{
		if (saveDataManagerOverride == SaveDataUtils.s_saveDataManager)
		{
			Log.Error("SetSaveDataManagerOverride failed: Cannot override default Save Data Manager with itself.");
			return;
		}
		SaveDataUtils.SaveDataManager = saveDataManagerOverride;
	}

	// Token: 0x060047B1 RID: 18353 RVA: 0x001C1D4C File Offset: 0x001BFF4C
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void ClearSaveDataManagerOverride()
	{
		if (SaveDataUtils.SaveDataManager == SaveDataUtils.s_saveDataManager)
		{
			Log.Error("ClearSaveDataManagerOverride failed: Save Data Manager override was not set or has already been cleared.");
			return;
		}
		SaveDataUtils.SaveDataManager.Cleanup();
		SaveDataUtils.SaveDataManager = null;
		SaveDataUtils.SaveDataManager = SaveDataUtils.s_saveDataManager;
	}

	// Token: 0x060047B2 RID: 18354 RVA: 0x001C1D7F File Offset: 0x001BFF7F
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void SetSaveDataPrefsOverride(ISaveDataPrefs saveDataPrefsOverride)
	{
		SaveDataUtils.SaveDataPrefs = saveDataPrefsOverride;
	}

	// Token: 0x060047B3 RID: 18355 RVA: 0x001C1D87 File Offset: 0x001BFF87
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void ClearSaveDataPrefsOverride()
	{
		SaveDataUtils.SaveDataPrefs = SaveDataUtils.s_saveDataPrefs;
	}

	// Token: 0x060047B4 RID: 18356 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public static bool IsManaged(string path)
	{
		return false;
	}

	// Token: 0x060047B5 RID: 18357 RVA: 0x001C1D93 File Offset: 0x001BFF93
	public static bool TryGetManagedPath(string path, out SaveDataManagedPath managedPath)
	{
		managedPath = null;
		return false;
	}

	// Token: 0x060047B6 RID: 18358 RVA: 0x001C1D99 File Offset: 0x001BFF99
	public static SaveDataManagedPath GetBackupPath(SaveDataManagedPath restorePath)
	{
		return new SaveDataManagedPath(restorePath.PathRelativeToRoot + ".bup");
	}

	// Token: 0x060047B7 RID: 18359 RVA: 0x001C1DB0 File Offset: 0x001BFFB0
	public static SaveDataManagedPath GetRestorePath(SaveDataManagedPath backupPath)
	{
		string pathRelativeToRoot = backupPath.PathRelativeToRoot;
		if (!pathRelativeToRoot.EndsWith(".bup"))
		{
			throw new ArgumentException(string.Format("Expected \"{0}\" to end with \"{1}\".", backupPath, ".bup"));
		}
		return new SaveDataManagedPath(pathRelativeToRoot.AsSpan(0, pathRelativeToRoot.Length - ".bup".Length));
	}

	// Token: 0x060047B8 RID: 18360 RVA: 0x001C1E0C File Offset: 0x001C000C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void UpdatePaths()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder.Append("^(?:");
		stringBuilder2.Append("^(?:");
		string normalizedPath = GameIO.GetNormalizedPath(GameIO.GetUserGameDataDir());
		stringBuilder.Append("(?<1>");
		string value = Regex.Escape(normalizedPath);
		stringBuilder.Append(value);
		stringBuilder2.Append(value);
		stringBuilder.Append(')');
		stringBuilder.Append(')');
		stringBuilder2.Append(')');
		stringBuilder.Append("(?:$|[\\\\/](?<2>.*)$)");
		stringBuilder2.Append("(?:$|[\\\\/])");
		SaveDataUtils.s_managedPathRegex = new Regex(stringBuilder.ToString(), RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
		SaveDataUtils.s_managedPathRegexWithoutGroups = new Regex(stringBuilder.ToString(), RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
		SaveDataUtils.s_saveDataRootPathPrefix = normalizedPath;
	}

	// Token: 0x060047B9 RID: 18361 RVA: 0x001C1ECC File Offset: 0x001C00CC
	public static void Destroy()
	{
		SdPlayerPrefs.Save();
		SaveDataUtils.SaveDataManager.Cleanup();
		SaveDataUtils.SaveDataPrefs = (SaveDataUtils.s_saveDataPrefs = SaveDataPrefsUninitialized.INSTANCE);
		SaveDataUtils.SaveDataManager = (SaveDataUtils.s_saveDataManager = null);
	}

	// Token: 0x040036F2 RID: 14066
	public const string BACKUP_FILE_EXTENSION = "bup";

	// Token: 0x040036F3 RID: 14067
	public const string BACKUP_FILE_EXTENSION_WITH_DOT = ".bup";

	// Token: 0x040036F4 RID: 14068
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool s_initStatic;

	// Token: 0x040036F6 RID: 14070
	[PublicizedFrom(EAccessModifier.Private)]
	public static ISaveDataManager s_saveDataManager;

	// Token: 0x040036F7 RID: 14071
	public static string s_saveDataRootPathPrefix;

	// Token: 0x040036F9 RID: 14073
	[PublicizedFrom(EAccessModifier.Private)]
	public static ISaveDataPrefs s_saveDataPrefs = SaveDataPrefsUninitialized.INSTANCE;

	// Token: 0x040036FA RID: 14074
	[PublicizedFrom(EAccessModifier.Private)]
	public static Regex s_managedPathRegex = new Regex("$^", RegexOptions.Compiled);

	// Token: 0x040036FB RID: 14075
	[PublicizedFrom(EAccessModifier.Private)]
	public static Regex s_managedPathRegexWithoutGroups = new Regex("$^", RegexOptions.Compiled);
}
