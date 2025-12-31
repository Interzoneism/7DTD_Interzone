using System;
using UnityEngine;

// Token: 0x02000837 RID: 2103
public class PhysicsBodyNullCollider : PhysicsBodyColliderBase
{
	// Token: 0x06003C70 RID: 15472 RVA: 0x00184CFC File Offset: 0x00182EFC
	public PhysicsBodyNullCollider(Transform _transform, PhysicsBodyColliderConfiguration _config) : base(_transform, _config)
	{
	}

	// Token: 0x17000631 RID: 1585
	// (set) Token: 0x06003C71 RID: 15473 RVA: 0x00002914 File Offset: 0x00000B14
	public override EnumColliderMode ColliderMode
	{
		set
		{
		}
	}
}
