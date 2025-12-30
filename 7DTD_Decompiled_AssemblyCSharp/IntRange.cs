using System;

// Token: 0x02001221 RID: 4641
public readonly struct IntRange
{
	// Token: 0x060090EC RID: 37100 RVA: 0x0039D734 File Offset: 0x0039B934
	public IntRange(int _min, int _max)
	{
		this.min = _min;
		this.max = _max;
	}

	// Token: 0x060090ED RID: 37101 RVA: 0x0039D744 File Offset: 0x0039B944
	public bool IsSet()
	{
		return this.min != 0 || this.max != 0;
	}

	// Token: 0x060090EE RID: 37102 RVA: 0x0039D759 File Offset: 0x0039B959
	public float Random(GameRandom _rnd)
	{
		return (float)_rnd.RandomRange(this.min, this.max);
	}

	// Token: 0x060090EF RID: 37103 RVA: 0x0039D770 File Offset: 0x0039B970
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"(",
			this.min.ToString(),
			"-",
			this.max.ToString(),
			")"
		});
	}

	// Token: 0x04006F6C RID: 28524
	public readonly int min;

	// Token: 0x04006F6D RID: 28525
	public readonly int max;
}
