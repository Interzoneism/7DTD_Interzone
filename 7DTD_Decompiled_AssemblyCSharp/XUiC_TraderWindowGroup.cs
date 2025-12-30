using System;
using UnityEngine.Scripting;

// Token: 0x02000E7C RID: 3708
[Preserve]
public class XUiC_TraderWindowGroup : XUiController
{
	// Token: 0x0600748F RID: 29839 RVA: 0x002F6C70 File Offset: 0x002F4E70
	public override void Init()
	{
		base.Init();
		XUiController childByType = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		if (childByType != null)
		{
			this.nonPagingHeader = (XUiC_WindowNonPagingHeader)childByType;
		}
		childByType = base.GetChildByType<XUiC_TraderWindow>();
		if (childByType != null)
		{
			this.TraderWindow = (XUiC_TraderWindow)childByType;
		}
		childByType = base.GetChildByType<XUiC_ServiceInfoWindow>();
		if (childByType != null)
		{
			this.ServiceInfoWindow = (XUiC_ServiceInfoWindow)childByType;
		}
	}

	// Token: 0x06007490 RID: 29840 RVA: 0x002F6CC8 File Offset: 0x002F4EC8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.windowGroup.isShowing)
		{
			if (!base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
			{
				this.wasReleased = true;
			}
			if (this.wasReleased)
			{
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.IsPressed)
				{
					this.activeKeyDown = true;
				}
				if (base.xui.playerUI.playerInput.PermanentActions.Activate.WasReleased && this.activeKeyDown)
				{
					this.activeKeyDown = false;
					if (!base.xui.playerUI.windowManager.IsInputActive())
					{
						base.xui.playerUI.windowManager.CloseAllOpenWindows(null, false);
					}
				}
			}
		}
	}

	// Token: 0x06007491 RID: 29841 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AlwaysUpdate()
	{
		return false;
	}

	// Token: 0x06007492 RID: 29842 RVA: 0x002F6DA8 File Offset: 0x002F4FA8
	public override void OnOpen()
	{
		base.xui.Trader.TraderWindowGroup = this;
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnOpen();
			base.ViewComponent.IsVisible = true;
		}
		if (this.nonPagingHeader != null)
		{
			this.nonPagingHeader.SetHeader((base.xui.Trader.TraderTileEntity.entityId == -1) ? Localization.Get("xuiVending", false) : Localization.Get("xuiTrader", false));
		}
		base.xui.RecenterWindowGroup(this.windowGroup, false);
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].OnOpen();
		}
		if (this.ServiceInfoWindow != null)
		{
			this.ServiceInfoWindow.ViewComponent.IsVisible = false;
		}
		if (base.xui.Trader.TraderTileEntity.entityId != -1 && base.xui.playerUI.entityPlayer.OverrideFOV != 30f)
		{
			base.xui.playerUI.entityPlayer.OverrideFOV = 30f;
			base.xui.playerUI.entityPlayer.OverrideLookAt = base.xui.Trader.TraderEntity.getHeadPosition();
		}
		base.xui.Dialog.keepZoomOnClose = false;
		this.IsDirty = true;
	}

	// Token: 0x06007493 RID: 29843 RVA: 0x002F6F18 File Offset: 0x002F5118
	public override void OnClose()
	{
		base.OnClose();
		this.wasReleased = false;
		this.activeKeyDown = false;
		base.xui.Trader.TraderWindowGroup = null;
		base.xui.playerUI.entityPlayer.OverrideFOV = -1f;
	}

	// Token: 0x06007494 RID: 29844 RVA: 0x002F6F64 File Offset: 0x002F5164
	public void RefreshTraderItems()
	{
		this.TraderWindow.CompletedTransaction = true;
		this.TraderWindow.RefreshTraderItems();
	}

	// Token: 0x06007495 RID: 29845 RVA: 0x002F6F7D File Offset: 0x002F517D
	public void RefreshTraderWindow()
	{
		this.TraderWindow.RefreshBindings(false);
	}

	// Token: 0x040058B8 RID: 22712
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeader;

	// Token: 0x040058B9 RID: 22713
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TraderWindow TraderWindow;

	// Token: 0x040058BA RID: 22714
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ServiceInfoWindow ServiceInfoWindow;

	// Token: 0x040058BB RID: 22715
	public static string ID = "trader";

	// Token: 0x040058BC RID: 22716
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x040058BD RID: 22717
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;
}
