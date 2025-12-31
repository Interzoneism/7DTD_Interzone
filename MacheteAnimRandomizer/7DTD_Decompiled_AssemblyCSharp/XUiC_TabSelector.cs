using System;
using System.Collections.Generic;
using GUI_2;
using UnityEngine.Scripting;

// Token: 0x02000E5A RID: 3674
[Preserve]
public class XUiC_TabSelector : XUiController
{
	// Token: 0x140000C5 RID: 197
	// (add) Token: 0x0600735D RID: 29533 RVA: 0x002F06D4 File Offset: 0x002EE8D4
	// (remove) Token: 0x0600735E RID: 29534 RVA: 0x002F070C File Offset: 0x002EE90C
	public event XUiC_TabSelector.TabChangedDelegate OnTabChanged;

	// Token: 0x17000BB0 RID: 2992
	// (get) Token: 0x0600735F RID: 29535 RVA: 0x002F0741 File Offset: 0x002EE941
	// (set) Token: 0x06007360 RID: 29536 RVA: 0x002F074C File Offset: 0x002EE94C
	public int SelectedTabIndex
	{
		get
		{
			return this.selectedTabIndex;
		}
		set
		{
			if (!this.enabled)
			{
				return;
			}
			if (value >= this.tabs.Count)
			{
				value = this.tabs.Count - 1;
			}
			if (this.selectedTabIndex == value)
			{
				return;
			}
			if (value < 0)
			{
				if (this.SelectedTab != null)
				{
					this.SelectedTab.TabSelected = false;
				}
				this.selectedTabIndex = value;
				this.updateTabVisibility();
				XUiC_TabSelector.TabChangedDelegate onTabChanged = this.OnTabChanged;
				if (onTabChanged == null)
				{
					return;
				}
				onTabChanged(this.selectedTabIndex, this.SelectedTab);
				return;
			}
			else
			{
				if (!this.tabs[value].TabVisible)
				{
					return;
				}
				if (this.SelectedTab != null)
				{
					this.SelectedTab.TabSelected = false;
				}
				this.selectedTabIndex = value;
				this.SelectedTab.TabSelected = true;
				this.updateTabVisibility();
				XUiC_TabSelector.TabChangedDelegate onTabChanged2 = this.OnTabChanged;
				if (onTabChanged2 != null)
				{
					onTabChanged2(this.selectedTabIndex, this.SelectedTab);
				}
				if (base.xui.playerUI.CursorController.navigationTarget != null && base.xui.playerUI.CursorController.navigationTarget.Controller.IsChildOf(this))
				{
					base.xui.playerUI.CursorController.SetNavigationTarget(null);
				}
				if (this.selectTabContentsOnChange)
				{
					this.pendingOnChangeSelection = true;
				}
				return;
			}
		}
	}

	// Token: 0x17000BB1 RID: 2993
	// (get) Token: 0x06007361 RID: 29537 RVA: 0x002F0887 File Offset: 0x002EEA87
	// (set) Token: 0x06007362 RID: 29538 RVA: 0x002F08B8 File Offset: 0x002EEAB8
	public XUiC_TabSelectorTab SelectedTab
	{
		get
		{
			if (this.selectedTabIndex < 0 || this.selectedTabIndex >= this.tabs.Count)
			{
				return null;
			}
			return this.tabs[this.selectedTabIndex];
		}
		set
		{
			if (value == this.SelectedTab)
			{
				return;
			}
			int num = this.tabs.IndexOf(value);
			if (num >= 0)
			{
				this.SelectedTabIndex = num;
			}
		}
	}

	// Token: 0x17000BB2 RID: 2994
	// (get) Token: 0x06007363 RID: 29539 RVA: 0x002F08E7 File Offset: 0x002EEAE7
	public XUiC_TabSelectorButton SelectedTabButton
	{
		get
		{
			XUiC_TabSelectorTab selectedTab = this.SelectedTab;
			if (selectedTab == null)
			{
				return null;
			}
			return selectedTab.TabButton;
		}
	}

