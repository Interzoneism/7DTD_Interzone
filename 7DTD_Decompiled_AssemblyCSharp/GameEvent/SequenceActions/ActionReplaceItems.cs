using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001694 RID: 5780
	[Preserve]
	public class ActionReplaceItems : ActionBaseItemAction
	{
		// Token: 0x0600B011 RID: 45073 RVA: 0x0044C85E File Offset: 0x0044AA5E
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnClientActionStarted(EntityPlayer player)
		{
			this.replaceItemTag = FastTags<TagGroup.Global>.Parse(this.itemTags);
		}

		// Token: 0x0600B012 RID: 45074 RVA: 0x0044C874 File Offset: 0x0044AA74
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool CheckEquipmentReplace(Equipment equipment, int slot)
		{
			ItemValue item = ItemClass.GetItem(this.ReplacedByItem, false);
			return equipment.PreferredItemSlot(item) == slot;
		}

		// Token: 0x0600B013 RID: 45075 RVA: 0x0044C898 File Offset: 0x0044AA98
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleItemStackChange(ref ItemStack stack, EntityPlayer player)
		{
			if (stack.IsEmpty() || !stack.itemValue.ItemClass.HasAnyTags(this.replaceItemTag) || !(stack.itemValue.ItemClass.GetItemName() != this.ReplacedByItem))
			{
				return false;
			}
			if (this.count != -1)
			{
				if (this.countType == ActionBaseItemAction.CountTypes.Items)
				{
					if (stack.count <= this.count)
					{
						this.count -= stack.count;
						stack = new ItemStack(ItemClass.GetItem(this.ReplacedByItem, false), stack.count);
					}
					else
					{
						stack.count -= this.count;
						ItemStack stack2 = new ItemStack(ItemClass.GetItem(this.ReplacedByItem, false), this.count);
						base.AddStack(player as EntityPlayerLocal, stack2);
						this.count = 0;
						this.isFinished = true;
					}
				}
				else
				{
					stack = new ItemStack(ItemClass.GetItem(this.ReplacedByItem, false), stack.count);
					this.count--;
					if (this.count == 0)
					{
						this.isFinished = true;
					}
				}
				return true;
			}
			stack = new ItemStack(ItemClass.GetItem(this.ReplacedByItem, false), stack.count);
			return true;
		}

		// Token: 0x0600B014 RID: 45076 RVA: 0x0044C9E8 File Offset: 0x0044ABE8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleItemValueChange(ref ItemValue itemValue, EntityPlayer player)
		{
			if (!itemValue.IsEmpty() && itemValue.ItemClass.HasAnyTags(this.replaceItemTag) && itemValue.ItemClass.GetItemName() != this.ReplacedByItem)
			{
				itemValue = ItemClass.GetItem(this.ReplacedByItem, false).Clone();
				if (this.count != -1)
				{
					this.count--;
					if (this.count == 0)
					{
						this.isFinished = true;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600B015 RID: 45077 RVA: 0x0044CA67 File Offset: 0x0044AC67
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionReplaceItems.PropReplacedByItem))
			{
				this.ReplacedByItem = properties.Values[ActionReplaceItems.PropReplacedByItem];
			}
		}

		// Token: 0x0600B016 RID: 45078 RVA: 0x0044CA98 File Offset: 0x0044AC98
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionReplaceItems
			{
				ReplacedByItem = this.ReplacedByItem
			};
		}

		// Token: 0x04008985 RID: 35205
		public string ReplacedByItem = "";

		// Token: 0x04008986 RID: 35206
		public static string PropReplacedByItem = "replaced_by_item";

		// Token: 0x04008987 RID: 35207
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> replaceItemTag = FastTags<TagGroup.Global>.none;
	}
}
