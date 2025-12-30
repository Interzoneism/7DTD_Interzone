using System;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D72 RID: 3442
[Preserve]
public class XUiC_PlayersListEntry : XUiController
{
	// Token: 0x17000ACD RID: 2765
	// (set) Token: 0x06006B9A RID: 27546 RVA: 0x002C0D45 File Offset: 0x002BEF45
	public bool ShowOnMapEnabled
	{
		set
		{
			this.buttonShowOnMap.Enabled = value;
			this.buttonShowOnMap.IsVisible = value;
			this.labelShowOnMap.IsVisible = !value;
		}
	}

	// Token: 0x17000ACE RID: 2766
	// (set) Token: 0x06006B9B RID: 27547 RVA: 0x002C0D70 File Offset: 0x002BEF70
	public bool IsOffline
	{
		set
		{
			this.isOffline = value;
			Color color = this.isOffline ? this.disabledColor : this.enabledColor;
			if (this.PlayerName.Color != color)
			{
				this.PlayerName.Color = color;
				this.LevelText.Color = color;
				this.GamestageText.Color = color;
				this.labelPartyIcon.Color = color;
				this.labelAllyIcon.Color = color;
				this.labelShowOnMap.Color = color;
				this.buttonAllyIcon.CurrentColor = color;
				this.DistanceToFriend.Color = color;
				this.ZombieKillsText.Color = color;
				this.PlayerKillsText.Color = color;
				this.DeathsText.Color = color;
				this.PingText.Color = color;
			}
		}
	}

	// Token: 0x17000ACF RID: 2767
	// (get) Token: 0x06006B9C RID: 27548 RVA: 0x002C0E41 File Offset: 0x002BF041
	// (set) Token: 0x06006B9D RID: 27549 RVA: 0x002C0E49 File Offset: 0x002BF049
	public XUiC_PlayersListEntry.EnumAllyInviteStatus AllyStatus
	{
		get
		{
			return this.m_allyStatus;
		}
		set
		{
			this.m_allyStatus = value;
			this.updateInviteStatus();
		}
	}

	// Token: 0x17000AD0 RID: 2768
	// (get) Token: 0x06006B9E RID: 27550 RVA: 0x002C0E58 File Offset: 0x002BF058
	// (set) Token: 0x06006B9F RID: 27551 RVA: 0x002C0E60 File Offset: 0x002BF060
	public XUiC_PlayersListEntry.EnumPartyStatus PartyStatus
	{
		get
		{
			return this.m_partyStatus;
		}
		set
		{
			this.m_partyStatus = value;
			this.updatePartyStatus();
		}
	}

	// Token: 0x17000AD1 RID: 2769
	// (set) Token: 0x06006BA0 RID: 27552 RVA: 0x002C0E6F File Offset: 0x002BF06F
	public bool IsAlternating
	{
		set
		{
			if (value)
			{
				this.rowBG.Color = this.alternatingColor;
			}
		}
	}

	// Token: 0x17000AD2 RID: 2770
	// (get) Token: 0x06006BA1 RID: 27553 RVA: 0x002C0E85 File Offset: 0x002BF085
	public static string SentKeyword
	{
		get
		{
			if (XUiC_PlayersListEntry.sentKeyword == "")
			{
				XUiC_PlayersListEntry.sentKeyword = Localization.Get("xuiSent", false);
			}
			return XUiC_PlayersListEntry.sentKeyword;
		}
	}

	// Token: 0x17000AD3 RID: 2771
	// (get) Token: 0x06006BA2 RID: 27554 RVA: 0x002C0EAD File Offset: 0x002BF0AD
	public static string ReceivedKeyword
	{
		get
		{
			if (XUiC_PlayersListEntry.receivedKeyword == "")
			{
				XUiC_PlayersListEntry.receivedKeyword = Localization.Get("xuiReceived", false);
			}
			return XUiC_PlayersListEntry.receivedKeyword;
		}
	}

