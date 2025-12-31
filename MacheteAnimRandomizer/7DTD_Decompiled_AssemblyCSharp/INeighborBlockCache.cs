using System;

// Token: 0x020009A8 RID: 2472
public interface INeighborBlockCache
{
	// Token: 0x06004B72 RID: 19314
	void Init(int _bX, int _bZ);

	// Token: 0x06004B73 RID: 19315
	void Clear();

	// Token: 0x06004B74 RID: 19316
	IChunk GetChunk(int x, int z);

	// Token: 0x06004B75 RID: 19317
	IChunk GetNeighborChunk(int x, int z);

	// Token: 0x06004B76 RID: 19318
	BlockValue Get(int relx, int absy, int relz);

	// Token: 0x06004B77 RID: 19319
	byte GetStab(int relx, int absy, int relz);

	// Token: 0x06004B78 RID: 19320
	bool IsWater(int relx, int absy, int relz);

	// Token: 0x06004B79 RID: 19321
	bool IsAir(int relx, int absy, int relz);
}
