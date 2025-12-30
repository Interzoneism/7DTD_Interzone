using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B47 RID: 2887
public static class CollectWaterUtils
{
	// Token: 0x060059E0 RID: 23008 RVA: 0x00242764 File Offset: 0x00240964
	public static int CollectInCube(ChunkCluster cc, int requiredMass, Vector3i origin, int maxRadius, List<CollectWaterUtils.WaterPoint> points)
	{
		int num = requiredMass;
		for (int i = 0; i <= maxRadius; i++)
		{
			int num2 = 0;
			int num3 = 0;
			foreach (Vector3i pos in GenerateVoxelCubeSurface.GenerateCubeSurfacePositions(origin, i))
			{
				WaterValue water = cc.GetWater(pos);
				if (water.HasMass())
				{
					int mass = water.GetMass();
					points.Add(new CollectWaterUtils.WaterPoint(pos, mass));
					num2 += mass;
					num3++;
				}
			}
			if (num2 > num)
			{
				int j = num2 - num;
				int a = (num2 - num) / num3;
				a = Mathf.Max(a, 1);
				int num4 = points.Count - num3;
				while (j > 0)
				{
					CollectWaterUtils.WaterPoint waterPoint = points[num4];
					if (waterPoint.massToTake > 0)
					{
						int num5 = Mathf.Min(a, waterPoint.massToTake);
						waterPoint.massToTake -= num5;
						j -= num5;
						num2 -= num5;
						points[num4] = waterPoint;
					}
					num4++;
					if (num4 == points.Count)
					{
						num4 = points.Count - num3;
					}
				}
			}
			num -= num2;
			if (num <= 0)
			{
				break;
			}
		}
		return requiredMass - num;
	}

	// Token: 0x02000B48 RID: 2888
	public struct WaterPoint
	{
		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x060059E1 RID: 23009 RVA: 0x002428A0 File Offset: 0x00240AA0
		public int finalMass
		{
			get
			{
				return this.mass - this.massToTake;
			}
		}

		// Token: 0x060059E2 RID: 23010 RVA: 0x002428AF File Offset: 0x00240AAF
		public WaterPoint(Vector3i _pos, int _mass)
		{
			this.worldPos = _pos;
			this.mass = _mass;
			this.massToTake = _mass;
		}

		// Token: 0x040044B3 RID: 17587
		public Vector3i worldPos;

		// Token: 0x040044B4 RID: 17588
		public int mass;

		// Token: 0x040044B5 RID: 17589
		public int massToTake;
	}
}
