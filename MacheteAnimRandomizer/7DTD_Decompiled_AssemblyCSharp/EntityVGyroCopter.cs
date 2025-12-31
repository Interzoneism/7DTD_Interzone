using System;
using UnityEngine.Scripting;

// Token: 0x02000477 RID: 1143
[Preserve]
public class EntityVGyroCopter : EntityDriveable
{
	// Token: 0x06002548 RID: 9544 RVA: 0x000EC114 File Offset: 0x000EA314
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateWheelsSteering()
	{
		this.wheels[0].wheelC.steerAngle = this.wheelDir;
	}
}
