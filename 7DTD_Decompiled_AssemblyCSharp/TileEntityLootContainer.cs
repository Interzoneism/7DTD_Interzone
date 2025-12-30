using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AFE RID: 2814
public class TileEntityLootContainer : TileEntity, IInventory, ITileEntityLootable, ITileEntity
{
	// Token: 0x17000895 RID: 2197
	// (get) Token: 0x060056B3 RID: 22195 RVA: 0x0023501B File Offset: 0x0023321B
	// (set) Token: 0x060056B4 RID: 22196 RVA: 0x00235023 File Offset: 0x00233223
	public string lootListName { get; set; }

	// Token: 0x17000896 RID: 2198
	// (get) Token: 0x060056B5 RID: 22197 RVA: 0x0023502C File Offset: 0x0023322C
	public virtual float LootStageMod
	{
		get
		{
			return (base.blockValue.Block as BlockLoot).LootStageMod;
		}
	}

	// Token: 0x17000897 RID: 2199
	// (get) Token: 0x060056B6 RID: 22198 RVA: 0x00235054 File Offset: 0x00233254
	public virtual float LootStageBonus
	{
		get
		{
			return (base.blockValue.Block as BlockLoot).LootStageBonus;
		}
	}

	// Token: 0x17000898 RID: 2200
	// (get) Token: 0x060056B7 RID: 22199 RVA: 0x00235079 File Offset: 0x00233279
	// (set) Token: 0x060056B8 RID: 22200 RVA: 0x002350AB File Offset: 0x002332AB
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

	// Token: 0x17000899 RID: 2201
	// (get) Token: 0x060056B9 RID: 22201 RVA: 0x002350B4 File Offset: 0x002332B4
	// (set) Token: 0x060056BA RID: 22202 RVA: 0x002350BC File Offset: 0x002332BC
	public bool bPlayerBackpack { get; set; }

	// Token: 0x1700089A RID: 2202
	// (get) Token: 0x060056BB RID: 22203 RVA: 0x002350C5 File Offset: 0x002332C5
	// (set) Token: 0x060056BC RID: 22204 RVA: 0x002350CD File Offset: 0x002332CD
	public bool bPlayerStorage { get; set; }

	// Token: 0x1700089B RID: 2203
	// (get) Token: 0x060056BD RID: 22205 RVA: 0x002350D6 File Offset: 0x002332D6
	// (set) Token: 0x060056BE RID: 22206 RVA: 0x002350DE File Offset: 0x002332DE
	public PreferenceTracker preferences { get; set; }

	// Token: 0x1700089C RID: 2204
	// (get) Token: 0x060056BF RID: 22207 RVA: 0x002350E7 File Offset: 0x002332E7
	// (set) Token: 0x060056C0 RID: 22208 RVA: 0x002350EF File Offset: 0x002332EF
	public bool bTouched { get; set; }

	// Token: 0x1700089D RID: 2205
	// (get) Token: 0x060056C1 RID: 22209 RVA: 0x002350F8 File Offset: 0x002332F8
	// (set) Token: 0x060056C2 RID: 22210 RVA: 0x00235100 File Offset: 0x00233300
	public ulong worldTimeTouched { get; set; }

	// Token: 0x1700089E RID: 2206
	// (get) Token: 0x060056C3 RID: 22211 RVA: 0x00235109 File Offset: 0x00233309
	// (set) Token: 0x060056C4 RID: 22212 RVA: 0x00235111 File Offset: 0x00233311
	public bool bWasTouched { get; set; }

	// Token: 0x1700089F RID: 2207
	// (get) Token: 0x060056C5 RID: 22213 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool HasSlotLocksSupport
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170008A0 RID: 2208
	// (get) Token: 0x060056C6 RID: 22214 RVA: 0x0023511A File Offset: 0x0023331A
	// (set) Token: 0x060056C7 RID: 22215 RVA: 0x00235122 File Offset: 0x00233322
	public PackedBoolArray SlotLocks { get; set; }

	// Token: 0x060056C8 RID: 22216 RVA: 0x0023512B File Offset: 0x0023332B
	public TileEntityLootContainer(Chunk _chunk) : base(_chunk)
	{
		this.containerSize = new Vector2i(3, 3);
		this.lootListName = null;
	}

