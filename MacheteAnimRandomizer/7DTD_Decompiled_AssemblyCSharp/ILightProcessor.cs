using System;

// Token: 0x020009E8 RID: 2536
public interface ILightProcessor
{
	// Token: 0x06004DAA RID: 19882
	void LightChunk(Chunk _chunk);

	// Token: 0x06004DAB RID: 19883
	void GenerateSunlight(Chunk chunk, bool bSpreadLight);

	// Token: 0x06004DAC RID: 19884
	void SpreadLight(Chunk c, int blockX, int blockY, int blockZ, byte lightValue, Chunk.LIGHT_TYPE type, bool bSetAtStarterPos = true);

	// Token: 0x06004DAD RID: 19885
	void UnspreadLight(Chunk c, int x, int y, int z, byte lightValue, Chunk.LIGHT_TYPE type);

	// Token: 0x06004DAE RID: 19886
	void RefreshSunlightAtLocalPos(Chunk c, int x, int y, bool _isSpread);

	// Token: 0x06004DAF RID: 19887
	byte RefreshLightAtLocalPos(Chunk c, int x, int y, int z, Chunk.LIGHT_TYPE type);
}
