using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AF7 RID: 2807
public class TileEntityDewCollector : TileEntity, IInventory
{
	// Token: 0x1700088E RID: 2190
	// (get) Token: 0x06005651 RID: 22097 RVA: 0x00232B1F File Offset: 0x00230D1F
	public bool IsBlocked
	{
		get
		{
			return this.isBlocked;
		}
	}

	// Token: 0x1700088F RID: 2191
	// (get) Token: 0x06005652 RID: 22098 RVA: 0x00232B27 File Offset: 0x00230D27
	// (set) Token: 0x06005653 RID: 22099 RVA: 0x00232B59 File Offset: 0x00230D59
	public float[] fillValues
	{
		get
		{
			if (this.fillValuesArr == null)
			{
				this.fillValuesArr = new float[this.containerSize.x * this.containerSize.y];
			}
			return this.fillValuesArr;
		}
		set
		{
			this.fillValuesArr = value;
		}
	}

	// Token: 0x17000890 RID: 2192
	// (get) Token: 0x06005654 RID: 22100 RVA: 0x00232B62 File Offset: 0x00230D62
	// (set) Token: 0x06005655 RID: 22101 RVA: 0x00232B94 File Offset: 0x00230D94
	public ItemStack[] items
	{
		get
		{
			if (this.itemsArr == null)
			{
				this.itemsArr = ItemStack.CreateArray(this.containerSize.x * this.containerSize.y);
			}
			return this.itemsArr;
		}
		set
		{
			this.itemsArr = value;
		}
	}

	// Token: 0x17000891 RID: 2193
	// (get) Token: 0x06005656 RID: 22102 RVA: 0x00232B9D File Offset: 0x00230D9D
	// (set) Token: 0x06005657 RID: 22103 RVA: 0x00232BA5 File Offset: 0x00230DA5
	public ItemStack[] ModSlots
	{
		get
		{
			return this.modSlots;
		}
		set
		{
			if (!this.IsModsSame(value))
			{
				this.modSlots = ItemStack.Clone(value);
				this.visibleChanged = true;
				this.modsChanged = true;
				this.HandleModChanged();
				this.UpdateVisible();
				this.setModified();
			}
		}
	}

