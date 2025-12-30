using System;

// Token: 0x02000926 RID: 2342
public readonly struct SaveDataSizes
{
	// Token: 0x060045DC RID: 17884 RVA: 0x001BDFB8 File Offset: 0x001BC1B8
	public SaveDataSizes(long total, long remaining)
	{
		this.m_total = total;
		this.m_remaining = remaining;
	}

	// Token: 0x17000754 RID: 1876
	// (get) Token: 0x060045DD RID: 17885 RVA: 0x001BDFC8 File Offset: 0x001BC1C8
	public long Total
	{
		get
		{
			return this.m_total;
		}
	}

	// Token: 0x17000755 RID: 1877
	// (get) Token: 0x060045DE RID: 17886 RVA: 0x001BDFD0 File Offset: 0x001BC1D0
	public long Used
	{
		get
		{
			return this.m_total - this.m_remaining;
		}
	}

	// Token: 0x17000756 RID: 1878
	// (get) Token: 0x060045DF RID: 17887 RVA: 0x001BDFDF File Offset: 0x001BC1DF
	public long Remaining
	{
		get
		{
			return this.m_remaining;
		}
	}

	// Token: 0x04003682 RID: 13954
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly long m_total;

	// Token: 0x04003683 RID: 13955
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly long m_remaining;
}
