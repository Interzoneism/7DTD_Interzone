using System;
using UnityEngine.Scripting;

// Token: 0x02000479 RID: 1145
[Preserve]
public class EntityVJeep : EntityDriveable
{
	// Token: 0x06002550 RID: 9552 RVA: 0x000F0D69 File Offset: 0x000EEF69
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateWheelsSteering()
	{
		this.wheels[0].wheelC.steerAngle = this.wheelDir;
		this.wheels[1].wheelC.steerAngle = this.wheelDir;
	}
}
