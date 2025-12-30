using System;
using System.Collections.Generic;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000FD RID: 253
[Preserve]
public class BlockDoorSecure : BlockDoor
{
	// Token: 0x17000074 RID: 116
	// (get) Token: 0x0600069B RID: 1691 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x0002EA5C File Offset: 0x0002CC5C
	public BlockDoorSecure()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x0002EB3C File Offset: 0x0002CD3C
	public override void Init()
	{
		base.Init();
		base.Properties.ParseString(BlockDoorSecure.PropLockedSound, ref this.lockedSound);
		base.Properties.ParseString(BlockDoorSecure.PropLockingSound, ref this.lockingSound);
		base.Properties.ParseString(BlockDoorSecure.PropUnLockingSound, ref this.unlockingSound);
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0002EB94 File Offset: 0x0002CD94
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_world.IsEditor())
		{
			return;
		}
		if (_blockValue.ischild)
		{
			return;
		}
		TileEntitySecureDoor tileEntitySecureDoor = _world.GetTileEntity(_chunk.ClrIdx, _blockPos) as TileEntitySecureDoor;
		if (tileEntitySecureDoor != null)
		{
			return;
		}
		tileEntitySecureDoor = new TileEntitySecureDoor(_chunk);
		tileEntitySecureDoor.SetDisableModifiedCheck(true);
		tileEntitySecureDoor.localChunkPos = World.toBlock(_blockPos);
		tileEntitySecureDoor.SetLocked(BlockDoorSecure.IsDoorLockedMeta(_blockValue.meta));
		tileEntitySecureDoor.SetDisableModifiedCheck(false);
		_chunk.AddTileEntity(tileEntitySecureDoor);
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x0002EC12 File Offset: 0x0002CE12
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		if (_world.IsEditor())
		{
			this.SetIsLocked(BlockDoorSecure.IsDoorLockedMeta(_newBlockValue.meta), _world, _blockPos);
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x0002EC41 File Offset: 0x0002CE41
	public override bool FilterIndexType(BlockValue bv)
	{
		return !(this.IndexName == "TraderOnOff") || !BlockDoorSecure.IsDoorLockedMeta(bv.meta);
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0002EC66 File Offset: 0x0002CE66
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		_chunk.RemoveTileEntityAt<TileEntitySecureDoor>((World)world, World.toBlock(_blockPos));
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x0002EC85 File Offset: 0x0002CE85
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return _world.GetTileEntity(_clrIdx, _blockPos) is TileEntitySecureDoor || _world.IsEditor();
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x0002ECA0 File Offset: 0x0002CEA0
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.GetBlockActivationCommands(_world, block, _clrIdx, parentPos, _entityFocusing);
		}
		TileEntitySecureDoor tileEntitySecureDoor = (TileEntitySecureDoor)_world.GetTileEntity(_clrIdx, _blockPos);
		if (tileEntitySecureDoor == null && !_world.IsEditor())
		{
			return BlockActivationCommand.Empty;
		}
		PlatformUserIdentifierAbs internalLocalUserIdentifier = PlatformManager.InternalLocalUserIdentifier;
		PersistentPlayerData persistentPlayerData = (!_world.IsEditor()) ? _world.GetGameManager().GetPersistentPlayerList().GetPlayerData(tileEntitySecureDoor.GetOwner()) : null;
		bool flag = _world.IsEditor() || (!tileEntitySecureDoor.LocalPlayerIsOwner() && (persistentPlayerData != null && persistentPlayerData.ACL != null) && persistentPlayerData.ACL.Contains(internalLocalUserIdentifier));
		bool flag2 = (!_world.IsEditor()) ? tileEntitySecureDoor.IsLocked() : BlockDoorSecure.IsDoorLockedMeta(_blockValue.meta);
		bool flag3 = _world.IsEditor() || tileEntitySecureDoor.LocalPlayerIsOwner();
		bool flag4 = !_world.IsEditor() && tileEntitySecureDoor.IsUserAllowed(internalLocalUserIdentifier);
		bool flag5 = !_world.IsEditor() && tileEntitySecureDoor.HasPassword();
		((Chunk)_world.ChunkClusters[_clrIdx].GetChunkSync(World.toChunkXZ(_blockPos.x), _blockPos.y, World.toChunkXZ(_blockPos.z))).GetBlockTrigger(World.toBlock(_blockPos));
		this.cmds[0].enabled = BlockDoor.IsDoorOpen(_blockValue.meta);
		this.cmds[1].enabled = !BlockDoor.IsDoorOpen(_blockValue.meta);
		this.cmds[2].enabled = (!flag2 && (flag3 || flag || _world.IsEditor()));
		this.cmds[3].enabled = (flag2 && (flag3 || _world.IsEditor()));
		this.cmds[4].enabled = ((!flag4 && flag5 && flag2) || flag3);
		this.cmds[5].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x0002EECC File Offset: 0x0002D0CC
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, _cIdx, parentPos, block, _player);
		}
		TileEntitySecureDoor tileEntitySecureDoor = (TileEntitySecureDoor)_world.GetTileEntity(_cIdx, _blockPos);
		if (tileEntitySecureDoor == null && !_world.IsEditor())
		{
			return false;
		}
		bool flag = (!_world.IsEditor()) ? tileEntitySecureDoor.IsLocked() : BlockDoorSecure.IsDoorLockedMeta(_blockValue.meta);
		bool flag2 = !_world.IsEditor() && tileEntitySecureDoor.IsUserAllowed(PlatformManager.InternalLocalUserIdentifier);
		if (!(_commandName == "close"))
		{
			if (!(_commandName == "open"))
			{
				if (_commandName == "lock")
				{
					this.SetIsLocked(true, _world, _blockPos);
					return true;
				}
				if (_commandName == "unlock")
				{
					this.SetIsLocked(false, _world, _blockPos);
					return true;
				}
				if (!(_commandName == "keypad"))
				{
					if (_commandName == "trigger")
					{
						XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, true, true);
					}
					return false;
				}
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_player);
				if (uiforPlayer != null && tileEntitySecureDoor != null)
				{
					XUiC_KeypadWindow.Open(uiforPlayer, tileEntitySecureDoor);
				}
				return true;
			}
			else
			{
				if (_world.IsEditor() || !flag || flag2)
				{
					base.HandleTrigger(_player, (World)_world, _cIdx, _blockPos, _blockValue);
					return this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
				}
				Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.lockedSound);
				return false;
			}
		}
		else
		{
			if (_world.IsEditor() || !flag || flag2)
			{
				base.HandleTrigger(_player, (World)_world, _cIdx, _blockPos, _blockValue);
				return this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
			}
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.lockedSound);
			return false;
		}
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x0002F0D0 File Offset: 0x0002D2D0
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.GetActivationText(_world, block, _clrIdx, parentPos, _entityFocusing);
		}
		TileEntitySecureDoor tileEntitySecureDoor = (TileEntitySecureDoor)_world.GetTileEntity(_clrIdx, _blockPos);
		if (tileEntitySecureDoor == null && !_world.IsEditor())
		{
			return "";
		}
		bool flag = (!_world.IsEditor()) ? tileEntitySecureDoor.IsLocked() : BlockDoorSecure.IsDoorLockedMeta(_blockValue.meta);
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string arg2 = Localization.Get("door", false);
		if (!flag)
		{
			return string.Format(Localization.Get("tooltipUnlocked", false), arg, arg2);
		}
		return string.Format(Localization.Get("tooltipLocked", false), arg, arg2);
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x0002F1B8 File Offset: 0x0002D3B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool IsDoorLockedMeta(byte _metadata)
	{
		return (_metadata & 4) > 0;
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x0002F1C0 File Offset: 0x0002D3C0
	public bool IsDoorLocked(WorldBase _world, Vector3i _blockPos)
	{
		BlockValue block = _world.GetBlock(_blockPos);
		if (block.isair)
		{
			return false;
		}
		if (block.ischild)
		{
			return this.IsDoorLocked(_world, _blockPos + block.parent);
		}
		if (_world.IsEditor())
		{
			return BlockDoorSecure.IsDoorLockedMeta(block.meta);
		}
		TileEntitySecureDoor tileEntitySecureDoor = _world.GetTileEntity(_blockPos) as TileEntitySecureDoor;
		return tileEntitySecureDoor != null && tileEntitySecureDoor.IsLocked();
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x0002F22C File Offset: 0x0002D42C
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue SetIsLocked(bool _isLocked, WorldBase _world, Vector3i _blockPos)
	{
		BlockValue block = _world.GetBlock(_blockPos);
		if (block.isair)
		{
			return block;
		}
		if (block.ischild)
		{
			return block;
		}
		if (_world.IsEditor())
		{
			return this.SetIsLockedEditor(_isLocked, _world, _blockPos, block);
		}
		this.SetIsLockedNonEditor(_isLocked, _world, _blockPos);
		return block;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x0002F274 File Offset: 0x0002D474
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetIsLockedNonEditor(bool _isLocked, WorldBase _world, Vector3i _blockPos)
	{
		TileEntitySecureDoor tileEntitySecureDoor = _world.GetTileEntity(_blockPos) as TileEntitySecureDoor;
		if (tileEntitySecureDoor == null || tileEntitySecureDoor.IsLocked() == _isLocked)
		{
			return;
		}
		tileEntitySecureDoor.SetLocked(_isLocked);
		this.PlayLockingSound(_blockPos);
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0002F2AC File Offset: 0x0002D4AC
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue SetIsLockedEditor(bool _isLocked, WorldBase _world, Vector3i _blockPos, BlockValue _blockValue)
	{
		byte meta = _blockValue.meta;
		byte b;
		if (_isLocked)
		{
			b = (meta | 4);
		}
		else
		{
			b = (byte)((int)meta & -5);
		}
		if (meta != b)
		{
			_blockValue.meta = b;
			_world.SetBlockRPC(_blockPos, _blockValue);
			this.PlayLockingSound(_blockPos);
		}
		return _blockValue;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x0002F2EF File Offset: 0x0002D4EF
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayLockingSound(Vector3i _blockPos)
	{
		Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.lockingSound);
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x0002F318 File Offset: 0x0002D518
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, _cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		if ((_blockValue.meta & 1) != 0)
		{
			Manager.BroadcastPlay(_blockPos.ToVector3() + Vector3.one * 0.5f, this.closeSound, 0f);
		}
		else
		{
			Manager.BroadcastPlay(_blockPos.ToVector3() + Vector3.one * 0.5f, this.openSound, 0f);
		}
		if (_triggeredBy != null && _triggeredBy.Unlock)
		{
			_blockValue = this.SetIsLocked(false, _world, _blockPos);
		}
		_blockValue.meta ^= 1;
		_blockChanges.Add(new BlockChangeInfo(_cIdx, _blockPos, _blockValue));
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0002F3D4 File Offset: 0x0002D5D4
	public override void OnBlockReset(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		TileEntitySecureDoor tileEntitySecureDoor = _world.GetTileEntity(_chunk.ClrIdx, _blockPos) as TileEntitySecureDoor;
		if (tileEntitySecureDoor != null)
		{
			tileEntitySecureDoor.SetLocked(BlockDoorSecure.IsDoorLockedMeta(_blockValue.meta));
		}
	}

	// Token: 0x040007B0 RID: 1968
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cDoorIsLockedMask = 4;

	// Token: 0x040007B1 RID: 1969
	[PublicizedFrom(EAccessModifier.Protected)]
	public string lockedSound = "Misc/locked";

	// Token: 0x040007B2 RID: 1970
	[PublicizedFrom(EAccessModifier.Protected)]
	public string lockingSound = "Misc/locking";

	// Token: 0x040007B3 RID: 1971
	[PublicizedFrom(EAccessModifier.Protected)]
	public string unlockingSound = "Misc/unlocking";

	// Token: 0x040007B4 RID: 1972
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLockedSound = "LockedSound";

	// Token: 0x040007B5 RID: 1973
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropLockingSound = "LockingSound";

	// Token: 0x040007B6 RID: 1974
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropUnLockingSound = "UnlockingSound";

	// Token: 0x040007B7 RID: 1975
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("close", "door", false, false, null),
		new BlockActivationCommand("open", "door", false, false, null),
		new BlockActivationCommand("lock", "lock", false, false, null),
		new BlockActivationCommand("unlock", "unlock", false, false, null),
		new BlockActivationCommand("keypad", "keypad", false, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}
