using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200013E RID: 318
[Preserve]
public class BlockSpikes : BlockDamage
{
	// Token: 0x060008E3 RID: 2275 RVA: 0x0003E21C File Offset: 0x0003C41C
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey(BlockSpikes.PropDontDamageOnTouch))
		{
			this.bDontDamageOnTouch = StringParsers.ParseBool(base.Properties.Values[BlockSpikes.PropDontDamageOnTouch], 0, -1, true);
		}
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0003E26C File Offset: 0x0003C46C
	public override void GetCollisionAABB(BlockValue _blockValue, int _x, int _y, int _z, float _distortedY, List<Bounds> _result)
	{
		base.GetCollisionAABB(_blockValue, _x, _y, _z, _distortedY, _result);
		Vector3 b = new Vector3(-0.3f, -0.2f, -0.3f);
		for (int i = 0; i < _result.Count; i++)
		{
			Bounds value = _result[i];
			value.extents = Vector3.Max(value.extents + b, Vector3.zero);
			_result[i] = value;
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace crossingFace)
	{
		return true;
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public override float GetStepHeight(IBlockAccess world, Vector3i blockPos, BlockValue _blockValue, BlockFace crossingFace)
	{
		return 1f;
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x0003E2E8 File Offset: 0x0003C4E8
	public override bool CanPlaceBlockAt(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bOmitCollideCheck = false)
	{
		return base.CanPlaceBlockAt(_world, _clrIdx, _blockPos, _blockValue, _bOmitCollideCheck) && _world.GetBlock(_clrIdx, _blockPos - Vector3i.up).Block.shape.IsSolidCube;
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x0003E32C File Offset: 0x0003C52C
	public override bool OnEntityCollidedWithBlock(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, Entity _targetEntity)
	{
		if (!base.OnEntityCollidedWithBlock(_world, _clrIdx, _blockPos, _blockValue, _targetEntity))
		{
			return false;
		}
		BlockValue block = _world.GetBlock(_clrIdx, _blockPos);
		if (!this.SiblingBlock.isair)
		{
			block.type = this.SiblingBlock.type;
			block.damage = 0;
			_world.SetBlockRPC(_clrIdx, _blockPos, block);
		}
		else
		{
			_world.SetBlockRPC(_clrIdx, _blockPos, BlockValue.Air);
		}
		return true;
	}

	// Token: 0x040008B1 RID: 2225
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropDontDamageOnTouch = "DontDamageOnTouch";

	// Token: 0x040008B2 RID: 2226
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDontDamageOnTouch;
}
