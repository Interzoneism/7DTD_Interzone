using System;
using UnityEngine.Scripting;

// Token: 0x0200042F RID: 1071
[Preserve]
public class EntityDriveable : EntityVehicle
{
	// Token: 0x060020EF RID: 8431 RVA: 0x000CFD41 File Offset: 0x000CDF41
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		if (this.nativeCollider != null)
		{
			this.nativeCollider.enabled = true;
		}
	}
}
