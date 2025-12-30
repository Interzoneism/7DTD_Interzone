using System;

// Token: 0x0200032F RID: 815
public enum DynamicItemState : byte
{
	// Token: 0x04000EFF RID: 3839
	Waiting,
	// Token: 0x04000F00 RID: 3840
	UpdateRequired,
	// Token: 0x04000F01 RID: 3841
	Empty,
	// Token: 0x04000F02 RID: 3842
	LoadRequested,
	// Token: 0x04000F03 RID: 3843
	Loading,
	// Token: 0x04000F04 RID: 3844
	Loaded,
	// Token: 0x04000F05 RID: 3845
	ReadyToDelete,
	// Token: 0x04000F06 RID: 3846
	Invalid
}
