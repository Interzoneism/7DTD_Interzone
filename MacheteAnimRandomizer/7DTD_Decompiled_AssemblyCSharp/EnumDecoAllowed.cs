using System;

// Token: 0x02000AAB RID: 2731
[Flags]
public enum EnumDecoAllowed : byte
{
	// Token: 0x04004097 RID: 16535
	Everything = 0,
	// Token: 0x04004098 RID: 16536
	SlopeLo = 1,
	// Token: 0x04004099 RID: 16537
	SlopeHi = 2,
	// Token: 0x0400409A RID: 16538
	SizeLo = 4,
	// Token: 0x0400409B RID: 16539
	SizeHi = 8,
	// Token: 0x0400409C RID: 16540
	StreetOnly = 16,
	// Token: 0x0400409D RID: 16541
	Nothing = 255
}
