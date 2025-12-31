using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class UMACharacterBodyAnimator : BodyAnimator
{
	// Token: 0x0600049D RID: 1181 RVA: 0x00020DF4 File Offset: 0x0001EFF4
	public UMACharacterBodyAnimator(EntityAlive _entity, AvatarCharacterController.AnimationStates _animStates, Transform _bodyTransform, BodyAnimator.EnumState _defaultState)
	{
		Transform rightHand = _bodyTransform.FindInChilds((_entity.emodel is EModelSDCS) ? "RightWeapon" : "Gunjoint", false);
		BodyAnimator.BodyParts bodyParts = new BodyAnimator.BodyParts(_bodyTransform, rightHand);
		base.initBodyAnimator(_entity, bodyParts, _defaultState);
		this.deathStates = _animStates.DeathStates;
		this.hitStates = _animStates.HitStates;
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x00020E54 File Offset: 0x0001F054
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void assignLayerWeights()
	{
		base.assignLayerWeights();
		Animator animator = base.Animator;
		if (animator)
		{
			if (this.Entity.IsDead())
			{
				animator.SetLayerWeight(1, 0f);
				animator.SetLayerWeight(2, 0f);
				animator.SetLayerWeight(3, 0f);
				return;
			}
			animator.SetLayerWeight(3, 1f);
			if (animator.GetInteger(AvatarController.vehiclePoseHash) >= 0)
			{
				animator.SetLayerWeight(1, 0f);
				animator.SetLayerWeight(2, 0f);
				return;
			}
			if (!animator.IsInTransition(1) && AnimationDelayData.AnimationDelay[this.Entity.inventory.holdingItem.HoldType.Value].TwoHanded)
			{
				animator.SetLayerWeight(1, 0f);
				animator.SetLayerWeight(2, 1f);
				return;
			}
			if (!animator.IsInTransition(2))
			{
				animator.SetLayerWeight(1, 1f);
				animator.SetLayerWeight(2, 0f);
			}
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x00020F4C File Offset: 0x0001F14C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void cacheLayerStateInfo()
	{
		base.cacheLayerStateInfo();
		Animator animator = base.Animator;
		if (animator)
		{
			this.currentOverrideLayer = animator.GetCurrentAnimatorStateInfo(1);
			this.twoHandedLayer = animator.GetCurrentAnimatorStateInfo(2);
			this.painLayer = animator.GetCurrentAnimatorStateInfo(4);
		}
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00020F95 File Offset: 0x0001F195
	[PublicizedFrom(EAccessModifier.Protected)]
	public override AnimatorStateInfo getCachedLayerStateInfo(int _layer)
	{
		if (_layer == 1)
		{
			return this.currentOverrideLayer;
		}
		if (_layer == 2)
		{
			return this.twoHandedLayer;
		}
		return base.getCachedLayerStateInfo(_layer);
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x00020FB4 File Offset: 0x0001F1B4
	public override void SetDrunk(float _numBeers)
	{
		if (base.Animator && this.Entity.AttachedToEntity == null && this.avatarController != null)
		{
			this.avatarController.UpdateFloat("drunk", _numBeers, true);
		}
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x00021004 File Offset: 0x0001F204
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateSpineRotation()
	{
		if (!base.RagdollActive && !this.Entity.IsDead() && base.Animator && this.Entity.AttachedToEntity == null && this.avatarController != null)
		{
			float a;
			this.avatarController.TryGetFloat(AvatarController.yLookHash, out a);
			this.avatarController.UpdateFloat(AvatarController.yLookHash, Mathf.Lerp(a, -this.Entity.rotation.x / 90f, Time.deltaTime * 12f), false);
		}
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x000210A3 File Offset: 0x0001F2A3
	public override void StartDeathAnimation(EnumBodyPartHit _bodyPart, int _movementState, float random)
	{
		base.StartDeathAnimation(_bodyPart, _movementState, random);
		this.isInDeathAnim = true;
		this.didDeathTransition = false;
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x000210BC File Offset: 0x0001F2BC
	public override void Update()
	{
		base.Update();
		this.updateSpineRotation();
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x000210CC File Offset: 0x0001F2CC
	public override void LateUpdate()
	{
		base.LateUpdate();
		Animator animator = base.Animator;
		if (animator)
		{
			AnimatorStateInfo cachedLayerStateInfo = this.getCachedLayerStateInfo(0);
			if (this.isInDeathAnim)
			{
				if (this.deathStates.Contains(cachedLayerStateInfo.fullPathHash) && !animator.IsInTransition(0))
				{
					this.didDeathTransition = true;
				}
				if (this.didDeathTransition && (cachedLayerStateInfo.normalizedTime >= 1f || animator.IsInTransition(0)))
				{
					this.isInDeathAnim = false;
					if (this.Entity.HasDeathAnim)
					{
						this.Entity.emodel.DoRagdoll(DamageResponse.New(true), 999999f);
					}
				}
			}
			if (animator.GetInteger(AvatarController.hitBodyPartHash) != 0 && this.avatarController != null && !animator.IsInTransition(4) && this.hitStates.Contains(this.painLayer.fullPathHash))
			{
				this.avatarController.UpdateInt(AvatarController.hitBodyPartHash, 0, true);
			}
		}
	}

	// Token: 0x040004E5 RID: 1253
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimatorStateInfo currentOverrideLayer;

	// Token: 0x040004E6 RID: 1254
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimatorStateInfo twoHandedLayer;

	// Token: 0x040004E7 RID: 1255
	[PublicizedFrom(EAccessModifier.Private)]
	public AnimatorStateInfo painLayer;

	// Token: 0x040004E8 RID: 1256
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<int> deathStates;

	// Token: 0x040004E9 RID: 1257
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<int> hitStates;

	// Token: 0x040004EA RID: 1258
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInDeathAnim;

	// Token: 0x040004EB RID: 1259
	[PublicizedFrom(EAccessModifier.Private)]
	public bool didDeathTransition;

	// Token: 0x040004EC RID: 1260
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cYLookUpdateSpeed = 12f;
}
