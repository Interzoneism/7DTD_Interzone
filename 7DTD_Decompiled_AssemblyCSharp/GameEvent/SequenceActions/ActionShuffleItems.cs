using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016AC RID: 5804
	[Preserve]
	public class ActionShuffleItems : ActionBaseClientAction
	{
		// Token: 0x0600B082 RID: 45186 RVA: 0x0044EB78 File Offset: 0x0044CD78
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				GameManager.Instance.StartCoroutine(this.handleShuffle(entityPlayer));
			}
		}

		// Token: 0x0600B083 RID: 45187 RVA: 0x0044EBA1 File Offset: 0x0044CDA1
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator handleShuffle(EntityPlayer player)
		{
			while (player.inventory.IsHoldingItemActionRunning() || !player.inventory.GetItem(player.inventory.DUMMY_SLOT_IDX).IsEmpty())
			{
				yield return new WaitForSeconds(0.25f);
			}
			List<ItemStack> list = new List<ItemStack>();
			if (this.itemLocations.Contains(ActionShuffleItems.ItemLocations.Toolbelt))
			{
				ItemStack[] collection = (player.AttachedToEntity != null && player.saveInventory != null) ? player.saveInventory.GetSlots() : player.inventory.GetSlots();
				list.AddRange(collection);
				list.RemoveAt(list.Count - 1);
			}
			if (this.itemLocations.Contains(ActionShuffleItems.ItemLocations.Backpack))
			{
				ItemStack[] slots = player.bag.GetSlots();
				list.AddRange(slots);
			}
			GameRandom random = GameEventManager.Current.Random;
			for (int i = 0; i < list.Count * 2; i++)
			{
				int index = random.RandomRange(list.Count);
				int index2 = random.RandomRange(list.Count);
				ItemStack value = list[index];
				list[index] = list[index2];
				list[index2] = value;
			}
			int num = 0;
			if (this.itemLocations.Contains(ActionShuffleItems.ItemLocations.Toolbelt))
			{
				Inventory inventory = (player.saveInventory != null) ? player.saveInventory : player.inventory;
				ItemStack[] slots2 = inventory.GetSlots();
				for (int j = 0; j < slots2.Length - 1; j++)
				{
					slots2[j] = list[num++];
				}
				inventory.SetSlots(slots2, true);
				player.bPlayerStatsChanged = true;
			}
			if (this.itemLocations.Contains(ActionShuffleItems.ItemLocations.Backpack))
			{
				ItemStack[] slots3 = player.bag.GetSlots();
				for (int k = 0; k < slots3.Length; k++)
				{
					slots3[k] = list[num++];
				}
				player.bag.SetSlots(slots3);
				player.bPlayerStatsChanged = true;
			}
			yield break;
		}

		// Token: 0x0600B084 RID: 45188 RVA: 0x0044EBB8 File Offset: 0x0044CDB8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			ActionShuffleItems.ItemLocations item = ActionShuffleItems.ItemLocations.Toolbelt;
			if (properties.Values.ContainsKey(ActionShuffleItems.PropItemLocation))
			{
				string[] array = properties.Values[ActionShuffleItems.PropItemLocation].Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					if (Enum.TryParse<ActionShuffleItems.ItemLocations>(array[i], true, out item))
					{
						this.itemLocations.Add(item);
					}
				}
			}
		}

		// Token: 0x0600B085 RID: 45189 RVA: 0x0044EC20 File Offset: 0x0044CE20
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionShuffleItems
			{
				itemLocations = this.itemLocations,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x04008A05 RID: 35333
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<ActionShuffleItems.ItemLocations> itemLocations = new List<ActionShuffleItems.ItemLocations>();

		// Token: 0x04008A06 RID: 35334
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropItemLocation = "items_location";

		// Token: 0x020016AD RID: 5805
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum ItemLocations
		{
			// Token: 0x04008A08 RID: 35336
			Toolbelt,
			// Token: 0x04008A09 RID: 35337
			Backpack
		}
	}
}
