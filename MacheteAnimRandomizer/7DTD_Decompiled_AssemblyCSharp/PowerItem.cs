using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000842 RID: 2114
public class PowerItem
{
	// Token: 0x17000641 RID: 1601
	// (get) Token: 0x06003CAF RID: 15535 RVA: 0x00186239 File Offset: 0x00184439
	public virtual bool IsPowered
	{
		get
		{
			return this.isPowered;
		}
	}

	// Token: 0x06003CB0 RID: 15536 RVA: 0x00186241 File Offset: 0x00184441
	public PowerItem()
	{
		this.Children = new List<PowerItem>();
	}

	// Token: 0x06003CB1 RID: 15537 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanParent(PowerItem newParent)
	{
		return true;
	}

	// Token: 0x17000642 RID: 1602
	// (get) Token: 0x06003CB2 RID: 15538 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual int InputCount
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x17000643 RID: 1603
	// (get) Token: 0x06003CB3 RID: 15539 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.Consumer;
		}
	}

	// Token: 0x06003CB4 RID: 15540 RVA: 0x00186266 File Offset: 0x00184466
	public virtual void AddTileEntity(TileEntityPowered tileEntityPowered)
	{
		if (this.TileEntity == null)
		{
			this.TileEntity = tileEntityPowered;
			this.TileEntity.CreateWireDataFromPowerItem();
		}
		this.TileEntity.MarkWireDirty();
	}

	// Token: 0x06003CB5 RID: 15541 RVA: 0x0018628D File Offset: 0x0018448D
	public void RemoveTileEntity(TileEntityPowered tileEntityPowered)
	{
		if (this.TileEntity == tileEntityPowered)
		{
			this.TileEntity = null;
		}
	}

	// Token: 0x06003CB6 RID: 15542 RVA: 0x0018629F File Offset: 0x0018449F
	public virtual PowerItem GetRoot()
	{
		if (this.Parent != null)
		{
			return this.Parent.GetRoot();
		}
		return this;
	}

	// Token: 0x06003CB7 RID: 15543 RVA: 0x001862B8 File Offset: 0x001844B8
	public virtual void read(BinaryReader _br, byte _version)
	{
		this.BlockID = _br.ReadUInt16();
		this.SetValuesFromBlock();
		this.Position = StreamUtils.ReadVector3i(_br);
		if (_br.ReadBoolean())
		{
			PowerManager.Instance.SetParent(this, PowerManager.Instance.GetPowerItemByWorldPos(StreamUtils.ReadVector3i(_br)));
		}
		int num = (int)_br.ReadByte();
		this.Children.Clear();
		for (int i = 0; i < num; i++)
		{
			PowerItem powerItem = PowerItem.CreateItem((PowerItem.PowerItemTypes)_br.ReadByte());
			powerItem.read(_br, _version);
			PowerManager.Instance.AddPowerNode(powerItem, this);
		}
	}

	// Token: 0x06003CB8 RID: 15544 RVA: 0x00186344 File Offset: 0x00184544
	public void RemoveSelfFromParent()
	{
		PowerManager.Instance.RemoveParent(this);
	}

	// Token: 0x06003CB9 RID: 15545 RVA: 0x00186354 File Offset: 0x00184554
	public virtual void write(BinaryWriter _bw)
	{
		_bw.Write(this.BlockID);
		StreamUtils.Write(_bw, this.Position);
		_bw.Write(this.Parent != null);
		if (this.Parent != null)
		{
			StreamUtils.Write(_bw, this.Parent.Position);
		}
		_bw.Write((byte)this.Children.Count);
		for (int i = 0; i < this.Children.Count; i++)
		{
			_bw.Write((byte)this.Children[i].PowerItemType);
			this.Children[i].write(_bw);
		}
	}

	// Token: 0x06003CBA RID: 15546 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool PowerChildren()
	{
		return true;
	}

	// Token: 0x06003CBB RID: 15547 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void IsPoweredChanged(bool newPowered)
	{
	}

	// Token: 0x06003CBC RID: 15548 RVA: 0x001863F4 File Offset: 0x001845F4
	public virtual void HandlePowerReceived(ref ushort power)
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

	// Token: 0x06003CBD RID: 15549 RVA: 0x00186488 File Offset: 0x00184688
	[PublicizedFrom(EAccessModifier.Internal)]
	public PowerItem GetChild(Vector3 childPosition)
	{
		Vector3i other = new Vector3i(childPosition);
		for (int i = 0; i < this.Children.Count; i++)
		{
			if (this.Children[i].Position == other)
			{
				return this.Children[i];
			}
		}
		return null;
	}

	// Token: 0x06003CBE RID: 15550 RVA: 0x001864DC File Offset: 0x001846DC
	[PublicizedFrom(EAccessModifier.Internal)]
	public bool HasChild(Vector3 child)
	{
		Vector3i other = new Vector3i(child);
		for (int i = 0; i < this.Children.Count; i++)
		{
			if (this.Children[i].Position == other)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003CBF RID: 15551 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void HandlePowerUpdate(bool isOn)
	{
	}

	// Token: 0x06003CC0 RID: 15552 RVA: 0x00186524 File Offset: 0x00184724
	public virtual void HandleDisconnect()
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
	}

	// Token: 0x06003CC1 RID: 15553 RVA: 0x00186578 File Offset: 0x00184778
	public static PowerItem CreateItem(PowerItem.PowerItemTypes itemType)
	{
		switch (itemType)
		{
		case PowerItem.PowerItemTypes.Consumer:
			return new PowerConsumer();
		case PowerItem.PowerItemTypes.ConsumerToggle:
			return new PowerConsumerToggle();
		case PowerItem.PowerItemTypes.Trigger:
			return new PowerTrigger();
		case PowerItem.PowerItemTypes.Timer:
			return new PowerTimerRelay();
		case PowerItem.PowerItemTypes.Generator:
			return new PowerGenerator();
		case PowerItem.PowerItemTypes.SolarPanel:
			return new PowerSolarPanel();
		case PowerItem.PowerItemTypes.BatteryBank:
			return new PowerBatteryBank();
		case PowerItem.PowerItemTypes.RangedTrap:
			return new PowerRangedTrap();
		case PowerItem.PowerItemTypes.ElectricWireRelay:
			return new PowerElectricWireRelay();
		case PowerItem.PowerItemTypes.TripWireRelay:
			return new PowerTripWireRelay();
		case PowerItem.PowerItemTypes.PressurePlate:
			return new PowerPressurePlate();
		default:
			return new PowerItem();
		}
	}

	// Token: 0x06003CC2 RID: 15554 RVA: 0x00186604 File Offset: 0x00184804
	public virtual void SetValuesFromBlock()
	{
		Block block = Block.list[(int)this.BlockID];
		if (block.Properties.Values.ContainsKey("RequiredPower"))
		{
			this.RequiredPower = ushort.Parse(block.Properties.Values["RequiredPower"]);
		}
	}

	// Token: 0x06003CC3 RID: 15555 RVA: 0x00186658 File Offset: 0x00184858
	public void ClearChildren()
	{
		for (int i = 0; i < this.Children.Count; i++)
		{
			PowerManager.Instance.RemoveChild(this.Children[i]);
		}
		if (this.TileEntity != null)
		{
			this.TileEntity.DrawWires();
		}
	}

	// Token: 0x06003CC4 RID: 15556 RVA: 0x001866A4 File Offset: 0x001848A4
	public void SendHasLocalChangesToRoot()
	{
		this.hasChangesLocal = true;
		for (PowerItem parent = this.Parent; parent != null; parent = parent.Parent)
		{
			parent.hasChangesLocal = true;
		}
	}

	// Token: 0x0400310A RID: 12554
	public PowerItem Parent;

	// Token: 0x0400310B RID: 12555
	public Vector3i Position;

	// Token: 0x0400310C RID: 12556
	public PowerItem Root;

	// Token: 0x0400310D RID: 12557
	public ushort Depth = ushort.MaxValue;

	// Token: 0x0400310E RID: 12558
	public ushort BlockID;

	// Token: 0x0400310F RID: 12559
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasChangesLocal;

	// Token: 0x04003110 RID: 12560
	public ushort RequiredPower = 5;

	// Token: 0x04003111 RID: 12561
	public List<PowerItem> Children;

	// Token: 0x04003112 RID: 12562
	public TileEntityPowered TileEntity;

	// Token: 0x04003113 RID: 12563
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isPowered;

	// Token: 0x02000843 RID: 2115
	public enum PowerItemTypes
	{
		// Token: 0x04003115 RID: 12565
		None,
		// Token: 0x04003116 RID: 12566
		Consumer,
		// Token: 0x04003117 RID: 12567
		ConsumerToggle,
		// Token: 0x04003118 RID: 12568
		Trigger,
		// Token: 0x04003119 RID: 12569
		Timer,
		// Token: 0x0400311A RID: 12570
		Generator,
		// Token: 0x0400311B RID: 12571
		SolarPanel,
		// Token: 0x0400311C RID: 12572
		BatteryBank,
		// Token: 0x0400311D RID: 12573
		RangedTrap,
		// Token: 0x0400311E RID: 12574
		ElectricWireRelay,
		// Token: 0x0400311F RID: 12575
		TripWireRelay,
		// Token: 0x04003120 RID: 12576
		PressurePlate
	}
}
