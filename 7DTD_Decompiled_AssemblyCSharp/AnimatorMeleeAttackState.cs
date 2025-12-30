using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000087 RID: 135
public class AnimatorMeleeAttackState : StateMachineBehaviour
{
	// Token: 0x06000273 RID: 627 RVA: 0x00013F38 File Offset: 0x00012138
	public AnimatorMeleeAttackState()
	{
		AnimatorMeleeAttackState.FistHoldHash = Animator.StringToHash("fistHold");
		AnimatorMeleeAttackState.FpvFistHoldHash = Animator.StringToHash("fpvFistHold");
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00013F8C File Offset: 0x0001218C
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.playingImpact)
		{
			return;
		}
		this.hasFired = false;
		this.actionIndex = animator.GetInteger(AvatarController.itemActionIndexHash);
		AnimationEventBridge component = animator.GetComponent<AnimationEventBridge>();
		this.entity = component.entity;
		if (this.actionIndex < 0 || this.actionIndex >= this.entity.inventory.holdingItemData.actionData.Count || this.entity.inventory.holdingItemData.actionData[this.actionIndex] == null)
		{
			return;
		}
		AnimatorClipInfo[] array = animator.GetNextAnimatorClipInfo(layerIndex);
		if (array.Length == 0)
		{
			array = animator.GetCurrentAnimatorClipInfo(layerIndex);
			if (array.Length == 0)
			{
				return;
			}
		}
		AnimationClip clip = array[0].clip;
		float length = clip.length;
		this.attacksPerMinute = (float)((int)(60f / length));
		FastTags<TagGroup.Global> fastTags = (this.actionIndex == 0) ? ItemActionAttack.PrimaryTag : ItemActionAttack.SecondaryTag;
		ItemValue holdingItemItemValue = this.entity.inventory.holdingItemItemValue;
		ItemClass itemClass = holdingItemItemValue.ItemClass;
		if (itemClass != null)
		{
			fastTags |= itemClass.ItemTags;
		}
		this.originalMeleeAttackSpeed = EffectManager.GetValue(PassiveEffects.AttacksPerMinute, holdingItemItemValue, this.attacksPerMinute, this.entity, null, fastTags, true, true, true, true, true, 1, true, false) / 60f * length;
		animator.SetFloat("MeleeAttackSpeed", this.originalMeleeAttackSpeed);
		ItemClass holdingItem = this.entity.inventory.holdingItem;
		holdingItem.Properties.ParseFloat((this.actionIndex == 0) ? "Action0.RaycastTime" : "Action1.RaycastTime", ref this.RaycastTime);
		float num = -1f;
		holdingItem.Properties.ParseFloat((this.actionIndex == 0) ? "Action0.ImpactDuration" : "Action1.ImpactDuration", ref num);
		if (num >= 0f)
		{
			this.ImpactDuration = num * this.originalMeleeAttackSpeed;
		}
		holdingItem.Properties.ParseFloat((this.actionIndex == 0) ? "Action0.ImpactPlaybackSpeed" : "Action1.ImpactPlaybackSpeed", ref this.ImpactPlaybackSpeed);
		if (this.originalMeleeAttackSpeed != 0f)
		{
			this.calculatedRaycastTime = this.RaycastTime / this.originalMeleeAttackSpeed;
			this.calculatedImpactDuration = this.ImpactDuration / this.originalMeleeAttackSpeed;
			this.calculatedImpactPlaybackSpeed = this.ImpactPlaybackSpeed / this.originalMeleeAttackSpeed;
		}
		else
		{
			this.calculatedRaycastTime = 0.001f;
			this.calculatedImpactDuration = 0.001f;
			this.calculatedImpactPlaybackSpeed = 0.001f;
		}
		ItemActionDynamicMelee.ItemActionDynamicMeleeData itemActionDynamicMeleeData = this.entity.inventory.holdingItemData.actionData[this.actionIndex] as ItemActionDynamicMelee.ItemActionDynamicMeleeData;
		if (itemActionDynamicMeleeData != null)
		{
			itemActionDynamicMeleeData.HasFinished = false;
		}
		GameManager.Instance.StartCoroutine(this.impactStart(animator, animator.GetNextAnimatorStateInfo(layerIndex), clip, layerIndex));
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0001422B File Offset: 0x0001242B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator impactStart(Animator animator, AnimatorStateInfo stateInfo, AnimationClip clip, int layerIndex)
	{
		yield return new WaitForSeconds(this.calculatedRaycastTime);
		if (!this.hasFired)
		{
			this.hasFired = true;
			if (this.entity != null && !this.entity.isEntityRemote && this.actionIndex >= 0)
			{
				ItemActionDynamicMelee.ItemActionDynamicMeleeData itemActionDynamicMeleeData = this.entity.inventory.holdingItemData.actionData[this.actionIndex] as ItemActionDynamicMelee.ItemActionDynamicMeleeData;
				if (itemActionDynamicMeleeData != null && (this.entity.inventory.holdingItem.Actions[this.actionIndex] as ItemActionDynamicMelee).Raycast(itemActionDynamicMeleeData))
				{
					animator.SetFloat("MeleeAttackSpeed", this.calculatedImpactPlaybackSpeed);
					this.playingImpact = true;
					GameManager.Instance.StartCoroutine(this.impactStop(animator, stateInfo, clip, layerIndex));
				}
			}
		}
		yield break;
	}

