using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008B8 RID: 2232
[Preserve]
public class ObjectiveClearSleepers : BaseObjective
{
	// Token: 0x170006C4 RID: 1732
	// (get) Token: 0x06004135 RID: 16693 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Boolean;
		}
	}

	// Token: 0x170006C5 RID: 1733
	// (get) Token: 0x06004136 RID: 16694 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool RequiresZombies
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06004137 RID: 16695 RVA: 0x001A6B08 File Offset: 0x001A4D08
	public override void SetupQuestTag()
	{
		base.OwnerQuest.AddQuestTag(QuestEventManager.clearTag);
	}

	// Token: 0x06004138 RID: 16696 RVA: 0x001A6B1A File Offset: 0x001A4D1A
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveClearAreas_keyword", false);
		this.SetupIcon();
	}

	// Token: 0x170006C6 RID: 1734
	// (get) Token: 0x06004139 RID: 16697 RVA: 0x001A6B33 File Offset: 0x001A4D33
	public override bool UpdateUI
	{
		get
		{
			return base.ObjectiveState != BaseObjective.ObjectiveStates.Failed;
		}
	}

	// Token: 0x0600413A RID: 16698 RVA: 0x001A6B41 File Offset: 0x001A4D41
	public override void SetupDisplay()
	{
		base.Description = this.keyword;
		this.StatusText = "";
	}

	// Token: 0x0600413B RID: 16699 RVA: 0x001A6B5C File Offset: 0x001A4D5C
	public override void AddHooks()
	{
		this.GetPosition();
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition);
		base.OwnerQuest.GetPositionData(out zero2, Quest.PositionDataTypes.POISize);
		QuestEventManager.Current.SleepersCleared += this.Current_SleepersCleared;
		QuestEventManager.Current.SleeperVolumePositionAdd += this.Current_SleeperVolumePositionAdd;
		QuestEventManager.Current.SleeperVolumePositionRemove += this.Current_SleeperVolumePositionRemove;
		QuestEventManager.Current.SubscribeToUpdateEvent(base.OwnerQuest.OwnerJournal.OwnerPlayer.entityId, zero);
		this.SetupZombieCompassBounds(zero, zero2);
	}

	// Token: 0x0600413C RID: 16700 RVA: 0x001A6C04 File Offset: 0x001A4E04
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_SleeperVolumePositionAdd(Vector3 position)
	{
		if (this.NavObjectName == "")
		{
			if (!this.SleeperMapObjectList.ContainsKey(position))
			{
				MapObjectSleeperVolume mapObjectSleeperVolume = new MapObjectSleeperVolume(position);
				GameManager.Instance.World.ObjectOnMapAdd(mapObjectSleeperVolume);
				this.SleeperMapObjectList.Add(position, mapObjectSleeperVolume);
				return;
			}
		}
		else if (!this.SleeperNavObjectList.ContainsKey(position))
		{
			NavObject value = NavObjectManager.Instance.RegisterNavObject(this.NavObjectName, position, "", false, -1, null);
			this.SleeperNavObjectList.Add(position, value);
		}
	}

	// Token: 0x0600413D RID: 16701 RVA: 0x001A6C8C File Offset: 0x001A4E8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_SleeperVolumePositionRemove(Vector3 position)
	{
		if (this.NavObjectName == "")
		{
			if (this.SleeperMapObjectList.ContainsKey(position))
			{
				MapObject mapObject = this.SleeperMapObjectList[position];
				GameManager.Instance.World.ObjectOnMapRemove(mapObject.type, (int)mapObject.key);
				this.SleeperMapObjectList.Remove(position);
				return;
			}
		}
		else if (this.SleeperNavObjectList.ContainsKey(position))
		{
			NavObject navObject = this.SleeperNavObjectList[position];
			NavObjectManager.Instance.UnRegisterNavObject(navObject);
			this.SleeperNavObjectList.Remove(position);
		}
	}

	// Token: 0x0600413E RID: 16702 RVA: 0x001A6D24 File Offset: 0x001A4F24
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemoveSleeperVolumeMapObjects()
	{
		if (this.NavObjectName == "")
		{
			GameManager.Instance.World.ObjectOnMapRemove(EnumMapObjectType.SleeperVolume);
			this.SleeperMapObjectList.Clear();
			return;
		}
		NavObjectManager.Instance.UnRegisterNavObjectByClass(this.NavObjectName);
		this.SleeperNavObjectList.Clear();
	}

	// Token: 0x0600413F RID: 16703 RVA: 0x001A6D7B File Offset: 0x001A4F7B
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupZombieCompassBounds(Vector3 poiPos, Vector3 poiSize)
	{
		base.OwnerQuest.OwnerJournal.OwnerPlayer.ZombieCompassBounds = new Rect(poiPos.x, poiPos.z, poiSize.x, poiSize.z);
	}

	// Token: 0x06004140 RID: 16704 RVA: 0x001A6DB0 File Offset: 0x001A4FB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_SleepersCleared(Vector3 prefabPos)
	{
		Vector3 zero = Vector3.zero;
		base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition);
		if (zero.x != prefabPos.x || zero.z != prefabPos.z)
		{
			return;
		}
		if (base.OwnerQuest.CheckRequirements())
		{
			base.Complete = true;
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004141 RID: 16705 RVA: 0x001A6E14 File Offset: 0x001A5014
	public override void RemoveHooks()
	{
		QuestEventManager.Current.SleepersCleared -= this.Current_SleepersCleared;
		Vector3 zero = Vector3.zero;
		base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition);
		QuestEventManager.Current.UnSubscribeToUpdateEvent(base.OwnerQuest.OwnerJournal.OwnerPlayer.entityId, zero);
		if (base.OwnerQuest.OwnerJournal.ActiveQuest == base.OwnerQuest)
		{
			base.OwnerQuest.OwnerJournal.OwnerPlayer.ZombieCompassBounds = default(Rect);
		}
		this.RemoveSleeperVolumeMapObjects();
	}

	// Token: 0x06004142 RID: 16706 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupIcon()
	{
	}

	// Token: 0x06004143 RID: 16707 RVA: 0x001A6EA5 File Offset: 0x001A50A5
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetPosition()
	{
		if (base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.POIPosition))
		{
			base.OwnerQuest.Position = this.position;
		}
		return Vector3.zero;
	}

	// Token: 0x06004144 RID: 16708 RVA: 0x001A6ED4 File Offset: 0x001A50D4
	public void FinalizePoint(float offset, float x, float y, float z)
	{
		this.distanceOffset = offset;
		this.position = new Vector3(x, y, z);
		base.OwnerQuest.DataVariables.Add(this.locationVariable, string.Format("{0},{1},{2},{3}", new object[]
		{
			offset.ToCultureInvariantString(),
			x.ToCultureInvariantString(),
			y.ToCultureInvariantString(),
			z.ToCultureInvariantString()
		}));
		base.OwnerQuest.Position = this.position;
		base.CurrentValue = 1;
	}

	// Token: 0x06004145 RID: 16709 RVA: 0x001A6F5A File Offset: 0x001A515A
	public override void Refresh()
	{
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004146 RID: 16710 RVA: 0x001A6F74 File Offset: 0x001A5174
	public override BaseObjective Clone()
	{
		ObjectiveClearSleepers objectiveClearSleepers = new ObjectiveClearSleepers();
		this.CopyValues(objectiveClearSleepers);
		return objectiveClearSleepers;
	}

	// Token: 0x06004147 RID: 16711 RVA: 0x001A6F8F File Offset: 0x001A518F
	public override bool SetLocation(Vector3 pos, Vector3 size)
	{
		this.FinalizePoint(this.distanceOffset, pos.x, pos.y, pos.z);
		return true;
	}

	// Token: 0x0400341B RID: 13339
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position;

	// Token: 0x0400341C RID: 13340
	[PublicizedFrom(EAccessModifier.Private)]
	public float distanceOffset;

	// Token: 0x0400341D RID: 13341
	[PublicizedFrom(EAccessModifier.Private)]
	public string icon = "ui_game_symbol_quest";

	// Token: 0x0400341E RID: 13342
	[PublicizedFrom(EAccessModifier.Private)]
	public string locationVariable = "gotolocation";

	// Token: 0x0400341F RID: 13343
	public Dictionary<Vector3, MapObjectSleeperVolume> SleeperMapObjectList = new Dictionary<Vector3, MapObjectSleeperVolume>();

	// Token: 0x04003420 RID: 13344
	public Dictionary<Vector3, NavObject> SleeperNavObjectList = new Dictionary<Vector3, NavObject>();

	// Token: 0x020008B9 RID: 2233
	[PublicizedFrom(EAccessModifier.Private)]
	public enum GotoStates
	{
		// Token: 0x04003422 RID: 13346
		NoPosition,
		// Token: 0x04003423 RID: 13347
		TryRefresh,
		// Token: 0x04003424 RID: 13348
		TryComplete,
		// Token: 0x04003425 RID: 13349
		Completed
	}
}
