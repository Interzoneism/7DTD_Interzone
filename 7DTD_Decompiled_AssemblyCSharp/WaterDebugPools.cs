using System;

// Token: 0x02000B71 RID: 2929
public static class WaterDebugPools
{
	// Token: 0x06005AE9 RID: 23273 RVA: 0x002471D6 File Offset: 0x002453D6
	public static void CreatePools()
	{
		WaterDebugPools.rendererPool = new MemoryPooledObject<WaterDebugRenderer>(250);
		WaterDebugPools.layerPool = new MemoryPooledObject<WaterDebugRendererLayer>(4000);
	}

	// Token: 0x06005AEA RID: 23274 RVA: 0x002471F6 File Offset: 0x002453F6
	public static void Cleanup()
	{
		MemoryPooledObject<WaterDebugRenderer> memoryPooledObject = WaterDebugPools.rendererPool;
		if (memoryPooledObject != null)
		{
			memoryPooledObject.Cleanup();
		}
		WaterDebugPools.rendererPool = null;
		MemoryPooledObject<WaterDebugRendererLayer> memoryPooledObject2 = WaterDebugPools.layerPool;
		if (memoryPooledObject2 != null)
		{
			memoryPooledObject2.Cleanup();
		}
		WaterDebugPools.layerPool = null;
	}

	// Token: 0x0400457D RID: 17789
	[PublicizedFrom(EAccessModifier.Private)]
	public const int maxActiveChunks = 250;

	// Token: 0x0400457E RID: 17790
	[PublicizedFrom(EAccessModifier.Private)]
	public const int numLayers = 16;

	// Token: 0x0400457F RID: 17791
	public static MemoryPooledObject<WaterDebugRenderer> rendererPool;

	// Token: 0x04004580 RID: 17792
	public static MemoryPooledObject<WaterDebugRendererLayer> layerPool;
}
