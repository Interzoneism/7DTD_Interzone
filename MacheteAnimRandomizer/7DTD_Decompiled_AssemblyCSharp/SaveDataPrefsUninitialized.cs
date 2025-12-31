using System;

// Token: 0x0200094B RID: 2379
public sealed class SaveDataPrefsUninitialized : ISaveDataPrefs
{
	// Token: 0x0600478F RID: 18319 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveDataPrefsUninitialized()
	{
	}

	// Token: 0x06004790 RID: 18320 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public float GetFloat(string key, float defaultValue)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004791 RID: 18321 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public void SetFloat(string key, float value)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004792 RID: 18322 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public int GetInt(string key, int defaultValue)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004793 RID: 18323 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public void SetInt(string key, int value)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004794 RID: 18324 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public string GetString(string key, string defaultValue)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004795 RID: 18325 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public void SetString(string key, string value)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004796 RID: 18326 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public bool HasKey(string key)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004797 RID: 18327 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public void DeleteKey(string key)
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004798 RID: 18328 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public void DeleteAll()
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x06004799 RID: 18329 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public void Save()
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x0600479A RID: 18330 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public void Load()
	{
		throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
	}

	// Token: 0x1700077D RID: 1917
	// (get) Token: 0x0600479B RID: 18331 RVA: 0x001C1C7D File Offset: 0x001BFE7D
	public bool CanLoad
	{
		get
		{
			throw new NotSupportedException("SdPlayerPrefs is being accessed while the pref instance is not initialized.");
		}
	}

	// Token: 0x040036EF RID: 14063
	public static readonly SaveDataPrefsUninitialized INSTANCE = new SaveDataPrefsUninitialized();

	// Token: 0x040036F0 RID: 14064
	[PublicizedFrom(EAccessModifier.Private)]
	public const string ERROR_MESSAGE = "SdPlayerPrefs is being accessed while the pref instance is not initialized.";
}
