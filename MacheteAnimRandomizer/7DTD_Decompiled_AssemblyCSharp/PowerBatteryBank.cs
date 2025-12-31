using System;
using UnityEngine;

// Token: 0x0200083D RID: 2109
public class PowerBatteryBank : PowerSource
{
	// Token: 0x17000636 RID: 1590
	// (get) Token: 0x06003C86 RID: 15494 RVA: 0x000583BD File Offset: 0x000565BD
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.BatteryBank;
		}
	}

	// Token: 0x17000637 RID: 1591
	// (get) Token: 0x06003C87 RID: 15495 RVA: 0x00185713 File Offset: 0x00183913
	public override string OnSound
	{
		get
		{
			return "batterybank_start";
		}
	}

	// Token: 0x17000638 RID: 1592
	// (get) Token: 0x06003C88 RID: 15496 RVA: 0x0018571A File Offset: 0x0018391A
	public override string OffSound
	{
		get
		{
			return "batterybank_stop";
		}
	}

	// Token: 0x06003C8A RID: 15498 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool CanParent(PowerItem parent)
	{
		return true;
	}

	// Token: 0x17000639 RID: 1593
	// (get) Token: 0x06003C8B RID: 15499 RVA: 0x00185729 File Offset: 0x00183929
	public override bool IsPowered
	{
		get
		{
			return this.isOn || this.isPowered;
		}
	}

	// Token: 0x1700063A RID: 1594
	// (get) Token: 0x06003C8C RID: 15500 RVA: 0x0018573C File Offset: 0x0018393C
	public bool ParentPowering
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.Parent == null)
			{
				return false;
			}
			if (this.Parent is PowerSolarPanel)
			{
				PowerSolarPanel powerSolarPanel = this.Parent as PowerSolarPanel;
				return powerSolarPanel.HasLight && powerSolarPanel.IsOn;
			}
			if (this.Parent is PowerSource)
			{
				return (this.Parent as PowerSource).IsOn;
			}
			if (this.Parent is PowerTrigger)
			{
				return this.Parent.IsPowered && (this.Parent as PowerTrigger).IsActive;
			}
			return this.Parent.IsPowered;
		}
	}

	// Token: 0x06003C8D RID: 15501 RVA: 0x001857D3 File Offset: 0x001839D3
	public override void Update()
	{
		if (this.Parent != null && this.LastPowerReceived > 0)
		{
			if (this.LastInputAmount > 0 && base.IsOn)
			{
				this.AddPowerToBatteries((int)this.LastInputAmount);
			}
			return;
		}
		base.Update();
	}

	// Token: 0x06003C8E RID: 15502 RVA: 0x0018580C File Offset: 0x00183A0C
	public override void HandleSendPower()
	{
		if (base.IsOn && !this.ParentPowering)
		{
			if (this.CurrentPower < this.MaxPower)
			{
				this.TickPowerGeneration();
			}
			else if (this.CurrentPower > this.MaxPower)
			{
				this.CurrentPower = this.MaxPower;
			}
			if (this.CurrentPower <= 0)
			{
				this.CurrentPower = 0;
				if (this.isPowered)
				{
					this.HandleDisconnect();
					this.hasChangesLocal = true;
				}
			}
			else
			{
				this.isPowered = true;
			}
			if (this.hasChangesLocal)
			{
				this.LastPowerUsed = 0;
				ushort num = (ushort)Mathf.Min((int)this.MaxOutput, (int)this.CurrentPower);
				ushort num2 = num;
				World world = GameManager.Instance.World;
				for (int i = 0; i < this.Children.Count; i++)
				{
					num = num2;
					this.Children[i].HandlePowerReceived(ref num2);
					this.LastPowerUsed += num - num2;
				}
			}
			this.CurrentPower -= (ushort)Mathf.Min((int)this.CurrentPower, (int)this.LastPowerUsed);
		}
	}

	// Token: 0x06003C8F RID: 15503 RVA: 0x0018591C File Offset: 0x00183B1C
	public override void HandlePowerReceived(ref ushort power)
	{
		this.LastPowerUsed = 0;
		if (this.LastPowerReceived != power)
		{
			this.LastPowerReceived = power;
			this.hasChangesLocal = true;
			for (int i = 0; i < this.Children.Count; i++)
			{
				this.Children[i].HandleDisconnect();
			}
		}
		if (power <= 0)
		{
			return;
		}
		if (base.IsOn && power > 0)
		{
			ushort power2 = (ushort)Mathf.Min((int)this.InputPerTick, (int)power);
			this.AddPowerToBatteries((int)power2);
			power -= this.LastInputAmount;
		}
		if (this.PowerChildren())
		{
			for (int j = 0; j < this.Children.Count; j++)
			{
				this.Children[j].HandlePowerReceived(ref power);
				if (power <= 0)
				{
					return;
				}
			}
		}
	}

	// Token: 0x06003C90 RID: 15504 RVA: 0x001859DC File Offset: 0x00183BDC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AddPowerToBatteries(int power)
	{
		int num = power;
		int b = power / (int)this.InputPerTick * (int)this.ChargePerInput;
		for (int i = this.Stacks.Length - 1; i >= 0; i--)
		{
			if (!this.Stacks[i].IsEmpty())
			{
				int num2 = (int)this.Stacks[i].itemValue.UseTimes;
				if (num2 > 0)
				{
					ushort num3 = (ushort)Mathf.Min(num2, b);
					num -= (int)(num3 * this.InputPerTick);
					this.Stacks[i].itemValue.UseTimes -= (float)num3;
				}
				if (num == 0)
				{
					break;
				}
			}
		}
		int num4 = power - num;
		if (this.LastInputAmount != (ushort)num4)
		{
			base.SendHasLocalChangesToRoot();
			this.LastInputAmount = (ushort)num4;
		}
	}

	// Token: 0x06003C91 RID: 15505 RVA: 0x00185A8C File Offset: 0x00183C8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void TickPowerGeneration()
	{
		base.TickPowerGeneration();
		ushort num = this.MaxPower - this.CurrentPower;
		ushort num2 = num / this.OutputPerCharge;
		if (num >= this.OutputPerCharge)
		{
			for (int i = 0; i < this.Stacks.Length; i++)
			{
				int num3 = (int)Mathf.Min((float)this.Stacks[i].itemValue.MaxUseTimes - this.Stacks[i].itemValue.UseTimes, (float)num2);
				if (num3 > 0)
				{
					this.Stacks[i].itemValue.UseTimes += (float)num3;
					this.CurrentPower += (ushort)(num3 * (int)this.OutputPerCharge);
					return;
				}
			}
		}
	}

	// Token: 0x06003C92 RID: 15506 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool PowerChildren()
	{
		return true;
	}

	// Token: 0x06003C93 RID: 15507 RVA: 0x00185B38 File Offset: 0x00183D38
	public override void HandlePowerUpdate(bool isOn)
	{
		if (this.Parent != null && this.LastPowerReceived > 0 && this.PowerChildren())
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				this.Children[i].HandlePowerUpdate(isOn);
			}
		}
	}

	// Token: 0x06003C94 RID: 15508 RVA: 0x00185B88 File Offset: 0x00183D88
	public override void HandleDisconnect()
	{
		if (this.isPowered)
		{
			this.IsPoweredChanged(false);
		}
		this.isPowered = false;
		this.HandlePowerUpdate(false);
		for (int i = 0; i < this.Children.Count; i++)
		{
			this.Children[i].HandleDisconnect();
		}
		this.LastInputAmount = 0;
		this.LastPowerReceived = 0;
		if (this.TileEntity != null)
		{
			this.TileEntity.SetModified();
		}
	}

	// Token: 0x06003C95 RID: 15509 RVA: 0x00185BFC File Offset: 0x00183DFC
	public override void SetValuesFromBlock()
	{
		base.SetValuesFromBlock();
		Block block = Block.list[(int)this.BlockID];
		if (block.Properties.Values.ContainsKey("InputPerTick"))
		{
			this.InputPerTick = ushort.Parse(block.Properties.Values["InputPerTick"]);
		}
		if (block.Properties.Values.ContainsKey("ChargePerInput"))
		{
			this.ChargePerInput = ushort.Parse(block.Properties.Values["ChargePerInput"]);
		}
		if (block.Properties.Values.ContainsKey("OutputPerCharge"))
		{
			this.OutputPerCharge = ushort.Parse(block.Properties.Values["OutputPerCharge"]);
		}
		if (block.Properties.Values.ContainsKey("MaxPower"))
		{
			this.MaxPower = ushort.Parse(block.Properties.Values["MaxPower"]);
		}
	}

	// Token: 0x040030FD RID: 12541
	public ushort LastInputAmount;

	// Token: 0x040030FE RID: 12542
	public ushort LastPowerReceived;

	// Token: 0x040030FF RID: 12543
	public ushort InputPerTick;

	// Token: 0x04003100 RID: 12544
	public ushort ChargePerInput;

	// Token: 0x04003101 RID: 12545
	public ushort OutputPerCharge;
}
