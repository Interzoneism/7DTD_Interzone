using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015C2 RID: 5570
	[Preserve]
	public class FloodFillNode : RaycastNode
	{
		// Token: 0x0600AB03 RID: 43779 RVA: 0x00434AF7 File Offset: 0x00432CF7
		public FloodFillNode(Vector3 pos, float scale = 1f, int depth = 0) : base(pos, scale, depth)
		{
			this.score = new FloodFillNodeScore();
		}

		// Token: 0x0600AB04 RID: 43780 RVA: 0x00434B0D File Offset: 0x00432D0D
		public FloodFillNode(Vector3 min, Vector3 max, float scale = 1f, int depth = 0) : base(min, max, scale, depth)
		{
			this.score = new FloodFillNodeScore();
		}

		// Token: 0x17001329 RID: 4905
		// (get) Token: 0x0600AB05 RID: 43781 RVA: 0x00434B25 File Offset: 0x00432D25
		// (set) Token: 0x0600AB06 RID: 43782 RVA: 0x00434B32 File Offset: 0x00432D32
		public float G
		{
			get
			{
				return this.score.G;
			}
			set
			{
				this.score.G = value;
			}
		}

		// Token: 0x1700132A RID: 4906
		// (get) Token: 0x0600AB07 RID: 43783 RVA: 0x00434B40 File Offset: 0x00432D40
		// (set) Token: 0x0600AB08 RID: 43784 RVA: 0x00434B4D File Offset: 0x00432D4D
		public float Heuristic
		{
			get
			{
				return this.score.H;
			}
			set
			{
				this.score.H = value;
			}
		}

		// Token: 0x1700132B RID: 4907
		// (get) Token: 0x0600AB09 RID: 43785 RVA: 0x00434B5B File Offset: 0x00432D5B
		public float F
		{
			get
			{
				return this.score.F;
			}
		}

		// Token: 0x0400857B RID: 34171
		[PublicizedFrom(EAccessModifier.Private)]
		public FloodFillNodeScore score;
	}
}
