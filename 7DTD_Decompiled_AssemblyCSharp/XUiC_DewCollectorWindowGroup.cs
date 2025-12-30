using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C8D RID: 3213
[Preserve]
public class XUiC_DewCollectorWindowGroup : XUiController
{
	// Token: 0x06006321 RID: 25377 RVA: 0x00283FEB File Offset: 0x002821EB
	public override void Init()
	{
		base.Init();
		this.dewCatcherWindow = base.GetChildByType<XUiC_DewCollectorWindow>();
		this.dewCollectorModGrid = base.GetChildByType<XUiC_DewCollectorModGrid>();
		this.nonPagingHeaderWindow = base.GetChildByType<XUiC_WindowNonPagingHeader>();
	}

	// Token: 0x06006322 RID: 25378 RVA: 0x00284017 File Offset: 0x00282217
	public void SetTileEntity(TileEntityDewCollector _te)
	{
		this.te = _te;
		this.dewCatcherWindow.SetTileEntity(_te);
		this.dewCollectorModGrid.SetTileEntity(_te);
		this.lootingHeader = Localization.Get("xuiDewCollector", false);
	}

	// Token: 0x06006323 RID: 25379 RVA: 0x0028404C File Offset: 0x0028224C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OpenContainer()
	{
		base.OnOpen();
		base.xui.playerUI.windowManager.OpenIfNotOpen("backpack", false, false, true);
		this.dewCatcherWindow.ViewComponent.UiTransform.gameObject.SetActive(true);
		this.dewCatcherWindow.OpenContainer();
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader(this.lootingHeader);
		}
		this.dewCatcherWindow.ViewComponent.IsVisible = true;
		if (this.windowGroup.UseStackPanelAlignment)
		{
			base.xui.RecenterWindowGroup(this.windowGroup, false);
		}
	}

	// Token: 0x06006324 RID: 25380 RVA: 0x002840EC File Offset: 0x002822EC
	public override void OnOpen()
	{
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (this.nonPagingHeaderWindow != null)
		{
			this.nonPagingHeaderWindow.SetHeader("LOOTING");
		}
		this.OpenContainer();
		BlockDewCollector blockDewCollector = this.te.blockValue.Block as BlockDewCollector;
		if (blockDewCollector != null)
		{
			string openSound = blockDewCollector.OpenSound;
			Manager.BroadcastPlayByLocalPlayer(this.te.ToWorldPos().ToVector3() + Vector3.one * 0.5f, openSound);
		}
	}

	// Token: 0x06006325 RID: 25381 RVA: 0x00284178 File Offset: 0x00282378
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen("backpack");
		this.te.ToWorldPos();
		if (this.te.blockValue.Block != null)
		{
			BlockDewCollector blockDewCollector = this.te.blockValue.Block as BlockDewCollector;
			if (blockDewCollector != null)
			{
				string closeSound = blockDewCollector.CloseSound;
				Manager.BroadcastPlayByLocalPlayer(this.te.ToWorldPos().ToVector3() + Vector3.one * 0.5f, closeSound);
			}
		}
	}

	// Token: 0x06006326 RID: 25382 RVA: 0x00284216 File Offset: 0x00282416
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_DewCollectorWindowGroup GetInstance(XUi _xuiInstance = null)
	{
		if (_xuiInstance == null)
		{
			_xuiInstance = LocalPlayerUI.GetUIForPrimaryPlayer().xui;
		}
		return (XUiC_DewCollectorWindowGroup)_xuiInstance.FindWindowGroupByName(XUiC_DewCollectorWindowGroup.ID);
	}

	// Token: 0x06006327 RID: 25383 RVA: 0x00284237 File Offset: 0x00282437
	public static Vector3i GetTeBlockPos(XUi _xuiInstance = null)
	{
		TileEntityDewCollector tileEntityDewCollector = XUiC_DewCollectorWindowGroup.GetInstance(_xuiInstance).te;
		if (tileEntityDewCollector == null)
		{
			return Vector3i.zero;
		}
		return tileEntityDewCollector.ToWorldPos();
	}

	// Token: 0x06006328 RID: 25384 RVA: 0x00284254 File Offset: 0x00282454
	public static void CloseIfOpenAtPos(Vector3i _blockPos, XUi _xuiInstance = null)
	{
		GUIWindowManager windowManager = XUiC_DewCollectorWindowGroup.GetInstance(_xuiInstance).xui.playerUI.windowManager;
		if (windowManager.IsWindowOpen(XUiC_DewCollectorWindowGroup.ID) && XUiC_DewCollectorWindowGroup.GetTeBlockPos(null) == _blockPos)
		{
			windowManager.Close(XUiC_DewCollectorWindowGroup.ID);
		}
	}

	// Token: 0x04004AB2 RID: 19122
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DewCollectorWindow dewCatcherWindow;

	// Token: 0x04004AB3 RID: 19123
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DewCollectorModGrid dewCollectorModGrid;

	// Token: 0x04004AB4 RID: 19124
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label headerName;

	// Token: 0x04004AB5 RID: 19125
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeaderWindow;

	// Token: 0x04004AB6 RID: 19126
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityDewCollector te;

	// Token: 0x04004AB7 RID: 19127
	[PublicizedFrom(EAccessModifier.Private)]
	public string lootingHeader;

	// Token: 0x04004AB8 RID: 19128
	public static string ID = "dewcollector";

	// Token: 0x04004AB9 RID: 19129
	[PublicizedFrom(EAccessModifier.Private)]
	public float totalOpenTime;
}
