using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000B33 RID: 2867
[Preserve]
public class VPEngine : VehiclePart
{
	// Token: 0x06005917 RID: 22807 RVA: 0x0023E614 File Offset: 0x0023C814
	public override void SetProperties(DynamicProperties _properties)
	{
		base.SetProperties(_properties);
		StringParsers.TryParseFloat(base.GetProperty("fuelKmPerL"), out this.fuelKmPerL, 0, -1, NumberStyles.Any);
		_properties.ParseVec("foodDrain", ref this.foodDrain, ref this.foodDrainTurbo);
		this.gears.Clear();
		for (int i = 1; i < 9; i++)
		{
			string property = base.GetProperty("gear" + i.ToString());
			if (property.Length == 0)
			{
				break;
			}
			string[] array = property.Split(',', StringSplitOptions.None);
			VPEngine.Gear gear = new VPEngine.Gear();
			this.gears.Add(gear);
			int num = 0;
			StringParsers.TryParseFloat(array[num++], out gear.rpmMin, 0, -1, NumberStyles.Any);
			StringParsers.TryParseFloat(array[num++], out gear.rpmMax, 0, -1, NumberStyles.Any);
			StringParsers.TryParseFloat(array[num++], out gear.rpmDecel, 0, -1, NumberStyles.Any);
			StringParsers.TryParseFloat(array[num++], out gear.rpmDownShiftPoint, 0, -1, NumberStyles.Any);
			StringParsers.TryParseFloat(array[num++], out gear.rpmDownShiftTo, 0, -1, NumberStyles.Any);
			StringParsers.TryParseFloat(array[num++], out gear.rpmAccel, 0, -1, NumberStyles.Any);
			StringParsers.TryParseFloat(array[num++], out gear.rpmUpShiftPoint, 0, -1, NumberStyles.Any);
			StringParsers.TryParseFloat(array[num++], out gear.rpmUpShiftTo, 0, -1, NumberStyles.Any);
			gear.accelSoundName = array[num++].Trim();
			gear.decelSoundName = array[num++].Trim();
			int num2 = (array.Length - num) / 8;
			if (num2 > 0)
			{
				gear.soundRanges = new VPEngine.SoundRange[num2];
				for (int j = 0; j < num2; j++)
				{
					VPEngine.SoundRange soundRange = new VPEngine.SoundRange();
					gear.soundRanges[j] = soundRange;
					int num3 = num + j * 8;
					StringParsers.TryParseFloat(array[num3], out soundRange.pitchMin, 0, -1, NumberStyles.Any);
					StringParsers.TryParseFloat(array[num3 + 1], out soundRange.pitchMax, 0, -1, NumberStyles.Any);
					StringParsers.TryParseFloat(array[num3 + 2], out soundRange.volumeMin, 0, -1, NumberStyles.Any);
					StringParsers.TryParseFloat(array[num3 + 3], out soundRange.volumeMax, 0, -1, NumberStyles.Any);
					StringParsers.TryParseFloat(array[num3 + 4], out soundRange.pitchFadeMin, 0, -1, NumberStyles.Any);
					StringParsers.TryParseFloat(array[num3 + 5], out soundRange.pitchFadeMax, 0, -1, NumberStyles.Any);
					StringParsers.TryParseFloat(array[num3 + 6], out soundRange.pitchFadeRange, 0, -1, NumberStyles.Any);
					soundRange.pitchFadeRange += 1E-05f;
					soundRange.name = array[num3 + 7].Trim();
				}
			}
		}
	}

	// Token: 0x06005918 RID: 22808 RVA: 0x0023E8EF File Offset: 0x0023CAEF
	public override void InitPrefabConnections()
	{
		this.ParticleEffectUpdate();
	}