	// Token: 0x06005658 RID: 22104 RVA: 0x00232BDC File Offset: 0x00230DDC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsModsSame(ItemStack[] _modSlots)
	{
		if (_modSlots == null || _modSlots.Length != this.modSlots.Length)
		{
			return false;
		}
		for (int i = 0; i < _modSlots.Length; i++)
		{
			if (!_modSlots[i].Equals(this.modSlots[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06005659 RID: 22105 RVA: 0x00232C20 File Offset: 0x00230E20
	public TileEntityDewCollector(Chunk _chunk) : base(_chunk)
	{
		this.containerSize = new Vector2i(3, 1);
		this.modSlots = ItemStack.CreateArray(3);
	}

	// Token: 0x0600565A RID: 22106 RVA: 0x00232C90 File Offset: 0x00230E90
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityDewCollector(TileEntityDewCollector _other) : base(null)
	{
		this.containerSize = _other.containerSize;
		this.items = ItemStack.Clone(_other.items);
		this.modSlots = ItemStack.Clone(_other.modSlots);
		this.worldTimeTouched = _other.worldTimeTouched;
		this.bUserAccessing = _other.bUserAccessing;
		this.ConvertToItem = _other.ConvertToItem;
		this.CurrentIndex = _other.CurrentIndex;
		this.CurrentConvertTime = _other.CurrentConvertTime;
		this.leftoverTime = _other.leftoverTime;
	}

	// Token: 0x0600565B RID: 22107 RVA: 0x00232D5B File Offset: 0x00230F5B
	public override TileEntity Clone()
	{
		return new TileEntityDewCollector(this);
	}

	// Token: 0x0600565C RID: 22108 RVA: 0x00232D63 File Offset: 0x00230F63
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		this.HandleUpdate(world);
	}

	// Token: 0x0600565D RID: 22109 RVA: 0x00232D74 File Offset: 0x00230F74
	public void HandleUpdate(World world)
	{
		if (this.ConvertToItem == null)
		{
			BlockDewCollector blockDewCollector = (BlockDewCollector)base.blockValue.Block;
			this.ConvertToItem = ItemClass.GetItemClass(blockDewCollector.ConvertToItem, false);
			this.ModdedConvertToItem = ItemClass.GetItemClass(blockDewCollector.ModdedConvertToItem, false);
			this.modsChanged = true;
		}
		if (base.blockValue.Block.IsUnderwater(GameManager.Instance.World, base.ToWorldPos(), base.blockValue))
		{
			return;
		}
		if (this.countdownBlockedCheck.HasPassed())
		{
			this.isBlocked = this.HandleSkyCheck();
			this.countdownBlockedCheck.ResetAndRestart();
		}
		if (this.isBlocked)
		{
			return;
		}
		bool flag = this.HandleLeftOverTime();
		this.worldTimeTouched = world.worldTime;
		float num = (this.lastWorldTime != 0UL) ? GameUtils.WorldTimeToTotalSeconds(world.worldTime - this.lastWorldTime) : 0f;
		this.lastWorldTime = world.worldTime;
		if (num <= 0f)
		{
			return;
		}
		this.HandleModChanged();
		if (this.CurrentIndex == -1)
		{
			for (int i = 0; i < this.items.Length; i++)
			{
				if (this.items[i].IsEmpty())
				{
					this.CurrentIndex = i;
					break;
				}
				if (this.fillValues[i] != -1f)
				{
					this.fillValues[i] = -1f;
					flag = true;
				}
			}
			if (this.CurrentIndex == -1)
			{
				this.leftoverTime = 0f;
			}
		}
		for (int j = 0; j < this.items.Length; j++)
		{
			if (this.CurrentIndex == j)
			{
				if (this.items[j].IsEmpty())
				{
					if (this.fillValues[j] == -1f)
					{
						BlockDewCollector blockDewCollector2 = (BlockDewCollector)base.blockValue.Block;
						this.CurrentConvertTime = GameManager.Instance.World.GetGameRandom().RandomRange(blockDewCollector2.MinConvertTime, blockDewCollector2.MaxConvertTime);
						this.fillValues[j] = this.leftoverTime;
						this.leftoverTime = 0f;
					}
					else
					{
						this.fillValues[j] += num * this.CurrentConvertSpeed;
						if (this.fillValues[j] >= this.CurrentConvertTime)
						{
							this.leftoverTime = this.fillValues[j] - this.CurrentConvertTime;
							this.items[j] = new ItemStack(new ItemValue(this.IsModdedConvertItem ? this.ModdedConvertToItem.Id : this.ConvertToItem.Id, false), this.CurrentConvertCount);
							this.fillValues[j] = -1f;
							this.CurrentConvertTime = -1f;
							this.CurrentIndex = -1;
						}
					}
					flag = true;
				}
				else
				{
					if (this.fillValues[j] != -1f)
					{
						this.fillValues[j] = -1f;
					}
					this.CurrentIndex = -1;
					flag = true;
				}
			}
			else if (this.fillValues[j] != -1f)
			{
				this.fillValues[j] = -1f;
				flag = true;
			}
		}
		if (flag)
		{
			base.NotifyListeners();
			base.emitHeatMapEvent(world, EnumAIDirectorChunkEvent.Campfire);
			this.setModified();
		}
	}

	// Token: 0x0600565E RID: 22110 RVA: 0x0023308C File Offset: 0x0023128C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleModChanged()
	{
		if (this.modsChanged)
		{
			this.modsChanged = false;
			BlockDewCollector blockDewCollector = (BlockDewCollector)base.blockValue.Block;
			this.IsModdedConvertItem = false;
			this.CurrentConvertCount = 1;
			this.CurrentConvertSpeed = 1f;
			for (int i = 0; i < this.modSlots.Length; i++)
			{
				if (!this.modSlots[i].IsEmpty())
				{
					switch (blockDewCollector.ModTypes[i])
					{
					case BlockDewCollector.ModEffectTypes.Type:
						this.IsModdedConvertItem = true;
						break;
					case BlockDewCollector.ModEffectTypes.Speed:
						this.CurrentConvertSpeed = blockDewCollector.ModdedConvertSpeed;
						break;
					case BlockDewCollector.ModEffectTypes.Count:
						this.CurrentConvertCount = blockDewCollector.ModdedConvertCount;
						break;
					}
				}
			}
		}
	}

	// Token: 0x0600565F RID: 22111 RVA: 0x0023313C File Offset: 0x0023133C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateVisible()
	{
		if (this.visibleChanged)
		{
			this.visibleChanged = false;
			BlockDewCollector blockDewCollector = GameManager.Instance.World.GetBlock(base.ToWorldPos()).Block as BlockDewCollector;
			if (blockDewCollector != null)
			{
				blockDewCollector.UpdateVisible(this);
			}
		}
	}

	// Token: 0x06005660 RID: 22112 RVA: 0x00233188 File Offset: 0x00231388
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HandleSkyCheck()
	{
		Vector3i localChunkPos = base.localChunkPos;
		for (int i = 0; i < 7; i++)
		{
			localChunkPos.y++;
			if (localChunkPos.y >= 256)
			{
				break;
			}
			BlockValue block = this.chunk.GetBlock(localChunkPos);
			if (block.Block != base.blockValue.Block && block.Block.IsCollideArrows)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005661 RID: 22113 RVA: 0x002331F8 File Offset: 0x002313F8
	public bool HandleLeftOverTime()
	{
		if (this.CurrentConvertTime == -1f)
		{
			this.SetupCurrentConvertTime();
		}
		if (this.leftoverTime == 0f)
		{
			return false;
		}
		bool result = false;
		if (this.CurrentIndex != -1)
		{
			if (this.items[this.CurrentIndex].IsEmpty())
			{
				if (this.fillValues[this.CurrentIndex] == -1f)
				{
					this.fillValues[this.CurrentIndex] = 0f;
				}
				if (this.leftoverTime > this.CurrentConvertTime)
				{
					this.items[this.CurrentIndex] = new ItemStack(new ItemValue(this.IsModdedConvertItem ? this.ModdedConvertToItem.Id : this.ConvertToItem.Id, false), this.CurrentConvertCount);
					this.leftoverTime -= this.CurrentConvertTime;
					this.fillValues[this.CurrentIndex] = -1f;
					this.CurrentIndex = -1;
				}
				else
				{
					this.fillValues[this.CurrentIndex] = this.leftoverTime;
					this.leftoverTime = 0f;
				}
				result = true;
			}
			else
			{
				this.CurrentIndex = -1;
			}
		}
		if (this.leftoverTime == 0f)
		{
			return result;
		}
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].IsEmpty())
			{
				if (this.leftoverTime <= this.CurrentConvertTime)
				{
					this.fillValues[i] = this.leftoverTime;
					this.leftoverTime = 0f;
					this.CurrentIndex = i;
					return true;
				}
				this.items[i] = new ItemStack(new ItemValue(this.IsModdedConvertItem ? this.ModdedConvertToItem.Id : this.ConvertToItem.Id, false), this.CurrentConvertCount);
				this.leftoverTime -= this.CurrentConvertTime;
				this.fillValues[i] = -1f;
				this.CurrentIndex = -1;
			}
		}
		return result;
	}

	// Token: 0x06005662 RID: 22114 RVA: 0x002333E4 File Offset: 0x002315E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupCurrentConvertTime()
	{
		BlockDewCollector blockDewCollector = (BlockDewCollector)base.blockValue.Block;
		this.CurrentConvertTime = GameManager.Instance.World.GetGameRandom().RandomRange(blockDewCollector.MinConvertTime, blockDewCollector.MaxConvertTime);
	}

	// Token: 0x06005663 RID: 22115 RVA: 0x0023342B File Offset: 0x0023162B
	public void SetWorldTime()
	{
		this.lastWorldTime = GameManager.Instance.World.worldTime;
	}

	// Token: 0x06005664 RID: 22116 RVA: 0x00233442 File Offset: 0x00231642
	public Vector2i GetContainerSize()
	{
		return this.containerSize;
	}

	// Token: 0x06005665 RID: 22117 RVA: 0x0023344C File Offset: 0x0023164C
	public void SetContainerSize(Vector2i _containerSize, bool clearItems = true)
	{
		this.containerSize = _containerSize;
		if (clearItems)
		{
			if (this.containerSize.x * this.containerSize.y != this.items.Length)
			{
				this.items = ItemStack.CreateArray(this.containerSize.x * this.containerSize.y);
				return;
			}
			for (int i = 0; i < this.items.Length; i++)
			{
				this.items[i] = ItemStack.Empty.Clone();
			}
		}
	}

	// Token: 0x06005666 RID: 22118 RVA: 0x002334CC File Offset: 0x002316CC
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		this.containerSize = default(Vector2i);
		this.containerSize.x = (int)_br.ReadUInt16();
		this.containerSize.y = (int)_br.ReadUInt16();
		this.lastWorldTime = _br.ReadUInt64();
		this.CurrentConvertTime = _br.ReadSingle();
		this.CurrentIndex = (int)_br.ReadInt16();
		this.leftoverTime = _br.ReadSingle();
		int num = Math.Min((int)_br.ReadInt16(), this.containerSize.x * this.containerSize.y);
		if (this.containerSize.x * this.containerSize.y != this.items.Length)
		{
			this.items = ItemStack.CreateArray(this.containerSize.x * this.containerSize.y);
		}
		if (this.containerSize.x * this.containerSize.y != this.fillValues.Length)
		{
			this.fillValues = new float[this.containerSize.x * this.containerSize.y];
		}
		for (int i = 0; i < num; i++)
		{
			this.items[i].Clear();
			this.items[i].Read(_br);
		}
		for (int j = 0; j < num; j++)
		{
			this.fillValues[j] = _br.ReadSingle();
		}
		if (this.readVersion >= 11 || _eStreamMode != TileEntity.StreamModeRead.Persistency)
		{
			int num2 = (int)_br.ReadInt16();
			for (int k = 0; k < num2; k++)
			{
				this.modSlots[k].Clear();
				this.modSlots[k].Read(_br);
			}
			this.modsChanged = true;
			this.HandleModChanged();
		}
	}

	// Token: 0x06005667 RID: 22119 RVA: 0x0023367C File Offset: 0x0023187C
	public override void write(PooledBinaryWriter stream, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(stream, _eStreamMode);
		stream.Write((ushort)this.containerSize.x);
		stream.Write((ushort)this.containerSize.y);
		stream.Write(this.lastWorldTime);
		stream.Write(this.CurrentConvertTime);
		stream.Write((short)this.CurrentIndex);
		stream.Write(this.leftoverTime);
		stream.Write((short)this.items.Length);
		ItemStack[] items = this.items;
		for (int i = 0; i < items.Length; i++)
		{
			items[i].Clone().Write(stream);
		}
		for (int j = 0; j < this.fillValues.Length; j++)
		{
			stream.Write(this.fillValues[j]);
		}
		stream.Write((short)this.modSlots.Length);
		items = this.modSlots;
		for (int i = 0; i < items.Length; i++)
		{
			items[i].Clone().Write(stream);
		}
	}

	// Token: 0x06005668 RID: 22120 RVA: 0x00075C39 File Offset: 0x00073E39
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.DewCollector;
	}

	// Token: 0x06005669 RID: 22121 RVA: 0x0023376B File Offset: 0x0023196B
	public ItemStack[] GetItems()
	{
		return this.items;
	}

	// Token: 0x0600566A RID: 22122 RVA: 0x00233774 File Offset: 0x00231974
	public override void UpgradeDowngradeFrom(TileEntity _other)
	{
		base.UpgradeDowngradeFrom(_other);
		this.OnDestroy();
		if (_other is TileEntityDewCollector)
		{
			TileEntityDewCollector tileEntityDewCollector = _other as TileEntityDewCollector;
			this.worldTimeTouched = tileEntityDewCollector.worldTimeTouched;
			this.items = ItemStack.Clone(tileEntityDewCollector.items, 0, this.containerSize.x * this.containerSize.y);
			if (this.items.Length != this.containerSize.x * this.containerSize.y)
			{
				Log.Error("UpgradeDowngradeFrom: other.size={0}, other.length={1}, this.size={2}, this.length={3}", new object[]
				{
					tileEntityDewCollector.containerSize,
					tileEntityDewCollector.items.Length,
					this.containerSize,
					this.items.Length
				});
			}
		}
	}

	// Token: 0x0600566B RID: 22123 RVA: 0x00233844 File Offset: 0x00231A44
	public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
		base.ReplacedBy(_bvOld, _bvNew, _teNew);
		TileEntityDewCollector tileEntityDewCollector;
		if (!_teNew.TryGetSelfOrFeature(out tileEntityDewCollector))
		{
			List<ItemStack> list = new List<ItemStack>();
			if (this.itemsArr != null)
			{
				list.AddRange(this.itemsArr);
			}
			if (this.modSlots != null)
			{
				list.AddRange(this.modSlots);
			}
			Vector3 pos = base.ToWorldCenterPos();
			pos.y += 0.9f;
			GameManager.Instance.DropContentInLootContainerServer(-1, "DroppedLootContainer", pos, list.ToArray(), true);
		}
	}

