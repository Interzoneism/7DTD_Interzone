using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008C6 RID: 2246
[Preserve]
public class ObjectiveGoto : BaseObjective
{
	// Token: 0x170006D2 RID: 1746
	// (get) Token: 0x060041D2 RID: 16850 RVA: 0x00075C39 File Offset: 0x00073E39
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Distance;
		}
	}

	// Token: 0x170006D3 RID: 1747
	// (get) Token: 0x060041D3 RID: 16851 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NeedsNPCSetPosition
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060041D4 RID: 16852 RVA: 0x001A94C8 File Offset: 0x001A76C8
	public override void SetupObjective()
	{
		if (this.ID == "trader")
		{
			Localization.Get("xuiTrader", false);
		}
		this.keyword = string.Format(Localization.Get("ObjectiveGoto_keyword", false), Localization.Get(this.locationName, false));
		this.distance = StringParsers.ParseFloat(this.Value, 0, -1, NumberStyles.Any);
		this.SetupIcon();
		if (base.OwnerQuest.Active)
		{
			base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, this.NavObjectName, -1);
		}
	}

	// Token: 0x170006D4 RID: 1748
	// (get) Token: 0x060041D5 RID: 16853 RVA: 0x001A6B33 File Offset: 0x001A4D33
	public override bool UpdateUI
	{
		get
		{
			return base.ObjectiveState != BaseObjective.ObjectiveStates.Failed;
		}
	}

	// Token: 0x170006D5 RID: 1749
	// (get) Token: 0x060041D6 RID: 16854 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool useUpdateLoop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return true;
		}
	}

	// Token: 0x060041D7 RID: 16855 RVA: 0x001A6B41 File Offset: 0x001A4D41
	public override void SetupDisplay()
	{
		base.Description = this.keyword;
		this.StatusText = "";
	}

	// Token: 0x170006D6 RID: 1750
	// (get) Token: 0x060041D8 RID: 16856 RVA: 0x001A9554 File Offset: 0x001A7754
	public override string StatusText
	{
		get
		{
			if (this.poiNotFound && base.OwnerQuest.Position == Vector3.zero)
			{
				return "NO TRADER";
			}
			if (base.OwnerQuest.CurrentState == Quest.QuestState.InProgress)
			{
				return ValueDisplayFormatters.Distance(this.currentDistance);
			}
			if (base.OwnerQuest.CurrentState == Quest.QuestState.NotStarted)
			{
				return "";
			}
			if (base.ObjectiveState == BaseObjective.ObjectiveStates.Failed)
			{
				return Localization.Get("failed", false);
			}
			return Localization.Get("completed", false);
		}
	}

	// Token: 0x060041D9 RID: 16857 RVA: 0x001A95D3 File Offset: 0x001A77D3
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetupIcon()
	{
		if (this.ID.EqualsCaseInsensitive("trader"))
		{
			this.icon = "ui_game_symbol_map_trader";
		}
	}

	// Token: 0x060041DA RID: 16858 RVA: 0x001A95F2 File Offset: 0x001A77F2
	public override bool SetupPosition(EntityNPC ownerNPC = null, EntityPlayer player = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		return this.GetPosition(ownerNPC, player, usedPOILocations, entityIDforQuests) != Vector3.zero;
	}

	// Token: 0x060041DB RID: 16859 RVA: 0x001A960C File Offset: 0x001A780C
	public override void SetPosition(Vector3 POIPosition, Vector3 POISize)
	{
		if (POISize.x > POISize.z)
		{
			this.distanceOffset = POISize.x;
		}
		else
		{
			this.distanceOffset = POISize.z;
		}
		base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.POIPosition, POIPosition);
		base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.POISize, POISize);
		this.position = this.GetMidPOIPosition(POIPosition, POISize);
		base.OwnerQuest.Position = this.position;
	}

	// Token: 0x060041DC RID: 16860 RVA: 0x001A967C File Offset: 0x001A787C
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 GetMidPOIPosition(Vector3 poiPosition, Vector3 poiSize)
	{
		int num = (int)(poiPosition.x + poiSize.x / 2f);
		int num2 = (int)(poiPosition.z + poiSize.z / 2f);
		int num3 = (int)GameManager.Instance.World.GetHeightAt((float)num, (float)num2);
		return new Vector3((float)num, (float)num3, (float)num2);
	}

	// Token: 0x060041DD RID: 16861 RVA: 0x001A96D8 File Offset: 0x001A78D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual Vector3 GetPosition(EntityNPC ownerNPC = null, EntityPlayer entityPlayer = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		EntityAlive entityAlive = (ownerNPC == null) ? base.OwnerQuest.OwnerJournal.OwnerPlayer : ownerNPC;
		if (entityPlayer == null)
		{
			entityPlayer = base.OwnerQuest.OwnerJournal.OwnerPlayer;
		}
		int traderId = (ownerNPC == null) ? -1 : ownerNPC.entityId;
		int playerId = (entityPlayer == null) ? -1 : entityPlayer.entityId;
		FastTags<TagGroup.Global> fastTags = FastTags<TagGroup.Global>.none;
		if (!string.IsNullOrEmpty(this.ID))
		{
			fastTags = FastTags<TagGroup.Global>.Parse(this.ID);
		}
		Vector3 zero = Vector3.zero;
		if (base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.POIPosition) && base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POISize))
		{
			this.position = this.GetMidPOIPosition(this.position, zero);
			base.OwnerQuest.Position = this.position;
			this.positionSet = true;
			if (zero.x > zero.z)
			{
				this.distanceOffset = zero.x;
			}
			else
			{
				this.distanceOffset = zero.z;
			}
			base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, this.NavObjectName, -1);
			base.CurrentValue = 2;
			return this.position;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			PrefabInstance prefabInstance = null;
			if (prefabInstance == null)
			{
				prefabInstance = base.OwnerQuest.QuestPrefab;
			}
			int factionID = (int)((ownerNPC != null) ? ownerNPC.NPCInfo.QuestFaction : base.OwnerQuest.QuestFaction);
			usedPOILocations = ((entityPlayer != null) ? entityPlayer.QuestJournal.GetTraderList(factionID) : null);
			if (prefabInstance == null)
			{
				prefabInstance = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator().GetClosestPOIToWorldPos(fastTags, new Vector2(entityAlive.position.x, entityAlive.position.z), usedPOILocations, -1, !this.allowCurrentPoi, this.biomeFilterType, this.biomeFilter);
				if (prefabInstance == null)
				{
					prefabInstance = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator().GetClosestPOIToWorldPos(fastTags, new Vector2(entityAlive.position.x, entityAlive.position.z), usedPOILocations, -1, !this.allowCurrentPoi, BiomeFilterTypes.SameBiome, "");
				}
			}
			if (prefabInstance == null)
			{
				return Vector3.zero;
			}
			Vector2 vector = new Vector2((float)prefabInstance.boundingBoxPosition.x + (float)prefabInstance.boundingBoxSize.x / 2f, (float)prefabInstance.boundingBoxPosition.z + (float)prefabInstance.boundingBoxSize.z / 2f);
			if (vector.x == -0.1f && vector.y == -0.1f)
			{
				Log.Error("ObjectiveGoto: No Trader found.");
				return Vector3.zero;
			}
			int num = (int)vector.x;
			int num2 = (int)vector.y;
			int num3 = (int)GameManager.Instance.World.GetHeightAt(vector.x, vector.y);
			this.position = new Vector3((float)num, (float)num3, (float)num2);
			if (GameManager.Instance.World.IsPositionInBounds(this.position))
			{
				base.OwnerQuest.Position = this.position;
				this.FinalizePoint(new Vector3((float)prefabInstance.boundingBoxPosition.x, (float)prefabInstance.boundingBoxPosition.y, (float)prefabInstance.boundingBoxPosition.z), new Vector3((float)prefabInstance.boundingBoxSize.x, (float)prefabInstance.boundingBoxSize.y, (float)prefabInstance.boundingBoxSize.z));
				base.OwnerQuest.QuestPrefab = prefabInstance;
				base.OwnerQuest.DataVariables.Add("POIName", Localization.Get(base.OwnerQuest.QuestPrefab.location.Name, false));
				return this.position;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestGotoPoint>().Setup(traderId, playerId, fastTags, base.OwnerQuest.QuestCode, NetPackageQuestGotoPoint.QuestGotoTypes.Trader, base.OwnerQuest.QuestClass.DifficultyTier, 0, -1, 0f, 0f, 0f, -1f, this.biomeFilterType, this.biomeFilter), false);
			base.CurrentValue = 1;
		}
		return Vector3.zero;
	}

	// Token: 0x060041DE RID: 16862 RVA: 0x001A9B18 File Offset: 0x001A7D18
	public void FinalizePoint(Vector3 POIPosition, Vector3 POISize)
	{
		if (POISize.x > POISize.z)
		{
			this.distanceOffset = POISize.x;
		}
		else
		{
			this.distanceOffset = POISize.z;
		}
		base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.POIPosition, POIPosition);
		base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.POISize, POISize);
		this.position = this.GetMidPOIPosition(POIPosition, POISize);
		this.positionSet = true;
		base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, this.NavObjectName, -1);
		if (GameSparksCollector.CollectGamePlayData && base.OwnerQuest.QuestClass.ID == "quest_whiterivercitizen1")
		{
			GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.QuestStarterTraderDistance, ((int)Vector3.Distance(this.position, base.OwnerQuest.OwnerJournal.OwnerPlayer.position) / 50 * 50).ToString(), 1, true, GameSparksCollector.GSDataCollection.SessionUpdates);
		}
		base.CurrentValue = 2;
	}

	// Token: 0x060041DF RID: 16863 RVA: 0x001A9BF4 File Offset: 0x001A7DF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateState_NeedSetup()
	{
		int entityIDforQuests = -1;
		if (base.OwnerQuest != null)
		{
			entityIDforQuests = base.OwnerQuest.OwnerJournal.OwnerPlayer.entityId;
		}
		if (this.GetPosition(null, base.OwnerQuest.OwnerJournal.OwnerPlayer, null, entityIDforQuests) != Vector3.zero)
		{
			this.poiNotFound = false;
			return;
		}
		this.poiNotFound = true;
	}

	// Token: 0x060041E0 RID: 16864 RVA: 0x001A9C58 File Offset: 0x001A7E58
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateState_Update()
	{
		if (!this.positionSet)
		{
			this.GetPosition(null, null, null, -1);
			return;
		}
		if (this.position.y == 0f)
		{
			this.position.y = (float)((int)GameManager.Instance.World.GetHeightAt(this.position.x, this.position.z));
		}
		EntityPlayer ownerPlayer = base.OwnerQuest.OwnerJournal.OwnerPlayer;
		if (base.OwnerQuest.NavObject != null && base.OwnerQuest.NavObject.TrackedPosition != this.position)
		{
			base.OwnerQuest.NavObject.TrackedPosition = this.position;
		}
		this.currentDistance = Vector3.Distance(ownerPlayer.position, this.position);
		if (this.currentDistance < this.distance + this.distanceOffset && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue = 3;
			this.Refresh();
		}
	}

	// Token: 0x060041E1 RID: 16865 RVA: 0x001A9D54 File Offset: 0x001A7F54
	public override void Refresh()
	{
		bool complete = base.CurrentValue == 3;
		base.Complete = complete;
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x060041E2 RID: 16866 RVA: 0x001A9D8C File Offset: 0x001A7F8C
	public override BaseObjective Clone()
	{
		ObjectiveGoto objectiveGoto = new ObjectiveGoto();
		this.CopyValues(objectiveGoto);
		return objectiveGoto;
	}

	// Token: 0x060041E3 RID: 16867 RVA: 0x001A9DA8 File Offset: 0x001A7FA8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CopyValues(BaseObjective objective)
	{
		base.CopyValues(objective);
		ObjectiveGoto objectiveGoto = (ObjectiveGoto)objective;
		objectiveGoto.distance = this.distance;
		objectiveGoto.biomeFilterType = this.biomeFilterType;
		objectiveGoto.biomeFilter = this.biomeFilter;
		objectiveGoto.locationName = this.locationName;
		objectiveGoto.allowCurrentPoi = this.allowCurrentPoi;
	}

	// Token: 0x060041E4 RID: 16868 RVA: 0x001A9DFD File Offset: 0x001A7FFD
	public override bool SetLocation(Vector3 pos, Vector3 size)
	{
		this.FinalizePoint(pos, size);
		return true;
	}

	// Token: 0x060041E5 RID: 16869 RVA: 0x001A9E08 File Offset: 0x001A8008
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveGoto.PropDistance))
		{
			this.Value = properties.Values[ObjectiveGoto.PropDistance];
			this.distance = StringParsers.ParseFloat(this.Value, 0, -1, NumberStyles.Any);
		}
		if (properties.Values.ContainsKey(ObjectiveGoto.PropAllowCurrentPOI))
		{
			this.allowCurrentPoi = StringParsers.ParseBool(properties.Values[ObjectiveGoto.PropAllowCurrentPOI], 0, -1, true);
		}
		properties.ParseString(ObjectiveGoto.PropLocation, ref this.ID);
		properties.ParseEnum<BiomeFilterTypes>(ObjectiveGoto.PropBiomeFilterType, ref this.biomeFilterType);
		properties.ParseString(ObjectiveGoto.PropBiomeFilter, ref this.biomeFilter);
		properties.ParseString(ObjectiveGoto.PropLocationName, ref this.locationName);
	}

	// Token: 0x060041E6 RID: 16870 RVA: 0x001A9ED0 File Offset: 0x001A80D0
	public override void HandleCompleted()
	{
		Vector3 vector;
		if (base.OwnerQuest.OwnerJournal != null && base.OwnerQuest.GetPositionData(out vector, Quest.PositionDataTypes.POIPosition))
		{
			base.OwnerQuest.OwnerJournal.AddTraderPOI(new Vector2(vector.x, vector.z), (int)base.OwnerQuest.QuestFaction);
		}
	}

	// Token: 0x060041E7 RID: 16871 RVA: 0x001A9F28 File Offset: 0x001A8128
	public override string ParseBinding(string bindingName)
	{
		string id = this.ID;
		string value = this.Value;
		if (bindingName == "location")
		{
			return this.ID;
		}
		if (!(bindingName == "name"))
		{
			if (!(bindingName == "distance"))
			{
				if (!(bindingName == "direction"))
				{
					if (bindingName == "directionfull")
					{
						if (base.OwnerQuest.QuestGiverID != -1)
						{
							EntityNPC entityNPC = GameManager.Instance.World.GetEntity(base.OwnerQuest.QuestGiverID) as EntityNPC;
							if (entityNPC != null)
							{
								this.position.y = 0f;
								Vector3 vector = entityNPC.position;
								vector.y = 0f;
								return ValueDisplayFormatters.Direction(GameUtils.GetDirByNormal(new Vector2(this.position.x - vector.x, this.position.z - vector.z)), true);
							}
						}
					}
				}
				else if (base.OwnerQuest.QuestGiverID != -1)
				{
					EntityNPC entityNPC2 = GameManager.Instance.World.GetEntity(base.OwnerQuest.QuestGiverID) as EntityNPC;
					if (entityNPC2 != null)
					{
						this.position.y = 0f;
						Vector3 vector2 = entityNPC2.position;
						vector2.y = 0f;
						return ValueDisplayFormatters.Direction(GameUtils.GetDirByNormal(new Vector2(this.position.x - vector2.x, this.position.z - vector2.z)), false);
					}
				}
			}
			else if (base.OwnerQuest.QuestGiverID != -1)
			{
				EntityNPC entityNPC3 = GameManager.Instance.World.GetEntity(base.OwnerQuest.QuestGiverID) as EntityNPC;
				if (entityNPC3 != null)
				{
					Vector3 a = entityNPC3.position;
					this.currentDistance = Vector3.Distance(a, this.position);
					return ValueDisplayFormatters.Distance(this.currentDistance);
				}
			}
			return "";
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (base.OwnerQuest.DataVariables.ContainsKey("POIName"))
			{
				return base.OwnerQuest.DataVariables["POIName"];
			}
			if (base.OwnerQuest.QuestPrefab == null)
			{
				return "";
			}
			return Localization.Get(base.OwnerQuest.QuestPrefab.location.Name, false);
		}
		else
		{
			if (!base.OwnerQuest.DataVariables.ContainsKey("POIName"))
			{
				return "";
			}
			return base.OwnerQuest.DataVariables["POIName"];
		}
	}

	// Token: 0x0400345A RID: 13402
	public static string PropAllowCurrentPOI = "allow_current_poi";

	// Token: 0x0400345B RID: 13403
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool allowCurrentPoi;

	// Token: 0x0400345C RID: 13404
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool positionSet;

	// Token: 0x0400345D RID: 13405
	[PublicizedFrom(EAccessModifier.Protected)]
	public float distance = 20f;

	// Token: 0x0400345E RID: 13406
	[PublicizedFrom(EAccessModifier.Protected)]
	public float distanceOffset;

	// Token: 0x0400345F RID: 13407
	[PublicizedFrom(EAccessModifier.Protected)]
	public float currentDistance;

	// Token: 0x04003460 RID: 13408
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 position;

	// Token: 0x04003461 RID: 13409
	[PublicizedFrom(EAccessModifier.Protected)]
	public string icon = "ui_game_symbol_quest";

	// Token: 0x04003462 RID: 13410
	[PublicizedFrom(EAccessModifier.Protected)]
	public string locationVariable = "gotolocation";

	// Token: 0x04003463 RID: 13411
	[PublicizedFrom(EAccessModifier.Protected)]
	public BiomeFilterTypes biomeFilterType = BiomeFilterTypes.SameBiome;

	// Token: 0x04003464 RID: 13412
	[PublicizedFrom(EAccessModifier.Protected)]
	public string biomeFilter = "";

	// Token: 0x04003465 RID: 13413
	[PublicizedFrom(EAccessModifier.Protected)]
	public string locationName = "";

	// Token: 0x04003466 RID: 13414
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool poiNotFound;

	// Token: 0x04003467 RID: 13415
	public static string PropLocation = "location_tag";

	// Token: 0x04003468 RID: 13416
	public static string PropDistance = "distance";

	// Token: 0x04003469 RID: 13417
	public static string PropBiomeFilterType = "biome_filter_type";

	// Token: 0x0400346A RID: 13418
	public static string PropBiomeFilter = "biome_filter";

	// Token: 0x0400346B RID: 13419
	public static string PropLocationName = "location_name";
}
