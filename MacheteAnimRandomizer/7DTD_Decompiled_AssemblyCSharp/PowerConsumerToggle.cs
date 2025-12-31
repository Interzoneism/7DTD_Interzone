using System;
using System.IO;
using UnityEngine;

// Token: 0x02000840 RID: 2112
public class PowerConsumerToggle : PowerConsumer
{
	// Token: 0x1700063C RID: 1596
	// (get) Token: 0x06003C9E RID: 15518 RVA: 0x000282C0 File Offset: 0x000264C0
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.ConsumerToggle;
		}
	}

	// Token: 0x1700063D RID: 1597
	// (get) Token: 0x06003C9F RID: 15519 RVA: 0x00185F4F File Offset: 0x0018414F
	// (set) Token: 0x06003CA0 RID: 15520 RVA: 0x00185F57 File Offset: 0x00184157
	public bool IsToggled
	{
		get
		{
			return this.isToggled;
		}
		set
		{
			this.isToggled = value;
			base.SendHasLocalChangesToRoot();
		}
	}

	// Token: 0x06003CA1 RID: 15521 RVA: 0x00185F68 File Offset: 0x00184168
	public override void HandlePowerUpdate(bool isOn)
	{
		bool flag = this.isPowered && isOn && this.isToggled;
		if (this.TileEntity != null)
		{
			this.TileEntity.Activate(flag);
			if (flag && this.lastActivate != flag)
			{
				this.TileEntity.ActivateOnce();
			}
		}
		this.lastActivate = flag;
		if (this.PowerChildren())
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				this.Children[i].HandlePowerUpdate(isOn);
			}
		}
	}

	// Token: 0x06003CA2 RID: 15522 RVA: 0x00185FF0 File Offset: 0x001841F0
	public override void HandlePowerReceived(ref ushort power)
	{
		ushort num = (ushort)Mathf.Min((int)this.RequiredPower, (int)power);
		bool flag = num == this.RequiredPower;
		if (flag != this.isPowered)
		{
			this.isPowered = flag;
			this.IsPoweredChanged(flag);
			if (this.TileEntity != null)
			{
				this.TileEntity.SetModified();
			}
		}
		power -= num;
		if (power <= 0)
		{
			return;
		}
		if (this.PowerChildren())
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				this.Children[i].HandlePowerReceived(ref power);
				if (power <= 0)
				{
					return;
				}
			}
		}
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x00186083 File Offset: 0x00184283
	public override void read(BinaryReader _br, byte _version)
	{
		base.read(_br, _version);
		this.isToggled = _br.ReadBoolean();
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x00186099 File Offset: 0x00184299
	public override void write(BinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.isToggled);
	}

	// Token: 0x04003106 RID: 12550
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isToggled = true;
}
