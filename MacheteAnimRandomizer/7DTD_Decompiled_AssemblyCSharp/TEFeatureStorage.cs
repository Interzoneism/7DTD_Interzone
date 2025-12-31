using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200015C RID: 348
[Preserve]
public class TEFeatureStorage : TEFeatureAbs, ITileEntityLootable, ITileEntity, IInventory
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000A4C RID: 2636 RVA: 0x0004380D File Offset: 0x00041A0D
	// (set) Token: 0x06000A4D RID: 2637 RVA: 0x00043815 File Offset: 0x00041A15
	public float LootStageMod { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x06000A4E RID: 2638 RVA: 0x0004381E File Offset: 0x00041A1E
	// (set) Token: 0x06000A4F RID: 2639 RVA: 0x00043826 File Offset: 0x00041A26
	public float LootStageBonus { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06000A50 RID: 2640 RVA: 0x00043830 File Offset: 0x00041A30
	public override void Init(TileEntityComposite _parent, TileEntityFeatureData _featureData)
	{
		base.Init(_parent, _featureData);
		this.lockFeature = base.Parent.GetFeature<ILockable>();
		this.lockpickFeature = base.Parent.GetFeature<ILockPickable>();
		DynamicProperties props = _featureData.Props;
		if (!props.Values.ContainsKey(BlockLoot.PropLootList))
		{
			Log.Error("Block with name " + base.Parent.TeData.Block.GetBlockName() + " does not have a loot list");
		}
		else
		{
			this.lootListName = props.Values[BlockLoot.PropLootList];
		}
		float lootStageMod = 0f;
		float lootStageBonus = 0f;
		props.ParseFloat(BlockLoot.PropLootStageMod, ref lootStageMod);
		props.ParseFloat(BlockLoot.PropLootStageBonus, ref lootStageBonus);
		this.LootStageMod = lootStageMod;
		this.LootStageBonus = lootStageBonus;
		for (int i = 1; i < 99; i++)
		{
			string text = BlockLoot.PropAlternateLootList + i.ToString();
			if (!props.Values.ContainsKey(text))
			{
				break;
			}
			string text2 = "";
			if (props.Params1.ContainsKey(text))
			{
				text2 = props.Params1[text];
			}
			if (!string.IsNullOrEmpty(text2))
			{
				FastTags<TagGroup.Global> tag = FastTags<TagGroup.Global>.Parse(text2);
				if (this.AlternateLootList == null)
				{
					this.AlternateLootList = new List<BlockLoot.AlternateLootEntry>();
				}
				this.AlternateLootList.Add(new BlockLoot.AlternateLootEntry
				{
					tag = tag,
					lootEntry = props.Values[text]
				});
			}
		}
		this.SetContainerSize(LootContainer.GetLootContainer(this.lootListName, true).size, true);
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x000439C4 File Offset: 0x00041BC4
	public override void CopyFrom(TileEntityComposite _other)
	{
		base.CopyFrom(_other);
		ITileEntityLootable tileEntityLootable;
		if (_other.TryGetSelfOrFeature(out tileEntityLootable))
		{
			this.lootListName = tileEntityLootable.lootListName;
			this.containerSize = tileEntityLootable.GetContainerSize();
			this.items = ItemStack.Clone(tileEntityLootable.items);
			this.bPlayerBackpack = tileEntityLootable.bPlayerBackpack;
			this.worldTimeTouched = tileEntityLootable.worldTimeTouched;
			this.bTouched = tileEntityLootable.bTouched;
		}
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x00043A2F File Offset: 0x00041C2F
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (!GameManager.IsDedicatedServer)
		{
			XUiC_LootWindowGroup.CloseIfOpenAtPos(base.ToWorldPos(), null);
		}
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00043A4A File Offset: 0x00041C4A
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _placingEntity)
	{
		base.PlaceBlock(_world, _result, _placingEntity);
		if (_placingEntity != null && _placingEntity.entityType == EntityType.Player)
		{
			this.worldTimeTouched = _world.GetWorldTime();
			this.SetEmpty();
		}
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00043A7C File Offset: 0x00041C7C
	public override void UpgradeDowngradeFrom(TileEntityComposite _other)
	{
		base.UpgradeDowngradeFrom(_other);
		ITileEntityLootable feature = _other.GetFeature<ITileEntityLootable>();
		if (feature != null)
		{
			this.bTouched = feature.bTouched;
			this.worldTimeTouched = feature.worldTimeTouched;
			this.bPlayerBackpack = feature.bPlayerBackpack;
			this.migrateItemsFromOtherContainer(feature);
		}
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x00043AC8 File Offset: 0x00041CC8
	public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
		base.ReplacedBy(_bvOld, _bvNew, _teNew);
		ITileEntityLootable tileEntityLootable;
		if (!_teNew.TryGetSelfOrFeature(out tileEntityLootable))
		{
			GameManager.Instance.DropContentOfLootContainerServer(_bvOld, base.ToWorldPos(), this.EntityId, this);
		}
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x00043B00 File Offset: 0x00041D00
	[PublicizedFrom(EAccessModifier.Private)]
	public void migrateItemsFromOtherContainer(ITileEntityLootable _other)
	{
		this.items = ItemStack.Clone(_other.items, 0, this.containerSize.x * this.containerSize.y);
		if (this.items.Length < _other.items.Length)
		{
			List<ItemStack> list = new List<ItemStack>();
			for (int i = this.items.Length; i < _other.items.Length; i++)
			{
				list.Add(_other.items[i]);
			}
			Vector3 pos = base.ToWorldCenterPos();
			pos.y += 0.9f;
			GameManager.Instance.DropContentInLootContainerServer(-1, "DroppedLootContainer", pos, list.ToArray(), true);
		}
		if (_other.HasSlotLocksSupport)
		{
			this.SlotLocks = _other.SlotLocks.Clone();
			this.SlotLocks.Length = this.items.Length;
			return;
		}
		this.SlotLocks = new PackedBoolArray(this.items.Length);
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x00043BE8 File Offset: 0x00041DE8
	public override void Reset(FastTags<TagGroup.Global> _questTags)
	{
		base.Reset(_questTags);
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
		if (this.AlternateLootList != null)
		{
			for (int j = 0; j < this.AlternateLootList.Count; j++)
			{
				if (_questTags.Test_AnySet(this.AlternateLootList[j].tag))
				{
					this.lootListName = this.AlternateLootList[j].lootEntry;
					break;
				}
			}
		}
		base.SetModified();
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00043C94 File Offset: 0x00041E94
	public override void UpdateTick(World _world)
	{
		base.UpdateTick(_world);
		if (base.Parent.PlayerPlaced)
		{
			return;
		}
		if (!this.bTouched || !this.IsEmpty() || GamePrefs.GetInt(EnumGamePrefs.LootRespawnDays) <= 0)
		{
			return;
		}
		int num = GameUtils.WorldTimeToTotalHours(this.worldTimeTouched);
		if ((GameUtils.WorldTimeToTotalHours(_world.worldTime) - num) / 24 >= GamePrefs.GetInt(EnumGamePrefs.LootRespawnDays))
		{
			this.bWasTouched = false;
			this.bTouched = false;
			base.SetModified();
			return;
		}
		if (this.entityTempList == null)
		{
			this.entityTempList = new List<Entity>();
		}
		else
		{
			this.entityTempList.Clear();
		}
		_world.GetEntitiesInBounds(typeof(EntityPlayer), new Bounds(base.ToWorldPos().ToVector3(), Vector3.one * 16f), this.entityTempList);
		if (this.entityTempList.Count > 0)
		{
			this.worldTimeTouched = _world.worldTime;
			base.SetModified();
			return;
		}
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00043D88 File Offset: 0x00041F88
	public override string GetActivationText(WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityAlive _entityFocusing, string _activateHotkeyMarkup, string _focusedTileEntityName)
	{
		base.GetActivationText(_world, _blockPos, _blockValue, _entityFocusing, _activateHotkeyMarkup, _focusedTileEntityName);
		if (this.lockFeature == null)
		{
			if (!this.bTouched)
			{
				return string.Format(Localization.Get("lootTooltipNew", false), _activateHotkeyMarkup, _focusedTileEntityName);
			}
			if (this.IsEmpty())
			{
				return string.Format(Localization.Get("lootTooltipEmpty", false), _activateHotkeyMarkup, _focusedTileEntityName);
			}
			return string.Format(Localization.Get("lootTooltipTouched", false), _activateHotkeyMarkup, _focusedTileEntityName);
		}
		else
		{
			if (!this.lockFeature.IsLocked())
			{
				return string.Format(Localization.Get("tooltipUnlocked", false), _activateHotkeyMarkup, _focusedTileEntityName);
			}
			if (this.lockFeature.LocalPlayerIsOwner() || this.lockpickFeature != null)
			{
				return string.Format(Localization.Get("tooltipLocked", false), _activateHotkeyMarkup, _focusedTileEntityName);
			}
			if (this.lockFeature.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
			{
				return string.Format(Localization.Get("tooltipLocked", false), _activateHotkeyMarkup, _focusedTileEntityName);
			}
			return string.Format(Localization.Get("tooltipJammed", false), _activateHotkeyMarkup, _focusedTileEntityName);
		}
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x00043E83 File Offset: 0x00042083
	public override void InitBlockActivationCommands(Action<BlockActivationCommand, TileEntityComposite.EBlockCommandOrder, TileEntityFeatureData> _addCallback)
	{
		base.InitBlockActivationCommands(_addCallback);
		_addCallback(new BlockActivationCommand("Search", "search", true, false, null), TileEntityComposite.EBlockCommandOrder.Normal, base.FeatureData);
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00043EAC File Offset: 0x000420AC
	public override bool OnBlockActivated(ReadOnlySpan<char> _commandName, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		base.OnBlockActivated(_commandName, _world, _blockPos, _blockValue, _player);
		if (!base.CommandIs(_commandName, "Search"))
		{
			return false;
		}
		if (this.lockFeature != null && this.lockFeature.IsLocked() && !this.lockFeature.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
		{
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, "Misc/locked");
			return false;
		}
		_player.AimingGun = false;
		Vector3i blockPos = base.Parent.ToWorldPos();
		this.bWasTouched = this.bTouched;
		_world.GetGameManager().TELockServer(0, blockPos, base.Parent.EntityId, _player.entityId, "container");
		return true;
	}

	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000A5C RID: 2652 RVA: 0x00043F6D File Offset: 0x0004216D
	// (set) Token: 0x06000A5D RID: 2653 RVA: 0x00043F75 File Offset: 0x00042175
	public string lootListName { get; set; }

	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x06000A5E RID: 2654 RVA: 0x00043F7E File Offset: 0x0004217E
	// (set) Token: 0x06000A5F RID: 2655 RVA: 0x00043F86 File Offset: 0x00042186
	public bool bPlayerBackpack { get; set; }

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x06000A60 RID: 2656 RVA: 0x00043F8F File Offset: 0x0004218F
	// (set) Token: 0x06000A61 RID: 2657 RVA: 0x00002914 File Offset: 0x00000B14
	public bool bPlayerStorage
	{
		get
		{
			return base.Parent.Owner != null;
		}
		set
		{
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x06000A62 RID: 2658 RVA: 0x00043F9F File Offset: 0x0004219F
	// (set) Token: 0x06000A63 RID: 2659 RVA: 0x00043FA7 File Offset: 0x000421A7
	public PreferenceTracker preferences { get; set; }

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000A64 RID: 2660 RVA: 0x00043FB0 File Offset: 0x000421B0
	// (set) Token: 0x06000A65 RID: 2661 RVA: 0x00043FB8 File Offset: 0x000421B8
	public ulong worldTimeTouched { get; set; }

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000A66 RID: 2662 RVA: 0x00043FC1 File Offset: 0x000421C1
	// (set) Token: 0x06000A67 RID: 2663 RVA: 0x00043FD3 File Offset: 0x000421D3
	public bool bTouched
	{
		get
		{
			return this.bPlayerStorage || this.internalTouched;
		}
		set
		{
			this.internalTouched = value;
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000A68 RID: 2664 RVA: 0x00043FDC File Offset: 0x000421DC
	// (set) Token: 0x06000A69 RID: 2665 RVA: 0x00043FE4 File Offset: 0x000421E4
	public bool bWasTouched { get; set; }

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000A6A RID: 2666 RVA: 0x00043FF0 File Offset: 0x000421F0
	// (set) Token: 0x06000A6B RID: 2667 RVA: 0x0004402C File Offset: 0x0004222C
	public ItemStack[] items
	{
		get
		{
			ItemStack[] result;
			if ((result = this.itemsArr) == null)
			{
				result = (this.itemsArr = ItemStack.CreateArray(this.containerSize.x * this.containerSize.y));
			}
			return result;
		}
		set
		{
			this.itemsArr = value;
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00044035 File Offset: 0x00042235
	public virtual Vector2i GetContainerSize()
	{
		return this.containerSize;
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00044040 File Offset: 0x00042240
	public virtual void SetContainerSize(Vector2i _containerSize, bool _clearItems = true)
	{
		this.containerSize = _containerSize;
		if (!_clearItems)
		{
			return;
		}
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

	// Token: 0x06000A6E RID: 2670 RVA: 0x000440C1 File Offset: 0x000422C1
	public void UpdateSlot(int _idx, ItemStack _item)
	{
		this.items[_idx] = _item.Clone();
		base.Parent.NotifyListeners();
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x000440DC File Offset: 0x000422DC
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

	// Token: 0x06000A70 RID: 2672 RVA: 0x00044128 File Offset: 0x00042328
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

	// Token: 0x06000A71 RID: 2673 RVA: 0x0004415C File Offset: 0x0004235C
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
		base.Parent.NotifyListeners();
		this.bTouched = true;
		base.SetModified();
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000A72 RID: 2674 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool HasSlotLocksSupport
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000A73 RID: 2675 RVA: 0x000441B2 File Offset: 0x000423B2
	// (set) Token: 0x06000A74 RID: 2676 RVA: 0x000441BA File Offset: 0x000423BA
	public PackedBoolArray SlotLocks { get; set; }

	// Token: 0x06000A75 RID: 2677 RVA: 0x000441C4 File Offset: 0x000423C4
	public bool AddItem(ItemStack _itemStack)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			if (this.items[i].IsEmpty())
			{
				this.UpdateSlot(i, _itemStack);
				base.SetModified();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x00044204 File Offset: 0x00042404
	[return: TupleElementNames(new string[]
	{
		"anyMoved",
		"allMoved"
	})]
	public ValueTuple<bool, bool> TryStackItem(int _startIndex, ItemStack _itemStack)
	{
		bool item = false;
		int count = _itemStack.count;
		for (int i = _startIndex; i < this.items.Length; i++)
		{
			int count2 = _itemStack.count;
			if (_itemStack.itemValue.type == this.items[i].itemValue.type && this.items[i].CanStackPartly(ref count2))
			{
				this.items[i].count += count2;
				_itemStack.count -= count2;
				if (_itemStack.count == 0)
				{
					break;
				}
			}
		}
		if (_itemStack.count != count)
		{
			item = true;
			base.SetModified();
			base.Parent.NotifyListeners();
		}
		return new ValueTuple<bool, bool>(item, _itemStack.count == 0);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x000442BC File Offset: 0x000424BC
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

	// Token: 0x06000A78 RID: 2680 RVA: 0x000442FC File Offset: 0x000424FC
	public override void Read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode, int _readVersion)
	{
		base.Read(_br, _eStreamMode, _readVersion);
		if (_br.ReadBoolean())
		{
			this.lootListName = _br.ReadString();
		}
		this.containerSize = new Vector2i
		{
			x = (int)_br.ReadUInt16(),
			y = (int)_br.ReadUInt16()
		};
		this.bTouched = _br.ReadBoolean();
		this.worldTimeTouched = (ulong)_br.ReadUInt32();
		_br.ReadBoolean();
		int num = Math.Min((int)_br.ReadInt16(), this.containerSize.x * this.containerSize.y);
		if (base.Parent.IsUserAccessing())
		{
			ItemStack itemStack = ItemStack.Empty.Clone();
			for (int i = 0; i < num; i++)
			{
				itemStack.Read(_br);
			}
		}
		else
		{
			if (this.containerSize.x * this.containerSize.y != this.items.Length)
			{
				this.items = ItemStack.CreateArray(this.containerSize.x * this.containerSize.y);
			}
			for (int j = 0; j < num; j++)
			{
				this.items[j].Clear();
				this.items[j].Read(_br);
			}
		}
		if (_br.ReadBoolean())
		{
			this.preferences = new PreferenceTracker(-1);
			this.preferences.Read(_br);
		}
		if (_readVersion >= 12 || _eStreamMode != TileEntity.StreamModeRead.Persistency)
		{
			this.SlotLocks = new PackedBoolArray(0);
			this.SlotLocks.Read(_br);
			return;
		}
		this.SlotLocks = new PackedBoolArray(this.items.Length);
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x00044488 File Offset: 0x00042688
	public override void Write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.Write(_bw, _eStreamMode);
		bool flag = !string.IsNullOrEmpty(this.lootListName);
		_bw.Write(flag);
		if (flag)
		{
			_bw.Write(this.lootListName);
		}
		_bw.Write((ushort)this.containerSize.x);
		_bw.Write((ushort)this.containerSize.y);
		_bw.Write(this.bTouched);
		_bw.Write((uint)this.worldTimeTouched);
		_bw.Write(false);
		_bw.Write((short)this.items.Length);
		ItemStack[] items = this.items;
		for (int i = 0; i < items.Length; i++)
		{
			items[i].Clone().Write(_bw);
		}
		bool flag2 = this.preferences != null;
		_bw.Write(flag2);
		if (flag2)
		{
			this.preferences.Write(_bw);
		}
		if (this.SlotLocks == null)
		{
			this.SlotLocks = new PackedBoolArray(this.items.Length);
		}
		this.SlotLocks.Write(_bw);
	}

	// Token: 0x0400092E RID: 2350
	[PublicizedFrom(EAccessModifier.Private)]
	public ILockable lockFeature;

	// Token: 0x0400092F RID: 2351
	[PublicizedFrom(EAccessModifier.Private)]
	public ILockPickable lockpickFeature;

	// Token: 0x04000932 RID: 2354
	public List<BlockLoot.AlternateLootEntry> AlternateLootList;

	// Token: 0x04000933 RID: 2355
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> entityTempList;

	// Token: 0x04000934 RID: 2356
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i containerSize = Vector2i.one;

	// Token: 0x04000935 RID: 2357
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] itemsArr;

	// Token: 0x0400093A RID: 2362
	[PublicizedFrom(EAccessModifier.Private)]
	public bool internalTouched;
}
