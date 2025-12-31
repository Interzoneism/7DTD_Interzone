using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x0200076A RID: 1898
[Preserve]
public class NetPackagePartyData : NetPackage
{
	// Token: 0x1700059B RID: 1435
	// (get) Token: 0x0600373F RID: 14143 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003740 RID: 14144 RVA: 0x0016A220 File Offset: 0x00168420
	public NetPackagePartyData Setup(Party _party, int _changedEntityID, NetPackagePartyData.PartyActions _partyAction, bool _disbandParty = false)
	{
		this.PartyID = _party.PartyID;
		this.LeaderIndex = _party.LeaderIndex;
		this.VoiceLobbyId = _party.VoiceLobbyId;
		this.partyMembers = _party.GetMemberIdArray();
		this.changedEntityID = _changedEntityID;
		this.partyAction = _partyAction;
		this.disbandParty = _disbandParty;
		return this;
	}

	// Token: 0x06003741 RID: 14145 RVA: 0x0016A274 File Offset: 0x00168474
	public override void read(PooledBinaryReader _br)
	{
		this.PartyID = _br.ReadInt32();
		this.LeaderIndex = (int)_br.ReadByte();
		this.VoiceLobbyId = _br.ReadString();
		int num = _br.ReadInt32();
		this.partyMembers = new int[num];
		for (int i = 0; i < num; i++)
		{
			this.partyMembers[i] = _br.ReadInt32();
		}
		this.changedEntityID = _br.ReadInt32();
		this.partyAction = (NetPackagePartyData.PartyActions)_br.ReadByte();
		this.disbandParty = _br.ReadBoolean();
	}

