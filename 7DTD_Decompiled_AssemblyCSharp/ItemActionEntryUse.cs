using System;
using System.Collections;
using UnityEngine.Scripting;

// Token: 0x02000BF1 RID: 3057
[Preserve]
public class ItemActionEntryUse : BaseItemActionEntry
{
	// Token: 0x06005DB2 RID: 23986 RVA: 0x0026168C File Offset: 0x0025F88C
	public ItemActionEntryUse(XUiController controller, ItemActionEntryUse.ConsumeType consumeType) : base(controller, "", "", BaseItemActionEntry.GamepadShortCut.DPadLeft, "crafting/craft_click_craft", "ui/ui_denied")
	{
		this.consumeType = consumeType;
		switch (consumeType)
		{
		case ItemActionEntryUse.ConsumeType.Eat:
			base.ActionName = Localization.Get("lblContextActionEat", false);
			base.IconName = "ui_game_symbol_fork";
			base.SoundName = "";
			break;
		case ItemActionEntryUse.ConsumeType.Drink:
			base.ActionName = Localization.Get("lblContextActionDrink", false);
			base.IconName = "ui_game_symbol_water";
			base.SoundName = "";
			break;
		case ItemActionEntryUse.ConsumeType.Heal:
			base.ActionName = Localization.Get("lblContextActionHeal", false);
			base.IconName = "ui_game_symbol_medical";
			break;
		case ItemActionEntryUse.ConsumeType.Read:
			base.ActionName = Localization.Get("lblContextActionRead", false);
			base.IconName = "ui_game_symbol_book";
			base.SoundName = "";
			break;
		case ItemActionEntryUse.ConsumeType.Quest:
			base.ActionName = Localization.Get("lblContextActionRead", false);
			base.IconName = "ui_game_symbol_quest";
			break;
		case ItemActionEntryUse.ConsumeType.Open:
			base.ActionName = Localization.Get("lblContextActionOpen", false);
			base.IconName = "ui_game_symbol_treasure";
			break;
		}
		this.RefreshEnabled();
		base.ActionName = base.ActionName;
	}

