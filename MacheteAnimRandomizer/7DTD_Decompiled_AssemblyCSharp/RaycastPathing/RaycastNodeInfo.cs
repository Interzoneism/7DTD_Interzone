using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015BD RID: 5565
	[Preserve]
	public class RaycastNodeInfo
	{
		// Token: 0x0600AAF2 RID: 43762 RVA: 0x0043476C File Offset: 0x0043296C
		public RaycastNodeInfo(Vector3 pos, float scale = 1f, int depth = 0)
		{
			this.Position = pos;
			this.BlockPos = World.worldToBlockPos(pos);
			this.Scale = scale;
			this.Depth = depth;
			this.Min = pos - Vector3.one * scale * 0.5f;
			this.Max = pos + Vector3.one * scale * 0.5f;
			this.Center = (this.Min + this.Max) * 0.5f;
		}

		// Token: 0x0600AAF3 RID: 43763 RVA: 0x00434804 File Offset: 0x00432A04
		public RaycastNodeInfo(Vector3 min, Vector3 max, float scale = 1f, int depth = 0)
		{
			this.Position = (min + max) * 0.5f;
			this.BlockPos = World.worldToBlockPos(this.Position);
			this.Scale = scale;
			this.Depth = depth;
			this.Min = min;
			this.Max = max;
			this.Center = this.Position;
		}

		// Token: 0x04008569 RID: 34153
		public readonly Vector3 Position;

		// Token: 0x0400856A RID: 34154
		public readonly Vector3i BlockPos;

		// Token: 0x0400856B RID: 34155
		public readonly float Scale;

		// Token: 0x0400856C RID: 34156
		public readonly int Depth;

		// Token: 0x0400856D RID: 34157
		public readonly Vector3 Min;

		// Token: 0x0400856E RID: 34158
		public readonly Vector3 Max;

		// Token: 0x0400856F RID: 34159
		public readonly Vector3 Center;
	}
}
