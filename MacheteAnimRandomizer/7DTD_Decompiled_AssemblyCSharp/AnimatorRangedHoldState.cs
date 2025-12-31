using System;
using Audio;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class AnimatorRangedHoldState : StateMachineBehaviour
{
	// Token: 0x0600028A RID: 650 RVA: 0x000145B8 File Offset: 0x000127B8
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		EntityAlive componentInParent = animator.GetComponentInParent<EntityAlive>();
		if (componentInParent == null)
		{
			return;
		}
		componentInParent.emodel.avatarController.UpdateInt("CurrentAnim", 0, true);
		this.actionData = (componentInParent.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged);
		if (this.actionData != null)
		{
			this.itemClass = this.actionData.invData.itemValue.ItemClass;
			if (this.itemClass != null && this.itemClass.Properties.Values.ContainsKey(ItemClass.PropSoundIdle))
			{
				if (this.actionData.invData.itemValue.Meta > 0)
				{
					Manager.Play(this.actionData.invData.holdingEntity, this.itemClass.Properties.Values[ItemClass.PropSoundIdle], 1f, false);
					this.actionData.invData.holdingEntitySoundID = 0;
					return;
				}
				Manager.Stop(this.actionData.invData.holdingEntity.entityId, this.itemClass.Properties.Values[ItemClass.PropSoundIdle]);
				this.actionData.invData.holdingEntitySoundID = -1;
			}
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00014704 File Offset: 0x00012904
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.itemClass != null && this.itemClass.Properties.Values.ContainsKey(ItemClass.PropSoundIdle))
		{
			if (this.actionData.invData.holdingEntitySoundID != -1 && this.actionData.invData.itemValue.Meta == 0)
			{
				Manager.Stop(this.actionData.invData.holdingEntity.entityId, this.itemClass.Properties.Values[ItemClass.PropSoundIdle]);
				this.actionData.invData.holdingEntitySoundID = -1;
				return;
			}
			if (this.actionData.invData.holdingEntitySoundID == -1 && this.actionData.invData.itemValue.Meta > 0)
			{
				Manager.Play(this.actionData.invData.holdingEntity, this.itemClass.Properties.Values[ItemClass.PropSoundIdle], 1f, false);
				this.actionData.invData.holdingEntitySoundID = 0;
			}
		}
	}

	// Token: 0x0400032E RID: 814
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemActionRanged.ItemActionDataRanged actionData;

	// Token: 0x0400032F RID: 815
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemClass itemClass;
}
