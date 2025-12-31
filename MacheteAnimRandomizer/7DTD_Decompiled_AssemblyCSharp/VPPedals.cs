using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000B38 RID: 2872
[Preserve]
public class VPPedals : VehiclePart
{
	// Token: 0x06005940 RID: 22848 RVA: 0x0023F9C7 File Offset: 0x0023DBC7
	public override void InitPrefabConnections()
	{
		this.initPedal("L", 0);
		this.initPedal("R", 1);
		this.ParticleEffectUpdate();
	}

	// Token: 0x06005941 RID: 22849 RVA: 0x0023F9E8 File Offset: 0x0023DBE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void initPedal(string name, int index)
	{
		this.crankT = base.GetTransform();
		Transform transform = this.crankT.Find("Pedal" + name);
		this.pedalTs[index] = transform;
		base.InitIKTarget(AvatarIKGoal.LeftFoot + index, transform);
	}

	// Token: 0x06005942 RID: 22850 RVA: 0x0023FA2B File Offset: 0x0023DC2B
	public override void SetProperties(DynamicProperties _properties)
	{
		base.SetProperties(_properties);
		_properties.ParseString("pedalSound", ref this.pedalSoundName);
		_properties.ParseVec("staminaDrain", ref this.staminaDrain, ref this.staminaDrainTurbo);
	}

	// Token: 0x06005943 RID: 22851 RVA: 0x0023FA5C File Offset: 0x0023DC5C
	public override void Update(float deltaTime)
	{
		if (!this.vehicle.entity.HasDriver)
		{
			return;
		}
		EntityAlive entityAlive = this.vehicle.entity.AttachedMainEntity as EntityAlive;
		if (!entityAlive)
		{
			return;
		}
		float currentMotorTorquePercent = this.vehicle.CurrentMotorTorquePercent;
		float currentForwardVelocity = this.vehicle.CurrentForwardVelocity;
		if (currentMotorTorquePercent > 0f)
		{
			if (currentForwardVelocity > 0f)
			{
				this.rotSpeed += deltaTime * 10f * currentForwardVelocity;
				this.didPedal = true;
				this.didRun |= this.vehicle.IsTurbo;
			}
			this.backPedalTime = 0f;
		}
		else if (UnityEngine.Random.value < 0.3f * deltaTime)
		{
			this.backPedalTime = UnityEngine.Random.value * 1.2f;
		}
		entityAlive.CurrentMovementTag = (this.didRun ? EntityAlive.MovementTagRunning : EntityAlive.MovementTagIdle);
		this.staminaCheckTime += deltaTime;
		if (this.staminaCheckTime >= 0.2f)
		{
			this.staminaCheckTime = 0f;
			if (this.didPedal)
			{
				float num = this.didRun ? this.staminaDrainTurbo : this.staminaDrain;
				entityAlive.AddStamina(-num * 0.2f);
				this.didPedal = false;
				this.didRun = false;
			}
		}
		if (currentForwardVelocity != 0f && this.backPedalTime > 0f)
		{
			this.backPedalTime -= deltaTime;
			this.rotSpeed += -15f * deltaTime;
		}
		this.rotSpeed *= 0.8f;
		if (Mathf.Abs(this.rotSpeed) > 0.1f)
		{
			this.rot += this.rotSpeed;
			this.crankT.localEulerAngles = new Vector3(this.rot, 0f, 0f);
			Quaternion localRotation = Quaternion.Inverse(this.crankT.localRotation);
			for (int i = 0; i < this.pedalTs.Length; i++)
			{
				this.pedalTs[i].localRotation = localRotation;
			}
			if (this.rotSpeed > 1f)
			{
				this.pedalSoundTime += deltaTime;
				float num2 = this.vehicle.IsTurbo ? 0.55f : 0.75f;
				if (this.pedalSoundTime > num2)
				{
					this.playSound(this.pedalSoundName);
					this.pedalSoundTime = 0f;
				}
			}
		}
		if (entityAlive.Stamina < 1f)
		{
			this.staminaCooldownDelay = 2f;
		}
		if (this.staminaCooldownDelay > 0f)
		{
			this.staminaCooldownDelay -= deltaTime;
			this.vehicle.CanTurbo = false;
			return;
		}
		this.vehicle.CanTurbo = true;
	}

	// Token: 0x06005944 RID: 22852 RVA: 0x0023FD08 File Offset: 0x0023DF08
	public override void HandleEvent(Vehicle.Event _event, float _arg)
	{
		if (_event == Vehicle.Event.Start || _event == Vehicle.Event.Stop || _event == Vehicle.Event.HealthChanged)
		{
			this.ParticleEffectUpdate();
		}
	}

	// Token: 0x06005945 RID: 22853 RVA: 0x0023FD1C File Offset: 0x0023DF1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParticleEffectUpdate()
	{
		float healthPercentage = base.GetHealthPercentage();
		if (healthPercentage <= 0f)
		{
			base.SetTransformActive("chain", false);
			base.SetTransformActive("particleDamaged", true);
			base.SetTransformActive("particleBroken", true);
			return;
		}
		base.SetTransformActive("chain", true);
		base.SetTransformActive("particleBroken", false);
		if (healthPercentage <= 0.25f)
		{
			base.SetTransformActive("particleDamaged", this.vehicle.entity.HasDriver);
			return;
		}
		base.SetTransformActive("particleDamaged", false);
	}

	// Token: 0x06005946 RID: 22854 RVA: 0x0023FDA5 File Offset: 0x0023DFA5
	[PublicizedFrom(EAccessModifier.Private)]
	public void playSound(string _sound)
	{
		if (this.vehicle.entity != null && !this.vehicle.entity.isEntityRemote)
		{
			this.vehicle.entity.PlayOneShot(_sound, false, false, false, null);
		}
	}

	// Token: 0x04004439 RID: 17465
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform crankT;

	// Token: 0x0400443A RID: 17466
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform[] pedalTs = new Transform[2];

	// Token: 0x0400443B RID: 17467
	[PublicizedFrom(EAccessModifier.Private)]
	public float rot;

	// Token: 0x0400443C RID: 17468
	[PublicizedFrom(EAccessModifier.Private)]
	public float rotSpeed;

	// Token: 0x0400443D RID: 17469
	[PublicizedFrom(EAccessModifier.Private)]
	public float backPedalTime;

	// Token: 0x0400443E RID: 17470
	[PublicizedFrom(EAccessModifier.Private)]
	public string pedalSoundName;

	// Token: 0x0400443F RID: 17471
	[PublicizedFrom(EAccessModifier.Private)]
	public float pedalSoundTime;

	// Token: 0x04004440 RID: 17472
	[PublicizedFrom(EAccessModifier.Private)]
	public bool didPedal;

	// Token: 0x04004441 RID: 17473
	[PublicizedFrom(EAccessModifier.Private)]
	public bool didRun;

	// Token: 0x04004442 RID: 17474
	[PublicizedFrom(EAccessModifier.Private)]
	public float staminaCheckTime;

	// Token: 0x04004443 RID: 17475
	[PublicizedFrom(EAccessModifier.Private)]
	public float staminaDrain;

	// Token: 0x04004444 RID: 17476
	[PublicizedFrom(EAccessModifier.Private)]
	public float staminaDrainTurbo;

	// Token: 0x04004445 RID: 17477
	[PublicizedFrom(EAccessModifier.Private)]
	public float staminaCooldownDelay;
}
