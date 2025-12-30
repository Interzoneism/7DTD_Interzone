using System;
using UnityEngine;

// Token: 0x020005A6 RID: 1446
public interface IMarchingCubes
{
	// Token: 0x06002EA1 RID: 11937
	void Polygonize(INeighborBlockCache _nBlocks, Vector3i _localPos, Vector3 _offsetPos, byte _sunLight, byte _blockLight, VoxelMesh _mesh);
}
