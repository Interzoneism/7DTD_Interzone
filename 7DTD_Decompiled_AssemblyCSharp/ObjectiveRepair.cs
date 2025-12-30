using System;
using UnityEngine.Scripting;

// Token: 0x020008D6 RID: 2262
[Preserve]
public class ObjectiveRepair : BaseObjective
{
	// Token: 0x170006EE RID: 1774
	// (get) Token: 0x0600428E RID: 17038 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x0600428F RID: 17039 RVA: 0x001AE264 File Offset: 0x001AC464
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveRepair_keyword", false);
		this.expectedItem = ItemClass.GetItem(this.ID, false);
		this.expectedItemClass = ItemClass.GetItemClass(this.ID, false);
		this.repairCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x06004290 RID: 17040 RVA: 0x001AE2B8 File Offset: 0x001AC4B8
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.expectedItemClass.GetLocalizedItemName());
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.repairCount);
	}

	// Token: 0x06004291 RID: 17041 RVA: 0x001AE307 File Offset: 0x001AC507
	public override void AddHooks()
	{
		QuestEventManager.Current.RepairItem += this.Current_RepairItem;
	}

	// Token: 0x06004292 RID: 17042 RVA: 0x001AE31F File Offset: 0x001AC51F
	public override void RemoveHooks()
	{
		QuestEventManager.Current.RepairItem -= this.Current_RepairItem;
	}

	// Token: 0x06004293 RID: 17043 RVA: 0x001AE338 File Offset: 0x001AC538
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_RepairItem(ItemValue itemValue)
	{
		if (base.Complete)
		{
			return;
		}
		if (itemValue.type == this.expectedItem.type && base.OwnerQuest.CheckRequirements())
		{
			byte currentValue = base.CurrentValue;
			base.CurrentValue = currentValue + 1;
			this.Refresh();
		}
	}

	// Token: 0x06004294 RID: 17044 RVA: 0x001AE385 File Offset: 0x001AC585
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		base.Complete = ((int)base.CurrentValue >= this.repairCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004295 RID: 17045 RVA: 0x001AE3C0 File Offset: 0x001AC5C0
	public override BaseObjective Clone()
	{
		ObjectiveRepair objectiveRepair = new ObjectiveRepair();
		this.CopyValues(objectiveRepair);
		return objectiveRepair;
	}

	// Token: 0x040034C8 RID: 13512
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue expectedItem = ItemValue.None.Clone();

	// Token: 0x040034C9 RID: 13513
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;

	// Token: 0x040034CA RID: 13514
	[PublicizedFrom(EAccessModifier.Private)]
	public int repairCount;
}
