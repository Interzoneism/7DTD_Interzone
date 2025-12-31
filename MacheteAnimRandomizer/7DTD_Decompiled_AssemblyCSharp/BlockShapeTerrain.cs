using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000189 RID: 393
[Preserve]
public class BlockShapeTerrain : BlockShapeCube
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x0004ED78 File Offset: 0x0004CF78
	public BlockShapeTerrain()
	{
		this.IsOmitTerrainSnappingUp = true;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0004EE40 File Offset: 0x0004D040
	public override void renderFace(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, BlockFace _face, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		float num = _drawPos.y - _vertices[0].y + 1f + 0.0001f;
		if (num > -0.01f)
		{
			num = 0.0001f;
		}
		UVRectTiling uvrectTiling = MeshDescription.meshes[4].textureAtlas.uvMapping[500 + (int)_blockValue.decaltex];
		Utils.MoveInBlockFaceDirection(_vertices, _face, num);
		_meshes[4].AddQuadNoCollision(_vertices[0], _vertices[1], _vertices[2], _vertices[3], Color.white, uvrectTiling.uv);
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0004EEDE File Offset: 0x0004D0DE
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return _blockValue.hasdecal && _blockValue.decalface == _face;
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsTerrain()
	{
		return true;
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0004EEF8 File Offset: 0x0004D0F8
	public override void renderFull(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		byte sun = _lightingAround[LightingAround.Pos.Middle].sun;
		byte block = _lightingAround[LightingAround.Pos.Middle].block;
		VoxelMeshTerrain voxelMeshTerrain = (VoxelMeshTerrain)_meshes[5];
		Block block2 = _blockValue.Block;
		byte meshIndex = block2.MeshIndex;
		voxelMeshTerrain.AddBlockSideTri(this.v[0] + _drawPos, this.v[2] + _drawPos, this.v[1] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_BOTTOM, BlockFace.Bottom, sun, block);
		voxelMeshTerrain.AddBlockSideTri(this.v[0] + _drawPos, this.v[1] + _drawPos, this.v[4] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_BOTTOM, BlockFace.Bottom, sun, block);
		voxelMeshTerrain.AddBlockSideTri(this.v[2] + _drawPos, this.v[5] + _drawPos, this.v[1] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block);
		voxelMeshTerrain.AddBlockSideTri(this.v[1] + _drawPos, this.v[5] + _drawPos, this.v[4] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block);
		voxelMeshTerrain.AddBlockSideTri(this.v[3] + _drawPos, this.v[5] + _drawPos, this.v[2] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_BOTTOM, BlockFace.Bottom, sun, block);
		voxelMeshTerrain.AddBlockSideTri(this.v[0] + _drawPos, this.v[3] + _drawPos, this.v[2] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_BOTTOM, BlockFace.Bottom, sun, block);
		voxelMeshTerrain.AddBlockSideTri(this.v[0] + _drawPos, this.v[4] + _drawPos, this.v[3] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block);
		voxelMeshTerrain.AddBlockSideTri(this.v[4] + _drawPos, this.v[5] + _drawPos, this.v[3] + _drawPos, (int)meshIndex, _blockValue, VoxelMesh.COLOR_TOP, BlockFace.Top, sun, block);
		int sideTextureId = block2.GetSideTextureId(_blockValue, BlockFace.Top, 0);
		int sideTextureId2 = block2.GetSideTextureId(_blockValue, BlockFace.South, 0);
		int submesh = voxelMeshTerrain.FindOrCreateSubMesh(sideTextureId << 16 | sideTextureId2, -1, -1);
		for (int i = 0; i < voxelMeshTerrain.Indices.Count; i += 3)
		{
			voxelMeshTerrain.AddIndices(voxelMeshTerrain.Indices[i], voxelMeshTerrain.Indices[i + 1], voxelMeshTerrain.Indices[i + 2], submesh);
		}
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0004F1DD File Offset: 0x0004D3DD
	public override Quaternion GetPreviewRotation()
	{
		return Quaternion.AngleAxis(55f, Vector3.up) * Quaternion.AngleAxis(10f, Vector3.forward);
	}

	// Token: 0x04000A09 RID: 2569
	[PublicizedFrom(EAccessModifier.Private)]
	public new readonly Vector3[] v = new Vector3[]
	{
		new Vector3(0.5f, 0f, 0.5f),
		new Vector3(0.5f, 0.5f, 0f),
		new Vector3(1f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 1f),
		new Vector3(0f, 0.5f, 0.5f),
		new Vector3(0.5f, 1f, 0.5f)
	};
}
