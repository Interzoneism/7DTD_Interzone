using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000D8 RID: 216
[Preserve]
public class BlockPlacementPineLeaves : BlockPlacement
{
	// Token: 0x06000543 RID: 1347 RVA: 0x00025834 File Offset: 0x00023A34
	public override BlockPlacement.Result OnPlaceBlock(BlockPlacement.EnumRotationMode _mode, int _localRot, WorldBase _world, BlockValue _blockValue, HitInfoDetails _hitInfo, Vector3 _entityPos)
	{
		if (_mode != BlockPlacement.EnumRotationMode.Auto)
		{
			return base.OnPlaceBlock(_mode, _localRot, _world, _blockValue, _hitInfo, _entityPos);
		}
		BlockPlacement.Result result = new BlockPlacement.Result(_blockValue, _hitInfo);
		result.blockValue.rotation = 0;
		return result;
	}
}
