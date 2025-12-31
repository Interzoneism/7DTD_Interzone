using System;
using System.Runtime.InteropServices;

// Token: 0x020019D1 RID: 6609
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
public class PublicizedFromAttribute : Attribute
{
	// Token: 0x0600C118 RID: 49432 RVA: 0x00491B3B File Offset: 0x0048FD3B
	public PublicizedFromAttribute(EAccessModifier _originalAccessModifier)
	{
		this.OriginalAccessModifier = _originalAccessModifier;
	}

	// Token: 0x04009AEB RID: 39659
	public readonly EAccessModifier OriginalAccessModifier;
}
