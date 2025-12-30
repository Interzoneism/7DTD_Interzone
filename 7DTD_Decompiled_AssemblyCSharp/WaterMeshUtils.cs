using System;
using UnityEngine;

// Token: 0x02000B6B RID: 2923
public static class WaterMeshUtils
{
	// Token: 0x06005AD3 RID: 23251 RVA: 0x0024706B File Offset: 0x0024526B
	public static void RenderFace(Vector3[] _vertices, LightingAround _lightingAround, long _textureFull, VoxelMesh[] _meshes, Vector2 UVdata, bool _alternateWinding = false)
	{
		_meshes[1].AddBasicQuad(_vertices, Color.white, UVdata, true, _alternateWinding);
	}
}