	// Token: 0x17000AD4 RID: 2772
	// (get) Token: 0x06006BA3 RID: 27555 RVA: 0x002C0ED5 File Offset: 0x002BF0D5
	public static string NAKeyword
	{
		get
		{
			if (XUiC_PlayersListEntry.naKeyword == "")
			{
				XUiC_PlayersListEntry.naKeyword = Localization.Get("xuiNA", false);
			}
			return XUiC_PlayersListEntry.naKeyword;
		}
	}

	// Token: 0x06006BA4 RID: 27556 RVA: 0x002C0F00 File Offset: 0x002BF100
	public override void Init()
	{
		base.Init();
		this.PlayerName = (XUiC_PlayerName)base.GetChildById("playerName");
		this.AdminSprite = (XUiV_Sprite)base.GetChildById("admin").ViewComponent;
		this.TwitchSprite = (XUiV_Sprite)base.GetChildById("twitch").ViewComponent;
		this.TwitchDisabledSprite = (XUiV_Sprite)base.GetChildById("twitchDisabled").ViewComponent;
		this.ZombieKillsText = (XUiV_Label)base.GetChildById("zombieKillsText").ViewComponent;
		this.PlayerKillsText = (XUiV_Label)base.GetChildById("playerKillsText").ViewComponent;
		this.DeathsText = (XUiV_Label)base.GetChildById("deathsText").ViewComponent;
		this.LevelText = (XUiV_Label)base.GetChildById("levelText").ViewComponent;
		this.PingText = (XUiV_Label)base.GetChildById("pingText").ViewComponent;
		this.GamestageText = (XUiV_Label)base.GetChildById("gamestageText").ViewComponent;
		this.Voice = (XUiV_Button)base.GetChildById("iconVoice").ViewComponent;
		this.Voice.Controller.OnPress += this.voiceChatButtonOnPress;
		this.Chat = (XUiV_Button)base.GetChildById("iconChat").ViewComponent;
		this.Chat.Controller.OnPress += this.textChatButtonOnPress;
		base.xui.OnShutdown += this.Shutdown;
		this.buttonShowOnMap = (XUiV_Button)base.GetChildById("iconShowOnMap").ViewComponent;
		this.DistanceToFriend = (XUiV_Label)base.GetChildById("labelDistanceWalked").ViewComponent;
		this.rowBG = (XUiV_Sprite)base.GetChildById("background").ViewComponent;
		this.buttonAllyIcon = (XUiV_Button)base.GetChildById("iconAllyIcon").ViewComponent;
		this.buttonAllyIcon.Controller.OnPress += this.oniconAllyIconPress;
		this.buttonPartyIcon = (XUiV_Button)base.GetChildById("iconPartyIcon").ViewComponent;
		this.buttonPartyIcon.Controller.OnPress += this.oniconPartyIconPress;
		this.buttonReportPlayer = (XUiV_Button)base.GetChildById("btnReportPlayer").ViewComponent;
		this.buttonReportPlayer.Controller.OnPress += this.onReportPlayerPress;
		this.buttonShowOnMap.Controller.OnPress += this.onShowOnMapPress;
		this.enabledColor = this.PingText.Color;
		this.labelPartyIcon = (XUiV_Label)base.GetChildById("labelPartyIcon").ViewComponent;
		this.labelAllyIcon = (XUiV_Label)base.GetChildById("labelAllyIcon").ViewComponent;
		this.labelShowOnMap = (XUiV_Label)base.GetChildById("labelShowOnMap").ViewComponent;
		PlatformUserManager.BlockedStateChanged += this.playerBlockStateChanged;
	}

	// Token: 0x06006BA5 RID: 27557 RVA: 0x002C1226 File Offset: 0x002BF426
	[PublicizedFrom(EAccessModifier.Private)]
	public void Shutdown()
	{
		base.xui.OnShutdown -= this.Shutdown;
		PlatformUserManager.BlockedStateChanged -= this.playerBlockStateChanged;
	}

