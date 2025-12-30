using System;

// Token: 0x0200114B RID: 4427
public class CachedStringFormatterFloat : CachedStringFormatter<float>
{
	// Token: 0x06008AD0 RID: 35536 RVA: 0x00381DF8 File Offset: 0x0037FFF8
	public CachedStringFormatterFloat(string _format = null) : base(null)
	{
		this.formatter = new Func<float, string>(this.formatterFunc);
		this.format = _format;
	}

	// Token: 0x06008AD1 RID: 35537 RVA: 0x00381E1A File Offset: 0x0038001A
	[PublicizedFrom(EAccessModifier.Private)]
	public string formatterFunc(float _f)
	{
		return _f.ToCultureInvariantString(this.format);
	}

	// Token: 0x04006C99 RID: 27801
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string format;
}
