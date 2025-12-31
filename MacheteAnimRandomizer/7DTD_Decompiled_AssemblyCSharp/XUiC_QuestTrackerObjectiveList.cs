using System;
using System.Collections.Generic;
using Challenges;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DC3 RID: 3523
[Preserve]
public class XUiC_QuestTrackerObjectiveList : XUiController
{
	// Token: 0x17000B17 RID: 2839
	// (get) Token: 0x06006E39 RID: 28217 RVA: 0x002CED4C File Offset: 0x002CCF4C
	// (set) Token: 0x06006E3A RID: 28218 RVA: 0x002CED54 File Offset: 0x002CCF54
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

	// Token: 0x17000B18 RID: 2840
	// (get) Token: 0x06006E3B RID: 28219 RVA: 0x002CED64 File Offset: 0x002CCF64
	// (set) Token: 0x06006E3C RID: 28220 RVA: 0x002CED6C File Offset: 0x002CCF6C
	public Challenge Challenge
	{
		get
		{
			return this.challenge;
		}
		set
		{
			if (this.challenge != null)
			{
				this.challenge.OnChallengeStateChanged -= this.CurrentChallenge_OnChallengeStateChanged;
			}
			this.challenge = value;
			if (this.challenge != null)
			{
				this.challenge.OnChallengeStateChanged += this.CurrentChallenge_OnChallengeStateChanged;
			}
			this.isDirty = true;
		}
	}

	// Token: 0x06006E3D RID: 28221 RVA: 0x002CEDC5 File Offset: 0x002CCFC5
	[PublicizedFrom(EAccessModifier.Private)]
	public void CurrentChallenge_OnChallengeStateChanged(Challenge challenge)
	{
		this.isDirty = true;
	}

