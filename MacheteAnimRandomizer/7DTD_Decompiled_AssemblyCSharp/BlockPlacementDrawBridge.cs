using System;
using UnityEngine.Scripting;

// Token: 0x020000D7 RID: 215
[Preserve]
public class BlockPlacementDrawBridge : BlockPlacementTowardsPlacer
{
	// Token: 0x06000541 RID: 1345 RVA: 0x000257EC File Offset: 0x000239EC
	public override byte LimitRotation(BlockPlacement.EnumRotationMode _mode, ref int _localRot, HitInfoDetails _hitInfo, bool _bAdd, BlockValue _bv, byte _rotation)
	{
		if (_mode != BlockPlacement.EnumRotationMode.Auto)
		{
			return base.LimitRotation(_mode, ref _localRot, _hitInfo, _bAdd, _bv, _rotation);
		}
		int num = (int)(_bAdd ? (_rotation + 1) : (_rotation - 1));
		if (num > 3)
		{
			return 0;
		}
		if (num < 0)
		{
			return 3;
		}
		return (byte)num;
	}
}
