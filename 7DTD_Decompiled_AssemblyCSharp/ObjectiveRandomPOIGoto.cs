using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008D5 RID: 2261
[Preserve]
public class ObjectiveRandomPOIGoto : ObjectiveGoto
{
	// Token: 0x170006EB RID: 1771
	// (get) Token: 0x0600427F RID: 17023 RVA: 0x001ADB18 File Offset: 0x001ABD18
	// (set) Token: 0x0600427E RID: 17022 RVA: 0x001ADB0F File Offset: 0x001ABD0F
	public int POITier
	{
		get
		{
			if (this.poiTier != -1)
			{
				return this.poiTier;
			}
			return (int)base.OwnerQuest.QuestClass.DifficultyTier;
		}
		set
		{
			this.poiTier = value;
		}
	}

	// Token: 0x06004280 RID: 17024 RVA: 0x001A6FE4 File Offset: 0x001A51E4
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveRallyPointHeadTo", false);
	}

	// Token: 0x06004281 RID: 17025 RVA: 0x001A7005 File Offset: 0x001A5205
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetupIcon()
	{
		this.icon = "ui_game_symbol_quest";
	}

	// Token: 0x170006EC RID: 1772
	// (get) Token: 0x06004282 RID: 17026 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NeedsNPCSetPosition
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170006ED RID: 1773
	// (get) Token: 0x06004283 RID: 17027 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool PlayObjectiveComplete
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06004284 RID: 17028 RVA: 0x001A95F2 File Offset: 0x001A77F2
	public override bool SetupPosition(EntityNPC ownerNPC = null, EntityPlayer player = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		return this.GetPosition(ownerNPC, player, usedPOILocations, entityIDforQuests) != Vector3.zero;
	}

	// Token: 0x06004285 RID: 17029 RVA: 0x001ADB3A File Offset: 0x001ABD3A
	public override void AddHooks()
	{
		base.AddHooks();
		base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, this.NavObjectName, -1);
	}

	// Token: 0x06004286 RID: 17030 RVA: 0x001ADB55 File Offset: 0x001ABD55
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetDistanceOffset(Vector3 POISize)
	{
		if (POISize.x > POISize.z)
		{
			this.distanceOffset = POISize.x;
			return;
		}
		this.distanceOffset = POISize.z;
	}

	// Token: 0x06004287 RID: 17031 RVA: 0x001ADB7E File Offset: 0x001ABD7E
	public override void SetPosition(Vector3 POIPosition, Vector3 POISize)
	{
		this.SetDistanceOffset(POISize);
		base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.POIPosition, POIPosition);
		base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.POISize, POISize);
		base.OwnerQuest.Position = POIPosition;
		this.position = POIPosition;
	}

	// Token: 0x06004288 RID: 17032 RVA: 0x001ADBB4 File Offset: 0x001ABDB4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 GetPosition(EntityNPC ownerNPC = null, EntityPlayer entityPlayer = null, List<Vector2> usedPOILocations = null, int entityIDforQuests = -1)
	{
		int traderId = (ownerNPC == null) ? -1 : ownerNPC.entityId;
		int playerId = (entityPlayer == null) ? -1 : entityPlayer.entityId;
		if (base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.POIPosition))
		{
			Vector3 vector;
			base.OwnerQuest.GetPositionData(out vector, Quest.PositionDataTypes.POISize);
			Vector2 vector2 = new Vector2(this.position.x + vector.x / 2f, this.position.z + vector.z / 2f);
			int num = (int)vector2.x;
			int num2 = (int)vector2.y;
			int num3 = (int)GameManager.Instance.World.GetHeightAt(vector2.x, vector2.y);
			this.position = new Vector3((float)num, (float)num3, (float)num2);
			base.OwnerQuest.Position = this.position;
			this.SetDistanceOffset(vector);
			this.positionSet = true;
			base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.POIPosition, this.NavObjectName, -1);
			base.CurrentValue = 2;
			return this.position;
		}
		EntityAlive entityAlive = entityPlayer;
		if (entityAlive == null)
		{
			entityAlive = ((ownerNPC == null) ? base.OwnerQuest.OwnerJournal.OwnerPlayer : ownerNPC);
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			PrefabInstance prefabInstance;
			if (ownerNPC != null)
			{
				prefabInstance = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator().GetRandomPOINearTrader(ownerNPC as EntityTrader, base.OwnerQuest.QuestTags, (byte)this.POITier, usedPOILocations, entityIDforQuests, this.biomeFilterType, this.biomeFilter);
			}
			else
			{
				prefabInstance = GameManager.Instance.World.ChunkClusters[0].ChunkProvider.GetDynamicPrefabDecorator().GetRandomPOINearWorldPos(new Vector2(entityAlive.position.x, entityAlive.position.z), 1000, 4000000, base.OwnerQuest.QuestTags, (byte)this.POITier, usedPOILocations, entityIDforQuests, this.biomeFilterType, this.biomeFilter);
			}
			if (prefabInstance == null)
			{
				return Vector3.zero;
			}
			if (prefabInstance != null)
			{
				Vector2 vector3 = new Vector2((float)prefabInstance.boundingBoxPosition.x + (float)prefabInstance.boundingBoxSize.x / 2f, (float)prefabInstance.boundingBoxPosition.z + (float)prefabInstance.boundingBoxSize.z / 2f);
				if (vector3.x == -0.1f && vector3.y == -0.1f)
				{
					Log.Error("ObjectiveRandomGoto: No POI found.");
					return Vector3.zero;
				}
				int num4 = (int)vector3.x;
				int num5 = (int)GameManager.Instance.World.GetHeightAt(vector3.x, vector3.y);
				int num6 = (int)vector3.y;
				this.position = new Vector3((float)num4, (float)num5, (float)num6);
				if (GameManager.Instance.World.IsPositionInBounds(this.position))
				{
					base.OwnerQuest.Position = this.position;
					base.FinalizePoint(new Vector3((float)prefabInstance.boundingBoxPosition.x, (float)prefabInstance.boundingBoxPosition.y, (float)prefabInstance.boundingBoxPosition.z), new Vector3((float)prefabInstance.boundingBoxSize.x, (float)prefabInstance.boundingBoxSize.y, (float)prefabInstance.boundingBoxSize.z));
					base.OwnerQuest.QuestPrefab = prefabInstance;
					base.OwnerQuest.DataVariables.Add("POIName", Localization.Get(base.OwnerQuest.QuestPrefab.location.Name, false));
					if (usedPOILocations != null)
					{
						usedPOILocations.Add(new Vector2((float)prefabInstance.boundingBoxPosition.x, (float)prefabInstance.boundingBoxPosition.z));
					}
					return this.position;
				}
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestGotoPoint>().Setup(traderId, playerId, base.OwnerQuest.QuestTags, base.OwnerQuest.QuestCode, NetPackageQuestGotoPoint.QuestGotoTypes.RandomPOI, (byte)this.POITier, 0, -1, 0f, 0f, 0f, -1f, this.biomeFilterType, this.biomeFilter), false);
			base.CurrentValue = 1;
		}
		return Vector3.zero;
	}

	// Token: 0x06004289 RID: 17033 RVA: 0x001ADFF4 File Offset: 0x001AC1F4
	public override BaseObjective Clone()
	{
		ObjectiveRandomPOIGoto objectiveRandomPOIGoto = new ObjectiveRandomPOIGoto();
		this.CopyValues(objectiveRandomPOIGoto);
		objectiveRandomPOIGoto.poiTier = this.poiTier;
		objectiveRandomPOIGoto.distance = this.distance;
		objectiveRandomPOIGoto.biomeFilterType = this.biomeFilterType;
		objectiveRandomPOIGoto.biomeFilter = this.biomeFilter;
		objectiveRandomPOIGoto.locationName = this.locationName;
		return objectiveRandomPOIGoto;
	}

	// Token: 0x0600428A RID: 17034 RVA: 0x001AE04C File Offset: 0x001AC24C
	public override string ParseBinding(string bindingName)
	{
		string id = this.ID;
		string value = this.Value;
		if (!(bindingName == "name"))
		{
			if (!(bindingName == "distance"))
			{
				if (bindingName == "direction")
				{
					if (base.OwnerQuest.QuestGiverID != -1)
					{
						EntityNPC entityNPC = GameManager.Instance.World.GetEntity(base.OwnerQuest.QuestGiverID) as EntityNPC;
						if (entityNPC != null)
						{
							this.position.y = 0f;
							Vector3 position = entityNPC.position;
							position.y = 0f;
							return ValueDisplayFormatters.Direction(GameUtils.GetDirByNormal(new Vector2(this.position.x - position.x, this.position.z - position.z)), false);
						}
					}
				}
			}
			else if (base.OwnerQuest.QuestGiverID != -1)
			{
				EntityNPC entityNPC2 = GameManager.Instance.World.GetEntity(base.OwnerQuest.QuestGiverID) as EntityNPC;
				if (entityNPC2 != null)
				{
					Vector3 position2 = entityNPC2.position;
					this.currentDistance = Vector3.Distance(position2, this.position);
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

	// Token: 0x0600428B RID: 17035 RVA: 0x001AE22D File Offset: 0x001AC42D
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseInt(ObjectiveRandomPOIGoto.PropPOITier, ref this.poiTier);
	}

	// Token: 0x040034C6 RID: 13510
	[PublicizedFrom(EAccessModifier.Private)]
	public int poiTier = -1;

	// Token: 0x040034C7 RID: 13511
	public static string PropPOITier = "poi_tier";
}
