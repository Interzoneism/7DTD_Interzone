using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Audio;
using GamePath;
using Platform;
using UAI;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000410 RID: 1040
[Preserve]
public abstract class EntityAlive : Entity
{
	// Token: 0x17000373 RID: 883
	// (get) Token: 0x06001F0F RID: 7951 RVA: 0x000C0E06 File Offset: 0x000BF006
	// (set) Token: 0x06001F10 RID: 7952 RVA: 0x000C0E11 File Offset: 0x000BF011
	public bool IsEquipping
	{
		get
		{
			return this.equippingCount > 0;
		}
		set
		{
			if (value)
			{
				this.equippingCount++;
				return;
			}
			if (this.equippingCount > 0)
			{
				this.equippingCount--;
			}
		}
	}

	// Token: 0x17000374 RID: 884
	// (get) Token: 0x06001F11 RID: 7953 RVA: 0x000C0E3C File Offset: 0x000BF03C
	// (set) Token: 0x06001F12 RID: 7954 RVA: 0x000C0E44 File Offset: 0x000BF044
	public bool IsDancing
	{
		get
		{
			return this.isDancing;
		}
		set
		{
			this.isDancing = value;
			if (value)
			{
				if (this.emodel != null && this.emodel.avatarController != null)
				{
					this.emodel.avatarController.UpdateInt("IsDancing", base.EntityClass.DanceTypeID, true);
					return;
				}
			}
			else if (this.emodel != null && this.emodel.avatarController != null)
			{
				this.emodel.avatarController.UpdateInt("IsDancing", 0, true);
			}
		}
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000C0ED6 File Offset: 0x000BF0D6
	public void BeginDynamicRagdoll(DynamicRagdollFlags flags, FloatRange stunTime)
	{
		this._dynamicRagdoll = flags;
		this._dynamicRagdollRootMotion = Vector3.zero;
		this._dynamicRagdollStunTime = stunTime.Random(this.rand);
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000C0F00 File Offset: 0x000BF100
	public void ActivateDynamicRagdoll()
	{
		if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.Active))
		{
			DynamicRagdollFlags dynamicRagdoll = this._dynamicRagdoll;
			this._dynamicRagdoll = DynamicRagdollFlags.None;
			Vector3 forceVec = this._dynamicRagdollRootMotion * 20f;
			this.bodyDamage.StunDuration = this._dynamicRagdollStunTime;
			this.emodel.DoRagdoll(this._dynamicRagdollStunTime, EnumBodyPartHit.None, forceVec, Vector3.zero, true);
			if (dynamicRagdoll.HasFlag(DynamicRagdollFlags.UseBoneVelocities) && this._ragdollPositionsPrev.Count == this._ragdollPositionsCur.Count)
			{
				List<Vector3> list = new List<Vector3>();
				for (int i = 0; i < this._ragdollPositionsPrev.Count; i++)
				{
					Vector3 a = this._ragdollPositionsCur[i] - this._ragdollPositionsPrev[i];
					list.Add(a * 20f);
				}
				this.emodel.ApplyRagdollVelocities(list);
			}
		}
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x000C0FF8 File Offset: 0x000BF1F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		this.entityName = base.GetType().Name;
		this.MinEventContext.Self = this;
		this.seeCache = new EntitySeeCache(this);
		this.maximumHomeDistance = -1;
		this.homePosition = new ChunkCoordinates(0, 0, 0);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !(this is EntityPlayer))
		{
			this.hasAI = true;
			this.navigator = AstarManager.CreateNavigator(this);
			this.aiManager = new EAIManager(this);
			this.lookHelper = new EntityLookHelper(this);
			this.moveHelper = new EntityMoveHelper(this);
		}
		this.equipment = new Equipment(this);
		this.InitInventory();
		if (this.bag == null)
		{
			this.bag = new Bag(this);
		}
		this.stepHeight = 0.52f;
		this.soundDelayTicks = this.GetSoundRandomTicks() / 3 - 5;
		this.spawnPoints = new EntityBedrollPositionList(this);
		this.CreationTimeSinceLevelLoad = Time.timeSinceLevelLoad;
		this.Buffs = new EntityBuffs(this);
		this.droppedBackpackPositions = new List<Vector3i>();
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000C1103 File Offset: 0x000BF303
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		this.InitStats();
		this.switchModelView(EnumEntityModelView.ThirdPerson);
		this.InitPostCommon();
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x000C111F File Offset: 0x000BF31F
	public override void InitFromPrefab(int _entityClass)
	{
		base.InitFromPrefab(_entityClass);
		this.switchModelView(EnumEntityModelView.ThirdPerson);
		this.InitPostCommon();
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000C1138 File Offset: 0x000BF338
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitPostCommon()
	{
		if (GameManager.IsDedicatedServer)
		{
			Transform modelTransform = this.emodel.GetModelTransform();
			if (modelTransform)
			{
				ServerHelper.SetupForServer(modelTransform.gameObject);
			}
		}
		this.AddCharacterController();
		this.wasSeenByPlayer = false;
		this.ticksToCheckSeenByPlayer = 20;
		if (EntityClass.list[this.entityClass].UseAIPackages)
		{
			this.hasAI = true;
			this.AIPackages = new List<string>();
			this.AIPackages.AddRange(EntityClass.list[this.entityClass].AIPackages);
			this.utilityAIContext = new Context(this);
		}
		List<string> buffs = EntityClass.list[this.entityClass].Buffs;
		if (buffs != null)
		{
			for (int i = 0; i < buffs.Count; i++)
			{
				string name = buffs[i];
				if (!this.Buffs.HasBuff(name))
				{
					this.Buffs.AddBuff(name, -1, true, false, -1f);
				}
			}
		}
		if ((this.entityFlags & EntityFlags.AIHearing) > EntityFlags.None)
		{
			this.emodel.SetVisible(false, false);
			this.emodel.SetFade(0f);
		}
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x000C1254 File Offset: 0x000BF454
	public override void PostInit()
	{
		base.PostInit();
		this.ApplySpawnState();
		LODGroup componentInChildren = this.emodel.GetModelTransform().GetComponentInChildren<LODGroup>();
		if (componentInChildren)
		{
			LOD[] lods = componentInChildren.GetLODs();
			lods[lods.Length - 1].screenRelativeTransitionHeight = 0.003f;
			componentInChildren.SetLODs(lods);
		}
		this.disableFallBehaviorUntilOnGround = true;
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x000C12B0 File Offset: 0x000BF4B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplySpawnState()
	{
		if (this.Health <= 0 && this.isEntityRemote)
		{
			this.ClientKill(DamageResponse.New(true));
		}
		this.ExecuteDismember(true);
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000C12D6 File Offset: 0x000BF4D6
	public virtual void InitInventory()
	{
		if (this.inventory == null)
		{
			this.inventory = new Inventory(GameManager.Instance, this);
		}
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000C12F1 File Offset: 0x000BF4F1
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void switchModelView(EnumEntityModelView modelView)
	{
		this.emodel.SwitchModelAndView(modelView == EnumEntityModelView.FirstPerson, this.IsMale);
		this.ReassignEquipmentTransforms();
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000C1310 File Offset: 0x000BF510
	public virtual void ReassignEquipmentTransforms()
	{
		if (this.isFirstTimeEquipmentReassigned)
		{
			this.Buffs.SetCustomVar("_equipReload", 0f, true, CVarOperation.set);
			this.isFirstTimeEquipmentReassigned = false;
		}
		else
		{
			this.Buffs.SetCustomVar("_equipReload", 1f, true, CVarOperation.set);
		}
		this.equipment.InitializeEquipmentTransforms();
		this.Buffs.SetCustomVar("_equipReload", 0f, true, CVarOperation.set);
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x000C1380 File Offset: 0x000BF580
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		string @string = entityClass.Properties.GetString(EntityClass.PropHandItem);
		if (@string.Length > 0)
		{
			this.handItem = ItemClass.GetItem(@string, false);
			if (this.handItem.IsEmpty())
			{
				throw new Exception("Item with name '" + @string + "' not found!");
			}
		}
		else
		{
			this.handItem = ItemClass.GetItem("meleeHandPlayer", false).Clone();
		}
		if (this.inventory != null)
		{
			this.inventory.SetBareHandItem(this.handItem);
		}
		this.rightHandTransformName = "Gunjoint";
		if (this.emodel is EModelSDCS)
		{
			this.rightHandTransformName = "RightWeapon";
		}
		entityClass.Properties.ParseString(EntityClass.PropRightHandJointName, ref this.rightHandTransformName);
		if (!(this is EntityPlayer))
		{
			this.factionId = 0;
			this.factionRank = 0;
			string string2 = entityClass.Properties.GetString("Faction");
			if (string2.Length > 0)
			{
				Faction factionByName = FactionManager.Instance.GetFactionByName(string2);
				if (factionByName != null)
				{
					this.factionId = factionByName.ID;
					string string3 = entityClass.Properties.GetString("FactionRank");
					if (string3.Length > 0)
					{
						this.factionRank = StringParsers.ParseUInt8(string3, 0, -1, NumberStyles.Integer);
					}
				}
			}
		}
		else if (FactionManager.Instance.GetFaction(this.factionId).ID == 0)
		{
			this.factionId = FactionManager.Instance.CreateFaction(this.entityName, true, "").ID;
			this.factionRank = byte.MaxValue;
		}
		this.maxViewAngle = 180f;
		entityClass.Properties.ParseFloat(EntityClass.PropMaxViewAngle, ref this.maxViewAngle);
		this.sightRangeBase = entityClass.SightRange;
		this.sightLightThreshold = entityClass.sightLightThreshold;
		this.SetSleeperSight(-1f, -1f);
		this.sightWakeThresholdAtRange.x = this.rand.RandomRange(entityClass.SleeperSightToWakeMin.x, entityClass.SleeperSightToWakeMin.y);
		this.sightWakeThresholdAtRange.y = this.rand.RandomRange(entityClass.SleeperSightToWakeMax.y, entityClass.SleeperSightToWakeMax.y);
		this.sightGroanThresholdAtRange.x = this.rand.RandomRange(entityClass.SleeperSightToSenseMin.x, entityClass.SleeperSightToSenseMin.y);
		this.sightGroanThresholdAtRange.y = this.rand.RandomRange(entityClass.SleeperSightToSenseMax.y, entityClass.SleeperSightToSenseMax.y);
		this.sleeperNoiseToSense = this.rand.RandomRange(entityClass.SleeperNoiseToSense.x, entityClass.SleeperNoiseToSense.y);
		this.sleeperNoiseToSenseSoundChance = entityClass.SleeperNoiseToSenseSoundChance;
		this.sleeperNoiseToWake = this.rand.RandomRange(entityClass.SleeperNoiseToWake.x, entityClass.SleeperNoiseToWake.y);
		float num = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropAttackTimeoutDay, ref num);
		this.attackTimeoutDay = (int)(num * 20f);
		entityClass.Properties.ParseFloat(EntityClass.PropAttackTimeoutNight, ref num);
		this.attackTimeoutNight = (int)(num * 20f);
		entityClass.Properties.ParseBool(EntityClass.PropStompsSpikes, ref this.stompsSpikes);
		this.weight = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropWeight, ref this.weight);
		this.weight = Utils.FastMax(this.weight, 0.5f);
		this.pushFactor = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropPushFactor, ref this.pushFactor);
		float num2 = 5f;
		entityClass.Properties.ParseFloat(EntityClass.PropTimeStayAfterDeath, ref num2);
		this.timeStayAfterDeath = (int)(num2 * 20f);
		this.IsMale = true;
		entityClass.Properties.ParseBool(EntityClass.PropIsMale, ref this.IsMale);
		this.IsFeral = entityClass.Tags.Test_Bit(EntityAlive.FeralTagBit);
		this.proneRefillRate = this.rand.RandomRange(entityClass.KnockdownProneRefillRate.x, entityClass.KnockdownProneRefillRate.y);
		this.kneelRefillRate = this.rand.RandomRange(entityClass.KnockdownKneelRefillRate.x, entityClass.KnockdownKneelRefillRate.y);
		this.moveSpeed = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropMoveSpeed, ref this.moveSpeed);
		this.moveSpeedNight = this.moveSpeed;
		entityClass.Properties.ParseFloat(EntityClass.PropMoveSpeedNight, ref this.moveSpeedNight);
		this.moveSpeedAggro = this.moveSpeed;
		this.moveSpeedAggroMax = this.moveSpeed;
		entityClass.Properties.ParseVec(EntityClass.PropMoveSpeedAggro, ref this.moveSpeedAggro, ref this.moveSpeedAggroMax);
		this.moveSpeedPanic = 1f;
		this.moveSpeedPanicMax = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropMoveSpeedPanic, ref this.moveSpeedPanic);
		if (this.moveSpeedPanic != 1f)
		{
			this.moveSpeedPanicMax = this.moveSpeedPanic;
		}
		entityClass.Properties.ParseFloat(EntityClass.PropSwimSpeed, ref this.swimSpeed);
		entityClass.Properties.ParseVec(EntityClass.PropSwimStrokeRate, ref this.swimStrokeRate);
		Vector2 negativeInfinity = Vector2.negativeInfinity;
		entityClass.Properties.ParseVec(EntityClass.PropMoveSpeedRand, ref negativeInfinity);
		if (negativeInfinity.x > -1f)
		{
			float num3 = this.rand.RandomRange(negativeInfinity.x, negativeInfinity.y);
			int @int = GameStats.GetInt(EnumGameStats.GameDifficulty);
			num3 *= EntityAlive.moveSpeedRandomness[@int];
			if (this.moveSpeedAggro < 1f)
			{
				this.moveSpeedAggro += num3;
				if (this.moveSpeedAggro < 0.1f)
				{
					this.moveSpeedAggro = 0.1f;
				}
				if (this.moveSpeedAggro > this.moveSpeedAggroMax)
				{
					this.moveSpeedAggro = this.moveSpeedAggroMax;
				}
			}
		}
		entityClass.Properties.ParseInt(EntityClass.PropCrouchType, ref this.crouchType);
		this.walkType = EntityAlive.GetSpawnWalkType(entityClass);
		entityClass.Properties.ParseBool(EntityClass.PropCanClimbLadders, ref this.bCanClimbLadders);
		entityClass.Properties.ParseBool(EntityClass.PropCanClimbVertical, ref this.bCanClimbVertical);
		Vector2 vector = new Vector2(1.9f, 2.1f);
		entityClass.Properties.ParseVec(EntityClass.PropJumpMaxDistance, ref vector);
		this.jumpMaxDistance = this.rand.RandomRange(vector.x, vector.y);
		this.jumpDelay = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropJumpDelay, ref this.jumpDelay);
		this.jumpDelay *= 20f;
		this.ExperienceValue = 20;
		entityClass.Properties.ParseInt(EntityClass.PropExperienceGain, ref this.ExperienceValue);
		if (this.aiManager != null)
		{
			this.aiManager.CopyPropertiesFromEntityClass(entityClass);
		}
		entityClass.Properties.ParseString(EntityClass.PropSoundSpawn, ref this.soundSpawn);
		entityClass.Properties.ParseString(EntityClass.PropSoundSleeperSense, ref this.soundSleeperGroan);
		entityClass.Properties.ParseString(EntityClass.PropSoundSleeperSnore, ref this.soundSleeperSnore);
		entityClass.Properties.ParseString(EntityClass.PropSoundDeath, ref this.soundDeath);
		entityClass.Properties.ParseString(EntityClass.PropSoundAlert, ref this.soundAlert);
		entityClass.Properties.ParseString(EntityClass.PropSoundAttack, ref this.soundAttack);
		entityClass.Properties.ParseString(EntityClass.PropSoundLiving, ref this.soundLiving);
		entityClass.Properties.ParseString(EntityClass.PropSoundRandom, ref this.soundRandom);
		entityClass.Properties.ParseString(EntityClass.PropSoundSense, ref this.soundSense);
		entityClass.Properties.ParseString(EntityClass.PropSoundGiveUp, ref this.soundGiveUp);
		this.soundStepType = "step";
		entityClass.Properties.ParseString(EntityClass.PropSoundStepType, ref this.soundStepType);
		entityClass.Properties.ParseString(EntityClass.PropSoundStamina, ref this.soundStamina);
		entityClass.Properties.ParseString(EntityClass.PropSoundJump, ref this.soundJump);
		entityClass.Properties.ParseString(EntityClass.PropSoundLand, ref this.soundLand);
		entityClass.Properties.ParseString(EntityClass.PropSoundHurt, ref this.soundHurt);
		entityClass.Properties.ParseString(EntityClass.PropSoundHurtSmall, ref this.soundHurtSmall);
		entityClass.Properties.ParseString(EntityClass.PropSoundDrownPain, ref this.soundDrownPain);
		entityClass.Properties.ParseString(EntityClass.PropSoundDrownDeath, ref this.soundDrownDeath);
		entityClass.Properties.ParseString(EntityClass.PropSoundWaterSurface, ref this.soundWaterSurface);
		this.soundAlertTicks = 25;
		entityClass.Properties.ParseInt(EntityClass.PropSoundAlertTime, ref this.soundAlertTicks);
		this.soundAlertTicks *= 20;
		this.soundRandomTicks = 25;
		entityClass.Properties.ParseInt(EntityClass.PropSoundRandomTime, ref this.soundRandomTicks);
		this.soundRandomTicks *= 20;
		entityClass.Properties.ParseString(EntityClass.PropParticleOnDeath, ref this.particleOnDeath);
		entityClass.Properties.ParseString(EntityClass.PropParticleOnDestroy, ref this.particleOnDestroy);
		string string4 = entityClass.Properties.GetString(EntityClass.PropCorpseBlock);
		if (string4.Length > 0)
		{
			this.corpseBlockValue = Block.GetBlockValue(string4, false);
		}
		this.corpseBlockChance = 1f;
		entityClass.Properties.ParseFloat(EntityClass.PropCorpseBlockChance, ref this.corpseBlockChance);
		GameMode gameModeForId = GameMode.GetGameModeForId(GameStats.GetInt(EnumGameStats.GameModeId));
		if (gameModeForId != null)
		{
			string string5 = entityClass.Properties.GetString(EntityClass.PropItemsOnEnterGame + "." + gameModeForId.GetTypeName());
			if (string5.Length > 0)
			{
				foreach (string text in string5.Split(',', StringSplitOptions.None))
				{
					ItemStack itemStack = ItemStack.FromString(text.Trim());
					if (itemStack.itemValue.IsEmpty())
					{
						throw new Exception("Item with name '" + text + "' not found in class " + EntityClass.list[this.entityClass].entityClassName);
					}
					if (itemStack.itemValue.ItemClass.CreativeMode != EnumCreativeMode.Console || (DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
					{
						this.itemsOnEnterGame.Add(itemStack);
					}
				}
			}
		}
		DynamicProperties dynamicProperties = entityClass.Properties.Classes[EntityClass.PropFallLandBehavior];
		if (dynamicProperties != null)
		{
			foreach (KeyValuePair<string, string> keyValuePair in dynamicProperties.Data.Dict)
			{
				string key = keyValuePair.Key;
				DictionarySave<string, string> dictionarySave = dynamicProperties.ParseKeyData(key);
				if (dictionarySave != null)
				{
					FloatRange height = default(FloatRange);
					FloatRange ragePer = default(FloatRange);
					FloatRange rageTime = default(FloatRange);
					IntRange difficulty = new IntRange(0, 10);
					string text2;
					EntityAlive.FallBehavior.Op type;
					if (!dictionarySave.TryGetValue("anim", out text2) || !Enum.TryParse<EntityAlive.FallBehavior.Op>(text2, out type))
					{
						Log.Error("Expected 'anim' parameter as float for FallBehavior " + key + ", skipping");
					}
					else
					{
						float num4 = 0f;
						if (!dictionarySave.TryGetValue("weight", out text2) || !StringParsers.TryParseFloat(text2, out num4, 0, -1, NumberStyles.Any))
						{
							Log.Error("Expected 'weight' parameter as float for FallBehavior " + key + ", skipping");
						}
						else if (dictionarySave.TryGetValue("height", out text2))
						{
							FloatRange floatRange;
							if (StringParsers.TryParseRange(text2, out floatRange, new float?(3.4028235E+38f)))
							{
								height = floatRange;
								if (dictionarySave.TryGetValue("ragePer", out text2))
								{
									FloatRange floatRange2;
									if (!StringParsers.TryParseRange(text2, out floatRange2, null))
									{
										Log.Error("Expected 'ragePer' parameter as range(min,min-max) " + key + ", skipping");
										continue;
									}
									ragePer = floatRange2;
								}
								if (dictionarySave.TryGetValue("rageTime", out text2))
								{
									FloatRange floatRange3;
									if (!StringParsers.TryParseRange(text2, out floatRange3, null))
									{
										Log.Error("Expected 'rageTime' parameter as range(min,min-max) " + key + ", skipping");
										continue;
									}
									rageTime = floatRange3;
								}
								if (dictionarySave.TryGetValue("difficulty", out text2))
								{
									IntRange intRange;
									if (!StringParsers.TryParseRange(text2, out intRange, null))
									{
										Log.Error("Expected 'difficulty' parameter as range(min,min-max) " + key + ", skipping");
										continue;
									}
									difficulty = intRange;
								}
								this.fallBehaviors.Add(new EntityAlive.FallBehavior(key, type, height, num4, ragePer, rageTime, difficulty));
							}
							else
							{
								Log.Error("Expected 'height' parameter as range(min,min-max) " + key + ", skipping");
							}
						}
						else
						{
							Log.Error("Expected 'height' parameter for FallBehavior " + key + ", skipping");
						}
					}
				}
			}
		}
		DynamicProperties dynamicProperties2 = entityClass.Properties.Classes[EntityClass.PropDestroyBlockBehavior];
		if (dynamicProperties2 != null)
		{
			EntityAlive.DestroyBlockBehavior.Op[] array2 = Enum.GetValues(typeof(EntityAlive.DestroyBlockBehavior.Op)) as EntityAlive.DestroyBlockBehavior.Op[];
			for (int j = 0; j < array2.Length; j++)
			{
				string text3 = array2[j].ToStringCached<EntityAlive.DestroyBlockBehavior.Op>();
				DictionarySave<string, string> dictionarySave2 = dynamicProperties2.ParseKeyData(array2[j].ToStringCached<EntityAlive.DestroyBlockBehavior.Op>());
				if (dictionarySave2 != null)
				{
					FloatRange ragePer2 = default(FloatRange);
					FloatRange rageTime2 = default(FloatRange);
					IntRange difficulty2 = new IntRange(0, 10);
					string input;
					float num5;
					if (!dictionarySave2.TryGetValue("weight", out input) || !StringParsers.TryParseFloat(input, out num5, 0, -1, NumberStyles.Any))
					{
						Log.Error(string.Format("Expected 'weight' parameter as float for FallBehavior {0}, skipping", array2[j]));
					}
					else
					{
						if (dictionarySave2.TryGetValue("ragePer", out input))
						{
							FloatRange floatRange4;
							if (!StringParsers.TryParseRange(input, out floatRange4, null))
							{
								Log.Error(string.Format("Expected 'ragePer' parameter as range(min,min-max) {0}, skipping", array2[j]));
								goto IL_E26;
							}
							ragePer2 = floatRange4;
						}
						if (dictionarySave2.TryGetValue("rageTime", out input))
						{
							FloatRange floatRange5;
							if (!StringParsers.TryParseRange(input, out floatRange5, null))
							{
								Log.Error(string.Format("Expected 'rageTime' parameter as range(min,min-max) {0}, skipping", array2[j]));
								goto IL_E26;
							}
							rageTime2 = floatRange5;
						}
						if (dictionarySave2.TryGetValue("difficulty", out input))
						{
							IntRange intRange2;
							if (!StringParsers.TryParseRange(input, out intRange2, null))
							{
								Log.Error("Expected 'difficulty' parameter as range(min,min-max) " + text3 + ", skipping");
								goto IL_E26;
							}
							difficulty2 = intRange2;
						}
						this._destroyBlockBehaviors.Add(new EntityAlive.DestroyBlockBehavior(text3, array2[j], num5, ragePer2, rageTime2, difficulty2));
					}
				}
				IL_E26:;
			}
		}
		this.distractionResistance = EffectManager.GetValue(PassiveEffects.DistractionResistance, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.distractionResistanceWithTarget = EffectManager.GetValue(PassiveEffects.DistractionResistance, null, 0f, this, null, EntityAlive.DistractionResistanceWithTargetTags, true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000C222C File Offset: 0x000C042C
	public static int GetSpawnWalkType(EntityClass _entityClass)
	{
		int result = 0;
		_entityClass.Properties.ParseInt(EntityClass.PropWalkType, ref result);
		return result;
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x000C2250 File Offset: 0x000C0450
	public override void VisiblityCheck(float _distanceSqr, bool _isZoom)
	{
		if ((this.entityFlags & EntityFlags.AIHearing) > EntityFlags.None)
		{
			if (GameManager.IsDedicatedServer)
			{
				this.emodel.SetVisible(true, false);
				return;
			}
			if (_distanceSqr < (float)(_isZoom ? 14400 : 8100))
			{
				this.renderFadeTarget = this.renderFadeMax;
				return;
			}
			this.renderFadeTarget = 0f;
		}
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x000C22AB File Offset: 0x000C04AB
	public virtual void SetSleeper()
	{
		this.IsSleeper = true;
		this.aiManager.pathCostScale += 0.2f;
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x000C22CB File Offset: 0x000C04CB
	public void SetSleeperSight(float angle, float range)
	{
		if (angle < 0f)
		{
			angle = this.maxViewAngle;
		}
		this.sleeperViewAngle = angle;
		if (range < 0f)
		{
			range = Utils.FastMax(3f, this.sightRangeBase * 0.2f);
		}
		this.sleeperSightRange = range;
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x000C230B File Offset: 0x000C050B
	public void SetSleeperHearing(float percent)
	{
		if (percent < 0.001f)
		{
			percent = 0.001f;
		}
		percent = 1f / percent;
		this.sleeperNoiseToSense *= percent;
		this.sleeperNoiseToWake *= percent;
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x000C2344 File Offset: 0x000C0544
	public int GetSleeperDisturbedLevel(float dist, float lightLevel)
	{
		float num = dist / this.sightRangeBase;
		if (num <= 1f)
		{
			float num2 = Mathf.Lerp(this.sightWakeThresholdAtRange.x, this.sightWakeThresholdAtRange.y, num);
			if (lightLevel > num2)
			{
				return 2;
			}
			float num3 = Mathf.Lerp(this.sightGroanThresholdAtRange.x, this.sightGroanThresholdAtRange.y, num);
			if (lightLevel > num3)
			{
				return 1;
			}
		}
		return 0;
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000C23AC File Offset: 0x000C05AC
	public void GetSleeperDebugScale(float dist, out float wake, out float groan)
	{
		float t = dist / this.sightRangeBase;
		wake = Mathf.Lerp(this.sightWakeThresholdAtRange.x, this.sightWakeThresholdAtRange.y, t);
		groan = Mathf.Lerp(this.sightGroanThresholdAtRange.x, this.sightGroanThresholdAtRange.y, t);
	}

	// Token: 0x17000375 RID: 885
	// (get) Token: 0x06001F26 RID: 7974 RVA: 0x000C23FE File Offset: 0x000C05FE
	public bool sleepingOrWakingUp
	{
		get
		{
			return this.IsSleeping;
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000C2408 File Offset: 0x000C0608
	public void TriggerSleeperPose(int _pose, bool _returningToSleep = false)
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.emodel && this.emodel.avatarController)
		{
			this.emodel.avatarController.TriggerSleeperPose(_pose, _returningToSleep);
			this.pendingSleepTrigger = -1;
			if (_pose != 5)
			{
				this.physicsHeight = 0.85f;
			}
		}
		else
		{
			this.pendingSleepTrigger = _pose;
		}
		this.lastSleeperPose = _pose;
		this.IsSleeping = true;
		this.SleeperSupressLivingSounds = true;
		this.sleeperLookDir = Quaternion.AngleAxis(this.rotation.y, Vector3.up) * this.SleeperSpawnLookDir;
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000C24A9 File Offset: 0x000C06A9
	public void ResumeSleeperPose()
	{
		this.TriggerSleeperPose(this.lastSleeperPose, true);
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000C24B8 File Offset: 0x000C06B8
	public void ConditionalTriggerSleeperWakeUp()
	{
		if (this.IsSleeping && !this.IsDead())
		{
			this.IsSleeping = false;
			this.IsSleeperPassive = false;
			int pose = (this.physicsHeight < 1f && !this.IsWalkTypeACrawl()) ? -2 : -1;
			this.emodel.avatarController.TriggerSleeperPose(pose, false);
			if (this.aiManager != null)
			{
				this.aiManager.SleeperWokeUp();
			}
			if (!this.world.IsRemote())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSleeperWakeup>().Setup(this.entityId), false, -1, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x000C2564 File Offset: 0x000C0764
	public void SetSleeperActive()
	{
		if (this.IsSleeperPassive)
		{
			this.IsSleeperPassive = false;
			if (!this.world.IsRemote())
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageSleeperPassiveChange>().Setup(this.entityId), false, -1, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x000C25BA File Offset: 0x000C07BA
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitStats()
	{
		this.entityStats = new EntityStats(this);
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000C25C8 File Offset: 0x000C07C8
	public void SetStats(EntityStats _stats)
	{
		this.entityStats.CopyFrom(_stats);
	}

	// Token: 0x17000376 RID: 886
	// (get) Token: 0x06001F2D RID: 7981 RVA: 0x000C25D6 File Offset: 0x000C07D6
	public EntityStats Stats
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.entityStats;
		}
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000C25DE File Offset: 0x000C07DE
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual ItemValue GetHandItem()
	{
		return this.handItem;
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000C25E6 File Offset: 0x000C07E6
	public bool IsHoldingLight()
	{
		return this.inventory.IsFlashlightOn;
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x00002914 File Offset: 0x00000B14
	public void CycleActivatableItems()
	{
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x000C25F4 File Offset: 0x000C07F4
	public List<ItemValue> GetActivatableItemPool()
	{
		List<ItemValue> list = new List<ItemValue>();
		this.CollectActivatableItems(list);
		return list;
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000C2610 File Offset: 0x000C0810
	public void CollectActivatableItems(List<ItemValue> _pool)
	{
		if (this.inventory != null)
		{
			EntityAlive.GetActivatableItems(this.inventory.holdingItemItemValue, _pool);
		}
		if (this.equipment != null)
		{
			int slotCount = this.equipment.GetSlotCount();
			for (int i = 0; i < slotCount; i++)
			{
				EntityAlive.GetActivatableItems(this.equipment.GetSlotItemOrNone(i), _pool);
			}
		}
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000C2668 File Offset: 0x000C0868
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GetActivatableItems(ItemValue _item, List<ItemValue> _itemPool)
	{
		ItemClass itemClass = _item.ItemClass;
		if (itemClass == null)
		{
			return;
		}
		if (itemClass.HasTrigger(MinEventTypes.onSelfItemActivate))
		{
			_itemPool.Add(_item);
		}
		for (int i = 0; i < _item.Modifications.Length; i++)
		{
			ItemValue itemValue = _item.Modifications[i];
			if (itemValue != null)
			{
				ItemClass itemClass2 = itemValue.ItemClass;
				if (itemClass2 != null && itemClass2.HasTrigger(MinEventTypes.onSelfItemActivate))
				{
					_itemPool.Add(itemValue);
				}
			}
		}
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x000C26CC File Offset: 0x000C08CC
	public override void OnUpdatePosition(float _partialTicks)
	{
		float rotYDelta = Utils.DeltaAngle(this.rotation.y, this.prevRotation.y);
		base.OnUpdatePosition(_partialTicks);
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.lastTickPos.Length - 1; i++)
		{
			vector.x += this.lastTickPos[i].x - this.lastTickPos[i + 1].x;
			vector.z += this.lastTickPos[i].z - this.lastTickPos[i + 1].z;
		}
		vector += this.position - this.lastTickPos[0];
		vector /= (float)this.lastTickPos.Length;
		if (this.AttachedToEntity == null)
		{
			this.updateStepSound(vector.x, vector.z, rotYDelta);
		}
		if (!this.RootMotion && !this.isEntityRemote)
		{
			this.updateSpeedForwardAndStrafe(vector, _partialTicks);
		}
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000C27E0 File Offset: 0x000C09E0
	public void Snore()
	{
		if (!this.isSnore && this.isGroan && this.snoreGroanCD <= 0)
		{
			this.isSnore = true;
			this.isGroan = false;
			this.snoreGroanCD = this.rand.RandomRange(20, 21);
			if (this.soundSleeperSnore != null && !this.isGroanSilent)
			{
				Manager.BroadcastPlay(this, this.soundSleeperSnore, false);
			}
		}
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000C2848 File Offset: 0x000C0A48
	public void Groan()
	{
		if (!this.isGroan && this.snoreGroanCD <= 0)
		{
			this.isGroan = true;
			this.isSnore = false;
			this.snoreGroanCD = this.rand.RandomRange(20, 21);
			if (this.sleeperNoiseToSenseSoundChance >= 1f || this.rand.RandomFloat <= this.sleeperNoiseToSenseSoundChance)
			{
				this.isGroanSilent = false;
				if (this.soundSleeperGroan != null)
				{
					Manager.BroadcastPlay(this, this.soundSleeperGroan, false);
					return;
				}
			}
			else
			{
				this.isGroanSilent = true;
			}
		}
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x000C28D0 File Offset: 0x000C0AD0
	public override void OnUpdateEntity()
	{
		base.OnUpdateEntity();
		this.Buffs.SetCustomVar("_underwater", this.inWaterPercent, true, CVarOperation.set);
		if (this.Buffs != null)
		{
			this.Buffs.Update(Time.deltaTime);
		}
		this.OnUpdateLive();
		if (!this.IsSleeping && (!this.isEntityRemote || !(this is EntityPlayer)))
		{
			this.bag.OnUpdate();
			if (this.inventory != null)
			{
				this.inventory.OnUpdate();
			}
		}
		if (this.Health <= 0 && !this.IsDead() && !this.isEntityRemote && !this.IsGodMode.Value)
		{
			if (this.Buffs.HasBuff("drowning"))
			{
				this.DamageEntity(DamageSource.suffocating, 1, false, 1f);
			}
			else
			{
				this.DamageEntity(DamageSource.disease, 1, false, 1f);
			}
		}
		if (base.IsAlive() && this.bPlayHurtSound)
		{
			string text = this.GetSoundHurt(this.woundedDamageSource, this.woundedStrength);
			if (text != null)
			{
				this.PlayOneShot(text, false, false, false, null);
			}
		}
		this.bPlayHurtSound = false;
		this.bBeenWounded = false;
		this.woundedStrength = 0;
		this.woundedDamageSource = null;
		if (this.snoreGroanCD > 0)
		{
			this.snoreGroanCD--;
		}
		if (!this.IsDead() && !this.isEntityRemote)
		{
			if (this.isRadiationSensitive() && this.biomeStandingOn != null && this.biomeStandingOn.m_RadiationLevel > 0 && !this.IsGodMode.Value && this.world.worldTime % 20UL == 0UL)
			{
				this.DamageEntity(DamageSource.radiation, this.biomeStandingOn.m_RadiationLevel, false, 1f);
			}
			if (this.hasAI)
			{
				if (this.IsSleeping && this.pendingSleepTrigger > -1)
				{
					this.TriggerSleeperPose(this.pendingSleepTrigger, false);
				}
				this.soundDelayTicks--;
				if (this.attackingTime <= 0 && this.soundDelayTicks <= 0 && this.aiClosestPlayerDistSq <= 400f && this.bodyDamage.CurrentStun == EnumEntityStunType.None && !this.SleeperSupressLivingSounds)
				{
					if (this.targetAlertChanged)
					{
						this.targetAlertChanged = false;
						this.soundDelayTicks = this.GetSoundAlertTicks();
						if (this.GetSoundAlert() != null && !this.IsScoutZombie)
						{
							this.PlayOneShot(this.GetSoundAlert(), false, false, false, null);
						}
						this.OnEntityTargeted(this.attackTarget);
					}
					else
					{
						this.soundDelayTicks = this.GetSoundRandomTicks();
						this.attackTargetLast = null;
						if (this.GetSoundRandom() != null)
						{
							this.PlayOneShot(this.GetSoundRandom(), false, false, false, null);
						}
					}
				}
			}
		}
		if (this.hasBeenAttackedTime > 0)
		{
			this.hasBeenAttackedTime--;
		}
		if (this.painResistPercent > 0f)
		{
			this.painResistPercent -= 0.010000001f;
			if (this.painResistPercent <= 0f)
			{
				this.painHitsFelt = 0f;
			}
		}
		if (this.attackingTime > 0)
		{
			this.attackingTime--;
		}
		if (this.investigatePositionTicks > 0)
		{
			int num = this.investigatePositionTicks - 1;
			this.investigatePositionTicks = num;
			if (num == 0)
			{
				this.ClearInvestigatePosition();
			}
		}
		bool flag = this.IsDead();
		if (this.alertEnabled)
		{
			this.isAlert = this.bReplicatedAlertFlag;
			if (!this.isEntityRemote)
			{
				if (this.alertTicks > 0)
				{
					this.alertTicks--;
				}
				this.isAlert = (!flag && (this.alertTicks > 0 || this.attackTarget || (this.HasInvestigatePosition && this.isInvestigateAlert)));
				if (this.bReplicatedAlertFlag != this.isAlert)
				{
					this.bReplicatedAlertFlag = this.isAlert;
					this.bEntityAliveFlagsChanged = true;
				}
			}
			if (!this.isAlert && !flag)
			{
				this.Buffs.SetCustomVar(EntityAlive.notAlertedId, 1f, true, CVarOperation.set);
				this.notAlertDelayTicks = 4;
			}
			else
			{
				if (this.notAlertDelayTicks > 0)
				{
					this.notAlertDelayTicks--;
				}
				if (this.notAlertDelayTicks == 0)
				{
					this.Buffs.SetCustomVar(EntityAlive.notAlertedId, 0f, true, CVarOperation.set);
				}
			}
		}
		if (flag)
		{
			this.OnDeathUpdate();
		}
		if (this.revengeEntity != null)
		{
			if (!this.revengeEntity.IsAlive())
			{
				this.SetRevengeTarget(null);
				return;
			}
			if (this.revengeTimer > 0)
			{
				this.revengeTimer--;
				return;
			}
			this.SetRevengeTarget(null);
		}
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x000C2D34 File Offset: 0x000C0F34
	public override void KillLootContainer()
	{
		if (!this.isEntityRemote && this.IsDead() && !this.corpseBlockValue.isair && this.deathUpdateTime < this.timeStayAfterDeath)
		{
			this.deathUpdateTime = this.timeStayAfterDeath - 1;
		}
		base.KillLootContainer();
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x000C2D80 File Offset: 0x000C0F80
	public override void Kill(DamageResponse _dmResponse)
	{
		this.NotifySleeperDeath();
		if (this.AttachedToEntity != null)
		{
			this.Detach();
		}
		if (this.deathUpdateTime == 0)
		{
			string text = this.GetSoundDeath(_dmResponse.Source);
			if (text != null)
			{
				this.PlayOneShot(text, false, false, false, null);
			}
		}
		if (this.IsDead())
		{
			this.SetDead();
			return;
		}
		this.ClientKill(_dmResponse);
		base.Kill(_dmResponse);
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000C2DE7 File Offset: 0x000C0FE7
	public override void SetDead()
	{
		base.SetDead();
		this.Stats.Health.Value = 0f;
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000C2E04 File Offset: 0x000C1004
	public void NotifySleeperDeath()
	{
		if (!this.isEntityRemote && this.IsSleeper)
		{
			this.world.NotifySleeperVolumesEntityDied(this);
		}
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000C2E22 File Offset: 0x000C1022
	public void ClearEntityThatKilledMe()
	{
		this.entityThatKilledMe = null;
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000C2E2C File Offset: 0x000C102C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void ClientKill(DamageResponse _dmResponse)
	{
		this.lastHitDirection = Utils.EnumHitDirection.Back;
		if (this.entityThatKilledMe == null && _dmResponse.Source != null)
		{
			Entity entity = (_dmResponse.Source.getEntityId() != -1) ? this.world.GetEntity(_dmResponse.Source.getEntityId()) : null;
			if (this.Spawned && entity is EntityAlive)
			{
				this.entityThatKilledMe = (EntityAlive)entity;
			}
		}
		if (!this.IsDead())
		{
			this.SetDead();
			if (this.Buffs != null)
			{
				this.Buffs.OnDeath(this.entityThatKilledMe, _dmResponse.Source != null && _dmResponse.Source.damageType == EnumDamageTypes.Crushing, (_dmResponse.Source == null) ? FastTags<TagGroup.Global>.Parse("crushing") : _dmResponse.Source.DamageTypeTag);
			}
			if (this.Progression != null)
			{
				this.Progression.OnDeath();
			}
			UnityEngine.Object x = this as EntityPlayer;
			this.AnalyticsSendDeath(_dmResponse);
			if (x == null && this.entityThatKilledMe is EntityPlayer && EffectManager.GetValue(PassiveEffects.CelebrationKill, null, 0f, this.entityThatKilledMe, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
			{
				this.HandleClientDeath((_dmResponse.Source != null) ? _dmResponse.Source.BlockPosition : base.GetBlockPosition());
				this.OnEntityDeath();
				float lightBrightness = this.world.GetLightBrightness(base.GetBlockPosition());
				this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect("confetti", this.position, lightBrightness, Color.white, null, null, false), this.entityId, false, true);
				Manager.BroadcastPlayByLocalPlayer(this.position, "twitch_celebrate");
				GameManager.Instance.World.RemoveEntity(this.entityId, EnumRemoveEntityReason.Killed);
				return;
			}
			this.HandleClientDeath((_dmResponse.Source != null) ? _dmResponse.Source.BlockPosition : base.GetBlockPosition());
			this.OnEntityDeath();
			this.emodel.OnDeath(_dmResponse, this.world.ChunkClusters[0]);
		}
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleClientDeath(Vector3i attackPos)
	{
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEntityTargeted(EntityAlive target)
	{
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x000C3040 File Offset: 0x000C1240
	public void ForceHoldingWeaponUpdate()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageHoldingItem>().Setup(this), false, -1, this.entityId, -1, null, 192, false);
			return;
		}
		if (this.entityId > 0 && this as EntityPlayerLocal != null)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageHoldingItem>().Setup(this), false);
		}
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x000C30C1 File Offset: 0x000C12C1
	public virtual void SetHoldingItemTransform(Transform _transform)
	{
		this.emodel.SetInRightHand(_transform);
		this.ForceHoldingWeaponUpdate();
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnHoldingItemChanged()
	{
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateCameraFOV(bool _bLerpPosition)
	{
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x000C30D5 File Offset: 0x000C12D5
	public virtual int GetCameraFOV()
	{
		return GamePrefs.GetInt(EnumGamePrefs.OptionsGfxFOV);
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000C30DE File Offset: 0x000C12DE
	public virtual float GetWetnessRate()
	{
		return this.inWaterPercent;
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000C30E8 File Offset: 0x000C12E8
	public float GetAmountEnclosed()
	{
		Vector3 position = this.position;
		position.y += 0.5f;
		Vector3i vector3i = World.worldToBlockPos(position);
		if (vector3i.y < 255)
		{
			IChunk chunkFromWorldPos = this.world.GetChunkFromWorldPos(vector3i);
			if (chunkFromWorldPos != null)
			{
				float v = (float)chunkFromWorldPos.GetLight(vector3i.x, vector3i.y, vector3i.z, Chunk.LIGHT_TYPE.SUN);
				float v2 = (float)chunkFromWorldPos.GetLight(vector3i.x, vector3i.y + 1, vector3i.z, Chunk.LIGHT_TYPE.SUN);
				float num = Utils.FastMax(v, v2) / 15f;
				return 1f - num;
			}
		}
		return 1f;
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000C3183 File Offset: 0x000C1383
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHeadUnderwaterStateChanged(bool _bUnderwater)
	{
		base.OnHeadUnderwaterStateChanged(_bUnderwater);
		if (_bUnderwater)
		{
			this.FireEvent(MinEventTypes.onSelfWaterSubmerge, true);
			return;
		}
		this.FireEvent(MinEventTypes.onSelfWaterSurface, true);
	}

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x06001F48 RID: 8008 RVA: 0x000C31A2 File Offset: 0x000C13A2
	// (set) Token: 0x06001F49 RID: 8009 RVA: 0x000C31AA File Offset: 0x000C13AA
	public virtual bool JetpackActive
	{
		get
		{
			return this.bJetpackActive;
		}
		set
		{
			if (value != this.bJetpackActive)
			{
				this.bJetpackActive = value;
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000378 RID: 888
	// (get) Token: 0x06001F4A RID: 8010 RVA: 0x000C31D2 File Offset: 0x000C13D2
	// (set) Token: 0x06001F4B RID: 8011 RVA: 0x000C31DA File Offset: 0x000C13DA
	public virtual bool JetpackWearing
	{
		get
		{
			return this.bJetpackWearing;
		}
		set
		{
			if (value != this.bJetpackWearing)
			{
				this.bJetpackWearing = value;
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000379 RID: 889
	// (get) Token: 0x06001F4C RID: 8012 RVA: 0x000C3202 File Offset: 0x000C1402
	// (set) Token: 0x06001F4D RID: 8013 RVA: 0x000C320A File Offset: 0x000C140A
	public virtual bool ParachuteWearing
	{
		get
		{
			return this.bParachuteWearing;
		}
		set
		{
			if (value != this.bParachuteWearing)
			{
				this.bParachuteWearing = value;
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700037A RID: 890
	// (get) Token: 0x06001F4E RID: 8014 RVA: 0x000C3232 File Offset: 0x000C1432
	// (set) Token: 0x06001F4F RID: 8015 RVA: 0x000C326C File Offset: 0x000C146C
	public virtual bool AimingGun
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.TryGetBool(AvatarController.isAimingHash, out this.bAimingGun) && this.bAimingGun;
		}
		set
		{
			bool aimingGun = this.AimingGun;
			if (value != aimingGun)
			{
				if (this.emodel.avatarController != null)
				{
					this.emodel.avatarController.UpdateBool(AvatarController.isAimingHash, value, true);
				}
				this.UpdateCameraFOV(true);
			}
			if (this is EntityPlayerLocal && this.inventory != null)
			{
				ItemAction itemAction = this.inventory.holdingItem.Actions[1];
				if (itemAction != null)
				{
					itemAction.AimingSet(this.inventory.holdingItemData.actionData[1], value, aimingGun);
				}
			}
		}
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x000C32FC File Offset: 0x000C14FC
	public virtual Vector3 GetChestTransformPosition()
	{
		if (this.IsCrouching || this.bodyDamage.CurrentStun == EnumEntityStunType.Kneel || this.bodyDamage.CurrentStun == EnumEntityStunType.Prone)
		{
			return base.transform.position + new Vector3(0f, this.GetEyeHeight() * 0.25f, 0f);
		}
		return base.transform.position + new Vector3(0f, this.GetEyeHeight() * 0.95f, 0f);
	}

	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06001F51 RID: 8017 RVA: 0x000C3384 File Offset: 0x000C1584
	// (set) Token: 0x06001F52 RID: 8018 RVA: 0x000C338C File Offset: 0x000C158C
	public virtual bool MovementRunning
	{
		get
		{
			return this.bMovementRunning;
		}
		set
		{
			if (value != this.bMovementRunning)
			{
				this.bMovementRunning = value;
			}
		}
	}

	// Token: 0x1700037C RID: 892
	// (get) Token: 0x06001F53 RID: 8019 RVA: 0x000C339E File Offset: 0x000C159E
	// (set) Token: 0x06001F54 RID: 8020 RVA: 0x000C33A8 File Offset: 0x000C15A8
	public virtual bool Crouching
	{
		get
		{
			return this.bCrouching;
		}
		set
		{
			if (value != this.bCrouching)
			{
				this.bCrouching = value;
				if (this.emodel.avatarController != null)
				{
					this.emodel.avatarController.SetCrouching(value);
				}
				this.CurrentStanceTag = (this.bCrouching ? EntityAlive.StanceTagCrouching : EntityAlive.StanceTagStanding);
				this.Buffs.SetCustomVar("_crouching", (float)(this.bCrouching ? 1 : 0), true, CVarOperation.set);
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700037D RID: 893
	// (get) Token: 0x06001F55 RID: 8021 RVA: 0x000C3438 File Offset: 0x000C1638
	public bool IsCrouching
	{
		get
		{
			return this.Crouching || this.CrouchingLocked;
		}
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x06001F56 RID: 8022 RVA: 0x000C344C File Offset: 0x000C164C
	// (set) Token: 0x06001F57 RID: 8023 RVA: 0x000C3490 File Offset: 0x000C1690
	public virtual bool Jumping
	{
		get
		{
			return this.bJumping && EffectManager.GetValue(PassiveEffects.JumpStrength, null, 1f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) != 0f;
		}
		set
		{
			if (value != this.bJumping)
			{
				this.bJumping = value;
				if (this.Jumping)
				{
					this.StartJump();
					this.CurrentMovementTag &= EntityAlive.MovementTagIdle;
					this.CurrentMovementTag |= EntityAlive.MovementTagJumping;
				}
				else
				{
					this.EndJump();
					this.CurrentMovementTag &= EntityAlive.MovementTagJumping;
					this.bJumping = false;
				}
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06001F58 RID: 8024 RVA: 0x000C3522 File Offset: 0x000C1722
	// (set) Token: 0x06001F59 RID: 8025 RVA: 0x000C352C File Offset: 0x000C172C
	public bool Climbing
	{
		get
		{
			return this.bClimbing;
		}
		set
		{
			if (value != this.bClimbing)
			{
				this.bClimbing = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (this.bClimbing)
				{
					this.CurrentMovementTag &= EntityAlive.MovementTagIdle;
					this.CurrentMovementTag |= EntityAlive.MovementTagClimbing;
					return;
				}
				this.CurrentMovementTag &= EntityAlive.MovementTagClimbing;
			}
		}
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000C35AA File Offset: 0x000C17AA
	public virtual bool CanNavigatePath()
	{
		return this.onGround || this.isSwimming || this.bInElevator || this.Climbing;
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x000C35CC File Offset: 0x000C17CC
	public AvatarController.ActionState GetAnimActionState()
	{
		if (this.emodel.avatarController)
		{
			return this.emodel.avatarController.GetActionState();
		}
		return AvatarController.ActionState.None;
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x000C35F4 File Offset: 0x000C17F4
	public virtual void StartAnimAction(int _animType)
	{
		if (this.emodel.avatarController)
		{
			this.emodel.avatarController.GetActionState();
			if (_animType != 9999)
			{
				if (this.emodel.avatarController.IsActionActive())
				{
					return;
				}
			}
			else if (!this.emodel.avatarController.IsActionActive())
			{
				return;
			}
			this.bPlayerStatsChanged |= !this.isEntityRemote;
			this.emodel.avatarController.StartAction(_animType);
		}
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x000C3679 File Offset: 0x000C1879
	public virtual void ContinueAnimAction(int _animType)
	{
		if (this.emodel.avatarController)
		{
			this.bPlayerStatsChanged |= !this.isEntityRemote;
			this.emodel.avatarController.StartAction(_animType);
		}
	}

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x06001F5E RID: 8030 RVA: 0x000C36B4 File Offset: 0x000C18B4
	// (set) Token: 0x06001F5F RID: 8031 RVA: 0x000C36DB File Offset: 0x000C18DB
	public virtual bool RightArmAnimationAttack
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationAttackPlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value && !this.emodel.avatarController.IsAnimationAttackPlaying())
			{
				this.emodel.avatarController.StartAnimationAttack();
			}
		}
	}

	// Token: 0x17000381 RID: 897
	// (get) Token: 0x06001F60 RID: 8032 RVA: 0x000C3715 File Offset: 0x000C1915
	// (set) Token: 0x06001F61 RID: 8033 RVA: 0x000C373C File Offset: 0x000C193C
	public virtual bool RightArmAnimationUse
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationUsePlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value != this.emodel.avatarController.IsAnimationUsePlaying())
			{
				this.emodel.avatarController.StartAnimationUse();
			}
		}
	}

	// Token: 0x17000382 RID: 898
	// (get) Token: 0x06001F62 RID: 8034 RVA: 0x000C3774 File Offset: 0x000C1974
	// (set) Token: 0x06001F63 RID: 8035 RVA: 0x000C379C File Offset: 0x000C199C
	public virtual bool SpecialAttack
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationSpecialAttackPlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value != this.emodel.avatarController.IsAnimationSpecialAttackPlaying())
			{
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				this.emodel.avatarController.StartAnimationSpecialAttack(value, 0);
			}
		}
	}

	// Token: 0x17000383 RID: 899
	// (get) Token: 0x06001F64 RID: 8036 RVA: 0x000C37F7 File Offset: 0x000C19F7
	// (set) Token: 0x06001F65 RID: 8037 RVA: 0x000C381E File Offset: 0x000C1A1E
	public virtual bool SpecialAttack2
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationSpecialAttack2Playing();
		}
		set
		{
			if (this.emodel.avatarController != null && value)
			{
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				this.emodel.avatarController.StartAnimationSpecialAttack2();
			}
		}
	}

	// Token: 0x17000384 RID: 900
	// (get) Token: 0x06001F66 RID: 8038 RVA: 0x000C385B File Offset: 0x000C1A5B
	// (set) Token: 0x06001F67 RID: 8039 RVA: 0x000C3882 File Offset: 0x000C1A82
	public virtual bool Raging
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationRagingPlaying();
		}
		set
		{
			if (this.emodel.avatarController != null && value && !this.emodel.avatarController.IsAnimationRagingPlaying())
			{
				this.emodel.avatarController.StartAnimationRaging();
			}
		}
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x06001F68 RID: 8040 RVA: 0x000C38BC File Offset: 0x000C1ABC
	// (set) Token: 0x06001F69 RID: 8041 RVA: 0x000C38F8 File Offset: 0x000C1AF8
	public virtual bool Electrocuted
	{
		get
		{
			return this.emodel != null && this.emodel.avatarController != null && this.emodel.avatarController.GetAnimationElectrocuteRemaining() > 0f;
		}
		set
		{
			if (this.emodel != null && this.emodel.avatarController != null && value != this.emodel.avatarController.GetAnimationElectrocuteRemaining() > 0.4f)
			{
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (value)
				{
					this.emodel.avatarController.StartAnimationElectrocute(0.6f);
					this.emodel.avatarController.Electrocute(true);
				}
			}
		}
	}

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x06001F6A RID: 8042 RVA: 0x000C397F File Offset: 0x000C1B7F
	// (set) Token: 0x06001F6B RID: 8043 RVA: 0x000C39A6 File Offset: 0x000C1BA6
	public virtual bool HarvestingAnimation
	{
		get
		{
			return this.emodel.avatarController != null && this.emodel.avatarController.IsAnimationHarvestingPlaying();
		}
		set
		{
			this.emodel.avatarController.UpdateBool("Harvesting", value, true);
		}
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000C39BF File Offset: 0x000C1BBF
	public virtual void StartHarvestingAnim(float _length, bool _weaponFireTrigger)
	{
		if (this.emodel != null && this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationHarvesting(_length, _weaponFireTrigger);
		}
	}

	// Token: 0x17000387 RID: 903
	// (get) Token: 0x06001F6D RID: 8045 RVA: 0x000C39F4 File Offset: 0x000C1BF4
	// (set) Token: 0x06001F6E RID: 8046 RVA: 0x000C39FC File Offset: 0x000C1BFC
	public bool IsEating
	{
		get
		{
			return this.m_isEating;
		}
		set
		{
			if (value != this.m_isEating)
			{
				this.m_isEating = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (this.emodel != null && this.emodel.avatarController != null)
				{
					if (this.m_isEating)
					{
						this.emodel.avatarController.StartEating();
						return;
					}
					this.emodel.avatarController.StopEating();
				}
			}
		}
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000C3A7C File Offset: 0x000C1C7C
	public virtual void SetVehicleAnimation(int _animHash, int _pose)
	{
		if (this.emodel && this.emodel.avatarController)
		{
			this.emodel.avatarController.SetVehicleAnimation(_animHash, _pose);
			this.bPlayerStatsChanged = !this.isEntityRemote;
			if (_pose == -1)
			{
				AvatarLocalPlayerController avatarLocalPlayerController = this.emodel.avatarController as AvatarLocalPlayerController;
				if (avatarLocalPlayerController != null)
				{
					avatarLocalPlayerController.TPVResetAnimPose();
				}
			}
		}
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x000C3AE7 File Offset: 0x000C1CE7
	public virtual int GetVehicleAnimation()
	{
		if (this.emodel && this.emodel.avatarController)
		{
			return this.emodel.avatarController.GetVehicleAnimation();
		}
		return -1;
	}

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x06001F71 RID: 8049 RVA: 0x000C3B1A File Offset: 0x000C1D1A
	// (set) Token: 0x06001F72 RID: 8050 RVA: 0x000C3B22 File Offset: 0x000C1D22
	public virtual int Died
	{
		get
		{
			return this.died;
		}
		set
		{
			if (value != this.died)
			{
				this.died = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x06001F73 RID: 8051 RVA: 0x000C3B4A File Offset: 0x000C1D4A
	// (set) Token: 0x06001F74 RID: 8052 RVA: 0x000C3B52 File Offset: 0x000C1D52
	public virtual int Score
	{
		get
		{
			return this.score;
		}
		set
		{
			if (value != this.score)
			{
				this.score = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x06001F75 RID: 8053 RVA: 0x000C3B7A File Offset: 0x000C1D7A
	// (set) Token: 0x06001F76 RID: 8054 RVA: 0x000C3B82 File Offset: 0x000C1D82
	public virtual int KilledZombies
	{
		get
		{
			return this.killedZombies;
		}
		set
		{
			if (value != this.killedZombies)
			{
				this.killedZombies = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700038B RID: 907
	// (get) Token: 0x06001F77 RID: 8055 RVA: 0x000C3BAA File Offset: 0x000C1DAA
	// (set) Token: 0x06001F78 RID: 8056 RVA: 0x000C3BB2 File Offset: 0x000C1DB2
	public virtual int KilledPlayers
	{
		get
		{
			return this.killedPlayers;
		}
		set
		{
			if (value != this.killedPlayers)
			{
				this.killedPlayers = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700038C RID: 908
	// (get) Token: 0x06001F79 RID: 8057 RVA: 0x000C3BDA File Offset: 0x000C1DDA
	// (set) Token: 0x06001F7A RID: 8058 RVA: 0x000C3BE2 File Offset: 0x000C1DE2
	public virtual int TeamNumber
	{
		get
		{
			return this.teamNumber;
		}
		set
		{
			if (value != this.teamNumber)
			{
				this.teamNumber = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
				if (!this.isEntityRemote)
				{
					GameManager.Instance.GameMessage(EnumGameMessages.ChangedTeam, this, null);
				}
			}
		}
	}

	// Token: 0x1700038D RID: 909
	// (get) Token: 0x06001F7B RID: 8059 RVA: 0x000C3C1F File Offset: 0x000C1E1F
	public virtual string EntityName
	{
		get
		{
			return this.entityName;
		}
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000C3C27 File Offset: 0x000C1E27
	public override void SetEntityName(string _name)
	{
		if (!_name.Equals(this.entityName))
		{
			this.entityName = _name;
			this.bPlayerStatsChanged |= !this.isEntityRemote;
			this.HandleSetNavName();
		}
	}

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x06001F7D RID: 8061 RVA: 0x000C3C5A File Offset: 0x000C1E5A
	// (set) Token: 0x06001F7E RID: 8062 RVA: 0x000C3C62 File Offset: 0x000C1E62
	public virtual int DeathHealth
	{
		get
		{
			return this.deathHealth;
		}
		set
		{
			if (value != this.deathHealth)
			{
				this.deathHealth = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06001F7F RID: 8063 RVA: 0x000C3C8A File Offset: 0x000C1E8A
	// (set) Token: 0x06001F80 RID: 8064 RVA: 0x000C3C92 File Offset: 0x000C1E92
	public virtual bool Spawned
	{
		get
		{
			return this.bSpawned;
		}
		set
		{
			if (value != this.bSpawned)
			{
				this.bSpawned = value;
				this.onSpawnStateChanged();
				this.bEntityAliveFlagsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x06001F81 RID: 8065 RVA: 0x000C3CC0 File Offset: 0x000C1EC0
	// (set) Token: 0x06001F82 RID: 8066 RVA: 0x000C3CC8 File Offset: 0x000C1EC8
	public bool IsBreakingBlocks
	{
		get
		{
			return this.m_isBreakingBlocks;
		}
		set
		{
			if (value != this.m_isBreakingBlocks)
			{
				this.m_isBreakingBlocks = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
		}
	}

	// Token: 0x06001F83 RID: 8067 RVA: 0x000C3C8A File Offset: 0x000C1E8A
	public override bool IsSpawned()
	{
		return this.bSpawned;
	}

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x06001F84 RID: 8068 RVA: 0x000C3CF0 File Offset: 0x000C1EF0
	public virtual EntityBedrollPositionList SpawnPoints
	{
		get
		{
			return this.spawnPoints;
		}
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x000C3CF8 File Offset: 0x000C1EF8
	public virtual void RemoveIKTargets()
	{
		this.emodel.RemoveIKController();
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x000C3D08 File Offset: 0x000C1F08
	public virtual void SetIKTargets(List<IKController.Target> targets)
	{
		IKController ikcontroller = this.emodel.AddIKController();
		if (ikcontroller)
		{
			ikcontroller.SetTargets(targets);
		}
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000C3D30 File Offset: 0x000C1F30
	public virtual List<Vector3i> GetDroppedBackpackPositions()
	{
		return this.droppedBackpackPositions;
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000C3D38 File Offset: 0x000C1F38
	public virtual Vector3i GetLastDroppedBackpackPosition()
	{
		if (this.droppedBackpackPositions == null)
		{
			return Vector3i.zero;
		}
		if (this.droppedBackpackPositions.Count == 0)
		{
			return Vector3i.zero;
		}
		List<Vector3i> list = this.droppedBackpackPositions;
		return list[list.Count - 1];
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x000C3D70 File Offset: 0x000C1F70
	public virtual bool EqualsDroppedBackpackPositions(Vector3i position)
	{
		if (this.droppedBackpackPositions != null)
		{
			foreach (Vector3i other in this.droppedBackpackPositions)
			{
				if (position.Equals(other))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000C3DD8 File Offset: 0x000C1FD8
	public virtual void SetDroppedBackpackPositions(List<Vector3i> positions)
	{
		this.droppedBackpackPositions.Clear();
		if (positions != null)
		{
			this.droppedBackpackPositions.AddRange(positions);
		}
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x000C3DF4 File Offset: 0x000C1FF4
	public virtual void ClearDroppedBackpackPositions()
	{
		this.droppedBackpackPositions.Clear();
	}

	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06001F8C RID: 8076 RVA: 0x000C3E01 File Offset: 0x000C2001
	// (set) Token: 0x06001F8D RID: 8077 RVA: 0x000C3E14 File Offset: 0x000C2014
	public virtual int Health
	{
		get
		{
			return (int)this.Stats.Health.Value;
		}
		set
		{
			this.Stats.Health.Value = (float)value;
		}
	}

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x06001F8E RID: 8078 RVA: 0x000C3E28 File Offset: 0x000C2028
	// (set) Token: 0x06001F8F RID: 8079 RVA: 0x000C3E3A File Offset: 0x000C203A
	public virtual float Stamina
	{
		get
		{
			return this.Stats.Stamina.Value;
		}
		set
		{
			this.Stats.Stamina.Value = value;
		}
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x06001F90 RID: 8080 RVA: 0x000C3E4D File Offset: 0x000C204D
	// (set) Token: 0x06001F91 RID: 8081 RVA: 0x000C3E5F File Offset: 0x000C205F
	public virtual float Water
	{
		get
		{
			return this.Stats.Water.Value;
		}
		set
		{
			this.Stats.Water.Value = value;
		}
	}

	// Token: 0x06001F92 RID: 8082 RVA: 0x000C3E72 File Offset: 0x000C2072
	public virtual int GetMaxHealth()
	{
		return (int)this.Stats.Health.Max;
	}

	// Token: 0x06001F93 RID: 8083 RVA: 0x000C3E85 File Offset: 0x000C2085
	public virtual int GetMaxStamina()
	{
		return (int)this.Stats.Stamina.Max;
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x000C3E98 File Offset: 0x000C2098
	public virtual int GetMaxWater()
	{
		return (int)this.Stats.Water.Max;
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x06001F95 RID: 8085 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsValidAimAssistSlowdownTarget
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06001F96 RID: 8086 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsValidAimAssistSnapTarget
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06001F97 RID: 8087 RVA: 0x000C3EAB File Offset: 0x000C20AB
	// (set) Token: 0x06001F98 RID: 8088 RVA: 0x000C3EB3 File Offset: 0x000C20B3
	public virtual EModelBase.HeadStates CurrentHeadState
	{
		get
		{
			return this.currentHeadState;
		}
		set
		{
			if (value != this.currentHeadState)
			{
				this.currentHeadState = value;
				this.bPlayerStatsChanged |= !this.isEntityRemote;
			}
			this.emodel.ForceHeadState(value);
		}
	}

	// Token: 0x06001F99 RID: 8089 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public virtual float GetStaminaMultiplier()
	{
		return 1f;
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x000C3EE8 File Offset: 0x000C20E8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetMovementState()
	{
		float num = this.speedStrafe;
		if (num >= 1234f)
		{
			num = 0f;
		}
		float num2 = this.speedForward * this.speedForward + num * num;
		this.MovementState = ((num2 > this.moveSpeedAggro * this.moveSpeedAggro) ? 3 : ((num2 > this.moveSpeed * this.moveSpeed) ? 2 : ((num2 > 0.001f) ? 1 : 0)));
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x000C3F54 File Offset: 0x000C2154
	public virtual void OnUpdateLive()
	{
		this.Stats.Health.RegenerationAmount = 0f;
		if (!this.isEntityRemote && !this.IsDead())
		{
			this.Stats.Tick(this.world.worldTime);
		}
		if (this.jumpTicks > 0)
		{
			this.jumpTicks--;
		}
		if (this.attackTargetTime > 0)
		{
			this.attackTargetTime--;
			if (this.attackTarget != null && this.attackTargetTime == 0)
			{
				this.attackTarget = null;
				if (!this.isEntityRemote)
				{
					this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, -1, NetPackageManager.GetPackage<NetPackageSetAttackTarget>().Setup(this.entityId, -1), false);
				}
			}
		}
		this.updateCurrentBlockPosAndValue();
		if (this.AttachedToEntity == null)
		{
			if (this.isEntityRemote)
			{
				if (this.RootMotion)
				{
					this.MoveEntityHeaded(Vector3.zero, false);
				}
			}
			else
			{
				if (this.Health <= 0)
				{
					this.bJumping = false;
					this.bClimbing = false;
					this.moveDirection = Vector3.zero;
					this.renderFadeMax = 1f;
				}
				else if (!this.world.IsRemote() && !this.IsDead() && !this.IsClientControlled() && this.hasAI)
				{
					this.updateTasks();
				}
				this.noisePlayer = null;
				this.noisePlayerDistance = 0f;
				this.noisePlayerVolume = 0f;
				if (this.bJumping)
				{
					this.UpdateJump();
				}
				else
				{
					this.jumpTicks = 0;
				}
				float num = this.landMovementFactor;
				this.landMovementFactor *= this.GetSpeedModifier();
				this.MoveEntityHeaded(this.moveDirection, this.isMoveDirAbsolute);
				this.landMovementFactor = num;
			}
			if (this.moveDirection.x > 0f || this.moveDirection.z > 0f)
			{
				if (this.bMovementRunning)
				{
					this.CurrentMovementTag = EntityAlive.MovementTagRunning;
				}
				else
				{
					this.CurrentMovementTag = EntityAlive.MovementTagWalking;
				}
			}
			else
			{
				this.CurrentMovementTag = EntityAlive.MovementTagIdle;
			}
		}
		if (this.bodyDamage.CurrentStun != EnumEntityStunType.None && !this.emodel.IsRagdollActive && !this.IsDead())
		{
			if (this.bodyDamage.CurrentStun == EnumEntityStunType.Getup)
			{
				if (!this.emodel.avatarController || !this.emodel.avatarController.IsAnimationStunRunning())
				{
					this.ClearStun();
				}
			}
			else
			{
				this.bodyDamage.StunDuration = this.bodyDamage.StunDuration - 0.05f;
				if (this.bodyDamage.StunDuration <= 0f)
				{
					this.SetStun(EnumEntityStunType.Getup);
					if (this.emodel.avatarController)
					{
						this.emodel.avatarController.EndStun();
					}
				}
			}
		}
		this.proneRefillCounter += 0.05f * this.proneRefillRate;
		while (this.proneRefillCounter >= 1f)
		{
			this.bodyDamage.StunProne = Mathf.Max(0, this.bodyDamage.StunProne - 1);
			this.proneRefillCounter -= 1f;
		}
		this.kneelRefillCounter += 0.05f * this.kneelRefillRate;
		while (this.kneelRefillCounter >= 1f)
		{
			this.bodyDamage.StunKnee = Mathf.Max(0, this.bodyDamage.StunKnee - 1);
			this.kneelRefillCounter -= 1f;
		}
		EntityPlayer primaryPlayer = this.world.GetPrimaryPlayer();
		if (primaryPlayer != null && primaryPlayer != this)
		{
			int num2 = this.ticksToCheckSeenByPlayer - 1;
			this.ticksToCheckSeenByPlayer = num2;
			if (num2 <= 0)
			{
				this.wasSeenByPlayer = primaryPlayer.CanSee(this);
				if (this.wasSeenByPlayer)
				{
					this.ticksToCheckSeenByPlayer = 200;
				}
				else
				{
					this.ticksToCheckSeenByPlayer = 20;
				}
			}
			else if (this.wasSeenByPlayer)
			{
				primaryPlayer.SetCanSee(this);
			}
		}
		if (this.onGround)
		{
			this.disableFallBehaviorUntilOnGround = false;
		}
		this.UpdateDynamicRagdoll();
		this.checkForTeleportOutOfTraderArea();
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000C4354 File Offset: 0x000C2554
	[PublicizedFrom(EAccessModifier.Protected)]
	public void checkForTeleportOutOfTraderArea()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && !GameManager.Instance.IsEditMode() && !this.IsGodMode.Value)
		{
			EntityPlayer entityPlayer = this as EntityPlayer;
			if (entityPlayer != null && Time.time - this.lastTimeTraderStationChecked > 0.1f)
			{
				this.lastTimeTraderStationChecked = Time.time;
				Vector3 position = this.position;
				position.y += 0.5f;
				Vector3i vector3i = World.worldToBlockPos(position);
				TraderArea traderAreaAt = this.world.GetTraderAreaAt(vector3i);
				if (traderAreaAt != null && traderAreaAt.IsInitialized)
				{
					Vector3 targetPos = default(Vector3);
					int num = 0;
					Prefab.PrefabTeleportVolume prefabTeleportVolume;
					if (this.world.IsWorldEvent(World.WorldEvent.BloodMoon) && traderAreaAt.IsWithinProtectArea(position))
					{
						targetPos = traderAreaAt.ProtectPosition + traderAreaAt.ProtectSize * 0.5f;
						num = Math.Max(traderAreaAt.ProtectSize.x, traderAreaAt.ProtectSize.z) / 2;
					}
					else if (traderAreaAt.IsWithinTeleportArea(position, out prefabTeleportVolume) && (traderAreaAt.IsClosed || EffectManager.GetValue(PassiveEffects.NoTrader, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 1f))
					{
						PrefabInstance poiatPosition = this.world.GetPOIAtPosition(vector3i, true);
						if (poiatPosition == null)
						{
							return;
						}
						targetPos = poiatPosition.boundingBoxPosition + traderAreaAt.PrefabSize * 0.5f;
						num = Math.Max(traderAreaAt.PrefabSize.x, traderAreaAt.PrefabSize.z) / 2;
					}
					if (num > 0)
					{
						num += this.traderTeleportStreak;
						this.traderTeleportStreak++;
						Vector3 vector;
						if (!this.world.GetRandomSpawnPositionMinMaxToPosition(targetPos, num, num + 1, 1, false, out vector, this.entityId, true, 20, true, EnumLandClaimOwner.Ally, true))
						{
							Log.Warning("Trader teleport: Could not find a valid teleport position, returning original position");
							return;
						}
						if (this.isEntityRemote)
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(this.entityId).SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(vector, null, false));
						}
						else if (entityPlayer)
						{
							entityPlayer.Teleport(vector, float.MinValue);
						}
						else if (this.AttachedToEntity != null)
						{
							this.AttachedToEntity.SetPosition(vector, true);
						}
						else
						{
							this.SetPosition(vector, true);
						}
						if (entityPlayer)
						{
							GameEventManager.Current.HandleAction("game_on_trader_teleport", entityPlayer, entityPlayer, false, "", "", false, true, "", null);
							return;
						}
					}
				}
				else
				{
					this.traderTeleportStreak = 1;
				}
			}
		}
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x000C45FC File Offset: 0x000C27FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StartJump()
	{
		this.jumpState = EntityAlive.JumpState.Leap;
		this.jumpStateTicks = 0;
		this.jumpDistance = 1f;
		this.jumpHeightDiff = 0f;
		this.disableFallBehaviorUntilOnGround = true;
		if (this.isSwimming)
		{
			this.jumpState = EntityAlive.JumpState.SwimStart;
			if (this.emodel.avatarController != null)
			{
				this.emodel.avatarController.SetSwim(true);
				return;
			}
		}
		else if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationJump(AnimJumpMode.Start);
		}
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x000C468C File Offset: 0x000C288C
	public virtual void SetJumpDistance(float _distance, float _heightDiff)
	{
		this.jumpDistance = _distance;
		this.jumpHeightDiff = _heightDiff;
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x000C469C File Offset: 0x000C289C
	public virtual void SetSwimValues(float _durationTicks, Vector3 _motion)
	{
		this.jumpSwimDurationTicks = Mathf.Clamp(_durationTicks / this.swimSpeed - 6f, 3f, 20f);
		this.jumpSwimMotion = _motion;
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x000C46C8 File Offset: 0x000C28C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateJump()
	{
		if (this.IsFlyMode.Value)
		{
			this.Jumping = false;
			return;
		}
		this.jumpStateTicks++;
		switch (this.jumpState)
		{
		case EntityAlive.JumpState.Leap:
			if (this.accumulatedRootMotion.y > 0.005f || (float)this.jumpStateTicks >= this.jumpDelay)
			{
				this.StartJumpMotion();
				this.jumpTicks = 200;
				this.jumpState = EntityAlive.JumpState.Air;
				this.jumpStateTicks = 0;
				this.jumpIsMoving = true;
				return;
			}
			break;
		case EntityAlive.JumpState.Air:
			if (this.onGround || (this.motionMultiplier < 0.45f && this.jumpStateTicks > 40))
			{
				this.jumpState = EntityAlive.JumpState.Land;
				this.jumpStateTicks = 0;
				this.jumpIsMoving = false;
				return;
			}
			break;
		case EntityAlive.JumpState.Land:
			if (this.jumpStateTicks > 5)
			{
				this.Jumping = false;
				return;
			}
			break;
		case EntityAlive.JumpState.SwimStart:
			if ((float)this.jumpStateTicks > 6f)
			{
				this.jumpTicks = 100;
				this.jumpState = EntityAlive.JumpState.Swim;
				this.jumpStateTicks = 0;
				this.jumpIsMoving = true;
				this.StartJumpSwimMotion();
				return;
			}
			break;
		case EntityAlive.JumpState.Swim:
			if (!this.isSwimming || (float)this.jumpStateTicks >= this.jumpSwimDurationTicks)
			{
				this.Jumping = false;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000C4800 File Offset: 0x000C2A00
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StartJumpSwimMotion()
	{
		if (this.inWaterPercent > 0.65f)
		{
			float num = Mathf.Sqrt(this.jumpSwimMotion.x * this.jumpSwimMotion.x + this.jumpSwimMotion.z * this.jumpSwimMotion.z) + 0.001f;
			float min = Mathf.Lerp(-0.6f, -0.05f, num * 0.8f);
			this.jumpSwimMotion.y = Utils.FastClamp(this.jumpSwimMotion.y, min, 1f);
			float num2 = this.jumpSwimDurationTicks;
			float num3 = (num2 - 1f) * this.world.Gravity * 0.025f * 0.4999f;
			num3 /= Mathf.Pow(0.91f, (num2 - 3f) * 0.91f * 0.115f);
			float t = (num2 - 1f) / 15f;
			float num4 = Mathf.LerpUnclamped(0.46f, 0.41860002f, t);
			float num5 = Mathf.Pow(0.91f, (num2 - 1f) * num4);
			float num6 = 1f / num2 / num5;
			num3 += this.jumpSwimMotion.y * num6;
			num6 /= Utils.FastMax(1f, num);
			this.motion.x = this.jumpSwimMotion.x * num6;
			this.motion.z = this.jumpSwimMotion.z * num6;
			this.motion.y = num3;
			return;
		}
		this.motion.y = 0f;
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000C498C File Offset: 0x000C2B8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void FaceJumpTo()
	{
		Vector3 vector = this.moveHelper.JumpToPos - this.position;
		float yaw = Mathf.Round(Mathf.Atan2(vector.x, vector.z) * 57.29578f / 90f) * 90f;
		base.SeekYaw(yaw, 0f, 0f);
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x000C49EC File Offset: 0x000C2BEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void StartJumpMotion()
	{
		base.SetAirBorne(true);
		float num = (float)((int)(5f + Mathf.Pow(this.jumpDistance * 8f, 0.5f)));
		this.motion = this.GetForwardVector() * (this.jumpDistance / num);
		float num2 = num * this.world.Gravity * 0.5f;
		this.motion.y = Utils.FastMax(num2 * 0.5f, num2 + this.jumpHeightDiff / num);
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x000C4A70 File Offset: 0x000C2C70
	[PublicizedFrom(EAccessModifier.Private)]
	public void JumpMove()
	{
		this.accumulatedRootMotion = Vector3.zero;
		Vector3 motion = this.motion;
		this.entityCollision(this.motion);
		this.motion.x = motion.x;
		this.motion.z = motion.z;
		if (this.motion.y != 0f)
		{
			this.motion.y = motion.y;
		}
		if (this.jumpState == EntityAlive.JumpState.Air)
		{
			this.motion.y = this.motion.y - this.world.Gravity;
			return;
		}
		this.motion.x = this.motion.x * 0.91f;
		this.motion.z = this.motion.z * 0.91f;
		this.motion.y = this.motion.y - this.world.Gravity * 0.025f;
		this.motion.y = this.motion.y * 0.91f;
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x000C4B60 File Offset: 0x000C2D60
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void EndJump()
	{
		this.jumpState = EntityAlive.JumpState.Off;
		this.jumpIsMoving = false;
		if (!this.isEntityRemote && this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationJump(AnimJumpMode.Land);
		}
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x000C4B9C File Offset: 0x000C2D9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CalcIfSwimming()
	{
		float num = (this.onGround || this.Jumping) ? 0.7f : 0.5f;
		return this.inWaterPercent >= num;
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x000C4BD2 File Offset: 0x000C2DD2
	public override void SwimChanged()
	{
		if (this.emodel.avatarController)
		{
			this.emodel.avatarController.SetSwim(this.isSwimming);
		}
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000C4BFC File Offset: 0x000C2DFC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		this.updateNetworkStats();
		if (!this.isEntityRemote && this.RootMotion && this.lerpForwardSpeed)
		{
			float num = 0.06935714f;
			if (this.speedForward > 0.01942f)
			{
				num = this.speedForwardTargetStep;
			}
			float num2 = Utils.FastMoveTowards(this.speedForward, this.speedForwardTarget, num * Time.deltaTime);
			if (this.speedForward > 0.01942f && num2 <= 0.01942f)
			{
				num2 = 0.01942f;
			}
			this.speedForward = num2;
		}
		if (this.isHeadUnderwater != (this.Buffs.GetCustomVar("_underwater") == 1f))
		{
			this.Buffs.SetCustomVar("_underwater", (float)(this.isHeadUnderwater ? 1 : 0), true, CVarOperation.set);
		}
		this.MinEventContext.Area = this.boundingBox;
		this.MinEventContext.Biome = this.biomeStandingOn;
		this.MinEventContext.ItemValue = this.inventory.holdingItemItemValue;
		this.MinEventContext.BlockValue = this.blockValueStandingOn;
		this.MinEventContext.ItemInventoryData = this.inventory.holdingItemData;
		this.MinEventContext.Position = this.position;
		this.MinEventContext.Seed = this.entityId + Mathf.Abs(GameManager.Instance.World.Seed);
		this.MinEventContext.Transform = base.transform;
		FastTags<TagGroup.Global>.CombineTags(EntityClass.list[this.entityClass].Tags, this.inventory.holdingItem.ItemTags, this.CurrentStanceTag, this.CurrentMovementTag, ref this.MinEventContext.Tags);
		if (this.Progression != null)
		{
			this.Progression.Update();
		}
		if (this.renderFade != this.renderFadeTarget)
		{
			this.renderFade = Mathf.MoveTowards(this.renderFade, this.renderFadeTarget, Time.deltaTime);
			this.emodel.SetFade(this.renderFade);
			bool flag = this.renderFade > 0.01f;
			if (this.emodel.visible != flag)
			{
				this.emodel.SetVisible(flag, false);
			}
		}
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x000C4E20 File Offset: 0x000C3020
	public virtual void OnDeathUpdate()
	{
		if (this.deathUpdateTime < this.timeStayAfterDeath)
		{
			this.deathUpdateTime++;
		}
		int deadBodyHitPoints = EntityClass.list[this.entityClass].DeadBodyHitPoints;
		if (deadBodyHitPoints > 0 && this.DeathHealth <= -deadBodyHitPoints)
		{
			this.deathUpdateTime = this.timeStayAfterDeath;
		}
		if (this.deathUpdateTime < this.timeStayAfterDeath)
		{
			return;
		}
		if (!this.isEntityRemote && !this.markedForUnload)
		{
			this.dropCorpseBlock();
			if (this.particleOnDestroy != null && this.particleOnDestroy.Length > 0)
			{
				float lightBrightness = this.world.GetLightBrightness(base.GetBlockPosition());
				this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(this.particleOnDestroy, this.getHeadPosition(), lightBrightness, Color.white, null, null, false), this.entityId, false, false);
			}
		}
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x000C4EFC File Offset: 0x000C30FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual Vector3i dropCorpseBlock()
	{
		if (this.corpseBlockValue.isair)
		{
			return Vector3i.zero;
		}
		if (this.rand.RandomFloat > this.corpseBlockChance)
		{
			return Vector3i.zero;
		}
		Vector3i vector3i = World.worldToBlockPos(this.position);
		while (vector3i.y < 254 && (float)vector3i.y - this.position.y < 3f && !this.corpseBlockValue.Block.CanPlaceBlockAt(this.world, 0, vector3i, this.corpseBlockValue, false))
		{
			vector3i += Vector3i.up;
		}
		if (vector3i.y >= 254)
		{
			return Vector3i.zero;
		}
		if ((float)vector3i.y - this.position.y >= 2.1f)
		{
			return Vector3i.zero;
		}
		this.world.SetBlockRPC(vector3i, this.corpseBlockValue);
		return vector3i;
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x000C4FDD File Offset: 0x000C31DD
	public void NotifyRootMotion(Animator animator)
	{
		this.accumulatedRootMotion += animator.deltaPosition;
	}

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06001FAC RID: 8108 RVA: 0x000C4FF6 File Offset: 0x000C31F6
	public virtual float MaxVelocity
	{
		get
		{
			return 5f;
		}
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x000C5000 File Offset: 0x000C3200
	[PublicizedFrom(EAccessModifier.Protected)]
	public void DefaultMoveEntity(Vector3 _direction, bool _isDirAbsolute)
	{
		float num = 0.91f;
		if (AIDirector.debugFreezePos && this.aiManager != null)
		{
			this.motion = Vector3.zero;
		}
		if (this.onGround)
		{
			num = 0.546f;
			if (!this.IsDead() && this is EntityPlayer)
			{
				BlockValue block = this.world.GetBlock(Utils.Fastfloor(this.position.x), Utils.Fastfloor(this.boundingBox.min.y), Utils.Fastfloor(this.position.z));
				if (block.isair || block.Block.blockMaterial.IsGroundCover)
				{
					block = this.world.GetBlock(Utils.Fastfloor(this.position.x), Utils.Fastfloor(this.boundingBox.min.y - 1f), Utils.Fastfloor(this.position.z));
				}
				if (!block.isair)
				{
					num = Mathf.Clamp(1f - block.Block.blockMaterial.Friction, 0.01f, 1f);
				}
			}
		}
		if (!this.RootMotion || (!this.onGround && this.jumpTicks > 0))
		{
			float num2;
			if (this.onGround)
			{
				num2 = this.landMovementFactor;
				float num3 = 0.163f / (num * num * num);
				num2 *= num3;
			}
			else
			{
				num2 = this.jumpMovementFactor;
			}
			this.Move(_direction, _isDirAbsolute, num2, this.MaxVelocity);
		}
		if (this.Climbing)
		{
			this.fallDistance = 0f;
			this.entityCollision(this.motion);
			this.distanceClimbed += this.motion.magnitude;
			if (this.distanceClimbed > 0.5f)
			{
				this.internalPlayStepSound(1f);
				this.distanceClimbed = 0f;
			}
		}
		else
		{
			if (base.IsInElevator())
			{
				if (!this.RootMotion)
				{
					float num4 = 0.15f;
					if (this.motion.x < -num4)
					{
						this.motion.x = -num4;
					}
					if (this.motion.x > num4)
					{
						this.motion.x = num4;
					}
					if (this.motion.z < -num4)
					{
						this.motion.z = -num4;
					}
					if (this.motion.z > num4)
					{
						this.motion.z = num4;
					}
				}
				this.fallDistance = 0f;
			}
			if (this.IsSleeping)
			{
				this.motion.x = 0f;
				this.motion.z = 0f;
			}
			this.entityCollision(this.motion);
		}
		if (this.isSwimming)
		{
			this.motion.x = this.motion.x * 0.91f;
			this.motion.z = this.motion.z * 0.91f;
			this.motion.y = this.motion.y - this.world.Gravity * 0.025f;
			this.motion.y = this.motion.y * 0.91f;
			return;
		}
		this.motion.x = this.motion.x * num;
		this.motion.z = this.motion.z * num;
		if (!this.bInElevator)
		{
			this.motion.y = this.motion.y - this.world.Gravity;
		}
		this.motion.y = this.motion.y * 0.98f;
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x000C5364 File Offset: 0x000C3564
	public virtual void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
		if (this.AttachedToEntity != null)
		{
			return;
		}
		if (this.jumpIsMoving)
		{
			this.JumpMove();
			return;
		}
		if (this.RootMotion)
		{
			if (this.isEntityRemote && this.bodyDamage.CurrentStun == EnumEntityStunType.None && !this.IsDead() && (!(this.emodel != null) || !(this.emodel.avatarController != null) || !this.emodel.avatarController.IsAnimationHitRunning()))
			{
				this.accumulatedRootMotion = Vector3.zero;
				return;
			}
			bool flag = this.emodel && this.emodel.IsRagdollActive;
			if (this.isSwimming && !flag)
			{
				this.motion += this.accumulatedRootMotion * 0.001f;
			}
			else if (this.onGround || this.jumpTicks > 0)
			{
				if (flag)
				{
					this.motion.x = 0f;
					this.motion.z = 0f;
				}
				else
				{
					float y = this.motion.y;
					this.motion = this.accumulatedRootMotion;
					this.motion.y = this.motion.y + y;
				}
			}
			this.accumulatedRootMotion = Vector3.zero;
		}
		if (this.IsFlyMode.Value)
		{
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			float num = (primaryPlayer != null) ? primaryPlayer.GodModeSpeedModifier : 1f;
			float num2 = 2f * (this.MovementRunning ? 0.35f : 0.12f) * num;
			if (!this.RootMotion)
			{
				this.Move(_direction, _isDirAbsolute, this.GetPassiveEffectSpeedModifier() * num2, this.GetPassiveEffectSpeedModifier() * num2);
			}
			if (!this.IsNoCollisionMode.Value)
			{
				this.entityCollision(this.motion);
				this.motion *= base.ConditionalScalePhysicsMulConstant(0.546f);
			}
			else
			{
				this.SetPosition(this.position + this.motion, true);
				this.motion = Vector3.zero;
			}
		}
		else
		{
			this.DefaultMoveEntity(_direction, _isDirAbsolute);
		}
		if (!this.isEntityRemote && this.RootMotion)
		{
			float num3 = this.landMovementFactor;
			num3 *= 2.5f;
			if (this.inWaterPercent > 0.3f)
			{
				if (num3 > 0.01f)
				{
					float t = (this.inWaterPercent - 0.3f) * 1.4285715f;
					num3 = Mathf.Lerp(num3, 0.01f + (num3 - 0.01f) * 0.1f, t);
				}
				if (this.isSwimming)
				{
					num3 = this.landMovementFactor * 5f;
				}
			}
			float magnitude = _direction.magnitude;
			if (magnitude > 1f)
			{
				num3 /= magnitude;
			}
			float num4 = _direction.z * num3;
			if (this.lerpForwardSpeed)
			{
				if (Utils.FastAbs(this.speedForwardTarget - num4) > 0.05f)
				{
					this.speedForwardTargetStep = Utils.FastAbs(num4 - this.speedForward) / 0.18f;
				}
				this.speedForwardTarget = num4;
			}
			else
			{
				this.speedForward = num4;
			}
			this.speedStrafe = _direction.x * num3;
			this.SetMovementState();
			base.ReplicateSpeeds();
		}
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x000C56A0 File Offset: 0x000C38A0
	public float GetPassiveEffectSpeedModifier()
	{
		if (this.IsCrouching)
		{
			if (this.MovementRunning)
			{
				return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, Constants.cPlayerSpeedModifierWalking, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			return EffectManager.GetValue(PassiveEffects.CrouchSpeed, null, Constants.cPlayerSpeedModifierCrouching, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		else
		{
			if (this.MovementRunning)
			{
				return EffectManager.GetValue(PassiveEffects.RunSpeed, null, Constants.cPlayerSpeedModifierRunning, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, Constants.cPlayerSpeedModifierWalking, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
	}

	// Token: 0x06001FB0 RID: 8112 RVA: 0x000C5754 File Offset: 0x000C3954
	public void SetMoveForward(float _moveForward)
	{
		this.moveDirection.x = 0f;
		this.moveDirection.z = _moveForward;
		this.isMoveDirAbsolute = false;
		this.Climbing = false;
		this.lerpForwardSpeed = true;
		this.motion.x = 0f;
		this.motion.z = 0f;
		this.accumulatedRootMotion.x = 0f;
		this.accumulatedRootMotion.z = 0f;
		if (this.bInElevator)
		{
			this.motion.y = 0f;
		}
	}

	// Token: 0x06001FB1 RID: 8113 RVA: 0x000C57EC File Offset: 0x000C39EC
	public void SetMoveForwardWithModifiers(float _speedModifier, float _speedScale, bool _climb)
	{
		this.moveDirection.x = 0f;
		this.moveDirection.z = 1f;
		this.isMoveDirAbsolute = false;
		this.Climbing = _climb;
		this.lerpForwardSpeed = true;
		float num = this.speedModifier;
		this.speedModifier = _speedModifier * _speedScale;
		if (num > 0.2f)
		{
			num = this.speedModifier / num;
			this.accumulatedRootMotion.x = this.accumulatedRootMotion.x * num;
			this.accumulatedRootMotion.z = this.accumulatedRootMotion.z * num;
		}
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x000C5870 File Offset: 0x000C3A70
	public void AddMotion(float dir, float speed)
	{
		float f = dir * 0.017453292f;
		this.accumulatedRootMotion.x = this.accumulatedRootMotion.x + Mathf.Sin(f) * speed;
		this.accumulatedRootMotion.z = this.accumulatedRootMotion.z + Mathf.Cos(f) * speed;
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x000C58B4 File Offset: 0x000C3AB4
	public void MakeMotionMoveToward(float x, float z, float minMotion, float maxMotion)
	{
		if (this.RootMotion)
		{
			float num = Mathf.Sqrt(x * x + z * z);
			if (num > 0f)
			{
				num = Utils.FastClamp(Mathf.Sqrt(this.accumulatedRootMotion.x * this.accumulatedRootMotion.x + this.accumulatedRootMotion.z * this.accumulatedRootMotion.z), minMotion, maxMotion) / num;
				if (num < 1f)
				{
					x *= num;
					z *= num;
				}
			}
			this.accumulatedRootMotion.x = x;
			this.accumulatedRootMotion.z = z;
			return;
		}
		this.moveDirection.x = x;
		this.moveDirection.z = z;
		this.isMoveDirAbsolute = true;
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x000C5968 File Offset: 0x000C3B68
	public bool IsInFrontOfMe(Vector3 _position)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 dir = _position - headPosition;
		Vector3 forwardVector = this.GetForwardVector();
		float angleBetween = Utils.GetAngleBetween(dir, forwardVector);
		float num = this.GetMaxViewAngle() * 0.5f;
		return angleBetween >= -num && angleBetween <= num;
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000C59AC File Offset: 0x000C3BAC
	public bool IsInViewCone(Vector3 _position)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 dir = _position - headPosition;
		Vector3 lookVector;
		float num;
		if (this.IsSleeping)
		{
			lookVector = this.sleeperLookDir;
			num = this.sleeperViewAngle;
		}
		else
		{
			lookVector = this.GetLookVector();
			num = this.GetMaxViewAngle();
		}
		num *= 0.5f;
		float angleBetween = Utils.GetAngleBetween(dir, lookVector);
		return angleBetween >= -num && angleBetween <= num;
	}

	// Token: 0x06001FB6 RID: 8118 RVA: 0x000C5A08 File Offset: 0x000C3C08
	public void DrawViewCone()
	{
		Vector3 vector;
		float num;
		if (this.IsSleeping)
		{
			vector = this.sleeperLookDir;
			num = this.sleeperViewAngle;
		}
		else
		{
			vector = this.GetLookVector();
			num = this.GetMaxViewAngle();
		}
		vector *= this.GetSeeDistance();
		num *= 0.5f;
		Vector3 start = this.getHeadPosition() - Origin.position;
		Debug.DrawRay(start, vector, new Color(0.9f, 0.9f, 0.5f), 0.1f);
		Vector3 dir = Quaternion.Euler(0f, -num, 0f) * vector;
		Debug.DrawRay(start, dir, new Color(0.6f, 0.6f, 0.3f), 0.1f);
		Vector3 dir2 = Quaternion.Euler(0f, num, 0f) * vector;
		Debug.DrawRay(start, dir2, new Color(0.6f, 0.6f, 0.3f), 0.1f);
	}

	// Token: 0x06001FB7 RID: 8119 RVA: 0x000C5AF0 File Offset: 0x000C3CF0
	public bool CanSee(Vector3 _pos)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 direction = _pos - headPosition;
		float seeDistance = this.GetSeeDistance();
		if (direction.magnitude > seeDistance)
		{
			return false;
		}
		if (!this.IsInViewCone(_pos))
		{
			return false;
		}
		Ray ray = new Ray(headPosition, direction);
		ray.origin += direction.normalized * 0.2f;
		int modelLayer = this.GetModelLayer();
		this.SetModelLayer(2, false, null);
		bool result = true;
		if (Voxel.Raycast(this.world, ray, seeDistance, false, false))
		{
			result = false;
		}
		this.SetModelLayer(modelLayer, false, null);
		return result;
	}

	// Token: 0x06001FB8 RID: 8120 RVA: 0x000C5B8C File Offset: 0x000C3D8C
	public bool CanEntityBeSeen(Entity _other)
	{
		Vector3 headPosition = this.getHeadPosition();
		Vector3 headPosition2 = _other.getHeadPosition();
		Vector3 direction = headPosition2 - headPosition;
		float magnitude = direction.magnitude;
		float num = this.GetSeeDistance();
		EntityPlayer entityPlayer = _other as EntityPlayer;
		if (entityPlayer != null)
		{
			num *= entityPlayer.DetectUsScale(this);
		}
		if (magnitude > num)
		{
			return false;
		}
		if (!this.IsInViewCone(headPosition2))
		{
			return false;
		}
		bool result = false;
		Ray ray = new Ray(headPosition, direction);
		ray.origin += direction.normalized * -0.1f;
		int modelLayer = this.GetModelLayer();
		this.SetModelLayer(2, false, null);
		if (Voxel.Raycast(this.world, ray, num, -1612492821, 64, 0f))
		{
			if (Voxel.voxelRayHitInfo.tag == "E_Vehicle")
			{
				EntityVehicle entityVehicle = EntityVehicle.FindCollisionEntity(Voxel.voxelRayHitInfo.transform);
				if (entityVehicle && entityVehicle.IsAttached(_other))
				{
					result = true;
				}
			}
			else
			{
				if (Voxel.voxelRayHitInfo.tag.StartsWith("E_BP_"))
				{
					Voxel.voxelRayHitInfo.transform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, Voxel.voxelRayHitInfo.transform);
				}
				if (_other.transform == Voxel.voxelRayHitInfo.transform)
				{
					result = true;
				}
			}
		}
		this.SetModelLayer(modelLayer, false, null);
		return result;
	}

	// Token: 0x06001FB9 RID: 8121 RVA: 0x000C5CE4 File Offset: 0x000C3EE4
	public virtual float GetSeeDistance()
	{
		this.senseScale = 1f;
		if (this.IsSleeping)
		{
			this.sightRange = this.sleeperSightRange;
			return this.sleeperSightRange;
		}
		this.sightRange = this.sightRangeBase;
		if (this.aiManager != null)
		{
			float num = EAIManager.CalcSenseScale();
			this.senseScale = 1f + num * this.aiManager.feralSense;
			this.sightRange = this.sightRangeBase * this.senseScale;
		}
		return this.sightRange;
	}

	// Token: 0x06001FBA RID: 8122 RVA: 0x000C5D64 File Offset: 0x000C3F64
	public bool CanSeeStealth(float dist, float lightLevel)
	{
		float t = dist / this.sightRange;
		float num = Utils.FastLerp(this.sightLightThreshold.x, this.sightLightThreshold.y, t);
		return lightLevel > num;
	}

	// Token: 0x06001FBB RID: 8123 RVA: 0x000C5DA0 File Offset: 0x000C3FA0
	public float GetSeeStealthDebugScale(float dist)
	{
		float t = dist / this.sightRange;
		return Utils.FastLerp(this.sightLightThreshold.x, this.sightLightThreshold.y, t);
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000C5DD4 File Offset: 0x000C3FD4
	public override void SetAlive()
	{
		if (this.IsDead())
		{
			this.lastAliveTime = Time.time;
		}
		base.SetAlive();
		if (!this.isEntityRemote)
		{
			this.Stats.ResetStats();
		}
		this.Stats.Health.MaxModifier = 0f;
		this.Health = (int)this.Stats.Health.ModifiedMax;
		this.Stamina = this.Stats.Stamina.ModifiedMax;
		this.deathUpdateTime = 0;
		this.bDead = false;
		this.RecordedDamage.Fatal = false;
		this.emodel.SetAlive();
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000C5E74 File Offset: 0x000C4074
	public float YawForTarget(Entity _otherEntity)
	{
		return this.YawForTarget(_otherEntity.GetPosition());
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x000C5E84 File Offset: 0x000C4084
	public float YawForTarget(Vector3 target)
	{
		float num = target.x - this.position.x;
		return -(float)(Math.Atan2((double)(target.z - this.position.z), (double)num) * 180.0 / 3.141592653589793) + 90f;
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000C5EDC File Offset: 0x000C40DC
	public void RotateTo(Entity _otherEntity, float _dYaw, float _dPitch)
	{
		float num = _otherEntity.position.x - this.position.x;
		float num2 = _otherEntity.position.z - this.position.z;
		float num3;
		if (_otherEntity is EntityAlive)
		{
			EntityAlive entityAlive = (EntityAlive)_otherEntity;
			num3 = this.position.y + this.GetEyeHeight() - (entityAlive.position.y + entityAlive.GetEyeHeight());
		}
		else
		{
			num3 = (_otherEntity.boundingBox.min.y + _otherEntity.boundingBox.max.y) / 2f - (this.position.y + this.GetEyeHeight());
		}
		float num4 = Mathf.Sqrt(num * num + num2 * num2);
		float intendedRotation = -(float)(Math.Atan2((double)num2, (double)num) * 180.0 / 3.141592653589793) + 90f;
		float intendedRotation2 = (float)(-(float)(Math.Atan2((double)num3, (double)num4) * 180.0 / 3.141592653589793));
		this.rotation.x = EntityAlive.UpdateRotation(this.rotation.x, intendedRotation2, _dPitch);
		this.rotation.y = EntityAlive.UpdateRotation(this.rotation.y, intendedRotation, _dYaw);
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x000C6024 File Offset: 0x000C4224
	public void RotateTo(float _x, float _y, float _z, float _dYaw, float _dPitch)
	{
		float num = _x - this.position.x;
		float num2 = _z - this.position.z;
		float num3 = Mathf.Sqrt(num * num + num2 * num2);
		float intendedRotation = -(float)(Math.Atan2((double)num2, (double)num) * 180.0 / 3.141592653589793) + 90f;
		this.rotation.y = EntityAlive.UpdateRotation(this.rotation.y, intendedRotation, _dYaw);
		if (_dPitch > 0f)
		{
			float intendedRotation2 = (float)(-(float)(Math.Atan2((double)(_y - this.position.y), (double)num3) * 180.0 / 3.141592653589793));
			this.rotation.x = -EntityAlive.UpdateRotation(this.rotation.x, intendedRotation2, _dPitch);
		}
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x000C60F4 File Offset: 0x000C42F4
	public static float UpdateRotation(float _curRotation, float _intendedRotation, float _maxIncr)
	{
		float num;
		for (num = _intendedRotation - _curRotation; num < -180f; num += 360f)
		{
		}
		while (num >= 180f)
		{
			num -= 360f;
		}
		if (num > _maxIncr)
		{
			num = _maxIncr;
		}
		if (num < -_maxIncr)
		{
			num = -_maxIncr;
		}
		return _curRotation + num;
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x000C613C File Offset: 0x000C433C
	public override float GetEyeHeight()
	{
		if (this.walkType == 21)
		{
			return 0.15f;
		}
		if (this.walkType == 22)
		{
			return 0.6f;
		}
		if (!this.IsCrouching)
		{
			return base.height * 0.8f;
		}
		return base.height * 0.5f;
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x000C618A File Offset: 0x000C438A
	public virtual float GetSpeedModifier()
	{
		return this.speedModifier;
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000C6194 File Offset: 0x000C4394
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fallHitGround(float _distance, Vector3 _fallMotion)
	{
		base.fallHitGround(_distance, _fallMotion);
		if (_distance > 2f)
		{
			int num = (int)((-_fallMotion.y - 0.85f) * 160f);
			if (num > 0)
			{
				this.DamageEntity(DamageSource.fall, num, false, 1f);
			}
			this.PlayHitGroundSound();
		}
		if (!this.IsDead() && !this.emodel.IsRagdollActive && (this.disableFallBehaviorUntilOnGround || !this.ChooseFallBehavior(_distance, _fallMotion)) && this.emodel && this.emodel.avatarController)
		{
			this.emodel.avatarController.StartAnimationJump(AnimJumpMode.Land);
		}
		if (this.aiManager != null)
		{
			this.aiManager.FallHitGround(_distance);
		}
	}

	// Token: 0x06001FC5 RID: 8133 RVA: 0x000C6250 File Offset: 0x000C4450
	public bool NotifyDestroyedBlock(ItemActionAttack.AttackHitInfo attackHitInfo)
	{
		if (attackHitInfo == null || this.moveHelper == null || this.moveHelper.BlockedFlags <= 0)
		{
			return false;
		}
		if (this.moveHelper.HitInfo.hit.blockPos == attackHitInfo.hitPosition)
		{
			this.moveHelper.ClearBlocked();
		}
		if (this._destroyBlockBehaviors.Count == 0)
		{
			return false;
		}
		float num = 0f;
		EntityAlive.weightBehaviorTemp.Clear();
		int @int = GameStats.GetInt(EnumGameStats.GameDifficulty);
		for (int i = 0; i < this._destroyBlockBehaviors.Count; i++)
		{
			EntityAlive.DestroyBlockBehavior destroyBlockBehavior = this._destroyBlockBehaviors[i];
			if (@int >= destroyBlockBehavior.Difficulty.min && @int <= destroyBlockBehavior.Difficulty.max)
			{
				EntityAlive.WeightBehavior item;
				item.weight = destroyBlockBehavior.Weight + num;
				item.index = i;
				EntityAlive.weightBehaviorTemp.Add(item);
				num += destroyBlockBehavior.Weight;
			}
		}
		bool result = false;
		if (num > 0f)
		{
			EntityAlive.DestroyBlockBehavior destroyBlockBehavior2 = null;
			float num2 = this.rand.RandomFloat * num;
			for (int j = 0; j < EntityAlive.weightBehaviorTemp.Count; j++)
			{
				if (num2 <= EntityAlive.weightBehaviorTemp[j].weight)
				{
					destroyBlockBehavior2 = this._destroyBlockBehaviors[EntityAlive.weightBehaviorTemp[j].index];
					break;
				}
			}
			if (destroyBlockBehavior2 != null)
			{
				result = this.ExecuteDestroyBlockBehavior(destroyBlockBehavior2, attackHitInfo);
			}
		}
		return result;
	}

	// Token: 0x06001FC6 RID: 8134 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool ExecuteDestroyBlockBehavior(EntityAlive.DestroyBlockBehavior behavior, ItemActionAttack.AttackHitInfo attackHitInfo)
	{
		return false;
	}

	// Token: 0x06001FC7 RID: 8135 RVA: 0x000C63C4 File Offset: 0x000C45C4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ChooseFallBehavior(float _distance, Vector3 _fallMotion)
	{
		if (this.fallBehaviors.Count == 0)
		{
			return false;
		}
		float num = 0f;
		EntityAlive.weightBehaviorTemp.Clear();
		int @int = GameStats.GetInt(EnumGameStats.GameDifficulty);
		for (int i = 0; i < this.fallBehaviors.Count; i++)
		{
			EntityAlive.FallBehavior fallBehavior = this.fallBehaviors[i];
			if (_distance >= fallBehavior.Height.min && _distance <= fallBehavior.Height.max && @int >= fallBehavior.Difficulty.min && @int <= fallBehavior.Difficulty.max)
			{
				EntityAlive.WeightBehavior item;
				item.weight = fallBehavior.Weight + num;
				item.index = i;
				EntityAlive.weightBehaviorTemp.Add(item);
				num += fallBehavior.Weight;
			}
		}
		bool result = false;
		if (num > 0f)
		{
			EntityAlive.FallBehavior fallBehavior2 = null;
			float num2 = this.rand.RandomFloat * num;
			for (int j = 0; j < EntityAlive.weightBehaviorTemp.Count; j++)
			{
				if (num2 <= EntityAlive.weightBehaviorTemp[j].weight)
				{
					fallBehavior2 = this.fallBehaviors[EntityAlive.weightBehaviorTemp[j].index];
					break;
				}
			}
			if (fallBehavior2 != null)
			{
				result = this.ExecuteFallBehavior(fallBehavior2, _distance, _fallMotion);
			}
		}
		return result;
	}

	// Token: 0x06001FC8 RID: 8136 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool ExecuteFallBehavior(EntityAlive.FallBehavior behavior, float _distance, Vector3 _fallMotion)
	{
		return false;
	}

	// Token: 0x06001FC9 RID: 8137 RVA: 0x000C6508 File Offset: 0x000C4708
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayHitGroundSound()
	{
		if (this.soundLand == null || this.soundLand.Length == 0)
		{
			this.PlayOneShot("entityhitsground", false, false, false, null);
			return;
		}
		this.PlayOneShot(this.soundLand, false, false, false, null);
	}

	// Token: 0x06001FCA RID: 8138 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool FriendlyFireCheck(EntityAlive other)
	{
		return true;
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool HasImmunity(BuffClass _buffClass)
	{
		return false;
	}

	// Token: 0x06001FCC RID: 8140 RVA: 0x000C653F File Offset: 0x000C473F
	public int CalculateBlockDamage(BlockDamage block, int defaultBlockDamage, out bool bypassMaxDamage)
	{
		if (this.stompsSpikes && block.HasTag(BlockTags.Spike))
		{
			bypassMaxDamage = true;
			return 999;
		}
		bypassMaxDamage = false;
		return defaultBlockDamage;
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x000C6560 File Offset: 0x000C4760
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale = 1f)
	{
		if (_damageSource.damageType == EnumDamageTypes.Suicide && this.emodel && this.emodel.avatarController is AvatarZombieController)
		{
			(this.emodel.avatarController as AvatarZombieController).CleanupDismemberedLimbs();
		}
		EnumDamageSource source = _damageSource.GetSource();
		if (_damageSource.IsIgnoreConsecutiveDamages() && source != EnumDamageSource.Internal)
		{
			if (this.damageSourceTimeouts.ContainsKey(source) && GameTimer.Instance.ticks - this.damageSourceTimeouts[source] < 30UL)
			{
				return -1;
			}
			this.damageSourceTimeouts[source] = GameTimer.Instance.ticks;
		}
		EntityAlive entityAlive = this.world.GetEntity(_damageSource.getEntityId()) as EntityAlive;
		if (!this.FriendlyFireCheck(entityAlive))
		{
			return -1;
		}
		bool flag = _damageSource.GetDamageType() == EnumDamageTypes.Heat;
		if (!flag && entityAlive && (this.entityFlags & entityAlive.entityFlags & EntityFlags.Zombie) > EntityFlags.None)
		{
			return -1;
		}
		if (this.IsGodMode.Value)
		{
			return -1;
		}
		if (!this.IsDead() && entityAlive)
		{
			float value = EffectManager.GetValue(PassiveEffects.DamageBonus, null, 0f, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			if (value > 0f)
			{
				_damageSource.DamageMultiplier = value;
				_damageSource.BonusDamageType = EnumDamageBonusType.Sneak;
			}
		}
		this.MinEventContext.Other = entityAlive;
		float num = Utils.FastMin(1f, EffectManager.GetValue(PassiveEffects.GeneralDamageResist, null, 0f, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		float num2 = (float)_strength * num + this.accumulatedDamageResisted;
		int num3 = Utils.FastMin(_strength, (int)num2);
		this.accumulatedDamageResisted = num2 - (float)num3;
		_strength -= num3;
		DamageResponse damageResponse = this.damageEntityLocal(_damageSource, _strength, _criticalHit, _impulseScale);
		NetPackage package = NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, damageResponse);
		if (this.world.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
		}
		else
		{
			int excludePlayer = -1;
			if (!flag && _damageSource.CreatorEntityId != -2)
			{
				excludePlayer = _damageSource.getEntityId();
				if (_damageSource.CreatorEntityId != -1)
				{
					Entity entity = this.world.GetEntity(_damageSource.CreatorEntityId);
					if (entity && !entity.isEntityRemote)
					{
						excludePlayer = -1;
					}
				}
			}
			this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, excludePlayer, package, false);
		}
		return damageResponse.ModStrength;
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x000C67B2 File Offset: 0x000C49B2
	public virtual void SetDamagedTarget(EntityAlive _attackTarget)
	{
		this.damagedTarget = _attackTarget;
	}

	// Token: 0x06001FCF RID: 8143 RVA: 0x000C67BB File Offset: 0x000C49BB
	public virtual void ClearDamagedTarget()
	{
		this.damagedTarget = null;
	}

	// Token: 0x06001FD0 RID: 8144 RVA: 0x000C67C4 File Offset: 0x000C49C4
	public EntityAlive GetDamagedTarget()
	{
		return this.damagedTarget;
	}

	// Token: 0x06001FD1 RID: 8145 RVA: 0x000C67CC File Offset: 0x000C49CC
	public override bool IsDead()
	{
		return base.IsDead() || this.RecordedDamage.Fatal;
	}

	// Token: 0x06001FD2 RID: 8146 RVA: 0x000C67E4 File Offset: 0x000C49E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual DamageResponse damageEntityLocal(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		DamageResponse damageResponse = default(DamageResponse);
		damageResponse.Source = _damageSource;
		damageResponse.Strength = _strength;
		damageResponse.Critical = _criticalHit;
		damageResponse.HitDirection = Utils.EnumHitDirection.None;
		damageResponse.MovementState = this.MovementState;
		damageResponse.Random = this.rand.RandomFloat;
		damageResponse.ImpulseScale = impulseScale;
		damageResponse.HitBodyPart = _damageSource.GetEntityDamageBodyPart(this);
		damageResponse.ArmorSlot = _damageSource.GetEntityDamageEquipmentSlot(this);
		damageResponse.ArmorSlotGroup = _damageSource.GetEntityDamageEquipmentSlotGroup(this);
		if (_strength > 0)
		{
			damageResponse.HitDirection = (_damageSource.Equals(DamageSource.fall) ? Utils.EnumHitDirection.Back : ((Utils.EnumHitDirection)Utils.Get4HitDirectionAsInt(_damageSource.getDirection(), this.GetLookVector())));
		}
		if (!GameManager.IsDedicatedServer && _damageSource.damageSource != EnumDamageSource.Internal && GameManager.Instance != null)
		{
			World world = GameManager.Instance.World;
			if (world != null && _damageSource.getEntityId() == world.GetPrimaryPlayerId())
			{
				Transform hitTransform = this.emodel.GetHitTransform(_damageSource);
				Vector3 position;
				if (hitTransform)
				{
					position = hitTransform.position;
				}
				else
				{
					position = this.emodel.transform.position;
				}
				bool flag = world.GetPrimaryPlayer().inventory.holdingItem.HasAnyTags(FastTags<TagGroup.Global>.Parse("ranged"));
				float magnitude = (world.GetPrimaryPlayer().GetPosition() - position).magnitude;
				if (flag && magnitude > EntityAlive.HitSoundDistance)
				{
					Manager.PlayInsidePlayerHead("HitEntitySound", -1, 0f, false, false);
				}
				if (EntityAlive.ShowDebugDisplayHit)
				{
					Transform transform = hitTransform ? hitTransform : this.emodel.transform;
					Vector3 position2 = Camera.main.transform.position;
					DebugLines.CreateAttached("EntityDamage" + this.entityId.ToString(), transform, position2 + Origin.position, _damageSource.getHitTransformPosition(), new Color(0.3f, 0f, 0.3f), new Color(1f, 0f, 1f), EntityAlive.DebugDisplayHitSize * 2f, EntityAlive.DebugDisplayHitSize, EntityAlive.DebugDisplayHitTime);
					DebugLines.CreateAttached("EntityDamage2" + this.entityId.ToString(), transform, _damageSource.getHitTransformPosition(), transform.position + Origin.position, new Color(0f, 0f, 0.5f), new Color(0.3f, 0.3f, 1f), EntityAlive.DebugDisplayHitSize * 2f, EntityAlive.DebugDisplayHitSize, EntityAlive.DebugDisplayHitTime);
				}
			}
		}
		if (_damageSource.AffectedByArmor())
		{
			this.equipment.CalcDamage(ref damageResponse.Strength, ref damageResponse.ArmorDamage, damageResponse.Source.DamageTypeTag, this.MinEventContext.Other, damageResponse.Source.AttackingItem);
		}
		float num = this.GetDamageFraction((float)damageResponse.Strength);
		if (damageResponse.Fatal || damageResponse.Strength >= this.Health)
		{
			if ((damageResponse.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
			{
				if (num >= 0.2f)
				{
					damageResponse.Source.DismemberChance = Utils.FastMax(damageResponse.Source.DismemberChance * 0.5f, 0.3f);
				}
			}
			else if (num >= 0.12f)
			{
				damageResponse.Source.DismemberChance = Utils.FastMax(damageResponse.Source.DismemberChance * 0.5f, 0.5f);
			}
			num = 1f;
			if (this.canDisintegrate)
			{
				this.Disintegrate();
			}
		}
		this.CheckDismember(ref damageResponse, num);
		int num2 = this.bodyDamage.StunKnee;
		int num3 = this.bodyDamage.StunProne;
		if ((damageResponse.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None && damageResponse.Dismember)
		{
			if (this.Health > 0)
			{
				damageResponse.Strength = this.Health;
			}
		}
		else if (_damageSource.CanStun && this.GetWalkType() != 21 && this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
		{
			if ((damageResponse.HitBodyPart & (EnumBodyPartHit.Torso | EnumBodyPartHit.Head | EnumBodyPartHit.LeftUpperArm | EnumBodyPartHit.RightUpperArm | EnumBodyPartHit.LeftLowerArm | EnumBodyPartHit.RightLowerArm)) > EnumBodyPartHit.None)
			{
				num3 += _strength;
			}
			else if (damageResponse.HitBodyPart.IsLeg())
			{
				num2 += _strength * (_criticalHit ? 2 : 1);
			}
		}
		if ((!damageResponse.HitBodyPart.IsLeg() || !damageResponse.Dismember) && this.GetWalkType() != 21 && !this.sleepingOrWakingUp)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			if (this.GetDamageFraction((float)num3) >= entityClass.KnockdownProneDamageThreshold && entityClass.KnockdownProneDamageThreshold > 0f)
			{
				if (this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
				{
					damageResponse.Stun = EnumEntityStunType.Prone;
					damageResponse.StunDuration = this.rand.RandomRange(entityClass.KnockdownProneStunDuration.x, entityClass.KnockdownProneStunDuration.y);
				}
			}
			else if (this.GetDamageFraction((float)num2) >= entityClass.KnockdownKneelDamageThreshold && entityClass.KnockdownKneelDamageThreshold > 0f && this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
			{
				damageResponse.Stun = EnumEntityStunType.Kneel;
				damageResponse.StunDuration = this.rand.RandomRange(entityClass.KnockdownKneelStunDuration.x, entityClass.KnockdownKneelStunDuration.y);
			}
		}
		bool flag2 = false;
		int num4 = damageResponse.Strength + damageResponse.ArmorDamage / 2;
		if (num4 > 0 && !this.IsGodMode.Value && damageResponse.Stun == EnumEntityStunType.None && !this.sleepingOrWakingUp)
		{
			flag2 = (damageResponse.Strength < this.Health);
			if (flag2)
			{
				flag2 = (this.GetWalkType() == 21 || !damageResponse.Dismember || !damageResponse.HitBodyPart.IsLeg());
			}
			if (flag2 && damageResponse.Source.GetDamageType() != EnumDamageTypes.Bashing)
			{
				flag2 = (num4 >= 6);
			}
			if (damageResponse.Source.GetDamageType() == EnumDamageTypes.BarbedWire)
			{
				flag2 = true;
			}
		}
		damageResponse.PainHit = flag2;
		if (damageResponse.Strength >= this.Health)
		{
			damageResponse.Fatal = true;
		}
		if (damageResponse.Fatal)
		{
			damageResponse.Stun = EnumEntityStunType.None;
		}
		if (this.isEntityRemote)
		{
			damageResponse.ModStrength = 0;
		}
		else
		{
			if (this.Health <= damageResponse.Strength)
			{
				_strength -= this.Health;
			}
			damageResponse.ModStrength = _strength;
		}
		if (damageResponse.Dismember)
		{
			EntityAlive entityAlive = this.world.GetEntity(damageResponse.Source.getEntityId()) as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.FireEvent(MinEventTypes.onDismember, true);
			}
		}
		if (this.MinEventContext.Other != null)
		{
			this.MinEventContext.Other.MinEventContext.DamageResponse = damageResponse;
			float value = EffectManager.GetValue(PassiveEffects.HealthSteal, null, 0f, this.MinEventContext.Other, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			if (value != 0f)
			{
				int num5 = (int)((float)num4 * value);
				if (num5 + this.MinEventContext.Other.Health <= 0)
				{
					num5 = (this.MinEventContext.Other.Health - 1) * -1;
				}
				this.MinEventContext.Other.AddHealth(num5);
				if (num5 < 0 && this.MinEventContext.Other is EntityPlayerLocal)
				{
					((EntityPlayerLocal)this.MinEventContext.Other).ForceBloodSplatter();
				}
			}
		}
		this.FireAttackedEvents(damageResponse);
		this.ProcessDamageResponseLocal(damageResponse);
		return damageResponse;
	}

	// Token: 0x06001FD3 RID: 8147 RVA: 0x000C6F28 File Offset: 0x000C5128
	public override void FireAttackedEvents(DamageResponse _dmResponse)
	{
		base.FireAttackedEvents(_dmResponse);
		if (_dmResponse.Source.BuffClass == null || this.Progression != null)
		{
			this.MinEventContext.DamageResponse = _dmResponse;
			EntityAlive entityAlive = this.world.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive;
			if (entityAlive && !entityAlive.isEntityRemote)
			{
				this.MinEventContext.IsLocal = (this is EntityPlayer && this.isEntityRemote);
			}
			if (_dmResponse.Source.BuffClass == null)
			{
				this.FireEvent(MinEventTypes.onOtherAttackedSelf, true);
			}
			else if (this.Progression != null)
			{
				this.Progression.FireEvent(MinEventTypes.onOtherAttackedSelf, this.MinEventContext);
			}
			this.MinEventContext.IsLocal = false;
		}
	}

	// Token: 0x17000399 RID: 921
	// (get) Token: 0x06001FD4 RID: 8148 RVA: 0x000C6FE8 File Offset: 0x000C51E8
	public virtual bool IsImmuneToLegDamage
	{
		get
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			return this.GetWalkType() == 21 || !this.bodyDamage.HasLeftLeg || !this.bodyDamage.HasRightLeg || (entityClass.LowerLegDismemberThreshold <= 0f && entityClass.UpperLegDismemberThreshold <= 0f);
		}
	}

	// Token: 0x06001FD5 RID: 8149 RVA: 0x000C704C File Offset: 0x000C524C
	public override void ProcessDamageResponse(DamageResponse _dmResponse)
	{
		if (Time.time - this.lastAliveTime < 1f)
		{
			return;
		}
		base.ProcessDamageResponse(_dmResponse);
		this.ProcessDamageResponseLocal(_dmResponse);
		if (!this.world.IsRemote())
		{
			Entity entity = this.world.GetEntity(_dmResponse.Source.getEntityId());
			if (entity && !entity.isEntityRemote && this.isEntityRemote && this is EntityPlayer)
			{
				this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.entityId, NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, _dmResponse), false);
				return;
			}
			if (_dmResponse.Source.BuffClass != null)
			{
				this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.entityId, NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, _dmResponse), false);
				return;
			}
			this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, _dmResponse.Source.getEntityId(), NetPackageManager.GetPackage<NetPackageDamageEntity>().Setup(this.entityId, _dmResponse), false);
		}
	}

	// Token: 0x06001FD6 RID: 8150 RVA: 0x000C7160 File Offset: 0x000C5360
	public virtual void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		if (this.emodel == null)
		{
			return;
		}
		if (_dmResponse.Source.BonusDamageType != EnumDamageBonusType.None)
		{
			EntityPlayerLocal primaryPlayer = this.world.GetPrimaryPlayer();
			if (primaryPlayer && primaryPlayer.entityId == _dmResponse.Source.getEntityId())
			{
				EnumDamageBonusType bonusDamageType = _dmResponse.Source.BonusDamageType;
				if (bonusDamageType != EnumDamageBonusType.Sneak)
				{
					if (bonusDamageType == EnumDamageBonusType.Stun)
					{
						primaryPlayer.NotifyDamageMultiplier(_dmResponse.Source.DamageMultiplier);
					}
				}
				else
				{
					primaryPlayer.NotifySneakDamage(_dmResponse.Source.DamageMultiplier);
				}
			}
		}
		EntityAlive entityAlive = this.world.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive;
		if (entityAlive != null)
		{
			entityAlive.SetDamagedTarget(this);
		}
		if (this.IsSleeperPassive)
		{
			this.world.CheckSleeperVolumeNoise(this.position);
		}
		this.ConditionalTriggerSleeperWakeUp();
		this.SleeperSupressLivingSounds = false;
		this.bPlayHurtSound = false;
		if (this.equipment != null && _dmResponse.ArmorDamage > 0)
		{
			List<ItemValue> armor = this.equipment.GetArmor();
			if (armor.Count > 0)
			{
				float num = (float)_dmResponse.ArmorDamage / (float)armor.Count;
				if (num < 1f && num != 0f)
				{
					num = 1f;
				}
				for (int i = 0; i < armor.Count; i++)
				{
					armor[i].UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, armor[i], num, this, null, armor[i].ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
				}
			}
		}
		this.ApplyLocalBodyDamage(_dmResponse);
		this.lastHitRanged = false;
		this.lastDamageResponse = _dmResponse;
		bool flag = EffectManager.GetValue(PassiveEffects.NegateDamageSelf, null, 0f, this, null, FastTags<TagGroup.Global>.Parse(_dmResponse.HitBodyPart.ToString()), true, true, true, true, true, 1, true, false) > 0f || EffectManager.GetValue(PassiveEffects.NegateDamageOther, (entityAlive != null) ? entityAlive.inventory.holdingItemItemValue : null, 0f, entityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f;
		if (_dmResponse.Dismember && !flag)
		{
			this.lastHitImpactDir = _dmResponse.Source.getDirection();
			if (entityAlive != null)
			{
				this.lastHitEntityFwd = entityAlive.GetForwardVector();
			}
			if (_dmResponse.Source.ItemClass != null && _dmResponse.Source.ItemClass.HasAnyTags(DismembermentManager.rangedTags))
			{
				this.lastHitRanged = true;
			}
			if (_dmResponse.Source.ItemClass != null)
			{
				float strength = (float)_dmResponse.ModStrength / (float)this.GetMaxHealth();
				this.lastHitForce = DismembermentManager.GetImpactForce(_dmResponse.Source.ItemClass, strength);
			}
			this.ExecuteDismember(false);
		}
		bool flag2 = _dmResponse.Stun > EnumEntityStunType.None;
		bool flag3 = this.bodyDamage.CurrentStun > EnumEntityStunType.None;
		if (!flag && _dmResponse.Fatal && this.isEntityRemote)
		{
			this.ClientKill(_dmResponse);
		}
		else if (flag2 && this.emodel.avatarController)
		{
			if (_dmResponse.Stun == EnumEntityStunType.Prone)
			{
				if (this.bodyDamage.CurrentStun == EnumEntityStunType.None)
				{
					if ((_dmResponse.Critical && _dmResponse.Source.damageType == EnumDamageTypes.Bashing) || this.rand.RandomFloat < 0.6f)
					{
						this.DoRagdoll(_dmResponse);
					}
					else
					{
						this.emodel.avatarController.BeginStun(EnumEntityStunType.Prone, _dmResponse.HitBodyPart, _dmResponse.HitDirection, _dmResponse.Critical, _dmResponse.Random);
					}
					this.SetStun(EnumEntityStunType.Prone);
					this.bodyDamage.StunDuration = _dmResponse.StunDuration;
				}
				else if (this.bodyDamage.CurrentStun != EnumEntityStunType.Prone)
				{
					this.DoRagdoll(_dmResponse);
					this.SetStun(EnumEntityStunType.Prone);
					this.bodyDamage.StunDuration = _dmResponse.StunDuration * 0.5f;
				}
			}
			else if (_dmResponse.Stun == EnumEntityStunType.Kneel)
			{
				bool flag4 = false;
				if (this.bodyDamage.CurrentStun == EnumEntityStunType.None)
				{
					if (_dmResponse.Critical || this.rand.RandomFloat < 0.25f)
					{
						flag4 = true;
					}
					else
					{
						this.SetStun(EnumEntityStunType.Kneel);
						this.emodel.avatarController.BeginStun(EnumEntityStunType.Kneel, _dmResponse.HitBodyPart, _dmResponse.HitDirection, _dmResponse.Critical, _dmResponse.Random);
					}
				}
				else if (this.bodyDamage.CurrentStun == EnumEntityStunType.Kneel)
				{
					flag4 = true;
				}
				if (flag4)
				{
					this.DoRagdoll(_dmResponse);
					this.SetStun(EnumEntityStunType.Prone);
				}
				this.bodyDamage.StunDuration = _dmResponse.StunDuration;
			}
		}
		else if (_dmResponse.PainHit && !flag3 && this.emodel.avatarController)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			float num2 = entityClass.PainResistPerHit;
			if (num2 >= 0f)
			{
				float num3 = (float)this.GetMaxHealth();
				if ((float)this.Health / num3 < entityClass.PainResistPerHitLowHealthPercent)
				{
					num2 = entityClass.PainResistPerHitLowHealth;
				}
				this.painResistPercent = Utils.FastMin(this.painResistPercent + num2, 3f);
				float duration = float.MaxValue;
				if (this.painResistPercent >= 3f && num2 >= 1f)
				{
					duration = 0f;
					this.painHitsFelt += 0.15f;
				}
				else if (this.painResistPercent >= 1f)
				{
					duration = Utils.FastLerp(0.5f, 0.15f, (this.painResistPercent - 1f) * 0.75f);
					this.painHitsFelt += 0.3f;
				}
				else
				{
					this.painHitsFelt += Utils.FastLerp(1f, 0.3f, this.painResistPercent);
				}
				this.emodel.avatarController.StartAnimationHit(_dmResponse.HitBodyPart, (int)_dmResponse.HitDirection, (int)((float)_dmResponse.Strength * 100f / num3), _dmResponse.Critical, _dmResponse.MovementState, _dmResponse.Random, duration);
			}
		}
		if (this.bodyDamage.CurrentStun == EnumEntityStunType.None)
		{
			if (_dmResponse.Source.CanStun)
			{
				if ((_dmResponse.HitBodyPart & (EnumBodyPartHit.Torso | EnumBodyPartHit.Head | EnumBodyPartHit.LeftUpperArm | EnumBodyPartHit.RightUpperArm | EnumBodyPartHit.LeftLowerArm | EnumBodyPartHit.RightLowerArm)) > EnumBodyPartHit.None)
				{
					this.bodyDamage.StunProne = this.bodyDamage.StunProne + _dmResponse.Strength;
				}
				else if (_dmResponse.HitBodyPart.IsLeg())
				{
					this.bodyDamage.StunKnee = this.bodyDamage.StunKnee + _dmResponse.Strength;
				}
			}
		}
		else
		{
			this.bodyDamage.StunProne = 0;
			this.bodyDamage.StunKnee = 0;
		}
		bool flag5 = this.Health <= 0;
		if (this.Health <= 0 && this.deathUpdateTime > 0)
		{
			this.DeathHealth -= _dmResponse.Strength;
		}
		int num4 = _dmResponse.Strength;
		if (EffectManager.GetValue(PassiveEffects.HeadShotOnly, null, 0f, GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f && (_dmResponse.HitBodyPart & EnumBodyPartHit.Head) == EnumBodyPartHit.None)
		{
			num4 = 0;
			_dmResponse.Fatal = false;
		}
		if (flag)
		{
			num4 = 0;
			_dmResponse.Fatal = false;
		}
		if (this.isEntityRemote)
		{
			this.Health -= num4;
			this.RecordedDamage = _dmResponse;
		}
		else
		{
			if (!this.IsGodMode.Value)
			{
				this.Health -= num4;
				if (_dmResponse.Fatal && this.Health > 0)
				{
					this.Health = 0;
				}
				this.hasBeenAttackedTime = 0;
				if (_dmResponse.PainHit)
				{
					this.hasBeenAttackedTime = this.GetMaxAttackTime();
				}
			}
			this.bPlayHurtSound = (this.bBeenWounded = (num4 > 0));
			if (this.bBeenWounded)
			{
				base.setBeenAttacked();
				this.MinEventContext.Other = (GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive);
				this.FireEvent(MinEventTypes.onOtherDamagedSelf, true);
			}
			if (num4 > this.woundedStrength)
			{
				this.woundedStrength = _dmResponse.Strength;
				this.woundedDamageSource = _dmResponse.Source;
			}
			this.lastHitDirection = _dmResponse.HitDirection;
			if (this.Health <= 0)
			{
				_dmResponse.Source.getDirection();
				_dmResponse.Strength += this.Health;
				Entity entity = (_dmResponse.Source.getEntityId() != -1) ? this.world.GetEntity(_dmResponse.Source.getEntityId()) : null;
				if (this.Spawned && !flag5)
				{
					if (entity is EntityAlive)
					{
						this.entityThatKilledMe = (EntityAlive)entity;
					}
					else
					{
						this.entityThatKilledMe = null;
					}
				}
				this.Kill(_dmResponse);
				if (!_dmResponse.Fatal && this.world.IsRemote())
				{
					this.DamageEntity(DamageSource.disease, 1, false, 1f);
				}
			}
		}
		Entity entity2 = (_dmResponse.Source.getEntityId() != -1) ? this.world.GetEntity(_dmResponse.Source.getEntityId()) : null;
		if (entity2 != null && entity2 != this)
		{
			if (entity2 is EntityAlive && !this.isEntityRemote && !entity2.IsIgnoredByAI())
			{
				this.SetRevengeTarget((EntityAlive)entity2);
				if (this.aiManager != null)
				{
					this.aiManager.DamagedByEntity();
				}
			}
			if (entity2 is EntityPlayer)
			{
				((EntityPlayer)entity2).FireEvent(MinEventTypes.onCombatEntered, true);
			}
			this.FireEvent(MinEventTypes.onCombatEntered, true);
		}
		if (_dmResponse.Strength > 0 && _dmResponse.Source.GetDamageType() == EnumDamageTypes.Electrical)
		{
			this.Electrocuted = true;
		}
		if (!GameManager.IsDedicatedServer && DamageText.Enabled && (this.world.GetPrimaryPlayer().cameraTransform.position + Origin.position - this.position).sqrMagnitude < 225f)
		{
			string text = string.Format("{0}", _dmResponse.Strength);
			Color color = ((_dmResponse.HitBodyPart & EnumBodyPartHit.Head) > EnumBodyPartHit.None) ? Color.red : Color.yellow;
			if (_dmResponse.Critical)
			{
				color.b = 0.8f;
			}
			DamageText.Create(text, color, this.getHeadPosition() + new Vector3(0f, 0.1f, 0f), new Vector3(this.rand.RandomRange(-0.7f, 0.7f), 0.8f, this.rand.RandomRange(-0.7f, 0.7f)), 0.22f);
		}
		this.RecordedDamage = _dmResponse;
	}

	// Token: 0x06001FD7 RID: 8151 RVA: 0x000C7BD8 File Offset: 0x000C5DD8
	public bool CanUseHeavyArmorSound()
	{
		using (List<ItemValue>.Enumerator enumerator = this.equipment.GetArmor().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.ItemClass.MadeOfMaterial.id == "MarmorHeavy")
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001FD8 RID: 8152 RVA: 0x000C7C4C File Offset: 0x000C5E4C
	public EntityAlive GetRevengeTarget()
	{
		return this.revengeEntity;
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x000C7C54 File Offset: 0x000C5E54
	public void SetRevengeTarget(EntityAlive _other)
	{
		this.revengeEntity = _other;
		this.revengeTimer = ((this.revengeEntity == null) ? 0 : 500);
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x000C7C79 File Offset: 0x000C5E79
	public void SetRevengeTimer(int ticks)
	{
		this.revengeTimer = ticks;
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x000B080C File Offset: 0x000AEA0C
	public override bool CanBePushed()
	{
		return !this.IsDead();
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x000C7C82 File Offset: 0x000C5E82
	public override bool CanCollideWith(Entity _other)
	{
		return !this.IsDead() && !(_other is EntityItem) && !(_other is EntitySupplyCrate);
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x000C7CA2 File Offset: 0x000C5EA2
	public override bool CanCollideWithBlocks()
	{
		return !this.IsSleeping;
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x000C7CAF File Offset: 0x000C5EAF
	public void DoRagdoll(DamageResponse _dmResponse)
	{
		this.emodel.DoRagdoll(_dmResponse, _dmResponse.StunDuration);
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x000C7CC4 File Offset: 0x000C5EC4
	public void AddScore(int _diedMySelfTimes, int _zombieKills, int _playerKills, int _otherTeamnumber, int _conditions)
	{
		this.KilledZombies += _zombieKills;
		this.KilledPlayers += _playerKills;
		this.Died += _diedMySelfTimes;
		this.Score += _zombieKills * GameStats.GetInt(EnumGameStats.ScoreZombieKillMultiplier) + _playerKills * GameStats.GetInt(EnumGameStats.ScorePlayerKillMultiplier) + _diedMySelfTimes * GameStats.GetInt(EnumGameStats.ScoreDiedMultiplier);
		if (this.Score < 0)
		{
			this.Score = 0;
		}
		if (this is EntityPlayerLocal)
		{
			if (_diedMySelfTimes > 0)
			{
				IAchievementManager achievementManager = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager != null)
				{
					achievementManager.SetAchievementStat(EnumAchievementDataStat.Deaths, _diedMySelfTimes);
				}
			}
			if (_zombieKills > 0)
			{
				IAchievementManager achievementManager2 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager2 != null)
				{
					achievementManager2.SetAchievementStat(EnumAchievementDataStat.ZombiesKilled, _zombieKills);
				}
			}
			if (_playerKills > 0)
			{
				IAchievementManager achievementManager3 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager3 != null)
				{
					achievementManager3.SetAchievementStat(EnumAchievementDataStat.PlayersKilled, _playerKills);
				}
			}
			if ((_conditions & 2) != 0)
			{
				IAchievementManager achievementManager4 = PlatformManager.NativePlatform.AchievementManager;
				if (achievementManager4 == null)
				{
					return;
				}
				achievementManager4.SetAchievementStat(EnumAchievementDataStat.KilledWith44Magnum, 1);
			}
		}
	}

	// Token: 0x06001FE0 RID: 8160 RVA: 0x000C7DAC File Offset: 0x000C5FAC
	public virtual void AwardKill(EntityAlive killer)
	{
		if (killer != null && killer != this)
		{
			int num = 0;
			int num2 = 0;
			int conditions = 0;
			EntityType entityType = this.entityType;
			if (entityType != EntityType.Player)
			{
				if (entityType == EntityType.Zombie)
				{
					num++;
				}
			}
			else
			{
				num2++;
			}
			EntityPlayer entityPlayer = killer as EntityPlayer;
			if (entityPlayer)
			{
				GameManager.Instance.AwardKill(killer, this);
				if (entityPlayer.inventory.IsHoldingGun() && entityPlayer.inventory.holdingItem.Name.Equals("gunHandgunT2Magnum44"))
				{
					conditions = 2;
				}
				GameManager.Instance.AddScoreServer(killer.entityId, num, num2, this.TeamNumber, conditions);
			}
		}
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x000C7E54 File Offset: 0x000C6054
	public virtual void OnEntityDeath()
	{
		if (this.deathUpdateTime != 0)
		{
			return;
		}
		this.AddScore(1, 0, 0, -1, 0);
		if (this.soundLiving != null && this.soundLivingID >= 0)
		{
			Manager.Stop(this.entityId, this.soundLiving);
			this.soundLivingID = -1;
		}
		if (this.AttachedToEntity)
		{
			this.Detach();
		}
		if (this.isEntityRemote)
		{
			return;
		}
		this.AwardKill(this.entityThatKilledMe);
		if (this.particleOnDeath != null && this.particleOnDeath.Length > 0)
		{
			float lightBrightness = this.world.GetLightBrightness(base.GetBlockPosition());
			this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(this.particleOnDeath, this.getHeadPosition(), lightBrightness, Color.white, null, null, false), this.entityId, false, false);
		}
		if (this.isGameMessageOnDeath())
		{
			GameManager.Instance.GameMessage(EnumGameMessages.EntityWasKilled, this, this.entityThatKilledMe);
		}
		if (this.entityThatKilledMe != null)
		{
			Log.Out("Entity {0} {1} killed by {2} {3}", new object[]
			{
				base.GetDebugName(),
				this.entityId,
				this.entityThatKilledMe.GetDebugName(),
				this.entityThatKilledMe.entityId
			});
		}
		else
		{
			Log.Out("Entity {0} {1} killed", new object[]
			{
				base.GetDebugName(),
				this.entityId
			});
		}
		ModEvents.SEntityKilledData sentityKilledData = new ModEvents.SEntityKilledData(this, this.entityThatKilledMe);
		ModEvents.EntityKilled.Invoke(ref sentityKilledData);
		this.dropItemOnDeath();
		this.entityThatKilledMe = null;
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000C7FE0 File Offset: 0x000C61E0
	public void Disintegrate()
	{
		this.timeStayAfterDeath = 0;
		this.isDisintegrated = true;
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x000C7FF0 File Offset: 0x000C61F0
	public virtual void PlayGiveUpSound()
	{
		if (this.soundGiveUp != null)
		{
			this.PlayOneShot(this.soundGiveUp, false, false, false, null);
		}
	}

	// Token: 0x06001FE4 RID: 8164 RVA: 0x000C800A File Offset: 0x000C620A
	public virtual Vector3 GetCameraLook(float _t)
	{
		return this.GetLookVector();
	}

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000C8014 File Offset: 0x000C6214
	public Vector3 GetForwardVector()
	{
		float num = Mathf.Cos(this.rotation.y * 0.0175f - 3.1415927f);
		float num2 = Mathf.Sin(this.rotation.y * 0.0175f - 3.1415927f);
		float num3 = -Mathf.Cos(0f);
		float y = Mathf.Sin(0f);
		return new Vector3(num2 * num3, y, num * num3);
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x000C807C File Offset: 0x000C627C
	public Vector2 GetForwardVector2()
	{
		float f = this.rotation.y * 0.017453292f;
		float y = Mathf.Cos(f);
		return new Vector2(Mathf.Sin(f), y);
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000C80AC File Offset: 0x000C62AC
	public virtual Vector3 GetLookVector()
	{
		float num = Mathf.Cos(this.rotation.y * 0.0175f - 3.1415927f);
		float num2 = Mathf.Sin(this.rotation.y * 0.0175f - 3.1415927f);
		float num3 = -Mathf.Cos(this.rotation.x * 0.0175f);
		float y = Mathf.Sin(this.rotation.x * 0.0175f);
		return new Vector3(num2 * num3, y, num * num3);
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x000C800A File Offset: 0x000C620A
	public virtual Vector3 GetLookVector(Vector3 _altLookVector)
	{
		return this.GetLookVector();
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000C812C File Offset: 0x000C632C
	[PublicizedFrom(EAccessModifier.Protected)]
	public int GetSoundRandomTicks()
	{
		return this.rand.RandomRange(this.soundRandomTicks / 2, this.soundRandomTicks);
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x000C8147 File Offset: 0x000C6347
	[PublicizedFrom(EAccessModifier.Protected)]
	public int GetSoundAlertTicks()
	{
		return this.rand.RandomRange(this.soundAlertTicks / 2, this.soundAlertTicks);
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000C8162 File Offset: 0x000C6362
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundRandom()
	{
		return this.soundRandom;
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x000C816A File Offset: 0x000C636A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundJump()
	{
		return this.soundJump;
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x000C8172 File Offset: 0x000C6372
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundHurt(DamageSource _damageSource, int _damageStrength)
	{
		return this.soundHurt;
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000C817A File Offset: 0x000C637A
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundHurtSmall()
	{
		return this.soundHurtSmall;
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000C8172 File Offset: 0x000C6372
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundHurt()
	{
		return this.soundHurt;
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000C8182 File Offset: 0x000C6382
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundDrownPain()
	{
		return this.soundDrownPain;
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x000C818A File Offset: 0x000C638A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundDeath(DamageSource _damageSource)
	{
		return this.soundDeath;
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000C8192 File Offset: 0x000C6392
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundAttack()
	{
		return this.soundAttack;
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000C819A File Offset: 0x000C639A
	public virtual string GetSoundAlert()
	{
		return this.soundAlert;
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x000C81A2 File Offset: 0x000C63A2
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual string GetSoundStamina()
	{
		return this.soundStamina;
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x000C81AA File Offset: 0x000C63AA
	public virtual Ray GetLookRay()
	{
		return new Ray(this.position + new Vector3(0f, this.GetEyeHeight(), 0f), this.GetLookVector());
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000C81D7 File Offset: 0x000C63D7
	public virtual Ray GetMeleeRay()
	{
		return this.GetLookRay();
	}

	// Token: 0x06001FF7 RID: 8183 RVA: 0x000C81E0 File Offset: 0x000C63E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void dropItemOnDeath()
	{
		for (int i = 0; i < this.inventory.GetItemCount(); i++)
		{
			ItemStack item = this.inventory.GetItem(i);
			ItemClass forId = ItemClass.GetForId(item.itemValue.type);
			if (forId != null && forId.CanDrop(null))
			{
				this.world.GetGameManager().ItemDropServer(item, this.position, new Vector3(0.5f, 0f, 0.5f), -1, Constants.cItemDroppedOnDeathLifetime, false);
				this.inventory.SetItem(i, ItemValue.None.Clone(), 0, true);
			}
		}
		this.inventory.SetFlashlight(false);
		this.equipment.DropItems();
		if (this.world.IsDark())
		{
			this.lootDropProb *= 1f;
		}
		if (this.entityThatKilledMe)
		{
			this.lootDropProb = EffectManager.GetValue(PassiveEffects.LootDropProb, this.entityThatKilledMe.inventory.holdingItemItemValue, this.lootDropProb, this.entityThatKilledMe, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		if (this.lootDropProb > this.rand.RandomFloat)
		{
			GameManager.Instance.DropContentOfLootContainerServer(BlockValue.Air, new Vector3i(this.position), this.entityId, null);
		}
	}

	// Token: 0x06001FF8 RID: 8184 RVA: 0x000C8330 File Offset: 0x000C6530
	public virtual Vector3 GetDropPosition()
	{
		if (this.ParachuteWearing || this.JetpackWearing)
		{
			return base.transform.position + base.transform.forward - Vector3.up * 0.3f + Origin.position;
		}
		return base.transform.position + base.transform.forward + Vector3.up + Origin.position;
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x000C83B6 File Offset: 0x000C65B6
	public virtual void OnFired()
	{
		if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationFiring();
		}
	}

	// Token: 0x06001FFA RID: 8186 RVA: 0x000C83DB File Offset: 0x000C65DB
	public virtual void OnReloadStart()
	{
		if (this.emodel.avatarController != null)
		{
			this.emodel.avatarController.StartAnimationReloading();
		}
	}

	// Token: 0x06001FFB RID: 8187 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnReloadEnd()
	{
	}

	// Token: 0x06001FFC RID: 8188 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool WillForceToFollow(EntityAlive _other)
	{
		return false;
	}

	// Token: 0x06001FFD RID: 8189 RVA: 0x000C8400 File Offset: 0x000C6600
	public void AddHealth(int _v)
	{
		if (this.Health <= 0)
		{
			return;
		}
		this.Health += _v;
	}

	// Token: 0x06001FFE RID: 8190 RVA: 0x000C841A File Offset: 0x000C661A
	public void AddStamina(float _v)
	{
		if (this.entityStats.Stamina != null && this.Health > 0)
		{
			this.entityStats.Stamina.Value += _v;
		}
	}

	// Token: 0x06001FFF RID: 8191 RVA: 0x000C844A File Offset: 0x000C664A
	public void AddWater(float _v)
	{
		this.Stats.Water.Value += _v;
	}

	// Token: 0x06002000 RID: 8192 RVA: 0x000C8464 File Offset: 0x000C6664
	public int GetTicksNoPlayerAdjacent()
	{
		return this.ticksNoPlayerAdjacent;
	}

	// Token: 0x06002001 RID: 8193 RVA: 0x000C846C File Offset: 0x000C666C
	public bool CanSee(EntityAlive _other)
	{
		return this.seeCache.CanSee(_other);
	}

	// Token: 0x06002002 RID: 8194 RVA: 0x000C847A File Offset: 0x000C667A
	public void SetCanSee(EntityAlive _other)
	{
		this.seeCache.SetCanSee(_other);
	}

	// Token: 0x06002003 RID: 8195 RVA: 0x000C8488 File Offset: 0x000C6688
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateTasks()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving))
		{
			this.SetMoveForwardWithModifiers(0f, 0f, false);
			if (this.aiManager != null)
			{
				this.aiManager.UpdateDebugName();
			}
			return;
		}
		this.CheckDespawn();
		this.seeCache.ClearIfExpired();
		bool useAIPackages = EntityClass.list[this.entityClass].UseAIPackages;
		this.aiActiveDelay -= this.aiActiveScale;
		if (this.aiActiveDelay <= 0f)
		{
			this.aiActiveDelay = 1f;
			if (!useAIPackages)
			{
				this.aiManager.Update();
			}
			else
			{
				UAIBase.Update(this.utilityAIContext);
			}
		}
		PathInfo path = PathFinderThread.Instance.GetPath(this.entityId);
		if (path.path != null)
		{
			bool flag = true;
			if (!useAIPackages)
			{
				flag = this.aiManager.CheckPath(path);
			}
			if (flag)
			{
				this.navigator.SetPath(path, path.speed);
			}
		}
		this.navigator.UpdateNavigation();
		this.moveHelper.UpdateMoveHelper();
		this.lookHelper.onUpdateLook();
		if (this.distraction != null && (this.distraction.IsDead() || this.distraction.IsMarkedForUnload()))
		{
			this.distraction = null;
		}
		if (this.pendingDistraction != null && (this.pendingDistraction.IsDead() || this.pendingDistraction.IsMarkedForUnload()))
		{
			this.pendingDistraction = null;
		}
	}

	// Token: 0x06002004 RID: 8196 RVA: 0x000C85F0 File Offset: 0x000C67F0
	public PathNavigate getNavigator()
	{
		return this.navigator;
	}

	// Token: 0x06002005 RID: 8197 RVA: 0x000C85F8 File Offset: 0x000C67F8
	public void FindPath(Vector3 targetPos, float moveSpeed, bool canBreak, EAIBase behavior)
	{
		Vector3 vector = targetPos - this.position;
		if (vector.x * vector.x + vector.z * vector.z > 1225f)
		{
			if (vector.y > 45f)
			{
				targetPos.y = this.position.y + 45f;
			}
			else if (vector.y < -45f)
			{
				targetPos.y = this.position.y - 45f;
			}
		}
		PathFinderThread.Instance.FindPath(this, targetPos, moveSpeed, canBreak, behavior);
	}

	// Token: 0x06002006 RID: 8198 RVA: 0x000C8690 File Offset: 0x000C6890
	public bool isWithinHomeDistanceCurrentPosition()
	{
		return this.isWithinHomeDistance(Utils.Fastfloor(this.position.x), Utils.Fastfloor(this.position.y), Utils.Fastfloor(this.position.z));
	}

	// Token: 0x06002007 RID: 8199 RVA: 0x000C86C8 File Offset: 0x000C68C8
	public bool isWithinHomeDistance(int _x, int _y, int _z)
	{
		return this.maximumHomeDistance < 0 || this.homePosition.getDistanceSquared(_x, _y, _z) < (float)(this.maximumHomeDistance * this.maximumHomeDistance);
	}

	// Token: 0x06002008 RID: 8200 RVA: 0x000C86F3 File Offset: 0x000C68F3
	public void setHomeArea(Vector3i _pos, int _maxDistance)
	{
		this.homePosition.position = _pos;
		this.maximumHomeDistance = _maxDistance;
	}

	// Token: 0x06002009 RID: 8201 RVA: 0x000C8708 File Offset: 0x000C6908
	public ChunkCoordinates getHomePosition()
	{
		return this.homePosition;
	}

	// Token: 0x0600200A RID: 8202 RVA: 0x000C8710 File Offset: 0x000C6910
	public int getMaximumHomeDistance()
	{
		return this.maximumHomeDistance;
	}

	// Token: 0x0600200B RID: 8203 RVA: 0x000C8718 File Offset: 0x000C6918
	public void detachHome()
	{
		this.maximumHomeDistance = -1;
	}

	// Token: 0x0600200C RID: 8204 RVA: 0x000C8721 File Offset: 0x000C6921
	public bool hasHome()
	{
		return this.maximumHomeDistance >= 0;
	}

	// Token: 0x0600200D RID: 8205 RVA: 0x000C872F File Offset: 0x000C692F
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool canDespawn()
	{
		return !this.IsClientControlled() && base.GetSpawnerSource() != EnumSpawnerSource.StaticSpawner && !this.IsSleeping;
	}

	// Token: 0x0600200E RID: 8206 RVA: 0x000C874D File Offset: 0x000C694D
	public void ResetDespawnTime()
	{
		this.ticksNoPlayerAdjacent = 0;
		this.seeCache.SetLastTimePlayerSeen();
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x000C8764 File Offset: 0x000C6964
	public void CheckDespawn()
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.CanUpdateEntity() && this.bIsChunkObserver && this.world.GetClosestPlayer(this, -1f, false) == null)
		{
			this.MarkToUnload();
			return;
		}
		if (!this.canDespawn())
		{
			return;
		}
		int num = this.despawnDelayCounter + 1;
		this.despawnDelayCounter = num;
		if (num < 20)
		{
			return;
		}
		this.despawnDelayCounter = 0;
		this.ticksNoPlayerAdjacent += 20;
		EnumSpawnerSource spawnerSource = base.GetSpawnerSource();
		EntityPlayer closestPlayer = this.world.GetClosestPlayer(this, -1f, false);
		if (spawnerSource == EnumSpawnerSource.Dynamic)
		{
			if (!closestPlayer)
			{
				if (!this.world.GetClosestPlayer(this, -1f, true))
				{
					this.Despawn();
				}
				return;
			}
		}
		else if (spawnerSource == EnumSpawnerSource.Biome && !this.world.GetClosestPlayer(this, 130f, false))
		{
			if (this.world.GetClosestPlayer(this, 20f, true))
			{
				this.isDespawnWhenPlayerFar = true;
			}
			else if (this.isDespawnWhenPlayerFar)
			{
				this.Despawn();
			}
		}
		if (!closestPlayer)
		{
			return;
		}
		float sqrMagnitude = (closestPlayer.position - this.position).sqrMagnitude;
		if (sqrMagnitude < 6400f)
		{
			this.ticksNoPlayerAdjacent = 0;
		}
		int num2 = int.MaxValue;
		float lastTimePlayerSeen = this.seeCache.GetLastTimePlayerSeen();
		if (lastTimePlayerSeen > 0f)
		{
			num2 = (int)(Time.time - lastTimePlayerSeen);
		}
		switch (spawnerSource)
		{
		case EnumSpawnerSource.Biome:
			if (this.ticksNoPlayerAdjacent > 100 && sqrMagnitude > 16384f)
			{
				this.Despawn();
				return;
			}
			if (this.ticksNoPlayerAdjacent > 1800)
			{
				this.Despawn();
				return;
			}
			break;
		case EnumSpawnerSource.StaticSpawner:
			break;
		case EnumSpawnerSource.Dynamic:
			if (this.attackTarget)
			{
				num2 = 0;
			}
			if (this.IsSleeper && !this.IsSleeping)
			{
				if (sqrMagnitude > 9216f && num2 > 80)
				{
					this.Despawn();
					return;
				}
			}
			else
			{
				if (sqrMagnitude > 2304f && num2 > 60 && !this.HasInvestigatePosition)
				{
					this.Despawn();
					return;
				}
				if (this.ticksNoPlayerAdjacent > 1800)
				{
					this.Despawn();
					return;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000C8974 File Offset: 0x000C6B74
	[PublicizedFrom(EAccessModifier.Private)]
	public void Despawn()
	{
		this.IsDespawned = true;
		this.MarkToUnload();
	}

	// Token: 0x06002011 RID: 8209 RVA: 0x000C8983 File Offset: 0x000C6B83
	public void ForceDespawn()
	{
		this.Despawn();
	}

	// Token: 0x06002012 RID: 8210 RVA: 0x000C898B File Offset: 0x000C6B8B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityAlive GetAttackTarget()
	{
		return this.attackTarget;
	}

	// Token: 0x06002013 RID: 8211 RVA: 0x000C8993 File Offset: 0x000C6B93
	public virtual Vector3 GetAttackTargetHitPosition()
	{
		return this.attackTarget.getChestPosition();
	}

	// Token: 0x06002014 RID: 8212 RVA: 0x000C89A0 File Offset: 0x000C6BA0
	public EntityAlive GetAttackTargetLocal()
	{
		if (this.isEntityRemote)
		{
			return this.attackTargetClient;
		}
		return this.attackTarget;
	}

	// Token: 0x06002015 RID: 8213 RVA: 0x000C89B8 File Offset: 0x000C6BB8
	public void SetAttackTarget(EntityAlive _attackTarget, int _attackTargetTime)
	{
		if (_attackTarget == this.attackTarget)
		{
			this.attackTargetTime = _attackTargetTime;
			return;
		}
		if (this.attackTarget)
		{
			this.attackTargetLast = this.attackTarget;
		}
		this.targetAlertChanged = false;
		if (_attackTarget)
		{
			if (_attackTarget != this.attackTargetLast)
			{
				this.targetAlertChanged = true;
				this.soundDelayTicks = this.rand.RandomRange(5, 20);
			}
			this.investigatePositionTicks = 0;
		}
		if (!this.isEntityRemote)
		{
			this.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.entityId, -1, NetPackageManager.GetPackage<NetPackageSetAttackTarget>().Setup(this.entityId, _attackTarget ? _attackTarget.entityId : -1), false);
		}
		this.attackTarget = _attackTarget;
		this.attackTargetTime = _attackTargetTime;
	}

	// Token: 0x06002016 RID: 8214 RVA: 0x000C8A82 File Offset: 0x000C6C82
	public void SetAttackTargetClient(EntityAlive _attackTarget)
	{
		this.attackTargetClient = _attackTarget;
	}

	// Token: 0x1700039A RID: 922
	// (get) Token: 0x06002017 RID: 8215 RVA: 0x000C8A8B File Offset: 0x000C6C8B
	public bool HasInvestigatePosition
	{
		get
		{
			return this.investigatePositionTicks > 0;
		}
	}

	// Token: 0x1700039B RID: 923
	// (get) Token: 0x06002018 RID: 8216 RVA: 0x000C8A96 File Offset: 0x000C6C96
	public Vector3 InvestigatePosition
	{
		get
		{
			return this.investigatePos;
		}
	}

	// Token: 0x06002019 RID: 8217 RVA: 0x000C8A9E File Offset: 0x000C6C9E
	public int GetInvestigatePositionTicks()
	{
		return this.investigatePositionTicks;
	}

	// Token: 0x0600201A RID: 8218 RVA: 0x000C8AA8 File Offset: 0x000C6CA8
	public void ClearInvestigatePosition()
	{
		this.investigatePos = Vector3.zero;
		this.investigatePositionTicks = 0;
		this.ResetDespawnTime();
		int num = this.rand.RandomRange(20, 35) * 20;
		if (this.entityType == EntityType.Zombie)
		{
			num /= 2;
		}
		this.SetAlertTicks(num);
	}

	// Token: 0x0600201B RID: 8219 RVA: 0x000C8AF4 File Offset: 0x000C6CF4
	public int CalcInvestigateTicks(int _ticks, EntityAlive _investigateEntity)
	{
		float value = EffectManager.GetValue(PassiveEffects.EnemySearchDuration, null, 1f, _investigateEntity, null, EntityClass.list[this.entityClass].Tags, true, true, true, true, true, 1, true, false);
		return (int)((float)_ticks / value);
	}

	// Token: 0x0600201C RID: 8220 RVA: 0x000C8B36 File Offset: 0x000C6D36
	public void SetInvestigatePosition(Vector3 pos, int ticks, bool isAlert = true)
	{
		this.investigatePos = pos;
		this.investigatePositionTicks = ticks;
		this.isInvestigateAlert = isAlert;
	}

	// Token: 0x0600201D RID: 8221 RVA: 0x000C8B4D File Offset: 0x000C6D4D
	public int GetAlertTicks()
	{
		return this.alertTicks;
	}

	// Token: 0x0600201E RID: 8222 RVA: 0x000C8B55 File Offset: 0x000C6D55
	public void SetAlertTicks(int ticks)
	{
		this.alertTicks = ticks;
	}

	// Token: 0x1700039C RID: 924
	// (get) Token: 0x0600201F RID: 8223 RVA: 0x000C8B5E File Offset: 0x000C6D5E
	public virtual bool IsAlert
	{
		get
		{
			if (this.isEntityRemote)
			{
				return this.bReplicatedAlertFlag;
			}
			return this.isAlert;
		}
	}

	// Token: 0x06002020 RID: 8224 RVA: 0x000C8B75 File Offset: 0x000C6D75
	public EntitySeeCache GetEntitySenses()
	{
		return this.seeCache;
	}

	// Token: 0x1700039D RID: 925
	// (get) Token: 0x06002021 RID: 8225 RVA: 0x000C8B7D File Offset: 0x000C6D7D
	public virtual bool IsRunning
	{
		get
		{
			return this.IsBloodMoon || this.world.IsDark();
		}
	}

	// Token: 0x06002022 RID: 8226 RVA: 0x000C8B94 File Offset: 0x000C6D94
	public virtual float GetMoveSpeed()
	{
		if (this.IsBloodMoon || this.world.IsDark())
		{
			return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, this.moveSpeedNight, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		return EffectManager.GetValue(PassiveEffects.CrouchSpeed, null, this.moveSpeed, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002023 RID: 8227 RVA: 0x000C8C00 File Offset: 0x000C6E00
	public virtual float GetMoveSpeedAggro()
	{
		if (this.IsBloodMoon || this.world.IsDark())
		{
			return EffectManager.GetValue(PassiveEffects.RunSpeed, null, this.moveSpeedAggroMax, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		return EffectManager.GetValue(PassiveEffects.WalkSpeed, null, this.moveSpeedAggro, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002024 RID: 8228 RVA: 0x000C8C6C File Offset: 0x000C6E6C
	public float GetMoveSpeedPanic()
	{
		return EffectManager.GetValue(PassiveEffects.RunSpeed, null, this.moveSpeedPanic, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
	}

	// Token: 0x06002025 RID: 8229 RVA: 0x000C8C9D File Offset: 0x000C6E9D
	public override float GetWeight()
	{
		return this.weight;
	}

	// Token: 0x06002026 RID: 8230 RVA: 0x000C8CA5 File Offset: 0x000C6EA5
	public override float GetPushFactor()
	{
		return this.pushFactor;
	}

	// Token: 0x06002027 RID: 8231 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanEntityJump()
	{
		return true;
	}

	// Token: 0x06002028 RID: 8232 RVA: 0x000C8CAD File Offset: 0x000C6EAD
	public void SetMaxViewAngle(float _angle)
	{
		this.maxViewAngle = _angle;
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x000C8CB6 File Offset: 0x000C6EB6
	public virtual float GetMaxViewAngle()
	{
		return this.maxViewAngle;
	}

	// Token: 0x0600202A RID: 8234 RVA: 0x000C8CBE File Offset: 0x000C6EBE
	public void SetSightLightThreshold(Vector2 _threshold)
	{
		this.sightLightThreshold = _threshold;
	}

	// Token: 0x0600202B RID: 8235 RVA: 0x000C8CC7 File Offset: 0x000C6EC7
	public int GetModelLayer()
	{
		return this.emodel.GetModelTransform().gameObject.layer;
	}

	// Token: 0x0600202C RID: 8236 RVA: 0x000C8CDE File Offset: 0x000C6EDE
	public virtual void SetModelLayer(int _layerId, bool force = false, string[] excludeTags = null)
	{
		Utils.SetLayerRecursively(this.emodel.GetModelTransform().gameObject, _layerId);
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x000C8CF6 File Offset: 0x000C6EF6
	public virtual void SetColliderLayer(int _layerId, bool _force = false)
	{
		Utils.SetColliderLayerRecursively(this.emodel.GetModelTransform().gameObject, _layerId);
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x000768A9 File Offset: 0x00074AA9
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual int GetMaxAttackTime()
	{
		return 10;
	}

	// Token: 0x0600202F RID: 8239 RVA: 0x000C8D0E File Offset: 0x000C6F0E
	public int GetAttackTimeoutTicks()
	{
		if (!this.world.IsDark())
		{
			return this.attackTimeoutDay;
		}
		return this.attackTimeoutNight;
	}

	// Token: 0x06002030 RID: 8240 RVA: 0x000C8D2A File Offset: 0x000C6F2A
	public override string GetLootList()
	{
		if (!string.IsNullOrEmpty(this.lootListOnDeath) && this.IsDead())
		{
			return this.lootListOnDeath;
		}
		return base.GetLootList();
	}

	// Token: 0x06002031 RID: 8241 RVA: 0x000C8D4E File Offset: 0x000C6F4E
	public override void MarkToUnload()
	{
		base.MarkToUnload();
		this.deathUpdateTime = this.timeStayAfterDeath;
	}

	// Token: 0x06002032 RID: 8242 RVA: 0x000C8D62 File Offset: 0x000C6F62
	public override bool IsMarkedForUnload()
	{
		return base.IsMarkedForUnload() && this.deathUpdateTime >= this.timeStayAfterDeath;
	}

	// Token: 0x06002033 RID: 8243 RVA: 0x000C8D80 File Offset: 0x000C6F80
	public virtual bool IsAttackValid()
	{
		if (!(this is EntityPlayer))
		{
			if (this.Electrocuted)
			{
				return false;
			}
			if (this.bodyDamage.CurrentStun == EnumEntityStunType.Kneel || this.bodyDamage.CurrentStun == EnumEntityStunType.Prone)
			{
				return false;
			}
		}
		return (!(this.emodel != null) || !(this.emodel.avatarController != null) || !this.emodel.avatarController.IsAttackPrevented()) && !this.IsDead() && (this.painResistPercent >= 1f || (this.hasBeenAttackedTime <= 0 && (this.emodel.avatarController == null || !this.emodel.avatarController.IsAnimationHitRunning())));
	}

	// Token: 0x06002034 RID: 8244 RVA: 0x000C8E3E File Offset: 0x000C703E
	public virtual bool IsAttackImpact()
	{
		return this.emodel && this.emodel.avatarController && this.emodel.avatarController.IsAttackImpact();
	}

	// Token: 0x06002035 RID: 8245 RVA: 0x000C8E71 File Offset: 0x000C7071
	public virtual void ShowHoldingItem(bool _show)
	{
		this.inventory.ShowRightHand(_show);
	}

	// Token: 0x06002036 RID: 8246 RVA: 0x000C8E7F File Offset: 0x000C707F
	public virtual bool IsHoldingItemInUse(int _actionIndex)
	{
		ItemAction itemAction = this.inventory.holdingItem.Actions[_actionIndex];
		return itemAction != null && itemAction.IsActionRunning(this.inventory.holdingItemData.actionData[_actionIndex]);
	}

	// Token: 0x06002037 RID: 8247 RVA: 0x000C8EB4 File Offset: 0x000C70B4
	public virtual bool UseHoldingItem(int _actionIndex, bool _isReleased)
	{
		if (!_isReleased)
		{
			if (_actionIndex == 0 && this.emodel && this.emodel.avatarController && this.emodel.avatarController.IsAnimationAttackPlaying())
			{
				return false;
			}
			if (!this.IsAttackValid())
			{
				return false;
			}
		}
		if (_actionIndex == 0 && _isReleased && this.GetSoundAttack() != null)
		{
			this.PlayOneShot(this.GetSoundAttack(), false, false, false, null);
		}
		this.attackingTime = 60;
		ItemAction itemAction = this.inventory.holdingItem.Actions[_actionIndex];
		if (itemAction != null)
		{
			itemAction.ExecuteAction(this.inventory.holdingItemData.actionData[_actionIndex], _isReleased);
		}
		return true;
	}

	// Token: 0x06002038 RID: 8248 RVA: 0x000C8F5F File Offset: 0x000C715F
	public bool Attack(bool _isReleased)
	{
		return this.UseHoldingItem(0, _isReleased);
	}

	// Token: 0x06002039 RID: 8249 RVA: 0x000C8F6C File Offset: 0x000C716C
	public Entity GetTargetIfAttackedNow()
	{
		if (!this.IsAttackValid())
		{
			return null;
		}
		ItemClass holdingItem = this.inventory.holdingItem;
		if (holdingItem != null)
		{
			int holdingItemIdx = this.inventory.holdingItemIdx;
			ItemAction itemAction = holdingItem.Actions[holdingItemIdx];
			if (itemAction != null)
			{
				WorldRayHitInfo executeActionTarget = itemAction.GetExecuteActionTarget(this.inventory.holdingItemData.actionData[holdingItemIdx]);
				if (executeActionTarget != null && executeActionTarget.bHitValid && executeActionTarget.transform)
				{
					float num = itemAction.Range;
					if (num == 0f)
					{
						ItemValue holdingItemItemValue = this.inventory.holdingItemItemValue;
						num = EffectManager.GetItemValue(PassiveEffects.MaxRange, holdingItemItemValue, 0f);
					}
					num += 0.3f;
					if (executeActionTarget.hit.distanceSq <= num * num)
					{
						Transform transform = executeActionTarget.transform;
						if (executeActionTarget.tag.StartsWith("E_BP_"))
						{
							transform = GameUtils.GetHitRootTransform(Voxel.voxelRayHitInfo.tag, executeActionTarget.transform);
						}
						if (transform != null)
						{
							Entity component = transform.GetComponent<Entity>();
							if (component)
							{
								return component;
							}
						}
						if (executeActionTarget.tag == "E_Vehicle")
						{
							return EntityVehicle.FindCollisionEntity(transform);
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x0600203A RID: 8250 RVA: 0x000C90A4 File Offset: 0x000C72A4
	public virtual float GetBlockDamageScale()
	{
		EnumGamePrefs eProperty = EnumGamePrefs.BlockDamageAI;
		if (this.IsBloodMoon)
		{
			eProperty = EnumGamePrefs.BlockDamageAIBM;
		}
		return (float)GamePrefs.GetInt(eProperty) * 0.01f;
	}

	// Token: 0x0600203B RID: 8251 RVA: 0x000C90CC File Offset: 0x000C72CC
	public virtual void PlayStepSound(float _volume)
	{
		this.internalPlayStepSound(_volume);
	}

	// Token: 0x0600203C RID: 8252 RVA: 0x000C90D8 File Offset: 0x000C72D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public void internalPlayStepSound(float _volume)
	{
		if (this.blockValueStandingOn.isair)
		{
			return;
		}
		if ((!this.onGround && !base.IsInElevator()) || this.isHeadUnderwater)
		{
			if (!(this is EntityPlayerLocal) && (this.isHeadUnderwater || this.world.IsWater(this.blockPosStandingOn)))
			{
				Manager.Play(this, "player_swim", 1f, false);
			}
			return;
		}
		BlockValue blockValue = this.blockValueStandingOn;
		Vector3i vector3i = this.blockPosStandingOn;
		vector3i.y++;
		BlockValue blockValue2 = this.world.GetBlock(vector3i);
		if (blockValue2.Block.blockMaterial.stepSound != null)
		{
			blockValue = blockValue2;
		}
		else
		{
			BlockValue block;
			blockValue2 = (block = this.world.GetBlock(vector3i + Vector3i.right));
			if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
			{
				blockValue = blockValue2;
			}
			else
			{
				blockValue2 = (block = this.world.GetBlock(vector3i - Vector3i.right));
				if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
				{
					blockValue = blockValue2;
				}
				else
				{
					blockValue2 = (block = this.world.GetBlock(vector3i + Vector3i.forward));
					if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
					{
						blockValue = blockValue2;
					}
					else
					{
						blockValue2 = (block = this.world.GetBlock(vector3i - Vector3i.forward));
						if (!block.isair && blockValue2.Block.blockMaterial.stepSound != null)
						{
							blockValue = blockValue2;
						}
					}
				}
			}
		}
		if (blockValue.isair)
		{
			return;
		}
		Block block2 = blockValue.Block;
		if (EffectManager.GetValue(PassiveEffects.SilenceBlockSteps, null, 0f, this, null, block2.Tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			return;
		}
		MaterialBlock materialForSide = block2.GetMaterialForSide(blockValue, BlockFace.Top);
		if (materialForSide != null && materialForSide.stepSound != null)
		{
			string name = materialForSide.stepSound.name;
			if (name.Length > 0)
			{
				string stepSound = this.soundStepType + name;
				this.PlayStepSound(stepSound, _volume);
			}
		}
	}

	// Token: 0x0600203D RID: 8253 RVA: 0x000C92EC File Offset: 0x000C74EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateStepSound(float _distX, float _distZ, float _rotYDelta)
	{
		if (this.blockValueStandingOn.isair)
		{
			return;
		}
		float num = Mathf.Sqrt(_distX * _distX + _distZ * _distZ);
		if (!this.onGround || this.isHeadUnderwater)
		{
			this.distanceSwam += num;
			if (this.distanceSwam > this.nextSwimDistance)
			{
				this.nextSwimDistance += 1f;
				if (this.nextSwimDistance < this.distanceSwam || this.nextSwimDistance > this.distanceSwam + 1f)
				{
					this.nextSwimDistance = this.distanceSwam + 1f;
				}
				this.internalPlayStepSound(1f);
			}
			return;
		}
		this.distanceWalked += num;
		if (num == 0f)
		{
			this.stepSoundDistanceRemaining = 0.25f;
		}
		else
		{
			this.stepSoundDistanceRemaining -= num;
			if (this.stepSoundDistanceRemaining <= 0f)
			{
				this.stepSoundDistanceRemaining = this.getNextStepSoundDistance();
				this.internalPlayStepSound(1f);
			}
		}
		this.stepSoundRotYRemaining -= Utils.FastAbs(_rotYDelta);
		if (this.stepSoundRotYRemaining <= 0f)
		{
			this.stepSoundRotYRemaining = 90f;
			this.internalPlayStepSound(1f);
		}
	}

	// Token: 0x0600203E RID: 8254 RVA: 0x000C9420 File Offset: 0x000C7620
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updatePlayerLandSound(float _distXZ, float _diffY)
	{
		if (this.blockValueStandingOn.isair)
		{
			return;
		}
		if (_distXZ >= 0.025f || Utils.FastAbs(_diffY) >= 0.015f)
		{
			float num = this.inWaterPercent * 2f;
			float x = num - this.landWaterLevel;
			this.landWaterLevel = num;
			float num2 = Utils.FastAbs(x);
			if (num > 0f)
			{
				num2 = Utils.FastMax(num2, _distXZ);
			}
			if (num2 >= 0.02f)
			{
				float volumeScale = Utils.FastMin(num2 * 2.2f + 0.01f, 1f);
				Manager.Play(this, "player_swim", volumeScale, false);
			}
		}
		if (this.isHeadUnderwater)
		{
			this.wasOnGround = true;
			return;
		}
		this.wasOnGround = this.onGround;
	}

	// Token: 0x0600203F RID: 8255 RVA: 0x000C94CC File Offset: 0x000C76CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updateCurrentBlockPosAndValue()
	{
		Vector3i vector3i = base.GetBlockPosition();
		BlockValue block = this.world.GetBlock(vector3i);
		if (block.isair)
		{
			vector3i.y--;
			block = this.world.GetBlock(vector3i);
		}
		if (block.ischild)
		{
			vector3i += block.parent;
			block = this.world.GetBlock(vector3i);
		}
		if (this.blockPosStandingOn != vector3i || !this.blockValueStandingOn.Equals(block) || (this.onGround && !this.wasOnGround))
		{
			this.blockPosStandingOn = vector3i;
			this.blockValueStandingOn = block;
			this.blockStandingOnChanged = !this.world.IsRemote();
			BiomeDefinition biome = this.world.GetBiome(this.blockPosStandingOn.x, this.blockPosStandingOn.z);
			if (biome != null && this.biomeStandingOn != biome && (this.biomeStandingOn == null || this.biomeStandingOn.m_Id != biome.m_Id))
			{
				this.onNewBiomeEntered(biome);
			}
		}
		this.CalcIfInElevator();
		Block block2 = this.blockValueStandingOn.Block;
		if (block2.BuffsWhenWalkedOn != null && block2.UseBuffsWhenWalkedOn(this.world, this.blockPosStandingOn, this.blockValueStandingOn))
		{
			bool flag = true;
			TileEntityWorkstation tileEntityWorkstation = this.world.GetTileEntity(0, this.blockPosStandingOn) as TileEntityWorkstation;
			if (tileEntityWorkstation != null)
			{
				flag = tileEntityWorkstation.IsBurning;
			}
			if (flag)
			{
				for (int i = 0; i < block2.BuffsWhenWalkedOn.Length; i++)
				{
					BuffValue buff = this.Buffs.GetBuff(block2.BuffsWhenWalkedOn[i]);
					if (buff == null || buff.DurationInSeconds >= 1f)
					{
						this.Buffs.AddBuff(block2.BuffsWhenWalkedOn[i], vector3i, -1, true, false, -1f);
					}
				}
			}
		}
		if (this.onGround && !this.IsFlyMode.Value)
		{
			if (block2.MovementFactor != 1f && block2.HasCollidingAABB(this.blockValueStandingOn, this.blockPosStandingOn.x, this.blockPosStandingOn.y, this.blockPosStandingOn.z, 0f, this.boundingBox))
			{
				this.SetMotionMultiplier(EffectManager.GetValue(PassiveEffects.MovementFactorMultiplier, null, block2.MovementFactor, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
			}
			if (this.blockStandingOnChanged)
			{
				this.blockStandingOnChanged = false;
				if (!this.blockValueStandingOn.isair)
				{
					block2.OnEntityWalking(this.world, this.blockPosStandingOn.x, this.blockPosStandingOn.y, this.blockPosStandingOn.z, this.blockValueStandingOn, this);
					if (GameManager.bPhysicsActive && !this.blockValueStandingOn.ischild && !this.blockValueStandingOn.Block.isOversized && this.world.GetStability(this.blockPosStandingOn) == 0 && Block.CanFallBelow(this.world, this.blockPosStandingOn.x, this.blockPosStandingOn.y, this.blockPosStandingOn.z))
					{
						Log.Warning("EntityAlive {0} AddFallingBlock stab 0 happens?", new object[]
						{
							this.EntityName
						});
						this.world.AddFallingBlock(this.blockPosStandingOn, false);
					}
				}
				BlockValue block3 = this.world.GetBlock(this.blockPosStandingOn + Vector3i.up);
				if (!block3.isair)
				{
					block3.Block.OnEntityWalking(this.world, this.blockPosStandingOn.x, this.blockPosStandingOn.y + 1, this.blockPosStandingOn.z, block3, this);
				}
			}
		}
		this.HandleLootStageMaxCheck();
	}

	// Token: 0x06002040 RID: 8256 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleLootStageMaxCheck()
	{
	}

	// Token: 0x06002041 RID: 8257 RVA: 0x000C9874 File Offset: 0x000C7A74
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcIfInElevator()
	{
		ChunkCluster chunkCache = this.world.ChunkCache;
		Vector3i pos = new Vector3i(this.blockPosStandingOn.x, Utils.Fastfloor(this.boundingBox.min.y), this.blockPosStandingOn.z);
		BlockValue block = chunkCache.GetBlock(pos);
		Block block2 = block.Block;
		this.bInElevator = block2.IsElevator((int)block.rotation);
		pos.y++;
		block = chunkCache.GetBlock(pos);
		block2 = block.Block;
		this.bInElevator |= block2.IsElevator((int)block.rotation);
	}

	// Token: 0x06002042 RID: 8258 RVA: 0x000C9916 File Offset: 0x000C7B16
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual float getNextStepSoundDistance()
	{
		return 1.5f;
	}

	// Token: 0x06002043 RID: 8259 RVA: 0x000C991D File Offset: 0x000C7B1D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void onNewBiomeEntered(BiomeDefinition _biome)
	{
		this.biomeStandingOn = _biome;
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x000C9928 File Offset: 0x000C7B28
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateSpeedForwardAndStrafe(Vector3 _dist, float _partialTicks)
	{
		if (this.isEntityRemote && _partialTicks > 1f)
		{
			_dist /= _partialTicks;
		}
		this.speedForward *= 0.5f;
		this.speedStrafe *= 0.5f;
		this.speedVertical *= 0.5f;
		if (Mathf.Abs(_dist.x) > 0.001f || Mathf.Abs(_dist.z) > 0.001f)
		{
			float num = Mathf.Sin(-this.rotation.y * 3.1415927f / 180f);
			float num2 = Mathf.Cos(-this.rotation.y * 3.1415927f / 180f);
			this.speedForward += num2 * _dist.z - num * _dist.x;
			this.speedStrafe += num2 * _dist.x + num * _dist.z;
		}
		if (Mathf.Abs(_dist.y) > 0.001f)
		{
			this.speedVertical += _dist.y;
		}
		this.SetMovementState();
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x000C9A4B File Offset: 0x000C7C4B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void PlayStepSound(string stepSound, float _volume)
	{
		if (this is EntityPlayerLocal)
		{
			Manager.BroadcastPlay(this, stepSound, false);
			return;
		}
		Manager.Play(this, stepSound, 1f, false);
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x000C9A6C File Offset: 0x000C7C6C
	public void SetLookPosition(Vector3 _lookPos)
	{
		if ((this.lookAtPosition - _lookPos).sqrMagnitude < 0.0016f)
		{
			return;
		}
		this.lookAtPosition = _lookPos;
		if (this.world.entityDistributer != null)
		{
			this.world.entityDistributer.SendPacketToTrackedPlayers(this.entityId, this.world.GetPrimaryPlayerId(), NetPackageManager.GetPackage<NetPackageEntityLookAt>().Setup(this.entityId, _lookPos), false);
		}
		if (this.emodel.avatarController)
		{
			this.emodel.avatarController.SetLookPosition(_lookPos);
		}
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isRadiationSensitive()
	{
		return true;
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsAimingGunPossible()
	{
		return true;
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x000C9AFF File Offset: 0x000C7CFF
	public int GetDeathTime()
	{
		return this.deathUpdateTime;
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x000C9B07 File Offset: 0x000C7D07
	public void SetDeathTime(int _deathTime)
	{
		this.deathUpdateTime = _deathTime;
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x000C9B10 File Offset: 0x000C7D10
	public int GetTimeStayAfterDeath()
	{
		return this.timeStayAfterDeath;
	}

	// Token: 0x0600204C RID: 8268 RVA: 0x000C9B18 File Offset: 0x000C7D18
	public bool IsCorpse()
	{
		return this.emodel && this.emodel.IsRagdollDead && (float)this.deathUpdateTime > 70f;
	}

	// Token: 0x0600204D RID: 8269 RVA: 0x000C9B48 File Offset: 0x000C7D48
	public override void OnAddedToWorld()
	{
		if (!(this is EntityPlayerLocal))
		{
			OcclusionManager.AddEntity(this, 7f);
		}
		this.m_addedToWorld = true;
		if (!this.isEntityRemote)
		{
			this.bSpawned = true;
		}
		if (this as EntityPlayer == null)
		{
			this.FireEvent(MinEventTypes.onSelfFirstSpawn, true);
		}
		this.StartStopLivingSound();
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000C9B9C File Offset: 0x000C7D9C
	public override void OnEntityUnload()
	{
		if (!(this is EntityPlayerLocal))
		{
			OcclusionManager.RemoveEntity(this);
		}
		if (this.navigator != null)
		{
			this.navigator.SetPath(null, 0f);
			this.navigator = null;
		}
		base.OnEntityUnload();
		this.lookHelper = null;
		this.moveHelper = null;
		this.seeCache = null;
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x000C9BF3 File Offset: 0x000C7DF3
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetDamageFraction(float _damage)
	{
		return _damage / (float)this.GetMaxHealth();
	}

	// Token: 0x06002050 RID: 8272 RVA: 0x000C9C00 File Offset: 0x000C7E00
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetDismemberChance(ref DamageResponse _dmResponse, float damagePer)
	{
		EnumBodyPartHit hitBodyPart = _dmResponse.HitBodyPart;
		EntityClass entityClass = EntityClass.list[this.entityClass];
		float num = 0f;
		switch (hitBodyPart.ToPrimary())
		{
		case BodyPrimaryHit.Head:
			num = entityClass.DismemberMultiplierHead;
			break;
		case BodyPrimaryHit.LeftUpperArm:
		case BodyPrimaryHit.RightUpperArm:
		case BodyPrimaryHit.LeftLowerArm:
		case BodyPrimaryHit.RightLowerArm:
			num = entityClass.DismemberMultiplierArms;
			break;
		case BodyPrimaryHit.LeftUpperLeg:
		case BodyPrimaryHit.RightUpperLeg:
		case BodyPrimaryHit.LeftLowerLeg:
		case BodyPrimaryHit.RightLowerLeg:
			num = entityClass.DismemberMultiplierLegs;
			break;
		}
		num = EffectManager.GetValue(PassiveEffects.DismemberSelfChance, null, num, this, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		float dismemberChance = _dmResponse.Source.DismemberChance;
		float num2 = (dismemberChance < 100f) ? (dismemberChance * damagePer * num) : 100f;
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_dmResponse.Source.getEntityId()) as EntityPlayerLocal;
		if (entityPlayerLocal && entityPlayerLocal.DebugDismembermentChance)
		{
			num2 = 1f;
		}
		if (DismembermentManager.DebugLogEnabled && num2 > 0f)
		{
			Log.Out("[EntityAlive.GetDismemberChance] - {0}, primary {1}, damage {2}, chance {3} * damage% {4} * multiplier {5} = {6}", new object[]
			{
				hitBodyPart,
				hitBodyPart.ToPrimary(),
				_dmResponse.Strength,
				dismemberChance.ToCultureInvariantString(),
				damagePer.ToCultureInvariantString(),
				num.ToCultureInvariantString(),
				num2.ToCultureInvariantString()
			});
		}
		return num2;
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x000C9D64 File Offset: 0x000C7F64
	public virtual void CheckDismember(ref DamageResponse _dmResponse, float damagePer)
	{
		bool flag = _dmResponse.HitBodyPart.IsLeg();
		if (flag && base.IsAlive() && (this.bodyDamage.CurrentStun != EnumEntityStunType.None || this.sleepingOrWakingUp))
		{
			return;
		}
		float dismemberChance = this.GetDismemberChance(ref _dmResponse, damagePer);
		if (dismemberChance > 0f && this.rand.RandomFloat <= dismemberChance)
		{
			_dmResponse.Dismember = true;
			if (flag)
			{
				_dmResponse.TurnIntoCrawler = true;
			}
			return;
		}
		if (flag)
		{
			EntityClass entityClass = EntityClass.list[this.entityClass];
			if (entityClass.LegCrawlerThreshold > 0f && this.GetDamageFraction((float)_dmResponse.Strength) >= entityClass.LegCrawlerThreshold)
			{
				_dmResponse.TurnIntoCrawler = true;
			}
			if (!this.bodyDamage.ShouldBeCrawler && !_dmResponse.TurnIntoCrawler && entityClass.LegCrippleScale > 0f)
			{
				float num = this.GetDamageFraction((float)_dmResponse.Strength) * entityClass.LegCrippleScale;
				if (num >= 0.05f)
				{
					if ((this.bodyDamage.Flags & 4096U) == 0U && _dmResponse.HitBodyPart.IsLeftLeg() && this.rand.RandomFloat < num)
					{
						_dmResponse.CrippleLegs = true;
					}
					if ((this.bodyDamage.Flags & 8192U) == 0U && _dmResponse.HitBodyPart.IsRightLeg() && this.rand.RandomFloat < num)
					{
						_dmResponse.CrippleLegs = true;
					}
				}
			}
		}
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x000C9EC4 File Offset: 0x000C80C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyLocalBodyDamage(DamageResponse _dmResponse)
	{
		EnumBodyPartHit enumBodyPartHit = _dmResponse.HitBodyPart;
		this.bodyDamage.bodyPartHit = enumBodyPartHit;
		this.bodyDamage.damageType = _dmResponse.Source.damageType;
		if (_dmResponse.Dismember)
		{
			if (DismembermentManager.DebugBodyPartHit != EnumBodyPartHit.None)
			{
				enumBodyPartHit = DismembermentManager.DebugBodyPartHit;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.Head) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 1U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftUpperArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 2U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftLowerArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 4U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightUpperArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 8U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightLowerArm) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 16U);
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftUpperLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 32U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.LeftLowerLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 64U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightUpperLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 128U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
			if ((enumBodyPartHit & EnumBodyPartHit.RightLowerLeg) > EnumBodyPartHit.None)
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 256U);
				this.bodyDamage.ShouldBeCrawler = true;
			}
		}
		if (_dmResponse.TurnIntoCrawler)
		{
			this.bodyDamage.ShouldBeCrawler = true;
		}
		if (_dmResponse.CrippleLegs)
		{
			if (_dmResponse.HitBodyPart.IsLeftLeg())
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 4096U);
			}
			if (_dmResponse.HitBodyPart.IsRightLeg())
			{
				this.bodyDamage.Flags = (this.bodyDamage.Flags | 8192U);
			}
		}
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x000CA080 File Offset: 0x000C8280
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ExecuteDismember(bool restoreState)
	{
		if (this.emodel == null || this.emodel.avatarController == null)
		{
			return;
		}
		this.emodel.avatarController.DismemberLimb(this.bodyDamage, restoreState);
		if (this.bodyDamage.ShouldBeCrawler)
		{
			this.SetupCrawlerState(restoreState);
		}
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000CA0DC File Offset: 0x000C82DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupCrawlerState(bool restoreState)
	{
		if (!this.IsDead())
		{
			this.emodel.avatarController.TurnIntoCrawler(restoreState);
			base.SetMaxHeight(0.5f);
			ItemValue itemValue = null;
			if (EntityClass.list[this.entityClass].Properties.Values.ContainsKey(EntityClass.PropHandItemCrawler))
			{
				itemValue = ItemClass.GetItem(EntityClass.list[this.entityClass].Properties.Values[EntityClass.PropHandItemCrawler], false);
				if (itemValue.IsEmpty())
				{
					itemValue = null;
				}
			}
			if (itemValue == null)
			{
				itemValue = ItemClass.GetItem("meleeHandZombie02", false);
			}
			this.inventory.SetBareHandItem(itemValue);
			this.TurnIntoCrawler();
		}
		this.walkType = 21;
	}

	// Token: 0x06002055 RID: 8277 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void TurnIntoCrawler()
	{
	}

	// Token: 0x06002056 RID: 8278 RVA: 0x000CA196 File Offset: 0x000C8396
	public void ClearStun()
	{
		this.bodyDamage.CurrentStun = EnumEntityStunType.None;
		this.bodyDamage.StunDuration = 0f;
		this.SetCVar("_stunned", 0f);
	}

	// Token: 0x06002057 RID: 8279 RVA: 0x000CA1C4 File Offset: 0x000C83C4
	public void SetStun(EnumEntityStunType stun)
	{
		this.bodyDamage.CurrentStun = stun;
		this.SetCVar("_stunned", 1f);
	}

	// Token: 0x06002058 RID: 8280 RVA: 0x000CA1E2 File Offset: 0x000C83E2
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void onSpawnStateChanged()
	{
		if (!this.m_addedToWorld)
		{
			return;
		}
		this.StartStopLivingSound();
	}

	// Token: 0x06002059 RID: 8281 RVA: 0x000CA1F4 File Offset: 0x000C83F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartStopLivingSound()
	{
		if (this.soundLiving != null)
		{
			if (this.Spawned)
			{
				if (!this.IsDead() && this.Health > 0)
				{
					Manager.Play(this, this.soundLiving, 1f, false);
					this.soundLivingID = 0;
				}
			}
			else if (this.soundLivingID >= 0)
			{
				Manager.Stop(this.entityId, this.soundLiving);
				this.soundLivingID = -1;
			}
		}
		if (this.Spawned && this.soundSpawn != null && !this.SleeperSupressLivingSounds)
		{
			this.PlayOneShot(this.soundSpawn, false, false, false, null);
		}
	}

	// Token: 0x0600205A RID: 8282 RVA: 0x000CA288 File Offset: 0x000C8488
	public void CrouchHeightFixedUpdate()
	{
		if (this.crouchType == 0)
		{
			return;
		}
		if (this.physicsBaseHeight <= 1.3f)
		{
			return;
		}
		float num = this.physicsBaseHeight;
		if (base.IsInElevator())
		{
			num *= 1.06f;
		}
		if (this.emodel.IsRagdollMovement || this.bodyDamage.CurrentStun == EnumEntityStunType.Prone)
		{
			num = this.physicsBaseHeight * 0.08f;
		}
		float num2 = this.m_characterController.GetRadius() * 0.9f;
		float num3 = num2 + 0.3f;
		float maxDistance = num + 0.01f - num3 - num2;
		Vector3 vector = this.PhysicsTransform.position;
		vector.y += num3;
		if (this.moveHelper != null && (this.moveHelper.BlockedFlags & 3) == 2)
		{
			vector += this.ModelTransform.forward * 0.15f;
		}
		RaycastHit raycastHit;
		if (Physics.SphereCast(vector, num2, Vector3.up, out raycastHit, maxDistance, 1083277312))
		{
			Transform transform = raycastHit.transform;
			if (transform && transform.CompareTag("Physics"))
			{
				Entity component = transform.GetComponent<Entity>();
				if (component)
				{
					component.PhysicsPush(transform.forward * (0.1f * Time.fixedDeltaTime), raycastHit.point, true);
				}
				return;
			}
			if (this.world.GetBlock(new Vector3i(raycastHit.point + Origin.position)).Block.Damage <= 0f)
			{
				num = raycastHit.point.y - (vector.y - num3) - 0.21f;
			}
		}
		if (num < this.physicsHeight)
		{
			if (base.IsInElevator())
			{
				return;
			}
			num = Mathf.MoveTowards(this.physicsHeight, num, 0.099999994f);
		}
		else
		{
			num = Mathf.MoveTowards(this.physicsHeight, num, 0.016666666f);
		}
		base.SetHeight(num);
		float num4 = this.physicsBaseHeight * 0.7f;
		if (num <= num4)
		{
			this.crouchBendPerTarget = 0f;
			int num5 = 8;
			if (this.walkType != num5 && this.walkType != 21)
			{
				this.walkTypeBeforeCrouch = this.walkType;
				this.SetWalkType(num5);
			}
		}
		else
		{
			this.crouchBendPerTarget = 1f - (num - num4) / (this.physicsBaseHeight - num4);
			if (this.walkTypeBeforeCrouch != 0)
			{
				this.SetWalkType(this.walkTypeBeforeCrouch);
				this.walkTypeBeforeCrouch = 0;
			}
		}
		this.crouchBendPer = Mathf.MoveTowards(this.crouchBendPer, this.crouchBendPerTarget, 0.099999994f);
	}

	// Token: 0x0600205B RID: 8283 RVA: 0x000CA501 File Offset: 0x000C8701
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetWalkType(int _walkType)
	{
		this.walkType = _walkType;
		this.emodel.avatarController.SetWalkType(_walkType, true);
	}

	// Token: 0x0600205C RID: 8284 RVA: 0x000CA51C File Offset: 0x000C871C
	public int GetWalkType()
	{
		return this.walkType;
	}

	// Token: 0x0600205D RID: 8285 RVA: 0x000CA524 File Offset: 0x000C8724
	public bool IsWalkTypeACrawl()
	{
		return this.walkType >= 20;
	}

	// Token: 0x0600205E RID: 8286 RVA: 0x000CA533 File Offset: 0x000C8733
	public string GetRightHandTransformName()
	{
		return this.rightHandTransformName;
	}

	// Token: 0x0600205F RID: 8287 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool isGameMessageOnDeath()
	{
		return true;
	}

	// Token: 0x06002060 RID: 8288 RVA: 0x000CA53C File Offset: 0x000C873C
	public override float GetLightBrightness()
	{
		Vector3i blockPosition = base.GetBlockPosition();
		Vector3i blockPos = blockPosition;
		blockPos.y += Mathf.RoundToInt(base.height + 0.5f);
		return Utils.FastMax(this.world.GetLightBrightness(blockPosition), this.world.GetLightBrightness(blockPos));
	}

	// Token: 0x06002061 RID: 8289 RVA: 0x000CA58C File Offset: 0x000C878C
	public virtual float GetLightLevel()
	{
		EntityAlive entityAlive = this.AttachedToEntity as EntityAlive;
		if (entityAlive)
		{
			return entityAlive.GetLightLevel();
		}
		return this.inventory.GetLightLevel();
	}

	// Token: 0x06002062 RID: 8290 RVA: 0x000CA5C0 File Offset: 0x000C87C0
	public override int AttachToEntity(Entity _other, int slot = -1)
	{
		slot = base.AttachToEntity(_other, slot);
		if (slot >= 0)
		{
			this.CurrentMovementTag = EntityAlive.MovementTagIdle;
			this.Crouching = false;
			if (!this.isEntityRemote)
			{
				this.saveInventory = null;
				if (_other is EntityAlive && _other.GetAttachedToInfo(slot).bReplaceLocalInventory)
				{
					this.saveInventory = this.inventory;
					this.saveHoldingItemIdxBeforeAttach = this.inventory.holdingItemIdx;
					this.inventory.SetHoldingItemIdxNoHolsterTime(this.inventory.DUMMY_SLOT_IDX);
					this.inventory = ((EntityAlive)_other).inventory;
				}
				this.bPlayerStatsChanged |= true;
			}
			else
			{
				this.ShowHoldingItem(false);
			}
		}
		return slot;
	}

	// Token: 0x06002063 RID: 8291 RVA: 0x000CA674 File Offset: 0x000C8874
	public override void Detach()
	{
		if (this.saveInventory != null)
		{
			this.inventory = this.saveInventory;
			this.inventory.SetHoldingItemIdxNoHolsterTime(this.saveHoldingItemIdxBeforeAttach);
			this.saveInventory = null;
		}
		base.Detach();
		this.bPlayerStatsChanged |= !this.isEntityRemote;
	}

	// Token: 0x06002064 RID: 8292 RVA: 0x000CA6C9 File Offset: 0x000C88C9
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		_bw.Write(this.deathHealth);
	}

	// Token: 0x06002065 RID: 8293 RVA: 0x000CA6DF File Offset: 0x000C88DF
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		if (_version > 24)
		{
			this.deathHealth = _br.ReadInt32();
		}
	}

	// Token: 0x06002066 RID: 8294 RVA: 0x000CA6FA File Offset: 0x000C88FA
	public override string ToString()
	{
		return string.Format("[type={0}, name={1}, id={2}]", base.GetType().Name, GameUtils.SafeStringFormat(this.EntityName), this.entityId);
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x000CA728 File Offset: 0x000C8928
	public virtual void FireEvent(MinEventTypes _eventType, bool useInventory = true)
	{
		MinEffectController effects = EntityClass.list[this.entityClass].Effects;
		if (effects != null)
		{
			effects.FireEvent(_eventType, this.MinEventContext);
		}
		if (this.Progression != null)
		{
			this.Progression.FireEvent(_eventType, this.MinEventContext);
		}
		if (this.challengeJournal != null)
		{
			this.challengeJournal.FireEvent(_eventType, this.MinEventContext);
		}
		if (this.inventory != null && useInventory)
		{
			this.inventory.FireEvent(_eventType, this.MinEventContext);
		}
		this.equipment.FireEvent(_eventType, this.MinEventContext);
		this.Buffs.FireEvent(_eventType, this.MinEventContext);
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x000CA7D2 File Offset: 0x000C89D2
	public float GetCVar(string _varName)
	{
		if (this.Buffs == null)
		{
			return 0f;
		}
		return this.Buffs.GetCustomVar(_varName);
	}

	// Token: 0x06002069 RID: 8297 RVA: 0x000CA7EE File Offset: 0x000C89EE
	public void SetCVar(string _varName, float _value)
	{
		if (this.Buffs == null)
		{
			return;
		}
		this.Buffs.SetCustomVar(_varName, _value, true, CVarOperation.set);
	}

	// Token: 0x0600206A RID: 8298 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void BuffAdded(BuffValue _buff)
	{
	}

	// Token: 0x0600206B RID: 8299 RVA: 0x000CA808 File Offset: 0x000C8A08
	public override void OnCollisionForward(Transform t, Collision collision, bool isStay)
	{
		if (!this.emodel.IsRagdollActive)
		{
			return;
		}
		if (collision.relativeVelocity.sqrMagnitude < 0.0625f)
		{
			return;
		}
		float sqrMagnitude = collision.impulse.sqrMagnitude;
		if (sqrMagnitude < 400f)
		{
			return;
		}
		if (this.IsDead())
		{
			EntityAlive.ImpactData impactData;
			this.impacts.TryGetValue(t, out impactData);
			impactData.count++;
			this.impacts[t] = impactData;
			if (impactData.count >= 10)
			{
				if (impactData.count == 10)
				{
					Rigidbody component = t.GetComponent<Rigidbody>();
					if (component)
					{
						component.velocity = Vector3.zero;
						component.angularVelocity = Vector3.zero;
						component.drag = 0.5f;
						component.angularDrag = 0.5f;
					}
					CharacterJoint component2 = t.GetComponent<CharacterJoint>();
					if (component2)
					{
						component2.enableProjection = false;
					}
				}
				if (impactData.count == 25 && !t.gameObject.CompareTag("E_BP_Body"))
				{
					t.GetComponent<Collider>().enabled = false;
				}
				return;
			}
		}
		if (Time.time - this.impactSoundTime < 0.25f)
		{
			return;
		}
		this.impactSoundTime = Time.time;
		if (t.lossyScale.x == 0f)
		{
			return;
		}
		string soundGroupName = "impactbodylight";
		if (sqrMagnitude >= 3600f)
		{
			soundGroupName = "impactbodyheavy";
		}
		Vector3 a = Vector3.zero;
		int contactCount = collision.contactCount;
		for (int i = 0; i < contactCount; i++)
		{
			a += collision.GetContact(i).point;
		}
		a *= 1f / (float)contactCount;
		Manager.BroadcastPlay(a + Origin.position, soundGroupName, 0f);
	}

	// Token: 0x0600206C RID: 8300 RVA: 0x000CA9C4 File Offset: 0x000C8BC4
	public void AddParticle(string _name, Transform _t)
	{
		if (this.particles.ContainsKey(_name))
		{
			this.particles[_name] = _t;
			return;
		}
		this.particles.Add(_name, _t);
	}

	// Token: 0x0600206D RID: 8301 RVA: 0x000CA9F0 File Offset: 0x000C8BF0
	public bool RemoveParticle(string _name)
	{
		Transform transform;
		if (this.particles.Remove(_name, out transform))
		{
			if (transform)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600206E RID: 8302 RVA: 0x000CAA24 File Offset: 0x000C8C24
	public bool HasParticle(string _name)
	{
		Transform transform;
		return this.particles.TryGetValue(_name, out transform);
	}

	// Token: 0x0600206F RID: 8303 RVA: 0x000CAA44 File Offset: 0x000C8C44
	public void AddPart(string _name, Transform _t)
	{
		if (this.parts.ContainsKey(_name))
		{
			this.parts[_name] = _t;
			return;
		}
		this.parts.Add(_name, _t);
	}

	// Token: 0x06002070 RID: 8304 RVA: 0x000CAA70 File Offset: 0x000C8C70
	public void RemovePart(string _name)
	{
		Transform transform;
		if (this.parts.TryGetValue(_name, out transform))
		{
			this.parts.Remove(_name);
			if (transform)
			{
				transform.gameObject.name = ".";
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
	}

	// Token: 0x06002071 RID: 8305 RVA: 0x000CAAC0 File Offset: 0x000C8CC0
	public void SetPartActive(string _name, bool isActive)
	{
		Transform transform;
		if (this.parts.TryGetValue(_name, out transform) && transform)
		{
			bool flag = true;
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				Transform child = transform.GetChild(i);
				if (child.CompareTag("ModOn"))
				{
					child.gameObject.SetActive(isActive);
					flag = false;
				}
				else if (child.CompareTag("ModMesh"))
				{
					if (transform.parent.name == "CameraNode")
					{
						child.gameObject.SetActive(false);
					}
					flag = false;
				}
			}
			if (flag)
			{
				transform.gameObject.SetActive(isActive);
			}
		}
	}

	// Token: 0x06002072 RID: 8306 RVA: 0x000CAB63 File Offset: 0x000C8D63
	public void AddOwnedEntity(OwnedEntityData _entityData)
	{
		if (_entityData != null)
		{
			this.ownedEntities.Add(_entityData);
		}
	}

	// Token: 0x06002073 RID: 8307 RVA: 0x000CAB74 File Offset: 0x000C8D74
	public void AddOwnedEntity(Entity _entity)
	{
		if (this.ownedEntities.Find((OwnedEntityData e) => e.Id == _entity.entityId) == null)
		{
			this.AddOwnedEntity(new OwnedEntityData(_entity));
		}
	}

	// Token: 0x06002074 RID: 8308 RVA: 0x000CABB8 File Offset: 0x000C8DB8
	public void RemoveOwnedEntity(OwnedEntityData _entityData)
	{
		if (_entityData != null)
		{
			this.ownedEntities.Remove(_entityData);
		}
	}

	// Token: 0x06002075 RID: 8309 RVA: 0x000CABCC File Offset: 0x000C8DCC
	public void RemoveOwnedEntity(int _entityId)
	{
		this.RemoveOwnedEntity(this.ownedEntities.Find((OwnedEntityData e) => e.Id == _entityId));
	}

	// Token: 0x06002076 RID: 8310 RVA: 0x000CAC03 File Offset: 0x000C8E03
	public void RemoveOwnedEntity(Entity _entity)
	{
		this.RemoveOwnedEntity(_entity.entityId);
	}

	// Token: 0x06002077 RID: 8311 RVA: 0x000CAC14 File Offset: 0x000C8E14
	public OwnedEntityData GetOwnedEntity(int _entityId)
	{
		return this.ownedEntities.Find((OwnedEntityData e) => e.Id == _entityId);
	}

	// Token: 0x06002078 RID: 8312 RVA: 0x000CAC48 File Offset: 0x000C8E48
	public OwnedEntityData[] GetOwnedEntityClass(string name)
	{
		List<OwnedEntityData> list = new List<OwnedEntityData>();
		for (int i = 0; i < this.ownedEntities.Count; i++)
		{
			OwnedEntityData ownedEntityData = this.ownedEntities[i];
			if (EntityClass.list[ownedEntityData.ClassId].entityClassName.ContainsCaseInsensitive(name))
			{
				list.Add(ownedEntityData);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06002079 RID: 8313 RVA: 0x000CACA8 File Offset: 0x000C8EA8
	public bool HasOwnedEntity(int _entityId)
	{
		return this.GetOwnedEntity(_entityId) != null;
	}

	// Token: 0x0600207A RID: 8314 RVA: 0x000CACB4 File Offset: 0x000C8EB4
	public OwnedEntityData[] GetOwnedEntities()
	{
		return this.ownedEntities.ToArray();
	}

	// Token: 0x1700039E RID: 926
	// (get) Token: 0x0600207B RID: 8315 RVA: 0x000CACC1 File Offset: 0x000C8EC1
	public int OwnedEntityCount
	{
		get
		{
			return this.ownedEntities.Count;
		}
	}

	// Token: 0x0600207C RID: 8316 RVA: 0x000CACCE File Offset: 0x000C8ECE
	public void ClearOwnedEntities()
	{
		this.ownedEntities.Clear();
	}

	// Token: 0x0600207D RID: 8317 RVA: 0x000CACDB File Offset: 0x000C8EDB
	public void HandleSetNavName()
	{
		if (this.NavObject != null)
		{
			this.NavObject.name = this.entityName;
		}
	}

	// Token: 0x0600207E RID: 8318 RVA: 0x000CACF8 File Offset: 0x000C8EF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDynamicRagdoll()
	{
		if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.Active))
		{
			if (this.accumulatedRootMotion != Vector3.zero)
			{
				this._dynamicRagdollRootMotion = this.accumulatedRootMotion;
			}
			if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.UseBoneVelocities))
			{
				this._ragdollPositionsPrev.Clear();
				this._ragdollPositionsCur.CopyTo(this._ragdollPositionsPrev);
				this.emodel.CaptureRagdollPositions(this._ragdollPositionsCur);
			}
			if (this._dynamicRagdoll.HasFlag(DynamicRagdollFlags.RagdollOnFall) && !this.onGround)
			{
				this.ActivateDynamicRagdoll();
				return;
			}
		}
	}

	// Token: 0x0600207F RID: 8319 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void AnalyticsSendDeath(DamageResponse _dmResponse)
	{
	}

	// Token: 0x06002080 RID: 8320 RVA: 0x00047178 File Offset: 0x00045378
	public virtual string MakeDebugNameInfo()
	{
		return string.Empty;
	}

	// Token: 0x06002081 RID: 8321 RVA: 0x000CADAC File Offset: 0x000C8FAC
	public static void SetupAllDebugNameHUDs(bool _isAdd)
	{
		List<Entity> list = GameManager.Instance.World.Entities.list;
		for (int i = 0; i < list.Count; i++)
		{
			EntityAlive entityAlive = list[i] as EntityAlive;
			if (entityAlive)
			{
				entityAlive.SetupDebugNameHUD(_isAdd);
			}
		}
	}

	// Token: 0x06002082 RID: 8322 RVA: 0x000CADFC File Offset: 0x000C8FFC
	public void SetupDebugNameHUD(bool _isAdd)
	{
		if (this is EntityPlayer)
		{
			return;
		}
		GUIHUDEntityName component = this.ModelTransform.GetComponent<GUIHUDEntityName>();
		if (_isAdd)
		{
			if (!component)
			{
				this.ModelTransform.gameObject.AddComponent<GUIHUDEntityName>();
				return;
			}
		}
		else if (component)
		{
			UnityEngine.Object.Destroy(component);
		}
	}

	// Token: 0x06002083 RID: 8323 RVA: 0x000CAE49 File Offset: 0x000C9049
	public EModelBase.HeadStates GetHeadState()
	{
		if (base.EntityClass.CanBigHead)
		{
			return this.emodel.HeadState;
		}
		return EModelBase.HeadStates.Standard;
	}

	// Token: 0x06002084 RID: 8324 RVA: 0x000CAE68 File Offset: 0x000C9068
	public void SetBigHead()
	{
		if ((this is EntityAnimal || this is EntityEnemy || this is EntityTrader) && base.EntityClass.CanBigHead && this.emodel.HeadState == EModelBase.HeadStates.Standard)
		{
			this.emodel.HeadState = EModelBase.HeadStates.Growing;
			Manager.BroadcastPlayByLocalPlayer(this.position, "twitch_bighead_inflate");
		}
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x000CAEC4 File Offset: 0x000C90C4
	public void ResetHead()
	{
		if ((this is EntityAnimal || this is EntityEnemy || this is EntityTrader) && base.EntityClass.CanBigHead && (this.emodel.HeadState == EModelBase.HeadStates.BigHead || this.emodel.HeadState == EModelBase.HeadStates.Growing))
		{
			base.StartCoroutine(this.resetHeadLater(this.emodel));
		}
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x000CAF25 File Offset: 0x000C9125
	public void SetDancing(bool enabled)
	{
		if (base.EntityClass.DanceTypeID != 0)
		{
			this.IsDancing = enabled;
			return;
		}
		this.IsDancing = false;
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x000CAF43 File Offset: 0x000C9143
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator resetHeadLater(EModelBase model)
	{
		yield return new WaitForSeconds(0.25f);
		if (this.emodel != null && this.emodel.GetHeadTransform() != null && this.emodel.GetHeadTransform().localScale.x > 1f)
		{
			this.emodel.HeadState = EModelBase.HeadStates.Shrinking;
			Manager.BroadcastPlayByLocalPlayer(this.position, "twitch_bighead_deflate");
		}
		yield break;
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000CAF52 File Offset: 0x000C9152
	public void SetSpawnByData(int newSpawnByID, string newSpawnByName)
	{
		this.spawnById = newSpawnByID;
		this.spawnByName = newSpawnByName;
		this.bPlayerStatsChanged |= !this.isEntityRemote;
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x000CAF78 File Offset: 0x000C9178
	[PublicizedFrom(EAccessModifier.Internal)]
	public void SetHeadSize(float overrideHeadSize)
	{
		this.OverrideHeadSize = overrideHeadSize;
		this.emodel.SetHeadScale(overrideHeadSize);
	}

	// Token: 0x0600208A RID: 8330 RVA: 0x000CAF8D File Offset: 0x000C918D
	public void SetVehiclePoseMode(int _pose)
	{
		this.vehiclePoseMode = _pose;
		if (_pose != this.GetVehicleAnimation())
		{
			this.Crouching = false;
			this.SetVehicleAnimation(AvatarController.vehiclePoseHash, _pose);
		}
	}

	// Token: 0x0600208B RID: 8331 RVA: 0x000CAFB4 File Offset: 0x000C91B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void updateNetworkStats()
	{
		if (this.networkStatsUpdateQueue.Count > 0)
		{
			EntityAlive.NetworkStatChange networkStatChange = this.networkStatsUpdateQueue[0];
			this.networkStatsUpdateQueue.RemoveAt(0);
			if (networkStatChange.m_NetworkStats != null)
			{
				networkStatChange.m_NetworkStats.ToEntity(this);
				return;
			}
			EntityAlive.EntityNetworkHoldingData holdingData = networkStatChange.m_HoldingData;
			if (holdingData != null)
			{
				ItemStack holdingItemStack = holdingData.m_HoldingItemStack;
				byte holdingItemIndex = holdingData.m_HoldingItemIndex;
				if (!this.inventory.GetItem((int)holdingItemIndex).Equals(holdingItemStack))
				{
					this.inventory.SetItem((int)holdingItemIndex, holdingItemStack);
				}
				if (this.inventory.holdingItemIdx != (int)holdingItemIndex)
				{
					this.inventory.SetHoldingItemIdxNoHolsterTime((int)holdingItemIndex);
				}
			}
		}
	}

	// Token: 0x0600208C RID: 8332 RVA: 0x000CB054 File Offset: 0x000C9254
	public void EnqueueNetworkStats(EntityAlive.EntityNetworkStats netStats)
	{
		EntityAlive.NetworkStatChange networkStatChange = new EntityAlive.NetworkStatChange();
		networkStatChange.m_NetworkStats = netStats;
		this.networkStatsUpdateQueue.Add(networkStatChange);
	}

	// Token: 0x0600208D RID: 8333 RVA: 0x000CB07C File Offset: 0x000C927C
	public void EnqueueNetworkHoldingData(ItemStack holdingItemStack, byte holdingItemIndex)
	{
		EntityAlive.NetworkStatChange networkStatChange = new EntityAlive.NetworkStatChange();
		networkStatChange.m_HoldingData = new EntityAlive.EntityNetworkHoldingData
		{
			m_HoldingItemStack = holdingItemStack,
			m_HoldingItemIndex = holdingItemIndex
		};
		this.networkStatsUpdateQueue.Add(networkStatChange);
	}

	// Token: 0x0600208E RID: 8334 RVA: 0x000CB0B8 File Offset: 0x000C92B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAlive()
	{
	}

	// Token: 0x040015BF RID: 5567
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cTraderTeleportCheckTime = 0.1f;

	// Token: 0x040015C0 RID: 5568
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageImmunityOnRespawnSeconds = 1f;

	// Token: 0x040015C1 RID: 5569
	public static readonly FastTags<TagGroup.Global> DistractionResistanceWithTargetTags = FastTags<TagGroup.Global>.GetTag("with_target");

	// Token: 0x040015C2 RID: 5570
	public static readonly int FeralTagBit = FastTags<TagGroup.Global>.GetBit("feral");

	// Token: 0x040015C3 RID: 5571
	public static readonly int FallingBuffTagBit = FastTags<TagGroup.Global>.GetBit("buffPlayerFallingDamage");

	// Token: 0x040015C4 RID: 5572
	public static readonly FastTags<TagGroup.Global> StanceTagCrouching = FastTags<TagGroup.Global>.GetTag("crouching");

	// Token: 0x040015C5 RID: 5573
	public static readonly FastTags<TagGroup.Global> StanceTagStanding = FastTags<TagGroup.Global>.GetTag("standing");

	// Token: 0x040015C6 RID: 5574
	public static readonly FastTags<TagGroup.Global> MovementTagIdle = FastTags<TagGroup.Global>.GetTag("idle");

	// Token: 0x040015C7 RID: 5575
	public static readonly FastTags<TagGroup.Global> MovementTagWalking = FastTags<TagGroup.Global>.GetTag("walking");

	// Token: 0x040015C8 RID: 5576
	public static readonly FastTags<TagGroup.Global> MovementTagRunning = FastTags<TagGroup.Global>.GetTag("running");

	// Token: 0x040015C9 RID: 5577
	public static readonly FastTags<TagGroup.Global> MovementTagFloating = FastTags<TagGroup.Global>.GetTag("floating");

	// Token: 0x040015CA RID: 5578
	public static readonly FastTags<TagGroup.Global> MovementTagSwimming = FastTags<TagGroup.Global>.GetTag("swimming");

	// Token: 0x040015CB RID: 5579
	public static readonly FastTags<TagGroup.Global> MovementTagSwimmingRun = FastTags<TagGroup.Global>.GetTag("swimmingRun");

	// Token: 0x040015CC RID: 5580
	public static readonly FastTags<TagGroup.Global> MovementTagJumping = FastTags<TagGroup.Global>.GetTag("jumping");

	// Token: 0x040015CD RID: 5581
	public static readonly FastTags<TagGroup.Global> MovementTagFalling = FastTags<TagGroup.Global>.GetTag("falling");

	// Token: 0x040015CE RID: 5582
	public static readonly FastTags<TagGroup.Global> MovementTagClimbing = FastTags<TagGroup.Global>.GetTag("climbing");

	// Token: 0x040015CF RID: 5583
	public static readonly FastTags<TagGroup.Global> MovementTagDriving = FastTags<TagGroup.Global>.GetTag("driving");

	// Token: 0x040015D0 RID: 5584
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly float[] moveSpeedRandomness = new float[]
	{
		0.2f,
		1f,
		1.1f,
		1.2f,
		1.35f,
		1.5f
	};

	// Token: 0x040015D1 RID: 5585
	public const float CLIMB_LADDER_SPEED = 1234f;

	// Token: 0x040015D2 RID: 5586
	public static ulong HitDelay = 11000UL;

	// Token: 0x040015D3 RID: 5587
	public static float HitSoundDistance = 10f;

	// Token: 0x040015D4 RID: 5588
	public MinEventParams MinEventContext = new MinEventParams();

	// Token: 0x040015D5 RID: 5589
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int equippingCount;

	// Token: 0x040015D6 RID: 5590
	public bool IsSleeper;

	// Token: 0x040015D7 RID: 5591
	public bool IsSleeping;

	// Token: 0x040015D8 RID: 5592
	public bool IsSleeperPassive;

	// Token: 0x040015D9 RID: 5593
	public bool SleeperSupressLivingSounds;

	// Token: 0x040015DA RID: 5594
	public Vector3 SleeperSpawnPosition;

	// Token: 0x040015DB RID: 5595
	public Vector3 SleeperSpawnLookDir;

	// Token: 0x040015DC RID: 5596
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float accumulatedDamageResisted;

	// Token: 0x040015DD RID: 5597
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int pendingSleepTrigger = -1;

	// Token: 0x040015DE RID: 5598
	public int lastSleeperPose;

	// Token: 0x040015DF RID: 5599
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 sleeperLookDir;

	// Token: 0x040015E0 RID: 5600
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float sleeperSightRange;

	// Token: 0x040015E1 RID: 5601
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float sleeperViewAngle;

	// Token: 0x040015E2 RID: 5602
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 sightLightThreshold;

	// Token: 0x040015E3 RID: 5603
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 sightWakeThresholdAtRange;

	// Token: 0x040015E4 RID: 5604
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 sightGroanThresholdAtRange;

	// Token: 0x040015E5 RID: 5605
	public float sleeperNoiseToSense;

	// Token: 0x040015E6 RID: 5606
	public float sleeperNoiseToWake;

	// Token: 0x040015E7 RID: 5607
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isSnore;

	// Token: 0x040015E8 RID: 5608
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isGroan;

	// Token: 0x040015E9 RID: 5609
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isGroanSilent;

	// Token: 0x040015EA RID: 5610
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float sleeperNoiseToSenseSoundChance;

	// Token: 0x040015EB RID: 5611
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int snoreGroanCD;

	// Token: 0x040015EC RID: 5612
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int kSnoreGroanMinCD = 20;

	// Token: 0x040015ED RID: 5613
	public float noisePlayerDistance;

	// Token: 0x040015EE RID: 5614
	public float noisePlayerVolume;

	// Token: 0x040015EF RID: 5615
	public EntityPlayer noisePlayer;

	// Token: 0x040015F0 RID: 5616
	public EntityItem pendingDistraction;

	// Token: 0x040015F1 RID: 5617
	public float pendingDistractionDistanceSq;

	// Token: 0x040015F2 RID: 5618
	public EntityItem distraction;

	// Token: 0x040015F3 RID: 5619
	public float distractionResistance;

	// Token: 0x040015F4 RID: 5620
	public float distractionResistanceWithTarget;

	// Token: 0x040015F5 RID: 5621
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimGravityPer = 0.025f;

	// Token: 0x040015F6 RID: 5622
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDragY = 0.91f;

	// Token: 0x040015F7 RID: 5623
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimDrag = 0.91f;

	// Token: 0x040015F8 RID: 5624
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSwimAnimDelay = 6f;

	// Token: 0x040015F9 RID: 5625
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int jumpTicks;

	// Token: 0x040015FA RID: 5626
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive.JumpState jumpState;

	// Token: 0x040015FB RID: 5627
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int jumpStateTicks;

	// Token: 0x040015FC RID: 5628
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpDistance;

	// Token: 0x040015FD RID: 5629
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpHeightDiff;

	// Token: 0x040015FE RID: 5630
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpSwimDurationTicks;

	// Token: 0x040015FF RID: 5631
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 jumpSwimMotion;

	// Token: 0x04001600 RID: 5632
	public float jumpDelay;

	// Token: 0x04001601 RID: 5633
	public float jumpMaxDistance;

	// Token: 0x04001602 RID: 5634
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool jumpIsMoving;

	// Token: 0x04001603 RID: 5635
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksNoPlayerAdjacent;

	// Token: 0x04001604 RID: 5636
	public int hasBeenAttackedTime;

	// Token: 0x04001605 RID: 5637
	public float painHitsFelt;

	// Token: 0x04001606 RID: 5638
	public float painResistPercent;

	// Token: 0x04001607 RID: 5639
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int attackingTime;

	// Token: 0x04001608 RID: 5640
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive revengeEntity;

	// Token: 0x04001609 RID: 5641
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int revengeTimer;

	// Token: 0x0400160A RID: 5642
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool targetAlertChanged;

	// Token: 0x0400160B RID: 5643
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastAliveTime;

	// Token: 0x0400160C RID: 5644
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool alertEnabled = true;

	// Token: 0x0400160D RID: 5645
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int alertTicks;

	// Token: 0x0400160E RID: 5646
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string notAlertedId = "_notAlerted";

	// Token: 0x0400160F RID: 5647
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int notAlertDelayTicks;

	// Token: 0x04001610 RID: 5648
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isAlert;

	// Token: 0x04001611 RID: 5649
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 investigatePos;

	// Token: 0x04001612 RID: 5650
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int investigatePositionTicks;

	// Token: 0x04001613 RID: 5651
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInvestigateAlert;

	// Token: 0x04001614 RID: 5652
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasAI;

	// Token: 0x04001615 RID: 5653
	public EAIManager aiManager;

	// Token: 0x04001616 RID: 5654
	public List<string> AIPackages;

	// Token: 0x04001617 RID: 5655
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Context utilityAIContext;

	// Token: 0x04001618 RID: 5656
	public EntityPlayer aiClosestPlayer;

	// Token: 0x04001619 RID: 5657
	public float aiClosestPlayerDistSq;

	// Token: 0x0400161A RID: 5658
	public float aiActiveScale;

	// Token: 0x0400161B RID: 5659
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float aiActiveDelay;

	// Token: 0x0400161C RID: 5660
	public bool IsBloodMoon;

	// Token: 0x0400161D RID: 5661
	public bool IsFeral;

	// Token: 0x0400161E RID: 5662
	public bool IsBreakingDoors;

	// Token: 0x0400161F RID: 5663
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_isBreakingBlocks;

	// Token: 0x04001620 RID: 5664
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_isEating;

	// Token: 0x04001621 RID: 5665
	public Vector3 ChaseReturnLocation;

	// Token: 0x04001622 RID: 5666
	public bool IsScoutZombie;

	// Token: 0x04001623 RID: 5667
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityLookHelper lookHelper;

	// Token: 0x04001624 RID: 5668
	public EntityMoveHelper moveHelper;

	// Token: 0x04001625 RID: 5669
	public PathNavigate navigator;

	// Token: 0x04001626 RID: 5670
	public bool bCanClimbLadders;

	// Token: 0x04001627 RID: 5671
	public bool bCanClimbVertical;

	// Token: 0x04001628 RID: 5672
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive damagedTarget;

	// Token: 0x04001629 RID: 5673
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive attackTarget;

	// Token: 0x0400162A RID: 5674
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackTargetTime;

	// Token: 0x0400162B RID: 5675
	public EntityAlive attackTargetClient;

	// Token: 0x0400162C RID: 5676
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive attackTargetLast;

	// Token: 0x0400162D RID: 5677
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntitySeeCache seeCache;

	// Token: 0x0400162E RID: 5678
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ChunkCoordinates homePosition;

	// Token: 0x0400162F RID: 5679
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int maximumHomeDistance;

	// Token: 0x04001630 RID: 5680
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float jumpMovementFactor = 0.02f;

	// Token: 0x04001631 RID: 5681
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float landMovementFactor = 0.1f;

	// Token: 0x04001632 RID: 5682
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float jumpMotionYValue = 0.419f;

	// Token: 0x04001633 RID: 5683
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stepSoundDistanceRemaining;

	// Token: 0x04001634 RID: 5684
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float stepSoundRotYRemaining;

	// Token: 0x04001635 RID: 5685
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float nextSwimDistance;

	// Token: 0x04001636 RID: 5686
	public Inventory inventory;

	// Token: 0x04001637 RID: 5687
	public Inventory saveInventory;

	// Token: 0x04001638 RID: 5688
	public Equipment equipment;

	// Token: 0x04001639 RID: 5689
	public Bag bag;

	// Token: 0x0400163A RID: 5690
	public ChallengeJournal challengeJournal;

	// Token: 0x0400163B RID: 5691
	public int ExperienceValue;

	// Token: 0x0400163C RID: 5692
	public int deathUpdateTime;

	// Token: 0x0400163D RID: 5693
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityAlive entityThatKilledMe;

	// Token: 0x0400163E RID: 5694
	public bool bPlayerStatsChanged;

	// Token: 0x0400163F RID: 5695
	public bool bEntityAliveFlagsChanged;

	// Token: 0x04001640 RID: 5696
	public bool bPlayerTwitchChanged;

	// Token: 0x04001641 RID: 5697
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<EnumDamageSource, ulong> damageSourceTimeouts = new EnumDictionary<EnumDamageSource, ulong>();

	// Token: 0x04001642 RID: 5698
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int traderTeleportStreak = 1;

	// Token: 0x04001643 RID: 5699
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bJetpackWearing;

	// Token: 0x04001644 RID: 5700
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bJetpackActive;

	// Token: 0x04001645 RID: 5701
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bParachuteWearing;

	// Token: 0x04001646 RID: 5702
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bAimingGun;

	// Token: 0x04001647 RID: 5703
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bMovementRunning;

	// Token: 0x04001648 RID: 5704
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bCrouching;

	// Token: 0x04001649 RID: 5705
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bJumping;

	// Token: 0x0400164A RID: 5706
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bClimbing;

	// Token: 0x0400164B RID: 5707
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int died;

	// Token: 0x0400164C RID: 5708
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int score;

	// Token: 0x0400164D RID: 5709
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int killedZombies;

	// Token: 0x0400164E RID: 5710
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int killedPlayers;

	// Token: 0x0400164F RID: 5711
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int teamNumber;

	// Token: 0x04001650 RID: 5712
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string entityName = string.Empty;

	// Token: 0x04001651 RID: 5713
	public string DebugNameInfo = string.Empty;

	// Token: 0x04001652 RID: 5714
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int damageLocationBits;

	// Token: 0x04001653 RID: 5715
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bSpawned;

	// Token: 0x04001654 RID: 5716
	public bool bReplicatedAlertFlag;

	// Token: 0x04001655 RID: 5717
	public int vehiclePoseMode = -1;

	// Token: 0x04001656 RID: 5718
	public byte factionId;

	// Token: 0x04001657 RID: 5719
	public byte factionRank;

	// Token: 0x04001658 RID: 5720
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int ticksToCheckSeenByPlayer;

	// Token: 0x04001659 RID: 5721
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasSeenByPlayer;

	// Token: 0x0400165A RID: 5722
	public DamageResponse RecordedDamage;

	// Token: 0x0400165B RID: 5723
	public float moveSpeed;

	// Token: 0x0400165C RID: 5724
	public float moveSpeedNight;

	// Token: 0x0400165D RID: 5725
	public float moveSpeedAggro;

	// Token: 0x0400165E RID: 5726
	public float moveSpeedAggroMax;

	// Token: 0x0400165F RID: 5727
	public float moveSpeedPanic;

	// Token: 0x04001660 RID: 5728
	public float moveSpeedPanicMax;

	// Token: 0x04001661 RID: 5729
	public float swimSpeed;

	// Token: 0x04001662 RID: 5730
	public Vector2 swimStrokeRate;

	// Token: 0x04001663 RID: 5731
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ItemValue handItem;

	// Token: 0x04001664 RID: 5732
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundSpawn;

	// Token: 0x04001665 RID: 5733
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundSleeperGroan;

	// Token: 0x04001666 RID: 5734
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundSleeperSnore;

	// Token: 0x04001667 RID: 5735
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundDeath;

	// Token: 0x04001668 RID: 5736
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundAlert;

	// Token: 0x04001669 RID: 5737
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundAttack;

	// Token: 0x0400166A RID: 5738
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundLiving;

	// Token: 0x0400166B RID: 5739
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundRandom;

	// Token: 0x0400166C RID: 5740
	public string soundSense;

	// Token: 0x0400166D RID: 5741
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundGiveUp;

	// Token: 0x0400166E RID: 5742
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundStepType;

	// Token: 0x0400166F RID: 5743
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundStamina;

	// Token: 0x04001670 RID: 5744
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundJump;

	// Token: 0x04001671 RID: 5745
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundLand;

	// Token: 0x04001672 RID: 5746
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundHurt;

	// Token: 0x04001673 RID: 5747
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundHurtSmall;

	// Token: 0x04001674 RID: 5748
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string soundDrownPain;

	// Token: 0x04001675 RID: 5749
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string soundDrownDeath;

	// Token: 0x04001676 RID: 5750
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string soundWaterSurface;

	// Token: 0x04001677 RID: 5751
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundDelayTicks;

	// Token: 0x04001678 RID: 5752
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundLivingID = -1;

	// Token: 0x04001679 RID: 5753
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSoundRandomMaxDist = 20f;

	// Token: 0x0400167A RID: 5754
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundAlertTicks;

	// Token: 0x0400167B RID: 5755
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int soundRandomTicks;

	// Token: 0x0400167C RID: 5756
	public int classMaxHealth;

	// Token: 0x0400167D RID: 5757
	public int classMaxStamina;

	// Token: 0x0400167E RID: 5758
	public int classMaxFood;

	// Token: 0x0400167F RID: 5759
	public int classMaxWater;

	// Token: 0x04001680 RID: 5760
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float weight;

	// Token: 0x04001681 RID: 5761
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float pushFactor;

	// Token: 0x04001682 RID: 5762
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float maxViewAngle;

	// Token: 0x04001683 RID: 5763
	public float sightRangeBase;

	// Token: 0x04001684 RID: 5764
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float sightRange;

	// Token: 0x04001685 RID: 5765
	public float senseScale;

	// Token: 0x04001686 RID: 5766
	public int timeStayAfterDeath;

	// Token: 0x04001687 RID: 5767
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public BlockValue corpseBlockValue;

	// Token: 0x04001688 RID: 5768
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float corpseBlockChance;

	// Token: 0x04001689 RID: 5769
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackTimeoutDay;

	// Token: 0x0400168A RID: 5770
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attackTimeoutNight;

	// Token: 0x0400168B RID: 5771
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string particleOnDeath;

	// Token: 0x0400168C RID: 5772
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string particleOnDestroy;

	// Token: 0x0400168D RID: 5773
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityBedrollPositionList spawnPoints;

	// Token: 0x0400168E RID: 5774
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<Vector3i> droppedBackpackPositions;

	// Token: 0x0400168F RID: 5775
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float speedModifier = 1f;

	// Token: 0x04001690 RID: 5776
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 accumulatedRootMotion;

	// Token: 0x04001691 RID: 5777
	public Vector3 moveDirection;

	// Token: 0x04001692 RID: 5778
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMoveDirAbsolute;

	// Token: 0x04001693 RID: 5779
	public Vector3 lookAtPosition;

	// Token: 0x04001694 RID: 5780
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3i blockPosStandingOn;

	// Token: 0x04001695 RID: 5781
	public BlockValue blockValueStandingOn;

	// Token: 0x04001696 RID: 5782
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool blockStandingOnChanged;

	// Token: 0x04001697 RID: 5783
	public BiomeDefinition biomeStandingOn;

	// Token: 0x04001698 RID: 5784
	public bool IsMale;

	// Token: 0x04001699 RID: 5785
	public int crouchType;

	// Token: 0x0400169A RID: 5786
	public float crouchBendPer;

	// Token: 0x0400169B RID: 5787
	public float crouchBendPerTarget;

	// Token: 0x0400169C RID: 5788
	public const int cWalkTypeSwim = -1;

	// Token: 0x0400169D RID: 5789
	public const int cWalkTypeFat = 1;

	// Token: 0x0400169E RID: 5790
	public const int cWalkTypeCripple = 5;

	// Token: 0x0400169F RID: 5791
	public const int cWalkTypeCrouch = 8;

	// Token: 0x040016A0 RID: 5792
	public const int cWalkTypeBandit = 15;

	// Token: 0x040016A1 RID: 5793
	public const int cWalkTypeCrawlFirst = 20;

	// Token: 0x040016A2 RID: 5794
	public const int cWalkTypeCrawler = 21;

	// Token: 0x040016A3 RID: 5795
	public const int cWalkTypeSpider = 22;

	// Token: 0x040016A4 RID: 5796
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int walkType;

	// Token: 0x040016A5 RID: 5797
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int walkTypeBeforeCrouch;

	// Token: 0x040016A6 RID: 5798
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string rightHandTransformName;

	// Token: 0x040016A7 RID: 5799
	public int pingToServer;

	// Token: 0x040016A8 RID: 5800
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<ItemStack> itemsOnEnterGame = new List<ItemStack>();

	// Token: 0x040016A9 RID: 5801
	public Utils.EnumHitDirection lastHitDirection = Utils.EnumHitDirection.None;

	// Token: 0x040016AA RID: 5802
	public Vector3 lastHitImpactDir = Vector3.zero;

	// Token: 0x040016AB RID: 5803
	public Vector3 lastHitEntityFwd = Vector3.zero;

	// Token: 0x040016AC RID: 5804
	public bool lastHitRanged;

	// Token: 0x040016AD RID: 5805
	public float lastHitForce;

	// Token: 0x040016AE RID: 5806
	public DamageResponse lastDamageResponse;

	// Token: 0x040016AF RID: 5807
	public bool canDisintegrate;

	// Token: 0x040016B0 RID: 5808
	public bool isDisintegrated;

	// Token: 0x040016B1 RID: 5809
	public float CreationTimeSinceLevelLoad;

	// Token: 0x040016B2 RID: 5810
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityStats entityStats;

	// Token: 0x040016B3 RID: 5811
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float proneRefillRate;

	// Token: 0x040016B4 RID: 5812
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float kneelRefillRate;

	// Token: 0x040016B5 RID: 5813
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float proneRefillCounter;

	// Token: 0x040016B6 RID: 5814
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float kneelRefillCounter;

	// Token: 0x040016B7 RID: 5815
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int deathHealth;

	// Token: 0x040016B8 RID: 5816
	public BodyDamage bodyDamage;

	// Token: 0x040016B9 RID: 5817
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool stompsSpikes;

	// Token: 0x040016BA RID: 5818
	public float OverrideSize = 1f;

	// Token: 0x040016BB RID: 5819
	public float OverrideHeadSize = 1f;

	// Token: 0x040016BC RID: 5820
	public float OverrideHeadDismemberScaleTime = 1.5f;

	// Token: 0x040016BD RID: 5821
	public float OverridePitch;

	// Token: 0x040016BE RID: 5822
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDancing;

	// Token: 0x040016BF RID: 5823
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeTraderStationChecked;

	// Token: 0x040016C0 RID: 5824
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lerpForwardSpeed;

	// Token: 0x040016C1 RID: 5825
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedForwardTarget;

	// Token: 0x040016C2 RID: 5826
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float speedForwardTargetStep = 1f;

	// Token: 0x040016C3 RID: 5827
	public EntityBuffs Buffs;

	// Token: 0x040016C4 RID: 5828
	public Progression Progression;

	// Token: 0x040016C5 RID: 5829
	public FastTags<TagGroup.Global> CurrentStanceTag = EntityAlive.StanceTagStanding;

	// Token: 0x040016C6 RID: 5830
	public FastTags<TagGroup.Global> CurrentMovementTag = FastTags<TagGroup.Global>.none;

	// Token: 0x040016C7 RID: 5831
	public float renderFadeMax = 1f;

	// Token: 0x040016C8 RID: 5832
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float renderFade;

	// Token: 0x040016C9 RID: 5833
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float renderFadeTarget;

	// Token: 0x040016CA RID: 5834
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<EntityAlive.FallBehavior> fallBehaviors = new List<EntityAlive.FallBehavior>();

	// Token: 0x040016CB RID: 5835
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool disableFallBehaviorUntilOnGround;

	// Token: 0x040016CC RID: 5836
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<EntityAlive.DestroyBlockBehavior> _destroyBlockBehaviors = new List<EntityAlive.DestroyBlockBehavior>();

	// Token: 0x040016CD RID: 5837
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public DynamicRagdollFlags _dynamicRagdoll;

	// Token: 0x040016CE RID: 5838
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float _dynamicRagdollStunTime;

	// Token: 0x040016CF RID: 5839
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 _dynamicRagdollRootMotion;

	// Token: 0x040016D0 RID: 5840
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<Vector3> _ragdollPositionsPrev = new List<Vector3>();

	// Token: 0x040016D1 RID: 5841
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<Vector3> _ragdollPositionsCur = new List<Vector3>();

	// Token: 0x040016D2 RID: 5842
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isFirstTimeEquipmentReassigned = true;

	// Token: 0x040016D3 RID: 5843
	public bool CrouchingLocked;

	// Token: 0x040016D4 RID: 5844
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EModelBase.HeadStates currentHeadState;

	// Token: 0x040016D5 RID: 5845
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<EntityAlive.WeightBehavior> weightBehaviorTemp = new List<EntityAlive.WeightBehavior>();

	// Token: 0x040016D6 RID: 5846
	public static bool ShowDebugDisplayHit = false;

	// Token: 0x040016D7 RID: 5847
	public static float DebugDisplayHitSize = 0.005f;

	// Token: 0x040016D8 RID: 5848
	public static float DebugDisplayHitTime = 10f;

	// Token: 0x040016D9 RID: 5849
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bPlayHurtSound;

	// Token: 0x040016DA RID: 5850
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool bBeenWounded;

	// Token: 0x040016DB RID: 5851
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int woundedStrength;

	// Token: 0x040016DC RID: 5852
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public DamageSource woundedDamageSource;

	// Token: 0x040016DD RID: 5853
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int despawnDelayCounter;

	// Token: 0x040016DE RID: 5854
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDespawnWhenPlayerFar;

	// Token: 0x040016DF RID: 5855
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool wasOnGround = true;

	// Token: 0x040016E0 RID: 5856
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float landWaterLevel;

	// Token: 0x040016E1 RID: 5857
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_addedToWorld;

	// Token: 0x040016E2 RID: 5858
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int saveHoldingItemIdxBeforeAttach;

	// Token: 0x040016E3 RID: 5859
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float impactSoundTime;

	// Token: 0x040016E4 RID: 5860
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Transform, EntityAlive.ImpactData> impacts = new Dictionary<Transform, EntityAlive.ImpactData>();

	// Token: 0x040016E5 RID: 5861
	public const string cParticlePrefix = "Ptl_";

	// Token: 0x040016E6 RID: 5862
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, Transform> particles = new Dictionary<string, Transform>();

	// Token: 0x040016E7 RID: 5863
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, Transform> parts = new Dictionary<string, Transform>();

	// Token: 0x040016E8 RID: 5864
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public List<OwnedEntityData> ownedEntities = new List<OwnedEntityData>();

	// Token: 0x040016E9 RID: 5865
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<EntityAlive.NetworkStatChange> networkStatsUpdateQueue = new List<EntityAlive.NetworkStatChange>();

	// Token: 0x02000411 RID: 1041
	public enum JumpState
	{
		// Token: 0x040016EB RID: 5867
		Off,
		// Token: 0x040016EC RID: 5868
		Climb,
		// Token: 0x040016ED RID: 5869
		Leap,
		// Token: 0x040016EE RID: 5870
		Air,
		// Token: 0x040016EF RID: 5871
		Land,
		// Token: 0x040016F0 RID: 5872
		SwimStart,
		// Token: 0x040016F1 RID: 5873
		Swim
	}

	// Token: 0x02000412 RID: 1042
	[PublicizedFrom(EAccessModifier.Protected)]
	public class FallBehavior
	{
		// Token: 0x06002090 RID: 8336 RVA: 0x000CB373 File Offset: 0x000C9573
		public FallBehavior(string name, EntityAlive.FallBehavior.Op type, FloatRange height, float weight, FloatRange ragePer, FloatRange rageTime, IntRange difficulty)
		{
			this.Name = name;
			this.ResponseOp = type;
			this.Height = height;
			this.Weight = weight;
			this.RagePer = ragePer;
			this.RageTime = rageTime;
			this.Difficulty = difficulty;
		}

		// Token: 0x040016F2 RID: 5874
		public string Name;

		// Token: 0x040016F3 RID: 5875
		public readonly EntityAlive.FallBehavior.Op ResponseOp;

		// Token: 0x040016F4 RID: 5876
		public readonly FloatRange Height;

		// Token: 0x040016F5 RID: 5877
		public readonly float Weight;

		// Token: 0x040016F6 RID: 5878
		public readonly FloatRange RagePer;

		// Token: 0x040016F7 RID: 5879
		public readonly FloatRange RageTime;

		// Token: 0x040016F8 RID: 5880
		public readonly IntRange Difficulty;

		// Token: 0x02000413 RID: 1043
		public enum Op
		{
			// Token: 0x040016FA RID: 5882
			None,
			// Token: 0x040016FB RID: 5883
			Land,
			// Token: 0x040016FC RID: 5884
			LandLow,
			// Token: 0x040016FD RID: 5885
			LandHard,
			// Token: 0x040016FE RID: 5886
			Stumble,
			// Token: 0x040016FF RID: 5887
			Ragdoll
		}
	}

	// Token: 0x02000414 RID: 1044
	[PublicizedFrom(EAccessModifier.Protected)]
	public class DestroyBlockBehavior
	{
		// Token: 0x06002091 RID: 8337 RVA: 0x000CB3B0 File Offset: 0x000C95B0
		public DestroyBlockBehavior(string name, EntityAlive.DestroyBlockBehavior.Op type, float weight, FloatRange ragePer, FloatRange rageTime, IntRange difficulty)
		{
			this.Name = name;
			this.ResponseOp = type;
			this.Weight = weight;
			this.RagePer = ragePer;
			this.RageTime = rageTime;
			this.Difficulty = difficulty;
		}

		// Token: 0x04001700 RID: 5888
		public string Name;

		// Token: 0x04001701 RID: 5889
		public readonly EntityAlive.DestroyBlockBehavior.Op ResponseOp;

		// Token: 0x04001702 RID: 5890
		public readonly float Weight;

		// Token: 0x04001703 RID: 5891
		public readonly FloatRange RagePer;

		// Token: 0x04001704 RID: 5892
		public readonly FloatRange RageTime;

		// Token: 0x04001705 RID: 5893
		public readonly IntRange Difficulty = new IntRange(int.MinValue, int.MaxValue);

		// Token: 0x02000415 RID: 1045
		public enum Op
		{
			// Token: 0x04001707 RID: 5895
			None,
			// Token: 0x04001708 RID: 5896
			Ragdoll,
			// Token: 0x04001709 RID: 5897
			Stumble
		}
	}

	// Token: 0x02000416 RID: 1046
	public enum EnumApproachState
	{
		// Token: 0x0400170B RID: 5899
		Ok,
		// Token: 0x0400170C RID: 5900
		TooFarAway,
		// Token: 0x0400170D RID: 5901
		BlockedByWorldMesh,
		// Token: 0x0400170E RID: 5902
		BlockedByEntity,
		// Token: 0x0400170F RID: 5903
		Unknown
	}

	// Token: 0x02000417 RID: 1047
	[PublicizedFrom(EAccessModifier.Private)]
	public struct WeightBehavior
	{
		// Token: 0x04001710 RID: 5904
		public float weight;

		// Token: 0x04001711 RID: 5905
		public int index;
	}

	// Token: 0x02000418 RID: 1048
	[PublicizedFrom(EAccessModifier.Private)]
	public struct ImpactData
	{
		// Token: 0x04001712 RID: 5906
		public int count;
	}

	// Token: 0x02000419 RID: 1049
	[PublicizedFrom(EAccessModifier.Private)]
	public class NetworkStatChange
	{
		// Token: 0x04001713 RID: 5907
		public EntityAlive.EntityNetworkStats m_NetworkStats;

		// Token: 0x04001714 RID: 5908
		public EntityAlive.EntityNetworkHoldingData m_HoldingData;
	}

	// Token: 0x0200041A RID: 1050
	public class EntityNetworkHoldingData
	{
		// Token: 0x04001715 RID: 5909
		public ItemStack m_HoldingItemStack;

		// Token: 0x04001716 RID: 5910
		public byte m_HoldingItemIndex;
	}

	// Token: 0x0200041B RID: 1051
	public class EntityNetworkStats
	{
		// Token: 0x06002095 RID: 8341 RVA: 0x000CB408 File Offset: 0x000C9608
		public void FillFromEntity(EntityAlive _entity)
		{
			this.killed = _entity.Died;
			this.holdingItemStack = _entity.inventory.holdingItemStack;
			this.holdingItemIndex = (byte)_entity.inventory.holdingItemIdx;
			this.deathHealth = _entity.DeathHealth;
			this.teamNumber = _entity.TeamNumber;
			this.equipment = _entity.equipment;
			if (GameManager.Instance.World.GetPrimaryPlayer() == _entity)
			{
				_entity.inventory.TurnOffLightFlares();
			}
			if (_entity.Progression != null && _entity.Progression.bProgressionStatsChanged)
			{
				_entity.Progression.bProgressionStatsChanged = false;
				this.hasProgression = true;
				this.progressionsData = _entity.Progression.ToBytes(false);
			}
			this.attachedToEntityId = ((_entity.AttachedToEntity != null) ? _entity.AttachedToEntity.entityId : -1);
			this.entityName = _entity.EntityName;
			EntityPlayer entityPlayer = _entity as EntityPlayer;
			if (entityPlayer != null)
			{
				this.isPlayer = true;
				this.killedPlayers = _entity.KilledPlayers;
				this.killedZombies = _entity.KilledZombies;
				this.experience = entityPlayer.Progression.ExpToNextLevel;
				this.level = entityPlayer.Progression.Level;
				this.totalItemsCrafted = entityPlayer.totalItemsCrafted;
				this.distanceWalked = entityPlayer.distanceWalked;
				this.longestLife = entityPlayer.longestLife;
				this.currentLife = entityPlayer.currentLife;
				this.totalTimePlayed = entityPlayer.totalTimePlayed;
				this.vehiclePose = entityPlayer.GetVehicleAnimation();
				this.isSpectator = entityPlayer.IsSpectator;
				return;
			}
			this.isPlayer = false;
			this.experience = 0;
			this.level = 1;
			this.distanceWalked = 0f;
			this.totalItemsCrafted = 0U;
			this.longestLife = 0f;
			this.currentLife = 0f;
			this.totalTimePlayed = 0f;
		}

		// Token: 0x06002096 RID: 8342 RVA: 0x000CB5E4 File Offset: 0x000C97E4
		public void ToEntity(EntityAlive _entity)
		{
			_entity.Died = this.killed;
			_entity.DeathHealth = this.deathHealth;
			_entity.TeamNumber = this.teamNumber;
			_entity.inventory.bResetLightLevelWhenChanged = true;
			if (!_entity.inventory.GetItem((int)this.holdingItemIndex).Equals(this.holdingItemStack))
			{
				_entity.inventory.SetItem((int)this.holdingItemIndex, this.holdingItemStack);
				_entity.inventory.ForceHoldingItemUpdate();
			}
			if (_entity.inventory.holdingItemIdx != (int)this.holdingItemIndex)
			{
				_entity.inventory.SetHoldingItemIdxNoHolsterTime((int)this.holdingItemIndex);
			}
			_entity.equipment.Apply(this.equipment, false);
			if (this.hasProgression)
			{
				_entity.Progression = Progression.FromBytes(this.progressionsData, _entity);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && _entity.Progression != null)
				{
					_entity.Progression.bProgressionStatsChanged = true;
				}
			}
			_entity.SetEntityName(this.entityName);
			EntityPlayer entityPlayer = _entity as EntityPlayer;
			if (entityPlayer != null && this.isPlayer)
			{
				if (_entity.NavObject != null)
				{
					_entity.NavObject.name = this.entityName;
				}
				_entity.KilledZombies = this.killedZombies;
				_entity.KilledPlayers = this.killedPlayers;
				entityPlayer.Progression.ExpToNextLevel = this.experience;
				entityPlayer.Progression.Level = this.level;
				entityPlayer.totalItemsCrafted = this.totalItemsCrafted;
				entityPlayer.distanceWalked = this.distanceWalked;
				entityPlayer.longestLife = this.longestLife;
				entityPlayer.currentLife = this.currentLife;
				entityPlayer.totalTimePlayed = this.totalTimePlayed;
				entityPlayer.SetVehiclePoseMode(this.vehiclePose);
				entityPlayer.IsSpectator = this.isSpectator;
			}
		}

		// Token: 0x06002097 RID: 8343 RVA: 0x000CB7A4 File Offset: 0x000C99A4
		public void read(PooledBinaryReader _reader)
		{
			this.killed = _reader.ReadInt32();
			this.holdingItemStack = new ItemStack();
			this.holdingItemStack.Read(_reader);
			this.holdingItemIndex = _reader.ReadByte();
			this.deathHealth = _reader.ReadInt32();
			this.teamNumber = (int)_reader.ReadByte();
			this.equipment = Equipment.Read(_reader);
			this.attachedToEntityId = _reader.ReadInt32();
			this.entityName = _reader.ReadString();
			this.isPlayer = _reader.ReadBoolean();
			if (this.isPlayer)
			{
				this.killedZombies = _reader.ReadInt32();
				this.killedPlayers = _reader.ReadInt32();
				this.experience = _reader.ReadInt32();
				this.level = _reader.ReadInt32();
				this.totalItemsCrafted = _reader.ReadUInt32();
				this.distanceWalked = _reader.ReadSingle();
				this.longestLife = _reader.ReadSingle();
				this.currentLife = _reader.ReadSingle();
				this.totalTimePlayed = _reader.ReadSingle();
				this.vehiclePose = _reader.ReadInt32();
				this.isSpectator = _reader.ReadBoolean();
			}
			this.hasProgression = _reader.ReadBoolean();
			if (this.hasProgression)
			{
				int num = (int)_reader.ReadInt16();
				this.progressionsData = new byte[num];
				_reader.Read(this.progressionsData, 0, num);
			}
		}

		// Token: 0x06002098 RID: 8344 RVA: 0x000CB8F0 File Offset: 0x000C9AF0
		public void write(PooledBinaryWriter _writer)
		{
			_writer.Write(this.killed);
			this.holdingItemStack.Write(_writer);
			_writer.Write(this.holdingItemIndex);
			_writer.Write(this.deathHealth);
			_writer.Write((byte)this.teamNumber);
			this.equipment.Write(_writer);
			_writer.Write(this.attachedToEntityId);
			_writer.Write(this.entityName);
			_writer.Write(this.isPlayer);
			if (this.isPlayer)
			{
				_writer.Write(this.killedZombies);
				_writer.Write(this.killedPlayers);
				_writer.Write(this.experience);
				_writer.Write(this.level);
				_writer.Write(this.totalItemsCrafted);
				_writer.Write(this.distanceWalked);
				_writer.Write(this.longestLife);
				_writer.Write(this.currentLife);
				_writer.Write(this.totalTimePlayed);
				_writer.Write(this.vehiclePose);
				_writer.Write(this.isSpectator);
			}
			_writer.Write(this.hasProgression);
			if (this.hasProgression)
			{
				_writer.Write((short)this.progressionsData.Length);
				_writer.Write(this.progressionsData, 0, this.progressionsData.Length);
			}
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000CBA31 File Offset: 0x000C9C31
		public void SetName(string name)
		{
			this.entityName = name;
		}

		// Token: 0x04001717 RID: 5911
		[PublicizedFrom(EAccessModifier.Private)]
		public int experience;

		// Token: 0x04001718 RID: 5912
		[PublicizedFrom(EAccessModifier.Private)]
		public int level;

		// Token: 0x04001719 RID: 5913
		[PublicizedFrom(EAccessModifier.Private)]
		public int killed;

		// Token: 0x0400171A RID: 5914
		[PublicizedFrom(EAccessModifier.Private)]
		public int killedZombies;

		// Token: 0x0400171B RID: 5915
		[PublicizedFrom(EAccessModifier.Private)]
		public int killedPlayers;

		// Token: 0x0400171C RID: 5916
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemStack holdingItemStack;

		// Token: 0x0400171D RID: 5917
		[PublicizedFrom(EAccessModifier.Private)]
		public byte holdingItemIndex;

		// Token: 0x0400171E RID: 5918
		[PublicizedFrom(EAccessModifier.Private)]
		public int deathHealth;

		// Token: 0x0400171F RID: 5919
		[PublicizedFrom(EAccessModifier.Private)]
		public int teamNumber;

		// Token: 0x04001720 RID: 5920
		[PublicizedFrom(EAccessModifier.Private)]
		public Equipment equipment;

		// Token: 0x04001721 RID: 5921
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasProgression;

		// Token: 0x04001722 RID: 5922
		[PublicizedFrom(EAccessModifier.Private)]
		public byte[] progressionsData;

		// Token: 0x04001723 RID: 5923
		[PublicizedFrom(EAccessModifier.Private)]
		public int attachedToEntityId;

		// Token: 0x04001724 RID: 5924
		[PublicizedFrom(EAccessModifier.Private)]
		public string entityName;

		// Token: 0x04001725 RID: 5925
		[PublicizedFrom(EAccessModifier.Private)]
		public float distanceWalked;

		// Token: 0x04001726 RID: 5926
		[PublicizedFrom(EAccessModifier.Private)]
		public uint totalItemsCrafted;

		// Token: 0x04001727 RID: 5927
		[PublicizedFrom(EAccessModifier.Private)]
		public float longestLife;

		// Token: 0x04001728 RID: 5928
		[PublicizedFrom(EAccessModifier.Private)]
		public float currentLife;

		// Token: 0x04001729 RID: 5929
		[PublicizedFrom(EAccessModifier.Private)]
		public float totalTimePlayed;

		// Token: 0x0400172A RID: 5930
		[PublicizedFrom(EAccessModifier.Private)]
		public int vehiclePose;

		// Token: 0x0400172B RID: 5931
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isSpectator;

		// Token: 0x0400172C RID: 5932
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isPlayer;
	}
}
