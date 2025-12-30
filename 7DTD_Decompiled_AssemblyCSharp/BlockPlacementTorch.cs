using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020000DB RID: 219
[Preserve]
public class BlockPlacementTorch : BlockPlacement
{
	// Token: 0x06000549 RID: 1353 RVA: 0x00025A68 File Offset: 0x00023C68
	public override BlockPlacement.Result OnPlaceBlock(BlockPlacement.EnumRotationMode _mode, int _localRot, WorldBase _world, BlockValue _blockValue, HitInfoDetails _hitInfo, Vector3 _entityPos)
	{
		if (_mode != BlockPlacement.EnumRotationMode.Auto)
		{
			return base.OnPlaceBlock(_mode, _localRot, _world, _blockValue, _hitInfo, _entityPos);
		}
		BlockPlacement.Result result = new BlockPlacement.Result(_blockValue, _hitInfo);
		result.blockValue.meta = 0;
		switch (_hitInfo.blockFace)
		{
		case BlockFace.North:
			result.blockValue.rotation = 0;
			break;
		case BlockFace.West:
			result.blockValue.rotation = 3;
			break;
		case BlockFace.South:
			result.blockValue.rotation = 2;
			break;
		case BlockFace.East:
			result.blockValue.rotation = 1;
			break;
		}
		return result;
	}
}
