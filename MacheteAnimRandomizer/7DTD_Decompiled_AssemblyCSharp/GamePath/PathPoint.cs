using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015D6 RID: 5590
	public class PathPoint
	{
		// Token: 0x0600AB9B RID: 43931 RVA: 0x004377C0 File Offset: 0x004359C0
		public static PathPoint Allocate(Vector3 _pos)
		{
			DynamicObjectPool<PathPoint> s_pool = MemoryPools.s_pool;
			PathPoint result;
			lock (s_pool)
			{
				PathPoint pathPoint = MemoryPools.s_pool.Allocate();
				pathPoint.x = (int)_pos.x;
				pathPoint.y = (int)_pos.y;
				pathPoint.z = (int)_pos.z;
				pathPoint.projectedLocation = _pos;
				pathPoint.hash = PathPoint.makeHash(pathPoint.x, pathPoint.y, pathPoint.z);
				result = pathPoint;
			}
			return result;
		}

		// Token: 0x0600AB9C RID: 43932 RVA: 0x00437854 File Offset: 0x00435A54
		public static void CompactPool()
		{
			DynamicObjectPool<PathPoint> s_pool = MemoryPools.s_pool;
			lock (s_pool)
			{
				MemoryPools.s_pool.Compact();
			}
		}

		// Token: 0x0600AB9D RID: 43933 RVA: 0x00437898 File Offset: 0x00435A98
		public void Release()
		{
			DynamicObjectPool<PathPoint> s_pool = MemoryPools.s_pool;
			lock (s_pool)
			{
				MemoryPools.s_pool.Free(this);
			}
		}

		// Token: 0x0600AB9E RID: 43934 RVA: 0x004378DC File Offset: 0x00435ADC
		public static int makeHash(int _x, int _y, int _z)
		{
			return (_y & 255) | (_x & 32767) << 8 | (_z & 32767) << 24 | ((_x >= 0) ? 0 : int.MinValue) | ((_z >= 0) ? 0 : 32768);
		}

		// Token: 0x0600AB9F RID: 43935 RVA: 0x00437914 File Offset: 0x00435B14
		public override bool Equals(object _obj)
		{
			PathPoint pathPoint = _obj as PathPoint;
			return pathPoint != null && this.hash == pathPoint.hash && this.IsSamePos(pathPoint);
		}

		// Token: 0x0600ABA0 RID: 43936 RVA: 0x00437944 File Offset: 0x00435B44
		public bool IsSamePos(PathPoint _p)
		{
			return _p.x == this.x && _p.y == this.y && _p.z == this.z;
		}

		// Token: 0x0600ABA1 RID: 43937 RVA: 0x00437972 File Offset: 0x00435B72
		public override int GetHashCode()
		{
			return this.hash;
		}

		// Token: 0x0600ABA2 RID: 43938 RVA: 0x0043797C File Offset: 0x00435B7C
		public float GetDistanceSq(int _x, int _y, int _z)
		{
			int num = this.x - _x;
			int num2 = this.y - _y;
			int num3 = this.z - _z;
			return (float)(num * num + num2 * num2 + num3 * num3);
		}

		// Token: 0x0600ABA3 RID: 43939 RVA: 0x004379AE File Offset: 0x00435BAE
		public Vector3 AdjustedPositionForEntity(Entity entity)
		{
			return this.projectedLocation;
		}

		// Token: 0x0600ABA4 RID: 43940 RVA: 0x004379AE File Offset: 0x00435BAE
		public Vector3 ProjectToGround(Entity entity)
		{
			return this.projectedLocation;
		}

		// Token: 0x0600ABA5 RID: 43941 RVA: 0x004379B6 File Offset: 0x00435BB6
		public Vector3i GetBlockPos()
		{
			return World.worldToBlockPos(this.projectedLocation);
		}

		// Token: 0x0600ABA6 RID: 43942 RVA: 0x004379C4 File Offset: 0x00435BC4
		public string toString()
		{
			return string.Concat(new string[]
			{
				this.x.ToString(),
				", ",
				this.y.ToString(),
				", ",
				this.z.ToString()
			});
		}

		// Token: 0x040085BF RID: 34239
		public Vector3 projectedLocation;

		// Token: 0x040085C0 RID: 34240
		[PublicizedFrom(EAccessModifier.Private)]
		public int x;

		// Token: 0x040085C1 RID: 34241
		[PublicizedFrom(EAccessModifier.Private)]
		public int y;

		// Token: 0x040085C2 RID: 34242
		[PublicizedFrom(EAccessModifier.Private)]
		public int z;

		// Token: 0x040085C3 RID: 34243
		[PublicizedFrom(EAccessModifier.Private)]
		public int hash;
	}
}
