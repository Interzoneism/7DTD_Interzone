using System;
using Audio;
using UnityEngine;

// Token: 0x02000385 RID: 901
public class JunkSledgeFireController : MiniTurretFireController
{
	// Token: 0x06001AC9 RID: 6857 RVA: 0x000A6CF0 File Offset: 0x000A4EF0
	public new void Update()
	{
		base.Update();
		if (this.ArmState == JunkSledgeFireController.ArmStates.Idle)
		{
			return;
		}
		if (this.ArmState == JunkSledgeFireController.ArmStates.Extending)
		{
			float num = Mathf.Clamp01(this.timeCounter / (this.burstFireRateMax * 0.5f * 0.25f));
			this.Arm1.localPosition = new Vector3(this.Arm1.localPosition.x, this.Arm1.localPosition.y, Mathf.Lerp(this.Arm1StartZ, this.Arm1EndZ, num));
			this.Arm2.localPosition = new Vector3(this.Arm2.localPosition.x, this.Arm2.localPosition.y, Mathf.Lerp(this.Arm2StartZ, this.Arm2EndZ, num));
			if (num >= 1f)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					this.hitTarget();
				}
				this.ArmState = JunkSledgeFireController.ArmStates.Retracting;
				this.timeCounter = 0f;
			}
		}
		else
		{
			float num2 = Mathf.Clamp01(this.timeCounter / (this.burstFireRateMax * 0.5f * 0.75f));
			this.Arm1.localPosition = new Vector3(this.Arm1.localPosition.x, this.Arm1.localPosition.y, Mathf.Lerp(this.Arm1EndZ, this.Arm1StartZ, num2));
			this.Arm2.localPosition = new Vector3(this.Arm2.localPosition.x, this.Arm2.localPosition.y, Mathf.Lerp(this.Arm2EndZ, this.Arm2StartZ, num2));
			if (num2 >= 1f)
			{
				this.ArmState = JunkSledgeFireController.ArmStates.Idle;
				this.timeCounter = 0f;
			}
		}
		this.timeCounter += Time.deltaTime;
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x000A6EBE File Offset: 0x000A50BE
	public override void Fire()
	{
		this.ArmState = JunkSledgeFireController.ArmStates.Extending;
		Manager.Play(this.entityTurret, this.fireSound, 1f, true);
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x000A6EE0 File Offset: 0x000A50E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void hitTarget()
	{
		Vector3 position = this.Cone.transform.position;
		EntityAlive holdingEntity = GameManager.Instance.World.GetEntity(this.entityTurret.belongsPlayerId) as EntityAlive;
		float maxDistance = base.MaxDistance;
		Vector3 vector = this.Cone.transform.forward;
		vector *= -1f;
		Ray ray = new Ray(position + Origin.position, vector);
		Voxel.Raycast(GameManager.Instance.World, ray, maxDistance, -538750981, 128, 0.15f);
		ItemActionAttack.Hit(Voxel.voxelRayHitInfo.Clone(), this.entityTurret.belongsPlayerId, EnumDamageTypes.Bashing, base.GetDamageBlock(this.entityTurret.OriginalItemValue, BlockValue.Air, holdingEntity, 1), base.GetDamageEntity(this.entityTurret.OriginalItemValue, holdingEntity, 1), 1f, this.entityTurret.OriginalItemValue.PercentUsesLeft, 0f, 0f, "metal", this.damageMultiplier, this.buffActions, new ItemActionAttack.AttackHitInfo(), 1, 0, 0f, null, null, ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, this.entityTurret.entityId, this.entityTurret.OriginalItemValue);
	}

	// Token: 0x04001193 RID: 4499
	public JunkSledgeFireController.ArmStates ArmState;

	// Token: 0x04001194 RID: 4500
	public Transform Arm1;

	// Token: 0x04001195 RID: 4501
	public float Arm1StartZ;

	// Token: 0x04001196 RID: 4502
	public float Arm1EndZ;

	// Token: 0x04001197 RID: 4503
	public Transform Arm2;

	// Token: 0x04001198 RID: 4504
	public float Arm2StartZ;

	// Token: 0x04001199 RID: 4505
	public float Arm2EndZ;

	// Token: 0x0400119A RID: 4506
	public float ExtentionTime;

	// Token: 0x0400119B RID: 4507
	public float RetractionTime;

	// Token: 0x0400119C RID: 4508
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeCounter;

	// Token: 0x02000386 RID: 902
	public enum ArmStates
	{
		// Token: 0x0400119E RID: 4510
		Idle,
		// Token: 0x0400119F RID: 4511
		Extending,
		// Token: 0x040011A0 RID: 4512
		Retracting
	}
}
