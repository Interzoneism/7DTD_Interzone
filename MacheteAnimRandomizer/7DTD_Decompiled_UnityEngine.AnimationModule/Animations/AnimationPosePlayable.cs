using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000054 RID: 84
	[RequiredByNativeCode]
	[StaticAccessor("AnimationPosePlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[NativeHeader("Modules/Animation/Director/AnimationPosePlayable.h")]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationPosePlayable.bindings.h")]
	internal struct AnimationPosePlayable : IPlayable, IEquatable<AnimationPosePlayable>
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x00005AA4 File Offset: 0x00003CA4
		public static AnimationPosePlayable Null
		{
			get
			{
				return AnimationPosePlayable.m_NullPlayable;
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00005ABC File Offset: 0x00003CBC
		public static AnimationPosePlayable Create(PlayableGraph graph)
		{
			PlayableHandle handle = AnimationPosePlayable.CreateHandle(graph);
			return new AnimationPosePlayable(handle);
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00005ADC File Offset: 0x00003CDC
		private static PlayableHandle CreateHandle(PlayableGraph graph)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !AnimationPosePlayable.CreateHandleInternal(graph, ref @null);
			PlayableHandle result;
			if (flag)
			{
				result = PlayableHandle.Null;
			}
			else
			{
				result = @null;
			}
			return result;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x00005B0C File Offset: 0x00003D0C
		internal AnimationPosePlayable(PlayableHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOfType<AnimationPosePlayable>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationPosePlayable.");
				}
			}
			this.m_Handle = handle;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00005B48 File Offset: 0x00003D48
		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00005B60 File Offset: 0x00003D60
		public static implicit operator Playable(AnimationPosePlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00005B80 File Offset: 0x00003D80
		public static explicit operator AnimationPosePlayable(Playable playable)
		{
			return new AnimationPosePlayable(playable.GetHandle());
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00005BA0 File Offset: 0x00003DA0
		public bool Equals(AnimationPosePlayable other)
		{
			return this.Equals(other.GetHandle());
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00005BCC File Offset: 0x00003DCC
		public bool GetMustReadPreviousPose()
		{
			return AnimationPosePlayable.GetMustReadPreviousPoseInternal(ref this.m_Handle);
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00005BE9 File Offset: 0x00003DE9
		public void SetMustReadPreviousPose(bool value)
		{
			AnimationPosePlayable.SetMustReadPreviousPoseInternal(ref this.m_Handle, value);
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00005BFC File Offset: 0x00003DFC
		public bool GetReadDefaultPose()
		{
			return AnimationPosePlayable.GetReadDefaultPoseInternal(ref this.m_Handle);
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00005C19 File Offset: 0x00003E19
		public void SetReadDefaultPose(bool value)
		{
			AnimationPosePlayable.SetReadDefaultPoseInternal(ref this.m_Handle, value);
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00005C2C File Offset: 0x00003E2C
		public bool GetApplyFootIK()
		{
			return AnimationPosePlayable.GetApplyFootIKInternal(ref this.m_Handle);
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00005C49 File Offset: 0x00003E49
		public void SetApplyFootIK(bool value)
		{
			AnimationPosePlayable.SetApplyFootIKInternal(ref this.m_Handle, value);
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00005C59 File Offset: 0x00003E59
		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationPosePlayable.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		// Token: 0x060003E4 RID: 996
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetMustReadPreviousPoseInternal(ref PlayableHandle handle);

		// Token: 0x060003E5 RID: 997
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMustReadPreviousPoseInternal(ref PlayableHandle handle, bool value);

		// Token: 0x060003E6 RID: 998
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetReadDefaultPoseInternal(ref PlayableHandle handle);

		// Token: 0x060003E7 RID: 999
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetReadDefaultPoseInternal(ref PlayableHandle handle, bool value);

		// Token: 0x060003E8 RID: 1000
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetApplyFootIKInternal(ref PlayableHandle handle);

		// Token: 0x060003E9 RID: 1001
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetApplyFootIKInternal(ref PlayableHandle handle, bool value);

		// Token: 0x060003EB RID: 1003
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);

		// Token: 0x04000151 RID: 337
		private PlayableHandle m_Handle;

		// Token: 0x04000152 RID: 338
		private static readonly AnimationPosePlayable m_NullPlayable = new AnimationPosePlayable(PlayableHandle.Null);
	}
}
