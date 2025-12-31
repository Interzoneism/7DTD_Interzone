using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020011CC RID: 4556
public static class MeshUnsafeCopyHelper
{
	// Token: 0x06008E60 RID: 36448 RVA: 0x003909E4 File Offset: 0x0038EBE4
	public static void CopyVertices(ArrayListMP<Vector3> _source, Mesh _mesh)
	{
		IndexFormat indexFormat = (_source.Count <= 65535) ? IndexFormat.UInt16 : IndexFormat.UInt32;
		_mesh.indexFormat = indexFormat;
		_mesh.SetVertices(_source.Items, 0, _source.Count);
	}

	// Token: 0x06008E61 RID: 36449 RVA: 0x00390A1D File Offset: 0x0038EC1D
	public static void CopyUV(ArrayListMP<Vector2> _source, Mesh _mesh)
	{
		_mesh.SetUVs(0, _source.Items, 0, _source.Count);
	}

	// Token: 0x06008E62 RID: 36450 RVA: 0x00390A33 File Offset: 0x0038EC33
	public static void CopyUV2(ArrayListMP<Vector2> _source, Mesh _mesh)
	{
		_mesh.SetUVs(1, _source.Items, 0, _source.Count);
	}

	// Token: 0x06008E63 RID: 36451 RVA: 0x00390A49 File Offset: 0x0038EC49
	public static void CopyUV3(ArrayListMP<Vector2> _source, Mesh _mesh)
	{
		_mesh.SetUVs(2, _source.Items, 0, _source.Count);
	}

	// Token: 0x06008E64 RID: 36452 RVA: 0x00390A5F File Offset: 0x0038EC5F
	public static void CopyUV4(ArrayListMP<Vector2> _source, Mesh _mesh)
	{
		_mesh.SetUVs(3, _source.Items, 0, _source.Count);
	}

	// Token: 0x06008E65 RID: 36453 RVA: 0x00390A75 File Offset: 0x0038EC75
	public static void CopyNormals(ArrayListMP<Vector3> _source, Mesh _mesh)
	{
		_mesh.SetNormals(_source.Items, 0, _source.Count);
	}

	// Token: 0x06008E66 RID: 36454 RVA: 0x00390A8A File Offset: 0x0038EC8A
	public static void CopyTangents(ArrayListMP<Vector4> _source, Mesh _mesh)
	{
		_mesh.SetTangents(_source.Items, 0, _source.Count);
	}

	// Token: 0x06008E67 RID: 36455 RVA: 0x00390A9F File Offset: 0x0038EC9F
	public static void CopyTriangles(ArrayListMP<int> _source, Mesh _mesh)
	{
		_mesh.SetTriangles(_source.Items, 0, _source.Count, 0, true, 0);
	}

	// Token: 0x06008E68 RID: 36456 RVA: 0x00390AB7 File Offset: 0x0038ECB7
	public static void CopyTriangles(ArrayListMP<int> _source, Mesh _mesh, int _subMeshIdx)
	{
		_mesh.SetTriangles(_source.Items, 0, _source.Count, _subMeshIdx, true, 0);
	}

	// Token: 0x06008E69 RID: 36457 RVA: 0x00390ACF File Offset: 0x0038ECCF
	public static void CopyColors(ArrayListMP<Color> _source, Mesh _mesh)
	{
		_mesh.SetColors(_source.Items, 0, _source.Count);
	}
}