	// Token: 0x06006BA6 RID: 27558 RVA: 0x002C1250 File Offset: 0x002BF450
	[PublicizedFrom(EAccessModifier.Private)]
	public void oniconAllyIconPress(XUiController _sender, int _mouseButton)
	{
		switch (this.m_allyStatus)
		{
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.NA:
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.Received:
			this.PlayersList.AddInvitePress(this.EntityId);
			return;
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.Friends:
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.Sent:
			base.xui.currentPopupMenu.Setup(new Vector2i(0, -26), this.buttonAllyIcon);
			base.xui.currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("lblRemove", false), "ui_game_symbol_x", true, Array.Empty<object>(), new XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate(this.RemoveAlly_ItemClicked)));
			return;
		default:
			return;
		}
	}

	// Token: 0x06006BA7 RID: 27559 RVA: 0x002C12E4 File Offset: 0x002BF4E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemoveAlly_ItemClicked(XUiC_PopupMenuItem.Entry entry)
	{
		if (this.PlayerData != null)
		{
			this.PlayersList.RemoveInvitePress(this.PlayerData);
		}
	}

	// Token: 0x06006BA8 RID: 27560 RVA: 0x002C1300 File Offset: 0x002BF500
	[PublicizedFrom(EAccessModifier.Private)]
	public void oniconPartyIconPress(XUiController _sender, int _mouseButton)
	{
		if (GameStats.GetBool(EnumGameStats.AutoParty))
		{
			return;
		}
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		switch (this.m_partyStatus)
		{
		case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_InParty:
			base.xui.currentPopupMenu.Setup(new Vector2i(0, -26), this.buttonPartyIcon);
			base.xui.currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("lblLeave", false), "ui_game_symbol_x", true, Array.Empty<object>(), new XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate(this.LeaveParty_ItemClicked)));
			return;
		case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_InPartyAsLead:
			base.xui.currentPopupMenu.Setup(new Vector2i(0, -26), this.buttonPartyIcon);
			base.xui.currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("lblLeave", false), "ui_game_symbol_x", true, Array.Empty<object>(), new XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate(this.LeaveParty_ItemClicked)));
			return;
		case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_NoParty:
		case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoPartyAsLead:
		{
			EntityPlayer entityPlayer2 = GameManager.Instance.World.GetEntity(this.EntityId) as EntityPlayer;
			if (Time.time <= this.lastTime)
			{
				GameManager.ShowTooltip(entityPlayer, string.Format(Localization.Get("ttPartyInviteWait", false), entityPlayer2.PlayerDisplayName), false, false, 0f);
				return;
			}
			this.lastTime = Time.time + 5f;
			if (!entityPlayer2.partyInvites.Contains(entityPlayer))
			{
				entityPlayer2.AddPartyInvite(entityPlayer.entityId);
			}
			GameManager.ShowTooltip(entityPlayer, string.Format(Localization.Get("ttPartyInviteSent", false), entityPlayer2.PlayerDisplayName), false, false, 0f);
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.SendInvite, entityPlayer.entityId, this.EntityId, null, null), false);
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.SendInvite, entityPlayer.entityId, this.EntityId, null, null), false, -1, -1, -1, null, 192, false);
			return;
		}
		case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_Received:
		{
			EntityPlayer invitedBy = GameManager.Instance.World.GetEntity(this.EntityId) as EntityPlayer;
			Manager.PlayInsidePlayerHead("party_join", -1, 0f, false, false);
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.AcceptInvite, this.EntityId, entityPlayer.entityId, null, null), false);
				return;
			}
			if (entityPlayer.Party == null)
			{
				Party.ServerHandleAcceptInvite(invitedBy, entityPlayer);
			}
			break;
		}
		case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InParty:
		case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InPartyIsLead:
		case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoParty:
			break;
		case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InPartyAsLead:
			base.xui.currentPopupMenu.Setup(new Vector2i(0, -26), this.buttonPartyIcon);
			base.xui.currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("lblKick", false), "ui_game_symbol_x", true, Array.Empty<object>(), new XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate(this.KickParty_ItemClicked)));
			base.xui.currentPopupMenu.AddItem(new XUiC_PopupMenuItem.Entry(Localization.Get("lblMakeLeader", false), "server_favorite", true, Array.Empty<object>(), new XUiC_PopupMenuItem.Entry.MenuItemClickedDelegate(this.MakeLeader_ItemClicked)));
			return;
		default:
			return;
		}
	}

	// Token: 0x06006BA9 RID: 27561 RVA: 0x002C1608 File Offset: 0x002BF808
	[PublicizedFrom(EAccessModifier.Private)]
	public void LeaveParty_ItemClicked(XUiC_PopupMenuItem.Entry entry)
	{
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		Manager.PlayInsidePlayerHead("party_invite_leave", -1, 0f, false, false);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.LeaveParty, entityPlayer.entityId, this.EntityId, null, null), false);
			return;
		}
		Party.ServerHandleLeaveParty(entityPlayer, this.EntityId);
	}

	// Token: 0x06006BAA RID: 27562 RVA: 0x002C1678 File Offset: 0x002BF878
	[PublicizedFrom(EAccessModifier.Private)]
	public void MakeLeader_ItemClicked(XUiC_PopupMenuItem.Entry entry)
	{
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.ChangeLead, entityPlayer.entityId, this.EntityId, null, null), false);
			return;
		}
		Party.ServerHandleChangeLead(GameManager.Instance.World.GetEntity(this.EntityId) as EntityPlayer);
	}

	// Token: 0x06006BAB RID: 27563 RVA: 0x002C16E8 File Offset: 0x002BF8E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void KickParty_ItemClicked(XUiC_PopupMenuItem.Entry entry)
	{
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackagePartyActions>().Setup(NetPackagePartyActions.PartyActions.KickFromParty, entityPlayer.entityId, this.EntityId, null, null), false);
			return;
		}
		Party.ServerHandleKickParty(this.EntityId);
	}

	// Token: 0x06006BAC RID: 27564 RVA: 0x002C1742 File Offset: 0x002BF942
	[PublicizedFrom(EAccessModifier.Private)]
	public void onReportPlayerPress(XUiController _sender, int _mouseButton)
	{
		if (PlatformManager.MultiPlatform.PlayerReporting != null && this.PlayerData != null)
		{
			XUiC_ReportPlayer.Open(this.PlayerData.PlayerData, "");
		}
	}

	// Token: 0x06006BAD RID: 27565 RVA: 0x002C1770 File Offset: 0x002BF970
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateInviteStatus()
	{
		switch (this.m_allyStatus)
		{
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.LocalPlayer:
			this.labelAllyIcon.IsVisible = true;
			this.buttonAllyIcon.IsVisible = false;
			return;
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.NA:
			this.labelAllyIcon.IsVisible = false;
			this.buttonAllyIcon.IsVisible = true;
			this.buttonAllyIcon.Enabled = true;
			this.buttonAllyIcon.DefaultSpriteName = "ui_game_symbol_add";
			return;
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.Friends:
		{
			this.labelAllyIcon.IsVisible = false;
			this.buttonAllyIcon.IsVisible = true;
			this.buttonAllyIcon.Enabled = true;
			this.buttonAllyIcon.DefaultSpriteName = "ui_game_symbol_allies";
			bool flag = this.isOffline;
			return;
		}
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.Sent:
			this.labelAllyIcon.IsVisible = false;
			this.buttonAllyIcon.IsVisible = true;
			this.buttonAllyIcon.Enabled = false;
			this.buttonAllyIcon.DefaultSpriteName = "ui_game_symbol_invite";
			return;
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.Received:
			this.labelAllyIcon.IsVisible = false;
			this.buttonAllyIcon.IsVisible = true;
			this.buttonAllyIcon.Enabled = true;
			this.buttonAllyIcon.DefaultSpriteName = "ui_game_symbol_invite";
			return;
		case XUiC_PlayersListEntry.EnumAllyInviteStatus.Empty:
		{
			this.buttonAllyIcon.IsVisible = false;
			this.labelAllyIcon.IsVisible = true;
			bool flag2 = this.isOffline;
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06006BAE RID: 27566 RVA: 0x002C18B8 File Offset: 0x002BFAB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void updatePartyStatus()
	{
		if (GameStats.GetBool(EnumGameStats.AutoParty))
		{
			if (this.m_partyStatus == XUiC_PlayersListEntry.EnumPartyStatus.Offline)
			{
				this.buttonPartyIcon.IsVisible = false;
				this.buttonPartyIcon.DefaultSpriteName = "";
				this.buttonPartyIcon.Enabled = false;
				this.labelPartyIcon.IsVisible = true;
				return;
			}
			this.buttonPartyIcon.IsVisible = true;
			this.buttonPartyIcon.DefaultSpriteName = "ui_game_symbol_players";
			this.buttonPartyIcon.Enabled = false;
			this.labelPartyIcon.IsVisible = false;
			return;
		}
		else
		{
			switch (this.m_partyStatus)
			{
			case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_InParty:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "ui_game_symbol_players";
				this.buttonPartyIcon.Enabled = true;
				this.labelPartyIcon.IsVisible = false;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_InPartyAsLead:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "server_favorite";
				this.buttonPartyIcon.Enabled = true;
				this.labelPartyIcon.IsVisible = false;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_NoParty:
			case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoParty:
			case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_PartyFullAsLead:
			case XUiC_PlayersListEntry.EnumPartyStatus.Offline:
				this.buttonPartyIcon.IsVisible = false;
				this.buttonPartyIcon.DefaultSpriteName = "";
				this.buttonPartyIcon.Enabled = false;
				this.labelPartyIcon.IsVisible = true;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.LocalPlayer_Received:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "ui_game_symbol_invite";
				this.buttonPartyIcon.Enabled = true;
				this.labelPartyIcon.IsVisible = false;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InParty:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "ui_game_symbol_players";
				this.buttonPartyIcon.Enabled = true;
				this.labelPartyIcon.IsVisible = false;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InPartyIsLead:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "server_favorite";
				this.buttonPartyIcon.Enabled = false;
				this.labelPartyIcon.IsVisible = false;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_InPartyAsLead:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "ui_game_symbol_players";
				this.buttonPartyIcon.Enabled = true;
				this.labelPartyIcon.IsVisible = false;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_NoPartyAsLead:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "ui_game_symbol_add";
				this.buttonPartyIcon.Enabled = true;
				this.labelPartyIcon.IsVisible = false;
				return;
			case XUiC_PlayersListEntry.EnumPartyStatus.OtherPlayer_Sent:
				this.buttonPartyIcon.IsVisible = true;
				this.buttonPartyIcon.DefaultSpriteName = "ui_game_symbol_invite";
				this.buttonPartyIcon.Enabled = false;
				this.labelPartyIcon.IsVisible = false;
				return;
			default:
				return;
			}
		}
	}

	// Token: 0x06006BAF RID: 27567 RVA: 0x002C1B5C File Offset: 0x002BFD5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void onShowOnMapPress(XUiController _sender, int _mouseButton)
	{
		this.PlayersList.ShowOnMap(this.EntityId);
	}

	// Token: 0x06006BB0 RID: 27568 RVA: 0x002C1B70 File Offset: 0x002BFD70
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "disabled_color")
		{
			this.disabledColor = StringParsers.ParseColor32(value);
			return true;
		}
		if (!(name == "alternating_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		this.alternatingColor = StringParsers.ParseColor32(value);
		return true;
	}

	// Token: 0x06006BB1 RID: 27569 RVA: 0x002C1BBE File Offset: 0x002BFDBE
	[PublicizedFrom(EAccessModifier.Private)]
	public void textChatButtonOnPress(XUiController _sender, int _mouseButton)
	{
		this.blockButtonPressed(EBlockType.TextChat);
	}

	// Token: 0x06006BB2 RID: 27570 RVA: 0x002C1BC7 File Offset: 0x002BFDC7
	[PublicizedFrom(EAccessModifier.Private)]
	public void voiceChatButtonOnPress(XUiController _sender, int _mouseButton)
	{
		this.blockButtonPressed(EBlockType.VoiceChat);
	}

	// Token: 0x06006BB3 RID: 27571 RVA: 0x002C1BD0 File Offset: 0x002BFDD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void blockButtonPressed(EBlockType _blockType)
	{
		IPlatformUserBlockedData platformUserBlockedData = this.PlayerData.PlatformData.Blocked[_blockType];
		if (platformUserBlockedData.State == EUserBlockState.ByPlatform)
		{
			return;
		}
		platformUserBlockedData.Locally = !platformUserBlockedData.Locally;
	}

	// Token: 0x06006BB4 RID: 27572 RVA: 0x002C1C10 File Offset: 0x002BFE10
	public void playerBlockStateChanged(IPlatformUserData _pud, EBlockType _blockType, EUserBlockState _blockState)
	{
		if (this.PlayerData == null || !object.Equals(_pud.PrimaryId, this.PlayerData.PrimaryId))
		{
			return;
		}
		switch (_blockType)
		{
		case EBlockType.TextChat:
			this.updateBlockButton(_blockState, this.Chat, "xuiBlockChat");
			return;
		case EBlockType.VoiceChat:
			this.updateBlockButton(_blockState, this.Voice, "xuiBlockVoice");
			return;
		case EBlockType.Play:
			return;
		default:
			throw new ArgumentOutOfRangeException("_blockType", _blockType, string.Format("{0}.{1} missing implementation for {2}.{3}.", new object[]
			{
				"XUiC_PlayersListEntry",
				"playerBlockStateChanged",
				"EBlockType",
				_blockType
			}));
		}
	}

	// Token: 0x06006BB5 RID: 27573 RVA: 0x002C1CBC File Offset: 0x002BFEBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBlockButton(EUserBlockState _blockState, XUiV_Button _button, string _typeLocalizationKey)
	{
		_button.ManualColors = true;
		Color currentColor;
		switch (_blockState)
		{
		case EUserBlockState.NotBlocked:
			currentColor = Color.white;
			break;
		case EUserBlockState.InGame:
			currentColor = new Color(0.8f, 0.4f, 0f);
			break;
		case EUserBlockState.ByPlatform:
			currentColor = new Color(0.8f, 0f, 0f);
			break;
		default:
			throw new ArgumentOutOfRangeException("_blockState", _blockState, null);
		}
		_button.CurrentColor = currentColor;
		_button.Enabled = (_blockState != EUserBlockState.ByPlatform);
		string format = Localization.Get(_typeLocalizationKey, false);
		string key;
		switch (_blockState)
		{
		case EUserBlockState.NotBlocked:
			key = "xuiChatNotBlocked";
			break;
		case EUserBlockState.InGame:
			key = "xuiChatBlockedInGame";
			break;
		case EUserBlockState.ByPlatform:
			key = "xuiChatBlockedByPlatform";
			break;
		default:
			throw new ArgumentOutOfRangeException("_blockState", _blockState, null);
		}
		string arg = Localization.Get(key, false);
		_button.ToolTip = string.Format(format, arg);
	}

	// Token: 0x040051C9 RID: 20937
	public XUiC_PlayersList PlayersList;

	// Token: 0x040051CA RID: 20938
	public int EntityId;

	// Token: 0x040051CB RID: 20939
	public PersistentPlayerData PlayerData;

	// Token: 0x040051CC RID: 20940
	public XUiC_PlayerName PlayerName;

	// Token: 0x040051CD RID: 20941
	public XUiV_Label ZombieKillsText;

	// Token: 0x040051CE RID: 20942
	public XUiV_Label PlayerKillsText;

	// Token: 0x040051CF RID: 20943
	public XUiV_Label DeathsText;

	// Token: 0x040051D0 RID: 20944
	public XUiV_Label LevelText;

	// Token: 0x040051D1 RID: 20945
	public XUiV_Label PingText;

	// Token: 0x040051D2 RID: 20946
	public XUiV_Label GamestageText;

	// Token: 0x040051D3 RID: 20947
	public XUiV_Sprite AdminSprite;

	// Token: 0x040051D4 RID: 20948
	public XUiV_Sprite TwitchSprite;

	// Token: 0x040051D5 RID: 20949
	public XUiV_Sprite TwitchDisabledSprite;

	// Token: 0x040051D6 RID: 20950
	public XUiV_Label labelPartyIcon;

	// Token: 0x040051D7 RID: 20951
	public XUiV_Label labelAllyIcon;

	// Token: 0x040051D8 RID: 20952
	public XUiV_Label labelShowOnMap;

	// Token: 0x040051D9 RID: 20953
	public bool IsFriend;

	// Token: 0x040051DA RID: 20954
	public XUiV_Button buttonShowOnMap;

	// Token: 0x040051DB RID: 20955
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOffline;

	// Token: 0x040051DC RID: 20956
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PlayersListEntry.EnumAllyInviteStatus m_allyStatus;

	// Token: 0x040051DD RID: 20957
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PlayersListEntry.EnumPartyStatus m_partyStatus;

	// Token: 0x040051DE RID: 20958
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite rowBG;

	// Token: 0x040051DF RID: 20959
	public XUiV_Button Voice;

	// Token: 0x040051E0 RID: 20960
	public XUiV_Button Chat;

	// Token: 0x040051E1 RID: 20961
	public XUiV_Label DistanceToFriend;

	// Token: 0x040051E2 RID: 20962
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button buttonPartyIcon;

	// Token: 0x040051E3 RID: 20963
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button buttonAllyIcon;

	// Token: 0x040051E4 RID: 20964
	public XUiV_Button buttonReportPlayer;

	// Token: 0x040051E5 RID: 20965
	[PublicizedFrom(EAccessModifier.Private)]
	public Color enabledColor;

	// Token: 0x040051E6 RID: 20966
	[PublicizedFrom(EAccessModifier.Private)]
	public Color disabledColor;

	// Token: 0x040051E7 RID: 20967
	[PublicizedFrom(EAccessModifier.Private)]
	public Color alternatingColor;

	// Token: 0x040051E8 RID: 20968
	[PublicizedFrom(EAccessModifier.Private)]
	public static string sentKeyword = "";

	// Token: 0x040051E9 RID: 20969
	[PublicizedFrom(EAccessModifier.Private)]
	public static string receivedKeyword = "";

	// Token: 0x040051EA RID: 20970
	[PublicizedFrom(EAccessModifier.Private)]
	public static string naKeyword = "";

	// Token: 0x040051EB RID: 20971
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastTime;

	// Token: 0x02000D73 RID: 3443
	public enum EnumAllyInviteStatus
	{
		// Token: 0x040051ED RID: 20973
		LocalPlayer,
		// Token: 0x040051EE RID: 20974
		NA,
		// Token: 0x040051EF RID: 20975
		Friends,
		// Token: 0x040051F0 RID: 20976
		Sent,
		// Token: 0x040051F1 RID: 20977
		Received,
		// Token: 0x040051F2 RID: 20978
		Empty
	}

	// Token: 0x02000D74 RID: 3444
	public enum EnumTrackStatus
	{
		// Token: 0x040051F4 RID: 20980
		Hidden,
		// Token: 0x040051F5 RID: 20981
		NotTracked,
		// Token: 0x040051F6 RID: 20982
		Tracked
	}

	// Token: 0x02000D75 RID: 3445
	public enum EnumPartyStatus
	{
		// Token: 0x040051F8 RID: 20984
		LocalPlayer_InParty,
		// Token: 0x040051F9 RID: 20985
		LocalPlayer_InPartyAsLead,
		// Token: 0x040051FA RID: 20986
		LocalPlayer_NoParty,
		// Token: 0x040051FB RID: 20987
		LocalPlayer_Received,
		// Token: 0x040051FC RID: 20988
		OtherPlayer_InParty,
		// Token: 0x040051FD RID: 20989
		OtherPlayer_InPartyIsLead,
		// Token: 0x040051FE RID: 20990
		OtherPlayer_InPartyAsLead,
		// Token: 0x040051FF RID: 20991
		OtherPlayer_NoParty,
		// Token: 0x04005200 RID: 20992
		OtherPlayer_NoPartyAsLead,
		// Token: 0x04005201 RID: 20993
		OtherPlayer_PartyFullAsLead,
		// Token: 0x04005202 RID: 20994
		OtherPlayer_Sent,
		// Token: 0x04005203 RID: 20995
		Offline
	}
}
