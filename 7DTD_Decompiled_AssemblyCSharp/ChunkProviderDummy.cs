using System;
using System.Collections;

// Token: 0x020009C1 RID: 2497
public class ChunkProviderDummy : ChunkProviderAbstract
{
	// Token: 0x06004C81 RID: 19585 RVA: 0x001E52E7 File Offset: 0x001E34E7
	public override IEnumerator Init(World _worldData)
	{
		MultiBlockManager.Instance.Initialize(null);
		yield return null;
		yield break;
	}

	// Token: 0x06004C82 RID: 19586 RVA: 0x001E52EF File Offset: 0x001E34EF
	public override void Cleanup()
	{
		base.Cleanup();
		MultiBlockManager.Instance.Cleanup();
	}

	// Token: 0x06004C83 RID: 19587 RVA: 0x00075C39 File Offset: 0x00073E39
	public override EnumChunkProviderId GetProviderId()
	{
		return EnumChunkProviderId.NetworkClient;
	}

	// Token: 0x06004C84 RID: 19588 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void setChunkPrerequisits(string sSeed)
	{
	}

	// Token: 0x06004C85 RID: 19589 RVA: 0x001E5301 File Offset: 0x001E3501
	public override void UnloadChunk(Chunk _c)
	{
		MemoryPools.PoolChunks.FreeSync(_c);
	}

	// Token: 0x06004C86 RID: 19590 RVA: 0x001E530E File Offset: 0x001E350E
	public override SpawnPointList GetSpawnPointList()
	{
		return this.dummySpawnPointList;
	}

	// Token: 0x06004C87 RID: 19591 RVA: 0x001E5316 File Offset: 0x001E3516
	public override void SetSpawnPointList(SpawnPointList _spawnPointList)
	{
		this.dummySpawnPointList = _spawnPointList;
	}

	// Token: 0x04003A7E RID: 14974
	[PublicizedFrom(EAccessModifier.Private)]
	public SpawnPointList dummySpawnPointList = new SpawnPointList();
}
