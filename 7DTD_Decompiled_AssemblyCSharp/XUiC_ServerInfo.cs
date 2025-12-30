using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E14 RID: 3604
[Preserve]
public class XUiC_ServerInfo : XUiController
{
	// Token: 0x060070DD RID: 28893 RVA: 0x002E04B4 File Offset: 0x002DE6B4
	public override void Init()
	{
		base.Init();
		foreach (XUiC_ServerBrowserGamePrefInfo xuiC_ServerBrowserGamePrefInfo in base.GetChildrenByType<XUiC_ServerBrowserGamePrefInfo>(null))
		{
			this.infoFields.Add(xuiC_ServerBrowserGamePrefInfo);
			if (xuiC_ServerBrowserGamePrefInfo.ValueType == GamePrefs.EnumType.Int)
			{
				GameInfoInt gameInfoInt = xuiC_ServerBrowserGamePrefInfo.GameInfoInt;
				if (gameInfoInt != GameInfoInt.CurrentServerTime)
				{
					if (gameInfoInt == GameInfoInt.AirDropFrequency)
					{
						xuiC_ServerBrowserGamePrefInfo.CustomIntValueFormatter = delegate(GameServerInfo _info, int _value)
						{
							string str;
							if (_value % 24 == 0)
							{
								str = "goAirDropValue";
								_value /= 24;
							}
							else
							{
								str = "goAirDropValueHour";
							}
							return string.Format(Localization.Get(str + ((_value == 1) ? "" : "s"), false), _value);
						};
					}
				}
				else
				{
					xuiC_ServerBrowserGamePrefInfo.CustomIntValueFormatter = ((GameServerInfo _info, int _worldTime) => ValueDisplayFormatters.WorldTime((ulong)((long)_worldTime), Localization.Get("xuiDayTimeLong", false)));
				}
			}
			else if (xuiC_ServerBrowserGamePrefInfo.ValueType == GamePrefs.EnumType.String && xuiC_ServerBrowserGamePrefInfo.GameInfoString == GameInfoString.ServerVersion)
			{
				xuiC_ServerBrowserGamePrefInfo.CustomStringValueFormatter = delegate(GameServerInfo _info, string _s)
				{
					if (_info.Version.Build != 0)
					{
						return (_info.IsCompatibleVersion ? "" : "[ff0000]") + _info.Version.LongString;
					}
					return "[ff0000]" + _s;
				};
			}
		}
		XUiController childById = base.GetChildById("ServerDescription");
		this.lblServerDescription = (XUiV_Label)childById.ViewComponent;
		XUiController childById2 = base.GetChildById("ServerWebsiteURL");
		this.lblServerUrl = (XUiV_Label)childById2.ViewComponent;
		childById2.OnHover += this.UrlController_OnHover;
		childById2.OnPress += this.UrlController_OnPress;
	}

	// Token: 0x060070DE RID: 28894 RVA: 0x002E0606 File Offset: 0x002DE806
	public override void OnOpen()
	{
		base.OnOpen();
		this.lblServerUrl.IsNavigatable = !string.IsNullOrEmpty(this.lblServerUrl.Text);
	}

	// Token: 0x060070DF RID: 28895 RVA: 0x002E062C File Offset: 0x002DE82C
	public void InitializeForListFilter(XUiC_ServersList.EnumServerLists mode)
	{
		if (mode == XUiC_ServersList.EnumServerLists.Peer || mode == XUiC_ServersList.EnumServerLists.Friends || mode == XUiC_ServersList.EnumServerLists.History)
		{
			this.SetDisplayMode(XUiC_ServerInfo.DisplayMode.Peer);
			return;
		}
		this.SetDisplayMode(XUiC_ServerInfo.DisplayMode.Dedicated);
	}

