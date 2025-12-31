using System;
using System.Collections;
using System.Globalization;
using UnityEngine;

// Token: 0x02000892 RID: 2194
public abstract class BaseQuestAction
{
	// Token: 0x1700069D RID: 1693
	// (get) Token: 0x0600400A RID: 16394 RVA: 0x001A3990 File Offset: 0x001A1B90
	// (set) Token: 0x0600400B RID: 16395 RVA: 0x001A3998 File Offset: 0x001A1B98
	public Quest OwnerQuest { get; set; }

	// Token: 0x1700069E RID: 1694
	// (get) Token: 0x0600400C RID: 16396 RVA: 0x001A39A1 File Offset: 0x001A1BA1
	// (set) Token: 0x0600400D RID: 16397 RVA: 0x001A39A9 File Offset: 0x001A1BA9
	public QuestClass Owner { get; set; }

	// Token: 0x1700069F RID: 1695
	// (get) Token: 0x0600400E RID: 16398 RVA: 0x001A39B2 File Offset: 0x001A1BB2
	// (set) Token: 0x0600400F RID: 16399 RVA: 0x001A39BA File Offset: 0x001A1BBA
	public int Phase { get; set; }

	// Token: 0x170006A0 RID: 1696
	// (get) Token: 0x06004010 RID: 16400 RVA: 0x001A39C3 File Offset: 0x001A1BC3
	// (set) Token: 0x06004011 RID: 16401 RVA: 0x001A39CB File Offset: 0x001A1BCB
	public float Delay { get; set; }

	// Token: 0x170006A1 RID: 1697
	// (get) Token: 0x06004012 RID: 16402 RVA: 0x001A39D4 File Offset: 0x001A1BD4
	// (set) Token: 0x06004013 RID: 16403 RVA: 0x001A39DC File Offset: 0x001A1BDC
	public bool OnComplete { get; set; }

	// Token: 0x06004014 RID: 16404 RVA: 0x001A39E5 File Offset: 0x001A1BE5
	public BaseQuestAction()
	{
		this.Phase = 1;
	}

	// Token: 0x06004015 RID: 16405 RVA: 0x001A39F4 File Offset: 0x001A1BF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CopyValues(BaseQuestAction action)
	{
		action.ID = this.ID;
		action.Value = this.Value;
		action.Phase = this.Phase;
		action.Delay = this.Delay;
		action.OnComplete = this.OnComplete;
	}

	// Token: 0x06004016 RID: 16406 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupAction()
	{
	}

	// Token: 0x06004017 RID: 16407 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void PerformAction(Quest ownerQuest)
	{
	}

	// Token: 0x06004018 RID: 16408 RVA: 0x001A3A32 File Offset: 0x001A1C32
	public void HandlePerformAction()
	{
		if (this.Delay == 0f)
		{
			this.PerformAction(this.OwnerQuest);
			return;
		}
		GameManager.Instance.StartCoroutine(this.PerformActionLater());
	}

	// Token: 0x06004019 RID: 16409 RVA: 0x001A3A5F File Offset: 0x001A1C5F
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator PerformActionLater()
	{
		yield return new WaitForSeconds(this.Delay);
		if (XUi.IsGameRunning())
		{
			this.PerformAction(this.OwnerQuest);
		}
		yield break;
	}

	// Token: 0x0600401A RID: 16410 RVA: 0x001A3A6E File Offset: 0x001A1C6E
	public virtual void HandleVariables()
	{
		this.ID = this.OwnerQuest.ParseVariable(this.ID);
		this.Value = this.OwnerQuest.ParseVariable(this.Value);
	}

	// Token: 0x0600401B RID: 16411 RVA: 0x00019766 File Offset: 0x00017966
	public virtual BaseQuestAction Clone()
	{
		return null;
	}

	// Token: 0x0600401C RID: 16412 RVA: 0x001A3AA0 File Offset: 0x001A1CA0
	public virtual void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		this.Owner.HandleVariablesForProperties(properties);
		if (properties.Values.ContainsKey(BaseQuestAction.PropID))
		{
			this.ID = properties.Values[BaseQuestAction.PropID];
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropValue))
		{
			this.Value = properties.Values[BaseQuestAction.PropValue];
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropPhase))
		{
			this.Phase = (int)Convert.ToByte(properties.Values[BaseQuestAction.PropPhase]);
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropPhase))
		{
			this.Phase = (int)Convert.ToByte(properties.Values[BaseQuestAction.PropPhase]);
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropDelay))
		{
			this.Delay = StringParsers.ParseFloat(properties.Values[BaseQuestAction.PropDelay], 0, -1, NumberStyles.Any);
		}
		if (properties.Values.ContainsKey(BaseQuestAction.PropOnComplete))
		{
			this.OnComplete = StringParsers.ParseBool(properties.Values[BaseQuestAction.PropOnComplete], 0, -1, true);
		}
	}

	// Token: 0x0400336E RID: 13166
	public static string PropID = "id";

	// Token: 0x0400336F RID: 13167
	public static string PropValue = "value";

	// Token: 0x04003370 RID: 13168
	public static string PropPhase = "phase";

	// Token: 0x04003371 RID: 13169
	public static string PropDelay = "delay";

	// Token: 0x04003372 RID: 13170
	public static string PropOnComplete = "on_complete";

	// Token: 0x04003373 RID: 13171
	public string ID;

	// Token: 0x04003374 RID: 13172
	public string Value;

	// Token: 0x0400337A RID: 13178
	public DynamicProperties Properties;
}
