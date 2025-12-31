using System;
using UnityEngine.Scripting;

// Token: 0x02000894 RID: 2196
[Preserve]
public class QuestActionGameEvent : BaseQuestAction
{
	// Token: 0x06004024 RID: 16420 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupAction()
	{
	}

	// Token: 0x06004025 RID: 16421 RVA: 0x001A3C7C File Offset: 0x001A1E7C
	public override void PerformAction(Quest ownerQuest)
	{
		GameEventManager.Current.HandleAction(this.ID, ownerQuest.OwnerJournal.OwnerPlayer, ownerQuest.OwnerJournal.OwnerPlayer, false, "", "", false, true, "", null);
	}

	// Token: 0x06004026 RID: 16422 RVA: 0x001A3CC4 File Offset: 0x001A1EC4
	public override BaseQuestAction Clone()
	{
		QuestActionGameEvent questActionGameEvent = new QuestActionGameEvent();
		base.CopyValues(questActionGameEvent);
		return questActionGameEvent;
	}

	// Token: 0x06004027 RID: 16423 RVA: 0x001A3CDF File Offset: 0x001A1EDF
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(QuestActionGameEvent.PropEventName, ref this.ID);
	}

	// Token: 0x0400337E RID: 13182
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropEventName = "event";
}
