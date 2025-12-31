using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016BD RID: 5821
	[Preserve]
	public class ActionUnloadItems : ActionBaseItemAction
	{
		// Token: 0x0600B0C6 RID: 45254 RVA: 0x0044FFDC File Offset: 0x0044E1DC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleItemStackChange(ref ItemStack stack, EntityPlayer player)
		{
			if (!stack.IsEmpty() && (this.itemTags == "" || stack.itemValue.ItemClass.HasAnyTags(this.fastItemTags)))
			{
				ItemClass itemClass = stack.itemValue.ItemClass;
				if (itemClass != null)
				{
					ItemActionAttack itemActionAttack = itemClass.Actions[0] as ItemActionAttack;
					if (itemActionAttack != null && !itemActionAttack.IsEditingTool())
					{
						int meta = stack.itemValue.Meta;
						string itemName = itemActionAttack.MagazineItemNames[(int)stack.itemValue.SelectedAmmoTypeIndex];
						stack.itemValue.Meta = 0;
						this.ItemStacks.Add(new ItemStack(ItemClass.GetItem(itemName, false), meta));
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600B0C7 RID: 45255 RVA: 0x00450094 File Offset: 0x0044E294
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnClientActionEnded(EntityPlayer player)
		{
			base.OnClientActionEnded(player);
			for (int i = this.ItemStacks.Count - 1; i >= 0; i--)
			{
				ItemStack itemStack = this.ItemStacks[i];
				if (LocalPlayerUI.GetUIForPlayer(player as EntityPlayerLocal).xui.PlayerInventory.AddItem(itemStack))
				{
					this.ItemStacks.RemoveAt(i);
				}
			}
			if (this.ItemStacks.Count > 0)
			{
				string text = "DroppedLootContainerTwitch";
				EntityLootContainer entityLootContainer = EntityFactory.CreateEntity(player.entityId, text.GetHashCode(), player.position, Vector3.zero) as EntityLootContainer;
				if (entityLootContainer != null)
				{
					entityLootContainer.SetContent(ItemStack.Clone(this.ItemStacks));
				}
				GameManager.Instance.World.SpawnEntityInWorld(entityLootContainer);
			}
		}

		// Token: 0x0600B0C8 RID: 45256 RVA: 0x00450156 File Offset: 0x0044E356
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionUnloadItems();
		}

		// Token: 0x04008A52 RID: 35410
		[PublicizedFrom(EAccessModifier.Private)]
		public List<ItemStack> ItemStacks = new List<ItemStack>();
	}
}
