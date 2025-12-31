using System;
using UnityEngine.Scripting;

// Token: 0x020008B5 RID: 2229
[Preserve]
public class ObjectiveBlockPlace : BaseObjective
{
	// Token: 0x170006C2 RID: 1730
	// (get) Token: 0x0600411B RID: 16667 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x0600411C RID: 16668 RVA: 0x001A6598 File Offset: 0x001A4798
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveBlockPlace_keyword", false);
		this.localizedName = ((this.ID != "" && this.ID != null) ? Localization.Get(this.ID, false) : "Any Block");
		this.neededCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x0600411D RID: 16669 RVA: 0x001A65FA File Offset: 0x001A47FA
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.localizedName);
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.neededCount);
	}

	// Token: 0x0600411E RID: 16670 RVA: 0x001A6639 File Offset: 0x001A4839
	public override void AddHooks()
	{
		QuestEventManager.Current.BlockPlace -= this.Current_BlockPlace;
		QuestEventManager.Current.BlockPlace += this.Current_BlockPlace;
	}

	// Token: 0x0600411F RID: 16671 RVA: 0x001A6667 File Offset: 0x001A4867
	public override void RemoveHooks()
	{
		QuestEventManager.Current.BlockPlace -= this.Current_BlockPlace;
	}

	// Token: 0x06004120 RID: 16672 RVA: 0x001A6680 File Offset: 0x001A4880
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockPlace(string blockname, Vector3i blockPos)
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

	// Token: 0x06004121 RID: 16673 RVA: 0x001A672C File Offset: 0x001A492C
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

	// Token: 0x06004122 RID: 16674 RVA: 0x001A678C File Offset: 0x001A498C
	public override BaseObjective Clone()
	{
		ObjectiveBlockPlace objectiveBlockPlace = new ObjectiveBlockPlace();
		this.CopyValues(objectiveBlockPlace);
		return objectiveBlockPlace;
	}

	// Token: 0x04003416 RID: 13334
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName = "";

	// Token: 0x04003417 RID: 13335
	[PublicizedFrom(EAccessModifier.Private)]
	public int neededCount;
}
