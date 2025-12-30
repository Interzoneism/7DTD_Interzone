using System;
using System.Collections;
using System.Collections.Generic;
using Twitch;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008CC RID: 2252
[Preserve]
public class ObjectiveRallyPoint : BaseObjective
{
	// Token: 0x170006E0 RID: 1760
	// (get) Token: 0x06004235 RID: 16949 RVA: 0x00075C39 File Offset: 0x00073E39
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Distance;
		}
	}

	// Token: 0x170006E1 RID: 1761
	// (get) Token: 0x06004236 RID: 16950 RVA: 0x001A6B33 File Offset: 0x001A4D33
	public override bool UpdateUI
	{
		get
		{
			return base.ObjectiveState != BaseObjective.ObjectiveStates.Failed;
		}
	}

	// Token: 0x170006E2 RID: 1762
	// (get) Token: 0x06004237 RID: 16951 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool useUpdateLoop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return true;
		}
	}

	// Token: 0x170006E3 RID: 1763
	// (get) Token: 0x06004238 RID: 16952 RVA: 0x001AB454 File Offset: 0x001A9654
	public override string StatusText
	{
		get
		{
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

	// Token: 0x06004239 RID: 16953 RVA: 0x001AB4B0 File Offset: 0x001A96B0
	public override void SetupObjective()
	{
		ObjectiveRallyPoint.textActivateRallyPoint = Localization.Get("ObjectiveRallyPointActivate", false);
		ObjectiveRallyPoint.textHeadToRallyPoint = Localization.Get("ObjectiveRallyPointHeadTo", false);
		ObjectiveRallyPoint.textWaitForActivate = Localization.Get("ObjectiveWaitForActivate_keyword", false);
		this.keyword = Localization.Get("ObjectiveBlockActivate_keyword", false);
		this.localizedName = ((this.ID != "" && this.ID != null) ? Localization.Get(this.ID, false) : "Any Block");
		if (base.OwnerQuest.SharedOwnerID != -1)
		{
			base.Description = ObjectiveRallyPoint.textWaitForActivate;
			return;
		}
		base.Description = ObjectiveRallyPoint.textActivateRallyPoint;
	}

	// Token: 0x0600423A RID: 16954 RVA: 0x001AB556 File Offset: 0x001A9756
	public override void AddHooks()
	{
		QuestEventManager.Current.BlockActivate += this.Current_BlockActivate;
	}

	// Token: 0x0600423B RID: 16955 RVA: 0x001AB570 File Offset: 0x001A9770
	public static void SetupFlags(List<BaseObjective> objectives)
	{
		HashSet<ObjectiveRallyPointData> hashSet = new HashSet<ObjectiveRallyPointData>();
		foreach (BaseObjective baseObjective in objectives)
		{
			ObjectiveRallyPoint objectiveRallyPoint = baseObjective as ObjectiveRallyPoint;
			if (objectiveRallyPoint != null && objectiveRallyPoint.isRallyVisible)
			{
				Transform blockTransform = ObjectiveRallyPoint.getBlockTransform(objectiveRallyPoint.rallyPos);
				if (blockTransform != null)
				{
					ObjectiveRallyPointData component = blockTransform.gameObject.GetComponent<ObjectiveRallyPointData>();
					if (component != null)
					{
						Quest ownerQuest = objectiveRallyPoint.OwnerQuest;
						if (ownerQuest.CurrentPhase == objectiveRallyPoint.Phase)
						{
							if (!hashSet.Contains(component))
							{
								hashSet.Add(component);
								component.ClearAllFlags();
							}
							component.AddFlag(objectiveRallyPoint.rallyMarkerType, ownerQuest.SharedOwnerID == -1);
						}
					}
				}
			}
		}
		foreach (ObjectiveRallyPointData objectiveRallyPointData in hashSet)
		{
			objectiveRallyPointData.UpdateAllFlags();
		}
	}

	// Token: 0x0600423C RID: 16956 RVA: 0x001AB684 File Offset: 0x001A9884
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform getBlockTransform(Vector3i rallyPos)
	{
		Transform result = null;
		Chunk chunk = GameManager.Instance.World.GetChunkFromWorldPos(rallyPos) as Chunk;
		if (chunk != null)
		{
			BlockEntityData blockEntity = chunk.GetBlockEntity(rallyPos);
			if (blockEntity != null)
			{
				result = blockEntity.transform;
			}
		}
		return result;
	}

	// Token: 0x0600423D RID: 16957 RVA: 0x001AB6BF File Offset: 0x001A98BF
	[PublicizedFrom(EAccessModifier.Private)]
	public void setVisibility(Transform t, string childName, bool isVisible)
	{
		this.isRallyVisible = isVisible;
	}

	// Token: 0x0600423E RID: 16958 RVA: 0x001AB6C8 File Offset: 0x001A98C8
	public override void RemoveObjectives()
	{
		base.RemoveObjectives();
		World world = GameManager.Instance.World;
		if (this.rallyPos != Vector3i.zero)
		{
			if (this.RallyStartType == ObjectiveRallyPoint.RallyStartTypes.Find)
			{
				Chunk chunk = world.GetChunkFromWorldPos(this.rallyPos) as Chunk;
				if (chunk != null)
				{
					BlockEntityData blockEntity = chunk.GetBlockEntity(this.rallyPos);
					if (blockEntity != null && blockEntity.transform != null)
					{
						this.setVisibility(blockEntity.transform, this.rallyMarkerType, false);
						return;
					}
				}
			}
			else
			{
				if (base.OwnerQuest.SharedOwnerID == -1)
				{
					GameManager.Instance.World.SetBlockRPC(this.rallyPos, BlockValue.Air, sbyte.MaxValue);
					return;
				}
				GameManager.Instance.StartCoroutine(this.setRallyPointVisibility(false));
			}
		}
	}

	// Token: 0x0600423F RID: 16959 RVA: 0x001AB788 File Offset: 0x001A9988
	public override void RemoveHooks()
	{
		World world = GameManager.Instance.World;
		QuestEventManager.Current.BlockActivate -= this.Current_BlockActivate;
		if (this.rallyPos != Vector3i.zero)
		{
			if (this.RallyStartType == ObjectiveRallyPoint.RallyStartTypes.Find)
			{
				bool rallyPointVisibility = base.OwnerQuest.OwnerJournal.HasQuestAtRallyPosition(this.rallyPos.ToVector3(), true) != null && base.OwnerQuest.OwnerJournal.ActiveQuest == null;
				GameManager.Instance.StartCoroutine(this.setRallyPointVisibility(rallyPointVisibility));
				return;
			}
			if (base.OwnerQuest.SharedOwnerID == -1)
			{
				world.SetBlockRPC(this.rallyPos, BlockValue.Air, sbyte.MaxValue);
				return;
			}
			GameManager.Instance.StartCoroutine(this.setRallyPointVisibility(false));
		}
	}

	// Token: 0x06004240 RID: 16960 RVA: 0x001AB84F File Offset: 0x001A9A4F
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator setRallyPointVisibility(bool visible)
	{
		QuestEventManager.Current.BlockActivate -= this.Current_BlockActivate;
		if (visible)
		{
			QuestEventManager.Current.BlockActivate += this.Current_BlockActivate;
		}
		BlockValue bvRallyType = Block.GetBlockValue("questRallyMarker", false);
		World world = GameManager.Instance.World;
		Chunk chunk = world.GetChunkFromWorldPos(this.rallyPos) as Chunk;
		if (chunk != null)
		{
			BlockEntityData bd = chunk.GetBlockEntity(this.rallyPos);
			if (bd != null)
			{
				while (bd.transform == null)
				{
					yield return null;
				}
				if (!visible)
				{
					ObjectiveRallyPointData component = bd.transform.gameObject.GetComponent<ObjectiveRallyPointData>();
					if (component != null)
					{
						component.RemoveFlag(this.rallyMarkerType);
						component.UpdateAllFlags();
					}
				}
				if (bd.blockValue.EqualsExceptRotation(bvRallyType))
				{
					this.setVisibility(bd.transform, this.rallyMarkerType, visible);
				}
				else
				{
					world.SetBlockRPC(chunk.ClrIdx, this.rallyPos, bvRallyType);
				}
			}
			else
			{
				world.SetBlockRPC(chunk.ClrIdx, this.rallyPos, bvRallyType);
			}
			bd = null;
		}
		yield break;
	}

	// Token: 0x06004241 RID: 16961 RVA: 0x001AB868 File Offset: 0x001A9A68
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockActivate(string blockName, Vector3i blockPos)
	{
		if (base.OwnerQuest.SharedOwnerID != -1)
		{
			return;
		}
		if (base.Complete)
		{
			return;
		}
		if (base.OwnerQuest.OwnerJournal.ActiveQuest != null)
		{
			return;
		}
		if (this.rallyPos != blockPos)
		{
			return;
		}
		if (TwitchManager.HasInstance && TwitchManager.Current.IsVoting)
		{
			GameManager.ShowTooltip(base.OwnerQuest.OwnerJournal.OwnerPlayer, Localization.Get("ttWaitForVoteQuest", false), false, false, 0f);
			return;
		}
		Vector3 zero = Vector3.zero;
		int num = GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime);
		if (this.startTime != -1 && this.endTime != -1)
		{
			if (this.startTime < this.endTime)
			{
				if (num < this.startTime || num >= this.endTime)
				{
					GameManager.ShowTooltip(base.OwnerQuest.OwnerJournal.OwnerPlayer, string.Format(Localization.Get("ObjectiveRallyPointInvalidStartTime", false), this.startTime, this.endTime), false, false, 0f);
					return;
				}
			}
			else if (num < this.startTime && num >= this.endTime)
			{
				GameManager.ShowTooltip(base.OwnerQuest.OwnerJournal.OwnerPlayer, string.Format(Localization.Get("ObjectiveRallyPointInvalidStartTime", false), this.startTime, this.endTime), false, false, 0f);
				return;
			}
		}
		base.OwnerQuest.RemoveSharedNotInRange();
		if (!base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition))
		{
			if (base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.Location))
			{
				this.RallyPointActivate(zero, true, QuestEventManager.POILockoutReasonTypes.None, 0UL);
			}
			return;
		}
		EntityPlayer ownerPlayer = base.OwnerQuest.OwnerJournal.OwnerPlayer;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			Vector2 prefabPos = new Vector2(zero.x, zero.z);
			ulong extraData;
			QuestEventManager.POILockoutReasonTypes poilockoutReasonTypes = QuestEventManager.Current.CheckForPOILockouts(ownerPlayer.entityId, prefabPos, out extraData);
			this.RallyPointActivate(zero, poilockoutReasonTypes == QuestEventManager.POILockoutReasonTypes.None, poilockoutReasonTypes, extraData);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.TryRallyMarker, ownerPlayer.entityId, zero, base.OwnerQuest.QuestCode), false);
	}

	// Token: 0x06004242 RID: 16962 RVA: 0x001ABA90 File Offset: 0x001A9C90
	public void RallyPointActivate(Vector3 prefabPos, bool activate, QuestEventManager.POILockoutReasonTypes lockoutReason, ulong extraData)
	{
		bool flag = base.OwnerQuest.PositionData.ContainsKey(Quest.PositionDataTypes.POIPosition);
		if (activate)
		{
			if (base.OwnerQuest.CheckRequirements())
			{
				if (!base.OwnerQuest.QuestClass.CanActivate())
				{
					GameManager.ShowTooltip(base.OwnerQuest.OwnerJournal.OwnerPlayer, Localization.Get("questunavailable", false), false, false, 0f);
					return;
				}
				this.HandleParty();
				base.OwnerQuest.RemoveMapObject();
				base.OwnerQuest.RallyMarkerActivated = true;
				EntityPlayer ownerPlayer = base.OwnerQuest.OwnerJournal.OwnerPlayer;
				if (flag)
				{
					base.OwnerQuest.OwnerJournal.ActiveQuest = base.OwnerQuest;
					base.OwnerQuest.Tracked = true;
					base.OwnerQuest.OwnerJournal.TrackedQuest = base.OwnerQuest;
					base.OwnerQuest.OwnerJournal.RefreshTracked();
					if (base.OwnerQuest.PositionData.ContainsKey(Quest.PositionDataTypes.TraderPosition))
					{
						base.OwnerQuest.OwnerJournal.AddPOIToTraderData((int)base.OwnerQuest.QuestClass.DifficultyTier, base.OwnerQuest.PositionData[Quest.PositionDataTypes.TraderPosition], base.OwnerQuest.PositionData[Quest.PositionDataTypes.POIPosition]);
					}
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						GameManager.Instance.StartCoroutine(QuestEventManager.Current.QuestLockPOI(ownerPlayer.entityId, base.OwnerQuest.QuestClass, prefabPos, base.OwnerQuest.QuestTags, base.OwnerQuest.GetSharedWithIDList(), new Action(this.RallyPointActivated)));
					}
					else
					{
						ObjectiveRallyPoint.OutstandingRallyPoint = this;
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.LockPOI, ownerPlayer.entityId, base.OwnerQuest.ID, base.OwnerQuest.QuestTags, prefabPos, base.OwnerQuest.GetSharedWithIDList()), false);
					}
				}
				else
				{
					this.rallyPointActivated();
				}
				if (this.activateEvent != "")
				{
					GameEventManager.Current.HandleAction(this.activateEvent, null, ownerPlayer, false, new Vector3i(prefabPos), "", "", false, true, "", null);
					return;
				}
			}
		}
		else
		{
			EntityPlayer ownerPlayer2 = base.OwnerQuest.OwnerJournal.OwnerPlayer;
			switch (lockoutReason)
			{
			case QuestEventManager.POILockoutReasonTypes.PlayerInside:
				GameManager.ShowTooltip(ownerPlayer2 as EntityPlayerLocal, Localization.Get("poiLockoutPlayerInside", false), false, false, 0f);
				return;
			case QuestEventManager.POILockoutReasonTypes.Bedroll:
				GameManager.ShowTooltip(ownerPlayer2 as EntityPlayerLocal, Localization.Get("poiLockoutBedroll", false), false, false, 0f);
				return;
			case QuestEventManager.POILockoutReasonTypes.LandClaim:
				GameManager.ShowTooltip(ownerPlayer2 as EntityPlayerLocal, Localization.Get("poiLockoutLandClaim", false), false, false, 0f);
				return;
			case QuestEventManager.POILockoutReasonTypes.QuestLock:
			{
				if (extraData == 0UL)
				{
					GameManager.ShowTooltip(ownerPlayer2 as EntityPlayerLocal, Localization.Get("poiLockoutQuestActiveQuesters", false), false, false, 0f);
					return;
				}
				ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(extraData);
				int item = valueTuple.Item2;
				int item2 = valueTuple.Item3;
				GameManager.ShowTooltip(ownerPlayer2 as EntityPlayerLocal, Localization.Get("ttQuestLockedUntil", false), string.Format("{0:00}:{1:00}", item, item2 + 1), null, null, false, false, 0f);
				break;
			}
			default:
				return;
			}
		}
	}

	// Token: 0x06004243 RID: 16963 RVA: 0x001ABDAF File Offset: 0x001A9FAF
	[PublicizedFrom(EAccessModifier.Private)]
	public void rallyPointActivated()
	{
		ObjectiveRallyPoint.OutstandingRallyPoint = null;
		base.CurrentValue = 1;
		this.Refresh();
	}

	// Token: 0x06004244 RID: 16964 RVA: 0x001ABDC4 File Offset: 0x001A9FC4
	public void RallyPointActivated()
	{
		this.rallyPointActivated();
	}

	// Token: 0x06004245 RID: 16965 RVA: 0x001ABDCC File Offset: 0x001A9FCC
	public bool IsActivated()
	{
		return this.currentValue == 1;
	}

	// Token: 0x06004246 RID: 16966 RVA: 0x001ABDD8 File Offset: 0x001A9FD8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleParty()
	{
		EntityPlayer ownerPlayer = base.OwnerQuest.OwnerJournal.OwnerPlayer;
		if (ownerPlayer.Party == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyQuestChange>().Setup(ownerPlayer.entityId, base.OwnerQuest.GetObjectiveIndex(this), true, base.OwnerQuest.QuestCode), true, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyQuestChange>().Setup(ownerPlayer.entityId, base.OwnerQuest.GetObjectiveIndex(this), true, base.OwnerQuest.QuestCode), false);
	}

	// Token: 0x06004247 RID: 16967 RVA: 0x001ABE84 File Offset: 0x001AA084
	public override void Refresh()
	{
		bool complete = base.CurrentValue == 1;
		base.Complete = complete;
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x170006E4 RID: 1764
	// (get) Token: 0x06004248 RID: 16968 RVA: 0x001ABEB9 File Offset: 0x001AA0B9
	public Vector3i RallyPos
	{
		get
		{
			return this.rallyPos;
		}
	}

	// Token: 0x06004249 RID: 16969 RVA: 0x001ABEC4 File Offset: 0x001AA0C4
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 HandleRallyPoint()
	{
		BlockValue blockValue = Block.GetBlockValue("questRallyMarker", false);
		if (this.RallyStartType == ObjectiveRallyPoint.RallyStartTypes.Find)
		{
			if (!base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.POIPosition))
			{
				return Vector3.zero;
			}
			Vector3 vector;
			if (!base.OwnerQuest.GetPositionData(out vector, Quest.PositionDataTypes.POISize))
			{
				return Vector3.zero;
			}
			Vector3i prefabPosition = new Vector3i(this.position);
			int num = 32;
			this.outerRect = new Rect(this.position.x - (float)num, this.position.z - (float)num, vector.x + (float)(num * 2), vector.z + (float)(num * 2));
			Vector3 v;
			if (base.OwnerQuest.GetPositionData(out v, Quest.PositionDataTypes.Activate))
			{
				this.rallyPos = new Vector3i(v);
				this.position = v;
				this.positionSet = ObjectiveRallyPoint.PositionSetTypes.RallyMarkerPosition;
			}
			else
			{
				World world = GameManager.Instance.World;
				if ((this.rallyPos = this.GetRallyPosition(world, prefabPosition, new Vector3i(vector))) != Vector3i.zero)
				{
					BlockValue block = world.GetBlock(this.rallyPos);
					if (!(block.Block is BlockRallyMarker))
					{
						return Vector3.zero;
					}
					base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.Activate, this.rallyPos.ToVector3());
					Chunk chunk = world.GetChunkFromWorldPos(this.rallyPos) as Chunk;
					if (chunk != null)
					{
						BlockEntityData blockEntity = chunk.GetBlockEntity(this.rallyPos);
						if (blockEntity == null || !(blockEntity.transform != null) || block.type != blockValue.type)
						{
							world.SetBlockRPC(chunk.ClrIdx, this.rallyPos, blockValue);
							return Vector3.zero;
						}
						this.setVisibility(blockEntity.transform, this.rallyMarkerType, base.OwnerQuest.OwnerJournal.ActiveQuest == null);
						this.positionSet = ObjectiveRallyPoint.PositionSetTypes.RallyMarkerPosition;
					}
				}
				else
				{
					this.rallyPos = new Vector3i(this.position);
					this.positionSet = ObjectiveRallyPoint.PositionSetTypes.POIPosition;
				}
			}
		}
		else
		{
			base.OwnerQuest.GetPositionData(out this.position, Quest.PositionDataTypes.Location);
			Vector3 v2;
			if (base.OwnerQuest.GetPositionData(out v2, Quest.PositionDataTypes.Activate))
			{
				this.rallyPos = new Vector3i(v2);
			}
			else
			{
				int num2 = (int)this.position.x;
				int num3 = (int)this.position.z;
				int height = (int)GameManager.Instance.World.GetHeight(num2, num3);
				this.rallyPos = new Vector3i(num2, height, num3);
			}
			World world2 = GameManager.Instance.World;
			Chunk chunk2 = world2.GetChunkFromWorldPos(this.rallyPos) as Chunk;
			if (chunk2 == null)
			{
				this.rallyPos = new Vector3i(this.position);
				this.positionSet = ObjectiveRallyPoint.PositionSetTypes.POIPosition;
			}
			else
			{
				BlockValue block2 = chunk2.GetBlock(World.toBlock(this.rallyPos));
				if (block2.ischild)
				{
					this.rallyPos = new Vector3i(this.rallyPos.x + block2.parentx, this.rallyPos.y + block2.parenty, this.rallyPos.z + block2.parentz);
					block2 = chunk2.GetBlock(World.toBlock(this.rallyPos));
					base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.Activate, this.rallyPos.ToVector3());
				}
				if (!(block2.Block is BlockRallyMarker))
				{
					if (!block2.isair && !block2.Block.IsTerrainDecoration)
					{
						this.rallyPos += Vector3i.up;
					}
					GameManager.Instance.World.SetBlockRPC(this.rallyPos, blockValue, sbyte.MaxValue);
					base.OwnerQuest.SetPositionData(Quest.PositionDataTypes.Activate, this.rallyPos.ToVector3());
					return Vector3.zero;
				}
				BlockEntityData blockEntity2 = chunk2.GetBlockEntity(this.rallyPos);
				if (blockEntity2 == null || !(blockEntity2.transform != null) || block2.type != blockValue.type)
				{
					world2.SetBlockRPC(chunk2.ClrIdx, this.rallyPos, blockValue);
					return Vector3.zero;
				}
				this.setVisibility(blockEntity2.transform, this.rallyMarkerType, true);
				this.positionSet = ObjectiveRallyPoint.PositionSetTypes.RallyMarkerPosition;
			}
		}
		this.position = this.rallyPos.ToVector3();
		base.OwnerQuest.HandleMapObject(Quest.PositionDataTypes.Activate, this.NavObjectName, -1);
		return this.rallyPos.ToVector3();
	}

	// Token: 0x0600424A RID: 16970 RVA: 0x001AC30C File Offset: 0x001AA50C
	public Vector3i GetRallyPosition(World _world, Vector3i _prefabPosition, Vector3i _prefabSize)
	{
		int num = World.toChunkXZ(_prefabPosition.x - 1);
		int num2 = World.toChunkXZ(_prefabPosition.x + _prefabSize.x + 1);
		int num3 = World.toChunkXZ(_prefabPosition.z - 1);
		int num4 = World.toChunkXZ(_prefabPosition.z + _prefabSize.z + 1);
		new List<Vector3i>();
		Rect rect = new Rect((float)_prefabPosition.x, (float)_prefabPosition.z, (float)_prefabSize.x, (float)_prefabSize.z);
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				Chunk chunk = _world.GetChunkSync(i, j) as Chunk;
				if (chunk != null)
				{
					List<Vector3i> list = chunk.IndexedBlocks["Rally"];
					if (list != null)
					{
						for (int k = 0; k < list.Count; k++)
						{
							Vector3 vector = chunk.ToWorldPos(list[k]).ToVector3();
							if (rect.Contains(new Vector2(vector.x, vector.z)))
							{
								base.CurrentValue = 2;
								return chunk.ToWorldPos(list[k]);
							}
						}
					}
				}
			}
		}
		return Vector3i.zero;
	}

	// Token: 0x0600424B RID: 16971 RVA: 0x001AC448 File Offset: 0x001AA648
	public override BaseObjective Clone()
	{
		ObjectiveRallyPoint objectiveRallyPoint = new ObjectiveRallyPoint();
		this.CopyValues(objectiveRallyPoint);
		return objectiveRallyPoint;
	}

	// Token: 0x0600424C RID: 16972 RVA: 0x001AC464 File Offset: 0x001AA664
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CopyValues(BaseObjective objective)
	{
		base.CopyValues(objective);
		ObjectiveRallyPoint objectiveRallyPoint = (ObjectiveRallyPoint)objective;
		objectiveRallyPoint.RallyStartType = this.RallyStartType;
		objectiveRallyPoint.startTime = this.startTime;
		objectiveRallyPoint.endTime = this.endTime;
		objectiveRallyPoint.activateEvent = this.activateEvent;
		objectiveRallyPoint.rallyMarkerType = this.rallyMarkerType;
	}

	// Token: 0x0600424D RID: 16973 RVA: 0x001AC4B9 File Offset: 0x001AA6B9
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateState_NeedSetup()
	{
		this.HandleRallyPoint();
		if (this.positionSet != ObjectiveRallyPoint.PositionSetTypes.None)
		{
			base.CurrentValue = 2;
		}
	}

	// Token: 0x0600424E RID: 16974 RVA: 0x001AC4D4 File Offset: 0x001AA6D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateState_Update()
	{
		if (this.positionSet != ObjectiveRallyPoint.PositionSetTypes.RallyMarkerPosition)
		{
			this.HandleRallyPoint();
			if (this.positionSet == ObjectiveRallyPoint.PositionSetTypes.None)
			{
				return;
			}
		}
		if (this.positionSet == ObjectiveRallyPoint.PositionSetTypes.RallyMarkerPosition)
		{
			GameManager.Instance.StartCoroutine(this.setRallyPointVisibility(base.OwnerQuest.OwnerJournal.ActiveQuest == null));
		}
		Vector3 vector = base.OwnerQuest.OwnerJournal.OwnerPlayer.position;
		if (this.RallyStartType == ObjectiveRallyPoint.RallyStartTypes.Create)
		{
			this.currentDistance = Vector3.Distance(vector, this.position + new Vector3(0.5f, 0f, 0.5f));
			if (this.currentDistance > this.distanceNeeded)
			{
				if (base.Description == "" || this.lastDistance <= this.distanceNeeded)
				{
					base.Description = ((base.OwnerQuest.SharedOwnerID != -1) ? ObjectiveRallyPoint.textWaitForActivate : ObjectiveRallyPoint.textHeadToRallyPoint);
				}
			}
			else
			{
				if (this.lastDistance > this.distanceNeeded)
				{
					this.setRallyPointVisible = true;
				}
				if (base.Description == "" || this.lastDistance > this.distanceNeeded)
				{
					base.Description = ((base.OwnerQuest.SharedOwnerID != -1) ? ObjectiveRallyPoint.textWaitForActivate : ObjectiveRallyPoint.textActivateRallyPoint);
				}
			}
			this.lastDistance = this.currentDistance;
			return;
		}
		this.currentDistance = Vector3.Distance(vector, this.position + new Vector3(0.5f, 0f, 0.5f));
		vector.y = vector.z;
		if (this.outerRect.Contains(vector))
		{
			if (base.Description == "" || this.lastDistance > this.distanceNeeded)
			{
				base.Description = ((base.OwnerQuest.SharedOwnerID != -1) ? ObjectiveRallyPoint.textWaitForActivate : ObjectiveRallyPoint.textActivateRallyPoint);
			}
			if (this.lastDistance > this.distanceNeeded)
			{
				this.setRallyPointVisible = true;
			}
			this.lastDistance = -1f;
			return;
		}
		if (this.lastDistance == -1f)
		{
			this.setRallyPointVisible = true;
			this.lastDistance = 1f;
		}
		if (base.Description == "" || this.lastDistance <= this.distanceNeeded)
		{
			base.Description = ((base.OwnerQuest.SharedOwnerID != -1) ? ObjectiveRallyPoint.textWaitForActivate : ObjectiveRallyPoint.textHeadToRallyPoint);
		}
	}

	// Token: 0x0600424F RID: 16975 RVA: 0x001AC72C File Offset: 0x001AA92C
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveRallyPoint.PropRallyStartMode))
		{
			this.RallyStartType = EnumUtils.Parse<ObjectiveRallyPoint.RallyStartTypes>(properties.Values[ObjectiveRallyPoint.PropRallyStartMode], false);
		}
		properties.ParseInt(ObjectiveRallyPoint.PropAllowedStartTime, ref this.startTime);
		properties.ParseInt(ObjectiveRallyPoint.PropAllowedEndTime, ref this.endTime);
		properties.ParseString(ObjectiveRallyPoint.PropActivateEvent, ref this.activateEvent);
		properties.ParseString(ObjectiveRallyPoint.PropRallyMarkerType, ref this.rallyMarkerType);
	}

	// Token: 0x06004250 RID: 16976 RVA: 0x001AC7B2 File Offset: 0x001AA9B2
	public void RallyPointRefresh()
	{
		GameManager.Instance.StartCoroutine(this.setRallyPointVisibility(true));
	}

	// Token: 0x04003481 RID: 13441
	public ObjectiveRallyPoint.RallyStartTypes RallyStartType;

	// Token: 0x04003482 RID: 13442
	public static string PropRallyStartMode = "start_mode";

	// Token: 0x04003483 RID: 13443
	public static string PropAllowedStartTime = "allowed_start_hour";

	// Token: 0x04003484 RID: 13444
	public static string PropAllowedEndTime = "allowed_end_hour";

	// Token: 0x04003485 RID: 13445
	public static string PropActivateEvent = "activate_event";

	// Token: 0x04003486 RID: 13446
	public static string PropRallyMarkerType = "rally_marker_type";

	// Token: 0x04003487 RID: 13447
	public static ObjectiveRallyPoint OutstandingRallyPoint = null;

	// Token: 0x04003488 RID: 13448
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 position;

	// Token: 0x04003489 RID: 13449
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName = "";

	// Token: 0x0400348A RID: 13450
	[PublicizedFrom(EAccessModifier.Protected)]
	public float currentDistance;

	// Token: 0x0400348B RID: 13451
	[PublicizedFrom(EAccessModifier.Private)]
	public Rect outerRect;

	// Token: 0x0400348C RID: 13452
	[PublicizedFrom(EAccessModifier.Private)]
	public int startTime = -1;

	// Token: 0x0400348D RID: 13453
	[PublicizedFrom(EAccessModifier.Private)]
	public int endTime = -1;

	// Token: 0x0400348E RID: 13454
	[PublicizedFrom(EAccessModifier.Private)]
	public ObjectiveRallyPoint.PositionSetTypes positionSet;

	// Token: 0x0400348F RID: 13455
	[PublicizedFrom(EAccessModifier.Private)]
	public static string textActivateRallyPoint;

	// Token: 0x04003490 RID: 13456
	[PublicizedFrom(EAccessModifier.Private)]
	public static string textHeadToRallyPoint;

	// Token: 0x04003491 RID: 13457
	[PublicizedFrom(EAccessModifier.Private)]
	public static string textWaitForActivate;

	// Token: 0x04003492 RID: 13458
	[PublicizedFrom(EAccessModifier.Protected)]
	public string rallyMarkerType = "questRallyMarker";

	// Token: 0x04003493 RID: 13459
	[PublicizedFrom(EAccessModifier.Protected)]
	public string activateEvent = "";

	// Token: 0x04003494 RID: 13460
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isRallyVisible;

	// Token: 0x04003495 RID: 13461
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i rallyPos = Vector3i.zero;

	// Token: 0x04003496 RID: 13462
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastDistance = -1f;

	// Token: 0x04003497 RID: 13463
	[PublicizedFrom(EAccessModifier.Private)]
	public float distanceNeeded = 50f;

	// Token: 0x04003498 RID: 13464
	[PublicizedFrom(EAccessModifier.Private)]
	public bool setRallyPointVisible;

	// Token: 0x020008CD RID: 2253
	public enum RallyStartTypes
	{
		// Token: 0x0400349A RID: 13466
		Find,
		// Token: 0x0400349B RID: 13467
		Create
	}

	// Token: 0x020008CE RID: 2254
	[PublicizedFrom(EAccessModifier.Private)]
	public enum PositionSetTypes
	{
		// Token: 0x0400349D RID: 13469
		None,
		// Token: 0x0400349E RID: 13470
		POIPosition,
		// Token: 0x0400349F RID: 13471
		RallyMarkerPosition
	}
}
