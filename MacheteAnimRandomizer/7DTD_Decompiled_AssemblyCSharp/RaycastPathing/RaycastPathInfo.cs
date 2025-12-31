using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015BA RID: 5562
	[Preserve]
	public class RaycastPathInfo
	{
		// Token: 0x0600AAD3 RID: 43731 RVA: 0x00434578 File Offset: 0x00432778
		public RaycastPathInfo(Vector3 start, Vector3 target)
		{
			this.Start = start;
			this.Target = target;
			this.StartNode = new RaycastNode(this.Start, 1f, 0);
			this.TargetNode = new RaycastNode(this.Target, 1f, 0);
			this.PathStartsIndoors = RaycastPathWorldUtils.IsUnderground(start);
			this.PathEndsIndoors = RaycastPathWorldUtils.IsUnderground(target);
		}

		// Token: 0x17001317 RID: 4887
		// (get) Token: 0x0600AAD4 RID: 43732 RVA: 0x004345DF File Offset: 0x004327DF
		public Vector3i StartBlockPos
		{
			get
			{
				return this.StartNode.BlockPos;
			}
		}

		// Token: 0x17001318 RID: 4888
		// (get) Token: 0x0600AAD5 RID: 43733 RVA: 0x004345EC File Offset: 0x004327EC
		public Vector3i TargetBlockPos
		{
			get
			{
				return this.TargetNode.BlockPos;
			}
		}

		// Token: 0x17001319 RID: 4889
		// (get) Token: 0x0600AAD6 RID: 43734 RVA: 0x004345F9 File Offset: 0x004327F9
		// (set) Token: 0x0600AAD7 RID: 43735 RVA: 0x00434601 File Offset: 0x00432801
		public bool PathStartsIndoors { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700131A RID: 4890
		// (get) Token: 0x0600AAD8 RID: 43736 RVA: 0x0043460A File Offset: 0x0043280A
		// (set) Token: 0x0600AAD9 RID: 43737 RVA: 0x00434612 File Offset: 0x00432812
		public bool PathEndsIndoors { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600AADA RID: 43738 RVA: 0x00180C18 File Offset: 0x0017EE18
		public static implicit operator bool(RaycastPathInfo exists)
		{
			return exists != null;
		}

		// Token: 0x0400855A RID: 34138
		public readonly Vector3 Start;

		// Token: 0x0400855B RID: 34139
		public readonly Vector3 Target;

		// Token: 0x0400855C RID: 34140
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNode StartNode;

		// Token: 0x0400855D RID: 34141
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNode TargetNode;
	}
}
