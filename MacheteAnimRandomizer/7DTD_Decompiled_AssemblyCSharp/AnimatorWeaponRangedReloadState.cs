using System;
using Audio;
using UnityEngine;

// Token: 0x02000097 RID: 151
public class AnimatorWeaponRangedReloadState : StateMachineBehaviour
{
	// Token: 0x060002AF RID: 687 RVA: 0x00015184 File Offset: 0x00013384
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool("Reload", false);
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		this.actionData = ((componentInParent != null) ? (componentInParent.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged) : null);
		this.actionData.isWeaponReloading = true;
		this.actionData.wasWeaponReloadCancelled = false;
		if (this.actionData.invData.item.Properties.Values[ItemClass.PropSoundIdle] != null)
		{
			Manager.Stop(this.actionData.invData.holdingEntity.entityId, this.actionData.invData.item.Properties.Values[ItemClass.PropSoundIdle]);
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00015250 File Offset: 0x00013450
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.actionData != null && this.actionData.isWeaponReloadCancelled && !this.actionData.wasWeaponReloadCancelled)
		{
			this.actionData.wasWeaponReloadCancelled = true;
			animator.Play(0, -1, 1f);
			animator.Update(0f);
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x000152A3 File Offset: 0x000134A3
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		this.actionData.isWeaponReloading = false;
		this.actionData.isWeaponReloadCancelled = false;
		animator.speed = 1f;
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x000152C8 File Offset: 0x000134C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (this.actionData != null)
		{
			this.actionData.isWeaponReloading = false;
			this.actionData.isWeaponReloadCancelled = false;
		}
	}

	// Token: 0x04000333 RID: 819
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemActionRanged.ItemActionDataRanged actionData;
}
