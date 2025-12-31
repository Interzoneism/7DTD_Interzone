using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D89 RID: 3465
[Preserve]
public class XUiC_PowerRangedTrapWindowGroup : XUiController
{
	// Token: 0x06006C5F RID: 27743 RVA: 0x002C4970 File Offset: 0x002C2B70
	public override void Init()
	{
		base.Init();
		XUiController xuiController = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		if (xuiController != null)
		{
			this.nonPagingHeader = (XUiC_WindowNonPagingHeader)xuiController;
		}
		xuiController = base.GetChildByType<XUiC_PowerRangedAmmoSlots>();
		if (xuiController != null)
		{
			this.ammoWindow = (XUiC_PowerRangedAmmoSlots)xuiController;
			this.ammoWindow.Owner = this;
		}
		xuiController = base.GetChildByType<XUiC_PowerRangedTrapOptions>();
		if (xuiController != null)
		{
			this.optionsWindow = (XUiC_PowerRangedTrapOptions)xuiController;
			this.optionsWindow.Owner = this;
		}
		xuiController = base.GetChildById("windowPowerCameraControlPreview");
		if (xuiController != null)
		{
			this.cameraWindowPreview = (XUiC_CameraWindow)xuiController;
			this.cameraWindowPreview.Owner = this;
		}
	}

	// Token: 0x17000AE6 RID: 2790
	// (get) Token: 0x06006C60 RID: 27744 RVA: 0x002C4A04 File Offset: 0x002C2C04
	// (set) Token: 0x06006C61 RID: 27745 RVA: 0x002C4A0C File Offset: 0x002C2C0C
	public TileEntityPoweredRangedTrap TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.ammoWindow.TileEntity = this.tileEntity;
			if (this.tileEntity.PowerItemType == PowerItem.PowerItemTypes.RangedTrap)
			{
				this.optionsWindow.TileEntity = this.tileEntity;
			}
			this.cameraWindowPreview.TileEntity = this.tileEntity;
		}
	}

	// Token: 0x06006C62 RID: 27746 RVA: 0x002C4A64 File Offset: 0x002C2C64
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

	// Token: 0x06006C63 RID: 27747 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AlwaysUpdate()
	{
		return false;
	}

	// Token: 0x06006C64 RID: 27748 RVA: 0x002C4B44 File Offset: 0x002C2D44
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnOpen();
			base.ViewComponent.IsVisible = true;
		}
		if (this.nonPagingHeader != null)
		{
			World world = GameManager.Instance.World;
			string localizedBlockName = this.tileEntity.GetChunk().GetBlock(this.tileEntity.localChunkPos).Block.GetLocalizedBlockName();
			this.nonPagingHeader.SetHeader(localizedBlockName);
		}
		base.xui.RecenterWindowGroup(this.windowGroup, false);
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].OnOpen();
		}
		if (!this.tileEntity.ShowTargeting)
		{
			this.optionsWindow.ViewComponent.IsVisible = false;
		}
		Manager.BroadcastPlayByLocalPlayer(this.TileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f, "open_vending");
		this.IsDirty = true;
		this.TileEntity.Destroyed += this.TileEntity_Destroyed;
	}

	// Token: 0x06006C65 RID: 27749 RVA: 0x002C4C74 File Offset: 0x002C2E74
	public override void OnClose()
	{
		base.OnClose();
		this.wasReleased = false;
		this.activeKeyDown = false;
		Vector3 position = this.TileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f;
		if (!XUiC_CameraWindow.hackyIsOpeningMaximizedWindow)
		{
			Manager.BroadcastPlayByLocalPlayer(position, "close_vending");
		}
		this.TileEntity.Destroyed -= this.TileEntity_Destroyed;
	}

	// Token: 0x06006C66 RID: 27750 RVA: 0x002C4CE8 File Offset: 0x002C2EE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_Destroyed(ITileEntity te)
	{
		if (this.TileEntity == te)
		{
			if (GameManager.Instance != null)
			{
				base.xui.playerUI.windowManager.Close("powerrangedtrap");
				base.xui.playerUI.windowManager.Close("powercamera");
				return;
			}
		}
		else
		{
			te.Destroyed -= this.TileEntity_Destroyed;
		}
	}

	// Token: 0x0400526D RID: 21101
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeader;

	// Token: 0x0400526E RID: 21102
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PowerRangedTrapOptions optionsWindow;

	// Token: 0x0400526F RID: 21103
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PowerRangedAmmoSlots ammoWindow;

	// Token: 0x04005270 RID: 21104
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CameraWindow cameraWindowPreview;

	// Token: 0x04005271 RID: 21105
	public static string ID = "powerrangedtrap";

	// Token: 0x04005272 RID: 21106
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04005273 RID: 21107
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;

	// Token: 0x04005274 RID: 21108
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPoweredRangedTrap tileEntity;
}
