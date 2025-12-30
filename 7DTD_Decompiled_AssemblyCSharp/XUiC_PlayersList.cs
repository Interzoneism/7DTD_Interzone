using System;
using System.Collections.Generic;
using GUI_2;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D70 RID: 3440
[Preserve]
public class XUiC_PlayersList : XUiController
{
	// Token: 0x06006B87 RID: 27527 RVA: 0x002BF8A0 File Offset: 0x002BDAA0
	public override void Init()
	{
		base.Init();
		this.playerList = (XUiV_Grid)base.GetChildById("playerList").ViewComponent;
		this.playerEntries = base.GetChildrenByType<XUiC_PlayersListEntry>(null);
		this.playerPager = (XUiC_Paging)base.GetChildById("playerPager");
		this.playerPager.OnPageChanged += this.updatePlayersList;
		this.numberOfPlayers = (XUiV_Label)base.GetChildById("numberOfPlayers").ViewComponent;
		if (Application.isPlaying)
		{
			GameManager.Instance.OnLocalPlayerChanged += this.onLocalPlayerChanged;
			base.xui.OnShutdown += this.onShutdown;
		}
		for (int i = 0; i < this.playerEntries.Length; i++)
		{
			this.playerEntries[i].PlayersList = this;
			this.playerEntries[i].IsAlternating = (i % 2 == 0);
		}
		if (XUiC_PlayersList.twitchDisabled == "")
		{
			XUiC_PlayersList.twitchDisabled = Localization.Get("xuiTwitchDisabled", false);
			XUiC_PlayersList.twitchSafe = Localization.Get("xuiTwitchSafe", false);
		}
	}

	// Token: 0x06006B88 RID: 27528 RVA: 0x002BF9BC File Offset: 0x002BDBBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void onShutdown()
	{
		base.xui.OnShutdown -= this.onShutdown;
		this.onLocalPlayerChanged(null);
		GameManager.Instance.OnLocalPlayerChanged -= this.onLocalPlayerChanged;
	}