	// Token: 0x06003742 RID: 14146 RVA: 0x0016A2F8 File Offset: 0x001684F8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.PartyID);
		_bw.Write((byte)this.LeaderIndex);
		_bw.Write(this.VoiceLobbyId ?? "");
		_bw.Write(this.partyMembers.Length);
		for (int i = 0; i < this.partyMembers.Length; i++)
		{
			_bw.Write(this.partyMembers[i]);
		}
		_bw.Write(this.changedEntityID);
		_bw.Write((byte)this.partyAction);
		_bw.Write(this.disbandParty);
	}

	// Token: 0x06003743 RID: 14147 RVA: 0x0016A390 File Offset: 0x00168590
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		Party party = PartyManager.Current.GetParty(this.PartyID);
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		Party party2 = null;
		if (primaryPlayer != null)
		{
			party2 = primaryPlayer.Party;
		}
		if (party == null)
		{
			party = PartyManager.Current.CreateClientParty(_world, this.PartyID, this.LeaderIndex, this.partyMembers, this.VoiceLobbyId);
		}
		else
		{
			party.LeaderIndex = this.LeaderIndex;
			party.VoiceLobbyId = this.VoiceLobbyId;
			party.UpdateMemberList(_world, this.partyMembers);
		}
		if (primaryPlayer != null)
		{
			EntityPlayer entityPlayer = (EntityPlayer)_world.GetEntity(this.changedEntityID);
			if (primaryPlayer == entityPlayer)
			{
				NetPackagePartyData.PartyActions partyActions = this.partyAction;
				if (partyActions != NetPackagePartyData.PartyActions.LeaveParty)
				{
					if (partyActions == NetPackagePartyData.PartyActions.KickFromParty)
					{
						Manager.PlayInsidePlayerHead("party_leave", -1, 0f, false, false);
						GameManager.ShowTooltip(primaryPlayer, Localization.Get("ttPartyKickedFromParty", false), false, false, 0f);
					}
				}
				else
				{
					Manager.PlayInsidePlayerHead("party_leave", -1, 0f, false, false);
				}
			}
			else if (party2 == party && this.changedEntityID != -1)
			{
				switch (this.partyAction)
				{
				case NetPackagePartyData.PartyActions.AcceptInvite:
					entityPlayer.RemoveAllPartyInvites();
					if (entityPlayer == primaryPlayer)
					{
						GameManager.Instance.RemovePartyInvitesFromAllPlayers(entityPlayer);
						Manager.PlayInsidePlayerHead("party_join", -1, 0f, false, false);
					}
					else
					{
						Manager.PlayInsidePlayerHead("party_member_join", -1, 0f, false, false);
					}
					break;
				case NetPackagePartyData.PartyActions.LeaveParty:
					entityPlayer.Party = null;
					if (entityPlayer != primaryPlayer)
					{
						Manager.PlayInsidePlayerHead("party_member_leave", -1, 0f, false, false);
						GameManager.ShowTooltip(primaryPlayer, string.Format(Localization.Get("ttPartyOtherLeftParty", false), entityPlayer.PlayerDisplayName), false, false, 0f);
					}
					break;
				case NetPackagePartyData.PartyActions.KickFromParty:
					entityPlayer.Party = null;
					if (entityPlayer != primaryPlayer)
					{
						Manager.PlayInsidePlayerHead("party_member_leave", -1, 0f, false, false);
						GameManager.ShowTooltip(primaryPlayer, string.Format(Localization.Get("ttPartyOtherKickedFromParty", false), entityPlayer.PlayerDisplayName), false, false, 0f);
						primaryPlayer.QuestJournal.RemovePlayerFromSharedWiths(entityPlayer);
					}
					break;
				case NetPackagePartyData.PartyActions.Disconnected:
					if (entityPlayer != primaryPlayer)
					{
						Manager.PlayInsidePlayerHead("party_member_leave", -1, 0f, false, false);
						GameManager.ShowTooltip(primaryPlayer, string.Format(Localization.Get("ttPartyDisconnectedFromParty", false), entityPlayer.PlayerDisplayName), false, false, 0f);
						primaryPlayer.QuestJournal.RemovePlayerFromSharedWiths(entityPlayer);
					}
					break;
				case NetPackagePartyData.PartyActions.SetVoiceLobby:
					for (int i = 0; i < party.MemberList.Count; i++)
					{
						party.MemberList[i].HandleOnPartyChanged();
					}
					break;
				}
			}
		}
		if (this.disbandParty)
		{
			party.Disband();
		}
	}

	// Token: 0x06003744 RID: 14148 RVA: 0x0011934C File Offset: 0x0011754C
	public override int GetLength()
	{
		return 9;
	}

	// Token: 0x04002CCF RID: 11471
	[PublicizedFrom(EAccessModifier.Private)]
	public int PartyID;

	// Token: 0x04002CD0 RID: 11472
	[PublicizedFrom(EAccessModifier.Private)]
	public int LeaderIndex;

	// Token: 0x04002CD1 RID: 11473
	[PublicizedFrom(EAccessModifier.Private)]
	public string VoiceLobbyId;

	// Token: 0x04002CD2 RID: 11474
	[PublicizedFrom(EAccessModifier.Private)]
	public int[] partyMembers;

	// Token: 0x04002CD3 RID: 11475
	[PublicizedFrom(EAccessModifier.Private)]
	public int changedEntityID = -1;

	// Token: 0x04002CD4 RID: 11476
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackagePartyData.PartyActions partyAction;

	// Token: 0x04002CD5 RID: 11477
	[PublicizedFrom(EAccessModifier.Private)]
	public bool disbandParty;

	// Token: 0x0200076B RID: 1899
	public enum PartyActions
	{
		// Token: 0x04002CD7 RID: 11479
		SendInvite,
		// Token: 0x04002CD8 RID: 11480
		AcceptInvite,
		// Token: 0x04002CD9 RID: 11481
		ChangeLead,
		// Token: 0x04002CDA RID: 11482
		LeaveParty,
		// Token: 0x04002CDB RID: 11483
		KickFromParty,
		// Token: 0x04002CDC RID: 11484
		Disconnected,
		// Token: 0x04002CDD RID: 11485
		AutoJoin,
		// Token: 0x04002CDE RID: 11486
		SetVoiceLobby
	}
}