	// Token: 0x0600566C RID: 22124 RVA: 0x002338C2 File Offset: 0x00231AC2
	public void UpdateSlot(int _idx, ItemStack _item)
	{
		this.items[_idx] = _item.Clone();
		base.NotifyListeners();
	}

	// Token: 0x0600566D RID: 22125 RVA: 0x002338D8 File Offset: 0x00231AD8
	public bool IsWaterEmpty()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (!this.items[i].IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600566E RID: 22126 RVA: 0x0023390C File Offset: 0x00231B0C
	public bool IsEmpty()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (!this.items[i].IsEmpty())
			{
				return false;
			}
		}
		for (int j = 0; j < this.modSlots.Length; j++)
		{
			if (!this.modSlots[j].IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600566F RID: 22127 RVA: 0x00233964 File Offset: 0x00231B64
	public void SetEmpty()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			this.items[i].Clear();
		}
		for (int j = 0; j < this.fillValues.Length; j++)
		{
			this.fillValues[j] = -1f;
		}
		base.NotifyListeners();
		this.setModified();
	}

	// Token: 0x06005670 RID: 22128 RVA: 0x002339C0 File Offset: 0x00231BC0
	[return: TupleElementNames(new string[]
	{
		"anyMoved",
		"allMoved"
	})]
	public ValueTuple<bool, bool> TryStackItem(int startIndex, ItemStack _itemStack)
	{
		int count = _itemStack.count;
		int num = 0;
		bool item = false;
		for (int i = startIndex; i < this.items.Length; i++)
		{
			num = _itemStack.count;
			if (_itemStack.itemValue.type == this.items[i].itemValue.type && this.items[i].CanStackPartly(ref num))
			{
				this.items[i].count += num;
				_itemStack.count -= num;
				this.setModified();
				item = true;
				if (_itemStack.count == 0)
				{
					base.NotifyListeners();
					return new ValueTuple<bool, bool>(true, true);
				}
			}
		}
		if (_itemStack.count != count)
		{
			item = true;
			base.NotifyListeners();
		}
		return new ValueTuple<bool, bool>(item, false);
	}

