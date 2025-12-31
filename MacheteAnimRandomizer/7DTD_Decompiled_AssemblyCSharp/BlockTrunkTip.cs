using System;
using UnityEngine.Scripting;

// Token: 0x0200014E RID: 334
[Preserve]
public class BlockTrunkTip : BlockDamage
{
	// Token: 0x0600094C RID: 2380 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool RotateVerticesOnCollisionCheck(BlockValue _blockValue)
	{
		return false;
	}
}