	// Token: 0x06000276 RID: 630 RVA: 0x00014257 File Offset: 0x00012457
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator impactStop(Animator animator, AnimatorStateInfo stateInfo, AnimationClip clip, int layerIndex)
	{
		animator.Play(0, layerIndex, Mathf.Min(1f, this.calculatedRaycastTime * this.originalMeleeAttackSpeed / clip.length));
		yield return new WaitForSeconds(this.calculatedImpactDuration);
		animator.SetFloat("MeleeAttackSpeed", this.originalMeleeAttackSpeed);
		this.playingImpact = false;
		yield break;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0001427C File Offset: 0x0001247C
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.entity != null && !this.entity.isEntityRemote && this.actionIndex >= 0)
		{
			ItemActionDynamicMelee.ItemActionDynamicMeleeData itemActionDynamicMeleeData = this.entity.inventory.holdingItemData.actionData[this.actionIndex] as ItemActionDynamicMelee.ItemActionDynamicMeleeData;
			if (itemActionDynamicMeleeData != null)
			{
				itemActionDynamicMeleeData.HasFinished = true;
				animator.SetFloat("MeleeAttackSpeed", this.originalMeleeAttackSpeed);
			}
		}
	}

	// Token: 0x06000278 RID: 632 RVA: 0x000142F0 File Offset: 0x000124F0
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		float normalizedTime = stateInfo.normalizedTime;
		if (float.IsInfinity(normalizedTime) || float.IsNaN(normalizedTime))
		{
			if (animator.HasState(layerIndex, AnimatorMeleeAttackState.FistHoldHash))
			{
				animator.Play(AnimatorMeleeAttackState.FistHoldHash, layerIndex, 1f);
				return;
			}
			if (animator.HasState(layerIndex, AnimatorMeleeAttackState.FpvFistHoldHash))
			{
				animator.Play(AnimatorMeleeAttackState.FpvFistHoldHash, layerIndex, 1f);
				return;
			}
			animator.Play(animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash, layerIndex);
		}
	}

	// Token: 0x04000313 RID: 787
	public float RaycastTime = 0.3f;

	// Token: 0x04000314 RID: 788
	public float ImpactDuration = 0.01f;

	// Token: 0x04000315 RID: 789
	public float ImpactPlaybackSpeed = 1f;

	// Token: 0x04000316 RID: 790
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float calculatedRaycastTime;

	// Token: 0x04000317 RID: 791
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float calculatedImpactDuration;

	// Token: 0x04000318 RID: 792
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float calculatedImpactPlaybackSpeed;

	// Token: 0x04000319 RID: 793
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasFired;

	// Token: 0x0400031A RID: 794
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int actionIndex;

	// Token: 0x0400031B RID: 795
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float originalMeleeAttackSpeed;

	// Token: 0x0400031C RID: 796
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool playingImpact;

	// Token: 0x0400031D RID: 797
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive entity;

	// Token: 0x0400031E RID: 798
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float attacksPerMinute;

	// Token: 0x0400031F RID: 799
	public static int FistHoldHash = Animator.StringToHash("fistHold");

	// Token: 0x04000320 RID: 800
	public static int FpvFistHoldHash = Animator.StringToHash("fpvFistHold");
}
