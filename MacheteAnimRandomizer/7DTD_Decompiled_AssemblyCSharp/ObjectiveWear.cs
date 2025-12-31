using System;
using UnityEngine.Scripting;

// Token: 0x020008E3 RID: 2275
[Preserve]
public class ObjectiveWear : BaseObjective
{
	// Token: 0x06004311 RID: 17169 RVA: 0x001B13C3 File Offset: 0x001AF5C3
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveWear_keyword", false);
		this.expectedItem = ItemClass.GetItem(this.ID, false);
		this.expectedItemClass = ItemClass.GetItemClass(this.ID, false);
	}

	// Token: 0x06004312 RID: 17170 RVA: 0x001B13FA File Offset: 0x001AF5FA
	public override void SetupDisplay()
	{
		byte currentValue = base.CurrentValue;
		base.Description = string.Format(this.keyword, this.expectedItemClass.GetLocalizedItemName());
		this.StatusText = "";
	}

	// Token: 0x06004313 RID: 17171 RVA: 0x001B142C File Offset: 0x001AF62C
	public override void AddHooks()
	{
		QuestEventManager.Current.WearItem += this.Current_WearItem;
		XUi xui = LocalPlayerUI.GetUIForPlayer(base.OwnerQuest.OwnerJournal.OwnerPlayer).xui;
		XUiM_PlayerInventory playerInventory = xui.PlayerInventory;
		if (xui.PlayerEquipment.IsWearing(this.expectedItem) && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue = 1;
			this.Refresh();
		}
	}

	// Token: 0x06004314 RID: 17172 RVA: 0x001B149C File Offset: 0x001AF69C
	public override void RemoveHooks()
	{
		QuestEventManager.Current.WearItem -= this.Current_WearItem;
	}

	// Token: 0x06004315 RID: 17173 RVA: 0x001B14B4 File Offset: 0x001AF6B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_WearItem(ItemValue itemValue)
	{
		if (itemValue.type == this.expectedItem.type && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue = 1;
			this.Refresh();
		}
	}

	// Token: 0x06004316 RID: 17174 RVA: 0x001B14E4 File Offset: 0x001AF6E4
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		bool complete = base.CurrentValue == 1;
		base.Complete = complete;
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004317 RID: 17175 RVA: 0x001B1524 File Offset: 0x001AF724
	public override BaseObjective Clone()
	{
		ObjectiveWear objectiveWear = new ObjectiveWear();
		this.CopyValues(objectiveWear);
		return objectiveWear;
	}

	// Token: 0x04003520 RID: 13600
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue expectedItem = ItemValue.None.Clone();

	// Token: 0x04003521 RID: 13601
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;
}