	// Token: 0x060056C9 RID: 22217 RVA: 0x00235148 File Offset: 0x00233348
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityLootContainer(TileEntityLootContainer _other) : base(null)
	{
		this.lootListName = _other.lootListName;
		this.containerSize = _other.containerSize;
		this.items = ItemStack.Clone(_other.items);
		this.bTouched = _other.bTouched;
		this.worldTimeTouched = _other.worldTimeTouched;
		this.bPlayerBackpack = _other.bPlayerBackpack;
		this.bPlayerStorage = _other.bPlayerStorage;
		this.bUserAccessing = _other.bUserAccessing;
	}

	// Token: 0x060056CA RID: 22218 RVA: 0x002351C1 File Offset: 0x002333C1
	public override TileEntity Clone()
	{
		return new TileEntityLootContainer(this);
	}

	// Token: 0x060056CB RID: 22219 RVA: 0x002351CC File Offset: 0x002333CC
	public void CopyLootContainerDataFromOther(TileEntityLootContainer _other)
	{
		this.lootListName = _other.lootListName;
		this.containerSize = _other.containerSize;
		this.items = ItemStack.Clone(_other.items);
		this.bTouched = _other.bTouched;
		this.worldTimeTouched = _other.worldTimeTouched;
		this.bPlayerBackpack = _other.bPlayerBackpack;
		this.bPlayerStorage = _other.bPlayerStorage;
		this.bUserAccessing = _other.bUserAccessing;
	}

