using System;
using UnityEngine.Scripting;

// Token: 0x020008B1 RID: 2225
[Preserve]
public class ObjectiveAssemble : BaseObjective
{
	// Token: 0x060040F4 RID: 16628 RVA: 0x001A5DDD File Offset: 0x001A3FDD
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveAssemble_keyword", false);
		this.expectedItem = ItemClass.GetItem(this.ID, false);
		this.expectedItemClass = ItemClass.GetItemClass(this.ID, false);
	}

	// Token: 0x060040F5 RID: 16629 RVA: 0x001A5E14 File Offset: 0x001A4014
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.expectedItemClass.GetLocalizedItemName());
		this.StatusText = "";
	}

	// Token: 0x060040F6 RID: 16630 RVA: 0x001A5E3D File Offset: 0x001A403D
	public override void AddHooks()
	{
		QuestEventManager.Current.AssembleItem += this.Current_AssembleItem;
	}

	// Token: 0x060040F7 RID: 16631 RVA: 0x001A5E55 File Offset: 0x001A4055
	public override void RemoveHooks()
	{
		QuestEventManager.Current.AssembleItem -= this.Current_AssembleItem;
	}

	// Token: 0x060040F8 RID: 16632 RVA: 0x001A5E6D File Offset: 0x001A406D
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_AssembleItem(ItemStack stack)
	{
		if (stack.itemValue.type == this.expectedItem.type && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue = 1;
			this.Refresh();
		}
	}

	// Token: 0x060040F9 RID: 16633 RVA: 0x001A5EA4 File Offset: 0x001A40A4
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

	// Token: 0x060040FA RID: 16634 RVA: 0x001A5EE4 File Offset: 0x001A40E4
	public override BaseObjective Clone()
	{
		ObjectiveAssemble objectiveAssemble = new ObjectiveAssemble();
		this.CopyValues(objectiveAssemble);
		return objectiveAssemble;
	}

	// Token: 0x04003405 RID: 13317
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue expectedItem = ItemValue.None.Clone();

	// Token: 0x04003406 RID: 13318
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass expectedItemClass;

	// Token: 0x04003407 RID: 13319
	[PublicizedFrom(EAccessModifier.Private)]
	public bool assembled;
}
