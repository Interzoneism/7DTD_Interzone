using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.DuckType.Jiggle;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000B7 RID: 183
[Preserve]
public class AvatarZombieController : AvatarController
{
	// Token: 0x06000443 RID: 1091 RVA: 0x0001DB98 File Offset: 0x0001BD98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.modelT = EModelBase.FindModel(base.transform);
		this.assignStates();
	}

	// Token: 0x06000444 RID: 1092 RVA: 0x0001DBB7 File Offset: 0x0001BDB7
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		this.hitLayerIndex = 3;
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x0001DBC0 File Offset: 0x0001BDC0
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (!this.bipedT)
		{
			this.bipedT = this.entity.emodel.GetModelTransform();
			this.rightHandT = this.FindTransform(this.entity.GetRightHandTransformName());
			base.SetAnimator(this.bipedT);
			if (this.entity.RootMotion)
			{
				this.rootMotion = this.bipedT.gameObject.AddComponent<AvatarRootMotion>();
				this.rootMotion.Init(this, this.anim);
			}
		}
		this.SetWalkType(this.entity.GetWalkType(), false);
		this._setBool(AvatarController.isDeadHash, this.entity.IsDead(), true);
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x0001DC71 File Offset: 0x0001BE71
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform FindTransform(string _name)
	{
		return this.bipedT.FindInChildren(_name);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x0001DC80 File Offset: 0x0001BE80
	public override void SetVisible(bool _b)
	{
		if (this.isVisible != _b || !this.isVisibleInit)
		{
			this.isVisible = _b;
			this.isVisibleInit = true;
			Transform transform = this.bipedT;
			if (transform)
			{
				Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].enabled = _b;
				}
			}
		}
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x0001DCDA File Offset: 0x0001BEDA
	public override Transform GetActiveModelRoot()
	{
		return this.modelT;
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x0001DCE2 File Offset: 0x0001BEE2
	public override Transform GetRightHandTransform()
	{
		return this.rightHandT;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x0001DCEC File Offset: 0x0001BEEC
	public override void SetInRightHand(Transform _transform)
	{
		this.idleTime = 0f;
		if (_transform)
		{
			Quaternion identity = Quaternion.identity;
			_transform.SetParent(this.GetRightHandTransform(), false);
			if (this.entity.inventory != null && this.entity.inventory.holdingItem != null)
			{
				AnimationGunjointOffsetData.AnimationGunjointOffsets animationGunjointOffsets = AnimationGunjointOffsetData.AnimationGunjointOffset[this.entity.inventory.holdingItem.HoldType.Value];
				_transform.localPosition = animationGunjointOffsets.position;
				_transform.localRotation = Quaternion.Euler(animationGunjointOffsets.rotation);
				return;
			}
			_transform.localPosition = Vector3.zero;
			_transform.localRotation = identity;
		}
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x0001DD98 File Offset: 0x0001BF98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		float deltaTime = Time.deltaTime;
		if (this.actionTimeActive > 0f)
		{
			this.actionTimeActive -= deltaTime;
		}
		if (this.attackPlayingTime > 0f)
		{
			this.attackPlayingTime -= deltaTime;
			if (this.attackPlayingTime <= 0f)
			{
				this.isAttackImpact = true;
			}
		}
		if (this.timeSpecialAttack2Playing > 0f)
		{
			this.timeSpecialAttack2Playing -= deltaTime;
		}
		if (this.timeRagePlaying > 0f)
		{
			this.timeRagePlaying -= deltaTime;
		}
		if (!this.isVisible && (!this.entity || !this.entity.RootMotion || this.entity.isEntityRemote))
		{
			return;
		}
		if (!this.bipedT || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!this.anim || !this.anim.avatar.isValid || !this.anim.enabled)
		{
			return;
		}
		this.UpdateLayerStateInfo();
		this.SetLayerWeights();
		float speedForward = this.entity.speedForward;
		this._setFloat(AvatarController.forwardHash, speedForward, false);
		if (!this.entity.IsDead())
		{
			if (this.movementStateOverride != -1)
			{
				this._setInt(AvatarController.movementStateHash, this.movementStateOverride, true);
				this.movementStateOverride = -1;
			}
			else
			{
				float num = speedForward * speedForward;
				this._setInt(AvatarController.movementStateHash, (num > this.entity.moveSpeedAggro * this.entity.moveSpeedAggro) ? 3 : ((num > this.entity.moveSpeed * this.entity.moveSpeed) ? 2 : ((num > 0.001f) ? 1 : 0)), false);
			}
		}
		if (this.electrocuteTime > 0.3f && !this.entity.emodel.IsRagdollActive)
		{
			this._setTrigger(AvatarController.isElectrocutedHash, true);
		}
		if (!this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.entity.IsInElevator() || this.entity.Climbing)
		{
			this._setBool(AvatarController.isClimbingHash, true, true);
			return;
		}
		this._setBool(AvatarController.isClimbingHash, false, true);
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x0001DFD0 File Offset: 0x0001C1D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void LateUpdate()
	{
		if (!this.entity || !this.bipedT || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!this.anim || !this.anim.enabled)
		{
			return;
		}
		this.UpdateLayerStateInfo();
		ItemClass holdingItem = this.entity.inventory.holdingItem;
		if (holdingItem.Actions[0] != null)
		{
			holdingItem.Actions[0].UpdateNozzleParticlesPosAndRot(this.entity.inventory.holdingItemData.actionData[0]);
		}
		if (holdingItem.Actions[1] != null)
		{
			holdingItem.Actions[1].UpdateNozzleParticlesPosAndRot(this.entity.inventory.holdingItemData.actionData[1]);
		}
		int fullPathHash = this.baseStateInfo.fullPathHash;
		bool flag = this.anim.IsInTransition(0);
		if (!flag)
		{
			this.isJumpStarted = false;
			if (fullPathHash == this.jumpState)
			{
				this._setBool(AvatarController.jumpHash, false, true);
			}
		}
		if (this.isInDeathAnim)
		{
			if (this.baseStateInfo.tagHash == AvatarController.deathHash && this.baseStateInfo.normalizedTime >= 1f && !flag)
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
		if (this.isCrawler && Time.time - this.crawlerTime > 2f)
		{
			this.isSuppressPain = false;
		}
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x0001E1A8 File Offset: 0x0001C3A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLayerStateInfo()
	{
		this.baseStateInfo = this.anim.GetCurrentAnimatorStateInfo(0);
		this.overrideStateInfo = this.anim.GetCurrentAnimatorStateInfo(1);
		this.fullBodyStateInfo = this.anim.GetCurrentAnimatorStateInfo(2);
		if (this.anim.layerCount > 3)
		{
			this.hitStateInfo = this.anim.GetCurrentAnimatorStateInfo(3);
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001E20C File Offset: 0x0001C40C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetLayerWeights()
	{
		this.isSuppressPain = (this.isSuppressPain && (this.anim.IsInTransition(2) || this.fullBodyStateInfo.fullPathHash != 0));
		this.anim.SetLayerWeight(1, 1f);
		this.anim.SetLayerWeight(2, (float)((this.isSuppressPain || this.entity.bodyDamage.CurrentStun != EnumEntityStunType.None) ? 0 : 1));
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x0001E285 File Offset: 0x0001C485
	public override void ResetAnimations()
	{
		base.ResetAnimations();
		this.anim.Play("None", 1, 0f);
		this.anim.Play("None", 2, 0f);
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetMeleeAttackSpeed(float _speed)
	{
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x0001E2BC File Offset: 0x0001C4BC
	public override AvatarController.ActionState GetActionState()
	{
		if (this.attackPlayingTime > 0f || this.overrideStateInfo.tagHash == AvatarController.attackHash)
		{
			return AvatarController.ActionState.Active;
		}
		int tagHash = this.fullBodyStateInfo.tagHash;
		if (tagHash == AvatarController.attackStartHash || this.actionTimeActive > 0f)
		{
			return AvatarController.ActionState.Start;
		}
		if (tagHash == AvatarController.attackReadyHash)
		{
			return AvatarController.ActionState.Ready;
		}
		if (tagHash == AvatarController.attackHash)
		{
			return AvatarController.ActionState.Active;
		}
		return AvatarController.ActionState.None;
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00018278 File Offset: 0x00016478
	public override bool IsActionActive()
	{
		return this.GetActionState() > AvatarController.ActionState.None;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x0001E322 File Offset: 0x0001C522
	public override void StartAction(int _animType)
	{
		if (_animType < 3000)
		{
			this.StartAnimationAttack();
			return;
		}
		this.idleTime = 0f;
		this._setInt(AvatarController.attackHash, _animType, true);
		this._setTrigger(AvatarController.attackTriggerHash, true);
		this.actionTimeActive = 0.2f;
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x0001E362 File Offset: 0x0001C562
	public override bool IsAnimationAttackPlaying()
	{
		return this.attackPlayingTime > 0f || this.overrideStateInfo.tagHash == AvatarController.attackHash || this.fullBodyStateInfo.tagHash == AvatarController.attackHash;
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0001E398 File Offset: 0x0001C598
	public override void StartAnimationAttack()
	{
		if (!this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		this.idleTime = 0f;
		this.isAttackImpact = false;
		this.attackPlayingTime = 2f;
		float randomFloat = this.entity.rand.RandomFloat;
		int num = -1;
		if (!this.rightArmDismembered)
		{
			num = 0;
			if (!this.leftArmDismembered)
			{
				num = (this.entity.rand.RandomInt & 1);
			}
		}
		else if (!this.leftArmDismembered)
		{
			num = 1;
		}
		int num2 = 8;
		if (num >= 0)
		{
			num2 = num;
		}
		int walkType = this.entity.GetWalkType();
		if (walkType >= 20)
		{
			num2 += walkType * 100;
		}
		if (this.entity.IsBreakingDoors && num >= 0)
		{
			num2 += 10;
		}
		if (num2 <= 1)
		{
			if (walkType == 1)
			{
				num2 += 100;
			}
			else if (this.entity.rand.RandomFloat < 0.25f)
			{
				num2 += 4;
			}
		}
		this._setInt(AvatarController.attackHash, num2, true);
		this._setFloat(AvatarController.attackBlendHash, randomFloat, true);
		this._setTrigger(AvatarController.attackTriggerHash, true);
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x0001E49F File Offset: 0x0001C69F
	public override void SetAttackImpact()
	{
		if (!this.isAttackImpact)
		{
			this.isAttackImpact = true;
			this.attackPlayingTime = 0.1f;
		}
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0001E4BB File Offset: 0x0001C6BB
	public override bool IsAttackImpact()
	{
		return this.isAttackImpact;
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x0001E4C4 File Offset: 0x0001C6C4
	public override bool IsAnimationHitRunning()
	{
		if (this.hitWeight == 0f)
		{
			return false;
		}
		int tagHash = this.hitStateInfo.tagHash;
		return tagHash == AvatarController.hitStartHash || (tagHash == AvatarController.hitHash && this.hitStateInfo.normalizedTime < 0.55f) || this.anim.IsInTransition(3);
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsAnimationSpecialAttackPlaying()
	{
		return false;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00002914 File Offset: 0x00000B14
	public override void StartAnimationSpecialAttack(bool _b, int _animType)
	{
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0001E51C File Offset: 0x0001C71C
	public override bool IsAnimationSpecialAttack2Playing()
	{
		return this.timeSpecialAttack2Playing > 0f;
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x0001E52B File Offset: 0x0001C72B
	public override void StartAnimationSpecialAttack2()
	{
		this.idleTime = 0f;
		this.timeSpecialAttack2Playing = 0.3f;
		this._setTrigger(AvatarController.specialAttack2Hash, true);
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0001E54F File Offset: 0x0001C74F
	public override bool IsAnimationRagingPlaying()
	{
		return this.timeRagePlaying > 0f;
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x0001E55E File Offset: 0x0001C75E
	public override void StartAnimationRaging()
	{
		this.idleTime = 0f;
		this._setTrigger(AvatarController.rageHash, true);
		this.timeRagePlaying = 0.3f;
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x0001E582 File Offset: 0x0001C782
	public override void StartAnimationElectrocute(float _duration)
	{
		base.StartAnimationElectrocute(_duration);
		this.idleTime = 0f;
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001E596 File Offset: 0x0001C796
	public override bool IsAnimationDigRunning()
	{
		return AvatarController.digHash == this.baseStateInfo.tagHash;
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x0001E5AA File Offset: 0x0001C7AA
	public override void StartAnimationDodge(float _blend)
	{
		this._setFloat(AvatarController.dodgeBlendHash, _blend, true);
		this._setBool(AvatarController.dodgeTriggerHash, true, true);
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x0001E5C8 File Offset: 0x0001C7C8
	public override void StartAnimationJumping()
	{
		this.idleTime = 0f;
		if (this.bipedT == null || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			this._setBool(AvatarController.jumpHash, true, true);
		}
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x0001E61C File Offset: 0x0001C81C
	public override void StartAnimationJump(AnimJumpMode jumpMode)
	{
		this.idleTime = 0f;
		if (this.bipedT == null || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		this.isJumpStarted = true;
		if (this.anim != null)
		{
			if (jumpMode == AnimJumpMode.Start)
			{
				this._setTrigger(AvatarController.jumpStartHash, true);
				return;
			}
			this._setTrigger(AvatarController.jumpLandHash, true);
			this._setInt(AvatarController.jumpLandResponseHash, 0, true);
		}
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x0001E693 File Offset: 0x0001C893
	public override bool IsAnimationJumpRunning()
	{
		return this.isJumpStarted || AvatarController.jumpHash == this.baseStateInfo.tagHash;
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x0001E6B4 File Offset: 0x0001C8B4
	public override bool IsAnimationWithMotionRunning()
	{
		int tagHash = this.baseStateInfo.tagHash;
		return tagHash == AvatarController.jumpHash || tagHash == AvatarController.moveHash;
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0001E6E0 File Offset: 0x0001C8E0
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

	// Token: 0x06000467 RID: 1127 RVA: 0x0001E724 File Offset: 0x0001C924
	public override void BeginStun(EnumEntityStunType stun, EnumBodyPartHit _bodyPart, Utils.EnumHitDirection _hitDirection, bool _criticalHit, float random)
	{
		this._setInt(AvatarController.stunTypeHash, (int)stun, true);
		this._setInt(AvatarController.stunBodyPartHash, (int)_bodyPart.ToPrimary().LowerToUpperLimb(), true);
		this._setInt(AvatarController.hitDirectionHash, (int)_hitDirection, true);
		this._setFloat(AvatarController.HitRandomValueHash, random, true);
		this._setTrigger(AvatarController.beginStunTriggerHash, true);
		this._resetTrigger(AvatarController.endStunTriggerHash, true);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x0001E788 File Offset: 0x0001C988
	public override void EndStun()
	{
		this._setTrigger(AvatarController.endStunTriggerHash, true);
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001E796 File Offset: 0x0001C996
	public override bool IsAnimationStunRunning()
	{
		return this.baseStateInfo.tagHash == AvatarController.stunHash;
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0001E7B0 File Offset: 0x0001C9B0
	public override void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
		this.idleTime = 0f;
		this.isInDeathAnim = true;
		this.didDeathTransition = false;
		if (this.bipedT == null || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			this.movementStateOverride = _movementState;
			this._setInt(AvatarController.movementStateHash, _movementState, true);
			this._setBool(AvatarController.isAliveHash, false, true);
			this._setInt(AvatarController.hitBodyPartHash, (int)_bodyPart.ToPrimary().LowerToUpperLimb(), true);
			this._setFloat(AvatarController.HitRandomValueHash, random, true);
			this.SetFallAndGround(false, this.entity.onGround);
		}
		if (this.bipedT == null || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.anim != null)
		{
			this._setTrigger(AvatarController.deathTriggerHash, true);
		}
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x0001E897 File Offset: 0x0001CA97
	public override void StartEating()
	{
		if (!this.isEating)
		{
			this._setInt(AvatarController.attackHash, 0, true);
			this._setTrigger(AvatarController.beginCorpseEatHash, true);
			this.isEating = true;
		}
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0001E8C1 File Offset: 0x0001CAC1
	public override void StopEating()
	{
		if (this.isEating)
		{
			this._setInt(AvatarController.attackHash, 0, true);
			this._setTrigger(AvatarController.endCorpseEatHash, true);
			this.isEating = false;
		}
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0001E8EB File Offset: 0x0001CAEB
	public override void StartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float _random, float _duration)
	{
		if (!this.isCrawler || Time.time - this.crawlerTime > 2f)
		{
			this.InternalStartAnimationHit(_bodyPart, _dir, _hitDamage, _criticalHit, _movementState, _random, _duration);
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0001E91C File Offset: 0x0001CB1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InternalStartAnimationHit(EnumBodyPartHit _bodyPart, int _dir, int _hitDamage, bool _criticalHit, int _movementState, float random, float _duration)
	{
		if (this.bipedT == null || !this.bipedT.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!base.CheckHit(_duration))
		{
			this.SetDataFloat(AvatarController.DataTypes.HitDuration, _duration, true);
			return;
		}
		this.idleTime = 0f;
		if (this.anim)
		{
			this.movementStateOverride = _movementState;
			this._setInt(AvatarController.movementStateHash, _movementState, true);
			this._setInt(AvatarController.hitDirectionHash, _dir, true);
			this._setInt(AvatarController.hitDamageHash, _hitDamage, true);
			this._setFloat(AvatarController.HitRandomValueHash, random, true);
			this._setInt(AvatarController.hitBodyPartHash, (int)_bodyPart.ToPrimary().LowerToUpperLimb(), true);
			this.SetDataFloat(AvatarController.DataTypes.HitDuration, _duration, true);
			this._setTrigger(AvatarController.hitTriggerHash, true);
		}
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x0600046F RID: 1135 RVA: 0x0001E9E2 File Offset: 0x0001CBE2
	public bool IsCrippled
	{
		get
		{
			return this.isCrippled;
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001E9EC File Offset: 0x0001CBEC
	public override void CrippleLimb(BodyDamage _bodyDamage, bool restoreState)
	{
		if (this.isCrippled)
		{
			return;
		}
		if (_bodyDamage.bodyPartHit.IsLeg())
		{
			int walkType = this.entity.GetWalkType();
			if (walkType != 5 && walkType < 20)
			{
				this.isCrippled = true;
				this.SetWalkType(5, false);
				this._setTrigger(AvatarController.movementTriggerHash, true);
			}
		}
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000471 RID: 1137 RVA: 0x0001EA3F File Offset: 0x0001CC3F
	public bool rightArmDismembered
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.rightUpperArmDismembered || this.rightLowerArmDismembered;
		}
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000472 RID: 1138 RVA: 0x0001EA51 File Offset: 0x0001CC51
	public bool leftArmDismembered
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.leftUpperArmDismembered || this.leftLowerArmDismembered;
		}
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x0001EA64 File Offset: 0x0001CC64
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isCensoredContent()
	{
		EntityClass entityClass = this.entity.EntityClass;
		if ((entityClass != null && entityClass.censorMode == 0) || !GameManager.Instance.IsGoreCensored())
		{
			return false;
		}
		EntityClass entityClass2 = this.entity.EntityClass;
		if (entityClass2 == null || entityClass2.censorType != 2)
		{
			EntityClass entityClass3 = this.entity.EntityClass;
			return entityClass3 != null && entityClass3.censorType == 3;
		}
		return true;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0001EAD4 File Offset: 0x0001CCD4
	public void CleanupDismemberedLimbs()
	{
		for (int i = 0; i < this.dismemberedParts.Count; i++)
		{
			this.dismemberedParts[i].ReadyForCleanup = true;
		}
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0001EB0C File Offset: 0x0001CD0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void _InitDismembermentMaterials()
	{
		if (!this.mainZombieMaterial)
		{
			EModelBase emodel = this.entity.emodel;
			if (emodel)
			{
				Transform meshTransform = emodel.meshTransform;
				if (meshTransform)
				{
					Renderer component = meshTransform.GetComponent<Renderer>();
					if (component)
					{
						this.mainZombieMaterial = component.sharedMaterial;
						bool flag = this.entity.HasAnyTags(DismembermentManager.radiatedTag) && (this.mainZombieMaterial.HasProperty("_IsRadiated") || this.mainZombieMaterial.HasProperty("_Irradiated"));
						DismembermentManager instance = DismembermentManager.Instance;
						this.gibCapMaterial = ((!flag) ? instance.GibCapsMaterial : instance.GibCapsRadMaterial);
					}
				}
			}
			this.isCensored = this.isCensoredContent();
		}
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0001EBD4 File Offset: 0x0001CDD4
	public override void RemoveLimb(BodyDamage _bodyDamage, bool restoreState)
	{
		if (DismembermentManager.DebugDontCreateParts)
		{
			return;
		}
		DismembermentManager instance = DismembermentManager.Instance;
		int num = (instance != null) ? instance.parts.Count : 0;
		if (this.entity.isDisintegrated && num >= 25)
		{
			return;
		}
		this._InitDismembermentMaterials();
		EnumBodyPartHit bodyPartHit = _bodyDamage.bodyPartHit;
		EnumDamageTypes enumDamageTypes = _bodyDamage.damageType;
		bool flag = enumDamageTypes == EnumDamageTypes.Heat;
		if (this.isCensored)
		{
			List<string> bluntCensors = DismembermentManager.BluntCensors;
			EntityClass entityClass = this.entity.EntityClass;
			if (bluntCensors.Contains((entityClass != null) ? entityClass.entityClassName : null))
			{
				enumDamageTypes = EnumDamageTypes.Bashing;
			}
			else
			{
				enumDamageTypes = EnumDamageTypes.Piercing;
			}
		}
		int num2 = 0;
		if (!this.headDismembered && (bodyPartHit & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
		{
			this.headDismembered = true;
			num2++;
			if (flag || this.entity.OverrideHeadSize != 1f)
			{
				enumDamageTypes = EnumDamageTypes.Piercing;
			}
			Transform partT = this.FindTransform("Neck");
			this.MakeDismemberedPart(1U, enumDamageTypes, partT, restoreState);
			Transform transform = this.bipedT.Find("HeadAccessories");
			if (transform)
			{
				transform.gameObject.SetActive(false);
			}
		}
		if (!this.leftUpperLegDismembered && (bodyPartHit & EnumBodyPartHit.LeftUpperLeg) > EnumBodyPartHit.None)
		{
			this.leftUpperLegDismembered = true;
			num2++;
			Transform partT2 = this.FindTransform("LeftUpLeg");
			this.MakeDismemberedPart(32U, enumDamageTypes, partT2, restoreState);
		}
		if (!this.leftLowerLegDismembered && !this.leftUpperLegDismembered && (bodyPartHit & EnumBodyPartHit.LeftLowerLeg) > EnumBodyPartHit.None)
		{
			this.leftLowerLegDismembered = true;
			num2++;
			Transform partT3 = this.FindTransform("LeftLeg");
			this.MakeDismemberedPart(64U, enumDamageTypes, partT3, restoreState);
		}
		if (!this.rightUpperLegDismembered && (bodyPartHit & EnumBodyPartHit.RightUpperLeg) > EnumBodyPartHit.None)
		{
			this.rightUpperLegDismembered = true;
			num2++;
			Transform partT4 = this.FindTransform("RightUpLeg");
			this.MakeDismemberedPart(128U, enumDamageTypes, partT4, restoreState);
		}
		if (!this.rightLowerLegDismembered && !this.rightUpperLegDismembered && (bodyPartHit & EnumBodyPartHit.RightLowerLeg) > EnumBodyPartHit.None)
		{
			this.rightLowerLegDismembered = true;
			num2++;
			Transform partT5 = this.FindTransform("RightLeg");
			this.MakeDismemberedPart(256U, enumDamageTypes, partT5, restoreState);
		}
		if (!this.leftUpperArmDismembered && (bodyPartHit & EnumBodyPartHit.LeftUpperArm) > EnumBodyPartHit.None)
		{
			this.leftUpperArmDismembered = true;
			num2++;
			Transform partT6 = this.FindTransform("LeftArm");
			this.MakeDismemberedPart(2U, enumDamageTypes, partT6, restoreState);
		}
		if (!this.leftLowerArmDismembered && !this.leftUpperArmDismembered && (bodyPartHit & EnumBodyPartHit.LeftLowerArm) > EnumBodyPartHit.None)
		{
			this.leftLowerArmDismembered = true;
			num2++;
			Transform partT7 = this.FindTransform("LeftForeArm");
			this.MakeDismemberedPart(4U, enumDamageTypes, partT7, restoreState);
		}
		if (!this.rightUpperArmDismembered && (bodyPartHit & EnumBodyPartHit.RightUpperArm) > EnumBodyPartHit.None)
		{
			this.rightUpperArmDismembered = true;
			num2++;
			Transform partT8 = this.FindTransform("RightArm");
			this.MakeDismemberedPart(8U, enumDamageTypes, partT8, restoreState);
		}
		if (!this.rightLowerArmDismembered && !this.rightUpperArmDismembered && (bodyPartHit & EnumBodyPartHit.RightLowerArm) > EnumBodyPartHit.None)
		{
			this.rightLowerArmDismembered = true;
			num2++;
			Transform partT9 = this.FindTransform("RightForeArm");
			this.MakeDismemberedPart(16U, enumDamageTypes, partT9, restoreState);
		}
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0001EEA8 File Offset: 0x0001D0A8
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform SpawnLimbGore(Transform parent, string path, bool restoreState)
	{
		if (!parent || string.IsNullOrEmpty(path))
		{
			return null;
		}
		string text = DismembermentManager.GetAssetBundlePath(path);
		GameObject gameObject = null;
		if (this.isCensored)
		{
			string text2 = text.Replace(".", "_CGore.");
			GameObject gameObject2 = DataLoader.LoadAsset<GameObject>(text2, false);
			if (gameObject2)
			{
				text = text2;
				gameObject = gameObject2;
			}
		}
		if (!gameObject)
		{
			gameObject = DataLoader.LoadAsset<GameObject>(text, false);
		}
		if (!gameObject)
		{
			return null;
		}
		GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject, parent);
		GorePrefab component = gameObject3.GetComponent<GorePrefab>();
		if (component)
		{
			component.restoreState = restoreState;
		}
		return gameObject3.transform;
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0001EF3C File Offset: 0x0001D13C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ProcDismemberedPart(Transform t, Transform partT, DismemberedPartData part, uint bodyDamageFlag)
	{
		Transform transform = partT.FindRecursive(part.targetBone);
		if (transform)
		{
			if (!part.attachToParent)
			{
				Vector3 localScale = t.localScale;
				localScale.x /= Utils.FastMax(0.01f, transform.localScale.x);
				localScale.y /= Utils.FastMax(0.01f, transform.localScale.y);
				localScale.z /= Utils.FastMax(0.01f, transform.localScale.z);
				t.localScale = localScale;
			}
			if (!string.IsNullOrEmpty(part.childTargetObj))
			{
				Transform transform2 = new GameObject("scaleTarget").transform;
				transform2.position = transform.position;
				for (int i = 0; i < transform.childCount; i++)
				{
					transform.GetChild(i).SetParent(transform2);
				}
				transform2.SetParent(transform.parent);
				transform.SetParent(transform2);
				transform2.localScale = Vector3.zero;
			}
			if (!string.IsNullOrEmpty(part.insertBoneObj))
			{
				Transform transform3 = new GameObject("scaleTarget").transform;
				transform3.position = transform.position;
				for (int j = 0; j < transform.childCount; j++)
				{
					transform.GetChild(j).SetParent(transform3);
				}
				transform3.SetParent(transform);
				if (this.defaultHeadPos != Vector3.zero)
				{
					transform3.position = this.defaultHeadPos;
					Vector3 localPosition = transform3.localPosition;
					localPosition.z = -localPosition.y * 0.5f;
					transform3.localPosition = localPosition;
				}
				transform3.localScale = Vector3.zero;
			}
		}
		if (part.hasRotOffset)
		{
			t.localEulerAngles = part.rot;
		}
		if (DismembermentManager.DebugShowArmRotations)
		{
			DismembermentManager.AddDebugArmObjects(partT, t);
		}
		if (part.offset != Vector3.zero)
		{
			Transform transform4 = t.FindRecursive("pos");
			if (transform4)
			{
				transform4.localPosition += part.offset;
			}
		}
		if (part.particlePaths != null)
		{
			for (int k = 0; k < part.particlePaths.Length; k++)
			{
				string text = part.particlePaths[k];
				if (!string.IsNullOrEmpty(text))
				{
					DismembermentManager.SpawnParticleEffect(new ParticleEffect(text, t.position + Origin.position, Quaternion.identity, 1f, Color.white), -1);
				}
			}
		}
		Transform transform5 = t.FindRecursive("pos");
		if (transform5)
		{
			Renderer[] componentsInChildren = transform5.GetComponentsInChildren<Renderer>(true);
			Material altMaterial = this.entity.emodel.AltMaterial;
			if (altMaterial)
			{
				this.altMatName = altMaterial.name;
				for (int l = 0; l < this.altMatName.Length; l++)
				{
					char c = this.altMatName[l];
					if (char.IsDigit(c))
					{
						this.altEntityMatId = int.Parse(c.ToString());
						break;
					}
				}
			}
			else
			{
				foreach (char c2 in this.mainZombieMaterial.name)
				{
					if (char.IsDigit(c2))
					{
						this.altEntityMatId = int.Parse(c2.ToString());
						break;
					}
				}
			}
			foreach (Renderer renderer in componentsInChildren)
			{
				if (!renderer.GetComponent<ParticleSystem>())
				{
					Material[] sharedMaterials = renderer.sharedMaterials;
					for (int num = 0; num < sharedMaterials.Length; num++)
					{
						Material material = sharedMaterials[num];
						string name2 = material.name;
						if ((!part.prefabPath.ContainsCaseInsensitive("head") || !name2.ContainsCaseInsensitive("hair")) && (!renderer.name.ContainsCaseInsensitive("eye") || material.HasProperty("_IsRadiated") || material.HasProperty("_Irradiated")))
						{
							bool flag = false;
							int num2 = 0;
							while (num2 < DismembermentManager.DefaultBundleGibs.Length)
							{
								flag = name2.ContainsCaseInsensitive(DismembermentManager.DefaultBundleGibs[num2]);
								if (flag)
								{
									if (name2.ContainsCaseInsensitive("ZombieGibs_caps"))
									{
										if (!this.gibCapMaterialCopy)
										{
											this.gibCapMaterialCopy = UnityEngine.Object.Instantiate<Material>(this.gibCapMaterial);
											this.gibCapMaterialCopy.name = this.gibCapMaterial.name.Replace("(global)", "(local)");
										}
										sharedMaterials[num] = this.gibCapMaterialCopy;
										break;
									}
									break;
								}
								else
								{
									num2++;
								}
							}
							if (!flag && material.name.Contains("HD_"))
							{
								if (!this.mainZombieMaterialCopy)
								{
									this.mainZombieMaterialCopy = UnityEngine.Object.Instantiate<Material>(this.mainZombieMaterial);
								}
								sharedMaterials[num] = this.mainZombieMaterialCopy;
							}
						}
					}
					renderer.materials = sharedMaterials;
				}
			}
		}
		if (this.entity.IsFeral && bodyDamageFlag == 1U)
		{
			this.setUpEyeMats(t);
			if (part.isDetachable)
			{
				Transform transform6 = t.FindRecursive("Detachable");
				if (transform6)
				{
					this.setUpEyeMats(transform6);
				}
			}
			Transform transform7 = t.FindRecursive("FeralFlame");
			if (transform7 && !this.entity.HasAnyTags(DismembermentManager.radiatedTag))
			{
				transform7.gameObject.SetActive(true);
				string text2 = "large_flames_LOD (3)";
				Transform transform8 = this.entity.transform.FindRecursive(text2);
				if (transform8)
				{
					transform8.gameObject.SetActive(false);
				}
				else
				{
					Log.Warning("entity {0} no longer has a child named {1}", new object[]
					{
						this.entity.name,
						text2
					});
				}
			}
		}
		if (!this.dismemberMat && !string.IsNullOrEmpty(this.subFolderDismemberEntityName))
		{
			string text3 = this.rootDismmemberDir + string.Format("/gibs_{0}", this.subFolderDismemberEntityName.ToLower());
			Material sharedMaterial = this.skinnedMeshRenderer.sharedMaterial;
			if (this.entity.HasAnyTags(DismembermentManager.radiatedTag) && (sharedMaterial.HasProperty("_IsRadiated") || sharedMaterial.HasProperty("_Irradiated")))
			{
				text3 += "_IsRadiated";
			}
			Material material2 = null;
			if (!string.IsNullOrEmpty(part.dismemberMatPath))
			{
				string text4 = this.rootDismmemberDir + "/" + part.dismemberMatPath + ".mat";
				material2 = DataLoader.LoadAsset<Material>(text4, false);
				if (material2)
				{
					text3 = text4;
				}
			}
			string str = text3;
			object obj = (this.altEntityMatId != -1) ? this.altEntityMatId : "";
			text3 = str + ((obj != null) ? obj.ToString() : null);
			if (this.isCensored)
			{
				string text5 = text3;
				text3 += "_CGore.mat";
				material2 = DataLoader.LoadAsset<Material>(text3, false);
				if (!material2)
				{
					text3 = text5;
				}
			}
			if (!material2)
			{
				text3 += ".mat";
			}
			material2 = DataLoader.LoadAsset<Material>(text3, false);
			if (!material2)
			{
				bool useMask = part.useMask;
				return;
			}
			this.dismemberMat = UnityEngine.Object.Instantiate<Material>(material2);
			if (this.dismemberMat.HasColor("_EmissiveColor") && sharedMaterial.HasColor("_EmissiveColor"))
			{
				this.dismemberMat.SetColor("_EmissiveColor", sharedMaterial.GetColor("_EmissiveColor"));
			}
			this.skinnedMeshRenderer.material = this.dismemberMat;
			if (this.smrLODOne)
			{
				this.smrLODOne.material = this.dismemberMat;
			}
			if (this.smrLODTwo)
			{
				this.smrLODTwo.material = this.dismemberMat;
			}
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0001F6EC File Offset: 0x0001D8EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void setUpEyeMats(Transform t)
	{
		Transform transform = t.FindRecursive("NormalEye");
		Transform transform2 = t.FindRecursive("FeralEye");
		if (transform2 && !this.entity.HasAnyTags(DismembermentManager.radiatedTag))
		{
			if (transform)
			{
				transform.gameObject.SetActive(false);
			}
			transform2.gameObject.SetActive(true);
			return;
		}
		if (transform)
		{
			MeshRenderer component = transform.GetComponent<MeshRenderer>();
			if (component)
			{
				Material material = UnityEngine.Object.Instantiate<Material>(component.material);
				if (material.HasProperty("_IsRadiated"))
				{
					material.SetFloat("_IsRadiated", 1f);
				}
				if (material.HasProperty("_Irradiated"))
				{
					material.SetFloat("_Irradiated", 1f);
				}
				component.material = material;
			}
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0001F7B0 File Offset: 0x0001D9B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void MakeDismemberedPart(uint bodyDamageFlag, EnumDamageTypes damageType, Transform partT, bool restoreState)
	{
		DismemberedPartData dismemberedPartData = DismembermentManager.DismemberPart(bodyDamageFlag, damageType, this.entity, true, DismembermentManager.DebugUseLegacy);
		if (dismemberedPartData == null)
		{
			return;
		}
		DismemberedPart dismemberedPart = new DismemberedPart(dismemberedPartData, bodyDamageFlag, damageType);
		this.dismemberedParts.Add(dismemberedPart);
		if (partT)
		{
			Transform transform = null;
			if (!string.IsNullOrEmpty(dismemberedPartData.targetBone))
			{
				Transform transform2 = partT.FindRecursive(dismemberedPartData.targetBone);
				if (!transform2)
				{
					transform2 = partT.FindParent(dismemberedPartData.targetBone);
				}
				Transform transform3 = new GameObject("DynamicGore").transform;
				if (!dismemberedPartData.attachToParent)
				{
					transform3.SetParent(transform2);
				}
				else
				{
					transform3.SetParent(transform2.parent);
				}
				transform3.localPosition = Vector3.zero;
				transform3.localRotation = Quaternion.identity;
				transform3.localScale = Vector3.one;
				transform = transform3;
				this.defaultHeadPos = Vector3.zero;
				if (!dismemberedPartData.useMask)
				{
					if (!string.IsNullOrEmpty(dismemberedPartData.insertBoneObj))
					{
						Transform transform4 = transform2.FindRecursive(dismemberedPartData.insertBoneObj);
						this.defaultHeadPos = transform4.position;
					}
					transform2.localScale = dismemberedPartData.scale;
					this.scaleOutChildBones(transform2);
				}
				else
				{
					Collider component = transform2.GetComponent<Collider>();
					if (component)
					{
						component.enabled = false;
					}
					this.disableChildColliders(transform2);
				}
			}
			else
			{
				partT.localScale = dismemberedPartData.scale;
			}
			if (!string.IsNullOrEmpty(dismemberedPartData.prefabPath))
			{
				if (string.IsNullOrEmpty(this.rootDismmemberDir) && dismemberedPartData.prefabPath.Contains("/"))
				{
					this.subFolderDismemberEntityName = dismemberedPartData.prefabPath.Remove(dismemberedPartData.prefabPath.IndexOf("/"));
					this.rootDismmemberDir = "@:Entities/Zombies/" + this.subFolderDismemberEntityName + "/Dismemberment";
				}
				if (!transform)
				{
					string tag = dismemberedPartData.propertyKey.Replace("DismemberTag_", "");
					transform = GameUtils.FindTagInChilds(this.bipedT, tag);
				}
				Transform transform5 = this.SpawnLimbGore(transform, dismemberedPartData.prefabPath, restoreState);
				if (transform5 && !string.IsNullOrEmpty(dismemberedPartData.targetBone))
				{
					if (DismembermentManager.DebugLogEnabled)
					{
						foreach (MeshFilter meshFilter in transform5.GetComponentsInChildren<MeshFilter>())
						{
							Mesh sharedMesh = meshFilter.sharedMesh;
							if (!sharedMesh || (sharedMesh && sharedMesh.vertexCount == 0))
							{
								Log.Warning(string.Format("{0} prefabPath {1} partName {2} is missing a mesh.", base.GetType(), dismemberedPartData.prefabPath, meshFilter.transform.name));
							}
						}
					}
					if (!this.skinnedMeshRenderer)
					{
						this.skinnedMeshRenderer = this.entity.emodel.meshTransform.GetComponent<SkinnedMeshRenderer>();
						Transform parent = this.skinnedMeshRenderer.transform.parent;
						for (int j = 0; j < parent.childCount; j++)
						{
							Transform child = parent.GetChild(j);
							if (child.name.ContainsCaseInsensitive("LOD1"))
							{
								this.smrLODOne = child.GetComponent<SkinnedMeshRenderer>();
							}
							if (child.name.ContainsCaseInsensitive("LOD2"))
							{
								this.smrLODTwo = child.GetComponent<SkinnedMeshRenderer>();
							}
						}
					}
					Renderer[] componentsInChildren2 = transform5.GetComponentsInChildren<Renderer>();
					bool flag = false;
					foreach (Renderer renderer in componentsInChildren2)
					{
						if (!renderer.GetComponent<ParticleSystem>())
						{
							Material[] sharedMaterials = renderer.sharedMaterials;
							for (int l = 0; l < sharedMaterials.Length; l++)
							{
								Material material = sharedMaterials[l];
								string name = material.name;
								if ((!dismemberedPart.prefabPath.ContainsCaseInsensitive("head") || !name.ContainsCaseInsensitive("hair")) && material.shader.name == "Game/Character" && !DismembermentManager.IsDefaultGib(name))
								{
									sharedMaterials[l] = this.mainZombieMaterial;
									flag = true;
								}
							}
							if (flag)
							{
								renderer.sharedMaterials = sharedMaterials;
							}
						}
					}
					this.ProcDismemberedPart(transform5, partT, dismemberedPartData, bodyDamageFlag);
					dismemberedPart.prefabT = transform5;
					Transform transform6 = partT.FindRecursive(dismemberedPartData.targetBone);
					if (!transform6)
					{
						transform6 = partT.FindParent(dismemberedPartData.targetBone);
					}
					dismemberedPart.targetT = transform6;
					if (dismemberedPartData.useMask)
					{
						if (dismemberedPartData.scaleOutLimb)
						{
							Transform transform7 = partT.FindRecursive(dismemberedPartData.targetBone);
							if (!transform7)
							{
								transform7 = partT.FindParent(dismemberedPartData.targetBone);
							}
							if (!string.IsNullOrEmpty(dismemberedPartData.solTarget))
							{
								transform7 = partT.FindRecursive(dismemberedPartData.solTarget);
								if (!transform7)
								{
									transform7 = partT.FindParent(dismemberedPartData.solTarget);
								}
							}
							this.scaleOutChildBones(transform7);
							if (dismemberedPartData.hasSolScale)
							{
								transform7.localScale = dismemberedPartData.solScale;
							}
						}
						else
						{
							this.scaleOutChildBones(transform6);
						}
						EnumBodyPartHit bodyPartHit = DismembermentManager.GetBodyPartHit(dismemberedPart.bodyDamageFlag);
						Transform transform8 = this.modelT.FindTagInChildren("D_Accessory");
						if (transform8)
						{
							DismembermentAccessoryMan component2 = transform8.GetComponent<DismembermentAccessoryMan>();
							if (component2)
							{
								component2.HidePart(bodyPartHit);
							}
						}
						if (!dismemberedPartData.scaleOutLimb || !string.IsNullOrEmpty(dismemberedPartData.solTarget))
						{
							this.setLimbShaderProps(bodyPartHit, dismemberedPart);
						}
					}
					if (dismemberedPartData.isDetachable)
					{
						this.ActivateDetachableLimbs(bodyDamageFlag, damageType, transform5, dismemberedPart);
					}
				}
			}
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0001FCE0 File Offset: 0x0001DEE0
	[Conditional("DEBUG_DISMEMBERMENT")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void logDismemberment(string _log)
	{
		if (DismembermentManager.DebugLogEnabled)
		{
			Type type = base.GetType();
			Log.Out(((type != null) ? type.ToString() : null) + " " + _log);
		}
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001FD0C File Offset: 0x0001DF0C
	[PublicizedFrom(EAccessModifier.Private)]
	public void scaleOutChildBones(Transform _boneT)
	{
		if (_boneT.childCount > 0)
		{
			for (int i = 0; i < _boneT.childCount; i++)
			{
				Transform child = _boneT.GetChild(i);
				if (child && !child.name.Equals("DynamicGore"))
				{
					child.localScale = Vector3.zero;
				}
			}
		}
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0001FD60 File Offset: 0x0001DF60
	[PublicizedFrom(EAccessModifier.Private)]
	public void disableChildColliders(Transform _boneT)
	{
		foreach (Collider collider in _boneT.GetComponentsInChildren<Collider>())
		{
			if (collider)
			{
				collider.enabled = false;
			}
		}
		foreach (CharacterJoint characterJoint in _boneT.GetComponentsInChildren<CharacterJoint>())
		{
			if (characterJoint)
			{
				Rigidbody component = characterJoint.GetComponent<Rigidbody>();
				UnityEngine.Object.Destroy(characterJoint);
				UnityEngine.Object.Destroy(component);
			}
		}
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001FDD4 File Offset: 0x0001DFD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ActivateDetachableLimbs(uint bodyDamageFlag, EnumDamageTypes damageType, Transform partT, DismemberedPart part)
	{
		Transform entitiesTransform = GameManager.Instance.World.EntitiesTransform;
		if (!entitiesTransform)
		{
			return;
		}
		Transform transform = entitiesTransform.Find("DismemberedLimbs");
		if (!transform)
		{
			transform = new GameObject("DismemberedLimbs").transform;
			transform.SetParent(entitiesTransform);
			transform.localPosition = Vector3.zero;
		}
		Transform transform2 = partT.FindRecursive("Detachable");
		if (transform2)
		{
			EnumBodyPartHit bodyPartHit = DismembermentManager.GetBodyPartHit(bodyDamageFlag);
			Transform transform3 = new GameObject(string.Format("{0}_{1}_{2}", this.entity.entityId, this.entity.EntityName, bodyPartHit)).transform;
			transform3.SetParent(transform);
			part.SetDetachedTransform(transform3, transform2);
			if (this.entity.IsBloodMoon)
			{
				part.lifeTime /= 3f;
			}
			if (this.leftLowerArmDismembered && bodyDamageFlag == 2U)
			{
				DismembermentManager.ActivateDetachable(transform2, "HalfArm");
				this.hideDismemberedPart(bodyDamageFlag);
			}
			if (this.leftLowerLegDismembered && bodyDamageFlag == 32U)
			{
				DismembermentManager.ActivateDetachable(transform2, "HalfLeg");
				this.hideDismemberedPart(bodyDamageFlag);
			}
			if (this.rightLowerArmDismembered && bodyDamageFlag == 8U)
			{
				DismembermentManager.ActivateDetachable(transform2, "HalfArm");
				this.hideDismemberedPart(bodyDamageFlag);
			}
			if (this.rightLowerLegDismembered && bodyDamageFlag == 128U)
			{
				DismembermentManager.ActivateDetachable(transform2, "HalfLeg");
				this.hideDismemberedPart(bodyDamageFlag);
			}
			if (!transform2.gameObject.activeSelf)
			{
				transform2.gameObject.SetActive(true);
			}
			if (this.entity.OverrideHeadSize != 1f && this.headDismembered && bodyDamageFlag == 1U)
			{
				float headBigSize = this.entity.emodel.HeadBigSize;
				part.overrideHeadSize = headBigSize;
				part.overrideHeadDismemberScaleTime = this.entity.OverrideHeadDismemberScaleTime;
				Transform transform4 = transform2.Find("Physics");
				Transform transform5 = new GameObject("pivot").transform;
				transform5.SetParent(transform4);
				transform5.localScale = Vector3.one;
				int i = 0;
				while (i < part.targetT.childCount)
				{
					Transform child = part.targetT.GetChild(i);
					if (child.CompareTag("E_BP_Head"))
					{
						Transform transform6 = transform2.FindRecursive(bodyPartHit.ToString());
						if (transform6)
						{
							Renderer component = transform6.GetComponent<Renderer>();
							transform5.position = child.position + (component.bounds.center - child.position);
							part.pivotT = transform5;
							break;
						}
						transform5.position = child.position;
						part.pivotT = transform5;
						break;
					}
					else
					{
						i++;
					}
				}
				List<Transform> list = new List<Transform>();
				for (int j = 0; j < transform4.childCount; j++)
				{
					Transform child2 = transform4.GetChild(j);
					if (child2 != transform4)
					{
						list.Add(child2);
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					list[k].SetParent(transform5);
				}
				transform5.localScale = new Vector3(headBigSize, headBigSize, headBigSize);
			}
			transform2.SetParent(transform3);
			DismembermentManager instance = DismembermentManager.Instance;
			if (instance != null)
			{
				instance.AddPart(part);
			}
			string text = string.Empty;
			foreach (Renderer renderer in transform2.GetComponentsInChildren<Renderer>())
			{
				if (renderer != null)
				{
					Material[] sharedMaterials = renderer.sharedMaterials;
					for (int m = 0; m < sharedMaterials.Length; m++)
					{
						Material material = sharedMaterials[m];
						if (material != null)
						{
							text = material.name;
							if ((!part.prefabPath.ContainsCaseInsensitive("head") || !text.ContainsCaseInsensitive("hair")) && (!renderer.name.ContainsCaseInsensitive("eye") || material.HasProperty("_IsRadiated") || material.HasProperty("_Irradiated")))
							{
								if (text.ContainsCaseInsensitive("ZombieGibs_caps"))
								{
									sharedMaterials[m] = this.gibCapMaterial;
								}
								if (text.Contains("HD_"))
								{
									sharedMaterials[m] = this.mainZombieMaterial;
								}
								sharedMaterials[m].DisableKeyword("_ELECTRIC_SHOCK_ON");
							}
						}
					}
					renderer.sharedMaterials = sharedMaterials;
				}
			}
			Jiggle[] componentsInChildren2 = transform2.GetComponentsInChildren<Jiggle>(true);
			for (int n = 0; n < componentsInChildren2.Length; n++)
			{
				componentsInChildren2[n].enabled = true;
			}
			Rigidbody componentInChildren = transform2.GetComponentInChildren<Rigidbody>();
			if (componentInChildren)
			{
				Vector3 vector = Vector3.up * this.entity.lastHitForce;
				float num = Vector3.Angle(this.entity.GetForwardVector(), this.entity.lastHitImpactDir);
				componentInChildren.AddTorque(Quaternion.FromToRotation(this.entity.GetForwardVector(), this.entity.lastHitImpactDir).eulerAngles * (1f + num / 90f), ForceMode.Impulse);
				componentInChildren.AddForce((this.entity.lastHitImpactDir + vector) * this.entity.lastHitForce, ForceMode.Impulse);
				string damageTag = DismembermentManager.GetDamageTag(damageType, this.entity.lastHitRanged);
				if (damageTag == "blunt")
				{
					if (damageType == EnumDamageTypes.Piercing)
					{
						componentInChildren.AddForce(this.entity.lastHitImpactDir + vector, ForceMode.Impulse);
					}
					else
					{
						componentInChildren.AddForce(this.entity.lastHitImpactDir * this.entity.lastHitForce * 1.5f + vector * 1.25f, ForceMode.Impulse);
					}
				}
				if (damageTag == "blade")
				{
					float num2 = Vector3.Dot(this.entity.GetForwardVector(), this.entity.lastHitImpactDir);
					float num3 = Vector3.Dot(this.entity.GetForwardVector(), this.entity.lastHitEntityFwd);
					componentInChildren.AddForce((num2 < num3) ? (-this.entity.transform.right * this.entity.lastHitForce + vector) : (this.entity.transform.right * this.entity.lastHitForce + vector), ForceMode.Impulse);
					componentInChildren.AddTorque(Quaternion.FromToRotation(this.entity.GetForwardVector(), this.entity.lastHitImpactDir).eulerAngles * (1f + num / 90f) * this.entity.lastHitForce, ForceMode.Impulse);
				}
				if (damageType == EnumDamageTypes.Heat)
				{
					float d = 2.67f;
					componentInChildren.AddForce(this.entity.lastHitImpactDir * d + Vector3.up * d * 0.67f, ForceMode.Impulse);
				}
			}
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x000204C0 File Offset: 0x0001E6C0
	[PublicizedFrom(EAccessModifier.Private)]
	public Material GetMainZombieBodyMaterial()
	{
		EModelBase emodel = this.entity.emodel;
		if (emodel)
		{
			Transform meshTransform = emodel.meshTransform;
			if (meshTransform)
			{
				return meshTransform.GetComponent<Renderer>().sharedMaterial;
			}
		}
		return null;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00020500 File Offset: 0x0001E700
	public override void Electrocute(bool enabled)
	{
		base.Electrocute(enabled);
		if (enabled)
		{
			Material mainZombieBodyMaterial = this.GetMainZombieBodyMaterial();
			if (mainZombieBodyMaterial)
			{
				mainZombieBodyMaterial.EnableKeyword("_ELECTRIC_SHOCK_ON");
			}
			if (this.dismemberMat)
			{
				this.dismemberMat.EnableKeyword("_ELECTRIC_SHOCK_ON");
			}
			if (this.mainZombieMaterialCopy)
			{
				this.mainZombieMaterialCopy.EnableKeyword("_ELECTRIC_SHOCK_ON");
			}
			if (this.gibCapMaterialCopy)
			{
				this.gibCapMaterialCopy.EnableKeyword("_ELECTRIC_SHOCK_ON");
			}
			this.StartAnimationElectrocute(0.6f);
			return;
		}
		Material mainZombieBodyMaterial2 = this.GetMainZombieBodyMaterial();
		if (mainZombieBodyMaterial2)
		{
			mainZombieBodyMaterial2.DisableKeyword("_ELECTRIC_SHOCK_ON");
		}
		if (this.dismemberMat)
		{
			this.dismemberMat.DisableKeyword("_ELECTRIC_SHOCK_ON");
		}
		if (this.mainZombieMaterialCopy)
		{
			this.mainZombieMaterialCopy.DisableKeyword("_ELECTRIC_SHOCK_ON");
		}
		if (this.gibCapMaterialCopy)
		{
			this.gibCapMaterialCopy.DisableKeyword("_ELECTRIC_SHOCK_ON");
		}
		this.StartAnimationElectrocute(0f);
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00020610 File Offset: 0x0001E810
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLimbShaderProps(EnumBodyPartHit partHit, DismemberedPart part)
	{
		DismemberedPartData data = part.Data;
		if (this.dismemberMat)
		{
			bool scaleOutLimb = data.scaleOutLimb;
			bool isLinked = data.isLinked;
			if (this.dismemberMat.HasProperty("_LeftLowerLeg") && (partHit & EnumBodyPartHit.LeftLowerLeg) > EnumBodyPartHit.None)
			{
				this.dismemberMat.SetFloat("_LeftLowerLeg", 1f);
				if (isLinked)
				{
					this.dismemberMat.SetFloat("_LeftUpperLeg", 1f);
				}
			}
			if (this.dismemberMat.HasProperty("_LeftUpperLeg") && (partHit & EnumBodyPartHit.LeftUpperLeg) > EnumBodyPartHit.None)
			{
				if (!isLinked)
				{
					this.dismemberMat.SetFloat("_LeftUpperLeg", 1f);
				}
				if (!scaleOutLimb)
				{
					this.dismemberMat.SetFloat("_LeftLowerLeg", 1f);
				}
			}
			if (this.dismemberMat.HasProperty("_RightLowerLeg") && (partHit & EnumBodyPartHit.RightLowerLeg) > EnumBodyPartHit.None)
			{
				this.dismemberMat.SetFloat("_RightLowerLeg", 1f);
				if (isLinked)
				{
					this.dismemberMat.SetFloat("_RightUpperLeg", 1f);
				}
			}
			if (this.dismemberMat.HasProperty("_RightUpperLeg") && (partHit & EnumBodyPartHit.RightUpperLeg) > EnumBodyPartHit.None)
			{
				if (!isLinked)
				{
					this.dismemberMat.SetFloat("_RightUpperLeg", 1f);
				}
				if (!scaleOutLimb)
				{
					this.dismemberMat.SetFloat("_RightLowerLeg", 1f);
				}
			}
			if (this.dismemberMat.HasProperty("_LeftLowerArm") && (partHit & EnumBodyPartHit.LeftLowerArm) > EnumBodyPartHit.None && !scaleOutLimb)
			{
				this.dismemberMat.SetFloat("_LeftLowerArm", 1f);
			}
			if (this.dismemberMat.HasProperty("_LeftUpperArm") && (partHit & EnumBodyPartHit.LeftUpperArm) > EnumBodyPartHit.None)
			{
				if (!isLinked)
				{
					this.dismemberMat.SetFloat("_LeftUpperArm", 1f);
				}
				if (!scaleOutLimb)
				{
					this.dismemberMat.SetFloat("_LeftLowerArm", 1f);
				}
			}
			if (this.dismemberMat.HasProperty("_RightLowerArm") && (partHit & EnumBodyPartHit.RightLowerArm) > EnumBodyPartHit.None && !scaleOutLimb)
			{
				this.dismemberMat.SetFloat("_RightLowerArm", 1f);
			}
			if (this.dismemberMat.HasProperty("_RightUpperArm") && (partHit & EnumBodyPartHit.RightUpperArm) > EnumBodyPartHit.None)
			{
				if (!isLinked)
				{
					this.dismemberMat.SetFloat("_RightUpperArm", 1f);
				}
				if (!scaleOutLimb)
				{
					this.dismemberMat.SetFloat("_RightLowerArm", 1f);
				}
			}
		}
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x0002085C File Offset: 0x0001EA5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void hideDismemberedPart(uint bodyDamageFlag)
	{
		uint lowerBodyPart = 0U;
		if (bodyDamageFlag == 2U)
		{
			lowerBodyPart = 4U;
		}
		if (bodyDamageFlag == 8U)
		{
			lowerBodyPart = 16U;
		}
		if (bodyDamageFlag == 32U)
		{
			lowerBodyPart = 64U;
		}
		if (bodyDamageFlag == 128U)
		{
			lowerBodyPart = 256U;
		}
		if (lowerBodyPart != 0U)
		{
			DismemberedPart dismemberedPart = this.dismemberedParts.Find((DismemberedPart p) => p.bodyDamageFlag == lowerBodyPart);
			if (dismemberedPart != null)
			{
				dismemberedPart.Hide();
			}
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000483 RID: 1155 RVA: 0x000208D6 File Offset: 0x0001EAD6
	public bool IsCrawler
	{
		get
		{
			return this.isCrawler;
		}
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x000208E0 File Offset: 0x0001EAE0
	public override void TurnIntoCrawler(bool restoreState)
	{
		if (!this.isCrawler && this.entity.GetWalkType() != 21)
		{
			this.isCrawler = true;
			this.crawlerTime = Time.time;
			this.isSuppressPain = true;
			this._setInt(AvatarController.hitBodyPartHash, 0, true);
			this.SetWalkType(21, false);
			this._setTrigger(AvatarController.toCrawlerTriggerHash, true);
		}
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x0002093F File Offset: 0x0001EB3F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _propertyHash, int _value, bool _netsync = true)
	{
		if (this.IsCrawler && _propertyHash == AvatarController.walkTypeHash && _value == 5)
		{
			return;
		}
		base._setInt(_propertyHash, _value, _netsync);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00020960 File Offset: 0x0001EB60
	public override void TriggerSleeperPose(int pose, bool returningToSleep = false)
	{
		if (returningToSleep)
		{
			base.TriggerSleeperPose(pose, returningToSleep);
			return;
		}
		if (this.anim)
		{
			this._setInt(AvatarController.sleeperPoseHash, pose, true);
			switch (pose)
			{
			case -2:
				this.anim.CrossFadeInFixedTime("Crouch Walk 8", 0.25f);
				return;
			case -1:
				this._setTrigger(AvatarController.sleeperTriggerHash, true);
				break;
			case 0:
				this.anim.Play(AvatarController.sleeperIdleSitHash);
				return;
			case 1:
				this.anim.Play(AvatarController.sleeperIdleSideRightHash);
				return;
			case 2:
				this.anim.Play(AvatarController.sleeperIdleSideLeftHash);
				return;
			case 3:
				this.anim.Play(AvatarController.sleeperIdleBackHash);
				return;
			case 4:
				this.anim.Play(AvatarController.sleeperIdleStomachHash);
				return;
			case 5:
				this.anim.Play(AvatarController.sleeperIdleStandHash);
				return;
			default:
				return;
			}
		}
	}

	// Token: 0x0400049D RID: 1181
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cOverrideLayerIndex = 1;

	// Token: 0x0400049E RID: 1182
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cFullBodyLayerIndex = 2;

	// Token: 0x0400049F RID: 1183
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int cHitLayerIndex = 3;

	// Token: 0x040004A0 RID: 1184
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AvatarRootMotion rootMotion;

	// Token: 0x040004A1 RID: 1185
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform modelT;

	// Token: 0x040004A2 RID: 1186
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform bipedT;

	// Token: 0x040004A3 RID: 1187
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform rightHandT;

	// Token: 0x040004A4 RID: 1188
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo baseStateInfo;

	// Token: 0x040004A5 RID: 1189
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo overrideStateInfo;

	// Token: 0x040004A6 RID: 1190
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo fullBodyStateInfo;

	// Token: 0x040004A7 RID: 1191
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AnimatorStateInfo hitStateInfo;

	// Token: 0x040004A8 RID: 1192
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isSuppressPain;

	// Token: 0x040004A9 RID: 1193
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isCrippled;

	// Token: 0x040004AA RID: 1194
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isCrawler;

	// Token: 0x040004AB RID: 1195
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isVisibleInit;

	// Token: 0x040004AC RID: 1196
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isVisible;

	// Token: 0x040004AD RID: 1197
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float idleTime;

	// Token: 0x040004AE RID: 1198
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float crawlerTime;

	// Token: 0x040004AF RID: 1199
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float actionTimeActive;

	// Token: 0x040004B0 RID: 1200
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float attackPlayingTime;

	// Token: 0x040004B1 RID: 1201
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isAttackImpact;

	// Token: 0x040004B2 RID: 1202
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeSpecialAttack2Playing;

	// Token: 0x040004B3 RID: 1203
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float timeRagePlaying;

	// Token: 0x040004B4 RID: 1204
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int jumpState;

	// Token: 0x040004B5 RID: 1205
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isJumpStarted;

	// Token: 0x040004B6 RID: 1206
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isEating;

	// Token: 0x040004B7 RID: 1207
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int movementStateOverride = -1;

	// Token: 0x040004B8 RID: 1208
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool headDismembered;

	// Token: 0x040004B9 RID: 1209
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool leftUpperArmDismembered;

	// Token: 0x040004BA RID: 1210
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool leftLowerArmDismembered;

	// Token: 0x040004BB RID: 1211
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool rightUpperArmDismembered;

	// Token: 0x040004BC RID: 1212
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool rightLowerArmDismembered;

	// Token: 0x040004BD RID: 1213
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool leftUpperLegDismembered;

	// Token: 0x040004BE RID: 1214
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool leftLowerLegDismembered;

	// Token: 0x040004BF RID: 1215
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool rightUpperLegDismembered;

	// Token: 0x040004C0 RID: 1216
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool rightLowerLegDismembered;

	// Token: 0x040004C1 RID: 1217
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool isInDeathAnim;

	// Token: 0x040004C2 RID: 1218
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool didDeathTransition;

	// Token: 0x040004C3 RID: 1219
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material mainZombieMaterial;

	// Token: 0x040004C4 RID: 1220
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material mainZombieMaterialCopy;

	// Token: 0x040004C5 RID: 1221
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material gibCapMaterial;

	// Token: 0x040004C6 RID: 1222
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material gibCapMaterialCopy;

	// Token: 0x040004C7 RID: 1223
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material dismemberMat;

	// Token: 0x040004C8 RID: 1224
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x040004C9 RID: 1225
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SkinnedMeshRenderer smrLODOne;

	// Token: 0x040004CA RID: 1226
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SkinnedMeshRenderer smrLODTwo;

	// Token: 0x040004CB RID: 1227
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string rootDismmemberDir;

	// Token: 0x040004CC RID: 1228
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string subFolderDismemberEntityName;

	// Token: 0x040004CD RID: 1229
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int altEntityMatId = -1;

	// Token: 0x040004CE RID: 1230
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string altMatName;

	// Token: 0x040004CF RID: 1231
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<DismemberedPart> dismemberedParts = new List<DismemberedPart>();

	// Token: 0x040004D0 RID: 1232
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isCensored;

	// Token: 0x040004D1 RID: 1233
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cEmissiveColor = "_EmissiveColor";

	// Token: 0x040004D2 RID: 1234
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 defaultHeadPos;

	// Token: 0x040004D3 RID: 1235
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string cElectrocuteKeyword = "_ELECTRIC_SHOCK_ON";
}
