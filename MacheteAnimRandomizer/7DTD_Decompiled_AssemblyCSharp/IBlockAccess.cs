using System;

// Token: 0x020009D8 RID: 2520
public interface IBlockAccess
{
	// Token: 0x06004D3B RID: 19771
	BlockValue GetBlock(int x, int y, int z);

	// Token: 0x06004D3C RID: 19772
	BlockValue GetBlock(Vector3i _pos);
}
