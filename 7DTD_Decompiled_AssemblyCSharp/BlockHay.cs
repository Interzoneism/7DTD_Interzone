using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000103 RID: 259
[Preserve]
public class BlockHay : Block
{
	// Token: 0x060006CF RID: 1743 RVA: 0x0002FEE6 File Offset: 0x0002E0E6
	public BlockHay()
	{
		this.IsCheckCollideWithEntity = true;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x0002FEF8 File Offset: 0x0002E0F8
	public override void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedY, List<Bounds> _result)
	{
		float num = 0.0625f;
		_result.Add(BoundsUtils.BoundsForMinMax((float)_x + num, (float)_y, (float)_z + num, (float)(_x + 1) - num, (float)(_y + 1) - num, (float)(_z + 1) - num));
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x0002D787 File Offset: 0x0002B987
	public override IList<Bounds> GetClipBoundsList(BlockValue _blockValue, Vector3 _blockPos)
	{
		Block.staticList_IntersectRayWithBlockList.Clear();
		this.GetCollisionAABB(_blockValue, (int)_blockPos.x, (int)_blockPos.y, (int)_blockPos.z, 0f, Block.staticList_IntersectRayWithBlockList);
		return Block.staticList_IntersectRayWithBlockList;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x0002FF35 File Offset: 0x0002E135
	public override bool OnEntityCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Entity _e)
	{
		_e.fallDistance = Mathf.Max(_e.fallDistance - 5f, 0f);
		return true;
	}
}
