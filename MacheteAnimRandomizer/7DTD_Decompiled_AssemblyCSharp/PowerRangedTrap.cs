using System;
using System.IO;

// Token: 0x02000845 RID: 2117
public class PowerRangedTrap : PowerConsumer
{
	// Token: 0x17000647 RID: 1607
	// (get) Token: 0x06003CDD RID: 15581 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override PowerItem.PowerItemTypes PowerItemType
	{
		get
		{
			return PowerItem.PowerItemTypes.RangedTrap;
		}
	}

	// Token: 0x06003CDE RID: 15582 RVA: 0x00187078 File Offset: 0x00185278
	public PowerRangedTrap()
	{
		this.Stacks = new ItemStack[3];
		for (int i = 0; i < this.Stacks.Length; i++)
		{
			this.Stacks[i] = ItemStack.Empty.Clone();
		}
	}

	// Token: 0x17000648 RID: 1608
	// (get) Token: 0x06003CDF RID: 15583 RVA: 0x001870C4 File Offset: 0x001852C4
	// (set) Token: 0x06003CE0 RID: 15584 RVA: 0x001870CC File Offset: 0x001852CC
	public bool IsLocked
	{
		get
		{
			return this.isLocked;
		}
		set
		{
			if (this.isLocked != value)
			{
				this.isLocked = value;
			}
		}
	}

	// Token: 0x06003CE1 RID: 15585 RVA: 0x001870E0 File Offset: 0x001852E0
	public bool TryStackItem(ItemStack itemStack)
	{
		int num = 0;
		for (int i = 0; i < this.Stacks.Length; i++)
		{
			num = itemStack.count;
			if (this.Stacks[i].IsEmpty())
			{
				this.Stacks[i] = itemStack.Clone();
				itemStack.count = 0;
				return true;
			}
			if (this.Stacks[i].itemValue.type == itemStack.itemValue.type && this.Stacks[i].CanStackPartly(ref num))
			{
				this.Stacks[i].count += num;
				itemStack.count -= num;
				if (itemStack.count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003CE2 RID: 15586 RVA: 0x00187194 File Offset: 0x00185394
	public bool AddItem(ItemStack itemStack)
	{
		if (!this.isLocked)
		{
			for (int i = 0; i < this.Stacks.Length; i++)
			{
				if (this.Stacks[i].IsEmpty())
				{
					this.Stacks[i] = itemStack;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003CE3 RID: 15587 RVA: 0x001871D7 File Offset: 0x001853D7
	public void SetSlots(ItemStack[] _stacks)
	{
		this.Stacks = _stacks;
	}

	// Token: 0x06003CE4 RID: 15588 RVA: 0x001871E0 File Offset: 0x001853E0
	public override void read(BinaryReader _br, byte _version)
	{
		base.read(_br, _version);
		this.isLocked = _br.ReadBoolean();
		this.SetSlots(GameUtils.ReadItemStack(_br));
		this.TargetType = (PowerRangedTrap.TargetTypes)_br.ReadInt32();
	}

	// Token: 0x06003CE5 RID: 15589 RVA: 0x0018720E File Offset: 0x0018540E
	public override void write(BinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.isLocked);
		GameUtils.WriteItemStack(_bw, this.Stacks);
		_bw.Write((int)this.TargetType);
	}

	// Token: 0x0400312E RID: 12590
	public ItemStack[] Stacks;

	// Token: 0x0400312F RID: 12591
	public PowerRangedTrap.TargetTypes TargetType = PowerRangedTrap.TargetTypes.Strangers | PowerRangedTrap.TargetTypes.Zombies;

	// Token: 0x04003130 RID: 12592
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isLocked;

	// Token: 0x02000846 RID: 2118
	[Flags]
	public enum TargetTypes
	{
		// Token: 0x04003132 RID: 12594
		None = 0,
		// Token: 0x04003133 RID: 12595
		Self = 1,
		// Token: 0x04003134 RID: 12596
		Allies = 2,
		// Token: 0x04003135 RID: 12597
		Strangers = 4,
		// Token: 0x04003136 RID: 12598
		Zombies = 8
	}
}
