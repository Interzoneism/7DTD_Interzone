using System;
using UnityEngine;

// Token: 0x02000B72 RID: 2930
public static class WaterDebugAssets
{
	// Token: 0x17000949 RID: 2377
	// (get) Token: 0x06005AEB RID: 23275 RVA: 0x00247224 File Offset: 0x00245424
	public static Mesh CubeMesh
	{
		get
		{
			return WaterDebugAssets.cubeMesh.Value;
		}
	}

	// Token: 0x06005AEC RID: 23276 RVA: 0x00247230 File Offset: 0x00245430
	public static Mesh GenerateCubeMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, -0.5f, -0.5f),
			new Vector3(0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, -0.5f),
			new Vector3(-0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.5f, -0.5f, 0.5f),
			new Vector3(-0.5f, -0.5f, 0.5f)
		};
		mesh.triangles = new int[]
		{
			0,
			2,
			1,
			0,
			3,
			2,
			2,
			3,
			4,
			2,
			4,
			5,
			1,
			2,
			5,
			1,
			5,
			6,
			0,
			7,
			4,
			0,
			4,
			3,
			5,
			4,
			7,
			5,
			7,
			6,
			0,
			6,
			7,
			0,
			1,
			6
		};
		mesh.Optimize();
		mesh.RecalculateNormals();
		return mesh;
	}

	// Token: 0x1700094A RID: 2378
	// (get) Token: 0x06005AED RID: 23277 RVA: 0x0024734C File Offset: 0x0024554C
	public static Material DebugMaterial
	{
		get
		{
			return WaterDebugAssets.sharedMaterial.Value;
		}
	}

	// Token: 0x06005AEE RID: 23278 RVA: 0x00247358 File Offset: 0x00245558
	public static Material CreateDebugMaterial()
	{
		return new Material(Shader.Find("Debug/DebugInstancedProcedural"))
		{
			enableInstancing = true
		};
	}

	// Token: 0x04004581 RID: 17793
	[PublicizedFrom(EAccessModifier.Private)]
	public static Lazy<Mesh> cubeMesh = new Lazy<Mesh>(new Func<Mesh>(WaterDebugAssets.GenerateCubeMesh));

	// Token: 0x04004582 RID: 17794
	[PublicizedFrom(EAccessModifier.Private)]
	public static Lazy<Material> sharedMaterial = new Lazy<Material>(new Func<Material>(WaterDebugAssets.CreateDebugMaterial));
}
