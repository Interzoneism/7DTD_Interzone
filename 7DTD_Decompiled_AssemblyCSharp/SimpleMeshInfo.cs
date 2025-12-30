using System;
using UnityEngine;

// Token: 0x02000891 RID: 2193
public class SimpleMeshInfo
{
	// Token: 0x06004009 RID: 16393 RVA: 0x001A396B File Offset: 0x001A1B6B
	public SimpleMeshInfo(string[] _meshNames, Mesh[] _meshes, float _offsetY, Material _mat)
	{
		this.meshNames = _meshNames;
		this.meshes = _meshes;
		this.offsetY = _offsetY;
		this.mat = _mat;
	}

	// Token: 0x0400336A RID: 13162
	public string[] meshNames;

	// Token: 0x0400336B RID: 13163
	public Mesh[] meshes;

	// Token: 0x0400336C RID: 13164
	public readonly float offsetY;

	// Token: 0x0400336D RID: 13165
	public readonly Material mat;
}
