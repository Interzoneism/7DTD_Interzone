using System;
using System.Collections.Generic;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000E97 RID: 3735
[Preserve]
public class XUiC_TwitchWindowSelector : XUiController
{
	// Token: 0x17000C01 RID: 3073
	// (get) Token: 0x060075DE RID: 30174 RVA: 0x00300490 File Offset: 0x002FE690
	// (set) Token: 0x060075DF RID: 30175 RVA: 0x00300498 File Offset: 0x002FE698
	public new XUiV_Button Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			if (this.selected != null)
			{
				this.selected.Selected = false;
			}
			this.selected = value;
			if (this.selected != null)
			{
				this.selected.Selected = true;
				this.HandleSelectedChange();
			}
		}
	}

	// Token: 0x17000C02 RID: 3074
	// (get) Token: 0x060075E0 RID: 30176 RVA: 0x003004CF File Offset: 0x002FE6CF
	public string SelectedName
	{
		get
		{
			if (this.selected == null)
			{
				return "";
			}
			return this.selected.ID;
		}
	}

	// Token: 0x060075E1 RID: 30177 RVA: 0x003004EC File Offset: 0x002FE6EC
	public override void Init()
	{
		base.Init();
		XUiC_TwitchWindowSelector.ID = base.WindowGroup.ID;
		XUiController childById = base.GetChildById("lblWindowName");
		if (childById != null)
		{
			this.lblWindowName = (XUiV_Label)childById.ViewComponent;
		}
		this.categories.Clear();
		for (int i = 0; i < this.children.Count; i++)
		{
			XUiController xuiController = this.children[i];
			if (xuiController.ViewComponent.EventOnPress)
			{
				xuiController.OnPress += this.HandleOnPress;
				this.categories.Add(xuiController.ViewComponent.ID.ToLower());
			}
			xuiController.ViewComponent.IsNavigatable = (xuiController.ViewComponent.IsSnappable = false);
		}
		this.SetSelected("actions");
	}

	// Token: 0x060075E2 RID: 30178 RVA: 0x003005BC File Offset: 0x002FE7BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnPress(XUiController _sender, int _mouseButton)
	{
		this.Selected = (XUiV_Button)_sender.ViewComponent;
		this.OpenSelectedWindow();
	}

	// Token: 0x060075E3 RID: 30179 RVA: 0x003005D5 File Offset: 0x002FE7D5
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleSelectedChange()
	{
		this.updateWindowTitle();
	}

	// Token: 0x060075E4 RID: 30180 RVA: 0x003005DD File Offset: 0x002FE7DD
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateWindowTitle()
	{
		this.lblWindowName.Text = ((this.selected != null) ? Localization.Get("TwitchInfo_" + this.Selected.ID, false) : "");
	}

	// Token: 0x060075E5 RID: 30181 RVA: 0x00300614 File Offset: 0x002FE814
	public void OpenSelectedWindow()
	{
		if (this.Selected != null)
		{
			this.updateWindowTitle();
			string id = this.Selected.ID;
			GUIWindowManager windowManager = base.xui.playerUI.windowManager;
			if (id == "Actions")
			{
				base.xui.FindWindowGroupByName("twitchInfo").GetChildByType<XUiC_TwitchEntryListWindow>().SetOpenToActions(XUiC_TwitchWindowSelector.extras);
				XUiC_TwitchWindowSelector.extras = false;
				windowManager.OpenIfNotOpen("twitchInfo", true, false, true);
				return;
			}
			if (id == "Votes")
			{
				base.xui.FindWindowGroupByName("twitchInfo").GetChildByType<XUiC_TwitchEntryListWindow>().SetOpenToVotes();
				windowManager.OpenIfNotOpen("twitchInfo", true, false, true);
				return;
			}
			if (id == "ActionHistory")
			{
				base.xui.FindWindowGroupByName("twitchInfo").GetChildByType<XUiC_TwitchEntryListWindow>().SetOpenToHistory();
				windowManager.OpenIfNotOpen("twitchInfo", true, false, true);
				return;
			}
			if (!(id == "Leaderboard"))
			{
				return;
			}
			base.xui.FindWindowGroupByName("twitchInfo").GetChildByType<XUiC_TwitchEntryListWindow>().SetOpenToLeaderboard();
			windowManager.OpenIfNotOpen("twitchInfo", true, false, true);
		}
	}

	// Token: 0x060075E6 RID: 30182 RVA: 0x00300738 File Offset: 0x002FE938
	public void SetSelected(string name)
	{
		XUiController childById = base.GetChildById(name.ToLower());
		if (((childById != null) ? childById.ViewComponent : null) is XUiV_Button)
		{
			this.Selected = (XUiV_Button)childById.ViewComponent;
			this.currentCategoryIndex = this.categories.IndexOf(this.Selected.ID.ToLower());
		}
	}

	// Token: 0x060075E7 RID: 30183 RVA: 0x00300797 File Offset: 0x002FE997
	public override void OnOpen()
	{
		base.OnOpen();
		this.OpenSelectedWindow();
		base.xui.dragAndDrop.InMenu = true;
		Manager.PlayInsidePlayerHead("open_inventory", -1, 0f, false, false);
		base.RefreshBindings(false);
	}

	// Token: 0x060075E8 RID: 30184 RVA: 0x003007D0 File Offset: 0x002FE9D0
	public override void OnClose()
	{
		base.OnClose();
		GameManager.Instance.SetPauseWindowEffects(false);
		base.xui.dragAndDrop.InMenu = false;
		Manager.PlayInsidePlayerHead("close_inventory", -1, 0f, false, false);
		if (base.xui.currentSelectedEntry != null)
		{
			base.xui.currentSelectedEntry.Selected = false;
		}
	}

	// Token: 0x060075E9 RID: 30185 RVA: 0x00300830 File Offset: 0x002FEA30
	[PublicizedFrom(EAccessModifier.Private)]
	public void openSelectorAndWindow(string _selectedPage)
	{
		_selectedPage = _selectedPage.ToLower();
		XUiC_FocusedBlockHealth.SetData(base.xui.playerUI, null, 0f);
		if (base.xui.playerUI.windowManager.IsWindowOpen("twitchWindowpaging") && this.SelectedName.EqualsCaseInsensitive(_selectedPage) && !this.OverrideClose)
		{
			base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
			if (base.xui.playerUI.windowManager.IsWindowOpen("twitchWindowpaging"))
			{
				base.xui.playerUI.windowManager.Close("twitchWindowpaging");
				return;
			}
		}
		else
		{
			this.SetSelected(_selectedPage);
			if (base.xui.playerUI.windowManager.IsWindowOpen("twitchWindowpaging"))
			{
				this.OpenSelectedWindow();
				return;
			}
			base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
			base.xui.playerUI.windowManager.Open("twitchWindowpaging", false, false, true);
		}
	}

	// Token: 0x060075EA RID: 30186 RVA: 0x0030093B File Offset: 0x002FEB3B
	public static void OpenSelectorAndWindow(EntityPlayerLocal _localPlayer, string _selectedPage, bool _extras = false)
	{
		if (_localPlayer.IsDead())
		{
			return;
		}
		XUiC_TwitchWindowSelector.extras = _extras;
		LocalPlayerUI.GetUIForPlayer(_localPlayer).xui.FindWindowGroupByName("twitchWindowpaging").GetChildByType<XUiC_TwitchWindowSelector>().openSelectorAndWindow(_selectedPage);
	}

	// Token: 0x060075EB RID: 30187 RVA: 0x0030096C File Offset: 0x002FEB6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void toggleCategory(int _dir)
	{
		int index = NGUIMath.RepeatIndex(this.currentCategoryIndex + _dir, this.categories.Count);
		XUiController childById = base.GetChildById(this.categories[index]);
		if (((childById != null) ? childById.ViewComponent : null) is XUiV_Button)
		{
			if (childById.ViewComponent.IsVisible)
			{
				this.SetSelected(this.categories[index]);
				this.OpenSelectedWindow();
				return;
			}
			this.currentCategoryIndex = index;
			this.toggleCategory(_dir);
		}
	}

	// Token: 0x060075EC RID: 30188 RVA: 0x003009EC File Offset: 0x002FEBEC
	public static void ToggleCategory(EntityPlayerLocal _localPlayer, int _dir)
	{
		LocalPlayerUI.GetUIForPlayer(_localPlayer).xui.FindWindowGroupByName("twitchWindowpaging").GetChildByType<XUiC_TwitchWindowSelector>().toggleCategory(_dir);
	}

	// Token: 0x060075ED RID: 30189 RVA: 0x00300A10 File Offset: 0x002FEC10
	public override void Update(float _dt)
	{
		base.Update(_dt);
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (base.xui.playerUI.windowManager.IsKeyShortcutsAllowed())
		{
			if (base.xui.playerUI.playerInput.GUIActions.WindowPagingLeft.WasReleased && windowManager.IsWindowOpen(XUiC_TwitchWindowSelector.ID))
			{
				XUiC_TwitchWindowSelector.ToggleCategory(base.xui.playerUI.entityPlayer, -1);
			}
			if (base.xui.playerUI.playerInput.GUIActions.WindowPagingRight.WasReleased && windowManager.IsWindowOpen(XUiC_TwitchWindowSelector.ID))
			{
				XUiC_TwitchWindowSelector.ToggleCategory(base.xui.playerUI.entityPlayer, 1);
			}
		}
		this.OverrideClose = false;
	}

	// Token: 0x040059F4 RID: 23028
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblWindowName;

	// Token: 0x040059F5 RID: 23029
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button selected;

	// Token: 0x040059F6 RID: 23030
	public static string ID = "";

	// Token: 0x040059F7 RID: 23031
	public bool OverrideClose;

	// Token: 0x040059F8 RID: 23032
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> categories = new List<string>();

	// Token: 0x040059F9 RID: 23033
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentCategoryIndex;

	// Token: 0x040059FA RID: 23034
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool extras = false;
}
