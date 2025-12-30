using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D76 RID: 3446
[Preserve]
public class XUiC_PlayersRecentListEntry : XUiController
{
	// Token: 0x17000AD5 RID: 2773
	// (get) Token: 0x06006BB8 RID: 27576 RVA: 0x002C1DC1 File Offset: 0x002BFFC1
	// (set) Token: 0x06006BB9 RID: 27577 RVA: 0x002C1DC9 File Offset: 0x002BFFC9
	public PlatformUserIdentifierAbs PlayerId { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000AD6 RID: 2774
	// (set) Token: 0x06006BBA RID: 27578 RVA: 0x002C1DD2 File Offset: 0x002BFFD2
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

	// Token: 0x06006BBB RID: 27579 RVA: 0x002C1DE8 File Offset: 0x002BFFE8
	public override void Init()
	{
		base.Init();
		this.rowBG = (XUiV_Sprite)base.GetChildById("background").ViewComponent;
		this.PlayerName = (XUiC_PlayerName)base.GetChildById("playerName");
		this.lblLastSeenTime = (XUiV_Label)base.GetChildById("lastSeen").ViewComponent;
		this.btnReportPlayer = (XUiV_Button)base.GetChildById("btnReportPlayer").ViewComponent;
		this.btnReportPlayer.Controller.OnPress += this.ReportPlayerPressed;
		this.btnBlockPlayer = (XUiV_Button)base.GetChildById("blockBtn").ViewComponent;
		this.btnBlockPlayer.Controller.OnPress += this.BlockPlayerPressed;
		this.btnViewProfile = (XUiV_Button)base.GetChildById("btnViewProfile").ViewComponent;
		this.btnViewProfile.Controller.OnPress += this.ViewProfilePressed;
	}

	// Token: 0x06006BBC RID: 27580 RVA: 0x002C1EEC File Offset: 0x002C00EC
	public void UpdateEntry(PlatformUserIdentifierAbs _playerId)
	{
		BlockedPlayerList.ListEntry playerStateInfo = BlockedPlayerList.Instance.GetPlayerStateInfo(_playerId);
		if (playerStateInfo == null || playerStateInfo.Blocked)
		{
			this.Clear();
			return;
		}
		this.PlayerId = _playerId;
		this.PlayerName.UpdatePlayerData(playerStateInfo.PlayerData, true, null);
		this.lblLastSeenTime.Text = Utils.DescribeTimeSince(DateTime.UtcNow, playerStateInfo.LastSeen);
		this.btnReportPlayer.IsVisible = true;
		if (this.PlayerName.CanShowProfile())
		{
			this.btnBlockPlayer.IsVisible = false;
			this.btnViewProfile.IsVisible = true;
			return;
		}
		this.btnBlockPlayer.IsVisible = true;
		this.btnViewProfile.IsVisible = false;
	}

	// Token: 0x06006BBD RID: 27581 RVA: 0x002C1F9C File Offset: 0x002C019C
	public void Clear()
	{
		this.PlayerId = null;
		this.PlayerName.ClearPlayerData();
		this.lblLastSeenTime.Text = "";
		this.btnReportPlayer.IsVisible = false;
		this.btnBlockPlayer.IsVisible = false;
		this.btnViewProfile.IsVisible = false;
	}

	// Token: 0x06006BBE RID: 27582 RVA: 0x002C1FF0 File Offset: 0x002C01F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReportPlayerPressed(XUiController _sender, int _mouseButton)
	{
		if (PlatformManager.MultiPlatform.PlayerReporting != null && this.PlayerId != null)
		{
			BlockedPlayerList.ListEntry playerStateInfo = BlockedPlayerList.Instance.GetPlayerStateInfo(this.PlayerId);
			bool flag = GameStats.GetInt(EnumGameStats.GameState) != 0;
			XUiC_ReportPlayer.Open(playerStateInfo.PlayerData, flag ? "" : XUiC_OptionsBlockedPlayersList.ID);
		}
	}

	// Token: 0x06006BBF RID: 27583 RVA: 0x002C2044 File Offset: 0x002C0244
	[PublicizedFrom(EAccessModifier.Private)]
	public void BlockPlayerPressed(XUiController _sender, int _mouseButton)
	{
		if (this.PlayerId == null || this.PlayerName.CanShowProfile())
		{
			return;
		}
		BlockedPlayerList.ListEntry playerStateInfo = BlockedPlayerList.Instance.GetPlayerStateInfo(this.PlayerId);
		if (playerStateInfo != null && playerStateInfo.ResolvedOnce)
		{
			ValueTuple<bool, string> valueTuple = playerStateInfo.SetBlockState(true);
			bool item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			if (item)
			{
				this.BlockList.IsDirty = true;
				return;
			}
			if (!string.IsNullOrEmpty(item2))
			{
				this.BlockList.DisplayMessage(Localization.Get("xuiBlockedPlayersCantAddHeader", false), item2);
			}
		}
	}

	// Token: 0x06006BC0 RID: 27584 RVA: 0x002C20C6 File Offset: 0x002C02C6
	[PublicizedFrom(EAccessModifier.Private)]
	public void ViewProfilePressed(XUiController _sender, int _mouseButton)
	{
		if (this.PlayerName.CanShowProfile())
		{
			this.PlayerName.ShowProfile();
			return;
		}
	}

	// Token: 0x06006BC1 RID: 27585 RVA: 0x002C20E4 File Offset: 0x002C02E4
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "enabled_color")
		{
			this.enabledColor = StringParsers.ParseColor32(value);
			return true;
		}
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

	// Token: 0x04005205 RID: 20997
	public XUiC_BlockedPlayersList BlockList;

	// Token: 0x04005206 RID: 20998
	public XUiC_PlayerName PlayerName;

	// Token: 0x04005207 RID: 20999
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblLastSeenTime;

	// Token: 0x04005208 RID: 21000
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnReportPlayer;

	// Token: 0x04005209 RID: 21001
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnBlockPlayer;

	// Token: 0x0400520A RID: 21002
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnViewProfile;

	// Token: 0x0400520B RID: 21003
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite rowBG;

	// Token: 0x0400520C RID: 21004
	[PublicizedFrom(EAccessModifier.Private)]
	public Color enabledColor = Color.white;

	// Token: 0x0400520D RID: 21005
	[PublicizedFrom(EAccessModifier.Private)]
	public Color disabledColor;

	// Token: 0x0400520E RID: 21006
	[PublicizedFrom(EAccessModifier.Private)]
	public Color alternatingColor;
}
