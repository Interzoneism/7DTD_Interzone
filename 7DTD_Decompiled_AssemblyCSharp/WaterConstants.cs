using System;

// Token: 0x02000B6F RID: 2927
public static class WaterConstants
{
	// Token: 0x06005ADF RID: 23263 RVA: 0x00247124 File Offset: 0x00245324
	public static int GetStableMassBelow(int mass, int massBelow)
	{
		return Utils.FastMin(mass + massBelow, 19500);
	}

	// Token: 0x04004576 RID: 17782
	public const int MIN_MASS = 195;

	// Token: 0x04004577 RID: 17783
	public const int MAX_MASS = 19500;

	// Token: 0x04004578 RID: 17784
	public const int OVERFULL_MAX = 58500;

	// Token: 0x04004579 RID: 17785
	public const int MIN_FLOW = 195;

	// Token: 0x0400457A RID: 17786
	public const float FLOW_SPEED = 0.5f;

	// Token: 0x0400457B RID: 17787
	public const int MIN_MASS_SIDE_SPREAD = 4875;
}
