using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D6F RID: 3439
[Preserve]
public class XUiC_PlayersBlockedListEntry : XUiController
{
	// Token: 0x17000ACB RID: 2763
	// (get) Token: 0x06006B7D RID: 27517 RVA: 0x002BF61B File Offset: 0x002BD81B
	// (set) Token: 0x06006B7E RID: 27518 RVA: 0x002BF623 File Offset: 0x002BD823
	public PlatformUserIdentifierAbs PlayerId { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000ACC RID: 2764
	// (set) Token: 0x06006B7F RID: 27519 RVA: 0x002BF62C File Offset: 0x002BD82C
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

	// Token: 0x06006B80 RID: 27520 RVA: 0x002BF644 File Offset: 0x002BD844
	public override void Init()
	{
		base.Init();
		this.rowBG = (XUiV_Sprite)base.GetChildById("background").ViewComponent;
		this.PlayerName = (XUiC_PlayerName)base.GetChildById("playerName");
		this.btnReportPlayer = (XUiV_Button)base.GetChildById("btnReportPlayer").ViewComponent;
		this.btnReportPlayer.Controller.OnPress += this.ReportPlayerPressed;
		this.btnUnblockPlayer = (XUiV_Button)base.GetChildById("unblockBtn").ViewComponent;
		this.btnUnblockPlayer.Controller.OnPress += this.UnblockPlayerPressed;
	}

	// Token: 0x06006B81 RID: 27521 RVA: 0x002BF6F8 File Offset: 0x002BD8F8
	public void UpdateEntry(PlatformUserIdentifierAbs _playerId)
	{
		BlockedPlayerList.ListEntry playerStateInfo = BlockedPlayerList.Instance.GetPlayerStateInfo(_playerId);
		if (playerStateInfo != null && playerStateInfo.Blocked)
		{
			this.PlayerId = _playerId;
			this.PlayerName.UpdatePlayerData(playerStateInfo.PlayerData, true, null);
			this.btnReportPlayer.IsVisible = true;
			this.btnUnblockPlayer.IsVisible = true;
			return;
		}
		this.Clear();
	}

	// Token: 0x06006B82 RID: 27522 RVA: 0x002BF755 File Offset: 0x002BD955
	public void Clear()
	{
		this.PlayerId = null;
		this.PlayerName.ClearPlayerData();
		this.btnReportPlayer.IsVisible = false;
		this.btnUnblockPlayer.IsVisible = false;
	}

	// Token: 0x06006B83 RID: 27523 RVA: 0x002BF784 File Offset: 0x002BD984
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

	// Token: 0x06006B84 RID: 27524 RVA: 0x002BF7D8 File Offset: 0x002BD9D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UnblockPlayerPressed(XUiController _sender, int _mouseButton)
	{
		if (this.PlayerId == null)
		{
			return;
		}
		BlockedPlayerList.ListEntry listEntry = BlockedPlayerList.Instance.GetPlayerStateInfo(this.PlayerId) ?? null;
		if (listEntry != null && listEntry.ResolvedOnce)
		{
			listEntry.SetBlockState(false);
			this.BlockList.IsDirty = true;
		}
	}

	// Token: 0x06006B85 RID: 27525 RVA: 0x002BF824 File Offset: 0x002BDA24
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

	// Token: 0x040051AE RID: 20910
	public XUiC_BlockedPlayersList BlockList;

	// Token: 0x040051AF RID: 20911
	public XUiC_PlayerName PlayerName;

	// Token: 0x040051B0 RID: 20912
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnReportPlayer;

	// Token: 0x040051B1 RID: 20913
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnUnblockPlayer;

	// Token: 0x040051B2 RID: 20914
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite rowBG;

	// Token: 0x040051B3 RID: 20915
	[PublicizedFrom(EAccessModifier.Private)]
	public Color enabledColor = Color.white;

	// Token: 0x040051B4 RID: 20916
	[PublicizedFrom(EAccessModifier.Private)]
	public Color disabledColor;

	// Token: 0x040051B5 RID: 20917
	[PublicizedFrom(EAccessModifier.Private)]
	public Color alternatingColor;
}
