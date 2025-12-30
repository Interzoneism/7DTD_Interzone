using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	// Token: 0x0200005F RID: 95
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationStreamHandles.bindings.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	public static class AnimationStreamHandleUtility
	{
		// Token: 0x060004EA RID: 1258 RVA: 0x00007258 File Offset: 0x00005458
		public static void WriteInts(AnimationStream stream, NativeArray<PropertyStreamHandle> handles, NativeArray<int> buffer, bool useMask)
		{
			stream.CheckIsValid();
			int num = AnimationSceneHandleUtility.ValidateAndGetArrayCount<PropertyStreamHandle, int>(ref stream, handles, buffer);
			bool flag = num == 0;
			if (!flag)
			{
				AnimationStreamHandleUtility.WriteStreamIntsInternal(ref stream, handles.GetUnsafePtr<PropertyStreamHandle>(), buffer.GetUnsafePtr<int>(), num, useMask);
			}
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00007298 File Offset: 0x00005498
		public static void WriteFloats(AnimationStream stream, NativeArray<PropertyStreamHandle> handles, NativeArray<float> buffer, bool useMask)
		{
			stream.CheckIsValid();
			int num = AnimationSceneHandleUtility.ValidateAndGetArrayCount<PropertyStreamHandle, float>(ref stream, handles, buffer);
			bool flag = num == 0;
			if (!flag)
			{
				AnimationStreamHandleUtility.WriteStreamFloatsInternal(ref stream, handles.GetUnsafePtr<PropertyStreamHandle>(), buffer.GetUnsafePtr<float>(), num, useMask);
			}
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x000072D8 File Offset: 0x000054D8
		public static void ReadInts(AnimationStream stream, NativeArray<PropertyStreamHandle> handles, NativeArray<int> buffer)
		{
			stream.CheckIsValid();
			int num = AnimationSceneHandleUtility.ValidateAndGetArrayCount<PropertyStreamHandle, int>(ref stream, handles, buffer);
			bool flag = num == 0;
			if (!flag)
			{
				AnimationStreamHandleUtility.ReadStreamIntsInternal(ref stream, handles.GetUnsafePtr<PropertyStreamHandle>(), buffer.GetUnsafePtr<int>(), num);
			}
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x00007318 File Offset: 0x00005518
		public static void ReadFloats(AnimationStream stream, NativeArray<PropertyStreamHandle> handles, NativeArray<float> buffer)
		{
			stream.CheckIsValid();
			int num = AnimationSceneHandleUtility.ValidateAndGetArrayCount<PropertyStreamHandle, float>(ref stream, handles, buffer);
			bool flag = num == 0;
			if (!flag)
			{
				AnimationStreamHandleUtility.ReadStreamFloatsInternal(ref stream, handles.GetUnsafePtr<PropertyStreamHandle>(), buffer.GetUnsafePtr<float>(), num);
			}
		}

		// Token: 0x060004EE RID: 1262
		[NativeMethod(Name = "AnimationHandleUtilityBindings::ReadStreamIntsInternal", IsFreeFunction = true, HasExplicitThis = false, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadStreamIntsInternal(ref AnimationStream stream, void* propertyStreamHandles, void* intBuffer, int count);

		// Token: 0x060004EF RID: 1263
		[NativeMethod(Name = "AnimationHandleUtilityBindings::ReadStreamFloatsInternal", IsFreeFunction = true, HasExplicitThis = false, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReadStreamFloatsInternal(ref AnimationStream stream, void* propertyStreamHandles, void* floatBuffer, int count);

		// Token: 0x060004F0 RID: 1264
		[NativeMethod(Name = "AnimationHandleUtilityBindings::WriteStreamIntsInternal", IsFreeFunction = true, HasExplicitThis = false, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void WriteStreamIntsInternal(ref AnimationStream stream, void* propertyStreamHandles, void* intBuffer, int count, bool useMask);

		// Token: 0x060004F1 RID: 1265
		[NativeMethod(Name = "AnimationHandleUtilityBindings::WriteStreamFloatsInternal", IsFreeFunction = true, HasExplicitThis = false, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void WriteStreamFloatsInternal(ref AnimationStream stream, void* propertyStreamHandles, void* floatBuffer, int count, bool useMask);
	}
}