	// Token: 0x060070E0 RID: 28896 RVA: 0x002E0649 File Offset: 0x002DE849
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetDisplayMode(XUiC_ServerInfo.DisplayMode _mode)
	{
		if (this.displayMode != _mode)
		{
			this.displayMode = _mode;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060070E1 RID: 28897 RVA: 0x002E0664 File Offset: 0x002DE864
	public void SetServerInfo(GameServerInfo _gameInfo)
	{
		foreach (XUiC_ServerBrowserGamePrefInfo xuiC_ServerBrowserGamePrefInfo in this.infoFields)
		{
			xuiC_ServerBrowserGamePrefInfo.SetCurrentValue(_gameInfo);
		}
		if (_gameInfo == null)
		{
			this.lblServerDescription.Text = "";
			this.lblServerUrl.Text = "";
			this.unfilteredUrlText = "";
			this.lblServerUrl.IsNavigatable = false;
			return;
		}
		AuthoredText authoredText = _gameInfo.ServerDescription;
		if (_gameInfo.IsDedicated || _gameInfo.IsLAN)
		{
			this.SetDisplayMode(XUiC_ServerInfo.DisplayMode.Dedicated);
		}
		else
		{
			this.SetDisplayMode(XUiC_ServerInfo.DisplayMode.Peer);
			if (string.IsNullOrEmpty(authoredText.Text))
			{
				authoredText = _gameInfo.ServerDisplayName;
			}
		}
		this.unfilteredUrlText = _gameInfo.ServerURL.Text;
		GeneratedTextManager.GetDisplayText(authoredText, delegate(string desc)
		{
			this.lblServerDescription.Text = desc;
		}, true, false, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.Supported);
		GeneratedTextManager.GetDisplayText(_gameInfo.ServerURL, delegate(string url)
		{
			this.lblServerUrl.Text = url;
		}, true, false, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
		this.lblServerUrl.IsNavigatable = !string.IsNullOrEmpty(_gameInfo.ServerURL.Text);
	}

	// Token: 0x060070E2 RID: 28898 RVA: 0x002E078C File Offset: 0x002DE98C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UrlController_OnHover(XUiController _sender, bool _isOver)
	{
		this.lblServerUrl.Color = (_isOver ? new Color32(250, byte.MaxValue, 163, byte.MaxValue) : Color.white);
	}

	// Token: 0x060070E3 RID: 28899 RVA: 0x002E07C1 File Offset: 0x002DE9C1
	[PublicizedFrom(EAccessModifier.Private)]
	public void UrlController_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_MessageBoxWindowGroup.ShowUrlConfirmationDialog(base.xui, this.unfilteredUrlText, false, null, null, null, this.lblServerUrl.Text);
	}

	// Token: 0x060070E4 RID: 28900 RVA: 0x002E07E4 File Offset: 0x002DE9E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "showip")
		{
			XUiC_ServerInfo.DisplayMode displayMode = this.displayMode;
			if (displayMode != XUiC_ServerInfo.DisplayMode.Dedicated)
			{
				if (displayMode == XUiC_ServerInfo.DisplayMode.Peer)
				{
					_value = false.ToString();
				}
			}
			else
			{
				_value = true.ToString();
			}
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x040055C3 RID: 21955
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_ServerBrowserGamePrefInfo> infoFields = new List<XUiC_ServerBrowserGamePrefInfo>();

	// Token: 0x040055C4 RID: 21956
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblServerDescription;

	// Token: 0x040055C5 RID: 21957
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblServerUrl;

	// Token: 0x040055C6 RID: 21958
	[PublicizedFrom(EAccessModifier.Private)]
	public string unfilteredUrlText;

	// Token: 0x040055C7 RID: 21959
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerInfo.DisplayMode displayMode;

	// Token: 0x02000E15 RID: 3605
	[PublicizedFrom(EAccessModifier.Private)]
	public enum DisplayMode
	{
		// Token: 0x040055C9 RID: 21961
		Dedicated,
		// Token: 0x040055CA RID: 21962
		Peer
	}
}
