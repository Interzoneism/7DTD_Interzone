using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020008A6 RID: 2214
[Preserve]
public class QuestEvent
{
	// Token: 0x06004087 RID: 16519 RVA: 0x001A4C4A File Offset: 0x001A2E4A
	public QuestEvent(string type)
	{
		this.EventType = type;
	}

	// Token: 0x06004088 RID: 16520 RVA: 0x001A4C70 File Offset: 0x001A2E70
	public void HandleEvent(Quest quest)
	{
		if (this.IsServerOnly && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (GameManager.Instance.World.GetGameRandom().RandomFloat < this.Chance)
		{
			for (int i = 0; i < this.Actions.Count; i++)
			{
				this.Actions[i].PerformAction(quest);
			}
		}
	}

	// Token: 0x06004089 RID: 16521 RVA: 0x001A4CD6 File Offset: 0x001A2ED6
	public virtual void ParseProperties(DynamicProperties properties)
	{
		this.Properties = properties;
		this.Owner.HandleVariablesForProperties(properties);
		properties.ParseFloat(QuestEvent.PropChance, ref this.Chance);
		properties.ParseBool(QuestEvent.PropServerOnly, ref this.IsServerOnly);
	}

	// Token: 0x0600408A RID: 16522 RVA: 0x001A4D10 File Offset: 0x001A2F10
	public QuestEvent Clone()
	{
		QuestEvent questEvent = new QuestEvent(this.EventType);
		questEvent.Chance = this.Chance;
		questEvent.IsServerOnly = this.IsServerOnly;
		if (this.Actions != null)
		{
			for (int i = 0; i < this.Actions.Count; i++)
			{
				BaseQuestAction baseQuestAction = this.Actions[i].Clone();
				baseQuestAction.Properties = new DynamicProperties();
				baseQuestAction.Owner = this.Owner;
				if (this.Actions[i].Properties != null)
				{
					baseQuestAction.Properties.CopyFrom(this.Actions[i].Properties, null);
				}
				questEvent.Actions.Add(baseQuestAction);
			}
		}
		return questEvent;
	}

	// Token: 0x040033A6 RID: 13222
	public string EventType;

	// Token: 0x040033A7 RID: 13223
	public float Chance = 1f;

	// Token: 0x040033A8 RID: 13224
	public List<BaseQuestAction> Actions = new List<BaseQuestAction>();

	// Token: 0x040033A9 RID: 13225
	public bool IsServerOnly;

	// Token: 0x040033AA RID: 13226
	public static string PropChance = "chance";

	// Token: 0x040033AB RID: 13227
	public static string PropServerOnly = "server_only";

	// Token: 0x040033AC RID: 13228
	public QuestClass Owner;

	// Token: 0x040033AD RID: 13229
	public DynamicProperties Properties;
}
