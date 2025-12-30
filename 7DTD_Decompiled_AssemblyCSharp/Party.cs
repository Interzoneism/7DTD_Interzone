using System;
using System.Collections.Generic;
using Audio;
using Twitch;
using UnityEngine;

// Token: 0x02000809 RID: 2057
public class Party
{
	// Token: 0x170005F6 RID: 1526
	// (get) Token: 0x06003AEF RID: 15087 RVA: 0x0017B4F4 File Offset: 0x001796F4
	public EntityPlayer Leader
	{
		get
		{
			if (this.LeaderIndex >= this.MemberList.Count)
			{
				return null;
			}
			return this.MemberList[this.LeaderIndex];
		}
	}

	// Token: 0x170005F7 RID: 1527
	// (get) Token: 0x06003AF0 RID: 15088 RVA: 0x0017B51C File Offset: 0x0017971C
	public int GameStage
	{
		get
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.MemberList.Count; i++)
			{
				EntityPlayer entityPlayer = this.MemberList[i];
				list.Add(entityPlayer.gameStage);
			}
			return GameStageDefinition.CalcPartyLevel(list);
		}
	}

	// Token: 0x170005F8 RID: 1528
	// (get) Token: 0x06003AF1 RID: 15089 RVA: 0x0017B564 File Offset: 0x00179764
	public int HighestGameStage
	{
		get
		{
			int num = 0;
			for (int i = 0; i < this.MemberList.Count; i++)
			{
				EntityPlayer entityPlayer = this.MemberList[i];
				num = Mathf.Max(num, entityPlayer.gameStage);
			}
			return num;
		}
	}

	// Token: 0x170005F9 RID: 1529
	// (get) Token: 0x06003AF2 RID: 15090 RVA: 0x0017B5A4 File Offset: 0x001797A4
	public bool HasTwitchMember
	{
		get
		{
			for (int i = 0; i < this.MemberList.Count; i++)
			{
				if (this.MemberList[i].TwitchEnabled)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x170005FA RID: 1530
	// (get) Token: 0x06003AF3 RID: 15091 RVA: 0x0017B5E0 File Offset: 0x001797E0
	public TwitchVoteLockTypes HasTwitchVoteLock
	{
		get
		{
			for (int i = 0; i < this.MemberList.Count; i++)
			{
				if (this.MemberList[i].TwitchVoteLock != TwitchVoteLockTypes.None)
				{
					return this.MemberList[i].TwitchVoteLock;
				}
			}
			return TwitchVoteLockTypes.None;
		}
	}

	// Token: 0x06003AF4 RID: 15092 RVA: 0x0017B62C File Offset: 0x0017982C
	public int GetHighestLootStage(float containerMod, float containerBonus)
	{
		int num = 0;
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			EntityPlayer entityPlayer = this.MemberList[i];
			num = Mathf.Max(num, entityPlayer.GetLootStage(containerMod, containerBonus));
		}
		return num;
	}

	// Token: 0x14000049 RID: 73
	// (add) Token: 0x06003AF5 RID: 15093 RVA: 0x0017B670 File Offset: 0x00179870
	// (remove) Token: 0x06003AF6 RID: 15094 RVA: 0x0017B6A8 File Offset: 0x001798A8
	public event OnPartyMembersChanged PartyMemberAdded;

	// Token: 0x1400004A RID: 74
	// (add) Token: 0x06003AF7 RID: 15095 RVA: 0x0017B6E0 File Offset: 0x001798E0
	// (remove) Token: 0x06003AF8 RID: 15096 RVA: 0x0017B718 File Offset: 0x00179918
	public event OnPartyMembersChanged PartyMemberRemoved;

	// Token: 0x1400004B RID: 75
	// (add) Token: 0x06003AF9 RID: 15097 RVA: 0x0017B750 File Offset: 0x00179950
	// (remove) Token: 0x06003AFA RID: 15098 RVA: 0x0017B788 File Offset: 0x00179988
	public event OnPartyChanged PartyLeaderChanged;

	// Token: 0x06003AFB RID: 15099 RVA: 0x0017B7C0 File Offset: 0x001799C0
	public bool AddPlayer(EntityPlayer player)
	{
		if (this.MemberList.Contains(player))
		{
			return false;
		}
		if (this.MemberList.Count == 8)
		{
			return false;
		}
		this.MemberList.Add(player);
		player.Party = this;
		player.RemoveAllPartyInvites();
		bool isInPartyOfLocalPlayer = false;
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			if (this.MemberList[i] is EntityPlayerLocal)
			{
				isInPartyOfLocalPlayer = true;
				break;
			}
		}
		if (this.PartyMemberAdded != null)
		{
			this.PartyMemberAdded(player);
		}
		for (int j = 0; j < this.MemberList.Count; j++)
		{
			this.MemberList[j].IsInPartyOfLocalPlayer = isInPartyOfLocalPlayer;
			this.MemberList[j].HandleOnPartyJoined();
			if (this.MemberList[j].NavObject != null)
			{
				this.MemberList[j].NavObject.UseOverrideColor = true;
				this.MemberList[j].NavObject.OverrideColor = Constants.TrackedFriendColors[j % Constants.TrackedFriendColors.Length];
				this.MemberList[j].NavObject.name = this.MemberList[j].PlayerDisplayName;
			}
		}
		return true;
	}

	// Token: 0x06003AFC RID: 15100 RVA: 0x0017B904 File Offset: 0x00179B04
	public bool KickPlayer(EntityPlayer player)
	{
		if (!this.MemberList.Contains(player))
		{
			return false;
		}
		if (player.NavObject != null)
		{
			player.NavObject.UseOverrideColor = false;
		}
		this.MemberList.Remove(player);
		if (this.PartyMemberRemoved != null)
		{
			this.PartyMemberRemoved(player);
		}
		player.LeaveParty();
		player.IsInPartyOfLocalPlayer = false;
		if (this.MemberList.Count == 1)
		{
			this.MemberList[0].LeaveParty();
		}
		return true;
	}

	// Token: 0x06003AFD RID: 15101 RVA: 0x0017B984 File Offset: 0x00179B84
	public bool RemovePlayer(EntityPlayer player)
	{
		if (!this.MemberList.Contains(player))
		{
			return false;
		}
		if (player.NavObject != null)
		{
			player.NavObject.UseOverrideColor = false;
		}
		this.MemberList.Remove(player);
		player.IsInPartyOfLocalPlayer = false;
		if (this.PartyMemberRemoved != null)
		{
			this.PartyMemberRemoved(player);
		}
		if (this.MemberList.Count != 1)
		{
			return true;
		}
		if (GameStats.GetBool(EnumGameStats.AutoParty) && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.MemberList[0].entityId == GameManager.Instance.World.GetPrimaryPlayerId())
		{
			return true;
		}
		this.MemberList[0].LeaveParty();
		return true;
	}

	// Token: 0x06003AFE RID: 15102 RVA: 0x0017BA38 File Offset: 0x00179C38
	public bool ContainsMember(EntityPlayer player)
	{
		return this.MemberList != null && this.MemberList.Contains(player);
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x0017BA50 File Offset: 0x00179C50
	public bool ContainsMember(int entityID)
	{
		if (this.MemberList == null)
		{
			return false;
		}
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			if (this.MemberList[i].entityId == entityID)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x0017BA94 File Offset: 0x00179C94
	[PublicizedFrom(EAccessModifier.Internal)]
	public int MemberCountInRange(EntityPlayer player)
	{
		int num = 0;
		for (int i = 0; i < player.Party.MemberList.Count; i++)
		{
			EntityPlayer entityPlayer = player.Party.MemberList[i];
			if (!(entityPlayer == player) && Vector3.Distance(player.position, entityPlayer.position) < (float)GameStats.GetInt(EnumGameStats.PartySharedKillRange))
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06003B01 RID: 15105 RVA: 0x0017BAFC File Offset: 0x00179CFC
	[PublicizedFrom(EAccessModifier.Internal)]
	public int MemberCountNotInRange(EntityPlayer player)
	{
		int num = 0;
		for (int i = 0; i < player.Party.MemberList.Count; i++)
		{
			EntityPlayer entityPlayer = player.Party.MemberList[i];
			if (!(entityPlayer == player) && Vector3.Distance(player.position, entityPlayer.position) >= 15f)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06003B02 RID: 15106 RVA: 0x0017BB60 File Offset: 0x00179D60
	public int MemberCountNotWithin(EntityPlayer player, Rect poiRect)
	{
		int num = 0;
		for (int i = 0; i < player.Party.MemberList.Count; i++)
		{
			EntityPlayer entityPlayer = player.Party.MemberList[i];
			if (!(entityPlayer == player))
			{
				Vector3 position = entityPlayer.position;
				position.y = position.z;
				if (!poiRect.Contains(position))
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x06003B03 RID: 15107 RVA: 0x0017BBC8 File Offset: 0x00179DC8
	public int GetPartyXP(EntityPlayer player, int startingXP)
	{
		int num = this.MemberCountInRange(player);
		return (int)((float)startingXP * (1f - 0.1f * (float)num));
	}

	// Token: 0x06003B04 RID: 15108 RVA: 0x0017BBF0 File Offset: 0x00179DF0
	public bool IsLocalParty()
	{
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			if (this.MemberList[i] is EntityPlayerLocal)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003B05 RID: 15109 RVA: 0x0017BC2C File Offset: 0x00179E2C
	public int[] GetMemberIdArray()
	{
		int[] array = new int[this.MemberList.Count];
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			array[i] = this.MemberList[i].entityId;
		}
		return array;
	}

	// Token: 0x06003B06 RID: 15110 RVA: 0x0017BC78 File Offset: 0x00179E78
	public List<int> GetMemberIdList(EntityPlayer exclude)
	{
		List<int> list = null;
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			if (!(this.MemberList[i] == exclude))
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(this.MemberList[i].entityId);
			}
		}
		return list;
	}

	// Token: 0x06003B07 RID: 15111 RVA: 0x0017BCD4 File Offset: 0x00179ED4
	[PublicizedFrom(EAccessModifier.Internal)]
	public void UpdateMemberList(World world, int[] partyMembers)
	{
		EntityPlayerLocal entityPlayerLocal = null;
		List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
		if (localPlayers != null && localPlayers.Count > 0)
		{
			entityPlayerLocal = localPlayers[0];
		}
		this.changedPlayers.Clear();
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < partyMembers.Length; j++)
			{
				if (this.MemberList[i].entityId == partyMembers[j])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.changedPlayers.Add(this.MemberList[i]);
			}
		}
		for (int k = 0; k < this.changedPlayers.Count; k++)
		{
			this.changedPlayers[k].Party = null;
			this.changedPlayers[k].IsInPartyOfLocalPlayer = false;
			this.changedPlayers[k].HandleOnPartyLeave(this);
			if (this.changedPlayers[k].NavObject != null && this.changedPlayers[k] != entityPlayerLocal)
			{
				this.changedPlayers[k].NavObject.UseOverrideColor = false;
			}
			if (entityPlayerLocal != null && entityPlayerLocal.Party == this)
			{
				entityPlayerLocal.QuestJournal.RemoveSharedQuestForOwner(this.changedPlayers[k].entityId);
				entityPlayerLocal.QuestJournal.RemoveSharedQuestEntryByOwner(this.changedPlayers[k].entityId);
			}
		}
		bool isInPartyOfLocalPlayer = false;
		this.changedPlayers.Clear();
		for (int l = 0; l < this.MemberList.Count; l++)
		{
			this.changedPlayers.Add(this.MemberList[l]);
		}
		this.MemberList.Clear();
		int index = 0;
		for (int m = 0; m < partyMembers.Length; m++)
		{
			bool flag = false;
			for (int n = 0; n < this.changedPlayers.Count; n++)
			{
				if (this.changedPlayers[n].entityId == partyMembers[m])
				{
					this.MemberList.Add(this.changedPlayers[n]);
					this.MemberList[index].Party = this;
					if (this.MemberList[index] is EntityPlayerLocal)
					{
						isInPartyOfLocalPlayer = true;
					}
					this.MemberList[index++].RemoveAllPartyInvites();
					this.changedPlayers.RemoveAt(n);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				EntityPlayer entityPlayer = world.GetEntity(partyMembers[m]) as EntityPlayer;
				if (entityPlayer != null)
				{
					this.MemberList.Add(entityPlayer);
					this.MemberList[index].Party = this;
					if (this.MemberList[index] is EntityPlayerLocal)
					{
						isInPartyOfLocalPlayer = true;
					}
					this.MemberList[index++].RemoveAllPartyInvites();
				}
			}
		}
		for (int num = 0; num < this.MemberList.Count; num++)
		{
			if (entityPlayerLocal != null && num != this.LeaderIndex)
			{
				entityPlayerLocal.RemovePartyInvite(this.MemberList[num].entityId);
			}
			if (this.MemberList[num].NavObject != null && entityPlayerLocal.Party == this.MemberList[num].Party)
			{
				this.MemberList[num].NavObject.UseOverrideColor = true;
				this.MemberList[num].NavObject.OverrideColor = Constants.TrackedFriendColors[num % Constants.TrackedFriendColors.Length];
				this.MemberList[num].NavObject.name = this.MemberList[num].PlayerDisplayName;
			}
			this.MemberList[num].IsInPartyOfLocalPlayer = isInPartyOfLocalPlayer;
			this.MemberList[num].HandleOnPartyJoined();
		}
	}

	// Token: 0x06003B08 RID: 15112 RVA: 0x0017C0E0 File Offset: 0x0017A2E0
	public void Disband()
	{
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			this.MemberList[i].LeaveParty();
		}
		PartyManager.Current.RemoveParty(this);
	}

	// Token: 0x06003B09 RID: 15113 RVA: 0x0017C120 File Offset: 0x0017A320
	public static void ServerHandleAcceptInvite(EntityPlayer invitedBy, EntityPlayer invitedEntity)
	{
		if (invitedBy.Party == null)
		{
			PartyManager.Current.CreateParty().AddPlayer(invitedBy);
		}
		invitedBy.Party.AddPlayer(invitedEntity);
		invitedEntity.RemoveAllPartyInvites();
		List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
		if (localPlayers != null && localPlayers.Count > 0)
		{
			EntityPlayerLocal entityPlayerLocal = localPlayers[0];
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.RemovePartyInvite(invitedEntity.entityId);
				GameManager.Instance.RemovePartyInvitesFromAllPlayers(entityPlayerLocal);
				if (entityPlayerLocal != invitedEntity && entityPlayerLocal.Party != null && entityPlayerLocal.Party == invitedEntity.Party)
				{
					Manager.PlayInsidePlayerHead("party_member_join", -1, 0f, false, false);
				}
				else if (entityPlayerLocal == invitedEntity)
				{
					Manager.PlayInsidePlayerHead("party_join", -1, 0f, false, false);
				}
			}
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyData>().Setup(invitedBy.Party, invitedEntity.entityId, NetPackagePartyData.PartyActions.AcceptInvite, false), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06003B0A RID: 15114 RVA: 0x0017C220 File Offset: 0x0017A420
	public static void ServerHandleChangeLead(EntityPlayer newHost)
	{
		newHost.Party.SetLeader(newHost);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyData>().Setup(newHost.Party, newHost.entityId, NetPackagePartyData.PartyActions.ChangeLead, false), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06003B0B RID: 15115 RVA: 0x0017C270 File Offset: 0x0017A470
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetLeader(EntityPlayer newHost)
	{
		this.LeaderIndex = this.MemberList.IndexOf(newHost);
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			this.MemberList[i].HandleOnPartyChanged();
		}
		if (this.PartyLeaderChanged != null)
		{
			this.PartyLeaderChanged(this, newHost);
		}
	}

	// Token: 0x06003B0C RID: 15116 RVA: 0x0017C2CC File Offset: 0x0017A4CC
	public static void ServerHandleKickParty(int entityID)
	{
		EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(entityID) as EntityPlayer;
		if (entityPlayer.Party != null)
		{
			Party party = entityPlayer.Party;
			EntityPlayer leader = party.Leader;
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer != null)
			{
				if (primaryPlayer == entityPlayer)
				{
					Manager.PlayInsidePlayerHead("party_leave", -1, 0f, false, false);
				}
				else if (primaryPlayer.Party == party)
				{
					primaryPlayer.QuestJournal.RemoveSharedQuestForOwner(entityPlayer.entityId);
					primaryPlayer.QuestJournal.RemoveSharedQuestEntryByOwner(entityPlayer.entityId);
					primaryPlayer.QuestJournal.RemovePlayerFromSharedWiths(entityPlayer);
					Manager.PlayInsidePlayerHead("party_member_leave", -1, 0f, false, false);
				}
			}
			party.KickPlayer(entityPlayer);
			entityPlayer.LeaveParty();
			if (leader == entityPlayer)
			{
				party.LeaderIndex = 0;
			}
			else
			{
				party.SetLeader(leader);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyData>().Setup(party, entityID, NetPackagePartyData.PartyActions.KickFromParty, party.MemberList.Count == 0), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06003B0D RID: 15117 RVA: 0x0017C3E8 File Offset: 0x0017A5E8
	public static void ServerHandleLeaveParty(EntityPlayer player, int entityID)
	{
		if (player.Party != null)
		{
			Party party = player.Party;
			EntityPlayer leader = party.Leader;
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer != null && primaryPlayer == player)
			{
				party.ClearAllNavObjectColors();
			}
			if (primaryPlayer != null)
			{
				if (primaryPlayer == player)
				{
					Manager.PlayInsidePlayerHead("party_leave", -1, 0f, false, false);
				}
				else if (primaryPlayer.Party == party)
				{
					primaryPlayer.QuestJournal.RemoveSharedQuestForOwner(player.entityId);
					primaryPlayer.QuestJournal.RemoveSharedQuestEntryByOwner(player.entityId);
					primaryPlayer.QuestJournal.RemovePlayerFromSharedWiths(player);
					Manager.PlayInsidePlayerHead("party_member_leave", -1, 0f, false, false);
				}
			}
			party.RemovePlayer(player);
			player.LeaveParty();
			if (leader == player)
			{
				party.LeaderIndex = 0;
			}
			else
			{
				party.SetLeader(leader);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyData>().Setup(party, entityID, NetPackagePartyData.PartyActions.LeaveParty, party.MemberList.Count == 0), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06003B0E RID: 15118 RVA: 0x0017C504 File Offset: 0x0017A704
	public static void ServerHandleDisconnectParty(EntityPlayer player)
	{
		if (player.Party != null)
		{
			Party party = player.Party;
			EntityPlayer leader = party.Leader;
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer != null)
			{
				if (primaryPlayer == player)
				{
					Manager.PlayInsidePlayerHead("party_leave", -1, 0f, false, false);
				}
				else if (primaryPlayer.Party == party)
				{
					primaryPlayer.QuestJournal.RemoveSharedQuestForOwner(player.entityId);
					primaryPlayer.QuestJournal.RemoveSharedQuestEntryByOwner(player.entityId);
					primaryPlayer.QuestJournal.RemovePlayerFromSharedWiths(player);
					Manager.PlayInsidePlayerHead("party_member_leave", -1, 0f, false, false);
				}
			}
			party.RemovePlayer(player);
			player.LeaveParty();
			if (leader == player)
			{
				party.LeaderIndex = 0;
			}
			else
			{
				party.SetLeader(leader);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyData>().Setup(party, player.entityId, NetPackagePartyData.PartyActions.Disconnected, party.MemberList.Count == 0), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06003B0F RID: 15119 RVA: 0x0017C60D File Offset: 0x0017A80D
	[PublicizedFrom(EAccessModifier.Internal)]
	public bool IsFull()
	{
		return this.MemberList.Count == 8;
	}

	// Token: 0x06003B10 RID: 15120 RVA: 0x0017C620 File Offset: 0x0017A820
	public static void ServerHandleAutoJoinParty(EntityPlayer joiningEntity)
	{
		Party party = PartyManager.Current.GetParty(1);
		if (party == null)
		{
			party = PartyManager.Current.CreateParty();
		}
		if (party.AddPlayer(joiningEntity))
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyData>().Setup(party, joiningEntity.entityId, NetPackagePartyData.PartyActions.AutoJoin, false), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06003B11 RID: 15121 RVA: 0x0017C680 File Offset: 0x0017A880
	public static void ServerHandleSetVoiceLoby(EntityPlayer player, string voiceLobbyId)
	{
		if (player.Party != null)
		{
			Party party = player.Party;
			party.VoiceLobbyId = voiceLobbyId;
			for (int i = 0; i < party.MemberList.Count; i++)
			{
				party.MemberList[i].HandleOnPartyChanged();
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyData>().Setup(party, player.entityId, NetPackagePartyData.PartyActions.SetVoiceLobby, party.MemberList.Count == 0), false, -1, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06003B12 RID: 15122 RVA: 0x0017C708 File Offset: 0x0017A908
	public void ClearAllNavObjectColors()
	{
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			if (this.MemberList[i].NavObject != null)
			{
				this.MemberList[i].NavObject.UseOverrideColor = false;
			}
		}
	}

	// Token: 0x06003B13 RID: 15123 RVA: 0x0017C758 File Offset: 0x0017A958
	[PublicizedFrom(EAccessModifier.Internal)]
	public EntityPlayer GetMemberAtIndex(int index, EntityPlayer excludePlayer)
	{
		int num = 0;
		for (int i = 0; i < this.MemberList.Count; i++)
		{
			if (this.MemberList[i] != excludePlayer)
			{
				num++;
			}
			if (num == index)
			{
				return this.MemberList[i];
			}
		}
		return null;
	}

	// Token: 0x04002FEC RID: 12268
	public int LeaderIndex;

	// Token: 0x04002FED RID: 12269
	public int PartyID = -1;

	// Token: 0x04002FEE RID: 12270
	public string VoiceLobbyId;

	// Token: 0x04002FEF RID: 12271
	public List<EntityPlayer> MemberList = new List<EntityPlayer>();

	// Token: 0x04002FF3 RID: 12275
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityPlayer> changedPlayers = new List<EntityPlayer>();
}
