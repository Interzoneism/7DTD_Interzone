using System;
using UnityEngine.Scripting;

// Token: 0x020008D9 RID: 2265
[Preserve]
public class ObjectiveSpendSkillPoints : BaseObjective
{
	// Token: 0x170006F1 RID: 1777
	// (get) Token: 0x060042A7 RID: 17063 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x060042A8 RID: 17064 RVA: 0x001AE6BB File Offset: 0x001AC8BB
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveSpendSkillPoints_keyword", false);
		this.pointCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x060042A9 RID: 17065 RVA: 0x001AE6E0 File Offset: 0x001AC8E0
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, (this.ID != null) ? this.ID : "Any");
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.pointCount);
	}

	// Token: 0x060042AA RID: 17066 RVA: 0x001AE739 File Offset: 0x001AC939
	public override void AddHooks()
	{
		QuestEventManager.Current.SkillPointSpent += this.Current_SkillPointSpent;
	}

	// Token: 0x060042AB RID: 17067 RVA: 0x001AE751 File Offset: 0x001AC951
	public override void RemoveHooks()
	{
		QuestEventManager.Current.SkillPointSpent -= this.Current_SkillPointSpent;
	}

	// Token: 0x060042AC RID: 17068 RVA: 0x001AE76C File Offset: 0x001AC96C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_SkillPointSpent(string skillName)
	{
		if (base.Complete)
		{
			return;
		}
		if ((this.ID == null || skillName.EqualsCaseInsensitive(this.ID)) && base.OwnerQuest.CheckRequirements())
		{
			byte currentValue = base.CurrentValue;
			base.CurrentValue = currentValue + 1;
		}
		this.Refresh();
	}

	// Token: 0x060042AD RID: 17069 RVA: 0x001AE7BC File Offset: 0x001AC9BC
	public override void Refresh()
	{
		if ((int)base.CurrentValue > this.pointCount)
		{
			base.CurrentValue = (byte)this.pointCount;
		}
		base.Complete = ((int)base.CurrentValue >= this.pointCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x060042AE RID: 17070 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RemoveObjectives()
	{
	}

	// Token: 0x060042AF RID: 17071 RVA: 0x001AE814 File Offset: 0x001ACA14
	public override BaseObjective Clone()
	{
		ObjectiveSpendSkillPoints objectiveSpendSkillPoints = new ObjectiveSpendSkillPoints();
		this.CopyValues(objectiveSpendSkillPoints);
		return objectiveSpendSkillPoints;
	}

	// Token: 0x040034CE RID: 13518
	[PublicizedFrom(EAccessModifier.Private)]
	public int pointCount;
}
