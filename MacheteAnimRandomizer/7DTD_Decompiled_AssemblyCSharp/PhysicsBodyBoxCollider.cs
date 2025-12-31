using System;
using UnityEngine;

// Token: 0x0200083A RID: 2106
public class PhysicsBodyBoxCollider : PhysicsBodyColliderBase
{
	// Token: 0x06003C76 RID: 15478 RVA: 0x00185117 File Offset: 0x00183317
	public PhysicsBodyBoxCollider(BoxCollider _collider, PhysicsBodyColliderConfiguration _config) : base(_collider.transform, _config)
	{
		this.center = _collider.center;
		this.size = _collider.size;
		this.collider = _collider;
	}

	// Token: 0x17000634 RID: 1588
	// (set) Token: 0x06003C77 RID: 15479 RVA: 0x00185148 File Offset: 0x00183348
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
				this.collider.size = this.size;
				this.collider.center = this.center;
				base.enableRigidBody(false);
				return;
			case EnumColliderMode.Collision:
				base.enableRigidBody(false);
				if ((base.Config.EnabledFlags & EnumColliderEnabledFlags.Collision) != EnumColliderEnabledFlags.Disabled)
				{
					this.collider.enabled = true;
					this.collider.size = Vector3.Scale(this.size, base.Config.CollisionScale);
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
					this.collider.size = Vector3.Scale(this.size, base.Config.RagdollScale);
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

	// Token: 0x040030ED RID: 12525
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 center;

	// Token: 0x040030EE RID: 12526
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 size;

	// Token: 0x040030EF RID: 12527
	[PublicizedFrom(EAccessModifier.Private)]
	public BoxCollider collider;

	// Token: 0x040030F0 RID: 12528
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldLayer;
}