	// Token: 0x06007364 RID: 29540 RVA: 0x002F08FC File Offset: 0x002EEAFC
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("tabsHeader");
		if (childById == null)
		{
			Log.Error("[XUi] TabSelector without 'tabsHeader' in windowGroup '" + this.windowGroup.ID + "'");
			return;
		}
		XUiController childById2 = base.GetChildById("tabsContents");
		if (childById2 == null)
		{
			Log.Error("[XUi] TabSelector without 'tabsContents' in windowGroup '" + this.windowGroup.ID + "'");
			return;
		}
		childById.GetChildrenByType<XUiC_TabSelectorButton>(this.tabButtons);
		childById2.GetChildrenByType<XUiC_TabSelectorTab>(this.tabs);
		if (this.tabButtons.Count == 0)
		{
			Log.Error("[XUi] TabSelector without any TabSelectorButtons in 'tabsHeader' in windowGroup '" + this.windowGroup.ID + "'");
			return;
		}
		if (this.tabs.Count == 0)
		{
			Log.Error("[XUi] TabSelector without any TabSelectorTabs in 'tabsContent' in windowGroup '" + this.windowGroup.ID + "'");
			return;
		}
		for (int i = 0; i < this.tabs.Count; i++)
		{
			if (i >= this.tabButtons.Count)
			{
				Log.Warning(string.Format("More tabs ({0}) than tab buttons ({1}) in windowGroup '{2}'", this.tabs.Count, this.tabButtons.Count, base.WindowGroup.ID));
				break;
			}
			XUiC_TabSelectorTab xuiC_TabSelectorTab = this.tabs[i];
			XUiC_TabSelectorButton xuiC_TabSelectorButton = this.tabButtons[i];
			xuiC_TabSelectorButton.Tab = xuiC_TabSelectorTab;
			xuiC_TabSelectorTab.TabButton = xuiC_TabSelectorButton;
		}
		XUiController childById3 = base.GetChildById("backgroundMainTabs");
		XUiV_Sprite xuiV_Sprite = ((childById3 != null) ? childById3.ViewComponent : null) as XUiV_Sprite;
		if (xuiV_Sprite != null)
		{
			this.tabHeaderBackground = xuiV_Sprite;
			XUiController childById4 = base.GetChildById("border");
			XUiV_Sprite xuiV_Sprite2 = ((childById4 != null) ? childById4.ViewComponent : null) as XUiV_Sprite;
			if (xuiV_Sprite2 != null)
			{
				this.tabSelectorBorder = xuiV_Sprite2;
				this.tabHeaderBackground.Sprite.rightAnchor.target = this.tabSelectorBorder.UiTransform;
				this.tabHeaderBackground.Sprite.rightAnchor.relative = 1f;
			}
		}
		this.tabs[0].TabSelected = true;
	}

	// Token: 0x06007365 RID: 29541 RVA: 0x002F0B10 File Offset: 0x002EED10
	public override void OnOpen()
	{
		base.OnOpen();
		this.updateTabVisibility();
		this.pendingOnOpenSelection = !this.selectOnOpen;
		this.pendingOnChangeSelection = false;
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.LeftBumper, "igcoTabLeft", XUiC_GamepadCalloutWindow.CalloutType.Tabs);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightBumper, "igcoTabRight", XUiC_GamepadCalloutWindow.CalloutType.Tabs);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Tabs, 0f);
	}

	// Token: 0x06007366 RID: 29542 RVA: 0x002F0B86 File Offset: 0x002EED86
	public override void OnClose()
	{
		base.OnClose();
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Tabs);
	}

	// Token: 0x06007367 RID: 29543 RVA: 0x002F0BA0 File Offset: 0x002EEDA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateTabVisibility()
	{
		int num = -1;
		for (int i = 0; i < this.tabButtons.Count; i++)
		{
			if (i >= this.tabs.Count)
			{
				this.tabButtons[i].ViewComponent.IsVisible = false;
			}
			else
			{
				bool tabVisible = this.tabs[i].TabVisible;
				bool isVisible = i == this.selectedTabIndex;
				this.tabButtons[i].ViewComponent.IsVisible = tabVisible;
				this.tabs[i].ViewComponent.IsVisible = isVisible;
				if (tabVisible)
				{
					num = i;
				}
			}
		}
		if (this.tabHeaderBackground != null)
		{
			UIRect.AnchorPoint leftAnchor = this.tabHeaderBackground.Sprite.leftAnchor;
			if (num >= 0)
			{
				leftAnchor.target = this.tabButtons[num].OuterDimensionsTransform;
				leftAnchor.relative = 1f;
			}
			else
			{
				UIRect.AnchorPoint anchorPoint = leftAnchor;
				XUiV_Sprite xuiV_Sprite = this.tabSelectorBorder;
				anchorPoint.target = ((xuiV_Sprite != null) ? xuiV_Sprite.UiTransform : null);
				leftAnchor.relative = 0f;
			}
			this.tabHeaderBackground.Sprite.ResetAndUpdateAnchors();
		}
	}

	// Token: 0x06007368 RID: 29544 RVA: 0x002F0CB8 File Offset: 0x002EEEB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void ToggleCategory(int _dir, bool _playSound = true)
	{
		int num = this.selectedTabIndex;
		do
		{
			num = NGUIMath.RepeatIndex(num + _dir, this.tabs.Count);
		}
		while (!this.tabs[num].TabVisible && num != this.selectedTabIndex);
		this.SelectedTabIndex = num;
		if (!this.SelectedTab.TabVisible)
		{
			this.SelectedTabIndex = -1;
		}
		if (_playSound)
		{
			XUiC_TabSelectorButton selectedTabButton = this.SelectedTabButton;
			if (selectedTabButton == null)
			{
				return;
			}
			selectedTabButton.PlayClickSound();
		}
	}

	// Token: 0x06007369 RID: 29545 RVA: 0x002F0D2C File Offset: 0x002EEF2C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		bool flag = this.tabInputAllowed;
		if (base.xui.playerUI.CursorController.lockNavigationToView != null)
		{
			this.tabInputAllowed = base.IsChildOf(base.xui.playerUI.CursorController.lockNavigationToView.Controller);
		}
		else
		{
			this.tabInputAllowed = true;
		}
		if (this.tabInputAllowed != flag)
		{
			if (this.tabInputAllowed)
			{
				base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Tabs, 0f);
			}
			else
			{
				base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Tabs);
			}
		}
		LocalPlayerUI playerUI = base.xui.playerUI;
		PlayerActionsGUI guiactions = playerUI.playerInput.GUIActions;
		GUIWindowManager windowManager = playerUI.windowManager;
		if (this.tabInputAllowed && windowManager.IsKeyShortcutsAllowed() && this.isActiveTabSelector)
		{
			if (guiactions.WindowPagingLeft.WasReleased && windowManager.IsWindowOpen(this.windowGroup.ID))
			{
				this.ToggleCategory(-1, true);
			}
			if (guiactions.WindowPagingRight.WasReleased && windowManager.IsWindowOpen(this.windowGroup.ID))
			{
				this.ToggleCategory(1, true);
			}
		}
		XUiC_TabSelectorTab selectedTab = this.SelectedTab;
		if (selectedTab != null)
		{
			if (!this.pendingOnOpenSelection)
			{
				this.pendingOnOpenSelection = selectedTab.SelectCursorElement(true, false);
			}
			if (this.pendingOnChangeSelection && this.isActiveTabSelector)
			{
				selectedTab.SelectCursorElement(true, false);
				this.pendingOnChangeSelection = false;
			}
		}
	}

	// Token: 0x0600736A RID: 29546 RVA: 0x002F0E8E File Offset: 0x002EF08E
	public void SetTabCaption(int _index, string _name)
	{
		if (_index >= this.tabs.Count)
		{
			throw new ArgumentOutOfRangeException("_index");
		}
		this.tabs[_index].TabHeaderText = _name;
	}

	// Token: 0x0600736B RID: 29547 RVA: 0x002F0EBB File Offset: 0x002EF0BB
	public XUiC_TabSelectorTab GetTab(int _index)
	{
		if (_index >= this.tabs.Count)
		{
			throw new ArgumentOutOfRangeException("_index");
		}
		return this.tabs[_index];
	}

	// Token: 0x0600736C RID: 29548 RVA: 0x002F0EE4 File Offset: 0x002EF0E4
	public void SelectTabByName(string _tabKey)
	{
		foreach (XUiC_TabSelectorTab xuiC_TabSelectorTab in this.tabs)
		{
			if (!(xuiC_TabSelectorTab.TabKey != _tabKey))
			{
				this.SelectedTab = xuiC_TabSelectorTab;
				break;
			}
		}
	}

	// Token: 0x0600736D RID: 29549 RVA: 0x002F0F48 File Offset: 0x002EF148
	public bool IsSelected(string _tabKey)
	{
		XUiC_TabSelectorTab selectedTab = this.SelectedTab;
		return selectedTab != null && selectedTab.TabKey.Equals(_tabKey);
	}

	// Token: 0x0600736E RID: 29550 RVA: 0x002F0F64 File Offset: 0x002EF164
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "select_tab_contents_on_open")
		{
			if (!string.IsNullOrEmpty(_value))
			{
				this.selectOnOpen = StringParsers.ParseBool(_value, 0, -1, true);
			}
			return true;
		}
		if (!(_name == "select_tab_contents_on_change"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		if (!string.IsNullOrEmpty(_value))
		{
			this.selectTabContentsOnChange = StringParsers.ParseBool(_value, 0, -1, true);
		}
		return true;
	}

	// Token: 0x0600736F RID: 29551 RVA: 0x002F0FC8 File Offset: 0x002EF1C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06007370 RID: 29552 RVA: 0x002F0FD2 File Offset: 0x002EF1D2
	public void TabVisibilityChanged(XUiC_TabSelectorTab _tab, bool _visible)
	{
		if (_tab == this.SelectedTab && !_visible)
		{
			this.ToggleCategory(-1, false);
		}
		else if (_visible && this.SelectedTabIndex < 0)
		{
			this.SelectedTab = _tab;
		}
		this.updateTabVisibility();
	}

	// Token: 0x06007371 RID: 29553 RVA: 0x002F1003 File Offset: 0x002EF203
	public void TabButtonClicked(XUiC_TabSelectorButton _tabButton)
	{
		if (!this.enabled)
		{
			return;
		}
		this.SelectedTabIndex = this.tabButtons.IndexOf(_tabButton);
	}

	// Token: 0x040057D9 RID: 22489
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_TabSelectorButton> tabButtons = new List<XUiC_TabSelectorButton>();

	// Token: 0x040057DA RID: 22490
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<XUiC_TabSelectorTab> tabs = new List<XUiC_TabSelectorTab>();

	// Token: 0x040057DB RID: 22491
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite tabSelectorBorder;

	// Token: 0x040057DC RID: 22492
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite tabHeaderBackground;

	// Token: 0x040057DD RID: 22493
	[PublicizedFrom(EAccessModifier.Private)]
	public bool enabled = true;

	// Token: 0x040057DE RID: 22494
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedTabIndex;

	// Token: 0x040057DF RID: 22495
	[PublicizedFrom(EAccessModifier.Private)]
	public bool selectOnOpen;

	// Token: 0x040057E0 RID: 22496
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pendingOnOpenSelection;

	// Token: 0x040057E1 RID: 22497
	[PublicizedFrom(EAccessModifier.Private)]
	public bool tabInputAllowed = true;

	// Token: 0x040057E3 RID: 22499
	public bool selectTabContentsOnChange = true;

	// Token: 0x040057E4 RID: 22500
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pendingOnChangeSelection;

	// Token: 0x040057E5 RID: 22501
	public bool isActiveTabSelector = true;

	// Token: 0x02000E5B RID: 3675
	// (Invoke) Token: 0x06007374 RID: 29556
	public delegate void TabChangedDelegate(int _tabIndex, XUiC_TabSelectorTab _tab);
}
