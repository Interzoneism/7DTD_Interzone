using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200018A RID: 394
[Preserve]
public class BlockShapeWater : BlockShapeCube
{
	// Token: 0x06000BB1 RID: 2993 RVA: 0x0004F202 File Offset: 0x0004D402
	public BlockShapeWater()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.LightOpacity = 0;
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0004F21F File Offset: 0x0004D41F
	public override void renderFace(Vector3[] _vertices, LightingAround _lightingAround, long _textureFull, VoxelMesh[] _meshes, Vector2 UVdata, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		_meshes[1].AddBasicQuad(_vertices, Color.white, UVdata, true, false);
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return false;
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public override float GetStepHeight(BlockValue _blockValue, BlockFace crossingFace)
	{
		return 0f;
	}

	// Token: 0x06000BB6 RID: 2998 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMovementBlocked(BlockValue _blockValue, BlockFace crossingFace)
	{
		return false;
	}
}
