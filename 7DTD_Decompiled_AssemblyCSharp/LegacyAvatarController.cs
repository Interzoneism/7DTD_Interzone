using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CB RID: 203
public abstract class LegacyAvatarController : AvatarController
{
	// Token: 0x1700005B RID: 91
	// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00023790 File Offset: 0x00021990
	public Transform HeldItemTransform
	{
		get
		{
			return this.rightHandItemTransform;
		}
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00023798 File Offset: 0x00021998
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		if (this.entity is EntityPlayerLocal)
		{
			this.modelTransform = base.transform.Find("Camera");
		}
		else
		{
			this.modelTransform = EModelBase.FindModel(base.transform);
		}
		this.assignStates();
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x000237E7 File Offset: 0x000219E7
	public override Transform GetActiveModelRoot()
	{
		return this.modelTransform;
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x000237F0 File Offset: 0x000219F0
	public override void SetInRightHand(Transform _transform)
	{
		this.idleTime = 0f;
		if (_transform != null)
		{
			_transform.SetParent(this.rightHand, false);
		}
		this.rightHandItemTransform = _transform;
		this.rightHandAnimator = ((_transform != null) ? _transform.GetComponent<Animator>() : null);
		if (this.rightHandAnimator != null)
		{
			this.rightHandAnimator.logWarnings = false;
		}
		if (this.rightHandItemTransform != null)
		{
			Utils.SetLayerRecursively(this.rightHandItemTransform.gameObject, 0);
		}
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00023790 File Offset: 0x00021990
	public override Transform GetRightHandTransform()
	{
		return this.rightHandItemTransform;
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00023876 File Offset: 0x00021A76
	public override bool IsAnimationAttackPlaying()
	{
		return this.timeAttackAnimationPlaying > 0f;
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00023888 File Offset: 0x00021A88
	public override void StartAnimationAttack()
	{
		this.idleTime = 0f;
		this.isAttackImpact = false;
		this.timeAttackAnimationPlaying = 0.3f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		this._setTrigger(AvatarController.weaponFireHash, true);
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x000238DF File Offset: 0x00021ADF
	public override void SetAttackImpact()
	{
		if (!this.isAttackImpact)
		{
			this.isAttackImpact = true;
			this.timeAttackAnimationPlaying = 0.1f;
		}
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x000238FB File Offset: 0x00021AFB
	public override bool IsAttackImpact()
	{
		return this.isAttackImpact;
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x00023903 File Offset: 0x00021B03
	public override bool IsAnimationUsePlaying()
	{
		return this.timeUseAnimationPlaying > 0f;
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00023914 File Offset: 0x00021B14
	public override void StartAnimationUse()
	{
		this.idleTime = 0f;
		this.itemUseTicks = 3;
		this.timeUseAnimationPlaying = 0.3f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		this._setBool(AvatarController.itemUseHash, true, true);
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0002396C File Offset: 0x00021B6C
	public override bool IsAnimationSpecialAttackPlaying()
	{
		return this.bSpecialAttackPlaying;
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00023974 File Offset: 0x00021B74
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

	// Token: 0x060004F6 RID: 1270 RVA: 0x000239AF File Offset: 0x00021BAF
	public override bool IsAnimationSpecialAttack2Playing()
	{
		return this.timeSpecialAttack2Playing > 0.3f;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x000239BE File Offset: 0x00021BBE
	public override void StartAnimationSpecialAttack2()
	{
		this.idleTime = 0f;
		this.timeSpecialAttack2Playing = 0.3f;
		this._resetTrigger(AvatarController.weaponFireHash, true);
		this._resetTrigger(AvatarController.weaponPreFireHash, true);
		this._setTrigger(AvatarController.weaponPreFireCancelHash, true);
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x000239FA File Offset: 0x00021BFA
	public override bool IsAnimationHarvestingPlaying()
	{
		return this.timeHarestingAnimationPlaying > 0f;
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x00023A09 File Offset: 0x00021C09
	public override void StartAnimationHarvesting(float _length, bool _weaponFireTrigger)
	{
		this.timeHarestingAnimationPlaying = _length;
		this._setBool(AvatarController.harvestingHash, true, true);
		if (_weaponFireTrigger)
		{
			this._setTrigger(AvatarController.weaponFireHash, true);
		}
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00023A30 File Offset: 0x00021C30
	public override void StartAnimationFiring()
	{
		this.idleTime = 0f;
		this.timeAttackAnimationPlaying = 0.3f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		this._setTrigger(AvatarController.weaponFireHash, true);
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x00023A80 File Offset: 0x00021C80
	public override void StartAnimationReloading()
	{
		this.idleTime = 0f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		float value = EffectManager.GetValue(PassiveEffects.ReloadSpeedMultiplier, this.entity.inventory.holdingItemItemValue, 1f, this.entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this._setBool(AvatarController.reloadHash, true, true);
		this._setFloat(AvatarController.reloadSpeedHash, value, true);
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00023B08 File Offset: 0x00021D08
	public override void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float random, float _duration)
	{
		this.InternalStartAnimationHit(_bodyPart, _dir, _hitDamage, _criticalHit, _movementState, random, _duration);
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00023B1C File Offset: 0x00021D1C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void InternalStartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float random, float _duration)
	{
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!base.CheckHit(_duration))
		{
			this.SetDataFloat(AvatarController.DataTypes.HitDuration, _duration, true);
			return;
		}
		this.idleTime = 0f;
		if (this.anim != null)
		{
			this.movementStateOverride = _movementState;
			this._setInt(AvatarController.movementStateHash, _movementState, true);
			this._setBool(AvatarController.isCriticalHash, _criticalHit, true);
			this._setInt(AvatarController.hitDirectionHash, _dir, true);
			this._setInt(AvatarController.hitDamageHash, _hitDamage, true);
			this._setFloat(AvatarController.hitRandomValueHash, random, true);
			this._setInt(AvatarController.hitBodyPartHash, (int)_bodyPart.ToPrimary().LowerToUpperLimb(), true);
			this._setTrigger(AvatarController.hitTriggerHash, true);
			this.SetDataFloat(AvatarController.DataTypes.HitDuration, _duration, true);
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x00023BF4 File Offset: 0x00021DF4
	public override void SetVisible(bool _b)
	{
		this.m_bVisible = _b;
		Transform meshTransform = this.GetMeshTransform();
		if (meshTransform != null && meshTransform.gameObject.activeSelf != _b)
		{
			meshTransform.gameObject.SetActive(_b);
			if (_b)
			{
				this.SwitchModelAndView(this.modelName, this.bFPV, this.bMale);
			}
		}
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x00023C4D File Offset: 0x00021E4D
	public override void SetVehicleAnimation(int _animHash, int _pose)
	{
		if (this.anim)
		{
			this._setInt(_animHash, _pose, true);
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00023C68 File Offset: 0x00021E68
	public override void SetAiming(bool _bEnable)
	{
		this.idleTime = 0f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			this._setBool(AvatarController.isAimingHash, _bEnable, true);
		}
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x00023CBC File Offset: 0x00021EBC
	public override void SetCrouching(bool _bEnable)
	{
		this.idleTime = 0f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			this._setBool(AvatarController.isCrouchingHash, _bEnable, true);
		}
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00023D10 File Offset: 0x00021F10
	public override bool IsAnimationDigRunning()
	{
		return AvatarController.digHash == this.baseStateInfo.tagHash;
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x00023D24 File Offset: 0x00021F24
	public override void StartAnimationJumping()
	{
		this.idleTime = 0f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			this._setBool(AvatarController.jumpHash, true, true);
		}
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00023D78 File Offset: 0x00021F78
	public override void StartAnimationJump(AnimJumpMode jumpMode)
	{
		this.idleTime = 0f;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		this.isJumpStarted = true;
		if (this.anim != null)
		{
			if (jumpMode == AnimJumpMode.Start)
			{
				this._setTrigger(AvatarController.jumpStartHash, true);
				this._setBool(AvatarController.inAirHash, true, true);
				return;
			}
			this._setTrigger(AvatarController.jumpLandHash, true);
			this._setInt(AvatarController.jumpLandResponseHash, 0, true);
			this._setBool(AvatarController.inAirHash, false, true);
		}
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00023E09 File Offset: 0x00022009
	public override bool IsAnimationJumpRunning()
	{
		return this.isJumpStarted || AvatarController.jumpHash == this.baseStateInfo.tagHash;
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00023E2C File Offset: 0x0002202C
	public override bool IsAnimationWithMotionRunning()
	{
		int tagHash = this.baseStateInfo.tagHash;
		return tagHash == AvatarController.jumpHash || tagHash == AvatarController.moveHash;
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00023E58 File Offset: 0x00022058
	public override void SetSwim(bool _enable)
	{
		int walkType = -1;
		if (!_enable)
		{
			walkType = this.entity.GetWalkType();
		}
		else
		{
			this._setFloat(AvatarController.swimSelectHash, this.entity.rand.RandomFloat, true);
		}
		this.SetWalkType(walkType, true);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00023E9C File Offset: 0x0002209C
	public override void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
		this.idleTime = 0f;
		this.isInDeathAnim = true;
		this.didDeathTransition = false;
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			this.movementStateOverride = _movementState;
			this._setInt(AvatarController.movementStateHash, _movementState, true);
			this._setBool(AvatarController.isAliveHash, false, true);
			this._setInt(AvatarController.hitBodyPartHash, (int)_bodyPart.ToPrimary().LowerToUpperLimb(), true);
			this._setFloat(AvatarController.hitRandomValueHash, random, true);
			this.SetFallAndGround(false, this.entity.onGround);
		}
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00023F48 File Offset: 0x00022148
	public override void SetRagdollEnabled(bool _b)
	{
		this.bIsRagdoll = _b;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00023F54 File Offset: 0x00022154
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateLayerStateInfo()
	{
		if (this.anim != null)
		{
			this.baseStateInfo = this.anim.GetCurrentAnimatorStateInfo(0);
			if (this.anim.layerCount > 1)
			{
				this.currentWeaponHoldLayer = this.anim.GetCurrentAnimatorStateInfo(1);
			}
		}
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00023FA4 File Offset: 0x000221A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateSpineRotation()
	{
		if (this.modelTransform.parent != null && !this.bIsRagdoll && !this.entity.IsDead())
		{
			if (this.bFPV)
			{
				this.spine3.transform.localEulerAngles = new Vector3(this.spine1.transform.localEulerAngles.x, this.spine1.transform.localEulerAngles.y, this.spine1.transform.localEulerAngles.z + 1f * this.entity.rotation.x);
				return;
			}
			float num = 1f * this.entity.rotation.x / 3f;
			if (Time.timeScale > 0.001f)
			{
				this.spine1.transform.localEulerAngles = new Vector3(this.spine1.transform.localEulerAngles.x, this.spine1.transform.localEulerAngles.y, this.spine1.transform.localEulerAngles.z + num);
				this.spine2.transform.localEulerAngles = new Vector3(this.spine2.transform.localEulerAngles.x, this.spine2.transform.localEulerAngles.y, this.spine2.transform.localEulerAngles.z + num);
				this.spine3.transform.localEulerAngles = new Vector3(this.spine3.transform.localEulerAngles.x, this.spine3.transform.localEulerAngles.y, this.spine3.transform.localEulerAngles.z + num);
				return;
			}
			this.spine1.transform.localEulerAngles = new Vector3(this.spine1.transform.localEulerAngles.x, this.spine1.transform.localEulerAngles.y, num);
			this.spine2.transform.localEulerAngles = new Vector3(this.spine2.transform.localEulerAngles.x, this.spine2.transform.localEulerAngles.y, num);
			this.spine3.transform.localEulerAngles = new Vector3(this.spine3.transform.localEulerAngles.x, this.spine3.transform.localEulerAngles.y, num);
		}
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00024248 File Offset: 0x00022448
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		if (this.entity == null || this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim == null || !this.anim.enabled)
		{
			return;
		}
		this.updateLayerStateInfo();
		this.updateSpineRotation();
		if (this.entity.inventory.holdingItem.Actions[0] != null)
		{
			this.entity.inventory.holdingItem.Actions[0].UpdateNozzleParticlesPosAndRot(this.entity.inventory.holdingItemData.actionData[0]);
		}
		if (this.entity.inventory.holdingItem.Actions[1] != null)
		{
			this.entity.inventory.holdingItem.Actions[1].UpdateNozzleParticlesPosAndRot(this.entity.inventory.holdingItemData.actionData[1]);
		}
		int fullPathHash = this.baseStateInfo.fullPathHash;
		bool flag = this.anim.IsInTransition(0);
		if (!flag)
		{
			this.isJumpStarted = false;
			if (fullPathHash == this.jumpState || fullPathHash == this.fpvJumpState)
			{
				this._setBool(AvatarController.jumpHash, false, true);
			}
			if (this.anim.GetBool(AvatarController.reloadHash) && this.reloadStates.Contains(this.currentWeaponHoldLayer.fullPathHash))
			{
				this._setBool(AvatarController.reloadHash, false, true);
			}
		}
		if (this.anim.GetBool(AvatarController.itemUseHash))
		{
			int num = this.itemUseTicks - 1;
			this.itemUseTicks = num;
			if (num <= 0)
			{
				this._setBool(AvatarController.itemUseHash, false, true);
			}
		}
		if (this.isInDeathAnim)
		{
			if ((this.baseStateInfo.tagHash == AvatarController.deathHash || this.deathStates.Contains(fullPathHash)) && this.baseStateInfo.normalizedTime >= 1f && !flag)
			{
				this.isInDeathAnim = false;
				if (this.entity.HasDeathAnim)
				{
					this.entity.emodel.DoRagdoll(DamageResponse.New(true), 999999f);
				}
			}
			if (this.entity.HasDeathAnim && this.entity.RootMotion && this.entity.isCollidedHorizontally)
			{
				this.isInDeathAnim = false;
				this.entity.emodel.DoRagdoll(DamageResponse.New(true), 999999f);
			}
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x000244B0 File Offset: 0x000226B0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void setLayerWeights()
	{
		if (this.anim == null || this.anim.layerCount <= 1)
		{
			return;
		}
		if (this.entity.IsDead())
		{
			this.anim.SetLayerWeight(1, 1f);
			if (!this.bFPV)
			{
				this.anim.SetLayerWeight(2, 1f);
				return;
			}
		}
		else
		{
			if (this.entity.inventory.holdingItem.HoldType == 0)
			{
				this.anim.SetLayerWeight(1, 0f);
			}
			else
			{
				this.anim.SetLayerWeight(1, 1f);
			}
			if (!this.bFPV)
			{
				this.anim.SetLayerWeight(2, 1f);
			}
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0002456C File Offset: 0x0002276C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (this.timeAttackAnimationPlaying > 0f)
		{
			this.timeAttackAnimationPlaying -= Time.deltaTime;
			if (this.timeAttackAnimationPlaying <= 0f)
			{
				this.isAttackImpact = true;
			}
		}
		if (this.timeUseAnimationPlaying > 0f)
		{
			this.timeUseAnimationPlaying -= Time.deltaTime;
		}
		if (this.timeHarestingAnimationPlaying > 0f)
		{
			this.timeHarestingAnimationPlaying -= Time.deltaTime;
			if (this.timeHarestingAnimationPlaying <= 0f && this.anim != null)
			{
				this._setBool(AvatarController.harvestingHash, false, true);
			}
		}
		if (this.timeSpecialAttack2Playing > 0f)
		{
			this.timeSpecialAttack2Playing -= Time.deltaTime;
		}
		if (!this.m_bVisible && (!this.entity || !this.entity.RootMotion || this.entity.isEntityRemote))
		{
			return;
		}
		if (this.bipedTransform == null || !this.bipedTransform.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim == null || !this.anim.avatar.isValid || !this.anim.enabled)
		{
			return;
		}
		this.updateLayerStateInfo();
		this.setLayerWeights();
		int value = this.entity.inventory.holdingItem.HoldType.Value;
		this._setInt(AvatarController.weaponHoldTypeHash, value, true);
		float speedForward = this.entity.speedForward;
		float speedStrafe = this.entity.speedStrafe;
		float num = speedStrafe;
		if (num >= 1234f)
		{
			num = 0f;
		}
		this._setFloat(AvatarController.forwardHash, speedForward, false);
		this._setFloat(AvatarController.strafeHash, num, false);
		if (!this.entity.IsDead())
		{
			if (this.movementStateOverride != -1)
			{
				this._setInt(AvatarController.movementStateHash, this.movementStateOverride, true);
				this.movementStateOverride = -1;
			}
			else if (speedStrafe >= 1234f)
			{
				this._setInt(AvatarController.movementStateHash, 4, true);
			}
			else
			{
				float num2 = speedForward * speedForward + num * num;
				this._setInt(AvatarController.movementStateHash, (num2 > this.entity.moveSpeedAggro * this.entity.moveSpeedAggro) ? 3 : ((num2 > this.entity.moveSpeed * this.entity.moveSpeed) ? 2 : ((num2 > 0.001f) ? 1 : 0)), true);
			}
		}
		if (base.IsMoving(speedForward, speedStrafe))
		{
			this.idleTime = 0f;
			this._setBool(AvatarController.isMovingHash, true, true);
		}
		else
		{
			this._setBool(AvatarController.isMovingHash, false, true);
		}
		if (this.useIdle)
		{
			float num3 = this.idleTime - this.idleTimeSent;
			if (num3 * num3 > 0.25f)
			{
				this.idleTimeSent = this.idleTime;
				this._setFloat(AvatarController.idleTimeHash, this.idleTime, false);
			}
			this.idleTime += Time.deltaTime;
		}
		float a;
		this.TryGetFloat(AvatarController.rotationPitchHash, out a);
		float value2 = Mathf.Lerp(a, this.entity.rotation.x, Time.deltaTime * 12f);
		this._setFloat(AvatarController.rotationPitchHash, value2, false);
		float a2;
		this.TryGetFloat(AvatarController.yLookHash, out a2);
		base.UpdateFloat(AvatarController.yLookHash, Mathf.Lerp(a2, -base.Entity.rotation.x / 90f, Time.deltaTime * 12f), false);
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x000248D7 File Offset: 0x00022AD7
	public virtual Transform GetMeshTransform()
	{
		return this.bipedTransform;
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x000248DF File Offset: 0x00022ADF
	public override void SetArchetypeStance(NPCInfo.StanceTypes stance)
	{
		if (this.anim != null && this.anim.avatar.isValid && this.anim.enabled)
		{
			this._setInt(AvatarController.archetypeStanceHash, (int)stance, true);
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0002491B File Offset: 0x00022B1B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setTrigger(int _propertyHash, bool _netsync = true)
	{
		base._setTrigger(_propertyHash, _netsync);
		if (this.rightHandAnimator != null && this.rightHandAnimator.runtimeAnimatorController != null)
		{
			this.rightHandAnimator.SetTrigger(_propertyHash);
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00024954 File Offset: 0x00022B54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _resetTrigger(int _propertyHash, bool _netsync = true)
	{
		base._resetTrigger(_propertyHash, _netsync);
		if (this.rightHandAnimator != null && this.rightHandAnimator.runtimeAnimatorController != null && this.rightHandAnimator.GetBool(_propertyHash))
		{
			this.rightHandAnimator.ResetTrigger(_propertyHash);
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x000249A4 File Offset: 0x00022BA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setFloat(int _propertyHash, float _value, bool _netsync = true)
	{
		base._setFloat(_propertyHash, _value, _netsync);
		if (this.rightHandAnimator != null && this.rightHandAnimator.runtimeAnimatorController != null)
		{
			this.rightHandAnimator.SetFloat(_propertyHash, _value);
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x000249DD File Offset: 0x00022BDD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setBool(int _propertyHash, bool _value, bool _netsync = true)
	{
		base._setBool(_propertyHash, _value, _netsync);
		if (this.rightHandAnimator != null && this.rightHandAnimator.runtimeAnimatorController != null)
		{
			this.rightHandAnimator.SetBool(_propertyHash, _value);
		}
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x00024A16 File Offset: 0x00022C16
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _propertyHash, int _value, bool _netsync = true)
	{
		base._setInt(_propertyHash, _value, _netsync);
		if (this.rightHandAnimator != null && this.rightHandAnimator.runtimeAnimatorController != null)
		{
			this.rightHandAnimator.SetInteger(_propertyHash, _value);
		}
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x00024A4F File Offset: 0x00022C4F
	[PublicizedFrom(EAccessModifier.Protected)]
	public LegacyAvatarController()
	{
	}

	// Token: 0x040005A9 RID: 1449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cPitchUpdateSpeed = 12f;

	// Token: 0x040005AA RID: 1450
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int jumpState;

	// Token: 0x040005AB RID: 1451
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int fpvJumpState;

	// Token: 0x040005AC RID: 1452
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isJumpStarted;

	// Token: 0x040005AD RID: 1453
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public HashSet<int> reloadStates = new HashSet<int>();

	// Token: 0x040005AE RID: 1454
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public HashSet<int> deathStates = new HashSet<int>();

	// Token: 0x040005AF RID: 1455
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_bVisible;

	// Token: 0x040005B0 RID: 1456
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bFPV;

	// Token: 0x040005B1 RID: 1457
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bMale;

	// Token: 0x040005B2 RID: 1458
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo baseStateInfo;

	// Token: 0x040005B3 RID: 1459
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo currentWeaponHoldLayer;

	// Token: 0x040005B4 RID: 1460
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo currentUpperBodyState;

	// Token: 0x040005B5 RID: 1461
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float lastAbsMotionX;

	// Token: 0x040005B6 RID: 1462
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float lastAbsMotionZ;

	// Token: 0x040005B7 RID: 1463
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float lastAbsMotion;

	// Token: 0x040005B8 RID: 1464
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform bipedTransform;

	// Token: 0x040005B9 RID: 1465
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelTransform;

	// Token: 0x040005BA RID: 1466
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform rightHand;

	// Token: 0x040005BB RID: 1467
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform pelvis;

	// Token: 0x040005BC RID: 1468
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform spine;

	// Token: 0x040005BD RID: 1469
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform spine1;

	// Token: 0x040005BE RID: 1470
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform spine2;

	// Token: 0x040005BF RID: 1471
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform spine3;

	// Token: 0x040005C0 RID: 1472
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform head;

	// Token: 0x040005C1 RID: 1473
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform cameraNode;

	// Token: 0x040005C2 RID: 1474
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeAttackAnimationPlaying;

	// Token: 0x040005C3 RID: 1475
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isAttackImpact;

	// Token: 0x040005C4 RID: 1476
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeUseAnimationPlaying;

	// Token: 0x040005C5 RID: 1477
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeHarestingAnimationPlaying;

	// Token: 0x040005C6 RID: 1478
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform rightHandItemTransform;

	// Token: 0x040005C7 RID: 1479
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Animator rightHandAnimator;

	// Token: 0x040005C8 RID: 1480
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bIsRagdoll;

	// Token: 0x040005C9 RID: 1481
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool useIdle = true;

	// Token: 0x040005CA RID: 1482
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float idleTime;

	// Token: 0x040005CB RID: 1483
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float idleTimeSent;

	// Token: 0x040005CC RID: 1484
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int itemUseTicks;

	// Token: 0x040005CD RID: 1485
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int movementStateOverride = -1;

	// Token: 0x040005CE RID: 1486
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string modelName;

	// Token: 0x040005CF RID: 1487
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isInDeathAnim;

	// Token: 0x040005D0 RID: 1488
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool didDeathTransition;

	// Token: 0x040005D1 RID: 1489
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bSpecialAttackPlaying;

	// Token: 0x040005D2 RID: 1490
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeSpecialAttack2Playing;
}
