using System;
using UnityEngine.Scripting;

// Token: 0x02000DB4 RID: 3508
[Preserve]
public class XUiC_QuestObjectivesWindow : XUiController
{
	// Token: 0x17000B00 RID: 2816
	// (get) Token: 0x06006DBD RID: 28093 RVA: 0x002CBEA1 File Offset: 0x002CA0A1
	// (set) Token: 0x06006DBE RID: 28094 RVA: 0x002CBEA9 File Offset: 0x002CA0A9
	public Quest CurrentQuest
	{
		get
		{
			return this.currentQuest;
		}
		set
		{
			this.currentQuest = value;
			this.questClass = ((value != null) ? QuestClass.GetQuest(value.ID) : null);
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006DBF RID: 28095 RVA: 0x002CBED0 File Offset: 0x002CA0D0
	public override void Init()
	{
		base.Init();
		this.objectiveList = base.GetChildByType<XUiC_QuestObjectiveList>();
	}

	// Token: 0x06006DC0 RID: 28096 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x06006DC1 RID: 28097 RVA: 0x002CBEE4 File Offset: 0x002CA0E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2375080848U)
		{
			if (num <= 196213477U)
			{
				if (num != 72500844U)
				{
					if (num == 196213477U)
					{
						if (bindingName == "questrequirementstitle")
						{
							value = Localization.Get("xuiRequirements", false);
							return true;
						}
					}
				}
				else if (bindingName == "showrequirements")
				{
					value = ((this.currentQuest != null) ? (this.currentQuest.Requirements.Count > 0).ToString() : "false");
					return true;
				}
			}
			else if (num != 2061479048U)
			{
				if (num != 2369797097U)
				{
					if (num == 2375080848U)
					{
						if (bindingName == "tieradd")
						{
							if (this.currentQuest != null && this.questClass.AddsToTierComplete && (this.currentQuest.AddsProgression || (this.currentQuest.OwnerJournal.CanAddProgression && this.currentQuest.QuestProgressDay == -2147483648)))
							{
								if (this.questClass.DifficultyTier != 0)
								{
									string arg = ((this.questClass.DifficultyTier > 0) ? "+" : "-") + this.questClass.DifficultyTier.ToString();
									value = string.Format(Localization.Get("xuiQuestTierAdd", false), arg);
								}
								else
								{
									value = "";
								}
							}
							else
							{
								value = "";
							}
							return true;
						}
					}
				}
				else if (bindingName == "questrequirements")
				{
					value = ((this.currentQuest != null) ? this.currentQuest.RequirementsString.Replace("DEFAULT_COLOR", this.defaultReqColor).Replace("MISSING_COLOR", this.missingReqColor) : "");
					return true;
				}
			}
			else if (bindingName == "questdifficulty")
			{
				value = ((this.currentQuest != null) ? this.questClass.Difficulty : "");
				return true;
			}
		}
		else if (num <= 3229612626U)
		{
			if (num != 2730462270U)
			{
				if (num != 3047389681U)
				{
					if (num == 3229612626U)
					{
						if (bindingName == "tieraddlimited")
						{
							if (this.currentQuest != null && this.questClass.AddsToTierComplete && ((!this.currentQuest.OwnerJournal.CanAddProgression && this.currentQuest.QuestProgressDay == -2147483648) || this.currentQuest.QuestProgressDay == -1))
							{
								value = "true";
							}
							else
							{
								value = "false";
							}
							return true;
						}
					}
				}
				else if (bindingName == "questtitle")
				{
					value = ((this.currentQuest != null) ? this.questClass.SubTitle : "");
					return true;
				}
			}
			else if (bindingName == "questname")
			{
				value = ((this.currentQuest != null) ? this.questClass.Name : "");
				return true;
			}
		}
		else if (num != 3231221182U)
		{
			if (num != 3607901221U)
			{
				if (num == 4060322893U)
				{
					if (bindingName == "showempty")
					{
						value = (this.currentQuest == null).ToString();
						return true;
					}
				}
			}
			else if (bindingName == "tieraddlimitedcolor")
			{
				if (this.currentQuest != null && this.questClass.AddsToTierComplete)
				{
					if (this.currentQuest.QuestProgressDay == -2147483648 && !this.currentQuest.OwnerJournal.CanAddProgression)
					{
						value = this.questlimitedColor;
					}
					else
					{
						value = this.questlimitedcompleteColor;
					}
				}
				else
				{
					value = "255,255,255,255";
				}
				return true;
			}
		}
		else if (bindingName == "showquest")
		{
			value = (this.currentQuest != null).ToString();
			return true;
		}
		return false;
	}

	// Token: 0x06006DC2 RID: 28098 RVA: 0x002CC2DC File Offset: 0x002CA4DC
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (name == "default_req_color")
			{
				this.defaultReqColor = Utils.ColorToHex(StringParsers.ParseColor32(value));
				return true;
			}
			if (name == "missing_req_color")
			{
				this.missingReqColor = Utils.ColorToHex(StringParsers.ParseColor32(value));
				return true;
			}
			if (name == "quest_limited_color")
			{
				this.questlimitedColor = value;
				return true;
			}
			if (name == "quest_limited_complete_color")
			{
				this.questlimitedcompleteColor = value;
				return true;
			}
		}
		return flag;
	}

	// Token: 0x06006DC3 RID: 28099 RVA: 0x002CC36F File Offset: 0x002CA56F
	public void SetQuest(XUiC_QuestEntry questEntry)
	{
		this.entry = questEntry;
		if (this.entry != null)
		{
			this.CurrentQuest = this.entry.Quest;
			this.objectiveList.Quest = this.entry.Quest;
			return;
		}
		this.CurrentQuest = null;
	}

	// Token: 0x04005355 RID: 21333
	[PublicizedFrom(EAccessModifier.Private)]
	public string defaultReqColor = "DECEA3";

	// Token: 0x04005356 RID: 21334
	[PublicizedFrom(EAccessModifier.Private)]
	public string missingReqColor = "FF0000";

	// Token: 0x04005357 RID: 21335
	[PublicizedFrom(EAccessModifier.Private)]
	public string questlimitedColor = "FF0000";

	// Token: 0x04005358 RID: 21336
	[PublicizedFrom(EAccessModifier.Private)]
	public string questlimitedcompleteColor = "FFFFFF";

	// Token: 0x04005359 RID: 21337
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestEntry entry;

	// Token: 0x0400535A RID: 21338
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_QuestObjectiveList objectiveList;

	// Token: 0x0400535B RID: 21339
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestClass questClass;

	// Token: 0x0400535C RID: 21340
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest currentQuest;
}
