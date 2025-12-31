using System;
using UnityEngine.Scripting;

// Token: 0x0200017F RID: 383
[Preserve]
public class BlockShapeInvisible : BlockShape
{
	// Token: 0x06000B4B RID: 2891 RVA: 0x00049E6C File Offset: 0x0004806C
	public BlockShapeInvisible()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.LightOpacity = 0;
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x00049E89 File Offset: 0x00048089
	public override void Init(Block _block)
	{
		base.Init(_block);
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return false;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}
}
