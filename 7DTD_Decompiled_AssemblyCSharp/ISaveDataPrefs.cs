using System;

// Token: 0x02000928 RID: 2344
public interface ISaveDataPrefs
{
	// Token: 0x060045FF RID: 17919 RVA: 0x001BDFE7 File Offset: 0x001BC1E7
	float GetFloat(string key)
	{
		return this.GetFloat(key, 0f);
	}

	// Token: 0x06004600 RID: 17920
	float GetFloat(string key, float defaultValue);

	// Token: 0x06004601 RID: 17921
	void SetFloat(string key, float value);

	// Token: 0x06004602 RID: 17922 RVA: 0x001BDFF5 File Offset: 0x001BC1F5
	int GetInt(string key)
	{
		return this.GetInt(key, 0);
	}

	// Token: 0x06004603 RID: 17923
	int GetInt(string key, int defaultValue);

	// Token: 0x06004604 RID: 17924
	void SetInt(string key, int value);

	// Token: 0x06004605 RID: 17925 RVA: 0x001BDFFF File Offset: 0x001BC1FF
	string GetString(string key)
	{
		return this.GetString(key, "");
	}

	// Token: 0x06004606 RID: 17926
	string GetString(string key, string defaultValue);

	// Token: 0x06004607 RID: 17927
	void SetString(string key, string value);

	// Token: 0x06004608 RID: 17928
	bool HasKey(string key);

	// Token: 0x06004609 RID: 17929
	void DeleteKey(string key);

	// Token: 0x0600460A RID: 17930
	void DeleteAll();

	// Token: 0x0600460B RID: 17931
	void Save();

	// Token: 0x0600460C RID: 17932
	void Load();

	// Token: 0x17000757 RID: 1879
	// (get) Token: 0x0600460D RID: 17933
	bool CanLoad { get; }
}
