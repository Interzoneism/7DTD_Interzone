using System;
using UnityEngine;

// Token: 0x020000CD RID: 205
[RequireComponent(typeof(Collider))]
public class RagdollWhenHit : RootTransformRefEntity
{
	// Token: 0x1700005C RID: 92
	// (get) Token: 0x06000519 RID: 1305 RVA: 0x00024A7B File Offset: 0x00022C7B
	public Entity theEntity
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this._entity == null && this.RootTransform != null)
			{
				this._entity = this.RootTransform.GetComponent<Entity>();
			}
			return this._entity;
		}
	}

	// Token: 0x0600051A RID: 1306 RVA: 0x00024AB0 File Offset: 0x00022CB0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Start()
	{
		base.Start();
		CapsuleCollider component = base.GetComponent<CapsuleCollider>();
		if (component != null)
		{
			this._radius = component.radius;
			this._offset = component.center;
			return;
		}
		SphereCollider component2 = base.GetComponent<SphereCollider>();
		if (component2 != null)
		{
			this._radius = component2.radius;
			this._offset = component2.center;
			return;
		}
		BoxCollider component3 = base.GetComponent<BoxCollider>();
		this._radius = Mathf.Max(new float[]
		{
			component3.size.x,
			component3.size.y,
			component3.size.z
		});
		this._offset = component3.center;
	}

	// Token: 0x0600051B RID: 1307 RVA: 0x00024B63 File Offset: 0x00022D63
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this._pos = base.transform.position;
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x00024B78 File Offset: 0x00022D78
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		Vector3 direction = base.transform.position - this._pos;
		if (direction.sqrMagnitude > 0.001f)
		{
			float magnitude = direction.magnitude;
			direction.Normalize();
			RaycastHit raycastHit;
			if (Physics.SphereCast(this._pos + this._offset, this._radius, direction, out raycastHit, magnitude, 65536))
			{
				base.enabled = false;
				if (this.theEntity != null)
				{
					DamageResponse dr = DamageResponse.New(false);
					dr.ImpulseScale = 0f;
					this.theEntity.emodel.DoRagdoll(dr, 999999f);
					return;
				}
			}
			else
			{
				this._pos = base.transform.position;
			}
		}
	}

	// Token: 0x040005D3 RID: 1491
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity _entity;

	// Token: 0x040005D4 RID: 1492
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public SphereCollider _sphere;

	// Token: 0x040005D5 RID: 1493
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BoxCollider _box;

	// Token: 0x040005D6 RID: 1494
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CapsuleCollider _capsule;

	// Token: 0x040005D7 RID: 1495
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool _hasFrame;

	// Token: 0x040005D8 RID: 1496
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 _pos;

	// Token: 0x040005D9 RID: 1497
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 _offset;

	// Token: 0x040005DA RID: 1498
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float _radius;
}
