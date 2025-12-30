using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000A6B RID: 2667
public class VoxelMesh
{
	// Token: 0x06005117 RID: 20759 RVA: 0x00206B18 File Offset: 0x00204D18
	public VoxelMesh(int _meshIndex, int _minSize = 1024, VoxelMesh.CreateFlags _flags = VoxelMesh.CreateFlags.Default)
	{
		this.meshIndex = _meshIndex;
		this.m_Vertices = new ArrayListMP<Vector3>(MemoryPools.poolVector3, _minSize);
		this.m_Indices = new ArrayListMP<int>(MemoryPools.poolInt, _minSize);
		this.m_Uvs = new ArrayListMP<Vector2>(MemoryPools.poolVector2, _minSize);
		if ((_flags & VoxelMesh.CreateFlags.Cracks) > VoxelMesh.CreateFlags.None)
		{
			this.UvsCrack = new ArrayListMP<Vector2>(MemoryPools.poolVector2, _minSize);
		}
		this.m_Normals = new ArrayListMP<Vector3>(MemoryPools.poolVector3, _minSize);
		this.m_Tangents = new ArrayListMP<Vector4>(MemoryPools.poolVector4, _minSize);
		this.m_ColorVertices = new ArrayListMP<Color>(MemoryPools.poolColor, _minSize);
		if ((_flags & VoxelMesh.CreateFlags.Collider) > VoxelMesh.CreateFlags.None)
		{
			this.m_CollVertices = new ArrayListMP<Vector3>(MemoryPools.poolVector3, _minSize);
			this.m_CollIndices = new ArrayListMP<int>(MemoryPools.poolInt, _minSize);
		}
	}

	// Token: 0x06005118 RID: 20760 RVA: 0x00206BD7 File Offset: 0x00204DD7
	public static VoxelMesh Create(int _meshIdx, VoxelMesh.EnumMeshType _meshType, int _minSize = 500)
	{
		if (_meshType == VoxelMesh.EnumMeshType.Terrain)
		{
			return new VoxelMeshTerrain(_meshIdx, _minSize);
		}
		if (_meshType == VoxelMesh.EnumMeshType.Decals)
		{
			return new VoxelMesh(_meshIdx, _minSize, VoxelMesh.CreateFlags.None);
		}
		return new VoxelMesh(_meshIdx, _minSize, VoxelMesh.CreateFlags.Default);
	}

	// Token: 0x06005119 RID: 20761 RVA: 0x00206BFA File Offset: 0x00204DFA
	public void SetTemperature(float _temperature)
	{
		this.temperature = _temperature;
	}

	// Token: 0x0600511A RID: 20762 RVA: 0x00206C04 File Offset: 0x00204E04
	public virtual void Write(BinaryWriter _bw)
	{
		_bw.Write((uint)this.m_Vertices.Count);
		for (int i = 0; i < this.m_Vertices.Count; i++)
		{
			_bw.Write(this.m_Vertices[i].x);
			_bw.Write(this.m_Vertices[i].y);
			_bw.Write(this.m_Vertices[i].z);
		}
		_bw.Write((uint)this.m_Normals.Count);
		for (int j = 0; j < this.m_Normals.Count; j++)
		{
			_bw.Write(this.m_Normals[j].x);
			_bw.Write(this.m_Normals[j].y);
			_bw.Write(this.m_Normals[j].z);
		}
		_bw.Write((uint)this.m_Uvs.Count);
		for (int k = 0; k < this.m_Uvs.Count; k++)
		{
			_bw.Write(this.m_Uvs[k].x);
			_bw.Write(this.m_Uvs[k].y);
		}
		ArrayListMP<Vector2> uvsCrack = this.UvsCrack;
		int num = (uvsCrack != null) ? uvsCrack.Count : 0;
		_bw.Write((uint)num);
		for (int l = 0; l < num; l++)
		{
			_bw.Write(this.UvsCrack[l].x);
			_bw.Write(this.UvsCrack[l].y);
		}
		_bw.Write((uint)this.m_ColorVertices.Count);
		for (int m = 0; m < this.m_ColorVertices.Count; m++)
		{
			_bw.Write(this.m_ColorVertices[m].r);
			_bw.Write(this.m_ColorVertices[m].g);
			_bw.Write(this.m_ColorVertices[m].b);
			_bw.Write(this.m_ColorVertices[m].a);
		}
		_bw.Write((uint)this.m_Indices.Count);
		for (int n = 0; n < this.m_Indices.Count; n++)
		{
			_bw.Write(this.m_Indices[n]);
		}
	}

	// Token: 0x0600511B RID: 20763 RVA: 0x00206E70 File Offset: 0x00205070
	public virtual void Read(BinaryReader _br)
	{
		uint num = _br.ReadUInt32();
		this.m_Vertices.Clear();
		int num2 = this.m_Vertices.Alloc((int)num);
		int num3 = 0;
		while ((long)num3 < (long)((ulong)num))
		{
			this.m_Vertices[num2++] = new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
			num3++;
		}
		num = _br.ReadUInt32();
		this.m_Normals.Clear();
		num2 = this.m_Normals.Alloc((int)num);
		int num4 = 0;
		while ((long)num4 < (long)((ulong)num))
		{
			this.m_Normals[num2++] = new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
			num4++;
		}
		num = _br.ReadUInt32();
		this.m_Uvs.Clear();
		num2 = this.m_Uvs.Alloc((int)num);
		int num5 = 0;
		while ((long)num5 < (long)((ulong)num))
		{
			this.m_Uvs[num2++] = new Vector2(_br.ReadSingle(), _br.ReadSingle());
			num5++;
		}
		num = _br.ReadUInt32();
		ArrayListMP<Vector2> uvsCrack = this.UvsCrack;
		if (uvsCrack != null)
		{
			uvsCrack.Clear();
		}
		if (num > 0U)
		{
			num2 = this.UvsCrack.Alloc((int)num);
			int num6 = 0;
			while ((long)num6 < (long)((ulong)num))
			{
				this.UvsCrack[num2++] = new Vector2(_br.ReadSingle(), _br.ReadSingle());
				num6++;
			}
		}
		num = _br.ReadUInt32();
		this.m_ColorVertices.Clear();
		num2 = this.m_ColorVertices.Alloc((int)num);
		int num7 = 0;
		while ((long)num7 < (long)((ulong)num))
		{
			this.m_ColorVertices[num2++] = new Color(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
			num7++;
		}
		num = _br.ReadUInt32();
		this.m_Indices.Clear();
		num2 = this.m_Indices.Alloc((int)num);
		int num8 = 0;
		while ((long)num8 < (long)((ulong)num))
		{
			this.m_Indices[num2++] = _br.ReadInt32();
			num8++;
		}
		this.m_Tangents.Clear();
	}

	// Token: 0x0600511C RID: 20764 RVA: 0x00207084 File Offset: 0x00205284
	public static float BlockFaceToColor(BlockFace _blockFace)
	{
		switch (_blockFace)
		{
		case BlockFace.Top:
			return VoxelMesh.COLOR_TOP;
		case BlockFace.Bottom:
			return VoxelMesh.COLOR_BOTTOM;
		case BlockFace.North:
			return VoxelMesh.COLOR_NORTH;
		case BlockFace.West:
			return VoxelMesh.COLOR_WEST;
		case BlockFace.South:
			return VoxelMesh.COLOR_SOUTH;
		case BlockFace.East:
			return VoxelMesh.COLOR_EAST;
		default:
			return 1f;
		}
	}

	// Token: 0x0600511D RID: 20765 RVA: 0x002070DA File Offset: 0x002052DA
	public static void CreateMeshFilter(int _meshIndex, int _yOffset, GameObject _gameObject, string _meshTag, bool _bAllowLOD, out MeshFilter[] _mf, out MeshRenderer[] _mr)
	{
		_mf = new MeshFilter[1];
		_mr = new MeshRenderer[1];
		VoxelMesh.CreateMeshFilter(_meshIndex, _yOffset, _gameObject, _meshTag, _bAllowLOD, out _mf[0], out _mr[0]);
	}

	// Token: 0x0600511E RID: 20766 RVA: 0x0020710C File Offset: 0x0020530C
	public static void CreateMeshFilter(int _meshIndex, int _yOffset, GameObject _gameObject, string _meshTag, bool _bAllowLOD, out MeshFilter[] _mf, out MeshRenderer[] _mr, out MeshCollider[] _mc)
	{
		_mf = new MeshFilter[1];
		_mr = new MeshRenderer[1];
		VoxelMesh.CreateMeshFilter(_meshIndex, _yOffset, _gameObject, _meshTag, _bAllowLOD, out _mf[0], out _mr[0]);
		_mc = new MeshCollider[1];
		VoxelMesh.CreateMeshCollider(_meshIndex, _gameObject, _meshTag, ref _mc[0]);
	}

	// Token: 0x0600511F RID: 20767 RVA: 0x00207164 File Offset: 0x00205364
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateMeshFilter(int _meshIndex, int _yOffset, GameObject _gameObject, string _meshTag, bool _bAllowLOD, out MeshFilter _mf, out MeshRenderer _mr)
	{
		_mf = null;
		_mr = null;
		MeshDescription meshDescription = MeshDescription.meshes[_meshIndex];
		_gameObject.transform.localPosition = Vector3.zero;
		if (!string.IsNullOrEmpty(_meshTag))
		{
			_gameObject.tag = _meshTag;
		}
		if (meshDescription.materials != null)
		{
			_mf = _gameObject.GetComponent<MeshFilter>();
			if (_mf == null)
			{
				_mf = _gameObject.AddComponent<MeshFilter>();
			}
			_mr = _gameObject.GetComponent<MeshRenderer>();
			if (_mr == null)
			{
				_mr = _gameObject.AddComponent<MeshRenderer>();
			}
			if (_meshIndex != 5)
			{
				if (meshDescription.materials.Length > 1)
				{
					_mr.sharedMaterials = meshDescription.materials;
				}
				else
				{
					_mr.sharedMaterial = meshDescription.materials[0];
				}
			}
			_mr.receiveShadows = meshDescription.bReceiveShadows;
			_mr.shadowCastingMode = (meshDescription.bCastShadows ? ShadowCastingMode.On : ShadowCastingMode.Off);
		}
	}

	// Token: 0x06005120 RID: 20768 RVA: 0x00207238 File Offset: 0x00205438
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CreateMeshCollider(int _meshIndex, GameObject _gameObject, string _meshTag, ref MeshCollider _mc)
	{
		MeshDescription meshDescription = MeshDescription.meshes[_meshIndex];
		if (meshDescription.ColliderLayerName != null && meshDescription.ColliderLayerName.Length > 0)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = _gameObject.name + "Collider";
			if (!string.IsNullOrEmpty(_meshTag))
			{
				gameObject.tag = _meshTag;
			}
			gameObject.transform.parent = _gameObject.transform.parent;
			_mc = gameObject.AddComponent<MeshCollider>();
		}
	}

