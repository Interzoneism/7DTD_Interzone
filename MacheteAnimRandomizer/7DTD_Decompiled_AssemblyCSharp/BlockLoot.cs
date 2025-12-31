using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000111 RID: 273
[Preserve]
public class BlockLoot : Block
{
	// Token: 0x06000766 RID: 1894 RVA: 0x00034A2C File Offset: 0x00032C2C
	public BlockLoot()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00034A6C File Offset: 0x00032C6C
	public override void Init()
	{
		base.Init();
		if (!base.Properties.Values.ContainsKey(BlockLoot.PropLootList))
		{
			throw new Exception("Block with name " + base.GetBlockName() + " doesnt have a loot list");
		}
		this.lootList = base.Properties.Values[BlockLoot.PropLootList];
		base.Properties.ParseFloat(BlockLoot.PropLootStageMod, ref this.LootStageMod);
		base.Properties.ParseFloat(BlockLoot.PropLootStageBonus, ref this.LootStageBonus);
		for (int i = 1; i < 99; i++)
		{
			string text = BlockLoot.PropAlternateLootList + i.ToString();
			if (!base.Properties.Values.ContainsKey(text))
			{
				break;
			}
			string text2 = "";
			if (base.Properties.Params1.ContainsKey(text))
			{
				text2 = base.Properties.Params1[text];
			}
			if (text2 != "")
			{
				FastTags<TagGroup.Global> tag = FastTags<TagGroup.Global>.Parse(text2);
				if (this.AlternateLootList == null)
				{
					this.AlternateLootList = new List<BlockLoot.AlternateLootEntry>();
				}
				this.AlternateLootList.Add(new BlockLoot.AlternateLootEntry
				{
					tag = tag,
					lootEntry = base.Properties.Values[text]
				});
			}
		}
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00034BBC File Offset: 0x00032DBC
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_result.clrIdx, _result.blockPos) as TileEntityLootContainer;
		if (tileEntityLootContainer == null)
		{
			return;
		}
		if (_ea != null && _ea.entityType == EntityType.Player)
		{
			tileEntityLootContainer.bPlayerStorage = true;
			tileEntityLootContainer.worldTimeTouched = _world.GetWorldTime();
			tileEntityLootContainer.SetEmpty();
		}
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00034C1C File Offset: 0x00032E1C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityLootContainer;
		if (tileEntityLootContainer == null)
		{
			return string.Empty;
		}
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		if (!tileEntityLootContainer.bTouched)
		{
			return string.Format(Localization.Get("lootTooltipNew", false), arg, localizedBlockName);
		}
		if (tileEntityLootContainer.IsEmpty())
		{
			return string.Format(Localization.Get("lootTooltipEmpty", false), arg, localizedBlockName);
		}
		return string.Format(Localization.Get("lootTooltipTouched", false), arg, localizedBlockName);
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00034CCA File Offset: 0x00032ECA
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if (LootContainer.GetLootContainer(this.lootList, true) == null)
		{
			return;
		}
		this.addTileEntity(world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00034D00 File Offset: 0x00032F00
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		TileEntityLootContainer tileEntityLootContainer = world.GetTileEntity(_chunk.ClrIdx, _blockPos) as TileEntityLootContainer;
		if (tileEntityLootContainer != null)
		{
			tileEntityLootContainer.OnDestroy();
		}
		this.removeTileEntity(world, _chunk, _blockPos, _blockValue);
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00034D40 File Offset: 0x00032F40
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void addTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		TileEntityLootContainer tileEntityLootContainer = new TileEntityLootContainer(_chunk);
		tileEntityLootContainer.localChunkPos = World.toBlock(_blockPos);
		tileEntityLootContainer.lootListName = this.lootList;
		tileEntityLootContainer.SetContainerSize(LootContainer.GetLootContainer(this.lootList, true).size, true);
		_chunk.AddTileEntity(tileEntityLootContainer);
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00034D8B File Offset: 0x00032F8B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void removeTileEntity(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		_chunk.RemoveTileEntityAt<TileEntityLootContainer>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x00034DA0 File Offset: 0x00032FA0
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntityLootContainer;
		if (tileEntityLootContainer != null)
		{
			tileEntityLootContainer.OnDestroy();
		}
		if (!GameManager.IsDedicatedServer)
		{
			XUiC_LootWindowGroup.CloseIfOpenAtPos(_blockPos, null);
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00034DD4 File Offset: 0x00032FD4
	public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_player.inventory.IsHoldingItemActionRunning())
		{
			return false;
		}
		TileEntityLootContainer tileEntityLootContainer = _world.GetTileEntity(_cIdx, _blockPos) as TileEntityLootContainer;
		if (tileEntityLootContainer == null)
		{
			return false;
		}
		_player.AimingGun = false;
		Vector3i blockPos = tileEntityLootContainer.ToWorldPos();
		tileEntityLootContainer.bWasTouched = tileEntityLootContainer.bTouched;
		_world.GetGameManager().TELockServer(_cIdx, blockPos, tileEntityLootContainer.entityId, _player.entityId, null);
		return true;
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x00034E3B File Offset: 0x0003303B
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		return _commandName == "Search" && this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00034E5A File Offset: 0x0003305A
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return this.cmds;
	}

	// Token: 0x04000810 RID: 2064
	public static string PropLootList = "LootList";

	// Token: 0x04000811 RID: 2065
	public static string PropAlternateLootList = "AlternateLootList";

	// Token: 0x04000812 RID: 2066
	public static string PropLootStageMod = "LootStageMod";

	// Token: 0x04000813 RID: 2067
	public static string PropLootStageBonus = "LootStageBonus";

	// Token: 0x04000814 RID: 2068
	[PublicizedFrom(EAccessModifier.Protected)]
	public string lootList;

	// Token: 0x04000815 RID: 2069
	public float LootStageMod;

	// Token: 0x04000816 RID: 2070
	public float LootStageBonus;

	// Token: 0x04000817 RID: 2071
	public List<BlockLoot.AlternateLootEntry> AlternateLootList;

	// Token: 0x04000818 RID: 2072
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("Search", "search", true, false, null)
	};

	// Token: 0x02000112 RID: 274
	public struct AlternateLootEntry
	{
		// Token: 0x04000819 RID: 2073
		public FastTags<TagGroup.Global> tag;

		// Token: 0x0400081A RID: 2074
		public string lootEntry;
	}
}
