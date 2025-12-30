using System;
using UnityEngine.Scripting;

// Token: 0x02000170 RID: 368
[Preserve]
public class BlockShapeBillboardAbstract : BlockShape
{
	// Token: 0x06000B03 RID: 2819 RVA: 0x0004724B File Offset: 0x0004544B
	public BlockShapeBillboardAbstract()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.LightOpacity = 0;
		this.IsOmitTerrainSnappingUp = true;
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsRenderDecoration()
	{
		return true;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return false;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x040009BC RID: 2492
	public float yPosSubtract;
}
