using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// Token: 0x02000B03 RID: 2819
public class TileEntityPoweredRangedTrap : TileEntityPoweredBlock
{
	// Token: 0x170008AA RID: 2218
	// (get) Token: 0x0600572B RID: 22315 RVA: 0x00236E1C File Offset: 0x0023501C
	// (set) Token: 0x0600572C RID: 22316 RVA: 0x00236E8B File Offset: 0x0023508B
	public ItemClass AmmoItem
	{
		get
		{
			if (this.ammoItem == null)
			{
				Block block = this.chunk.GetBlock(base.localChunkPos).Block;
				BlockLauncher blockLauncher = block as BlockLauncher;
				if (blockLauncher != null)
				{
					this.ammoItem = ItemClass.GetItemClass(blockLauncher.AmmoItemName, false);
				}
				else
				{
					BlockRanged blockRanged = block as BlockRanged;
					if (blockRanged != null)
					{
						this.ammoItem = ItemClass.GetItemClass(blockRanged.AmmoItemName, false);
					}
				}
			}
			return this.ammoItem;
		}
		set
		{
			this.ammoItem = value;
		}
	}

	// Token: 0x0600572D RID: 22317 RVA: 0x00236E94 File Offset: 0x00235094
	public TileEntityPoweredRangedTrap(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x170008AB RID: 2219
	// (get) Token: 0x0600572E RID: 22318 RVA: 0x00236EB6 File Offset: 0x002350B6
	// (set) Token: 0x0600572F RID: 22319 RVA: 0x00236ECD File Offset: 0x002350CD
	public int OwnerEntityID
	{
		get
		{
			if (this.ownerEntityID == -1)
			{
				this.SetOwnerEntityID();
			}
			return this.ownerEntityID;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.ownerEntityID = value;
		}
	}

	// Token: 0x06005730 RID: 22320 RVA: 0x00236ED6 File Offset: 0x002350D6
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x06005731 RID: 22321 RVA: 0x00236EE9 File Offset: 0x002350E9
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x06005732 RID: 22322 RVA: 0x00236EF1 File Offset: 0x002350F1
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.SetOwnerEntityID();
		this.setModified();
	}

	// Token: 0x06005733 RID: 22323 RVA: 0x00236F06 File Offset: 0x00235106
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetSendSlots()
	{
		this.ClientData.SendSlots = true;
	}

	// Token: 0x06005734 RID: 22324 RVA: 0x00002914 File Offset: 0x00000B14
	public override void OnSetLocalChunkPosition()
	{
	}

