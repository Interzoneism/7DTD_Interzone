using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200017C RID: 380
[Preserve]
public class BlockShapeExt3dModel : BlockShapeRotatedAbstract
{
	// Token: 0x06000B37 RID: 2871 RVA: 0x00049587 File Offset: 0x00047787
	public BlockShapeExt3dModel()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.LightOpacity = 0;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x000495B4 File Offset: 0x000477B4
	public override void Init(Block _block)
	{
		this.ext3dModelName = _block.Properties.Values["Model"];
		if (this.ext3dModelName == null)
		{
			throw new Exception("No model specified on block with name " + _block.GetBlockName());
		}
		this.modelOffset = Vector3.zero;
		if (_block.Properties.Values.ContainsKey("ModelOffset"))
		{
			this.modelOffset = StringParsers.ParseVector3(_block.Properties.Values["ModelOffset"], 0, -1);
		}
		base.Init(_block);
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x00049645 File Offset: 0x00047845
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createBoundingBoxes()
	{
		if (this.modelMesh == null || this.modelMesh.boundingBoxMesh.Vertices.Count > 0)
		{
			return;
		}
		base.createBoundingBoxes();
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00049670 File Offset: 0x00047870
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void createVertices()
	{
		TextureAtlasExternalModels textureAtlasExternalModels = (MeshDescription.meshes != null) ? ((TextureAtlasExternalModels)MeshDescription.meshes[0].textureAtlas) : null;
		if (textureAtlasExternalModels == null)
		{
			return;
		}
		if (!textureAtlasExternalModels.Meshes.ContainsKey(this.ext3dModelName))
		{
			throw new Exception("External 3D model with name '" + this.ext3dModelName + "' not found! Maybe you need to create the atlas first?");
		}
		this.modelMesh = ((TextureAtlasExternalModels)MeshDescription.meshes[0].textureAtlas).Meshes[this.ext3dModelName];
		this.vertices = this.modelMesh.Vertices.ToArray();
		this.normals = this.modelMesh.Normals.ToArray();
		if (this.modelMesh != null && this.modelMesh.aabb != null && this.modelMesh.aabb.Length != 0)
		{
			this.boundsArr = new Bounds[this.modelMesh.aabb.Length];
			for (int i = 0; i < this.modelMesh.aabb.Length; i++)
			{
				this.boundsArr[i] = new Bounds(this.modelMesh.aabb[i].center, this.modelMesh.aabb[i].size);
			}
		}
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x000497B1 File Offset: 0x000479B1
	public override Quaternion GetRotation(BlockValue _blockValue)
	{
		return BlockShapeNew.GetRotationStatic((int)_blockValue.rotation);
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x000497C0 File Offset: 0x000479C0
	public override void renderFull(Vector3i _worldPos, BlockValue _blockValue, Vector3 _drawPos, Vector3[] _vertices, LightingAround _lightingAround, TextureFullArray _textureFullArray, VoxelMesh[] _meshes, BlockShape.MeshPurpose _purpose = BlockShape.MeshPurpose.World)
	{
		byte sun = _lightingAround[LightingAround.Pos.Middle].sun;
		byte block = _lightingAround[LightingAround.Pos.Middle].block;
		Vector3[] array = base.rotateVertices(this.vertices, _drawPos + this.modelOffset, _blockValue);
		Vector3[] array2 = this.rotateNormals(this.normals, _blockValue);
		Block block2 = _blockValue.Block;
		byte meshIndex = block2.MeshIndex;
		_meshes[(int)meshIndex].CheckVertexLimit(array.Length);
		_meshes[(int)meshIndex].AddMesh(_drawPos, this.vertices.Length, array, array2, this.modelMesh.Indices, this.modelMesh.Uvs, sun, block, this.GetBoundsMesh(_blockValue), (int)(10f * (float)_blockValue.damage) / block2.MaxDamage);
		MemoryPools.poolVector3.Free(array);
		MemoryPools.poolVector3.Free(array2);
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00049890 File Offset: 0x00047A90
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3[] rotateNormals(Vector3[] _normals, BlockValue _blockValue)
	{
		Quaternion rotation = this.GetRotation(_blockValue);
		Vector3[] array = MemoryPools.poolVector3.Alloc(_normals.Length);
		for (int i = 0; i < _normals.Length; i++)
		{
			array[i] = rotation * _normals[i];
		}
		return array;
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x000498D6 File Offset: 0x00047AD6
	public override float GetStepHeight(BlockValue _blockValue, BlockFace crossingFace)
	{
		if (_blockValue.Block.HasTag(BlockTags.Door) || _blockValue.Block.HasTag(BlockTags.Window))
		{
			return 0f;
		}
		return base.GetStepHeight(_blockValue, crossingFace);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00049904 File Offset: 0x00047B04
	public override bool IsMovementBlocked(BlockValue _blockValue, BlockFace crossingFace)
	{
		return _blockValue.Block.HasTag(BlockTags.Door) || _blockValue.Block.HasTag(BlockTags.Window) || base.IsMovementBlocked(_blockValue, crossingFace);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x0004992E File Offset: 0x00047B2E
	public override byte Rotate(bool _bLeft, int _rotation)
	{
		_rotation += (_bLeft ? -1 : 1);
		if (_rotation > 9)
		{
			_rotation = 0;
		}
		if (_rotation < 0)
		{
			_rotation = 9;
		}
		return (byte)_rotation;
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00049950 File Offset: 0x00047B50
	public override BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		if (_bLeft)
		{
			_rotCount = -_rotCount;
		}
		int rotation = (int)_blockValue.rotation;
		if (rotation >= 24)
		{
			_blockValue.rotation = (byte)((rotation - 24 + _rotCount & 3) + 24);
		}
		else
		{
			int num = 90 * _rotCount;
			_blockValue.rotation = (byte)BlockShapeNew.ConvertRotationFree(rotation, Quaternion.AngleAxis((float)num, Vector3.up), false);
		}
		return _blockValue;
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x000499A7 File Offset: 0x00047BA7
	public override Quaternion GetPreviewRotation()
	{
		return Quaternion.AngleAxis(180f, Vector3.up);
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x000499B8 File Offset: 0x00047BB8
	public override VoxelMesh GetBoundsMesh(BlockValue _blockValue)
	{
		VoxelMesh voxelMesh = this.cachedRotatedBoundsMeshes[(int)_blockValue.rotation];
		if (voxelMesh == null)
		{
			Vector3[] vertices = this.modelMesh.boundingBoxMesh.Vertices.ToArray();
			Vector3[] array = base.rotateVertices(vertices, Vector3.zero, _blockValue);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] += this.modelOffset;
			}
			voxelMesh = new VoxelMesh(-1, 0, VoxelMesh.CreateFlags.Default);
			voxelMesh.Vertices.AddRange(array, 0, array.Length);
			voxelMesh.Indices = this.modelMesh.boundingBoxMesh.Indices;
			MemoryPools.poolVector3.Free(array);
			this.cachedRotatedBoundsMeshes[(int)_blockValue.rotation] = voxelMesh;
		}
		return voxelMesh;
	}

	// Token: 0x040009CD RID: 2509
	public string ext3dModelName;

	// Token: 0x040009CE RID: 2510
	[PublicizedFrom(EAccessModifier.Protected)]
	public VoxelMeshExt3dModel modelMesh;

	// Token: 0x040009CF RID: 2511
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3[] normals;

	// Token: 0x040009D0 RID: 2512
	[PublicizedFrom(EAccessModifier.Private)]
	public VoxelMesh[] cachedRotatedBoundsMeshes = new VoxelMesh[32];

	// Token: 0x040009D1 RID: 2513
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 modelOffset;
}
