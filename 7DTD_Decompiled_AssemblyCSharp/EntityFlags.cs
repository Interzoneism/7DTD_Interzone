using System;

// Token: 0x02000428 RID: 1064
[Flags]
public enum EntityFlags : uint
{
	// Token: 0x04001741 RID: 5953
	None = 0U,
	// Token: 0x04001742 RID: 5954
	Player = 1U,
	// Token: 0x04001743 RID: 5955
	Zombie = 2U,
	// Token: 0x04001744 RID: 5956
	Animal = 4U,
	// Token: 0x04001745 RID: 5957
	Bandit = 8U,
	// Token: 0x04001746 RID: 5958
	Edible = 16U,
	// Token: 0x04001747 RID: 5959
	All = 4294967295U,
	// Token: 0x04001748 RID: 5960
	AIHearing = 14U
}
