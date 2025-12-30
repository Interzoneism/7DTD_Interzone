using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008DB RID: 2267
[Preserve]
public class ObjectiveStayWithin : BaseObjective
{
	// Token: 0x170006F3 RID: 1779
	// (get) Token: 0x060042BE RID: 17086 RVA: 0x00075C39 File Offset: 0x00073E39
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Distance;
		}
	}

	// Token: 0x170006F4 RID: 1780
	// (get) Token: 0x060042BF RID: 17087 RVA: 0x001A6B33 File Offset: 0x001A4D33
	public override bool UpdateUI
	{
		get
		{
			return base.ObjectiveState != BaseObjective.ObjectiveStates.Failed;
		}
	}

	// Token: 0x170006F5 RID: 1781
	// (get) Token: 0x060042C0 RID: 17088 RVA: 0x001AEA5C File Offset: 0x001ACC5C
	public override string StatusText
	{
		get
		{
			if (base.OwnerQuest.CurrentState == Quest.QuestState.InProgress)
			{
				if (this.currentDistance < this.maxDistance)
				{
					return ValueDisplayFormatters.Distance(this.currentDistance) + "/" + this.displayDistance;
				}
				base.ObjectiveState = BaseObjective.ObjectiveStates.Failed;
				return Localization.Get("failed", false);
			}
			else
			{
				if (base.OwnerQuest.CurrentState == Quest.QuestState.NotStarted)
				{
					return this.displayDistance;
				}
				if (base.ObjectiveState == BaseObjective.ObjectiveStates.Failed)
				{
					return Localization.Get("failed", false);
				}
				return Localization.Get("completed", false);
			}
		}
	}

	// Token: 0x060042C1 RID: 17089 RVA: 0x001AB046 File Offset: 0x001A9246
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveStayWithin_keyword", false);
	}

	// Token: 0x060042C2 RID: 17090 RVA: 0x001A6FF7 File Offset: 0x001A51F7
	public override void SetupDisplay()
	{
		base.Description = this.keyword;
	}

	// Token: 0x060042C3 RID: 17091 RVA: 0x001AB071 File Offset: 0x001A9271
	public override void AddHooks()
	{
		QuestEventManager.Current.AddObjectiveToBeUpdated(this);
	}

	// Token: 0x060042C4 RID: 17092 RVA: 0x001AEAE8 File Offset: 0x001ACCE8
	public override void RemoveHooks()
	{
		QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
		if (base.OwnerQuest == base.OwnerQuest.OwnerJournal.ActiveQuest || base.OwnerQuest.OwnerJournal.ActiveQuest == null)
		{
			QuestEventManager.Current.QuestBounds = default(Rect);
		}
	}

	// Token: 0x060042C5 RID: 17093 RVA: 0x001AB0D9 File Offset: 0x001A92D9
	public override void Refresh()
	{
		this.SetupDisplay();
		if (base.ObjectiveState == BaseObjective.ObjectiveStates.NotStarted && base.OwnerQuest.CurrentState == Quest.QuestState.InProgress)
		{
			return;
		}
		base.Complete = (base.OwnerQuest.CurrentState != Quest.QuestState.Failed);
	}

	// Token: 0x060042C6 RID: 17094 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Read(BinaryReader _br)
	{
	}

	// Token: 0x060042C7 RID: 17095 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Write(BinaryWriter _bw)
	{
	}

	// Token: 0x060042C8 RID: 17096 RVA: 0x001AEB3A File Offset: 0x001ACD3A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CopyValues(BaseObjective objective)
	{
		base.CopyValues(objective);
		ObjectiveStayWithin objectiveStayWithin = (ObjectiveStayWithin)objective;
		objectiveStayWithin.maxDistance = this.maxDistance;
		objectiveStayWithin.displayDistance = this.displayDistance;
	}

	// Token: 0x060042C9 RID: 17097 RVA: 0x001AEB60 File Offset: 0x001ACD60
	public override BaseObjective Clone()
	{
		ObjectiveStayWithin objectiveStayWithin = new ObjectiveStayWithin();
		this.CopyValues(objectiveStayWithin);
		return objectiveStayWithin;
	}

	// Token: 0x060042CA RID: 17098 RVA: 0x001AEB7C File Offset: 0x001ACD7C
	public override void Update(float updateTime)
	{
		Vector3 position = base.OwnerQuest.OwnerJournal.OwnerPlayer.position;
		Vector3 position2 = base.OwnerQuest.Position;
		if (!this.positionSetup)
		{
			if (base.OwnerQuest.GetPositionData(out position2, Quest.PositionDataTypes.Location))
			{
				base.OwnerQuest.Position = position2;
				QuestEventManager.Current.QuestBounds = new Rect(position2.x, position2.z, this.maxDistance, this.maxDistance);
				this.positionSetup = true;
			}
			else if (base.OwnerQuest.GetPositionData(out position2, Quest.PositionDataTypes.POIPosition))
			{
				base.OwnerQuest.Position = position2;
				QuestEventManager.Current.QuestBounds = new Rect(position2.x, position2.z, this.maxDistance, this.maxDistance);
				this.positionSetup = true;
			}
		}
		position.y = 0f;
		position2.y = 0f;
		this.currentDistance = (position - position2).magnitude;
		float num = this.currentDistance / this.maxDistance;
		if (num > 1f)
		{
			base.Complete = false;
			base.ObjectiveState = BaseObjective.ObjectiveStates.Failed;
			base.OwnerQuest.CloseQuest(Quest.QuestState.Failed, null);
			return;
		}
		if (num > 0.75f)
		{
			base.ObjectiveState = BaseObjective.ObjectiveStates.Warning;
			return;
		}
		base.ObjectiveState = BaseObjective.ObjectiveStates.Complete;
	}

	// Token: 0x060042CB RID: 17099 RVA: 0x001AECC4 File Offset: 0x001ACEC4
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveStayWithin.PropRadius))
		{
			this.maxDistance = StringParsers.ParseFloat(properties.Values[ObjectiveStayWithin.PropRadius], 0, -1, NumberStyles.Any);
			this.displayDistance = ValueDisplayFormatters.Distance(this.maxDistance);
		}
	}

	// Token: 0x040034D6 RID: 13526
	[PublicizedFrom(EAccessModifier.Private)]
	public float maxDistance = 50f;

	// Token: 0x040034D7 RID: 13527
	[PublicizedFrom(EAccessModifier.Private)]
	public float currentDistance;

	// Token: 0x040034D8 RID: 13528
	[PublicizedFrom(EAccessModifier.Private)]
	public string displayDistance = "0 km";

	// Token: 0x040034D9 RID: 13529
	public static string PropRadius = "radius";

	// Token: 0x040034DA RID: 13530
	[PublicizedFrom(EAccessModifier.Private)]
	public bool positionSetup;
}
