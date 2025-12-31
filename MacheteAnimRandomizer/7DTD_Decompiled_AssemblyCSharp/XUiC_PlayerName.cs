using System;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D6E RID: 3438
[Preserve]
public class XUiC_PlayerName : XUiController
{
	// Token: 0x17000ACA RID: 2762
	// (get) Token: 0x06006B71 RID: 27505 RVA: 0x002BF26A File Offset: 0x002BD46A
	// (set) Token: 0x06006B72 RID: 27506 RVA: 0x002BF277 File Offset: 0x002BD477
	public Color Color
	{
		get
		{
			return this.lblName.Color;
		}
		set
		{
			this.lblName.Color = value;
			this.lblNameCrossplay.Color = value;
		}
	}

	// Token: 0x06006B73 RID: 27507 RVA: 0x002BF294 File Offset: 0x002BD494
	public override void Init()
	{
		base.Init();
		this.rect = (XUiV_Rect)base.GetChildById("playerName").ViewComponent;
		this.lblName = (XUiV_Label)base.GetChildById("name").ViewComponent;
		this.lblNameCrossplay = (XUiV_Label)base.GetChildById("nameCrossplay").ViewComponent;
		this.sprIconCrossplay = (XUiV_Sprite)base.GetChildById("iconCrossplay").ViewComponent;
		this.rect.IsNavigatable = false;
		this.rect.IsSnappable = false;
		base.OnPress += this.PlayerName_OnPress;
	}

	// Token: 0x06006B74 RID: 27508 RVA: 0x002BF33D File Offset: 0x002BD53D
	public override void Cleanup()
	{
		base.Cleanup();
		base.OnPress -= this.PlayerName_OnPress;
	}

	// Token: 0x06006B75 RID: 27509 RVA: 0x002BF357 File Offset: 0x002BD557
	public void SetGenericName(string _name)
	{
		this.UpdatePlayerData(null, false, _name);
	}

	// Token: 0x06006B76 RID: 27510 RVA: 0x002BF364 File Offset: 0x002BD564
	public void UpdatePlayerData(PlayerData _playerData, bool _showCrossplay, string _displayName = null)
	{
		this.PlayerData = _playerData;
		bool flag = false;
		if (_displayName != null)
		{
			XUiV_Label xuiV_Label = this.lblName;
			this.lblNameCrossplay.Text = _displayName;
			xuiV_Label.Text = _displayName;
			flag = true;
		}
		else if (this.PlayerData != null)
		{
			GeneratedTextManager.GetDisplayText(this.PlayerData.PlayerName, delegate(string name)
			{
				XUiV_Label xuiV_Label2 = this.lblName;
				this.lblNameCrossplay.Text = name;
				xuiV_Label2.Text = name;
			}, true, false, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
			flag = true;
		}
		this.rect.EventOnPress = (flag && this.CanShowProfile());
		this.rect.IsSnappable = flag;
		this.rect.IsNavigatable = flag;
		if (_showCrossplay && this.PlayerData != null && this.PlayerData.PlayGroup != EPlayGroup.Unknown)
		{
			this.sprIconCrossplay.SpriteName = PlatformManager.NativePlatform.Utils.GetCrossplayPlayerIcon(this.PlayerData.PlayGroup, true, this.PlayerData.NativeId.PlatformIdentifier);
			this.sprIconCrossplay.UIAtlas = "SymbolAtlas";
			this.sprIconCrossplay.IsVisible = true;
			this.lblName.IsVisible = false;
			this.lblNameCrossplay.IsVisible = true;
		}
		else
		{
			this.sprIconCrossplay.IsVisible = false;
			this.lblName.IsVisible = true;
			this.lblNameCrossplay.IsVisible = false;
		}
		base.RefreshBindings(false);
	}

	// Token: 0x06006B77 RID: 27511 RVA: 0x002BF4A6 File Offset: 0x002BD6A6
	public void ClearPlayerData()
	{
		this.PlayerData = null;
		this.lblName.Text = string.Empty;
		this.lblNameCrossplay.Text = string.Empty;
		this.sprIconCrossplay.IsVisible = false;
	}

	// Token: 0x06006B78 RID: 27512 RVA: 0x002BF4DC File Offset: 0x002BD6DC
	public bool CanShowProfile()
	{
		return this.PlayerData != null && ((this.PlayerData.NativeId != null && PlatformManager.MultiPlatform.User.CanShowProfile(this.PlayerData.NativeId)) || (this.PlayerData.PrimaryId != null && PlatformManager.MultiPlatform.User.CanShowProfile(this.PlayerData.PrimaryId)));
	}

	// Token: 0x06006B79 RID: 27513 RVA: 0x002BF54C File Offset: 0x002BD74C
	public void ShowProfile()
	{
		if (this.PlayerData == null)
		{
			return;
		}
		if (this.PlayerData.NativeId != null && PlatformManager.MultiPlatform.User.CanShowProfile(this.PlayerData.NativeId))
		{
			PlatformManager.MultiPlatform.User.ShowProfile(this.PlayerData.NativeId);
			return;
		}
		if (this.PlayerData.PrimaryId != null && PlatformManager.MultiPlatform.User.CanShowProfile(this.PlayerData.PrimaryId))
		{
			PlatformManager.MultiPlatform.User.ShowProfile(this.PlayerData.PrimaryId);
		}
	}

	// Token: 0x06006B7A RID: 27514 RVA: 0x002BF5E9 File Offset: 0x002BD7E9
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayerName_OnPress(XUiController _sender, int _mousebutton)
	{
		this.ShowProfile();
	}

	// Token: 0x040051A8 RID: 20904
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Rect rect;

	// Token: 0x040051A9 RID: 20905
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblName;

	// Token: 0x040051AA RID: 20906
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblNameCrossplay;

	// Token: 0x040051AB RID: 20907
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite sprIconCrossplay;

	// Token: 0x040051AC RID: 20908
	public PlayerData PlayerData;
}
