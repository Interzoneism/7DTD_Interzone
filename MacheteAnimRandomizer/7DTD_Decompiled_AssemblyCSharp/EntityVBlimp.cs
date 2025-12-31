using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000469 RID: 1129
[Preserve]
public class EntityVBlimp : EntityDriveable
{
	// Token: 0x060024AA RID: 9386 RVA: 0x000E9E19 File Offset: 0x000E8019
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		this.vehicleRB.useGravity = false;
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000E9E30 File Offset: 0x000E8030
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void PhysicsInputMove()
	{
		this.vehicleRB.velocity *= 0.996f;
		this.vehicleRB.velocity += new Vector3(0f, -0.001f, 0f);
		this.vehicleRB.angularVelocity *= 0.98f;
		if (this.movementInput != null)
		{
			float num = 2f;
			if (this.movementInput.running)
			{
				num *= 6f;
			}
			this.wheelMotor = this.movementInput.moveForward;
			this.vehicleRB.AddRelativeForce(0f, 0f, this.wheelMotor * num * 0.05f, ForceMode.VelocityChange);
			float num2;
			if (this.movementInput.lastInputController)
			{
				num2 = this.movementInput.moveStrafe * num;
			}
			else
			{
				num2 = this.movementInput.moveStrafe * num;
			}
			this.vehicleRB.AddRelativeTorque(0f, num2 * 0.01f, 0f, ForceMode.VelocityChange);
			if (this.movementInput.jump)
			{
				this.vehicleRB.AddRelativeForce(0f, 0.02f * num, 0f, ForceMode.VelocityChange);
			}
			if (this.movementInput.down)
			{
				this.vehicleRB.AddRelativeForce(0f, -0.02f * num, 0f, ForceMode.VelocityChange);
			}
		}
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetWheelsForces(float motorTorque, float motorTorqueBase, float brakeTorque, float _friction)
	{
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateWheelsSteering()
	{
	}
}
