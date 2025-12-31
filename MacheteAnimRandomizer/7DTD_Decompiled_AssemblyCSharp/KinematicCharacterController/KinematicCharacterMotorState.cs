using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001971 RID: 6513
	[Serializable]
	public struct KinematicCharacterMotorState
	{
		// Token: 0x04009576 RID: 38262
		public Vector3 Position;

		// Token: 0x04009577 RID: 38263
		public Quaternion Rotation;

		// Token: 0x04009578 RID: 38264
		public Vector3 BaseVelocity;

		// Token: 0x04009579 RID: 38265
		public bool MustUnground;

		// Token: 0x0400957A RID: 38266
		public float MustUngroundTime;

		// Token: 0x0400957B RID: 38267
		public bool LastMovementIterationFoundAnyGround;

		// Token: 0x0400957C RID: 38268
		public CharacterTransientGroundingReport GroundingStatus;

		// Token: 0x0400957D RID: 38269
		public Rigidbody AttachedRigidbody;

		// Token: 0x0400957E RID: 38270
		public Vector3 AttachedRigidbodyVelocity;
	}
}