	// Token: 0x06005121 RID: 20769 RVA: 0x002072AC File Offset: 0x002054AC
	public static Mesh GetPooledMesh()
	{
		int num = VoxelMesh.meshPool.Count;
		Mesh mesh;
		if (num > 0)
		{
			num--;
			mesh = VoxelMesh.meshPool[num];
			VoxelMesh.meshPool.RemoveAt(num);
		}
		else
		{
			mesh = new Mesh();
			mesh.name = "Pool";
		}
		return mesh;
	}

	// Token: 0x06005122 RID: 20770 RVA: 0x002072F7 File Offset: 0x002054F7
	public static void AddPooledMesh(Mesh mesh)
	{
		if (VoxelMesh.meshPool.Count < 250)
		{
			mesh.Clear(false);
			VoxelMesh.meshPool.Add(mesh);
			return;
		}
		UnityEngine.Object.Destroy(mesh);
	}

	// Token: 0x06005123 RID: 20771 RVA: 0x00207324 File Offset: 0x00205524
	public virtual int CopyToMesh(MeshFilter[] _mf, MeshRenderer[] _mr, int _lodLevel, Action _onCopyComplete = null)
	{
		MeshFilter meshFilter = _mf[0];
		Mesh mesh = meshFilter.sharedMesh;
		int count = this.m_Vertices.Count;
		if (count == 0)
		{
			if (mesh)
			{
				meshFilter.sharedMesh = null;
				VoxelMesh.AddPooledMesh(mesh);
			}
			if (_onCopyComplete != null)
			{
				_onCopyComplete();
			}
			return 0;
		}
		if (!mesh)
		{
			mesh = VoxelMesh.GetPooledMesh();
			meshFilter.sharedMesh = mesh;
		}
		else
		{
			mesh.Clear(false);
		}
		if (count != this.m_ColorVertices.Count)
		{
			Log.Error("ERROR: VM.mesh[{2}].Vertices.Count ({0}) != VM.mesh[{2}].ColorSides.Count ({1})", new object[]
			{
				count,
				this.m_ColorVertices.Count,
				this.meshIndex
			});
			if (_onCopyComplete != null)
			{
				_onCopyComplete();
			}
			return this.m_Triangles;
		}
		if (count != this.m_Uvs.Count)
		{
			Log.Error("ERROR: VM.mesh.chunkMesh[{2}].Vertices.Count ({0}) != VM.mesh[{2}].Uvs.Count ({1})", new object[]
			{
				count,
				this.m_Uvs.Count,
				this.meshIndex
			});
			if (_onCopyComplete != null)
			{
				_onCopyComplete();
			}
			return this.m_Triangles;
		}
		this.copyToMesh(mesh, this.m_Vertices, this.m_Indices, this.m_Uvs, this.UvsCrack, this.m_Normals, this.m_Tangents, this.m_ColorVertices, _onCopyComplete);
		return this.m_Triangles;
	}

	// Token: 0x06005124 RID: 20772 RVA: 0x00207478 File Offset: 0x00205678
	public void copyToMesh(Mesh _mesh, ArrayListMP<Vector3> _vertices, ArrayListMP<int> _indices, ArrayListMP<Vector2> _uvs, ArrayListMP<Vector2> _uvCracks, ArrayListMP<Vector3> _normals, ArrayListMP<Vector4> _tangents, ArrayListMP<Color> _colorVertices, Action _onCopyComplete = null)
	{
		if (MeshDataManager.Enabled)
		{
			MeshDataManager.Instance.Add(_mesh, _vertices, _indices, _normals, _tangents, _colorVertices, _uvs, _uvCracks, false, true, true, false, true, _onCopyComplete);
			MeshDataManager.Instance.StartBatches();
			return;
		}
		MeshUnsafeCopyHelper.CopyVertices(_vertices, _mesh);
		if (_uvs.Count > 0)
		{
			MeshUnsafeCopyHelper.CopyUV(_uvs, _mesh);
			if (_uvCracks != null)
			{
				MeshUnsafeCopyHelper.CopyUV2(_uvCracks, _mesh);
			}
		}
		MeshUnsafeCopyHelper.CopyColors(_colorVertices, _mesh);
		MeshUnsafeCopyHelper.CopyTriangles(_indices, _mesh);
		if (_normals.Count == 0)
		{
			_mesh.RecalculateNormals();
		}
		else
		{
			if (_vertices.Count != _normals.Count)
			{
				Log.Error("ERROR: Vertices.Count ({0}) != Normals.Count ({1}), MeshIdx={2} TriIdx={3}", new object[]
				{
					_vertices.Count,
					_normals.Count,
					this.meshIndex,
					this.CurTriangleIndex
				});
			}
			MeshUnsafeCopyHelper.CopyNormals(_normals, _mesh);
		}
		if (_uvs.Count > 0)
		{
			if (_tangents.Count == 0)
			{
				Utils.CalculateMeshTangents(_vertices, _indices, _normals, _uvs, _tangents, _mesh, false);
			}
			if (_vertices.Count != _tangents.Count)
			{
				Log.Out("copyToMesh {0} verts #{1} != tangents #{2}, MeshIdx={3} TriIdx={4}", new object[]
				{
					_mesh.name,
					_vertices.Count,
					_tangents.Count,
					this.meshIndex,
					this.CurTriangleIndex
				});
			}
			else
			{
				MeshUnsafeCopyHelper.CopyTangents(_tangents, _mesh);
			}
		}
		_mesh.RecalculateUVDistributionMetrics(1E-09f);
		GameUtils.SetMeshVertexAttributes(_mesh, false);
		_mesh.UploadMeshData(false);
		if (_onCopyComplete != null)
		{
			_onCopyComplete();
		}
	}

	// Token: 0x06005125 RID: 20773 RVA: 0x00207610 File Offset: 0x00205810
	public virtual int CopyToColliders(int _clrIdx, MeshCollider _meshCollider, out Mesh mesh)
	{
		if (this.m_CollIndices.Count == 0)
		{
			GameManager.Instance.World.m_ChunkManager.BakeDestroyCancel(_meshCollider);
			mesh = null;
			return 0;
		}
		mesh = this.ResetMesh(_meshCollider);
		MeshUnsafeCopyHelper.CopyVertices(this.m_CollVertices, mesh);
		MeshUnsafeCopyHelper.CopyTriangles(this.m_CollIndices, mesh);
		return this.m_CollIndices.Count / 3;
	}

	// Token: 0x06005126 RID: 20774 RVA: 0x00207674 File Offset: 0x00205874
	public Mesh ResetMesh(MeshCollider _meshCollider)
	{
		Mesh mesh = GameManager.Instance.World.m_ChunkManager.BakeCancelAndGetMesh(_meshCollider);
		if (!mesh)
		{
			mesh = new Mesh();
		}
		else
		{
			mesh.Clear(false);
		}
		return mesh;
	}

