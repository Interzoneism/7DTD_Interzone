using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000FC RID: 252
[Preserve]
public class BlockDoor : BlockSecure
{
	// Token: 0x06000686 RID: 1670 RVA: 0x0002E1A0 File Offset: 0x0002C3A0
	public override void Init()
	{
		if (base.Properties.Values[Block.PropMultiBlockDim] == null)
		{
			base.Properties.Values[Block.PropMultiBlockDim] = "1,2,1";
		}
		base.Init();
		if (base.Properties.Values.ContainsKey("TintableMaterials"))
		{
			foreach (string str in base.Properties.Values["TintableMaterials"].Split(',', StringSplitOptions.None))
			{
				this.tintableMaterials.Add(str + " (Instance)");
			}
		}
		if (base.Properties.Values.ContainsKey("OpenSound"))
		{
			this.openSound = base.Properties.Values["OpenSound"];
		}
		if (base.Properties.Values.ContainsKey("CloseSound"))
		{
			this.closeSound = base.Properties.Values["CloseSound"];
		}
		if (base.Properties.Values.ContainsKey(BlockDoor.PropPlaceEverywhere))
		{
			StringParsers.TryParseBool(base.Properties.Values[BlockDoor.PropPlaceEverywhere], out this.bPlaceEverywhere, 0, -1, true);
		}
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x0002E2E1 File Offset: 0x0002C4E1
	public static bool IsDoorOpen(byte _metadata)
	{
		return (_metadata & 1) > 0;
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x0002E2EC File Offset: 0x0002C4EC
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

	// Token: 0x06000689 RID: 1673 RVA: 0x0002E398 File Offset: 0x0002C598
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

	// Token: 0x0600068A RID: 1674 RVA: 0x0002E440 File Offset: 0x0002C640
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		if (this.shape is BlockShapeModelEntity && (_oldBlockValue.type != _newBlockValue.type || _oldBlockValue.meta != _newBlockValue.meta) && !_newBlockValue.ischild)
		{
			if (VehicleManager.Instance != null)
			{
				VehicleManager.Instance.PhysicsWakeNear(_blockPos.ToVector3());
			}
			BlockEntityData blockEntity = ((World)_world).ChunkClusters[_clrIdx].GetBlockEntity(_blockPos);
			bool flag = BlockDoor.IsDoorOpen(_newBlockValue.meta);
			bool flag2 = BlockDoor.IsDoorOpen(_oldBlockValue.meta);
			if (flag != flag2)
			{
				this.updateAnimState(blockEntity, flag);
			}
		}
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x0002E4EC File Offset: 0x0002C6EC
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.OnBlockActivated(_commandName, _world, _clrIdx, parentPos, block, _player);
		}
		if (_commandName == "close")
		{
			_blockValue.meta |= 1;
			return this.OnBlockActivated(_world, _clrIdx, _blockPos, _blockValue, _player);
		}
		if (!(_commandName == "open"))
		{
			return false;
		}
		_blockValue.meta = (byte)((int)_blockValue.meta & -2);
		return this.OnBlockActivated(_world, _clrIdx, _blockPos, _blockValue, _player);
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0002E58C File Offset: 0x0002C78C
	public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		bool flag = !BlockDoor.IsDoorOpen(_blockValue.meta);
		this.updateOpenCloseState(flag, _world, _blockPos, _cIdx, _blockValue, false);
		if (_player != null)
		{
			Manager.BroadcastPlayByLocalPlayer(_blockPos.ToVector3() + Vector3.one * 0.5f, flag ? this.openSound : this.closeSound);
		}
		return true;
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0002E5F4 File Offset: 0x0002C7F4
	public void HandleOpenCloseSound(bool isOpen, Vector3i currentPos)
	{
		if (isOpen)
		{
			Manager.BroadcastPlayByLocalPlayer(currentPos.ToVector3() + Vector3.one * 0.5f, this.openSound);
			return;
		}
		Manager.BroadcastPlayByLocalPlayer(currentPos.ToVector3() + Vector3.one * 0.5f, this.closeSound);
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0002E654 File Offset: 0x0002C854
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

	// Token: 0x0600068F RID: 1679 RVA: 0x0002E6AC File Offset: 0x0002C8AC
	public override void ForceAnimationState(BlockValue _blockValue, BlockEntityData _ebcd)
	{
		if (_ebcd != null && _ebcd.bHasTransform)
		{
			Animator[] componentsInChildren = _ebcd.transform.GetComponentsInChildren<Animator>();
			if (componentsInChildren != null)
			{
				bool flag = BlockDoor.IsDoorOpen(_blockValue.meta);
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

	// Token: 0x06000690 RID: 1680 RVA: 0x0002E728 File Offset: 0x0002C928
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

	// Token: 0x06000691 RID: 1681 RVA: 0x0002E77A File Offset: 0x0002C97A
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		this.updateOpenCloseState(BlockDoor.IsDoorOpen(_blockValue.meta), _world, _blockPos, _chunk.ClrIdx, _blockValue, true);
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public override float GetStepHeight(IBlockAccess world, Vector3i blockPos, BlockValue blockDef, BlockFace stepFace)
	{
		return 0f;
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x0002E7B8 File Offset: 0x0002C9B8
	public bool IsOpen(IBlockAccess _blockAccess, int _x, int _y, int _z)
	{
		return BlockDoor.IsDoorOpen(_blockAccess.GetBlock(_x, _y, _z).meta);
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x0002E7DC File Offset: 0x0002C9DC
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (_blockValue.ischild)
		{
			Vector3i parentPos = _blockValue.Block.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			BlockValue block = _world.GetBlock(parentPos);
			return this.GetBlockActivationCommands(_world, block, _clrIdx, parentPos, _entityFocusing);
		}
		this.cmds[0].enabled = BlockDoor.IsDoorOpen(_blockValue.meta);
		this.cmds[1].enabled = !BlockDoor.IsDoorOpen(_blockValue.meta);
		return this.cmds;
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x0002E860 File Offset: 0x0002CA60
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

	// Token: 0x06000697 RID: 1687 RVA: 0x0002E9B0 File Offset: 0x0002CBB0
	public override void RenderDecorations(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, INeighborBlockCache _nBlocks)
	{
		if (!(this.shape is BlockShapeModelEntity) || (_blockValue.meta & 2) == 0)
		{
			this.shape.renderDecorations(_worldPos, _blockValue, _drawPos, _vertices, _lightingAround, _textureFullArray, _meshes, _nBlocks);
		}
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x0002E9F0 File Offset: 0x0002CBF0
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		return string.Format(Localization.Get("useBlock", false), arg, _blockValue.Block.GetLocalizedBlockName());
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsExplosionAffected()
	{
		return false;
	}

	// Token: 0x040007AA RID: 1962
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropPlaceEverywhere = "PlaceEverywhere";

	// Token: 0x040007AB RID: 1963
	[PublicizedFrom(EAccessModifier.Protected)]
	public string openSound;

	// Token: 0x040007AC RID: 1964
	[PublicizedFrom(EAccessModifier.Protected)]
	public string closeSound;

	// Token: 0x040007AD RID: 1965
	[PublicizedFrom(EAccessModifier.Protected)]
	public HashSet<string> tintableMaterials = new HashSet<string>();

	// Token: 0x040007AE RID: 1966
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bPlaceEverywhere;

	// Token: 0x040007AF RID: 1967
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("close", "door", false, false, null),
		new BlockActivationCommand("open", "door", false, false, null)
	};
}
