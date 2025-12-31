using System;

// Token: 0x020004C1 RID: 1217
public static class EnumDamageSourceExtensions
{
	// Token: 0x060027D4 RID: 10196 RVA: 0x000C07F3 File Offset: 0x000BE9F3
	public static bool AffectedByArmor(this EnumDamageSource s)
	{
		return s == EnumDamageSource.External;
	}
}