	// Token: 0x06005127 RID: 20775 RVA: 0x002076AF File Offset: 0x002058AF
	public virtual void GetColorForTextureId(int _subMeshIdx, ref Transvoxel.BuildVertex _data)
	{
		_data.color = Color.black;
		_data.uv = Vector2.zero;
		_data.uv2 = Vector2.zero;
		_data.uv3 = Vector2.zero;
		_data.uv4 = Vector2.zero;
	}

	// Token: 0x06005128 RID: 20776 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual int FindOrCreateSubMesh(int _t0, int _t1, int _t2)
	{
		return 0;
	}

	// Token: 0x06005129 RID: 20777 RVA: 0x002076E8 File Offset: 0x002058E8
	public virtual void AddIndices(int _i0, int _i1, int _i2, int _submesh)
	{
		this.m_Indices.Add(_i0);
		this.m_Indices.Add(_i1);
		this.m_Indices.Add(_i2);
	}

	// Token: 0x0600512A RID: 20778 RVA: 0x00207710 File Offset: 0x00205910
	public virtual void ClearMesh()
	{
		this.CurTriangleIndex = 0;
		this.m_Vertices.Clear();
		this.m_Indices.Clear();
		this.m_Uvs.Clear();
		if (this.UvsCrack != null)
		{
			this.UvsCrack.Clear();
		}
		if (this.m_Uvs3 != null)
		{
			this.m_Uvs3.Clear();
		}
		if (this.m_Uvs4 != null)
		{
			this.m_Uvs4.Clear();
		}
		this.m_ColorVertices.Clear();
		this.m_Normals.Clear();
		this.m_Tangents.Clear();
		if (this.m_CollVertices != null)
		{
			this.m_CollVertices.Clear();
			this.m_CollIndices.Clear();
		}
	}

	// Token: 0x0600512B RID: 20779 RVA: 0x002077C0 File Offset: 0x002059C0
	public virtual void SizeToChunkDefaults(int _meshIndex)
	{
		int newSize;
		int newSize2;
		int newSize3;
		VoxelMesh.GetDefaultSizes(_meshIndex, out newSize, out newSize2, out newSize3);
		this.m_Vertices.Grow(newSize);
		this.m_Indices.Grow(newSize2);
		this.m_Uvs.Grow(newSize);
		if (this.UvsCrack != null)
		{
			this.UvsCrack.Grow(newSize);
		}
		if (this.m_Uvs3 != null)
		{
			this.m_Uvs3.Grow(newSize);
			this.m_Uvs4.Grow(newSize);
		}
		this.m_ColorVertices.Grow(newSize);
		this.m_Normals.Grow(newSize);
		this.m_Tangents.Grow(newSize);
		if (this.m_CollVertices != null)
		{
			this.m_CollVertices.Grow(newSize3);
			this.m_CollIndices.Grow(newSize3);
		}
	}

	// Token: 0x0600512C RID: 20780 RVA: 0x00207878 File Offset: 0x00205A78
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GetDefaultSizes(int _meshIndex, out int _vertMax, out int _indexMax, out int _colliderMax)
	{
		switch (_meshIndex)
		{
		case 0:
			_vertMax = 65536;
			_indexMax = 131072;
			_colliderMax = 32768;
			return;
		case 1:
			_vertMax = 16384;
			_indexMax = 32768;
			_colliderMax = 32768;
			return;
		case 2:
			_vertMax = 4096;
			_indexMax = 8192;
			_colliderMax = 4096;
			return;
		case 3:
			_vertMax = 16384;
			_indexMax = 32768;
			_colliderMax = 8192;
			return;
		case 4:
			_vertMax = 128;
			_indexMax = 128;
			_colliderMax = 0;
			return;
		case 5:
			_vertMax = 1024;
			_indexMax = 1024;
			_colliderMax = 1024;
			return;
		default:
			_vertMax = 512;
			_indexMax = 1024;
			_colliderMax = 512;
			return;
		}
	}

	// Token: 0x0600512D RID: 20781 RVA: 0x0020793D File Offset: 0x00205B3D
	public virtual void Finished()
	{
		this.m_Triangles = this.m_Indices.Count / 3;
	}

	// Token: 0x0600512E RID: 20782 RVA: 0x00207954 File Offset: 0x00205B54
	public virtual Color[] UpdateColors(byte _suncolor, byte _blockcolor)
	{
		for (int i = 0; i < this.m_ColorVertices.Count; i++)
		{
			this.m_ColorVertices[i] = Lighting.ToColor((int)_suncolor, (int)_blockcolor, 1f);
		}
		return this.m_ColorVertices.ToArray();
	}

	// Token: 0x0600512F RID: 20783 RVA: 0x0020799C File Offset: 0x00205B9C
	public void AddBlockSide(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, BlockValue _blockValue, BlockFace blockFace, Lighting _lighting, int meshIndex)
	{
		Color color = _lighting.ToColor();
		this.AddQuadWithCracks(v1, color, v2, color, v3, color, v4, color, _blockValue.Block.getUVRectFromSideAndMetadata(meshIndex, blockFace, v1, _blockValue), WorldConstants.MapDamageToUVRect(_blockValue), false);
	}

	// Token: 0x06005130 RID: 20784 RVA: 0x002079DC File Offset: 0x00205BDC
	public void AddBlockSide(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, BlockValue _blockValue, float colorBlockFace, BlockFace blockFace, byte sunlight, byte blocklight, int meshIndex)
	{
		Color color = Lighting.ToColor((int)sunlight, (int)blocklight, colorBlockFace);
		this.AddQuadWithCracks(v1, color, v2, color, v3, color, v4, color, _blockValue.Block.getUVRectFromSideAndMetadata(meshIndex, blockFace, v1, _blockValue), WorldConstants.MapDamageToUVRect(_blockValue), false);
	}

	// Token: 0x06005131 RID: 20785 RVA: 0x00207A20 File Offset: 0x00205C20
	public void AddBlockSideTri(Vector3 v1, Vector3 v2, Vector3 v3, int _meshIdx, BlockValue _blockValue, float colorBlockFace, BlockFace blockFace, byte sunlight, byte blocklight)
	{
		Color color = Lighting.ToColor((int)sunlight, (int)blocklight, colorBlockFace);
		this.AddTriWithCracks(v1, color, v2, color, v3, color, WorldConstants.MapBlockToUVRect(_meshIdx, _blockValue, blockFace), WorldConstants.MapDamageToUVRect(_blockValue), false);
	}

	// Token: 0x06005132 RID: 20786 RVA: 0x00207A58 File Offset: 0x00205C58
	public void AddBlockSideTri(Vector3 v1, Color c1, Vector3 v2, Color c2, Vector3 v3, Color c3, Rect uv1, Rect uv2)
	{
		this.AddTriWithCracks(v1, c1, v2, c2, v3, c3, uv1, uv2, false);
	}

