using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000DB3 RID: 3507
[Preserve]
public class XUiC_QuestObjectiveList : XUiController
{
	// Token: 0x06006DB6 RID: 28086 RVA: 0x002CBBD0 File Offset: 0x002C9DD0
	public void SetIsTracker()
	{
		this.isTracker = true;
		for (int i = 0; i < this.objectiveEntries.Count; i++)
		{
			((XUiC_QuestObjectiveEntry)this.objectiveEntries[i]).SetIsTracker();
		}
	}

	// Token: 0x17000AFF RID: 2815
	// (get) Token: 0x06006DB7 RID: 28087 RVA: 0x002CBC10 File Offset: 0x002C9E10
	// (set) Token: 0x06006DB8 RID: 28088 RVA: 0x002CBC18 File Offset: 0x002C9E18
	public Quest Quest
	{
		get
		{
			return this.quest;
		}
		set
		{
			this.quest = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06006DB9 RID: 28089 RVA: 0x002CBC28 File Offset: 0x002C9E28
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_QuestObjectiveEntry>(null);
		XUiController[] array = childrenByType;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				this.objectiveEntries.Add(array[i]);
			}
		}
	}

	// Token: 0x06006DBA RID: 28090 RVA: 0x002CBC66 File Offset: 0x002C9E66
	public override void OnOpen()
	{
		base.OnOpen();
		this.isDirty = true;
	}

	// Token: 0x06006DBB RID: 28091 RVA: 0x002CBC78 File Offset: 0x002C9E78
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			if (this.quest != null)
			{
				int count = this.objectiveEntries.Count;
				int count2 = this.quest.Objectives.Count;
				int num = 0;
				for (int i = 0; i < count2; i++)
				{
					if (this.quest.Objectives[i].Phase <= this.quest.CurrentPhase && !this.quest.Objectives[i].HiddenObjective && this.quest.Objectives[i].ShowInQuestLog)
					{
						if (this.objectiveEntries[num] is XUiC_QuestObjectiveEntry)
						{
							((XUiC_QuestObjectiveEntry)this.objectiveEntries[num]).Owner = this;
							if (i < count2)
							{
								((XUiC_QuestObjectiveEntry)this.objectiveEntries[num]).Objective = this.quest.Objectives[i];
							}
							else
							{
								((XUiC_QuestObjectiveEntry)this.objectiveEntries[num]).Objective = null;
							}
						}
						num++;
					}
				}
				if (num < count)
				{
					for (int j = num; j < count; j++)
					{
						if (this.objectiveEntries[j] is XUiC_QuestObjectiveEntry)
						{
							((XUiC_QuestObjectiveEntry)this.objectiveEntries[j]).Objective = null;
						}
					}
				}
			}
			else
			{
				int count3 = this.objectiveEntries.Count;
				for (int k = 0; k < count3; k++)
				{
					if (this.objectiveEntries[k] is XUiC_QuestObjectiveEntry)
					{
						((XUiC_QuestObjectiveEntry)this.objectiveEntries[k]).Owner = this;
						((XUiC_QuestObjectiveEntry)this.objectiveEntries[k]).Objective = null;
					}
				}
			}
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x0400534C RID: 21324
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest quest;

	// Token: 0x0400534D RID: 21325
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiController> objectiveEntries = new List<XUiController>();

	// Token: 0x0400534E RID: 21326
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400534F RID: 21327
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTracker;

	// Token: 0x04005350 RID: 21328
	public string completeHexColor = "FF00FF00";

	// Token: 0x04005351 RID: 21329
	public string incompleteHexColor = "FFFF0000";

	// Token: 0x04005352 RID: 21330
	public string warningHexColor = "FFFF00FF";

	// Token: 0x04005353 RID: 21331
	public string inactiveHexColor = "888888FF";

	// Token: 0x04005354 RID: 21332
	public string activeHexColor = "FFFFFFFF";
}
