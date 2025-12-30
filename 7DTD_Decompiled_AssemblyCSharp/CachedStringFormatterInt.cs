using System;

// Token: 0x0200114A RID: 4426
public class CachedStringFormatterInt : CachedStringFormatter<int>
{
	// Token: 0x06008ACE RID: 35534 RVA: 0x00381DDB File Offset: 0x0037FFDB
	public CachedStringFormatterInt() : base(new Func<int, string>(CachedStringFormatterInt.formatterFunc))
	{
	}

	// Token: 0x06008ACF RID: 35535 RVA: 0x00381DEF File Offset: 0x0037FFEF
	[PublicizedFrom(EAccessModifier.Private)]
	public static string formatterFunc(int _i)
	{
		return _i.ToString();
	}
}
