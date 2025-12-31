using System;
using UnityEngine;

// Token: 0x02000839 RID: 2105
public class PhysicsBodyCapsuleCollider : PhysicsBodyColliderBase
{
	// Token: 0x06003C74 RID: 15476 RVA: 0x00184EDB File Offset: 0x001830DB
	public PhysicsBodyCapsuleCollider(CapsuleCollider _collider, PhysicsBodyColliderConfiguration _config) : base(_collider.transform, _config)
	{
		this.center = _collider.center;
		this.radius = _collider.radius;
		this.height = _collider.height;
		this.collider = _collider;
	}

	// Token: 0x17000633 RID: 1587
	// (set) Token: 0x06003C75 RID: 15477 RVA: 0x00184F18 File Offset: 0x00183118
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
				this.collider.height = this.height;
				this.collider.center = this.center;
				base.enableRigidBody(false);
				return;
			case EnumColliderMode.Collision:
				base.enableRigidBody(false);
				if ((base.Config.EnabledFlags & EnumColliderEnabledFlags.Collision) != EnumColliderEnabledFlags.Disabled)
				{
					this.collider.enabled = true;
					this.collider.radius = this.radius * base.Config.CollisionScale.x;
					this.collider.height = this.height * base.Config.CollisionScale.y;
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
					this.collider.height = this.height * base.Config.RagdollScale.y;
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

	// Token: 0x040030E8 RID: 12520
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 center;

	// Token: 0x040030E9 RID: 12521
	[PublicizedFrom(EAccessModifier.Private)]
	public float radius;

	// Token: 0x040030EA RID: 12522
	[PublicizedFrom(EAccessModifier.Private)]
	public float height;

	// Token: 0x040030EB RID: 12523
	[PublicizedFrom(EAccessModifier.Private)]
	public CapsuleCollider collider;

	// Token: 0x040030EC RID: 12524
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldLayer;
}
