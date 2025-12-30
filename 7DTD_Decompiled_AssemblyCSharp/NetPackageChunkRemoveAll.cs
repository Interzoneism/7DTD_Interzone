using System;
using UnityEngine.Scripting;

// Token: 0x0200070E RID: 1806
[Preserve]
public class NetPackageChunkRemoveAll : NetPackage
{
	// Token: 0x1700055F RID: 1375
	// (get) Token: 0x0600351D RID: 13597 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int Channel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x0600351E RID: 13598 RVA: 0x00002914 File Offset: 0x00000B14
	public override void read(PooledBinaryReader _reader)
	{
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x00161C1F File Offset: 0x0015FE1F
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x001629DC File Offset: 0x00160BDC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		GameManager.Instance.World.m_ChunkManager.RemoveAllChunks();
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x17000560 RID: 1376
	// (get) Token: 0x06003522 RID: 13602 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}
}
