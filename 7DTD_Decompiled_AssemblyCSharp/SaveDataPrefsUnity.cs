using System;
using UnityEngine;

// Token: 0x0200094C RID: 2380
public sealed class SaveDataPrefsUnity : ISaveDataPrefs
{
	// Token: 0x1700077E RID: 1918
	// (get) Token: 0x0600479D RID: 18333 RVA: 0x001C1C95 File Offset: 0x001BFE95
	public static SaveDataPrefsUnity INSTANCE
	{
		get
		{
			SaveDataPrefsUnity result;
			if ((result = SaveDataPrefsUnity.s_instance) == null)
			{
				result = (SaveDataPrefsUnity.s_instance = new SaveDataPrefsUnity());
			}
			return result;
		}
	}

	// Token: 0x0600479E RID: 18334 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveDataPrefsUnity()
	{
	}

	// Token: 0x0600479F RID: 18335 RVA: 0x001C1CAB File Offset: 0x001BFEAB
	public float GetFloat(string key, float defaultValue)
	{
		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	// Token: 0x060047A0 RID: 18336 RVA: 0x001C1CB4 File Offset: 0x001BFEB4
	public void SetFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
	}

	// Token: 0x060047A1 RID: 18337 RVA: 0x001C1CBD File Offset: 0x001BFEBD
	public int GetInt(string key, int defaultValue)
	{
		return PlayerPrefs.GetInt(key, defaultValue);
	}

	// Token: 0x060047A2 RID: 18338 RVA: 0x001C1CC6 File Offset: 0x001BFEC6
	public void SetInt(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
	}

	// Token: 0x060047A3 RID: 18339 RVA: 0x001C1CCF File Offset: 0x001BFECF
	public string GetString(string key, string defaultValue)
	{
		return PlayerPrefs.GetString(key, defaultValue);
	}

	// Token: 0x060047A4 RID: 18340 RVA: 0x001C1CD8 File Offset: 0x001BFED8
	public void SetString(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
	}

	// Token: 0x060047A5 RID: 18341 RVA: 0x001C1CE1 File Offset: 0x001BFEE1
	public bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	// Token: 0x060047A6 RID: 18342 RVA: 0x001C1CE9 File Offset: 0x001BFEE9
	public void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	// Token: 0x060047A7 RID: 18343 RVA: 0x001C1CF1 File Offset: 0x001BFEF1
	public void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	// Token: 0x060047A8 RID: 18344 RVA: 0x001C1CF8 File Offset: 0x001BFEF8
	public void Save()
	{
		PlayerPrefs.Save();
	}

	// Token: 0x060047A9 RID: 18345 RVA: 0x001C1CFF File Offset: 0x001BFEFF
	public void Load()
	{
		throw new NotSupportedException("Unity PlayerPrefs does not support explicit loading.");
	}

	// Token: 0x1700077F RID: 1919
	// (get) Token: 0x060047AA RID: 18346 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool CanLoad
	{
		get
		{
			return false;
		}
	}

	// Token: 0x040036F1 RID: 14065
	[PublicizedFrom(EAccessModifier.Private)]
	public static SaveDataPrefsUnity s_instance;
}
