using System;

// Token: 0x02000167 RID: 359
public struct SBlockPosValue
{
	// Token: 0x06000ABF RID: 2751 RVA: 0x00045754 File Offset: 0x00043954
	public SBlockPosValue(Vector3i _blockPosition, BlockValue _blockValue)
	{
		this.blockPos = _blockPosition;
		this.blockValue = _blockValue;
	}

	// Token: 0x0400097C RID: 2428
	public Vector3i blockPos;

	// Token: 0x0400097D RID: 2429
	public BlockValue blockValue;
}