	// Token: 0x060056CC RID: 22220 RVA: 0x00235240 File Offset: 0x00233440
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		if (GamePrefs.GetInt(EnumGamePrefs.LootRespawnDays) > 0 && !this.bPlayerStorage && this.bTouched && this.IsEmpty())
		{
			int num = GameUtils.WorldTimeToHours(this.worldTimeTouched);
			num += GameUtils.WorldTimeToDays(this.worldTimeTouched) * 24;
			if ((GameUtils.WorldTimeToHours(world.worldTime) + GameUtils.WorldTimeToDays(world.worldTime) * 24 - num) / 24 < GamePrefs.GetInt(EnumGamePrefs.LootRespawnDays))
			{
				if (this.entList == null)
				{
					this.entList = new List<Entity>();
				}
				else
				{
					this.entList.Clear();
				}
				world.GetEntitiesInBounds(typeof(EntityPlayer), new Bounds(base.ToWorldPos().ToVector3(), Vector3.one * 16f), this.entList);
				if (this.entList.Count > 0)
				{
					this.worldTimeTouched = world.worldTime;
					this.setModified();
					return;
				}
				return;
			}
			else
			{
				this.bWasTouched = false;
				this.bTouched = false;
				this.setModified();
			}
		}
	}

	// Token: 0x060056CD RID: 22221 RVA: 0x00235356 File Offset: 0x00233556
	public Vector2i GetContainerSize()
	{
		return this.containerSize;
	}

	// Token: 0x060056CE RID: 22222 RVA: 0x00235360 File Offset: 0x00233560
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

	// Token: 0x060056CF RID: 22223 RVA: 0x002353E0 File Offset: 0x002335E0
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		if (_eStreamMode == TileEntity.StreamModeRead.Persistency && this.readVersion <= 8)
		{
			throw new Exception("Outdated loot data");
		}
		if (_br.ReadBoolean())
		{
			this.lootListName = _br.ReadString();
		}
		this.containerSize = default(Vector2i);
		this.containerSize.x = (int)_br.ReadUInt16();
		this.containerSize.y = (int)_br.ReadUInt16();
		this.bTouched = _br.ReadBoolean();
		this.worldTimeTouched = (ulong)_br.ReadUInt32();
		this.bPlayerBackpack = _br.ReadBoolean();
		int num = Math.Min((int)_br.ReadInt16(), this.containerSize.x * this.containerSize.y);
		if (this.bUserAccessing)
		{
			ItemStack itemStack = ItemStack.Empty.Clone();
			if (_eStreamMode == TileEntity.StreamModeRead.Persistency && this.readVersion < 3)
			{
				for (int i = 0; i < num; i++)
				{
					itemStack.ReadOld(_br);
				}
			}
			else
			{
				for (int j = 0; j < num; j++)
				{
					itemStack.Read(_br);
				}
			}
		}
		else
		{
			if (this.containerSize.x * this.containerSize.y != this.items.Length)
			{
				this.items = ItemStack.CreateArray(this.containerSize.x * this.containerSize.y);
			}
			if (_eStreamMode == TileEntity.StreamModeRead.Persistency && this.readVersion < 3)
			{
				for (int k = 0; k < num; k++)
				{
					this.items[k].Clear();
					this.items[k].ReadOld(_br);
				}
			}
			else
			{
				for (int l = 0; l < num; l++)
				{
					this.items[l].Clear();
					this.items[l].Read(_br);
				}
			}
		}
		this.bPlayerStorage = _br.ReadBoolean();
		if ((_eStreamMode != TileEntity.StreamModeRead.Persistency || this.readVersion > 9) && _br.ReadBoolean())
		{
			this.preferences = new PreferenceTracker(-1);
			this.preferences.Read(_br);
		}
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency || this.readVersion >= 12)
		{
			this.SlotLocks = new PackedBoolArray(0);
			this.SlotLocks.Read(_br);
			return;
		}
		this.SlotLocks = new PackedBoolArray(this.items.Length);
	}

	// Token: 0x060056D0 RID: 22224 RVA: 0x00235608 File Offset: 0x00233808
	public override void write(PooledBinaryWriter stream, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(stream, _eStreamMode);
		bool flag = !string.IsNullOrEmpty(this.lootListName);
		stream.Write(flag);
		if (flag)
		{
			stream.Write(this.lootListName);
		}
		stream.Write((ushort)this.containerSize.x);
		stream.Write((ushort)this.containerSize.y);
		stream.Write(this.bTouched);
		stream.Write((uint)this.worldTimeTouched);
		stream.Write(this.bPlayerBackpack);
		stream.Write((short)this.items.Length);
		ItemStack[] items = this.items;
		for (int i = 0; i < items.Length; i++)
		{
			items[i].Clone().Write(stream);
		}
		stream.Write(this.bPlayerStorage);
		bool flag2 = this.preferences != null;
		stream.Write(flag2);
		if (flag2)
		{
			this.preferences.Write(stream);
		}
		if (this.SlotLocks == null)
		{
			this.SlotLocks = new PackedBoolArray(this.items.Length);
		}
		this.SlotLocks.Write(stream);
	}

	// Token: 0x060056D1 RID: 22225 RVA: 0x00075E2B File Offset: 0x0007402B
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Loot;
	}

	// Token: 0x060056D2 RID: 22226 RVA: 0x00235713 File Offset: 0x00233913
	public ItemStack[] GetItems()
	{
		return this.items;
	}

	// Token: 0x060056D3 RID: 22227 RVA: 0x0023571C File Offset: 0x0023391C
	public override void UpgradeDowngradeFrom(TileEntity _other)
	{
		base.UpgradeDowngradeFrom(_other);
		this.OnDestroy();
		if (_other is TileEntityLootContainer)
		{
			TileEntityLootContainer tileEntityLootContainer = _other as TileEntityLootContainer;
			this.bTouched = tileEntityLootContainer.bTouched;
			this.worldTimeTouched = tileEntityLootContainer.worldTimeTouched;
			this.bPlayerBackpack = tileEntityLootContainer.bPlayerBackpack;
			this.bPlayerStorage = tileEntityLootContainer.bPlayerStorage;
			this.items = ItemStack.Clone(tileEntityLootContainer.items, 0, this.containerSize.x * this.containerSize.y);
			if (this.items.Length != this.containerSize.x * this.containerSize.y)
			{
				Log.Error("UpgradeDowngradeFrom: other.size={0}, other.length={1}, this.size={2}, this.length={3}", new object[]
				{
					tileEntityLootContainer.containerSize,
					tileEntityLootContainer.items.Length,
					this.containerSize,
					this.items.Length
				});
			}
			if (tileEntityLootContainer.HasSlotLocksSupport && tileEntityLootContainer.SlotLocks != null)
			{
				this.SlotLocks = tileEntityLootContainer.SlotLocks.Clone();
				this.SlotLocks.Length = this.items.Length;
				return;
			}
			this.SlotLocks = new PackedBoolArray(this.items.Length);
		}
	}

	// Token: 0x060056D4 RID: 22228 RVA: 0x00235858 File Offset: 0x00233A58
	public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
		base.ReplacedBy(_bvOld, _bvNew, _teNew);
		ITileEntityLootable tileEntityLootable;
		if (!_teNew.TryGetSelfOrFeature(out tileEntityLootable))
		{
			GameManager.Instance.DropContentOfLootContainerServer(_bvOld, base.ToWorldPos(), base.EntityId, this);
		}
	}

	// Token: 0x060056D5 RID: 22229 RVA: 0x00235890 File Offset: 0x00233A90
	public void UpdateSlot(int _idx, ItemStack _item)
	{
		this.items[_idx] = _item.Clone();
		base.NotifyListeners();
	}

	// Token: 0x060056D6 RID: 22230 RVA: 0x002358A8 File Offset: 0x00233AA8
	public bool IsEmpty()
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

	// Token: 0x060056D7 RID: 22231 RVA: 0x002358DC File Offset: 0x00233ADC
	public void SetEmpty()
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			this.items[i].Clear();
		}
		PackedBoolArray slotLocks = this.SlotLocks;
		if (slotLocks != null)
		{
			slotLocks.Clear();
		}
		base.NotifyListeners();
		this.bTouched = true;
		this.setModified();
	}

	// Token: 0x060056D8 RID: 22232 RVA: 0x00235930 File Offset: 0x00233B30
	[return: TupleElementNames(new string[]
	{
		"anyMoved",
		"allMoved"
	})]
	public ValueTuple<bool, bool> TryStackItem(int startIndex, ItemStack _itemStack)
	{
		bool item = false;
		int count = _itemStack.count;
		int num = 0;
		for (int i = startIndex; i < this.items.Length; i++)
		{
			num = _itemStack.count;
			if (_itemStack.itemValue.type == this.items[i].itemValue.type && this.items[i].CanStackPartly(ref num))
			{
				this.items[i].count += num;
				_itemStack.count -= num;
				this.setModified();
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

	// Token: 0x060056D9 RID: 22233 RVA: 0x002359EC File Offset: 0x00233BEC
	public bool AddItem(ItemStack _item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].IsEmpty())
			{
				this.UpdateSlot(i, _item);
				base.SetModified();
				return true;
			}
		}
		return false;
	}

	// Token: 0x060056DA RID: 22234 RVA: 0x00235A2C File Offset: 0x00233C2C
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

	// Token: 0x060056DB RID: 22235 RVA: 0x00235A6C File Offset: 0x00233C6C
	public void RemoveItem(ItemValue _item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].itemValue.ItemClass == _item.ItemClass)
			{
				this.UpdateSlot(i, ItemStack.Empty.Clone());
			}
		}
	}

	// Token: 0x060056DC RID: 22236 RVA: 0x00235AB8 File Offset: 0x00233CB8
	public override void Reset(FastTags<TagGroup.Global> questTags)
	{
		base.Reset(questTags);
		if (this.bPlayerStorage || this.bPlayerBackpack)
		{
			return;
		}
		this.bTouched = false;
		this.bWasTouched = false;
		for (int i = 0; i < this.items.Length; i++)
		{
			this.items[i].Clear();
		}
		PackedBoolArray slotLocks = this.SlotLocks;
		if (slotLocks != null)
		{
			slotLocks.Clear();
		}
		BlockLoot blockLoot = base.blockValue.Block as BlockLoot;
		if (blockLoot != null && blockLoot.AlternateLootList != null)
		{
			for (int j = 0; j < blockLoot.AlternateLootList.Count; j++)
			{
				if (questTags.Test_AnySet(blockLoot.AlternateLootList[j].tag))
				{
					this.lootListName = blockLoot.AlternateLootList[j].lootEntry;
					break;
				}
			}
		}
		this.setModified();
	}

	// Token: 0x0400430D RID: 17165
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i containerSize;

	// Token: 0x0400430E RID: 17166
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] itemsArr;

	// Token: 0x04004316 RID: 17174
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> entList;
}
