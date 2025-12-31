using System;
using System.IO;
using UnityEngine;

// Token: 0x02000836 RID: 2102
public abstract class PhysicsBodyColliderBase : IBodyColliderInstance
{
	// Token: 0x06003C6A RID: 15466 RVA: 0x00184BE8 File Offset: 0x00182DE8
	public PhysicsBodyColliderBase(Transform _transform, PhysicsBodyColliderConfiguration _config)
	{
		this.transform = _transform;
		this.config = _config;
		this.rigidBody = _transform.GetComponent<Rigidbody>();
		if (this.rigidBody == null)
		{
			this.rigidBody = _transform.gameObject.AddComponent<Rigidbody>();
		}
		this.transform.tag = this.config.Tag;
		this.transform.gameObject.layer = this.config.CollisionLayer;
		this.enableRigidBody(false);
	}

	// Token: 0x06003C6B RID: 15467 RVA: 0x000424BD File Offset: 0x000406BD
	public void WriteToXML(TextWriter stream)
	{
		throw new NotImplementedException();
	}

	// Token: 0x1700062E RID: 1582
	// (get) Token: 0x06003C6C RID: 15468 RVA: 0x00184C6C File Offset: 0x00182E6C
	public Transform Transform
	{
		get
		{
			return this.transform;
		}
	}

	// Token: 0x1700062F RID: 1583
	// (get) Token: 0x06003C6D RID: 15469 RVA: 0x00184C74 File Offset: 0x00182E74
	public PhysicsBodyColliderConfiguration Config
	{
		get
		{
			return this.config;
		}
	}

	// Token: 0x17000630 RID: 1584
	// (set) Token: 0x06003C6E RID: 15470
	public abstract EnumColliderMode ColliderMode { set; }

	// Token: 0x06003C6F RID: 15471 RVA: 0x00184C7C File Offset: 0x00182E7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void enableRigidBody(bool _enabled)
	{
		if (this.rigidBody)
		{
			bool isKinematic = this.rigidBody.isKinematic;
			this.rigidBody.isKinematic = !_enabled;
			if (_enabled)
			{
				if (isKinematic)
				{
					this.rigidBody.velocity = Vector3.zero;
					this.rigidBody.angularVelocity = Vector3.zero;
				}
				this.rigidBody.useGravity = true;
				this.rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
				return;
			}
			this.rigidBody.interpolation = RigidbodyInterpolation.None;
		}
	}

	// Token: 0x040030E1 RID: 12513
	[PublicizedFrom(EAccessModifier.Private)]
	public Rigidbody rigidBody;

	// Token: 0x040030E2 RID: 12514
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform transform;

	// Token: 0x040030E3 RID: 12515
	[PublicizedFrom(EAccessModifier.Private)]
	public PhysicsBodyColliderConfiguration config;
}
