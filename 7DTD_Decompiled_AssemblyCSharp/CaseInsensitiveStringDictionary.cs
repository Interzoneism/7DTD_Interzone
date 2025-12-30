using System;
using System.Collections.Generic;

// Token: 0x0200114D RID: 4429
public class CaseInsensitiveStringDictionary<TValue> : Dictionary<string, TValue>
{
	// Token: 0x06008AD4 RID: 35540 RVA: 0x00381E4F File Offset: 0x0038004F
	public CaseInsensitiveStringDictionary() : base(StringComparer.OrdinalIgnoreCase)
	{
	}

	// Token: 0x06008AD5 RID: 35541 RVA: 0x00381E5C File Offset: 0x0038005C
	public CaseInsensitiveStringDictionary(int _capacity) : base(_capacity, StringComparer.OrdinalIgnoreCase)
	{
	}

	// Token: 0x06008AD6 RID: 35542 RVA: 0x00381E6A File Offset: 0x0038006A
	public CaseInsensitiveStringDictionary(IDictionary<string, TValue> _dictionary) : base(_dictionary, StringComparer.OrdinalIgnoreCase)
	{
	}

	// Token: 0x06008AD7 RID: 35543 RVA: 0x00381E78 File Offset: 0x00380078
	[Obsolete("CaseInsensitiveStringDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public CaseInsensitiveStringDictionary(IEqualityComparer<string> _comparer) : base(_comparer)
	{
	}

	// Token: 0x06008AD8 RID: 35544 RVA: 0x00381E81 File Offset: 0x00380081
	[Obsolete("CaseInsensitiveStringDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public CaseInsensitiveStringDictionary(int _capacity, IEqualityComparer<string> _comparer) : base(_capacity, _comparer)
	{
	}

	// Token: 0x06008AD9 RID: 35545 RVA: 0x00381E8B File Offset: 0x0038008B
	[Obsolete("CaseInsensitiveStringDictionary constructors with explicit comparer are deprecated in favor of the variants without, as these automatically set the appropriate comparer for the enum key type.", true)]
	public CaseInsensitiveStringDictionary(IDictionary<string, TValue> _dictionary, IEqualityComparer<string> _comparer) : base(_dictionary, _comparer)
	{
	}
}
