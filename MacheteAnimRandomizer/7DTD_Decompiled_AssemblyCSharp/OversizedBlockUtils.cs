using System;
using UnityEngine;

// Token: 0x0200128A RID: 4746
public static class OversizedBlockUtils
{
	// Token: 0x06009464 RID: 37988 RVA: 0x003B297C File Offset: 0x003B0B7C
	public static Bounds GetLocalStabilityBounds(Bounds localBounds, Quaternion rotation)
	{
		Vector3 vector = Quaternion.Inverse(rotation) * Vector3.down;
		float num = Vector3.Dot(vector, Vector3.one);
		if (!Mathf.Approximately(Mathf.Abs(num), 1f))
		{
			Debug.LogError(string.Format("Error in GetLocalStabilityBounds: relativeDown is not an axis-aligned unit vector. Value: {0}. This could suggest an oversized block has been used with an unsupported rotation.", vector));
		}
		Vector3 min;
		Vector3 max;
		if (num < 0f)
		{
			min = localBounds.min + vector;
			max = localBounds.max + Vector3.Scale(vector, localBounds.size);
		}
		else
		{
			min = localBounds.min + Vector3.Scale(vector, localBounds.size);
			max = localBounds.max + vector;
		}
		Bounds result = default(Bounds);
		result.SetMinMax(min, max);
		result.extents += new Vector3(-0.05f, -0.05f, -0.05f);
		return result;
	}

	// Token: 0x06009465 RID: 37989 RVA: 0x003B2A60 File Offset: 0x003B0C60
	public unsafe static void GetWorldAlignedBoundsExtents(Vector3i position, Quaternion rotation, Bounds localBounds, out Vector3i min, out Vector3i max)
	{
		Span<Vector3> span = new Span<Vector3>(stackalloc byte[checked(unchecked((UIntPtr)8) * (UIntPtr)sizeof(Vector3))], 8);
		OversizedBlockUtils.GetWorldCorners(ref span, position, rotation, localBounds);
		min = new Vector3i(int.MaxValue, int.MaxValue, int.MaxValue);
		max = new Vector3i(int.MinValue, int.MinValue, int.MinValue);
		Span<Vector3> span2 = span;
		for (int i = 0; i < span2.Length; i++)
		{
			Vector3 vector = *span2[i];
			min.x = Mathf.Min(min.x, Mathf.FloorToInt(vector.x));
			min.y = Mathf.Min(min.y, Mathf.FloorToInt(vector.y));
			min.z = Mathf.Min(min.z, Mathf.FloorToInt(vector.z));
			max.x = Mathf.Max(max.x, Mathf.CeilToInt(vector.x));
			max.y = Mathf.Max(max.y, Mathf.CeilToInt(vector.y));
			max.z = Mathf.Max(max.z, Mathf.CeilToInt(vector.z));
		}
	}

	// Token: 0x06009466 RID: 37990 RVA: 0x003B2B94 File Offset: 0x003B0D94
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static void GetWorldCorners(ref Span<Vector3> corners, Vector3i position, Quaternion rotation, Bounds localBounds)
	{
		Vector3 extents = localBounds.extents;
		for (int i = 0; i < 8; i++)
		{
			*corners[i] = localBounds.center + new Vector3(((i & 1) == 0) ? extents.x : (-extents.x), ((i & 2) == 0) ? extents.y : (-extents.y), ((i & 4) == 0) ? extents.z : (-extents.z));
			*corners[i] = position + rotation * *corners[i];
		}
	}

	// Token: 0x06009467 RID: 37991 RVA: 0x003B2C40 File Offset: 0x003B0E40
	public static Matrix4x4 GetBlockWorldToLocalMatrix(Vector3i position, Quaternion rotation)
	{
		return Matrix4x4.TRS(position + new Vector3(0.5f, 0.5f, 0.5f), rotation, Vector3.one).inverse;
	}

	// Token: 0x06009468 RID: 37992 RVA: 0x003B2C80 File Offset: 0x003B0E80
	public static bool IsBlockCenterWithinBounds(Vector3i blockPosition, Bounds localBounds, Matrix4x4 worldToLocalMatrix)
	{
		Vector3 point = worldToLocalMatrix.MultiplyPoint3x4(blockPosition + new Vector3(0.5f, 0.5f, 0.5f));
		return localBounds.Contains(point);
	}
}
