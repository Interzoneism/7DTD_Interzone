using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	// Token: 0x0200003A RID: 58
	[NativeHeader("Modules/Animation/RuntimeAnimatorController.h")]
	[UsedByNativeCode]
	[ExcludeFromObjectFactory]
	public class RuntimeAnimatorController : Object
	{
		// Token: 0x06000288 RID: 648 RVA: 0x00003A43 File Offset: 0x00001C43
		protected RuntimeAnimatorController()
		{
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000289 RID: 649
		public extern AnimationClip[] animationClips { [MethodImpl(MethodImplOptions.InternalCall)] get; }
	}
}
