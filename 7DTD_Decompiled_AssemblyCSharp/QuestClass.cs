using System;
using System.Collections.Generic;
using Platform;
using Quests.Requirements;

// Token: 0x020008EA RID: 2282
public class QuestClass
{
	// Token: 0x17000718 RID: 1816
	// (get) Token: 0x06004385 RID: 17285 RVA: 0x001B4B23 File Offset: 0x001B2D23
	// (set) Token: 0x06004386 RID: 17286 RVA: 0x001B4B2B File Offset: 0x001B2D2B
	public string ID { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000719 RID: 1817
	// (get) Token: 0x06004387 RID: 17287 RVA: 0x001B4B34 File Offset: 0x001B2D34
	// (set) Token: 0x06004388 RID: 17288 RVA: 0x001B4B3C File Offset: 0x001B2D3C
	public string Name { get; set; }

	// Token: 0x1700071A RID: 1818
	// (get) Token: 0x06004389 RID: 17289 RVA: 0x001B4B45 File Offset: 0x001B2D45
	// (set) Token: 0x0600438A RID: 17290 RVA: 0x001B4B4D File Offset: 0x001B2D4D
	public string GroupName { get; set; }

	// Token: 0x1700071B RID: 1819
	// (get) Token: 0x0600438B RID: 17291 RVA: 0x001B4B56 File Offset: 0x001B2D56
	// (set) Token: 0x0600438C RID: 17292 RVA: 0x001B4B5E File Offset: 0x001B2D5E
	public string SubTitle { get; set; }

	// Token: 0x1700071C RID: 1820
	// (get) Token: 0x0600438D RID: 17293 RVA: 0x001B4B67 File Offset: 0x001B2D67
	// (set) Token: 0x0600438E RID: 17294 RVA: 0x001B4B6F File Offset: 0x001B2D6F
	public string Description { get; set; }

	// Token: 0x1700071D RID: 1821
	// (get) Token: 0x0600438F RID: 17295 RVA: 0x001B4B78 File Offset: 0x001B2D78
	// (set) Token: 0x06004390 RID: 17296 RVA: 0x001B4B80 File Offset: 0x001B2D80
	public string Offer { get; set; }

	// Token: 0x1700071E RID: 1822
	// (get) Token: 0x06004391 RID: 17297 RVA: 0x001B4B89 File Offset: 0x001B2D89
	// (set) Token: 0x06004392 RID: 17298 RVA: 0x001B4B91 File Offset: 0x001B2D91
	public string Difficulty { get; set; }

	// Token: 0x1700071F RID: 1823
	// (get) Token: 0x06004393 RID: 17299 RVA: 0x001B4B9A File Offset: 0x001B2D9A
	// (set) Token: 0x06004394 RID: 17300 RVA: 0x001B4BA2 File Offset: 0x001B2DA2
	public string Icon { get; set; }

	// Token: 0x17000720 RID: 1824
	// (get) Token: 0x06004395 RID: 17301 RVA: 0x001B4BAB File Offset: 0x001B2DAB
	// (set) Token: 0x06004396 RID: 17302 RVA: 0x001B4BB3 File Offset: 0x001B2DB3
	public bool Repeatable { get; set; }

	// Token: 0x17000721 RID: 1825
	// (get) Token: 0x06004397 RID: 17303 RVA: 0x001B4BBC File Offset: 0x001B2DBC
	// (set) Token: 0x06004398 RID: 17304 RVA: 0x001B4BC4 File Offset: 0x001B2DC4
	public bool Shareable { get; set; }

	// Token: 0x17000722 RID: 1826
	// (get) Token: 0x06004399 RID: 17305 RVA: 0x001B4BCD File Offset: 0x001B2DCD
	// (set) Token: 0x0600439A RID: 17306 RVA: 0x001B4BD5 File Offset: 0x001B2DD5
	public string Category { get; set; }

	// Token: 0x17000723 RID: 1827
	// (get) Token: 0x0600439B RID: 17307 RVA: 0x001B4BDE File Offset: 0x001B2DDE
	// (set) Token: 0x0600439C RID: 17308 RVA: 0x001B4BE6 File Offset: 0x001B2DE6
	public string StatementText { get; set; }

	// Token: 0x17000724 RID: 1828
	// (get) Token: 0x0600439D RID: 17309 RVA: 0x001B4BEF File Offset: 0x001B2DEF
	// (set) Token: 0x0600439E RID: 17310 RVA: 0x001B4BF7 File Offset: 0x001B2DF7
	public string ResponseText { get; set; }

	// Token: 0x17000725 RID: 1829
	// (get) Token: 0x0600439F RID: 17311 RVA: 0x001B4C00 File Offset: 0x001B2E00
	// (set) Token: 0x060043A0 RID: 17312 RVA: 0x001B4C08 File Offset: 0x001B2E08
	public string CompleteText { get; set; }

	// Token: 0x17000726 RID: 1830
	// (get) Token: 0x060043A1 RID: 17313 RVA: 0x001B4C11 File Offset: 0x001B2E11
	// (set) Token: 0x060043A2 RID: 17314 RVA: 0x001B4C19 File Offset: 0x001B2E19
	public byte CurrentVersion { get; set; }

	// Token: 0x17000727 RID: 1831
	// (get) Token: 0x060043A3 RID: 17315 RVA: 0x001B4C22 File Offset: 0x001B2E22
	// (set) Token: 0x060043A4 RID: 17316 RVA: 0x001B4C2A File Offset: 0x001B2E2A
	public byte HighestPhase { get; set; }

	// Token: 0x17000728 RID: 1832
	// (get) Token: 0x060043A5 RID: 17317 RVA: 0x001B4C33 File Offset: 0x001B2E33
	// (set) Token: 0x060043A6 RID: 17318 RVA: 0x001B4C3B File Offset: 0x001B2E3B
	public byte QuestFaction { get; set; }

	// Token: 0x17000729 RID: 1833
	// (get) Token: 0x060043A7 RID: 17319 RVA: 0x001B4C44 File Offset: 0x001B2E44
	// (set) Token: 0x060043A8 RID: 17320 RVA: 0x001B4C4C File Offset: 0x001B2E4C
	public byte DifficultyTier { get; set; }

	// Token: 0x1700072A RID: 1834
	// (get) Token: 0x060043A9 RID: 17321 RVA: 0x001B4C55 File Offset: 0x001B2E55
	// (set) Token: 0x060043AA RID: 17322 RVA: 0x001B4C5D File Offset: 0x001B2E5D
	public bool LoginRallyReset { get; set; }

	// Token: 0x1700072B RID: 1835
	// (get) Token: 0x060043AB RID: 17323 RVA: 0x001B4C66 File Offset: 0x001B2E66
	// (set) Token: 0x060043AC RID: 17324 RVA: 0x001B4C6E File Offset: 0x001B2E6E
	public bool ReturnToQuestGiver { get; set; }

	// Token: 0x1700072C RID: 1836
	// (get) Token: 0x060043AD RID: 17325 RVA: 0x001B4C77 File Offset: 0x001B2E77
	// (set) Token: 0x060043AE RID: 17326 RVA: 0x001B4C7F File Offset: 0x001B2E7F
	public string UniqueKey { get; set; }

	// Token: 0x1700072D RID: 1837
	// (get) Token: 0x060043AF RID: 17327 RVA: 0x001B4C88 File Offset: 0x001B2E88
	// (set) Token: 0x060043B0 RID: 17328 RVA: 0x001B4C90 File Offset: 0x001B2E90
	public string QuestType { get; set; }

	// Token: 0x1700072E RID: 1838
	// (get) Token: 0x060043B1 RID: 17329 RVA: 0x001B4C99 File Offset: 0x001B2E99
	// (set) Token: 0x060043B2 RID: 17330 RVA: 0x001B4CA1 File Offset: 0x001B2EA1
	public bool AddsToTierComplete { get; set; }

	// Token: 0x060043B3 RID: 17331 RVA: 0x001B4CAC File Offset: 0x001B2EAC
	public static QuestClass NewClass(string id)
	{
		if (QuestClass.s_Quests.ContainsKey(id))
		{
			return null;
		}
		QuestClass questClass = new QuestClass(id.ToLower());
		QuestClass.s_Quests[id] = questClass;
		return questClass;
	}

	// Token: 0x060043B4 RID: 17332 RVA: 0x001B4CE4 File Offset: 0x001B2EE4
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestClass(string id)
	{
		this.ID = id;
		this.Difficulty = "veryeasy";
		this.HighestPhase = 1;
		this.AddsToTierComplete = true;
	}

	// Token: 0x060043B5 RID: 17333 RVA: 0x001B4D93 File Offset: 0x001B2F93
	[PublicizedFrom(EAccessModifier.Internal)]
	public static QuestClass GetQuest(string questID)
	{
		if (!QuestClass.s_Quests.ContainsKey(questID))
		{
			return null;
		}
		return QuestClass.s_Quests[questID];
	}

	// Token: 0x060043B6 RID: 17334 RVA: 0x001B4DB0 File Offset: 0x001B2FB0
	[PublicizedFrom(EAccessModifier.Internal)]
	public DynamicProperties AssignValuesFrom(QuestClass oldQuest)
	{
		DynamicProperties dynamicProperties = new DynamicProperties();
		HashSet<string> exclude = new HashSet<string>();
		if (oldQuest.Properties != null)
		{
			dynamicProperties.CopyFrom(oldQuest.Properties, exclude);
		}
		for (int i = 0; i < oldQuest.Requirements.Count; i++)
		{
			BaseRequirement baseRequirement = oldQuest.Requirements[i].Clone();
			baseRequirement.Properties = new DynamicProperties();
			if (oldQuest.Requirements[i].Properties != null)
			{
				baseRequirement.Properties.CopyFrom(oldQuest.Requirements[i].Properties, null);
			}
			baseRequirement.Owner = this;
			this.Requirements.Add(baseRequirement);
		}
		for (int j = 0; j < oldQuest.Actions.Count; j++)
		{
			BaseQuestAction baseQuestAction = oldQuest.Actions[j].Clone();
			baseQuestAction.Properties = new DynamicProperties();
			if (oldQuest.Actions[j].Properties != null)
			{
				baseQuestAction.Properties.CopyFrom(oldQuest.Actions[j].Properties, null);
			}
			baseQuestAction.Owner = this;
			this.Actions.Add(baseQuestAction);
		}
		for (int k = 0; k < oldQuest.Objectives.Count; k++)
		{
			BaseObjective baseObjective = oldQuest.Objectives[k].Clone();
			baseObjective.Properties = new DynamicProperties();
			if (oldQuest.Objectives[k].Properties != null)
			{
				baseObjective.Properties.CopyFrom(oldQuest.Objectives[k].Properties, null);
			}
			if (oldQuest.Objectives[k].Phase > this.HighestPhase)
			{
				this.HighestPhase = oldQuest.Objectives[k].Phase;
			}
			baseObjective.OwnerQuestClass = this;
			this.Objectives.Add(baseObjective);
		}
		for (int l = 0; l < oldQuest.Events.Count; l++)
		{
			QuestEvent questEvent = oldQuest.Events[l].Clone();
			questEvent.Properties = new DynamicProperties();
			if (oldQuest.Events[l].Properties != null)
			{
				questEvent.Properties.CopyFrom(oldQuest.Events[l].Properties, null);
			}
			questEvent.Owner = this;
			this.Events.Add(questEvent);
		}
		return dynamicProperties;
	}

	// Token: 0x060043B7 RID: 17335 RVA: 0x001B5013 File Offset: 0x001B3213
	public static Quest CreateQuest(string ID)
	{
		return QuestClass.GetQuest(ID).CreateQuest();
	}

	// Token: 0x060043B8 RID: 17336 RVA: 0x001B5020 File Offset: 0x001B3220
	public Quest CreateQuest()
	{
		Quest quest = new Quest(this.ID);
		quest.CurrentQuestVersion = this.CurrentVersion;
		quest.Tracked = false;
		quest.FinishTime = 0UL;
		quest.QuestFaction = this.QuestFaction;
		if (!this.ExtraTags.IsEmpty)
		{
			quest.QuestTags |= this.ExtraTags;
		}
		for (int i = 0; i < this.Actions.Count; i++)
		{
			BaseQuestAction baseQuestAction = this.Actions[i].Clone();
			baseQuestAction.OwnerQuest = quest;
			quest.Actions.Add(baseQuestAction);
		}
		for (int j = 0; j < this.Requirements.Count; j++)
		{
			BaseRequirement baseRequirement = this.Requirements[j].Clone();
			baseRequirement.OwnerQuest = quest;
			quest.Requirements.Add(baseRequirement);
		}
		for (int k = 0; k < this.Objectives.Count; k++)
		{
			BaseObjective baseObjective = this.Objectives[k].Clone();
			baseObjective.OwnerQuest = quest;
			quest.Objectives.Add(baseObjective);
		}
		int num = 0;
		for (int l = 0; l < this.Rewards.Count; l++)
		{
			BaseReward baseReward = this.Rewards[l].Clone();
			baseReward.OwnerQuest = quest;
			quest.Rewards.Add(baseReward);
			if (!baseReward.isChainReward && baseReward.isChosenReward && !baseReward.isFixedLocation)
			{
				num++;
			}
		}
		return quest;
	}

	// Token: 0x060043B9 RID: 17337 RVA: 0x001B51AC File Offset: 0x001B33AC
	public void ResetObjectives()
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].ResetObjective();
		}
	}

	// Token: 0x060043BA RID: 17338 RVA: 0x001B51E0 File Offset: 0x001B33E0
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseQuestAction AddAction(BaseQuestAction action)
	{
		if (action != null)
		{
			this.Actions.Add(action);
		}
		return action;
	}

	// Token: 0x060043BB RID: 17339 RVA: 0x001B51F2 File Offset: 0x001B33F2
	public QuestEvent AddEvent(QuestEvent questEvent)
	{
		if (questEvent != null)
		{
			this.Events.Add(questEvent);
		}
		return questEvent;
	}

	// Token: 0x060043BC RID: 17340 RVA: 0x001B5204 File Offset: 0x001B3404
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseRequirement AddRequirement(BaseRequirement requirement)
	{
		if (requirement != null)
		{
			this.Requirements.Add(requirement);
		}
		return requirement;
	}

	// Token: 0x060043BD RID: 17341 RVA: 0x001B5216 File Offset: 0x001B3416
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseObjective AddObjective(BaseObjective objective)
	{
		if (objective != null)
		{
			objective.OwnerQuestClass = this;
			this.Objectives.Add(objective);
		}
		return objective;
	}

	// Token: 0x060043BE RID: 17342 RVA: 0x001B522F File Offset: 0x001B342F
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseReward AddReward(BaseReward reward)
	{
		if (reward != null)
		{
			this.Rewards.Add(reward);
		}
		return reward;
	}

	// Token: 0x060043BF RID: 17343 RVA: 0x001B5241 File Offset: 0x001B3441
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseQuestCriteria AddCriteria(BaseQuestCriteria criteria)
	{
		if (criteria != null)
		{
			this.Criteria.Add(criteria);
		}
		return criteria;
	}

	// Token: 0x060043C0 RID: 17344 RVA: 0x001B5254 File Offset: 0x001B3454
	public bool CheckCriteriaQuestGiver(EntityNPC entityNPC)
	{
		for (int i = 0; i < this.Criteria.Count; i++)
		{
			if (this.Criteria[i].CriteriaType == BaseQuestCriteria.CriteriaTypes.QuestGiver && !this.Criteria[i].CheckForQuestGiver(entityNPC))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060043C1 RID: 17345 RVA: 0x001B52A4 File Offset: 0x001B34A4
	public bool CheckCriteriaOffer(EntityPlayer player)
	{
		for (int i = 0; i < this.Criteria.Count; i++)
		{
			if (this.Criteria[i].CriteriaType == BaseQuestCriteria.CriteriaTypes.QuestGiver && !this.Criteria[i].CheckForPlayer(player))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060043C2 RID: 17346 RVA: 0x001B52F4 File Offset: 0x001B34F4
	public bool CanActivate()
	{
		if (GameStats.GetBool(EnumGameStats.EnemySpawnMode))
		{
			return true;
		}
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (this.Objectives[i].RequiresZombies)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060043C3 RID: 17347 RVA: 0x001B5338 File Offset: 0x001B3538
	public string GetCurrentHint(int phase)
	{
		phase--;
		if (this.QuestHints == null || phase >= this.QuestHints.Count)
		{
			return "";
		}
		string text = this.QuestHints[phase];
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			string key = text + "_alt";
			if (Localization.Exists(key, false))
			{
				return Localization.Get(key, false);
			}
		}
		return Localization.Get(text, false);
	}

	// Token: 0x060043C4 RID: 17348 RVA: 0x001B53AC File Offset: 0x001B35AC
	[PublicizedFrom(EAccessModifier.Internal)]
	public void Init()
	{
		if (this.Properties.Values.ContainsKey(QuestClass.PropCategory))
		{
			this.Category = this.Properties.Values[QuestClass.PropCategory];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropCategoryKey))
		{
			this.Category = Localization.Get(this.Properties.Values[QuestClass.PropCategoryKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropGroupName))
		{
			this.GroupName = this.Properties.Values[QuestClass.PropGroupName];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropGroupNameKey))
		{
			this.GroupName = Localization.Get(this.Properties.Values[QuestClass.PropGroupNameKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropName))
		{
			this.Name = this.Properties.Values[QuestClass.PropName];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropNameKey))
		{
			this.Name = Localization.Get(this.Properties.Values[QuestClass.PropNameKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropSubtitle))
		{
			this.SubTitle = this.Properties.Values[QuestClass.PropSubtitle];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropSubtitleKey))
		{
			this.SubTitle = Localization.Get(this.Properties.Values[QuestClass.PropSubtitleKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropDescription))
		{
			this.Description = this.Properties.Values[QuestClass.PropDescription];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropDescriptionKey))
		{
			this.Description = Localization.Get(this.Properties.Values[QuestClass.PropDescriptionKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropOffer))
		{
			this.Offer = this.Properties.Values[QuestClass.PropOffer];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropOfferKey))
		{
			this.Offer = Localization.Get(this.Properties.Values[QuestClass.PropOfferKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropIcon))
		{
			this.Icon = this.Properties.Values[QuestClass.PropIcon];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropRepeatable))
		{
			bool repeatable;
			StringParsers.TryParseBool(this.Properties.Values[QuestClass.PropRepeatable], out repeatable, 0, -1, true);
			this.Repeatable = repeatable;
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropShareable))
		{
			bool shareable;
			StringParsers.TryParseBool(this.Properties.Values[QuestClass.PropShareable], out shareable, 0, -1, true);
			this.Shareable = shareable;
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropDifficulty))
		{
			this.Difficulty = Localization.Get(string.Format("difficulty_{0}", this.Properties.Values[QuestClass.PropDifficulty]), false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropCompletionType))
		{
			this.CompletionType = EnumUtils.Parse<QuestClass.CompletionTypes>(this.Properties.Values[QuestClass.PropCompletionType], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropCurrentVersion))
		{
			this.CurrentVersion = Convert.ToByte(this.Properties.Values[QuestClass.PropCurrentVersion]);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropStatementText))
		{
			this.StatementText = this.Properties.Values[QuestClass.PropStatementText];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropResponseText))
		{
			this.ResponseText = this.Properties.Values[QuestClass.PropResponseText];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropCompleteText))
		{
			this.CompleteText = this.Properties.Values[QuestClass.PropCompleteText];
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropStatementKey))
		{
			this.StatementText = Localization.Get(this.Properties.Values[QuestClass.PropStatementKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropResponseKey))
		{
			this.ResponseText = Localization.Get(this.Properties.Values[QuestClass.PropResponseKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropCompleteKey))
		{
			this.CompleteText = Localization.Get(this.Properties.Values[QuestClass.PropCompleteKey], false);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropQuestFaction))
		{
			this.QuestFaction = Convert.ToByte(this.Properties.Values[QuestClass.PropQuestFaction]);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropDifficultyTier))
		{
			this.DifficultyTier = Convert.ToByte(this.Properties.Values[QuestClass.PropDifficultyTier]);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropLoginRallyReset))
		{
			this.LoginRallyReset = Convert.ToBoolean(this.Properties.Values[QuestClass.PropLoginRallyReset]);
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropUniqueKey))
		{
			this.UniqueKey = this.Properties.Values[QuestClass.PropUniqueKey];
		}
		else
		{
			this.UniqueKey = "";
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropReturnToQuestGiver))
		{
			this.ReturnToQuestGiver = StringParsers.ParseBool(this.Properties.Values[QuestClass.PropReturnToQuestGiver], 0, -1, true);
		}
		else
		{
			this.ReturnToQuestGiver = true;
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropQuestType))
		{
			this.QuestType = this.Properties.Values[QuestClass.PropQuestType];
		}
		else
		{
			this.QuestType = "";
		}
		if (this.Properties.Values.ContainsKey(QuestClass.PropAddsToTierComplete))
		{
			this.AddsToTierComplete = StringParsers.ParseBool(this.Properties.Values[QuestClass.PropAddsToTierComplete], 0, -1, true);
		}
		this.Properties.ParseInt(QuestClass.PropRewardChoicesCount, ref this.RewardChoicesCount);
		if (this.Properties.Values.ContainsKey(QuestClass.PropExtraTags))
		{
			string text = this.Properties.Values[QuestClass.PropExtraTags];
			if (text != "")
			{
				this.ExtraTags = FastTags<TagGroup.Global>.Parse(text);
			}
		}
		string text2 = "";
		this.Properties.ParseString(QuestClass.PropQuestHints, ref text2);
		if (text2 != "")
		{
			if (this.QuestHints == null)
			{
				this.QuestHints = new List<string>();
			}
			this.QuestHints.AddRange(text2.Split(',', StringSplitOptions.None));
		}
		this.Properties.ParseFloat(QuestClass.PropQuestGameStageMod, ref this.GameStageMod);
		this.Properties.ParseFloat(QuestClass.PropQuestGameStageBonus, ref this.GameStageBonus);
		this.Properties.ParseFloat(QuestClass.PropSpawnMultiplier, ref this.SpawnMultiplier);
		this.Properties.ParseBool(QuestClass.PropResetTraderQuests, ref this.ResetTraderQuests);
		this.Properties.ParseBool(QuestClass.PropSingleQuest, ref this.SingleQuest);
		this.Properties.ParseBool(QuestClass.PropAlwaysAllow, ref this.AlwaysAllow);
		this.Properties.ParseBool(QuestClass.PropAllowRemove, ref this.AllowRemove);
	}

	// Token: 0x060043C5 RID: 17349 RVA: 0x001B5BB4 File Offset: 0x001B3DB4
	public void HandleVariablesForProperties(DynamicProperties properties)
	{
		foreach (KeyValuePair<string, string> keyValuePair in properties.Params1.Dict)
		{
			if (this.Variables.ContainsKey(keyValuePair.Value))
			{
				properties.Values[keyValuePair.Key] = this.Variables[keyValuePair.Value];
			}
		}
	}

	// Token: 0x060043C6 RID: 17350 RVA: 0x001B5C40 File Offset: 0x001B3E40
	public void HandleTemplateInit()
	{
		for (int i = 0; i < this.Actions.Count; i++)
		{
			this.HandleVariablesForProperties(this.Actions[i].Properties);
			this.Actions[i].ParseProperties(this.Actions[i].Properties);
		}
		for (int j = 0; j < this.Events.Count; j++)
		{
			this.HandleVariablesForProperties(this.Events[j].Properties);
			this.Events[j].ParseProperties(this.Events[j].Properties);
			for (int k = 0; k < this.Events[j].Actions.Count; k++)
			{
				BaseQuestAction baseQuestAction = this.Events[j].Actions[k];
				this.HandleVariablesForProperties(baseQuestAction.Properties);
				baseQuestAction.ParseProperties(baseQuestAction.Properties);
			}
		}
		for (int l = 0; l < this.Objectives.Count; l++)
		{
			BaseObjective baseObjective = this.Objectives[l];
			this.HandleVariablesForProperties(baseObjective.Properties);
			baseObjective.ParseProperties(baseObjective.Properties);
			if (baseObjective.Modifiers != null)
			{
				for (int m = 0; m < baseObjective.Modifiers.Count; m++)
				{
					BaseObjectiveModifier baseObjectiveModifier = baseObjective.Modifiers[m];
					this.HandleVariablesForProperties(baseObjectiveModifier.Properties);
					baseObjectiveModifier.ParseProperties(baseObjectiveModifier.Properties);
				}
			}
		}
	}

	// Token: 0x04003562 RID: 13666
	public static Dictionary<string, QuestClass> s_Quests = new CaseInsensitiveStringDictionary<QuestClass>();

	// Token: 0x04003563 RID: 13667
	public static string PropCategory = "category";

	// Token: 0x04003564 RID: 13668
	public static string PropCategoryKey = "category_key";

	// Token: 0x04003565 RID: 13669
	public static string PropGroupName = "group_name";

	// Token: 0x04003566 RID: 13670
	public static string PropGroupNameKey = "group_name_key";

	// Token: 0x04003567 RID: 13671
	public static string PropName = "name";

	// Token: 0x04003568 RID: 13672
	public static string PropNameKey = "name_key";

	// Token: 0x04003569 RID: 13673
	public static string PropSubtitle = "subtitle";

	// Token: 0x0400356A RID: 13674
	public static string PropSubtitleKey = "subtitle_key";

	// Token: 0x0400356B RID: 13675
	public static string PropDescription = "description";

	// Token: 0x0400356C RID: 13676
	public static string PropDescriptionKey = "description_key";

	// Token: 0x0400356D RID: 13677
	public static string PropOffer = "offer";

	// Token: 0x0400356E RID: 13678
	public static string PropOfferKey = "offer_key";

	// Token: 0x0400356F RID: 13679
	public static string PropIcon = "icon";

	// Token: 0x04003570 RID: 13680
	public static string PropRepeatable = "repeatable";

	// Token: 0x04003571 RID: 13681
	public static string PropShareable = "shareable";

	// Token: 0x04003572 RID: 13682
	public static string PropDifficulty = "difficulty";

	// Token: 0x04003573 RID: 13683
	public static string PropCompletionType = "completiontype";

	// Token: 0x04003574 RID: 13684
	public static string PropCurrentVersion = "currentversion";

	// Token: 0x04003575 RID: 13685
	public static string PropStatementText = "statement_text";

	// Token: 0x04003576 RID: 13686
	public static string PropResponseText = "response_text";

	// Token: 0x04003577 RID: 13687
	public static string PropCompleteText = "completion_text";

	// Token: 0x04003578 RID: 13688
	public static string PropStatementKey = "statement_key";

	// Token: 0x04003579 RID: 13689
	public static string PropResponseKey = "response_key";

	// Token: 0x0400357A RID: 13690
	public static string PropCompleteKey = "completion_key";

	// Token: 0x0400357B RID: 13691
	public static string PropVariations = "variations";

	// Token: 0x0400357C RID: 13692
	public static string PropQuestFaction = "quest_faction";

	// Token: 0x0400357D RID: 13693
	public static string PropDifficultyTier = "difficulty_tier";

	// Token: 0x0400357E RID: 13694
	public static string PropLoginRallyReset = "login_rally_reset";

	// Token: 0x0400357F RID: 13695
	public static string PropUniqueKey = "unique_key";

	// Token: 0x04003580 RID: 13696
	public static string PropReturnToQuestGiver = "return_to_quest_giver";

	// Token: 0x04003581 RID: 13697
	public static string PropQuestType = "quest_type";

	// Token: 0x04003582 RID: 13698
	public static string PropAddsToTierComplete = "add_to_tier_complete";

	// Token: 0x04003583 RID: 13699
	public static string PropRewardChoicesCount = "reward_choices_count";

	// Token: 0x04003584 RID: 13700
	public static string PropExtraTags = "extra_tags";

	// Token: 0x04003585 RID: 13701
	public static string PropQuestStage = "quest_stage";

	// Token: 0x04003586 RID: 13702
	public static string PropQuestHints = "quest_hints";

	// Token: 0x04003587 RID: 13703
	public static string PropQuestGameStageMod = "gamestage_mod";

	// Token: 0x04003588 RID: 13704
	public static string PropQuestGameStageBonus = "gamestage_bonus";

	// Token: 0x04003589 RID: 13705
	public static string PropSpawnMultiplier = "spawn_multiplier";

	// Token: 0x0400358A RID: 13706
	public static string PropResetTraderQuests = "reset_trader_quests";

	// Token: 0x0400358B RID: 13707
	public static string PropSingleQuest = "single_quest";

	// Token: 0x0400358C RID: 13708
	public static string PropAlwaysAllow = "always_allow";

	// Token: 0x0400358D RID: 13709
	public static string PropAllowRemove = "allow_remove";

	// Token: 0x040035A5 RID: 13733
	public List<string> QuestHints;

	// Token: 0x040035A6 RID: 13734
	public int RewardChoicesCount = 2;

	// Token: 0x040035A7 RID: 13735
	public FastTags<TagGroup.Global> ExtraTags = FastTags<TagGroup.Global>.none;

	// Token: 0x040035A8 RID: 13736
	public float GameStageMod;

	// Token: 0x040035A9 RID: 13737
	public float GameStageBonus;

	// Token: 0x040035AA RID: 13738
	public float SpawnMultiplier = 1f;

	// Token: 0x040035AB RID: 13739
	public bool ResetTraderQuests;

	// Token: 0x040035AC RID: 13740
	public bool SingleQuest;

	// Token: 0x040035AD RID: 13741
	public bool AlwaysAllow;

	// Token: 0x040035AE RID: 13742
	public bool AllowRemove = true;

	// Token: 0x040035AF RID: 13743
	public QuestClass.CompletionTypes CompletionType;

	// Token: 0x040035B0 RID: 13744
	public List<BaseQuestCriteria> Criteria = new List<BaseQuestCriteria>();

	// Token: 0x040035B1 RID: 13745
	public List<BaseQuestAction> Actions = new List<BaseQuestAction>();

	// Token: 0x040035B2 RID: 13746
	public List<QuestEvent> Events = new List<QuestEvent>();

	// Token: 0x040035B3 RID: 13747
	public List<BaseRequirement> Requirements = new List<BaseRequirement>();

	// Token: 0x040035B4 RID: 13748
	public List<BaseObjective> Objectives = new List<BaseObjective>();

	// Token: 0x040035B5 RID: 13749
	public List<BaseReward> Rewards = new List<BaseReward>();

	// Token: 0x040035B6 RID: 13750
	public Dictionary<string, string> Variables = new Dictionary<string, string>();

	// Token: 0x040035B7 RID: 13751
	public DynamicProperties Properties = new DynamicProperties();

	// Token: 0x020008EB RID: 2283
	public enum CompletionTypes
	{
		// Token: 0x040035B9 RID: 13753
		AutoComplete,
		// Token: 0x040035BA RID: 13754
		TurnIn
	}
}
