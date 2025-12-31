using System;

// Token: 0x02001220 RID: 4640
public readonly struct FloatRange
{
	// Token: 0x060090E8 RID: 37096 RVA: 0x0039D6A1 File Offset: 0x0039B8A1
	public FloatRange(float _min, float _max)
	{
		this.min = _min;
		this.max = _max;
	}

	// Token: 0x060090E9 RID: 37097 RVA: 0x0039D6B1 File Offset: 0x0039B8B1
	public bool IsSet()
	{
		return this.min != 0f || this.max != 0f;
	}

	// Token: 0x060090EA RID: 37098 RVA: 0x0039D6D2 File Offset: 0x0039B8D2
	public float Random(GameRandom _rnd)
	{
		return _rnd.RandomRange(this.min, this.max);
	}

	// Token: 0x060090EB RID: 37099 RVA: 0x0039D6E8 File Offset: 0x0039B8E8
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"(",
			this.min.ToCultureInvariantString(),
			"-",
			this.max.ToCultureInvariantString(),
			")"
		});
	}

	// Token: 0x04006F6A RID: 28522
	public readonly float min;

	// Token: 0x04006F6B RID: 28523
	public readonly float max;
}
