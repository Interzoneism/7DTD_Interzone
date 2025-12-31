using System;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200011D RID: 285
[Preserve]
public class BlockPlayerSign : Block
{
	// Token: 0x060007CF RID: 1999 RVA: 0x0003720C File Offset: 0x0003540C
	public BlockPlayerSign()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x000372B0 File Offset: 0x000354B0
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("LineWidth"))
		{
			this.characterWidth = int.Parse(base.Properties.Values["LineWidth"]);
		}
		if (base.Properties.Values.ContainsKey("LineCount"))
		{
			this.lineCount = int.Parse(base.Properties.Values["LineCount"]);
		}
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x00037334 File Offset: 0x00035534
	public override void OnBlockAdded(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if ((TileEntitySign)world.GetTileEntity(_chunk.ClrIdx, _blockPos) == null)
		{
			TileEntitySign tileEntitySign = new TileEntitySign(_chunk);
			if (tileEntitySign != null)
			{
				tileEntitySign.localChunkPos = World.toBlock(_blockPos);
				_chunk.AddTileEntity(tileEntitySign);
			}
		}
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x0003738C File Offset: 0x0003558C
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (_ebcd == null)
		{
			return;
		}
		Chunk chunk = (Chunk)((World)_world).GetChunkFromWorldPos(_blockPos);
		TileEntitySign tileEntitySign = (TileEntitySign)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntitySign == null)
		{
			tileEntitySign = new TileEntitySign(chunk);
			if (tileEntitySign != null)
			{
				tileEntitySign.localChunkPos = World.toBlock(_blockPos);
				chunk.AddTileEntity(tileEntitySign);
			}
		}
		if (tileEntitySign == null)
		{
			Log.Error("Tile Entity Sign was unable to be created!");
			return;
		}
		tileEntitySign.SetBlockEntityData(_ebcd);
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00037404 File Offset: 0x00035604
	public override void PlaceBlock(WorldBase _world, BlockPlacement.Result _result, EntityAlive _ea)
	{
		base.PlaceBlock(_world, _result, _ea);
		TileEntitySign tileEntitySign = (TileEntitySign)_world.GetTileEntity(_result.clrIdx, _result.blockPos);
		if (tileEntitySign != null)
		{
			tileEntitySign.SetOwner(PlatformManager.InternalLocalUserIdentifier);
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00037440 File Offset: 0x00035640
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		_chunk.RemoveTileEntityAt<TileEntitySign>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x00037460 File Offset: 0x00035660
	public override Block.DestroyedResult OnBlockDestroyedBy(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId, bool _bUseHarvestTool)
	{
		TileEntitySecureLootContainer tileEntitySecureLootContainer = _world.GetTileEntity(_clrIdx, _blockPos) as TileEntitySecureLootContainer;
		if (tileEntitySecureLootContainer != null)
		{
			tileEntitySecureLootContainer.OnDestroy();
		}
		return Block.DestroyedResult.Downgrade;
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00037485 File Offset: 0x00035685
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return Localization.Get("useWorkstation", false);
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x00037494 File Offset: 0x00035694
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, _cIdx, parentPos, block, _player);
		}
		TileEntitySign te = _world.GetTileEntity(_cIdx, _blockPos) as TileEntitySign;
		if (te == null)
		{
			return false;
		}
		if (!(_commandName == "edit"))
		{
			if (_commandName == "lock")
			{
				te.SetLocked(true);
				Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, "Misc/locking");
				GameManager.ShowTooltip(_player, "containerLocked", false, false, 0f);
				return true;
			}
			if (_commandName == "unlock")
			{
				te.SetLocked(false);
				Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, "Misc/unlocking");
				GameManager.ShowTooltip(_player, "containerUnlocked", false, false, 0f);
				return true;
			}
			if (_commandName == "keypad")
			{
				XUiC_KeypadWindow.Open(LocalPlayerUI.GetUIForPlayer(_player), te);
				return true;
			}
			if (!(_commandName == "report"))
			{
				return false;
			}
			GeneratedTextManager.GetDisplayText(te.GetAuthoredText(), delegate(string _filtered)
			{
				ThreadManager.AddSingleTaskMainThread("OpenReportWindow", delegate(object _)
				{
					PersistentPlayerData playerData = GameManager.Instance.persistentPlayers.GetPlayerData(te.GetAuthoredText().Author);
					XUiC_ReportPlayer.Open((playerData != null) ? playerData.PlayerData : null, EnumReportCategory.VerbalAbuse, string.Format(Localization.Get("xuiReportOffensiveTextMessage", false), _filtered), "");
				}, null);
			}, true, false, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
			return true;
		}
		else
		{
			if (GameManager.Instance.IsEditMode() || !te.IsLocked() || te.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier))
			{
				return this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
			}
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, "Misc/locked");
			return false;
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00037670 File Offset: 0x00035870
	public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_world, _cIdx, parentPos, block, _player);
		}
		TileEntitySign tileEntitySign = (TileEntitySign)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntitySign == null)
		{
			return false;
		}
		_player.AimingGun = false;
		Vector3i blockPos = tileEntitySign.ToWorldPos();
		_world.GetGameManager().TELockServer(_cIdx, blockPos, tileEntitySign.entityId, _player.entityId, null);
		return true;
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x000376EE File Offset: 0x000358EE
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return _world.GetTileEntity(_clrIdx, _blockPos) is TileEntitySign;
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x00037704 File Offset: 0x00035904
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		TileEntitySign tileEntitySign = (TileEntitySign)_world.GetTileEntity(_clrIdx, _blockPos);
		if (tileEntitySign == null)
		{
			return BlockActivationCommand.Empty;
		}
		PlatformUserIdentifierAbs internalLocalUserIdentifier = PlatformManager.InternalLocalUserIdentifier;
		PersistentPlayerData playerData = _world.GetGameManager().GetPersistentPlayerList().GetPlayerData(tileEntitySign.GetOwner());
		bool flag = tileEntitySign.LocalPlayerIsOwner();
		bool flag2 = !tileEntitySign.LocalPlayerIsOwner() && (playerData != null && playerData.ACL != null) && playerData.ACL.Contains(internalLocalUserIdentifier);
		this.cmds[0].enabled = true;
		this.cmds[1].enabled = (!tileEntitySign.IsLocked() && (flag || flag2));
		this.cmds[2].enabled = (tileEntitySign.IsLocked() && flag);
		this.cmds[3].enabled = ((!tileEntitySign.IsUserAllowed(internalLocalUserIdentifier) && tileEntitySign.HasPassword() && tileEntitySign.IsLocked()) || flag);
		bool flag3 = PlatformManager.MultiPlatform.PlayerReporting != null && !string.IsNullOrEmpty(tileEntitySign.GetAuthoredText().Text) && !internalLocalUserIdentifier.Equals(tileEntitySign.GetAuthoredText().Author);
		PersistentPlayerData playerData2 = GameManager.Instance.persistentPlayers.GetPlayerData(tileEntitySign.GetAuthoredText().Author);
		bool flag4 = playerData2 != null && playerData2.PlatformData.Blocked[EBlockType.TextChat].IsBlocked();
		this.cmds[4].enabled = (playerData != null && flag3 && !flag4);
		return this.cmds;
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsTileEntitySavedInPrefab()
	{
		return true;
	}

	// Token: 0x04000848 RID: 2120
	[PublicizedFrom(EAccessModifier.Private)]
	public int characterWidth;

	// Token: 0x04000849 RID: 2121
	[PublicizedFrom(EAccessModifier.Private)]
	public int lineCount;

	// Token: 0x0400084A RID: 2122
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("edit", "pen", false, false, null),
		new BlockActivationCommand("lock", "lock", false, false, null),
		new BlockActivationCommand("unlock", "unlock", false, false, null),
		new BlockActivationCommand("keypad", "keypad", false, false, null),
		new BlockActivationCommand("report", "report", false, false, null)
	};
}
