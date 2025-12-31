using System;
using Audio;

// Token: 0x0200084F RID: 2127
public class PowerPressurePlate : PowerTrigger
{
	// Token: 0x1700065A RID: 1626
	// (get) Token: 0x06003D2D RID: 15661 RVA: 0x0018853A File Offset: 0x0018673A
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.PressurePlate;
		}
	}

	// Token: 0x1700065B RID: 1627
	// (get) Token: 0x06003D2E RID: 15662 RVA: 0x0018853E File Offset: 0x0018673E
	// (set) Token: 0x06003D2F RID: 15663 RVA: 0x00188546 File Offset: 0x00186746
	public bool Pressed
	{
		get
		{
			return this.pressed;
		}
		set
		{
			this.pressed = value;
			if (this.pressed && !this.lastPressed)
			{
				Manager.BroadcastPlay(this.Position.ToVector3(), "pressureplate_down", 0f);
			}
			this.lastPressed = this.pressed;
		}
	}

	// Token: 0x06003D30 RID: 15664 RVA: 0x00188588 File Offset: 0x00186788
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CheckForActiveChange()
	{
		base.CheckForActiveChange();
		if (!this.pressed && this.lastPressed)
		{
			Manager.BroadcastPlay(this.Position.ToVector3(), "pressureplate_up", 0f);
			if (this.powerTime == 0f)
			{
				this.isActive = false;
				this.HandleDisconnectChildren();
				base.SendHasLocalChangesToRoot();
				this.powerTime = -1f;
			}
		}
	}

	// Token: 0x06003D31 RID: 15665 RVA: 0x001885F0 File Offset: 0x001867F0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleSoundDisable()
	{
		base.HandleSoundDisable();
		this.lastPressed = this.pressed;
		this.pressed = false;
	}

	// Token: 0x0400317F RID: 12671
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool pressed;

	// Token: 0x04003180 RID: 12672
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lastPressed;
}
