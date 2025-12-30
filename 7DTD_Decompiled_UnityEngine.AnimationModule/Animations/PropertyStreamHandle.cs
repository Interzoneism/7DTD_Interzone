using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	// Token: 0x0200005B RID: 91
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/Director/AnimationStreamHandles.h")]
	public struct PropertyStreamHandle
	{
		// Token: 0x06000482 RID: 1154 RVA: 0x000068C8 File Offset: 0x00004AC8
		public bool IsValid(AnimationStream stream)
		{
			return this.IsValidInternal(ref stream);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x000068E4 File Offset: 0x00004AE4
		private bool IsValidInternal(ref AnimationStream stream)
		{
			return stream.isValid && this.createdByNative && this.hasHandleIndex && this.hasBindType;
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000484 RID: 1156 RVA: 0x00006918 File Offset: 0x00004B18
		private bool createdByNative
		{
			get
			{
				return this.animatorBindingsVersion > 0U;
			}
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x00006934 File Offset: 0x00004B34
		private bool IsSameVersionAsStream(ref AnimationStream stream)
		{
			return this.animatorBindingsVersion == stream.animatorBindingsVersion;
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x00006954 File Offset: 0x00004B54
		private bool hasHandleIndex
		{
			get
			{
				return this.handleIndex != -1;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x00006974 File Offset: 0x00004B74
		private bool hasValueArrayIndex
		{
			get
			{
				return this.valueArrayIndex != -1;
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000488 RID: 1160 RVA: 0x00006994 File Offset: 0x00004B94
		private bool hasBindType
		{
			get
			{
				return this.bindType != 0;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x000069BC File Offset: 0x00004BBC
		// (set) Token: 0x06000489 RID: 1161 RVA: 0x000069AF File Offset: 0x00004BAF
		internal uint animatorBindingsVersion
		{
			get
			{
				return this.m_AnimatorBindingsVersion;
			}
			private set
			{
				this.m_AnimatorBindingsVersion = value;
			}
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x000069D4 File Offset: 0x00004BD4
		public void Resolve(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x000069E0 File Offset: 0x00004BE0
		public bool IsResolved(AnimationStream stream)
		{
			return this.IsResolvedInternal(ref stream);
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x000069FC File Offset: 0x00004BFC
		private bool IsResolvedInternal(ref AnimationStream stream)
		{
			return this.IsValidInternal(ref stream) && this.IsSameVersionAsStream(ref stream) && this.hasValueArrayIndex;
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00006A2C File Offset: 0x00004C2C
		private void CheckIsValidAndResolve(ref AnimationStream stream)
		{
			stream.CheckIsValid();
			bool flag = this.IsResolvedInternal(ref stream);
			if (!flag)
			{
				bool flag2 = !this.createdByNative || !this.hasHandleIndex || !this.hasBindType;
				if (flag2)
				{
					throw new InvalidOperationException("The PropertyStreamHandle is invalid. Please use proper function to create the handle.");
				}
				bool flag3 = !this.IsSameVersionAsStream(ref stream) || (this.hasHandleIndex && !this.hasValueArrayIndex);
				if (flag3)
				{
					this.ResolveInternal(ref stream);
				}
				bool flag4 = this.hasHandleIndex && !this.hasValueArrayIndex;
				if (flag4)
				{
					throw new InvalidOperationException("The PropertyStreamHandle cannot be resolved.");
				}
			}
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00006ACC File Offset: 0x00004CCC
		public float GetFloat(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 5;
			if (flag)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetFloatInternal(ref stream);
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00006B0C File Offset: 0x00004D0C
		public void SetFloat(AnimationStream stream, float value)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 5;
			if (flag)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetFloatInternal(ref stream, value);
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00006B48 File Offset: 0x00004D48
		public int GetInt(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 10 && this.bindType != 11 && this.bindType != 9;
			if (flag)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetIntInternal(ref stream);
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00006BA0 File Offset: 0x00004DA0
		public void SetInt(AnimationStream stream, int value)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 10 && this.bindType != 11 && this.bindType != 9;
			if (flag)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetIntInternal(ref stream, value);
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00006BF4 File Offset: 0x00004DF4
		public bool GetBool(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 6 && this.bindType != 7;
			if (flag)
			{
				throw new InvalidOperationException("GetValue type doesn't match PropertyStreamHandle bound type.");
			}
			return this.GetBoolInternal(ref stream);
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x00006C40 File Offset: 0x00004E40
		public void SetBool(AnimationStream stream, bool value)
		{
			this.CheckIsValidAndResolve(ref stream);
			bool flag = this.bindType != 6 && this.bindType != 7;
			if (flag)
			{
				throw new InvalidOperationException("SetValue type doesn't match PropertyStreamHandle bound type.");
			}
			this.SetBoolInternal(ref stream, value);
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x00006C88 File Offset: 0x00004E88
		public bool GetReadMask(AnimationStream stream)
		{
			this.CheckIsValidAndResolve(ref stream);
			return this.GetReadMaskInternal(ref stream);
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x00006CAB File Offset: 0x00004EAB
		[NativeMethod(Name = "Resolve", IsThreadSafe = true)]
		private void ResolveInternal(ref AnimationStream stream)
		{
			PropertyStreamHandle.ResolveInternal_Injected(ref this, ref stream);
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x00006CB4 File Offset: 0x00004EB4
		[NativeMethod(Name = "GetFloat", IsThreadSafe = true)]
		private float GetFloatInternal(ref AnimationStream stream)
		{
			return PropertyStreamHandle.GetFloatInternal_Injected(ref this, ref stream);
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x00006CBD File Offset: 0x00004EBD
		[NativeMethod(Name = "SetFloat", IsThreadSafe = true)]
		private void SetFloatInternal(ref AnimationStream stream, float value)
		{
			PropertyStreamHandle.SetFloatInternal_Injected(ref this, ref stream, value);
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00006CC7 File Offset: 0x00004EC7
		[NativeMethod(Name = "GetInt", IsThreadSafe = true)]
		private int GetIntInternal(ref AnimationStream stream)
		{
			return PropertyStreamHandle.GetIntInternal_Injected(ref this, ref stream);
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00006CD0 File Offset: 0x00004ED0
		[NativeMethod(Name = "SetInt", IsThreadSafe = true)]
		private void SetIntInternal(ref AnimationStream stream, int value)
		{
			PropertyStreamHandle.SetIntInternal_Injected(ref this, ref stream, value);
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x00006CDA File Offset: 0x00004EDA
		[NativeMethod(Name = "GetBool", IsThreadSafe = true)]
		private bool GetBoolInternal(ref AnimationStream stream)
		{
			return PropertyStreamHandle.GetBoolInternal_Injected(ref this, ref stream);
		}

		// Token: 0x0600049C RID: 1180 RVA: 0x00006CE3 File Offset: 0x00004EE3
		[NativeMethod(Name = "SetBool", IsThreadSafe = true)]
		private void SetBoolInternal(ref AnimationStream stream, bool value)
		{
			PropertyStreamHandle.SetBoolInternal_Injected(ref this, ref stream, value);
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x00006CED File Offset: 0x00004EED
		[NativeMethod(Name = "GetReadMask", IsThreadSafe = true)]
		private bool GetReadMaskInternal(ref AnimationStream stream)
		{
			return PropertyStreamHandle.GetReadMaskInternal_Injected(ref this, ref stream);
		}

		// Token: 0x0600049E RID: 1182
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		// Token: 0x0600049F RID: 1183
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		// Token: 0x060004A0 RID: 1184
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, float value);

		// Token: 0x060004A1 RID: 1185
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		// Token: 0x060004A2 RID: 1186
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, int value);

		// Token: 0x060004A3 RID: 1187
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		// Token: 0x060004A4 RID: 1188
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream, bool value);

		// Token: 0x060004A5 RID: 1189
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetReadMaskInternal_Injected(ref PropertyStreamHandle _unity_self, ref AnimationStream stream);

		// Token: 0x0400016E RID: 366
		private uint m_AnimatorBindingsVersion;

		// Token: 0x0400016F RID: 367
		private int handleIndex;

		// Token: 0x04000170 RID: 368
		private int valueArrayIndex;

		// Token: 0x04000171 RID: 369
		private int bindType;
	}
}
