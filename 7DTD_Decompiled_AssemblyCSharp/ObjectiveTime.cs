using System;
using System.Globalization;
using System.IO;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x020008DC RID: 2268
[Preserve]
public class ObjectiveTime : BaseObjective
{
	// Token: 0x170006F6 RID: 1782
	// (get) Token: 0x060042CE RID: 17102 RVA: 0x000282C0 File Offset: 0x000264C0
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Time;
		}
	}

	// Token: 0x170006F7 RID: 1783
	// (get) Token: 0x060042CF RID: 17103 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool UpdateUI
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170006F8 RID: 1784
	// (get) Token: 0x060042D0 RID: 17104 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ShowInQuestLog
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170006F9 RID: 1785
	// (get) Token: 0x060042D1 RID: 17105 RVA: 0x001AED48 File Offset: 0x001ACF48
	public override string StatusText
	{
		get
		{
			if (this.currentTime > 0f)
			{
				return XUiM_PlayerBuffs.GetTimeString(this.currentTime);
			}
			if (base.Optional)
			{
				base.ObjectiveState = BaseObjective.ObjectiveStates.Failed;
				return Localization.Get("failed", false);
			}
			base.ObjectiveState = BaseObjective.ObjectiveStates.Complete;
			return Localization.Get("completed", false);
		}
	}

	// Token: 0x060042D2 RID: 17106 RVA: 0x001AED9B File Offset: 0x001ACF9B
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveTime_keyword", false);
		this.dayLengthInSeconds = GamePrefs.GetInt(EnumGamePrefs.DayNightLength) * 60;
	}

	// Token: 0x060042D3 RID: 17107 RVA: 0x001AEDBE File Offset: 0x001ACFBE
	public override void SetupDisplay()
	{
		base.Description = string.Format("{0}:", this.keyword);
	}

	// Token: 0x060042D4 RID: 17108 RVA: 0x001AB071 File Offset: 0x001A9271
	public override void AddHooks()
	{
		QuestEventManager.Current.AddObjectiveToBeUpdated(this);
	}

	// Token: 0x060042D5 RID: 17109 RVA: 0x001ACF00 File Offset: 0x001AB100
	public override void RemoveHooks()
	{
		QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
	}

	// Token: 0x060042D6 RID: 17110 RVA: 0x001AEDD8 File Offset: 0x001ACFD8
	public override void Refresh()
	{
		this.SetupDisplay();
		if (base.Optional)
		{
			base.Complete = (this.currentTime > 0f);
		}
		else
		{
			base.Complete = (this.currentTime <= 0f);
		}
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x060042D7 RID: 17111 RVA: 0x001AEE35 File Offset: 0x001AD035
	public override void Read(BinaryReader _br)
	{
		this.currentTime = (float)_br.ReadUInt16();
		this.currentValue = 1;
	}

	// Token: 0x060042D8 RID: 17112 RVA: 0x001AEE4B File Offset: 0x001AD04B
	public override void Write(BinaryWriter _bw)
	{
		_bw.Write((ushort)this.currentTime);
	}

	// Token: 0x060042D9 RID: 17113 RVA: 0x001AEE5A File Offset: 0x001AD05A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CopyValues(BaseObjective objective)
	{
		base.CopyValues(objective);
		ObjectiveTime objectiveTime = (ObjectiveTime)objective;
		objectiveTime.currentTime = this.currentTime;
		objectiveTime.overrideType = this.overrideType;
		objectiveTime.overrideOffset = this.overrideOffset;
	}

	// Token: 0x060042DA RID: 17114 RVA: 0x001AEE8C File Offset: 0x001AD08C
	public override BaseObjective Clone()
	{
		ObjectiveTime objectiveTime = new ObjectiveTime();
		this.CopyValues(objectiveTime);
		return objectiveTime;
	}

	// Token: 0x060042DB RID: 17115 RVA: 0x001AEEA8 File Offset: 0x001AD0A8
	public override void Update(float updateTime)
	{
		if (this.firstRun)
		{
			if (this.overrideType == ObjectiveTime.OverrideTypes.VoteTime && TwitchManager.HasInstance && TwitchManager.Current.IsVoting)
			{
				this.currentTime = TwitchManager.Current.VotingManager.VoteTimeRemaining + this.overrideOffset;
				this.firstRun = false;
				return;
			}
			if (this.Value.EqualsCaseInsensitive("day"))
			{
				this.currentTime = (float)this.dayLengthInSeconds;
			}
			else
			{
				this.currentTime = StringParsers.ParseFloat(this.Value, 0, -1, NumberStyles.Any);
			}
			this.firstRun = false;
		}
		this.currentTime -= updateTime;
		if (this.currentTime < 0f)
		{
			this.Refresh();
			base.HandleRemoveHooks();
		}
	}

	// Token: 0x060042DC RID: 17116 RVA: 0x001AEF67 File Offset: 0x001AD167
	public override void HandleFailed()
	{
		this.currentTime = 0f;
		base.Complete = false;
	}

	// Token: 0x060042DD RID: 17117 RVA: 0x001AEF7B File Offset: 0x001AD17B
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(ObjectiveTime.PropTime, ref this.Value);
		properties.ParseEnum<ObjectiveTime.OverrideTypes>(ObjectiveTime.PropOverrideType, ref this.overrideType);
		properties.ParseFloat(ObjectiveTime.PropOverrideOffset, ref this.overrideOffset);
	}

	// Token: 0x040034DB RID: 13531
	[PublicizedFrom(EAccessModifier.Private)]
	public float currentTime;

	// Token: 0x040034DC RID: 13532
	[PublicizedFrom(EAccessModifier.Private)]
	public ObjectiveTime.OverrideTypes overrideType;

	// Token: 0x040034DD RID: 13533
	[PublicizedFrom(EAccessModifier.Private)]
	public float overrideOffset;

	// Token: 0x040034DE RID: 13534
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstRun = true;

	// Token: 0x040034DF RID: 13535
	public static string PropTime = "time";

	// Token: 0x040034E0 RID: 13536
	public static string PropOverrideType = "override_type";

	// Token: 0x040034E1 RID: 13537
	public static string PropOverrideOffset = "override_offset";

	// Token: 0x040034E2 RID: 13538
	[PublicizedFrom(EAccessModifier.Private)]
	public int dayLengthInSeconds;

	// Token: 0x020008DD RID: 2269
	[PublicizedFrom(EAccessModifier.Private)]
	public enum OverrideTypes
	{
		// Token: 0x040034E4 RID: 13540
		None,
		// Token: 0x040034E5 RID: 13541
		VoteTime
	}
}
