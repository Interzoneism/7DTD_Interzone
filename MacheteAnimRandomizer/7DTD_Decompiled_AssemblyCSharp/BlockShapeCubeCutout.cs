using System;
using UnityEngine.Scripting;

// Token: 0x02000178 RID: 376
[Preserve]
public class BlockShapeCubeCutout : BlockShapeCube
{
	// Token: 0x06000B26 RID: 2854 RVA: 0x000492B8 File Offset: 0x000474B8
	public BlockShapeCubeCutout()
	{
		this.IsSolidCube = false;
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x000492C7 File Offset: 0x000474C7
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return _blockValue.type != _adjBlockValue.type;
	}
}
