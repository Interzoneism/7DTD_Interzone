using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001973 RID: 6515
	public struct CharacterGroundingReport
	{
		// Token: 0x0600BF79 RID: 49017 RVA: 0x00489BC8 File Offset: 0x00487DC8
		public void CopyFrom(CharacterTransientGroundingReport transientGroundingReport)
		{
			this.FoundAnyGround = transientGroundingReport.FoundAnyGround;
			this.IsStableOnGround = transientGroundingReport.IsStableOnGround;
			this.SnappingPrevented = transientGroundingReport.SnappingPrevented;
			this.GroundNormal = transientGroundingReport.GroundNormal;
			this.InnerGroundNormal = transientGroundingReport.InnerGroundNormal;
			this.OuterGroundNormal = transientGroundingReport.OuterGroundNormal;
			this.GroundCollider = null;
			this.GroundPoint = Vector3.zero;
		}

		// Token: 0x04009581 RID: 38273
		public bool FoundAnyGround;

		// Token: 0x04009582 RID: 38274
		public bool IsStableOnGround;

		// Token: 0x04009583 RID: 38275
		public bool SnappingPrevented;

		// Token: 0x04009584 RID: 38276
		public Vector3 GroundNormal;

		// Token: 0x04009585 RID: 38277
		public Vector3 InnerGroundNormal;

		// Token: 0x04009586 RID: 38278
		public Vector3 OuterGroundNormal;

		// Token: 0x04009587 RID: 38279
		public Collider GroundCollider;

		// Token: 0x04009588 RID: 38280
		public Vector3 GroundPoint;
	}
}
