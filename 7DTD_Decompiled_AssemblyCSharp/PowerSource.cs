using System;
using System.IO;
using Audio;
using UnityEngine;

// Token: 0x02000848 RID: 2120
public class PowerSource : PowerItem
{
	// Token: 0x1700064D RID: 1613
	// (get) Token: 0x06003CF3 RID: 15603 RVA: 0x001874C4 File Offset: 0x001856C4
	// (set) Token: 0x06003CF4 RID: 15604 RVA: 0x001874CC File Offset: 0x001856CC
	public bool IsOn
	{
		get
		{
			return this.isOn;
		}
		set
		{
			if (this.isOn != value)
			{
				base.SendHasLocalChangesToRoot();
				this.isOn = value;
				this.HandleOnOffSound();
				if (!this.isOn)
				{
					this.HandleDisconnect();
				}
				this.LastPowerUsed = 0;
				if (this.TileEntity != null)
				{
					this.TileEntity.Activate(this.isOn);
				}
			}
		}
	}

	// Token: 0x06003CF5 RID: 15605 RVA: 0x00187524 File Offset: 0x00185724
	public PowerSource()
	{
		this.Stacks = new ItemStack[6];
		for (int i = 0; i < this.Stacks.Length; i++)
		{
			this.Stacks[i] = ItemStack.Empty.Clone();
		}
	}

	// Token: 0x1700064E RID: 1614
	// (get) Token: 0x06003CF6 RID: 15606 RVA: 0x001874C4 File Offset: 0x001856C4
	public override bool IsPowered
	{
		get
		{
			return this.isOn;
		}
	}

	// Token: 0x1700064F RID: 1615
	// (get) Token: 0x06003CF7 RID: 15607 RVA: 0x0002B133 File Offset: 0x00029333
	public virtual string OnSound
	{
		get
		{
			return "";
		}
	}

	// Token: 0x17000650 RID: 1616
	// (get) Token: 0x06003CF8 RID: 15608 RVA: 0x0002B133 File Offset: 0x00029333
	public virtual string OffSound
	{
		get
		{
			return "";
		}
	}

	// Token: 0x06003CF9 RID: 15609 RVA: 0x00187573 File Offset: 0x00185773
	public void Refresh()
	{
		if (this.TileEntity != null)
		{
			this.TileEntity.Activate(this.isOn);
		}
	}

	// Token: 0x06003CFA RID: 15610 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanParent(PowerItem newParent)
	{
		return false;
	}

	// Token: 0x06003CFB RID: 15611 RVA: 0x00187590 File Offset: 0x00185790
	public override void read(BinaryReader _br, byte _version)
	{
		base.read(_br, _version);
		this.LastCurrentPower = (this.CurrentPower = _br.ReadUInt16());
		this.IsOn = _br.ReadBoolean();
		this.SetSlots(GameUtils.ReadItemStack(_br));
		this.hasChangesLocal = true;
	}