	// Token: 0x06005919 RID: 22809 RVA: 0x0023E8F8 File Offset: 0x0023CAF8
	public override void Update(float _dt)
	{
		if (this.IsBroken())
		{
			this.stopEngine(false);
			return;
		}
		EntityAlive entityAlive = this.vehicle.entity.AttachedMainEntity as EntityAlive;
		if (entityAlive)
		{
			entityAlive.CurrentMovementTag = EntityAlive.MovementTagDriving;
			float value = 0f;
			if (this.vehicle.CurrentIsAccel)
			{
				value = this.foodDrain;
				if (this.vehicle.IsTurbo)
				{
					value = this.foodDrainTurbo;
				}
			}
			entityAlive.SetCVar("_vehicleFood", value);
		}
		if (!this.isRunning)
		{
			return;
		}
		float magnitude = this.vehicle.CurrentVelocity.magnitude;
		float num = _dt / (this.fuelKmPerL * 1000f);
		if (this.vehicle.IsTurbo)
		{
			num *= 2f;
		}
		if (this.vehicle.CurrentIsAccel)
		{
			num *= magnitude;
		}
		else
		{
			num *= this.vehicle.VelocityMaxForward * 0.1f;
		}
		num *= this.vehicle.EffectFuelUsePer;
		this.vehicle.FireEvent(VehiclePart.Event.FuelRemove, this, num);
	}

	// Token: 0x0600591A RID: 22810 RVA: 0x0023E9FC File Offset: 0x0023CBFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParticleEffectUpdate()
	{
		base.SetTransformActive("particleOn", this.isRunning);
		float healthPercentage = base.GetHealthPercentage();
		if (healthPercentage <= 0f)
		{
			base.SetTransformActive("particleDamaged", true);
			base.SetTransformActive("particleBroken", true);
			return;
		}
		base.SetTransformActive("particleBroken", false);
		if (healthPercentage <= 0.25f)
		{
			base.SetTransformActive("particleDamaged", this.isRunning);
			return;
		}
		base.SetTransformActive("particleDamaged", false);
	}

	// Token: 0x0600591B RID: 22811 RVA: 0x0023EA74 File Offset: 0x0023CC74
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateEngineSimulation()
	{
		if (!this.isRunning)
		{
			return;
		}
		float num = 500f;
		float num2 = 5000f;
		float num3 = -2400f;
		float num4 = 2700f;
		float num5 = 2700f;
		float num6 = 5000f;
		float num7 = 1500f;
		float num8 = 2800f;
		if (this.gears.Count > 0)
		{
			VPEngine.Gear gear = this.gears[this.gearIndex];
			num = gear.rpmMin;
			num2 = gear.rpmMax;
			num3 = gear.rpmDecel;
			num4 = gear.rpmDownShiftPoint;
			num5 = gear.rpmDownShiftTo;
			num8 = gear.rpmAccel;
			num6 = gear.rpmUpShiftPoint;
			num7 = gear.rpmUpShiftTo;
		}
		if (this.vehicle.CurrentIsAccel)
		{
			this.rpm += num8 * Time.deltaTime;
			this.rpm = Mathf.Min(this.rpm, num2);
			if (this.rpm >= num6 && this.gearIndex < this.gears.Count - 1 && this.vehicle.CurrentForwardVelocity > 4f)
			{
				this.gearIndex++;
				this.rpm = num7;
				this.vehicle.entity.AddRelativeForce(new Vector3(0f, 0.2f, -2f), ForceMode.VelocityChange);
				VPEngine.Gear gear2 = this.gears[this.gearIndex];
				this.playAccelDecelSound(gear2.accelSoundName);
			}
			if (this.acceleratePhase <= 0)
			{
				if (this.gears.Count > 0)
				{
					VPEngine.Gear gear3 = this.gears[this.gearIndex];
					this.isDecelSoundPlayed = false;
					this.playAccelDecelSound(gear3.accelSoundName);
				}
				this.acceleratePhase = 1;
			}
			float rpmPercent = (this.rpm - num) / (num2 - num);
			this.updateEngineSounds(rpmPercent);
			return;
		}
		if (this.acceleratePhase >= 0)
		{
			float num9 = num3;
			if (Mathf.Abs(this.vehicle.CurrentForwardVelocity) < 2f)
			{
				num9 *= 2f;
			}
			this.rpm += num9 * Time.deltaTime;
			if (this.rpm > num4)
			{
				float rpmPercent2 = (this.rpm - num) / (num2 - num);
				this.updateEngineSounds(rpmPercent2);
				return;
			}
			if (this.gears.Count > 0 && !this.isDecelSoundPlayed)
			{
				this.isDecelSoundPlayed = true;
				VPEngine.Gear gear4 = this.gears[this.gearIndex];
				this.playAccelDecelSound(gear4.decelSoundName);
			}
			if (this.gearIndex <= 0)
			{
				this.acceleratePhase = -1;
				this.updateEngineSounds(0f);
				return;
			}
			this.acceleratePhase = 0;
			this.gearIndex = 0;
			if (num5 > 0f)
			{
				this.rpm = num5;
				return;
			}
		}
		else
		{
			this.updateEngineSounds(0f);
		}
	}

