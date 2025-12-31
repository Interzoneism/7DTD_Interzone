using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015D0 RID: 5584
	public class PathEntity
	{
		// Token: 0x0600AB6B RID: 43883 RVA: 0x0043745C File Offset: 0x0043565C
		public void Destruct()
		{
			PathPoint[] array = this.points;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Release();
			}
			this.points = null;
		}

		// Token: 0x0600AB6C RID: 43884 RVA: 0x0043748D File Offset: 0x0043568D
		public void SetPoints(PathPoint[] _points)
		{
			this.points = _points;
			this.pathLength = _points.Length;
		}

		// Token: 0x0600AB6D RID: 43885 RVA: 0x0043749F File Offset: 0x0043569F
		public bool HasPoints()
		{
			return this.points != null;
		}

		// Token: 0x0600AB6E RID: 43886 RVA: 0x004374AA File Offset: 0x004356AA
		public bool isFinished()
		{
			return this.currentPathIndex >= this.pathLength;
		}

		// Token: 0x0600AB6F RID: 43887 RVA: 0x004374BD File Offset: 0x004356BD
		public int NodeCountRemaining()
		{
			return this.pathLength - this.currentPathIndex;
		}

		// Token: 0x0600AB70 RID: 43888 RVA: 0x004374CC File Offset: 0x004356CC
		public Vector3 GetEndPos()
		{
			if (this.pathLength > 0)
			{
				return this.points[this.pathLength - 1].projectedLocation;
			}
			return this.rawEndPos;
		}

		// Token: 0x0600AB71 RID: 43889 RVA: 0x004374F2 File Offset: 0x004356F2
		public PathPoint GetEndPoint()
		{
			if (this.pathLength > 0)
			{
				return this.points[this.pathLength - 1];
			}
			return null;
		}

		// Token: 0x0600AB72 RID: 43890 RVA: 0x00437510 File Offset: 0x00435710
		public void ShortenEnd(float _distance)
		{
			if (this.pathLength >= 2)
			{
				PathPoint pathPoint = this.points[this.pathLength - 2];
				PathPoint pathPoint2 = this.points[this.pathLength - 1];
				pathPoint2.projectedLocation = Vector3.MoveTowards(pathPoint2.projectedLocation, pathPoint.projectedLocation, _distance);
			}
		}

		// Token: 0x0600AB73 RID: 43891 RVA: 0x0043755C File Offset: 0x0043575C
		public PathPoint getPathPointFromIndex(int _idx)
		{
			return this.points[_idx];
		}

		// Token: 0x0600AB74 RID: 43892 RVA: 0x00437566 File Offset: 0x00435766
		public int getCurrentPathLength()
		{
			return this.pathLength;
		}

		// Token: 0x0600AB75 RID: 43893 RVA: 0x0043756E File Offset: 0x0043576E
		public void setCurrentPathLength(int _length)
		{
			this.pathLength = _length;
		}

		// Token: 0x0600AB76 RID: 43894 RVA: 0x00437577 File Offset: 0x00435777
		public int getCurrentPathIndex()
		{
			return this.currentPathIndex;
		}

		// Token: 0x0600AB77 RID: 43895 RVA: 0x0043757F File Offset: 0x0043577F
		public void setCurrentPathIndex(int _idx, Entity entity, Vector3 entityPos)
		{
			this.currentPathIndex = _idx;
		}

		// Token: 0x1700132F RID: 4911
		// (get) Token: 0x0600AB78 RID: 43896 RVA: 0x00437588 File Offset: 0x00435788
		public PathPoint CurrentPoint
		{
			get
			{
				return this.points[this.currentPathIndex];
			}
		}

		// Token: 0x17001330 RID: 4912
		// (get) Token: 0x0600AB79 RID: 43897 RVA: 0x00437598 File Offset: 0x00435798
		public PathPoint NextPoint
		{
			get
			{
				int num = this.currentPathIndex + 1;
				if (num >= this.points.Length)
				{
					return null;
				}
				return this.points[num];
			}
		}

		// Token: 0x0600AB7A RID: 43898 RVA: 0x004375C4 File Offset: 0x004357C4
		public override bool Equals(object _other)
		{
			if (!(_other is PathEntity) || _other == null)
			{
				return false;
			}
			PathEntity pathEntity = (PathEntity)_other;
			if (pathEntity.points.Length != this.points.Length)
			{
				return false;
			}
			for (int i = 0; i < this.points.Length; i++)
			{
				if (!this.points[i].IsSamePos(pathEntity.points[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600AB7B RID: 43899 RVA: 0x00437628 File Offset: 0x00435828
		public override int GetHashCode()
		{
			if (this.points == null)
			{
				return 0;
			}
			int num = 0;
			foreach (PathPoint pathPoint in this.points)
			{
				num += pathPoint.GetHashCode();
			}
			return num;
		}

		// Token: 0x0400859B RID: 34203
		public PathPoint[] points;

		// Token: 0x0400859C RID: 34204
		public Vector3 toPos;

		// Token: 0x0400859D RID: 34205
		public Vector3 rawEndPos;

		// Token: 0x0400859E RID: 34206
		[PublicizedFrom(EAccessModifier.Private)]
		public int currentPathIndex;

		// Token: 0x0400859F RID: 34207
		[PublicizedFrom(EAccessModifier.Private)]
		public int pathLength;
	}
}