	// Token: 0x06006B89 RID: 27529 RVA: 0x002BF9F4 File Offset: 0x002BDBF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~XUiC_PlayersList()
	{
		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnLocalPlayerChanged -= this.onLocalPlayerChanged;
		}
	}

	// Token: 0x06006B8A RID: 27530 RVA: 0x002BFA40 File Offset: 0x002BDC40
	[PublicizedFrom(EAccessModifier.Private)]
	public void onLocalPlayerChanged(EntityPlayerLocal _localPlayer)
	{
		if (_localPlayer != null)
		{
			return;
		}
		if (this.persistentLocalPlayer != null)
		{
			this.persistentLocalPlayer.RemovePlayerEventHandler(new PersistentPlayerData.PlayerEventHandler(this.OnPlayerEventHandler));
			this.persistentLocalPlayer = null;
		}
		if (this.persistentPlayerList != null)
		{
			this.persistentPlayerList.RemovePlayerEventHandler(new PersistentPlayerData.PlayerEventHandler(this.OnListEventHandler));
			this.persistentPlayerList = null;
		}
	}

	// Token: 0x06006B8B RID: 27531 RVA: 0x002BFAA4 File Offset: 0x002BDCA4
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.bOpened)
		{
			if (this.persistentPlayerList == null)
			{
				this.persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
				this.persistentPlayerList.AddPlayerEventHandler(new PersistentPlayerData.PlayerEventHandler(this.OnListEventHandler));
			}
			if (this.persistentLocalPlayer == null)
			{
				this.persistentLocalPlayer = GameManager.Instance.persistentLocalPlayer;
				this.persistentLocalPlayer.AddPlayerEventHandler(new PersistentPlayerData.PlayerEventHandler(this.OnPlayerEventHandler));
			}
		}
		this.bOpened = true;
		base.xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", false, false, true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoExit", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu, 0f);
		this.playerPager.Reset();
		this.updatePlayersList();
		XUiController xuiController = base.xui.FindWindowGroupByName("windowpaging");
		if (xuiController != null)
		{
			XUiC_WindowSelector childByType = xuiController.GetChildByType<XUiC_WindowSelector>();
			if (childByType != null)
			{
				childByType.SetSelected("players");
			}
		}
		this.windowGroup.isEscClosable = false;
	}

	// Token: 0x06006B8C RID: 27532 RVA: 0x002BFBE0 File Offset: 0x002BDDE0
	public override void OnClose()
	{
		base.OnClose();
		BlockedPlayerList instance = BlockedPlayerList.Instance;
		if (instance != null)
		{
			instance.MarkForWrite();
		}
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		this.bOpened = false;
	}

	// Token: 0x06006B8D RID: 27533 RVA: 0x002BFC38 File Offset: 0x002BDE38
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePlayersList()
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		List<XUiC_PlayersList.SEntityIdRef> list = new List<XUiC_PlayersList.SEntityIdRef>();
		for (int i = 0; i < GameManager.Instance.World.Players.list.Count; i++)
		{
			list.Add(new XUiC_PlayersList.SEntityIdRef(GameManager.Instance.World.Players.list[i]));
		}
		if (GameManager.Instance.persistentLocalPlayer.ACL != null)
		{
			foreach (PlatformUserIdentifierAbs userIdentifier in GameManager.Instance.persistentLocalPlayer.ACL)
			{
				PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(userIdentifier);
				if (playerData != null && !(GameManager.Instance.World.GetEntity(playerData.EntityId) != null))
				{
					list.Add(new XUiC_PlayersList.SEntityIdRef(playerData));
				}
			}
		}
		list.Sort();
		this.numberOfPlayers.Text = list.Count.ToString();
		this.playerPager.SetLastPageByElementsAndPageLength(list.Count, this.playerList.Rows);
		GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
		bool flag = gameServerInfo != null && gameServerInfo.AllowsCrossplay;
		if (!flag)
		{
			EPlayGroup eplayGroup = DeviceFlag.StandaloneWindows.ToPlayGroup();
			for (int j = 0; j < list.Count; j++)
			{
				PersistentPlayerData playerData2 = list[j].PlayerData;
				EPlayGroup eplayGroup2 = (playerData2 != null) ? playerData2.PlayGroup : EPlayGroup.Unknown;
				if (eplayGroup2 != EPlayGroup.Unknown && eplayGroup2 != eplayGroup)
				{
					flag = true;
					break;
				}
			}
		}
		int k;
		for (k = 0; k < this.playerList.Rows; k++)
		{
			if (k >= list.Count)
			{
				break;
			}
			int num = k + this.playerList.Rows * this.playerPager.GetPage();
			if (num >= list.Count)
			{
				break;
			}
			XUiC_PlayersListEntry xuiC_PlayersListEntry = this.playerEntries[k];
			if (xuiC_PlayersListEntry != null)
			{
				XUiC_PlayersList.SEntityIdRef sentityIdRef = list[num];
				EntityPlayer @ref = sentityIdRef.Ref;
				bool flag2 = @ref != null && @ref != entityPlayer && @ref.IsInPartyOfLocalPlayer;
				bool flag3 = @ref == null || (@ref != entityPlayer && @ref.IsFriendOfLocalPlayer);
				int entityId = (@ref == null) ? -1 : @ref.entityId;
				PersistentPlayerData persistentPlayerData = (sentityIdRef.PlayerId != null) ? GameManager.Instance.persistentPlayers.GetPlayerData(sentityIdRef.PlayerId) : GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(entityId);
				if (persistentPlayerData != null)
				{
					foreach (EBlockType eblockType in EnumUtils.Values<EBlockType>())
					{
						xuiC_PlayersListEntry.playerBlockStateChanged(persistentPlayerData.PlatformData, eblockType, persistentPlayerData.PlatformData.Blocked[eblockType].State);
					}
					if (@ref != null)
					{
						xuiC_PlayersListEntry.IsOffline = false;
						xuiC_PlayersListEntry.EntityId = @ref.entityId;
						xuiC_PlayersListEntry.PlayerData = persistentPlayerData;
						xuiC_PlayersListEntry.ViewComponent.IsVisible = true;
						xuiC_PlayersListEntry.PlayerName.UpdatePlayerData(persistentPlayerData.PlayerData, flag, persistentPlayerData.PlayerName.DisplayName);
						xuiC_PlayersListEntry.AdminSprite.IsVisible = @ref.IsAdmin;
						xuiC_PlayersListEntry.TwitchSprite.IsVisible = (@ref.TwitchEnabled && @ref.TwitchActionsEnabled == EntityPlayer.TwitchActionsStates.Enabled);
						xuiC_PlayersListEntry.TwitchDisabledSprite.IsVisible = (@ref.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled || @ref.TwitchSafe);
						xuiC_PlayersListEntry.TwitchDisabledSprite.SpriteName = ((@ref.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled) ? "ui_game_symbol_twitch_action_disabled" : "ui_game_symbol_brick");
						xuiC_PlayersListEntry.TwitchDisabledSprite.ToolTip = ((@ref.TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Enabled) ? XUiC_PlayersList.twitchDisabled : XUiC_PlayersList.twitchSafe);
						xuiC_PlayersListEntry.ZombieKillsText.Text = @ref.KilledZombies.ToString();
						xuiC_PlayersListEntry.PlayerKillsText.Text = @ref.KilledPlayers.ToString();
						xuiC_PlayersListEntry.DeathsText.Text = @ref.Died.ToString();
						xuiC_PlayersListEntry.LevelText.Text = @ref.Progression.GetLevel().ToString();
						xuiC_PlayersListEntry.GamestageText.Text = @ref.gameStage.ToString();
						xuiC_PlayersListEntry.PingText.Text = ((@ref == entityPlayer && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer) ? "--" : ((@ref.pingToServer < 0) ? "--" : @ref.pingToServer.ToString()));
						xuiC_PlayersListEntry.Voice.IsVisible = (@ref != entityPlayer);
						xuiC_PlayersListEntry.Chat.IsVisible = (@ref != entityPlayer);
						xuiC_PlayersListEntry.IsFriend = flag3;
						xuiC_PlayersListEntry.ShowOnMapEnabled = (flag3 || flag2);
						if (flag3 || flag2)
						{
							float magnitude = (@ref.GetPosition() - entityPlayer.GetPosition()).magnitude;
							xuiC_PlayersListEntry.DistanceToFriend.Text = ValueDisplayFormatters.Distance(magnitude);
						}
						else
						{
							xuiC_PlayersListEntry.DistanceToFriend.Text = "--";
						}
						xuiC_PlayersListEntry.buttonReportPlayer.IsVisible = (PlatformManager.MultiPlatform.PlayerReporting != null && @ref != entityPlayer);
						if (@ref == entityPlayer)
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.LocalPlayer;
						}
						else if (flag3)
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.Friends;
						}
						else if (this.invitesReceivedList.Contains(persistentPlayerData.PrimaryId))
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.Received;
						}
						else if (this.invitesSentList.Contains(persistentPlayerData.PrimaryId))
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.Sent;
						}
						else
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.NA;
						}
						if (@ref == entityPlayer)
						{
							if (entityPlayer.partyInvites.Contains(@ref))
							{
								xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_Received;
							}
							else if (entityPlayer.IsInParty())
							{
								xuiC_PlayersListEntry.PartyStatus = ((entityPlayer.Party.Leader == entityPlayer) ? XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_InPartyAsLead : XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_InParty);
							}
							else
							{
								xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_NoParty;
							}
						}
						else if (entityPlayer.IsInParty())
						{
							bool flag4 = entityPlayer.IsPartyLead();
							if (entityPlayer.Party.MemberList.Contains(@ref))
							{
								if (flag4)
								{
									xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InPartyAsLead;
								}
								else
								{
									xuiC_PlayersListEntry.PartyStatus = (@ref.IsPartyLead() ? XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InPartyIsLead : XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InParty);
								}
							}
							else if (@ref.IsInParty() && @ref.Party.IsFull())
							{
								xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_PartyFullAsLead;
							}
							else
							{
								xuiC_PlayersListEntry.PartyStatus = (flag4 ? XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoPartyAsLead : XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoParty);
							}
						}
						else if (entityPlayer.partyInvites.Contains(@ref))
						{
							if (@ref.IsInParty() && @ref.Party.IsFull())
							{
								xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoPartyAsLead;
								entityPlayer.partyInvites.Remove(@ref);
							}
							else
							{
								xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_Received;
							}
						}
						else
						{
							xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoPartyAsLead;
						}
					}
					else
					{
						xuiC_PlayersListEntry.IsOffline = true;
						xuiC_PlayersListEntry.EntityId = -1;
						xuiC_PlayersListEntry.PlayerData = persistentPlayerData;
						xuiC_PlayersListEntry.PlayerName.UpdatePlayerData(persistentPlayerData.PlayerData, flag, persistentPlayerData.PlayerName.DisplayName ?? sentityIdRef.PlayerId.CombinedString);
						xuiC_PlayersListEntry.AdminSprite.IsVisible = false;
						xuiC_PlayersListEntry.TwitchSprite.IsVisible = false;
						xuiC_PlayersListEntry.TwitchDisabledSprite.IsVisible = false;
						xuiC_PlayersListEntry.DistanceToFriend.IsVisible = true;
						xuiC_PlayersListEntry.DistanceToFriend.Text = "--";
						xuiC_PlayersListEntry.ZombieKillsText.Text = "--";
						xuiC_PlayersListEntry.PlayerKillsText.Text = "--";
						xuiC_PlayersListEntry.DeathsText.Text = "--";
						xuiC_PlayersListEntry.LevelText.Text = "--";
						xuiC_PlayersListEntry.GamestageText.Text = "--";
						xuiC_PlayersListEntry.PingText.Text = "--";
						xuiC_PlayersListEntry.Voice.IsVisible = false;
						xuiC_PlayersListEntry.Chat.IsVisible = false;
						xuiC_PlayersListEntry.IsOffline = true;
						if (@ref == entityPlayer)
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.LocalPlayer;
						}
						else if (flag3)
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.Friends;
						}
						else if (this.invitesReceivedList.Contains(persistentPlayerData.PrimaryId))
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.Received;
						}
						else if (this.invitesSentList.Contains(persistentPlayerData.PrimaryId))
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.Sent;
						}
						else
						{
							xuiC_PlayersListEntry.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.NA;
						}
						xuiC_PlayersListEntry.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.Offline;
						xuiC_PlayersListEntry.labelPartyIcon.IsVisible = true;
						xuiC_PlayersListEntry.buttonReportPlayer.IsVisible = true;
						xuiC_PlayersListEntry.buttonShowOnMap.IsVisible = false;
						xuiC_PlayersListEntry.labelShowOnMap.IsVisible = true;
					}
					xuiC_PlayersListEntry.RefreshBindings(false);
				}
			}
		}
		while (k < this.playerList.Rows)
		{
			XUiC_PlayersListEntry xuiC_PlayersListEntry2 = this.playerEntries[k];
			if (xuiC_PlayersListEntry2 != null)
			{
				xuiC_PlayersListEntry2.EntityId = -1;
				xuiC_PlayersListEntry2.PlayerData = null;
				xuiC_PlayersListEntry2.PlayerName.ClearPlayerData();
				xuiC_PlayersListEntry2.AdminSprite.IsVisible = false;
				xuiC_PlayersListEntry2.TwitchSprite.IsVisible = false;
				xuiC_PlayersListEntry2.TwitchDisabledSprite.IsVisible = false;
				xuiC_PlayersListEntry2.ZombieKillsText.Text = string.Empty;
				xuiC_PlayersListEntry2.PlayerKillsText.Text = string.Empty;
				xuiC_PlayersListEntry2.DeathsText.Text = string.Empty;
				xuiC_PlayersListEntry2.LevelText.Text = string.Empty;
				xuiC_PlayersListEntry2.GamestageText.Text = string.Empty;
				xuiC_PlayersListEntry2.PingText.Text = string.Empty;
				xuiC_PlayersListEntry2.Voice.IsVisible = false;
				xuiC_PlayersListEntry2.Chat.IsVisible = false;
				xuiC_PlayersListEntry2.ShowOnMapEnabled = false;
				xuiC_PlayersListEntry2.DistanceToFriend.IsVisible = false;
				xuiC_PlayersListEntry2.AllyStatus = XUiC_PlayersListEntry.EnumAllyInviteStatus.Empty;
				xuiC_PlayersListEntry2.PartyStatus = XUiC_PlayersListEntry.EnumPartyStatus.Offline;
				xuiC_PlayersListEntry2.buttonReportPlayer.IsVisible = false;
				xuiC_PlayersListEntry2.labelAllyIcon.IsVisible = false;
				xuiC_PlayersListEntry2.labelPartyIcon.IsVisible = false;
				xuiC_PlayersListEntry2.labelShowOnMap.IsVisible = false;
			}
			k++;
		}
	}

	// Token: 0x06006B8E RID: 27534 RVA: 0x002C0688 File Offset: 0x002BE888
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnListEventHandler(PersistentPlayerData _ppData, PersistentPlayerData _otherPlayer, EnumPersistentPlayerDataReason _reason)
	{
		if (_otherPlayer != null && _reason == EnumPersistentPlayerDataReason.Disconnected)
		{
			this.invitesReceivedList.Remove(_otherPlayer.PrimaryId);
			this.invitesSentList.Remove(_otherPlayer.PrimaryId);
		}
		this.updatePlayersList();
	}

	// Token: 0x06006B8F RID: 27535 RVA: 0x002C06BC File Offset: 0x002BE8BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPlayerEventHandler(PersistentPlayerData _localPpData, PersistentPlayerData _otherPpData, EnumPersistentPlayerDataReason _reason)
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		EntityPlayer entityPlayer2 = GameManager.Instance.World.GetEntity(_otherPpData.EntityId) as EntityPlayer;
		switch (_reason)
		{
		case EnumPersistentPlayerDataReason.ACL_AcceptedInvite:
			if (this.invitesSentList.Contains(_otherPpData.PrimaryId))
			{
				GameManager.ShowTooltip(entityPlayer, "friendInviteAccepted2", _otherPpData.PlayerName.SafeDisplayName, null, null, false, false, 0f);
			}
			this.invitesReceivedList.Remove(_otherPpData.PrimaryId);
			this.invitesSentList.Remove(_otherPpData.PrimaryId);
			break;
		case EnumPersistentPlayerDataReason.ACL_DeclinedInvite:
			GameManager.ShowTooltip(entityPlayer, "friendInviteDeclined2", _otherPpData.PlayerName.SafeDisplayName, null, null, false, false, 0f);
			this.invitesReceivedList.Remove(_otherPpData.PrimaryId);
			this.invitesSentList.Remove(_otherPpData.PrimaryId);
			break;
		case EnumPersistentPlayerDataReason.ACL_Removed:
			if ((entityPlayer2 != null && entityPlayer2.entityId != this.entityIdJustRemoved) || (entityPlayer2 == null && !_otherPpData.PrimaryId.Equals(this.playerIdJustRemoved)))
			{
				GameManager.ShowTooltip(entityPlayer, "friendRemoved2", _otherPpData.PlayerName.SafeDisplayName, null, null, false, false, 0f);
			}
			this.entityIdJustRemoved = -1;
			this.invitesReceivedList.Remove(_otherPpData.PrimaryId);
			this.invitesSentList.Remove(_otherPpData.PrimaryId);
			break;
		}
		this.updatePlayersList();
	}

	// Token: 0x06006B90 RID: 27536 RVA: 0x002C0833 File Offset: 0x002BEA33
	public bool AddInvite(PlatformUserIdentifierAbs _otherPlayerId)
	{
		if (this.invitesSentList.Contains(_otherPlayerId))
		{
			GameManager.Instance.ReplyToPlayerACLInvite(_otherPlayerId, true);
			return true;
		}
		if (!this.invitesReceivedList.Contains(_otherPlayerId))
		{
			this.invitesReceivedList.Add(_otherPlayerId);
			return true;
		}
		return false;
	}

	// Token: 0x06006B91 RID: 27537 RVA: 0x002C0870 File Offset: 0x002BEA70
	public void AddInvitePress(int _otherPlayerId)
	{
		PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(_otherPlayerId);
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (this.invitesReceivedList.Contains(playerDataFromEntityID.PrimaryId))
		{
			GameManager.Instance.ReplyToPlayerACLInvite(playerDataFromEntityID.PrimaryId, true);
			this.invitesSentList.Remove(playerDataFromEntityID.PrimaryId);
			this.invitesReceivedList.Remove(playerDataFromEntityID.PrimaryId);
			GameManager.ShowTooltip(entityPlayer, "friendInviteAccepted", ((EntityPlayer)GameManager.Instance.World.GetEntity(_otherPlayerId)).PlayerDisplayName, null, null, false, false, 0f);
		}
		else
		{
			GameManager.Instance.SendPlayerACLInvite(playerDataFromEntityID);
			GameManager.ShowTooltip(entityPlayer, "friendSentInvite", ((EntityPlayer)GameManager.Instance.World.GetEntity(_otherPlayerId)).PlayerDisplayName, null, null, false, false, 0f);
			this.invitesSentList.Add(playerDataFromEntityID.PrimaryId);
		}
		this.updatePlayersList();
	}

	// Token: 0x06006B92 RID: 27538 RVA: 0x002C0968 File Offset: 0x002BEB68
	public void RemoveInvitePress(PersistentPlayerData _otherPpData)
	{
		EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(_otherPpData.EntityId) as EntityPlayer;
		if (entityPlayer != null)
		{
			PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(base.xui.playerUI.entityPlayer.entityId);
			if (entityPlayer.IsFriendOfLocalPlayer || (playerDataFromEntityID.ACL != null && playerDataFromEntityID.ACL.Contains(_otherPpData.PrimaryId)))
			{
				GameManager.Instance.RemovePlayerFromACL(_otherPpData);
			}
			else
			{
				GameManager.Instance.ReplyToPlayerACLInvite(_otherPpData.PrimaryId, false);
				this.invitesReceivedList.Remove(_otherPpData.PrimaryId);
				this.invitesSentList.Remove(_otherPpData.PrimaryId);
			}
			GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "friendRemoved", entityPlayer.PlayerDisplayName, null, null, false, false, 0f);
			this.entityIdJustRemoved = entityPlayer.entityId;
		}
		else
		{
			this.playerIdJustRemoved = _otherPpData.PrimaryId;
			GameManager.Instance.RemovePlayerFromACL(_otherPpData);
			GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "friendRemoved", _otherPpData.PlayerName.DisplayName, null, null, false, false, 0f);
		}
		this.updatePlayersList();
	}

	// Token: 0x06006B93 RID: 27539 RVA: 0x002C0AA8 File Offset: 0x002BECA8
	public void ShowOnMap(int _playerId)
	{
		Entity entity = GameManager.Instance.World.GetEntity(_playerId);
		if (entity == null)
		{
			return;
		}
		XUiC_WindowSelector.OpenSelectorAndWindow(base.xui.playerUI.entityPlayer, "map");
		((XUiC_MapArea)base.xui.GetWindow("mapArea").Controller).PositionMapAt(entity.GetPosition());
	}

	// Token: 0x06006B94 RID: 27540 RVA: 0x002C0B10 File Offset: 0x002BED10
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.updateLimiter -= _dt;
		if (base.ViewComponent.IsVisible && this.updateLimiter < 0f)
		{
			this.updateLimiter = 1f;
			this.updatePlayersList();
		}
	}

	// Token: 0x040051B6 RID: 20918
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Grid playerList;

	// Token: 0x040051B7 RID: 20919
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PlayersListEntry[] playerEntries;

	// Token: 0x040051B8 RID: 20920
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Paging playerPager;

	// Token: 0x040051B9 RID: 20921
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<PlatformUserIdentifierAbs> invitesReceivedList = new List<PlatformUserIdentifierAbs>();

	// Token: 0x040051BA RID: 20922
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<PlatformUserIdentifierAbs> invitesSentList = new List<PlatformUserIdentifierAbs>();

	// Token: 0x040051BB RID: 20923
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label numberOfPlayers;

	// Token: 0x040051BC RID: 20924
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite reportHeaderSprite;

	// Token: 0x040051BD RID: 20925
	[PublicizedFrom(EAccessModifier.Private)]
	public PersistentPlayerData persistentLocalPlayer;

	// Token: 0x040051BE RID: 20926
	[PublicizedFrom(EAccessModifier.Private)]
	public PersistentPlayerList persistentPlayerList;

	// Token: 0x040051BF RID: 20927
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateLimiter;

	// Token: 0x040051C0 RID: 20928
	[PublicizedFrom(EAccessModifier.Private)]
	public static string twitchDisabled = "";

	// Token: 0x040051C1 RID: 20929
	[PublicizedFrom(EAccessModifier.Private)]
	public static string twitchSafe = "";

	// Token: 0x040051C2 RID: 20930
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bOpened;

	// Token: 0x040051C3 RID: 20931
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityIdJustRemoved = -1;

	// Token: 0x040051C4 RID: 20932
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs playerIdJustRemoved;

	// Token: 0x02000D71 RID: 3441
	[PublicizedFrom(EAccessModifier.Private)]
	public struct SEntityIdRef : IComparable
	{
		// Token: 0x06006B97 RID: 27543 RVA: 0x002C0B98 File Offset: 0x002BED98
		public SEntityIdRef(EntityPlayer _ref)
		{
			this.EntityId = _ref.entityId;
			this.Ref = _ref;
			this.PlayerData = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.EntityId);
			PersistentPlayerData playerData = this.PlayerData;
			this.PlayerId = ((playerData != null) ? playerData.PrimaryId : null);
		}

		// Token: 0x06006B98 RID: 27544 RVA: 0x002C0BEB File Offset: 0x002BEDEB
		public SEntityIdRef(PersistentPlayerData _playerData)
		{
			this.EntityId = -1;
			this.Ref = null;
			this.PlayerData = _playerData;
			PersistentPlayerData playerData = this.PlayerData;
			this.PlayerId = ((playerData != null) ? playerData.PrimaryId : null);
		}

		// Token: 0x06006B99 RID: 27545 RVA: 0x002C0C1C File Offset: 0x002BEE1C
		public int CompareTo(object _other)
		{
			if (!(_other is XUiC_PlayersList.SEntityIdRef))
			{
				return 1;
			}
			XUiC_PlayersList.SEntityIdRef sentityIdRef = (XUiC_PlayersList.SEntityIdRef)_other;
			if (this.Ref == null)
			{
				return 1;
			}
			if (sentityIdRef.Ref == null)
			{
				return -1;
			}
			if (this.Ref is EntityPlayerLocal)
			{
				return -1;
			}
			if (sentityIdRef.Ref is EntityPlayerLocal)
			{
				return 1;
			}
			if (this.Ref.IsFriendOfLocalPlayer && sentityIdRef.Ref.IsFriendOfLocalPlayer)
			{
				if (this.Ref.Progression.GetLevel() > sentityIdRef.Ref.Progression.GetLevel())
				{
					return -1;
				}
				if (this.Ref.Progression.GetLevel() != sentityIdRef.Ref.Progression.GetLevel())
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (this.Ref.IsFriendOfLocalPlayer)
				{
					return -1;
				}
				if (sentityIdRef.Ref.IsFriendOfLocalPlayer)
				{
					return 1;
				}
				if (this.Ref.Progression.GetLevel() > sentityIdRef.Ref.Progression.GetLevel())
				{
					return -1;
				}
				if (this.Ref.Progression.GetLevel() != sentityIdRef.Ref.Progression.GetLevel())
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x040051C5 RID: 20933
		public readonly PersistentPlayerData PlayerData;

		// Token: 0x040051C6 RID: 20934
		public readonly PlatformUserIdentifierAbs PlayerId;

		// Token: 0x040051C7 RID: 20935
		public readonly int EntityId;

		// Token: 0x040051C8 RID: 20936
		public readonly EntityPlayer Ref;
	}
}