	// Token: 0x06005735 RID: 22325 RVA: 0x00236F14 File Offset: 0x00235114
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetOwnerEntityID()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
		PersistentPlayerData persistentPlayerData = (persistentPlayerList != null) ? persistentPlayerList.GetPlayerData(this.ownerID) : null;
		if (persistentPlayerData != null)
		{
			this.ownerEntityID = persistentPlayerData.EntityId;
			return;
		}
		this.ownerEntityID = -1;
	}

	// Token: 0x06005736 RID: 22326 RVA: 0x00236F64 File Offset: 0x00235164
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.SetOwnerEntityID();
		if (_eStreamMode != TileEntity.StreamModeRead.Persistency)
		{
			if (_eStreamMode == TileEntity.StreamModeRead.FromClient)
			{
				this.bUserAccessing = _br.ReadBoolean();
				if (this.PowerItem == null)
				{
					this.PowerItem = base.CreatePowerItemForTileEntity((ushort)this.chunk.GetBlock(base.localChunkPos).type);
				}
				(this.PowerItem as PowerRangedTrap).IsLocked = _br.ReadBoolean();
				if (_br.ReadBoolean())
				{
					this.ClientData.ItemSlots = GameUtils.ReadItemStack(_br);
					(this.PowerItem as PowerRangedTrap).SetSlots(this.ClientData.ItemSlots);
				}
				this.TargetType = _br.ReadInt32();
				return;
			}
			bool flag = _br.ReadBoolean();
			bool flag2 = !this.bUserAccessing || (this.bUserAccessing && this.ClientData.IsLocked);
			if (flag)
			{
				this.ClientData.IsLocked = _br.ReadBoolean();
				ItemStack[] itemSlots = GameUtils.ReadItemStack(_br);
				if (flag2)
				{
					this.ClientData.ItemSlots = itemSlots;
				}
			}
			int targetType = _br.ReadInt32();
			if (!this.bUserAccessing)
			{
				this.TargetType = targetType;
			}
		}
	}

	// Token: 0x06005737 RID: 22327 RVA: 0x00237094 File Offset: 0x00235294
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		this.ownerID.ToStream(_bw, false);
		if (_eStreamMode != TileEntity.StreamModeWrite.Persistency)
		{
			if (_eStreamMode == TileEntity.StreamModeWrite.ToServer)
			{
				_bw.Write(this.bUserAccessing);
				_bw.Write(this.IsLocked);
				_bw.Write(this.ClientData.SendSlots);
				if (this.ClientData.SendSlots)
				{
					GameUtils.WriteItemStack(_bw, this.ClientData.ItemSlots);
					this.ClientData.SendSlots = false;
				}
				_bw.Write(this.TargetType);
				return;
			}
			PowerRangedTrap powerRangedTrap = this.PowerItem as PowerRangedTrap;
			_bw.Write(powerRangedTrap != null);
			if (powerRangedTrap != null)
			{
				_bw.Write(powerRangedTrap.IsLocked);
				GameUtils.WriteItemStack(_bw, powerRangedTrap.Stacks);
			}
			_bw.Write(this.TargetType);
		}
	}

	// Token: 0x170008AC RID: 2220
	// (get) Token: 0x06005738 RID: 22328 RVA: 0x0023715D File Offset: 0x0023535D
	// (set) Token: 0x06005739 RID: 22329 RVA: 0x00237187 File Offset: 0x00235387
	public bool IsLocked
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (this.PowerItem as PowerRangedTrap).IsLocked;
			}
			return this.ClientData.IsLocked;
		}
		set
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				(this.PowerItem as PowerRangedTrap).IsLocked = value;
				return;
			}
			this.ClientData.IsLocked = value;
			base.SetModified();
		}
	}

	// Token: 0x170008AD RID: 2221
	// (get) Token: 0x0600573A RID: 22330 RVA: 0x002371B9 File Offset: 0x002353B9
	// (set) Token: 0x0600573B RID: 22331 RVA: 0x002371E3 File Offset: 0x002353E3
	public ItemStack[] ItemSlots
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (this.PowerItem as PowerRangedTrap).Stacks;
			}
			return this.ClientData.ItemSlots;
		}
		set
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				(this.PowerItem as PowerRangedTrap).SetSlots(value);
				return;
			}
			this.ClientData.ItemSlots = value;
			base.SetModified();
		}
	}

	// Token: 0x170008AE RID: 2222
	// (get) Token: 0x0600573C RID: 22332 RVA: 0x00237215 File Offset: 0x00235415
	// (set) Token: 0x0600573D RID: 22333 RVA: 0x0023723F File Offset: 0x0023543F
	public int TargetType
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return (int)(this.PowerItem as PowerRangedTrap).TargetType;
			}
			return this.ClientData.TargetType;
		}
		set
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				(this.PowerItem as PowerRangedTrap).TargetType = (PowerRangedTrap.TargetTypes)value;
				return;
			}
			this.ClientData.TargetType = value;
		}
	}

	// Token: 0x170008AF RID: 2223
	// (get) Token: 0x0600573E RID: 22334 RVA: 0x0023726B File Offset: 0x0023546B
	public bool TargetSelf
	{
		get
		{
			return (this.TargetType & 1) == 1;
		}
	}

	// Token: 0x170008B0 RID: 2224
	// (get) Token: 0x0600573F RID: 22335 RVA: 0x00237278 File Offset: 0x00235478
	public bool TargetAllies
	{
		get
		{
			return (this.TargetType & 2) == 2;
		}
	}

	// Token: 0x170008B1 RID: 2225
	// (get) Token: 0x06005740 RID: 22336 RVA: 0x00237285 File Offset: 0x00235485
	public bool TargetStrangers
	{
		get
		{
			return (this.TargetType & 4) == 4;
		}
	}

	// Token: 0x170008B2 RID: 2226
	// (get) Token: 0x06005741 RID: 22337 RVA: 0x00237292 File Offset: 0x00235492
	public bool TargetZombies
	{
		get
		{
			return (this.TargetType & 8) == 8;
		}
	}

	// Token: 0x06005742 RID: 22338 RVA: 0x002372A0 File Offset: 0x002354A0
	public bool TryStackItem(ItemStack itemStack)
	{
		if (this.IsLocked)
		{
			return false;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return (this.PowerItem as PowerRangedTrap).TryStackItem(itemStack);
		}
		for (int i = 0; i < this.ClientData.ItemSlots.Length; i++)
		{
			int count = itemStack.count;
			if (this.ClientData.ItemSlots[i].IsEmpty())
			{
				this.ClientData.ItemSlots[i] = itemStack.Clone();
				this.ClientData.SendSlots = true;
				base.SetModified();
				itemStack.count = 0;
				return true;
			}
			if (this.ClientData.ItemSlots[i].itemValue.type == itemStack.itemValue.type && this.ClientData.ItemSlots[i].CanStackPartly(ref count))
			{
				this.ClientData.ItemSlots[i].count += count;
				itemStack.count -= count;
				this.ClientData.SendSlots = true;
				base.SetModified();
				if (itemStack.count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06005743 RID: 22339 RVA: 0x002373BC File Offset: 0x002355BC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetValuesFromBlock(ushort blockID)
	{
		base.SetValuesFromBlock(blockID);
		if (Block.list[(int)blockID].Properties.Values.ContainsKey("BurstFireRate"))
		{
			this.DelayTime = StringParsers.ParseFloat(Block.list[(int)blockID].Properties.Values["BurstFireRate"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.DelayTime = 0.5f;
		}
		if (Block.list[(int)blockID].Properties.Values.ContainsKey("ShowTargeting"))
		{
			this.ShowTargeting = StringParsers.ParseBool(Block.list[(int)blockID].Properties.Values["ShowTargeting"], 0, -1, true);
			return;
		}
		this.ShowTargeting = true;
	}

	// Token: 0x06005744 RID: 22340 RVA: 0x00237475 File Offset: 0x00235675
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.PowerRangeTrap;
	}

	// Token: 0x06005745 RID: 22341 RVA: 0x0023747C File Offset: 0x0023567C
	[PublicizedFrom(EAccessModifier.Internal)]
	public bool DecrementAmmo()
	{
		for (int i = 0; i < this.ItemSlots.Length; i++)
		{
			if (this.ItemSlots[i].count > 0)
			{
				this.ItemSlots[i].count--;
				if (this.ItemSlots[i].count == 0)
				{
					this.ItemSlots[i] = ItemStack.Empty.Clone();
				}
				base.SetModified();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005746 RID: 22342 RVA: 0x002374EC File Offset: 0x002356EC
	public bool AddItem(ItemStack itemStack)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return (this.PowerItem as PowerRangedTrap).AddItem(itemStack);
		}
		for (int i = 0; i < this.ItemSlots.Length; i++)
		{
			if (this.ItemSlots[i].IsEmpty())
			{
				this.ItemSlots[i] = itemStack;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06005747 RID: 22343 RVA: 0x00237548 File Offset: 0x00235748
	public override void ClientUpdate()
	{
		if (Time.time > this.updateTime)
		{
			this.updateTime = Time.time + this.DelayTime;
			World world = GameManager.Instance.World;
			BlockValue block = this.chunk.GetBlock(base.localChunkPos);
			Block block2 = block.Block;
			BlockLauncher blockLauncher = block2 as BlockLauncher;
			if (blockLauncher != null)
			{
				blockLauncher.InstantiateProjectile(world, base.GetClrIdx(), base.ToWorldPos());
				return;
			}
			if (block2 is BlockRanged)
			{
				block2.ActivateBlock(world, base.GetClrIdx(), base.ToWorldPos(), block, base.IsPowered, base.IsPowered);
			}
		}
	}

	// Token: 0x06005748 RID: 22344 RVA: 0x002375E4 File Offset: 0x002357E4
	public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
		base.ReplacedBy(_bvOld, _bvNew, _teNew);
		TileEntityPoweredRangedTrap tileEntityPoweredRangedTrap;
		if (_teNew.TryGetSelfOrFeature(out tileEntityPoweredRangedTrap))
		{
			return;
		}
		List<ItemStack> list = new List<ItemStack>();
		list.AddRange(this.ItemSlots);
		Vector3 pos = base.ToWorldCenterPos();
		pos.y += 0.9f;
		GameManager.Instance.DropContentInLootContainerServer(-1, "DroppedLootContainer", pos, list.ToArray(), true);
	}

	// Token: 0x0400432D RID: 17197
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass ammoItem;

	// Token: 0x0400432E RID: 17198
	public readonly TileEntityPoweredRangedTrap.ClientAmmoData ClientData = new TileEntityPoweredRangedTrap.ClientAmmoData();

	// Token: 0x0400432F RID: 17199
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x04004330 RID: 17200
	[PublicizedFrom(EAccessModifier.Private)]
	public int ownerEntityID = -1;

	// Token: 0x04004331 RID: 17201
	public bool ShowTargeting = true;

	// Token: 0x02000B04 RID: 2820
	public class ClientAmmoData
	{
		// Token: 0x06005749 RID: 22345 RVA: 0x00237648 File Offset: 0x00235848
		public ClientAmmoData()
		{
			for (int i = 0; i < this.ItemSlots.Length; i++)
			{
				this.ItemSlots[i] = ItemStack.Empty.Clone();
			}
		}

		// Token: 0x04004332 RID: 17202
		public bool IsLocked;

		// Token: 0x04004333 RID: 17203
		public bool SendSlots;

		// Token: 0x04004334 RID: 17204
		public ItemStack[] ItemSlots = new ItemStack[3];

		// Token: 0x04004335 RID: 17205
		public int TargetType = 12;
	}
}
