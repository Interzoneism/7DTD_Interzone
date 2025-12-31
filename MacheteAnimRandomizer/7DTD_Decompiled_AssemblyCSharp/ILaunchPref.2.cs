using System;

// Token: 0x02000FEC RID: 4076
public interface ILaunchPref<out T> : ILaunchPref
{
	// Token: 0x17000D7A RID: 3450
	// (get) Token: 0x06008167 RID: 33127
	T Value { get; }
}
