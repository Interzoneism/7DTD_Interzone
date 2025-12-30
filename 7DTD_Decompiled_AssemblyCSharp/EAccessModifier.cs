using System;
using System.Runtime.InteropServices;

// Token: 0x020019D0 RID: 6608
[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public enum EAccessModifier
{
	// Token: 0x04009AE4 RID: 39652
	Unknown,
	// Token: 0x04009AE5 RID: 39653
	Public,
	// Token: 0x04009AE6 RID: 39654
	Private,
	// Token: 0x04009AE7 RID: 39655
	Protected,
	// Token: 0x04009AE8 RID: 39656
	Internal,
	// Token: 0x04009AE9 RID: 39657
	ProtectedInternal,
	// Token: 0x04009AEA RID: 39658
	PrivateProtected
}
