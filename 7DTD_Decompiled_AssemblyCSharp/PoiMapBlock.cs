using System;

// Token: 0x02000A8B RID: 2699
public class PoiMapBlock
{
	// Token: 0x0600534D RID: 21325 RVA: 0x002159AD File Offset: 0x00213BAD
	public PoiMapBlock(BlockValue _blockValue, float _prob, int _offset)
	{
		this.blockValue = _blockValue;
		this.m_Prob = _prob;
		this.offset = _offset;
	}

	// Token: 0x04003F84 RID: 16260
	public BlockValue blockValue;

	// Token: 0x04003F85 RID: 16261
	public float m_Prob;

	// Token: 0x04003F86 RID: 16262
	public int offset;
}
