using System;
using System.Collections.Generic;

// Token: 0x02001186 RID: 4486
public class EnumDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : struct, IConvertible
{
	// Token: 0x06008C21 RID: 35873 RVA: 0x00387001 File Offset: 0x00385201
	public EnumDictionary() : base(new FastEnumIntEqualityComparer<TKey>())
	{
	}

	// Token: 0x06008C22 RID: 35874 RVA: 0x0038700E File Offset: 0x0038520E
	public EnumDictionary(int capacity) : base(capacity, new FastEnumIntEqualityComparer<TKey>())
	{
	}

	// Token: 0x06008C23 RID: 35875 RVA: 0x0038701C File Offset: 0x0038521C
	public EnumDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary, new FastEnumIntEqualityComparer<TKey>())
	{
	}

	// Token: 0x06008C24 RID: 35876 RVA: 0x0038702A File Offset: 0x0038522A
	[Obsolete("EnumDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public EnumDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
	{
	}

	// Token: 0x06008C25 RID: 35877 RVA: 0x00387033 File Offset: 0x00385233
	[Obsolete("EnumDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public EnumDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
	{
	}

	// Token: 0x06008C26 RID: 35878 RVA: 0x0038703D File Offset: 0x0038523D
	[Obsolete("EnumDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public EnumDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
	{
	}
}
