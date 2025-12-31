using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001975 RID: 6517
	public struct HitStabilityReport
	{
		// Token: 0x0400958F RID: 38287
		public bool IsStable;

		// Token: 0x04009590 RID: 38288
		public Vector3 InnerNormal;

		// Token: 0x04009591 RID: 38289
		public Vector3 OuterNormal;

		// Token: 0x04009592 RID: 38290
		public bool ValidStepDetected;

		// Token: 0x04009593 RID: 38291
		public Collider SteppedCollider;

		// Token: 0x04009594 RID: 38292
		public bool LedgeDetected;

		// Token: 0x04009595 RID: 38293
		public bool IsOnEmptySideOfLedge;

		// Token: 0x04009596 RID: 38294
		public float DistanceFromLedge;

		// Token: 0x04009597 RID: 38295
		public bool IsMovingTowardsEmptySideOfLedge;

		// Token: 0x04009598 RID: 38296
		public Vector3 LedgeGroundNormal;

		// Token: 0x04009599 RID: 38297
		public Vector3 LedgeRightDirection;

		// Token: 0x0400959A RID: 38298
		public Vector3 LedgeFacingDirection;
	}
}
