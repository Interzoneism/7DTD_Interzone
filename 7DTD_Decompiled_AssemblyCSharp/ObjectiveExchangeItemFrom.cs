using System;
using UnityEngine.Scripting;

// Token: 0x020008BD RID: 2237
[Preserve]
public class ObjectiveExchangeItemFrom : BaseObjective
{
	// Token: 0x170006CA RID: 1738
	// (get) Token: 0x0600416B RID: 16747 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x0600416C RID: 16748 RVA: 0x001A793C File Offset: 0x001A5B3C
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveExchangeItemFrom_keyword", false);
		this.expectedItem = ItemClass.GetItem(this.ID, false);
		this.expectedItemClass = ItemClass.GetItemClass(this.ID, false);
		this.exchangeCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x0600416D RID: 16749 RVA: 0x001A7990 File Offset: 0x001A5B90
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.expectedItemClass.GetLocalizedItemName());
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.exchangeCount);
	}

	// Token: 0x0600416E RID: 16750 RVA: 0x001A79DF File Offset: 0x001A5BDF
	public override void AddHooks()
	{
		QuestEventManager.Current.ExchangeFromItem += this.Current_ExchangeItem;
	}

	// Token: 0x0600416F RID: 16751 RVA: 0x001A79F7 File Offset: 0x001A5BF7
	public override void RemoveHooks()
	{
		QuestEventManager.Current.ExchangeFromItem -= this.Current_ExchangeItem;
	}

	// Token: 0x06004170 RID: 16752 RVA: 0x001A7A10 File Offset: 0x001A5C10
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_ExchangeItem(ItemStack itemStack)
	{
		if (base.Complete)
		{
			return;
		}
		if (itemStack.itemValue.type == this.expectedItem.type && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue += (byte)itemStack.count;
			this.Refresh();
		}
	}

	// Token: 0x06004171 RID: 16753 RVA: 0x001A7A66 File Offset: 0x001A5C66
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		base.Complete = ((int)base.CurrentValue >= this.exchangeCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004172 RID: 16754 RVA: 0x001A7AA0 File Offset: 0x001A5CA0
	public override BaseObjective Clone()
	{
		ObjectiveExchangeItemFrom objectiveExchangeItemFrom = new ObjectiveExchangeItemFrom();
		this.CopyValues(objectiveExchangeItemFrom);
		return objectiveExchangeItemFrom;
	}

	// Token: 0x04003433 RID: 13363
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue expectedItem = ItemValue.None.Clone();

	// Token: 0x04003434 RID: 13364
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;

	// Token: 0x04003435 RID: 13365
	[PublicizedFrom(EAccessModifier.Private)]
	public int exchangeCount;
}
