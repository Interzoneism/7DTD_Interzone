using System;
using System.Collections.Generic;

// Token: 0x020009A3 RID: 2467
public class ChunkBlockClearData : ChunkCustomData
{
	// Token: 0x06004B36 RID: 19254 RVA: 0x001DB7C8 File Offset: 0x001D99C8
	public ChunkBlockClearData()
	{
	}

	// Token: 0x06004B37 RID: 19255 RVA: 0x001DB7DB File Offset: 0x001D99DB
	public ChunkBlockClearData(string _key, ulong _expiresInWorldTime, bool _isSavedToNetwork, World _world) : base(_key, _expiresInWorldTime, _isSavedToNetwork)
	{
		this.World = _world;
	}

	// Token: 0x06004B38 RID: 19256 RVA: 0x001DB7FC File Offset: 0x001D99FC
	public override void OnRemove(Chunk chunk)
	{
		for (int i = this.BlockList.Count - 1; i >= 0; i--)
		{
			Vector3i vector3i = this.BlockList[i];
			chunk.SetBlock(this.World, vector3i.x, vector3i.y, vector3i.z, BlockValue.Air, true, true, false, false, -1);
		}
	}

	// Token: 0x0400399F RID: 14751
	public List<Vector3i> BlockList = new List<Vector3i>();

	// Token: 0x040039A0 RID: 14752
	public World World;
}
