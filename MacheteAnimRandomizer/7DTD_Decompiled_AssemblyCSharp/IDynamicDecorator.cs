using System;

// Token: 0x020009E0 RID: 2528
public interface IDynamicDecorator
{
	// Token: 0x06004D88 RID: 19848
	void DecorateChunk(World _world, Chunk _chunk);

	// Token: 0x06004D89 RID: 19849
	void Cleanup();
}
