using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000E12 RID: 3602
[Preserve]
public class XUiC_ServerFilters : XUiController
{
	// Token: 0x140000BE RID: 190
	// (add) Token: 0x060070C3 RID: 28867 RVA: 0x002DFD48 File Offset: 0x002DDF48
	// (remove) Token: 0x060070C4 RID: 28868 RVA: 0x002DFD80 File Offset: 0x002DDF80
	public event Action<IServerBrowserFilterControl> OnFilterChanged;

	// Token: 0x060070C5 RID: 28869 RVA: 0x002DFDB8 File Offset: 0x002DDFB8
	public override void Init()
	{
		base.Init();
		base.GetChildById("outclick").OnPress += this.CloseFiltersButton_OnPress;
		base.GetChildById("btnCloseFilters").OnPress += this.CloseFiltersButton_OnPress;
		((XUiC_SimpleButton)base.GetChildById("btnResetFilters")).OnPressed += this.ResetFiltersButton_OnPress;
		((XUiC_SimpleButton)base.GetChildById("btnBack")).OnPressed += this.BackButton_OnPress;
		this.btnStartSearch = (XUiC_SimpleButton)base.GetChildById("btnStartSearch");
		this.btnStartSearch.OnPressed += this.StartSearchButton_OnPress;
		this.btnStartSearch.Text = "[action:gui:GUI Apply] " + Localization.Get("xuiServerStartSearch", false).ToUpper();
		foreach (XUiC_ServerBrowserGamePrefSelectorCombo xuiC_ServerBrowserGamePrefSelectorCombo in base.GetChildrenByType<XUiC_ServerBrowserGamePrefSelectorCombo>(null))
		{
			GameInfoInt gameInfoInt = xuiC_ServerBrowserGamePrefSelectorCombo.GameInfoInt;
			if (gameInfoInt != GameInfoInt.CurrentServerTime)
			{
				if (gameInfoInt != GameInfoInt.AirDropFrequency)
				{
					if (gameInfoInt == GameInfoInt.WorldSize)
					{
						if (PlatformOptimizations.EnforceMaxWorldSizeClient)
						{
							xuiC_ServerBrowserGamePrefSelectorCombo.ValueRangeMin = new int?(0);
							xuiC_ServerBrowserGamePrefSelectorCombo.ValueRangeMax = new int?(PlatformOptimizations.MaxWorldSizeClient);
						}
					}
				}
				else
				{
					xuiC_ServerBrowserGamePrefSelectorCombo.ValuePreDisplayModifierFunc = ((int _n) => _n / 24);
				}
			}
			else
			{
				xuiC_ServerBrowserGamePrefSelectorCombo.CustomValuePreFilterModifierFunc = ((int _n) => (_n - 1) * 24000);
			}
			if (xuiC_ServerBrowserGamePrefSelectorCombo.GameInfoString == GameInfoString.Region)
			{
				this.regionFilter = xuiC_ServerBrowserGamePrefSelectorCombo;
			}
			GameInfoBool gameInfoBool = xuiC_ServerBrowserGamePrefSelectorCombo.GameInfoBool;
			switch (gameInfoBool)
			{
			case GameInfoBool.EACEnabled:
				this.eacFilter = xuiC_ServerBrowserGamePrefSelectorCombo;
				break;
			case GameInfoBool.SanctionsIgnored:
				this.ignoreSanctionsFilter = xuiC_ServerBrowserGamePrefSelectorCombo;
				break;
			case GameInfoBool.Architecture64:
			case GameInfoBool.StockSettings:
			case GameInfoBool.StockFiles:
				break;
			case GameInfoBool.ModdedConfig:
				this.moddedConfigsFilter = xuiC_ServerBrowserGamePrefSelectorCombo;
				break;
			case GameInfoBool.RequiresMod:
				this.requiresModsFilter = xuiC_ServerBrowserGamePrefSelectorCombo;
				break;
			default:
				if (gameInfoBool == GameInfoBool.AllowCrossplay)
				{
					this.crossplayFilter = xuiC_ServerBrowserGamePrefSelectorCombo;
				}
				break;
			}
			XUiC_ServerBrowserGamePrefSelectorCombo xuiC_ServerBrowserGamePrefSelectorCombo2 = xuiC_ServerBrowserGamePrefSelectorCombo;
			xuiC_ServerBrowserGamePrefSelectorCombo2.OnValueChanged = (Action<IServerBrowserFilterControl>)Delegate.Combine(xuiC_ServerBrowserGamePrefSelectorCombo2.OnValueChanged, new Action<IServerBrowserFilterControl>(this.OnFilterValueChanged));
			this.allFilterControls.Add(xuiC_ServerBrowserGamePrefSelectorCombo);
		}
		foreach (XUiC_ServerBrowserGamePrefString xuiC_ServerBrowserGamePrefString in base.GetChildrenByType<XUiC_ServerBrowserGamePrefString>(null))
		{
			if (xuiC_ServerBrowserGamePrefString.GameInfoString == GameInfoString.Language)
			{
				this.languageFilter = xuiC_ServerBrowserGamePrefString;
			}
			XUiC_ServerBrowserGamePrefString xuiC_ServerBrowserGamePrefString2 = xuiC_ServerBrowserGamePrefString;
			xuiC_ServerBrowserGamePrefString2.OnValueChanged = (Action<IServerBrowserFilterControl>)Delegate.Combine(xuiC_ServerBrowserGamePrefString2.OnValueChanged, new Action<IServerBrowserFilterControl>(this.OnFilterValueChanged));
			this.allFilterControls.Add(xuiC_ServerBrowserGamePrefString);
		}
	}

