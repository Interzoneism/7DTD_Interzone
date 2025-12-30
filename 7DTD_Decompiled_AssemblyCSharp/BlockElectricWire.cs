using System;
using System.Collections.Generic;
using System.Globalization;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000FF RID: 255
[Preserve]
public class BlockElectricWire : BlockPowered
{
	// Token: 0x060006B4 RID: 1716 RVA: 0x0002F567 File Offset: 0x0002D767
	public BlockElectricWire()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x0002F578 File Offset: 0x0002D778
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("BrokenPercentage"))
		{
			this.brokenPercentage = Mathf.Clamp01(StringParsers.ParseFloat(base.Properties.Values["BrokenPercentage"], 0, -1, NumberStyles.Any));
			return;
		}
		this.brokenPercentage = 0.25f;
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x0002F5DA File Offset: 0x0002D7DA
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		return new TileEntityPoweredMeleeTrap(chunk)
		{
			PowerItemType = PowerItem.PowerItemTypes.ElectricWireRelay
		};
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x0002F5EC File Offset: 0x0002D7EC
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityPoweredMeleeTrap tileEntityPoweredMeleeTrap = _world.GetTileEntity(_result.clrIdx, _result.blockPos) as TileEntityPoweredMeleeTrap;
		if (tileEntityPoweredMeleeTrap != null && _ea != null && _ea.entityType == EntityType.Player)
		{
			tileEntityPoweredMeleeTrap.SetOwner(PlatformManager.InternalLocalUserIdentifier);
		}
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0002F63C File Offset: 0x0002D83C
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (!(_world.GetTileEntity(_chunk.ClrIdx, _blockPos) is TileEntityPoweredMeleeTrap))
		{
			TileEntityPowered tileEntityPowered = this.CreateTileEntity(_chunk);
			tileEntityPowered.localChunkPos = World.toBlock(_blockPos);
			tileEntityPowered.InitializePowerData();
			_chunk.AddTileEntity(tileEntityPowered);
		}
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x0002F68C File Offset: 0x0002D88C
	public override int OnBlockDamaged(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _damagePoints, int _entityIdThatDamaged, ItemActionAttack.AttackHitInfo _attackHitInfo, bool _bUseHarvestTool, bool _bBypassMaxDamage, int _recDepth = 0)
	{
		if ((_blockValue.meta & 2) > 0 && 1f - (float)_blockValue.damage / (float)_blockValue.Block.MaxDamage > this.brokenPercentage)
		{
			if (this.buffActions == null && base.Properties.Values.ContainsKey("Buff"))
			{
				string[] array = base.Properties.Values["Buff"].Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					this.buffActions.Add(array[i]);
				}
			}
			if (this.buffActions != null)
			{
				TileEntityPoweredMeleeTrap tileEntityPoweredMeleeTrap = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityPoweredMeleeTrap;
				if (tileEntityPoweredMeleeTrap != null && tileEntityPoweredMeleeTrap.IsPowered)
				{
					EntityAlive entityAlive = _world.GetEntity(_entityIdThatDamaged) as EntityAlive;
					if (entityAlive != null)
					{
						ItemAction itemAction = entityAlive.inventory.holdingItemData.item.Actions[0];
						if (entityAlive != null)
						{
							if (itemAction is ItemActionRanged)
							{
								ItemActionRanged itemActionRanged = itemAction as ItemActionRanged;
								if (itemActionRanged == null || (itemActionRanged.Hitmask & 128) == 0)
								{
									goto IL_150;
								}
							}
							for (int j = 0; j < this.buffActions.Count; j++)
							{
								entityAlive.Buffs.AddBuff(this.buffActions[j], tileEntityPoweredMeleeTrap.OwnerEntityID, true, false, -1f);
							}
						}
					}
				}
			}
		}
		IL_150:
		return base.OnBlockDamaged(_world, _clrIdx, _blockPos, _blockValue, _damagePoints, _entityIdThatDamaged, _attackHitInfo, _bUseHarvestTool, _bBypassMaxDamage, _recDepth);
	}

	// Token: 0x040007B8 RID: 1976
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> buffActions;

	// Token: 0x040007B9 RID: 1977
	[PublicizedFrom(EAccessModifier.Private)]
	public float brokenPercentage;
}
