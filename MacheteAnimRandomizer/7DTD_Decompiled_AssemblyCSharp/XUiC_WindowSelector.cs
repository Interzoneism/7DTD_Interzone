using System;
using System.Collections.Generic;
using Audio;
using UnityEngine.Scripting;

// Token: 0x02000EA7 RID: 3751
[Preserve]
public class XUiC_WindowSelector : XUiController
{
	// Token: 0x17000C13 RID: 3091
	// (get) Token: 0x06007670 RID: 30320 RVA: 0x0030361F File Offset: 0x0030181F
	// (set) Token: 0x06007671 RID: 30321 RVA: 0x00303627 File Offset: 0x00301827
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

	// Token: 0x17000C14 RID: 3092
	// (get) Token: 0x06007672 RID: 30322 RVA: 0x0030365E File Offset: 0x0030185E
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

	// Token: 0x06007673 RID: 30323 RVA: 0x0030367C File Offset: 0x0030187C
	public override void Init()
	{
		base.Init();
		XUiC_WindowSelector.ID = base.WindowGroup.ID;
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
		this.SetSelected("crafting");
	}

	// Token: 0x06007674 RID: 30324 RVA: 0x0030374C File Offset: 0x0030194C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnPress(XUiController _sender, int _mouseButton)
	{
		this.Selected = (XUiV_Button)_sender.ViewComponent;
		this.OpenSelectedWindow();
	}

	// Token: 0x06007675 RID: 30325 RVA: 0x00303765 File Offset: 0x00301965
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleSelectedChange()
	{
		this.updateWindowTitle();
	}

	// Token: 0x06007676 RID: 30326 RVA: 0x0030376D File Offset: 0x0030196D
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateWindowTitle()
	{
		this.lblWindowName.Text = ((this.selected != null) ? Localization.Get("xui" + this.Selected.ID, false) : "");
	}

	// Token: 0x06007677 RID: 30327 RVA: 0x003037A4 File Offset: 0x003019A4
	public void OpenSelectedWindow()
	{
		if (this.Selected != null)
		{
			this.updateWindowTitle();
			string id = this.Selected.ID;
			if (!base.xui.playerUI.windowManager.IsWindowOpen(id))
			{
				base.xui.playerUI.windowManager.CloseAllOpenWindows("windowpaging");
				base.xui.playerUI.windowManager.Open(id, true, false, true);
			}
		}
	}

	// Token: 0x06007678 RID: 30328 RVA: 0x00303818 File Offset: 0x00301A18
	public void SetSelected(string name)
	{
		XUiController childById = base.GetChildById(name.ToLower());
		if (((childById != null) ? childById.ViewComponent : null) is XUiV_Button)
		{
			this.Selected = (XUiV_Button)childById.ViewComponent;
			this.currentCategoryIndex = this.categories.IndexOf(this.Selected.ID.ToLower());
		}
	}

	// Token: 0x06007679 RID: 30329 RVA: 0x00303877 File Offset: 0x00301A77
	public override void OnOpen()
	{
		base.OnOpen();
		this.OpenSelectedWindow();
		base.xui.dragAndDrop.InMenu = true;
		Manager.PlayInsidePlayerHead("open_inventory", -1, 0f, false, false);
		base.RefreshBindings(false);
	}

	// Token: 0x0600767A RID: 30330 RVA: 0x003038B0 File Offset: 0x00301AB0
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
		XUiWindowGroup xuiWindowGroup = base.xui.playerUI.windowManager.GetWindow("toolbelt") as XUiWindowGroup;
		if (xuiWindowGroup == null)
		{
			return;
		}
		XUiC_Toolbelt childByType = xuiWindowGroup.Controller.GetChildByType<XUiC_Toolbelt>();
		if (childByType == null)
		{
			return;
		}
		childByType.ClearHoveredItems();
	}

	// Token: 0x0600767B RID: 30331 RVA: 0x00303948 File Offset: 0x00301B48
	[PublicizedFrom(EAccessModifier.Private)]
	public void openSelectorAndWindow(string _selectedPage)
	{
		_selectedPage = _selectedPage.ToLower();
		XUiC_FocusedBlockHealth.SetData(base.xui.playerUI, null, 0f);
		if (base.xui.playerUI.windowManager.IsWindowOpen("windowpaging") && this.SelectedName.EqualsCaseInsensitive(_selectedPage) && !this.OverrideClose)
		{
			base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
			if (base.xui.playerUI.windowManager.IsWindowOpen("windowpaging"))
			{
				base.xui.playerUI.windowManager.Close("windowpaging");
				return;
			}
		}
		else
		{
			this.SetSelected(_selectedPage);
			if (base.xui.playerUI.windowManager.IsWindowOpen("windowpaging"))
			{
				this.OpenSelectedWindow();
				return;
			}
			base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
			base.xui.playerUI.windowManager.Open("windowpaging", false, false, true);
		}
	}

	// Token: 0x0600767C RID: 30332 RVA: 0x00303A53 File Offset: 0x00301C53
	public static void OpenSelectorAndWindow(EntityPlayerLocal _localPlayer, string selectedPage)
	{
		if (_localPlayer.IsDead())
		{
			return;
		}
		LocalPlayerUI.GetUIForPlayer(_localPlayer).xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>().openSelectorAndWindow(selectedPage);
	}

	// Token: 0x0600767D RID: 30333 RVA: 0x00303A80 File Offset: 0x00301C80
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

	// Token: 0x0600767E RID: 30334 RVA: 0x00303B00 File Offset: 0x00301D00
	public static void ToggleCategory(EntityPlayerLocal _localPlayer, int _dir)
	{
		LocalPlayerUI.GetUIForPlayer(_localPlayer).xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>().toggleCategory(_dir);
	}

	// Token: 0x0600767F RID: 30335 RVA: 0x00303B24 File Offset: 0x00301D24
	public override void Update(float _dt)
	{
		base.Update(_dt);
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (base.xui.playerUI.windowManager.IsKeyShortcutsAllowed())
		{
			if (base.xui.playerUI.playerInput.GUIActions.WindowPagingLeft.WasReleased && windowManager.IsWindowOpen(XUiC_WindowSelector.ID))
			{
				XUiC_WindowSelector.ToggleCategory(base.xui.playerUI.entityPlayer, -1);
			}
			if (base.xui.playerUI.playerInput.GUIActions.WindowPagingRight.WasReleased && windowManager.IsWindowOpen(XUiC_WindowSelector.ID))
			{
				XUiC_WindowSelector.ToggleCategory(base.xui.playerUI.entityPlayer, 1);
			}
		}
		this.OverrideClose = false;
	}

	// Token: 0x04005A57 RID: 23127
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblWindowName;

	// Token: 0x04005A58 RID: 23128
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button selected;

	// Token: 0x04005A59 RID: 23129
	public static string ID = "";

	// Token: 0x04005A5A RID: 23130
	public bool OverrideClose;

	// Token: 0x04005A5B RID: 23131
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> categories = new List<string>();

	// Token: 0x04005A5C RID: 23132
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentCategoryIndex;
}
