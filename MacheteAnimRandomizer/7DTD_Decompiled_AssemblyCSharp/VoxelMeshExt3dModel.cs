using System;
using System.IO;
using UnityEngine;

// Token: 0x02000A6E RID: 2670
public class VoxelMeshExt3dModel : VoxelMesh
{
	// Token: 0x06005165 RID: 20837 RVA: 0x0020AFBC File Offset: 0x002091BC
	public VoxelMeshExt3dModel(int _meshIndex, int _minSize = 500) : base(_meshIndex, _minSize, VoxelMesh.CreateFlags.Default)
	{
	}

	// Token: 0x06005166 RID: 20838 RVA: 0x0020B01C File Offset: 0x0020921C
	public override void Write(BinaryWriter _bw)
	{
		base.Write(_bw);
		_bw.Write((uint)this.aabb.Length);
		foreach (Bounds bounds in this.aabb)
		{
			BoundsUtils.WriteBounds(_bw, bounds);
		}
		this.boundingBoxMesh.Write(_bw);
		this.lod1Mesh.Write(_bw);
	}

	// Token: 0x06005167 RID: 20839 RVA: 0x0020B07C File Offset: 0x0020927C
	public override void Read(BinaryReader _br)
	{
		base.Read(_br);
		uint num = _br.ReadUInt32();
		this.aabb = new Bounds[num];
		int num2 = 0;
		while ((long)num2 < (long)((ulong)num))
		{
			this.aabb[num2] = BoundsUtils.ReadBounds(_br);
			num2++;
		}
		this.boundingBoxMesh.ClearMesh();
		this.boundingBoxMesh.Read(_br);
		this.lod1Mesh.ClearMesh();
		this.lod1Mesh.Read(_br);
		this.lod1Mesh.ClearMesh();
	}

	// Token: 0x06005168 RID: 20840 RVA: 0x0020B0FC File Offset: 0x002092FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void addColliders(Vector3 _drawPos, VoxelMesh _colliderMesh)
	{
		if (_colliderMesh.Vertices.Count == 0)
		{
			return;
		}
		int count = this.boundsVertices.Count;
		for (int i = 0; i < _colliderMesh.Vertices.Count; i++)
		{
			this.boundsVertices.Add(_drawPos + _colliderMesh.Vertices[i]);
		}
		for (int j = 0; j < _colliderMesh.Indices.Count; j++)
		{
			this.boundsTriangles.Add(count + _colliderMesh.Indices[j]);
		}
	}

	// Token: 0x06005169 RID: 20841 RVA: 0x0020B188 File Offset: 0x00209388
	public override void AddMesh(Vector3 _drawPos, int _count, Vector3[] _vertices, Vector3[] _normals, ArrayListMP<int> _indices, ArrayListMP<Vector2> _uvs, byte _sunlight, byte _blocklight, VoxelMesh _specialColliders, int damage)
	{
		if (_specialColliders != null)
		{
			this.addColliders(_drawPos, _specialColliders);
		}
		base.AddMesh(_drawPos, _count, _vertices, _normals, _indices, _uvs, _sunlight, _blocklight, null, damage);
	}

	// Token: 0x0600516A RID: 20842 RVA: 0x0020B1B8 File Offset: 0x002093B8
	public override int CopyToColliders(int _clrIdx, MeshCollider _meshCollider, out Mesh mesh)
	{
		if (this.boundsVertices == null || this.boundsVertices.Count == 0)
		{
			GameManager.Instance.World.m_ChunkManager.BakeDestroyCancel(_meshCollider);
			mesh = null;
			return 0;
		}
		mesh = base.ResetMesh(_meshCollider);
		MeshUnsafeCopyHelper.CopyVertices(this.boundsVertices, mesh);
		MeshUnsafeCopyHelper.CopyTriangles(this.boundsTriangles, mesh);
		_meshCollider.tag = "T_Mesh_B";
		return this.boundsTriangles.Count / 3;
	}

	// Token: 0x0600516B RID: 20843 RVA: 0x0020B22F File Offset: 0x0020942F
	public override void ClearMesh()
	{
		base.ClearMesh();
		this.lod1Mesh.ClearMesh();
		this.boundsVertices.Clear();
		this.boundsTriangles.Clear();
	}

	// Token: 0x0600516C RID: 20844 RVA: 0x0020B258 File Offset: 0x00209458
	public override void Finished()
	{
		base.Finished();
		this.lod1Mesh.Finished();
	}

	// Token: 0x04003E78 RID: 15992
	public VoxelMesh boundingBoxMesh = new VoxelMesh(-1, 0, VoxelMesh.CreateFlags.Default);

	// Token: 0x04003E79 RID: 15993
	public Bounds[] aabb = new Bounds[0];

	// Token: 0x04003E7A RID: 15994
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayListMP<Vector3> boundsVertices = new ArrayListMP<Vector3>(MemoryPools.poolVector3, 0);

	// Token: 0x04003E7B RID: 15995
	[PublicizedFrom(EAccessModifier.Private)]
	public ArrayListMP<int> boundsTriangles = new ArrayListMP<int>(MemoryPools.poolInt, 0);

	// Token: 0x04003E7C RID: 15996
	public VoxelMesh lod1Mesh = new VoxelMesh(-1, 0, VoxelMesh.CreateFlags.Default);
}
