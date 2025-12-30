using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x0200196D RID: 6509
	public interface IMoverController
	{
		// Token: 0x0600BF77 RID: 49015
		void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime);
	}
}
