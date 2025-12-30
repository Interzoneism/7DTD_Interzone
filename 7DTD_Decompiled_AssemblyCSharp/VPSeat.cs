using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000B39 RID: 2873
[Preserve]
public class VPSeat : VehiclePart
{
	// Token: 0x06005948 RID: 22856 RVA: 0x0023FDF5 File Offset: 0x0023DFF5
	public override void InitPrefabConnections()
	{
		base.InitIKTarget(AvatarIKGoal.LeftHand, null);
		base.InitIKTarget(AvatarIKGoal.RightHand, null);
		base.InitIKTarget(AvatarIKGoal.LeftFoot, null);
		base.InitIKTarget(AvatarIKGoal.RightFoot, null);
	}
}
