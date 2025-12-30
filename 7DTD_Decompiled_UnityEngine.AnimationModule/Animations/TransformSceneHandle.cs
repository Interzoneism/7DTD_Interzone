using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	// Token: 0x0200005C RID: 92
	[NativeHeader("Modules/Animation/Director/AnimationSceneHandles.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationStreamHandles.bindings.h")]
	public struct TransformSceneHandle
	{
		// Token: 0x060004A6 RID: 1190 RVA: 0x00006CF8 File Offset: 0x00004EF8
		public bool IsValid(AnimationStream stream)
		{
			return stream.isValid && this.createdByNative && this.hasTransformSceneHandleDefinitionIndex && this.HasValidTransform(ref stream);
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x00006D30 File Offset: 0x00004F30
		private bool createdByNative
		{
			get
			{
				return this.valid > 0U;
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x00006D4C File Offset: 0x00004F4C
		private bool hasTransformSceneHandleDefinitionIndex
		{
			get
			{
				return this.transformSceneHandleDefinitionIndex != -1;
			}
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x00006D6C File Offset: 0x00004F6C
		private void CheckIsValid(ref AnimationStream stream)
		{
			stream.CheckIsValid();
			bool flag = !this.createdByNative || !this.hasTransformSceneHandleDefinitionIndex;
			if (flag)
			{
				throw new InvalidOperationException("The TransformSceneHandle is invalid. Please use proper function to create the handle.");
			}
			bool flag2 = !this.HasValidTransform(ref stream);
			if (flag2)
			{
				throw new NullReferenceException("The transform is invalid.");
			}
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x00006DC0 File Offset: 0x00004FC0
		public Vector3 GetPosition(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetPositionInternal(ref stream);
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x00002059 File Offset: 0x00000259
		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetPosition(AnimationStream stream, Vector3 position)
		{
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00006DE4 File Offset: 0x00004FE4
		public Vector3 GetLocalPosition(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalPositionInternal(ref stream);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00002059 File Offset: 0x00000259
		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetLocalPosition(AnimationStream stream, Vector3 position)
		{
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00006E08 File Offset: 0x00005008
		public Quaternion GetRotation(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetRotationInternal(ref stream);
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00002059 File Offset: 0x00000259
		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetRotation(AnimationStream stream, Quaternion rotation)
		{
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00006E2C File Offset: 0x0000502C
		public Quaternion GetLocalRotation(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalRotationInternal(ref stream);
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x00002059 File Offset: 0x00000259
		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetLocalRotation(AnimationStream stream, Quaternion rotation)
		{
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x00006E50 File Offset: 0x00005050
		public Vector3 GetLocalScale(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalScaleInternal(ref stream);
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00006E73 File Offset: 0x00005073
		public void GetLocalTRS(AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			this.CheckIsValid(ref stream);
			this.GetLocalTRSInternal(ref stream, out position, out rotation, out scale);
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00006E8C File Offset: 0x0000508C
		public Matrix4x4 GetLocalToParentMatrix(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalToParentMatrixInternal(ref stream);
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x00006EAF File Offset: 0x000050AF
		public void GetGlobalTR(AnimationStream stream, out Vector3 position, out Quaternion rotation)
		{
			this.CheckIsValid(ref stream);
			this.GetGlobalTRInternal(ref stream, out position, out rotation);
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00006EC8 File Offset: 0x000050C8
		public Matrix4x4 GetLocalToWorldMatrix(AnimationStream stream)
		{
			this.CheckIsValid(ref stream);
			return this.GetLocalToWorldMatrixInternal(ref stream);
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00002059 File Offset: 0x00000259
		[Obsolete("SceneHandle is now read-only; it was problematic with the engine multithreading and determinism", true)]
		public void SetLocalScale(AnimationStream stream, Vector3 scale)
		{
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00006EEB File Offset: 0x000050EB
		[ThreadSafe]
		private bool HasValidTransform(ref AnimationStream stream)
		{
			return TransformSceneHandle.HasValidTransform_Injected(ref this, ref stream);
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x00006EF4 File Offset: 0x000050F4
		[NativeMethod(Name = "TransformSceneHandleBindings::GetPositionInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetPositionInternal(ref AnimationStream stream)
		{
			Vector3 result;
			TransformSceneHandle.GetPositionInternal_Injected(ref this, ref stream, out result);
			return result;
		}

		// Token: 0x060004BA RID: 1210 RVA: 0x00006F0C File Offset: 0x0000510C
		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalPositionInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLocalPositionInternal(ref AnimationStream stream)
		{
			Vector3 result;
			TransformSceneHandle.GetLocalPositionInternal_Injected(ref this, ref stream, out result);
			return result;
		}

		// Token: 0x060004BB RID: 1211 RVA: 0x00006F24 File Offset: 0x00005124
		[NativeMethod(Name = "TransformSceneHandleBindings::GetRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion GetRotationInternal(ref AnimationStream stream)
		{
			Quaternion result;
			TransformSceneHandle.GetRotationInternal_Injected(ref this, ref stream, out result);
			return result;
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x00006F3C File Offset: 0x0000513C
		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalRotationInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion GetLocalRotationInternal(ref AnimationStream stream)
		{
			Quaternion result;
			TransformSceneHandle.GetLocalRotationInternal_Injected(ref this, ref stream, out result);
			return result;
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x00006F54 File Offset: 0x00005154
		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalScaleInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLocalScaleInternal(ref AnimationStream stream)
		{
			Vector3 result;
			TransformSceneHandle.GetLocalScaleInternal_Injected(ref this, ref stream, out result);
			return result;
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x00006F6B File Offset: 0x0000516B
		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalTRSInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void GetLocalTRSInternal(ref AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale)
		{
			TransformSceneHandle.GetLocalTRSInternal_Injected(ref this, ref stream, out position, out rotation, out scale);
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x00006F78 File Offset: 0x00005178
		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalToParentMatrixInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Matrix4x4 GetLocalToParentMatrixInternal(ref AnimationStream stream)
		{
			Matrix4x4 result;
			TransformSceneHandle.GetLocalToParentMatrixInternal_Injected(ref this, ref stream, out result);
			return result;
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x00006F8F File Offset: 0x0000518F
		[NativeMethod(Name = "TransformSceneHandleBindings::GetGlobalTRInternal", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void GetGlobalTRInternal(ref AnimationStream stream, out Vector3 position, out Quaternion rotation)
		{
			TransformSceneHandle.GetGlobalTRInternal_Injected(ref this, ref stream, out position, out rotation);
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x00006F9C File Offset: 0x0000519C
		[NativeMethod(Name = "TransformSceneHandleBindings::GetLocalToWorldMatrixInternal", IsFreeFunction = true, HasExplicitThis = true, IsThreadSafe = true)]
		private Matrix4x4 GetLocalToWorldMatrixInternal(ref AnimationStream stream)
		{
			Matrix4x4 result;
			TransformSceneHandle.GetLocalToWorldMatrixInternal_Injected(ref this, ref stream, out result);
			return result;
		}

		// Token: 0x060004C2 RID: 1218
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasValidTransform_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream);

		// Token: 0x060004C3 RID: 1219
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		// Token: 0x060004C4 RID: 1220
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalPositionInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		// Token: 0x060004C5 RID: 1221
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		// Token: 0x060004C6 RID: 1222
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalRotationInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Quaternion ret);

		// Token: 0x060004C7 RID: 1223
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalScaleInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 ret);

		// Token: 0x060004C8 RID: 1224
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalTRSInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 position, out Quaternion rotation, out Vector3 scale);

		// Token: 0x060004C9 RID: 1225
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalToParentMatrixInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Matrix4x4 ret);

		// Token: 0x060004CA RID: 1226
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGlobalTRInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Vector3 position, out Quaternion rotation);

		// Token: 0x060004CB RID: 1227
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLocalToWorldMatrixInternal_Injected(ref TransformSceneHandle _unity_self, ref AnimationStream stream, out Matrix4x4 ret);

		// Token: 0x04000172 RID: 370
		private uint valid;

		// Token: 0x04000173 RID: 371
		private int transformSceneHandleDefinitionIndex;
	}
}
