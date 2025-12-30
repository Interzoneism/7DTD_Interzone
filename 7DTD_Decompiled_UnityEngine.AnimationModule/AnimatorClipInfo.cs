using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x0200001B RID: 27
	[NativeHeader("Modules/Animation/ScriptBindings/Animation.bindings.h")]
	[NativeHeader("Modules/Animation/AnimatorInfo.h")]
	[UsedByNativeCode]
	public struct AnimatorClipInfo
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00002634 File Offset: 0x00000834
		public AnimationClip clip
		{
			get
			{
				return (this.m_ClipInstanceID != 0) ? AnimatorClipInfo.InstanceIDToAnimationClipPPtr(this.m_ClipInstanceID) : null;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x0000265C File Offset: 0x0000085C
		public float weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		// Token: 0x060000A6 RID: 166
		[FreeFunction("AnimationBindings::InstanceIDToAnimationClipPPtr")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationClip InstanceIDToAnimationClipPPtr(int instanceID);

		// Token: 0x04000053 RID: 83
		private int m_ClipInstanceID;

		// Token: 0x04000054 RID: 84
		private float m_Weight;
	}
}
