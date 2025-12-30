using System;
using System.ComponentModel;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x0200002F RID: 47
	[NativeHeader("Modules/Animation/HumanDescription.h")]
	[NativeType(CodegenOptions.Custom, "MonoSkeletonBone")]
	[RequiredByNativeCode]
	public struct SkeletonBone
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600021E RID: 542 RVA: 0x00003BA8 File Offset: 0x00001DA8
		// (set) Token: 0x0600021F RID: 543 RVA: 0x00002059 File Offset: 0x00000259
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("transformModified is no longer used and has been deprecated.", true)]
		public int transformModified
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		// Token: 0x04000109 RID: 265
		[NativeName("m_Name")]
		public string name;

		// Token: 0x0400010A RID: 266
		[NativeName("m_ParentName")]
		internal string parentName;

		// Token: 0x0400010B RID: 267
		[NativeName("m_Position")]
		public Vector3 position;

		// Token: 0x0400010C RID: 268
		[NativeName("m_Rotation")]
		public Quaternion rotation;

		// Token: 0x0400010D RID: 269
		[NativeName("m_Scale")]
		public Vector3 scale;
	}
}
