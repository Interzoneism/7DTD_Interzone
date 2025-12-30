using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B07 RID: 2823
public class TileEntityPowerSource : TileEntityPowered
{
	// Token: 0x06005762 RID: 22370 RVA: 0x00237DE7 File Offset: 0x00235FE7
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x06005763 RID: 22371 RVA: 0x00237DFA File Offset: 0x00235FFA
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x06005764 RID: 22372 RVA: 0x00237E02 File Offset: 0x00236002
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.setModified();
	}

	// Token: 0x170008BC RID: 2236
	// (get) Token: 0x06005765 RID: 22373 RVA: 0x00237E14 File Offset: 0x00236014
	// (set) Token: 0x06005766 RID: 22374 RVA: 0x00237E5E File Offset: 0x0023605E
	public ItemClass SlotItem
	{
		get
		{
			if (this.slotItem == null)
			{
				this.slotItem = ItemClass.GetItemClass((this.chunk.GetBlock(base.localChunkPos).Block as BlockPowerSource).SlotItemName, false);
			}
			return this.slotItem;
		}
		set
		{
			this.slotItem = value;
		}
	}

	// Token: 0x06005767 RID: 22375 RVA: 0x00237E67 File Offset: 0x00236067
	public TileEntityPowerSource(Chunk _chunk) : base(_chunk)
	{
		this.ownerID = null;
	}

	// Token: 0x06005768 RID: 22376 RVA: 0x00237E89 File Offset: 0x00236089
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetSendSlots()
	{
		this.ClientData.SendSlots = true;
	}

	// Token: 0x06005769 RID: 22377 RVA: 0x00237E97 File Offset: 0x00236097
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPowerSource(TileEntityPowerSource _other) : base(null)
	{
		this.ownerID = _other.ownerID;
		this.PowerItem = _other.PowerItem;
	}

	// Token: 0x0600576A RID: 22378 RVA: 0x00237ECA File Offset: 0x002360CA
	public override TileEntity Clone()
	{
		return new TileEntityPowerSource(this);
	}

	// Token: 0x0600576B RID: 22379 RVA: 0x002322E1 File Offset: 0x002304E1
	public int GetEntityID()
	{
		return this.entityId;
	}

	// Token: 0x0600576C RID: 22380 RVA: 0x00234CF8 File Offset: 0x00232EF8
	public void SetEntityID(int _entityID)
	{
		this.entityId = _entityID;
	}

	// Token: 0x0600576D RID: 22381 RVA: 0x00237ED4 File Offset: 0x002360D4
	public override bool Activate(bool activated)
	{
		World world = GameManager.Instance.World;
		BlockValue block = this.chunk.GetBlock(base.localChunkPos);
		return block.Block.ActivateBlock(world, base.GetClrIdx(), base.ToWorldPos(), block, activated, activated);
	}

	// Token: 0x0600576E RID: 22382 RVA: 0x00237F1A File Offset: 0x0023611A
	public override bool CanHaveParent(IPowered powered)
	{
		return this.PowerItemType == PowerItem.PowerItemTypes.BatteryBank;
	}

	// Token: 0x0600576F RID: 22383 RVA: 0x00237F28 File Offset: 0x00236128
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (this.IsOn && base.IsByWater(world, base.ToWorldPos()))
		{
			(this.PowerItem as PowerSource).IsOn = false;
			base.SetModified();
		}
		if (this.bUserAccessing && this.IsOn)
		{
			base.SetModified();
		}
	}

	// Token: 0x06005770 RID: 22384 RVA: 0x00237F90 File Offset: 0x00236190
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		if (this.ClientData == null)
		{
			this.ClientData = new TileEntityPowerSource.ClientPowerData();
		}
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency)
		{
			if (_eStreamMode == TileEntity.StreamModeRead.FromClient)
			{
				this.bUserAccessing = _br.ReadBoolean();
				if (this.PowerItem == null)
				{
					this.PowerItem = base.CreatePowerItemForTileEntity((ushort)this.chunk.GetBlock(base.localChunkPos).type);
				}
				this.ClientData.AddedFuel = _br.ReadUInt16();
				if (this.ClientData.AddedFuel > 0)
				{
					ushort num = this.CurrentFuel + this.ClientData.AddedFuel;
					if (num > this.MaxFuel)
					{
						num = this.MaxFuel;
					}
					(this.PowerItem as PowerGenerator).CurrentFuel = num;
					this.ClientData.AddedFuel = 0;
					base.SetModified();
				}
				if (_br.ReadBoolean())
				{
					this.ClientData.ItemSlots = GameUtils.ReadItemStack(_br);
					(this.PowerItem as PowerSource).SetSlots(this.ClientData.ItemSlots);
					return;
				}
			}
			else if (_br.ReadBoolean())
			{
				this.ClientData.IsOn = _br.ReadBoolean();
				if (this.PowerItemType == PowerItem.PowerItemTypes.Generator)
				{
					this.ClientData.MaxFuel = _br.ReadUInt16();
					this.ClientData.CurrentFuel = _br.ReadUInt16();
				}
				else if (this.PowerItemType == PowerItem.PowerItemTypes.SolarPanel)
				{
					this.ClientData.SolarInput = _br.ReadUInt16();
				}
				ItemStack[] itemSlots = GameUtils.ReadItemStack(_br);
				if (!this.bUserAccessing || (this.bUserAccessing && this.IsOn))
				{
					this.ClientData.ItemSlots = itemSlots;
				}
				this.ClientData.MaxOutput = _br.ReadUInt16();
				this.ClientData.LastOutput = _br.ReadUInt16();
			}
		}
	}

	// Token: 0x06005771 RID: 22385 RVA: 0x00238150 File Offset: 0x00236350
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		if (_eStreamMode != TileEntity.StreamModeWrite.Persistency)
		{
			if (_eStreamMode == TileEntity.StreamModeWrite.ToServer)
			{
				_bw.Write(this.bUserAccessing);
				_bw.Write(this.ClientData.AddedFuel);
				this.ClientData.AddedFuel = 0;
				_bw.Write(this.ClientData.SendSlots);
				if (this.ClientData.SendSlots)
				{
					GameUtils.WriteItemStack(_bw, this.ClientData.ItemSlots);
					this.ClientData.SendSlots = false;
					return;
				}
			}
			else
			{
				PowerSource powerSource = this.PowerItem as PowerSource;
				_bw.Write(powerSource != null);
				if (powerSource != null)
				{
					_bw.Write(powerSource.IsOn);
					if (this.PowerItemType == PowerItem.PowerItemTypes.Generator)
					{
						_bw.Write((powerSource as PowerGenerator).MaxFuel);
						_bw.Write((powerSource as PowerGenerator).CurrentFuel);
					}
					else if (this.PowerItemType == PowerItem.PowerItemTypes.SolarPanel)
					{
						_bw.Write((powerSource as PowerSolarPanel).InputFromSun);
					}
					GameUtils.WriteItemStack(_bw, powerSource.Stacks);
					_bw.Write(powerSource.MaxOutput);
					_bw.Write(powerSource.LastPowerUsed);
				}
			}
		}
	}

	// Token: 0x06005772 RID: 22386 RVA: 0x00238268 File Offset: 0x00236468
	public bool HasSlottedItems()
	{
		ItemStack[] itemSlots = this.ItemSlots;
		for (int i = 0; i < itemSlots.Length; i++)
		{
			if (!itemSlots[i].IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x170008BD RID: 2237
	// (get) Token: 0x06005773 RID: 22387 RVA: 0x00238298 File Offset: 0x00236498
	public bool IsOn
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				PowerSource powerSource = this.PowerItem as PowerSource;
				return powerSource != null && powerSource.IsOn;
			}
			return this.ClientData.IsOn;
		}
	}

	// Token: 0x170008BE RID: 2238
	// (get) Token: 0x06005774 RID: 22388 RVA: 0x002382D4 File Offset: 0x002364D4
	// (set) Token: 0x06005775 RID: 22389 RVA: 0x002382FE File Offset: 0x002364FE
	public ushort CurrentFuel
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (this.PowerItem as PowerGenerator).CurrentFuel;
			}
			return this.ClientData.CurrentFuel;
		}
		set
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				(this.PowerItem as PowerGenerator).CurrentFuel = value;
				return;
			}
			this.ClientData.CurrentFuel = value;
			base.SetModified();
		}
	}

	// Token: 0x170008BF RID: 2239
	// (get) Token: 0x06005776 RID: 22390 RVA: 0x00238330 File Offset: 0x00236530
	public ushort MaxFuel
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (this.PowerItem as PowerGenerator).MaxFuel;
			}
			return this.ClientData.MaxFuel;
		}
	}

	// Token: 0x170008C0 RID: 2240
	// (get) Token: 0x06005777 RID: 22391 RVA: 0x0023835A File Offset: 0x0023655A
	public ushort MaxOutput
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (this.PowerItem as PowerSource).MaxOutput;
			}
			return this.ClientData.MaxOutput;
		}
	}

	// Token: 0x170008C1 RID: 2241
	// (get) Token: 0x06005778 RID: 22392 RVA: 0x00238384 File Offset: 0x00236584
	public ushort LastOutput
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (this.PowerItem as PowerSource).LastPowerUsed;
			}
			return this.ClientData.LastOutput;
		}
	}

	// Token: 0x170008C2 RID: 2242
	// (get) Token: 0x06005779 RID: 22393 RVA: 0x002383AE File Offset: 0x002365AE
	// (set) Token: 0x0600577A RID: 22394 RVA: 0x002383D8 File Offset: 0x002365D8
	public ItemStack[] ItemSlots
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (this.PowerItem as PowerSource).Stacks;
			}
			return this.ClientData.ItemSlots;
		}
		set
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				(this.PowerItem as PowerSource).SetSlots(value);
				return;
			}
			this.ClientData.ItemSlots = value;
			base.SetModified();
		}
	}

	// Token: 0x0600577B RID: 22395 RVA: 0x0023840C File Offset: 0x0023660C
	public bool TryAddItemToSlot(ItemClass itemClass, ItemStack itemStack)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return (this.PowerItem as PowerSource).TryAddItemToSlot(itemClass, itemStack);
		}
		if (!this.IsOn)
		{
			for (int i = 0; i < this.ClientData.ItemSlots.Length; i++)
			{
				if (this.ClientData.ItemSlots[i].IsEmpty())
				{
					this.ClientData.ItemSlots[i] = itemStack;
					this.ClientData.SendSlots = true;
					base.SetModified();
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600577C RID: 22396 RVA: 0x00238490 File Offset: 0x00236690
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			PowerSource powerSource = this.PowerItem as PowerSource;
			powerSource.HandleDisconnect();
			PowerManager.Instance.RemovePowerNode(powerSource);
		}
	}

	// Token: 0x0600577D RID: 22397 RVA: 0x002384CC File Offset: 0x002366CC
	public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
		base.ReplacedBy(_bvOld, _bvNew, _teNew);
		TileEntityPowerSource tileEntityPowerSource;
		if (_teNew.TryGetSelfOrFeature(out tileEntityPowerSource))
		{
			return;
		}
		List<ItemStack> list = new List<ItemStack>();
		list.AddRange(this.ItemSlots);
		if (this.PowerItemType == PowerItem.PowerItemTypes.Generator && this.CurrentFuel > 0)
		{
			ItemValue itemValue = new ItemValue(ItemClass.GetItemWithTag(XUiC_PowerSourceStats.tag).Id, false);
			int value = itemValue.ItemClass.Stacknumber.Value;
			int num;
			for (int i = (int)this.CurrentFuel; i > 0; i -= num)
			{
				num = Mathf.Min(i, value);
				list.Add(new ItemStack(itemValue, num));
			}
		}
		Vector3 pos = base.ToWorldCenterPos();
		pos.y += 0.9f;
		GameManager.Instance.DropContentInLootContainerServer(-1, "DroppedLootContainer", pos, list.ToArray(), true);
	}

	// Token: 0x0600577E RID: 22398 RVA: 0x00163F5F File Offset: 0x0016215F
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.PowerSource;
	}

	// Token: 0x0400433F RID: 17215
	public bool syncNeeded = true;

	// Token: 0x04004340 RID: 17216
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x04004341 RID: 17217
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass slotItem;

	// Token: 0x04004342 RID: 17218
	public TileEntityPowerSource.ClientPowerData ClientData = new TileEntityPowerSource.ClientPowerData();

	// Token: 0x02000B08 RID: 2824
	public class ClientPowerData
	{
		// Token: 0x0600577F RID: 22399 RVA: 0x00238598 File Offset: 0x00236798
		public ClientPowerData()
		{
			for (int i = 0; i < this.ItemSlots.Length; i++)
			{
				this.ItemSlots[i] = ItemStack.Empty.Clone();
			}
		}

		// Token: 0x04004343 RID: 17219
		public bool IsOn;

		// Token: 0x04004344 RID: 17220
		public ushort MaxFuel;

		// Token: 0x04004345 RID: 17221
		public ushort CurrentFuel;

		// Token: 0x04004346 RID: 17222
		public ushort SolarInput;

		// Token: 0x04004347 RID: 17223
		public ushort MaxOutput;

		// Token: 0x04004348 RID: 17224
		public ushort LastOutput;

		// Token: 0x04004349 RID: 17225
		public ushort AddedFuel;

		// Token: 0x0400434A RID: 17226
		public bool SendSlots;

		// Token: 0x0400434B RID: 17227
		public ItemStack[] ItemSlots = new ItemStack[6];
	}
}
