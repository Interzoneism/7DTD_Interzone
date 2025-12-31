using System;
using UnityEngine;

// Token: 0x02000AB6 RID: 2742
public class DistantChunkMapInfo
{
	// Token: 0x04004119 RID: 16665
	public Vector4[] ChunkTriggerArea;

	// Token: 0x0400411A RID: 16666
	public Vector2i[][] ChunkToDelete;

	// Token: 0x0400411B RID: 16667
	public Vector2i[][] ChunkToAdd;

	// Token: 0x0400411C RID: 16668
	public Vector2i[][] ChunkToConvDel;

	// Token: 0x0400411D RID: 16669
	public Vector2i[][] ChunkToConvAdd;

	// Token: 0x0400411E RID: 16670
	public Vector4[][] ChunkToConvAddEdgeFactor;

	// Token: 0x0400411F RID: 16671
	public Vector4[][] ChunkToAddEdgeFactor;

	// Token: 0x04004120 RID: 16672
	public Vector2i[][][] ChunkEdgeToOwnResLevel;

	// Token: 0x04004121 RID: 16673
	public int[][][] ChunkEdgeToOwnRLEdgeId;

	// Token: 0x04004122 RID: 16674
	public Vector2i[][][] ChunkEdgeToNextResLevel;

	// Token: 0x04004123 RID: 16675
	public int[][][] ChunkEdgeToNextRLEdgeId;

	// Token: 0x04004124 RID: 16676
	public Vector2i[] ChunkLLIntPos;

	// Token: 0x04004125 RID: 16677
	public Vector2[] ChunkLLPos;

	// Token: 0x04004126 RID: 16678
	public int[][] NeighbResLevel;

	// Token: 0x04004127 RID: 16679
	public float[][] EdgeResFactor;

	// Token: 0x04004128 RID: 16680
	public float NextResLevelEdgeFactor;

	// Token: 0x04004129 RID: 16681
	public int NbChunk;

	// Token: 0x0400412A RID: 16682
	public int NbCurChunkInOneNextLevelChunk;

	// Token: 0x0400412B RID: 16683
	public int ResLevel;

	// Token: 0x0400412C RID: 16684
	public int LayerId = 28;

	// Token: 0x0400412D RID: 16685
	public int ChunkDataListResLevel;

	// Token: 0x0400412E RID: 16686
	public int ChunkResolution;

	// Token: 0x0400412F RID: 16687
	public int ColliderResolution;

	// Token: 0x04004130 RID: 16688
	public bool IsColliderEnabled;

	// Token: 0x04004131 RID: 16689
	public float ResRadius;

	// Token: 0x04004132 RID: 16690
	public int IntResRadius;

	// Token: 0x04004133 RID: 16691
	public Vector2i LLIntArea;

	// Token: 0x04004134 RID: 16692
	public float ChunkWidth;

	// Token: 0x04004135 RID: 16693
	public float UnitStep;

	// Token: 0x04004136 RID: 16694
	public Vector2 ShiftVec;

	// Token: 0x04004137 RID: 16695
	public Vector3 ChunkExtraShiftVector;

	// Token: 0x04004138 RID: 16696
	public DistantChunkBasicMesh BaseMesh;

	// Token: 0x04004139 RID: 16697
	public int[][] EdgeMap;

	// Token: 0x0400413A RID: 16698
	public int[] SouthMap;

	// Token: 0x0400413B RID: 16699
	public int[] WestMap;

	// Token: 0x0400413C RID: 16700
	public int[] NorthMap;

	// Token: 0x0400413D RID: 16701
	public int[] EastMap;
}
