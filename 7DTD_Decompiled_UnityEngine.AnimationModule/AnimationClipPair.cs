using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	// Token: 0x02000021 RID: 33
	[Obsolete("This class is not used anymore. See AnimatorOverrideController.GetOverrides() and AnimatorOverrideController.ApplyOverrides()")]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimationClipPair
	{
		// Token: 0x0400006D RID: 109
		public AnimationClip originalClip;

		// Token: 0x0400006E RID: 110
		public AnimationClip overrideClip;
	}
}
