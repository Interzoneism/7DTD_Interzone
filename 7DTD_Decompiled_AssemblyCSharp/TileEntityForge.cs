using System;
using UnityEngine;

// Token: 0x02000AF9 RID: 2809
public class TileEntityForge : TileEntity
{
	// Token: 0x06005677 RID: 22135 RVA: 0x00233C0C File Offset: 0x00231E0C
	public TileEntityForge(Chunk _chunk) : base(_chunk)
	{
		this.fuel = ItemStack.CreateArray(3);
		this.input = ItemStack.CreateArray(1);
		this.mold = ItemStack.Empty.Clone();
		this.output = ItemStack.Empty.Clone();
		this.outputItem = ItemValue.None.Clone();
		this.burningItemValue = ItemValue.None.Clone();
		this.fuelInForgeInTicks = 0;
	}

	// Token: 0x06005678 RID: 22136 RVA: 0x00233C9F File Offset: 0x00231E9F
	public bool CanOperate(ulong _worldTimeInTicks)
	{
		return this.GetFuelLeft(_worldTimeInTicks) + this.GetFuelInStorage() > 0 && this.outputWeight > 0 && this.metalInForge > 0;
	}

	// Token: 0x06005679 RID: 22137 RVA: 0x00233CC8 File Offset: 0x00231EC8
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		this.recalcStats();
		int num = (this.lastTickTime != 0UL) ? ((int)(GameTimer.Instance.ticks - this.lastTickTime)) : 0;
		this.lastTickTime = GameTimer.Instance.ticks;
		this.lastTickTimeDataCalculated = this.lastTickTime;
		this.updateLightState(world);
		if (this.fuelInStorageInTicks + this.fuelInForgeInTicks == 0)
		{
			return;
		}
		base.emitHeatMapEvent(world, EnumAIDirectorChunkEvent.Forge);
		bool flag = false;
		num = Utils.FastMin(num, (int)((float)(this.fuelInStorageInTicks + this.fuelInForgeInTicks) / 1f));
		if (this.fuelInStorageInTicks + this.fuelInForgeInTicks > 0)
		{
			int num2 = (int)((float)num * 1f);
			this.fuelInForgeInTicks -= num2;
			while (this.fuelInForgeInTicks < 0)
			{
				flag |= this.moveDown(this.fuel);
				if (!this.fuel[this.fuel.Length - 1].IsEmpty())
				{
					this.burningItemValue = this.fuel[this.fuel.Length - 1].itemValue;
					this.fuelInForgeInTicks += ItemClass.GetFuelValue(this.fuel[this.fuel.Length - 1].itemValue) * 20;
					this.fuel[this.fuel.Length - 1].count--;
					if (this.fuel[this.fuel.Length - 1].count == 0)
					{
						this.fuel[this.fuel.Length - 1].Clear();
					}
				}
			}
			this.updateLightState(world);
			flag = true;
		}
		flag |= this.moveDown(this.fuel);
		if (this.outputWeight > 0)
		{
			int num3 = (int)((float)num * 0.1f);
			while (this.metalInForge < num3 && this.inputMetal >= num3 - this.metalInForge)
			{
				flag |= this.moveDown(this.input);
				if (!this.input[this.input.Length - 1].IsEmpty())
				{
					this.metalInForge += ItemClass.GetForId(this.input[this.input.Length - 1].itemValue.type).GetWeight();
					this.input[this.input.Length - 1].count--;
					if (this.input[this.input.Length - 1].count == 0)
					{
						this.input[this.input.Length - 1].Clear();
					}
					flag = true;
				}
				this.recalcStats();
			}
			if (this.metalInForge > 0)
			{
				num3 = Utils.FastMin(this.metalInForge, num3);
				this.metalInForge -= num3;
				this.moldedMetalSoFar += num3;
				flag = true;
			}
			bool flag2 = false;
			while (this.moldedMetalSoFar >= this.outputWeight)
			{
				this.moldedMetalSoFar -= this.outputWeight;
				this.output = new ItemStack(this.outputItem, this.output.count + 1);
				flag2 = true;
				flag = true;
			}
			if (flag2)
			{
				world.GetGameManager().PlaySoundAtPositionServer(base.ToWorldPos().ToVector3(), "Forge/forge_item_complete", AudioRolloffMode.Logarithmic, 100);
			}
		}
		if (flag)
		{
			this.setModified();
		}
	}

	// Token: 0x0600567A RID: 22138 RVA: 0x00234000 File Offset: 0x00232200
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateLightState(World world)
	{
		BlockValue block = world.GetBlock(base.ToWorldPos());
		if (this.fuelInStorageInTicks + this.fuelInForgeInTicks == 0 && block.meta != 0)
		{
			block.meta = 0;
			world.SetBlockRPC(base.ToWorldPos(), block);
			return;
		}
		if (this.fuelInStorageInTicks + this.fuelInForgeInTicks != 0 && block.meta == 0)
		{
			block.meta = 1;
			world.SetBlockRPC(base.ToWorldPos(), block);
		}
	}

	// Token: 0x0600567B RID: 22139 RVA: 0x00234078 File Offset: 0x00232278
	[PublicizedFrom(EAccessModifier.Private)]
	public bool moveDown(ItemStack[] _items)
	{
		if (_items.Length < 2)
		{
			return false;
		}
		bool result = false;
		for (int i = _items.Length - 1; i > 0; i--)
		{
			if (_items[i].IsEmpty() && !_items[i - 1].IsEmpty())
			{
				_items[i] = _items[i - 1].Clone();
				_items[i - 1].Clear();
				result = true;
			}
		}
		return result;
	}

	// Token: 0x0600567C RID: 22140 RVA: 0x002340D0 File Offset: 0x002322D0
	public int GetFuelLeft(ulong _worldTimeInTicks)
	{
		if (_worldTimeInTicks == 0UL || this.lastTickTimeDataCalculated == 0UL)
		{
			return this.fuelInForgeInTicks / 20;
		}
		float num = (_worldTimeInTicks - this.lastTickTimeDataCalculated) * 1f;
		return (int)Math.Max((float)this.fuelInForgeInTicks - num, 0f) / 20;
	}

	// Token: 0x0600567D RID: 22141 RVA: 0x0023411C File Offset: 0x0023231C
	public override bool IsActive(World world)
	{
		return world.GetBlock(base.ToWorldPos()).meta > 0;
	}

	// Token: 0x0600567E RID: 22142 RVA: 0x00234140 File Offset: 0x00232340
	public int GetInputWeight()
	{
		return this.inputMetal;
	}

	// Token: 0x0600567F RID: 22143 RVA: 0x00234148 File Offset: 0x00232348
	public int GetFuelInStorage()
	{
		return this.fuelInStorageInTicks / 20;
	}

	// Token: 0x06005680 RID: 22144 RVA: 0x00234154 File Offset: 0x00232354
	public int GetMetalForgedSoFar(ulong _currentTickTime)
	{
		if (_currentTickTime == 0UL || this.lastTickTimeDataCalculated == 0UL || !this.CanOperate(_currentTickTime))
		{
			return this.moldedMetalSoFar;
		}
		int num = (int)((_currentTickTime - this.lastTickTimeDataCalculated) * 0.1f);
		if (num < this.metalInForge)
		{
			return Math.Min(this.moldedMetalSoFar + num, this.moldedMetalSoFar + this.metalInForge);
		}
		return Math.Min(this.moldedMetalSoFar + num, this.outputWeight);
	}

	// Token: 0x06005681 RID: 22145 RVA: 0x002341C5 File Offset: 0x002323C5
	public int GetOutputWeight()
	{
		return this.outputWeight;
	}

	// Token: 0x06005682 RID: 22146 RVA: 0x002341D0 File Offset: 0x002323D0
	public int GetCurrentMetalInForge(ulong _currentTickTime)
	{
		if (_currentTickTime == 0UL || this.lastTickTimeDataCalculated == 0UL || !this.CanOperate(_currentTickTime))
		{
			return this.metalInForge;
		}
		int num = (int)((_currentTickTime - this.lastTickTimeDataCalculated) * 0.1f);
		return Math.Max(this.metalInForge - num, 0);
	}

	// Token: 0x06005683 RID: 22147 RVA: 0x00234218 File Offset: 0x00232418
	public float GetMoldTimeNeeded(ulong _currentTickTime)
	{
		return (float)(this.GetInputWeight() + this.GetCurrentMetalInForge(_currentTickTime)) / 2f;
	}

	// Token: 0x06005684 RID: 22148 RVA: 0x0023422F File Offset: 0x0023242F
	public ItemStack[] GetFuel()
	{
		return this.fuel;
	}

	// Token: 0x06005685 RID: 22149 RVA: 0x00234237 File Offset: 0x00232437
	public void SetFuel(ItemStack[] _fuel)
	{
		this.fuel = ItemStack.Clone(_fuel);
		this.setModified();
	}

	// Token: 0x06005686 RID: 22150 RVA: 0x0023424B File Offset: 0x0023244B
	public ItemStack[] GetInput()
	{
		return this.input;
	}

	// Token: 0x06005687 RID: 22151 RVA: 0x00234253 File Offset: 0x00232453
	public void SetInput(ItemStack[] _input, bool _bSetModified = true)
	{
		this.input = ItemStack.Clone(_input);
		if (_bSetModified)
		{
			this.setModified();
		}
	}

	// Token: 0x06005688 RID: 22152 RVA: 0x0023426A File Offset: 0x0023246A
	public ItemStack GetMold()
	{
		return this.mold;
	}

	// Token: 0x06005689 RID: 22153 RVA: 0x00234272 File Offset: 0x00232472
	public void SetMold(ItemStack _mold)
	{
		this.mold = _mold;
		this.moldedMetalSoFar = 0;
		this.metalInForge = 0;
		this.setModified();
	}

	// Token: 0x0600568A RID: 22154 RVA: 0x0023428F File Offset: 0x0023248F
	public ItemStack GetOutput()
	{
		return this.output;
	}

	// Token: 0x0600568B RID: 22155 RVA: 0x00234297 File Offset: 0x00232497
	public ItemValue GetBurningItemValue()
	{
		return this.burningItemValue;
	}

	// Token: 0x0600568C RID: 22156 RVA: 0x0023429F File Offset: 0x0023249F
	public void SetOutput(ItemStack _output, bool _bSetModified = true)
	{
		this.output = _output;
		if (_bSetModified)
		{
			this.setModified();
		}
	}

	// Token: 0x0600568D RID: 22157 RVA: 0x002342B1 File Offset: 0x002324B1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setModified()
	{
		this.recalcStats();
		base.setModified();
	}

	// Token: 0x0600568E RID: 22158 RVA: 0x002342C0 File Offset: 0x002324C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void recalcStats()
	{
		this.outputWeight = 0;
		this.outputItem = ItemValue.None.Clone();
		if (!this.mold.IsEmpty())
		{
			this.outputItem = new ItemValue(ItemClass.GetForId(this.mold.itemValue.type).MoldTarget.Id, false);
			this.outputWeight = ItemClass.GetForId(this.outputItem.type).GetWeight();
		}
		this.fuelInStorageInTicks = 0;
		for (int i = 0; i < this.fuel.Length; i++)
		{
			if (!this.fuel[i].IsEmpty())
			{
				this.fuelInStorageInTicks += ItemClass.GetFuelValue(this.fuel[i].itemValue) * this.fuel[i].count * 20;
			}
		}
		this.inputMetal = 0;
		for (int j = 0; j < this.input.Length; j++)
		{
			ItemClass forId = ItemClass.GetForId(this.input[j].itemValue.type);
			if (forId != null)
			{
				this.inputMetal += forId.GetWeight() * this.input[j].count;
			}
		}
	}

	// Token: 0x0600568F RID: 22159 RVA: 0x002343E8 File Offset: 0x002325E8
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		if (_eStreamMode == TileEntity.StreamModeRead.Persistency)
		{
			this.lastTickTime = _br.ReadUInt64();
			this.lastTickTimeDataCalculated = GameTimer.Instance.ticks;
			int num = (int)_br.ReadByte();
			if (this.fuel == null || this.fuel.Length != num)
			{
				this.fuel = ItemStack.CreateArray(num);
			}
			if (this.readVersion < 3)
			{
				for (int i = 0; i < num; i++)
				{
					this.fuel[i].ReadOld(_br);
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					this.fuel[j].Read(_br);
				}
			}
			int num2 = (int)_br.ReadByte();
			if (this.input == null || this.input.Length != num2)
			{
				this.input = ItemStack.CreateArray(num2);
			}
			if (this.readVersion < 3)
			{
				for (int k = 0; k < num2; k++)
				{
					this.input[k].Read(_br);
				}
			}
			else
			{
				for (int l = 0; l < num2; l++)
				{
					this.input[l].Read(_br);
				}
			}
			if (this.readVersion < 3)
			{
				this.mold.ReadOld(_br);
				this.output.ReadOld(_br);
			}
			else
			{
				this.mold.Read(_br);
				this.output.Read(_br);
			}
			this.fuelInForgeInTicks = _br.ReadInt32();
			this.moldedMetalSoFar = (int)_br.ReadInt16();
			this.metalInForge = (int)_br.ReadInt16();
			this.burningItemValue.Read(_br);
		}
		else if (_eStreamMode == TileEntity.StreamModeRead.FromClient)
		{
			this.lastTickTimeDataCalculated = GameTimer.Instance.ticks;
			int num3 = (int)_br.ReadByte();
			if (this.fuel == null || this.fuel.Length != num3)
			{
				this.fuel = ItemStack.CreateArray(num3);
			}
			for (int m = 0; m < num3; m++)
			{
				this.fuel[m].ReadDelta(_br, this.fuel[m]);
			}
			int num4 = (int)_br.ReadByte();
			if (this.input == null || this.input.Length != num4)
			{
				this.input = ItemStack.CreateArray(num4);
			}
			for (int n = 0; n < num4; n++)
			{
				this.input[n].ReadDelta(_br, this.input[n]);
			}
			this.mold.ReadDelta(_br, this.mold);
			this.output.ReadDelta(_br, this.output);
			if (this.mold.itemValue.type == 0)
			{
				this.moldedMetalSoFar = 0;
				this.metalInForge = 0;
			}
		}
		else if (_eStreamMode == TileEntity.StreamModeRead.FromServer)
		{
			if (base.bWaitingForServerResponse)
			{
				Log.Warning("Throwing away server packet as we are waiting for status update!");
			}
			this.lastTickTimeDataCalculated = GameTimer.Instance.ticks;
			int num5 = (int)_br.ReadByte();
			if (this.fuel == null || this.fuel.Length != num5)
			{
				this.fuel = ItemStack.CreateArray(num5);
			}
			if (!base.bWaitingForServerResponse)
			{
				for (int num6 = 0; num6 < num5; num6++)
				{
					this.fuel[num6].Read(_br);
				}
				this.lastServerFuel = ItemStack.Clone(this.fuel);
			}
			else
			{
				ItemStack itemStack = ItemStack.Empty.Clone();
				for (int num7 = 0; num7 < num5; num7++)
				{
					itemStack.Read(_br);
				}
			}
			int num8 = (int)_br.ReadByte();
			if (this.input == null || this.input.Length != num8)
			{
				this.input = ItemStack.CreateArray(num8);
			}
			if (!base.bWaitingForServerResponse)
			{
				for (int num9 = 0; num9 < num8; num9++)
				{
					this.input[num9].Read(_br);
				}
				this.lastServerInput = ItemStack.Clone(this.input);
			}
			else
			{
				ItemStack itemStack2 = ItemStack.Empty.Clone();
				for (int num10 = 0; num10 < num8; num10++)
				{
					itemStack2.Read(_br);
				}
			}
			if (!base.bWaitingForServerResponse)
			{
				this.mold.Read(_br);
				this.lastServerMold = this.mold.Clone();
			}
			else
			{
				ItemStack.Empty.Clone().Read(_br);
			}
			if (!base.bWaitingForServerResponse)
			{
				this.output.Read(_br);
				this.lastServerOutput = this.output.Clone();
			}
			else
			{
				ItemStack.Empty.Clone().Read(_br);
			}
			this.fuelInForgeInTicks = _br.ReadInt32();
			this.moldedMetalSoFar = (int)_br.ReadInt16();
			this.metalInForge = (int)_br.ReadInt16();
			this.burningItemValue.Read(_br);
		}
		this.recalcStats();
	}

	// Token: 0x06005690 RID: 22160 RVA: 0x00234860 File Offset: 0x00232A60
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		if (_eStreamMode == TileEntity.StreamModeWrite.Persistency)
		{
			_bw.Write(this.lastTickTime);
			_bw.Write((byte)this.fuel.Length);
			for (int i = 0; i < this.fuel.Length; i++)
			{
				this.fuel[i].Write(_bw);
			}
			_bw.Write((byte)this.input.Length);
			for (int j = 0; j < this.input.Length; j++)
			{
				this.input[j].Write(_bw);
			}
			this.mold.Write(_bw);
			this.output.Write(_bw);
			_bw.Write(this.fuelInForgeInTicks);
			_bw.Write((short)this.moldedMetalSoFar);
			_bw.Write((short)this.metalInForge);
			this.burningItemValue.Write(_bw);
			return;
		}
		if (_eStreamMode == TileEntity.StreamModeWrite.ToServer)
		{
			_bw.Write((byte)this.fuel.Length);
			for (int k = 0; k < this.fuel.Length; k++)
			{
				this.fuel[k].WriteDelta(_bw, (this.lastServerFuel != null) ? this.lastServerFuel[k] : ItemStack.Empty.Clone());
			}
			_bw.Write((byte)this.input.Length);
			for (int l = 0; l < this.input.Length; l++)
			{
				this.input[l].WriteDelta(_bw, (this.lastServerInput != null) ? this.lastServerInput[l] : ItemStack.Empty.Clone());
			}
			this.mold.WriteDelta(_bw, this.lastServerMold);
			this.output.WriteDelta(_bw, this.lastServerOutput);
			return;
		}
		if (_eStreamMode == TileEntity.StreamModeWrite.ToClient)
		{
			_bw.Write((byte)this.fuel.Length);
			for (int m = 0; m < this.fuel.Length; m++)
			{
				this.fuel[m].Write(_bw);
			}
			_bw.Write((byte)this.input.Length);
			for (int n = 0; n < this.input.Length; n++)
			{
				this.input[n].Write(_bw);
			}
			this.mold.Write(_bw);
			this.output.Write(_bw);
			_bw.Write(this.fuelInForgeInTicks);
			_bw.Write((short)this.moldedMetalSoFar);
			_bw.Write((short)this.metalInForge);
			this.burningItemValue.Write(_bw);
		}
	}

	// Token: 0x06005691 RID: 22161 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Forge;
	}

	// Token: 0x040042E6 RID: 17126
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cFuelBurnPerTick = 1f;

	// Token: 0x040042E7 RID: 17127
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cMoldPerTick = 0.1f;

	// Token: 0x040042E8 RID: 17128
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] fuel;

	// Token: 0x040042E9 RID: 17129
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] input;

	// Token: 0x040042EA RID: 17130
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack mold;

	// Token: 0x040042EB RID: 17131
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack output;

	// Token: 0x040042EC RID: 17132
	[PublicizedFrom(EAccessModifier.Private)]
	public int fuelInForgeInTicks;

	// Token: 0x040042ED RID: 17133
	[PublicizedFrom(EAccessModifier.Private)]
	public int moldedMetalSoFar;

	// Token: 0x040042EE RID: 17134
	[PublicizedFrom(EAccessModifier.Private)]
	public int metalInForge;

	// Token: 0x040042EF RID: 17135
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue burningItemValue;

	// Token: 0x040042F0 RID: 17136
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong lastTickTime;

	// Token: 0x040042F1 RID: 17137
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong lastTickTimeDataCalculated;

	// Token: 0x040042F2 RID: 17138
	[PublicizedFrom(EAccessModifier.Private)]
	public int inputMetal;

	// Token: 0x040042F3 RID: 17139
	[PublicizedFrom(EAccessModifier.Private)]
	public int outputWeight;

	// Token: 0x040042F4 RID: 17140
	[PublicizedFrom(EAccessModifier.Private)]
	public int fuelInStorageInTicks;

	// Token: 0x040042F5 RID: 17141
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue outputItem;

	// Token: 0x040042F6 RID: 17142
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] lastServerFuel;

	// Token: 0x040042F7 RID: 17143
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] lastServerInput;

	// Token: 0x040042F8 RID: 17144
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack lastServerMold = ItemStack.Empty.Clone();

	// Token: 0x040042F9 RID: 17145
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack lastServerOutput = ItemStack.Empty.Clone();
}
