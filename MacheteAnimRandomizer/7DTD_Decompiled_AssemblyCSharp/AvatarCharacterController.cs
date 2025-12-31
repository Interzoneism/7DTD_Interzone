using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public abstract class AvatarCharacterController : AvatarMultiBodyController
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x0600032E RID: 814 RVA: 0x00018881 File Offset: 0x00016A81
	public BodyAnimator CharacterBody
	{
		get
		{
			return this.characterBody;
		}
	}

	// Token: 0x0600032F RID: 815 RVA: 0x0001888C File Offset: 0x00016A8C
	public override void SwitchModelAndView(string _modelName, bool _bFPV, bool _bMale)
	{
		if (this.characterBody != null && this.modelName != _modelName)
		{
			if (this.characterBody.Parts.BodyObj != null)
			{
				this.characterBody.Parts.BodyObj.SetActive(false);
			}
			base.removeBodyAnimator(this.characterBody);
			this.characterBody = null;
		}
		if (this.characterBody == null)
		{
			this.modelName = _modelName;
			Transform transform = EModelBase.FindModel(base.transform);
			if (transform != null)
			{
				Transform transform2 = transform.Find(_modelName);
				if (transform2)
				{
					transform2.gameObject.SetActive(true);
					this.characterBody = base.addBodyAnimator(this.createCharacterBody(transform2));
				}
			}
		}
		if (this.characterBody != null)
		{
			this.initBodyAnimator(this.characterBody, _bFPV, _bMale);
		}
		base.SwitchModelAndView(_modelName, _bFPV, _bMale);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00018964 File Offset: 0x00016B64
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void initBodyAnimator(BodyAnimator _body, bool _bFPV, bool _bMale)
	{
		base._setBool("IsMale", _bMale, true);
		base._setFloat("IsMaleFloat", _bMale ? 1f : 0f, true);
		this.SetWalkType(this.entity.GetWalkType(), false);
		this._setBool(AvatarController.isDeadHash, this.entity.IsDead(), true);
		this._setBool(AvatarController.isFPVHash, _bFPV, true);
		this._setBool(AvatarController.isAliveHash, this.entity.IsAlive(), true);
		if (_body == this.characterBody)
		{
			this.characterBody.State = (_bFPV ? BodyAnimator.EnumState.OnlyColliders : BodyAnimator.EnumState.Visible);
		}
	}

	// Token: 0x06000331 RID: 817
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract BodyAnimator createCharacterBody(Transform _bodyTransform);

	// Token: 0x06000332 RID: 818 RVA: 0x00018A01 File Offset: 0x00016C01
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getJumpStates()
	{
		return new HashSet<int>
		{
			Animator.StringToHash("Base Layer.Jump")
		};
	}

	// Token: 0x06000333 RID: 819 RVA: 0x00018A19 File Offset: 0x00016C19
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getDeathStates()
	{
		HashSet<int> hashSet = new HashSet<int>();
		AvatarCharacterController.GetFirstPersonDeathStates(hashSet);
		AvatarCharacterController.GetThirdPersonDeathStates(hashSet);
		return hashSet;
	}

	// Token: 0x06000334 RID: 820 RVA: 0x00018A2C File Offset: 0x00016C2C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getReloadStates()
	{
		HashSet<int> hashSet = new HashSet<int>();
		AvatarCharacterController.GetFirstPersonReloadStates(hashSet);
		AvatarCharacterController.GetThirdPersonReloadStates(hashSet);
		return hashSet;
	}

	// Token: 0x06000335 RID: 821 RVA: 0x00018A3F File Offset: 0x00016C3F
	public override Animator GetAnimator()
	{
		return this.characterBody.Animator;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x00018A4C File Offset: 0x00016C4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual HashSet<int> getHitStates()
	{
		HashSet<int> hashSet = new HashSet<int>();
		AvatarCharacterController.GetThirdPersonHitStates(hashSet);
		return hashSet;
	}

	// Token: 0x06000337 RID: 823 RVA: 0x00018A5C File Offset: 0x00016C5C
	public static void GetFirstPersonReloadStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("Base Layer.fpvBlunderbussReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvSawedOffShotgunReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvPistolReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvMP5Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvSniperRifleReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvM136Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvCrossbowReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvHuntingRifleReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvAugerReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvChainsawReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvSawedOffShotgunReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvMagnumReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvBowReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvNailGunReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvAK47Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvBowReload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvAK47Reload"));
		hashSet.Add(Animator.StringToHash("Base Layer.fpvCompoundBowReload"));
	}

	// Token: 0x06000338 RID: 824 RVA: 0x00018B9C File Offset: 0x00016D9C
	public static void GetThirdPersonReloadStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSawedOffShotgunReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleMP5Reload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSniperRifleReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleM136Reload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleCrossbowReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleHuntingRifleReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleAugerReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleChainsawReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSawedOffShotgunReloadIntro"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleSawedOffShotgunReloadExit"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.Female44MagnumReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemaleNailGunReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleBowReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleBlunderbussReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleBowReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.FemalePistolReload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleAk47Reload"));
		hashSet.Add(Animator.StringToHash("TwoHandedOverlays.femaleCompoundBowReload"));
	}

	// Token: 0x06000339 RID: 825 RVA: 0x00002914 File Offset: 0x00000B14
	public static void GetFirstPersonHitStates(HashSet<int> hashSet)
	{
	}

	// Token: 0x0600033A RID: 826 RVA: 0x00018CDC File Offset: 0x00016EDC
	public static void GetThirdPersonHitStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchHeadLeft"));
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchHeadRight"));
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchChestLeft"));
		hashSet.Add(Animator.StringToHash("PainOverlays.femaleTwitchChestRight"));
	}

	// Token: 0x0600033B RID: 827 RVA: 0x00002914 File Offset: 0x00000B14
	public static void GetFirstPersonDeathStates(HashSet<int> hashSet)
	{
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00018D30 File Offset: 0x00016F30
	public static void GetThirdPersonDeathStates(HashSet<int> hashSet)
	{
		hashSet.Add(Animator.StringToHash("Base Layer.generic"));
		hashSet.Add(Animator.StringToHash("Base Layer.FemaleDeath01"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingHead"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingChest"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingLeftArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingRightArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingLeftLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.idlingRightLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadFront"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeft"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeftA"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeftB"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadLeftC"));
		hashSet.Add(Animator.StringToHash("Base Layer.meleeHeadRight"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningChestA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningChestB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningHeadA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningHeadB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningLeftArmA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningLeftArmB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningRightArmA"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningRightArmB"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningLeftLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.runningRightLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingChestA"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingChestB"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingHeadA"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingHeadB"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingLeftArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingRightArm"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingLeftLeg"));
		hashSet.Add(Animator.StringToHash("Base Layer.walkingRightLeg"));
	}

	// Token: 0x0600033D RID: 829 RVA: 0x00018F60 File Offset: 0x00017160
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setTrigger(int _pid, bool _netsync = true)
	{
		base._setTrigger(_pid, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (AvatarMultiBodyController.animatorIsValid(animator))
			{
				animator.SetTrigger(_pid);
			}
		}
	}

	// Token: 0x0600033E RID: 830 RVA: 0x00018F98 File Offset: 0x00017198
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _resetTrigger(int _pid, bool _netsync = true)
	{
		base._resetTrigger(_pid, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.ResetTrigger(_pid);
			}
		}
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00018FD0 File Offset: 0x000171D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setFloat(int _pid, float _value, bool _netsync = true)
	{
		base._setFloat(_pid, _value, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.SetFloat(_pid, _value);
			}
		}
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0001900C File Offset: 0x0001720C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setBool(int _pid, bool _value, bool _netsync = true)
	{
		base._setBool(_pid, _value, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.SetBool(_pid, _value);
			}
		}
	}

	// Token: 0x06000341 RID: 833 RVA: 0x00019048 File Offset: 0x00017248
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void _setInt(int _pid, int _value, bool _netsync = true)
	{
		base._setInt(_pid, _value, _netsync);
		if (this.characterBody != null)
		{
			Animator animator = this.characterBody.Animator;
			if (animator)
			{
				animator.SetInteger(_pid, _value);
			}
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x00019082 File Offset: 0x00017282
	[PublicizedFrom(EAccessModifier.Protected)]
	public AvatarCharacterController()
	{
	}

	// Token: 0x040003CE RID: 974
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BodyAnimator characterBody;

	// Token: 0x040003CF RID: 975
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string modelName;

	// Token: 0x020000A8 RID: 168
	public class AnimationStates
	{
		// Token: 0x06000343 RID: 835 RVA: 0x0001908A File Offset: 0x0001728A
		public AnimationStates(HashSet<int> _jumpStates, HashSet<int> _deathStates, HashSet<int> _reloadStates, HashSet<int> _hitStates)
		{
			this.JumpStates = _jumpStates;
			this.DeathStates = _deathStates;
			this.ReloadStates = _reloadStates;
			this.HitStates = _hitStates;
		}

		// Token: 0x040003D0 RID: 976
		public readonly HashSet<int> JumpStates;

		// Token: 0x040003D1 RID: 977
		public readonly HashSet<int> DeathStates;

		// Token: 0x040003D2 RID: 978
		public readonly HashSet<int> ReloadStates;

		// Token: 0x040003D3 RID: 979
		public readonly HashSet<int> HitStates;
	}
}