	// Token: 0x0600591C RID: 22812 RVA: 0x0023ED24 File Offset: 0x0023CF24
	public override void HandleEvent(Vehicle.Event _event, float _arg)
	{
		switch (_event)
		{
		case Vehicle.Event.Start:
			if (!this.IsBroken())
			{
				this.startEngine();
				return;
			}
			break;
		case Vehicle.Event.Started:
		case Vehicle.Event.Stopped:
			break;
		case Vehicle.Event.Stop:
		{
			EntityAlive entityAlive = this.vehicle.entity.AttachedMainEntity as EntityAlive;
			if (entityAlive)
			{
				entityAlive.SetCVar("_vehicleFood", 0f);
			}
			this.stopEngine(false);
			return;
		}
		case Vehicle.Event.SimulationUpdate:
			if (!this.IsBroken())
			{
				this.updateEngineSimulation();
				return;
			}
			break;
		case Vehicle.Event.HealthChanged:
			this.ParticleEffectUpdate();
			break;
		default:
			return;
		}
	}

	// Token: 0x0600591D RID: 22813 RVA: 0x0023EDAA File Offset: 0x0023CFAA
	public override void HandleEvent(VehiclePart.Event _event, VehiclePart _part, float _arg)
	{
		if (_event == VehiclePart.Event.FuelEmpty)
		{
			this.stopEngine(true);
		}
	}

	// Token: 0x0600591E RID: 22814 RVA: 0x0023EDB8 File Offset: 0x0023CFB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void startEngine()
	{
		if (this.isRunning)
		{
			return;
		}
		this.isRunning = true;
		if (this.vehicle.GetFuelLevel() > 0f)
		{
			this.playSound(this.properties.Values["sound_start"]);
			this.gearIndex = 0;
			this.updateEngineSounds(0f);
		}
		this.vehicle.entity.IsEngineRunning = true;
		this.vehicle.FireEvent(Vehicle.Event.Started);
		this.ParticleEffectUpdate();
	}

	// Token: 0x0600591F RID: 22815 RVA: 0x0023EE38 File Offset: 0x0023D038
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopEngine(bool _outOfFuel = false)
	{
		if (!this.isRunning)
		{
			return;
		}
		this.isRunning = false;
		this.stopEngineSounds();
		if (!_outOfFuel)
		{
			this.playSound(this.properties.Values["sound_shut_off"]);
		}
		else
		{
			this.playSound(this.properties.Values["sound_no_fuel_shut_off"]);
		}
		this.vehicle.entity.IsEngineRunning = false;
		this.vehicle.FireEvent(Vehicle.Event.Stopped);
		this.ParticleEffectUpdate();
	}

	// Token: 0x06005920 RID: 22816 RVA: 0x0023EEB9 File Offset: 0x0023D0B9
	[PublicizedFrom(EAccessModifier.Private)]
	public void playSound(string _sound)
	{
		if (this.vehicle.entity && !this.vehicle.entity.isEntityRemote)
		{
			this.vehicle.entity.PlayOneShot(_sound, false, false, false, null);
		}
	}

	// Token: 0x06005921 RID: 22817 RVA: 0x0023EEF4 File Offset: 0x0023D0F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopSound(string _sound)
	{
		if (this.vehicle.entity && !this.vehicle.entity.isEntityRemote)
		{
			this.vehicle.entity.StopOneShot(_sound);
		}
	}

	// Token: 0x06005922 RID: 22818 RVA: 0x0023EF2B File Offset: 0x0023D12B
	[PublicizedFrom(EAccessModifier.Private)]
	public void changeSoundLoop(string soundName, ref Handle handle)
	{
		this.stopSoundLoop(ref handle);
		this.playSoundLoop(soundName, ref handle);
	}