	// Token: 0x06006E3E RID: 28222 RVA: 0x002CEDD0 File Offset: 0x002CCFD0
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_QuestTrackerObjectiveEntry>(null);
		XUiController[] array = childrenByType;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				this.objectiveEntries.Add(array[i]);
			}
		}
	}

	// Token: 0x06006E3F RID: 28223 RVA: 0x002CEE10 File Offset: 0x002CD010
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
					if ((this.quest.Objectives[i].Phase == this.quest.CurrentPhase || this.quest.Objectives[i].Phase == 0) && !this.quest.Objectives[i].HiddenObjective)
					{
						if (num < count)
						{
							XUiC_QuestTrackerObjectiveEntry xuiC_QuestTrackerObjectiveEntry = this.objectiveEntries[num] as XUiC_QuestTrackerObjectiveEntry;
							if (xuiC_QuestTrackerObjectiveEntry != null)
							{
								xuiC_QuestTrackerObjectiveEntry.Owner = this;
								if (i < count2)
								{
									xuiC_QuestTrackerObjectiveEntry.QuestObjective = this.quest.Objectives[i];
								}
								else
								{
									xuiC_QuestTrackerObjectiveEntry.ClearObjective();
								}
							}
						}
						num++;
					}
				}
				if (num < count)
				{
					for (int j = num; j < count; j++)
					{
						XUiC_QuestTrackerObjectiveEntry xuiC_QuestTrackerObjectiveEntry2 = this.objectiveEntries[j] as XUiC_QuestTrackerObjectiveEntry;
						if (xuiC_QuestTrackerObjectiveEntry2 != null)
						{
							xuiC_QuestTrackerObjectiveEntry2.ClearObjective();
						}
					}
				}
			}
			else if (this.challenge != null)
			{
				List<BaseChallengeObjective> objectiveList = this.challenge.GetObjectiveList();
				int count3 = this.objectiveEntries.Count;
				int count4 = objectiveList.Count;
				int num2 = 0;
				for (int k = 0; k < count4; k++)
				{
					if (num2 < count3)
					{
						XUiC_QuestTrackerObjectiveEntry xuiC_QuestTrackerObjectiveEntry3 = this.objectiveEntries[num2] as XUiC_QuestTrackerObjectiveEntry;
						if (xuiC_QuestTrackerObjectiveEntry3 != null)
						{
							xuiC_QuestTrackerObjectiveEntry3.Owner = this;
							if (k < count4)
							{
								xuiC_QuestTrackerObjectiveEntry3.ChallengeObjective = objectiveList[k];
							}
							else
							{
								xuiC_QuestTrackerObjectiveEntry3.ClearObjective();
							}
						}
					}
					num2++;
				}
				if (num2 < count3)
				{
					for (int l = num2; l < count3; l++)
					{
						XUiC_QuestTrackerObjectiveEntry xuiC_QuestTrackerObjectiveEntry4 = this.objectiveEntries[l] as XUiC_QuestTrackerObjectiveEntry;
						if (xuiC_QuestTrackerObjectiveEntry4 != null)
						{
							xuiC_QuestTrackerObjectiveEntry4.ClearObjective();
						}
					}
				}
			}
			else
			{
				int count5 = this.objectiveEntries.Count;
				for (int m = 0; m < count5; m++)
				{
					XUiC_QuestTrackerObjectiveEntry xuiC_QuestTrackerObjectiveEntry5 = this.objectiveEntries[m] as XUiC_QuestTrackerObjectiveEntry;
					if (xuiC_QuestTrackerObjectiveEntry5 != null)
					{
						xuiC_QuestTrackerObjectiveEntry5.Owner = this;
						xuiC_QuestTrackerObjectiveEntry5.ClearObjective();
					}
				}
			}
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x06006E40 RID: 28224 RVA: 0x002CF058 File Offset: 0x002CD258
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "complete_icon")
		{
			this.completeIconName = value;
			return true;
		}
		if (name == "incomplete_icon")
		{
			this.incompleteIconName = value;
			return true;
		}
		if (name == "complete_color")
		{
			Color32 color = StringParsers.ParseColor(value);
			this.completeColor = string.Format("{0},{1},{2},{3}", new object[]
			{
				color.r,
				color.g,
				color.b,
				color.a
			});
			this.completeHexColor = Utils.ColorToHex(color);
			return true;
		}
		if (!(name == "incomplete_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		Color32 color2 = StringParsers.ParseColor(value);
		this.incompleteColor = string.Format("{0},{1},{2},{3}", new object[]
		{
			color2.r,
			color2.g,
			color2.b,
			color2.a
		});
		this.incompleteHexColor = Utils.ColorToHex(color2);
		return true;
	}

	// Token: 0x040053BE RID: 21438
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest quest;

	// Token: 0x040053BF RID: 21439
	[PublicizedFrom(EAccessModifier.Private)]
	public Challenge challenge;

	// Token: 0x040053C0 RID: 21440
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiController> objectiveEntries = new List<XUiController>();

	// Token: 0x040053C1 RID: 21441
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040053C2 RID: 21442
	public string completeIconName = "";

	// Token: 0x040053C3 RID: 21443
	public string incompleteIconName = "";

	// Token: 0x040053C4 RID: 21444
	public string completeHexColor = "FF00FF00";

	// Token: 0x040053C5 RID: 21445
	public string incompleteHexColor = "FFB400";

	// Token: 0x040053C6 RID: 21446
	public string warningHexColor = "FFFF00FF";

	// Token: 0x040053C7 RID: 21447
	public string inactiveHexColor = "888888FF";

	// Token: 0x040053C8 RID: 21448
	public string activeHexColor = "FFFFFFFF";

	// Token: 0x040053C9 RID: 21449
	public string completeColor = "0,255,0,255";

	// Token: 0x040053CA RID: 21450
	public string incompleteColor = "255, 180, 0, 255";

	// Token: 0x040053CB RID: 21451
	public string warningColor = "255,255,0,255";
}
