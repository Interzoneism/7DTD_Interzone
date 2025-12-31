using System;

// Token: 0x02000A5A RID: 2650
public interface ITileArea<T> where T : class
{
	// Token: 0x1700082F RID: 2095
	// (get) Token: 0x060050B9 RID: 20665
	TileAreaConfig Config { get; }

	// Token: 0x17000830 RID: 2096
	T this[int _tileX, int _tileZ]
	{
		get;
	}

	// Token: 0x17000831 RID: 2097
	T this[uint _key]
	{
		get;
	}

	// Token: 0x060050BC RID: 20668 RVA: 0x00002914 File Offset: 0x00000B14
	void Cleanup()
	{
	}
}
