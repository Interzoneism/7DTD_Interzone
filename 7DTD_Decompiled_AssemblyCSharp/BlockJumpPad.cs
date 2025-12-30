using System;
using UnityEngine.Scripting;

// Token: 0x02000105 RID: 261
[Preserve]
public class BlockJumpPad : Block
{
	// Token: 0x060006E8 RID: 1768 RVA: 0x000308D1 File Offset: 0x0002EAD1
	public override void OnEntityWalking(WorldBase _world, int _x, int _y, int _z, BlockValue _blockValue, Entity entity)
	{
		entity.motion.y = 3f;
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override BlockFace getInventoryFace()
	{
		return BlockFace.Top;
	}
}
