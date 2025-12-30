using System;
using UnityEngine.Scripting;

// Token: 0x020000F9 RID: 249
[Preserve]
public class BlockDeadgrass : Block
{
	// Token: 0x06000670 RID: 1648 RVA: 0x0002D909 File Offset: 0x0002BB09
	public override void Init()
	{
		base.Init();
		this.IsDecoration = true;
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0002D918 File Offset: 0x0002BB18
	public override void OnNeighborBlockChange(WorldBase world, int _clrIdx, Vector3i _myBlockPos, BlockValue _myBlockValue, Vector3i _blockPosThatChanged, BlockValue _newNeighborBlockValue, BlockValue _oldNeighborBlockValue)
	{
		if (_blockPosThatChanged.x == _myBlockPos.x && _blockPosThatChanged.z == _myBlockPos.z && _blockPosThatChanged.y == _myBlockPos.y - 1 && !_newNeighborBlockValue.Block.shape.IsSolidCube)
		{
			world.SetBlockRPC(_myBlockPos, BlockValue.Air);
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0002D973 File Offset: 0x0002BB73
	public override BlockValue OnBlockPlaced(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, GameRandom _rnd)
	{
		_blockValue.meta = (byte)_rnd.RandomRange(16);
		return _blockValue;
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0002D988 File Offset: 0x0002BB88
	public override void OnBlockPlaceBefore(WorldBase _world, ref BlockPlacement.Result _bpResult, EntityAlive _ea, GameRandom _rnd)
	{
		_bpResult.blockValue.meta = (byte)_rnd.RandomRange(16);
	}
}
