using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000179 RID: 377
[Preserve]
public class BlockShapeCubeCutoutBackFaces : BlockShapeCubeCutout
{
	// Token: 0x06000B29 RID: 2857 RVA: 0x000492DC File Offset: 0x000474DC
	public BlockShapeCubeCutoutBackFaces()
	{
		this.IsSolidCube = false;
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x000492C7 File Offset: 0x000474C7
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return _blockValue.type != _adjBlockValue.type;
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x000492EC File Offset: 0x000474EC
	public override void renderFace(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, BlockFace _face, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		base.renderFace(_worldPos, _blockValue, _drawPos, _face, _vertices, _lightingAround, _textureFullArray, _meshes, _purpose);
		byte meshIndex = _blockValue.Block.MeshIndex;
		Rect uvrectFromSideAndMetadata = this.block.getUVRectFromSideAndMetadata((int)meshIndex, _face, _vertices, _blockValue);
		_meshes[(int)meshIndex].AddQuadWithCracks(_vertices[3], Color.white, _vertices[2], Color.white, _vertices[1], Color.white, _vertices[0], Color.white, uvrectFromSideAndMetadata, WorldConstants.MapDamageToUVRect(_blockValue), false);
	}
}
