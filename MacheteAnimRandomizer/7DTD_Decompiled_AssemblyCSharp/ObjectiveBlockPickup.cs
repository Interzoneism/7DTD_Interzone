using System;
using UnityEngine.Scripting;

// Token: 0x020008B4 RID: 2228
[Preserve]
public class ObjectiveBlockPickup : BaseObjective
{
	// Token: 0x170006C1 RID: 1729
	// (get) Token: 0x06004112 RID: 16658 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x06004113 RID: 16659 RVA: 0x001A6388 File Offset: 0x001A4588
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveBlockPickup_keyword", false);
		this.localizedName = ((this.ID != "" && this.ID != null) ? Localization.Get(this.ID, false) : "Any Block");
		this.neededCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x06004114 RID: 16660 RVA: 0x001A63EA File Offset: 0x001A45EA
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.localizedName);
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.neededCount);
	}

	// Token: 0x06004115 RID: 16661 RVA: 0x001A6429 File Offset: 0x001A4629
	public override void AddHooks()
	{
		QuestEventManager.Current.BlockPickup += this.Current_BlockPickup;
	}

	// Token: 0x06004116 RID: 16662 RVA: 0x001A6441 File Offset: 0x001A4641
	public override void RemoveHooks()
	{
		QuestEventManager.Current.BlockPickup -= this.Current_BlockPickup;
	}

	// Token: 0x06004117 RID: 16663 RVA: 0x001A645C File Offset: 0x001A465C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockPickup(string blockname, Vector3i blockPos)
	{
		if (base.Complete)
		{
			return;
		}
		bool flag = false;
		if (this.ID == null || this.ID == "" || this.ID.EqualsCaseInsensitive(blockname))
		{
			flag = true;
		}
		if (!flag && this.ID != null && this.ID != "")
		{
			Block blockByName = Block.GetBlockByName(this.ID, true);
			if (blockByName != null && blockByName.SelectAlternates && blockByName.ContainsAlternateBlock(blockname))
			{
				flag = true;
			}
		}
		if (flag && base.OwnerQuest.CheckRequirements())
		{
			byte currentValue = base.CurrentValue;
			base.CurrentValue = currentValue + 1;
			this.Refresh();
		}
	}

	// Token: 0x06004118 RID: 16664 RVA: 0x001A6508 File Offset: 0x001A4708
	public override void Refresh()
	{
		if ((int)base.CurrentValue > this.neededCount)
		{
			base.CurrentValue = (byte)this.neededCount;
		}
		if (base.Complete)
		{
			return;
		}
		base.Complete = ((int)base.CurrentValue >= this.neededCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004119 RID: 16665 RVA: 0x001A6568 File Offset: 0x001A4768
	public override BaseObjective Clone()
	{
		ObjectiveBlockPickup objectiveBlockPickup = new ObjectiveBlockPickup();
		this.CopyValues(objectiveBlockPickup);
		return objectiveBlockPickup;
	}

	// Token: 0x04003414 RID: 13332
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName = "";

	// Token: 0x04003415 RID: 13333
	[PublicizedFrom(EAccessModifier.Private)]
	public int neededCount;
}
