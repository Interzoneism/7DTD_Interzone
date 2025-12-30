using System;
using UnityEngine;

// Token: 0x02000AB0 RID: 2736
public class DChunkSquareMesh
{
	// Token: 0x06005461 RID: 21601 RVA: 0x0021F48C File Offset: 0x0021D68C
	public DChunkSquareMesh(DistantChunkMap DCMap, int LODLevel)
	{
		DistantChunkMapInfo distantChunkMapInfo = DCMap.ChunkMapInfoArray[LODLevel];
		this.Init(distantChunkMapInfo.BaseMesh.Vertices.Length, LODLevel, distantChunkMapInfo.ChunkResolution, distantChunkMapInfo.ColliderResolution, DCMap.NbResLevel, distantChunkMapInfo.BaseMesh.Triangles.Length);
	}

	// Token: 0x06005462 RID: 21602 RVA: 0x0021F4EC File Offset: 0x0021D6EC
	public void Init(int NbVertices, int ResLevel, int Resolution, int ColliderResolution, int MaxNbResLevel, int NbTriangles)
	{
		this.Normals = new Vector3[NbVertices];
		this.Tangents = new Vector4[NbVertices];
		this.EdgeCorNormals = new Vector3[Resolution * 4];
		this.Colors = new Color[NbVertices];
		this.TextureId = new int[NbVertices];
		this.IsWater = new bool[NbVertices];
		this.ChunkBound = default(Bounds);
		this.ColVertices = new Vector3[ColliderResolution * ColliderResolution];
		this.ColVerticesHeight = new float[ColliderResolution * ColliderResolution];
	}

	// Token: 0x040040B2 RID: 16562
	public Vector3[] Normals;

	// Token: 0x040040B3 RID: 16563
	public Vector4[] Tangents;

	// Token: 0x040040B4 RID: 16564
	public Vector3[] EdgeCorNormals;

	// Token: 0x040040B5 RID: 16565
	public Color[] Colors;

	// Token: 0x040040B6 RID: 16566
	public int[] TextureId;

	// Token: 0x040040B7 RID: 16567
	public Bounds ChunkBound;

	// Token: 0x040040B8 RID: 16568
	public int WaterPlaneBlockId;

	// Token: 0x040040B9 RID: 16569
	public Vector3[] ColVertices;

	// Token: 0x040040BA RID: 16570
	public float[] ColVerticesHeight;

	// Token: 0x040040BB RID: 16571
	public VoxelMeshTerrain VoxelMesh = new VoxelMeshTerrain(0, 500);

	// Token: 0x040040BC RID: 16572
	public bool[] IsWater;
}