	// Token: 0x06005DB3 RID: 23987 RVA: 0x002617CC File Offset: 0x0025F9CC
	public override void OnDisabledActivate()
	{
		EntityPlayerLocal entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
		if (entityPlayer.AttachedToEntity)
		{
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttCannotUseWhileOnVehicle", false), false, false, 0f);
			return;
		}
		if (base.ItemController.xui.PlayerInventory.Toolbelt.IsHoldingItemActionRunning() || base.ItemController.xui.isUsingItemActionEntryUse)
		{
			GameManager.ShowTooltip(entityPlayer, Localization.Get("isBusy", false), false, false, 0f);
			return;
		}
		if (XUiC_AssembleWindowGroup.GetWindowGroup(base.ItemController.xui).IsOpen)
		{
			GameManager.ShowTooltip(entityPlayer, Localization.Get("ttCannotUseWhileAssembling", false), false, false, 0f);
			return;
		}
		switch (this.consumeType)
		{
		case ItemActionEntryUse.ConsumeType.Eat:
			if (XUiM_Player.GetFoodPercent(entityPlayer) >= 1f)
			{
				GameManager.ShowTooltip(entityPlayer, Localization.Get("notHungry", false), false, false, 0f);
				return;
			}
			GameManager.ShowTooltip(entityPlayer, Localization.Get("isBusy", false), false, false, 0f);
			return;
		case ItemActionEntryUse.ConsumeType.Drink:
			if (XUiM_Player.GetWaterPercent(entityPlayer) >= 1f)
			{
				GameManager.ShowTooltip(entityPlayer, Localization.Get("notThirsty", false), false, false, 0f);
				return;
			}
			GameManager.ShowTooltip(entityPlayer, Localization.Get("isBusy", false), false, false, 0f);
			return;
		case ItemActionEntryUse.ConsumeType.Heal:
			GameManager.ShowTooltip(entityPlayer, Localization.Get("notHurt", false), false, false, 0f);
			return;
		case ItemActionEntryUse.ConsumeType.Read:
			switch (this.state)
			{
			case ItemActionEntryUse.StateTypes.RecipeKnown:
				GameManager.ShowTooltip(entityPlayer, Localization.Get("alreadyKnown", false), false, false, 0f);
				return;
			case ItemActionEntryUse.StateTypes.SkillRequirementsNotMet:
				GameManager.ShowTooltip(entityPlayer, Localization.Get("ttSkillRequirementsNotMet", false), false, false, 0f);
				return;
			case ItemActionEntryUse.StateTypes.SkillKnown:
				GameManager.ShowTooltip(entityPlayer, Localization.Get("ttSkillMaxLevel", false), false, false, 0f);
				return;
			default:
				return;
			}
			break;
		case ItemActionEntryUse.ConsumeType.Quest:
			GameManager.ShowTooltip(entityPlayer, Localization.Get("questunavailable", false), false, false, 0f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06005DB4 RID: 23988 RVA: 0x002619C8 File Offset: 0x0025FBC8
	public override void RefreshEnabled()
	{
		this.state = ItemActionEntryUse.StateTypes.Normal;
		EntityPlayer entityPlayer = base.ItemController.xui.playerUI.entityPlayer;
		if (entityPlayer.AttachedToEntity)
		{
			base.Enabled = false;
			return;
		}
		if (entityPlayer.inventory.IsHoldingItemActionRunning())
		{
			base.Enabled = false;
			return;
		}
		if (XUiC_AssembleWindowGroup.GetWindowGroup(base.ItemController.xui).IsOpen)
		{
			base.Enabled = false;
			return;
		}
		ItemStack itemStack = ((XUiC_ItemStack)base.ItemController).ItemStack;
		switch (this.consumeType)
		{
		case ItemActionEntryUse.ConsumeType.Eat:
			base.Enabled = (!base.ItemController.xui.isUsingItemActionEntryUse && XUiM_Player.GetFoodPercent(entityPlayer) < 1f);
			break;
		case ItemActionEntryUse.ConsumeType.Drink:
			base.Enabled = (!base.ItemController.xui.isUsingItemActionEntryUse && XUiM_Player.GetWaterPercent(entityPlayer) < 1f);
			break;
		case ItemActionEntryUse.ConsumeType.Heal:
			base.Enabled = true;
			break;
		case ItemActionEntryUse.ConsumeType.Read:
			if (itemStack != null && !itemStack.IsEmpty())
			{
				ItemClass forId = ItemClass.GetForId(itemStack.itemValue.type);
				bool flag = false;
				for (int i = 0; i < forId.Actions.Length; i++)
				{
					ItemActionLearnRecipe itemActionLearnRecipe = forId.Actions[i] as ItemActionLearnRecipe;
					if (itemActionLearnRecipe != null)
					{
						for (int j = 0; j < itemActionLearnRecipe.RecipesToLearn.Length; j++)
						{
							this.state = ItemActionEntryUse.StateTypes.RecipeKnown;
							if (!XUiM_Recipes.GetRecipeIsUnlocked(base.ItemController.xui, itemActionLearnRecipe.RecipesToLearn[j]))
							{
								this.state = ItemActionEntryUse.StateTypes.Normal;
								flag = true;
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
				base.Enabled = flag;
			}
			else
			{
				base.Enabled = false;
			}
			break;
		case ItemActionEntryUse.ConsumeType.Quest:
			if (itemStack != null && !itemStack.IsEmpty())
			{
				ItemClass forId2 = ItemClass.GetForId(itemStack.itemValue.type);
				int k = 0;
				while (k < forId2.Actions.Length)
				{
					ItemActionQuest itemActionQuest = forId2.Actions[k] as ItemActionQuest;
					if (itemActionQuest != null)
					{
						if (!QuestClass.GetQuest(itemActionQuest.QuestGiven).CanActivate())
						{
							base.Enabled = false;
							break;
						}
						Quest quest = base.ItemController.xui.playerUI.entityPlayer.QuestJournal.FindQuest(itemActionQuest.QuestGiven, -1);
						bool flag2 = base.ItemController.xui.playerUI.entityPlayer.QuestJournal.FindActiveQuest(itemActionQuest.QuestGiven, -1) != null;
						base.Enabled = (quest == null || (QuestClass.GetQuest(itemActionQuest.QuestGiven).Repeatable && !flag2));
						break;
					}
					else
					{
						k++;
					}
				}
			}
			break;
		case ItemActionEntryUse.ConsumeType.Open:
			base.Enabled = true;
			break;
		}
		Inventory toolbelt = base.ItemController.xui.PlayerInventory.Toolbelt;
		base.Enabled = (base.Enabled && toolbelt.GetItem(toolbelt.DUMMY_SLOT_IDX).IsEmpty());
	}

	// Token: 0x06005DB5 RID: 23989 RVA: 0x00261CD0 File Offset: 0x0025FED0
	public override void OnActivated()
	{
		ItemActionEntryUse.<>c__DisplayClass7_0 CS$<>8__locals1 = new ItemActionEntryUse.<>c__DisplayClass7_0();
		CS$<>8__locals1.<>4__this = this;
		if (base.ItemController.xui.isUsingItemActionEntryUse)
		{
			return;
		}
		CS$<>8__locals1.stackControl = (XUiC_ItemStack)base.ItemController;
		if (!CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass.CanExecuteAction(0, base.ItemController.xui.playerUI.entityPlayer, CS$<>8__locals1.stackControl.ItemStack.itemValue))
		{
			GameManager.ShowTooltip(base.ItemController.xui.playerUI.entityPlayer, Localization.Get("ttCannotUseAtThisTime", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		base.ItemController.xui.isUsingItemActionEntryUse = true;
		CS$<>8__locals1.itemStack = new ItemStack(CS$<>8__locals1.stackControl.ItemStack.itemValue.Clone(), 1);
		CS$<>8__locals1.originalStack = new ItemStack(CS$<>8__locals1.stackControl.ItemStack.itemValue.Clone(), CS$<>8__locals1.stackControl.ItemStack.count);
		CS$<>8__locals1.inventory = base.ItemController.xui.PlayerInventory.Toolbelt;
		if (this.consumeType == ItemActionEntryUse.ConsumeType.Quest)
		{
			base.ItemController.xui.FindWindowGroupByName("questOffer").GetChildByType<XUiC_QuestOfferWindow>().ItemStackController = CS$<>8__locals1.stackControl;
			CS$<>8__locals1.stackControl.QuestLock = true;
		}
		else
		{
			CS$<>8__locals1.stackControl.HiddenLock = true;
		}
		CS$<>8__locals1.stackControl.WindowGroup.Controller.SetAllChildrenDirty(false);
		this.RefreshEnabled();
		this.oldToolbeltFocusID = CS$<>8__locals1.inventory.GetFocusedItemIdx();
		CS$<>8__locals1.actionIdx = 0;
		if (CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass != null)
		{
			for (int i = 0; i < CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass.Actions.Length; i++)
			{
				bool flag = false;
				switch (this.consumeType)
				{
				case ItemActionEntryUse.ConsumeType.Eat:
				case ItemActionEntryUse.ConsumeType.Drink:
				case ItemActionEntryUse.ConsumeType.Heal:
					if (CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass.Actions[i] != null)
					{
						flag = true;
					}
					break;
				case ItemActionEntryUse.ConsumeType.Read:
					if (CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass.Actions[i] is ItemActionLearnRecipe)
					{
						flag = true;
					}
					break;
				case ItemActionEntryUse.ConsumeType.Quest:
					if (CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass.Actions[i] is ItemActionQuest)
					{
						flag = true;
					}
					break;
				case ItemActionEntryUse.ConsumeType.Open:
					if (CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass.Actions[i] is ItemActionOpenBundle)
					{
						flag = true;
					}
					else if (CS$<>8__locals1.stackControl.ItemStack.itemValue.ItemClass.Actions[i] is ItemActionOpenLootBundle)
					{
						flag = true;
					}
					break;
				}
				if (flag)
				{
					CS$<>8__locals1.actionIdx = i;
					break;
				}
			}
		}
		if (!(CS$<>8__locals1.itemStack.itemValue.ItemClass.Actions[CS$<>8__locals1.actionIdx] is ItemActionEat))
		{
			CS$<>8__locals1.originalStack.count--;
			if (CS$<>8__locals1.originalStack.count == 0)
			{
				CS$<>8__locals1.originalStack = ItemStack.Empty.Clone();
			}
		}
		if (this.consumeType != ItemActionEntryUse.ConsumeType.Quest)
		{
			CS$<>8__locals1.stackControl.ItemStack = CS$<>8__locals1.originalStack;
		}
		if (!CS$<>8__locals1.itemStack.itemValue.ItemClass.Actions[CS$<>8__locals1.actionIdx].UseAnimation && CS$<>8__locals1.itemStack.itemValue.ItemClass.Actions[CS$<>8__locals1.actionIdx].ExecuteInstantAction(base.ItemController.xui.playerUI.entityPlayer, CS$<>8__locals1.itemStack, false, CS$<>8__locals1.stackControl))
		{
			if (this.consumeType != ItemActionEntryUse.ConsumeType.Quest)
			{
				CS$<>8__locals1.stackControl.HiddenLock = false;
				CS$<>8__locals1.stackControl.WindowGroup.Controller.SetAllChildrenDirty(false);
			}
			base.ItemController.xui.isUsingItemActionEntryUse = false;
			return;
		}
		ItemActionEat itemActionEat = CS$<>8__locals1.itemStack.itemValue.ItemClass.Actions[CS$<>8__locals1.actionIdx] as ItemActionEat;
		if (itemActionEat != null && itemActionEat.UsePrompt)
		{
			base.ItemController.xui.isUsingItemActionEntryPromptComplete = true;
			XUiC_MessageBoxWindowGroup.ShowMessageBox(base.ItemController.xui, Localization.Get(itemActionEat.PromptTitle, false), Localization.Get(itemActionEat.PromptDescription, false), XUiC_MessageBoxWindowGroup.MessageBoxTypes.OkCancel, new Action(CS$<>8__locals1.<OnActivated>g__UseItemWithAnimation|1), new Action(CS$<>8__locals1.<OnActivated>g__SwitchBack|2), true, true, true);
			return;
		}
		CS$<>8__locals1.<OnActivated>g__UseItemWithAnimation|1();
	}

	// Token: 0x06005DB6 RID: 23990 RVA: 0x00262167 File Offset: 0x00260367
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SwitchBackCoroutine(Inventory inventory)
	{
		while (inventory.IsHolsterDelayActive())
		{
			yield return null;
		}
		((XUiC_ItemStack)base.ItemController).HiddenLock = false;
		base.ParentActionList.RefreshActionList();
		base.ItemController.xui.isUsingItemActionEntryUse = false;
		this.RefreshEnabled();
		yield break;
	}

	// Token: 0x06005DB7 RID: 23991 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnTimerCompleted()
	{
	}

	// Token: 0x040046D9 RID: 18137
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldToolbeltFocusID;

	// Token: 0x040046DA RID: 18138
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemActionEntryUse.ConsumeType consumeType;

	// Token: 0x040046DB RID: 18139
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemActionEntryUse.StateTypes state;

	// Token: 0x02000BF2 RID: 3058
	[PublicizedFrom(EAccessModifier.Private)]
	public enum StateTypes
	{
		// Token: 0x040046DD RID: 18141
		Normal,
		// Token: 0x040046DE RID: 18142
		RecipeKnown,
		// Token: 0x040046DF RID: 18143
		SkillRequirementsNotMet,
		// Token: 0x040046E0 RID: 18144
		SkillKnown
	}

	// Token: 0x02000BF3 RID: 3059
	public enum ConsumeType
	{
		// Token: 0x040046E2 RID: 18146
		None,
		// Token: 0x040046E3 RID: 18147
		Eat,
		// Token: 0x040046E4 RID: 18148
		Drink,
		// Token: 0x040046E5 RID: 18149
		Heal,
		// Token: 0x040046E6 RID: 18150
		Read,
		// Token: 0x040046E7 RID: 18151
		Quest,
		// Token: 0x040046E8 RID: 18152
		Open
	}
}
