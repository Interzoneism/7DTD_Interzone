using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000911 RID: 2321
public class QuestJournal
{
	// Token: 0x17000736 RID: 1846
	// (get) Token: 0x060044F0 RID: 17648 RVA: 0x001B95A8 File Offset: 0x001B77A8
	// (set) Token: 0x060044F1 RID: 17649 RVA: 0x001B95B0 File Offset: 0x001B77B0
	public Quest TrackedQuest
	{
		get
		{
			return this.trackedQuest;
		}
		set
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.OwnerPlayer);
			if (uiforPlayer != null && uiforPlayer.xui != null && uiforPlayer.xui.QuestTracker != null)
			{
				Quest quest = uiforPlayer.xui.QuestTracker.TrackedQuest;
				if (quest != null)
				{
					quest.Tracked = false;
				}
			}
			this.trackedQuest = value;
			if (uiforPlayer != null && uiforPlayer.xui != null && uiforPlayer.xui.QuestTracker != null)
			{
				if (value != null)
				{
					this.trackedQuest.Tracked = true;
					uiforPlayer.xui.Recipes.TrackedRecipe = null;
					uiforPlayer.xui.QuestTracker.TrackedChallenge = null;
				}
				uiforPlayer.xui.QuestTracker.TrackedQuest = this.trackedQuest;
			}
		}
	}

	// Token: 0x060044F2 RID: 17650 RVA: 0x001B967C File Offset: 0x001B787C
	public void AddTraderPOI(Vector2 pos, int factionID)
	{
		if (!this.TraderPOIs.Contains(pos))
		{
			this.TraderPOIs.Add(pos);
		}
		if (!this.TradersByFaction.ContainsKey(factionID))
		{
			this.TradersByFaction.Add(factionID, new List<Vector2>());
		}
		if (!this.TradersByFaction[factionID].Contains(pos))
		{
			this.TradersByFaction[factionID].Add(pos);
		}
	}

	// Token: 0x060044F3 RID: 17651 RVA: 0x001B96E8 File Offset: 0x001B78E8
	public bool HasTraderPOI(Vector2 pos)
	{
		return this.TraderPOIs.Contains(pos);
	}

	// Token: 0x060044F4 RID: 17652 RVA: 0x001B96F6 File Offset: 0x001B78F6
	public List<Vector2> GetTraderList(int factionID)
	{
		if (this.TradersByFaction.ContainsKey(factionID))
		{
			return this.TradersByFaction[factionID];
		}
		return null;
	}

	// Token: 0x060044F5 RID: 17653 RVA: 0x001B9714 File Offset: 0x001B7914
	public Quest GetSharedQuest(int questCode)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestCode == questCode && this.quests[i].CurrentState == Quest.QuestState.InProgress)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x060044F6 RID: 17654 RVA: 0x001B9770 File Offset: 0x001B7970
	public void RemoveSharedQuestByOwner(int questCode)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestCode == questCode && this.quests[i].CurrentState == Quest.QuestState.InProgress && !this.quests[i].RallyMarkerActivated)
			{
				Quest quest = this.quests[i];
				this.RemoveQuest(quest);
				GameManager.ShowTooltip(this.OwnerPlayer, "Shared quest {0} has been removed by quest owner.", quest.QuestClass.Name, null, null, false, false, 0f);
				return;
			}
		}
	}

	// Token: 0x060044F7 RID: 17655 RVA: 0x001B980C File Offset: 0x001B7A0C
	public void RemoveSharedQuestForOwner(int entityID)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].SharedOwnerID == entityID && this.quests[i].CurrentState == Quest.QuestState.InProgress && !this.quests[i].RallyMarkerActivated)
			{
				Quest quest = this.quests[i];
				this.RemoveQuest(quest);
				GameManager.ShowTooltip(this.OwnerPlayer, "Shared quest {0} has been removed.", quest.QuestClass.Name, null, null, false, false, 0f);
			}
		}
	}

	// Token: 0x060044F8 RID: 17656 RVA: 0x001B98A8 File Offset: 0x001B7AA8
	public void FailAllSharedQuests()
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].SharedOwnerID != -1 && this.quests[i].CurrentState == Quest.QuestState.InProgress && this.quests[i].CurrentPhase < this.quests[i].QuestClass.HighestPhase)
			{
				this.quests[i].CloseQuest(Quest.QuestState.Failed, null);
			}
		}
	}

	// Token: 0x060044F9 RID: 17657 RVA: 0x001B9930 File Offset: 0x001B7B30
	public void FailAllActivatedQuests()
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].RallyMarkerActivated && this.quests[i].CurrentState == Quest.QuestState.InProgress && this.quests[i].CurrentPhase < this.quests[i].QuestClass.HighestPhase)
			{
				this.quests[i].CloseQuest(Quest.QuestState.Failed, null);
			}
		}
	}

	// Token: 0x060044FA RID: 17658 RVA: 0x001B99B8 File Offset: 0x001B7BB8
	public List<Vector2> GetUsedPOIs(Vector2 traderPOI, int tier)
	{
		QuestTraderData traderData = this.GetTraderData(traderPOI);
		if (traderData != null)
		{
			return traderData.GetTierPOIs(tier);
		}
		return null;
	}

	// Token: 0x060044FB RID: 17659 RVA: 0x001B99DC File Offset: 0x001B7BDC
	public void RemoveAllSharedQuests()
	{
		for (int i = this.quests.Count - 1; i >= 0; i--)
		{
			if (this.quests[i].SharedOwnerID != -1 && this.quests[i].CurrentState == Quest.QuestState.InProgress && this.quests[i].CurrentPhase < this.quests[i].QuestClass.HighestPhase)
			{
				Quest quest = this.quests[i];
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(quest.QuestCode, quest.SharedOwnerID, this.OwnerPlayer.entityId, false), false, quest.SharedOwnerID, -1, -1, null, 192, false);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(quest.QuestCode, quest.SharedOwnerID, this.OwnerPlayer.entityId, false), false);
				}
				this.RemoveQuest(quest);
				GameManager.ShowTooltip(this.OwnerPlayer, "Shared quest {0} has been removed.", quest.QuestClass.Name, null, null, false, false, 0f);
			}
		}
		for (int j = 0; j < this.sharedQuestEntries.Count; j++)
		{
			this.sharedQuestEntries[j].Quest.RemoveMapObject();
		}
		this.sharedQuestEntries.Clear();
		this.OwnerPlayer.TriggerSharedQuestRemovedEvent(null);
	}

	// Token: 0x060044FC RID: 17660 RVA: 0x001B9B5C File Offset: 0x001B7D5C
	public void RemovePlayerFromSharedWiths(EntityPlayer player)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			this.quests[i].RemoveSharedWith(player);
		}
	}

	// Token: 0x060044FD RID: 17661 RVA: 0x001B9B94 File Offset: 0x001B7D94
	public Quest FindSharedQuest(int questCode)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestCode == questCode)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x060044FE RID: 17662 RVA: 0x001B9BDC File Offset: 0x001B7DDC
	public Quest FindQuest(string questName, int questFaction = -1)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].ID == questName && (questFaction == -1 || (int)this.quests[i].QuestFaction == questFaction))
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x060044FF RID: 17663 RVA: 0x001B9C40 File Offset: 0x001B7E40
	public bool FindCompletedQuest(string questName, int questFaction = -1)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest = this.quests[i];
			if (quest.ID == questName && (questFaction == -1 || (int)quest.QuestFaction == questFaction) && quest.CurrentState == Quest.QuestState.Completed)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004500 RID: 17664 RVA: 0x001B9C98 File Offset: 0x001B7E98
	public Quest FindNonSharedQuest(string questName)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].ID == questName && this.quests[i].SharedOwnerID == -1)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x06004501 RID: 17665 RVA: 0x001B9CF8 File Offset: 0x001B7EF8
	public Quest FindNonSharedQuest(int questCode)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestCode == questCode && this.quests[i].SharedOwnerID == -1)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x06004502 RID: 17666 RVA: 0x001B9D54 File Offset: 0x001B7F54
	public Quest FindLatestNonSharedQuest(string questName)
	{
		Quest quest = null;
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].ID == questName && (quest == null || this.quests[i].Active || this.quests[i].FinishTime > quest.FinishTime) && this.quests[i].SharedOwnerID == -1)
			{
				quest = this.quests[i];
			}
		}
		return quest;
	}

	// Token: 0x06004503 RID: 17667 RVA: 0x001B9DE4 File Offset: 0x001B7FE4
	public Quest FindActiveQuest()
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].CurrentState == Quest.QuestState.InProgress || this.quests[i].CurrentState == Quest.QuestState.ReadyForTurnIn)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x06004504 RID: 17668 RVA: 0x001B9E40 File Offset: 0x001B8040
	public bool QuestIsActive(Quest quest)
	{
		bool result = false;
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest2 = this.quests[i];
			if (quest == quest2)
			{
				result = (quest2.CurrentState == Quest.QuestState.InProgress || quest2.CurrentState == Quest.QuestState.ReadyForTurnIn);
				break;
			}
		}
		return result;
	}

	// Token: 0x06004505 RID: 17669 RVA: 0x001B9E90 File Offset: 0x001B8090
	public Quest FindActiveQuest(int questCode)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestCode == questCode && (this.quests[i].CurrentState == Quest.QuestState.InProgress || this.quests[i].CurrentState == Quest.QuestState.ReadyForTurnIn))
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x06004506 RID: 17670 RVA: 0x001B9F00 File Offset: 0x001B8100
	public Quest FindActiveQuest(string questName, int questFaction = -1)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].ID == questName && (this.quests[i].CurrentState == Quest.QuestState.InProgress || this.quests[i].CurrentState == Quest.QuestState.ReadyForTurnIn) && (questFaction == -1 || (int)this.quests[i].QuestFaction == questFaction))
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x06004507 RID: 17671 RVA: 0x001B9F8C File Offset: 0x001B818C
	public Quest FindActiveOrCompleteQuest(string questName, int questFaction = -1)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].ID == questName && this.quests[i].CurrentState != Quest.QuestState.Failed && (questFaction == -1 || (int)this.quests[i].QuestFaction == questFaction))
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x06004508 RID: 17672 RVA: 0x001BA004 File Offset: 0x001B8204
	public Quest FindActiveQuestByGiver(int questGiverID)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].CheckIsQuestGiver(questGiverID) && this.quests[i].CurrentState == Quest.QuestState.InProgress && this.quests[i].SharedOwnerID == -1)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x06004509 RID: 17673 RVA: 0x001BA074 File Offset: 0x001B8274
	public Quest FindActiveQuestByGiver(int questGiverID, string questType)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].CheckIsQuestGiver(questGiverID) && (this.quests[i].CurrentState == Quest.QuestState.InProgress || this.quests[i].CurrentState == Quest.QuestState.ReadyForTurnIn) && this.quests[i].SharedOwnerID == -1 && this.quests[i].QuestClass.QuestType == questType)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x0600450A RID: 17674 RVA: 0x001BA118 File Offset: 0x001B8318
	public Quest FindReadyForTurnInQuestByGiver(int questGiverID)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].CheckIsQuestGiver(questGiverID) && (this.quests[i].CurrentState == Quest.QuestState.InProgress || this.quests[i].CurrentState == Quest.QuestState.ReadyForTurnIn) && this.quests[i].RallyMarkerActivated)
			{
				return this.quests[i];
			}
		}
		return null;
	}

	// Token: 0x0600450B RID: 17675 RVA: 0x001BA198 File Offset: 0x001B8398
	public bool HasActiveQuestByQuestCode(int questCode)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestCode == questCode && this.quests[i].CurrentState == Quest.QuestState.InProgress)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600450C RID: 17676 RVA: 0x001BA1E8 File Offset: 0x001B83E8
	public void AddQuest(Quest q, bool notify = true)
	{
		q.OwnerJournal = this;
		if (this.FindActiveQuest(q.QuestCode) == null)
		{
			q.StartQuest(true, notify);
			this.quests.Add(q);
			this.OwnerPlayer.TriggerQuestAddedEvent(q);
			foreach (KeyValuePair<Quest.PositionDataTypes, Vector3> keyValuePair in q.PositionData)
			{
				GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.OwnerPlayer.entityId).AddQuestPosition(q.QuestCode, keyValuePair.Key, World.worldToBlockPos(keyValuePair.Value));
			}
		}
	}

	// Token: 0x0600450D RID: 17677 RVA: 0x001BA2AC File Offset: 0x001B84AC
	[PublicizedFrom(EAccessModifier.Internal)]
	public void RefreshTracked()
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].Tracked)
			{
				this.TrackedQuest = this.quests[i];
				return;
			}
		}
	}

	// Token: 0x0600450E RID: 17678 RVA: 0x001BA2F8 File Offset: 0x001B84F8
	public void CompleteQuest(Quest q)
	{
		q.CurrentState = Quest.QuestState.Completed;
		q.FinishTime = GameManager.Instance.World.worldTime;
		this.OwnerPlayer.TriggerQuestChangedEvent(q);
		if (q.QuestClass.AddsToTierComplete && q.AddsProgression)
		{
			this.AddQuestFactionPoint(q.QuestFaction, (int)q.QuestClass.DifficultyTier);
			if (q.PositionData.ContainsKey(Quest.PositionDataTypes.TraderPosition) && q.PositionData.ContainsKey(Quest.PositionDataTypes.POIPosition) && SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				Vector3 vector = q.PositionData[Quest.PositionDataTypes.TraderPosition];
				Vector3 vector2 = q.PositionData[Quest.PositionDataTypes.POIPosition];
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageNPCQuestList>().Setup(this.OwnerPlayer.entityId, new Vector2(vector.x, vector.z), (int)q.QuestClass.DifficultyTier, new Vector2(vector2.x, vector2.z)), false);
			}
			this.ResetAddToProgression();
		}
		if (this.ActiveQuest == q)
		{
			this.ActiveQuest = null;
			this.RefreshRallyMarkerPositions();
		}
		GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.OwnerPlayer.entityId).RemovePositionsForQuest(q.QuestCode);
	}

	// Token: 0x0600450F RID: 17679 RVA: 0x001BA434 File Offset: 0x001B8634
	public int AddTraderData(Vector2 pos)
	{
		for (int i = 0; i < this.TraderData.Count; i++)
		{
			if (this.TraderData[i].TraderPOI == pos)
			{
				return i;
			}
		}
		this.TraderData.Add(new QuestTraderData(pos)
		{
			Owner = this
		});
		return this.TraderData.Count - 1;
	}

	// Token: 0x06004510 RID: 17680 RVA: 0x001BA498 File Offset: 0x001B8698
	public void AddPOIToTraderData(int tier, Vector3 questGiver, Vector3 poiPosition)
	{
		int index = this.AddTraderData(new Vector2(questGiver.x, questGiver.z));
		this.TraderData[index].AddPOI(tier, new Vector2(poiPosition.x, poiPosition.z));
	}

	// Token: 0x06004511 RID: 17681 RVA: 0x001BA4E0 File Offset: 0x001B86E0
	public void AddPOIToTraderData(int tier, Vector2 questGiver, Vector2 poiPosition)
	{
		int index = this.AddTraderData(questGiver);
		this.TraderData[index].AddPOI(tier, poiPosition);
	}

	// Token: 0x06004512 RID: 17682 RVA: 0x001BA508 File Offset: 0x001B8708
	public void ClearTraderDataTier(int tier, Vector2 questGiver)
	{
		QuestTraderData traderData = this.GetTraderData(questGiver);
		if (traderData != null)
		{
			traderData.ClearTier(tier);
		}
	}

	// Token: 0x06004513 RID: 17683 RVA: 0x001BA528 File Offset: 0x001B8728
	public QuestTraderData GetTraderData(Vector2 questGiver)
	{
		for (int i = 0; i < this.TraderData.Count; i++)
		{
			if (this.TraderData[i].TraderPOI == questGiver)
			{
				return this.TraderData[i];
			}
		}
		return null;
	}

	// Token: 0x06004514 RID: 17684 RVA: 0x001BA574 File Offset: 0x001B8774
	public void Clear()
	{
		this.TrackedQuest = null;
		this.ForceRemoveAllQuests();
		this.ActiveQuest = null;
		this.TraderData.Clear();
		this.sharedQuestEntries.Clear();
		this.TradersByFaction.Clear();
		this.QuestFactionPoints.Clear();
		this.GlobalFactionPoints = 0;
		this.TraderPOIs.Clear();
	}

	// Token: 0x06004515 RID: 17685 RVA: 0x001BA5D4 File Offset: 0x001B87D4
	public void FailedQuest(Quest q)
	{
		q.CurrentState = Quest.QuestState.Failed;
		q.FinishTime = GameManager.Instance.World.worldTime;
		this.OwnerPlayer.TriggerQuestChangedEvent(q);
		GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.OwnerPlayer.entityId).RemovePositionsForQuest(q.QuestCode);
	}

	// Token: 0x06004516 RID: 17686 RVA: 0x001BA630 File Offset: 0x001B8830
	public void RemoveQuest(Quest q)
	{
		if (this.FindActiveQuest(q.QuestCode) != null)
		{
			q.CloseQuest(Quest.QuestState.Failed, null);
			this.quests.Remove(q);
			if (q.SharedOwnerID != -1)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(q.QuestCode, q.SharedOwnerID, this.OwnerPlayer.entityId, false), false, q.SharedOwnerID, -1, -1, null, 192, false);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(q.QuestCode, q.SharedOwnerID, this.OwnerPlayer.entityId, false), false);
				}
			}
			this.OwnerPlayer.TriggerQuestRemovedEvent(q);
			this.HandlePartyRemoveQuest(q);
			GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.OwnerPlayer.entityId).RemovePositionsForQuest(q.QuestCode);
		}
	}

	// Token: 0x06004517 RID: 17687 RVA: 0x001BA724 File Offset: 0x001B8924
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandlePartyRemoveQuest(Quest q)
	{
		if (this.OwnerPlayer.IsInParty() && q.SharedOwnerID == -1)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				for (int i = 0; i < this.OwnerPlayer.Party.MemberList.Count; i++)
				{
					EntityPlayer entityPlayer = this.OwnerPlayer.Party.MemberList[i];
					if (entityPlayer is EntityPlayerLocal)
					{
						entityPlayer.QuestJournal.RemoveSharedQuestByOwner(q.QuestCode);
						entityPlayer.QuestJournal.RemoveSharedQuestEntry(q.QuestCode);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(q.QuestCode, this.OwnerPlayer.entityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
					}
				}
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageSharedQuest>().Setup(q.QuestCode, this.OwnerPlayer.entityId), false);
		}
	}

	// Token: 0x06004518 RID: 17688 RVA: 0x001BA82C File Offset: 0x001B8A2C
	public void ForceRemoveAllQuests()
	{
		for (int i = this.quests.Count - 1; i >= 0; i--)
		{
			this.ForceRemoveQuest(this.quests[i]);
		}
	}

	// Token: 0x06004519 RID: 17689 RVA: 0x001BA864 File Offset: 0x001B8A64
	public void ForceRemoveQuest(Quest quest)
	{
		this.quests.Remove(quest);
		quest.UnhookQuest();
		this.OwnerPlayer.TriggerQuestRemovedEvent(quest);
		GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.OwnerPlayer.entityId).RemovePositionsForQuest(quest.QuestCode);
	}

	// Token: 0x0600451A RID: 17690 RVA: 0x001BA8B8 File Offset: 0x001B8AB8
	public void ForceRemoveQuest(string questID)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].ID.EqualsCaseInsensitive(questID))
			{
				Quest quest = this.quests[i];
				this.quests.Remove(quest);
				quest.UnhookQuest();
				this.OwnerPlayer.TriggerQuestRemovedEvent(quest);
				GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.OwnerPlayer.entityId).RemovePositionsForQuest(quest.QuestCode);
				return;
			}
		}
	}

	// Token: 0x0600451B RID: 17691 RVA: 0x001BA946 File Offset: 0x001B8B46
	public void RefreshQuest(Quest q)
	{
		this.OwnerPlayer.TriggerQuestChangedEvent(q);
	}

	// Token: 0x0600451C RID: 17692 RVA: 0x001BA954 File Offset: 0x001B8B54
	public void UnHookQuests()
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			this.quests[i].UnhookQuest();
		}
	}

	// Token: 0x0600451D RID: 17693 RVA: 0x001BA988 File Offset: 0x001B8B88
	public void Read(PooledBinaryReader _br)
	{
		this.quests.Clear();
		byte b = _br.ReadByte();
		if (b >= 2)
		{
			int num = (int)_br.ReadByte();
			this.TraderPOIs.Clear();
			for (int i = 0; i < num; i++)
			{
				this.TraderPOIs.Add(StreamUtils.ReadVector2(_br));
			}
		}
		if (b > 2)
		{
			int num2 = (int)_br.ReadByte();
			this.TradersByFaction.Clear();
			for (int j = 0; j < num2; j++)
			{
				int key = _br.ReadInt32();
				List<Vector2> list = new List<Vector2>();
				int num3 = _br.ReadInt32();
				for (int k = 0; k < num3; k++)
				{
					list.Add(StreamUtils.ReadVector2(_br));
				}
				this.TradersByFaction.Add(key, list);
			}
		}
		int num4 = (int)_br.ReadInt16();
		for (int l = 0; l < num4; l++)
		{
			PooledBinaryReader.StreamReadSizeMarker streamReadSizeMarker = default(PooledBinaryReader.StreamReadSizeMarker);
			if (b >= 5)
			{
				streamReadSizeMarker = _br.ReadSizeMarker(PooledBinaryWriter.EMarkerSize.UInt16);
			}
			string text = _br.ReadString();
			try
			{
				byte b2 = _br.ReadByte();
				if (QuestClass.GetQuest(text) == null)
				{
					Log.Error("Loading player quests: Quest with ID " + text + " not found, ignoring");
					uint num5;
					_br.ValidateSizeMarker(ref streamReadSizeMarker, out num5, true);
				}
				else
				{
					Quest quest = QuestClass.CreateQuest(text);
					Quest quest2 = quest.Clone();
					quest2.CurrentQuestVersion = b2;
					quest2.Read(_br);
					if (quest.CurrentQuestVersion != b2)
					{
						quest2 = quest.Clone();
					}
					quest2.OwnerJournal = this;
					this.quests.Add(quest2);
					if (quest2.CurrentState == Quest.QuestState.Completed && quest2.QuestClass.AddsToTierComplete && quest2.AddsProgression)
					{
						this.AddQuestFactionPoint(quest2.QuestFaction, (int)quest2.QuestClass.DifficultyTier);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				uint num5;
				if (b >= 5 && !_br.ValidateSizeMarker(ref streamReadSizeMarker, out num5, true))
				{
					Log.Error("Loading player quests: Error loading quest " + text + ", ignoring");
				}
			}
		}
		if (b > 3)
		{
			this.TraderData.Clear();
			num4 = (int)_br.ReadByte();
			for (int m = 0; m < num4; m++)
			{
				QuestTraderData questTraderData = new QuestTraderData
				{
					Owner = this
				};
				questTraderData.Read(_br, b);
				this.TraderData.Add(questTraderData);
			}
		}
	}

	// Token: 0x0600451E RID: 17694 RVA: 0x001BABDC File Offset: 0x001B8DDC
	public void AddQuestFactionPoint(byte id, int difficultyTier)
	{
		if (difficultyTier == 0)
		{
			return;
		}
		this.GlobalFactionPoints += difficultyTier;
		if (!this.QuestFactionPoints.ContainsKey(id))
		{
			this.QuestFactionPoints.Add(id, difficultyTier);
			return;
		}
		Dictionary<byte, int> questFactionPoints = this.QuestFactionPoints;
		questFactionPoints[id] += difficultyTier;
	}

	// Token: 0x0600451F RID: 17695 RVA: 0x001BAC30 File Offset: 0x001B8E30
	public int GetQuestFactionPoints(byte id)
	{
		if (this.QuestFactionPoints.ContainsKey(id))
		{
			return this.QuestFactionPoints[id];
		}
		return 0;
	}

	// Token: 0x06004520 RID: 17696 RVA: 0x001BAC50 File Offset: 0x001B8E50
	public int GetQuestFactionMax(byte id, int tier)
	{
		int num = 0;
		for (int i = 1; i <= tier; i++)
		{
			num += i * Quest.QuestsPerTier;
		}
		return num;
	}

	// Token: 0x06004521 RID: 17697 RVA: 0x001BAC78 File Offset: 0x001B8E78
	public void ResetAddToProgression()
	{
		int num = 0;
		int worldDay = GameManager.Instance.World.WorldDay;
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestProgressDay == worldDay)
			{
				num++;
			}
		}
		int questsPerDay = Quest.QuestsPerDay;
		this.CanAddProgression = (questsPerDay == -1 || num < questsPerDay);
	}

	// Token: 0x06004522 RID: 17698 RVA: 0x001BACDC File Offset: 0x001B8EDC
	public void HandleQuestCompleteToday(Quest q)
	{
		int num = 0;
		int worldDay = GameManager.Instance.World.WorldDay;
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].QuestProgressDay == worldDay)
			{
				num++;
			}
		}
		int questsPerDay = Quest.QuestsPerDay;
		if (questsPerDay == -1 || num < questsPerDay)
		{
			q.QuestProgressDay = worldDay;
		}
		else
		{
			q.QuestProgressDay = -1;
		}
		this.ResetAddToProgression();
		if (!this.CanAddProgression)
		{
			this.OwnerPlayer.Buffs.AddBuff("buffShowQuestLimitReached", -1, true, false, -1f);
		}
	}

	// Token: 0x06004523 RID: 17699 RVA: 0x001BAD74 File Offset: 0x001B8F74
	public int GetCurrentFactionTier(byte id, int offset = 0, bool allowExtraTierOverMax = false)
	{
		int num = this.GetQuestFactionPoints(id) + offset;
		for (int i = 1; i < 100; i++)
		{
			num -= i * Quest.QuestsPerTier;
			if (num < 0)
			{
				return Math.Min(i, Quest.MaxQuestTier + (allowExtraTierOverMax ? 1 : 0));
			}
		}
		return 1;
	}

	// Token: 0x06004524 RID: 17700 RVA: 0x001BADBC File Offset: 0x001B8FBC
	public void Write(PooledBinaryWriter _bw)
	{
		_bw.Write(5);
		int count = this.TraderPOIs.Count;
		_bw.Write((byte)count);
		for (int i = 0; i < count; i++)
		{
			StreamUtils.Write(_bw, this.TraderPOIs[i]);
		}
		_bw.Write((byte)this.TradersByFaction.Count);
		foreach (int num in this.TradersByFaction.Keys)
		{
			_bw.Write(num);
			_bw.Write(this.TradersByFaction[num].Count);
			for (int j = 0; j < this.TradersByFaction[num].Count; j++)
			{
				StreamUtils.Write(_bw, this.TradersByFaction[num][j]);
			}
		}
		int count2 = this.quests.Count;
		_bw.Write((ushort)count2);
		for (int k = 0; k < count2; k++)
		{
			PooledBinaryWriter.StreamWriteSizeMarker streamWriteSizeMarker = _bw.ReserveSizeMarker(PooledBinaryWriter.EMarkerSize.UInt16);
			this.quests[k].Write(_bw);
			_bw.FinalizeSizeMarker(ref streamWriteSizeMarker);
		}
		count2 = this.TraderData.Count;
		_bw.Write((byte)count2);
		for (int l = 0; l < count2; l++)
		{
			this.TraderData[l].Write(_bw);
		}
	}

	// Token: 0x06004525 RID: 17701 RVA: 0x001BAF34 File Offset: 0x001B9134
	public QuestJournal Clone()
	{
		QuestJournal questJournal = new QuestJournal();
		questJournal.OwnerPlayer = this.OwnerPlayer;
		for (int i = 0; i < this.TraderPOIs.Count; i++)
		{
			questJournal.TraderPOIs.Add(this.TraderPOIs[i]);
		}
		foreach (int key in this.TradersByFaction.Keys)
		{
			questJournal.TradersByFaction.Add(key, this.TradersByFaction[key]);
		}
		for (int j = 0; j < this.quests.Count; j++)
		{
			questJournal.quests.Add(this.quests[j].Clone());
		}
		for (int k = 0; k < this.sharedQuestEntries.Count; k++)
		{
			questJournal.sharedQuestEntries.Add(this.sharedQuestEntries[k].Clone());
		}
		foreach (KeyValuePair<byte, int> keyValuePair in this.QuestFactionPoints)
		{
			questJournal.AddQuestFactionPoint(keyValuePair.Key, keyValuePair.Value);
		}
		for (int l = 0; l < this.TraderData.Count; l++)
		{
			questJournal.TraderData.Add(this.TraderData[l]);
		}
		return questJournal;
	}

	// Token: 0x06004526 RID: 17702 RVA: 0x001BB0D0 File Offset: 0x001B92D0
	[PublicizedFrom(EAccessModifier.Internal)]
	public void StartQuests()
	{
		if (!GameManager.Instance.World.IsEditor() && !GameUtils.IsWorldEditor() && !GameUtils.IsPlaytesting())
		{
			this.OwnerPlayer.challengeJournal.StartChallenges(this.OwnerPlayer);
		}
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].RallyMarkerActivated)
			{
				if (this.quests[i].SharedOwnerID != -1 && this.quests[i].CurrentPhase < this.quests[i].QuestClass.HighestPhase && this.quests[i].CurrentState == Quest.QuestState.InProgress)
				{
					this.quests[i].CloseQuest(Quest.QuestState.Failed, null);
				}
				else
				{
					this.quests[i].ResetToRallyPointObjective();
					this.quests[i].StartQuest(false, true);
				}
			}
			else
			{
				this.quests[i].StartQuest(false, true);
			}
		}
	}

	// Token: 0x06004527 RID: 17703 RVA: 0x001BB1E4 File Offset: 0x001B93E4
	public bool AddSharedQuestEntry(int questCode, string questID, string poiName, Vector3 position, Vector3 size, Vector3 returnPos, int sharerID, int questGiverID)
	{
		for (int i = 0; i < this.sharedQuestEntries.Count; i++)
		{
			SharedQuestEntry sharedQuestEntry = this.sharedQuestEntries[i];
			if (sharedQuestEntry.QuestCode == questCode && sharedQuestEntry.QuestID == questID && sharedQuestEntry.SharedByPlayerID == sharerID && sharedQuestEntry.QuestGiverID == questGiverID)
			{
				if (PartyQuests.AutoAccept)
				{
					Log.Out(string.Format("Ignoring received quest, already have a sharedquest with the questCode={0}, id={1}, sharer={2}, giver={3}:", new object[]
					{
						questCode,
						questID,
						sharerID,
						questGiverID
					}));
					for (int j = 0; j < this.sharedQuestEntries.Count; j++)
					{
						SharedQuestEntry sharedQuestEntry2 = this.sharedQuestEntries[j];
						Log.Out(string.Format("  {0}.: id={1}, code={2}, name={3}, POI={4}, state={5}, owner={6}", new object[]
						{
							j,
							sharedQuestEntry2.QuestID,
							sharedQuestEntry2.QuestCode,
							sharedQuestEntry2.Quest.QuestClass.Name,
							sharedQuestEntry2.Quest.GetParsedText("{poi.name}"),
							sharedQuestEntry2.Quest.CurrentState,
							sharedQuestEntry2.SharedByPlayerID
						}));
					}
					Log.Out("Quests:");
					for (int k = 0; k < this.quests.Count; k++)
					{
						Quest quest = this.quests[k];
						Log.Out(string.Format("  {0}.: id={1}, code={2}, name={3}, POI={4}, state={5}, owner={6}", new object[]
						{
							k,
							quest.ID,
							quest.QuestCode,
							quest.QuestClass.Name,
							quest.GetParsedText("{poi.name}"),
							quest.CurrentState,
							quest.SharedOwnerID
						}));
					}
				}
				return false;
			}
		}
		SharedQuestEntry sharedQuestEntry3 = new SharedQuestEntry(questCode, questID, poiName, position, size, returnPos, sharerID, questGiverID, this, null);
		Log.Out(string.Format("Received shared quest: questCode={0}, id={1}, POI {2}", questCode, questID, poiName));
		this.sharedQuestEntries.Add(sharedQuestEntry3);
		this.OwnerPlayer.TriggerSharedQuestAddedEvent(sharedQuestEntry3);
		return true;
	}

	// Token: 0x06004528 RID: 17704 RVA: 0x001BB431 File Offset: 0x001B9631
	public void RemoveSharedQuestEntry(SharedQuestEntry entry)
	{
		if (this.sharedQuestEntries.Contains(entry))
		{
			this.sharedQuestEntries.Remove(entry);
			entry.Quest.RemoveMapObject();
			this.OwnerPlayer.TriggerSharedQuestRemovedEvent(entry);
		}
	}

	// Token: 0x06004529 RID: 17705 RVA: 0x001BB468 File Offset: 0x001B9668
	public void RemoveSharedQuestEntry(int questCode)
	{
		for (int i = this.sharedQuestEntries.Count - 1; i >= 0; i--)
		{
			if (this.sharedQuestEntries[i].Quest.QuestCode == questCode)
			{
				SharedQuestEntry sharedQuestEntry = this.sharedQuestEntries[i];
				sharedQuestEntry.Quest.RemoveMapObject();
				this.sharedQuestEntries.RemoveAt(i);
				this.OwnerPlayer.TriggerSharedQuestRemovedEvent(sharedQuestEntry);
			}
		}
	}

	// Token: 0x0600452A RID: 17706 RVA: 0x001BB4D8 File Offset: 0x001B96D8
	public void RemoveSharedQuestEntryByOwner(int entityID)
	{
		for (int i = this.sharedQuestEntries.Count - 1; i >= 0; i--)
		{
			if (this.sharedQuestEntries[i].Quest.SharedOwnerID == entityID)
			{
				SharedQuestEntry sharedQuestEntry = this.sharedQuestEntries[i];
				sharedQuestEntry.Quest.RemoveMapObject();
				this.sharedQuestEntries.RemoveAt(i);
				this.OwnerPlayer.TriggerSharedQuestRemovedEvent(sharedQuestEntry);
			}
		}
	}

	// Token: 0x0600452B RID: 17707 RVA: 0x001BB548 File Offset: 0x001B9748
	public void ClearSharedQuestMarkers()
	{
		for (int i = 0; i < this.sharedQuestEntries.Count; i++)
		{
			this.sharedQuestEntries[i].Quest.RemoveMapObject();
		}
	}

	// Token: 0x0600452C RID: 17708 RVA: 0x001BB584 File Offset: 0x001B9784
	public Quest GetNextCompletedQuest(Quest lastQuest, int entityId)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (lastQuest != null)
			{
				if (this.quests[i] == lastQuest)
				{
					lastQuest = null;
				}
			}
			else
			{
				Quest quest = this.quests[i];
				if (quest.CurrentState == Quest.QuestState.ReadyForTurnIn && (!quest.QuestClass.ReturnToQuestGiver || quest.QuestGiverID == -1 || quest.CheckIsQuestGiver(entityId)))
				{
					return quest;
				}
			}
		}
		return null;
	}

	// Token: 0x0600452D RID: 17709 RVA: 0x001BB5F8 File Offset: 0x001B97F8
	public void SetActivePositionData(Quest.PositionDataTypes dataType, Vector3i position)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].Active && this.quests[i].RallyMarkerActivated)
			{
				this.quests[i].SetObjectivePosition(dataType, position);
			}
		}
	}

	// Token: 0x0600452E RID: 17710 RVA: 0x001BB654 File Offset: 0x001B9854
	public void HandleRestorePowerReceived(Vector3 prefabPos, List<Vector3i> activateList)
	{
		int num = 0;
		while (num < this.quests.Count && (!this.quests[num].Active || !this.quests[num].RallyMarkerActivated || !this.quests[num].HandleActivateListReceived(prefabPos, activateList)))
		{
			num++;
		}
	}

	// Token: 0x0600452F RID: 17711 RVA: 0x001BB6B4 File Offset: 0x001B98B4
	public void HandleRallyMarkerActivation(int questCode, Vector3 prefabPos, bool rallyMarkerActivated, QuestEventManager.POILockoutReasonTypes lockoutReason, ulong extraData = 0UL)
	{
		int num = 0;
		while (num < this.quests.Count && (this.quests[num].QuestCode != questCode || !this.quests[num].Active || !this.quests[num].HandleRallyMarkerActivation(prefabPos, rallyMarkerActivated, lockoutReason, extraData)))
		{
			num++;
		}
	}

	// Token: 0x06004530 RID: 17712 RVA: 0x001BB718 File Offset: 0x001B9918
	public bool CheckRallyMarkerActivation()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < this.quests.Count; i++)
		{
			if (this.quests[i].Active)
			{
				foreach (BaseObjective baseObjective in this.quests[i].Objectives)
				{
					ObjectiveRallyPoint objectiveRallyPoint = baseObjective as ObjectiveRallyPoint;
					if (objectiveRallyPoint != null)
					{
						flag2 = true;
						flag = objectiveRallyPoint.IsActivated();
						break;
					}
				}
			}
		}
		return !flag2 || flag;
	}

	// Token: 0x06004531 RID: 17713 RVA: 0x001BB7B8 File Offset: 0x001B99B8
	public Quest HasQuestAtRallyPosition(Vector3 rallyPos, bool mustBeHost = true)
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest = this.quests[i];
			Vector3 vector;
			if (quest.Active && (quest.SharedOwnerID == -1 || !mustBeHost) && !quest.RallyMarkerActivated && quest.GetPositionData(out vector, Quest.PositionDataTypes.Activate) && vector.x == rallyPos.x && vector.z == rallyPos.z)
			{
				return quest;
			}
		}
		return null;
	}

	// Token: 0x06004532 RID: 17714 RVA: 0x001BB830 File Offset: 0x001B9A30
	public void RefreshRallyMarkerPositions()
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest = this.quests[i];
			if (quest.Active && !quest.RallyMarkerActivated)
			{
				quest.RefreshRallyMarker();
			}
		}
	}

	// Token: 0x06004533 RID: 17715 RVA: 0x001BB878 File Offset: 0x001B9A78
	public bool HasCraftingQuest()
	{
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest = this.quests[i];
			if (quest.Active && quest.QuestTags.Test_AnySet(QuestEventManager.craftingTag))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004534 RID: 17716 RVA: 0x001BB8C8 File Offset: 0x001B9AC8
	public List<string> GetQuestRecipes()
	{
		this.questRecipeList.Clear();
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest = this.quests[i];
			if (quest.Active)
			{
				for (int j = 0; j < quest.Objectives.Count; j++)
				{
					if ((quest.Objectives[j].Phase == 0 || quest.Objectives[j].Phase == quest.CurrentPhase) && quest.Objectives[j] is ObjectiveCraft && !quest.Objectives[j].Complete)
					{
						this.questRecipeList.Add((quest.Objectives[j] as ObjectiveCraft).ID);
					}
				}
			}
		}
		return this.questRecipeList;
	}

	// Token: 0x06004535 RID: 17717 RVA: 0x001BB9A8 File Offset: 0x001B9BA8
	public T GetObjectiveForQuest<T>(int _questCode) where T : BaseObjective
	{
		Quest quest = this.FindActiveQuest(_questCode);
		if (quest != null)
		{
			for (int i = 0; i < quest.Objectives.Count; i++)
			{
				if (quest.CurrentPhase == quest.Objectives[i].Phase && quest.Objectives[i] is T)
				{
					return quest.Objectives[i] as T;
				}
			}
		}
		return default(T);
	}

	// Token: 0x06004536 RID: 17718 RVA: 0x001BBA24 File Offset: 0x001B9C24
	public int GetRewardedSkillPoints()
	{
		int num = 0;
		for (int i = 0; i < this.quests.Count; i++)
		{
			Quest quest = this.quests[i];
			if (quest.CurrentState == Quest.QuestState.Completed)
			{
				for (int j = 0; j < quest.QuestClass.Rewards.Count; j++)
				{
					RewardSkillPoints rewardSkillPoints = quest.QuestClass.Rewards[j] as RewardSkillPoints;
					if (rewardSkillPoints != null)
					{
						num += StringParsers.ParseSInt32(rewardSkillPoints.Value, 0, -1, NumberStyles.Integer);
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06004537 RID: 17719 RVA: 0x001BBAAC File Offset: 0x001B9CAC
	public void Update(int worldDay)
	{
		if (this.previousDay != worldDay)
		{
			this.previousDay = worldDay;
			this.ResetAddToProgression();
			if (this.CanAddProgression)
			{
				this.OwnerPlayer.Buffs.RemoveBuff("buffShowQuestLimitReached", true);
				return;
			}
			this.OwnerPlayer.Buffs.AddBuff("buffShowQuestLimitReached", -1, true, false, -1f);
		}
	}

	// Token: 0x0400361D RID: 13853
	public EntityPlayerLocal OwnerPlayer;

	// Token: 0x0400361E RID: 13854
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCurrentSaveVersion = 5;

	// Token: 0x0400361F RID: 13855
	public List<Quest> quests = new List<Quest>();

	// Token: 0x04003620 RID: 13856
	public List<SharedQuestEntry> sharedQuestEntries = new List<SharedQuestEntry>();

	// Token: 0x04003621 RID: 13857
	public List<Vector2> TraderPOIs = new List<Vector2>();

	// Token: 0x04003622 RID: 13858
	public List<QuestTraderData> TraderData = new List<QuestTraderData>();

	// Token: 0x04003623 RID: 13859
	public Dictionary<int, List<Vector2>> TradersByFaction = new Dictionary<int, List<Vector2>>();

	// Token: 0x04003624 RID: 13860
	[PublicizedFrom(EAccessModifier.Private)]
	public Quest trackedQuest;

	// Token: 0x04003625 RID: 13861
	public Quest ActiveQuest;

	// Token: 0x04003626 RID: 13862
	public Dictionary<byte, int> QuestFactionPoints = new Dictionary<byte, int>();

	// Token: 0x04003627 RID: 13863
	public int GlobalFactionPoints;

	// Token: 0x04003628 RID: 13864
	public bool CanAddProgression;

	// Token: 0x04003629 RID: 13865
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> questRecipeList = new List<string>();

	// Token: 0x0400362A RID: 13866
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousDay = -1;
}
