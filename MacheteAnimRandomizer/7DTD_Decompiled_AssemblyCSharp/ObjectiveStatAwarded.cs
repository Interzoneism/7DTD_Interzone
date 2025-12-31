using System;
using UnityEngine.Scripting;

// Token: 0x020008DA RID: 2266
[Preserve]
public class ObjectiveStatAwarded : BaseObjective
{
	// Token: 0x170006F2 RID: 1778
	// (get) Token: 0x060042B1 RID: 17073 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x060042B2 RID: 17074 RVA: 0x001AE82F File Offset: 0x001ACA2F
	public override void SetupObjective()
	{
		this.keyword = this.statText;
		this.neededCount = Convert.ToInt32(this.Value);
	}

	// Token: 0x060042B3 RID: 17075 RVA: 0x001AE84E File Offset: 0x001ACA4E
	public override void SetupDisplay()
	{
		base.Description = this.statText;
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.neededCount);
	}

	// Token: 0x060042B4 RID: 17076 RVA: 0x001AE882 File Offset: 0x001ACA82
	public override void AddHooks()
	{
		QuestEventManager.Current.QuestAwardCredit += this.Current_QuestAwardCredit;
	}

	// Token: 0x060042B5 RID: 17077 RVA: 0x001AE89A File Offset: 0x001ACA9A
	public override void RemoveHooks()
	{
		QuestEventManager.Current.QuestAwardCredit -= this.Current_QuestAwardCredit;
	}

	// Token: 0x060042B6 RID: 17078 RVA: 0x001AE8B2 File Offset: 0x001ACAB2
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_QuestAwardCredit(string stat, int awardCount)
	{
		if (base.Complete)
		{
			return;
		}
		if (this.statName.EqualsCaseInsensitive(stat))
		{
			base.CurrentValue += (byte)awardCount;
			if ((int)base.CurrentValue >= this.neededCount)
			{
				this.Refresh();
			}
		}
	}

	// Token: 0x060042B7 RID: 17079 RVA: 0x001AE8F0 File Offset: 0x001ACAF0
	public override void Refresh()
	{
		if ((int)base.CurrentValue > this.neededCount)
		{
			base.CurrentValue = (byte)this.neededCount;
		}
		base.Complete = ((int)base.CurrentValue >= this.neededCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x060042B8 RID: 17080 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RemoveObjectives()
	{
	}

	// Token: 0x060042B9 RID: 17081 RVA: 0x001AE946 File Offset: 0x001ACB46
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CopyValues(BaseObjective objective)
	{
		base.CopyValues(objective);
		ObjectiveStatAwarded objectiveStatAwarded = (ObjectiveStatAwarded)objective;
		objectiveStatAwarded.statName = this.statName;
		objectiveStatAwarded.statText = this.statText;
	}

	// Token: 0x060042BA RID: 17082 RVA: 0x001AE96C File Offset: 0x001ACB6C
	public override BaseObjective Clone()
	{
		ObjectiveStatAwarded objectiveStatAwarded = new ObjectiveStatAwarded();
		this.CopyValues(objectiveStatAwarded);
		return objectiveStatAwarded;
	}

	// Token: 0x060042BB RID: 17083 RVA: 0x001AE988 File Offset: 0x001ACB88
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		string text = "";
		properties.ParseString(ObjectiveStatAwarded.PropStatName, ref this.statName);
		properties.ParseString(ObjectiveStatAwarded.PropStatText, ref this.statText);
		if (properties.Contains(ObjectiveStatAwarded.PropNeededCount))
		{
			int num = 0;
			properties.ParseInt(ObjectiveStatAwarded.PropNeededCount, ref num);
			this.Value = num.ToString();
		}
		properties.ParseString(ObjectiveStatAwarded.PropStatTextKey, ref text);
		if (text != "")
		{
			this.statText = Localization.Get(text, false);
		}
	}

	// Token: 0x040034CF RID: 13519
	[PublicizedFrom(EAccessModifier.Private)]
	public string statName = "";

	// Token: 0x040034D0 RID: 13520
	[PublicizedFrom(EAccessModifier.Private)]
	public string statText = "";

	// Token: 0x040034D1 RID: 13521
	[PublicizedFrom(EAccessModifier.Private)]
	public int neededCount;

	// Token: 0x040034D2 RID: 13522
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStatName = "stat_name";

	// Token: 0x040034D3 RID: 13523
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStatText = "stat_text";

	// Token: 0x040034D4 RID: 13524
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropStatTextKey = "stat_text_key";

	// Token: 0x040034D5 RID: 13525
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropNeededCount = "count";
}
