using System;

// Token: 0x02000FEB RID: 4075
public interface ILaunchPref
{
	// Token: 0x17000D79 RID: 3449
	// (get) Token: 0x06008165 RID: 33125
	string Name { get; }

	// Token: 0x06008166 RID: 33126
	bool TrySet(string stringRepresentation);
}
