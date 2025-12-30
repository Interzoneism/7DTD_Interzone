using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001974 RID: 6516
	public struct CharacterTransientGroundingReport
	{
		// Token: 0x0600BF7A RID: 49018 RVA: 0x00489C30 File Offset: 0x00487E30
		public void CopyFrom(CharacterGroundingReport groundingReport)
		{
			this.FoundAnyGround = groundingReport.FoundAnyGround;
			this.IsStableOnGround = groundingReport.IsStableOnGround;
			this.SnappingPrevented = groundingReport.SnappingPrevented;
			this.GroundNormal = groundingReport.GroundNormal;
			this.InnerGroundNormal = groundingReport.InnerGroundNormal;
			this.OuterGroundNormal = groundingReport.OuterGroundNormal;
		}

		// Token: 0x04009589 RID: 38281
		public bool FoundAnyGround;

		// Token: 0x0400958A RID: 38282
		public bool IsStableOnGround;

		// Token: 0x0400958B RID: 38283
		public bool SnappingPrevented;

		// Token: 0x0400958C RID: 38284
		public Vector3 GroundNormal;

		// Token: 0x0400958D RID: 38285
		public Vector3 InnerGroundNormal;

		// Token: 0x0400958E RID: 38286
		public Vector3 OuterGroundNormal;
	}
}
