using System;
using KinematicCharacterController;
using UnityEngine;

// Token: 0x02000487 RID: 1159
public class CC : ICharacterController
{
	// Token: 0x060025C5 RID: 9669 RVA: 0x000F3AE4 File Offset: 0x000F1CE4
	public void Move()
	{
		this.collisionFlags = CollisionFlags.None;
		this.hadWallOverlap = false;
		this.tickCount++;
		this.motor.UpdatePhase1(0.05f);
		this.motor.UpdatePhase2(0.05f);
		this.motor.Transform.SetPositionAndRotation(this.motor.TransientPosition, this.motor.TransientRotation);
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x00002914 File Offset: 0x00000B14
	public void BeforeCharacterUpdate(float deltaTime)
	{
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000F3B54 File Offset: 0x000F1D54
	public bool OnCollisionOverlap(int nbOverlaps, Collider[] _colliders)
	{
		Vector3 position = this.motor.Transform.position;
		bool flag;
		do
		{
			flag = false;
			for (int i = 0; i < nbOverlaps - 1; i++)
			{
				Collider collider = _colliders[i];
				Collider collider2 = _colliders[i + 1];
				if (collider.gameObject.layer == 15)
				{
					if (collider2.gameObject.layer != 15)
					{
						_colliders[i] = collider2;
						_colliders[i + 1] = collider;
						flag = true;
					}
					else
					{
						float sqrMagnitude = (collider.transform.position - position).sqrMagnitude;
						if ((collider2.transform.position - position).sqrMagnitude < sqrMagnitude)
						{
							_colliders[i] = collider2;
							_colliders[i + 1] = collider;
							flag = true;
						}
					}
				}
			}
		}
		while (flag);
		if (_colliders[0].gameObject.layer != 15)
		{
			this.hadWallOverlap = true;
		}
		else if (this.hadWallOverlap)
		{
			return false;
		}
		return true;
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x000F3C34 File Offset: 0x000F1E34
	public float GetCollisionOverlapScale(Transform overlappedTransform)
	{
		if (overlappedTransform.gameObject.layer != 15)
		{
			return 1f;
		}
		if ((this.entity.entityId + this.tickCount & 15) != 0)
		{
			return 0.1f;
		}
		return 0.5f;
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x00002914 File Offset: 0x00000B14
	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000F3C70 File Offset: 0x000F1E70
	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{
		if (this.vel.y <= 0.001f)
		{
			if (this.motor.GroundingStatus.IsStableOnGround)
			{
				Vector3 groundNormal = this.motor.GroundingStatus.GroundNormal;
				this.vel.y = 0f;
				float magnitude = this.vel.magnitude;
				Vector3 rhs = Vector3.Cross(this.vel, this.motor.CharacterUp);
				this.vel = Vector3.Cross(groundNormal, rhs).normalized * magnitude;
				this.vel = this.vel * 0.5f + currentVelocity * 0.5f;
			}
			else
			{
				this.vel = this.vel * 0.5f + currentVelocity * 0.5f;
			}
		}
		currentVelocity = this.vel;
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x00002914 File Offset: 0x00000B14
	public void AfterCharacterUpdate(float deltaTime)
	{
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool IsColliderValidForCollisions(Collider coll)
	{
		return true;
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000F3D6A File Offset: 0x000F1F6A
	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		if (hitNormal.y >= 0.64f)
		{
			this.collisionFlags |= CollisionFlags.Below;
			return;
		}
		if (hitNormal.y > -0.5f)
		{
			this.collisionFlags |= CollisionFlags.Sides;
		}
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x00002914 File Offset: 0x00000B14
	public void PostGroundingUpdate(float deltaTime)
	{
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x00002914 File Offset: 0x00000B14
	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
	}

	// Token: 0x04001CAE RID: 7342
	public Entity entity;

	// Token: 0x04001CAF RID: 7343
	public KinematicCharacterMotor motor;

	// Token: 0x04001CB0 RID: 7344
	public CollisionFlags collisionFlags;

	// Token: 0x04001CB1 RID: 7345
	public Vector3 vel;

	// Token: 0x04001CB2 RID: 7346
	[PublicizedFrom(EAccessModifier.Private)]
	public int tickCount;

	// Token: 0x04001CB3 RID: 7347
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hadWallOverlap;
}
