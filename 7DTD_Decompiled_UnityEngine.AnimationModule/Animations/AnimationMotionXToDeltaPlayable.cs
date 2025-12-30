using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x0200004F RID: 79
	[StaticAccessor("AnimationMotionXToDeltaPlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationMotionXToDeltaPlayable.bindings.h")]
	[RequiredByNativeCode]
	internal struct AnimationMotionXToDeltaPlayable : IPlayable, IEquatable<AnimationMotionXToDeltaPlayable>
	{
		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600039C RID: 924 RVA: 0x000055D8 File Offset: 0x000037D8
		public static AnimationMotionXToDeltaPlayable Null
		{
			get
			{
				return AnimationMotionXToDeltaPlayable.m_NullPlayable;
			}
		}

		// Token: 0x0600039D RID: 925 RVA: 0x000055F0 File Offset: 0x000037F0
		public static AnimationMotionXToDeltaPlayable Create(PlayableGraph graph)
		{
			PlayableHandle handle = AnimationMotionXToDeltaPlayable.CreateHandle(graph);
			return new AnimationMotionXToDeltaPlayable(handle);
		}

		// Token: 0x0600039E RID: 926 RVA: 0x00005610 File Offset: 0x00003810
		private static PlayableHandle CreateHandle(PlayableGraph graph)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !AnimationMotionXToDeltaPlayable.CreateHandleInternal(graph, ref @null);
			PlayableHandle result;
			if (flag)
			{
				result = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(1);
				result = @null;
			}
			return result;
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0000564C File Offset: 0x0000384C
		private AnimationMotionXToDeltaPlayable(PlayableHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOfType<AnimationMotionXToDeltaPlayable>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationMotionXToDeltaPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x00005688 File Offset: 0x00003888
		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x000056A0 File Offset: 0x000038A0
		public static implicit operator Playable(AnimationMotionXToDeltaPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x000056C0 File Offset: 0x000038C0
		public static explicit operator AnimationMotionXToDeltaPlayable(Playable playable)
		{
			return new AnimationMotionXToDeltaPlayable(playable.GetHandle());
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x000056E0 File Offset: 0x000038E0
		public bool Equals(AnimationMotionXToDeltaPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00005704 File Offset: 0x00003904
		public bool IsAbsoluteMotion()
		{
			return AnimationMotionXToDeltaPlayable.IsAbsoluteMotionInternal(ref this.m_Handle);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00005721 File Offset: 0x00003921
		public void SetAbsoluteMotion(bool value)
		{
			AnimationMotionXToDeltaPlayable.SetAbsoluteMotionInternal(ref this.m_Handle, value);
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00005731 File Offset: 0x00003931
		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationMotionXToDeltaPlayable.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		// Token: 0x060003A7 RID: 935
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAbsoluteMotionInternal(ref PlayableHandle handle);

		// Token: 0x060003A8 RID: 936
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAbsoluteMotionInternal(ref PlayableHandle handle, bool value);

		// Token: 0x060003AA RID: 938
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);

		// Token: 0x0400014C RID: 332
		private PlayableHandle m_Handle;

		// Token: 0x0400014D RID: 333
		private static readonly AnimationMotionXToDeltaPlayable m_NullPlayable = new AnimationMotionXToDeltaPlayable(PlayableHandle.Null);
	}
}
