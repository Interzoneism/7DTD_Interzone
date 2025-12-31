using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x020007EB RID: 2027
public class TraderData
{
	// Token: 0x06003A3E RID: 14910 RVA: 0x001771E8 File Offset: 0x001753E8
	public TraderData()
	{
		this.world = GameManager.Instance.World;
	}

	// Token: 0x06003A3F RID: 14911 RVA: 0x00177228 File Offset: 0x00175428
	public TraderData(TraderData other)
	{
		this.lastInventoryUpdate = other.lastInventoryUpdate;
		this.TraderID = other.TraderID;
		this.AvailableMoney = other.AvailableMoney;
		this.PrimaryInventory.AddRange(ItemStack.Clone(other.PrimaryInventory));
		this.priceMarkupList.AddRange(other.priceMarkupList);
		for (int i = 0; i < other.TierItemGroups.Count; i++)
		{
			this.TierItemGroups.Add(ItemStack.Clone(other.TierItemGroups[i]));
		}
	}

	// Token: 0x170005E5 RID: 1509
	// (get) Token: 0x06003A40 RID: 14912 RVA: 0x001772E0 File Offset: 0x001754E0
	public TraderInfo TraderInfo
	{
		get
		{
			if (this.TraderID != -1)
			{
				return TraderInfo.traderInfoList[this.TraderID];
			}
			return null;
		}
	}

	// Token: 0x170005E6 RID: 1510
	// (get) Token: 0x06003A41 RID: 14913 RVA: 0x001772F9 File Offset: 0x001754F9
	public float FullTime
	{
		get
		{
			return (float)((this.TraderInfo != null) ? this.TraderInfo.ResetIntervalInTicks : 0);
		}
	}

	// Token: 0x170005E7 RID: 1511
	// (get) Token: 0x06003A42 RID: 14914 RVA: 0x00177312 File Offset: 0x00175512
	public float CurrentTime
	{
		get
		{
			if (this.lastInventoryUpdate == 0UL)
			{
				return 0f;
			}
			return (this.FullTime - (float)((int)(this.world.GetWorldTime() - this.lastInventoryUpdate))) / 10f;
		}
	}

	// Token: 0x170005E8 RID: 1512
	// (get) Token: 0x06003A43 RID: 14915 RVA: 0x00177343 File Offset: 0x00175543
	public ulong NextResetTime
	{
		get
		{
			if (this.TraderInfo == null)
			{
				return 0UL;
			}
			return this.lastInventoryUpdate + (ulong)((long)this.TraderInfo.ResetIntervalInTicks);
		}
	}

	// Token: 0x06003A44 RID: 14916 RVA: 0x00177364 File Offset: 0x00175564
	public void AddToPrimaryInventory(ItemStack stack, bool addMarkup)
	{
		for (int i = 0; i < this.PrimaryInventory.Count; i++)
		{
			if (stack.itemValue.type == this.PrimaryInventory[i].itemValue.type)
			{
				ItemClass forId = ItemClass.GetForId(stack.itemValue.type);
				if (forId.CanStack())
				{
					int num = Math.Min(stack.count, forId.Stacknumber.Value - this.PrimaryInventory[i].count);
					stack.count -= num;
					this.PrimaryInventory[i].count += num;
					if (stack.count == 0)
					{
						return;
					}
				}
			}
		}
		if (stack.count > 0)
		{
			this.PrimaryInventory.Add(stack.Clone());
			if (addMarkup)
			{
				this.priceMarkupList.Add(0);
			}
		}
	}

	// Token: 0x06003A45 RID: 14917 RVA: 0x0017744C File Offset: 0x0017564C
	public int GetPrimaryItemCount(ItemValue itemValue)
	{
		int num = 0;
		for (int i = 0; i < this.PrimaryInventory.Count; i++)
		{
			if (itemValue.type == this.PrimaryInventory[i].itemValue.type)
			{
				num += this.PrimaryInventory[i].count;
			}
		}
		return num;
	}

	// Token: 0x06003A46 RID: 14918 RVA: 0x001774A4 File Offset: 0x001756A4
	public int GetMarkupByIndex(int index)
	{
		if (this.priceMarkupList.Count <= index || index == -1)
		{
			return 0;
		}
		return (int)this.priceMarkupList[index];
	}

