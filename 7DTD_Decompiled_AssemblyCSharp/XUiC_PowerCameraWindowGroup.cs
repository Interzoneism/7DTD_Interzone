using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D84 RID: 3460
[Preserve]
public class XUiC_PowerCameraWindowGroup : XUiController
{
	// Token: 0x06006C24 RID: 27684 RVA: 0x002C3720 File Offset: 0x002C1920
	public override void Init()
	{
		base.Init();
		XUiController childByType = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		if (childByType != null)
		{
			this.nonPagingHeader = (XUiC_WindowNonPagingHeader)childByType;
		}
		childByType = base.GetChildByType<XUiC_CameraWindow>();
		if (childByType != null)
		{
			this.cameraWindow = (XUiC_CameraWindow)childByType;
		}
	}

	// Token: 0x17000ADF RID: 2783
	// (get) Token: 0x06006C25 RID: 27685 RVA: 0x002C375F File Offset: 0x002C195F
	// (set) Token: 0x06006C26 RID: 27686 RVA: 0x002C3767 File Offset: 0x002C1967
	public TileEntityPowered TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.cameraWindow.TileEntity = this.tileEntity;
		}
	}

	// Token: 0x06006C27 RID: 27687 RVA: 0x002C3784 File Offset: 0x002C1984
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

	// Token: 0x06006C28 RID: 27688 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AlwaysUpdate()
	{
		return false;
	}

	// Token: 0x06006C29 RID: 27689 RVA: 0x002C3864 File Offset: 0x002C1A64
	public override void OnOpen()
	{
		base.OnOpen();
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
		this.IsDirty = true;
		base.xui.playerUI.CursorController.Locked = true;
	}

	// Token: 0x06006C2A RID: 27690 RVA: 0x002C3948 File Offset: 0x002C1B48
	public override void OnClose()
	{
		base.OnClose();
		bool flag = true;
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		if (entityPlayer.IsDead() || Vector3.Distance(entityPlayer.position, this.TileEntity.ToWorldPos().ToVector3()) > Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance)
		{
			flag = false;
		}
		this.wasReleased = false;
		this.activeKeyDown = false;
		if (flag && base.xui.playerUI.windowManager.HasWindow(XUiC_CameraWindow.lastWindowGroup))
		{
			if (XUiC_CameraWindow.lastWindowGroup == "powerrangedtrap")
			{
				((XUiC_PowerRangedTrapWindowGroup)((XUiWindowGroup)base.xui.playerUI.windowManager.GetWindow(XUiC_CameraWindow.lastWindowGroup)).Controller).TileEntity = (TileEntityPoweredRangedTrap)this.TileEntity;
			}
			else if (XUiC_CameraWindow.lastWindowGroup == "powertrigger")
			{
				((XUiC_PowerTriggerWindowGroup)((XUiWindowGroup)base.xui.playerUI.windowManager.GetWindow(XUiC_CameraWindow.lastWindowGroup)).Controller).TileEntity = (TileEntityPoweredTrigger)this.TileEntity;
			}
			else
			{
				((XUiC_PoweredGenericWindowGroup)((XUiWindowGroup)base.xui.playerUI.windowManager.GetWindow(XUiC_CameraWindow.lastWindowGroup)).Controller).TileEntity = this.TileEntity;
			}
			base.xui.playerUI.windowManager.Open(XUiC_CameraWindow.lastWindowGroup, true, false, true);
		}
		base.xui.playerUI.CursorController.Locked = false;
	}

	// Token: 0x04005249 RID: 21065
	public bool UseEdgeDetection = true;

	// Token: 0x0400524A RID: 21066
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeader;

	// Token: 0x0400524B RID: 21067
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CameraWindow cameraWindow;

	// Token: 0x0400524C RID: 21068
	public static string ID = "powercamera";

	// Token: 0x0400524D RID: 21069
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x0400524E RID: 21070
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;

	// Token: 0x0400524F RID: 21071
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPowered tileEntity;
}
