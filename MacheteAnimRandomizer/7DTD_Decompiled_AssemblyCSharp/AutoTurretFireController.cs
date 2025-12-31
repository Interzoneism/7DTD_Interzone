using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine;

// Token: 0x0200037E RID: 894
public class AutoTurretFireController : MonoBehaviour
{
	// Token: 0x1700030B RID: 779
	// (get) Token: 0x06001A8A RID: 6794 RVA: 0x000A4696 File Offset: 0x000A2896
	// (set) Token: 0x06001A8B RID: 6795 RVA: 0x000A46A8 File Offset: 0x000A28A8
	public Vector3 BlockPosition
	{
		get
		{
			return this.blockPos - Origin.position;
		}
		set
		{
			this.blockPos = value;
		}
	}

	// Token: 0x1700030C RID: 780
	// (get) Token: 0x06001A8C RID: 6796 RVA: 0x000A46B1 File Offset: 0x000A28B1
	// (set) Token: 0x06001A8D RID: 6797 RVA: 0x000A46BE File Offset: 0x000A28BE
	public float CenteredYaw
	{
		get
		{
			return this.TileEntity.CenteredYaw;
		}
		set
		{
			this.TileEntity.CenteredYaw = value;
		}
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06001A8E RID: 6798 RVA: 0x000A46CC File Offset: 0x000A28CC
	// (set) Token: 0x06001A8F RID: 6799 RVA: 0x000A46D9 File Offset: 0x000A28D9
	public float CenteredPitch
	{
		get
		{
			return this.TileEntity.CenteredPitch;
		}
		set
		{
			this.TileEntity.CenteredPitch = value;
		}
	}

	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06001A90 RID: 6800 RVA: 0x000A46E7 File Offset: 0x000A28E7
	public float MaxDistance
	{
		get
		{
			return this.maxDistance;
		}
	}

	// Token: 0x06001A91 RID: 6801 RVA: 0x000A46F0 File Offset: 0x000A28F0
	public void Init(DynamicProperties _properties, AutoTurretController _atc)
	{
		this.atc = _atc;
		this.IsOn = false;
		if (_properties.Values.ContainsKey("FireSound"))
		{
			this.fireSound = _properties.Values["FireSound"];
		}
		else
		{
			this.fireSound = "Electricity/Turret/turret_fire";
		}
		if (_properties.Values.ContainsKey("WakeUpSound"))
		{
			this.wakeUpSound = _properties.Values["WakeUpSound"];
		}
		else
		{
			this.wakeUpSound = "Electricity/Turret/turret_windup";
		}
		if (_properties.Values.ContainsKey("OverheatSound"))
		{
			this.overheatSound = _properties.Values["OverheatSound"];
		}
		else
		{
			this.overheatSound = "Electricity/Turret/turret_overheat_lp";
		}
		if (_properties.Values.ContainsKey("TargetingSound"))
		{
			this.targetingSound = _properties.Values["TargetingSound"];
		}
		else
		{
			this.targetingSound = "Electricity/Turret/turret_retarget_lp";
		}
		if (_properties.Values.ContainsKey("IdleSound"))
		{
			this.idleSound = _properties.Values["IdleSound"];
		}
		else
		{
			this.idleSound = "Electricity/Turret/turret_idle_lp";
		}
		if (_properties.Values.ContainsKey("EntityDamage"))
		{
			this.entityDamage = int.Parse(_properties.Values["EntityDamage"]);
		}
		if (_properties.Values.ContainsKey("BlockDamage"))
		{
			this.blockDamage = int.Parse(_properties.Values["BlockDamage"]);
		}
		else
		{
			this.blockDamage = 0;
		}
		if (_properties.Values.ContainsKey("MaxDistance"))
		{
			this.maxDistance = StringParsers.ParseFloat(_properties.Values["MaxDistance"], 0, -1, NumberStyles.Any);
		}
		else
		{
			this.maxDistance = 16f;
		}
		if (_properties.Values.ContainsKey("YawRange"))
		{
			float num = StringParsers.ParseFloat(_properties.Values["YawRange"], 0, -1, NumberStyles.Any);
			num *= 0.5f;
			this.yawRange = new Vector2(-num, num);
		}
		else
		{
			this.yawRange = new Vector2(-22.5f, 22.5f);
		}
		if (_properties.Values.ContainsKey("PitchRange"))
		{
			float num2 = StringParsers.ParseFloat(_properties.Values["PitchRange"], 0, -1, NumberStyles.Any);
			num2 *= 0.5f;
			this.pitchRange = new Vector2(-num2, num2);
		}
		else
		{
			this.pitchRange = new Vector2(-22.5f, 22.5f);
		}
		if (_properties.Values.ContainsKey("RaySpread"))
		{
			float num3 = StringParsers.ParseFloat(_properties.Values["RaySpread"], 0, -1, NumberStyles.Any);
			num3 *= 0.5f;
			this.spread = new Vector2(-num3, num3);
		}
		else
		{
			this.spread = new Vector2(-1f, 1f);
		}
		if (_properties.Values.ContainsKey("RayCount"))
		{
			this.rayCount = int.Parse(_properties.Values["RayCount"]);
		}
		else
		{
			this.rayCount = 1;
		}
		if (_properties.Values.ContainsKey("WakeUpTime"))
		{
			this.wakeUpTimeMax = StringParsers.ParseFloat(_properties.Values["WakeUpTime"], 0, -1, NumberStyles.Any);
		}
		if (_properties.Values.ContainsKey("FallAsleepTime"))
		{
			this.fallAsleepTimeMax = StringParsers.ParseFloat(_properties.Values["FallAsleepTime"], 0, -1, NumberStyles.Any);
		}
		if (_properties.Values.ContainsKey("BurstRoundCount"))
		{
			this.burstRoundCountMax = int.Parse(_properties.Values["BurstRoundCount"]);
		}
		if (_properties.Values.ContainsKey("BurstFireRate"))
		{
			this.burstFireRateMax = StringParsers.ParseFloat(_properties.Values["BurstFireRate"], 0, -1, NumberStyles.Any);
		}
		if (_properties.Values.ContainsKey("CooldownTime"))
		{
			this.coolOffTimeMax = StringParsers.ParseFloat(_properties.Values["CooldownTime"], 0, -1, NumberStyles.Any);
		}
		if (_properties.Values.ContainsKey("OvershootTime"))
		{
			this.overshootTimeMax = StringParsers.ParseFloat(_properties.Values["OvershootTime"], 0, -1, NumberStyles.Any);
		}
		_properties.ParseString("ParticlesMuzzleFire", ref this.muzzleFireParticle);
		_properties.ParseString("ParticlesMuzzleSmoke", ref this.muzzleSmokeParticle);
		if (_properties.Values.ContainsKey("AmmoItem"))
		{
			this.ammoItemName = _properties.Values["AmmoItem"];
		}
		else
		{
			this.ammoItemName = "9mmBullet";
		}
		this.buffActions = new List<string>();
		if (_properties.Values.ContainsKey("Buff"))
		{
			string[] collection = _properties.Values["Buff"].Replace(" ", "").Split(',', StringSplitOptions.None);
			this.buffActions.AddRange(collection);
		}
		this.targetingBounds = this.Cone.GetComponent<MeshRenderer>().bounds;
		this.damageMultiplier = new DamageMultiplier(_properties, null);
		this.sorter = new AutoTurretFireController.TurretEntitySorter(this.BlockPosition);
		this.state = AutoTurretFireController.TurretState.Asleep;
		this.Cone.localScale = new Vector3(this.Cone.localScale.x * (this.yawRange.y / 22.5f) * (this.maxDistance / 5.25f), this.Cone.localScale.y * (this.pitchRange.y / 22.5f) * (this.maxDistance / 5.25f), this.Cone.localScale.z * (this.maxDistance / 5.25f));
		this.Cone.gameObject.SetActive(false);
		this.Laser.localScale = new Vector3(this.Laser.localScale.x, this.Laser.localScale.y, this.Laser.localScale.z * (this.maxDistance / 5.25f));
		this.Laser.gameObject.SetActive(false);
	}

	// Token: 0x06001A92 RID: 6802 RVA: 0x000A4D10 File Offset: 0x000A2F10
	public void OnDestroy()
	{
		this.OnPoweredOff();
	}

	// Token: 0x1700030F RID: 783
	// (get) Token: 0x06001A93 RID: 6803 RVA: 0x000A4D18 File Offset: 0x000A2F18
	public bool hasTarget
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.currentEntityTarget != null;
		}
	}

	// Token: 0x06001A94 RID: 6804 RVA: 0x000A4D28 File Offset: 0x000A2F28
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.atc == null || this.TileEntity == null)
		{
			return;
		}
		if (!this.IsOn || this.atc.IsUserAccessing || this.TileEntity.IsUserAccessing())
		{
			if (this.atc.IsUserAccessing)
			{
				this.atc.YawController.Yaw = this.CenteredYaw;
				this.atc.YawController.UpdateYaw();
				this.atc.PitchController.Pitch = this.CenteredPitch;
				this.atc.PitchController.UpdatePitch();
				switch (this.state)
				{
				case AutoTurretFireController.TurretState.Asleep:
					this.state = AutoTurretFireController.TurretState.Awake;
					return;
				case AutoTurretFireController.TurretState.Awake:
					if (this.burstRoundCount >= this.burstRoundCountMax)
					{
						this.state = AutoTurretFireController.TurretState.Overheated;
						this.burstRoundCount = 0;
						return;
					}
					break;
				case AutoTurretFireController.TurretState.Overheated:
					if (this.coolOffTime == 0f)
					{
						this.broadcastPlay(this.overheatSound);
					}
					if (this.coolOffTime < this.coolOffTimeMax)
					{
						this.coolOffTime += Time.deltaTime;
						return;
					}
					this.state = AutoTurretFireController.TurretState.Awake;
					this.coolOffTime = 0f;
					this.broadcastStop(this.overheatSound);
					return;
				default:
					return;
				}
			}
			else if (!this.IsOn)
			{
				if (this.atc.YawController.Yaw != this.CenteredYaw)
				{
					this.atc.YawController.Yaw = this.CenteredYaw;
					this.atc.YawController.UpdateYaw();
				}
				if (this.atc.PitchController.Pitch != this.CenteredPitch)
				{
					this.atc.PitchController.Pitch = this.CenteredPitch;
					this.atc.PitchController.UpdatePitch();
					return;
				}
			}
			else
			{
				if (this.atc.YawController.Yaw != this.CenteredYaw)
				{
					this.atc.YawController.Yaw = this.CenteredYaw;
					this.atc.YawController.UpdateYaw();
				}
				if (this.atc.PitchController.Pitch != this.CenteredPitch)
				{
					this.atc.PitchController.Pitch = this.CenteredPitch;
					this.atc.PitchController.UpdatePitch();
				}
			}
			return;
		}
		if (!this.hasTarget)
		{
			this.findTarget();
		}
		else if (this.shouldIgnoreTarget(this.currentEntityTarget))
		{
			this.currentEntityTarget = null;
			if (!this.state.Equals(AutoTurretFireController.TurretState.Overheated))
			{
				this.state = AutoTurretFireController.TurretState.Asleep;
				this.wakeUpTime = 0f;
			}
		}
		if (this.atc.IsTurning)
		{
			this.broadcastPlay(this.targetingSound);
			this.broadcastStop(this.idleSound);
		}
		else
		{
			this.broadcastStop(this.targetingSound);
			this.broadcastPlay(this.idleSound);
		}
		switch (this.state)
		{
		case AutoTurretFireController.TurretState.Asleep:
			if (this.hasTarget)
			{
				if (this.wakeUpTime == 0f)
				{
					this.broadcastPlay(this.wakeUpSound);
				}
				float num = this.wakeUpTime;
				PassiveEffects passiveEffect = PassiveEffects.TurretWakeUp;
				ItemValue originalItemValue = null;
				EntityAlive entity = this.currentEntityTarget;
				if (num < EffectManager.GetValue(passiveEffect, originalItemValue, this.wakeUpTimeMax, entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false))
				{
					this.wakeUpTime += Time.deltaTime;
				}
				else
				{
					this.state = AutoTurretFireController.TurretState.Awake;
					this.wakeUpTime = 0f;
				}
			}
			else
			{
				this.atc.YawController.Yaw = this.CenteredYaw;
				this.atc.PitchController.Pitch = this.CenteredPitch;
			}
			break;
		case AutoTurretFireController.TurretState.Awake:
			if (this.hasTarget)
			{
				float yaw = this.atc.YawController.Yaw;
				float pitch = this.atc.PitchController.Pitch;
				Vector3 zero = Vector3.zero;
				if (!this.canHitEntity(ref yaw, ref pitch, out zero))
				{
					this.overshootTime += Time.deltaTime;
				}
				if (this.overshootTime >= this.overshootTimeMax)
				{
					this.currentEntityTarget = null;
					this.overshootTime = 0f;
					return;
				}
				this.fallAsleepTime = 0f;
				this.atc.YawController.Yaw = yaw;
				this.atc.PitchController.Pitch = pitch;
				if (this.burstRoundCount < this.burstRoundCountMax)
				{
					if (this.burstFireRate < this.burstFireRateMax)
					{
						this.burstFireRate += Time.deltaTime;
					}
					else
					{
						this.Fire();
						this.burstFireRate = 0f;
					}
				}
				else
				{
					this.state = AutoTurretFireController.TurretState.Overheated;
					this.burstRoundCount = 0;
				}
			}
			else if (this.currentEntityTarget != null && this.fallAsleepTime < this.fallAsleepTimeMax)
			{
				this.fallAsleepTime += Time.deltaTime;
			}
			else
			{
				this.currentEntityTarget = null;
				this.state = AutoTurretFireController.TurretState.Asleep;
				this.fallAsleepTime = 0f;
			}
			break;
		case AutoTurretFireController.TurretState.Overheated:
			if (this.coolOffTime == 0f)
			{
				this.broadcastPlay(this.overheatSound);
			}
			if (this.coolOffTime < this.coolOffTimeMax)
			{
				this.coolOffTime += Time.deltaTime;
			}
			else
			{
				this.state = AutoTurretFireController.TurretState.Awake;
				this.coolOffTime = 0f;
				this.broadcastStop(this.overheatSound);
			}
			break;
		}
		this.dispatchSoundCommandsThrottle(Time.deltaTime);
	}

	// Token: 0x06001A95 RID: 6805 RVA: 0x000A5280 File Offset: 0x000A3480
	[PublicizedFrom(EAccessModifier.Private)]
	public void findTarget()
	{
		Vector3 position = this.Cone.transform.position;
		this.currentEntityTarget = null;
		List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(typeof(EntityAlive), new Bounds(this.blockPos, Vector3.one * (this.maxDistance * 2f)), new List<Entity>());
		entitiesInBounds.Sort(this.sorter);
		bool flag = false;
		Collider[] array = Physics.OverlapSphere(position + Origin.position, 0.05f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject != this.atc.gameObject)
			{
				flag = true;
				break;
			}
		}
		if (entitiesInBounds.Count > 0 && !flag)
		{
			for (int j = 0; j < entitiesInBounds.Count; j++)
			{
				if (!this.shouldIgnoreTarget(entitiesInBounds[j]))
				{
					Vector3 zero = Vector3.zero;
					float centeredYaw = this.CenteredYaw;
					float centeredPitch = this.CenteredPitch;
					if (this.trackTarget(entitiesInBounds[j], ref centeredYaw, ref centeredPitch, out zero))
					{
						Vector3 normalized = (zero - position).normalized;
						Ray ray = new Ray(position + Origin.position - normalized * 0.05f, normalized);
						if (Voxel.Raycast(GameManager.Instance.World, ray, this.maxDistance + 0.05f, -538750981, 8, 0f) && Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
						{
							if (Voxel.voxelRayHitInfo.tag == "E_Vehicle")
							{
								EntityVehicle entityVehicle = EntityVehicle.FindCollisionEntity(Voxel.voxelRayHitInfo.transform);
								if (entityVehicle != null && entityVehicle.IsAttached(entitiesInBounds[j]))
								{
									this.currentEntityTarget = (entitiesInBounds[j] as EntityAlive);
									return;
								}
								this.currentEntityTarget = null;
							}
							else
							{
								Transform hitRootTransform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
								if (!(hitRootTransform == null))
								{
									Entity component = hitRootTransform.GetComponent<Entity>();
									if (component != null)
									{
										if (component == entitiesInBounds[j])
										{
											this.currentEntityTarget = (component as EntityAlive);
											return;
										}
										this.currentEntityTarget = null;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001A96 RID: 6806 RVA: 0x000A54E8 File Offset: 0x000A36E8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shouldIgnoreTarget(Entity _target)
	{
		if (Vector3.Dot(_target.position - this.TileEntity.ToWorldPos().ToVector3(), this.Cone.transform.forward) > 0f)
		{
			if (_target == this.currentEntityTarget)
			{
				this.currentEntityTarget = null;
			}
			return true;
		}
		if (!_target.IsAlive())
		{
			return true;
		}
		if (_target is EntitySupplyCrate)
		{
			return true;
		}
		if (_target is EntityVehicle)
		{
			Entity attachedMainEntity = (_target as EntityVehicle).AttachedMainEntity;
			if (attachedMainEntity == null)
			{
				return true;
			}
			_target = attachedMainEntity;
		}
		if (_target is EntityPlayer)
		{
			bool flag = false;
			bool flag2 = false;
			EnumPlayerKillingMode @int = (EnumPlayerKillingMode)GamePrefs.GetInt(EnumGamePrefs.PlayerKillingMode);
			PersistentPlayerList persistentPlayerList = GameManager.Instance.GetPersistentPlayerList();
			if (persistentPlayerList != null && persistentPlayerList.EntityToPlayerMap.ContainsKey(_target.entityId) && this.TileEntity.IsOwner(persistentPlayerList.EntityToPlayerMap[_target.entityId].PrimaryId))
			{
				flag = true;
			}
			if (!flag)
			{
				PersistentPlayerData playerData = persistentPlayerList.GetPlayerData(this.TileEntity.GetOwner());
				if (playerData != null && persistentPlayerList.EntityToPlayerMap.ContainsKey(_target.entityId))
				{
					PersistentPlayerData persistentPlayerData = persistentPlayerList.EntityToPlayerMap[_target.entityId];
					if (playerData.ACL != null && persistentPlayerData != null && playerData.ACL.Contains(persistentPlayerData.PrimaryId))
					{
						flag2 = true;
					}
				}
			}
			if (@int == EnumPlayerKillingMode.NoKilling)
			{
				return true;
			}
			if (flag && !this.TileEntity.TargetSelf)
			{
				return true;
			}
			if (flag2 && (!this.TileEntity.TargetAllies || (@int != EnumPlayerKillingMode.KillEveryone && @int != EnumPlayerKillingMode.KillAlliesOnly)))
			{
				return true;
			}
			if (!flag && !flag2 && (!this.TileEntity.TargetStrangers || (@int != EnumPlayerKillingMode.KillStrangersOnly && @int != EnumPlayerKillingMode.KillEveryone)))
			{
				return true;
			}
		}
		return _target is EntityTurret || _target is EntityDrone || (_target is EntityNPC && !this.TileEntity.TargetStrangers) || (_target is EntityEnemy && !this.TileEntity.TargetZombies) || (_target is EntityAnimal && !_target.EntityClass.bIsEnemyEntity);
	}

	// Token: 0x06001A97 RID: 6807 RVA: 0x000A56F4 File Offset: 0x000A38F4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canHitEntity(ref float _yaw, ref float _pitch, out Vector3 targetPos)
	{
		Vector3 origin = this.Cone.transform.position - Origin.position;
		if (!this.trackTarget(this.currentEntityTarget, ref _yaw, ref _pitch, out targetPos))
		{
			return false;
		}
		Ray ray = new Ray(origin, (targetPos - this.Cone.transform.position).normalized);
		if (Voxel.Raycast(GameManager.Instance.World, ray, this.maxDistance, -538750981, 8, 0f) && Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
		{
			Transform hitRootTransform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
			if (hitRootTransform == null)
			{
				return false;
			}
			Entity component = hitRootTransform.GetComponent<Entity>();
			if (component != null && component.IsAlive() && this.currentEntityTarget == component)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001A98 RID: 6808 RVA: 0x000A57E8 File Offset: 0x000A39E8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool trackTarget(Entity _target, ref float _yaw, ref float _pitch, out Vector3 _targetPos)
	{
		if (GameManager.Instance.World.GetGameRandom().RandomFloat < 0.05f)
		{
			_targetPos = _target.getHeadPosition() - Origin.position;
		}
		else
		{
			_targetPos = _target.getChestPosition() - Origin.position;
		}
		Vector3 normalized = (_targetPos - this.atc.YawController.transform.position).normalized;
		Vector3 normalized2 = (_targetPos - this.atc.PitchController.transform.position).normalized;
		float num = Quaternion.LookRotation(normalized).eulerAngles.y - this.atc.transform.rotation.eulerAngles.y;
		float num2 = Quaternion.LookRotation(normalized2).eulerAngles.x - this.atc.transform.rotation.z;
		if (num > 180f)
		{
			num -= 360f;
		}
		if (num2 > 180f)
		{
			num2 -= 360f;
		}
		float num3 = this.CenteredYaw % 360f;
		float num4 = this.CenteredPitch % 360f;
		if (num3 > 180f)
		{
			num3 -= 360f;
		}
		if (num4 > 180f)
		{
			num4 -= 360f;
		}
		if (num < num3 + this.yawRange.x || num > num3 + this.yawRange.y || num2 < num4 + this.pitchRange.x || num2 > num4 + this.pitchRange.y)
		{
			return false;
		}
		_yaw = num;
		_pitch = num2;
		return true;
	}

	// Token: 0x06001A99 RID: 6809 RVA: 0x000A599C File Offset: 0x000A3B9C
	public void PlayerFire(bool buttonPressed)
	{
		if (this.state == AutoTurretFireController.TurretState.Awake)
		{
			if (this.burstFireRate < this.burstFireRateMax)
			{
				this.burstFireRate += Time.deltaTime;
				return;
			}
			if (buttonPressed)
			{
				if (this.TileEntity.ClientData != null)
				{
					this.TileEntity.ClientData.SendSlots = true;
				}
				this.Fire();
				if (this.TileEntity.ClientData != null)
				{
					this.TileEntity.ClientData.SendSlots = false;
				}
				this.burstFireRate = 0f;
			}
		}
	}

	// Token: 0x06001A9A RID: 6810 RVA: 0x000A5A24 File Offset: 0x000A3C24
	public void Fire()
	{
		if (this.TileEntity != null)
		{
			if (!this.TileEntity.IsLocked)
			{
				return;
			}
			if ((SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !this.TileEntity.IsUserAccessing()) || (this.atc != null && this.atc.IsUserAccessing))
			{
				if (!this.TileEntity.DecrementAmmo())
				{
					this.TileEntity.IsLocked = false;
					this.TileEntity.SetModified();
					return;
				}
				this.burstRoundCount++;
			}
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || (this.atc != null && this.atc.IsUserAccessing))
		{
			Vector3 origin = this.Cone.position + Origin.position;
			Ray ray = new Ray(origin, Vector3.forward);
			Vector3 vector = this.Cone.forward * -1f;
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			for (int i = 0; i < this.rayCount; i++)
			{
				Vector3 vector2 = vector;
				vector2 = Quaternion.Euler(gameRandom.RandomRange(this.spread.x, this.spread.y), gameRandom.RandomRange(this.spread.x, this.spread.y), 0f) * vector2;
				ray.direction = vector2;
				this.waterCollisionParticles.Init(this.TileEntity.OwnerEntityID, "bullet", "water", 16);
				this.waterCollisionParticles.CheckCollision(ray.origin, ray.direction, this.maxDistance, -1);
				Voxel.Raycast(GameManager.Instance.World, ray, this.maxDistance, -538750981, 8, 0f);
				GameManager.Instance.GetPersistentPlayerList().GetPlayerData(this.TileEntity.GetOwner());
				ItemActionAttack.Hit(Voxel.voxelRayHitInfo.Clone(), this.TileEntity.OwnerEntityID, EnumDamageTypes.Bashing, (float)this.blockDamage, (float)this.entityDamage, 1f, 1f, 0.5f, 0.05f, "bullet", this.damageMultiplier, this.buffActions, new ItemActionAttack.AttackHitInfo(), 3, 0, 0f, null, null, ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, (this.atc != null && this.atc.IsUserAccessing) ? this.TileEntity.EntityId : -2, this.TileEntity.blockValue.ToItemValue());
			}
			if (!string.IsNullOrEmpty(this.muzzleFireParticle))
			{
				FireControllerUtils.SpawnParticleEffect(new ParticleEffect(this.muzzleFireParticle, this.Muzzle.position + Origin.position, this.Muzzle.rotation, 1f, Color.white, this.fireSound, null), -1);
			}
			if (!string.IsNullOrEmpty(this.muzzleSmokeParticle))
			{
				float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(this.BlockPosition)) / 2f;
				FireControllerUtils.SpawnParticleEffect(new ParticleEffect(this.muzzleSmokeParticle, this.Muzzle.position + Origin.position, this.Muzzle.rotation, lightValue, new Color(1f, 1f, 1f, 0.3f), null, null), -1);
			}
		}
	}

	// Token: 0x06001A9B RID: 6811 RVA: 0x000A5D7D File Offset: 0x000A3F7D
	public void OnPoweredOff()
	{
		this.broadcastStop(this.targetingSound);
		this.broadcastStop(this.overheatSound);
		this.broadcastStop(this.idleSound);
		this.dispatchSoundCommands();
	}

	// Token: 0x06001A9C RID: 6812 RVA: 0x000A5DA9 File Offset: 0x000A3FA9
	[PublicizedFrom(EAccessModifier.Private)]
	public void broadcastPlay(string name)
	{
		this.broadcastSoundAction(name, true);
	}

	// Token: 0x06001A9D RID: 6813 RVA: 0x000A5DB3 File Offset: 0x000A3FB3
	[PublicizedFrom(EAccessModifier.Private)]
	public void broadcastStop(string name)
	{
		this.broadcastSoundAction(name, false);
	}

	// Token: 0x06001A9E RID: 6814 RVA: 0x000A5DBD File Offset: 0x000A3FBD
	[PublicizedFrom(EAccessModifier.Private)]
	public void broadcastSoundAction(string name, bool play)
	{
		if (!string.IsNullOrEmpty(name))
		{
			this.soundCommandDictionary[name] = play;
		}
	}

	// Token: 0x06001A9F RID: 6815 RVA: 0x000A5DD4 File Offset: 0x000A3FD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void dispatchSoundCommands()
	{
		foreach (KeyValuePair<string, bool> keyValuePair in this.soundCommandDictionary)
		{
			if (keyValuePair.Value)
			{
				Manager.BroadcastPlay(this.blockPos, keyValuePair.Key, 0f);
			}
			else
			{
				Manager.BroadcastStop(this.blockPos, keyValuePair.Key);
			}
		}
		this.soundCommandDictionary.Clear();
	}

	// Token: 0x06001AA0 RID: 6816 RVA: 0x000A5E60 File Offset: 0x000A4060
	[PublicizedFrom(EAccessModifier.Private)]
	public void dispatchSoundCommandsThrottle(float deltaTime)
	{
		this.timeSinceDispatchSounds += Time.deltaTime;
		if (this.timeSinceDispatchSounds > 1f)
		{
			this.timeSinceDispatchSounds %= 1f;
			this.dispatchSoundCommands();
		}
	}

	// Token: 0x04001137 RID: 4407
	public bool IsOn;

	// Token: 0x04001138 RID: 4408
	public Transform Cone;

	// Token: 0x04001139 RID: 4409
	public Transform Laser;

	// Token: 0x0400113A RID: 4410
	public Transform Muzzle;

	// Token: 0x0400113B RID: 4411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 blockPos;

	// Token: 0x0400113C RID: 4412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConeYaw = 22.5f;

	// Token: 0x0400113D RID: 4413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConePitch = 22.5f;

	// Token: 0x0400113E RID: 4414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConeDistance = 5.25f;

	// Token: 0x0400113F RID: 4415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fireRateMax = 0.25f;

	// Token: 0x04001140 RID: 4416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float findTargetDelayMax = 0.5f;

	// Token: 0x04001141 RID: 4417
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float maxDistance;

	// Token: 0x04001142 RID: 4418
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int entityDamage;

	// Token: 0x04001143 RID: 4419
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int blockDamage;

	// Token: 0x04001144 RID: 4420
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int rayCount;

	// Token: 0x04001145 RID: 4421
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int raySpread;

	// Token: 0x04001146 RID: 4422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float wakeUpTime;

	// Token: 0x04001147 RID: 4423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float wakeUpTimeMax = 0.6522f;

	// Token: 0x04001148 RID: 4424
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallAsleepTime;

	// Token: 0x04001149 RID: 4425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallAsleepTimeMax = 10f;

	// Token: 0x0400114A RID: 4426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int burstRoundCount;

	// Token: 0x0400114B RID: 4427
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int burstRoundCountMax = 20;

	// Token: 0x0400114C RID: 4428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float burstFireRate;

	// Token: 0x0400114D RID: 4429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float burstFireRateMax = 0.1f;

	// Token: 0x0400114E RID: 4430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float coolOffTime;

	// Token: 0x0400114F RID: 4431
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float coolOffTimeMax = 2f;

	// Token: 0x04001150 RID: 4432
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float overshootTime;

	// Token: 0x04001151 RID: 4433
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float overshootTimeMax = 0.5f;

	// Token: 0x04001152 RID: 4434
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float retargetSoundTime;

	// Token: 0x04001153 RID: 4435
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float retargetSoundTimeMax = 0.874f;

	// Token: 0x04001154 RID: 4436
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Bounds targetingBounds;

	// Token: 0x04001155 RID: 4437
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AutoTurretFireController.TurretState state;

	// Token: 0x04001156 RID: 4438
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string fireSound;

	// Token: 0x04001157 RID: 4439
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string wakeUpSound;

	// Token: 0x04001158 RID: 4440
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string overheatSound;

	// Token: 0x04001159 RID: 4441
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string targetingSound;

	// Token: 0x0400115A RID: 4442
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string idleSound;

	// Token: 0x0400115B RID: 4443
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string muzzleFireParticle;

	// Token: 0x0400115C RID: 4444
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string muzzleSmokeParticle;

	// Token: 0x0400115D RID: 4445
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string ammoItemName;

	// Token: 0x0400115E RID: 4446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DamageMultiplier damageMultiplier;

	// Token: 0x0400115F RID: 4447
	public List<string> buffActions;

	// Token: 0x04001160 RID: 4448
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 yawRange;

	// Token: 0x04001161 RID: 4449
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 pitchRange;

	// Token: 0x04001162 RID: 4450
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 spread;

	// Token: 0x04001163 RID: 4451
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fireRate = -1f;

	// Token: 0x04001164 RID: 4452
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float findTargetDelay;

	// Token: 0x04001165 RID: 4453
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive currentEntityTarget;

	// Token: 0x04001166 RID: 4454
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AutoTurretController atc;

	// Token: 0x04001167 RID: 4455
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AutoTurretFireController.TurretEntitySorter sorter;

	// Token: 0x04001168 RID: 4456
	public TileEntityPoweredRangedTrap TileEntity;

	// Token: 0x04001169 RID: 4457
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public CollisionParticleController waterCollisionParticles = new CollisionParticleController();

	// Token: 0x0400116A RID: 4458
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTimeBetweenSoundDispatch = 1f;

	// Token: 0x0400116B RID: 4459
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeSinceDispatchSounds;

	// Token: 0x0400116C RID: 4460
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, bool> soundCommandDictionary = new Dictionary<string, bool>();

	// Token: 0x0400116D RID: 4461
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> soundsPlayOrder = new List<string>();

	// Token: 0x0200037F RID: 895
	[PublicizedFrom(EAccessModifier.Private)]
	public enum TurretState
	{
		// Token: 0x0400116F RID: 4463
		Asleep,
		// Token: 0x04001170 RID: 4464
		Awake,
		// Token: 0x04001171 RID: 4465
		Overheated
	}

	// Token: 0x02000380 RID: 896
	public class TurretEntitySorter : IComparer<Entity>
	{
		// Token: 0x06001AA2 RID: 6818 RVA: 0x000A5F3B File Offset: 0x000A413B
		public TurretEntitySorter(Vector3 _self)
		{
			this.self = _self;
		}

		// Token: 0x06001AA3 RID: 6819 RVA: 0x000A5F4C File Offset: 0x000A414C
		[PublicizedFrom(EAccessModifier.Private)]
		public int isNearer(Entity _e, Entity _other)
		{
			float num = this.DistanceSqr(this.self, _e.position);
			float num2 = this.DistanceSqr(this.self, _other.position);
			if (num < num2)
			{
				return -1;
			}
			if (num > num2)
			{
				return 1;
			}
			return 0;
		}

		// Token: 0x06001AA4 RID: 6820 RVA: 0x000A5F8C File Offset: 0x000A418C
		public int Compare(Entity _obj1, Entity _obj2)
		{
			return this.isNearer(_obj1, _obj2);
		}

		// Token: 0x06001AA5 RID: 6821 RVA: 0x000A5F98 File Offset: 0x000A4198
		public float DistanceSqr(Vector3 pointA, Vector3 pointB)
		{
			Vector3 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		// Token: 0x04001172 RID: 4466
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 self;
	}
}
