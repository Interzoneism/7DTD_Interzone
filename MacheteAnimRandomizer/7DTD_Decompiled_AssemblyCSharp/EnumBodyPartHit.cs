using System;

// Token: 0x02000408 RID: 1032
[Flags]
public enum EnumBodyPartHit
{
	// Token: 0x04001576 RID: 5494
	None = 0,
	// Token: 0x04001577 RID: 5495
	Torso = 1,
	// Token: 0x04001578 RID: 5496
	Head = 2,
	// Token: 0x04001579 RID: 5497
	LeftUpperArm = 4,
	// Token: 0x0400157A RID: 5498
	RightUpperArm = 8,
	// Token: 0x0400157B RID: 5499
	LeftUpperLeg = 16,
	// Token: 0x0400157C RID: 5500
	RightUpperLeg = 32,
	// Token: 0x0400157D RID: 5501
	LeftLowerArm = 64,
	// Token: 0x0400157E RID: 5502
	RightLowerArm = 128,
	// Token: 0x0400157F RID: 5503
	LeftLowerLeg = 256,
	// Token: 0x04001580 RID: 5504
	RightLowerLeg = 512,
	// Token: 0x04001581 RID: 5505
	Special = 1024,
	// Token: 0x04001582 RID: 5506
	UpperArms = 12,
	// Token: 0x04001583 RID: 5507
	LowerArms = 192,
	// Token: 0x04001584 RID: 5508
	Arms = 204,
	// Token: 0x04001585 RID: 5509
	UpperLegs = 48,
	// Token: 0x04001586 RID: 5510
	LowerLegs = 768,
	// Token: 0x04001587 RID: 5511
	Legs = 816,
	// Token: 0x04001588 RID: 5512
	BitsUsed = 11
}
