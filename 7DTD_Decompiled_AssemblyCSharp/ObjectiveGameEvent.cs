using System;
using UnityEngine.Scripting;

// Token: 0x020008C4 RID: 2244
[Preserve]
public class ObjectiveGameEvent : BaseObjective
{
	// Token: 0x170006D1 RID: 1745
	// (get) Token: 0x060041C5 RID: 16837 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool useUpdateLoop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return true;
		}
	}

	// Token: 0x060041C6 RID: 16838 RVA: 0x001A92BD File Offset: 0x001A74BD
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveAssemble_keyword", false);
	}

	// Token: 0x060041C7 RID: 16839 RVA: 0x001A92D0 File Offset: 0x001A74D0
	public override void SetupDisplay()
	{
		base.Description = "Test Game Event";
		this.StatusText = "";
	}

	// Token: 0x060041C8 RID: 16840 RVA: 0x001A92E8 File Offset: 0x001A74E8
	public override void AddHooks()
	{
		GameEventManager gameEventManager = GameEventManager.Current;
		gameEventManager.GameEventCompleted += this.Current_GameEventCompleted;
		gameEventManager.GameEventDenied += this.Current_GameEventDenied;
	}

	// Token: 0x060041C9 RID: 16841 RVA: 0x001A9312 File Offset: 0x001A7512
	public override void RemoveHooks()
	{
		GameEventManager gameEventManager = GameEventManager.Current;
		gameEventManager.GameEventCompleted -= this.Current_GameEventCompleted;
		gameEventManager.GameEventDenied -= this.Current_GameEventDenied;
	}

	// Token: 0x060041CA RID: 16842 RVA: 0x001A933C File Offset: 0x001A753C
	public override void Update(float updateTime)
	{
		switch (this.GameEventState)
		{
		case ObjectiveGameEvent.GameEventStates.Start:
		{
			EntityPlayer ownerPlayer = base.OwnerQuest.OwnerJournal.OwnerPlayer;
			GameEventManager.Current.HandleAction(this.gameEventID, ownerPlayer, ownerPlayer, false, "", this.gameEventTag, false, true, "", null);
			this.GameEventState = ObjectiveGameEvent.GameEventStates.Waiting;
			break;
		}
		case ObjectiveGameEvent.GameEventStates.Waiting:
		case ObjectiveGameEvent.GameEventStates.Complete:
			break;
		default:
			return;
		}
	}

	// Token: 0x060041CB RID: 16843 RVA: 0x001A93A4 File Offset: 0x001A75A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_GameEventCompleted(string _gameEventID, int _targetEntityID, string _extraData, string _tag)
	{
		if (this.gameEventID == _gameEventID && _tag == this.gameEventTag && _targetEntityID == base.OwnerQuest.OwnerJournal.OwnerPlayer.entityId && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue = 1;
			this.Refresh();
		}
	}

	// Token: 0x060041CC RID: 16844 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_GameEventDenied(string gameEventID, int targetEntityID, string extraData, string tag)
	{
	}

	// Token: 0x060041CD RID: 16845 RVA: 0x001A9400 File Offset: 0x001A7600
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		base.Complete = (base.CurrentValue == 1);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x060041CE RID: 16846 RVA: 0x001A9434 File Offset: 0x001A7634
	public override BaseObjective Clone()
	{
		ObjectiveGameEvent objectiveGameEvent = new ObjectiveGameEvent();
		this.CopyValues(objectiveGameEvent);
		objectiveGameEvent.gameEventID = this.gameEventID;
		objectiveGameEvent.gameEventTag = this.gameEventTag;
		return objectiveGameEvent;
	}

	// Token: 0x060041CF RID: 16847 RVA: 0x001A9467 File Offset: 0x001A7667
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(ObjectiveGameEvent.PropGameEventID, ref this.gameEventID);
		properties.ParseString(ObjectiveGameEvent.PropGameEventTag, ref this.gameEventTag);
	}

	// Token: 0x04003451 RID: 13393
	[PublicizedFrom(EAccessModifier.Protected)]
	public string gameEventID = "";

	// Token: 0x04003452 RID: 13394
	[PublicizedFrom(EAccessModifier.Protected)]
	public string gameEventTag = "quest";

	// Token: 0x04003453 RID: 13395
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGameEventID = "event";

	// Token: 0x04003454 RID: 13396
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGameEventTag = "event_tag";

	// Token: 0x04003455 RID: 13397
	[PublicizedFrom(EAccessModifier.Protected)]
	public ObjectiveGameEvent.GameEventStates GameEventState;

	// Token: 0x020008C5 RID: 2245
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum GameEventStates
	{
		// Token: 0x04003457 RID: 13399
		Start,
		// Token: 0x04003458 RID: 13400
		Waiting,
		// Token: 0x04003459 RID: 13401
		Complete
	}
}
