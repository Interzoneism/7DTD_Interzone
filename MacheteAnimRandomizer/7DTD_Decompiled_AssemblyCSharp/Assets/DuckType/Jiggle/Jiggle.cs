using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DuckType.Jiggle
{
	// Token: 0x0200199B RID: 6555
	public class Jiggle : MonoBehaviour
	{
		// Token: 0x0600C0EB RID: 49387 RVA: 0x0049079C File Offset: 0x0048E99C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDrawGizmos()
		{
			if (!this.ShowViewportGizmos)
			{
				return;
			}
			Vector3 vector = base.transform.localToWorldMatrix.MultiplyPoint3x4(this.CenterOfMass);
			Gizmos.color = Color.green;
			if (this.UseCenterOfMass)
			{
				Gizmos.DrawSphere(vector, this.CenterOfMassInertia * 5f * this.GizmoScale);
				Gizmos.DrawLine(base.transform.position, vector);
			}
			if (this.Hinge)
			{
				this.DrawGizmosArc(base.transform.position, base.transform.position + this.GetRestRotationWorld() * this.CenterOfMass * 11f * this.GizmoScale, this.GetHingeNormalWorld(), 360f);
			}
		}

		// Token: 0x0600C0EC RID: 49388 RVA: 0x00490864 File Offset: 0x0048EA64
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDrawGizmosSelected()
		{
			if (!this.ShowViewportGizmos)
			{
				return;
			}
			if (this.UseAngleLimit & this.AngleLimit > 0f)
			{
				float d = 10f * this.GizmoScale;
				Vector3 vector = this.GetRestRotationWorld() * this.CenterOfMass;
				List<Vector3> list;
				if (this.Hinge)
				{
					Vector3 vector2 = Vector3.Cross(vector, this.GetHingeNormalWorld());
					list = new List<Vector3>
					{
						vector2,
						-vector2
					};
				}
				else
				{
					list = vector.GetOrthogonalVectors(12);
				}
				foreach (Vector3 current in list)
				{
					Gizmos.color = Color.red;
					Vector3 vector3 = (this.AngleLimit < 90f) ? Vector3.RotateTowards(current, base.transform.rotation * this.CenterOfMass, (90f - this.AngleLimit) * 0.017453292f, 1f) : Vector3.RotateTowards(current, base.transform.rotation * -this.CenterOfMass, (this.AngleLimit - 90f) * 0.017453292f, 1f);
					vector3 *= d;
					Gizmos.DrawRay(base.transform.position, vector3);
					if (this.UseSoftLimit)
					{
						Gizmos.color = Color.yellow;
						Vector3 startPoint = base.transform.position + vector3;
						this.DrawGizmosArc(base.transform.position, startPoint, Vector3.Cross(vector3, vector), this.AngleLimit * this.SoftLimitInfluence);
					}
				}
			}
		}

		// Token: 0x0600C0ED RID: 49389 RVA: 0x00490A2C File Offset: 0x0048EC2C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Start()
		{
			this.m_Initialised = true;
			this.m_RestLocalRotation = base.transform.localRotation;
			this.m_LastWorldRotation = base.transform.rotation;
			this.m_LastCenterOfMassWorld = base.transform.localToWorldMatrix.MultiplyPoint3x4(this.CenterOfMass);
		}

		// Token: 0x0600C0EE RID: 49390 RVA: 0x00490A81 File Offset: 0x0048EC81
		[PublicizedFrom(EAccessModifier.Private)]
		public void LateUpdate()
		{
			JiggleScheduler.Update(this);
		}

		// Token: 0x0600C0EF RID: 49391 RVA: 0x00490A89 File Offset: 0x0048EC89
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			JiggleScheduler.Register(this);
		}

		// Token: 0x0600C0F0 RID: 49392 RVA: 0x00490A91 File Offset: 0x0048EC91
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			JiggleScheduler.Deregister(this);
		}

		// Token: 0x0600C0F1 RID: 49393 RVA: 0x00490A9C File Offset: 0x0048EC9C
		public void ScheduledUpdate(float deltaTime)
		{
			Quaternion quaternion;
			if (this.RotationInertia > 0f)
			{
				quaternion = Quaternion.SlerpUnclamped(base.transform.rotation, this.m_LastWorldRotation, this.RotationInertia);
			}
			else
			{
				quaternion = base.transform.rotation;
			}
			if (this.UseCenterOfMass && this.CenterOfMassInertia > 0f)
			{
				Quaternion source = Quaternion.FromToRotation(quaternion * this.CenterOfMass, this.m_LastCenterOfMassWorld - base.transform.position);
				Debug.DrawLine(this.m_LastCenterOfMassWorld, base.transform.position);
				quaternion = source.Scale(this.CenterOfMassInertia) * quaternion;
			}
			quaternion *= this.m_Torque.Scale(deltaTime / 0.001f);
			Quaternion quaternion2 = ((base.transform.parent != null) ? base.transform.parent.rotation : Quaternion.identity) * this.m_RestLocalRotation;
			if (this.BlendToOriginalRotation)
			{
				quaternion2 = base.transform.rotation;
			}
			float num = Quaternion.Angle(quaternion, quaternion2);
			if (this.UseAngleLimit && num > this.AngleLimit)
			{
				quaternion = Quaternion.Slerp(quaternion2, quaternion, this.AngleLimit / num);
			}
			if (this.Hinge)
			{
				Vector3 vector = quaternion * this.CenterOfMass;
				Vector3 hingeNormalWorld = this.GetHingeNormalWorld();
				Vector3 toDirection = Vector3.Cross(hingeNormalWorld, Vector3.Cross(vector, hingeNormalWorld));
				quaternion = Quaternion.FromToRotation(vector, toDirection) * quaternion;
			}
			base.transform.rotation = quaternion;
			if (this.SpringStrength > 0f)
			{
				Quaternion quaternion3 = base.transform.rotation.FromToRotation(quaternion2);
				quaternion3 = quaternion3.Scale(0.001f * this.SpringStrength * 250f * deltaTime);
				this.m_Torque = this.m_Torque.Append(quaternion3);
			}
			if (this.UseCenterOfMass)
			{
				if (this.Gravity > 0f)
				{
					Quaternion closestRotationFromTo = this.GetClosestRotationFromTo(base.transform.rotation * this.CenterOfMass, Vector3.down);
					this.m_Torque = this.m_Torque.Append(closestRotationFromTo.Scale(0.001f * this.Gravity * 50f * deltaTime));
				}
				if (this.AddWind)
				{
					Quaternion closestRotationFromTo2 = this.GetClosestRotationFromTo(base.transform.rotation * this.CenterOfMass, this.WindDirection);
					this.m_Torque = this.m_Torque.Append(closestRotationFromTo2.Scale(0.001f * this.WindStrength * 50f * deltaTime));
				}
				if (this.AddNoise)
				{
					Vector3 noiseVector = this.GetNoiseVector(base.transform.localToWorldMatrix.MultiplyPoint3x4(this.CenterOfMass), this.NoiseScale * 10f, this.m_NoisePhase += deltaTime * this.NoiseSpeed);
					Quaternion closestRotationFromTo3 = this.GetClosestRotationFromTo(base.transform.rotation * this.CenterOfMass, noiseVector);
					this.m_Torque = this.m_Torque.Append(closestRotationFromTo3.Scale(0.001f * this.NoiseStrength * 50f * deltaTime));
				}
			}
			if (this.UseSoftLimit && this.UseAngleLimit && this.AngleLimit > 0f && this.SoftLimitStrength > 0f)
			{
				num = Quaternion.Angle(quaternion, quaternion2);
				float num2 = this.AngleLimit * (1f - this.SoftLimitInfluence);
				if (num > num2)
				{
					float num3 = Mathf.Min((num - num2) / (this.AngleLimit - num2), 1f);
					Quaternion source2 = base.transform.rotation.FromToRotation(quaternion2);
					this.m_Torque = this.m_Torque.Append(source2.Scale(0.001f * num3 * this.SoftLimitStrength * 250f * deltaTime));
				}
			}
			this.m_Torque = this.m_Torque.Scale((1f - this.Dampening * 10f * deltaTime).Clamp01());
			this.m_LastCenterOfMassWorld = base.transform.localToWorldMatrix.MultiplyPoint3x4(this.CenterOfMass);
			this.m_LastWorldRotation = base.transform.rotation;
		}

		// Token: 0x0600C0F2 RID: 49394 RVA: 0x00490ED4 File Offset: 0x0048F0D4
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 GetHingeNormalWorld()
		{
			Vector3 point = (Mathf.Abs(this.CenterOfMass.normalized.y) != 1f) ? (Quaternion.AngleAxis(this.HingeAngle, this.CenterOfMass) * Vector3.Cross(this.CenterOfMass, Vector3.up)) : (Quaternion.AngleAxis(this.HingeAngle, this.CenterOfMass) * Vector3.Cross(this.CenterOfMass, Vector3.right));
			return this.GetRestRotationWorld() * point;
		}

		// Token: 0x0600C0F3 RID: 49395 RVA: 0x00490F58 File Offset: 0x0048F158
		[PublicizedFrom(EAccessModifier.Private)]
		public Quaternion GetRestRotationWorld()
		{
			if (!this.m_Initialised)
			{
				return base.transform.rotation;
			}
			if (!(base.transform.parent != null))
			{
				return this.m_RestLocalRotation;
			}
			return base.transform.parent.rotation * this.m_RestLocalRotation;
		}

		// Token: 0x0600C0F4 RID: 49396 RVA: 0x00490FB0 File Offset: 0x0048F1B0
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 GetNoiseVector(Vector3 pos, float scale, float phase)
		{
			pos /= scale;
			return new Vector3(Mathf.PerlinNoise(pos.x, pos.y + phase) - 0.5f, Mathf.PerlinNoise(pos.y, pos.z + phase) - 0.5f, Mathf.PerlinNoise(pos.z, pos.x + phase) - 0.5f);
		}

		// Token: 0x0600C0F5 RID: 49397 RVA: 0x00491018 File Offset: 0x0048F218
		[PublicizedFrom(EAccessModifier.Private)]
		public Quaternion GetClosestRotationFromTo(Vector3 from, Vector3 to)
		{
			Quaternion target = Quaternion.FromToRotation(from, to) * base.transform.rotation;
			return base.transform.rotation.FromToRotation(target);
		}

		// Token: 0x0600C0F6 RID: 49398 RVA: 0x00491050 File Offset: 0x0048F250
		[PublicizedFrom(EAccessModifier.Private)]
		public void DrawGizmosArc(Vector3 center, Vector3 startPoint, Vector3 normal, float degrees)
		{
			int num = (int)Mathf.Ceil(degrees / 20f);
			Quaternion rotation = Quaternion.AngleAxis(degrees / (float)num, normal);
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = rotation * (startPoint - center) + center;
				Gizmos.DrawRay(startPoint, vector - startPoint);
				startPoint = vector;
			}
		}

		// Token: 0x04009657 RID: 38487
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const float TORQUE_FACTOR = 0.001f;

		// Token: 0x04009658 RID: 38488
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool m_Initialised;

		// Token: 0x04009659 RID: 38489
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion m_RestLocalRotation;

		// Token: 0x0400965A RID: 38490
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion m_LastWorldRotation;

		// Token: 0x0400965B RID: 38491
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion m_Torque = Quaternion.identity;

		// Token: 0x0400965C RID: 38492
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 m_LastCenterOfMassWorld;

		// Token: 0x0400965D RID: 38493
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public float m_NoisePhase;

		// Token: 0x0400965E RID: 38494
		public bool UpdateWithPhysics;

		// Token: 0x0400965F RID: 38495
		public bool UseCenterOfMass = true;

		// Token: 0x04009660 RID: 38496
		public Vector3 CenterOfMass = new Vector3(1f, 0f, 0f);

		// Token: 0x04009661 RID: 38497
		public float CenterOfMassInertia = 1f;

		// Token: 0x04009662 RID: 38498
		public bool AddWind;

		// Token: 0x04009663 RID: 38499
		public Vector3 WindDirection = new Vector3(1f, 0f, 0f);

		// Token: 0x04009664 RID: 38500
		public float WindStrength = 1f;

		// Token: 0x04009665 RID: 38501
		public bool AddNoise;

		// Token: 0x04009666 RID: 38502
		public float NoiseStrength = 1f;

		// Token: 0x04009667 RID: 38503
		public float NoiseScale = 1f;

		// Token: 0x04009668 RID: 38504
		public float NoiseSpeed = 1f;

		// Token: 0x04009669 RID: 38505
		public float RotationInertia = 1f;

		// Token: 0x0400966A RID: 38506
		public float Gravity;

		// Token: 0x0400966B RID: 38507
		public float SpringStrength = 0.4f;

		// Token: 0x0400966C RID: 38508
		public float Dampening = 0.4f;

		// Token: 0x0400966D RID: 38509
		public bool BlendToOriginalRotation;

		// Token: 0x0400966E RID: 38510
		public bool Hinge;

		// Token: 0x0400966F RID: 38511
		public float HingeAngle;

		// Token: 0x04009670 RID: 38512
		public bool UseAngleLimit;

		// Token: 0x04009671 RID: 38513
		public float AngleLimit = 180f;

		// Token: 0x04009672 RID: 38514
		public bool UseSoftLimit;

		// Token: 0x04009673 RID: 38515
		public float SoftLimitInfluence = 0.5f;

		// Token: 0x04009674 RID: 38516
		public float SoftLimitStrength = 0.5f;

		// Token: 0x04009675 RID: 38517
		public bool ShowViewportGizmos = true;

		// Token: 0x04009676 RID: 38518
		public float GizmoScale = 0.1f;
	}
}
