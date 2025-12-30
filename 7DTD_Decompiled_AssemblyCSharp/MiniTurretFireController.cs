using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine;

// Token: 0x02000387 RID: 903
public class MiniTurretFireController : MonoBehaviour
{
	// Token: 0x17000317 RID: 791
	// (get) Token: 0x06001ACD RID: 6861 RVA: 0x000A701B File Offset: 0x000A521B
	public bool IsOn
	{
		get
		{
			return this.entityTurret != null && this.entityTurret.IsOn;
		}
	}

	// Token: 0x17000318 RID: 792
	// (get) Token: 0x06001ACE RID: 6862 RVA: 0x000A7038 File Offset: 0x000A5238
	public Vector3 TurretPosition
	{
		get
		{
			return this.entityTurret.transform.position + Origin.position;
		}
	}

	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06001ACF RID: 6863 RVA: 0x000A7054 File Offset: 0x000A5254
	// (set) Token: 0x06001AD0 RID: 6864 RVA: 0x000A7061 File Offset: 0x000A5261
	public float CenteredYaw
	{
		get
		{
			return this.entityTurret.CenteredYaw;
		}
		set
		{
			this.entityTurret.CenteredYaw = value;
		}
	}

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06001AD1 RID: 6865 RVA: 0x000A706F File Offset: 0x000A526F
	// (set) Token: 0x06001AD2 RID: 6866 RVA: 0x000A707C File Offset: 0x000A527C
	public float CenteredPitch
	{
		get
		{
			return this.entityTurret.CenteredPitch;
		}
		set
		{
			this.entityTurret.CenteredPitch = value;
		}
	}

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06001AD3 RID: 6867 RVA: 0x000A708A File Offset: 0x000A528A
	public float MaxDistance
	{
		get
		{
			return this.maxDistance;
		}
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x000A7094 File Offset: 0x000A5294
	public void Init(DynamicProperties _properties, EntityTurret _entity)
	{
		this.entityTurret = _entity;
		this.Cone = this.entityTurret.Cone;
		this.Laser = this.entityTurret.Laser;
		this.Muzzle = this.entityTurret.transform.FindInChilds("Muzzle", false);
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
			this.spreadHorizontal = new Vector2(-num3, num3);
		}
		else
		{
			this.spreadHorizontal = new Vector2(-1f, 1f);
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
		if (this.entityTurret.YawController != null)
		{
			this.entityTurret.YawController.Init(_properties);
		}
		if (this.entityTurret.PitchController != null)
		{
			this.entityTurret.PitchController.Init(_properties);
		}
		this.buffActions = new List<string>();
		if (_properties.Values.ContainsKey("Buff"))
		{
			string[] collection = _properties.Values["Buff"].Replace(" ", "").Split(',', StringSplitOptions.None);
			this.buffActions.AddRange(collection);
		}
		this.damageMultiplier = new DamageMultiplier(_properties, null);
		this.sorter = new MiniTurretFireController.TurretEntitySorter(this.TurretPosition);
		this.state = MiniTurretFireController.TurretState.Asleep;
		if (this.Cone != null)
		{
			this.Cone.localScale = new Vector3(this.Cone.localScale.x * (this.yawRange.y / 22.5f) * (this.maxDistance / 5.25f), this.Cone.localScale.y * (this.pitchRange.y / 22.5f) * (this.maxDistance / 5.25f), this.Cone.localScale.z * (this.maxDistance / 5.25f));
			this.Cone.gameObject.SetActive(false);
		}
		if (this.Laser != null)
		{
			this.Laser.localScale = new Vector3(this.Laser.localScale.x, this.Laser.localScale.y, this.Laser.localScale.z * this.maxDistance);
			this.Laser.gameObject.SetActive(false);
		}
		this.entityTurret.transform.GetComponent<SphereCollider>().enabled = true;
		this.waterCollisionParticles.Init(this.entityTurret.belongsPlayerId, "bullet", "water", 16);
	}

	// Token: 0x06001AD5 RID: 6869 RVA: 0x000A76D8 File Offset: 0x000A58D8
	public virtual float GetRange(EntityAlive owner)
	{
		return EffectManager.GetValue(PassiveEffects.MaxRange, this.entityTurret.OriginalItemValue, this.maxDistance, owner, null, default(FastTags<TagGroup.Global>), true, false, true, true, true, 1, true, false);
	}

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06001AD6 RID: 6870 RVA: 0x000A7710 File Offset: 0x000A5910
	public bool hasTarget
	{
		get
		{
			return this.currentEntityTarget != null;
		}
	}

	// Token: 0x06001AD7 RID: 6871 RVA: 0x000A7720 File Offset: 0x000A5920
	public void Update()
	{
		if (this.entityTurret == null)
		{
			return;
		}
		if (!this.entityTurret.IsOn)
		{
			if (this.entityTurret.YawController.Yaw != this.CenteredYaw)
			{
				this.entityTurret.YawController.Yaw = this.CenteredYaw;
			}
			if (this.entityTurret.PitchController.Pitch != this.CenteredPitch + 35f)
			{
				this.entityTurret.PitchController.Pitch = this.CenteredPitch + 35f;
			}
			if (this.turretSpinAudioHandle != null)
			{
				this.turretSpinAudioHandle.Stop(this.entityTurret.entityId);
				this.turretSpinAudioHandle = null;
			}
			this.entityTurret.YawController.UpdateYaw();
			this.entityTurret.PitchController.UpdatePitch();
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (!this.hasTarget)
			{
				this.findTarget();
			}
			else if (this.shouldIgnoreTarget(this.currentEntityTarget))
			{
				this.currentEntityTarget = null;
				this.entityTurret.TargetEntityId = -1;
			}
		}
		else if (this.entityTurret.TargetEntityId != -1)
		{
			this.currentEntityTarget = (GameManager.Instance.World.GetEntity(this.entityTurret.TargetEntityId) as EntityAlive);
			if (this.currentEntityTarget == null || this.currentEntityTarget.IsDead())
			{
				this.currentEntityTarget = null;
				this.entityTurret.TargetEntityId = -1;
			}
		}
		else
		{
			this.currentEntityTarget = null;
			this.entityTurret.TargetEntityId = -1;
		}
		if (this.entityTurret.IsTurning)
		{
			if (this.turretSpinAudioHandle == null)
			{
				this.turretSpinAudioHandle = Manager.Play(this.entityTurret, this.targetingSound, 1f, true);
			}
		}
		else if (this.turretSpinAudioHandle != null)
		{
			this.turretSpinAudioHandle.Stop(this.entityTurret.entityId);
			this.turretSpinAudioHandle = null;
		}
		this.targetChestHeadDelay -= Time.deltaTime;
		if (this.targetChestHeadDelay <= 0f)
		{
			this.targetChestHeadDelay = 1f;
			this.targetChestHeadPercent = this.entityTurret.rand.RandomFloat;
		}
		this.burstFireRate += Time.deltaTime;
		if (this.hasTarget)
		{
			this.entityTurret.YawController.IdleScan = false;
			float yaw = this.entityTurret.YawController.Yaw;
			float pitch = this.entityTurret.PitchController.Pitch;
			Vector3 vector;
			if (this.trackTarget(this.currentEntityTarget, ref yaw, ref pitch, out vector))
			{
				this.entityTurret.YawController.Yaw = yaw;
				this.entityTurret.PitchController.Pitch = pitch;
				EntityAlive entity = GameManager.Instance.World.GetEntity(this.entityTurret.belongsPlayerId) as EntityAlive;
				FastTags<TagGroup.Global> tags = this.entityTurret.OriginalItemValue.ItemClass.ItemTags | this.entityTurret.EntityClass.Tags;
				this.burstFireRateMax = 60f / (EffectManager.GetValue(PassiveEffects.RoundsPerMinute, this.entityTurret.OriginalItemValue, this.burstFireRateMax, entity, null, tags, true, false, true, true, true, 1, true, false) + 1E-05f);
				if (this.burstFireRate >= this.burstFireRateMax)
				{
					this.Fire();
					this.burstFireRate = 0f;
				}
			}
		}
		else
		{
			if (!this.entityTurret.YawController.IdleScan || (this.entityTurret.YawController.Yaw != this.yawRange.y && this.entityTurret.YawController.Yaw != this.yawRange.x))
			{
				this.entityTurret.YawController.IdleScan = true;
				this.entityTurret.YawController.Yaw = this.yawRange.y;
			}
			float num = (this.yawRange.y > 0f) ? this.yawRange.y : (360f + this.yawRange.y);
			float num2 = (this.yawRange.x > 0f) ? this.yawRange.x : (360f + this.yawRange.x);
			if (Mathf.Abs(this.entityTurret.YawController.CurrentYaw - num) < 1f || Mathf.Abs(this.entityTurret.YawController.CurrentYaw - this.yawRange.y) < 1f)
			{
				this.entityTurret.YawController.Yaw = this.yawRange.x;
			}
			else if (Mathf.Abs(this.entityTurret.YawController.CurrentYaw - num2) < 1f || Mathf.Abs(this.entityTurret.YawController.CurrentYaw - this.yawRange.x) < 1f)
			{
				this.entityTurret.YawController.Yaw = this.yawRange.y;
			}
			this.entityTurret.PitchController.Pitch = this.CenteredPitch;
		}
		this.entityTurret.YawController.UpdateYaw();
		this.entityTurret.PitchController.UpdatePitch();
		if (this.Laser != null)
		{
			this.updateLaser();
		}
	}

	// Token: 0x06001AD8 RID: 6872 RVA: 0x000A7C6C File Offset: 0x000A5E6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void findTarget()
	{
		Vector3 position = base.transform.position;
		if (this.Cone != null)
		{
			position = this.Cone.transform.position;
		}
		this.currentEntityTarget = null;
		this.entityTurret.TargetEntityId = -1;
		if (GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(typeof(EntityAlive), new Bounds(this.TurretPosition, Vector3.one * (this.maxDistance * 2f + 1f)), new List<Entity>());
		entitiesInBounds.Sort(this.sorter);
		if (entitiesInBounds.Count > 0)
		{
			for (int i = 0; i < entitiesInBounds.Count; i++)
			{
				Entity entity = entitiesInBounds[i];
				if (!this.shouldIgnoreTarget(entity))
				{
					Vector3 zero = Vector3.zero;
					float centeredYaw = this.CenteredYaw;
					float centeredPitch = this.CenteredPitch;
					if (this.trackTarget(entity, ref centeredYaw, ref centeredPitch, out zero))
					{
						Vector3 normalized = (zero - position).normalized;
						Ray ray = new Ray(position + Origin.position - normalized * 0.05f, normalized);
						if (Voxel.Raycast(GameManager.Instance.World, ray, this.maxDistance + 0.05f, -538750981, 8, 0.05f) && Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
						{
							Transform hitRootTransform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
							if (!(hitRootTransform == null))
							{
								Entity component = hitRootTransform.GetComponent<Entity>();
								if (component != null)
								{
									if (component == entity)
									{
										this.currentEntityTarget = (component as EntityAlive);
										this.entityTurret.TargetEntityId = this.currentEntityTarget.entityId;
										this.entityTurret.YawController.Yaw = centeredYaw;
										this.entityTurret.PitchController.Pitch = centeredPitch;
										return;
									}
									this.currentEntityTarget = null;
									this.entityTurret.TargetEntityId = -1;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06001AD9 RID: 6873 RVA: 0x000A7EA0 File Offset: 0x000A60A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateLaser()
	{
		float num = this.maxDistance;
		Ray ray = new Ray(this.Laser.transform.position + Origin.position, -this.Laser.transform.forward);
		if (Voxel.Raycast(GameManager.Instance.World, ray, num, 1082195968, 128, 0.25f))
		{
			num = Vector3.Distance(Voxel.voxelRayHitInfo.hit.pos - Origin.position, ray.origin - Origin.position);
		}
		this.Laser.transform.localScale = new Vector3(1f, 1f, num);
	}

	// Token: 0x06001ADA RID: 6874 RVA: 0x000A7F5C File Offset: 0x000A615C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shouldIgnoreTarget(Entity _target)
	{
		if (_target == null)
		{
			return true;
		}
		Vector3 forward = base.transform.forward;
		if (this.Cone != null)
		{
			forward = this.Cone.transform.forward;
		}
		if (Vector3.Dot(_target.position - this.entityTurret.position, forward) > 0f)
		{
			return true;
		}
		if (!_target.IsAlive())
		{
			return true;
		}
		if (_target.entityId == this.entityTurret.entityId)
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
			if (this.entityTurret.belongsPlayerId == _target.entityId)
			{
				flag = true;
			}
			if (!flag && persistentPlayerList.EntityToPlayerMap.ContainsKey(this.entityTurret.belongsPlayerId))
			{
				PersistentPlayerData persistentPlayerData = persistentPlayerList.EntityToPlayerMap[this.entityTurret.belongsPlayerId];
				if (persistentPlayerData != null && persistentPlayerList.EntityToPlayerMap.ContainsKey(_target.entityId))
				{
					PersistentPlayerData persistentPlayerData2 = persistentPlayerList.EntityToPlayerMap[_target.entityId];
					if (persistentPlayerData.ACL != null && persistentPlayerData2 != null && persistentPlayerData.ACL.Contains(persistentPlayerData2.PrimaryId))
					{
						flag2 = true;
					}
				}
				EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(this.entityTurret.belongsPlayerId) as EntityPlayer;
				if (!flag2 && entityPlayer != null && entityPlayer.Party != null && entityPlayer.Party.ContainsMember(_target as EntityPlayer))
				{
					flag2 = true;
				}
			}
			if (@int == EnumPlayerKillingMode.NoKilling)
			{
				return true;
			}
			if (flag && (!this.entityTurret.TargetOwner || @int != EnumPlayerKillingMode.KillEveryone))
			{
				return true;
			}
			if (flag2 && (!this.entityTurret.TargetAllies || (@int != EnumPlayerKillingMode.KillEveryone && @int != EnumPlayerKillingMode.KillAlliesOnly)))
			{
				return true;
			}
			if (!flag && !flag2 && (!this.entityTurret.TargetStrangers || (@int != EnumPlayerKillingMode.KillStrangersOnly && @int != EnumPlayerKillingMode.KillEveryone)))
			{
				return true;
			}
		}
		if (_target is EntityNPC)
		{
			if (_target is EntityTrader)
			{
				return true;
			}
			if (!this.entityTurret.TargetStrangers)
			{
				return true;
			}
		}
		if (_target is EntityEnemy && !this.entityTurret.TargetEnemies)
		{
			return true;
		}
		if (_target is EntityTurret)
		{
			return true;
		}
		if (_target is EntityDrone)
		{
			return true;
		}
		if (_target is EntitySupplyCrate)
		{
			return true;
		}
		float num = 0f;
		float num2 = 0f;
		Vector3 vector;
		return _target as EntityAlive != null && !this.canHitEntity(_target, ref num, ref num2, out vector);
	}

	// Token: 0x06001ADB RID: 6875 RVA: 0x000A8200 File Offset: 0x000A6400
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canHitEntity(Entity _targetEntity, ref float _yaw, ref float _pitch, out Vector3 targetPos)
	{
		Vector3 position = base.transform.position;
		if (this.Cone != null)
		{
			position = this.Cone.transform.position;
		}
		if (!this.trackTarget(_targetEntity, ref _yaw, ref _pitch, out targetPos))
		{
			return false;
		}
		Ray ray = new Ray(position + Origin.position, (targetPos - position).normalized);
		if (Voxel.Raycast(GameManager.Instance.World, ray, this.maxDistance, -538750981, 8, 0f) && Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
		{
			Transform hitRootTransform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
			if (hitRootTransform == null)
			{
				return false;
			}
			Entity component = hitRootTransform.GetComponent<Entity>();
			if (component != null && component.IsAlive() && _targetEntity == component)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001ADC RID: 6876 RVA: 0x000A82F4 File Offset: 0x000A64F4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool trackTarget(Entity _target, ref float _yaw, ref float _pitch, out Vector3 _targetPos)
	{
		_targetPos = Vector3.Lerp(_target.getChestPosition(), _target.getHeadPosition(), this.targetChestHeadPercent) - Origin.position;
		Vector3 position = base.transform.position;
		if (this.Laser != null)
		{
			position = this.Laser.transform.position;
		}
		Vector3 eulerAngles = Quaternion.LookRotation((_targetPos - position).normalized).eulerAngles;
		float num = Mathf.DeltaAngle(this.entityTurret.transform.rotation.eulerAngles.y, eulerAngles.y);
		float num2 = eulerAngles.x;
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

	// Token: 0x06001ADD RID: 6877 RVA: 0x000A844C File Offset: 0x000A664C
	public virtual void Fire()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || this.entityTurret == null || !this.entityTurret.IsOn)
		{
			return;
		}
		Vector3 position = this.Laser.transform.position;
		EntityAlive entity = GameManager.Instance.World.GetEntity(this.entityTurret.belongsPlayerId) as EntityAlive;
		GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
		FastTags<TagGroup.Global> itemTags = this.entityTurret.OriginalItemValue.ItemClass.ItemTags;
		int num = (int)EffectManager.GetValue(PassiveEffects.RoundRayCount, this.entityTurret.OriginalItemValue, (float)this.rayCount, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
		this.maxDistance = EffectManager.GetValue(PassiveEffects.MaxRange, this.entityTurret.OriginalItemValue, this.MaxDistance, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
		for (int i = 0; i < num; i++)
		{
			Vector3 vector = this.Muzzle.transform.forward;
			this.spreadHorizontal.x = -(EffectManager.GetValue(PassiveEffects.SpreadDegreesHorizontal, this.entityTurret.OriginalItemValue, 22f, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false) * 0.5f);
			this.spreadHorizontal.y = EffectManager.GetValue(PassiveEffects.SpreadDegreesHorizontal, this.entityTurret.OriginalItemValue, 22f, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false) * 0.5f;
			this.spreadVertical.x = -(EffectManager.GetValue(PassiveEffects.SpreadDegreesVertical, this.entityTurret.OriginalItemValue, 22f, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false) * 0.5f);
			this.spreadVertical.y = EffectManager.GetValue(PassiveEffects.SpreadDegreesVertical, this.entityTurret.OriginalItemValue, 22f, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false) * 0.5f;
			vector = Quaternion.Euler(gameRandom.RandomRange(this.spreadHorizontal.x, this.spreadHorizontal.y), gameRandom.RandomRange(this.spreadVertical.x, this.spreadVertical.y), 0f) * vector;
			Ray ray = new Ray(position + Origin.position, vector);
			this.waterCollisionParticles.Reset();
			this.waterCollisionParticles.CheckCollision(ray.origin, ray.direction, this.maxDistance, -1);
			int num2 = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.EntityPenetrationCount, this.entityTurret.OriginalItemValue, 0f, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false));
			num2++;
			int num3 = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.BlockPenetrationFactor, this.entityTurret.OriginalItemValue, 1f, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false));
			EntityAlive x = null;
			for (int j = 0; j < num2; j++)
			{
				if (Voxel.Raycast(GameManager.Instance.World, ray, this.maxDistance, -538750981, 8, 0f))
				{
					WorldRayHitInfo worldRayHitInfo = Voxel.voxelRayHitInfo.Clone();
					if (worldRayHitInfo.tag.StartsWith("E_"))
					{
						string text;
						EntityAlive entityAlive = ItemActionAttack.FindHitEntityNoTagCheck(worldRayHitInfo, out text) as EntityAlive;
						if (x == entityAlive)
						{
							ray.origin = worldRayHitInfo.hit.pos + ray.direction * 0.1f;
							j--;
							goto IL_4E2;
						}
						x = entityAlive;
					}
					else
					{
						j += Mathf.FloorToInt((float)ItemActionAttack.GetBlockHit(GameManager.Instance.World, worldRayHitInfo).Block.MaxDamage / (float)num3);
					}
					ItemActionAttack.Hit(worldRayHitInfo, this.entityTurret.belongsPlayerId, EnumDamageTypes.Piercing, this.GetDamageBlock(this.entityTurret.OriginalItemValue, BlockValue.Air, GameManager.Instance.World.GetEntity(this.entityTurret.belongsPlayerId) as EntityAlive, 1), this.GetDamageEntity(this.entityTurret.OriginalItemValue, GameManager.Instance.World.GetEntity(this.entityTurret.belongsPlayerId) as EntityAlive, 1), 1f, this.entityTurret.OriginalItemValue.PercentUsesLeft, 0f, 0f, "bullet", this.damageMultiplier, this.buffActions, new ItemActionAttack.AttackHitInfo(), 1, 0, 0f, null, null, ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, this.entityTurret.entityId, this.entityTurret.OriginalItemValue);
				}
				IL_4E2:;
			}
		}
		if (!string.IsNullOrEmpty(this.muzzleFireParticle))
		{
			FireControllerUtils.SpawnParticleEffect(new ParticleEffect(this.muzzleFireParticle, this.Muzzle.position + Origin.position, this.Muzzle.rotation, 1f, Color.white, this.fireSound, null), -1);
		}
		if (!string.IsNullOrEmpty(this.muzzleSmokeParticle))
		{
			float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(this.TurretPosition)) / 2f;
			FireControllerUtils.SpawnParticleEffect(new ParticleEffect(this.muzzleSmokeParticle, this.Muzzle.position + Origin.position, this.Muzzle.rotation, lightValue, new Color(1f, 1f, 1f, 0.3f), null, null), -1);
		}
		this.burstRoundCount++;
		if ((int)EffectManager.GetValue(PassiveEffects.MagazineSize, this.entityTurret.OriginalItemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0)
		{
			EntityTurret entityTurret = this.entityTurret;
			int ammoCount = entityTurret.AmmoCount;
			entityTurret.AmmoCount = ammoCount - 1;
		}
		this.entityTurret.OriginalItemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, this.entityTurret.OriginalItemValue, 1f, entity, null, this.entityTurret.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
	}

	// Token: 0x06001ADE RID: 6878 RVA: 0x000A8AC4 File Offset: 0x000A6CC4
	public float GetDamageEntity(ItemValue _itemValue, EntityAlive _holdingEntity = null, int actionIndex = 0)
	{
		return EffectManager.GetValue(PassiveEffects.EntityDamage, _itemValue, (float)this.entityDamage, _holdingEntity, null, _itemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
	}

	// Token: 0x06001ADF RID: 6879 RVA: 0x000A8AF4 File Offset: 0x000A6CF4
	public float GetDamageBlock(ItemValue _itemValue, BlockValue _blockValue, EntityAlive _holdingEntity = null, int actionIndex = 0)
	{
		this.tmpTag = _itemValue.ItemClass.ItemTags;
		this.tmpTag |= _blockValue.Block.Tags;
		float value = EffectManager.GetValue(PassiveEffects.BlockDamage, _itemValue, (float)this.blockDamage, _holdingEntity, null, this.tmpTag, true, false, true, true, true, 1, true, false);
		return Utils.FastMin((float)_blockValue.Block.blockMaterial.MaxIncomingDamage, value);
	}

	// Token: 0x040011A1 RID: 4513
	public Transform Cone;

	// Token: 0x040011A2 RID: 4514
	public Transform Laser;

	// Token: 0x040011A3 RID: 4515
	public Transform Muzzle;

	// Token: 0x040011A4 RID: 4516
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConeYaw = 22.5f;

	// Token: 0x040011A5 RID: 4517
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConePitch = 22.5f;

	// Token: 0x040011A6 RID: 4518
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float baseConeDistance = 5.25f;

	// Token: 0x040011A7 RID: 4519
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float maxDistance;

	// Token: 0x040011A8 RID: 4520
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int entityDamage;

	// Token: 0x040011A9 RID: 4521
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int blockDamage;

	// Token: 0x040011AA RID: 4522
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int rayCount;

	// Token: 0x040011AB RID: 4523
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float wakeUpTimeMax = 0.6522f;

	// Token: 0x040011AC RID: 4524
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float fallAsleepTimeMax = 10f;

	// Token: 0x040011AD RID: 4525
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int burstRoundCount;

	// Token: 0x040011AE RID: 4526
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int burstRoundCountMax = 20;

	// Token: 0x040011AF RID: 4527
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float burstFireRate;

	// Token: 0x040011B0 RID: 4528
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float burstFireRateMax = 0.1f;

	// Token: 0x040011B1 RID: 4529
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float coolOffTimeMax = 2f;

	// Token: 0x040011B2 RID: 4530
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float overshootTimeMax = 0.5f;

	// Token: 0x040011B3 RID: 4531
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MiniTurretFireController.TurretState state;

	// Token: 0x040011B4 RID: 4532
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string fireSound;

	// Token: 0x040011B5 RID: 4533
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string wakeUpSound;

	// Token: 0x040011B6 RID: 4534
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string overheatSound;

	// Token: 0x040011B7 RID: 4535
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string targetingSound;

	// Token: 0x040011B8 RID: 4536
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string muzzleFireParticle;

	// Token: 0x040011B9 RID: 4537
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string muzzleSmokeParticle;

	// Token: 0x040011BA RID: 4538
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string ammoItemName;

	// Token: 0x040011BB RID: 4539
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public DamageMultiplier damageMultiplier;

	// Token: 0x040011BC RID: 4540
	public List<string> buffActions;

	// Token: 0x040011BD RID: 4541
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 yawRange;

	// Token: 0x040011BE RID: 4542
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 pitchRange;

	// Token: 0x040011BF RID: 4543
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 spreadHorizontal;

	// Token: 0x040011C0 RID: 4544
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 spreadVertical;

	// Token: 0x040011C1 RID: 4545
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive currentEntityTarget;

	// Token: 0x040011C2 RID: 4546
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float targetChestHeadDelay;

	// Token: 0x040011C3 RID: 4547
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float targetChestHeadPercent;

	// Token: 0x040011C4 RID: 4548
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MiniTurretFireController.TurretEntitySorter sorter;

	// Token: 0x040011C5 RID: 4549
	public EntityTurret entityTurret;

	// Token: 0x040011C6 RID: 4550
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public CollisionParticleController waterCollisionParticles = new CollisionParticleController();

	// Token: 0x040011C7 RID: 4551
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Handle turretSpinAudioHandle;

	// Token: 0x040011C8 RID: 4552
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSeekRayRadius = 0.05f;

	// Token: 0x040011C9 RID: 4553
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public FastTags<TagGroup.Global> tmpTag;

	// Token: 0x040011CA RID: 4554
	public static FastTags<TagGroup.Global> RangedTag = FastTags<TagGroup.Global>.Parse("ranged");

	// Token: 0x040011CB RID: 4555
	public static FastTags<TagGroup.Global> MeleeTag = FastTags<TagGroup.Global>.Parse("melee");

	// Token: 0x040011CC RID: 4556
	public static FastTags<TagGroup.Global> PrimaryTag = FastTags<TagGroup.Global>.Parse("primary");

	// Token: 0x040011CD RID: 4557
	public static FastTags<TagGroup.Global> SecondaryTag = FastTags<TagGroup.Global>.Parse("secondary");

	// Token: 0x040011CE RID: 4558
	public static FastTags<TagGroup.Global> TurretTag = FastTags<TagGroup.Global>.Parse("turret");

	// Token: 0x02000388 RID: 904
	[PublicizedFrom(EAccessModifier.Private)]
	public enum TurretState
	{
		// Token: 0x040011D0 RID: 4560
		Asleep,
		// Token: 0x040011D1 RID: 4561
		Awake,
		// Token: 0x040011D2 RID: 4562
		Overheated
	}

	// Token: 0x02000389 RID: 905
	public class TurretEntitySorter : IComparer<Entity>
	{
		// Token: 0x06001AE2 RID: 6882 RVA: 0x000A8C20 File Offset: 0x000A6E20
		public TurretEntitySorter(Vector3 _self)
		{
			this.self = _self;
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x000A8C30 File Offset: 0x000A6E30
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

		// Token: 0x06001AE4 RID: 6884 RVA: 0x000A8C70 File Offset: 0x000A6E70
		public int Compare(Entity _obj1, Entity _obj2)
		{
			return this.isNearer(_obj1, _obj2);
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x000A8C7C File Offset: 0x000A6E7C
		public float DistanceSqr(Vector3 pointA, Vector3 pointB)
		{
			Vector3 vector = pointA - pointB;
			return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
		}

		// Token: 0x040011D3 RID: 4563
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 self;
	}
}
