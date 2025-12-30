using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008D7 RID: 2263
[Preserve]
public class ObjectiveReturnToNPC : ObjectiveRandomGoto
{
	// Token: 0x06004297 RID: 17047 RVA: 0x001AE3F3 File Offset: 0x001AC5F3
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveReturnToTrader_keyword", false) + ((base.OwnerQuest.CurrentState == Quest.QuestState.NotStarted) ? "" : ":");
		this.completeWithinRange = false;
	}

	// Token: 0x06004298 RID: 17048 RVA: 0x001AE42B File Offset: 0x001AC62B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetupIcon()
	{
		this.icon = "ui_game_symbol_quest";
	}

	// Token: 0x170006EF RID: 1775
	// (get) Token: 0x06004299 RID: 17049 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool PlayObjectiveComplete
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600429A RID: 17050 RVA: 0x001AE438 File Offset: 0x001AC638
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 GetPosition()
	{
		if (base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.QuestGiver))
		{
			base.OwnerQuest.Position = this.position;
			this.positionSet = true;
			base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.QuestGiver, this.NavObjectName, -1);
			base.CurrentValue = 2;
			return this.position;
		}
		base.CurrentValue = 3;
		base.Complete = true;
		base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		this.positionSet = true;
		QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
		this.HiddenObjective = true;
		base.OwnerQuest.RemoveMapObject();
		return Vector3.zero;
	}

	// Token: 0x0600429B RID: 17051 RVA: 0x001AE4D8 File Offset: 0x001AC6D8
	public override void OnStart()
	{
		base.OnStart();
		if (base.OwnerQuest.QuestClass.AddsToTierComplete)
		{
			base.OwnerQuest.OwnerJournal.HandleQuestCompleteToday(base.OwnerQuest);
		}
	}

	// Token: 0x0600429C RID: 17052 RVA: 0x001AE508 File Offset: 0x001AC708
	public override BaseObjective Clone()
	{
		ObjectiveReturnToNPC objectiveReturnToNPC = new ObjectiveReturnToNPC();
		this.CopyValues(objectiveReturnToNPC);
		return objectiveReturnToNPC;
	}
}
