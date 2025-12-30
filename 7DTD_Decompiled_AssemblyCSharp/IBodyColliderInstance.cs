using System;
using UnityEngine;

// Token: 0x02000833 RID: 2099
public interface IBodyColliderInstance
{
	// Token: 0x17000628 RID: 1576
	// (get) Token: 0x06003C55 RID: 15445
	Transform Transform { get; }

	// Token: 0x17000629 RID: 1577
	// (set) Token: 0x06003C56 RID: 15446
	EnumColliderMode ColliderMode { set; }

	// Token: 0x1700062A RID: 1578
	// (get) Token: 0x06003C57 RID: 15447
	PhysicsBodyColliderConfiguration Config { get; }
}