	// Token: 0x06005923 RID: 22819 RVA: 0x0023EF3C File Offset: 0x0023D13C
	[PublicizedFrom(EAccessModifier.Private)]
	public void playSoundLoop(string soundName, ref Handle handle)
	{
		if (handle != null)
		{
			return;
		}
		if (this.vehicle.entity)
		{
			handle = Manager.Play(this.vehicle.entity, soundName, 1f, true);
		}
	}

	// Token: 0x06005924 RID: 22820 RVA: 0x0023EF6E File Offset: 0x0023D16E
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopSoundLoop(ref Handle handle)
	{
		if (handle != null)
		{
			handle.Stop(this.vehicle.entity.entityId);
			handle = null;
		}
	}

	// Token: 0x06005925 RID: 22821 RVA: 0x0023EF8E File Offset: 0x0023D18E
	[PublicizedFrom(EAccessModifier.Private)]
	public void playAccelDecelSound(string name)
	{
		if (this.accelDecelSoundName != null)
		{
			this.stopSound(this.accelDecelSoundName);
		}
		if (name != null && name.Length > 0)
		{
			this.playSound(name);
		}
		this.accelDecelSoundName = name;
	}

	// Token: 0x06005926 RID: 22822 RVA: 0x0023EFC0 File Offset: 0x0023D1C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateEngineSounds(float rpmPercent)
	{
		if (this.gears.Count > 0)
		{
			VPEngine.Gear gear;
			for (int i = 0; i < this.gears.Count; i++)
			{
				if (i != this.gearIndex)
				{
					gear = this.gears[i];
					for (int j = 0; j < gear.soundRanges.Length; j++)
					{
						VPEngine.SoundRange soundRange = gear.soundRanges[j];
						if (soundRange.soundHandle != null)
						{
							this.stopSoundLoop(ref soundRange.soundHandle);
						}
					}
				}
			}
			float deltaTime = Time.deltaTime;
			this.pitchRandTime -= deltaTime;
			if (this.pitchRandTime <= 0f)
			{
				this.pitchRandTime = 0.75f;
				this.pitchRand = this.vehicle.entity.rand.RandomRange(-1f, 1f) * 0.03f;
			}
			float num = this.pitchRand;
			if (rpmPercent > 0f && this.vehicle.IsTurbo)
			{
				num += 0.2f;
				if (!this.isTurbo)
				{
					this.playSound("vehicle_turbo");
				}
			}
			this.isTurbo = (rpmPercent > 0f && this.vehicle.IsTurbo);
			this.pitchAdd = Mathf.MoveTowards(this.pitchAdd, num, deltaTime * 0.15f);
			gear = this.gears[this.gearIndex];
			for (int k = 0; k < gear.soundRanges.Length; k++)
			{
				VPEngine.SoundRange soundRange2 = gear.soundRanges[k];
				float num2 = Mathf.Lerp(soundRange2.pitchMin, soundRange2.pitchMax, rpmPercent);
				float num3 = Mathf.Lerp(soundRange2.volumeMin, soundRange2.volumeMax, rpmPercent);
				float num4 = 1f;
				float num5 = soundRange2.pitchFadeMin - num2;
				if (num5 > 0f)
				{
					num4 = Mathf.Lerp(1f, 0f, num5 / soundRange2.pitchFadeRange);
				}
				else
				{
					float num6 = num2 - soundRange2.pitchFadeMax;
					if (num6 > 0f)
					{
						num4 = Mathf.Lerp(1f, 0f, num6 / soundRange2.pitchFadeRange);
					}
				}
				float num7 = num3 * num4;
				if (num7 < 0.01f)
				{
					if (soundRange2.soundHandle != null)
					{
						this.stopSoundLoop(ref soundRange2.soundHandle);
					}
				}
				else
				{
					if (soundRange2.soundHandle == null)
					{
						this.playSoundLoop(soundRange2.name, ref soundRange2.soundHandle);
					}
					if (soundRange2.soundHandle != null)
					{
						soundRange2.soundHandle.SetPitch(num2 + this.pitchAdd);
						soundRange2.soundHandle.SetVolume(num7);
					}
				}
			}
		}
	}

