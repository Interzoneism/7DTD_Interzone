using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000368 RID: 872
public class MeshLists
{
	// Token: 0x060019BC RID: 6588 RVA: 0x0009D76A File Offset: 0x0009B96A
	public static MeshLists GetList()
	{
		if (MeshLists.MeshListCache.Count == 0)
		{
			return new MeshLists();
		}
		return MeshLists.MeshListCache.Pop();
	}

	// Token: 0x060019BD RID: 6589 RVA: 0x0009D788 File Offset: 0x0009B988
	public static void ReturnList(MeshLists list)
	{
		list.Reset();
		MeshLists.MeshListCache.Push(list);
		if (MeshLists.MeshListCache.Count > MeshLists.LastLargest)
		{
			MeshLists.LastLargest = MeshLists.MeshListCache.Count;
			Log.Out("Meshlist count is now " + MeshLists.MeshListCache.Count.ToString());
		}
	}

	// Token: 0x060019BE RID: 6590 RVA: 0x0009D7E8 File Offset: 0x0009B9E8
	public void Reset()
	{
		this.Vertices.Clear();
		this.Uvs.Clear();
		this.Uvs2.Clear();
		this.Uvs3.Clear();
		this.Uvs4.Clear();
		this.Triangles.Clear();
		foreach (List<int> list in this.TerrainTriangles)
		{
			list.Clear();
		}
		this.TerrainTriangles.Clear();
		this.Colours.Clear();
		this.Normals.Clear();
		this.Tangents.Clear();
	}

	// Token: 0x04001089 RID: 4233
	public static Stack<MeshLists> MeshListCache = new Stack<MeshLists>();

	// Token: 0x0400108A RID: 4234
	public static int LastLargest = 0;

	// Token: 0x0400108B RID: 4235
	public List<Vector3> Vertices = new List<Vector3>();

	// Token: 0x0400108C RID: 4236
	public List<Vector2> Uvs = new List<Vector2>();

	// Token: 0x0400108D RID: 4237
	public List<Vector2> Uvs2 = new List<Vector2>();

	// Token: 0x0400108E RID: 4238
	public List<Vector2> Uvs3 = new List<Vector2>();

	// Token: 0x0400108F RID: 4239
	public List<Vector2> Uvs4 = new List<Vector2>();

	// Token: 0x04001090 RID: 4240
	public List<int> Triangles = new List<int>();

	// Token: 0x04001091 RID: 4241
	public List<List<int>> TerrainTriangles = new List<List<int>>();

	// Token: 0x04001092 RID: 4242
	public List<Color> Colours = new List<Color>();

	// Token: 0x04001093 RID: 4243
	public List<Vector3> Normals = new List<Vector3>();

	// Token: 0x04001094 RID: 4244
	public List<Vector4> Tangents = new List<Vector4>();
}
