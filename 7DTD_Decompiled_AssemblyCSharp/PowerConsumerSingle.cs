using System;

// Token: 0x0200083F RID: 2111
public class PowerConsumerSingle : PowerItem
{
	// Token: 0x1700063B RID: 1595
	// (get) Token: 0x06003C9A RID: 15514 RVA: 0x000282C0 File Offset: 0x000264C0
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.ConsumerToggle;
		}
	}

	// Token: 0x06003C9B RID: 15515 RVA: 0x00185E84 File Offset: 0x00184084
	public override void HandlePowerUpdate(bool isOn)
	{
		bool isPowered = this.isPowered;
		if (isPowered && this.lastActivate != isPowered && this.TileEntity != null)
		{
			this.TileEntity.ActivateOnce();
		}
		this.lastActivate = isPowered;
		if (this.PowerChildren())
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				this.Children[i].HandlePowerUpdate(isOn);
			}
		}
	}

	// Token: 0x06003C9C RID: 15516 RVA: 0x00185EF0 File Offset: 0x001840F0
	public override void SetValuesFromBlock()
	{
		base.SetValuesFromBlock();
		Block block = Block.list[(int)this.BlockID];
		if (block.Properties.Values.ContainsKey("RequiredPower"))
		{
			this.RequiredPower = ushort.Parse(block.Properties.Values["RequiredPower"]);
		}
	}

	// Token: 0x04003105 RID: 12549
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastActivate;
}