	// Token: 0x06005671 RID: 22129 RVA: 0x00233A80 File Offset: 0x00231C80
	public bool AddItem(ItemStack _item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].IsEmpty())
			{
				this.UpdateSlot(i, _item);
				base.NotifyListeners();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005672 RID: 22130 RVA: 0x00233AC0 File Offset: 0x00231CC0
	public bool HasItem(ItemValue _item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].itemValue.ItemClass == _item.ItemClass)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005673 RID: 22131 RVA: 0x00233B00 File Offset: 0x00231D00
	public void RemoveItem(ItemValue _item)
	{
		bool flag = false;
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].itemValue.ItemClass == _item.ItemClass)
			{
				this.UpdateSlot(i, ItemStack.Empty.Clone());
				flag = true;
			}
		}
		if (flag)
		{
			base.NotifyListeners();
		}
	}

	// Token: 0x040042D3 RID: 17107
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i containerSize;

	// Token: 0x040042D4 RID: 17108
	public ItemClass ConvertToItem;

	// Token: 0x040042D5 RID: 17109
	public ItemClass ModdedConvertToItem;

	// Token: 0x040042D6 RID: 17110
	public float CurrentConvertTime = -1f;

	// Token: 0x040042D7 RID: 17111
	public float CurrentConvertSpeed = 1f;

	// Token: 0x040042D8 RID: 17112
	public int CurrentConvertCount = 1;

	// Token: 0x040042D9 RID: 17113
	public float leftoverTime;

	// Token: 0x040042DA RID: 17114
	public int CurrentIndex = -1;

	// Token: 0x040042DB RID: 17115
	public bool IsModdedConvertItem;

	// Token: 0x040042DC RID: 17116
	[PublicizedFrom(EAccessModifier.Private)]
	public static System.Random r = new System.Random();

	// Token: 0x040042DD RID: 17117
	[PublicizedFrom(EAccessModifier.Private)]
	public CountdownTimer countdownBlockedCheck = new CountdownTimer(5f + (float)TileEntityDewCollector.r.NextDouble(), true);

	// Token: 0x040042DE RID: 17118
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBlocked;

	// Token: 0x040042DF RID: 17119
	[PublicizedFrom(EAccessModifier.Private)]
	public bool visibleChanged;

	// Token: 0x040042E0 RID: 17120
	[PublicizedFrom(EAccessModifier.Private)]
	public bool modsChanged;

	// Token: 0x040042E1 RID: 17121
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong lastWorldTime;

	// Token: 0x040042E2 RID: 17122
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] fillValuesArr;

	// Token: 0x040042E3 RID: 17123
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] itemsArr;

	// Token: 0x040042E4 RID: 17124
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] modSlots;

	// Token: 0x040042E5 RID: 17125
	public ulong worldTimeTouched;
}
