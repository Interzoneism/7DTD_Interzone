using System;
using System.Collections.Generic;
using UnityEngine;

namespace UAI
{
	// Token: 0x020014AE RID: 5294
	public static class UAIUtils
	{
		// Token: 0x0600A36B RID: 41835 RVA: 0x00410CD4 File Offset: 0x0040EED4
		public static float DistanceSqr(Vector3 pointA, Vector3 pointB)
		{
			Vector3 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		// Token: 0x0600A36C RID: 41836 RVA: 0x00410D14 File Offset: 0x0040EF14
		public static float DistanceSqr(Vector2 pointA, Vector2 pointB)
		{
			Vector2 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y;
		}

		// Token: 0x0600A36D RID: 41837 RVA: 0x00410D44 File Offset: 0x0040EF44
		public static EntityAlive ConvertToEntityAlive(object obj)
		{
			EntityAlive result = null;
			try
			{
				result = (EntityAlive)obj;
			}
			catch
			{
			}
			return result;
		}

		// Token: 0x020014AF RID: 5295
		public class NearestWaypointSorter : IComparer<Vector3>
		{
			// Token: 0x0600A36E RID: 41838 RVA: 0x00410D70 File Offset: 0x0040EF70
			public NearestWaypointSorter(Entity _self)
			{
				this.self = _self;
			}

			// Token: 0x0600A36F RID: 41839 RVA: 0x00410D80 File Offset: 0x0040EF80
			public int Compare(Vector3 _obj1, Vector3 _obj2)
			{
				float distanceSq = this.self.GetDistanceSq(_obj1);
				float distanceSq2 = this.self.GetDistanceSq(_obj2);
				if (distanceSq < distanceSq2)
				{
					return -1;
				}
				if (distanceSq > distanceSq2)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x04007E85 RID: 32389
			[PublicizedFrom(EAccessModifier.Private)]
			public Entity self;
		}

		// Token: 0x020014B0 RID: 5296
		public class NearestEntitySorter : IComparer<Entity>
		{
			// Token: 0x0600A370 RID: 41840 RVA: 0x00410DB4 File Offset: 0x0040EFB4
			public NearestEntitySorter(Entity _self)
			{
				this.self = _self;
			}

			// Token: 0x0600A371 RID: 41841 RVA: 0x00410DC4 File Offset: 0x0040EFC4
			public int Compare(Entity _obj1, Entity _obj2)
			{
				float distanceSq = this.self.GetDistanceSq(_obj1);
				float distanceSq2 = this.self.GetDistanceSq(_obj2);
				if (distanceSq < distanceSq2)
				{
					return -1;
				}
				if (distanceSq > distanceSq2)
				{
					return 1;
				}
				return 0;
			}

			// Token: 0x04007E86 RID: 32390
			[PublicizedFrom(EAccessModifier.Private)]
			public Entity self;
		}
	}
}
