using System;
using System.Collections.Generic;

// Token: 0x02000B49 RID: 2889
public class GenerateVoxelCubeSurface
{
	// Token: 0x060059E3 RID: 23011 RVA: 0x002428C6 File Offset: 0x00240AC6
	public static IEnumerable<Vector3i> GenerateCubeSurfacePositions(Vector3i origin, int radius)
	{
		if (radius <= 0)
		{
			yield return origin;
			yield break;
		}
		foreach (Vector3i vector3i in GenerateVoxelCubeSurface.GenerateCubeSurfaceTop(origin, radius))
		{
			yield return vector3i;
		}
		IEnumerator<Vector3i> enumerator = null;
		foreach (Vector3i vector3i2 in GenerateVoxelCubeSurface.GenerateCubeSurfaceBottom(origin, radius))
		{
			yield return vector3i2;
		}
		enumerator = null;
		foreach (Vector3i vector3i3 in GenerateVoxelCubeSurface.GenerateCubeSurfaceLeft(origin, radius))
		{
			yield return vector3i3;
		}
		enumerator = null;
		foreach (Vector3i vector3i4 in GenerateVoxelCubeSurface.GenerateCubeSurfaceRight(origin, radius))
		{
			yield return vector3i4;
		}
		enumerator = null;
		foreach (Vector3i vector3i5 in GenerateVoxelCubeSurface.GenerateCubeSurfaceFront(origin, radius))
		{
			yield return vector3i5;
		}
		enumerator = null;
		foreach (Vector3i vector3i6 in GenerateVoxelCubeSurface.GenerateCubeSurfaceBack(origin, radius))
		{
			yield return vector3i6;
		}
		enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060059E4 RID: 23012 RVA: 0x002428DD File Offset: 0x00240ADD
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateCubeSurfaceTop(Vector3i origin, int radius)
	{
		origin.x -= radius;
		origin.y += radius;
		origin.z -= radius;
		int num = radius * 2 + 1;
		foreach (Vector3i vector3i in GenerateVoxelCubeSurface.GenerateXZ(origin, num, num))
		{
			yield return vector3i;
		}
		IEnumerator<Vector3i> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060059E5 RID: 23013 RVA: 0x002428F4 File Offset: 0x00240AF4
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateCubeSurfaceBottom(Vector3i origin, int radius)
	{
		origin.x -= radius;
		origin.y -= radius;
		origin.z -= radius;
		int num = radius * 2 + 1;
		foreach (Vector3i vector3i in GenerateVoxelCubeSurface.GenerateXZ(origin, num, num))
		{
			yield return vector3i;
		}
		IEnumerator<Vector3i> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060059E6 RID: 23014 RVA: 0x0024290B File Offset: 0x00240B0B
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateCubeSurfaceRight(Vector3i origin, int radius)
	{
		origin.x += radius;
		origin.y -= radius - 1;
		origin.z -= radius;
		int num = radius * 2 + 1;
		foreach (Vector3i vector3i in GenerateVoxelCubeSurface.GenerateYZ(origin, num - 2, num))
		{
			yield return vector3i;
		}
		IEnumerator<Vector3i> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060059E7 RID: 23015 RVA: 0x00242922 File Offset: 0x00240B22
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateCubeSurfaceLeft(Vector3i origin, int radius)
	{
		origin.x -= radius;
		origin.y -= radius - 1;
		origin.z -= radius;
		int num = radius * 2 + 1;
		foreach (Vector3i vector3i in GenerateVoxelCubeSurface.GenerateYZ(origin, num - 2, num))
		{
			yield return vector3i;
		}
		IEnumerator<Vector3i> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060059E8 RID: 23016 RVA: 0x00242939 File Offset: 0x00240B39
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateCubeSurfaceFront(Vector3i origin, int radius)
	{
		origin.x -= radius - 1;
		origin.y -= radius - 1;
		origin.z += radius;
		int num = radius * 2 - 1;
		foreach (Vector3i vector3i in GenerateVoxelCubeSurface.GenerateXY(origin, num, num))
		{
			yield return vector3i;
		}
		IEnumerator<Vector3i> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060059E9 RID: 23017 RVA: 0x00242950 File Offset: 0x00240B50
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateCubeSurfaceBack(Vector3i origin, int radius)
	{
		origin.x -= radius - 1;
		origin.y -= radius - 1;
		origin.z -= radius;
		int num = radius * 2 - 1;
		foreach (Vector3i vector3i in GenerateVoxelCubeSurface.GenerateXY(origin, num, num))
		{
			yield return vector3i;
		}
		IEnumerator<Vector3i> enumerator = null;
		yield break;
		yield break;
	}

	// Token: 0x060059EA RID: 23018 RVA: 0x00242967 File Offset: 0x00240B67
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateXZ(Vector3i min, int xLength, int zLength)
	{
		if (min.y < 0 || min.y > 255)
		{
			yield break;
		}
		int xEnd = min.x + xLength;
		int zEnd = min.z + zLength;
		int num;
		for (int x = min.x; x < xEnd; x = num + 1)
		{
			for (int z = min.z; z < zEnd; z = num + 1)
			{
				yield return new Vector3i(x, min.y, z);
				num = z;
			}
			num = x;
		}
		yield break;
	}

	// Token: 0x060059EB RID: 23019 RVA: 0x00242985 File Offset: 0x00240B85
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateYZ(Vector3i min, int yLength, int zLength)
	{
		int yEnd = min.y + yLength;
		int zEnd = min.z + zLength;
		int num;
		for (int y = min.y; y < yEnd; y = num + 1)
		{
			if (y >= 0 && y < 256)
			{
				for (int z = min.z; z < zEnd; z = num + 1)
				{
					yield return new Vector3i(min.x, y, z);
					num = z;
				}
			}
			num = y;
		}
		yield break;
	}

	// Token: 0x060059EC RID: 23020 RVA: 0x002429A3 File Offset: 0x00240BA3
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerable<Vector3i> GenerateXY(Vector3i min, int xLength, int yLength)
	{
		int xEnd = min.x + xLength;
		int yEnd = min.y + yLength;
		int num;
		for (int y = min.y; y < yEnd; y = num + 1)
		{
			if (y >= 0 && y < 256)
			{
				for (int x = min.x; x < xEnd; x = num + 1)
				{
					yield return new Vector3i(x, y, min.z);
					num = x;
				}
			}
			num = y;
		}
		yield break;
	}
}
