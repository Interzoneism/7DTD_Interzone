using System;

// Token: 0x02000CFF RID: 3327
public abstract class XUiListEntry<T> : IComparable<T> where T : XUiListEntry<T>
{
	// Token: 0x06006751 RID: 26449
	public abstract int CompareTo(T _otherEntry);

	// Token: 0x06006752 RID: 26450
	public abstract bool GetBindingValue(ref string _value, string _bindingName);

	// Token: 0x06006753 RID: 26451
	public abstract bool MatchesSearch(string _searchString);

	// Token: 0x06006754 RID: 26452 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiListEntry()
	{
	}
}
