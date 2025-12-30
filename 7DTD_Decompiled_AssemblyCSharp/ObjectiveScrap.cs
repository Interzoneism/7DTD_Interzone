using System;
using UnityEngine.Scripting;

// Token: 0x020008D8 RID: 2264
[Preserve]
public class ObjectiveScrap : BaseObjective
{
	// Token: 0x170006F0 RID: 1776
	// (get) Token: 0x0600429E RID: 17054 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x0600429F RID: 17055 RVA: 0x001AE524 File Offset: 0x001AC724
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveScrap_keyword", false);
		this.expectedItem = ItemClass.GetItem(this.ID, false);
		this.expectedItemClass = ItemClass.GetItemClass(this.ID, false);
		this.scrapCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x060042A0 RID: 17056 RVA: 0x001AE578 File Offset: 0x001AC778
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.expectedItemClass.GetLocalizedItemName());
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.scrapCount);
	}

	// Token: 0x060042A1 RID: 17057 RVA: 0x001AE5C7 File Offset: 0x001AC7C7
	public override void AddHooks()
	{
		QuestEventManager.Current.ScrapItem += this.Current_ScrapItem;
	}

	// Token: 0x060042A2 RID: 17058 RVA: 0x001AE5DF File Offset: 0x001AC7DF
	public override void RemoveHooks()
	{
		QuestEventManager.Current.ScrapItem -= this.Current_ScrapItem;
	}

	// Token: 0x060042A3 RID: 17059 RVA: 0x001AE5F8 File Offset: 0x001AC7F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_ScrapItem(ItemStack stack)
	{
		if (base.Complete)
		{
			return;
		}
		if (stack.itemValue.type == this.expectedItem.type && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue += (byte)stack.count;
			this.Refresh();
		}
	}

	// Token: 0x060042A4 RID: 17060 RVA: 0x001AE64E File Offset: 0x001AC84E
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		base.Complete = ((int)base.CurrentValue >= this.scrapCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x060042A5 RID: 17061 RVA: 0x001AE688 File Offset: 0x001AC888
	public override BaseObjective Clone()
	{
		ObjectiveScrap objectiveScrap = new ObjectiveScrap();
		this.CopyValues(objectiveScrap);
		return objectiveScrap;
	}

	// Token: 0x040034CB RID: 13515
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue expectedItem = ItemValue.None.Clone();

	// Token: 0x040034CC RID: 13516
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;

	// Token: 0x040034CD RID: 13517
	[PublicizedFrom(EAccessModifier.Private)]
	public int scrapCount;
}
