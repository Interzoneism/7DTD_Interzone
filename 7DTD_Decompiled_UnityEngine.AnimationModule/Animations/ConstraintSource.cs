using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000064 RID: 100
	[NativeType(CodegenOptions = CodegenOptions.Custom, Header = "Modules/Animation/Constraints/ConstraintSource.h", IntermediateScriptingStructName = "MonoConstraintSource")]
	[UsedByNativeCode]
	[NativeHeader("Modules/Animation/Constraints/Constraint.bindings.h")]
	[Serializable]
	public struct ConstraintSource
	{
		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x00007CF0 File Offset: 0x00005EF0
		// (set) Token: 0x06000584 RID: 1412 RVA: 0x00007D08 File Offset: 0x00005F08
		public Transform sourceTransform
		{
			get
			{
				return this.m_SourceTransform;
			}
			set
			{
				this.m_SourceTransform = value;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000585 RID: 1413 RVA: 0x00007D14 File Offset: 0x00005F14
		// (set) Token: 0x06000586 RID: 1414 RVA: 0x00007D2C File Offset: 0x00005F2C
		public float weight
		{
			get
			{
				return this.m_Weight;
			}
			set
			{
				this.m_Weight = value;
			}
		}

		// Token: 0x04000181 RID: 385
		[NativeName("sourceTransform")]
		private Transform m_SourceTransform;

		// Token: 0x04000182 RID: 386
		[NativeName("weight")]
		private float m_Weight;
	}
}
