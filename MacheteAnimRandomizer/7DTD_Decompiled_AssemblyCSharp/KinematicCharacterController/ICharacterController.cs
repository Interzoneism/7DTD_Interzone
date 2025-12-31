using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x0200196C RID: 6508
	public interface ICharacterController
	{
		// Token: 0x0600BF6B RID: 49003
		void UpdateRotation(ref Quaternion currentRotation, float deltaTime);

		// Token: 0x0600BF6C RID: 49004
		void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);

		// Token: 0x0600BF6D RID: 49005
		void BeforeCharacterUpdate(float deltaTime);

		// Token: 0x0600BF6E RID: 49006
		void PostGroundingUpdate(float deltaTime);

		// Token: 0x0600BF6F RID: 49007
		void AfterCharacterUpdate(float deltaTime);

		// Token: 0x0600BF70 RID: 49008
		bool IsColliderValidForCollisions(Collider coll);

		// Token: 0x0600BF71 RID: 49009
		void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600BF72 RID: 49010
		void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600BF73 RID: 49011
		void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600BF74 RID: 49012
		void OnDiscreteCollisionDetected(Collider hitCollider);

		// Token: 0x0600BF75 RID: 49013
		bool OnCollisionOverlap(int nbOverlaps, Collider[] _internalProbedColliders);

		// Token: 0x0600BF76 RID: 49014
		float GetCollisionOverlapScale(Transform overlappedTransform);
	}
}
