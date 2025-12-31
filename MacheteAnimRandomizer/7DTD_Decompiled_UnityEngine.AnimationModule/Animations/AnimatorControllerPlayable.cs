using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Playables;
using UnityEngine.Scripting;

namespace UnityEngine.Animations
{
	// Token: 0x02000060 RID: 96
	[NativeHeader("Modules/Animation/ScriptBindings/Animator.bindings.h")]
	[NativeHeader("Modules/Animation/Director/AnimatorControllerPlayable.h")]
	[NativeHeader("Modules/Animation/RuntimeAnimatorController.h")]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimatorControllerPlayable.bindings.h")]
	[NativeHeader("Modules/Animation/AnimatorInfo.h")]
	[StaticAccessor("AnimatorControllerPlayableBindings", StaticAccessorType.DoubleColon)]
	[RequiredByNativeCode]
	public struct AnimatorControllerPlayable : IPlayable, IEquatable<AnimatorControllerPlayable>
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x00007358 File Offset: 0x00005558
		public static AnimatorControllerPlayable Null
		{
			get
			{
				return AnimatorControllerPlayable.m_NullPlayable;
			}
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00007370 File Offset: 0x00005570
		public static AnimatorControllerPlayable Create(PlayableGraph graph, RuntimeAnimatorController controller)
		{
			PlayableHandle handle = AnimatorControllerPlayable.CreateHandle(graph, controller);
			return new AnimatorControllerPlayable(handle);
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00007390 File Offset: 0x00005590
		private static PlayableHandle CreateHandle(PlayableGraph graph, RuntimeAnimatorController controller)
		{
			PlayableHandle @null = PlayableHandle.Null;
			bool flag = !AnimatorControllerPlayable.CreateHandleInternal(graph, controller, ref @null);
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

		// Token: 0x060004F5 RID: 1269 RVA: 0x000073C1 File Offset: 0x000055C1
		internal AnimatorControllerPlayable(PlayableHandle handle)
		{
			this.m_Handle = PlayableHandle.Null;
			this.SetHandle(handle);
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x000073D8 File Offset: 0x000055D8
		public PlayableHandle GetHandle()
		{
			return this.m_Handle;
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x000073F0 File Offset: 0x000055F0
		public void SetHandle(PlayableHandle handle)
		{
			bool flag = this.m_Handle.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("Cannot call IPlayable.SetHandle on an instance that already contains a valid handle.");
			}
			bool flag2 = handle.IsValid();
			if (flag2)
			{
				bool flag3 = !handle.IsPlayableOfType<AnimatorControllerPlayable>();
				if (flag3)
				{
					throw new InvalidCastException("Can't set handle: the playable is not an AnimatorControllerPlayable.");
				}
			}
			this.m_Handle = handle;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x00007448 File Offset: 0x00005648
		public static implicit operator Playable(AnimatorControllerPlayable playable)
		{
			return new Playable(playable.GetHandle());
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x00007468 File Offset: 0x00005668
		public static explicit operator AnimatorControllerPlayable(Playable playable)
		{
			return new AnimatorControllerPlayable(playable.GetHandle());
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00007488 File Offset: 0x00005688
		public bool Equals(AnimatorControllerPlayable other)
		{
			return this.GetHandle() == other.GetHandle();
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x000074AC File Offset: 0x000056AC
		public float GetFloat(string name)
		{
			return AnimatorControllerPlayable.GetFloatString(ref this.m_Handle, name);
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x000074CC File Offset: 0x000056CC
		public float GetFloat(int id)
		{
			return AnimatorControllerPlayable.GetFloatID(ref this.m_Handle, id);
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x000074EA File Offset: 0x000056EA
		public void SetFloat(string name, float value)
		{
			AnimatorControllerPlayable.SetFloatString(ref this.m_Handle, name, value);
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x000074FB File Offset: 0x000056FB
		public void SetFloat(int id, float value)
		{
			AnimatorControllerPlayable.SetFloatID(ref this.m_Handle, id, value);
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0000750C File Offset: 0x0000570C
		public bool GetBool(string name)
		{
			return AnimatorControllerPlayable.GetBoolString(ref this.m_Handle, name);
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0000752C File Offset: 0x0000572C
		public bool GetBool(int id)
		{
			return AnimatorControllerPlayable.GetBoolID(ref this.m_Handle, id);
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0000754A File Offset: 0x0000574A
		public void SetBool(string name, bool value)
		{
			AnimatorControllerPlayable.SetBoolString(ref this.m_Handle, name, value);
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0000755B File Offset: 0x0000575B
		public void SetBool(int id, bool value)
		{
			AnimatorControllerPlayable.SetBoolID(ref this.m_Handle, id, value);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0000756C File Offset: 0x0000576C
		public int GetInteger(string name)
		{
			return AnimatorControllerPlayable.GetIntegerString(ref this.m_Handle, name);
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0000758C File Offset: 0x0000578C
		public int GetInteger(int id)
		{
			return AnimatorControllerPlayable.GetIntegerID(ref this.m_Handle, id);
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x000075AA File Offset: 0x000057AA
		public void SetInteger(string name, int value)
		{
			AnimatorControllerPlayable.SetIntegerString(ref this.m_Handle, name, value);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x000075BB File Offset: 0x000057BB
		public void SetInteger(int id, int value)
		{
			AnimatorControllerPlayable.SetIntegerID(ref this.m_Handle, id, value);
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x000075CC File Offset: 0x000057CC
		public void SetTrigger(string name)
		{
			AnimatorControllerPlayable.SetTriggerString(ref this.m_Handle, name);
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x000075DC File Offset: 0x000057DC
		public void SetTrigger(int id)
		{
			AnimatorControllerPlayable.SetTriggerID(ref this.m_Handle, id);
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x000075EC File Offset: 0x000057EC
		public void ResetTrigger(string name)
		{
			AnimatorControllerPlayable.ResetTriggerString(ref this.m_Handle, name);
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x000075FC File Offset: 0x000057FC
		public void ResetTrigger(int id)
		{
			AnimatorControllerPlayable.ResetTriggerID(ref this.m_Handle, id);
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x0000760C File Offset: 0x0000580C
		public bool IsParameterControlledByCurve(string name)
		{
			return AnimatorControllerPlayable.IsParameterControlledByCurveString(ref this.m_Handle, name);
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x0000762C File Offset: 0x0000582C
		public bool IsParameterControlledByCurve(int id)
		{
			return AnimatorControllerPlayable.IsParameterControlledByCurveID(ref this.m_Handle, id);
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x0000764C File Offset: 0x0000584C
		public int GetLayerCount()
		{
			return AnimatorControllerPlayable.GetLayerCountInternal(ref this.m_Handle);
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0000766C File Offset: 0x0000586C
		public string GetLayerName(int layerIndex)
		{
			return AnimatorControllerPlayable.GetLayerNameInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0000768C File Offset: 0x0000588C
		public int GetLayerIndex(string layerName)
		{
			return AnimatorControllerPlayable.GetLayerIndexInternal(ref this.m_Handle, layerName);
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x000076AC File Offset: 0x000058AC
		public float GetLayerWeight(int layerIndex)
		{
			return AnimatorControllerPlayable.GetLayerWeightInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x000076CA File Offset: 0x000058CA
		public void SetLayerWeight(int layerIndex, float weight)
		{
			AnimatorControllerPlayable.SetLayerWeightInternal(ref this.m_Handle, layerIndex, weight);
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x000076DC File Offset: 0x000058DC
		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetCurrentAnimatorStateInfoInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x000076FC File Offset: 0x000058FC
		public AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetNextAnimatorStateInfoInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0000771C File Offset: 0x0000591C
		public AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetAnimatorTransitionInfoInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x06000515 RID: 1301 RVA: 0x0000773C File Offset: 0x0000593C
		public AnimatorClipInfo[] GetCurrentAnimatorClipInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetCurrentAnimatorClipInfoInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x06000516 RID: 1302 RVA: 0x0000775C File Offset: 0x0000595C
		public void GetCurrentAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			bool flag = clips == null;
			if (flag)
			{
				throw new ArgumentNullException("clips");
			}
			AnimatorControllerPlayable.GetAnimatorClipInfoInternal(ref this.m_Handle, layerIndex, true, clips);
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0000778C File Offset: 0x0000598C
		public void GetNextAnimatorClipInfo(int layerIndex, List<AnimatorClipInfo> clips)
		{
			bool flag = clips == null;
			if (flag)
			{
				throw new ArgumentNullException("clips");
			}
			AnimatorControllerPlayable.GetAnimatorClipInfoInternal(ref this.m_Handle, layerIndex, false, clips);
		}

		// Token: 0x06000518 RID: 1304
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex, bool isCurrent, object clips);

		// Token: 0x06000519 RID: 1305 RVA: 0x000077BC File Offset: 0x000059BC
		public int GetCurrentAnimatorClipInfoCount(int layerIndex)
		{
			return AnimatorControllerPlayable.GetAnimatorClipInfoCountInternal(ref this.m_Handle, layerIndex, true);
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x000077DC File Offset: 0x000059DC
		public int GetNextAnimatorClipInfoCount(int layerIndex)
		{
			return AnimatorControllerPlayable.GetAnimatorClipInfoCountInternal(ref this.m_Handle, layerIndex, false);
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x000077FC File Offset: 0x000059FC
		public AnimatorClipInfo[] GetNextAnimatorClipInfo(int layerIndex)
		{
			return AnimatorControllerPlayable.GetNextAnimatorClipInfoInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0000781C File Offset: 0x00005A1C
		public bool IsInTransition(int layerIndex)
		{
			return AnimatorControllerPlayable.IsInTransitionInternal(ref this.m_Handle, layerIndex);
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0000783C File Offset: 0x00005A3C
		public int GetParameterCount()
		{
			return AnimatorControllerPlayable.GetParameterCountInternal(ref this.m_Handle);
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0000785C File Offset: 0x00005A5C
		public AnimatorControllerParameter GetParameter(int index)
		{
			AnimatorControllerParameter parameterInternal = AnimatorControllerPlayable.GetParameterInternal(ref this.m_Handle, index);
			bool flag = parameterInternal.m_Type == (AnimatorControllerParameterType)0;
			if (flag)
			{
				throw new IndexOutOfRangeException("Invalid parameter index.");
			}
			return parameterInternal;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x00007894 File Offset: 0x00005A94
		public void CrossFadeInFixedTime(string stateName, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, -1, 0f);
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x000078B0 File Offset: 0x00005AB0
		public void CrossFadeInFixedTime(string stateName, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, 0f);
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x000078CC File Offset: 0x00005ACC
		public void CrossFadeInFixedTime(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, fixedTime);
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x000078E5 File Offset: 0x00005AE5
		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, stateNameHash, transitionDuration, -1, 0f);
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x000078FC File Offset: 0x00005AFC
		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, 0f);
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00007913 File Offset: 0x00005B13
		public void CrossFadeInFixedTime(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("0.0f")] float fixedTime)
		{
			AnimatorControllerPlayable.CrossFadeInFixedTimeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, fixedTime);
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00007927 File Offset: 0x00005B27
		public void CrossFade(string stateName, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, -1, float.NegativeInfinity);
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00007943 File Offset: 0x00005B43
		public void CrossFade(string stateName, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, float.NegativeInfinity);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x0000795F File Offset: 0x00005B5F
		public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), transitionDuration, layer, normalizedTime);
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00007978 File Offset: 0x00005B78
		public void CrossFade(int stateNameHash, float transitionDuration)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, stateNameHash, transitionDuration, -1, float.NegativeInfinity);
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0000798F File Offset: 0x00005B8F
		public void CrossFade(int stateNameHash, float transitionDuration, int layer)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, float.NegativeInfinity);
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x000079A6 File Offset: 0x00005BA6
		public void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.CrossFadeInternal(ref this.m_Handle, stateNameHash, transitionDuration, layer, normalizedTime);
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x000079BA File Offset: 0x00005BBA
		public void PlayInFixedTime(string stateName)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), -1, float.NegativeInfinity);
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x000079D5 File Offset: 0x00005BD5
		public void PlayInFixedTime(string stateName, int layer)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, float.NegativeInfinity);
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x000079F0 File Offset: 0x00005BF0
		public void PlayInFixedTime(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, fixedTime);
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x00007A07 File Offset: 0x00005C07
		public void PlayInFixedTime(int stateNameHash)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, stateNameHash, -1, float.NegativeInfinity);
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x00007A1D File Offset: 0x00005C1D
		public void PlayInFixedTime(int stateNameHash, int layer)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, stateNameHash, layer, float.NegativeInfinity);
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x00007A33 File Offset: 0x00005C33
		public void PlayInFixedTime(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float fixedTime)
		{
			AnimatorControllerPlayable.PlayInFixedTimeInternal(ref this.m_Handle, stateNameHash, layer, fixedTime);
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x00007A45 File Offset: 0x00005C45
		public void Play(string stateName)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), -1, float.NegativeInfinity);
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00007A60 File Offset: 0x00005C60
		public void Play(string stateName, int layer)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, float.NegativeInfinity);
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00007A7B File Offset: 0x00005C7B
		public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, AnimatorControllerPlayable.StringToHash(stateName), layer, normalizedTime);
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x00007A92 File Offset: 0x00005C92
		public void Play(int stateNameHash)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, stateNameHash, -1, float.NegativeInfinity);
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x00007AA8 File Offset: 0x00005CA8
		public void Play(int stateNameHash, int layer)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, stateNameHash, layer, float.NegativeInfinity);
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x00007ABE File Offset: 0x00005CBE
		public void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			AnimatorControllerPlayable.PlayInternal(ref this.m_Handle, stateNameHash, layer, normalizedTime);
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x00007AD0 File Offset: 0x00005CD0
		public bool HasState(int layerIndex, int stateID)
		{
			return AnimatorControllerPlayable.HasStateInternal(ref this.m_Handle, layerIndex, stateID);
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x00007AF0 File Offset: 0x00005CF0
		internal string ResolveHash(int hash)
		{
			return AnimatorControllerPlayable.ResolveHashInternal(ref this.m_Handle, hash);
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00007B0E File Offset: 0x00005D0E
		[NativeThrows]
		private static bool CreateHandleInternal(PlayableGraph graph, RuntimeAnimatorController controller, ref PlayableHandle handle)
		{
			return AnimatorControllerPlayable.CreateHandleInternal_Injected(ref graph, controller, ref handle);
		}

		// Token: 0x0600053A RID: 1338
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RuntimeAnimatorController GetAnimatorControllerInternal(ref PlayableHandle handle);

		// Token: 0x0600053B RID: 1339
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerCountInternal(ref PlayableHandle handle);

		// Token: 0x0600053C RID: 1340
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetLayerNameInternal(ref PlayableHandle handle, int layerIndex);

		// Token: 0x0600053D RID: 1341
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerIndexInternal(ref PlayableHandle handle, string layerName);

		// Token: 0x0600053E RID: 1342
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetLayerWeightInternal(ref PlayableHandle handle, int layerIndex);

		// Token: 0x0600053F RID: 1343
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLayerWeightInternal(ref PlayableHandle handle, int layerIndex, float weight);

		// Token: 0x06000540 RID: 1344 RVA: 0x00007B1C File Offset: 0x00005D1C
		[NativeThrows]
		private static AnimatorStateInfo GetCurrentAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorStateInfo result;
			AnimatorControllerPlayable.GetCurrentAnimatorStateInfoInternal_Injected(ref handle, layerIndex, out result);
			return result;
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x00007B34 File Offset: 0x00005D34
		[NativeThrows]
		private static AnimatorStateInfo GetNextAnimatorStateInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorStateInfo result;
			AnimatorControllerPlayable.GetNextAnimatorStateInfoInternal_Injected(ref handle, layerIndex, out result);
			return result;
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x00007B4C File Offset: 0x00005D4C
		[NativeThrows]
		private static AnimatorTransitionInfo GetAnimatorTransitionInfoInternal(ref PlayableHandle handle, int layerIndex)
		{
			AnimatorTransitionInfo result;
			AnimatorControllerPlayable.GetAnimatorTransitionInfoInternal_Injected(ref handle, layerIndex, out result);
			return result;
		}

		// Token: 0x06000543 RID: 1347
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorClipInfo[] GetCurrentAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex);

		// Token: 0x06000544 RID: 1348
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAnimatorClipInfoCountInternal(ref PlayableHandle handle, int layerIndex, bool current);

		// Token: 0x06000545 RID: 1349
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorClipInfo[] GetNextAnimatorClipInfoInternal(ref PlayableHandle handle, int layerIndex);

		// Token: 0x06000546 RID: 1350
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ResolveHashInternal(ref PlayableHandle handle, int hash);

		// Token: 0x06000547 RID: 1351
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsInTransitionInternal(ref PlayableHandle handle, int layerIndex);

		// Token: 0x06000548 RID: 1352
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimatorControllerParameter GetParameterInternal(ref PlayableHandle handle, int index);

		// Token: 0x06000549 RID: 1353
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetParameterCountInternal(ref PlayableHandle handle);

		// Token: 0x0600054A RID: 1354
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int StringToHash(string name);

		// Token: 0x0600054B RID: 1355
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFadeInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer, float fixedTime);

		// Token: 0x0600054C RID: 1356
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CrossFadeInternal(ref PlayableHandle handle, int stateNameHash, float transitionDuration, int layer, float normalizedTime);

		// Token: 0x0600054D RID: 1357
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayInFixedTimeInternal(ref PlayableHandle handle, int stateNameHash, int layer, float fixedTime);

		// Token: 0x0600054E RID: 1358
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PlayInternal(ref PlayableHandle handle, int stateNameHash, int layer, float normalizedTime);

		// Token: 0x0600054F RID: 1359
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasStateInternal(ref PlayableHandle handle, int layerIndex, int stateID);

		// Token: 0x06000550 RID: 1360
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatString(ref PlayableHandle handle, string name, float value);

		// Token: 0x06000551 RID: 1361
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFloatID(ref PlayableHandle handle, int id, float value);

		// Token: 0x06000552 RID: 1362
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatString(ref PlayableHandle handle, string name);

		// Token: 0x06000553 RID: 1363
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFloatID(ref PlayableHandle handle, int id);

		// Token: 0x06000554 RID: 1364
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolString(ref PlayableHandle handle, string name, bool value);

		// Token: 0x06000555 RID: 1365
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoolID(ref PlayableHandle handle, int id, bool value);

		// Token: 0x06000556 RID: 1366
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolString(ref PlayableHandle handle, string name);

		// Token: 0x06000557 RID: 1367
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetBoolID(ref PlayableHandle handle, int id);

		// Token: 0x06000558 RID: 1368
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerString(ref PlayableHandle handle, string name, int value);

		// Token: 0x06000559 RID: 1369
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetIntegerID(ref PlayableHandle handle, int id, int value);

		// Token: 0x0600055A RID: 1370
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerString(ref PlayableHandle handle, string name);

		// Token: 0x0600055B RID: 1371
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIntegerID(ref PlayableHandle handle, int id);

		// Token: 0x0600055C RID: 1372
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerString(ref PlayableHandle handle, string name);

		// Token: 0x0600055D RID: 1373
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTriggerID(ref PlayableHandle handle, int id);

		// Token: 0x0600055E RID: 1374
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerString(ref PlayableHandle handle, string name);

		// Token: 0x0600055F RID: 1375
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetTriggerID(ref PlayableHandle handle, int id);

		// Token: 0x06000560 RID: 1376
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveString(ref PlayableHandle handle, string name);

		// Token: 0x06000561 RID: 1377
		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsParameterControlledByCurveID(ref PlayableHandle handle, int id);

		// Token: 0x06000563 RID: 1379
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateHandleInternal_Injected(ref PlayableGraph graph, RuntimeAnimatorController controller, ref PlayableHandle handle);

		// Token: 0x06000564 RID: 1380
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCurrentAnimatorStateInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out AnimatorStateInfo ret);

		// Token: 0x06000565 RID: 1381
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNextAnimatorStateInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out AnimatorStateInfo ret);

		// Token: 0x06000566 RID: 1382
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAnimatorTransitionInfoInternal_Injected(ref PlayableHandle handle, int layerIndex, out AnimatorTransitionInfo ret);

		// Token: 0x04000176 RID: 374
		private PlayableHandle m_Handle;

		// Token: 0x04000177 RID: 375
		private static readonly AnimatorControllerPlayable m_NullPlayable = new AnimatorControllerPlayable(PlayableHandle.Null);
	}
}
