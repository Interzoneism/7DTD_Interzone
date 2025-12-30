using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000050 RID: 80
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationOffsetPlayable.bindings.h")]
	[NativeHeader("Modules/Animation/Director/AnimationOffsetPlayable.h")]
	[NativeHeader("Runtime/Director/Core/HPlayable.h")]
	[StaticAccessor("AnimationOffsetPlayableBindings", StaticAccessorType.DoubleColon)]
	[RequiredByNativeCode]
	internal struct AnimationOffsetPlayable : IPlayable, IEquatable<AnimationOffsetPlayable>
	{
		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060003AB RID: 939 RVA: 0x0000574C File Offset: 0x0000394C
		public static AnimationOffsetPlayable Null
		{
			get
			{
				return AnimationOffsetPlayable.m_NullPlayable;
			}
		}

		// Token: 0x060003AC RID: 940 RVA: 0x00005764 File Offset: 0x00003964
		public static AnimationOffsetPlayable Create(PlayableGraph graph, Vector3 position, Quaternion rotation, int inputCount)
		{
			PlayableHandle handle = AnimationOffsetPlayable.CreateHandle(graph, position, rotation, inputCount);
			return new AnimationOffsetPlayable(handle);
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00005788 File Offset: 0x00003988
		private static PlayableHandle CreateHandle(PlayableGraph graph, Vector3 position, Quaternion rotation, int inputCount)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !AnimationOffsetPlayable.CreateHandleInternal(graph, position, rotation, ref @null);
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

		// Token: 0x060003AE RID: 942 RVA: 0x000057C4 File Offset: 0x000039C4
		internal AnimationOffsetPlayable(PlayableHandle handle)
		{
			bool flag = handle.IsValid();
			if (flag)
			{
				bool flag2 = !handle.IsPlayableOfType<AnimationOffsetPlayable>();
				if (flag2)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimationOffsetPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00005800 File Offset: 0x00003A00
		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00005818 File Offset: 0x00003A18
		public static implicit operator Playable(AnimationOffsetPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00005838 File Offset: 0x00003A38
		public static explicit operator AnimationOffsetPlayable(Playable playable)
		{
			return new AnimationOffsetPlayable(playable.GetHandle());
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00005858 File Offset: 0x00003A58
		public bool Equals(AnimationOffsetPlayable other)
		{
			return this.Equals(other.GetHandle());
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00005884 File Offset: 0x00003A84
		public Vector3 GetPosition()
		{
			return AnimationOffsetPlayable.GetPositionInternal(ref this.m_Handle);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000058A1 File Offset: 0x00003AA1
		public void SetPosition(Vector3 value)
		{
			AnimationOffsetPlayable.SetPositionInternal(ref this.m_Handle, value);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x000058B4 File Offset: 0x00003AB4
		public Quaternion GetRotation()
		{
			return AnimationOffsetPlayable.GetRotationInternal(ref this.m_Handle);
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x000058D1 File Offset: 0x00003AD1
		public void SetRotation(Quaternion value)
		{
			AnimationOffsetPlayable.SetRotationInternal(ref this.m_Handle, value);
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000058E1 File Offset: 0x00003AE1
		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, Vector3 position, Quaternion rotation, ref PlayableHandle handle)
		{
			return AnimationOffsetPlayable.CreateHandleInternal_Injected(ref graph, ref position, ref rotation, ref handle);
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x000058F0 File Offset: 0x00003AF0
		[NativeThrows]
		private static Vector3 GetPositionInternal(ref PlayableHandle handle)
		{
			Vector3 result;
			AnimationOffsetPlayable.GetPositionInternal_Injected(ref handle, out result);
			return result;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00005906 File Offset: 0x00003B06
		[NativeThrows]
		private static void SetPositionInternal(ref PlayableHandle handle, Vector3 value)
		{
			AnimationOffsetPlayable.SetPositionInternal_Injected(ref handle, ref value);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00005910 File Offset: 0x00003B10
		[NativeThrows]
		private static Quaternion GetRotationInternal(ref PlayableHandle handle)
		{
			Quaternion result;
			AnimationOffsetPlayable.GetRotationInternal_Injected(ref handle, out result);
			return result;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00005926 File Offset: 0x00003B26
		[NativeThrows]
		private static void SetRotationInternal(ref PlayableHandle handle, Quaternion value)
		{
			AnimationOffsetPlayable.SetRotationInternal_Injected(ref handle, ref value);
		}

		// Token: 0x060003BD RID: 957
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, ref Vector3 position, ref Quaternion rotation, ref PlayableHandle handle);

		// Token: 0x060003BE RID: 958
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPositionInternal_Injected(ref PlayableHandle handle, out Vector3 ret);

		// Token: 0x060003BF RID: 959
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPositionInternal_Injected(ref PlayableHandle handle, ref Vector3 value);

		// Token: 0x060003C0 RID: 960
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRotationInternal_Injected(ref PlayableHandle handle, out Quaternion ret);

		// Token: 0x060003C1 RID: 961
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRotationInternal_Injected(ref PlayableHandle handle, ref Quaternion value);

		// Token: 0x0400014E RID: 334
		private PlayableHandle m_Handle;

		// Token: 0x0400014F RID: 335
		private static readonly AnimationOffsetPlayable m_NullPlayable = new AnimationOffsetPlayable(PlayableHandle.Null);
	}
}
