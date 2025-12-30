using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Audio;
using Quests;
using Quests.Requirements;
using UnityEngine;

// Token: 0x020008E5 RID: 2277
public class Quest
{
	// Token: 0x17000700 RID: 1792
	// (get) Token: 0x06004319 RID: 17177 RVA: 0x001B1557 File Offset: 0x001AF757
	// (set) Token: 0x0600431A RID: 17178 RVA: 0x001B155F File Offset: 0x001AF75F
	public Quest.QuestState CurrentState
	{
		get
		{
			return this._currentState;
		}
		set
		{
			if (this._currentState != value)
			{
				this._currentState = value;
				PrefabInstance.RefreshSwitchesInContainingPoi(this);
			}
		}
	}

	// Token: 0x17000701 RID: 1793
	// (get) Token: 0x0600431B RID: 17179 RVA: 0x001B1577 File Offset: 0x001AF777
	// (set) Token: 0x0600431C RID: 17180 RVA: 0x001B157F File Offset: 0x001AF77F
	public string ID { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000702 RID: 1794
	// (get) Token: 0x0600431D RID: 17181 RVA: 0x001B1588 File Offset: 0x001AF788
	// (set) Token: 0x0600431E RID: 17182 RVA: 0x001B1590 File Offset: 0x001AF790
	public byte CurrentQuestVersion { get; set; }

	// Token: 0x17000703 RID: 1795
	// (get) Token: 0x0600431F RID: 17183 RVA: 0x001B1599 File Offset: 0x001AF799
	// (set) Token: 0x06004320 RID: 17184 RVA: 0x001B15A1 File Offset: 0x001AF7A1
	public byte CurrentFileVersion { get; set; }

	// Token: 0x17000704 RID: 1796
	// (get) Token: 0x06004321 RID: 17185 RVA: 0x001B15AA File Offset: 0x001AF7AA
	// (set) Token: 0x06004322 RID: 17186 RVA: 0x001B15B2 File Offset: 0x001AF7B2
	public string PreviousQuest { get; set; }

	// Token: 0x17000705 RID: 1797
	// (get) Token: 0x06004323 RID: 17187 RVA: 0x001B15BB File Offset: 0x001AF7BB
	// (set) Token: 0x06004324 RID: 17188 RVA: 0x001B15C3 File Offset: 0x001AF7C3
	public bool OptionalComplete { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000706 RID: 1798
	// (get) Token: 0x06004325 RID: 17189 RVA: 0x001B15CC File Offset: 0x001AF7CC
	// (set) Token: 0x06004326 RID: 17190 RVA: 0x001B15D4 File Offset: 0x001AF7D4
	public ulong FinishTime { get; set; }

	// Token: 0x17000707 RID: 1799
	// (get) Token: 0x06004327 RID: 17191 RVA: 0x001B15DD File Offset: 0x001AF7DD
	// (set) Token: 0x06004328 RID: 17192 RVA: 0x001B15E5 File Offset: 0x001AF7E5
	public byte CurrentPhase { get; set; }

	// Token: 0x17000708 RID: 1800
	// (get) Token: 0x06004329 RID: 17193 RVA: 0x001B15EE File Offset: 0x001AF7EE
	// (set) Token: 0x0600432A RID: 17194 RVA: 0x001B15F6 File Offset: 0x001AF7F6
	public int SharedOwnerID
	{
		get
		{
			return this.sharedOwnerID;
		}
		set
		{
			if (this.sharedOwnerID != value)
			{
				this.sharedOwnerID = value;
			}
		}
	}

	// Token: 0x17000709 RID: 1801
	// (get) Token: 0x0600432B RID: 17195 RVA: 0x001B1608 File Offset: 0x001AF808
	public static int QuestsPerDay
	{
		get
		{
			return GameStats.GetInt(EnumGameStats.QuestProgressionDailyLimit);
		}
	}

	// Token: 0x0600432C RID: 17196 RVA: 0x001B1611 File Offset: 0x001AF811
	public void AddQuestTag(FastTags<TagGroup.Global> tag)
	{
		this.QuestTags |= tag;
	}

	// Token: 0x1700070A RID: 1802
	// (get) Token: 0x0600432D RID: 17197 RVA: 0x001B1625 File Offset: 0x001AF825
	public bool Active
	{
		get
		{
			return this.CurrentState == Quest.QuestState.InProgress || this.CurrentState == Quest.QuestState.ReadyForTurnIn;
		}
	}

	// Token: 0x1700070B RID: 1803
	// (get) Token: 0x0600432E RID: 17198 RVA: 0x001B163B File Offset: 0x001AF83B
	// (set) Token: 0x0600432F RID: 17199 RVA: 0x001B1643 File Offset: 0x001AF843
	public Vector3 Position
	{
		get
		{
			return this.position;
		}
		set
		{
			this.position = value;
		}
	}

	// Token: 0x1700070C RID: 1804
	// (get) Token: 0x06004330 RID: 17200 RVA: 0x001B164C File Offset: 0x001AF84C
	public bool HasPosition
	{
		get
		{
			return this.MapObject != null || this.NavObject != null;
		}
	}

	// Token: 0x1700070D RID: 1805
	// (get) Token: 0x06004331 RID: 17201 RVA: 0x001B1664 File Offset: 0x001AF864
	public string RequirementsString
	{
		get
		{
			if (this.Requirements.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					stringBuilder.Append(this.Requirements[i].CheckRequirement() ? "[DEFAULT_COLOR]" : "[MISSING_COLOR]");
					stringBuilder.Append(this.Requirements[i].Description);
					stringBuilder.Append("[-]");
					stringBuilder.Append((i < this.Requirements.Count - 1) ? ", " : "");
				}
				return stringBuilder.ToString();
			}
			return "";
		}
	}

	// Token: 0x1700070E RID: 1806
	// (get) Token: 0x06004332 RID: 17202 RVA: 0x001B1717 File Offset: 0x001AF917
	// (set) Token: 0x06004333 RID: 17203 RVA: 0x001B171F File Offset: 0x001AF91F
	public bool Tracked
	{
		get
		{
			return this.tracked;
		}
		set
		{
			if (this.tracked)
			{
				this.SetMapObjectSelected(false);
			}
			this.tracked = value;
			if (this.tracked)
			{
				this.SetMapObjectSelected(true);
			}
		}
	}

	// Token: 0x06004334 RID: 17204 RVA: 0x001B1748 File Offset: 0x001AF948
	public byte GetActionIndex(BaseQuestAction action)
	{
		for (int i = 0; i < this.Actions.Count; i++)
		{
			if (action == this.Actions[i])
			{
				return (byte)i;
			}
		}
		return 0;
	}

	// Token: 0x06004335 RID: 17205 RVA: 0x001B1780 File Offset: 0x001AF980
	public byte GetObjectiveIndex(BaseObjective objective)
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (objective == this.Objectives[i])
			{
				return (byte)i;
			}
		}
		return 0;
	}

	// Token: 0x1700070F RID: 1807
	// (get) Token: 0x06004336 RID: 17206 RVA: 0x001B17B8 File Offset: 0x001AF9B8
	public int ActiveObjectives
	{
		get
		{
			int num = 0;
			for (int i = 0; i < this.Objectives.Count; i++)
			{
				if ((this.Objectives[i].Phase == 0 || this.Objectives[i].Phase == this.CurrentPhase) && !this.Objectives[i].HiddenObjective)
				{
					num++;
				}
			}
			return num;
		}
	}

	// Token: 0x17000710 RID: 1808
	// (get) Token: 0x06004337 RID: 17207 RVA: 0x001B1821 File Offset: 0x001AFA21
	public QuestClass QuestClass
	{
		get
		{
			if (this.questClass == null)
			{
				this.questClass = QuestClass.GetQuest(this.ID);
			}
			return this.questClass;
		}
	}

	// Token: 0x17000711 RID: 1809
	// (get) Token: 0x06004338 RID: 17208 RVA: 0x001B1842 File Offset: 0x001AFA42
	public bool IsShareable
	{
		get
		{
			return this.SharedOwnerID == -1 && this.QuestClass.Shareable && !this.RallyMarkerActivated && this.CurrentState == Quest.QuestState.InProgress;
		}
	}

	// Token: 0x17000712 RID: 1810
	// (get) Token: 0x06004339 RID: 17209 RVA: 0x001B186D File Offset: 0x001AFA6D
	// (set) Token: 0x0600433A RID: 17210 RVA: 0x001B1875 File Offset: 0x001AFA75
	public MapObject MapObject
	{
		get
		{
			return this.mapObject;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.mapObject != null)
			{
				GameManager.Instance.World.ObjectOnMapRemove(this.mapObject.type, (int)this.mapObject.key);
			}
			this.mapObject = value;
		}
	}

	// Token: 0x17000713 RID: 1811
	// (get) Token: 0x0600433B RID: 17211 RVA: 0x001B18AC File Offset: 0x001AFAAC
	public bool AddsProgression
	{
		get
		{
			return this.QuestProgressDay > 0;
		}
	}

	// Token: 0x0600433C RID: 17212 RVA: 0x001B18B8 File Offset: 0x001AFAB8
	public void HandleMapObject(Quest.PositionDataTypes dataType, string navObjectName, int defaultTreasureRadius = -1)
	{
		if (this.OwnerJournal == null)
		{
			return;
		}
		this.RemoveMapObject();
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		float extraData = -1f;
		bool flag = false;
		switch (dataType)
		{
		case Quest.PositionDataTypes.QuestGiver:
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.QuestGiver))
			{
				this.Position = zero;
				if (navObjectName == "")
				{
					this.MapObject = new MapObjectQuest(zero, "ui_game_symbol_quest");
					GameManager.Instance.World.ObjectOnMapAdd(this.MapObject);
				}
				flag = true;
			}
			break;
		case Quest.PositionDataTypes.Location:
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.Location))
			{
				this.Position = zero;
				if (navObjectName == "")
				{
					this.MapObject = new MapObjectQuest(zero, "ui_game_symbol_quest");
					GameManager.Instance.World.ObjectOnMapAdd(this.MapObject);
				}
				new Vector3(0.5f, 0f, 0.5f);
				flag = true;
			}
			break;
		case Quest.PositionDataTypes.POIPosition:
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition))
			{
				Vector3 zero3 = Vector3.zero;
				if (this.GetPositionData(out zero3, Quest.PositionDataTypes.POISize))
				{
					Vector3 vector = new Vector3(zero.x + zero3.x / 2f, zero.y, zero.z + zero3.z / 2f);
					vector.y = (float)((int)GameManager.Instance.World.GetHeightAt(vector.x, vector.y));
					this.Position = vector;
					if (navObjectName == "")
					{
						this.MapObject = new MapObjectQuest(vector, "ui_game_symbol_quest");
						GameManager.Instance.World.ObjectOnMapAdd(this.MapObject);
					}
				}
				new Vector3(0.5f, 0f, 0.5f);
				flag = true;
			}
			break;
		case Quest.PositionDataTypes.TreasurePoint:
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.TreasurePoint))
			{
				if (defaultTreasureRadius == -1)
				{
					defaultTreasureRadius = ObjectiveTreasureChest.TreasureRadiusInitial;
				}
				float num = EffectManager.GetValue(PassiveEffects.TreasureRadius, null, (float)defaultTreasureRadius, this.OwnerJournal.OwnerPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				num = Mathf.Clamp(num, 0f, (float)defaultTreasureRadius);
				World world = GameManager.Instance.World;
				Vector3 a;
				this.GetPositionData(out a, Quest.PositionDataTypes.TreasureOffset);
				this.Position = zero + a * num;
				if (navObjectName == "")
				{
					if (this.MapObject is MapObjectTreasureChest)
					{
						(this.MapObject as MapObjectTreasureChest).SetPosition(this.Position);
					}
					else
					{
						this.MapObject = new MapObjectTreasureChest(this.Position, this.QuestCode, defaultTreasureRadius);
						GameManager.Instance.World.ObjectOnMapAdd(this.MapObject);
					}
				}
				else
				{
					extraData = (float)defaultTreasureRadius;
				}
				flag = true;
			}
			break;
		case Quest.PositionDataTypes.FetchContainer:
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.FetchContainer))
			{
				this.Position = zero;
				if (navObjectName == "")
				{
					this.MapObject = new MapObjectFetchItem(zero + Vector3.one * 0.5f);
					GameManager.Instance.World.ObjectOnMapAdd(this.MapObject);
				}
				new Vector3(0.5f, 0f, 0.5f);
				flag = true;
			}
			break;
		case Quest.PositionDataTypes.HiddenCache:
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.HiddenCache))
			{
				this.Position = zero;
				if (navObjectName == "")
				{
					this.MapObject = new MapObjectHiddenCache(zero + Vector3.one * 0.5f);
					GameManager.Instance.World.ObjectOnMapAdd(this.MapObject);
				}
				new Vector3(0.5f, 0f, 0.5f);
				flag = true;
			}
			break;
		case Quest.PositionDataTypes.Activate:
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.Activate))
			{
				this.Position = zero;
				if (navObjectName == "")
				{
					this.MapObject = new MapObjectQuest(zero + Vector3.one * 0.5f, "ui_game_symbol_quest");
					GameManager.Instance.World.ObjectOnMapAdd(this.MapObject);
				}
				new Vector3(0.5f, 0f, 0.5f);
				flag = true;
			}
			break;
		}
		if (navObjectName != "" && flag)
		{
			World world2 = GameManager.Instance.World;
			EntityPlayer entityPlayer = world2.GetEntity(this.sharedOwnerID) as EntityPlayer;
			if (entityPlayer == null)
			{
				entityPlayer = world2.GetPrimaryPlayer();
			}
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(navObjectName, this.Position + new Vector3(0.5f, 0f, 0.5f), "", false, -1, entityPlayer);
			this.NavObject.IsActive = false;
			this.NavObject.ExtraData = extraData;
			QuestClass questClass = this.QuestClass;
			this.NavObject.name = string.Format("{0} ({1})", questClass.Name, entityPlayer.PlayerDisplayName);
		}
		this.SetMapObjectSelected(this.tracked);
	}

	// Token: 0x0600433D RID: 17213 RVA: 0x001B1DA8 File Offset: 0x001AFFA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMapObjectSelected(bool isSelected)
	{
		if (this.NavObject != null)
		{
			this.NavObject.IsActive = isSelected;
		}
		if (this.MapObject != null)
		{
			if (this.MapObject is MapObjectQuest)
			{
				((MapObjectQuest)this.MapObject).IsSelected = isSelected;
				return;
			}
			if (this.MapObject is MapObjectTreasureChest)
			{
				((MapObjectTreasureChest)this.MapObject).IsSelected = isSelected;
				return;
			}
			if (this.MapObject is MapObjectFetchItem)
			{
				((MapObjectFetchItem)this.MapObject).IsSelected = isSelected;
				return;
			}
			if (this.MapObject is MapObjectHiddenCache)
			{
				((MapObjectHiddenCache)this.MapObject).IsSelected = isSelected;
				return;
			}
			if (this.MapObject is MapObjectRestorePower)
			{
				((MapObjectRestorePower)this.MapObject).IsSelected = isSelected;
			}
		}
	}

	// Token: 0x0600433E RID: 17214 RVA: 0x001B1E70 File Offset: 0x001B0070
	public void SetupQuestCode()
	{
		if (this.QuestCode == 0)
		{
			this.QuestCode = string.Concat(new string[]
			{
				Time.unscaledTime.ToString(),
				"_",
				this.ID,
				"_",
				this.OwnerJournal.OwnerPlayer.entityId.ToString(),
				"_",
				this.QuestGiverID.ToString()
			}).GetHashCode();
		}
	}

	// Token: 0x0600433F RID: 17215 RVA: 0x001B1EF4 File Offset: 0x001B00F4
	public void RemoveMapObject()
	{
		if (this.MapObject != null)
		{
			this.MapObject = null;
		}
		if (this.NavObject != null)
		{
			NavObjectManager.Instance.UnRegisterNavObject(this.NavObject);
			this.NavObject = null;
		}
		Vector3 zero = Vector3.zero;
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition))
		{
			Vector3 zero2 = Vector3.zero;
			if (this.GetPositionData(out zero2, Quest.PositionDataTypes.POISize))
			{
				Vector3 vector = new Vector3(zero.x + zero2.x / 2f, this.OwnerJournal.OwnerPlayer.position.y, zero.z + zero2.z / 2f);
				GameManager.Instance.World.ObjectOnMapRemove(EnumMapObjectType.Quest, vector);
				return;
			}
		}
		else
		{
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.Location))
			{
				GameManager.Instance.World.ObjectOnMapRemove(EnumMapObjectType.Quest, zero);
				return;
			}
			if (this.GetPositionData(out zero, Quest.PositionDataTypes.TreasurePoint))
			{
				GameManager.Instance.World.ObjectOnMapRemove(EnumMapObjectType.TreasureChest, this.QuestCode);
			}
		}
	}

	// Token: 0x06004340 RID: 17216 RVA: 0x001B1FEC File Offset: 0x001B01EC
	[PublicizedFrom(EAccessModifier.Internal)]
	public void UnhookQuest()
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].HandleRemoveHooks();
		}
		for (int j = 0; j < this.Objectives.Count; j++)
		{
			this.Objectives[j].RemoveObjectives();
		}
		this.RemoveMapObject();
	}

	// Token: 0x06004341 RID: 17217 RVA: 0x001B2050 File Offset: 0x001B0250
	public Quest(string id)
	{
		this.ID = id;
		this.CurrentPhase = 1;
		this.CurrentState = Quest.QuestState.InProgress;
	}

	// Token: 0x06004342 RID: 17218 RVA: 0x001B20F4 File Offset: 0x001B02F4
	public void SetupTags()
	{
		this.NeedsNPCSetPosition = false;
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].OwnerQuest = this;
			this.Objectives[i].HandleVariables();
			this.Objectives[i].SetupQuestTag();
			if (this.Objectives[i].NeedsNPCSetPosition)
			{
				this.NeedsNPCSetPosition = true;
			}
		}
	}

	// Token: 0x06004343 RID: 17219 RVA: 0x001B216C File Offset: 0x001B036C
	public bool SetupPosition(EntityNPC ownerNPC, EntityPlayer player = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (this.Objectives[i].SetupPosition(ownerNPC, player, usedPOILocations, entityIDforQuests))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004344 RID: 17220 RVA: 0x001B21AC File Offset: 0x001B03AC
	public void SetPosition(EntityNPC ownerNPC, Vector3 position, Vector3 size)
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].OwnerQuest = this;
			this.Objectives[i].HandleVariables();
			this.Objectives[i].SetupQuestTag();
		}
		for (int j = 0; j < this.Objectives.Count; j++)
		{
			this.Objectives[j].SetPosition(position, size);
		}
	}

	// Token: 0x06004345 RID: 17221 RVA: 0x001B222C File Offset: 0x001B042C
	public void SetObjectivePosition(Quest.PositionDataTypes dataType, Vector3i position)
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].OwnerQuest = this;
			this.Objectives[i].HandleVariables();
			this.Objectives[i].SetupQuestTag();
		}
		for (int j = 0; j < this.Objectives.Count; j++)
		{
			this.Objectives[j].SetPosition(dataType, position);
		}
	}

	// Token: 0x06004346 RID: 17222 RVA: 0x001B22AC File Offset: 0x001B04AC
	public bool HandleRallyMarkerActivation(Vector3 prefabPos, bool rallyMarkerActivated, QuestEventManager.POILockoutReasonTypes lockoutReason, ulong extraData)
	{
		Vector3 zero = Vector3.zero;
		if (!this.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition))
		{
			return false;
		}
		if (zero != prefabPos)
		{
			return false;
		}
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].OwnerQuest = this;
			this.Objectives[i].HandleVariables();
			this.Objectives[i].SetupQuestTag();
		}
		for (int j = 0; j < this.Objectives.Count; j++)
		{
			ObjectiveRallyPoint objectiveRallyPoint = this.Objectives[j] as ObjectiveRallyPoint;
			if (objectiveRallyPoint != null)
			{
				objectiveRallyPoint.RallyPointActivate(prefabPos, rallyMarkerActivated, lockoutReason, extraData);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004347 RID: 17223 RVA: 0x001B235C File Offset: 0x001B055C
	public void RefreshRallyMarker()
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (this.Objectives[i] is ObjectiveRallyPoint && this.Objectives[i].Phase == this.CurrentPhase)
			{
				(this.Objectives[i] as ObjectiveRallyPoint).RallyPointRefresh();
				return;
			}
		}
	}

	// Token: 0x06004348 RID: 17224 RVA: 0x001B23C4 File Offset: 0x001B05C4
	public bool CheckIsQuestGiver(int entityID)
	{
		Entity entity = GameManager.Instance.World.GetEntity(entityID);
		return this.QuestGiverID == entityID || (entity != null && (entity.position - this.GetQuestGiverLocation()).magnitude < 3f);
	}

	// Token: 0x06004349 RID: 17225 RVA: 0x001B2418 File Offset: 0x001B0618
	public Vector3 GetQuestGiverLocation()
	{
		Vector3 zero = Vector3.zero;
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.QuestGiver))
		{
			return zero;
		}
		return Vector3.zero;
	}

	// Token: 0x0600434A RID: 17226 RVA: 0x001B2440 File Offset: 0x001B0640
	public Vector3 GetLocation()
	{
		Vector3 zero = Vector3.zero;
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition))
		{
			return zero;
		}
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.TreasurePoint))
		{
			return zero;
		}
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.Location))
		{
			return zero;
		}
		return Vector3.zero;
	}

	// Token: 0x0600434B RID: 17227 RVA: 0x001B2480 File Offset: 0x001B0680
	public Vector3 GetLocationSize()
	{
		Vector3 zero = Vector3.zero;
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.POISize))
		{
			return zero;
		}
		return Vector3.zero;
	}

	// Token: 0x0600434C RID: 17228 RVA: 0x001B24A8 File Offset: 0x001B06A8
	public Rect GetLocationRect()
	{
		int num = 5;
		Vector3 zero = Vector3.zero;
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition))
		{
			Vector3 zero2 = Vector3.zero;
			if (this.GetPositionData(out zero2, Quest.PositionDataTypes.POISize))
			{
				return new Rect(zero.x - (float)num, zero.z - (float)num, zero2.x + (float)(num * 2), zero2.z + (float)(num * 2));
			}
		}
		return Rect.zero;
	}

	// Token: 0x0600434D RID: 17229 RVA: 0x001B250C File Offset: 0x001B070C
	public void StartQuest(bool newQuest = true, bool notify = true)
	{
		if (newQuest)
		{
			this.CurrentState = Quest.QuestState.InProgress;
		}
		for (int i = 0; i < this.Actions.Count; i++)
		{
			this.Actions[i].OwnerQuest = this;
			this.Actions[i].HandleVariables();
			this.Actions[i].SetupAction();
			if (newQuest && this.Actions[i].Phase == (int)this.CurrentPhase && !this.Actions[i].OnComplete)
			{
				this.Actions[i].HandlePerformAction();
			}
		}
		for (int j = 0; j < this.Requirements.Count; j++)
		{
			this.Requirements[j].OwnerQuest = this;
			this.Requirements[j].HandleVariables();
			this.Requirements[j].SetupRequirement();
		}
		for (int k = 0; k < this.Objectives.Count; k++)
		{
			this.Objectives[k].OwnerQuest = this;
			this.Objectives[k].HandleVariables();
			this.Objectives[k].SetupQuestTag();
		}
		for (int l = 0; l < this.Objectives.Count; l++)
		{
			this.Objectives[l].SetupObjective();
			this.Objectives[l].SetupDisplay();
		}
		for (int m = 0; m < this.Objectives.Count; m++)
		{
			if (this.Objectives[m].Phase == this.CurrentPhase)
			{
				if (this.Objectives[m].ObjectiveState == BaseObjective.ObjectiveStates.NotStarted)
				{
					this.Objectives[m].ObjectiveState = BaseObjective.ObjectiveStates.InProgress;
				}
				if (this.CurrentState == Quest.QuestState.InProgress && this.Objectives[m].Phase == this.CurrentPhase)
				{
					this.Objectives[m].HandleRemoveHooks();
					this.Objectives[m].HandleAddHooks();
					this.Objectives[m].Refresh();
				}
			}
		}
		bool flag = false;
		for (int n = 0; n < this.Rewards.Count; n++)
		{
			this.Rewards[n].OwnerQuest = this;
			this.Rewards[n].HandleVariables();
			if (this.Rewards[n].ReceiveStage == BaseReward.ReceiveStages.QuestStart && newQuest)
			{
				this.Rewards[n].GiveReward();
			}
			if (this.Rewards[n].RewardIndex > 0 && !flag)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			this.SetupRewards();
		}
		if (newQuest && notify)
		{
			QuestClass quest = QuestClass.GetQuest(this.ID);
			string arg = (quest.Name == quest.SubTitle) ? quest.Name : string.Format("{0} - {1}", quest.Name, quest.SubTitle);
			GameManager.ShowTooltip(this.OwnerJournal.OwnerPlayer, string.Format("{0} {1}: {2}", quest.Category, Localization.Get("started", false), arg), false, false, 0f);
			Manager.PlayInsidePlayerHead("quest_started", -1, 0f, false, false);
			GameManager.Instance.StartCoroutine(this.trackLater(this));
		}
		this.SetupQuestCode();
		this.TrackingHelper.LocalPlayer = this.OwnerJournal.OwnerPlayer;
		this.TrackingHelper.QuestCode = this.QuestCode;
		this.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, false, null);
	}

	// Token: 0x0600434E RID: 17230 RVA: 0x001B28AC File Offset: 0x001B0AAC
	public void SetupSharedQuest()
	{
		this.CurrentState = Quest.QuestState.NotStarted;
		for (int i = 0; i < this.Actions.Count; i++)
		{
			this.Actions[i].OwnerQuest = this;
			this.Actions[i].HandleVariables();
			this.Actions[i].SetupAction();
		}
		for (int j = 0; j < this.Requirements.Count; j++)
		{
			this.Requirements[j].OwnerQuest = this;
			this.Requirements[j].HandleVariables();
			this.Requirements[j].SetupRequirement();
		}
		for (int k = 0; k < this.Objectives.Count; k++)
		{
			this.Objectives[k].OwnerQuest = this;
			this.Objectives[k].HandleVariables();
			this.Objectives[k].SetupObjective();
			this.Objectives[k].SetupDisplay();
		}
		for (int l = 0; l < this.Rewards.Count; l++)
		{
			this.Rewards[l].OwnerQuest = this;
			this.Rewards[l].HandleVariables();
		}
	}

	// Token: 0x0600434F RID: 17231 RVA: 0x001B29E8 File Offset: 0x001B0BE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void AdvancePhase()
	{
		byte currentPhase = this.CurrentPhase;
		this.CurrentPhase = currentPhase + 1;
		for (int i = 0; i < this.Actions.Count; i++)
		{
			if (this.Actions[i].Phase == (int)this.CurrentPhase && !this.Actions[i].OnComplete)
			{
				this.Actions[i].HandlePerformAction();
			}
		}
		for (int j = 0; j < this.Objectives.Count; j++)
		{
			if (this.CurrentState == Quest.QuestState.InProgress)
			{
				if (this.Objectives[j].Phase == this.CurrentPhase - 1)
				{
					this.Objectives[j].HandlePhaseCompleted();
				}
				if (this.Objectives[j].Phase == this.CurrentPhase || this.Objectives[j].Phase == 0)
				{
					if (this.Objectives[j].ObjectiveState == BaseObjective.ObjectiveStates.NotStarted)
					{
						this.Objectives[j].ObjectiveState = BaseObjective.ObjectiveStates.InProgress;
					}
					this.Objectives[j].HandleRemoveHooks();
					this.Objectives[j].HandleAddHooks();
				}
				else
				{
					this.Objectives[j].HandleRemoveHooks();
				}
			}
			else
			{
				this.Objectives[j].HandleRemoveHooks();
			}
		}
	}

	// Token: 0x06004350 RID: 17232 RVA: 0x001B2B48 File Offset: 0x001B0D48
	public void SetupRewards()
	{
		int num = 0;
		for (int i = 0; i < this.Rewards.Count; i++)
		{
			BaseReward baseReward = this.Rewards[i];
			baseReward.RewardIndex = (byte)i;
			if (!baseReward.isChainReward && baseReward.isChosenReward && !baseReward.isFixedLocation)
			{
				num++;
			}
		}
		if (num > 1)
		{
			World world = this.OwnerJournal.OwnerPlayer.world;
			for (int j = 0; j < 100; j++)
			{
				int num2 = world.GetGameRandom().RandomRange(this.Rewards.Count);
				int num3 = world.GetGameRandom().RandomRange(this.Rewards.Count);
				if (num2 != num3)
				{
					BaseReward baseReward2 = this.Rewards[num2];
					BaseReward baseReward3 = this.Rewards[num3];
					if (!baseReward2.isFixedLocation && baseReward2.isChosenReward && !baseReward3.isFixedLocation && baseReward3.isChosenReward)
					{
						byte rewardIndex = this.Rewards[num2].RewardIndex;
						this.Rewards[num2].RewardIndex = this.Rewards[num3].RewardIndex;
						this.Rewards[num3].RewardIndex = rewardIndex;
					}
				}
			}
		}
	}

	// Token: 0x06004351 RID: 17233 RVA: 0x001B2C94 File Offset: 0x001B0E94
	public bool HandleActivateListReceived(Vector3 prefabPos, List<Vector3i> activateList)
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (this.Objectives[i].SetupActivationList(prefabPos, activateList))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004352 RID: 17234 RVA: 0x001B2CD0 File Offset: 0x001B0ED0
	[PublicizedFrom(EAccessModifier.Private)]
	public void TrackQuest_Event(object obj)
	{
		Quest q = (Quest)obj;
		GameManager.Instance.StartCoroutine(this.trackLater(q));
	}

	// Token: 0x06004353 RID: 17235 RVA: 0x001B2CF6 File Offset: 0x001B0EF6
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator trackLater(Quest q)
	{
		yield return new WaitForSeconds(0.5f);
		if (XUi.IsGameRunning())
		{
			if (q.CurrentState == Quest.QuestState.InProgress)
			{
				if (this.OwnerJournal != null && null != this.OwnerJournal.OwnerPlayer)
				{
					LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.OwnerJournal.OwnerPlayer);
					if (null != uiforPlayer && null != uiforPlayer.xui && uiforPlayer.xui.QuestTracker != null)
					{
						XUiM_Quest questTracker = uiforPlayer.xui.QuestTracker;
						if (uiforPlayer.xui.Recipes.TrackedRecipe == null && questTracker.TrackedChallenge == null && questTracker.TrackedQuest == null)
						{
							q.Tracked = (uiforPlayer.xui.QuestTracker.TrackedQuest == null || uiforPlayer.xui.QuestTracker.TrackedQuest == q);
						}
					}
					else
					{
						q.Tracked = false;
					}
					this.OwnerJournal.RefreshTracked();
				}
				else
				{
					q.Tracked = false;
				}
			}
			else
			{
				q.Tracked = false;
			}
		}
		yield break;
	}

	// Token: 0x06004354 RID: 17236 RVA: 0x001B2D0C File Offset: 0x001B0F0C
	public bool CheckRequirements()
	{
		for (int i = 0; i < this.Requirements.Count; i++)
		{
			if ((this.Requirements[i].Phase == 0 || this.Requirements[i].Phase == (int)this.CurrentPhase) && !this.Requirements[i].CheckRequirement())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004355 RID: 17237 RVA: 0x001B2D74 File Offset: 0x001B0F74
	public Quest Clone()
	{
		Quest quest = new Quest(this.ID);
		quest.ID = this.ID;
		quest.OwnerJournal = this.OwnerJournal;
		quest.CurrentQuestVersion = this.CurrentQuestVersion;
		quest.CurrentState = this.CurrentState;
		quest.FinishTime = this.FinishTime;
		quest.SharedOwnerID = this.SharedOwnerID;
		quest.QuestGiverID = this.QuestGiverID;
		quest.CurrentPhase = this.CurrentPhase;
		quest.QuestCode = this.QuestCode;
		quest.RallyMarkerActivated = this.RallyMarkerActivated;
		quest.Tracked = this.Tracked;
		quest.OptionalComplete = this.OptionalComplete;
		quest.QuestTags = this.QuestTags;
		quest.TrackingHelper = this.TrackingHelper;
		quest.QuestFaction = this.QuestFaction;
		quest.QuestProgressDay = this.QuestProgressDay;
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
		for (int l = 0; l < this.Rewards.Count; l++)
		{
			BaseReward baseReward = this.Rewards[l].Clone();
			baseReward.OwnerQuest = quest;
			quest.Rewards.Add(baseReward);
		}
		foreach (KeyValuePair<string, string> keyValuePair in this.DataVariables)
		{
			quest.DataVariables.Add(keyValuePair.Key, keyValuePair.Value);
		}
		foreach (KeyValuePair<Quest.PositionDataTypes, Vector3> keyValuePair2 in this.PositionData)
		{
			quest.PositionData.Add(keyValuePair2.Key, keyValuePair2.Value);
		}
		return quest;
	}

	// Token: 0x06004356 RID: 17238 RVA: 0x001B3000 File Offset: 0x001B1200
	public void ResetObjectives()
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].ResetObjective();
		}
	}

	// Token: 0x06004357 RID: 17239 RVA: 0x001B3034 File Offset: 0x001B1234
	public void ResetToRallyPointObjective()
	{
		if (this.CurrentPhase == this.QuestClass.HighestPhase || !this.QuestClass.LoginRallyReset)
		{
			return;
		}
		this.RallyMarkerActivated = false;
		int num = -1;
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (this.Objectives[i] is ObjectiveRallyPoint)
			{
				num = (int)this.Objectives[i].Phase;
			}
			if (num != -1 && (int)this.Objectives[i].Phase >= num && this.Objectives[i].Phase <= this.CurrentPhase)
			{
				this.Objectives[i].ResetObjective();
			}
		}
		if (num != -1 && num < (int)this.CurrentPhase)
		{
			this.CurrentPhase = (byte)num;
		}
	}

	// Token: 0x06004358 RID: 17240 RVA: 0x001B30FD File Offset: 0x001B12FD
	public void RefreshQuestCompletion(QuestClass.CompletionTypes currentCompletionType = QuestClass.CompletionTypes.AutoComplete, List<BaseReward> rewardChoice = null, bool playObjectiveComplete = true, EntityNPC turnInNPC = null)
	{
		this.refreshQuestCompletion(currentCompletionType, rewardChoice, playObjectiveComplete, turnInNPC);
		PrefabInstance.RefreshSwitchesInContainingPoi(this);
	}

	// Token: 0x06004359 RID: 17241 RVA: 0x001B3110 File Offset: 0x001B1310
	[PublicizedFrom(EAccessModifier.Private)]
	public void refreshQuestCompletion(QuestClass.CompletionTypes currentCompletionType, List<BaseReward> rewardChoice, bool playObjectiveComplete, EntityNPC turnInNPC)
	{
		if (this.CurrentState != Quest.QuestState.InProgress && this.CurrentState != Quest.QuestState.ReadyForTurnIn)
		{
			return;
		}
		if (this.OwnerJournal == null)
		{
			return;
		}
		if (this.CurrentState == Quest.QuestState.InProgress)
		{
			this.OwnerJournal.RefreshQuest(this);
			this.OptionalComplete = true;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < this.Objectives.Count; i++)
			{
				if (this.Objectives[i].Phase == this.CurrentPhase || this.Objectives[i].Phase == 0)
				{
					if (this.Objectives[i].Optional)
					{
						if (!this.Objectives[i].Complete)
						{
							this.OptionalComplete = false;
						}
					}
					else if (!this.Objectives[i].Complete && !this.Objectives[i].AlwaysComplete)
					{
						flag = true;
					}
					else if (this.Objectives[i].ForcePhaseFinish)
					{
						flag2 = true;
					}
				}
			}
			if (flag)
			{
				if (flag2)
				{
					this.CloseQuest(Quest.QuestState.Failed, null);
					return;
				}
				return;
			}
			else if (this.CurrentPhase < this.QuestClass.HighestPhase)
			{
				this.AdvancePhase();
				if (playObjectiveComplete)
				{
					Manager.PlayInsidePlayerHead("quest_objective_complete", -1, 0f, false, false);
				}
				if (this.CurrentPhase == this.QuestClass.HighestPhase && this.OwnerJournal.ActiveQuest == this)
				{
					this.OwnerJournal.ActiveQuest = null;
					this.OwnerJournal.RefreshRallyMarkerPositions();
				}
				return;
			}
		}
		if (currentCompletionType != this.QuestClass.CompletionType)
		{
			this.CurrentState = Quest.QuestState.ReadyForTurnIn;
			return;
		}
		QuestEventManager.Current.QuestCompleted(this.QuestTags, this.questClass);
		this.CloseQuest(Quest.QuestState.Completed, rewardChoice);
		if (this.QuestClass.ResetTraderQuests)
		{
			EntityTrader entityTrader = turnInNPC as EntityTrader;
			if (entityTrader != null)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					EntityPlayerLocal ownerPlayer = this.OwnerJournal.OwnerPlayer;
					entityTrader.ClearActiveQuests(ownerPlayer.entityId);
					entityTrader.SetupActiveQuestsForPlayer(ownerPlayer, -1);
					return;
				}
				EntityPlayer ownerPlayer2 = this.OwnerJournal.OwnerPlayer;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.ResetTraderQuests, ownerPlayer2.entityId, this.QuestGiverID, entityTrader.GetQuestFactionPoints(ownerPlayer2)), false);
			}
		}
	}

	// Token: 0x0600435A RID: 17242 RVA: 0x001B3340 File Offset: 0x001B1540
	public void CloseQuest(Quest.QuestState finalState, List<BaseReward> rewardChoice = null)
	{
		if (finalState != Quest.QuestState.Completed && finalState != Quest.QuestState.Failed)
		{
			Log.Warning(string.Format("Ending a quest in a state {0}. Should be {1} or {2}", finalState, Quest.QuestState.Completed, Quest.QuestState.Failed));
		}
		if (this.OwnerJournal == null)
		{
			return;
		}
		this.OwnerJournal.RefreshQuest(this);
		this.CurrentState = finalState;
		bool flag = finalState == Quest.QuestState.Completed;
		this.HandleUnlockPOI(null);
		bool flag2 = !string.IsNullOrEmpty(this.PreviousQuest);
		bool flag3 = flag && flag2;
		if (flag3)
		{
			for (int i = 0; i < this.Rewards.Count; i++)
			{
				if (this.Rewards[i] is RewardQuest && (this.Rewards[i] as RewardQuest).IsChainQuest)
				{
					flag3 = false;
				}
			}
		}
		ToolTipEvent toolTipEvent = new ToolTipEvent();
		for (int j = 0; j < this.Rewards.Count; j++)
		{
			if (this.Rewards[j].ReceiveStage == BaseReward.ReceiveStages.AfterCompleteNotification)
			{
				toolTipEvent.EventHandler += this.QuestRewardsLater_Event;
				toolTipEvent.Parameter = this;
				break;
			}
		}
		EntityPlayerLocal ownerPlayer = this.OwnerJournal.OwnerPlayer;
		string arg = (this.questClass.Name == this.questClass.SubTitle) ? this.questClass.Name : string.Format("{0} - {1}", this.questClass.Name, this.questClass.SubTitle);
		string arg2 = flag ? Localization.Get("completed", false) : Localization.Get("failed", false);
		string alertSound = flag ? "quest_subtask_complete" : "quest_failed";
		ToolTipEvent handler = (flag && !flag3) ? toolTipEvent : null;
		GameManager.ShowTooltip(ownerPlayer, string.Format("{0} {1}: {2}", this.questClass.Category, arg2, arg), string.Empty, alertSound, handler, false, false, 0f);
		if (flag3)
		{
			GameManager.ShowTooltip(ownerPlayer, string.Format("{0} {1}: {2}", Localization.Get("questChain", false), Localization.Get("completed", false), this.questClass.GroupName), string.Empty, "quest_master_complete", toolTipEvent, false, false, 0f);
		}
		if (this.OwnerJournal.TrackedQuest == this)
		{
			this.OwnerJournal.TrackedQuest = null;
		}
		for (int k = 0; k < this.Objectives.Count; k++)
		{
			this.Objectives[k].HandleRemoveHooks();
		}
		for (int l = 0; l < this.Objectives.Count; l++)
		{
			this.Objectives[l].RemoveObjectives();
			if (flag)
			{
				this.Objectives[l].HandleCompleted();
			}
			else
			{
				this.Objectives[l].HandleFailed();
			}
		}
		this.RemoveMapObject();
		if (flag)
		{
			if (this.AddsProgression)
			{
				QuestEventManager.Current.HandleNewCompletedQuest(this.OwnerJournal.OwnerPlayer, this.QuestFaction, (int)this.QuestClass.DifficultyTier, this.QuestClass.AddsToTierComplete);
			}
			for (int m = 0; m < this.Rewards.Count; m++)
			{
				if (this.Rewards[m].ReceiveStage == BaseReward.ReceiveStages.QuestCompletion && (!this.Rewards[m].Optional || (this.Rewards[m].Optional && this.OptionalComplete)) && (!this.Rewards[m].isChosenReward || (this.Rewards[m].isChosenReward && rewardChoice != null && rewardChoice.Contains(this.Rewards[m]))))
				{
					this.Rewards[m].GiveReward();
				}
			}
			this.OwnerJournal.CompleteQuest(this);
			for (int n = 0; n < this.Actions.Count; n++)
			{
				BaseQuestAction baseQuestAction = this.Actions[n];
				if (baseQuestAction.OnComplete)
				{
					baseQuestAction.HandlePerformAction();
				}
			}
		}
		else
		{
			this.OptionalComplete = false;
			this.tracked = false;
			this.OwnerJournal.FailedQuest(this);
		}
		if (this.OwnerJournal.ActiveQuest == this)
		{
			this.OwnerJournal.ActiveQuest = null;
			this.OwnerJournal.RefreshRallyMarkerPositions();
		}
		if (this.QuestClass.ResetTraderQuests)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				QuestEventManager.Current.AddTraderResetQuestsForPlayer(this.OwnerJournal.OwnerPlayer.entityId, this.QuestGiverID);
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.ResetTraderQuests, this.OwnerJournal.OwnerPlayer.entityId, this.QuestGiverID, -1), false);
		}
	}

	// Token: 0x0600435B RID: 17243 RVA: 0x001B37E0 File Offset: 0x001B19E0
	public void HandleUnlockPOI(EntityPlayer player = null)
	{
		Vector3 zero = Vector3.zero;
		if (this.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition))
		{
			if (player == null)
			{
				player = this.OwnerJournal.OwnerPlayer;
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				QuestEventManager.Current.QuestUnlockPOI(player.entityId, zero);
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.UnlockPOI, player.entityId, zero), false);
		}
	}

	// Token: 0x0600435C RID: 17244 RVA: 0x001B3850 File Offset: 0x001B1A50
	public void HandleQuestEvent(Quest ownerQuest, string eventType)
	{
		for (int i = 0; i < this.questClass.Events.Count; i++)
		{
			if (this.questClass.Events[i].EventType == eventType)
			{
				this.questClass.Events[i].HandleEvent(ownerQuest);
			}
		}
	}

	// Token: 0x0600435D RID: 17245 RVA: 0x001B38B0 File Offset: 0x001B1AB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void QuestRewardsLater_Event(object obj)
	{
		Quest q = (Quest)obj;
		GameManager.Instance.StartCoroutine(this.GiveRewardsLater(q));
	}

	// Token: 0x0600435E RID: 17246 RVA: 0x001B38D6 File Offset: 0x001B1AD6
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator GiveRewardsLater(Quest q)
	{
		yield return new WaitForSeconds(3f);
		if (XUi.IsGameRunning() && q.CurrentState == Quest.QuestState.Completed)
		{
			for (int i = 0; i < this.Rewards.Count; i++)
			{
				if (this.Rewards[i].ReceiveStage == BaseReward.ReceiveStages.AfterCompleteNotification && (!this.Rewards[i].Optional || (this.Rewards[i].Optional && q.OptionalComplete)))
				{
					this.Rewards[i].GiveReward();
				}
			}
		}
		yield break;
	}

	// Token: 0x0600435F RID: 17247 RVA: 0x001B38EC File Offset: 0x001B1AEC
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseQuestAction AddAction(BaseQuestAction action)
	{
		if (action != null)
		{
			this.Actions.Add(action);
		}
		return action;
	}

	// Token: 0x06004360 RID: 17248 RVA: 0x001B38FE File Offset: 0x001B1AFE
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseRequirement AddRequirement(BaseRequirement requirement)
	{
		if (requirement != null)
		{
			this.Requirements.Add(requirement);
		}
		return requirement;
	}

	// Token: 0x06004361 RID: 17249 RVA: 0x001B3910 File Offset: 0x001B1B10
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseObjective AddObjective(BaseObjective objective)
	{
		if (objective != null)
		{
			this.Objectives.Add(objective);
		}
		return objective;
	}

	// Token: 0x06004362 RID: 17250 RVA: 0x001B3922 File Offset: 0x001B1B22
	[PublicizedFrom(EAccessModifier.Internal)]
	public BaseReward AddReward(BaseReward reward)
	{
		if (reward != null)
		{
			this.Rewards.Add(reward);
		}
		return reward;
	}

	// Token: 0x06004363 RID: 17251 RVA: 0x001B3934 File Offset: 0x001B1B34
	public string ParseVariable(string value)
	{
		if (value != null && value.Contains("{"))
		{
			int num = value.IndexOf("{") + 1;
			int num2 = value.IndexOf("}", num);
			if (num2 != -1)
			{
				string key = value.Substring(num, num2 - num);
				if (this.DataVariables.ContainsKey(key))
				{
					value = this.DataVariables[key];
				}
			}
		}
		return value;
	}

	// Token: 0x06004364 RID: 17252 RVA: 0x001B3998 File Offset: 0x001B1B98
	public void Read(PooledBinaryReader _br)
	{
		bool flag = true;
		this.CurrentFileVersion = _br.ReadByte();
		this.CurrentState = (Quest.QuestState)_br.ReadByte();
		this.SharedOwnerID = _br.ReadInt32();
		this.QuestGiverID = _br.ReadInt32();
		if (this.CurrentState == Quest.QuestState.InProgress)
		{
			this.tracked = _br.ReadBoolean();
			this.CurrentPhase = _br.ReadByte();
			this.QuestCode = _br.ReadInt32();
		}
		else if (this.CurrentState == Quest.QuestState.Completed)
		{
			this.CurrentPhase = this.QuestClass.HighestPhase;
		}
		PooledBinaryReader.StreamReadSizeMarker streamReadSizeMarker = default(PooledBinaryReader.StreamReadSizeMarker);
		if (this.CurrentFileVersion >= 7)
		{
			streamReadSizeMarker = _br.ReadSizeMarker(PooledBinaryWriter.EMarkerSize.UInt16);
		}
		try
		{
			for (int i = 0; i < this.Objectives.Count; i++)
			{
				this.Objectives[i].Read(_br);
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex.ToString());
		}
		finally
		{
			uint num;
			if (this.CurrentFileVersion >= 7 && !_br.ValidateSizeMarker(ref streamReadSizeMarker, out num, true))
			{
				this.Objectives.Clear();
				Log.Error("Loading player quests: Quest with ID " + this.ID + ": Failed loading objectives");
				flag = false;
			}
		}
		int num2 = (int)_br.ReadByte();
		for (int j = 0; j < num2; j++)
		{
			string key = _br.ReadString();
			string value = _br.ReadString();
			if (!this.DataVariables.ContainsKey(key))
			{
				this.DataVariables.Add(key, value);
			}
			else
			{
				this.DataVariables[key] = value;
			}
		}
		if (this.CurrentState == Quest.QuestState.InProgress)
		{
			this.PositionData.Clear();
			int num3 = (int)_br.ReadByte();
			for (int k = 0; k < num3; k++)
			{
				Quest.PositionDataTypes dataType = (Quest.PositionDataTypes)_br.ReadByte();
				Vector3 value2 = StreamUtils.ReadVector3(_br);
				this.SetPositionData(dataType, value2);
			}
			this.RallyMarkerActivated = _br.ReadBoolean();
		}
		else
		{
			this.FinishTime = _br.ReadUInt64();
		}
		if (this.CurrentState == Quest.QuestState.InProgress || this.CurrentState == Quest.QuestState.ReadyForTurnIn)
		{
			PooledBinaryReader.StreamReadSizeMarker streamReadSizeMarker2 = default(PooledBinaryReader.StreamReadSizeMarker);
			if (this.CurrentFileVersion >= 7)
			{
				streamReadSizeMarker2 = _br.ReadSizeMarker(PooledBinaryWriter.EMarkerSize.UInt16);
			}
			try
			{
				if (this.CurrentFileVersion <= 5)
				{
					for (int l = 0; l < this.Rewards.Count; l++)
					{
						this.Rewards[l].Read(_br);
					}
				}
				else
				{
					int num4 = _br.ReadInt32();
					for (int m = 0; m < num4; m++)
					{
						this.Rewards[m].Read(_br);
					}
				}
			}
			catch (Exception ex2)
			{
				Log.Error(ex2.ToString());
			}
			finally
			{
				uint num;
				if (this.CurrentFileVersion >= 7 && !_br.ValidateSizeMarker(ref streamReadSizeMarker2, out num, true))
				{
					this.Rewards.Clear();
					Log.Error("Loading player quests: Quest with ID " + this.ID + ": Failed loading rewards");
					flag = false;
				}
			}
		}
		if (this.CurrentFileVersion > 4)
		{
			this.QuestFaction = _br.ReadByte();
		}
		if (!flag && this.CurrentState != Quest.QuestState.Completed)
		{
			this.CurrentState = Quest.QuestState.Failed;
		}
		if (this.CurrentFileVersion >= 8)
		{
			this.QuestProgressDay = _br.ReadInt32();
		}
	}

	// Token: 0x06004365 RID: 17253 RVA: 0x001B3CB8 File Offset: 0x001B1EB8
	public void Write(PooledBinaryWriter _bw)
	{
		_bw.Write(this.ID);
		_bw.Write(this.CurrentQuestVersion);
		_bw.Write(Quest.FileVersion);
		_bw.Write((byte)this.CurrentState);
		_bw.Write(this.SharedOwnerID);
		_bw.Write(this.QuestGiverID);
		if (this.CurrentState == Quest.QuestState.InProgress)
		{
			_bw.Write(this.Tracked);
			_bw.Write(this.CurrentPhase);
			_bw.Write(this.QuestCode);
		}
		PooledBinaryWriter.StreamWriteSizeMarker streamWriteSizeMarker = _bw.ReserveSizeMarker(PooledBinaryWriter.EMarkerSize.UInt16);
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			this.Objectives[i].Write(_bw);
		}
		_bw.FinalizeSizeMarker(ref streamWriteSizeMarker);
		_bw.Write((byte)this.DataVariables.Count);
		foreach (KeyValuePair<string, string> keyValuePair in this.DataVariables)
		{
			_bw.Write(keyValuePair.Key);
			_bw.Write(keyValuePair.Value);
		}
		if (this.CurrentState == Quest.QuestState.InProgress)
		{
			_bw.Write((byte)this.PositionData.Count);
			foreach (KeyValuePair<Quest.PositionDataTypes, Vector3> keyValuePair2 in this.PositionData)
			{
				_bw.Write((byte)keyValuePair2.Key);
				StreamUtils.Write(_bw, keyValuePair2.Value);
			}
			_bw.Write(this.RallyMarkerActivated);
		}
		else
		{
			_bw.Write(this.FinishTime);
		}
		if (this.CurrentState == Quest.QuestState.InProgress || this.CurrentState == Quest.QuestState.ReadyForTurnIn)
		{
			PooledBinaryWriter.StreamWriteSizeMarker streamWriteSizeMarker2 = _bw.ReserveSizeMarker(PooledBinaryWriter.EMarkerSize.UInt16);
			_bw.Write(this.Rewards.Count);
			for (int j = 0; j < this.Rewards.Count; j++)
			{
				this.Rewards[j].Write(_bw);
			}
			_bw.FinalizeSizeMarker(ref streamWriteSizeMarker2);
		}
		_bw.Write(this.QuestFaction);
		_bw.Write(this.QuestProgressDay);
	}

	// Token: 0x06004366 RID: 17254 RVA: 0x001B3EE4 File Offset: 0x001B20E4
	public void AddSharedLocation(Vector3 pos, Vector3 size)
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (this.Objectives[i].Phase == this.CurrentPhase && this.Objectives[i].SetLocation(pos, size))
			{
				return;
			}
		}
	}

	// Token: 0x06004367 RID: 17255 RVA: 0x001B3F38 File Offset: 0x001B2138
	public void AddSharedKill(string enemyType)
	{
		for (int i = 0; i < this.Objectives.Count; i++)
		{
			if (this.Objectives[i].Phase == this.CurrentPhase && this.Objectives[i].ID == enemyType)
			{
				BaseObjective baseObjective = this.Objectives[i];
				byte currentValue = baseObjective.CurrentValue;
				baseObjective.CurrentValue = currentValue + 1;
				this.Objectives[i].Refresh();
			}
		}
	}

	// Token: 0x06004368 RID: 17256 RVA: 0x001B3FBA File Offset: 0x001B21BA
	public int GetSharedWithCount()
	{
		if (this.sharedWithList == null)
		{
			return 0;
		}
		return this.sharedWithList.Count;
	}

	// Token: 0x06004369 RID: 17257 RVA: 0x001B3FD4 File Offset: 0x001B21D4
	public int GetSharedWithCountNotInRange()
	{
		if (this.sharedWithList == null)
		{
			return 0;
		}
		EntityPlayer ownerPlayer = this.OwnerJournal.OwnerPlayer;
		int num = 0;
		Rect locationRect = this.GetLocationRect();
		for (int i = 0; i < this.sharedWithList.Count; i++)
		{
			EntityPlayer entityPlayer = this.sharedWithList[i];
			if (locationRect != Rect.zero)
			{
				Vector3 vector = entityPlayer.position;
				vector.y = vector.z;
				if (!locationRect.Contains(vector))
				{
					num++;
				}
			}
			else if (Vector3.Distance(ownerPlayer.position, entityPlayer.position) >= 15f)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600436A RID: 17258 RVA: 0x001B4078 File Offset: 0x001B2278
	public List<EntityPlayer> GetSharedWithListNotInRange()
	{
		if (this.sharedWithList == null)
		{
			return null;
		}
		EntityPlayer ownerPlayer = this.OwnerJournal.OwnerPlayer;
		Rect locationRect = this.GetLocationRect();
		List<EntityPlayer> list = new List<EntityPlayer>();
		for (int i = 0; i < this.sharedWithList.Count; i++)
		{
			EntityPlayer entityPlayer = this.sharedWithList[i];
			if (locationRect != Rect.zero)
			{
				Vector3 vector = entityPlayer.position;
				vector.y = vector.z;
				if (!locationRect.Contains(vector))
				{
					list.Add(entityPlayer);
				}
			}
			else if (Vector3.Distance(ownerPlayer.position, entityPlayer.position) >= 15f)
			{
				list.Add(entityPlayer);
			}
		}
		return list;
	}

	// Token: 0x0600436B RID: 17259 RVA: 0x001B4128 File Offset: 0x001B2328
	public void RemoveSharedNotInRange()
	{
		if (this.sharedWithList == null)
		{
			return;
		}
		EntityPlayer ownerPlayer = this.OwnerJournal.OwnerPlayer;
		Rect locationRect = this.GetLocationRect();
		for (int i = this.sharedWithList.Count - 1; i >= 0; i--)
		{
			EntityPlayer entityPlayer = this.sharedWithList[i];
			if (locationRect != Rect.zero)
			{
				Vector3 vector = entityPlayer.position;
				vector.y = vector.z;
				if (!locationRect.Contains(vector))
				{
					this.sharedWithList.RemoveAt(i);
				}
			}
			else if (Vector3.Distance(ownerPlayer.position, entityPlayer.position) >= 15f)
			{
				this.sharedWithList.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600436C RID: 17260 RVA: 0x001B41D8 File Offset: 0x001B23D8
	public int[] GetSharedWithIDList()
	{
		if (this.sharedWithList == null)
		{
			return null;
		}
		int[] array = new int[this.sharedWithList.Count];
		for (int i = 0; i < this.sharedWithList.Count; i++)
		{
			array[i] = this.sharedWithList[i].entityId;
		}
		return array;
	}

	// Token: 0x0600436D RID: 17261 RVA: 0x001B422C File Offset: 0x001B242C
	public bool HasSharedWith(EntityPlayer player)
	{
		if (this.sharedWithList == null)
		{
			return false;
		}
		for (int i = 0; i < this.sharedWithList.Count; i++)
		{
			if (this.sharedWithList[i] == player)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600436E RID: 17262 RVA: 0x001B4270 File Offset: 0x001B2470
	public void AddSharedWith(EntityPlayer player)
	{
		if (this.sharedWithList == null)
		{
			this.sharedWithList = new List<EntityPlayer>();
		}
		if (!this.sharedWithList.Contains(player))
		{
			this.sharedWithList.Add(player);
		}
	}

	// Token: 0x0600436F RID: 17263 RVA: 0x001B42A0 File Offset: 0x001B24A0
	public bool RemoveSharedWith(EntityPlayer player)
	{
		bool result = false;
		if (this.sharedWithList == null)
		{
			return false;
		}
		for (int i = this.sharedWithList.Count - 1; i >= 0; i--)
		{
			if (this.sharedWithList[i].entityId == player.entityId)
			{
				result = true;
				this.sharedWithList.RemoveAt(i);
			}
		}
		if (this.sharedWithList.Count == 0)
		{
			this.sharedWithList = null;
		}
		return result;
	}

	// Token: 0x06004370 RID: 17264 RVA: 0x001B4310 File Offset: 0x001B2510
	public void SetPositionData(Quest.PositionDataTypes dataType, Vector3 value)
	{
		if (!this.PositionData.ContainsKey(dataType))
		{
			this.PositionData.Add(dataType, value);
		}
		else
		{
			this.PositionData[dataType] = value;
		}
		if (this.OwnerJournal != null)
		{
			PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.OwnerJournal.OwnerPlayer.entityId);
			if (playerDataFromEntityID != null)
			{
				playerDataFromEntityID.AddQuestPosition(this.QuestCode, dataType, value);
			}
		}
	}

	// Token: 0x06004371 RID: 17265 RVA: 0x001B4380 File Offset: 0x001B2580
	public void RemovePositionData(Quest.PositionDataTypes dataType)
	{
		if (this.PositionData.ContainsKey(dataType))
		{
			this.PositionData.Remove(dataType);
		}
	}

	// Token: 0x06004372 RID: 17266 RVA: 0x001B439D File Offset: 0x001B259D
	public bool GetPositionData(out Vector3 pos, Quest.PositionDataTypes dataType)
	{
		if (this.PositionData.ContainsKey(dataType))
		{
			pos = this.PositionData[dataType];
			return true;
		}
		pos = Vector3.zero;
		return false;
	}

	// Token: 0x06004373 RID: 17267 RVA: 0x001B43CD File Offset: 0x001B25CD
	public string GetParsedText(string text)
	{
		if (text.Contains("{"))
		{
			text = this.ParseBindingVariables(text);
		}
		return text;
	}

	// Token: 0x06004374 RID: 17268 RVA: 0x001B43E8 File Offset: 0x001B25E8
	[PublicizedFrom(EAccessModifier.Private)]
	public string ParseBindingVariables(string response)
	{
		if (string.IsNullOrEmpty(response))
		{
			return response;
		}
		int num = response.IndexOf('{');
		while (num != -1)
		{
			int num2 = response.IndexOf('}', num);
			if (num2 != -1)
			{
				string text = response.Substring(num, num2 - num + 1);
				string[] array = text.Substring(1, text.Length - 2).Split(new char[]
				{
					'_',
					'.'
				});
				if (array.Length == 2)
				{
					response = response.Replace(text, this.GetVariableText(array[0], -1, array[1]));
				}
				if (array.Length == 3)
				{
					response = response.Replace(text, this.GetVariableText(array[0], Convert.ToInt32(array[1]), array[2]));
				}
			}
			if (num + 1 < response.Length)
			{
				num = response.IndexOf('{', num + 1);
			}
			else
			{
				num = -1;
			}
		}
		return response;
	}

	// Token: 0x06004375 RID: 17269 RVA: 0x001B44B0 File Offset: 0x001B26B0
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetVariableText(string field, int index, string variableName)
	{
		int num = 0;
		if (!(field == "fetch"))
		{
			if (!(field == "buff"))
			{
				if (!(field == "kill"))
				{
					if (!(field == "goto"))
					{
						if (!(field == "poi"))
						{
							if (field == "treasure")
							{
								for (int i = 0; i < this.Objectives.Count; i++)
								{
									if (this.Objectives[i] is ObjectiveTreasureChest && (++num == index || index == -1))
									{
										return this.Objectives[i].ParseBinding(variableName);
									}
								}
							}
						}
						else
						{
							for (int j = 0; j < this.Objectives.Count; j++)
							{
								if (this.Objectives[j] is ObjectiveRandomPOIGoto && (++num == index || index == -1))
								{
									return this.Objectives[j].ParseBinding(variableName);
								}
							}
						}
					}
					else
					{
						for (int k = 0; k < this.Objectives.Count; k++)
						{
							if (this.Objectives[k] is ObjectiveGoto && (++num == index || index == -1))
							{
								return this.Objectives[k].ParseBinding(variableName);
							}
						}
					}
				}
				else
				{
					for (int l = 0; l < this.Objectives.Count; l++)
					{
						if (this.Objectives[l] is ObjectiveEntityKill && (++num == index || index == -1))
						{
							return this.Objectives[l].ParseBinding(variableName);
						}
					}
				}
			}
			else
			{
				for (int m = 0; m < this.Objectives.Count; m++)
				{
					if (this.Objectives[m] is ObjectiveBuff && (++num == index || index == -1))
					{
						return this.Objectives[m].ParseBinding(variableName);
					}
				}
			}
		}
		else
		{
			for (int n = 0; n < this.Objectives.Count; n++)
			{
				if ((this.Objectives[n] is ObjectiveFetch || this.Objectives[n] is ObjectiveFetchKeep) && (++num == index || index == -1))
				{
					return this.Objectives[n].ParseBinding(variableName);
				}
			}
		}
		return field;
	}

	// Token: 0x06004376 RID: 17270 RVA: 0x001B470D File Offset: 0x001B290D
	public string GetPOIName()
	{
		if (this.DataVariables.ContainsKey("POIName"))
		{
			return this.DataVariables["POIName"];
		}
		return "";
	}

	// Token: 0x06004377 RID: 17271 RVA: 0x001B4738 File Offset: 0x001B2938
	public bool CanTurnInQuest(List<BaseReward> rewardChoice)
	{
		EntityPlayerLocal ownerPlayer = this.OwnerJournal.OwnerPlayer;
		ItemStack[] array = this.OwnerJournal.OwnerPlayer.bag.CloneItemStack();
		ItemStack[] array2 = this.OwnerJournal.OwnerPlayer.inventory.CloneItemStack();
		ItemStack[] array3 = new ItemStack[array.Length + array2.Length];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			array3[num++] = array[i];
		}
		for (int j = 0; j < array2.Length; j++)
		{
			array3[num++] = array2[j];
		}
		for (int k = 0; k < this.Rewards.Count; k++)
		{
			if (this.Rewards[k].ReceiveStage == BaseReward.ReceiveStages.QuestCompletion && (!this.Rewards[k].Optional || (this.Rewards[k].Optional && this.OptionalComplete)) && (!this.Rewards[k].isChosenReward || (this.Rewards[k].isChosenReward && rewardChoice != null && rewardChoice.Contains(this.Rewards[k]))))
			{
				ItemStack rewardItem = this.Rewards[k].GetRewardItem();
				if (!rewardItem.IsEmpty())
				{
					XUiM_PlayerInventory.TryStackItem(0, rewardItem, array3);
					if (rewardItem.count > 0 && ItemStack.AddToItemStackArray(array3, rewardItem, -1) == -1)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	// Token: 0x04003527 RID: 13607
	public QuestJournal OwnerJournal;

	// Token: 0x04003528 RID: 13608
	public static byte FileVersion = 8;

	// Token: 0x04003529 RID: 13609
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest.QuestState _currentState;

	// Token: 0x04003531 RID: 13617
	[PublicizedFrom(EAccessModifier.Private)]
	public int sharedOwnerID = -1;

	// Token: 0x04003532 RID: 13618
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityPlayer> sharedWithList;

	// Token: 0x04003533 RID: 13619
	public int QuestCode;

	// Token: 0x04003534 RID: 13620
	public int QuestGiverID = -1;

	// Token: 0x04003535 RID: 13621
	public static int MaxQuestTier = 5;

	// Token: 0x04003536 RID: 13622
	public static int QuestsPerTier = 10;

	// Token: 0x04003537 RID: 13623
	public byte QuestFaction;

	// Token: 0x04003538 RID: 13624
	public bool RallyMarkerActivated;

	// Token: 0x04003539 RID: 13625
	public FastTags<TagGroup.Global> QuestTags = FastTags<TagGroup.Global>.none;

	// Token: 0x0400353A RID: 13626
	public bool NeedsNPCSetPosition;

	// Token: 0x0400353B RID: 13627
	public int QuestProgressDay = int.MinValue;

	// Token: 0x0400353C RID: 13628
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position = Vector3.zero;

	// Token: 0x0400353D RID: 13629
	[PublicizedFrom(EAccessModifier.Private)]
	public bool tracked;

	// Token: 0x0400353E RID: 13630
	[PublicizedFrom(EAccessModifier.Private)]
	public QuestClass questClass;

	// Token: 0x0400353F RID: 13631
	[PublicizedFrom(EAccessModifier.Private)]
	public MapObject mapObject;

	// Token: 0x04003540 RID: 13632
	public PrefabInstance QuestPrefab;

	// Token: 0x04003541 RID: 13633
	public NavObject NavObject;

	// Token: 0x04003542 RID: 13634
	public Dictionary<Quest.PositionDataTypes, Vector3> PositionData = new EnumDictionary<Quest.PositionDataTypes, Vector3>();

	// Token: 0x04003543 RID: 13635
	public Dictionary<string, string> DataVariables = new Dictionary<string, string>();

	// Token: 0x04003544 RID: 13636
	public List<BaseQuestAction> Actions = new List<BaseQuestAction>();

	// Token: 0x04003545 RID: 13637
	public List<BaseRequirement> Requirements = new List<BaseRequirement>();

	// Token: 0x04003546 RID: 13638
	public List<BaseObjective> Objectives = new List<BaseObjective>();

	// Token: 0x04003547 RID: 13639
	public List<BaseReward> Rewards = new List<BaseReward>();

	// Token: 0x04003548 RID: 13640
	public TrackingHandler TrackingHelper = new TrackingHandler();

	// Token: 0x020008E6 RID: 2278
	public enum QuestState
	{
		// Token: 0x0400354A RID: 13642
		NotStarted,
		// Token: 0x0400354B RID: 13643
		InProgress,
		// Token: 0x0400354C RID: 13644
		ReadyForTurnIn,
		// Token: 0x0400354D RID: 13645
		Completed,
		// Token: 0x0400354E RID: 13646
		Failed
	}

	// Token: 0x020008E7 RID: 2279
	public enum PositionDataTypes
	{
		// Token: 0x04003550 RID: 13648
		QuestGiver,
		// Token: 0x04003551 RID: 13649
		Location,
		// Token: 0x04003552 RID: 13650
		POIPosition,
		// Token: 0x04003553 RID: 13651
		POISize,
		// Token: 0x04003554 RID: 13652
		TreasurePoint,
		// Token: 0x04003555 RID: 13653
		FetchContainer,
		// Token: 0x04003556 RID: 13654
		HiddenCache,
		// Token: 0x04003557 RID: 13655
		Activate,
		// Token: 0x04003558 RID: 13656
		TreasureOffset,
		// Token: 0x04003559 RID: 13657
		TraderPosition
	}
}
