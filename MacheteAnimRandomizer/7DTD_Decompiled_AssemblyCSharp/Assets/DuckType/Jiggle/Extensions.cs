using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DuckType.Jiggle
{
	// Token: 0x0200199A RID: 6554
	public static class Extensions
	{
		// Token: 0x0600C0E4 RID: 49380 RVA: 0x004906AF File Offset: 0x0048E8AF
		public static Quaternion Append(this Quaternion source, Quaternion quaternion)
		{
			return quaternion * source;
		}

		// Token: 0x0600C0E5 RID: 49381 RVA: 0x004906B8 File Offset: 0x0048E8B8
		public static Quaternion FromToRotation(this Quaternion source, Quaternion target)
		{
			return Quaternion.Inverse(source) * target;
		}

		// Token: 0x0600C0E6 RID: 49382 RVA: 0x004906C6 File Offset: 0x0048E8C6
		public static Quaternion Scale(this Quaternion source, float scale)
		{
			return Quaternion.SlerpUnclamped(Quaternion.identity, source, scale);
		}

		// Token: 0x0600C0E7 RID: 49383 RVA: 0x004906D4 File Offset: 0x0048E8D4
		public static Quaternion Inverse(this Quaternion source)
		{
			return Quaternion.Inverse(source);
		}

		// Token: 0x0600C0E8 RID: 49384 RVA: 0x004906DC File Offset: 0x0048E8DC
		public static List<Vector3> GetOrthogonalVectors(this Vector3 source, int numVectors)
		{
			Vector3 normalized = source.normalized;
			Vector3 point = (Mathf.Abs(source.normalized.y) != 1f) ? Vector3.Cross(source, Vector3.up) : Vector3.Cross(source, Vector3.right);
			float num = 360f / (float)numVectors;
			List<Vector3> list = new List<Vector3>();
			for (int i = 0; i < numVectors; i++)
			{
				list.Add(Quaternion.AngleAxis(num * (float)i, source) * point);
			}
			return list;
		}

		// Token: 0x0600C0E9 RID: 49385 RVA: 0x00490754 File Offset: 0x0048E954
		public static bool HasLength(this Vector3 source)
		{
			return source.x != 0f || source.y != 0f || source.z != 0f;
		}

		// Token: 0x0600C0EA RID: 49386 RVA: 0x00490782 File Offset: 0x0048E982
		public static float Clamp01(this float source)
		{
			return Mathf.Max(Mathf.Min(source, 1f), 0f);
		}
	}
}
