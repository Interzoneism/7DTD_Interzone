using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001651 RID: 5713
	[Preserve]
	public class ActionBaseItemAction : ActionBaseClientAction
	{
		// Token: 0x0600AEEC RID: 44780 RVA: 0x00444820 File Offset: 0x00442A20
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				this.OnClientActionStarted(entityPlayer);
				this.count = GameEventManager.GetIntValue(entityPlayer, this.countText, -1);
				bool flag = false;
				FastTags<TagGroup.Global>.Parse(this.itemTags);
				if (this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.Toolbelt) && !this.isFinished)
				{
					ItemStack[] array = (entityPlayer.AttachedToEntity != null && entityPlayer.saveInventory != null) ? entityPlayer.saveInventory.GetSlots() : entityPlayer.inventory.GetSlots();
					for (int i = 0; i < array.Length; i++)
					{
						if (this.HandleItemStackChange(ref array[i], entityPlayer))
						{
							flag = true;
						}
						if (this.isFinished)
						{
							break;
						}
					}
					if (flag)
					{
						entityPlayer.inventory.SetSlots(array, true);
						entityPlayer.bPlayerStatsChanged = true;
					}
				}
				flag = false;
				if (this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.Equipment) || (this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.BiomeBadge) && !this.isFinished))
				{
					int slotCount = entityPlayer.equipment.GetSlotCount();
					int num = 4;
					for (int j = 0; j < slotCount; j++)
					{
						if ((j < num || this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.BiomeBadge)) && (j >= num || this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.Equipment)))
						{
							if (this.CheckEquipmentReplace(entityPlayer.equipment, j))
							{
								ItemValue slotItemOrNone = entityPlayer.equipment.GetSlotItemOrNone(j);
								if (this.HandleItemValueChange(ref slotItemOrNone, entityPlayer))
								{
									entityPlayer.equipment.SetSlotItem(j, slotItemOrNone, true);
									flag = true;
								}
							}
							if (this.isFinished)
							{
								break;
							}
						}
					}
					if (flag)
					{
						entityPlayer.bPlayerStatsChanged = true;
					}
				}
				flag = false;
				if (this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.Backpack) && !this.isFinished)
				{
					ItemStack[] slots = entityPlayer.bag.GetSlots();
					for (int k = 0; k < slots.Length; k++)
					{
						if (this.HandleItemStackChange(ref slots[k], entityPlayer))
						{
							flag = true;
						}
						if (this.isFinished)
						{
							break;
						}
					}
					if (flag)
					{
						entityPlayer.bag.SetSlots(slots);
						entityPlayer.bPlayerStatsChanged = true;
					}
				}
				if (this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.Backpack) && !this.isFinished)
				{
					XUiC_DragAndDropWindow dragAndDrop = LocalPlayerUI.GetUIForPrimaryPlayer().xui.dragAndDrop;
					if (!dragAndDrop.CurrentStack.IsEmpty())
					{
						ItemStack currentStack = dragAndDrop.CurrentStack;
						if (this.HandleItemStackChange(ref currentStack, entityPlayer))
						{
							entityPlayer.bPlayerStatsChanged = true;
						}
					}
				}
				if (!this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.Toolbelt) && this.itemLocations.Contains(ActionBaseItemAction.ItemLocations.Held) && !this.isFinished)
				{
					Inventory inventory = (entityPlayer.saveInventory != null) ? entityPlayer.saveInventory : entityPlayer.inventory;
					if (inventory.holdingItem != entityPlayer.inventory.GetBareHandItem())
					{
						ItemStack holdingItemStack = inventory.holdingItemStack;
						if (this.HandleItemStackChange(ref holdingItemStack, entityPlayer))
						{
							inventory.SetItem(inventory.holdingItemIdx, holdingItemStack);
							entityPlayer.bPlayerStatsChanged = true;
						}
					}
				}
				this.OnClientActionEnded(entityPlayer);
			}
		}

		// Token: 0x0600AEED RID: 44781 RVA: 0x000197A5 File Offset: 0x000179A5
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool CheckEquipmentReplace(Equipment equipment, int slot)
		{
			return true;
		}

		// Token: 0x0600AEEE RID: 44782 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnClientActionStarted(EntityPlayer player)
		{
		}

		// Token: 0x0600AEEF RID: 44783 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnClientActionEnded(EntityPlayer player)
		{
		}

		// Token: 0x0600AEF0 RID: 44784 RVA: 0x0000FB42 File Offset: 0x0000DD42
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool HandleItemStackChange(ref ItemStack stack, EntityPlayer player)
		{
			return false;
		}

		// Token: 0x0600AEF1 RID: 44785 RVA: 0x0000FB42 File Offset: 0x0000DD42
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool HandleItemValueChange(ref ItemValue itemValue, EntityPlayer player)
		{
			return false;
		}

		// Token: 0x0600AEF2 RID: 44786 RVA: 0x00444AE6 File Offset: 0x00442CE6
		[PublicizedFrom(EAccessModifier.Protected)]
		public void AddStack(EntityPlayerLocal player, ItemStack stack)
		{
			if (!LocalPlayerUI.GetUIForPlayer(player).xui.PlayerInventory.AddItem(stack))
			{
				GameManager.Instance.ItemDropServer(stack, player.GetPosition(), Vector3.zero, -1, 60f, false);
			}
		}

		// Token: 0x0600AEF3 RID: 44787 RVA: 0x00444B20 File Offset: 0x00442D20
		public override BaseAction Clone()
		{
			ActionBaseItemAction actionBaseItemAction = (ActionBaseItemAction)base.Clone();
			actionBaseItemAction.countText = this.countText;
			actionBaseItemAction.countType = this.countType;
			actionBaseItemAction.itemTags = this.itemTags;
			actionBaseItemAction.fastItemTags = this.fastItemTags;
			actionBaseItemAction.itemLocations = new List<ActionBaseItemAction.ItemLocations>(this.itemLocations);
			return actionBaseItemAction;
		}

		// Token: 0x0600AEF4 RID: 44788 RVA: 0x00444B7C File Offset: 0x00442D7C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			ActionBaseItemAction.ItemLocations item = ActionBaseItemAction.ItemLocations.Toolbelt;
			if (properties.Values.ContainsKey(ActionBaseItemAction.PropItemLocation))
			{
				string[] array = properties.Values[ActionBaseItemAction.PropItemLocation].Split(',', StringSplitOptions.None);
				this.itemLocations.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					if (Enum.TryParse<ActionBaseItemAction.ItemLocations>(array[i], true, out item))
					{
						this.itemLocations.Add(item);
					}
				}
			}
			properties.ParseString(ActionBaseItemAction.PropItemTag, ref this.itemTags);
			this.fastItemTags = FastTags<TagGroup.Global>.Parse(this.itemTags);
			properties.ParseString(ActionBaseItemAction.PropFullCount, ref this.countText);
			properties.ParseEnum<ActionBaseItemAction.CountTypes>(ActionBaseItemAction.PropCountType, ref this.countType);
		}

		// Token: 0x040087E5 RID: 34789
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<ActionBaseItemAction.ItemLocations> itemLocations = new List<ActionBaseItemAction.ItemLocations>();

		// Token: 0x040087E6 RID: 34790
		[PublicizedFrom(EAccessModifier.Protected)]
		public string itemTags = "";

		// Token: 0x040087E7 RID: 34791
		[PublicizedFrom(EAccessModifier.Protected)]
		public FastTags<TagGroup.Global> fastItemTags = FastTags<TagGroup.Global>.none;

		// Token: 0x040087E8 RID: 34792
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionBaseItemAction.CountTypes countType;

		// Token: 0x040087E9 RID: 34793
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isFinished;

		// Token: 0x040087EA RID: 34794
		[PublicizedFrom(EAccessModifier.Protected)]
		public int count = -1;

		// Token: 0x040087EB RID: 34795
		[PublicizedFrom(EAccessModifier.Protected)]
		public string countText = "";

		// Token: 0x040087EC RID: 34796
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropItemLocation = "items_location";

		// Token: 0x040087ED RID: 34797
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropItemTag = "items_tags";

		// Token: 0x040087EE RID: 34798
		public static string PropFullCount = "count";

		// Token: 0x040087EF RID: 34799
		public static string PropCountType = "count_type";

		// Token: 0x02001652 RID: 5714
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum ItemLocations
		{
			// Token: 0x040087F1 RID: 34801
			Toolbelt,
			// Token: 0x040087F2 RID: 34802
			Backpack,
			// Token: 0x040087F3 RID: 34803
			Equipment,
			// Token: 0x040087F4 RID: 34804
			BiomeBadge,
			// Token: 0x040087F5 RID: 34805
			Held
		}

		// Token: 0x02001653 RID: 5715
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum CountTypes
		{
			// Token: 0x040087F7 RID: 34807
			Items,
			// Token: 0x040087F8 RID: 34808
			Slots
		}
	}
}