	// Token: 0x06005133 RID: 20787 RVA: 0x00207A7C File Offset: 0x00205C7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AddQuad(Vector3 _v0, Vector2 _uv0, Vector3 _v1, Vector2 _uv1, Vector3 _v2, Vector2 _uv3, Vector3 _v3, Vector2 _uv4, byte sunlight, byte blocklight, float sideColor)
	{
		if (this.m_Vertices.Count > 786428)
		{
			return;
		}
		this.m_Vertices.Add(_v0);
		this.m_Vertices.Add(_v1);
		this.m_Vertices.Add(_v2);
		this.m_Vertices.Add(_v3);
		int count = this.m_CollVertices.Count;
		this.m_CollVertices.Add(_v0);
		this.m_CollVertices.Add(_v1);
		this.m_CollVertices.Add(_v2);
		this.m_CollVertices.Add(_v3);
		this.m_Normals.Add(Vector3.Cross(_v3 - _v0, _v1 - _v0).normalized);
		this.m_Normals.Add(Vector3.Cross(_v0 - _v1, _v2 - _v1).normalized);
		this.m_Normals.Add(Vector3.Cross(_v1 - _v2, _v3 - _v2).normalized);
		this.m_Normals.Add(Vector3.Cross(_v2 - _v3, _v0 - _v3).normalized);
		Color item = Lighting.ToColor((int)sunlight, (int)blocklight, sideColor);
		item.a = this.temperature;
		this.m_ColorVertices.Add(item);
		this.m_ColorVertices.Add(item);
		this.m_ColorVertices.Add(item);
		this.m_ColorVertices.Add(item);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex + 1);
		this.m_Indices.Add(this.CurTriangleIndex + 3);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_CollIndices.Add(count);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count + 1);
		this.m_CollIndices.Add(count + 3);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count);
		this.m_Uvs.Add(_uv0);
		this.m_Uvs.Add(_uv1);
		this.m_Uvs.Add(_uv3);
		this.m_Uvs.Add(_uv4);
		this.UvsCrack.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.CurTriangleIndex += 4;
	}

	// Token: 0x06005134 RID: 20788 RVA: 0x00207D34 File Offset: 0x00205F34
	public void CreateFromQuadList(Vector3[] _vertices, Color _color)
	{
		this.m_Vertices.Add(_vertices[0]);
		this.m_Vertices.Add(_vertices[0] + new Vector3(1f, 0f, 0f));
		this.m_Vertices.Add(_vertices[0] + new Vector3(1f, 0f, 1f));
		this.m_Vertices.Add(_vertices[0] + new Vector3(0f, 0f, 1f));
		this.m_Vertices.Add(_vertices[0] + new Vector3(0.5f, 0.15f, 0.5f));
		this.m_CollVertices.Add(_vertices[0]);
		this.m_CollVertices.Add(_vertices[0] + new Vector3(1f, 0f, 0f));
		this.m_CollVertices.Add(_vertices[0] + new Vector3(1f, 0f, 1f));
		this.m_CollVertices.Add(_vertices[0] + new Vector3(0f, 0f, 1f));
		this.m_CollVertices.Add(_vertices[0] + new Vector3(0.5f, 0.15f, 0.5f));
		this.m_ColorVertices.Add(_color);
		this.m_ColorVertices.Add(_color);
		this.m_ColorVertices.Add(_color);
		this.m_ColorVertices.Add(_color);
		this.m_ColorVertices.Add(_color);
		this.m_Normals.Add(new Vector3(0f, 1f, 0f));
		this.m_Normals.Add(new Vector3(0f, 1f, 0f));
		this.m_Normals.Add(new Vector3(0f, 1f, 0f));
		this.m_Normals.Add(new Vector3(0f, 1f, 0f));
		this.m_Normals.Add(new Vector3(0f, 1f, 0f));
		this.Uvs.Add(Vector2.zero);
		this.Uvs.Add(Vector2.zero);
		this.Uvs.Add(Vector2.zero);
		this.Uvs.Add(Vector2.zero);
		this.Uvs.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.UvsCrack.Add(Vector2.zero);
		this.m_Indices.Add(this.CurTriangleIndex + 4);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_Indices.Add(this.CurTriangleIndex + 3);
		this.m_CollIndices.Add(this.CurTriangleIndex + 4);
		this.m_CollIndices.Add(this.CurTriangleIndex);
		this.m_CollIndices.Add(this.CurTriangleIndex + 3);
		this.m_Indices.Add(this.CurTriangleIndex + 4);
		this.m_Indices.Add(this.CurTriangleIndex + 1);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_CollIndices.Add(this.CurTriangleIndex + 4);
		this.m_CollIndices.Add(this.CurTriangleIndex + 1);
		this.m_CollIndices.Add(this.CurTriangleIndex);
		this.m_Indices.Add(this.CurTriangleIndex + 4);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex + 1);
		this.m_CollIndices.Add(this.CurTriangleIndex + 4);
		this.m_CollIndices.Add(this.CurTriangleIndex + 2);
		this.m_CollIndices.Add(this.CurTriangleIndex + 1);
		this.m_Indices.Add(this.CurTriangleIndex + 4);
		this.m_Indices.Add(this.CurTriangleIndex + 3);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_CollIndices.Add(this.CurTriangleIndex + 4);
		this.m_CollIndices.Add(this.CurTriangleIndex + 3);
		this.m_CollIndices.Add(this.CurTriangleIndex + 2);
		this.CurTriangleIndex += 5;
	}

	// Token: 0x06005135 RID: 20789 RVA: 0x00208204 File Offset: 0x00206404
	public void AddBasicQuad(Vector3[] _vertices, Color _color, Vector2 _UVdata, bool bForceNormalsUp = false, bool bAlternateWinding = false)
	{
		if (this.m_Vertices.Count > 786428)
		{
			return;
		}
		if (_vertices.Length != 4)
		{
			return;
		}
		Vector3 vector = _vertices[0];
		Vector3 vector2 = _vertices[1];
		Vector3 vector3 = _vertices[2];
		Vector3 vector4 = _vertices[3];
		int num = this.m_Vertices.Alloc(4);
		this.m_Vertices[num] = vector;
		this.m_Vertices[num + 1] = vector2;
		this.m_Vertices[num + 2] = vector3;
		this.m_Vertices[num + 3] = vector4;
		int num2 = this.m_CollVertices.Alloc(4);
		this.m_CollVertices[num] = vector;
		this.m_CollVertices[num + 1] = vector2;
		this.m_CollVertices[num + 2] = vector3;
		this.m_CollVertices[num + 3] = vector4;
		num = this.m_Normals.Alloc(4);
		if (bForceNormalsUp)
		{
			Vector3 up = Vector3.up;
			this.m_Normals[num] = up;
			this.m_Normals[num + 1] = up;
			this.m_Normals[num + 2] = up;
			this.m_Normals[num + 3] = up;
		}
		else
		{
			this.m_Normals[num] = Vector3.Cross(vector4 - vector, vector2 - vector).normalized;
			this.m_Normals[num + 1] = Vector3.Cross(vector - vector2, vector3 - vector2).normalized;
			this.m_Normals[num + 2] = Vector3.Cross(vector2 - vector3, vector4 - vector3).normalized;
			this.m_Normals[num + 3] = Vector3.Cross(vector3 - vector4, vector - vector4).normalized;
		}
		num = this.m_ColorVertices.Alloc(4);
		this.m_ColorVertices[num] = _color;
		this.m_ColorVertices[num + 1] = _color;
		this.m_ColorVertices[num + 2] = _color;
		this.m_ColorVertices[num + 3] = _color;
		num = this.m_Indices.Alloc(6);
		this.m_Indices[num] = (bAlternateWinding ? (this.CurTriangleIndex + 3) : this.CurTriangleIndex);
		this.m_Indices[num + 1] = this.CurTriangleIndex + 2;
		this.m_Indices[num + 2] = this.CurTriangleIndex + 1;
		this.m_Indices[num + 3] = this.CurTriangleIndex + 3;
		this.m_Indices[num + 4] = (bAlternateWinding ? (this.CurTriangleIndex + 1) : (this.CurTriangleIndex + 2));
		this.m_Indices[num + 5] = this.CurTriangleIndex;
		num = this.m_CollIndices.Alloc(6);
		this.m_CollIndices[num] = num2;
		this.m_CollIndices[num + 1] = num2 + 2;
		this.m_CollIndices[num + 2] = num2 + 1;
		this.m_CollIndices[num + 3] = num2 + 3;
		this.m_CollIndices[num + 4] = num2 + 2;
		this.m_CollIndices[num + 5] = num2;
		num = this.m_Uvs.Alloc(4);
		this.m_Uvs.Items[num].x = 0f;
		this.m_Uvs.Items[num].y = 1f;
		this.m_Uvs.Items[++num].x = 1f;
		this.m_Uvs.Items[num].y = 1f;
		this.m_Uvs.Items[++num].x = 1f;
		this.m_Uvs.Items[num].y = 0f;
		this.m_Uvs.Items[++num].x = 0f;
		this.m_Uvs.Items[num].y = 0f;
		num = this.UvsCrack.Alloc(4);
		this.UvsCrack[num] = _UVdata;
		this.UvsCrack[num + 1] = _UVdata;
		this.UvsCrack[num + 2] = _UVdata;
		this.UvsCrack[num + 3] = _UVdata;
		this.CurTriangleIndex += 4;
	}

	// Token: 0x06005136 RID: 20790 RVA: 0x002086B8 File Offset: 0x002068B8
	public void AddQuadNoCollision(Vector3 _v0, Vector3 _v1, Vector3 _v2, Vector3 _v3, Color _color, Rect _uvTex)
	{
		if (this.m_Vertices.Count > 786428)
		{
			return;
		}
		int num = this.m_Vertices.Alloc(4);
		this.m_Vertices[num] = _v0;
		this.m_Vertices[num + 1] = _v1;
		this.m_Vertices[num + 2] = _v2;
		this.m_Vertices[num + 3] = _v3;
		num = this.m_Normals.Alloc(4);
		this.m_Normals[num] = Vector3.Cross(_v3 - _v0, _v1 - _v0).normalized;
		this.m_Normals[num + 1] = Vector3.Cross(_v0 - _v1, _v2 - _v1).normalized;
		this.m_Normals[num + 2] = Vector3.Cross(_v1 - _v2, _v3 - _v2).normalized;
		this.m_Normals[num + 3] = Vector3.Cross(_v2 - _v3, _v0 - _v3).normalized;
		_color.a = this.temperature;
		num = this.m_ColorVertices.Alloc(4);
		this.m_ColorVertices[num] = _color;
		this.m_ColorVertices[num + 1] = _color;
		this.m_ColorVertices[num + 2] = _color;
		this.m_ColorVertices[num + 3] = _color;
		num = this.m_Indices.Alloc(6);
		this.m_Indices[num] = this.CurTriangleIndex;
		this.m_Indices[num + 1] = this.CurTriangleIndex + 2;
		this.m_Indices[num + 2] = this.CurTriangleIndex + 1;
		this.m_Indices[num + 3] = this.CurTriangleIndex + 3;
		this.m_Indices[num + 4] = this.CurTriangleIndex + 2;
		this.m_Indices[num + 5] = this.CurTriangleIndex;
		num = this.m_Uvs.Alloc(4);
		float x = _uvTex.x;
		float y = _uvTex.y;
		float width = _uvTex.width;
		float height = _uvTex.height;
		this.m_Uvs.Items[num].x = x;
		this.m_Uvs.Items[num].y = y;
		this.m_Uvs.Items[++num].x = x + width;
		this.m_Uvs.Items[num].y = y;
		this.m_Uvs.Items[++num].x = x + width;
		this.m_Uvs.Items[num].y = y + height;
		this.m_Uvs.Items[++num].x = x;
		this.m_Uvs.Items[num].y = y + height;
		this.CurTriangleIndex += 4;
	}

	// Token: 0x06005137 RID: 20791 RVA: 0x002089C4 File Offset: 0x00206BC4
	public void AddQuadWithCracks(Vector3 _v0, Color _c0, Vector3 _v1, Color _c1, Vector3 _v2, Color _c2, Vector3 _v3, Color _c3, Rect _uvTex, Rect _uvOverlay, bool bSwitchUvHorizontal)
	{
		if (this.m_Vertices.Count > 786428)
		{
			return;
		}
		int num = this.m_Vertices.Alloc(4);
		this.m_Vertices[num] = _v0;
		this.m_Vertices[num + 1] = _v1;
		this.m_Vertices[num + 2] = _v2;
		this.m_Vertices[num + 3] = _v3;
		int count = this.m_CollVertices.Count;
		this.m_CollVertices.Add(_v0);
		this.m_CollVertices.Add(_v1);
		this.m_CollVertices.Add(_v2);
		this.m_CollVertices.Add(_v3);
		num = this.m_Normals.Alloc(4);
		this.m_Normals[num] = Vector3.Cross(_v3 - _v0, _v1 - _v0).normalized;
		this.m_Normals[num + 1] = Vector3.Cross(_v0 - _v1, _v2 - _v1).normalized;
		this.m_Normals[num + 2] = Vector3.Cross(_v1 - _v2, _v3 - _v2).normalized;
		this.m_Normals[num + 3] = Vector3.Cross(_v2 - _v3, _v0 - _v3).normalized;
		float a = this.temperature;
		_c0.a = a;
		_c1.a = a;
		_c2.a = a;
		_c3.a = a;
		num = this.m_ColorVertices.Alloc(4);
		this.m_ColorVertices[num] = _c0;
		this.m_ColorVertices[num + 1] = _c1;
		this.m_ColorVertices[num + 2] = _c2;
		this.m_ColorVertices[num + 3] = _c3;
		num = this.m_Indices.Alloc(6);
		this.m_Indices[num] = this.CurTriangleIndex;
		this.m_Indices[num + 1] = this.CurTriangleIndex + 2;
		this.m_Indices[num + 2] = this.CurTriangleIndex + 1;
		this.m_Indices[num + 3] = this.CurTriangleIndex + 3;
		this.m_Indices[num + 4] = this.CurTriangleIndex + 2;
		this.m_Indices[num + 5] = this.CurTriangleIndex;
		this.m_CollIndices.Add(count);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count + 1);
		this.m_CollIndices.Add(count + 3);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count);
		num = this.m_Uvs.Alloc(4);
		float x = _uvTex.x;
		float y = _uvTex.y;
		float width = _uvTex.width;
		float height = _uvTex.height;
		if (!bSwitchUvHorizontal)
		{
			this.m_Uvs.Items[num].x = x;
			this.m_Uvs.Items[num].y = y;
			this.m_Uvs.Items[++num].x = x + width;
			this.m_Uvs.Items[num].y = y;
			this.m_Uvs.Items[++num].x = x + width;
			this.m_Uvs.Items[num].y = y + height;
			this.m_Uvs.Items[++num].x = x;
			this.m_Uvs.Items[num].y = y + height;
		}
		else
		{
			this.m_Uvs.Items[num].x = x + width;
			this.m_Uvs.Items[num].y = y;
			this.m_Uvs.Items[++num].x = x;
			this.m_Uvs.Items[num].y = y;
			this.m_Uvs.Items[++num].x = x;
			this.m_Uvs.Items[num].y = y + height;
			this.m_Uvs.Items[++num].x = x + width;
			this.m_Uvs.Items[num].y = y + height;
		}
		num = this.UvsCrack.Alloc(4);
		x = _uvOverlay.x;
		y = _uvOverlay.y;
		width = _uvOverlay.width;
		height = _uvOverlay.height;
		this.UvsCrack.Items[num].x = x;
		this.UvsCrack.Items[num].y = y;
		this.UvsCrack.Items[++num].x = x;
		this.UvsCrack.Items[num].y = y + height;
		this.UvsCrack.Items[++num].x = x + width;
		this.UvsCrack.Items[num].y = y + height;
		this.UvsCrack.Items[++num].x = x + width;
		this.UvsCrack.Items[num].y = y;
		this.CurTriangleIndex += 4;
	}

	// Token: 0x06005138 RID: 20792 RVA: 0x00208F68 File Offset: 0x00207168
	public void AddRectangle(Vector3 _v0, Vector2 _uv0, Color _c0, Vector3 _v1, Vector2 _uv1, Color _c1, Vector3 _v2, Vector2 _uv2, Color _c2, Vector3 _v3, Vector2 _uv3, Color _c3, Rect _uvTex, Rect _uvOverlay)
	{
		if (this.m_Vertices.Count > 786428)
		{
			return;
		}
		this.m_Vertices.Add(_v0);
		this.m_Vertices.Add(_v1);
		this.m_Vertices.Add(_v2);
		this.m_Vertices.Add(_v3);
		int count = this.m_CollVertices.Count;
		this.m_CollVertices.Add(_v0);
		this.m_CollVertices.Add(_v1);
		this.m_CollVertices.Add(_v2);
		this.m_CollVertices.Add(_v3);
		this.m_Normals.Add(Vector3.Cross(_v3 - _v0, _v1 - _v0).normalized);
		this.m_Normals.Add(Vector3.Cross(_v0 - _v1, _v2 - _v1).normalized);
		this.m_Normals.Add(Vector3.Cross(_v1 - _v2, _v3 - _v2).normalized);
		this.m_Normals.Add(Vector3.Cross(_v2 - _v3, _v0 - _v3).normalized);
		float a = this.temperature;
		_c0.a = a;
		_c1.a = a;
		_c2.a = a;
		_c3.a = a;
		this.m_ColorVertices.Add(_c0);
		this.m_ColorVertices.Add(_c1);
		this.m_ColorVertices.Add(_c2);
		this.m_ColorVertices.Add(_c3);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex + 1);
		this.m_Indices.Add(this.CurTriangleIndex + 3);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_CollIndices.Add(count);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count + 1);
		this.m_CollIndices.Add(count + 3);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count);
		this.m_Uvs.Add(new Vector2(_uvTex.x + _uv0.x * _uvTex.width, _uvTex.y + _uv0.y * _uvTex.height));
		this.m_Uvs.Add(new Vector2(_uvTex.x + _uv1.x * _uvTex.width, _uvTex.y + _uv1.y * _uvTex.height));
		this.m_Uvs.Add(new Vector2(_uvTex.x + _uv2.x * _uvTex.width, _uvTex.y + _uv2.y * _uvTex.height));
		this.m_Uvs.Add(new Vector2(_uvTex.x + _uv3.x * _uvTex.width, _uvTex.y + _uv3.y * _uvTex.height));
		this.UvsCrack.Add(new Vector2(_uvOverlay.x + _uv0.x * _uvOverlay.width, _uvOverlay.y + _uv0.y * _uvOverlay.height));
		this.UvsCrack.Add(new Vector2(_uvOverlay.x + _uv1.x * _uvOverlay.width, _uvOverlay.y + _uv1.y * _uvOverlay.height));
		this.UvsCrack.Add(new Vector2(_uvOverlay.x + _uv2.x * _uvOverlay.width, _uvOverlay.y + _uv2.y * _uvOverlay.height));
		this.UvsCrack.Add(new Vector2(_uvOverlay.x + _uv3.x * _uvOverlay.width, _uvOverlay.y + _uv3.y * _uvOverlay.height));
		this.CurTriangleIndex += 4;
	}

	// Token: 0x06005139 RID: 20793 RVA: 0x002093B0 File Offset: 0x002075B0
	public void AddRectangle(Vector3 _v0, Vector2 _uv0, Vector3 _v1, Vector2 _uv1, Vector3 _v2, Vector2 _uv2, Vector3 _v3, Vector2 _uv3, Vector3 _normal, Vector3 _normalTop, Vector4 _tangent, Rect _uvTex, byte _sunlight, byte _blocklight)
	{
		if (this.m_Vertices.Count > 786428)
		{
			return;
		}
		int num = this.m_Vertices.Alloc(4);
		this.m_Vertices[num] = _v0;
		this.m_Vertices[num + 1] = _v1;
		this.m_Vertices[num + 2] = _v2;
		this.m_Vertices[num + 3] = _v3;
		num = this.m_Normals.Alloc(4);
		this.m_Normals[num] = _normal;
		this.m_Normals[num + 1] = _normal;
		this.m_Normals[num + 2] = _normalTop;
		this.m_Normals[num + 3] = _normalTop;
		num = this.m_Tangents.Alloc(4);
		this.m_Tangents[num] = _tangent;
		this.m_Tangents[num + 1] = _tangent;
		this.m_Tangents[num + 2] = _tangent;
		this.m_Tangents[num + 3] = _tangent;
		Color value = Lighting.ToColor((int)_sunlight, (int)_blocklight);
		value.a = this.temperature;
		num = this.m_ColorVertices.Alloc(4);
		this.m_ColorVertices[num] = value;
		this.m_ColorVertices[num + 1] = value;
		this.m_ColorVertices[num + 2] = value;
		this.m_ColorVertices[num + 3] = value;
		num = this.m_Indices.Alloc(6);
		this.m_Indices[num] = this.CurTriangleIndex;
		this.m_Indices[num + 1] = this.CurTriangleIndex + 2;
		this.m_Indices[num + 2] = this.CurTriangleIndex + 1;
		this.m_Indices[num + 3] = this.CurTriangleIndex + 3;
		this.m_Indices[num + 4] = this.CurTriangleIndex + 2;
		this.m_Indices[num + 5] = this.CurTriangleIndex;
		num = this.m_Uvs.Alloc(4);
		float x = _uvTex.x;
		float y = _uvTex.y;
		float width = _uvTex.width;
		float height = _uvTex.height;
		this.m_Uvs.Items[num].x = x + _uv0.x * width;
		this.m_Uvs.Items[num].y = y + _uv0.y * height;
		this.m_Uvs.Items[++num].x = x + _uv1.x * width;
		this.m_Uvs.Items[num].y = y + _uv1.y * height;
		this.m_Uvs.Items[++num].x = x + _uv2.x * width;
		this.m_Uvs.Items[num].y = y + _uv2.y * height;
		this.m_Uvs.Items[++num].x = x + _uv3.x * width;
		this.m_Uvs.Items[num].y = y + _uv3.y * height;
		num = this.UvsCrack.Alloc(4);
		this.UvsCrack[num] = Vector2.zero;
		this.UvsCrack[num + 1] = Vector2.zero;
		this.UvsCrack[num + 2] = Vector2.zero;
		this.UvsCrack[num + 3] = Vector2.zero;
		this.CurTriangleIndex += 4;
	}

	// Token: 0x0600513A RID: 20794 RVA: 0x00209748 File Offset: 0x00207948
	public void AddRectangleColliderPair(Vector3 _v0, Vector3 _v1, Vector3 _v2, Vector3 _v3)
	{
		if (this.m_CollVertices.Count > 786428)
		{
			return;
		}
		int num = this.m_CollVertices.Alloc(4);
		this.m_CollVertices[num] = _v0;
		this.m_CollVertices[num + 1] = _v1;
		this.m_CollVertices[num + 2] = _v2;
		this.m_CollVertices[num + 3] = _v3;
		int num2 = this.m_CollIndices.Alloc(12);
		this.m_CollIndices[num2] = num;
		this.m_CollIndices[num2 + 1] = num + 1;
		this.m_CollIndices[num2 + 2] = num + 2;
		this.m_CollIndices[num2 + 3] = num;
		this.m_CollIndices[num2 + 4] = num + 2;
		this.m_CollIndices[num2 + 5] = num + 3;
		this.m_CollIndices[num2 + 6] = num;
		this.m_CollIndices[num2 + 7] = num + 2;
		this.m_CollIndices[num2 + 8] = num + 1;
		this.m_CollIndices[num2 + 9] = num + 3;
		this.m_CollIndices[num2 + 10] = num + 2;
		this.m_CollIndices[num2 + 11] = num;
	}

	// Token: 0x0600513B RID: 20795 RVA: 0x00209884 File Offset: 0x00207A84
	public void AddColoredRectangle(Vector3 _v0, Color _c0, Vector3 _v1, Color _c1, Vector3 _v2, Color _c2, Vector3 _v3, Color _c3)
	{
		if (this.m_Vertices.Count > 786428)
		{
			return;
		}
		this.m_Vertices.Add(_v0);
		this.m_Vertices.Add(_v1);
		this.m_Vertices.Add(_v2);
		this.m_Vertices.Add(_v3);
		int count = this.m_CollVertices.Count;
		this.m_CollVertices.Add(_v0);
		this.m_CollVertices.Add(_v1);
		this.m_CollVertices.Add(_v2);
		this.m_CollVertices.Add(_v3);
		this.m_Normals.Add(Vector3.Cross(_v3 - _v0, _v1 - _v0).normalized);
		this.m_Normals.Add(Vector3.Cross(_v0 - _v1, _v2 - _v1).normalized);
		this.m_Normals.Add(Vector3.Cross(_v1 - _v2, _v3 - _v2).normalized);
		this.m_Normals.Add(Vector3.Cross(_v2 - _v3, _v0 - _v3).normalized);
		float a = this.temperature;
		_c0.a = a;
		_c1.a = a;
		_c2.a = a;
		_c3.a = a;
		this.m_ColorVertices.Add(_c0);
		this.m_ColorVertices.Add(_c1);
		this.m_ColorVertices.Add(_c2);
		this.m_ColorVertices.Add(_c3);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex + 1);
		this.m_Indices.Add(this.CurTriangleIndex + 3);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_CollIndices.Add(count);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count + 1);
		this.m_CollIndices.Add(count + 3);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count);
		this.CurTriangleIndex += 4;
	}

	// Token: 0x0600513C RID: 20796 RVA: 0x00209ADC File Offset: 0x00207CDC
	public void AddTriWithCracks(Vector3 _v0, Color _c0, Vector3 _v1, Color _c1, Vector3 _v2, Color _c2, Rect uvTex, Rect uvOverlay, bool bSwitchUvHorizontal)
	{
		if (this.m_Vertices.Count > 786429)
		{
			return;
		}
		this.m_Vertices.Add(_v0);
		this.m_Vertices.Add(_v1);
		this.m_Vertices.Add(_v2);
		int count = this.m_CollVertices.Count;
		this.m_CollVertices.Add(_v0);
		this.m_CollVertices.Add(_v1);
		this.m_CollVertices.Add(_v2);
		this.m_Normals.Add(Vector3.Cross(_v2 - _v0, _v1 - _v0).normalized);
		this.m_Normals.Add(Vector3.Cross(_v0 - _v1, _v2 - _v1).normalized);
		this.m_Normals.Add(Vector3.Cross(_v1 - _v2, _v0 - _v2).normalized);
		float a = this.temperature;
		_c0.a = a;
		_c1.a = a;
		_c2.a = a;
		this.ColorVertices.Add(_c0);
		this.ColorVertices.Add(_c1);
		this.ColorVertices.Add(_c2);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex + 1);
		this.m_CollIndices.Add(count);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count + 1);
		this.Uvs.Add(new Vector2(uvTex.x + 0f, uvTex.y + 0f));
		if (!bSwitchUvHorizontal)
		{
			this.Uvs.Add(new Vector2(uvTex.x + 0f, uvTex.y + uvTex.height - 0f));
		}
		else
		{
			this.Uvs.Add(new Vector2(uvTex.x + uvTex.width - 0f, uvTex.y + 0f));
		}
		this.Uvs.Add(new Vector2(uvTex.x + uvTex.width - 0f, uvTex.y + uvTex.height - 0f));
		this.UvsCrack.Add(new Vector2(uvOverlay.x + 0f, uvOverlay.y + 0f));
		if (!bSwitchUvHorizontal)
		{
			this.UvsCrack.Add(new Vector2(uvOverlay.x + 0f, uvOverlay.y + uvOverlay.height - 0f));
		}
		else
		{
			this.UvsCrack.Add(new Vector2(uvOverlay.x + uvOverlay.width - 0f, uvOverlay.y + 0f));
		}
		this.UvsCrack.Add(new Vector2(uvOverlay.x + uvOverlay.width - 0f, uvOverlay.y + uvOverlay.height - 0f));
		this.CurTriangleIndex += 3;
	}

	// Token: 0x0600513D RID: 20797 RVA: 0x00209E18 File Offset: 0x00208018
	public void AddTriangle(Vector3 _v0, Vector2 _uv0, Color _c0, Vector3 _v1, Vector2 _uv1, Color _c1, Vector3 _v2, Vector2 _uv2, Color _c2, Rect uvTex, Rect uvOverlay)
	{
		if (this.m_Vertices.Count > 786429)
		{
			return;
		}
		this.m_Vertices.Add(_v0);
		this.m_Vertices.Add(_v1);
		this.m_Vertices.Add(_v2);
		int count = this.m_CollVertices.Count;
		this.m_CollVertices.Add(_v0);
		this.m_CollVertices.Add(_v1);
		this.m_CollVertices.Add(_v2);
		this.m_Normals.Add(Vector3.Cross(_v2 - _v0, _v1 - _v0).normalized);
		this.m_Normals.Add(Vector3.Cross(_v0 - _v1, _v2 - _v1).normalized);
		this.m_Normals.Add(Vector3.Cross(_v1 - _v2, _v0 - _v2).normalized);
		float a = this.temperature;
		_c0.a = a;
		_c1.a = a;
		_c2.a = a;
		this.ColorVertices.Add(_c0);
		this.ColorVertices.Add(_c1);
		this.ColorVertices.Add(_c2);
		this.m_Indices.Add(this.CurTriangleIndex);
		this.m_Indices.Add(this.CurTriangleIndex + 2);
		this.m_Indices.Add(this.CurTriangleIndex + 1);
		this.m_CollIndices.Add(count);
		this.m_CollIndices.Add(count + 2);
		this.m_CollIndices.Add(count + 1);
		this.Uvs.Add(new Vector2(uvTex.x + _uv0.x * uvTex.width, uvTex.y + _uv0.y * uvTex.height));
		this.Uvs.Add(new Vector2(uvTex.x + _uv1.x * uvTex.width, uvTex.y + _uv1.y * uvTex.height));
		this.Uvs.Add(new Vector2(uvTex.x + _uv2.x * uvTex.width, uvTex.y + _uv2.y * uvTex.height));
		this.UvsCrack.Add(new Vector2(uvOverlay.x + _uv0.x * uvOverlay.width, uvOverlay.y + _uv0.y * uvOverlay.height));
		this.UvsCrack.Add(new Vector2(uvOverlay.x + _uv1.x * uvOverlay.width, uvOverlay.y + _uv1.y * uvOverlay.height));
		this.UvsCrack.Add(new Vector2(uvOverlay.x + _uv2.x * uvOverlay.width, uvOverlay.y + _uv2.y * uvOverlay.height));
		this.CurTriangleIndex += 3;
	}

	// Token: 0x0600513E RID: 20798 RVA: 0x0020A12C File Offset: 0x0020832C
	public virtual void AddMesh(Vector3 _drawPos, int _count, Vector3[] _vertices, Vector3[] _normals, ArrayListMP<int> _indices, ArrayListMP<Vector2> _uvs, byte _sunlight, byte _blocklight, VoxelMesh _specialColliders, int damage)
	{
		if (_count + this.m_Vertices.Count > 786432)
		{
			return;
		}
		int curTriangleIndex = this.CurTriangleIndex;
		this.CurTriangleIndex += _count;
		this.m_Vertices.AddRange(_vertices, 0, _count);
		this.m_Normals.AddRange(_normals, 0, _count);
		Color value = new Color((float)_sunlight / 15f, 0f, 0f, this.temperature);
		int num = this.m_ColorVertices.Alloc(_count);
		for (int i = 0; i < _count; i++)
		{
			this.m_ColorVertices[num + i] = value;
		}
		num = this.m_Indices.Alloc(_indices.Count);
		for (int j = 0; j < _indices.Count; j++)
		{
			this.m_Indices[num + j] = _indices[j] + curTriangleIndex;
		}
		this.m_Uvs.AddRange(_uvs.Items, 0, _uvs.Count);
		Vector2 value2 = new Vector2((float)damage, 0f);
		num = this.UvsCrack.Alloc(_uvs.Count);
		for (int k = 0; k < _uvs.Count; k++)
		{
			this.UvsCrack[num + k] = value2;
		}
	}

	// Token: 0x1700083D RID: 2109
	// (get) Token: 0x0600513F RID: 20799 RVA: 0x0020A272 File Offset: 0x00208472
	// (set) Token: 0x06005140 RID: 20800 RVA: 0x0020A27A File Offset: 0x0020847A
	public ArrayListMP<int> Indices
	{
		get
		{
			return this.m_Indices;
		}
		set
		{
			this.m_Indices = value;
		}
	}

	// Token: 0x1700083E RID: 2110
	// (get) Token: 0x06005141 RID: 20801 RVA: 0x0020A283 File Offset: 0x00208483
	// (set) Token: 0x06005142 RID: 20802 RVA: 0x0020A28B File Offset: 0x0020848B
	public ArrayListMP<Vector2> Uvs
	{
		get
		{
			return this.m_Uvs;
		}
		set
		{
			this.m_Uvs = value;
		}
	}

	// Token: 0x1700083F RID: 2111
	// (get) Token: 0x06005143 RID: 20803 RVA: 0x0020A294 File Offset: 0x00208494
	// (set) Token: 0x06005144 RID: 20804 RVA: 0x0020A29C File Offset: 0x0020849C
	public ArrayListMP<Vector2> Uvs3
	{
		get
		{
			return this.m_Uvs3;
		}
		set
		{
			this.m_Uvs3 = value;
		}
	}

	// Token: 0x17000840 RID: 2112
	// (get) Token: 0x06005145 RID: 20805 RVA: 0x0020A2A5 File Offset: 0x002084A5
	// (set) Token: 0x06005146 RID: 20806 RVA: 0x0020A2AD File Offset: 0x002084AD
	public ArrayListMP<Vector2> Uvs4
	{
		get
		{
			return this.m_Uvs4;
		}
		set
		{
			this.m_Uvs4 = value;
		}
	}

	// Token: 0x17000841 RID: 2113
	// (get) Token: 0x06005147 RID: 20807 RVA: 0x0020A2B6 File Offset: 0x002084B6
	// (set) Token: 0x06005148 RID: 20808 RVA: 0x0020A2BE File Offset: 0x002084BE
	public ArrayListMP<Vector3> Vertices
	{
		get
		{
			return this.m_Vertices;
		}
		set
		{
			this.m_Vertices = value;
		}
	}

	// Token: 0x17000842 RID: 2114
	// (get) Token: 0x06005149 RID: 20809 RVA: 0x0020A2C7 File Offset: 0x002084C7
	// (set) Token: 0x0600514A RID: 20810 RVA: 0x0020A2CF File Offset: 0x002084CF
	public ArrayListMP<Vector3> Normals
	{
		get
		{
			return this.m_Normals;
		}
		set
		{
			this.m_Normals = value;
		}
	}

	// Token: 0x17000843 RID: 2115
	// (get) Token: 0x0600514B RID: 20811 RVA: 0x0020A2D8 File Offset: 0x002084D8
	// (set) Token: 0x0600514C RID: 20812 RVA: 0x0020A2E0 File Offset: 0x002084E0
	public ArrayListMP<Vector4> Tangents
	{
		get
		{
			return this.m_Tangents;
		}
		set
		{
			this.m_Tangents = value;
		}
	}

	// Token: 0x17000844 RID: 2116
	// (get) Token: 0x0600514D RID: 20813 RVA: 0x0020A2E9 File Offset: 0x002084E9
	// (set) Token: 0x0600514E RID: 20814 RVA: 0x0020A2F1 File Offset: 0x002084F1
	public ArrayListMP<Color> ColorVertices
	{
		get
		{
			return this.m_ColorVertices;
		}
		set
		{
			this.m_ColorVertices = value;
		}
	}

	// Token: 0x17000845 RID: 2117
	// (get) Token: 0x0600514F RID: 20815 RVA: 0x0020A2FA File Offset: 0x002084FA
	public ArrayListMP<int> CollIndices
	{
		get
		{
			return this.m_CollIndices;
		}
	}

	// Token: 0x17000846 RID: 2118
	// (get) Token: 0x06005150 RID: 20816 RVA: 0x0020A302 File Offset: 0x00208502
	public ArrayListMP<Vector3> CollVertices
	{
		get
		{
			return this.m_CollVertices;
		}
	}

	// Token: 0x17000847 RID: 2119
	// (get) Token: 0x06005151 RID: 20817 RVA: 0x0020A30C File Offset: 0x0020850C
	public int Size
	{
		get
		{
			int num = this.m_Indices.Count * 4 + this.m_Uvs.Count * 8 + this.m_Vertices.Count * 12 + this.m_ColorVertices.Count * 16 + this.m_Normals.Count * 12 + this.m_Tangents.Count * 16;
			if (this.UvsCrack != null)
			{
				num += this.UvsCrack.Count * 8;
			}
			return num;
		}
	}

	// Token: 0x17000848 RID: 2120
	// (get) Token: 0x06005152 RID: 20818 RVA: 0x0020A38A File Offset: 0x0020858A
	public int Triangles
	{
		get
		{
			return this.m_Triangles;
		}
	}

	// Token: 0x06005153 RID: 20819 RVA: 0x0020A394 File Offset: 0x00208594
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void calculateMeshTangentsDummy(Mesh mesh)
	{
		int num = mesh.vertices.Length;
		Vector4[] array = new Vector4[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = Vector4.one;
		}
		mesh.tangents = array;
	}

	// Token: 0x06005154 RID: 20820 RVA: 0x0020A3D0 File Offset: 0x002085D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void calculateMeshNormalsDummy(Mesh mesh)
	{
		int num = mesh.vertices.Length;
		Vector3[] array = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = Vector3.one;
		}
		mesh.normals = array;
	}

	// Token: 0x06005155 RID: 20821 RVA: 0x00002914 File Offset: 0x00000B14
	public void CheckVertexLimit(int _count)
	{
	}

	// Token: 0x06005156 RID: 20822 RVA: 0x0020A40C File Offset: 0x0020860C
	public void AddRectXYFacingNorth(float _x, float _y, float _z, int _xAdd, int _yAdd)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y + (float)_yAdd, _z));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z));
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count);
	}

	// Token: 0x06005157 RID: 20823 RVA: 0x0020A4D4 File Offset: 0x002086D4
	public void AddRectXYFacingNorth(float _x, float _y, float _z, int _xAdd, int _yAdd, Color _c)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y + (float)_yAdd, _z));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z));
		_c.a = this.temperature;
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count);
	}

	// Token: 0x06005158 RID: 20824 RVA: 0x0020A5DC File Offset: 0x002087DC
	public void AddRectXYFacingSouth(float _x, float _y, float _z, int _xAdd, int _zAdd)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y + (float)_zAdd, _z));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_zAdd, _z));
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count);
	}

	// Token: 0x06005159 RID: 20825 RVA: 0x0020A6A4 File Offset: 0x002088A4
	public void AddRectXYFacingSouth(float _x, float _y, float _z, int _xAdd, int _zAdd, Color _c)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y + (float)_zAdd, _z));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_zAdd, _z));
		_c.a = this.temperature;
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count);
	}

	// Token: 0x0600515A RID: 20826 RVA: 0x0020A7AC File Offset: 0x002089AC
	public void AddRectYZFacingWest(float _x, float _y, float _z, int _yAdd, int _zAdd)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z));
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count);
	}

	// Token: 0x0600515B RID: 20827 RVA: 0x0020A874 File Offset: 0x00208A74
	public void AddRectYZFacingWest(float _x, float _y, float _z, int _yAdd, int _zAdd, Color _c)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z));
		_c.a = this.temperature;
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count);
	}

	// Token: 0x0600515C RID: 20828 RVA: 0x0020A97C File Offset: 0x00208B7C
	public void AddRectYZFacingEast(float _x, float _y, float _z, int _yAdd, int _zAdd)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z));
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count);
	}

	// Token: 0x0600515D RID: 20829 RVA: 0x0020AA44 File Offset: 0x00208C44
	public void AddRectYZFacingEast(float _x, float _y, float _z, int _yAdd, int _zAdd, Color _c)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y + (float)_yAdd, _z));
		_c.a = this.temperature;
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count);
	}

	// Token: 0x0600515E RID: 20830 RVA: 0x0020AB4C File Offset: 0x00208D4C
	public void AddRectXZFacingUp(float _x, float _y, float _z, int _xAdd, int _zAdd)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count);
	}

	// Token: 0x0600515F RID: 20831 RVA: 0x0020AC14 File Offset: 0x00208E14
	public void AddRectXZFacingUp(float _x, float _y, float _z, int _xAdd, int _zAdd, Color _c)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		_c.a = this.temperature;
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count);
	}

	// Token: 0x06005160 RID: 20832 RVA: 0x0020AD1C File Offset: 0x00208F1C
	public void AddRectXZFacingDown(float _x, float _y, float _z, int _xAdd, int _zAdd)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count);
	}

	// Token: 0x06005161 RID: 20833 RVA: 0x0020ADE4 File Offset: 0x00208FE4
	public void AddRectXZFacingDown(float _x, float _y, float _z, int _xAdd, int _zAdd, Color _c)
	{
		int count = this.m_Vertices.Count;
		this.m_Vertices.Add(new Vector3(_x, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z));
		this.m_Vertices.Add(new Vector3(_x + (float)_xAdd, _y, _z + (float)_zAdd));
		this.m_Vertices.Add(new Vector3(_x, _y, _z + (float)_zAdd));
		_c.a = this.temperature;
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_ColorVertices.Add(_c);
		this.m_Indices.Add(count);
		this.m_Indices.Add(count + 1);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 2);
		this.m_Indices.Add(count + 3);
		this.m_Indices.Add(count);
	}

	// Token: 0x06005162 RID: 20834 RVA: 0x0020AEEA File Offset: 0x002090EA
	public static float GetTemperature(BiomeDefinition bd)
	{
		if (bd == null)
		{
			return -10f;
		}
		return 0f;
	}

	// Token: 0x06005163 RID: 20835 RVA: 0x0020AEFC File Offset: 0x002090FC
	public void ClearTemperatureValues()
	{
		for (int i = 0; i < this.m_ColorVertices.Count; i++)
		{
			this.m_ColorVertices[i] = new Color(this.m_ColorVertices[i].r, this.m_ColorVertices[i].g, this.m_ColorVertices[i].b, 100f);
		}
	}

	// Token: 0x04003E56 RID: 15958
	public static float COLOR_SOUTH = 0.9f;

	// Token: 0x04003E57 RID: 15959
	public static float COLOR_WEST = 0.8f;

	// Token: 0x04003E58 RID: 15960
	public static float COLOR_NORTH = 0.7f;

	// Token: 0x04003E59 RID: 15961
	public static float COLOR_EAST = 0.85f;

	// Token: 0x04003E5A RID: 15962
	public static float COLOR_TOP = 1f;

	// Token: 0x04003E5B RID: 15963
	public static float COLOR_BOTTOM = 0.65f;

	// Token: 0x04003E5C RID: 15964
	public ArrayListMP<Vector3> m_Vertices;

	// Token: 0x04003E5D RID: 15965
	public ArrayListMP<int> m_Indices;

	// Token: 0x04003E5E RID: 15966
	public ArrayListMP<Vector2> m_Uvs;

	// Token: 0x04003E5F RID: 15967
	public ArrayListMP<Vector2> UvsCrack;

	// Token: 0x04003E60 RID: 15968
	public ArrayListMP<Vector2> m_Uvs3;

	// Token: 0x04003E61 RID: 15969
	public ArrayListMP<Vector2> m_Uvs4;

	// Token: 0x04003E62 RID: 15970
	public ArrayListMP<Vector3> m_Normals;

	// Token: 0x04003E63 RID: 15971
	public ArrayListMP<Vector4> m_Tangents;

	// Token: 0x04003E64 RID: 15972
	public ArrayListMP<Color> m_ColorVertices;

	// Token: 0x04003E65 RID: 15973
	public ArrayListMP<Vector3> m_CollVertices;

	// Token: 0x04003E66 RID: 15974
	public ArrayListMP<int> m_CollIndices;

	// Token: 0x04003E67 RID: 15975
	public int CurTriangleIndex;

	// Token: 0x04003E68 RID: 15976
	[PublicizedFrom(EAccessModifier.Protected)]
	public int m_Triangles;

	// Token: 0x04003E69 RID: 15977
	public int meshIndex;

	// Token: 0x04003E6A RID: 15978
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTemperatureDefault = -10f;

	// Token: 0x04003E6B RID: 15979
	[PublicizedFrom(EAccessModifier.Private)]
	public float temperature;

	// Token: 0x04003E6C RID: 15980
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPoolMeshMax = 250;

	// Token: 0x04003E6D RID: 15981
	public static List<Mesh> meshPool = new List<Mesh>();

	// Token: 0x02000A6C RID: 2668
	public enum EnumMeshType
	{
		// Token: 0x04003E6F RID: 15983
		Blocks,
		// Token: 0x04003E70 RID: 15984
		Models,
		// Token: 0x04003E71 RID: 15985
		Terrain,
		// Token: 0x04003E72 RID: 15986
		Decals
	}

	// Token: 0x02000A6D RID: 2669
	public enum CreateFlags
	{
		// Token: 0x04003E74 RID: 15988
		None,
		// Token: 0x04003E75 RID: 15989
		Collider,
		// Token: 0x04003E76 RID: 15990
		Cracks,
		// Token: 0x04003E77 RID: 15991
		Default
	}
}
