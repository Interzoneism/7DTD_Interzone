using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public abstract class AvatarMultiBodyController : AvatarController
{
	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060003E5 RID: 997 RVA: 0x0001ADB7 File Offset: 0x00018FB7
	// (set) Token: 0x060003E6 RID: 998 RVA: 0x0001ADBF File Offset: 0x00018FBF
	public BodyAnimator PrimaryBody
	{
		get
		{
			return this.primaryBody;
		}
		set
		{
			this.primaryBody = value;
			this.SetInRightHand(this.heldItemTransform);
		}
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060003E7 RID: 999 RVA: 0x0001ADD4 File Offset: 0x00018FD4
	public List<BodyAnimator> BodyAnimators
	{
		get
		{
			return this.bodyAnimators;
		}
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060003E8 RID: 1000 RVA: 0x0001ADDC File Offset: 0x00018FDC
	public Animator HeldItemAnimator
	{
		get
		{
			return this.heldItemAnimator;
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060003E9 RID: 1001 RVA: 0x0001ADE4 File Offset: 0x00018FE4
	public Transform HeldItemTransform
	{
		get
		{
			return this.heldItemTransform;
		}
	}

	// Token: 0x060003EA RID: 1002 RVA: 0x0001ADEC File Offset: 0x00018FEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public BodyAnimator addBodyAnimator(BodyAnimator _body)
	{
		Animator animator = _body.Animator;
		if (animator)
		{
			animator.logWarnings = false;
		}
		this.bodyAnimators.Add(_body);
		base.SetAnimator(this.bodyAnimators[0].Animator);
		return _body;
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x0001AE33 File Offset: 0x00019033
	[PublicizedFrom(EAccessModifier.Protected)]
	public void removeBodyAnimator(BodyAnimator _body)
	{
		this.bodyAnimators.Remove(_body);
	}

	// Token: 0x060003EC RID: 1004 RVA: 0x0001AE44 File Offset: 0x00019044
	public override void PlayPlayerFPRevive()
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				animator.SetTrigger(AvatarController.reviveHash);
			}
		}
		this.reviveTime = Time.time;
	}

	// Token: 0x060003ED RID: 1005 RVA: 0x0001AE99 File Offset: 0x00019099
	public override bool IsAnimationPlayerFPRevivePlaying()
	{
		return Time.time - this.reviveTime < 4.5f;
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x0001AEB0 File Offset: 0x000190B0
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (this.heldItemTransform == null || this.entity == null || this.entity.inventory == null || this.entity.inventory.holdingItem == null)
		{
			return;
		}
		if (_bFPV)
		{
			this.heldItemTransform.localPosition = Vector3.zero;
			this.heldItemTransform.localEulerAngles = Vector3.zero;
			return;
		}
		this.heldItemTransform.localPosition = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value].position;
		this.heldItemTransform.localEulerAngles = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value].rotation;
	}

	// Token: 0x060003EF RID: 1007 RVA: 0x0001AF85 File Offset: 0x00019185
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnTrigger(int _id)
	{
		if (_id == AvatarController.weaponFireHash)
		{
			this.animationToDodgeTime = 1f;
		}
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x0001AF9A File Offset: 0x0001919A
	public override bool IsAnimationToDodge()
	{
		return this.animationToDodgeTime > 0f;
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsAnimationAttackPlaying()
	{
		return false;
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x0001AFA9 File Offset: 0x000191A9
	public override void SetInAir(bool inAir)
	{
		this._setBool(AvatarController.inAirHash, inAir, true);
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x0001AFB8 File Offset: 0x000191B8
	public override void StartAnimationAttack()
	{
		this._setBool(AvatarController.harvestingHash, false, true);
		int meta = this.entity.inventory.holdingItemItemValue.Meta;
		this._setInt(AvatarController.weaponAmmoRemaining, meta, true);
		this._setTrigger(AvatarController.weaponFireHash, true);
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x0001B001 File Offset: 0x00019201
	public override bool IsAnimationUsePlaying()
	{
		return this.timeUseAnimationPlaying > 0f;
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x0001B010 File Offset: 0x00019210
	public override void StartAnimationUse()
	{
		this._setTrigger(AvatarController.useItemHash, true);
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x0001B01E File Offset: 0x0001921E
	public override bool IsAnimationSpecialAttackPlaying()
	{
		return this.bSpecialAttackPlaying;
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0001B026 File Offset: 0x00019226
	public override void StartAnimationSpecialAttack(bool _b, int _animType)
	{
		this.idleTime = 0f;
		this.bSpecialAttackPlaying = _b;
		if (_b)
		{
			this._resetTrigger(AvatarController.weaponFireHash, true);
			this._resetTrigger(AvatarController.weaponPreFireCancelHash, true);
			this._setTrigger(AvatarController.weaponPreFireHash, true);
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x0001B061 File Offset: 0x00019261
	public override bool IsAnimationSpecialAttack2Playing()
	{
		return this.timeSpecialAttack2Playing > 0f;
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x0001B070 File Offset: 0x00019270
	public override void StartAnimationSpecialAttack2()
	{
		this.idleTime = 0f;
		this.timeSpecialAttack2Playing = 0.3f;
		this._resetTrigger(AvatarController.weaponFireHash, true);
		this._resetTrigger(AvatarController.weaponPreFireHash, true);
		this._setTrigger(AvatarController.weaponPreFireCancelHash, true);
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x0001B0AC File Offset: 0x000192AC
	public override bool IsAnimationHarvestingPlaying()
	{
		return this.timeHarestingAnimationPlaying > 0f;
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x0001B0BB File Offset: 0x000192BB
	public override void StartAnimationHarvesting(float _length, bool _weaponFireTrigger)
	{
		this.timeHarestingAnimationPlaying = _length;
		this._setBool(AvatarController.harvestingHash, true, true);
		if (_weaponFireTrigger)
		{
			this._setTrigger(AvatarController.weaponFireHash, true);
		}
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x0001B0E0 File Offset: 0x000192E0
	public override void SetDrunk(float _numBeers)
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			BodyAnimator bodyAnimator = this.bodyAnimators[i];
			if (bodyAnimator.Animator)
			{
				bodyAnimator.SetDrunk(_numBeers);
			}
		}
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x0001B128 File Offset: 0x00019328
	public override void SetVehicleAnimation(int _animHash, int _pose)
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				animator.SetInteger(_animHash, _pose);
			}
		}
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x0001B16F File Offset: 0x0001936F
	public override void SetAiming(bool _bEnable)
	{
		this.idleTime = 0f;
		this._setBool(AvatarController.isAimingHash, _bEnable, true);
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x0001B189 File Offset: 0x00019389
	public override void SetCrouching(bool _bEnable)
	{
		this.idleTime = 0f;
		this._setBool(AvatarController.isCrouchingHash, _bEnable, true);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0001B1A3 File Offset: 0x000193A3
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void avatarVisibilityChanged(BodyAnimator _body, bool _bVisible)
	{
		_body.State = (_bVisible ? BodyAnimator.EnumState.Visible : BodyAnimator.EnumState.Disabled);
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x0001B1B4 File Offset: 0x000193B4
	public override void SetVisible(bool _b)
	{
		if (this.visible != _b)
		{
			int count = this.bodyAnimators.Count;
			for (int i = 0; i < count; i++)
			{
				BodyAnimator body = this.bodyAnimators[i];
				this.avatarVisibilityChanged(body, _b);
			}
			this.visible = _b;
		}
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x0001B200 File Offset: 0x00019400
	public override void SetRagdollEnabled(bool _b)
	{
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			this.bodyAnimators[i].RagdollActive = _b;
		}
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x0001B238 File Offset: 0x00019438
	public override void StartAnimationReloading()
	{
		this.idleTime = 0f;
		float value = EffectManager.GetValue(PassiveEffects.ReloadSpeedMultiplier, this.entity.inventory.holdingItemItemValue, 1f, this.entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		int count = this.bodyAnimators.Count;
		bool value2 = this.entity as EntityPlayerLocal != null && (this.entity as EntityPlayerLocal).emodel.IsFPV;
		this._setBool(AvatarController.isFPVHash, value2, true);
		this._setBool(AvatarController.reloadHash, true, true);
		this._setFloat(AvatarController.reloadSpeedHash, value, true);
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x0001B2E4 File Offset: 0x000194E4
	public override void StartAnimationJump(AnimJumpMode jumpMode)
	{
		this.idleTime = 0f;
		if (jumpMode == AnimJumpMode.Start)
		{
			this._setTrigger(AvatarController.jumpTriggerHash, true);
			this._setBool(AvatarController.inAirHash, true, true);
			return;
		}
		if (jumpMode != AnimJumpMode.Land)
		{
			return;
		}
		this._setTrigger(AvatarController.jumpLandHash, true);
		this._setInt(AvatarController.jumpLandResponseHash, 0, true);
		this._setBool(AvatarController.inAirHash, false, true);
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x0001B344 File Offset: 0x00019544
	public override void SetSwim(bool _enable)
	{
		int walkType = -1;
		if (!_enable)
		{
			walkType = this.entity.GetWalkType();
		}
		this.SetWalkType(walkType, true);
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x0001B36A File Offset: 0x0001956A
	public override void StartAnimationFiring()
	{
		this.StartAnimationAttack();
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0001B374 File Offset: 0x00019574
	public override void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
		this.idleTime = 0f;
		this._setInt(AvatarController.movementStateHash, _movementState, true);
		this._setInt(AvatarController.hitDirectionHash, _dir, true);
		this._setInt(AvatarController.hitDamageHash, _hitDamage, true);
		this._setInt(AvatarController.hitBodyPartHash, (int)_bodyPart, true);
		this._setFloat(AvatarController.hitRandomValueHash, _random, true);
		this._setBool(AvatarController.isCriticalHash, _criticalHit, true);
		this._setTrigger(AvatarController.hitTriggerHash, true);
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x0001B3EC File Offset: 0x000195EC
	public override void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
		this.idleTime = 0f;
		int count = this.bodyAnimators.Count;
		for (int i = 0; i < count; i++)
		{
			this.bodyAnimators[i].StartDeathAnimation(_bodyPart, _movementState, random);
		}
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x0001B430 File Offset: 0x00019630
	public override void SetInRightHand(Transform _transform)
	{
		this.idleTime = 0f;
		if (_transform != null)
		{
			Quaternion localRotation = (this.heldItemTransform != null) ? this.heldItemTransform.localRotation : Quaternion.identity;
			_transform.SetParent(this.GetRightHandTransform(), false);
			if ((!this.entity.emodel.IsFPV || this.entity.isEntityRemote) && this.entity.inventory != null && this.entity.inventory.holdingItem != null)
			{
				AnimationGunjointOffsetData.AnimationGunjointOffsets animationGunjointOffsets = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value];
				_transform.localPosition = animationGunjointOffsets.position;
				_transform.localRotation = Quaternion.Euler(animationGunjointOffsets.rotation);
			}
			else
			{
				_transform.localPosition = Vector3.zero;
				_transform.localRotation = localRotation;
			}
		}
		this.heldItemTransform = _transform;
		this.heldItemAnimator = ((_transform != null) ? _transform.GetComponent<Animator>() : null);
		if (this.heldItemAnimator != null)
		{
			this.heldItemAnimator.logWarnings = false;
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x0001B54F File Offset: 0x0001974F
	public override Transform GetRightHandTransform()
	{
		if (this.primaryBody == null)
		{
			return null;
		}
		return this.primaryBody.Parts.RightHandT;
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x0001B56C File Offset: 0x0001976C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (this.animationToDodgeTime > 0f)
		{
			this.animationToDodgeTime -= deltaTime;
		}
		if (this.timeUseAnimationPlaying > 0f)
		{
			this.timeUseAnimationPlaying -= deltaTime;
		}
		if (this.timeHarestingAnimationPlaying > 0f)
		{
			this.timeHarestingAnimationPlaying -= deltaTime;
		}
		if (this.timeSpecialAttack2Playing > 0f)
		{
			this.timeSpecialAttack2Playing -= deltaTime;
		}
		int value = this.entity.inventory.holdingItem.HoldType.Value;
		this._setInt(AvatarController.weaponHoldTypeHash, value, true);
		float speedForward = this.entity.speedForward;
		float speedStrafe = this.entity.speedStrafe;
		float x = this.entity.rotation.x;
		bool flag = this.entity.IsDead();
		bool flag2 = base.IsMoving(speedForward, speedStrafe);
		if (flag2)
		{
			this.idleTime = 0f;
		}
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			this.bodyAnimators[i].Update();
		}
		float num = speedStrafe;
		if (num >= 1234f)
		{
			num = 0f;
		}
		this._setFloat(AvatarController.forwardHash, speedForward, false);
		this._setFloat(AvatarController.strafeHash, num, false);
		this._setBool(AvatarController.isMovingHash, flag2, false);
		this._setFloat(AvatarController.rotationPitchHash, x, false);
		if (!flag)
		{
			if (speedStrafe >= 1234f)
			{
				this._setInt(AvatarController.movementStateHash, 4, true);
			}
			else
			{
				float num2 = speedForward * speedForward + speedStrafe * speedStrafe;
				this._setInt(AvatarController.movementStateHash, (num2 > base.Entity.moveSpeedAggro * base.Entity.moveSpeedAggro) ? 3 : ((num2 > base.Entity.moveSpeed * base.Entity.moveSpeed) ? 2 : ((num2 > 0.001f) ? 1 : 0)), true);
			}
		}
		float num3 = this.idleTime - this.idleTimeSent;
		if (num3 * num3 > 0.25f)
		{
			this.idleTimeSent = this.idleTime;
			this._setFloat(AvatarController.idleTimeHash, this.idleTime, false);
		}
		this.idleTime += deltaTime;
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0001B79C File Offset: 0x0001999C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		if (base.Entity.inventory.holdingItem.Actions[0] != null)
		{
			base.Entity.inventory.holdingItem.Actions[0].UpdateNozzleParticlesPosAndRot(base.Entity.inventory.holdingItemData.actionData[0]);
		}
		if (base.Entity.inventory.holdingItem.Actions[1] != null)
		{
			base.Entity.inventory.holdingItem.Actions[1].UpdateNozzleParticlesPosAndRot(base.Entity.inventory.holdingItemData.actionData[1]);
		}
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00002914 File Offset: 0x00000B14
	public override void NotifyAnimatorMove(Animator anim)
	{
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0001B849 File Offset: 0x00019A49
	public override Animator GetAnimator()
	{
		return this.primaryBody.Animator;
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x0001B856 File Offset: 0x00019A56
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool animatorIsValid(Animator animator)
	{
		return animator && animator.enabled && animator.gameObject.activeInHierarchy;
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0001B878 File Offset: 0x00019A78
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setTrigger(int _propertyHash, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (AvatarMultiBodyController.animatorIsValid(animator) && !animator.GetBool(_propertyHash))
			{
				animator.SetTrigger(_propertyHash);
				this.changed = true;
			}
		}
		if (AvatarMultiBodyController.animatorIsValid(this.heldItemAnimator) && !this.heldItemAnimator.GetBool(_propertyHash))
		{
			this.heldItemAnimator.SetTrigger(_propertyHash);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Trigger, true));
		}
		if (this.changed)
		{
			this.OnTrigger(_propertyHash);
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x0001B940 File Offset: 0x00019B40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _resetTrigger(int _propertyHash, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator && animator.GetBool(_propertyHash))
			{
				animator.ResetTrigger(_propertyHash);
				this.changed = true;
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetBool(_propertyHash))
		{
			this.heldItemAnimator.ResetTrigger(_propertyHash);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Trigger, false));
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0001BA08 File Offset: 0x00019C08
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setFloat(int _propertyHash, float _value, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				float num = animator.GetFloat(_propertyHash) - _value;
				if (num * num > 1.0000001E-06f)
				{
					animator.SetFloat(_propertyHash, _value);
					this.changed = true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetFloat(_propertyHash) != _value)
		{
			this.heldItemAnimator.SetFloat(_propertyHash, _value);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Float, _value));
		}
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x0001BADC File Offset: 0x00019CDC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setBool(int _propertyHash, bool _value, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator && animator.GetBool(_propertyHash) != _value)
			{
				animator.SetBool(_propertyHash, _value);
				this.changed = true;
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetBool(_propertyHash) != _value)
		{
			this.heldItemAnimator.SetBool(_propertyHash, _value);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _propertyHash != AvatarController.isFPVHash && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Bool, _value));
		}
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x0001BBB8 File Offset: 0x00019DB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _propertyHash, int _value, bool _netsync = true)
	{
		this.changed = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator && animator.GetInteger(_propertyHash) != _value)
			{
				animator.SetInteger(_propertyHash, _value);
				this.changed = true;
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy && this.heldItemAnimator.GetInteger(_propertyHash) != _value)
		{
			this.heldItemAnimator.SetInteger(_propertyHash, _value);
			this.changed = true;
		}
		if (!this.entity.isEntityRemote && this.changed && _netsync)
		{
			this.changedAnimationParameters.Add(new AnimParamData(_propertyHash, AnimParamData.ValueTypes.Int, _value));
		}
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x0001BC84 File Offset: 0x00019E84
	public override bool TryGetTrigger(int _propertyHash, out bool _value)
	{
		_value = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value |= animator.GetBool(_propertyHash);
				if (_value)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value |= this.heldItemAnimator.GetBool(_propertyHash);
		}
		return true;
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x0001BD08 File Offset: 0x00019F08
	public override bool TryGetFloat(int _propertyHash, out float _value)
	{
		_value = float.NaN;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value = animator.GetFloat(_propertyHash);
				if (_value != float.NaN)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value = this.heldItemAnimator.GetFloat(_propertyHash);
		}
		return _value != float.NaN;
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x0001BD9C File Offset: 0x00019F9C
	public override bool TryGetBool(int _propertyHash, out bool _value)
	{
		_value = false;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value |= animator.GetBool(_propertyHash);
				if (_value)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value |= this.heldItemAnimator.GetBool(_propertyHash);
		}
		return true;
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x0001BE20 File Offset: 0x0001A020
	public override bool TryGetInt(int _propertyHash, out int _value)
	{
		_value = int.MinValue;
		for (int i = 0; i < this.bodyAnimators.Count; i++)
		{
			Animator animator = this.bodyAnimators[i].Animator;
			if (animator)
			{
				_value = animator.GetInteger(_propertyHash);
				if (_value != -2147483648)
				{
					return true;
				}
			}
		}
		if (this.heldItemAnimator && this.heldItemAnimator.gameObject.activeInHierarchy)
		{
			_value = this.heldItemAnimator.GetInteger(_propertyHash);
		}
		return _value != int.MinValue;
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x0001BEB1 File Offset: 0x0001A0B1
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarMultiBodyController()
	{
	}

	// Token: 0x0400047E RID: 1150
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<BodyAnimator> bodyAnimators = new List<BodyAnimator>();

	// Token: 0x0400047F RID: 1151
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BodyAnimator primaryBody;

	// Token: 0x04000480 RID: 1152
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform heldItemTransform;

	// Token: 0x04000481 RID: 1153
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Animator heldItemAnimator;

	// Token: 0x04000482 RID: 1154
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool visible = true;

	// Token: 0x04000483 RID: 1155
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float animationToDodgeTime;

	// Token: 0x04000484 RID: 1156
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeUseAnimationPlaying;

	// Token: 0x04000485 RID: 1157
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeHarestingAnimationPlaying;

	// Token: 0x04000486 RID: 1158
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bSpecialAttackPlaying;

	// Token: 0x04000487 RID: 1159
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeSpecialAttack2Playing;

	// Token: 0x04000488 RID: 1160
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float idleTime;

	// Token: 0x04000489 RID: 1161
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float idleTimeSent;

	// Token: 0x0400048A RID: 1162
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float reviveTime;

	// Token: 0x0400048B RID: 1163
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cReviveAnimLength = 4.5f;

	// Token: 0x0400048C RID: 1164
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<int, AnimParamData> FullSyncAnimationParameters = new Dictionary<int, AnimParamData>();

	// Token: 0x0400048D RID: 1165
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool changed;
}
