using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001663 RID: 5731
	[Preserve]
	public class ActionDropHeldItem : ActionBaseItemAction
	{
		// Token: 0x0600AF40 RID: 44864 RVA: 0x00447320 File Offset: 0x00445520
		public override bool CanPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			return entityPlayer != null && ((entityPlayer.saveInventory != null) ? entityPlayer.saveInventory : entityPlayer.inventory).holdingItemStack != ItemStack.Empty;
		}

		// Token: 0x0600AF41 RID: 44865 RVA: 0x00447360 File Offset: 0x00445560
		public override void OnClientPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				Inventory inventory = (entityAlive.saveInventory != null) ? entityAlive.saveInventory : entityAlive.inventory;
				if (inventory.holdingItem != entityAlive.inventory.GetBareHandItem())
				{
					Vector3 dropPosition = entityAlive.GetDropPosition();
					ItemValue holdingItemItemValue = inventory.holdingItemItemValue;
					int count = inventory.holdingItemStack.count;
					GameManager.Instance.DropContentInLootContainerServer(entityAlive.entityId, "DroppedLootContainerTwitch", dropPosition, new ItemStack[]
					{
						inventory.holdingItemStack.Clone()
					}, false);
					entityAlive.AddUIHarvestingItem(new ItemStack(holdingItemItemValue, -count), false);
					Manager.BroadcastPlay(entityAlive, this.DropSound, false);
					inventory.DecHoldingItem(count);
				}
			}
		}

		// Token: 0x0600AF42 RID: 44866 RVA: 0x00447417 File Offset: 0x00445617
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionDropHeldItem.PropDropSound, ref this.DropSound);
		}

		// Token: 0x0600AF43 RID: 44867 RVA: 0x00447431 File Offset: 0x00445631
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionDropHeldItem
			{
				targetGroup = this.targetGroup,
				DropSound = this.DropSound
			};
		}

		// Token: 0x0400886D RID: 34925
		public string DropSound = "";

		// Token: 0x0400886E RID: 34926
		public static string PropDropSound = "drop_sound";
	}
}
