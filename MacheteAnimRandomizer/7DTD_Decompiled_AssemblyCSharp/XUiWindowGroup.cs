using System;
using Platform;

// Token: 0x02000F2D RID: 3885
public class XUiWindowGroup : GUIWindow
{
	// Token: 0x17000D01 RID: 3329
	// (get) Token: 0x06007BEB RID: 31723 RVA: 0x0032277C File Offset: 0x0032097C
	public string ID
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x17000D02 RID: 3330
	// (get) Token: 0x06007BEC RID: 31724 RVA: 0x00322784 File Offset: 0x00320984
	// (set) Token: 0x06007BED RID: 31725 RVA: 0x0032278C File Offset: 0x0032098C
	public XUi xui
	{
		get
		{
			return this.mXUi;
		}
		set
		{
			this.mXUi = value;
			this.playerUI = this.mXUi.playerUI;
			this.windowManager = this.playerUI.windowManager;
			this.nguiWindowManager = this.playerUI.nguiWindowManager;
		}
	}

	// Token: 0x06007BEE RID: 31726 RVA: 0x003227C8 File Offset: 0x003209C8
	public XUiWindowGroup(string _id, XUiWindowGroup.EHasActionSetFor _hasActionSetFor = XUiWindowGroup.EHasActionSetFor.Both, string _defaultSelectedName = "") : base(_id)
	{
		this.hasActionSetFor = _hasActionSetFor;
		this.defaultSelectedView = _defaultSelectedName;
	}

	// Token: 0x17000D03 RID: 3331
	// (get) Token: 0x06007BEF RID: 31727 RVA: 0x00322807 File Offset: 0x00320A07
	public bool Initialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x06007BF0 RID: 31728 RVA: 0x00322810 File Offset: 0x00320A10
	public void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.Controller.Init();
		for (int i = 0; i < this.Controller.Children.Count; i++)
		{
			XUiController xuiController = this.Controller.Children[i];
			if (xuiController.ViewComponent != null)
			{
				xuiController.ViewComponent.IsVisible = false;
				string name = xuiController.ViewComponent.UiTransform.parent.name;
				this.UseStackPanelAlignment |= (name.EqualsCaseInsensitive("left") || name.EqualsCaseInsensitive("right"));
			}
		}
		this.windowManager.Add(this.id, this);
		this.initialized = true;
	}

	// Token: 0x06007BF1 RID: 31729 RVA: 0x003228CC File Offset: 0x00320ACC
	public override void OnOpen()
	{
		base.OnOpen();
		this.Controller.OnOpen();
		if (!string.IsNullOrEmpty(this.defaultSelectedView))
		{
			if (this.defaultSelectedView.StartsWith("bp."))
			{
				XUiC_BackpackWindow.defaultSelectedElement = this.defaultSelectedView.Remove(0, 3);
			}
			else
			{
				XUiController childById = this.Controller.GetChildById(this.defaultSelectedView);
				if (childById != null)
				{
					childById.SelectCursorElement(true, false);
				}
				else
				{
					Log.Warning("Could not find selectable element {0} in WindowGroup {1}", new object[]
					{
						this.defaultSelectedView,
						this.ID
					});
				}
			}
		}
		if (this.closeCompassOnOpen)
		{
			this.windowManager.CloseIfOpen("compass");
		}
		if (this.openBackpackOnOpen && GameManager.Instance != null)
		{
			this.windowManager.OpenIfNotOpen("backpack", false, false, true);
		}
		this.xui.RecenterWindowGroup(this, false);
		switch (this.hasActionSetFor)
		{
		case XUiWindowGroup.EHasActionSetFor.Both:
			this.hasActionSetThisOpen = true;
			return;
		case XUiWindowGroup.EHasActionSetFor.OnlyController:
			this.hasActionSetThisOpen = (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard);
			return;
		case XUiWindowGroup.EHasActionSetFor.OnlyKeyboard:
			this.hasActionSetThisOpen = (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard);
			return;
		case XUiWindowGroup.EHasActionSetFor.None:
			this.hasActionSetThisOpen = false;
			return;
		default:
			return;
		}
	}

	// Token: 0x06007BF2 RID: 31730 RVA: 0x00322A10 File Offset: 0x00320C10
	public override void OnClose()
	{
		base.OnClose();
		if (this.xui.dragAndDrop != null)
		{
			this.xui.dragAndDrop.PlaceItemBackInInventory();
		}
		this.Controller.OnClose();
		if (this.openBackpackOnOpen && GameManager.Instance != null)
		{
			this.windowManager.CloseIfOpen("backpack");
		}
		if (this.xui.currentToolTip != null)
		{
			this.xui.currentToolTip.ToolTip = "";
		}
		if (this.xui.currentPopupMenu != null)
		{
			this.xui.currentPopupMenu.ClearItems();
		}
	}

	// Token: 0x06007BF3 RID: 31731 RVA: 0x00322AAF File Offset: 0x00320CAF
	public override bool HasActionSet()
	{
		return this.hasActionSetThisOpen;
	}

	// Token: 0x06007BF4 RID: 31732 RVA: 0x00322AB8 File Offset: 0x00320CB8
	public bool HasStackPanelWindows()
	{
		if (this.Controller == null)
		{
			return false;
		}
		foreach (XUiController xuiController in this.Controller.Children)
		{
			XUiV_Window xuiV_Window = xuiController.ViewComponent as XUiV_Window;
			if (xuiV_Window != null && xuiV_Window.IsInStackpanel)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04005DBB RID: 23995
	public XUiController Controller;

	// Token: 0x04005DBC RID: 23996
	public bool LeftPanelVAlignTop = true;

	// Token: 0x04005DBD RID: 23997
	public bool RightPanelVAlignTop = true;

	// Token: 0x04005DBE RID: 23998
	public bool UseStackPanelAlignment;

	// Token: 0x04005DBF RID: 23999
	public bool BoundsCalculated;

	// Token: 0x04005DC0 RID: 24000
	public int StackPanelYOffset = 457;

	// Token: 0x04005DC1 RID: 24001
	public int StackPanelPadding = 9;

	// Token: 0x04005DC2 RID: 24002
	public bool openBackpackOnOpen;

	// Token: 0x04005DC3 RID: 24003
	public bool closeCompassOnOpen;

	// Token: 0x04005DC4 RID: 24004
	public string defaultSelectedView;

	// Token: 0x04005DC5 RID: 24005
	[PublicizedFrom(EAccessModifier.Private)]
	public XUi mXUi;

	// Token: 0x04005DC6 RID: 24006
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiWindowGroup.EHasActionSetFor hasActionSetFor;

	// Token: 0x04005DC7 RID: 24007
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasActionSetThisOpen = true;

	// Token: 0x04005DC8 RID: 24008
	[PublicizedFrom(EAccessModifier.Private)]
	public bool initialized;

	// Token: 0x02000F2E RID: 3886
	public enum EHasActionSetFor
	{
		// Token: 0x04005DCA RID: 24010
		Both,
		// Token: 0x04005DCB RID: 24011
		OnlyController,
		// Token: 0x04005DCC RID: 24012
		OnlyKeyboard,
		// Token: 0x04005DCD RID: 24013
		None
	}
}
