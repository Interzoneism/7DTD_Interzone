using System;
using UnityEngine.Scripting;

// Token: 0x020000FE RID: 254
[Preserve]
public class BlockDrawBridge : BlockDoorSecure
{
	// Token: 0x060006AF RID: 1711 RVA: 0x0002F434 File Offset: 0x0002D634
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace _face)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			return !BlockDoor.IsDoorOpen(_world.GetBlock(parentPos).meta) || _blockPos.y == parentPos.y;
		}
		return true;
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x0002F488 File Offset: 0x0002D688
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		if (this.isMultiBlock && _blockValue.ischild)
		{
			Vector3i parentPos = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			return !BlockDoor.IsDoorOpen(_world.GetBlock(parentPos).meta) || _blockPos.y == parentPos.y;
		}
		return true;
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0002F4DC File Offset: 0x0002D6DC
	public override float GetStepHeight(IBlockAccess world, Vector3i _blockPos, BlockValue _blockValue, BlockFace crossingFace)
	{
		if (world == null)
		{
			return 0f;
		}
		Vector3i vector3i = _blockPos;
		if (this.isMultiBlock && _blockValue.ischild)
		{
			vector3i = this.multiBlockPos.GetParentPos(_blockPos, _blockValue);
			_blockValue = world.GetBlock(vector3i);
		}
		if (!BlockDoor.IsDoorOpen(_blockValue.meta))
		{
			return 1f;
		}
		if (_blockPos.y == vector3i.y)
		{
			return 1f;
		}
		return 0f;
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x0002F549 File Offset: 0x0002D749
	public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, GameRandom _rnd)
	{
		_bpResult.blockValue.meta = (_bpResult.blockValue.meta | 1);
	}
}
