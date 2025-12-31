using System;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200091A RID: 2330
[Preserve]
public class RewardItem : BaseReward
{
	// Token: 0x0600457E RID: 17790 RVA: 0x001BC6F8 File Offset: 0x001BA8F8
	public override void SetupReward()
	{
		ItemClass itemClass = ItemClass.GetItemClass(base.ID, false);
		base.Description = itemClass.GetLocalizedItemName();
		base.ValueText = base.Value;
		string text = itemClass.Groups[0].ToLower();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1893056498U)
		{
			if (num <= 332883202U)
			{
				if (num != 46554813U)
				{
					if (num != 131445777U)
					{
						if (num == 332883202U)
						{
							if (text == "food/cooking")
							{
								base.Icon = "ui_game_symbol_fork";
							}
						}
					}
					else if (text == "books")
					{
						base.Icon = "ui_game_symbol_book";
					}
				}
				else if (text == "science")
				{
					base.Icon = "ui_game_symbol_science";
				}
			}
			else if (num != 481379405U)
			{
				if (num != 954139509U)
				{
					if (num == 1893056498U)
					{
						if (text == "resources")
						{
							base.Icon = "ui_game_symbol_resource";
						}
					}
				}
				else if (text == "building")
				{
					base.Icon = "ui_game_symbol_map_house";
				}
			}
			else if (text == "clothing")
			{
				base.Icon = "ui_game_symbol_shirt";
			}
		}
		else if (num <= 2154995914U)
		{
			if (num != 1917176822U)
			{
				if (num != 2115735777U)
				{
					if (num == 2154995914U)
					{
						if (text == "basics")
						{
							base.Icon = "ui_game_symbol_shopping_cart";
						}
					}
				}
				else if (text == "decor/miscellaneous")
				{
					base.Icon = "ui_game_symbol_chair";
				}
			}
			else if (text == "chemicals")
			{
				base.Icon = "ui_game_symbol_water";
			}
		}
		else if (num <= 3292735525U)
		{
			if (num != 2816984135U)
			{
				if (num == 3292735525U)
				{
					if (text == "ammo/weapons")
					{
						base.Icon = "ui_game_symbol_knife";
					}
				}
			}
			else if (text == "tools/traps")
			{
				base.Icon = "ui_game_symbol_tool";
			}
		}
		else if (num != 4134465488U)
		{
			if (num == 4185622628U)
			{
				if (text == "special items")
				{
					base.Icon = "ui_game_symbol_book";
				}
			}
		}
		else if (text == "mods")
		{
			base.Icon = "ui_game_symbol_assemble";
		}
		base.IconAtlas = "ItemIconAtlas";
	}

	// Token: 0x17000747 RID: 1863
	// (get) Token: 0x0600457F RID: 17791 RVA: 0x001BC9CD File Offset: 0x001BABCD
	public ItemStack Item
	{
		get
		{
			if (this.item == null || this.item.IsEmpty())
			{
				this.SetupItem();
			}
			return this.item;
		}
	}

	// Token: 0x06004580 RID: 17792 RVA: 0x001BC9F0 File Offset: 0x001BABF0
	public override void Read(BinaryReader _br)
	{
		base.Read(_br);
		this.item = new ItemStack();
		this.item.Read(_br);
	}

	// Token: 0x06004581 RID: 17793 RVA: 0x001BCA11 File Offset: 0x001BAC11
	public override void Write(BinaryWriter _bw)
	{
		base.Write(_bw);
		if (this.item == null)
		{
			this.item = ItemStack.Empty.Clone();
		}
		this.item.Write(_bw);
	}

	// Token: 0x06004582 RID: 17794 RVA: 0x001BCA40 File Offset: 0x001BAC40
	public override void GiveReward(EntityPlayer player)
	{
		this.item = this.GetRewardItem();
		XUiM_PlayerInventory playerInventory = LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui.PlayerInventory;
		if (!playerInventory.AddItem(this.Item))
		{
			playerInventory.DropItem(this.Item);
		}
	}

	// Token: 0x06004583 RID: 17795 RVA: 0x001BCA94 File Offset: 0x001BAC94
	public override ItemStack GetRewardItem()
	{
		ItemStack itemStack = this.Item.Clone();
		if (!itemStack.itemValue.ItemClass.HasAllTags(FastTags<TagGroup.Global>.Parse("dukes")))
		{
			return itemStack;
		}
		int count = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.QuestBonusItemReward, null, (float)itemStack.count, base.OwnerQuest.OwnerJournal.OwnerPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		itemStack.count = count;
		return itemStack;
	}

	// Token: 0x06004584 RID: 17796 RVA: 0x001BCB10 File Offset: 0x001BAD10
	public void SetupItem()
	{
		ItemValue itemValue = ItemClass.GetItem(base.ID, false);
		ItemValue itemValue2 = new ItemValue(ItemClass.GetItem(base.ID, false).type, true);
		int num = 1;
		if (base.Value != null && base.Value != "")
		{
			if (int.TryParse(base.Value, out num))
			{
				if (itemValue2.HasQuality)
				{
					itemValue2 = new ItemValue(itemValue.type, num, num, true, null, 1f);
					num = 1;
				}
				else
				{
					itemValue2 = new ItemValue(itemValue.type, true);
				}
			}
			else if (base.Value.Contains("-"))
			{
				string[] array = base.Value.Split('-', StringSplitOptions.None);
				int num2 = Convert.ToInt32(array[0]);
				int num3 = Convert.ToInt32(array[1]);
				if (itemValue2.HasQuality)
				{
					itemValue2 = new ItemValue(itemValue.type, num2, num3, true, null, 1f);
					num = 1;
				}
				else
				{
					itemValue2 = new ItemValue(itemValue.type, true);
					num = GameManager.Instance.World.GetGameRandom().RandomRange(num2, num3);
				}
			}
		}
		this.item = new ItemStack(itemValue2, num);
	}

	// Token: 0x06004585 RID: 17797 RVA: 0x001BCC2C File Offset: 0x001BAE2C
	public override BaseReward Clone()
	{
		RewardItem rewardItem = new RewardItem();
		base.CopyValues(rewardItem);
		if (this.item != null)
		{
			rewardItem.item = this.item.Clone();
		}
		return rewardItem;
	}

	// Token: 0x06004586 RID: 17798 RVA: 0x001BCC60 File Offset: 0x001BAE60
	public override string GetRewardText()
	{
		return this.Item.count.ToString() + " x " + this.Item.itemValue.ItemClass.GetLocalizedItemName();
	}

	// Token: 0x04003666 RID: 13926
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack item;
}
