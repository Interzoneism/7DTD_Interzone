using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x02000031 RID: 49
	[NativeHeader("Modules/Animation/HumanDescription.h")]
	[RequiredByNativeCode]
	[NativeType(CodegenOptions.Custom, "MonoHumanBone")]
	public struct HumanBone
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600022A RID: 554 RVA: 0x00003C78 File Offset: 0x00001E78
		// (set) Token: 0x0600022B RID: 555 RVA: 0x00003C90 File Offset: 0x00001E90
		public string boneName
		{
			get
			{
				return this.m_BoneName;
			}
			set
			{
				this.m_BoneName = value;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600022C RID: 556 RVA: 0x00003C9C File Offset: 0x00001E9C
		// (set) Token: 0x0600022D RID: 557 RVA: 0x00003CB4 File Offset: 0x00001EB4
		public string humanName
		{
			get
			{
				return this.m_HumanName;
			}
			set
			{
				this.m_HumanName = value;
			}
		}

		// Token: 0x04000113 RID: 275
		private string m_BoneName;

		// Token: 0x04000114 RID: 276
		private string m_HumanName;

		// Token: 0x04000115 RID: 277
		[NativeName("m_Limit")]
		public HumanLimit limit;
	}
}
