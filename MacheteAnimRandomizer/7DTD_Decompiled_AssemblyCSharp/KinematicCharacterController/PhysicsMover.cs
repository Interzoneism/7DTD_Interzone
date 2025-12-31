using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x0200197A RID: 6522
	[RequireComponent(typeof(Rigidbody))]
	public class PhysicsMover : MonoBehaviour
	{
		// Token: 0x17001605 RID: 5637
		// (get) Token: 0x0600BFD1 RID: 49105 RVA: 0x0048CD14 File Offset: 0x0048AF14
		// (set) Token: 0x0600BFD2 RID: 49106 RVA: 0x0048CD1C File Offset: 0x0048AF1C
		public int IndexInCharacterSystem { get; set; }

		// Token: 0x17001606 RID: 5638
		// (get) Token: 0x0600BFD3 RID: 49107 RVA: 0x0048CD25 File Offset: 0x0048AF25
		// (set) Token: 0x0600BFD4 RID: 49108 RVA: 0x0048CD2D File Offset: 0x0048AF2D
		public Vector3 InitialTickPosition { get; set; }

		// Token: 0x17001607 RID: 5639
		// (get) Token: 0x0600BFD5 RID: 49109 RVA: 0x0048CD36 File Offset: 0x0048AF36
		// (set) Token: 0x0600BFD6 RID: 49110 RVA: 0x0048CD3E File Offset: 0x0048AF3E
		public Quaternion InitialTickRotation { get; set; }

		// Token: 0x17001608 RID: 5640
		// (get) Token: 0x0600BFD7 RID: 49111 RVA: 0x0048CD47 File Offset: 0x0048AF47
		// (set) Token: 0x0600BFD8 RID: 49112 RVA: 0x0048CD4F File Offset: 0x0048AF4F
		public Transform Transform { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001609 RID: 5641
		// (get) Token: 0x0600BFD9 RID: 49113 RVA: 0x0048CD58 File Offset: 0x0048AF58
		// (set) Token: 0x0600BFDA RID: 49114 RVA: 0x0048CD60 File Offset: 0x0048AF60
		public Vector3 InitialSimulationPosition { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700160A RID: 5642
		// (get) Token: 0x0600BFDB RID: 49115 RVA: 0x0048CD69 File Offset: 0x0048AF69
		// (set) Token: 0x0600BFDC RID: 49116 RVA: 0x0048CD71 File Offset: 0x0048AF71
		public Quaternion InitialSimulationRotation { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700160B RID: 5643
		// (get) Token: 0x0600BFDD RID: 49117 RVA: 0x0048CD7A File Offset: 0x0048AF7A
		// (set) Token: 0x0600BFDE RID: 49118 RVA: 0x0048CD82 File Offset: 0x0048AF82
		public Vector3 TransientPosition
		{
			get
			{
				return this._internalTransientPosition;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._internalTransientPosition = value;
			}
		}

		// Token: 0x1700160C RID: 5644
		// (get) Token: 0x0600BFDF RID: 49119 RVA: 0x0048CD8B File Offset: 0x0048AF8B
		// (set) Token: 0x0600BFE0 RID: 49120 RVA: 0x0048CD93 File Offset: 0x0048AF93
		public Quaternion TransientRotation
		{
			get
			{
				return this._internalTransientRotation;
			}
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this._internalTransientRotation = value;
			}
		}

		// Token: 0x0600BFE1 RID: 49121 RVA: 0x0048CD9C File Offset: 0x0048AF9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Reset()
		{
			this.ValidateData();
		}

		// Token: 0x0600BFE2 RID: 49122 RVA: 0x0048CD9C File Offset: 0x0048AF9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnValidate()
		{
			this.ValidateData();
		}

		// Token: 0x0600BFE3 RID: 49123 RVA: 0x0048CDA4 File Offset: 0x0048AFA4
		public void ValidateData()
		{
			this.Rigidbody = base.gameObject.GetComponent<Rigidbody>();
			this.Rigidbody.centerOfMass = Vector3.zero;
			this.Rigidbody.useGravity = false;
			this.Rigidbody.drag = 0f;
			this.Rigidbody.angularDrag = 0f;
			this.Rigidbody.maxAngularVelocity = float.PositiveInfinity;
			this.Rigidbody.maxDepenetrationVelocity = float.PositiveInfinity;
			this.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			this.Rigidbody.isKinematic = true;
			this.Rigidbody.constraints = RigidbodyConstraints.None;
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
		}

		// Token: 0x0600BFE4 RID: 49124 RVA: 0x0048CE4E File Offset: 0x0048B04E
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnEnable()
		{
			KinematicCharacterSystem.EnsureCreation();
			KinematicCharacterSystem.RegisterPhysicsMover(this);
		}

		// Token: 0x0600BFE5 RID: 49125 RVA: 0x0048CE5B File Offset: 0x0048B05B
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisable()
		{
			KinematicCharacterSystem.UnregisterPhysicsMover(this);
		}

		// Token: 0x0600BFE6 RID: 49126 RVA: 0x0048CE64 File Offset: 0x0048B064
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			this.Transform = base.transform;
			this.ValidateData();
			this.TransientPosition = this.Rigidbody.position;
			this.TransientRotation = this.Rigidbody.rotation;
			this.InitialSimulationPosition = this.Rigidbody.position;
			this.InitialSimulationRotation = this.Rigidbody.rotation;
		}

		// Token: 0x0600BFE7 RID: 49127 RVA: 0x0048CEC7 File Offset: 0x0048B0C7
		public void SetPosition(Vector3 position)
		{
			this.Transform.position = position;
			this.Rigidbody.position = position;
			this.InitialSimulationPosition = position;
			this.TransientPosition = position;
		}

		// Token: 0x0600BFE8 RID: 49128 RVA: 0x0048CEEF File Offset: 0x0048B0EF
		public void SetRotation(Quaternion rotation)
		{
			this.Transform.rotation = rotation;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationRotation = rotation;
			this.TransientRotation = rotation;
		}

		// Token: 0x0600BFE9 RID: 49129 RVA: 0x0048CF18 File Offset: 0x0048B118
		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			this.Transform.SetPositionAndRotation(position, rotation);
			this.Rigidbody.position = position;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationPosition = position;
			this.InitialSimulationRotation = rotation;
			this.TransientPosition = position;
			this.TransientRotation = rotation;
		}

		// Token: 0x0600BFEA RID: 49130 RVA: 0x0048CF68 File Offset: 0x0048B168
		public PhysicsMoverState GetState()
		{
			return new PhysicsMoverState
			{
				Position = this.TransientPosition,
				Rotation = this.TransientRotation,
				Velocity = this.Rigidbody.velocity,
				AngularVelocity = this.Rigidbody.velocity
			};
		}

		// Token: 0x0600BFEB RID: 49131 RVA: 0x0048CFBC File Offset: 0x0048B1BC
		public void ApplyState(PhysicsMoverState state)
		{
			this.SetPositionAndRotation(state.Position, state.Rotation);
			this.Rigidbody.velocity = state.Velocity;
			this.Rigidbody.angularVelocity = state.AngularVelocity;
		}

		// Token: 0x0600BFEC RID: 49132 RVA: 0x0048CFF4 File Offset: 0x0048B1F4
		public void VelocityUpdate(float deltaTime)
		{
			this.InitialSimulationPosition = this.TransientPosition;
			this.InitialSimulationRotation = this.TransientRotation;
			this.MoverController.UpdateMovement(out this._internalTransientPosition, out this._internalTransientRotation, deltaTime);
			if (deltaTime > 0f)
			{
				this.Rigidbody.velocity = (this.TransientPosition - this.InitialSimulationPosition) / deltaTime;
				Quaternion quaternion = this.TransientRotation * Quaternion.Inverse(this.InitialSimulationRotation);
				this.Rigidbody.angularVelocity = 0.017453292f * quaternion.eulerAngles / deltaTime;
			}
		}

		// Token: 0x0400960A RID: 38410
		[ReadOnly]
		public Rigidbody Rigidbody;

		// Token: 0x0400960B RID: 38411
		[NonSerialized]
		public IMoverController MoverController;

		// Token: 0x04009612 RID: 38418
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Vector3 _internalTransientPosition;

		// Token: 0x04009613 RID: 38419
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public Quaternion _internalTransientRotation;
	}
}