	// Token: 0x06003A47 RID: 14919 RVA: 0x001774C8 File Offset: 0x001756C8
	public void IncreaseMarkup(int index)
	{
		if (this.priceMarkupList.Count > index && this.priceMarkupList[index] < 100)
		{
			List<sbyte> list = this.priceMarkupList;
			sbyte b = list[index];
			list[index] = b + 1;
		}
	}

	// Token: 0x06003A48 RID: 14920 RVA: 0x00177510 File Offset: 0x00175710
	public void DecreaseMarkup(int index)
	{
		if (this.priceMarkupList.Count > index && this.priceMarkupList[index] > -4)
		{
			List<sbyte> list = this.priceMarkupList;
			sbyte b = list[index];
			list[index] = b - 1;
		}
	}

	// Token: 0x06003A49 RID: 14921 RVA: 0x00177555 File Offset: 0x00175755
	public void ResetMarkup(int index)
	{
		if (this.priceMarkupList.Count > index)
		{
			this.priceMarkupList[index] = 0;
		}
	}

	// Token: 0x06003A4A RID: 14922 RVA: 0x00177572 File Offset: 0x00175772
	public void RemoveMarkup(int index)
	{
		if (this.priceMarkupList.Count > index)
		{
			this.priceMarkupList.RemoveAt(index);
		}
	}

	// Token: 0x06003A4B RID: 14923 RVA: 0x0017758E File Offset: 0x0017578E
	public void ClearMarkupList()
	{
		this.priceMarkupList.Clear();
	}

	// Token: 0x06003A4C RID: 14924 RVA: 0x0017759B File Offset: 0x0017579B
	public void Read(byte _version, BinaryReader _br)
	{
		this.TraderID = _br.ReadInt32();
		this.lastInventoryUpdate = _br.ReadUInt64();
		_br.ReadByte();
		this.ReadInventoryData(_br);
	}

	// Token: 0x06003A4D RID: 14925 RVA: 0x001775C4 File Offset: 0x001757C4
	public void ReadInventoryData(BinaryReader _br)
	{
		this.PrimaryInventory.Clear();
		this.PrimaryInventory.AddRange(GameUtils.ReadItemStack(_br));
		this.TierItemGroups.Clear();
		int num = (int)_br.ReadByte();
		for (int i = 0; i < num; i++)
		{
			this.TierItemGroups.Add(GameUtils.ReadItemStack(_br));
		}
		this.AvailableMoney = _br.ReadInt32();
		this.priceMarkupList.Clear();
		int num2 = _br.ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			this.priceMarkupList.Add(_br.ReadSByte());
		}
	}

	// Token: 0x06003A4E RID: 14926 RVA: 0x00177657 File Offset: 0x00175857
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.TraderID);
		_bw.Write(this.lastInventoryUpdate);
		_bw.Write(TraderData.FileVersion);
		this.WriteInventoryData(_bw);
	}

	// Token: 0x06003A4F RID: 14927 RVA: 0x00177684 File Offset: 0x00175884
	public void WriteInventoryData(BinaryWriter _bw)
	{
		GameUtils.WriteItemStack(_bw, this.PrimaryInventory);
		_bw.Write((byte)this.TierItemGroups.Count);
		for (int i = 0; i < this.TierItemGroups.Count; i++)
		{
			GameUtils.WriteItemStack(_bw, this.TierItemGroups[i]);
		}
		_bw.Write(this.AvailableMoney);
		_bw.Write(this.priceMarkupList.Count);
		for (int j = 0; j < this.priceMarkupList.Count; j++)
		{
			_bw.Write(this.priceMarkupList[j]);
		}
	}

	// Token: 0x04002F20 RID: 12064
	public List<ItemStack> PrimaryInventory = new List<ItemStack>();

	// Token: 0x04002F21 RID: 12065
	public List<ItemStack[]> TierItemGroups = new List<ItemStack[]>();

	// Token: 0x04002F22 RID: 12066
	public ulong lastInventoryUpdate;

	// Token: 0x04002F23 RID: 12067
	public int TraderID = -1;

	// Token: 0x04002F24 RID: 12068
	public int AvailableMoney;

	// Token: 0x04002F25 RID: 12069
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04002F26 RID: 12070
	[PublicizedFrom(EAccessModifier.Private)]
	public List<sbyte> priceMarkupList = new List<sbyte>();

	// Token: 0x04002F27 RID: 12071
	public static byte FileVersion = 1;
}
