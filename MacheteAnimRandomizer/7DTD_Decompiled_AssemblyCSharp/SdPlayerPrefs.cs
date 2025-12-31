using System;

// Token: 0x02000933 RID: 2355
public static class SdPlayerPrefs
{
	// Token: 0x060046D9 RID: 18137 RVA: 0x001BFAFA File Offset: 0x001BDCFA
	public static float GetFloat(string key)
	{
		return SaveDataUtils.SaveDataPrefs.GetFloat(key);
	}

	// Token: 0x060046DA RID: 18138 RVA: 0x001BFB07 File Offset: 0x001BDD07
	public static float GetFloat(string key, float defaultValue)
	{
		return SaveDataUtils.SaveDataPrefs.GetFloat(key, defaultValue);
	}

	// Token: 0x060046DB RID: 18139 RVA: 0x001BFB15 File Offset: 0x001BDD15
	public static void SetFloat(string key, float value)
	{
		SaveDataUtils.SaveDataPrefs.SetFloat(key, value);
	}

	// Token: 0x060046DC RID: 18140 RVA: 0x001BFB23 File Offset: 0x001BDD23
	public static int GetInt(string key)
	{
		return SaveDataUtils.SaveDataPrefs.GetInt(key);
	}

	// Token: 0x060046DD RID: 18141 RVA: 0x001BFB30 File Offset: 0x001BDD30
	public static int GetInt(string key, int defaultValue)
	{
		return SaveDataUtils.SaveDataPrefs.GetInt(key, defaultValue);
	}

	// Token: 0x060046DE RID: 18142 RVA: 0x001BFB3E File Offset: 0x001BDD3E
	public static void SetInt(string key, int value)
	{
		SaveDataUtils.SaveDataPrefs.SetInt(key, value);
	}

	// Token: 0x060046DF RID: 18143 RVA: 0x001BFB4C File Offset: 0x001BDD4C
	public static string GetString(string key)
	{
		return SaveDataUtils.SaveDataPrefs.GetString(key);
	}

	// Token: 0x060046E0 RID: 18144 RVA: 0x001BFB59 File Offset: 0x001BDD59
	public static string GetString(string key, string defaultValue)
	{
		return SaveDataUtils.SaveDataPrefs.GetString(key, defaultValue);
	}

	// Token: 0x060046E1 RID: 18145 RVA: 0x001BFB67 File Offset: 0x001BDD67
	public static void SetString(string key, string value)
	{
		SaveDataUtils.SaveDataPrefs.SetString(key, value);
	}

	// Token: 0x060046E2 RID: 18146 RVA: 0x001BFB75 File Offset: 0x001BDD75
	public static bool HasKey(string key)
	{
		return SaveDataUtils.SaveDataPrefs.HasKey(key);
	}

	// Token: 0x060046E3 RID: 18147 RVA: 0x001BFB82 File Offset: 0x001BDD82
	public static void DeleteKey(string key)
	{
		SaveDataUtils.SaveDataPrefs.DeleteKey(key);
	}

	// Token: 0x060046E4 RID: 18148 RVA: 0x001BFB8F File Offset: 0x001BDD8F
	public static void DeleteAll()
	{
		SaveDataUtils.SaveDataPrefs.DeleteAll();
	}

	// Token: 0x060046E5 RID: 18149 RVA: 0x001BFB9B File Offset: 0x001BDD9B
	public static void Save()
	{
		SaveDataUtils.SaveDataPrefs.Save();
	}

	// Token: 0x060046E6 RID: 18150 RVA: 0x001BFBA7 File Offset: 0x001BDDA7
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void Load()
	{
		SaveDataUtils.SaveDataPrefs.Load();
	}

	// Token: 0x1700076C RID: 1900
	// (get) Token: 0x060046E7 RID: 18151 RVA: 0x001BFBB3 File Offset: 0x001BDDB3
	public static bool CanLoad
	{
		[PublicizedFrom(EAccessModifier.Internal)]
		get
		{
			return SaveDataUtils.SaveDataPrefs.CanLoad;
		}
	}
}
