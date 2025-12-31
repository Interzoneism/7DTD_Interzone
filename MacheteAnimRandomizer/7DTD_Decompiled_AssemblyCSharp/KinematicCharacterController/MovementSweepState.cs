using System;

namespace KinematicCharacterController
{
	// Token: 0x02001970 RID: 6512
	public enum MovementSweepState
	{
		// Token: 0x04009572 RID: 38258
		Initial,
		// Token: 0x04009573 RID: 38259
		AfterFirstHit,
		// Token: 0x04009574 RID: 38260
		FoundBlockingCrease,
		// Token: 0x04009575 RID: 38261
		FoundBlockingCorner
	}
}
