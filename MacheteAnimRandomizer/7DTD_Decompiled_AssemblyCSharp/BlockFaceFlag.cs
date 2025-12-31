using System;

// Token: 0x0200113C RID: 4412
[Flags]
public enum BlockFaceFlag
{
	// Token: 0x04006C5E RID: 27742
	None = 0,
	// Token: 0x04006C5F RID: 27743
	Top = 1,
	// Token: 0x04006C60 RID: 27744
	Bottom = 2,
	// Token: 0x04006C61 RID: 27745
	North = 4,
	// Token: 0x04006C62 RID: 27746
	West = 8,
	// Token: 0x04006C63 RID: 27747
	South = 16,
	// Token: 0x04006C64 RID: 27748
	East = 32,
	// Token: 0x04006C65 RID: 27749
	All = 63,
	// Token: 0x04006C66 RID: 27750
	Solid = 63,
	// Token: 0x04006C67 RID: 27751
	Axials = 60
}
