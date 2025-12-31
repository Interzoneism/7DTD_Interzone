using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D8F RID: 3471
[Preserve]
public class XUiC_PowerTriggerWindowGroup : XUiController
{
	// Token: 0x06006CAC RID: 27820 RVA: 0x002C6D60 File Offset: 0x002C4F60
	public override void Init()
	{
		base.Init();
		XUiController childByType = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		if (childByType != null)
		{
			this.nonPagingHeader = (XUiC_WindowNonPagingHeader)childByType;
		}
		childByType = base.GetChildByType<XUiC_PowerTriggerOptions>();
		if (childByType != null)
		{
			this.triggerWindow = (XUiC_PowerTriggerOptions)childByType;
			this.triggerWindow.Owner = this;
		}
		childByType = base.GetChildByType<XUiC_CameraWindow>();
		if (childByType != null)
		{
			this.cameraWindow = (XUiC_CameraWindow)childByType;
		}
	}

	// Token: 0x17000AF0 RID: 2800
	// (get) Token: 0x06006CAD RID: 27821 RVA: 0x002C6DC1 File Offset: 0x002C4FC1
	// (set) Token: 0x06006CAE RID: 27822 RVA: 0x002C6DC9 File Offset: 0x002C4FC9
	public TileEntityPoweredTrigger TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.triggerWindow.TileEntity = this.tileEntity;
			this.cameraWindow.TileEntity = this.tileEntity;
		}
	}

	// Token: 0x06006CAF RID: 27823 RVA: 0x002C6DF4 File Offset: 0x002C4FF4
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

	// Token: 0x06006CB0 RID: 27824 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AlwaysUpdate()
	{
		return false;
	}

	// Token: 0x06006CB1 RID: 27825 RVA: 0x002C6ED4 File Offset: 0x002C50D4
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
			string header = Localization.Get("xuiTrigger", false);
			this.nonPagingHeader.SetHeader(header);
		}
		base.xui.RecenterWindowGroup(this.windowGroup, false);
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].OnOpen();
		}
		Manager.BroadcastPlayByLocalPlayer(this.TileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f, "open_vending");
		this.IsDirty = true;
		this.TileEntity.Destroyed += this.TileEntity_Destroyed;
	}

	// Token: 0x06006CB2 RID: 27826 RVA: 0x002C6FC4 File Offset: 0x002C51C4
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

	// Token: 0x06006CB3 RID: 27827 RVA: 0x002C7038 File Offset: 0x002C5238
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_Destroyed(ITileEntity te)
	{
		if (this.TileEntity == te)
		{
			if (GameManager.Instance != null)
			{
				base.xui.playerUI.windowManager.Close("powertrigger");
				base.xui.playerUI.windowManager.Close("powercamera");
				return;
			}
		}
		else
		{
			te.Destroyed -= this.TileEntity_Destroyed;
		}
	}

	// Token: 0x040052B8 RID: 21176
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeader;

	// Token: 0x040052B9 RID: 21177
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PowerTriggerOptions triggerWindow;

	// Token: 0x040052BA RID: 21178
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CameraWindow cameraWindow;

	// Token: 0x040052BB RID: 21179
	public static string ID = "powertrigger";

	// Token: 0x040052BC RID: 21180
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x040052BD RID: 21181
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;

	// Token: 0x040052BE RID: 21182
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPoweredTrigger tileEntity;
}
