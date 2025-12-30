using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000B3A RID: 2874
[Preserve]
public class VPSteering : VehiclePart
{
	// Token: 0x0600594A RID: 22858 RVA: 0x0023FE18 File Offset: 0x0023E018
	public override void InitPrefabConnections()
	{
		StringParsers.TryParseFloat(base.GetProperty("steerMaxAngle"), out this.steerMaxAngle, 0, -1, NumberStyles.Any);
		this.properties.ParseVec("steerAngle", ref this.steerAngles);
		this.steeringJoint = base.GetTransform();
		if (this.steeringJoint)
		{
			this.baseRotation = this.steeringJoint.localRotation;
		}
		base.InitIKTarget(AvatarIKGoal.LeftHand, this.steeringJoint);
		base.InitIKTarget(AvatarIKGoal.RightHand, this.steeringJoint);
	}

	// Token: 0x0600594B RID: 22859 RVA: 0x0023FEA0 File Offset: 0x0023E0A0
	public override void Update(float _dt)
	{
		if (this.steerMaxAngle != 0f)
		{
			this.steeringJoint.localRotation = this.baseRotation * Quaternion.AngleAxis(this.vehicle.CurrentSteeringPercent * this.steerMaxAngle, Vector3.up);
		}
		if (this.steerAngles.sqrMagnitude != 0f)
		{
			float currentSteeringPercent = this.vehicle.CurrentSteeringPercent;
			this.steeringJoint.localRotation = this.baseRotation * Quaternion.Euler(currentSteeringPercent * this.steerAngles.x, currentSteeringPercent * this.steerAngles.y, currentSteeringPercent * this.steerAngles.z);
		}
	}

	// Token: 0x04004446 RID: 17478
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform steeringJoint;

	// Token: 0x04004447 RID: 17479
	[PublicizedFrom(EAccessModifier.Private)]
	public Quaternion baseRotation;

	// Token: 0x04004448 RID: 17480
	[PublicizedFrom(EAccessModifier.Private)]
	public float steerMaxAngle;

	// Token: 0x04004449 RID: 17481
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 steerAngles;
}