	// Token: 0x060070C6 RID: 28870 RVA: 0x002DED21 File Offset: 0x002DCF21
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.IsDirty = false;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060070C7 RID: 28871 RVA: 0x002E0040 File Offset: 0x002DE240
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "crossplayTooltip")
		{
			_value = (PermissionsManager.GetPermissionDenyReason(EUserPerms.Crossplay, PermissionsManager.PermissionSources.All) ?? Localization.Get("xuiOptionsGeneralCrossplayTooltip", false));
			return true;
		}
		if (!(_bindingName == "results"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.currentResults.ToString();
		return true;
	}

	// Token: 0x060070C8 RID: 28872 RVA: 0x002E009B File Offset: 0x002DE29B
	[PublicizedFrom(EAccessModifier.Private)]
	public void CloseFiltersButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.closeFilters();
	}

	// Token: 0x060070C9 RID: 28873 RVA: 0x002E00A3 File Offset: 0x002DE2A3
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetFiltersButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.ResetFilters();
	}

	// Token: 0x060070CA RID: 28874 RVA: 0x0027DCF9 File Offset: 0x0027BEF9
	[PublicizedFrom(EAccessModifier.Private)]
	public void BackButton_OnPress(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_MainMenu.ID, true, false, true);
	}

	// Token: 0x060070CB RID: 28875 RVA: 0x002E00AC File Offset: 0x002DE2AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartSearchButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.closeFilters();
		List<IServerListInterface.ServerFilter> list = new List<IServerListInterface.ServerFilter>();
		foreach (IServerBrowserFilterControl serverBrowserFilterControl in this.allFilterControls)
		{
			XUiC_ServersList.UiServerFilter filter = serverBrowserFilterControl.GetFilter();
			if (filter.Type != IServerListInterface.ServerFilter.EServerFilterType.Any)
			{
				list.Add(filter);
			}
		}
		ServerListManager.Instance.StartSearch(list);
	}

	// Token: 0x060070CC RID: 28876 RVA: 0x002E0124 File Offset: 0x002DE324
	public void StartShortcutPressed()
	{
		if (this.btnStartSearch.Enabled && this.btnStartSearch.IsVisible)
		{
			this.StartSearchButton_OnPress(null, 0);
			return;
		}
		this.CloseFiltersButton_OnPress(null, 0);
	}

	// Token: 0x060070CD RID: 28877 RVA: 0x002E0154 File Offset: 0x002DE354
	public void ResetFilters()
	{
		foreach (IServerBrowserFilterControl serverBrowserFilterControl in this.allFilterControls)
		{
			serverBrowserFilterControl.Reset();
		}
		if (this.regionFilter != null)
		{
			string stringValue = GamePrefs.GetString(EnumGamePrefs.Region) ?? "";
			this.regionFilter.SelectEntry(new XUiC_ServerBrowserGamePrefSelectorCombo.GameOptionValue(stringValue, null));
		}
		if (this.languageFilter != null)
		{
			string value = GamePrefs.GetString(EnumGamePrefs.LanguageBrowser) ?? "";
			this.languageFilter.SetValue(value);
		}
		this.ApplyForcedSettings();
	}

	// Token: 0x060070CE RID: 28878 RVA: 0x002E0200 File Offset: 0x002DE400
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyForcedSettings()
	{
		bool flag = PermissionsManager.IsCrossplayAllowed();
		this.crossplayFilter.Enabled = flag;
		if (!flag)
		{
			this.crossplayFilter.SelectEntry(new XUiC_ServerBrowserGamePrefSelectorCombo.GameOptionValue(0, null));
		}
		if (!LaunchPrefs.AllowJoinConfigModded.Value)
		{
			this.moddedConfigsFilter.SelectEntry(new XUiC_ServerBrowserGamePrefSelectorCombo.GameOptionValue(0, null));
			this.moddedConfigsFilter.ViewComponent.UiTransform.gameObject.SetActive(false);
		}
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			this.requiresModsFilter.SelectEntry(new XUiC_ServerBrowserGamePrefSelectorCombo.GameOptionValue(0, null));
			this.requiresModsFilter.ViewComponent.UiTransform.gameObject.SetActive(false);
			if (Submission.Enabled)
			{
				this.eacFilter.SelectEntry(new XUiC_ServerBrowserGamePrefSelectorCombo.GameOptionValue(1, null));
				this.eacFilter.ViewComponent.UiTransform.gameObject.SetActive(false);
				this.ignoreSanctionsFilter.SelectEntry(new XUiC_ServerBrowserGamePrefSelectorCombo.GameOptionValue(0, null));
				this.ignoreSanctionsFilter.ViewComponent.UiTransform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060070CF RID: 28879 RVA: 0x002E0307 File Offset: 0x002DE507
	public void closeFilters()
	{
		this.windowGroup.Controller.GetChildByType<XUiC_ServerBrowser>().ShowingFilters = false;
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
	}

	// Token: 0x060070D0 RID: 28880 RVA: 0x002E0336 File Offset: 0x002DE536
	public void SetServersList(XUiC_ServersList _serversList)
	{
		_serversList.OnFilterResultsChanged += this.ServersList_OnFilterResultsChanged;
	}

	// Token: 0x060070D1 RID: 28881 RVA: 0x002E034A File Offset: 0x002DE54A
	[PublicizedFrom(EAccessModifier.Private)]
	public void ServersList_OnFilterResultsChanged(int _count)
	{
		this.currentResults = _count;
		this.IsDirty = true;
	}

	// Token: 0x060070D2 RID: 28882 RVA: 0x002E035A File Offset: 0x002DE55A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnFilterValueChanged(IServerBrowserFilterControl _sender)
	{
		Action<IServerBrowserFilterControl> onFilterChanged = this.OnFilterChanged;
		if (onFilterChanged != null)
		{
			onFilterChanged(_sender);
		}
		this.IsDirty = true;
	}

	// Token: 0x060070D3 RID: 28883 RVA: 0x002E0375 File Offset: 0x002DE575
	public override void OnVisibilityChanged(bool _isVisible)
	{
		base.OnVisibilityChanged(_isVisible);
		if (_isVisible && base.IsOpen)
		{
			this.SelectInitialElement();
		}
	}

	// Token: 0x060070D4 RID: 28884 RVA: 0x002E038F File Offset: 0x002DE58F
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.openedBefore)
		{
			this.openedBefore = true;
			this.ResetFilters();
		}
		else
		{
			this.ApplyForcedSettings();
		}
		base.RefreshBindings(false);
	}

	// Token: 0x060070D5 RID: 28885 RVA: 0x002E03BB File Offset: 0x002DE5BB
	public void SelectInitialElement()
	{
		if (ServerListManager.Instance.IsPrefilteredSearch)
		{
			base.GetChildById("btnStartSearch").SelectCursorElement(true, false);
			return;
		}
		base.GetChildById("btnResetFilters").SelectCursorElement(true, false);
	}

	// Token: 0x060070D6 RID: 28886 RVA: 0x002E03F0 File Offset: 0x002DE5F0
	public XUiView GetInitialSelectedElement()
	{
		if (ServerListManager.Instance.IsPrefilteredSearch)
		{
			return base.GetChildById("btnStartSearch").ViewComponent;
		}
		return base.GetChildById("btnResetFilters").ViewComponent;
	}

	// Token: 0x060070D7 RID: 28887 RVA: 0x002E0420 File Offset: 0x002DE620
	public override void OnClose()
	{
		base.OnClose();
		if (this.regionFilter != null)
		{
			XUiC_ServerBrowserGamePrefSelectorCombo.GameOptionValue selection = this.regionFilter.GetSelection();
			if (selection.Type == XUiC_ServerBrowserGamePrefSelectorCombo.EOptionValueType.String)
			{
				GamePrefs.Set(EnumGamePrefs.Region, selection.StringValue);
			}
		}
		if (this.languageFilter != null)
		{
			string value = this.languageFilter.GetValue();
			GamePrefs.Set(EnumGamePrefs.LanguageBrowser, value);
		}
		GamePrefs.Set(EnumGamePrefs.IgnoreEOSSanctions, false);
	}

	// Token: 0x040055B4 RID: 21940
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<IServerBrowserFilterControl> allFilterControls = new List<IServerBrowserFilterControl>();

	// Token: 0x040055B6 RID: 21942
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGamePrefSelectorCombo regionFilter;

	// Token: 0x040055B7 RID: 21943
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGamePrefString languageFilter;

	// Token: 0x040055B8 RID: 21944
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGamePrefSelectorCombo crossplayFilter;

	// Token: 0x040055B9 RID: 21945
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGamePrefSelectorCombo moddedConfigsFilter;

	// Token: 0x040055BA RID: 21946
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGamePrefSelectorCombo requiresModsFilter;

	// Token: 0x040055BB RID: 21947
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGamePrefSelectorCombo eacFilter;

	// Token: 0x040055BC RID: 21948
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServerBrowserGamePrefSelectorCombo ignoreSanctionsFilter;

	// Token: 0x040055BD RID: 21949
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentResults;

	// Token: 0x040055BE RID: 21950
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openedBefore;

	// Token: 0x040055BF RID: 21951
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnStartSearch;
}
