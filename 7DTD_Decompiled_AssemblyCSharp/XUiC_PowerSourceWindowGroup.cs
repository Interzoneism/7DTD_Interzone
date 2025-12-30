using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D8D RID: 3469
[Preserve]
public class XUiC_PowerSourceWindowGroup : XUiController
{
	// Token: 0x06006C90 RID: 27792 RVA: 0x002C5D10 File Offset: 0x002C3F10
	public override void Init()
	{
		base.Init();
		XUiController childByType = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		if (childByType != null)
		{
			this.nonPagingHeader = (XUiC_WindowNonPagingHeader)childByType;
		}
		childByType = base.GetChildByType<XUiC_PowerSourceStats>();
		if (childByType != null)
		{
			this.GeneratorStats = (XUiC_PowerSourceStats)childByType;
			this.GeneratorStats.Owner = this;
		}
		childByType = base.GetChildByType<XUiC_PowerSourceSlots>();
		if (childByType != null)
		{
			this.PowerSourceSlots = (XUiC_PowerSourceSlots)childByType;
			this.PowerSourceSlots.Owner = this;
		}
	}

	// Token: 0x17000AED RID: 2797
	// (get) Token: 0x06006C91 RID: 27793 RVA: 0x002C5D7D File Offset: 0x002C3F7D
	// (set) Token: 0x06006C92 RID: 27794 RVA: 0x002C5D85 File Offset: 0x002C3F85
	public TileEntityPowerSource TileEntity
	{
		get
		{
			return this.tileEntity;
		}
		set
		{
			this.tileEntity = value;
			this.GeneratorStats.TileEntity = this.tileEntity;
			this.PowerSourceSlots.TileEntity = this.tileEntity;
		}
	}

	// Token: 0x06006C93 RID: 27795 RVA: 0x002C5DB0 File Offset: 0x002C3FB0
	public void SetOn(bool isOn)
	{
		if (this.PowerSourceSlots != null && this.PowerSourceSlots.ViewComponent.IsVisible)
		{
			this.PowerSourceSlots.SetOn(isOn);
		}
	}

	// Token: 0x06006C94 RID: 27796 RVA: 0x002C5DD8 File Offset: 0x002C3FD8
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

	// Token: 0x06006C95 RID: 27797 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool AlwaysUpdate()
	{
		return false;
	}

	// Token: 0x06006C96 RID: 27798 RVA: 0x002C5EB8 File Offset: 0x002C40B8
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
			string header = "";
			EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
			switch (this.TileEntity.PowerItemType)
			{
			case PowerItem.PowerItemTypes.Generator:
				header = Localization.Get("generatorbank", false);
				break;
			case PowerItem.PowerItemTypes.SolarPanel:
				header = Localization.Get("solarbank", false);
				break;
			case PowerItem.PowerItemTypes.BatteryBank:
				header = Localization.Get("batterybank", false);
				break;
			}
			this.nonPagingHeader.SetHeader(header);
		}
		base.xui.RecenterWindowGroup(this.windowGroup, false);
		for (int i = 0; i < this.children.Count; i++)
		{
			this.children[i].OnOpen();
		}
		if (this.PowerSourceSlots != null && this.TileEntity != null)
		{
			this.PowerSourceSlots.OnOpen();
		}
		Manager.BroadcastPlayByLocalPlayer(this.TileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f, "open_vending");
		this.IsDirty = true;
		this.TileEntity.Destroyed += this.TileEntity_Destroyed;
	}

	// Token: 0x06006C97 RID: 27799 RVA: 0x002C6010 File Offset: 0x002C4210
	public override void OnClose()
	{
		base.OnClose();
		this.wasReleased = false;
		this.activeKeyDown = false;
		Manager.BroadcastPlayByLocalPlayer(this.TileEntity.ToWorldPos().ToVector3() + Vector3.one * 0.5f, "close_vending");
		this.TileEntity.Destroyed -= this.TileEntity_Destroyed;
	}

	// Token: 0x06006C98 RID: 27800 RVA: 0x002C607C File Offset: 0x002C427C
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_Destroyed(ITileEntity te)
	{
		if (this.TileEntity == te)
		{
			if (GameManager.Instance != null)
			{
				base.xui.playerUI.windowManager.Close("powersource");
				return;
			}
		}
		else
		{
			te.Destroyed -= this.TileEntity_Destroyed;
		}
	}

	// Token: 0x04005299 RID: 21145
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeader;

	// Token: 0x0400529A RID: 21146
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PowerSourceStats GeneratorStats;

	// Token: 0x0400529B RID: 21147
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_PowerSourceSlots PowerSourceSlots;

	// Token: 0x0400529C RID: 21148
	public static string ID = "powersource";

	// Token: 0x0400529D RID: 21149
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activeKeyDown;

	// Token: 0x0400529E RID: 21150
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasReleased;

	// Token: 0x0400529F RID: 21151
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPowerSource tileEntity;
}
