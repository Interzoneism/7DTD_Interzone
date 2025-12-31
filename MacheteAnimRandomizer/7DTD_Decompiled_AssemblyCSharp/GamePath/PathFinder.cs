using System;
using UnityEngine;

namespace GamePath
{
	// Token: 0x020015D1 RID: 5585
	public class PathFinder
	{
		// Token: 0x0600AB7C RID: 43900 RVA: 0x00437664 File Offset: 0x00435864
		public PathFinder(PathInfo _pathInfo, bool _bDrn, bool _canClimbLadders, bool _bCanClimbWalls)
		{
			this.pathInfo = _pathInfo;
			this.canEntityDrown = _bDrn;
			this.canClimbWalls = _bCanClimbWalls;
			this.canClimbLadders = _canClimbLadders;
		}

		// Token: 0x0600AB7D RID: 43901 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Calculate(Vector3 _fromPos, Vector3 _toPos)
		{
		}

		// Token: 0x0600AB7E RID: 43902 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Destruct()
		{
		}

		// Token: 0x040085A0 RID: 34208
		[PublicizedFrom(EAccessModifier.Protected)]
		public PathInfo pathInfo;

		// Token: 0x040085A1 RID: 34209
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canClimbWalls;

		// Token: 0x040085A2 RID: 34210
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canClimbLadders;

		// Token: 0x040085A3 RID: 34211
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool canEntityDrown;
	}
}
