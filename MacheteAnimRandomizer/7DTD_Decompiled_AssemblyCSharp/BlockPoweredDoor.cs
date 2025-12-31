using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000122 RID: 290
[Preserve]
public class BlockPoweredDoor : BlockPowered
{
	// Token: 0x060007F4 RID: 2036 RVA: 0x00037FC4 File Offset: 0x000361C4
	public BlockPoweredDoor()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x00038004 File Offset: 0x00036204
	public override void Init()
	{
		if (base.Properties.Values[Block.PropMultiBlockDim] == null)
		{
			base.Properties.Values[Block.PropMultiBlockDim] = "1,2,1";
		}
		base.Init();
		if (base.Properties.Values.ContainsKey("OpenSound"))
		{
			this.openSound = base.Properties.Values["OpenSound"];
		}
		if (base.Properties.Values.ContainsKey("CloseSound"))
		{
			this.closeSound = base.Properties.Values["CloseSound"];
		}
		if (base.Properties.Values.ContainsKey(BlockPoweredDoor.PropPlaceEverywhere))
		{
			StringParsers.TryParseBool(base.Properties.Values[BlockPoweredDoor.PropPlaceEverywhere], out this.bPlaceEverywhere, 0, -1, true);
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0002E2E1 File Offset: 0x0002C4E1
	public static bool IsDoorOpen(byte _metadata)
	{
		return (_metadata & 1) > 0;
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x000380E8 File Offset: 0x000362E8
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace _face)
	{
		if (!this.isMultiBlock || !_blockValue.ischild)
		{
			return !BlockDoor.IsDoorOpen(_blockValue.meta);
		}
		Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
		BlockValue block = _world.GetBlock(parentPos);
		if (block.ischild)
		{
			string[] array = new string[5];
			array[0] = "Door on position ";
			int num = 1;
			Vector3i vector3i = parentPos;
			array[num] = vector3i.ToString();
			array[2] = " with value ";
			int num2 = 3;
			BlockValue blockValue = block;
			array[num2] = blockValue.ToString();
			array[4] = " should be a parent but is not! (2)";
			Log.Error(string.Concat(array));
			return true;
		}
		return this.IsMovementBlocked(_world, parentPos, block, _face);
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x00038194 File Offset: 0x00036394
	public override bool IsSeeThrough(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (!this.isMultiBlock || !_blockValue.ischild)
		{
			return BlockDoor.IsDoorOpen(_blockValue.meta);
		}
		Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
		BlockValue block = _world.GetBlock(parentPos);
		if (block.ischild)
		{
			string[] array = new string[5];
			array[0] = "Door on position ";
			int num = 1;
			Vector3i vector3i = parentPos;
			array[num] = vector3i.ToString();
			array[2] = " with value ";
			int num2 = 3;
			BlockValue blockValue = block;
			array[num2] = blockValue.ToString();
			array[4] = " should be a parent but is not! (1)";
			Log.Error(string.Concat(array));
			return true;
		}
		return this.IsSeeThrough(_world, _clrIdx, parentPos, block);
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0003823C File Offset: 0x0003643C
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = _world.IsMyLandProtectedBlock(_blockPos, _world.GetGameManager().GetPersistentLocalPlayer(), false);
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.GetBlockActivationCommands(_world, block, _clrIdx, parentPos, _entityFocusing);
		}
		TileEntityPoweredBlock tileEntityPoweredBlock = (TileEntityPoweredBlock)_world.GetTileEntity(_clrIdx, _blockPos);
		if (tileEntityPoweredBlock != null)
		{
			bool isPowered = tileEntityPoweredBlock.IsPowered;
		}
		this.cmds[0].enabled = (flag && this.TakeDelay > 0f);
		return this.cmds;
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x000382D4 File Offset: 0x000364D4
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, _cIdx, parentPos, block, _player);
		}
		if (_commandName == "take")
		{
			base.TakeItemWithTimer(_cIdx, _blockPos, _blockValue, _player);
			return true;
		}
		return false;
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x00038334 File Offset: 0x00036534
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		if (this.shape is BlockShapeModelEntity && (_oldBlockValue.type != _newBlockValue.type || _oldBlockValue.meta != _newBlockValue.meta) && !_newBlockValue.ischild)
		{
			BlockEntityData blockEntity = ((World)_world).ChunkClusters[_clrIdx].GetBlockEntity(_blockPos);
			bool flag = BlockPoweredDoor.IsDoorOpen(_newBlockValue.meta);
			bool flag2 = BlockPoweredDoor.IsDoorOpen(_oldBlockValue.meta);
			if (flag != flag2)
			{
				this.updateAnimState(blockEntity, flag);
			}
		}
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x000383C8 File Offset: 0x000365C8
	public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		bool flag = !BlockPoweredDoor.IsDoorOpen(_blockValue.meta);
		this.updateOpenCloseState(flag, _world, _blockPos, _cIdx, _blockValue, false);
		if (_player != null)
		{
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, flag ? this.openSound : this.closeSound);
		}
		return true;
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x00038430 File Offset: 0x00036630
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateAnimState(WorldBase _world, int _cIdx, Vector3i _blockPos, bool _bOpen)
	{
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		if (chunkCluster == null)
		{
			return;
		}
		IChunk chunkSync = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
		if (chunkSync == null)
		{
			return;
		}
		BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return;
		}
		this.updateAnimState(blockEntity, _bOpen);
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x00038498 File Offset: 0x00036698
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateAnimState(BlockEntityData _ebcd, bool _bOpen)
	{
		if (_ebcd != null && _ebcd.bHasTransform)
		{
			Animator[] componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>();
			if (componentsInChildren != null)
			{
				for (int i = componentsInChildren.Length - 1; i >= 0; i--)
				{
					Animator animator = componentsInChildren[i];
					animator.enabled = true;
					animator.SetBool(AnimatorDoorState.IsOpenHash, _bOpen);
					animator.SetTrigger(AnimatorDoorState.OpenTriggerHash);
				}
			}
		}
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x000384F0 File Offset: 0x000366F0
	public override bool ActivateBlock(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn, bool isPowered)
	{
		byte meta = _blockValue.meta;
		_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (isOn ? 1 : 0));
		if (meta != _blockValue.meta)
		{
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
			this.updateAnimState(_world, _cIdx, _blockPos, isOn);
			if (isOn)
			{
				Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.openSound);
			}
			else
			{
				Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, this.closeSound);
			}
		}
		return true;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x00038590 File Offset: 0x00036790
	public override TileEntityPowered CreateTileEntity(Chunk chunk)
	{
		PowerItem.PowerItemTypes powerItemType = PowerItem.PowerItemTypes.Consumer;
		return new TileEntityPoweredBlock(chunk)
		{
			PowerItemType = powerItemType
		};
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x000385AC File Offset: 0x000367AC
	public override void ForceAnimationState(BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (_ebcd != null && _ebcd.bHasTransform)
		{
			Animator[] componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>();
			if (componentsInChildren != null)
			{
				bool flag = BlockPoweredDoor.IsDoorOpen(_blockValue.meta);
				for (int i = componentsInChildren.Length - 1; i >= 0; i--)
				{
					Animator animator = componentsInChildren[i];
					animator.enabled = true;
					animator.keepAnimatorStateOnDisable = true;
					animator.SetBool(AnimatorDoorState.IsOpenHash, flag);
					animator.Play(flag ? AnimatorDoorState.OpenHash : AnimatorDoorState.CloseHash, 0, 1f);
				}
			}
		}
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x00038628 File Offset: 0x00036828
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateOpenCloseState(bool _bOpen, WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, bool _bOnlyLocal)
	{
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		if (chunkCluster == null)
		{
			return;
		}
		_blockValue.meta = (byte)((_bOpen ? 1 : 0) | ((int)_blockValue.meta & -2));
		if (!_bOnlyLocal)
		{
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
			return;
		}
		chunkCluster.SetBlockRaw(_blockPos, _blockValue);
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0003867A File Offset: 0x0003687A
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		this.updateOpenCloseState(BlockPoweredDoor.IsDoorOpen(_blockValue.meta), _world, _blockPos, _chunk.ClrIdx, _blockValue, true);
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public override float GetStepHeight(IBlockAccess world, Vector3i blockPos, BlockValue blockDef, BlockFace stepFace)
	{
		return 0f;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x000386B0 File Offset: 0x000368B0
	public bool IsOpen(IBlockAccess _blockAccess, int _x, int _y, int _z)
	{
		return BlockPoweredDoor.IsDoorOpen(_blockAccess.GetBlock(_x, _y, _z).meta);
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x000386D4 File Offset: 0x000368D4
	public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		if (!base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck))
		{
			return false;
		}
		if (_blockValue.Block.HasTag(BlockTags.Window))
		{
			return true;
		}
		if (this.bPlaceEverywhere)
		{
			return true;
		}
		if ((_world.GetBlock(_clrIdx, _blockPos.x - 1, _blockPos.y, _blockPos.z).Block.shape.IsSolidCube && _world.GetBlock(_blockPos.x + 1, _blockPos.y, _blockPos.z).Block.shape.IsSolidCube) || (_world.GetBlock(_clrIdx, _blockPos.x, _blockPos.y, _blockPos.z - 1).Block.shape.IsSolidCube && _world.GetBlock(_blockPos.x, _blockPos.y, _blockPos.z + 1).Block.shape.IsSolidCube))
		{
			if (!_world.GetBlock(_clrIdx, _blockPos.x, _blockPos.y - 1, _blockPos.z).Block.shape.IsSolidCube)
			{
				return false;
			}
			if (_world.GetBlock(_clrIdx, _blockPos.x, _blockPos.y + 1, _blockPos.z).isair)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x00038824 File Offset: 0x00036A24
	public override void RenderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		if (!(this.shape is BlockShapeModelEntity) || (_blockValue.meta & 2) == 0)
		{
			this.shape.renderDecorations(_worldPos, _blockValue, _drawPos, _vertices, _lightingAround, _textureFullArray, _meshes, _nBlocks);
		}
	}

	// Token: 0x04000855 RID: 2133
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlaceEverywhere = "PlaceEverywhere";

	// Token: 0x04000856 RID: 2134
	[PublicizedFrom(EAccessModifier.Private)]
	public string openSound;

	// Token: 0x04000857 RID: 2135
	[PublicizedFrom(EAccessModifier.Private)]
	public string closeSound;

	// Token: 0x04000858 RID: 2136
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bPlaceEverywhere;

	// Token: 0x04000859 RID: 2137
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockEntityData ebcd;

	// Token: 0x0400085A RID: 2138
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("take", "hand", false, false, null)
	};
}
