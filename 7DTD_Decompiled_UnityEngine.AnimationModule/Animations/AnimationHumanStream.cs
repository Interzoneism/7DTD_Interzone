using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	// Token: 0x0200004C RID: 76
	[RequiredByNativeCode]
	[NativeHeader("Modules/Animation/Director/AnimationHumanStream.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationHumanStream.bindings.h")]
	public struct AnimationHumanStream
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000305 RID: 773 RVA: 0x00004B4C File Offset: 0x00002D4C
		public bool isValid
		{
			get
			{
				return this.stream != IntPtr.Zero;
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00004B70 File Offset: 0x00002D70
		private void ThrowIfInvalid()
		{
			bool flag = !this.isValid;
			if (flag)
			{
				throw new InvalidOperationException("The AnimationHumanStream is invalid.");
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000307 RID: 775 RVA: 0x00004B98 File Offset: 0x00002D98
		public float humanScale
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetHumanScale();
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000308 RID: 776 RVA: 0x00004BB8 File Offset: 0x00002DB8
		public float leftFootHeight
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetFootHeight(true);
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000309 RID: 777 RVA: 0x00004BD8 File Offset: 0x00002DD8
		public float rightFootHeight
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetFootHeight(false);
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600030A RID: 778 RVA: 0x00004BF8 File Offset: 0x00002DF8
		// (set) Token: 0x0600030B RID: 779 RVA: 0x00004C17 File Offset: 0x00002E17
		public Vector3 bodyLocalPosition
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyLocalPosition();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyLocalPosition(value);
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600030C RID: 780 RVA: 0x00004C2C File Offset: 0x00002E2C
		// (set) Token: 0x0600030D RID: 781 RVA: 0x00004C4B File Offset: 0x00002E4B
		public Quaternion bodyLocalRotation
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyLocalRotation();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyLocalRotation(value);
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600030E RID: 782 RVA: 0x00004C60 File Offset: 0x00002E60
		// (set) Token: 0x0600030F RID: 783 RVA: 0x00004C7F File Offset: 0x00002E7F
		public Vector3 bodyPosition
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyPosition();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyPosition(value);
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000310 RID: 784 RVA: 0x00004C94 File Offset: 0x00002E94
		// (set) Token: 0x06000311 RID: 785 RVA: 0x00004CB3 File Offset: 0x00002EB3
		public Quaternion bodyRotation
		{
			get
			{
				this.ThrowIfInvalid();
				return this.InternalGetBodyRotation();
			}
			set
			{
				this.ThrowIfInvalid();
				this.InternalSetBodyRotation(value);
			}
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00004CC8 File Offset: 0x00002EC8
		public float GetMuscle(MuscleHandle muscle)
		{
			this.ThrowIfInvalid();
			return this.InternalGetMuscle(muscle);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00004CE8 File Offset: 0x00002EE8
		public void SetMuscle(MuscleHandle muscle, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetMuscle(muscle, value);
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000314 RID: 788 RVA: 0x00004CFC File Offset: 0x00002EFC
		public Vector3 leftFootVelocity
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetLeftFootVelocity();
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000315 RID: 789 RVA: 0x00004D1C File Offset: 0x00002F1C
		public Vector3 rightFootVelocity
		{
			get
			{
				this.ThrowIfInvalid();
				return this.GetRightFootVelocity();
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00004D3B File Offset: 0x00002F3B
		public void ResetToStancePose()
		{
			this.ThrowIfInvalid();
			this.InternalResetToStancePose();
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00004D4C File Offset: 0x00002F4C
		public Vector3 GetGoalPositionFromPose(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalPositionFromPose(index);
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00004D6C File Offset: 0x00002F6C
		public Quaternion GetGoalRotationFromPose(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalRotationFromPose(index);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00004D8C File Offset: 0x00002F8C
		public Vector3 GetGoalLocalPosition(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalLocalPosition(index);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00004DAC File Offset: 0x00002FAC
		public void SetGoalLocalPosition(AvatarIKGoal index, Vector3 pos)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalLocalPosition(index, pos);
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00004DC0 File Offset: 0x00002FC0
		public Quaternion GetGoalLocalRotation(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalLocalRotation(index);
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00004DE0 File Offset: 0x00002FE0
		public void SetGoalLocalRotation(AvatarIKGoal index, Quaternion rot)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalLocalRotation(index, rot);
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00004DF4 File Offset: 0x00002FF4
		public Vector3 GetGoalPosition(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalPosition(index);
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00004E14 File Offset: 0x00003014
		public void SetGoalPosition(AvatarIKGoal index, Vector3 pos)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalPosition(index, pos);
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00004E28 File Offset: 0x00003028
		public Quaternion GetGoalRotation(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalRotation(index);
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00004E48 File Offset: 0x00003048
		public void SetGoalRotation(AvatarIKGoal index, Quaternion rot)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalRotation(index, rot);
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00004E5B File Offset: 0x0000305B
		public void SetGoalWeightPosition(AvatarIKGoal index, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalWeightPosition(index, value);
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00004E6E File Offset: 0x0000306E
		public void SetGoalWeightRotation(AvatarIKGoal index, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetGoalWeightRotation(index, value);
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00004E84 File Offset: 0x00003084
		public float GetGoalWeightPosition(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalWeightPosition(index);
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00004EA4 File Offset: 0x000030A4
		public float GetGoalWeightRotation(AvatarIKGoal index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetGoalWeightRotation(index);
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00004EC4 File Offset: 0x000030C4
		public Vector3 GetHintPosition(AvatarIKHint index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetHintPosition(index);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00004EE4 File Offset: 0x000030E4
		public void SetHintPosition(AvatarIKHint index, Vector3 pos)
		{
			this.ThrowIfInvalid();
			this.InternalSetHintPosition(index, pos);
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00004EF7 File Offset: 0x000030F7
		public void SetHintWeightPosition(AvatarIKHint index, float value)
		{
			this.ThrowIfInvalid();
			this.InternalSetHintWeightPosition(index, value);
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00004F0C File Offset: 0x0000310C
		public float GetHintWeightPosition(AvatarIKHint index)
		{
			this.ThrowIfInvalid();
			return this.InternalGetHintWeightPosition(index);
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00004F2C File Offset: 0x0000312C
		public void SetLookAtPosition(Vector3 lookAtPosition)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtPosition(lookAtPosition);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00004F3E File Offset: 0x0000313E
		public void SetLookAtClampWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtClampWeight(weight);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00004F50 File Offset: 0x00003150
		public void SetLookAtBodyWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtBodyWeight(weight);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00004F62 File Offset: 0x00003162
		public void SetLookAtHeadWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtHeadWeight(weight);
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00004F74 File Offset: 0x00003174
		public void SetLookAtEyesWeight(float weight)
		{
			this.ThrowIfInvalid();
			this.InternalSetLookAtEyesWeight(weight);
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00004F86 File Offset: 0x00003186
		public void SolveIK()
		{
			this.ThrowIfInvalid();
			this.InternalSolveIK();
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00004F97 File Offset: 0x00003197
		[NativeMethod(IsThreadSafe = true)]
		private float GetHumanScale()
		{
			return AnimationHumanStream.GetHumanScale_Injected(ref this);
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00004F9F File Offset: 0x0000319F
		[NativeMethod(IsThreadSafe = true)]
		private float GetFootHeight(bool left)
		{
			return AnimationHumanStream.GetFootHeight_Injected(ref this, left);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00004FA8 File Offset: 0x000031A8
		[NativeMethod(Name = "ResetToStancePose", IsThreadSafe = true)]
		private void InternalResetToStancePose()
		{
			AnimationHumanStream.InternalResetToStancePose_Injected(ref this);
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00004FB0 File Offset: 0x000031B0
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalPositionFromPose", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetGoalPositionFromPose(AvatarIKGoal index)
		{
			Vector3 result;
			AnimationHumanStream.InternalGetGoalPositionFromPose_Injected(ref this, index, out result);
			return result;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00004FC8 File Offset: 0x000031C8
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalRotationFromPose", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetGoalRotationFromPose(AvatarIKGoal index)
		{
			Quaternion result;
			AnimationHumanStream.InternalGetGoalRotationFromPose_Injected(ref this, index, out result);
			return result;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00004FE0 File Offset: 0x000031E0
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetBodyLocalPosition()
		{
			Vector3 result;
			AnimationHumanStream.InternalGetBodyLocalPosition_Injected(ref this, out result);
			return result;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00004FF6 File Offset: 0x000031F6
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyLocalPosition(Vector3 value)
		{
			AnimationHumanStream.InternalSetBodyLocalPosition_Injected(ref this, ref value);
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00005000 File Offset: 0x00003200
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetBodyLocalRotation()
		{
			Quaternion result;
			AnimationHumanStream.InternalGetBodyLocalRotation_Injected(ref this, out result);
			return result;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00005016 File Offset: 0x00003216
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyLocalRotation(Quaternion value)
		{
			AnimationHumanStream.InternalSetBodyLocalRotation_Injected(ref this, ref value);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00005020 File Offset: 0x00003220
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetBodyPosition()
		{
			Vector3 result;
			AnimationHumanStream.InternalGetBodyPosition_Injected(ref this, out result);
			return result;
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00005036 File Offset: 0x00003236
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyPosition(Vector3 value)
		{
			AnimationHumanStream.InternalSetBodyPosition_Injected(ref this, ref value);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00005040 File Offset: 0x00003240
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetBodyRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetBodyRotation()
		{
			Quaternion result;
			AnimationHumanStream.InternalGetBodyRotation_Injected(ref this, out result);
			return result;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00005056 File Offset: 0x00003256
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetBodyRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetBodyRotation(Quaternion value)
		{
			AnimationHumanStream.InternalSetBodyRotation_Injected(ref this, ref value);
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00005060 File Offset: 0x00003260
		[NativeMethod(Name = "GetMuscle", IsThreadSafe = true)]
		private float InternalGetMuscle(MuscleHandle muscle)
		{
			return AnimationHumanStream.InternalGetMuscle_Injected(ref this, ref muscle);
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0000506A File Offset: 0x0000326A
		[NativeMethod(Name = "SetMuscle", IsThreadSafe = true)]
		private void InternalSetMuscle(MuscleHandle muscle, float value)
		{
			AnimationHumanStream.InternalSetMuscle_Injected(ref this, ref muscle, value);
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00005078 File Offset: 0x00003278
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetLeftFootVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetLeftFootVelocity()
		{
			Vector3 result;
			AnimationHumanStream.GetLeftFootVelocity_Injected(ref this, out result);
			return result;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00005090 File Offset: 0x00003290
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetRightFootVelocity", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 GetRightFootVelocity()
		{
			Vector3 result;
			AnimationHumanStream.GetRightFootVelocity_Injected(ref this, out result);
			return result;
		}

		// Token: 0x06000340 RID: 832 RVA: 0x000050A8 File Offset: 0x000032A8
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetGoalLocalPosition(AvatarIKGoal index)
		{
			Vector3 result;
			AnimationHumanStream.InternalGetGoalLocalPosition_Injected(ref this, index, out result);
			return result;
		}

		// Token: 0x06000341 RID: 833 RVA: 0x000050BF File Offset: 0x000032BF
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalLocalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalLocalPosition(AvatarIKGoal index, Vector3 pos)
		{
			AnimationHumanStream.InternalSetGoalLocalPosition_Injected(ref this, index, ref pos);
		}

		// Token: 0x06000342 RID: 834 RVA: 0x000050CC File Offset: 0x000032CC
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetGoalLocalRotation(AvatarIKGoal index)
		{
			Quaternion result;
			AnimationHumanStream.InternalGetGoalLocalRotation_Injected(ref this, index, out result);
			return result;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x000050E3 File Offset: 0x000032E3
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalLocalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalLocalRotation(AvatarIKGoal index, Quaternion rot)
		{
			AnimationHumanStream.InternalSetGoalLocalRotation_Injected(ref this, index, ref rot);
		}

		// Token: 0x06000344 RID: 836 RVA: 0x000050F0 File Offset: 0x000032F0
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetGoalPosition(AvatarIKGoal index)
		{
			Vector3 result;
			AnimationHumanStream.InternalGetGoalPosition_Injected(ref this, index, out result);
			return result;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00005107 File Offset: 0x00003307
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalPosition(AvatarIKGoal index, Vector3 pos)
		{
			AnimationHumanStream.InternalSetGoalPosition_Injected(ref this, index, ref pos);
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00005114 File Offset: 0x00003314
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetGoalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Quaternion InternalGetGoalRotation(AvatarIKGoal index)
		{
			Quaternion result;
			AnimationHumanStream.InternalGetGoalRotation_Injected(ref this, index, out result);
			return result;
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0000512B File Offset: 0x0000332B
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetGoalRotation", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetGoalRotation(AvatarIKGoal index, Quaternion rot)
		{
			AnimationHumanStream.InternalSetGoalRotation_Injected(ref this, index, ref rot);
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00005136 File Offset: 0x00003336
		[NativeMethod(Name = "SetGoalWeightPosition", IsThreadSafe = true)]
		private void InternalSetGoalWeightPosition(AvatarIKGoal index, float value)
		{
			AnimationHumanStream.InternalSetGoalWeightPosition_Injected(ref this, index, value);
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00005140 File Offset: 0x00003340
		[NativeMethod(Name = "SetGoalWeightRotation", IsThreadSafe = true)]
		private void InternalSetGoalWeightRotation(AvatarIKGoal index, float value)
		{
			AnimationHumanStream.InternalSetGoalWeightRotation_Injected(ref this, index, value);
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0000514A File Offset: 0x0000334A
		[NativeMethod(Name = "GetGoalWeightPosition", IsThreadSafe = true)]
		private float InternalGetGoalWeightPosition(AvatarIKGoal index)
		{
			return AnimationHumanStream.InternalGetGoalWeightPosition_Injected(ref this, index);
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00005153 File Offset: 0x00003353
		[NativeMethod(Name = "GetGoalWeightRotation", IsThreadSafe = true)]
		private float InternalGetGoalWeightRotation(AvatarIKGoal index)
		{
			return AnimationHumanStream.InternalGetGoalWeightRotation_Injected(ref this, index);
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0000515C File Offset: 0x0000335C
		[NativeMethod(Name = "AnimationHumanStreamBindings::GetHintPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private Vector3 InternalGetHintPosition(AvatarIKHint index)
		{
			Vector3 result;
			AnimationHumanStream.InternalGetHintPosition_Injected(ref this, index, out result);
			return result;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00005173 File Offset: 0x00003373
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetHintPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetHintPosition(AvatarIKHint index, Vector3 pos)
		{
			AnimationHumanStream.InternalSetHintPosition_Injected(ref this, index, ref pos);
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0000517E File Offset: 0x0000337E
		[NativeMethod(Name = "SetHintWeightPosition", IsThreadSafe = true)]
		private void InternalSetHintWeightPosition(AvatarIKHint index, float value)
		{
			AnimationHumanStream.InternalSetHintWeightPosition_Injected(ref this, index, value);
		}

		// Token: 0x0600034F RID: 847 RVA: 0x00005188 File Offset: 0x00003388
		[NativeMethod(Name = "GetHintWeightPosition", IsThreadSafe = true)]
		private float InternalGetHintWeightPosition(AvatarIKHint index)
		{
			return AnimationHumanStream.InternalGetHintWeightPosition_Injected(ref this, index);
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00005191 File Offset: 0x00003391
		[NativeMethod(Name = "AnimationHumanStreamBindings::SetLookAtPosition", IsFreeFunction = true, IsThreadSafe = true, HasExplicitThis = true)]
		private void InternalSetLookAtPosition(Vector3 lookAtPosition)
		{
			AnimationHumanStream.InternalSetLookAtPosition_Injected(ref this, ref lookAtPosition);
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0000519B File Offset: 0x0000339B
		[NativeMethod(Name = "SetLookAtClampWeight", IsThreadSafe = true)]
		private void InternalSetLookAtClampWeight(float weight)
		{
			AnimationHumanStream.InternalSetLookAtClampWeight_Injected(ref this, weight);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x000051A4 File Offset: 0x000033A4
		[NativeMethod(Name = "SetLookAtBodyWeight", IsThreadSafe = true)]
		private void InternalSetLookAtBodyWeight(float weight)
		{
			AnimationHumanStream.InternalSetLookAtBodyWeight_Injected(ref this, weight);
		}

		// Token: 0x06000353 RID: 851 RVA: 0x000051AD File Offset: 0x000033AD
		[NativeMethod(Name = "SetLookAtHeadWeight", IsThreadSafe = true)]
		private void InternalSetLookAtHeadWeight(float weight)
		{
			AnimationHumanStream.InternalSetLookAtHeadWeight_Injected(ref this, weight);
		}

		// Token: 0x06000354 RID: 852 RVA: 0x000051B6 File Offset: 0x000033B6
		[NativeMethod(Name = "SetLookAtEyesWeight", IsThreadSafe = true)]
		private void InternalSetLookAtEyesWeight(float weight)
		{
			AnimationHumanStream.InternalSetLookAtEyesWeight_Injected(ref this, weight);
		}

		// Token: 0x06000355 RID: 853 RVA: 0x000051BF File Offset: 0x000033BF
		[NativeMethod(Name = "SolveIK", IsThreadSafe = true)]
		private void InternalSolveIK()
		{
			AnimationHumanStream.InternalSolveIK_Injected(ref this);
		}

		// Token: 0x06000356 RID: 854
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetHumanScale_Injected(ref AnimationHumanStream _unity_self);

		// Token: 0x06000357 RID: 855
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetFootHeight_Injected(ref AnimationHumanStream _unity_self, bool left);

		// Token: 0x06000358 RID: 856
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalResetToStancePose_Injected(ref AnimationHumanStream _unity_self);

		// Token: 0x06000359 RID: 857
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalPositionFromPose_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

		// Token: 0x0600035A RID: 858
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalRotationFromPose_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

		// Token: 0x0600035B RID: 859
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyLocalPosition_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		// Token: 0x0600035C RID: 860
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyLocalPosition_Injected(ref AnimationHumanStream _unity_self, ref Vector3 value);

		// Token: 0x0600035D RID: 861
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyLocalRotation_Injected(ref AnimationHumanStream _unity_self, out Quaternion ret);

		// Token: 0x0600035E RID: 862
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyLocalRotation_Injected(ref AnimationHumanStream _unity_self, ref Quaternion value);

		// Token: 0x0600035F RID: 863
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyPosition_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		// Token: 0x06000360 RID: 864
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyPosition_Injected(ref AnimationHumanStream _unity_self, ref Vector3 value);

		// Token: 0x06000361 RID: 865
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetBodyRotation_Injected(ref AnimationHumanStream _unity_self, out Quaternion ret);

		// Token: 0x06000362 RID: 866
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetBodyRotation_Injected(ref AnimationHumanStream _unity_self, ref Quaternion value);

		// Token: 0x06000363 RID: 867
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float InternalGetMuscle_Injected(ref AnimationHumanStream _unity_self, ref MuscleHandle muscle);

		// Token: 0x06000364 RID: 868
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetMuscle_Injected(ref AnimationHumanStream _unity_self, ref MuscleHandle muscle, float value);

		// Token: 0x06000365 RID: 869
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLeftFootVelocity_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		// Token: 0x06000366 RID: 870
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRightFootVelocity_Injected(ref AnimationHumanStream _unity_self, out Vector3 ret);

		// Token: 0x06000367 RID: 871
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalLocalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

		// Token: 0x06000368 RID: 872
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalLocalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, ref Vector3 pos);

		// Token: 0x06000369 RID: 873
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalLocalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

		// Token: 0x0600036A RID: 874
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalLocalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, ref Quaternion rot);

		// Token: 0x0600036B RID: 875
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Vector3 ret);

		// Token: 0x0600036C RID: 876
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, ref Vector3 pos);

		// Token: 0x0600036D RID: 877
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetGoalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, out Quaternion ret);

		// Token: 0x0600036E RID: 878
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, ref Quaternion rot);

		// Token: 0x0600036F RID: 879
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalWeightPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, float value);

		// Token: 0x06000370 RID: 880
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGoalWeightRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index, float value);

		// Token: 0x06000371 RID: 881
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float InternalGetGoalWeightPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index);

		// Token: 0x06000372 RID: 882
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float InternalGetGoalWeightRotation_Injected(ref AnimationHumanStream _unity_self, AvatarIKGoal index);

		// Token: 0x06000373 RID: 883
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetHintPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index, out Vector3 ret);

		// Token: 0x06000374 RID: 884
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetHintPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index, ref Vector3 pos);

		// Token: 0x06000375 RID: 885
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetHintWeightPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index, float value);

		// Token: 0x06000376 RID: 886
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float InternalGetHintWeightPosition_Injected(ref AnimationHumanStream _unity_self, AvatarIKHint index);

		// Token: 0x06000377 RID: 887
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetLookAtPosition_Injected(ref AnimationHumanStream _unity_self, ref Vector3 lookAtPosition);

		// Token: 0x06000378 RID: 888
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetLookAtClampWeight_Injected(ref AnimationHumanStream _unity_self, float weight);

		// Token: 0x06000379 RID: 889
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetLookAtBodyWeight_Injected(ref AnimationHumanStream _unity_self, float weight);

		// Token: 0x0600037A RID: 890
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetLookAtHeadWeight_Injected(ref AnimationHumanStream _unity_self, float weight);

		// Token: 0x0600037B RID: 891
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetLookAtEyesWeight_Injected(ref AnimationHumanStream _unity_self, float weight);

		// Token: 0x0600037C RID: 892
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSolveIK_Injected(ref AnimationHumanStream _unity_self);

		// Token: 0x04000147 RID: 327
		private IntPtr stream;
	}
}
