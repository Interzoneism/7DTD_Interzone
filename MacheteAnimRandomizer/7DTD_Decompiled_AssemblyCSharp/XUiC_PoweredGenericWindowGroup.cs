using System;
using UnityEngine.Scripting;

// Token: 0x02000D85 RID: 3461
[Preserve]
public class XUiC_PoweredGenericWindowGroup : XUiController
{
	// Token: 0x17000AE0 RID: 2784
	// (get) Token: 0x06006C2D RID: 27693 RVA: 0x002C3AF3 File Offset: 0x002C1CF3
	// (set) Token: 0x06006C2E RID: 27694 RVA: 0x002C3AFB File Offset: 0x002C1CFB
	public TileEntityPowered TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.setupWindowTileEntities();
		}
	}

	// Token: 0x06006C2F RID: 27695 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void setupWindowTileEntities()
	{
	}

	// Token: 0x06006C30 RID: 27696 RVA: 0x002C3B0C File Offset: 0x002C1D0C
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

	// Token: 0x06006C31 RID: 27697 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AlwaysUpdate()
	{
		return false;
	}

	// Token: 0x06006C32 RID: 27698 RVA: 0x002C3BEC File Offset: 0x002C1DEC
	public override void OnOpen()
	{
		base.OnOpen();
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.OnOpen();
			base.ViewComponent.IsVisible = true;
		}
		base.xui.RecenterWindowGroup(this.windowGroup, false);
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].OnOpen();
		}
		this.IsDirty = true;
	}

	// Token: 0x06006C33 RID: 27699 RVA: 0x002C3C6C File Offset: 0x002C1E6C
	public override void OnClose()
	{
		base.OnClose();
		this.wasReleased = false;
		this.activeKeyDown = false;
		if (this.tileEntity != null && !XUiC_CameraWindow.hackyIsOpeningMaximizedWindow)
		{
			GameManager.Instance.TEUnlockServer(this.tileEntity.GetClrIdx(), this.tileEntity.ToWorldPos(), this.tileEntity.entityId, true);
			this.tileEntity = null;
		}
	}

	// Token: 0x04005250 RID: 21072
	[PublicizedFrom(EAccessModifier.Protected)]
	public TileEntityPowered tileEntity;

	// Token: 0x04005251 RID: 21073
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x04005252 RID: 21074
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;
}
