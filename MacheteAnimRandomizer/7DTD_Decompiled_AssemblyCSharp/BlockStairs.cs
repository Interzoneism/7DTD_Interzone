using System;
using UnityEngine.Scripting;

// Token: 0x02000140 RID: 320
[Preserve]
public class BlockStairs : Block
{
	// Token: 0x060008F9 RID: 2297 RVA: 0x0003EAED File Offset: 0x0003CCED
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFace _face)
	{
		return !_blockValue.ischild;
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x0003EAED File Offset: 0x0003CCED
	public override bool IsMovementBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockValue, BlockFaceFlag _sides)
	{
		return !_blockValue.ischild;
	}
}
