using System;
using GUI_2;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D54 RID: 3412
[Preserve]
public class XUiC_OptionsMenu : XUiController
{
	// Token: 0x06006A86 RID: 27270 RVA: 0x002B5E4C File Offset: 0x002B404C
	public override void Init()
	{
		base.Init();
		UIOptions.OnOptionsVideoWindowChanged += this.OnVideoOptionsWindowChanged;
		XUiC_OptionsMenu.ID = base.WindowGroup.ID;
		(base.GetChildById("btnGeneral") as XUiC_SimpleButton).OnPressed += this.btnGeneral_OnPressed;
		(base.GetChildById("btnVideo") as XUiC_SimpleButton).OnPressed += this.btnVideo_OnPressed;
		(base.GetChildById("btnAudio") as XUiC_SimpleButton).OnPressed += this.btnAudio_OnPressed;
		(base.GetChildById("btnControls") as XUiC_SimpleButton).OnPressed += this.btnControls_OnPressed;
		(base.GetChildById("btnProfiles") as XUiC_SimpleButton).OnPressed += this.btnProfiles_OnPressed;
		(base.GetChildById("btnBlockList") as XUiC_SimpleButton).OnPressed += this.btnBlockList_OnPressed;
		(base.GetChildById("btnAccount") as XUiC_SimpleButton).OnPressed += this.btnAccount_OnPressed;
		(base.GetChildById("btnTwitch") as XUiC_SimpleButton).OnPressed += this.btnTwitch_OnPressed;
		(base.GetChildById("btnController") as XUiC_SimpleButton).OnPressed += this.btnController_OnPressed;
		XUiController[] childrenById = base.GetChildrenById("btnBack", null);
		for (int i = 0; i < childrenById.Length; i++)
		{
			((XUiC_SimpleButton)childrenById[i]).OnPressed += this.btnBack_OnPressed;
		}
	}

