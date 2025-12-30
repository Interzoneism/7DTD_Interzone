using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000008 RID: 8
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[PublicizedFrom(EAccessModifier.Internal)]
	public sealed class NullableContextAttribute : Attribute
	{
		// Token: 0x06000015 RID: 21 RVA: 0x000022FD File Offset: 0x000004FD
		public NullableContextAttribute(byte A_1)
		{
			this.Flag = A_1;
		}

		// Token: 0x04000006 RID: 6
		public readonly byte Flag;
	}
}
