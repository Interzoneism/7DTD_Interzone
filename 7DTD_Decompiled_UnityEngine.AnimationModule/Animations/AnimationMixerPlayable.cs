using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x0200004E RID: 78
	[StaticAccessor("AnimationMixerPlayableBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationMixerPlayable.bindings.h")]
	[NativeHeader("Modules/Animation/Director/AnimationMixerPlayable.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[RequiredByNativeCode]
	public struct AnimationMixerPlayable : IPlayable, IEquatable<AnimationMixerPlayable>
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000390 RID: 912 RVA: 0x00005474 File Offset: 0x00003674
		public static AnimationMixerPlayable Null
		{
			get
			{
				return AnimationMixerPlayable.m_NullPlayable;
			}
		}

		// Token: 0x06000391 RID: 913 RVA: 0x0000548C File Offset: 0x0000368C
		[Obsolete("normalizeWeights is obsolete. It has no effect and will be removed.")]
		public static AnimationMixerPlayable Create(PlayableGraph graph, int inputCount, bool normalizeWeights)
		{
			return AnimationMixerPlayable.Create(graph, inputCount);
		}

		// Token: 0x06000392 RID: 914 RVA: 0x000054A8 File Offset: 0x000036A8
		public static AnimationMixerPlayable Create(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle handle = AnimationMixerPlayable.CreateHandle(graph, inputCount);
			return new AnimationMixerPlayable(handle);
		}

		// Token: 0x06000393 RID: 915 RVA: 0x000054C8 File Offset: 0x000036C8
		private static PlayableHandle CreateHandle(PlayableGraph graph, int inputCount = 0)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !AnimationMixerPlayable.CreateHandleInternal(graph, ref @null);
			PlayableHandle result;
			if (flag)
			{
				result = PlayableHandle.Null;
			}
			else
			{
				@null.SetInputCount(inputCount);
				result = @null;
			}
			return result;
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00005504 File Offset: 0x00003704
		internal AnimationMixerPlayable(PlayableHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOfType<AnimationMixerPlayable>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationMixerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x00005540 File Offset: 0x00003740
		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00005558 File Offset: 0x00003758
		public static implicit operator Playable(AnimationMixerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00005578 File Offset: 0x00003778
		public static explicit operator AnimationMixerPlayable(Playable playable)
		{
			return new AnimationMixerPlayable(playable.GetHandle());
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00005598 File Offset: 0x00003798
		public bool Equals(AnimationMixerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		// Token: 0x06000399 RID: 921 RVA: 0x000055BC File Offset: 0x000037BC
		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, ref PlayableHandle handle)
		{
			return AnimationMixerPlayable.CreateHandleInternal_Injected(ref graph, ref handle);
		}

		// Token: 0x0600039B RID: 923
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref PlayableHandle handle);

		// Token: 0x0400014A RID: 330
		private PlayableHandle m_Handle;

		// Token: 0x0400014B RID: 331
		private static readonly AnimationMixerPlayable m_NullPlayable = new AnimationMixerPlayable(PlayableHandle.Null);
	}
}
