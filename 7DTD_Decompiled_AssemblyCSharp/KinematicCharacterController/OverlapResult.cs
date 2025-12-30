using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02001972 RID: 6514
	public struct OverlapResult
	{
		// Token: 0x0600BF78 RID: 49016 RVA: 0x00489BB7 File Offset: 0x00487DB7
		public OverlapResult(Vector3 normal, Collider collider)
		{
			this.Normal = normal;
			this.Collider = collider;
		}

		// Token: 0x0400957F RID: 38271
		public Vector3 Normal;

		// Token: 0x04009580 RID: 38272
		public Collider Collider;
	}
}
