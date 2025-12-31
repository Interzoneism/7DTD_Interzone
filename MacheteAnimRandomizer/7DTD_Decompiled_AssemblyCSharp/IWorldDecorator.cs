using System;

// Token: 0x02000AD1 RID: 2769
public interface IWorldDecorator
{
	// Token: 0x06005540 RID: 21824
	void DecorateChunkOverlapping(World _world, Chunk chunk, Chunk cXp1Z, Chunk cXZp1, Chunk cXp1Zp1, int seed);
}
