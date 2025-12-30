using System;
using UnityEngine.Scripting;

// Token: 0x020000F1 RID: 241
[Preserve]
public class BlockBackpack : BlockLoot
{
	// Token: 0x06000637 RID: 1591 RVA: 0x0002CD28 File Offset: 0x0002AF28
	public override void OnBlockRemoved(WorldBase world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(world, _chunk, _blockPos, _blockValue);
		EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
		if (primaryPlayer != null && primaryPlayer.EqualsDroppedBackpackPositions(_blockPos))
		{
			primaryPlayer.SetDroppedBackpackPositions(primaryPlayer.persistentPlayerData.GetDroppedBackpackPositions());
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsWaterBlocked(IBlockAccess _world, Vector3i _blockPos, BlockValue _blockDef, BlockFaceFlag _sides)
	{
		return true;
	}
}
