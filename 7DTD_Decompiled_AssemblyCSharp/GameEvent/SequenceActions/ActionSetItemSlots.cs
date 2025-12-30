using System;
using System.Globalization;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A3 RID: 5795
	[Preserve]
	public class ActionSetItemSlots : ActionBaseClientAction
	{
		// Token: 0x0600B05A RID: 45146 RVA: 0x0044E29C File Offset: 0x0044C49C
		public override void OnClientPerform(Entity target)
		{
			if (this.Items == null || (this.ItemLocation != ActionSetItemSlots.ItemLocations.Equipment && this.SlotNumbers == null))
			{
				return;
			}
			XUiM_PlayerEquipment xuiM_PlayerEquipment = null;
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				for (int i = 0; i < this.Items.Length; i++)
				{
					string value = (this.ItemCounts != null && this.ItemCounts.Length > i) ? this.ItemCounts[i] : "1";
					int num = (this.SlotNumbers != null && this.SlotNumbers.Length > i) ? StringParsers.ParseSInt32(this.SlotNumbers[i], 0, -1, NumberStyles.Integer) : -1;
					if (num == -1 && this.ItemLocation != ActionSetItemSlots.ItemLocations.Equipment)
					{
						return;
					}
					ItemClass itemClass = ItemClass.GetItemClass(this.Items[i], false);
					int num2 = GameEventManager.GetIntValue(entityPlayerLocal, value, 1);
					ItemValue itemValue;
					if (itemClass.HasQuality)
					{
						itemValue = new ItemValue(itemClass.Id, num2, num2, false, null, 1f);
						num2 = 1;
					}
					else
					{
						itemValue = new ItemValue(itemClass.Id, false);
					}
					ItemActionRanged itemActionRanged = itemValue.ItemClass.Actions[0] as ItemActionRanged;
					if (itemActionRanged != null)
					{
						itemValue.Meta = (int)EffectManager.GetValue(PassiveEffects.MagazineSize, itemValue, (float)itemActionRanged.BulletsPerMagazine, entityPlayerLocal, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					}
					ItemStack itemStack = new ItemStack(itemValue, num2);
					switch (this.ItemLocation)
					{
					case ActionSetItemSlots.ItemLocations.Toolbelt:
						entityPlayerLocal.inventory.SetItem(num, itemStack);
						break;
					case ActionSetItemSlots.ItemLocations.Backpack:
						entityPlayerLocal.bag.SetSlot(num, itemStack, true);
						break;
					case ActionSetItemSlots.ItemLocations.Equipment:
						if (xuiM_PlayerEquipment == null)
						{
							xuiM_PlayerEquipment = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal).xui.PlayerEquipment;
						}
						xuiM_PlayerEquipment.EquipItem(itemStack);
						break;
					}
				}
			}
		}

		// Token: 0x0600B05B RID: 45147 RVA: 0x0044E44C File Offset: 0x0044C64C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<ActionSetItemSlots.ItemLocations>(ActionSetItemSlots.PropItemLocation, ref this.ItemLocation);
			if (properties.Values.ContainsKey(ActionSetItemSlots.PropItems))
			{
				this.Items = properties.Values[ActionSetItemSlots.PropItems].Replace(" ", "").Split(',', StringSplitOptions.None);
				if (properties.Values.ContainsKey(ActionSetItemSlots.PropItemCounts))
				{
					this.ItemCounts = properties.Values[ActionSetItemSlots.PropItemCounts].Replace(" ", "").Split(',', StringSplitOptions.None);
				}
				else
				{
					this.ItemCounts = null;
				}
				if (properties.Values.ContainsKey(ActionSetItemSlots.PropSlotNumbers))
				{
					this.SlotNumbers = properties.Values[ActionSetItemSlots.PropSlotNumbers].Replace(" ", "").Split(',', StringSplitOptions.None);
					return;
				}
				this.SlotNumbers = null;
				if (this.ItemLocation != ActionSetItemSlots.ItemLocations.Equipment)
				{
					this.Items = null;
					this.ItemCounts = null;
					return;
				}
			}
			else
			{
				this.Items = null;
				this.SlotNumbers = null;
				this.ItemCounts = null;
			}
		}

		// Token: 0x0600B05C RID: 45148 RVA: 0x0044E570 File Offset: 0x0044C770
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSetItemSlots
			{
				ItemLocation = this.ItemLocation,
				Items = this.Items,
				ItemCounts = this.ItemCounts,
				SlotNumbers = this.SlotNumbers,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x040089DD RID: 35293
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionSetItemSlots.ItemLocations ItemLocation;

		// Token: 0x040089DE RID: 35294
		public string[] Items;

		// Token: 0x040089DF RID: 35295
		public string[] ItemCounts;

		// Token: 0x040089E0 RID: 35296
		public string[] SlotNumbers;

		// Token: 0x040089E1 RID: 35297
		public static string PropItemLocation = "items_location";

		// Token: 0x040089E2 RID: 35298
		public static string PropItems = "items";

		// Token: 0x040089E3 RID: 35299
		public static string PropItemCounts = "item_counts";

		// Token: 0x040089E4 RID: 35300
		public static string PropSlotNumbers = "slot_numbers";

		// Token: 0x020016A4 RID: 5796
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum ItemLocations
		{
			// Token: 0x040089E6 RID: 35302
			Toolbelt,
			// Token: 0x040089E7 RID: 35303
			Backpack,
			// Token: 0x040089E8 RID: 35304
			Equipment
		}
	}
}
