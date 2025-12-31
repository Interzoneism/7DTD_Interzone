using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001979 RID: 6521
	[Serializable]
	public struct PhysicsMoverState
	{
		// Token: 0x04009606 RID: 38406
		public Vector3 Position;

		// Token: 0x04009607 RID: 38407
		public Quaternion Rotation;

		// Token: 0x04009608 RID: 38408
		public Vector3 Velocity;

		// Token: 0x04009609 RID: 38409
		public Vector3 AngularVelocity;
	}
}