	// Token: 0x06003CFC RID: 15612 RVA: 0x001875D9 File Offset: 0x001857D9
	public override void write(BinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.CurrentPower);
		_bw.Write(this.IsOn);
		GameUtils.WriteItemStack(_bw, this.Stacks);
	}

	// Token: 0x06003CFD RID: 15613 RVA: 0x00187608 File Offset: 0x00185808
	public virtual void Update()
	{
		this.HandleSendPower();
		if (this.hasChangesLocal)
		{
			for (int i = 0; i < this.Children.Count; i++)
			{
				this.Children[i].HandlePowerUpdate(this.IsOn);
			}
			this.hasChangesLocal = false;
		}
	}

	// Token: 0x06003CFE RID: 15614 RVA: 0x00187658 File Offset: 0x00185858
	public virtual void HandleSendPower()
	{
		if (this.IsOn)
		{
			if (this.CurrentPower < this.MaxPower)
			{
				this.TickPowerGeneration();
			}
			else if (this.CurrentPower > this.MaxPower)
			{
				this.CurrentPower = this.MaxPower;
			}
			if (this.ShouldAutoTurnOff())
			{
				this.CurrentPower = 0;
				this.IsOn = false;
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
			this.CurrentPower -= this.LastPowerUsed;
		}
	}

	// Token: 0x06003CFF RID: 15615 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool ShouldAutoTurnOff()
	{
		return false;
	}

	// Token: 0x06003D00 RID: 15616 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void TickPowerGeneration()
	{
	}

	// Token: 0x06003D01 RID: 15617 RVA: 0x00187738 File Offset: 0x00185938
	public override void SetValuesFromBlock()
	{
		base.SetValuesFromBlock();
		Block block = Block.list[(int)this.BlockID];
		if (block.Properties.Values.ContainsKey("OutputPerStack"))
		{
			this.OutputPerStack = ushort.Parse(block.Properties.Values["OutputPerStack"]);
		}
		this.RequiredPower = (this.MaxPower = (this.MaxOutput = this.OutputPerStack * this.SlotCount));
	}

	// Token: 0x06003D02 RID: 15618 RVA: 0x001877B5 File Offset: 0x001859B5
	public void SetSlots(ItemStack[] _stacks)
	{
		this.Stacks = _stacks;
		this.RefreshPowerStats();
	}

	// Token: 0x06003D03 RID: 15619 RVA: 0x001877C4 File Offset: 0x001859C4
	public bool TryAddItemToSlot(ItemClass itemClass, ItemStack itemStack)
	{
		if (!this.IsOn)
		{
			for (int i = 0; i < this.Stacks.Length; i++)
			{
				if (this.Stacks[i].IsEmpty())
				{
					this.Stacks[i] = itemStack;
					this.RefreshPowerStats();
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003D04 RID: 15620 RVA: 0x00187810 File Offset: 0x00185A10
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void RefreshPowerStats()
	{
		this.SlotCount = 0;
		this.MaxOutput = 0;
		for (int i = 0; i < this.Stacks.Length; i++)
		{
			if (!this.Stacks[i].IsEmpty())
			{
				this.MaxOutput += (ushort)((float)this.OutputPerStack * Mathf.Lerp(0.5f, 1f, (float)this.Stacks[i].itemValue.Quality / 6f));
				this.SlotCount += 1;
			}
		}
		if (this.BlockID == 0 && this.TileEntity != null)
		{
			this.BlockID = (ushort)GameManager.Instance.World.GetBlock(this.TileEntity.ToWorldPos()).type;
			this.SetValuesFromBlock();
		}
		if (this.MaxPower == 0)
		{
			this.MaxPower = this.MaxOutput;
		}
		if (this.RequiredPower == 0)
		{
			this.RequiredPower = this.MaxOutput;
		}
	}

	// Token: 0x06003D05 RID: 15621 RVA: 0x00187902 File Offset: 0x00185B02
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleOnOffSound()
	{
		if (GameManager.Instance.GameHasStarted)
		{
			Manager.BroadcastPlay(this.Position.ToVector3(), this.isOn ? this.OnSound : this.OffSound, 0f);
		}
	}

	// Token: 0x0400313D RID: 12605
	public ushort OutputPerStack;

	// Token: 0x0400313E RID: 12606
	public ushort SlotCount;

	// Token: 0x0400313F RID: 12607
	public ushort MaxOutput;

	// Token: 0x04003140 RID: 12608
	public ushort MaxPower = 60000;

	// Token: 0x04003141 RID: 12609
	public ushort LastPowerUsed;

	// Token: 0x04003142 RID: 12610
	public ushort CurrentPower;

	// Token: 0x04003143 RID: 12611
	public ushort LastCurrentPower;

	// Token: 0x04003144 RID: 12612
	public ItemStack[] Stacks;

	// Token: 0x04003145 RID: 12613
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isOn;
}