	// Token: 0x06006A87 RID: 27271 RVA: 0x002B5FDD File Offset: 0x002B41DD
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnVideo_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(this.GetOptionsVideoWindowName(UIOptions.OptionsVideoWindow));
	}

	// Token: 0x06006A88 RID: 27272 RVA: 0x002B5FF0 File Offset: 0x002B41F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVideoOptionsWindowChanged(OptionsVideoWindowMode _mode)
	{
		if (XUi.InGameMenuOpen && (base.xui.playerUI.windowManager.IsWindowOpen(XUiC_OptionsVideoSimplified.ID) || base.xui.playerUI.windowManager.IsWindowOpen(XUiC_OptionsVideo.ID)))
		{
			this.OpenOptions(this.GetOptionsVideoWindowName(_mode));
		}
	}

	// Token: 0x06006A89 RID: 27273 RVA: 0x002B6049 File Offset: 0x002B4249
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetOptionsVideoWindowName(OptionsVideoWindowMode _mode)
	{
		if (_mode == OptionsVideoWindowMode.Simplified)
		{
			return XUiC_OptionsVideoSimplified.ID;
		}
		if (_mode != OptionsVideoWindowMode.Detailed)
		{
			Log.Error(string.Format("Unknown video options menu {0}", _mode));
			return XUiC_OptionsVideo.ID;
		}
		return XUiC_OptionsVideo.ID;
	}

	// Token: 0x06006A8A RID: 27274 RVA: 0x002B607A File Offset: 0x002B427A
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnGeneral_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(XUiC_OptionsGeneral.ID);
	}

	// Token: 0x06006A8B RID: 27275 RVA: 0x002B6087 File Offset: 0x002B4287
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnAudio_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(XUiC_OptionsAudio.ID);
	}

	// Token: 0x06006A8C RID: 27276 RVA: 0x002B6094 File Offset: 0x002B4294
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnControls_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(XUiC_OptionsControls.ID);
	}

	// Token: 0x06006A8D RID: 27277 RVA: 0x002B60A4 File Offset: 0x002B42A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnProfiles_OnPressed(XUiController _sender, int _mouseButton)
	{
		int v = EntityClass.FromString("playerMale");
		EntityClass entityClass = EntityClass.list[v];
		this.OpenOptions(XUiC_OptionsProfiles.ID);
	}

	// Token: 0x06006A8E RID: 27278 RVA: 0x002B60D3 File Offset: 0x002B42D3
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnAccount_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(XUiC_OptionsUsername.ID);
	}

	// Token: 0x06006A8F RID: 27279 RVA: 0x002B60E0 File Offset: 0x002B42E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTwitch_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(XUiC_OptionsTwitch.ID);
	}

	// Token: 0x06006A90 RID: 27280 RVA: 0x002B60ED File Offset: 0x002B42ED
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnController_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(XUiC_OptionsController.ID);
	}

	// Token: 0x06006A91 RID: 27281 RVA: 0x002B60FA File Offset: 0x002B42FA
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnBlockList_OnPressed(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions(XUiC_OptionsBlockedPlayersList.ID);
	}

	// Token: 0x06006A92 RID: 27282 RVA: 0x002B6108 File Offset: 0x002B4308
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		if (GameStats.GetInt(EnumGameStats.GameState) == 0)
		{
			base.xui.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
		}
		XUi.InGameMenuOpen = false;
	}

	// Token: 0x06006A93 RID: 27283 RVA: 0x002B6160 File Offset: 0x002B4360
	[PublicizedFrom(EAccessModifier.Private)]
	public void OpenOptions(string _optionsWindowName)
	{
		this.continueGamePause = true;
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(_optionsWindowName, true, false, true);
		XUi.InGameMenuOpen = true;
	}

	// Token: 0x06006A94 RID: 27284 RVA: 0x002B61B4 File Offset: 0x002B43B4
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
		this.continueGamePause = false;
		this.windowGroup.openWindowOnEsc = ((GameStats.GetInt(EnumGameStats.GameState) == 0) ? XUiC_MainMenu.ID : null);
		base.RefreshBindings(false);
		XUi.InGameMenuOpen = true;
		base.xui.playerUI.windowManager.OpenIfNotOpen("CalloutGroup", false, false, true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonSouth, "igcoSelect", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonEast, "igcoBack", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.SetCalloutsEnabled(XUiC_GamepadCalloutWindow.CalloutType.Menu, true);
	}

	// Token: 0x06006A95 RID: 27285 RVA: 0x002B626B File Offset: 0x002B446B
	public override void OnClose()
	{
		base.OnClose();
		if (!this.continueGamePause && GameStats.GetInt(EnumGameStats.GameState) == 2)
		{
			GameManager.Instance.Pause(false);
		}
		XUi.InGameMenuOpen = false;
	}

	// Token: 0x06006A96 RID: 27286 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006A97 RID: 27287 RVA: 0x002B6298 File Offset: 0x002B4498
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "ingame")
		{
			_value = (GameStats.GetInt(EnumGameStats.GameState) != 0).ToString();
			return true;
		}
		if (_bindingName == "notingame")
		{
			_value = (GameStats.GetInt(EnumGameStats.GameState) == 0).ToString();
			return true;
		}
		if (_bindingName == "notreleaseingame")
		{
			_value = "false";
			return true;
		}
		if (_bindingName == "ingamenoteditor")
		{
			if (GameStats.GetInt(EnumGameStats.GameState) != 0)
			{
				_value = (!GameManager.Instance.World.IsEditor()).ToString();
			}
			else
			{
				_value = "false";
			}
			return true;
		}
		if (!(_bindingName == "showblocklist"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = (BlockedPlayerList.Instance != null).ToString();
		return true;
	}

	// Token: 0x04005058 RID: 20568
	public static string ID = "";

	// Token: 0x04005059 RID: 20569
	[PublicizedFrom(EAccessModifier.Private)]
	public bool continueGamePause;
}
