using System;
using UnityEngine;

// Token: 0x02000838 RID: 2104
public class PhysicsBodySphereCollider : PhysicsBodyColliderBase
{
	// Token: 0x06003C72 RID: 15474 RVA: 0x00184D06 File Offset: 0x00182F06
	public PhysicsBodySphereCollider(SphereCollider _collider, PhysicsBodyColliderConfiguration _config) : base(_collider.transform, _config)
	{
		this.center = _collider.center;
		this.radius = _collider.radius;
		this.collider = _collider;
	}

	// Token: 0x17000632 RID: 1586
	// (set) Token: 0x06003C73 RID: 15475 RVA: 0x00184D34 File Offset: 0x00182F34
	public override EnumColliderMode ColliderMode
	{
		set
		{
			if (this.collider == null)
			{
				return;
			}
			switch (value)
			{
			case EnumColliderMode.Disabled:
				this.collider.enabled = false;
				this.collider.radius = this.radius;
				this.collider.center = this.center;
				base.enableRigidBody(false);
				return;
			case EnumColliderMode.Collision:
				base.enableRigidBody(false);
				if ((base.Config.EnabledFlags & EnumColliderEnabledFlags.Collision) != EnumColliderEnabledFlags.Disabled)
				{
					this.collider.enabled = true;
					this.collider.radius = this.radius * base.Config.CollisionScale.x;
					this.collider.center = this.center + base.Config.CollisionOffset;
					this.collider.gameObject.layer = this.oldLayer;
					return;
				}
				this.collider.enabled = false;
				return;
			case EnumColliderMode.Ragdoll:
			case EnumColliderMode.RagdollDead:
				if ((base.Config.EnabledFlags & EnumColliderEnabledFlags.Ragdoll) != EnumColliderEnabledFlags.Disabled)
				{
					this.collider.enabled = true;
					this.collider.radius = this.radius * base.Config.RagdollScale.x;
					this.collider.center = this.center + base.Config.RagdollOffset;
					this.oldLayer = this.collider.gameObject.layer;
					this.collider.gameObject.layer = ((value == EnumColliderMode.Ragdoll) ? base.Config.RagdollLayer : 17);
					base.enableRigidBody(true);
					return;
				}
				this.collider.enabled = false;
				base.enableRigidBody(false);
				return;
			default:
				return;
			}
		}
	}

	// Token: 0x040030E4 RID: 12516
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 center;

	// Token: 0x040030E5 RID: 12517
	[PublicizedFrom(EAccessModifier.Private)]
	public float radius;

	// Token: 0x040030E6 RID: 12518
	[PublicizedFrom(EAccessModifier.Private)]
	public SphereCollider collider;

	// Token: 0x040030E7 RID: 12519
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldLayer;
}
