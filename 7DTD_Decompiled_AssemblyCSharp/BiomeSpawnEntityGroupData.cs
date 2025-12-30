using System;

// Token: 0x02000975 RID: 2421
public class BiomeSpawnEntityGroupData
{
	// Token: 0x06004937 RID: 18743 RVA: 0x001CF528 File Offset: 0x001CD728
	public BiomeSpawnEntityGroupData(int _idHash, int _maxCount, int _respawndelay, EDaytime _daytime)
	{
		this.idHash = _idHash;
		this.maxCount = _maxCount;
		this.daytime = _daytime;
		this.respawnDelayInWorldTime = _respawndelay;
	}

	// Token: 0x0400385B RID: 14427
	public int idHash;

	// Token: 0x0400385C RID: 14428
	public string entityGroupName;

	// Token: 0x0400385D RID: 14429
	public int maxCount;

	// Token: 0x0400385E RID: 14430
	public int respawnDelayInWorldTime;

	// Token: 0x0400385F RID: 14431
	public EDaytime daytime;

	// Token: 0x04003860 RID: 14432
	public FastTags<TagGroup.Poi> POITags;

	// Token: 0x04003861 RID: 14433
	public FastTags<TagGroup.Poi> noPOITags;
}
