using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001976 RID: 6518
	public struct RigidbodyProjectionHit
	{
		// Token: 0x0400959B RID: 38299
		public Rigidbody Rigidbody;

		// Token: 0x0400959C RID: 38300
		public Vector3 HitPoint;

		// Token: 0x0400959D RID: 38301
		public Vector3 EffectiveHitNormal;

		// Token: 0x0400959E RID: 38302
		public Vector3 HitVelocity;

		// Token: 0x0400959F RID: 38303
		public bool StableOnHit;
	}
}
