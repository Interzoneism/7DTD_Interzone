using System;

// Token: 0x02000323 RID: 803
[Flags]
public enum DynamicMeshStates : short
{
	// Token: 0x04000E83 RID: 3715
	None = 0,
	// Token: 0x04000E84 RID: 3716
	ThreadUpdating = 1,
	// Token: 0x04000E85 RID: 3717
	SaveRequired = 2,
	// Token: 0x04000E86 RID: 3718
	LoadRequired = 4,
	// Token: 0x04000E87 RID: 3719
	UnloadMark1 = 8,
	// Token: 0x04000E88 RID: 3720
	UnloadMark2 = 16,
	// Token: 0x04000E89 RID: 3721
	UnloadMark3 = 32,
	// Token: 0x04000E8A RID: 3722
	MarkedForDelete = 64,
	// Token: 0x04000E8B RID: 3723
	LoadBoosted = 128,
	// Token: 0x04000E8C RID: 3724
	MainThreadLoadRequest = 256,
	// Token: 0x04000E8D RID: 3725
	FileMissing = 512,
	// Token: 0x04000E8E RID: 3726
	Generating = 1024
}
