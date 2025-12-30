using System;

// Token: 0x02000937 RID: 2359
public static class SaveDataLimit
{
	// Token: 0x060046EC RID: 18156 RVA: 0x001BFCA8 File Offset: 0x001BDEA8
	public static void SetLimitToPref(long limit)
	{
		GamePrefs.Set(EnumGamePrefs.SaveDataLimit, SaveDataLimit.ToPrefValue(limit));
	}

	// Token: 0x060046ED RID: 18157 RVA: 0x001BFCBA File Offset: 0x001BDEBA
	public static long GetLimitFromPref()
	{
		return SaveDataLimit.FromPrefValue(GamePrefs.GetInt(EnumGamePrefs.SaveDataLimit));
	}

	// Token: 0x060046EE RID: 18158 RVA: 0x001BFCCC File Offset: 0x001BDECC
	[PublicizedFrom(EAccessModifier.Private)]
	public static int ToPrefValue(long limit)
	{
		if (limit >= 0L)
		{
			return (int)((limit + 1048576L - 1L) / 1048576L);
		}
		if (PlatformOptimizations.LimitedSaveData)
		{
			Log.Warning(string.Format("[{0}] Expected finite save data limit for {1}, but was: {2}", "SaveDataLimit", "ToPrefValue", limit));
		}
		return -1;
	}

	// Token: 0x060046EF RID: 18159 RVA: 0x001BFD19 File Offset: 0x001BDF19
	[PublicizedFrom(EAccessModifier.Private)]
	public static long FromPrefValue(int limit)
	{
		if (limit >= 0)
		{
			return (long)(limit * 1048576);
		}
		if (PlatformOptimizations.LimitedSaveData)
		{
			Log.Warning(string.Format("[{0}] Expected finite save data limit for {1}, but was: {2}", "SaveDataLimit", "FromPrefValue", limit));
		}
		return -1L;
	}

	// Token: 0x040036A5 RID: 13989
	public const int SAVE_DATA_LIMIT_DISABLED = -1;

	// Token: 0x040036A6 RID: 13990
	[PublicizedFrom(EAccessModifier.Private)]
	public const int SAVE_DATA_LIMIT_PREF_BYTES_PER = 1048576;
}