	// Token: 0x06005927 RID: 22823 RVA: 0x0023F244 File Offset: 0x0023D444
	[PublicizedFrom(EAccessModifier.Private)]
	public void stopEngineSounds()
	{
		if (this.gears.Count > 0)
		{
			for (int i = 0; i < this.gears.Count; i++)
			{
				VPEngine.Gear gear = this.gears[i];
				for (int j = 0; j < gear.soundRanges.Length; j++)
				{
					VPEngine.SoundRange soundRange = gear.soundRanges[j];
					if (soundRange.soundHandle != null)
					{
						this.stopSoundLoop(ref soundRange.soundHandle);
					}
				}
			}
		}
		this.playAccelDecelSound(null);
	}

	// Token: 0x04004403 RID: 17411
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cIdleFuelPercent = 0.1f;

	// Token: 0x04004404 RID: 17412
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cTurboFuelPercent = 2f;

	// Token: 0x04004405 RID: 17413
	[PublicizedFrom(EAccessModifier.Private)]
	public float fuelKmPerL;

	// Token: 0x04004406 RID: 17414
	[PublicizedFrom(EAccessModifier.Private)]
	public float foodDrain;

	// Token: 0x04004407 RID: 17415
	[PublicizedFrom(EAccessModifier.Private)]
	public float foodDrainTurbo;

	// Token: 0x04004408 RID: 17416
	public bool isRunning;

	// Token: 0x04004409 RID: 17417
	[PublicizedFrom(EAccessModifier.Private)]
	public int acceleratePhase;

	// Token: 0x0400440A RID: 17418
	[PublicizedFrom(EAccessModifier.Private)]
	public float rpm;

	// Token: 0x0400440B RID: 17419
	[PublicizedFrom(EAccessModifier.Private)]
	public int gearIndex;

	// Token: 0x0400440C RID: 17420
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDecelSoundPlayed;

	// Token: 0x0400440D RID: 17421
	[PublicizedFrom(EAccessModifier.Private)]
	public string accelDecelSoundName;

	// Token: 0x0400440E RID: 17422
	[PublicizedFrom(EAccessModifier.Private)]
	public float pitchRandTime;

	// Token: 0x0400440F RID: 17423
	[PublicizedFrom(EAccessModifier.Private)]
	public float pitchRand;

	// Token: 0x04004410 RID: 17424
	[PublicizedFrom(EAccessModifier.Private)]
	public float pitchAdd;

	// Token: 0x04004411 RID: 17425
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTurbo;

	// Token: 0x04004412 RID: 17426
	[PublicizedFrom(EAccessModifier.Private)]
	public List<VPEngine.Gear> gears = new List<VPEngine.Gear>();

	// Token: 0x02000B34 RID: 2868
	[PublicizedFrom(EAccessModifier.Private)]
	public class Gear
	{
		// Token: 0x04004413 RID: 17427
		public float rpmMin;

		// Token: 0x04004414 RID: 17428
		public float rpmMax;

		// Token: 0x04004415 RID: 17429
		public float rpmDecel;

		// Token: 0x04004416 RID: 17430
		public float rpmAccel;

		// Token: 0x04004417 RID: 17431
		public float rpmDownShiftPoint;

		// Token: 0x04004418 RID: 17432
		public float rpmUpShiftPoint;

		// Token: 0x04004419 RID: 17433
		public float rpmDownShiftTo;

		// Token: 0x0400441A RID: 17434
		public float rpmUpShiftTo;

		// Token: 0x0400441B RID: 17435
		public string accelSoundName;

		// Token: 0x0400441C RID: 17436
		public string decelSoundName;

		// Token: 0x0400441D RID: 17437
		public VPEngine.SoundRange[] soundRanges;
	}

	// Token: 0x02000B35 RID: 2869
	[PublicizedFrom(EAccessModifier.Private)]
	public class SoundRange
	{
		// Token: 0x0400441E RID: 17438
		public float pitchMin;

		// Token: 0x0400441F RID: 17439
		public float pitchMax;

		// Token: 0x04004420 RID: 17440
		public float volumeMin;

		// Token: 0x04004421 RID: 17441
		public float volumeMax;

		// Token: 0x04004422 RID: 17442
		public float pitchFadeMin;

		// Token: 0x04004423 RID: 17443
		public float pitchFadeMax;

		// Token: 0x04004424 RID: 17444
		public float pitchFadeRange;

		// Token: 0x04004425 RID: 17445
		public string name;

		// Token: 0x04004426 RID: 17446
		public Handle soundHandle;
	}
}
