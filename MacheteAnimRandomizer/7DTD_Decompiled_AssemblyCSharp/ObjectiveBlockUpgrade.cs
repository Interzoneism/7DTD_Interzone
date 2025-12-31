using System;
using UnityEngine.Scripting;

// Token: 0x020008B6 RID: 2230
[Preserve]
public class ObjectiveBlockUpgrade : BaseObjective
{
	// Token: 0x170006C3 RID: 1731
	// (get) Token: 0x06004124 RID: 16676 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x06004125 RID: 16677 RVA: 0x001A67BC File Offset: 0x001A49BC
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveBlockUpgrade_keyword", false);
		this.localizedName = ((this.ID != "" && this.ID != null) ? Localization.Get(this.ID, false) : "Any Block");
		this.neededCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x06004126 RID: 16678 RVA: 0x001A681E File Offset: 0x001A4A1E
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.localizedName);
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.neededCount);
	}

	// Token: 0x06004127 RID: 16679 RVA: 0x001A685D File Offset: 0x001A4A5D
	public override void AddHooks()
	{
		QuestEventManager.Current.BlockUpgrade -= this.Current_BlockUpgrade;
		QuestEventManager.Current.BlockUpgrade += this.Current_BlockUpgrade;
	}

	// Token: 0x06004128 RID: 16680 RVA: 0x001A688B File Offset: 0x001A4A8B
	public override void RemoveHooks()
	{
		QuestEventManager.Current.BlockUpgrade -= this.Current_BlockUpgrade;
	}

	// Token: 0x06004129 RID: 16681 RVA: 0x001A68A4 File Offset: 0x001A4AA4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockUpgrade(string blockname, Vector3i blockPos)
	{
		if (base.Complete)
		{
			return;
		}
		bool flag = false;
		if (this.ID == null || this.ID == "")
		{
			flag = true;
		}
		else
		{
			if (this.ID.EqualsCaseInsensitive(blockname))
			{
				flag = true;
			}
			if (blockname.Contains(":") && this.ID.EqualsCaseInsensitive(blockname.Substring(0, blockname.IndexOf(':'))))
			{
				flag = true;
			}
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

	// Token: 0x0600412A RID: 16682 RVA: 0x001A697C File Offset: 0x001A4B7C
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

	// Token: 0x0600412B RID: 16683 RVA: 0x001A69DC File Offset: 0x001A4BDC
	public override BaseObjective Clone()
	{
		ObjectiveBlockUpgrade objectiveBlockUpgrade = new ObjectiveBlockUpgrade();
		this.CopyValues(objectiveBlockUpgrade);
		return objectiveBlockUpgrade;
	}

	// Token: 0x04003418 RID: 13336
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName = "";

	// Token: 0x04003419 RID: 13337
	[PublicizedFrom(EAccessModifier.Private)]
	public int neededCount;
}
