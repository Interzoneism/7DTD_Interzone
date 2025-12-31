using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using GameEvent.GameEventHelpers;
using UnityEngine;

// Token: 0x020004AF RID: 1199
public class GameEventManager
{
	// Token: 0x1700040D RID: 1037
	// (get) Token: 0x0600272E RID: 10030 RVA: 0x000FD709 File Offset: 0x000FB909
	public static GameEventManager Current
	{
		get
		{
			if (GameEventManager.instance == null)
			{
				GameEventManager.instance = new GameEventManager();
			}
			return GameEventManager.instance;
		}
	}

	// Token: 0x0600272F RID: 10031 RVA: 0x000FD724 File Offset: 0x000FB924
	[PublicizedFrom(EAccessModifier.Private)]
	public GameEventManager()
	{
		this.Random = GameRandomManager.Instance.CreateGameRandom();
	}

	// Token: 0x1700040E RID: 1038
	// (get) Token: 0x06002730 RID: 10032 RVA: 0x000FD7DE File Offset: 0x000FB9DE
	public static bool HasInstance
	{
		get
		{
			return GameEventManager.instance != null;
		}
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000FD7E8 File Offset: 0x000FB9E8
	public void AddSequence(GameEventActionSequence action)
	{
		if (!GameEventManager.GameEventSequences.ContainsKey(action.Name))
		{
			GameEventManager.GameEventSequences.Add(action.Name, action);
		}
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000FD80D File Offset: 0x000FBA0D
	public void Cleanup()
	{
		this.ClearActions();
		this.GameEventFlags.Clear();
		this.BossGroups.Clear();
		this.CurrentBossGroup = null;
		this.HomerunManager.Cleanup();
	}

	// Token: 0x06002733 RID: 10035 RVA: 0x000FD83D File Offset: 0x000FBA3D
	public void ClearActions()
	{
		this.ActionSequenceUpdates.Clear();
		GameEventManager.GameEventSequences.Clear();
		this.CategoryList.Clear();
		this.spawnEntries.Clear();
		this.blockEntries.Clear();
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x06002734 RID: 10036 RVA: 0x000FD875 File Offset: 0x000FBA75
	// (set) Token: 0x06002735 RID: 10037 RVA: 0x000FD880 File Offset: 0x000FBA80
	public BossGroup CurrentBossGroup
	{
		get
		{
			return this.currentBossGroup;
		}
		set
		{
			if (this.currentBossGroup == value)
			{
				return;
			}
			if (this.currentBossGroup != null)
			{
				this.currentBossGroup.IsCurrent = false;
				this.currentBossGroup.RemoveNavObjects();
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
				{
					this.currentBossGroup.MinionEntities = null;
				}
			}
			this.currentBossGroup = value;
			if (this.currentBossGroup != null)
			{
				this.currentBossGroup.IsCurrent = true;
				this.currentBossGroup.RequestStatRefresh();
				this.currentBossGroup.AddNavObjects();
			}
		}
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06002736 RID: 10038 RVA: 0x000FD8FF File Offset: 0x000FBAFF
	public int CurrentCount
	{
		get
		{
			return this.spawnEntries.Count + this.ReservedCount;
		}
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000FD913 File Offset: 0x000FBB13
	public GameEventActionSequence.TargetTypes GetTargetType(string gameEventName)
	{
		if (GameEventManager.GameEventSequences.ContainsKey(gameEventName))
		{
			return GameEventManager.GameEventSequences[gameEventName].TargetType;
		}
		return GameEventActionSequence.TargetTypes.Entity;
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x000FD934 File Offset: 0x000FBB34
	public bool HandleAction(string name, EntityPlayer requester, Entity entity, bool twitchActivated, string extraData = "", string tag = "", bool crateShare = false, bool allowRefunds = true, string sequenceLink = "", GameEventActionSequence ownerSeq = null)
	{
		return this.HandleAction(name, requester, entity, twitchActivated, Vector3i.zero, extraData, tag, crateShare, allowRefunds, sequenceLink, ownerSeq);
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000FD964 File Offset: 0x000FBB64
	public bool HandleAction(string name, EntityPlayer requester, Entity entity, bool twitchActivated, Vector3 targetPosition, string extraData = "", string tag = "", bool crateShare = false, bool allowRefunds = true, string sequenceLink = "", GameEventActionSequence ownerSeq = null)
	{
		if (name != null && name.Contains(','))
		{
			string[] array = name.Split(',', StringSplitOptions.None);
			bool result = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (this.HandleAction(array[i], requester, entity, twitchActivated, extraData, tag, crateShare, allowRefunds, sequenceLink, ownerSeq))
				{
					result = true;
				}
			}
			return result;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return this.HandleActionClient(name, entity, twitchActivated, targetPosition, extraData, tag, crateShare, allowRefunds, sequenceLink);
		}
		if (GameEventManager.GameEventSequences.ContainsKey(name))
		{
			GameEventActionSequence gameEventActionSequence = GameEventManager.GameEventSequences[name];
			if (gameEventActionSequence.CanPerform(entity))
			{
				if (gameEventActionSequence.SingleInstance)
				{
					for (int j = 0; j < this.ActionSequenceUpdates.Count; j++)
					{
						if (this.ActionSequenceUpdates[j].Name == name)
						{
							return false;
						}
					}
				}
				GameEventActionSequence gameEventActionSequence2 = gameEventActionSequence.Clone();
				gameEventActionSequence2.Target = entity;
				gameEventActionSequence2.TargetPosition = targetPosition;
				if (ownerSeq == null && sequenceLink != "" && requester != null)
				{
					ownerSeq = this.GetSequenceLink(requester, sequenceLink);
				}
				if (ownerSeq != null)
				{
					gameEventActionSequence2.Requester = ownerSeq.Requester;
					gameEventActionSequence2.ExtraData = ownerSeq.ExtraData;
					gameEventActionSequence2.CrateShare = ownerSeq.CrateShare;
					gameEventActionSequence2.Tag = ownerSeq.Tag;
					gameEventActionSequence2.AllowRefunds = ownerSeq.AllowRefunds;
					gameEventActionSequence2.TwitchActivated = ownerSeq.TwitchActivated;
				}
				else
				{
					gameEventActionSequence2.Requester = requester;
					gameEventActionSequence2.ExtraData = extraData;
					gameEventActionSequence2.CrateShare = crateShare;
					gameEventActionSequence2.Tag = tag;
					gameEventActionSequence2.AllowRefunds = allowRefunds;
					gameEventActionSequence2.TwitchActivated = twitchActivated;
				}
				gameEventActionSequence2.OwnerSequence = ownerSeq;
				if (gameEventActionSequence2.TargetType != GameEventActionSequence.TargetTypes.Entity)
				{
					gameEventActionSequence2.POIPosition = new Vector3i(targetPosition);
				}
				gameEventActionSequence2.SetupTarget();
				this.ActionSequenceUpdates.Add(gameEventActionSequence2);
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000FDB48 File Offset: 0x000FBD48
	public bool HandleActionClient(string name, Entity entity, bool twitchActivated, string extraData = "", string tag = "", bool crateShare = false, bool allowRefunds = true, string sequenceLink = "")
	{
		return this.HandleActionClient(name, entity, twitchActivated, Vector3.zero, extraData, tag, crateShare, allowRefunds, sequenceLink);
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000FDB70 File Offset: 0x000FBD70
	public bool HandleActionClient(string name, Entity entity, bool twitchActivated, Vector3 targetPosition, string extraData = "", string tag = "", bool crateShare = false, bool allowRefunds = true, string sequenceLink = "")
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageGameEventRequest>().Setup(name, entity ? entity.entityId : -1, twitchActivated, targetPosition, extraData, tag, crateShare, allowRefunds, sequenceLink), false);
		return true;
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000FDBB4 File Offset: 0x000FBDB4
	public void Update(float deltaTime)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && GameManager.Instance.World != null)
		{
			this.HandleSpawnUpdates(deltaTime);
			this.HandleActionUpdates();
			this.HandleBlockUpdates(deltaTime);
			this.HandleEventFlagUpdates(deltaTime);
			this.HandleBossGroupUpdates(deltaTime);
			this.HomerunManager.Update(deltaTime);
		}
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000FDC08 File Offset: 0x000FBE08
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleSpawnUpdates(float deltaTime)
	{
		bool flag = false;
		if (this.spawnEntries.Count > 0)
		{
			this.attackTimerUpdate -= deltaTime;
			if (this.attackTimerUpdate <= 0f)
			{
				flag = true;
				this.attackTimerUpdate = 2f;
			}
		}
		for (int i = this.spawnEntries.Count - 1; i >= 0; i--)
		{
			GameEventManager.SpawnEntry spawnEntry = this.spawnEntries[i];
			if (spawnEntry.SpawnedEntity.IsDespawned)
			{
				spawnEntry.GameEvent.HasDespawn = true;
				this.spawnEntries.RemoveAt(i);
				if (spawnEntry.Requester != null)
				{
					if (spawnEntry.Requester is EntityPlayerLocal)
					{
						GameEventManager.Current.HandleGameEntityDespawned(spawnEntry.SpawnedEntity.entityId);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(NetPackageGameEventResponse.ResponseTypes.EntityDespawned, spawnEntry.SpawnedEntity.entityId, -1, "", false), false, spawnEntry.Requester.entityId, -1, -1, null, 192, false);
					}
				}
			}
			else if (!spawnEntry.SpawnedEntity.IsAlive() || spawnEntry.SpawnedEntity.emodel == null)
			{
				this.spawnEntries.RemoveAt(i);
				if (spawnEntry.Requester != null)
				{
					if (spawnEntry.Requester is EntityPlayerLocal)
					{
						GameEventManager.Current.HandleGameEntityKilled(spawnEntry.SpawnedEntity.entityId);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(NetPackageGameEventResponse.ResponseTypes.EntityKilled, spawnEntry.SpawnedEntity.entityId, -1, "", false), false, spawnEntry.Requester.entityId, -1, -1, null, 192, false);
					}
				}
			}
			else if (flag)
			{
				spawnEntry.HandleUpdate();
			}
		}
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x000FDDD4 File Offset: 0x000FBFD4
	public void RemoveSpawnedEntry(Entity spawnedEntity)
	{
		for (int i = this.spawnEntries.Count - 1; i >= 0; i--)
		{
			if (this.spawnEntries[i].SpawnedEntity == spawnedEntity)
			{
				GameEventManager.SpawnEntry spawnEntry = this.spawnEntries[i];
				spawnEntry.GameEvent.HasDespawn = true;
				this.spawnEntries.RemoveAt(i);
				if (spawnEntry.Requester != null)
				{
					if (spawnEntry.Requester is EntityPlayerLocal)
					{
						GameEventManager.Current.HandleGameEntityDespawned(spawnEntry.SpawnedEntity.entityId);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(NetPackageGameEventResponse.ResponseTypes.EntityDespawned, spawnEntry.SpawnedEntity.entityId, -1, "", false), false, spawnEntry.Requester.entityId, -1, -1, null, 192, false);
					}
				}
			}
		}
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x000FDEB8 File Offset: 0x000FC0B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleActionUpdates()
	{
		for (int i = 0; i < this.ActionSequenceUpdates.Count; i++)
		{
			GameEventActionSequence gameEventActionSequence = this.ActionSequenceUpdates[i];
			try
			{
				if (gameEventActionSequence.StartTime <= 0f)
				{
					gameEventActionSequence.StartSequence(this);
				}
				gameEventActionSequence.Update();
			}
			catch
			{
				if (gameEventActionSequence != null)
				{
					Log.Error("Exception while updating action sequence " + gameEventActionSequence.Name);
				}
				throw;
			}
		}
		for (int j = this.ActionSequenceUpdates.Count - 1; j >= 0; j--)
		{
			GameEventActionSequence gameEventActionSequence2 = this.ActionSequenceUpdates[j];
			if (!gameEventActionSequence2.HasTarget() && gameEventActionSequence2.AllowRefunds)
			{
				gameEventActionSequence2.IsComplete = true;
			}
			if (gameEventActionSequence2.IsComplete)
			{
				this.ReservedCount -= gameEventActionSequence2.ReservedSpawnCount;
				this.ActionSequenceUpdates.RemoveAt(j);
			}
		}
	}

	// Token: 0x06002740 RID: 10048 RVA: 0x000FDF94 File Offset: 0x000FC194
	public void RegisterSpawnedEntity(Entity spawned, Entity target, EntityPlayer requester, GameEventActionSequence gameEvent, bool isAggressive = true)
	{
		this.spawnEntries.Add(new GameEventManager.SpawnEntry
		{
			SpawnedEntity = (spawned as EntityAlive),
			Target = (target as EntityAlive),
			Requester = requester,
			GameEvent = gameEvent
		});
	}

	// Token: 0x06002741 RID: 10049 RVA: 0x000FDFD0 File Offset: 0x000FC1D0
	public GameEventManager.SpawnedBlocksEntry RegisterSpawnedBlocks(List<Vector3i> blockList, Entity target, EntityPlayer requester, GameEventActionSequence gameEvent, float timeAlive, string removeSound, Vector3 center, bool refundOnRemove)
	{
		GameEventManager.SpawnedBlocksEntry spawnedBlocksEntry = new GameEventManager.SpawnedBlocksEntry
		{
			BlockList = blockList,
			Target = target,
			Requester = requester,
			GameEvent = gameEvent,
			TimeAlive = timeAlive,
			RemoveSound = removeSound,
			Center = center,
			RefundOnRemove = refundOnRemove
		};
		this.blockEntries.Add(spawnedBlocksEntry);
		return spawnedBlocksEntry;
	}

	// Token: 0x1400002B RID: 43
	// (add) Token: 0x06002742 RID: 10050 RVA: 0x000FE030 File Offset: 0x000FC230
	// (remove) Token: 0x06002743 RID: 10051 RVA: 0x000FE068 File Offset: 0x000FC268
	public event OnGameEventAccessApproved GameEventAccessApproved;

	// Token: 0x06002744 RID: 10052 RVA: 0x000FE09D File Offset: 0x000FC29D
	public void HandleGameEventAccessApproved()
	{
		if (this.GameEventAccessApproved != null)
		{
			this.GameEventAccessApproved();
		}
	}

	// Token: 0x1400002C RID: 44
	// (add) Token: 0x06002745 RID: 10053 RVA: 0x000FE0B4 File Offset: 0x000FC2B4
	// (remove) Token: 0x06002746 RID: 10054 RVA: 0x000FE0EC File Offset: 0x000FC2EC
	public event OnGameEntityAdded GameEntitySpawned;

	// Token: 0x06002747 RID: 10055 RVA: 0x000FE121 File Offset: 0x000FC321
	public void HandleGameEntitySpawned(string gameEventID, int entityID, string tag)
	{
		if (this.GameEntitySpawned != null)
		{
			this.GameEntitySpawned(gameEventID, entityID, tag);
		}
	}

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x06002748 RID: 10056 RVA: 0x000FE13C File Offset: 0x000FC33C
	// (remove) Token: 0x06002749 RID: 10057 RVA: 0x000FE174 File Offset: 0x000FC374
	public event OnGameEntityChanged GameEntityDespawned;

	// Token: 0x0600274A RID: 10058 RVA: 0x000FE1A9 File Offset: 0x000FC3A9
	public void HandleGameEntityDespawned(int entityID)
	{
		if (this.GameEntityDespawned != null)
		{
			this.GameEntityDespawned(entityID);
		}
	}

	// Token: 0x1400002E RID: 46
	// (add) Token: 0x0600274B RID: 10059 RVA: 0x000FE1C0 File Offset: 0x000FC3C0
	// (remove) Token: 0x0600274C RID: 10060 RVA: 0x000FE1F8 File Offset: 0x000FC3F8
	public event OnGameEntityChanged GameEntityKilled;

	// Token: 0x0600274D RID: 10061 RVA: 0x000FE22D File Offset: 0x000FC42D
	public void HandleGameEntityKilled(int entityID)
	{
		if (this.GameEntityKilled != null)
		{
			this.GameEntityKilled(entityID);
		}
	}

	// Token: 0x1400002F RID: 47
	// (add) Token: 0x0600274E RID: 10062 RVA: 0x000FE244 File Offset: 0x000FC444
	// (remove) Token: 0x0600274F RID: 10063 RVA: 0x000FE27C File Offset: 0x000FC47C
	public event OnGameBlocksAdded GameBlocksAdded;

	// Token: 0x06002750 RID: 10064 RVA: 0x000FE2B1 File Offset: 0x000FC4B1
	public void HandleGameBlocksAdded(string gameEventID, int blockGroupID, List<Vector3i> blockList, string tag)
	{
		if (this.GameBlocksAdded != null)
		{
			this.GameBlocksAdded(gameEventID, blockGroupID, blockList, tag);
		}
	}

	// Token: 0x14000030 RID: 48
	// (add) Token: 0x06002751 RID: 10065 RVA: 0x000FE2CC File Offset: 0x000FC4CC
	// (remove) Token: 0x06002752 RID: 10066 RVA: 0x000FE304 File Offset: 0x000FC504
	public event OnGameBlockRemoved GameBlockRemoved;

	// Token: 0x06002753 RID: 10067 RVA: 0x000FE33C File Offset: 0x000FC53C
	public void BlockRemoved(Vector3i blockPos)
	{
		for (int i = 0; i < this.blockEntries.Count; i++)
		{
			if (this.blockEntries[i].RemoveBlock(blockPos))
			{
				if (this.blockEntries[i].Requester is EntityPlayerLocal)
				{
					GameEventManager.Current.HandleGameBlockRemoved(blockPos);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(NetPackageGameEventResponse.ResponseTypes.BlockRemoved, blockPos), false, this.blockEntries[i].Requester.entityId, -1, -1, null, 192, false);
				}
				if (this.blockEntries[i].BlockList.Count == 0)
				{
					this.blockEntries.RemoveAt(i);
				}
				return;
			}
		}
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000FE405 File Offset: 0x000FC605
	public void HandleGameBlockRemoved(Vector3i blockPos)
	{
		if (this.GameBlockRemoved != null)
		{
			this.GameBlockRemoved(blockPos);
		}
	}

	// Token: 0x14000031 RID: 49
	// (add) Token: 0x06002755 RID: 10069 RVA: 0x000FE41C File Offset: 0x000FC61C
	// (remove) Token: 0x06002756 RID: 10070 RVA: 0x000FE454 File Offset: 0x000FC654
	public event OnGameBlocksRemoved GameBlocksRemoved;

	// Token: 0x06002757 RID: 10071 RVA: 0x000FE489 File Offset: 0x000FC689
	public void HandleGameBlocksRemoved(int blockGroupID, bool isDespawn)
	{
		if (this.GameBlocksRemoved != null)
		{
			this.GameBlocksRemoved(blockGroupID, isDespawn);
		}
	}

	// Token: 0x14000032 RID: 50
	// (add) Token: 0x06002758 RID: 10072 RVA: 0x000FE4A0 File Offset: 0x000FC6A0
	// (remove) Token: 0x06002759 RID: 10073 RVA: 0x000FE4D8 File Offset: 0x000FC6D8
	public event OnGameEventStatus GameEventApproved;

	// Token: 0x0600275A RID: 10074 RVA: 0x000FE50D File Offset: 0x000FC70D
	public void HandleGameEventApproved(string gameEventID, int targetEntityID, string extraData, string tag)
	{
		if (this.GameEventApproved != null)
		{
			this.GameEventApproved(gameEventID, targetEntityID, extraData, tag);
		}
	}

	// Token: 0x14000033 RID: 51
	// (add) Token: 0x0600275B RID: 10075 RVA: 0x000FE528 File Offset: 0x000FC728
	// (remove) Token: 0x0600275C RID: 10076 RVA: 0x000FE560 File Offset: 0x000FC760
	public event OnGameEventStatus GameEventDenied;

	// Token: 0x0600275D RID: 10077 RVA: 0x000FE595 File Offset: 0x000FC795
	public void HandleGameEventDenied(string gameEventID, int targetEntityID, string extraData, string tag)
	{
		if (this.GameEventDenied != null)
		{
			this.GameEventDenied(gameEventID, targetEntityID, extraData, tag);
		}
	}

	// Token: 0x14000034 RID: 52
	// (add) Token: 0x0600275E RID: 10078 RVA: 0x000FE5B0 File Offset: 0x000FC7B0
	// (remove) Token: 0x0600275F RID: 10079 RVA: 0x000FE5E8 File Offset: 0x000FC7E8
	public event OnGameEventStatus TwitchPartyGameEventApproved;

	// Token: 0x06002760 RID: 10080 RVA: 0x000FE61D File Offset: 0x000FC81D
	public void HandleTwitchPartyGameEventApproved(string gameEventID, int targetEntityID, string extraData, string tag)
	{
		if (this.TwitchPartyGameEventApproved != null)
		{
			this.TwitchPartyGameEventApproved(gameEventID, targetEntityID, extraData, tag);
		}
	}

	// Token: 0x14000035 RID: 53
	// (add) Token: 0x06002761 RID: 10081 RVA: 0x000FE638 File Offset: 0x000FC838
	// (remove) Token: 0x06002762 RID: 10082 RVA: 0x000FE670 File Offset: 0x000FC870
	public event OnGameEventStatus TwitchRefundNeeded;

	// Token: 0x06002763 RID: 10083 RVA: 0x000FE6A5 File Offset: 0x000FC8A5
	public void HandleTwitchRefundNeeded(string gameEventID, int targetEntityID, string extraData, string tag)
	{
		if (this.TwitchRefundNeeded != null)
		{
			this.TwitchRefundNeeded(gameEventID, targetEntityID, extraData, tag);
		}
	}

	// Token: 0x14000036 RID: 54
	// (add) Token: 0x06002764 RID: 10084 RVA: 0x000FE6C0 File Offset: 0x000FC8C0
	// (remove) Token: 0x06002765 RID: 10085 RVA: 0x000FE6F8 File Offset: 0x000FC8F8
	public event OnGameEventStatus GameEventCompleted;

	// Token: 0x06002766 RID: 10086 RVA: 0x000FE72D File Offset: 0x000FC92D
	public void HandleGameEventCompleted(string gameEventID, int targetEntityID, string extraData, string tag)
	{
		if (this.GameEventCompleted != null)
		{
			this.GameEventCompleted(gameEventID, targetEntityID, extraData, tag);
		}
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000FE748 File Offset: 0x000FC948
	public void HandleGameEventSequenceItemForClient(string gameEventID, int index)
	{
		EntityPlayer player = XUiM_Player.GetPlayer();
		GameEventManager.GameEventSequences[gameEventID].HandleClientPerform(player, index);
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000FE770 File Offset: 0x000FC970
	public void HandleTwitchSetOwner(int targetEntityID, int entitySpawnedID, string extraData)
	{
		EntityAlive entityAlive = GameManager.Instance.World.GetEntity(entitySpawnedID) as EntityAlive;
		if (entityAlive != null)
		{
			entityAlive.SetSpawnByData(targetEntityID, extraData);
		}
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000FE7A4 File Offset: 0x000FC9A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleBlockUpdates(float deltaTime)
	{
		for (int i = this.blockEntries.Count - 1; i >= 0; i--)
		{
			GameEventManager.SpawnedBlocksEntry spawnedBlocksEntry = this.blockEntries[i];
			if (spawnedBlocksEntry.TimeAlive > 0f)
			{
				spawnedBlocksEntry.TimeAlive -= deltaTime;
			}
			else if (spawnedBlocksEntry.TimeAlive != -1f)
			{
				if (spawnedBlocksEntry.TryRemoveBlocks())
				{
					this.blockEntries.RemoveAt(i);
				}
				else
				{
					spawnedBlocksEntry.TimeAlive = 5f;
				}
			}
			if (spawnedBlocksEntry.IsRefunded)
			{
				this.blockEntries.RemoveAt(i);
			}
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000FE838 File Offset: 0x000FCA38
	public void RefundSpawnedBlock(Vector3i blockPos)
	{
		for (int i = 0; i < this.blockEntries.Count; i++)
		{
			GameEventManager.SpawnedBlocksEntry spawnedBlocksEntry = this.blockEntries[i];
			if (spawnedBlocksEntry.BlockList.Contains(blockPos) && !spawnedBlocksEntry.IsRefunded)
			{
				spawnedBlocksEntry.GameEvent.SetRefundNeeded();
				spawnedBlocksEntry.IsRefunded = true;
			}
		}
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000FE890 File Offset: 0x000FCA90
	public void SendBlockDamageUpdate(Vector3i blockPos)
	{
		for (int i = 0; i < this.blockEntries.Count; i++)
		{
			GameEventManager.SpawnedBlocksEntry spawnedBlocksEntry = this.blockEntries[i];
			if (spawnedBlocksEntry.BlockList.Contains(blockPos))
			{
				spawnedBlocksEntry.GameEvent.EventVariables.ModifyEventVariable("Damaged", GameEventVariables.OperationTypes.Add, 1, int.MinValue, int.MaxValue);
			}
		}
	}

	// Token: 0x0600276C RID: 10092 RVA: 0x000FE8F0 File Offset: 0x000FCAF0
	public void SetGameEventFlag(GameEventManager.GameEventFlagTypes flag, bool value, float duration)
	{
		if (value)
		{
			for (int i = 0; i < this.GameEventFlags.Count; i++)
			{
				if (this.GameEventFlags[i].FlagType == flag)
				{
					this.GameEventFlags[i].Duration = duration;
					return;
				}
			}
			this.GameEventFlags.Add(new GameEventManager.GameEventFlag
			{
				FlagType = flag,
				Duration = duration
			});
			this.HandleFlagChanged(flag, false, true);
			return;
		}
		for (int j = 0; j < this.GameEventFlags.Count; j++)
		{
			if (this.GameEventFlags[j].FlagType == flag)
			{
				this.GameEventFlags.RemoveAt(j);
				this.HandleFlagChanged(flag, true, false);
				return;
			}
		}
	}

	// Token: 0x0600276D RID: 10093 RVA: 0x000FE9A8 File Offset: 0x000FCBA8
	public bool CheckGameEventFlag(GameEventManager.GameEventFlagTypes flag)
	{
		for (int i = 0; i < this.GameEventFlags.Count; i++)
		{
			if (this.GameEventFlags[i].FlagType == flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600276E RID: 10094 RVA: 0x000FE9E4 File Offset: 0x000FCBE4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleEventFlagUpdates(float deltaTime)
	{
		for (int i = this.GameEventFlags.Count - 1; i >= 0; i--)
		{
			GameEventManager.GameEventFlag gameEventFlag = this.GameEventFlags[i];
			if (gameEventFlag.Duration > 0f)
			{
				gameEventFlag.Duration -= deltaTime;
				this.HandleFlagBuffUpdates(gameEventFlag.FlagType, deltaTime);
				if (gameEventFlag.Duration <= 0f)
				{
					this.GameEventFlags.RemoveAt(i);
					this.HandleFlagChanged(gameEventFlag.FlagType, true, false);
				}
			}
		}
	}

	// Token: 0x0600276F RID: 10095 RVA: 0x000FEA68 File Offset: 0x000FCC68
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleFlagBuffUpdates(GameEventManager.GameEventFlagTypes flag, float deltaTime)
	{
		this.gameFlagCheckTime -= deltaTime;
		if (this.gameFlagCheckTime <= 0f)
		{
			string name = "";
			switch (flag)
			{
			case GameEventManager.GameEventFlagTypes.BigHead:
				name = "twitch_buffBigHead";
				break;
			case GameEventManager.GameEventFlagTypes.Dancing:
				name = "twitch_buffDance";
				break;
			case GameEventManager.GameEventFlagTypes.BucketHead:
				name = "twitch_buffBucketHead";
				break;
			case GameEventManager.GameEventFlagTypes.TinyZombies:
				name = "twitch_buffTinyZombies";
				break;
			}
			foreach (EntityPlayer entityPlayer in GameManager.Instance.World.Players.dict.Values)
			{
				if (!entityPlayer.Buffs.HasBuff(name))
				{
					entityPlayer.Buffs.AddBuff(name, -1, true, false, -1f);
				}
			}
			this.gameFlagCheckTime = 1f;
		}
	}

	// Token: 0x06002770 RID: 10096 RVA: 0x000FEB50 File Offset: 0x000FCD50
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleFlagChanged(GameEventManager.GameEventFlagTypes flag, bool oldValue, bool newValue)
	{
		switch (flag)
		{
		case GameEventManager.GameEventFlagTypes.BigHead:
			foreach (Entity entity in GameManager.Instance.World.Entities.dict.Values)
			{
				EntityAlive entityAlive = entity as EntityAlive;
				if (entityAlive != null && !(entityAlive is EntityPlayer))
				{
					if (newValue)
					{
						entityAlive.Buffs.AddBuff("twitch_bighead", -1, true, false, -1f);
					}
					else
					{
						entityAlive.Buffs.RemoveBuff("twitch_bighead", true);
					}
				}
			}
			using (Dictionary<int, EntityPlayer>.ValueCollection.Enumerator enumerator2 = GameManager.Instance.World.Players.dict.Values.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					EntityPlayer entityPlayer = enumerator2.Current;
					if (newValue)
					{
						entityPlayer.Buffs.AddBuff("twitch_buffBigHead", -1, true, false, -1f);
					}
					else
					{
						entityPlayer.Buffs.RemoveBuff("twitch_buffBigHead", true);
					}
				}
				return;
			}
			break;
		case GameEventManager.GameEventFlagTypes.Dancing:
			break;
		case GameEventManager.GameEventFlagTypes.BucketHead:
			goto IL_215;
		case GameEventManager.GameEventFlagTypes.TinyZombies:
			goto IL_321;
		default:
			return;
		}
		foreach (Entity entity2 in GameManager.Instance.World.Entities.dict.Values)
		{
			EntityAlive entityAlive2 = entity2 as EntityAlive;
			if (entityAlive2 != null && !(entityAlive2 is EntityPlayer))
			{
				if (newValue)
				{
					entityAlive2.Buffs.AddBuff("twitch_dance", -1, true, false, -1f);
				}
				else
				{
					entityAlive2.Buffs.RemoveBuff("twitch_dance", true);
				}
			}
		}
		using (Dictionary<int, EntityPlayer>.ValueCollection.Enumerator enumerator2 = GameManager.Instance.World.Players.dict.Values.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				EntityPlayer entityPlayer2 = enumerator2.Current;
				if (newValue)
				{
					entityPlayer2.Buffs.AddBuff("twitch_buffDance", -1, true, false, -1f);
				}
				else
				{
					entityPlayer2.Buffs.RemoveBuff("twitch_buffDance", true);
				}
			}
			return;
		}
		IL_215:
		foreach (Entity entity3 in GameManager.Instance.World.Entities.dict.Values)
		{
			EntityAlive entityAlive3 = entity3 as EntityAlive;
			if (entityAlive3 != null && !(entityAlive3 is EntityPlayer) && !(entityAlive3 is EntityVehicle))
			{
				if (newValue)
				{
					entityAlive3.Buffs.AddBuff("twitch_buckethead", -1, true, false, -1f);
				}
				else
				{
					entityAlive3.Buffs.RemoveBuff("twitch_buckethead", true);
				}
			}
		}
		using (Dictionary<int, EntityPlayer>.ValueCollection.Enumerator enumerator2 = GameManager.Instance.World.Players.dict.Values.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				EntityPlayer entityPlayer3 = enumerator2.Current;
				if (newValue)
				{
					entityPlayer3.Buffs.AddBuff("twitch_buffBucketHead", -1, true, false, -1f);
				}
				else
				{
					entityPlayer3.Buffs.RemoveBuff("twitch_buffBucketHead", true);
				}
			}
			return;
		}
		IL_321:
		foreach (Entity entity4 in GameManager.Instance.World.Entities.dict.Values)
		{
			EntityAlive entityAlive4 = entity4 as EntityAlive;
			if (entityAlive4 != null && entityAlive4 is EntityZombie)
			{
				if (newValue)
				{
					entityAlive4.Buffs.AddBuff("twitch_tiny", -1, true, false, -1f);
				}
				else
				{
					entityAlive4.Buffs.RemoveBuff("twitch_tiny", true);
				}
			}
		}
		foreach (EntityPlayer entityPlayer4 in GameManager.Instance.World.Players.dict.Values)
		{
			if (newValue)
			{
				entityPlayer4.Buffs.AddBuff("twitch_buffTinyZombies", -1, true, false, -1f);
			}
			else
			{
				entityPlayer4.Buffs.RemoveBuff("twitch_buffTinyZombies", true);
			}
		}
	}

	// Token: 0x06002771 RID: 10097 RVA: 0x000FEFE4 File Offset: 0x000FD1E4
	public void HandleSpawnModifier(EntityAlive alive)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return;
		}
		for (int i = 0; i < this.GameEventFlags.Count; i++)
		{
			switch (this.GameEventFlags[i].FlagType)
			{
			case GameEventManager.GameEventFlagTypes.BigHead:
				if (alive != null && !(alive is EntityPlayer))
				{
					alive.Buffs.AddBuff("twitch_bighead", -1, true, false, -1f);
				}
				break;
			case GameEventManager.GameEventFlagTypes.Dancing:
				if (alive != null && !(alive is EntityPlayer))
				{
					alive.Buffs.AddBuff("twitch_dance", -1, true, false, -1f);
				}
				break;
			case GameEventManager.GameEventFlagTypes.BucketHead:
				if (alive != null && !(alive is EntityPlayer) && !(alive is EntityVehicle))
				{
					alive.Buffs.AddBuff("twitch_buckethead", -1, true, false, -1f);
				}
				break;
			case GameEventManager.GameEventFlagTypes.TinyZombies:
				if (alive != null && alive is EntityZombie)
				{
					alive.Buffs.AddBuff("twitch_tiny", -1, true, false, -1f);
				}
				break;
			}
		}
	}

	// Token: 0x06002772 RID: 10098 RVA: 0x000FF10C File Offset: 0x000FD30C
	public void HandleForceBossDespawn(EntityPlayer player)
	{
		for (int i = 0; i < this.BossGroups.Count; i++)
		{
			if (this.BossGroups[i].TargetPlayer == player)
			{
				this.BossGroups[i].RemoveNavObjects();
				this.BossGroups[i].DespawnAll();
			}
		}
	}

	// Token: 0x06002773 RID: 10099 RVA: 0x000FF16C File Offset: 0x000FD36C
	public int SetupBossGroup(EntityPlayer target, EntityAlive boss, List<EntityAlive> minions, BossGroup.BossGroupTypes bossGroupType, string bossIcon)
	{
		for (int i = 0; i < this.BossGroups.Count; i++)
		{
			if (this.BossGroups[i].BossEntity == boss)
			{
				return this.BossGroups[i].BossGroupID;
			}
		}
		BossGroup bossGroup = new BossGroup(target, boss, minions, bossGroupType, bossIcon);
		this.BossGroups.Add(bossGroup);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.AddGroup, bossGroup.BossGroupID, bossGroup.CurrentGroupType, bossGroup.BossEntityID, bossGroup.MinionEntityIDs, bossGroup.BossIcon), false, -1, -1, -1, null, 192, false);
		return bossGroup.BossGroupID;
	}

	// Token: 0x06002774 RID: 10100 RVA: 0x000FF220 File Offset: 0x000FD420
	public void UpdateBossGroupType(int bossGroupID, BossGroup.BossGroupTypes bossGroupType)
	{
		for (int i = 0; i < this.BossGroups.Count; i++)
		{
			if (bossGroupID == this.BossGroups[i].BossGroupID)
			{
				BossGroup bossGroup = this.BossGroups[i];
				bossGroup.CurrentGroupType = bossGroupType;
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.UpdateGroupType, bossGroupID, bossGroupType), false, -1, -1, -1, null, 192, false);
				}
				if (bossGroup.IsCurrent)
				{
					bossGroup.RemoveNavObjects();
					bossGroup.AddNavObjects();
				}
			}
		}
	}

	// Token: 0x06002775 RID: 10101 RVA: 0x000FF2B8 File Offset: 0x000FD4B8
	public void SetupClientBossGroup(int bossGroupID, BossGroup.BossGroupTypes bossGroupType, int bossID, List<int> minionIDs, string bossIcon1)
	{
		for (int i = 0; i < this.BossGroups.Count; i++)
		{
			if (bossGroupID == this.BossGroups[i].BossGroupID)
			{
				this.BossGroups[i].CurrentGroupType = bossGroupType;
				return;
			}
		}
		this.BossGroups.Add(new BossGroup(bossGroupID, bossGroupType, bossID, minionIDs, bossIcon1));
	}

	// Token: 0x06002776 RID: 10102 RVA: 0x000FF31C File Offset: 0x000FD51C
	public void RemoveClientBossGroup(int bossGroupID)
	{
		for (int i = 0; i < this.BossGroups.Count; i++)
		{
			if (bossGroupID == this.BossGroups[i].BossGroupID)
			{
				this.BossGroups[i].RemoveNavObjects();
				this.BossGroups.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x06002777 RID: 10103 RVA: 0x000FF374 File Offset: 0x000FD574
	public void RemoveEntityFromBossGroup(int bossGroupID, int entityID)
	{
		for (int i = 0; i < this.BossGroups.Count; i++)
		{
			if (bossGroupID == this.BossGroups[i].BossGroupID)
			{
				this.BossGroups[i].RemoveMinion(entityID);
			}
		}
	}

	// Token: 0x06002778 RID: 10104 RVA: 0x000FF3C0 File Offset: 0x000FD5C0
	public void SendBossGroups(int entityID)
	{
		for (int i = 0; i < this.BossGroups.Count; i++)
		{
			BossGroup bossGroup = this.BossGroups[i];
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.AddGroup, bossGroup.BossGroupID, bossGroup.CurrentGroupType, bossGroup.BossEntityID, bossGroup.MinionEntityIDs, bossGroup.BossIcon), false, entityID, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06002779 RID: 10105 RVA: 0x000FF436 File Offset: 0x000FD636
	public void RequestBossGroupStatRefresh(int bossGroupID, int playerID)
	{
		if (this.BossGroups.Count > 0)
		{
			this.BossGroups[0].RefreshStats(playerID);
		}
	}

	// Token: 0x0600277A RID: 10106 RVA: 0x000FF458 File Offset: 0x000FD658
	public void HandleBossGroupUpdates(float deltaTime)
	{
		if (GameManager.Instance.World == null)
		{
			return;
		}
		this.bossCheckTime -= deltaTime;
		for (int i = this.BossGroups.Count - 1; i >= 0; i--)
		{
			this.BossGroups[i].HandleAutoPull();
			this.BossGroups[i].HandleLiveHandling();
			if (this.bossCheckTime <= 0f && this.BossGroups[i].ServerUpdate())
			{
				if (this.CurrentBossGroup == this.BossGroups[i])
				{
					this.CurrentBossGroup = null;
				}
				this.BossGroups.RemoveAt(i);
			}
		}
		if (this.bossCheckTime <= 0f)
		{
			this.bossCheckTime = 1f;
		}
	}

	// Token: 0x0600277B RID: 10107 RVA: 0x000FF51C File Offset: 0x000FD71C
	public void UpdateCurrentBossGroup(EntityPlayerLocal player)
	{
		this.serverBossGroupCheckTime -= Time.deltaTime;
		if (!this.BossGroupInitialized)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageBossEvent>().Setup(NetPackageBossEvent.BossEventTypes.RequestGroups, -1), false);
			}
			this.BossGroupInitialized = true;
			return;
		}
		if (this.serverBossGroupCheckTime <= 0f)
		{
			if (this.CurrentBossGroup != null)
			{
				this.CurrentBossGroup.Update(player);
				if (this.CurrentBossGroup.ReadyForRemove || !this.CurrentBossGroup.IsPlayerWithinRange(player))
				{
					this.CurrentBossGroup = null;
				}
			}
			else
			{
				for (int i = 0; i < this.BossGroups.Count; i++)
				{
					this.BossGroups[i].Update(player);
					if (!this.BossGroups[i].ReadyForRemove && this.BossGroups[i].IsPlayerWithinRange(player))
					{
						this.CurrentBossGroup = this.BossGroups[i];
						this.serverBossGroupCheckTime = 1f;
						return;
					}
				}
				this.CurrentBossGroup = null;
			}
			this.serverBossGroupCheckTime = 1f;
		}
	}

	// Token: 0x0600277C RID: 10108 RVA: 0x000FF638 File Offset: 0x000FD838
	public void RegisterLink(EntityPlayer player, GameEventActionSequence seq, string tag)
	{
		for (int i = 0; i < this.SequenceLinks.Count; i++)
		{
			if (this.SequenceLinks[i].CheckLink(player, tag))
			{
				return;
			}
		}
		this.SequenceLinks.Add(new GameEventManager.SequenceLink
		{
			Owner = player,
			OwnerSeq = seq,
			Tag = tag
		});
	}

	// Token: 0x0600277D RID: 10109 RVA: 0x000FF698 File Offset: 0x000FD898
	public bool HasSequenceLink(GameEventActionSequence seq)
	{
		for (int i = 0; i < this.SequenceLinks.Count; i++)
		{
			if (this.SequenceLinks[i].OwnerSeq == seq)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600277E RID: 10110 RVA: 0x000FF6D4 File Offset: 0x000FD8D4
	public GameEventActionSequence GetSequenceLink(EntityPlayer player, string tag)
	{
		if (player == null || tag == "")
		{
			return null;
		}
		for (int i = 0; i < this.SequenceLinks.Count; i++)
		{
			if (this.SequenceLinks[i].CheckLink(player, tag))
			{
				return this.SequenceLinks[i].OwnerSeq;
			}
		}
		return null;
	}

	// Token: 0x0600277F RID: 10111 RVA: 0x000FF738 File Offset: 0x000FD938
	public void UnRegisterLink(EntityPlayer player, string tag)
	{
		for (int i = 0; i < this.SequenceLinks.Count; i++)
		{
			if (this.SequenceLinks[i].CheckLink(player, tag))
			{
				this.SequenceLinks.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002780 RID: 10112 RVA: 0x000FF77C File Offset: 0x000FD97C
	public static int GetIntValue(EntityAlive alive, string value, int defaultValue = 0)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		if (value.StartsWith("@"))
		{
			if (alive != null)
			{
				return (int)alive.Buffs.GetCustomVar(value.Substring(1));
			}
			return defaultValue;
		}
		else
		{
			if (value.Contains("-"))
			{
				string[] array = value.Split('-', StringSplitOptions.None);
				int min = StringParsers.ParseSInt32(array[0], 0, -1, NumberStyles.Integer);
				int maxExclusive = StringParsers.ParseSInt32(array[1], 0, -1, NumberStyles.Integer) + 1;
				return GameEventManager.instance.Random.RandomRange(min, maxExclusive);
			}
			int result = 0;
			StringParsers.TryParseSInt32(value, out result, 0, -1, NumberStyles.Integer);
			return result;
		}
	}

	// Token: 0x06002781 RID: 10113 RVA: 0x000FF810 File Offset: 0x000FDA10
	public static float GetFloatValue(EntityAlive alive, string value, float defaultValue = 0f)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		if (value.StartsWith("@"))
		{
			if (alive != null)
			{
				return alive.Buffs.GetCustomVar(value.Substring(1));
			}
			return defaultValue;
		}
		else
		{
			if (value.Contains("-"))
			{
				string[] array = value.Split('-', StringSplitOptions.None);
				float min = (float)StringParsers.ParseSInt32(array[0], 0, -1, NumberStyles.Integer);
				float maxExclusive = (float)(StringParsers.ParseSInt32(array[1], 0, -1, NumberStyles.Integer) + 1);
				return GameEventManager.instance.Random.RandomRange(min, maxExclusive);
			}
			float result = 0f;
			StringParsers.TryParseFloat(value, out result, 0, -1, NumberStyles.Any);
			return result;
		}
	}

	// Token: 0x04001DDC RID: 7644
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameEventManager instance = null;

	// Token: 0x04001DDD RID: 7645
	public static Dictionary<string, GameEventActionSequence> GameEventSequences = new Dictionary<string, GameEventActionSequence>();

	// Token: 0x04001DDE RID: 7646
	public GameRandom Random;

	// Token: 0x04001DDF RID: 7647
	public List<string> ActiveRecipients = new List<string>();

	// Token: 0x04001DE0 RID: 7648
	public List<GameEventActionSequence> ActionSequenceUpdates = new List<GameEventActionSequence>();

	// Token: 0x04001DE1 RID: 7649
	public List<GameEventManager.SpawnEntry> spawnEntries = new List<GameEventManager.SpawnEntry>();

	// Token: 0x04001DE2 RID: 7650
	public List<GameEventManager.SpawnedBlocksEntry> blockEntries = new List<GameEventManager.SpawnedBlocksEntry>();

	// Token: 0x04001DE3 RID: 7651
	public List<GameEventManager.Category> CategoryList = new List<GameEventManager.Category>();

	// Token: 0x04001DE4 RID: 7652
	public HomerunManager HomerunManager = new HomerunManager();

	// Token: 0x04001DE5 RID: 7653
	public int MaxSpawnCount = 20;

	// Token: 0x04001DE6 RID: 7654
	public int ReservedCount;

	// Token: 0x04001DE7 RID: 7655
	public const int AttackTime = 12000;

	// Token: 0x04001DE8 RID: 7656
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GameEventManager.GameEventFlag> GameEventFlags = new List<GameEventManager.GameEventFlag>();

	// Token: 0x04001DE9 RID: 7657
	public bool BossGroupInitialized;

	// Token: 0x04001DEA RID: 7658
	public List<BossGroup> BossGroups = new List<BossGroup>();

	// Token: 0x04001DEB RID: 7659
	[PublicizedFrom(EAccessModifier.Private)]
	public BossGroup currentBossGroup;

	// Token: 0x04001DEC RID: 7660
	[PublicizedFrom(EAccessModifier.Private)]
	public float serverBossGroupCheckTime = 1f;

	// Token: 0x04001DED RID: 7661
	[PublicizedFrom(EAccessModifier.Private)]
	public float bossCheckTime = 1f;

	// Token: 0x04001DEE RID: 7662
	[PublicizedFrom(EAccessModifier.Private)]
	public float gameFlagCheckTime = 1f;

	// Token: 0x04001DEF RID: 7663
	[PublicizedFrom(EAccessModifier.Private)]
	public float attackTimerUpdate = 2f;

	// Token: 0x04001DFC RID: 7676
	public List<GameEventManager.SequenceLink> SequenceLinks = new List<GameEventManager.SequenceLink>();

	// Token: 0x020004B0 RID: 1200
	public class Category
	{
		// Token: 0x04001DFD RID: 7677
		public string Name;

		// Token: 0x04001DFE RID: 7678
		public string DisplayName;

		// Token: 0x04001DFF RID: 7679
		public string Icon;
	}

	// Token: 0x020004B1 RID: 1201
	public enum GameEventFlagTypes
	{
		// Token: 0x04001E01 RID: 7681
		Invalid = -1,
		// Token: 0x04001E02 RID: 7682
		BigHead,
		// Token: 0x04001E03 RID: 7683
		Dancing,
		// Token: 0x04001E04 RID: 7684
		BucketHead,
		// Token: 0x04001E05 RID: 7685
		TinyZombies
	}

	// Token: 0x020004B2 RID: 1202
	public class SpawnEntry
	{
		// Token: 0x06002784 RID: 10116 RVA: 0x000FF8C0 File Offset: 0x000FDAC0
		public void HandleUpdate()
		{
			if (!this.IsAggressive)
			{
				return;
			}
			EntityPlayer entityPlayer = this.SpawnedEntity.GetAttackTarget() as EntityPlayer;
			if (entityPlayer == null)
			{
				this.SpawnedEntity.SetAttackTarget(this.SpawnedEntity.world.GetClosestPlayer(this.SpawnedEntity, 500f, false), 1000);
				return;
			}
			this.SpawnedEntity.SetAttackTarget(entityPlayer, 1000);
		}

		// Token: 0x04001E06 RID: 7686
		public EntityAlive SpawnedEntity;

		// Token: 0x04001E07 RID: 7687
		public EntityAlive Target;

		// Token: 0x04001E08 RID: 7688
		public EntityPlayer Requester;

		// Token: 0x04001E09 RID: 7689
		public GameEventActionSequence GameEvent;

		// Token: 0x04001E0A RID: 7690
		public bool IsAggressive;
	}

	// Token: 0x020004B3 RID: 1203
	public class SpawnedBlocksEntry
	{
		// Token: 0x06002786 RID: 10118 RVA: 0x000FF92E File Offset: 0x000FDB2E
		public SpawnedBlocksEntry()
		{
			this.BlockGroupID = ++GameEventManager.SpawnedBlocksEntry.newID;
		}

		// Token: 0x06002787 RID: 10119 RVA: 0x000FF960 File Offset: 0x000FDB60
		public bool RemoveBlock(Vector3i blockPos)
		{
			bool result = false;
			for (int i = this.BlockList.Count - 1; i >= 0; i--)
			{
				if (this.BlockList[i] == blockPos)
				{
					this.BlockList.RemoveAt(i);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002788 RID: 10120 RVA: 0x000FF9AC File Offset: 0x000FDBAC
		[PublicizedFrom(EAccessModifier.Internal)]
		public bool TryRemoveBlocks()
		{
			List<BlockChangeInfo> list = null;
			World world = GameManager.Instance.World;
			IChunk chunk = null;
			for (int i = this.BlockList.Count - 1; i >= 0; i--)
			{
				if (world.GetChunkFromWorldPos(this.BlockList[i], ref chunk))
				{
					if (list == null)
					{
						list = new List<BlockChangeInfo>();
					}
					list.Add(new BlockChangeInfo(0, this.BlockList[i], BlockValue.Air, false));
					this.BlockList.RemoveAt(i);
				}
			}
			if (list != null)
			{
				GameManager.Instance.World.SetBlocksRPC(list);
			}
			if (this.BlockList.Count == 0)
			{
				if (!string.IsNullOrEmpty(this.RemoveSound))
				{
					Manager.BroadcastPlayByLocalPlayer(this.Center, this.RemoveSound);
				}
				if (this.RefundOnRemove)
				{
					this.GameEvent.SetRefundNeeded();
				}
				if (this.Requester is EntityPlayerLocal)
				{
					GameEventManager.Current.HandleGameBlocksRemoved(this.BlockGroupID, this.IsDespawn);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(NetPackageGameEventResponse.ResponseTypes.BlocksRemoved, -1, this.BlockGroupID, "", this.IsDespawn), false, this.Requester.entityId, -1, -1, null, 192, false);
				}
			}
			return this.BlockList.Count == 0;
		}

		// Token: 0x04001E0B RID: 7691
		public int BlockGroupID;

		// Token: 0x04001E0C RID: 7692
		[PublicizedFrom(EAccessModifier.Private)]
		public static int newID;

		// Token: 0x04001E0D RID: 7693
		public List<Vector3i> BlockList = new List<Vector3i>();

		// Token: 0x04001E0E RID: 7694
		public Vector3 Center;

		// Token: 0x04001E0F RID: 7695
		public Entity Target;

		// Token: 0x04001E10 RID: 7696
		public EntityPlayer Requester;

		// Token: 0x04001E11 RID: 7697
		public GameEventActionSequence GameEvent;

		// Token: 0x04001E12 RID: 7698
		public float TimeAlive = -1f;

		// Token: 0x04001E13 RID: 7699
		public string RemoveSound;

		// Token: 0x04001E14 RID: 7700
		public bool RefundOnRemove;

		// Token: 0x04001E15 RID: 7701
		public bool IsDespawn;

		// Token: 0x04001E16 RID: 7702
		public bool IsRefunded;
	}

	// Token: 0x020004B4 RID: 1204
	[PublicizedFrom(EAccessModifier.Private)]
	public class GameEventFlag
	{
		// Token: 0x04001E17 RID: 7703
		public GameEventManager.GameEventFlagTypes FlagType;

		// Token: 0x04001E18 RID: 7704
		public float Duration = -1f;
	}

	// Token: 0x020004B5 RID: 1205
	public class SequenceLink
	{
		// Token: 0x0600278A RID: 10122 RVA: 0x000FFB09 File Offset: 0x000FDD09
		public bool CheckLink(EntityPlayer player, string tag)
		{
			return this.Owner == player && this.Tag == tag;
		}

		// Token: 0x04001E19 RID: 7705
		public EntityPlayer Owner;

		// Token: 0x04001E1A RID: 7706
		public GameEventActionSequence OwnerSeq;

		// Token: 0x04001E1B RID: 7707
		public string Tag = "";
	}
}
