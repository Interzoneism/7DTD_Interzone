using System;

// Token: 0x020009D9 RID: 2521
public interface IChunkAccess
{
	// Token: 0x06004D3D RID: 19773
	IChunk GetChunkFromWorldPos(int x, int y, int z);

	// Token: 0x06004D3E RID: 19774
	IChunk GetChunkFromWorldPos(Vector3i _blockPos);
}
