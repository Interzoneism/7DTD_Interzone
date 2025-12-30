using System;

// Token: 0x020000E4 RID: 228
[Flags]
public enum EBlockRotationClasses
{
	// Token: 0x04000617 RID: 1559
	None = 0,
	// Token: 0x04000618 RID: 1560
	Basic90 = 1,
	// Token: 0x04000619 RID: 1561
	Headfirst = 2,
	// Token: 0x0400061A RID: 1562
	Sideways = 4,
	// Token: 0x0400061B RID: 1563
	Basic45 = 8,
	// Token: 0x0400061C RID: 1564
	Basic90And45 = 9,
	// Token: 0x0400061D RID: 1565
	No45 = 7,
	// Token: 0x0400061E RID: 1566
	Advanced = 6,
	// Token: 0x0400061F RID: 1567
	All = 15
}
