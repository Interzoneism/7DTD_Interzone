using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000AA RID: 170
public abstract class AvatarController : MonoBehaviour
{
	// Token: 0x06000344 RID: 836 RVA: 0x000190AF File Offset: 0x000172AF
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		AvatarController.StaticInit();
		this.entity = base.GetComponent<EntityAlive>();
	}

	// Token: 0x06000345 RID: 837 RVA: 0x000190C4 File Offset: 0x000172C4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void StaticInit()
	{
		if (AvatarController.initialized)
		{
			return;
		}
		AvatarController.initialized = true;
		AvatarController.hashNames = new Dictionary<int, string>();
		AvatarController.AssignAnimatorHash(ref AvatarController.attackHash, "Attack");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackBlendHash, "AttackBlend");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackStartHash, "AttackStart");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackReadyHash, "AttackReady");
		AvatarController.AssignAnimatorHash(ref AvatarController.meleeAttackSpeedHash, "MeleeAttackSpeed");
		AvatarController.AssignAnimatorHash(ref AvatarController.deathHash, "Death");
		AvatarController.AssignAnimatorHash(ref AvatarController.digHash, "Dig");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitStartHash, "HitStart");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitHash, "Hit");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpHash, "Jump");
		AvatarController.AssignAnimatorHash(ref AvatarController.moveHash, "Move");
		AvatarController.AssignAnimatorHash(ref AvatarController.stunHash, "Stun");
		AvatarController.AssignAnimatorHash(ref AvatarController.beginCorpseEatHash, "BeginCorpseEat");
		AvatarController.AssignAnimatorHash(ref AvatarController.endCorpseEatHash, "EndCorpseEat");
		AvatarController.AssignAnimatorHash(ref AvatarController.forwardHash, "Forward");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitBodyPartHash, "HitBodyPart");
		AvatarController.AssignAnimatorHash(ref AvatarController.idleTimeHash, "IdleTime");
		AvatarController.AssignAnimatorHash(ref AvatarController.isAimingHash, "IsAiming");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemUseHash, "ItemUse");
		AvatarController.AssignAnimatorHash(ref AvatarController.movementStateHash, "MovementState");
		AvatarController.AssignAnimatorHash(ref AvatarController.rotationPitchHash, "RotationPitch");
		AvatarController.AssignAnimatorHash(ref AvatarController.strafeHash, "Strafe");
		AvatarController.AssignAnimatorHash(ref AvatarController.swimSelectHash, "SwimSelect");
		AvatarController.AssignAnimatorHash(ref AvatarController.turnRateHash, "TurnRate");
		AvatarController.AssignAnimatorHash(ref AvatarController.walkTypeHash, "WalkType");
		AvatarController.AssignAnimatorHash(ref AvatarController.walkTypeBlendHash, "WalkTypeBlend");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponHoldTypeHash, "WeaponHoldType");
		AvatarController.AssignAnimatorHash(ref AvatarController.isAliveHash, "IsAlive");
		AvatarController.AssignAnimatorHash(ref AvatarController.isDeadHash, "IsDead");
		AvatarController.AssignAnimatorHash(ref AvatarController.isFPVHash, "IsFPV");
		AvatarController.AssignAnimatorHash(ref AvatarController.isMovingHash, "IsMoving");
		AvatarController.AssignAnimatorHash(ref AvatarController.isSwimHash, "IsSwim");
		AvatarController.AssignAnimatorHash(ref AvatarController.attackTriggerHash, "AttackTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.deathTriggerHash, "DeathTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitTriggerHash, "HitTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.movementTriggerHash, "MovementTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.electrocuteTriggerHash, "ElectrocuteTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.painTriggerHash, "PainTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemHasChangedTriggerHash, "ItemHasChangedTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemThrownAwayTriggerHash, "ItemThrownAwayTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.dodgeBlendHash, "DodgeBlend");
		AvatarController.AssignAnimatorHash(ref AvatarController.dodgeTriggerHash, "DodgeTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.reactionTriggerHash, "ReactionTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.reactionTypeHash, "ReactionType");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperPoseHash, "SleeperPose");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperTriggerHash, "SleeperTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpLandResponseHash, "JumpLandResponse");
		AvatarController.AssignAnimatorHash(ref AvatarController.forcedRootMotionHash, "ForcedRootMotion");
		AvatarController.AssignAnimatorHash(ref AvatarController.preventAttackHash, "PreventAttack");
		AvatarController.AssignAnimatorHash(ref AvatarController.canFallHash, "CanFall");
		AvatarController.AssignAnimatorHash(ref AvatarController.isOnGroundHash, "IsOnGround");
		AvatarController.AssignAnimatorHash(ref AvatarController.triggerAliveHash, "TriggerAlive");
		AvatarController.AssignAnimatorHash(ref AvatarController.bodyPartHitHash, "BodyPartHit");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitDirectionHash, "HitDirection");
		AvatarController.AssignAnimatorHash(ref AvatarController.criticalHitHash, "CriticalHit");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitDamageHash, "HitDamage");
		AvatarController.AssignAnimatorHash(ref AvatarController.randomHash, "Random");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpStartHash, "JumpStart");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpLandHash, "JumpLand");
		AvatarController.AssignAnimatorHash(ref AvatarController.isMaleHash, "IsMale");
		AvatarController.AssignAnimatorHash(ref AvatarController.specialAttack2Hash, "SpecialAttack2");
		AvatarController.AssignAnimatorHash(ref AvatarController.rageHash, "Rage");
		AvatarController.AssignAnimatorHash(ref AvatarController.stunTypeHash, "StunType");
		AvatarController.AssignAnimatorHash(ref AvatarController.stunBodyPartHash, "StunBodyPart");
		AvatarController.AssignAnimatorHash(ref AvatarController.isCriticalHash, "isCritical");
		AvatarController.AssignAnimatorHash(ref AvatarController.HitRandomValueHash, "HitRandomValue");
		AvatarController.AssignAnimatorHash(ref AvatarController.beginStunTriggerHash, "BeginStunTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.endStunTriggerHash, "EndStunTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.toCrawlerTriggerHash, "ToCrawlerTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.isElectrocutedHash, "IsElectrocuted");
		AvatarController.AssignAnimatorHash(ref AvatarController.isClimbingHash, "IsClimbing");
		AvatarController.AssignAnimatorHash(ref AvatarController.verticalSpeedHash, "VerticalSpeed");
		AvatarController.AssignAnimatorHash(ref AvatarController.reviveHash, "Revive");
		AvatarController.AssignAnimatorHash(ref AvatarController.harvestingHash, "Harvesting");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponFireHash, "WeaponFire");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponPreFireCancelHash, "WeaponPreFireCancel");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponPreFireHash, "WeaponPreFire");
		AvatarController.AssignAnimatorHash(ref AvatarController.weaponAmmoRemaining, "WeaponAmmoRemaining");
		AvatarController.AssignAnimatorHash(ref AvatarController.useItemHash, "UseItem");
		AvatarController.AssignAnimatorHash(ref AvatarController.itemActionIndexHash, "ItemActionIndex");
		AvatarController.AssignAnimatorHash(ref AvatarController.isCrouchingHash, "IsCrouching");
		AvatarController.AssignAnimatorHash(ref AvatarController.reloadHash, "Reload");
		AvatarController.AssignAnimatorHash(ref AvatarController.reloadSpeedHash, "ReloadSpeed");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpTriggerHash, "JumpTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.inAirHash, "InAir");
		AvatarController.AssignAnimatorHash(ref AvatarController.jumpLandHash, "JumpLand");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitRandomValueHash, "HitRandomValue");
		AvatarController.AssignAnimatorHash(ref AvatarController.hitTriggerHash, "HitTrigger");
		AvatarController.AssignAnimatorHash(ref AvatarController.archetypeStanceHash, "ArchetypeStance");
		AvatarController.AssignAnimatorHash(ref AvatarController.yLookHash, "YLook");
		AvatarController.AssignAnimatorHash(ref AvatarController.vehiclePoseHash, "VehiclePose");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleBackHash, "SleeperIdleBack");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleSideLeftHash, "SleeperIdleSideLeft");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleSideRightHash, "SleeperIdleSideRight");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleSitHash, "SleeperIdleSit");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleStandHash, "SleeperIdleStand");
		AvatarController.AssignAnimatorHash(ref AvatarController.sleeperIdleStomachHash, "SleeperIdleStomach");
	}

	// Token: 0x06000346 RID: 838 RVA: 0x00019698 File Offset: 0x00017898
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void AssignAnimatorHash(ref int hash, string parameterName)
	{
		hash = Animator.StringToHash(parameterName);
		if (!AvatarController.hashNames.ContainsKey(hash))
		{
			AvatarController.hashNames.Add(hash, parameterName);
		}
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void assignStates()
	{
	}

	// Token: 0x06000348 RID: 840 RVA: 0x000196BD File Offset: 0x000178BD
	public virtual Animator GetAnimator()
	{
		return this.anim;
	}

	// Token: 0x06000349 RID: 841 RVA: 0x000196C5 File Offset: 0x000178C5
	public void SetAnimator(Transform _animT)
	{
		this.SetAnimator(_animT.GetComponent<Animator>());
	}

	// Token: 0x0600034A RID: 842 RVA: 0x000196D4 File Offset: 0x000178D4
	public void SetAnimator(Animator _anim)
	{
		if (this.anim == _anim)
		{
			return;
		}
		this.anim = _anim;
		if (this.anim)
		{
			this.anim.logWarnings = false;
			AnimatorControllerParameter[] parameters = this.anim.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].nameHash == AvatarController.turnRateHash)
				{
					this.hasTurnRate = true;
				}
			}
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x0600034B RID: 843 RVA: 0x00019740 File Offset: 0x00017940
	public EntityAlive Entity
	{
		get
		{
			return this.entity;
		}
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00019748 File Offset: 0x00017948
	public bool IsMoving(float forwardSpeed, float strafeSpeed)
	{
		return forwardSpeed * forwardSpeed + strafeSpeed * strafeSpeed > 0.0001f;
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00019758 File Offset: 0x00017958
	public virtual void NotifyAnimatorMove(Animator instigator)
	{
		this.entity.NotifyRootMotion(instigator);
	}

	// Token: 0x0600034E RID: 846
	public abstract Transform GetActiveModelRoot();

	// Token: 0x0600034F RID: 847 RVA: 0x00019766 File Offset: 0x00017966
	public virtual Transform GetRightHandTransform()
	{
		return null;
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00019766 File Offset: 0x00017966
	public Texture2D GetTexture()
	{
		return null;
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0001976C File Offset: 0x0001796C
	public virtual void ResetAnimations()
	{
		Animator animator = this.GetAnimator();
		if (animator)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.enabled = true;
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00019796 File Offset: 0x00017996
	public virtual void SetMeleeAttackSpeed(float _speed)
	{
		this.UpdateFloat(AvatarController.meleeAttackSpeedHash, _speed, true);
	}

	// Token: 0x06000353 RID: 851
	public abstract bool IsAnimationAttackPlaying();

	// Token: 0x06000354 RID: 852
	public abstract void StartAnimationAttack();

	// Token: 0x06000355 RID: 853 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetInAir(bool inAir)
	{
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetAttackImpact()
	{
	}

	// Token: 0x06000357 RID: 855 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsAttackImpact()
	{
		return true;
	}

	// Token: 0x06000358 RID: 856 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsAnimationWithMotionRunning()
	{
		return true;
	}

	// Token: 0x06000359 RID: 857 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual AvatarController.ActionState GetActionState()
	{
		return AvatarController.ActionState.None;
	}

	// Token: 0x0600035A RID: 858 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsActionActive()
	{
		return false;
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAction(int _animType)
	{
	}

	// Token: 0x0600035C RID: 860 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationSpecialAttackPlaying()
	{
		return false;
	}

	// Token: 0x0600035D RID: 861 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationSpecialAttack(bool _b, int _animType)
	{
	}

	// Token: 0x0600035E RID: 862 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationSpecialAttack2Playing()
	{
		return false;
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationSpecialAttack2()
	{
	}

	// Token: 0x06000360 RID: 864 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationRagingPlaying()
	{
		return false;
	}

	// Token: 0x06000361 RID: 865 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationRaging()
	{
	}

	// Token: 0x06000362 RID: 866 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationFiring()
	{
	}

	// Token: 0x06000363 RID: 867 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationHitRunning()
	{
		return false;
	}

	// Token: 0x06000364 RID: 868 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
	}

	// Token: 0x06000365 RID: 869 RVA: 0x000197A8 File Offset: 0x000179A8
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool CheckHit(float duration)
	{
		return this.hitWeight < 0.15f || duration > this.hitDuration || !this.IsAnimationHitRunning();
	}

	// Token: 0x06000366 RID: 870 RVA: 0x000197CC File Offset: 0x000179CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitHitDuration(float duration)
	{
		if (this.hitWeight > 0.15f)
		{
			float num = 0.2f;
			if (duration == 0f)
			{
				num = 0.1f;
			}
			if (this.hitWeightTarget > this.hitWeight)
			{
				this.hitWeightTarget += num;
				if (this.hitWeightTarget > this.hitWeightMax)
				{
					this.hitWeightTarget = this.hitWeightMax;
				}
			}
			this.hitWeight += num;
			if (this.hitWeight > this.hitWeightMax)
			{
				this.hitWeight = this.hitWeightMax;
			}
			return;
		}
		duration = Utils.FastMax(duration, 0.120000005f);
		this.hitDuration = duration;
		float num2 = Utils.FastMin(duration * 0.25f, 0.1f);
		this.hitDurationOut = duration - num2;
		this.hitWeightTarget = num2 / 0.1f;
		this.hitWeightTarget = Utils.FastClamp(this.hitWeightTarget, 0.2f, 0.8f);
		this.hitWeightDuration = num2 / Utils.FastMax(0.01f, this.hitWeightTarget - this.hitWeight);
		if (this.hitWeight == 0f)
		{
			this.anim.SetLayerWeight(this.hitLayerIndex, 0.01f);
		}
	}

	// Token: 0x06000367 RID: 871 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationHarvestingPlaying()
	{
		return false;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationHarvesting(float _length, bool _weaponFireTrigger)
	{
	}

	// Token: 0x06000369 RID: 873 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationDigRunning()
	{
		return false;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationDodge(float _blend)
	{
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationToDodge()
	{
		return false;
	}

	// Token: 0x0600036C RID: 876 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationJumping()
	{
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationJump(AnimJumpMode jumpMode)
	{
	}

	// Token: 0x0600036E RID: 878 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationJumpRunning()
	{
		return false;
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetSwim(bool _enable)
	{
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000198F2 File Offset: 0x00017AF2
	public virtual float GetAnimationElectrocuteRemaining()
	{
		return this.electrocuteTime;
	}

	// Token: 0x06000371 RID: 881 RVA: 0x000198FA File Offset: 0x00017AFA
	public virtual void StartAnimationElectrocute(float _duration)
	{
		this.electrocuteTime = _duration;
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Electrocute(bool enabled)
	{
	}

	// Token: 0x06000373 RID: 883 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationReloading()
	{
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00019903 File Offset: 0x00017B03
	public void SetReloadBool(bool value)
	{
		this._setBool(AvatarController.reloadHash, value, true);
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
	}

	// Token: 0x06000376 RID: 886 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationUsePlaying()
	{
		return false;
	}

	// Token: 0x06000377 RID: 887 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartAnimationUse()
	{
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
	}

	// Token: 0x06000379 RID: 889 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetAiming(bool _bEnable)
	{
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00019912 File Offset: 0x00017B12
	public virtual void SetAlive()
	{
		if (this.anim != null)
		{
			this._setBool(AvatarController.isAliveHash, true, true);
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetCrouching(bool _bEnable)
	{
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetDrunk(float _numBeers)
	{
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetInRightHand(Transform _transform)
	{
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetLookPosition(Vector3 _pos)
	{
	}

	// Token: 0x0600037F RID: 895 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetVehicleAnimation(int _animHash, int _pose)
	{
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00019930 File Offset: 0x00017B30
	public virtual int GetVehicleAnimation()
	{
		int result;
		if (this.TryGetInt(AvatarController.vehiclePoseHash, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetRagdollEnabled(bool _b)
	{
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetWalkingSpeed(float _f)
	{
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00019950 File Offset: 0x00017B50
	public virtual void SetWalkType(int _walkType, bool _trigger = false)
	{
		this._setInt(AvatarController.walkTypeHash, _walkType, true);
		if (_walkType >= 20)
		{
			this._setFloat(AvatarController.walkTypeBlendHash, 1f, true);
		}
		else if (_walkType > 0)
		{
			this._setFloat(AvatarController.walkTypeBlendHash, 0f, true);
		}
		if (_trigger)
		{
			this._setTrigger(AvatarController.movementTriggerHash, true);
		}
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetHeadAngles(float _nick, float _yaw)
	{
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetArmsAngles(float _rightArmAngle, float _leftArmAngle)
	{
	}

	// Token: 0x06000386 RID: 902
	public abstract void SetVisible(bool _b);

	// Token: 0x06000387 RID: 903 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetArchetypeStance(NPCInfo.StanceTypes stance)
	{
	}

	// Token: 0x06000388 RID: 904 RVA: 0x000199A6 File Offset: 0x00017BA6
	public virtual void TriggerReaction(int reaction)
	{
		if (this.anim != null)
		{
			this._setInt(AvatarController.reactionTypeHash, reaction, true);
			this._setTrigger(AvatarController.reactionTriggerHash, true);
		}
	}

	// Token: 0x06000389 RID: 905 RVA: 0x000199CF File Offset: 0x00017BCF
	public virtual void TriggerSleeperPose(int pose, bool returningToSleep = false)
	{
		if (this.anim != null)
		{
			this._setInt(AvatarController.sleeperPoseHash, pose, true);
			this._setTrigger(AvatarController.sleeperTriggerHash, true);
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void RemoveLimb(BodyDamage _bodyDamage, bool restoreState)
	{
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void CrippleLimb(BodyDamage _bodyDamage, bool restoreState)
	{
	}

	// Token: 0x0600038C RID: 908 RVA: 0x000199F8 File Offset: 0x00017BF8
	public virtual void DismemberLimb(BodyDamage _bodyDamage, bool restoreState)
	{
		if (_bodyDamage.IsCrippled)
		{
			this.CrippleLimb(_bodyDamage, restoreState);
		}
		if (_bodyDamage.bodyPartHit != EnumBodyPartHit.None)
		{
			this.RemoveLimb(_bodyDamage, restoreState);
		}
	}

	// Token: 0x0600038D RID: 909 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void TurnIntoCrawler(bool restoreState)
	{
	}

	// Token: 0x0600038E RID: 910 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void BeginStun(EnumEntityStunType stun, EnumBodyPartHit _bodyPart, Utils.EnumHitDirection _hitDirection, bool _criticalHit, float random)
	{
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void EndStun()
	{
	}

	// Token: 0x06000390 RID: 912 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationStunRunning()
	{
		return false;
	}

	// Token: 0x06000391 RID: 913 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StartEating()
	{
	}

	// Token: 0x06000392 RID: 914 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void StopEating()
	{
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void PlayPlayerFPRevive()
	{
	}

	// Token: 0x06000394 RID: 916 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsAnimationPlayerFPRevivePlaying()
	{
		return false;
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00019A1B File Offset: 0x00017C1B
	public bool IsRootMotionForced()
	{
		return this.anim != null && this.anim.GetFloat(AvatarController.forcedRootMotionHash) > 0f;
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00019A44 File Offset: 0x00017C44
	public bool IsAttackPrevented()
	{
		return this.anim != null && this.anim.GetFloat(AvatarController.preventAttackHash) > 0f;
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00019A6D File Offset: 0x00017C6D
	public virtual void SetFallAndGround(bool _canFall, bool _onGnd)
	{
		this._setBool(AvatarController.canFallHash, _canFall, false);
		this._setBool(AvatarController.isOnGroundHash, _onGnd, false);
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00019A8C File Offset: 0x00017C8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FixedUpdate()
	{
		if (this.hasTurnRate)
		{
			float y = this.entity.transform.eulerAngles.y;
			float num = Mathf.DeltaAngle(y, this.turnRateFacing) * 50f;
			if ((num > 5f && this.turnRate >= 0f) || (num < -5f && this.turnRate <= 0f))
			{
				float num2 = Utils.FastAbs(num) - Utils.FastAbs(this.turnRate);
				if (num2 > 0f)
				{
					this.turnRate = Utils.FastLerpUnclamped(this.turnRate, num, 0.2f);
				}
				else if (num2 < -50f)
				{
					this.turnRate = Utils.FastLerpUnclamped(this.turnRate, num, 0.05f);
				}
			}
			else
			{
				this.turnRate *= 0.92f;
				this.turnRate = Utils.FastMoveTowards(this.turnRate, 0f, 2f);
			}
			this.turnRateFacing = y;
			this._setFloat(AvatarController.turnRateHash, this.turnRate, false);
		}
		this.updateNetworkAnimData();
	}

	// Token: 0x06000399 RID: 921 RVA: 0x00019B98 File Offset: 0x00017D98
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.electrocuteTime > 0f)
		{
			this.electrocuteTime -= deltaTime;
		}
		else if (this.electrocuteTime <= 0f)
		{
			this.Electrocute(false);
			this.electrocuteTime = 0f;
		}
		if (this.hitLayerIndex >= 0)
		{
			if (this.hitWeightTarget > 0f && this.hitWeight == this.hitWeightTarget)
			{
				if (this.hitDuration > 999f)
				{
					if (!this.IsAnimationHitRunning() || this.entity.IsDead() || this.entity.emodel.IsRagdollActive)
					{
						this.hitWeightDuration = 0.4f;
						this.hitWeightTarget = 0f;
					}
				}
				else if (this.hitWeightTarget > 0.15f)
				{
					this.hitWeightDuration = (this.hitDurationOut + 0.2f) / (this.hitWeight - 0.15f);
					this.hitWeightTarget = 0.15f;
				}
				else
				{
					this.hitWeightDuration = 4f;
					this.hitWeightTarget = 0f;
				}
			}
			if (this.hitWeight != this.hitWeightTarget)
			{
				this.hitWeight = Mathf.MoveTowards(this.hitWeight, this.hitWeightTarget, deltaTime / this.hitWeightDuration);
				this.anim.SetLayerWeight(this.hitLayerIndex, this.hitWeight);
			}
		}
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00019CF4 File Offset: 0x00017EF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void processAnimParamData(List<AnimParamData> animationParameterData)
	{
		for (int i = 0; i < animationParameterData.Count; i++)
		{
			int nameHash = animationParameterData[i].NameHash;
			switch (animationParameterData[i].ValueType)
			{
			case AnimParamData.ValueTypes.Bool:
				this.UpdateBool(nameHash, animationParameterData[i].IntValue != 0, true);
				break;
			case AnimParamData.ValueTypes.Trigger:
				if (animationParameterData[i].IntValue != 0)
				{
					this.TriggerEvent(nameHash);
				}
				else
				{
					this.CancelEvent(nameHash);
				}
				break;
			case AnimParamData.ValueTypes.Float:
				this.UpdateFloat(nameHash, animationParameterData[i].FloatValue, true);
				break;
			case AnimParamData.ValueTypes.Int:
				this.UpdateInt(nameHash, animationParameterData[i].IntValue, true);
				break;
			case AnimParamData.ValueTypes.DataFloat:
				this.SetDataFloat((AvatarController.DataTypes)nameHash, animationParameterData[i].FloatValue, true);
				break;
			}
		}
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00019DC7 File Offset: 0x00017FC7
	public void TriggerEvent(string _property)
	{
		this._setTrigger(_property, true);
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00019DD1 File Offset: 0x00017FD1
	public void TriggerEvent(int _pid)
	{
		this._setTrigger(_pid, true);
	}

	// Token: 0x0600039D RID: 925 RVA: 0x00019DDB File Offset: 0x00017FDB
	public void CancelEvent(string _property)
	{
		this._resetTrigger(_property, true);
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00019DE5 File Offset: 0x00017FE5
	public void CancelEvent(int _pid)
	{
		this._resetTrigger(_pid, true);
	}

	// Token: 0x0600039F RID: 927 RVA: 0x00019DEF File Offset: 0x00017FEF
	public void UpdateFloat(string _property, float _value, bool _netsync = true)
	{
		this._setFloat(_property, _value, _netsync);
	}

	// Token: 0x060003A0 RID: 928 RVA: 0x00019DFA File Offset: 0x00017FFA
	public void UpdateFloat(int _pid, float _value, bool _netsync = true)
	{
		this._setFloat(_pid, _value, _netsync);
	}

	// Token: 0x060003A1 RID: 929 RVA: 0x00019E05 File Offset: 0x00018005
	public void UpdateBool(string _property, bool _value, bool _netsync = true)
	{
		this._setBool(_property, _value, _netsync);
	}

	// Token: 0x060003A2 RID: 930 RVA: 0x00019E10 File Offset: 0x00018010
	public void UpdateBool(int _pid, bool _value, bool _netsync = true)
	{
		this._setBool(_pid, _value, _netsync);
	}

	// Token: 0x060003A3 RID: 931 RVA: 0x00019E1B File Offset: 0x0001801B
	public void UpdateInt(string _property, int _value, bool _netsync = true)
	{
		this._setInt(_property, _value, _netsync);
	}

	// Token: 0x060003A4 RID: 932 RVA: 0x00019E26 File Offset: 0x00018026
	public void UpdateInt(int _pid, int _value, bool _netsync = true)
	{
		this._setInt(_pid, _value, _netsync);
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x00019E31 File Offset: 0x00018031
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setTrigger(string _property, bool _netsync = true)
	{
		this._setTrigger(Animator.StringToHash(_property), _netsync);
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x00019E40 File Offset: 0x00018040
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setTrigger(int _pid, bool _netsync = true)
	{
		if (this.anim != null)
		{
			this.anim.SetTrigger(_pid);
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_pid, AnimParamData.ValueTypes.Trigger, true));
			}
			this.OnTrigger(_pid);
		}
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnTrigger(int _id)
	{
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x00019E93 File Offset: 0x00018093
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _resetTrigger(string _property, bool _netsync = true)
	{
		this._resetTrigger(Animator.StringToHash(_property), _netsync);
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x00019EA4 File Offset: 0x000180A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _resetTrigger(int _propertyHash, bool _netsync = true)
	{
		if (this.anim != null && this.anim.gameObject.activeSelf && this.anim.GetBool(_propertyHash))
		{
			this.anim.ResetTrigger(_propertyHash);
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Trigger, false));
			}
		}
	}

	// Token: 0x060003AA RID: 938 RVA: 0x00019F10 File Offset: 0x00018110
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setFloat(string _property, float _value, bool _netsync = true)
	{
		this._setFloat(Animator.StringToHash(_property), _value, _netsync);
	}

	// Token: 0x060003AB RID: 939 RVA: 0x00019F20 File Offset: 0x00018120
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setBool(string _property, bool _value, bool _netsync = true)
	{
		int propertyHash = Animator.StringToHash(_property);
		this._setBool(propertyHash, _value, _netsync);
	}

	// Token: 0x060003AC RID: 940 RVA: 0x00019F3D File Offset: 0x0001813D
	[PublicizedFrom(EAccessModifier.Protected)]
	public void _setInt(string _property, int _value, bool _netsync = true)
	{
		this._setInt(Animator.StringToHash(_property), _value, _netsync);
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00019F50 File Offset: 0x00018150
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setFloat(int _propertyHash, float _value, bool _netSync = true)
	{
		if (this.anim)
		{
			if (!_netSync)
			{
				this.anim.SetFloat(_propertyHash, _value);
				return;
			}
			float num = this.anim.GetFloat(_propertyHash) - _value;
			if (num * num > 0.0001f)
			{
				this.anim.SetFloat(_propertyHash, _value);
				if (!this.entity.isEntityRemote && _netSync)
				{
					this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Float, _value));
				}
			}
		}
	}

	// Token: 0x060003AE RID: 942 RVA: 0x00019FC4 File Offset: 0x000181C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setBool(int _propertyHash, bool _value, bool _netsync = true)
	{
		if (this.anim != null && this.anim.GetBool(_propertyHash) != _value)
		{
			this.anim.SetBool(_propertyHash, _value);
			if (_propertyHash == AvatarController.isFPVHash)
			{
				return;
			}
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Bool, _value));
			}
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0001A02C File Offset: 0x0001822C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void _setInt(int _propertyHash, int _value, bool _netsync = true)
	{
		if (this.anim != null && this.anim.GetInteger(_propertyHash) != _value)
		{
			this.anim.SetInteger(_propertyHash, _value);
			if (!this.entity.isEntityRemote && _netsync)
			{
				this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Int, _value));
			}
		}
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x0001A088 File Offset: 0x00018288
	public virtual void SetDataFloat(AvatarController.DataTypes _type, float _value, bool _netsync = true)
	{
		if (_type == AvatarController.DataTypes.HitDuration)
		{
			this.InitHitDuration(_value);
		}
		if (!this.entity.isEntityRemote && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData((int)_type, AnimParamData.ValueTypes.DataFloat, _value));
		}
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x0001A0B9 File Offset: 0x000182B9
	public virtual bool TryGetTrigger(string _property, out bool _value)
	{
		return this.TryGetTrigger(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x0001A0C8 File Offset: 0x000182C8
	public virtual bool TryGetFloat(string _property, out float _value)
	{
		return this.TryGetFloat(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x0001A0D7 File Offset: 0x000182D7
	public virtual bool TryGetBool(string _property, out bool _value)
	{
		return this.TryGetBool(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x0001A0E6 File Offset: 0x000182E6
	public virtual bool TryGetInt(string _property, out int _value)
	{
		return this.TryGetInt(Animator.StringToHash(_property), out _value);
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x0001A0F8 File Offset: 0x000182F8
	public virtual bool TryGetTrigger(int _propertyHash, out bool _value)
	{
		if (this.anim == null)
		{
			return _value = false;
		}
		_value = this.anim.GetBool(_propertyHash);
		return true;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x0001A129 File Offset: 0x00018329
	public virtual bool TryGetFloat(int _propertyHash, out float _value)
	{
		if (this.anim == null)
		{
			_value = 0f;
			return false;
		}
		_value = this.anim.GetFloat(_propertyHash);
		return true;
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x0001A154 File Offset: 0x00018354
	public virtual bool TryGetBool(int _propertyHash, out bool _value)
	{
		if (this.anim == null)
		{
			return _value = false;
		}
		_value = this.anim.GetBool(_propertyHash);
		return true;
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x0001A185 File Offset: 0x00018385
	public virtual bool TryGetInt(int _propertyHash, out int _value)
	{
		if (this.anim == null)
		{
			_value = 0;
			return false;
		}
		_value = this.anim.GetInteger(_propertyHash);
		return true;
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0001A1AC File Offset: 0x000183AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updateNetworkAnimData()
	{
		if (this.entity == null)
		{
			return;
		}
		if (this.entity.isEntityRemote)
		{
			if (this.queuedAnimParams.Count > 0)
			{
				do
				{
					this.processAnimParamData(this.queuedAnimParams[0]);
					this.queuedAnimParams.RemoveAt(0);
				}
				while (this.queuedAnimParams.Count > 10);
				return;
			}
		}
		else
		{
			foreach (List<AnimParamData> animationParameterData in this.changedAnimationParameters.GetParameterLists())
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entity.entityId, animationParameterData), false, -1, this.entity.entityId, this.entity.entityId, null, 192, false);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entity.entityId, animationParameterData), false);
				}
			}
		}
	}

	// Token: 0x060003BA RID: 954 RVA: 0x0001A2D4 File Offset: 0x000184D4
	public void SyncAnimParameters(int _toEntityId)
	{
		if (!this.anim)
		{
			return;
		}
		Dictionary<int, AnimParamData> dictionary = new Dictionary<int, AnimParamData>();
		foreach (AnimatorControllerParameter animatorControllerParameter in this.anim.parameters)
		{
			switch (animatorControllerParameter.type)
			{
			case AnimatorControllerParameterType.Float:
			{
				float @float = this.anim.GetFloat(animatorControllerParameter.nameHash);
				dictionary[animatorControllerParameter.nameHash] = new AnimParamData(animatorControllerParameter.nameHash, AnimParamData.ValueTypes.Float, @float);
				break;
			}
			case AnimatorControllerParameterType.Int:
			{
				int integer = this.anim.GetInteger(animatorControllerParameter.nameHash);
				dictionary[animatorControllerParameter.nameHash] = new AnimParamData(animatorControllerParameter.nameHash, AnimParamData.ValueTypes.Int, integer);
				break;
			}
			case AnimatorControllerParameterType.Bool:
			{
				bool @bool = this.anim.GetBool(animatorControllerParameter.nameHash);
				dictionary[animatorControllerParameter.nameHash] = new AnimParamData(animatorControllerParameter.nameHash, AnimParamData.ValueTypes.Bool, @bool);
				break;
			}
			}
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAnimationData>().Setup(this.entity.entityId, dictionary), false, _toEntityId, -1, -1, null, 192, false);
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0001A400 File Offset: 0x00018600
	public virtual string GetParameterName(int _nameHash)
	{
		foreach (AnimatorControllerParameter animatorControllerParameter in this.anim.parameters)
		{
			if (animatorControllerParameter.nameHash == _nameHash)
			{
				return animatorControllerParameter.name;
			}
		}
		return "?";
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0001A440 File Offset: 0x00018640
	public void SetAnimParameters(List<AnimParamData> animationParameterData)
	{
		this.queuedAnimParams.Add(animationParameterData);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0001A450 File Offset: 0x00018650
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarController()
	{
	}

	// Token: 0x040003D7 RID: 983
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMinFloatChangeSquared = 0.0001f;

	// Token: 0x040003D8 RID: 984
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool initialized;

	// Token: 0x040003D9 RID: 985
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int attackTag;

	// Token: 0x040003DA RID: 986
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int deathHash;

	// Token: 0x040003DB RID: 987
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int digHash;

	// Token: 0x040003DC RID: 988
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitStartHash;

	// Token: 0x040003DD RID: 989
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitHash;

	// Token: 0x040003DE RID: 990
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpHash;

	// Token: 0x040003DF RID: 991
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int moveHash;

	// Token: 0x040003E0 RID: 992
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int stunHash;

	// Token: 0x040003E1 RID: 993
	public static int attackHash;

	// Token: 0x040003E2 RID: 994
	public static int attackBlendHash;

	// Token: 0x040003E3 RID: 995
	public static int attackStartHash;

	// Token: 0x040003E4 RID: 996
	public static int attackReadyHash;

	// Token: 0x040003E5 RID: 997
	public static int meleeAttackSpeedHash;

	// Token: 0x040003E6 RID: 998
	public static int beginCorpseEatHash;

	// Token: 0x040003E7 RID: 999
	public static int endCorpseEatHash;

	// Token: 0x040003E8 RID: 1000
	public static int forwardHash;

	// Token: 0x040003E9 RID: 1001
	public static int hitBodyPartHash;

	// Token: 0x040003EA RID: 1002
	public static int idleTimeHash;

	// Token: 0x040003EB RID: 1003
	public static int isAimingHash;

	// Token: 0x040003EC RID: 1004
	public static int itemUseHash;

	// Token: 0x040003ED RID: 1005
	public static int movementStateHash;

	// Token: 0x040003EE RID: 1006
	public static int rotationPitchHash;

	// Token: 0x040003EF RID: 1007
	public static int strafeHash;

	// Token: 0x040003F0 RID: 1008
	public static int swimSelectHash;

	// Token: 0x040003F1 RID: 1009
	public static int turnRateHash;

	// Token: 0x040003F2 RID: 1010
	public static int weaponHoldTypeHash;

	// Token: 0x040003F3 RID: 1011
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int walkTypeHash;

	// Token: 0x040003F4 RID: 1012
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int walkTypeBlendHash;

	// Token: 0x040003F5 RID: 1013
	public static int isAliveHash;

	// Token: 0x040003F6 RID: 1014
	public static int isDeadHash;

	// Token: 0x040003F7 RID: 1015
	public static int isFPVHash;

	// Token: 0x040003F8 RID: 1016
	public static int isMovingHash;

	// Token: 0x040003F9 RID: 1017
	public static int isSwimHash;

	// Token: 0x040003FA RID: 1018
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int attackTriggerHash;

	// Token: 0x040003FB RID: 1019
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int deathTriggerHash;

	// Token: 0x040003FC RID: 1020
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitTriggerHash;

	// Token: 0x040003FD RID: 1021
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int movementTriggerHash;

	// Token: 0x040003FE RID: 1022
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int electrocuteTriggerHash;

	// Token: 0x040003FF RID: 1023
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int painTriggerHash;

	// Token: 0x04000400 RID: 1024
	public static int itemHasChangedTriggerHash;

	// Token: 0x04000401 RID: 1025
	public static int itemThrownAwayTriggerHash;

	// Token: 0x04000402 RID: 1026
	public static int reloadHash;

	// Token: 0x04000403 RID: 1027
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int dodgeBlendHash;

	// Token: 0x04000404 RID: 1028
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int dodgeTriggerHash;

	// Token: 0x04000405 RID: 1029
	public static int reactionTypeHash;

	// Token: 0x04000406 RID: 1030
	public static int reactionTriggerHash;

	// Token: 0x04000407 RID: 1031
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperPoseHash;

	// Token: 0x04000408 RID: 1032
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperTriggerHash;

	// Token: 0x04000409 RID: 1033
	public static int jumpLandResponseHash;

	// Token: 0x0400040A RID: 1034
	public static int forcedRootMotionHash;

	// Token: 0x0400040B RID: 1035
	public static int preventAttackHash;

	// Token: 0x0400040C RID: 1036
	public static int canFallHash;

	// Token: 0x0400040D RID: 1037
	public static int isOnGroundHash;

	// Token: 0x0400040E RID: 1038
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int triggerAliveHash;

	// Token: 0x0400040F RID: 1039
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int bodyPartHitHash;

	// Token: 0x04000410 RID: 1040
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitDirectionHash;

	// Token: 0x04000411 RID: 1041
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitDamageHash;

	// Token: 0x04000412 RID: 1042
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int criticalHitHash;

	// Token: 0x04000413 RID: 1043
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int randomHash;

	// Token: 0x04000414 RID: 1044
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpStartHash;

	// Token: 0x04000415 RID: 1045
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpLandHash;

	// Token: 0x04000416 RID: 1046
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isMaleHash;

	// Token: 0x04000417 RID: 1047
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int specialAttack2Hash;

	// Token: 0x04000418 RID: 1048
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int rageHash;

	// Token: 0x04000419 RID: 1049
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int stunTypeHash;

	// Token: 0x0400041A RID: 1050
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int stunBodyPartHash;

	// Token: 0x0400041B RID: 1051
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isCriticalHash;

	// Token: 0x0400041C RID: 1052
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int HitRandomValueHash;

	// Token: 0x0400041D RID: 1053
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int beginStunTriggerHash;

	// Token: 0x0400041E RID: 1054
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int endStunTriggerHash;

	// Token: 0x0400041F RID: 1055
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int toCrawlerTriggerHash;

	// Token: 0x04000420 RID: 1056
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isElectrocutedHash;

	// Token: 0x04000421 RID: 1057
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isClimbingHash;

	// Token: 0x04000422 RID: 1058
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int verticalSpeedHash;

	// Token: 0x04000423 RID: 1059
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int reviveHash;

	// Token: 0x04000424 RID: 1060
	public static int harvestingHash;

	// Token: 0x04000425 RID: 1061
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponFireHash;

	// Token: 0x04000426 RID: 1062
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponPreFireCancelHash;

	// Token: 0x04000427 RID: 1063
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponPreFireHash;

	// Token: 0x04000428 RID: 1064
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int weaponAmmoRemaining;

	// Token: 0x04000429 RID: 1065
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int useItemHash;

	// Token: 0x0400042A RID: 1066
	public static int itemActionIndexHash;

	// Token: 0x0400042B RID: 1067
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int isCrouchingHash;

	// Token: 0x0400042C RID: 1068
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int reloadSpeedHash;

	// Token: 0x0400042D RID: 1069
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpTriggerHash;

	// Token: 0x0400042E RID: 1070
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int inAirHash;

	// Token: 0x0400042F RID: 1071
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int jumpLandTriggerHash;

	// Token: 0x04000430 RID: 1072
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int hitRandomValueHash;

	// Token: 0x04000431 RID: 1073
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleSitHash;

	// Token: 0x04000432 RID: 1074
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleSideRightHash;

	// Token: 0x04000433 RID: 1075
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleSideLeftHash;

	// Token: 0x04000434 RID: 1076
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleBackHash;

	// Token: 0x04000435 RID: 1077
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleStomachHash;

	// Token: 0x04000436 RID: 1078
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int sleeperIdleStandHash;

	// Token: 0x04000437 RID: 1079
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static int archetypeStanceHash;

	// Token: 0x04000438 RID: 1080
	public static int yLookHash;

	// Token: 0x04000439 RID: 1081
	public static int vehiclePoseHash;

	// Token: 0x0400043A RID: 1082
	public const int cSleeperPoseMove = -2;

	// Token: 0x0400043B RID: 1083
	public const int cSleeperPoseAwake = -1;

	// Token: 0x0400043C RID: 1084
	public const int cSleeperPoseSit = 0;

	// Token: 0x0400043D RID: 1085
	public const int cSleeperPoseSideRight = 1;

	// Token: 0x0400043E RID: 1086
	public const int cSleeperPoseSideLeft = 2;

	// Token: 0x0400043F RID: 1087
	public const int cSleeperPoseBack = 3;

	// Token: 0x04000440 RID: 1088
	public const int cSleeperPoseStomach = 4;

	// Token: 0x04000441 RID: 1089
	public const int cSleeperPoseStand = 5;

	// Token: 0x04000442 RID: 1090
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive entity;

	// Token: 0x04000443 RID: 1091
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Animator anim;

	// Token: 0x04000444 RID: 1092
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<List<AnimParamData>> queuedAnimParams = new List<List<AnimParamData>>();

	// Token: 0x04000445 RID: 1093
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AvatarController.ChangedAnimationParameters changedAnimationParameters = new AvatarController.ChangedAnimationParameters();

	// Token: 0x04000446 RID: 1094
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float animSyncWaitTime = 0.5f;

	// Token: 0x04000447 RID: 1095
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float electrocuteTime;

	// Token: 0x04000448 RID: 1096
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitBlendInTimeMax = 0.1f;

	// Token: 0x04000449 RID: 1097
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitBlendOutExtraTime = 0.2f;

	// Token: 0x0400044A RID: 1098
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitWeightFastTarget = 0.15f;

	// Token: 0x0400044B RID: 1099
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitAgainWeightAdd = 0.2f;

	// Token: 0x0400044C RID: 1100
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHitAgainWeightAddWeak = 0.1f;

	// Token: 0x0400044D RID: 1101
	public float hitWeightMax = 1f;

	// Token: 0x0400044E RID: 1102
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitDuration;

	// Token: 0x0400044F RID: 1103
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitDurationOut;

	// Token: 0x04000450 RID: 1104
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int hitLayerIndex = -1;

	// Token: 0x04000451 RID: 1105
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitWeight = 0.001f;

	// Token: 0x04000452 RID: 1106
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitWeightTarget;

	// Token: 0x04000453 RID: 1107
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float hitWeightDuration;

	// Token: 0x04000454 RID: 1108
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float forwardSpeedLerpMultiplier = 10f;

	// Token: 0x04000455 RID: 1109
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float strafeSpeedLerpMultiplier = 10f;

	// Token: 0x04000456 RID: 1110
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float targetSpeedForward;

	// Token: 0x04000457 RID: 1111
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float targetSpeedStrafe;

	// Token: 0x04000458 RID: 1112
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cPhysicsTicks = 50f;

	// Token: 0x04000459 RID: 1113
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasTurnRate;

	// Token: 0x0400045A RID: 1114
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float turnRateFacing;

	// Token: 0x0400045B RID: 1115
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float turnRate;

	// Token: 0x0400045C RID: 1116
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static Dictionary<int, string> hashNames;

	// Token: 0x0400045D RID: 1117
	public const int cActionSpecial = 3000;

	// Token: 0x0400045E RID: 1118
	public const int cActionEnd = 9999;

	// Token: 0x0400045F RID: 1119
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cMaxQueuedAnimData = 10;

	// Token: 0x020000AB RID: 171
	public enum DataTypes
	{
		// Token: 0x04000461 RID: 1121
		HitDuration
	}

	// Token: 0x020000AC RID: 172
	[PublicizedFrom(EAccessModifier.Protected)]
	public class ChangedAnimationParameters
	{
		// Token: 0x060003BE RID: 958 RVA: 0x0001A4B8 File Offset: 0x000186B8
		public void Add(AnimParamData apd)
		{
			int count = this.m_animationParameters.Count;
			List<AnimParamData> list;
			if (count < 1)
			{
				list = this.newPacket();
			}
			else
			{
				list = this.m_animationParameters[count - 1];
			}
			int index;
			if (this.m_animationParameterLookup.TryGetValue(apd.NameHash, out index))
			{
				list.RemoveAt(index);
			}
			this.m_animationParameterLookup[apd.NameHash] = list.Count;
			list.Add(apd);
			if (apd.ValueType == AnimParamData.ValueTypes.Trigger)
			{
				this.newPacket();
				this.m_hasAnyTriggers = true;
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0001A540 File Offset: 0x00018740
		[PublicizedFrom(EAccessModifier.Private)]
		public List<AnimParamData> newPacket()
		{
			List<AnimParamData> list = new List<AnimParamData>();
			this.m_animationParameters.Add(list);
			this.m_animationParameterLookup.Clear();
			return list;
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0001A56C File Offset: 0x0001876C
		public List<List<AnimParamData>> GetParameterLists()
		{
			List<List<AnimParamData>> list = new List<List<AnimParamData>>();
			this.m_sendDelay -= Time.deltaTime;
			if (!this.m_hasAnyTriggers)
			{
				if (this.m_sendDelay > 0f)
				{
					return list;
				}
			}
			while (this.m_animationParameters.Count > 0)
			{
				List<AnimParamData> list2 = this.m_animationParameters[0];
				this.m_animationParameters.RemoveAt(0);
				if (list2.Count != 0)
				{
					list.Add(list2);
				}
			}
			this.m_hasAnyTriggers = false;
			this.m_sendDelay = 0.05f;
			return list;
		}

		// Token: 0x04000462 RID: 1122
		[PublicizedFrom(EAccessModifier.Private)]
		public const float sendPeriodInSeconds = 0.05f;

		// Token: 0x04000463 RID: 1123
		[PublicizedFrom(EAccessModifier.Private)]
		public float m_sendDelay;

		// Token: 0x04000464 RID: 1124
		[PublicizedFrom(EAccessModifier.Private)]
		public List<List<AnimParamData>> m_animationParameters = new List<List<AnimParamData>>();

		// Token: 0x04000465 RID: 1125
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, int> m_animationParameterLookup = new Dictionary<int, int>();

		// Token: 0x04000466 RID: 1126
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_hasAnyTriggers;
	}

	// Token: 0x020000AD RID: 173
	public enum ActionState
	{
		// Token: 0x04000468 RID: 1128
		None,
		// Token: 0x04000469 RID: 1129
		Start,
		// Token: 0x0400046A RID: 1130
		Ready,
		// Token: 0x0400046B RID: 1131
		Active
	}
}
