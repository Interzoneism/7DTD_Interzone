using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200039A RID: 922
[Preserve]
public class DroneWeapons
{
	// Token: 0x0400125B RID: 4699
	public const string cSHOCK_BUFF_NAME = "buffShocked";

	// Token: 0x0400125C RID: 4700
	public const string cBuffHealCooldown = "buffJunkDroneHealCooldownEffect";

	// Token: 0x0400125D RID: 4701
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> healingItemTags = FastTags<TagGroup.Global>.Parse("medical");

	// Token: 0x0400125E RID: 4702
	public const string cHealWeaponJoint = "WristLeft";

	// Token: 0x0200039B RID: 923
	[Preserve]
	public class Weapon
	{
		// Token: 0x06001B89 RID: 7049 RVA: 0x000AC7F4 File Offset: 0x000AA9F4
		public Weapon(EntityAlive _entity)
		{
			this.entity = _entity;
			this.properties = _entity.EntityClass.Properties;
			this.belongsPlayerId = this.entity.belongsPlayerId;
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Init()
		{
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x000AC848 File Offset: 0x000AAA48
		public virtual void Update()
		{
			if (this.cooldownTimer > 0f)
			{
				this.cooldownTimer -= 0.05f;
				if (this.cooldownTimer <= 0f || (this.target && this.target.IsDead()))
				{
					this.InvokeFireComplete();
				}
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06001B8C RID: 7052 RVA: 0x000AC8A1 File Offset: 0x000AAAA1
		public float TimeRemaning
		{
			get
			{
				return this.cooldownTimer;
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06001B8D RID: 7053 RVA: 0x000AC8A9 File Offset: 0x000AAAA9
		public float TimeLength
		{
			get
			{
				return this.actionTime + this.cooldown;
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06001B8E RID: 7054 RVA: 0x000AC8B8 File Offset: 0x000AAAB8
		public float Range
		{
			get
			{
				return this.range;
			}
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x000AC8C0 File Offset: 0x000AAAC0
		public virtual bool canFire()
		{
			return this.cooldownTimer <= 0f;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x000AC8D2 File Offset: 0x000AAAD2
		public virtual void Fire(EntityAlive _target)
		{
			this.target = _target;
			this.cooldownTimer = this.actionTime + this.cooldown;
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x000AC8EE File Offset: 0x000AAAEE
		public virtual bool hasActionCompleted()
		{
			return this.cooldownTimer < this.cooldown;
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x000AC8FE File Offset: 0x000AAAFE
		public void RegisterOnFireComplete(Action _onFireComplete)
		{
			this.onFireComplete = _onFireComplete;
		}

		// Token: 0x06001B93 RID: 7059 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnFireComplete()
		{
		}

		// Token: 0x06001B94 RID: 7060 RVA: 0x000AC907 File Offset: 0x000AAB07
		[PublicizedFrom(EAccessModifier.Protected)]
		public void InvokeFireComplete()
		{
			this.OnFireComplete();
			Action action = this.onFireComplete;
			if (action != null)
			{
				action();
			}
			this.onFireComplete = null;
			this.target = null;
		}

		// Token: 0x06001B95 RID: 7061 RVA: 0x000AC92E File Offset: 0x000AAB2E
		[PublicizedFrom(EAccessModifier.Protected)]
		public void TargetApplyBuff(string _buff)
		{
			this.target.Buffs.AddBuff(_buff, (this.belongsPlayerId != -1) ? this.belongsPlayerId : this.entity.entityId, true, false, -1f);
		}

		// Token: 0x06001B96 RID: 7062 RVA: 0x000AC968 File Offset: 0x000AAB68
		[PublicizedFrom(EAccessModifier.Protected)]
		public void SpawnParticleEffect(ParticleEffect _pe, int _entityId)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				if (!GameManager.IsDedicatedServer)
				{
					GameManager.Instance.SpawnParticleEffectClient(_pe, _entityId, false, false);
				}
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, false), false, -1, _entityId, -1, null, 192, false);
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, false), false);
		}

		// Token: 0x06001B97 RID: 7063 RVA: 0x000AC9E0 File Offset: 0x000AABE0
		[PublicizedFrom(EAccessModifier.Protected)]
		public Transform SpawnDroneParticleEffect(ParticleEffect _pe, int _entityId, DroneWeapons.NetPackageDroneParticleEffect.cActionType actionType)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<DroneWeapons.NetPackageDroneParticleEffect>().Setup(_pe, _entityId, actionType), false, -1, -1, -1, null, 192, false);
				if (!GameManager.IsDedicatedServer)
				{
					return GameManager.Instance.SpawnParticleEffectClientForceCreation(_pe, _entityId, false);
				}
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<DroneWeapons.NetPackageDroneParticleEffect>().Setup(_pe, _entityId, actionType), false);
			}
			return null;
		}

		// Token: 0x0400125F RID: 4703
		public Transform WeaponJoint;

		// Token: 0x04001260 RID: 4704
		[PublicizedFrom(EAccessModifier.Protected)]
		public EntityAlive entity;

		// Token: 0x04001261 RID: 4705
		[PublicizedFrom(EAccessModifier.Protected)]
		public DynamicProperties properties;

		// Token: 0x04001262 RID: 4706
		[PublicizedFrom(EAccessModifier.Protected)]
		public int belongsPlayerId;

		// Token: 0x04001263 RID: 4707
		[PublicizedFrom(EAccessModifier.Protected)]
		public float actionTime;

		// Token: 0x04001264 RID: 4708
		[PublicizedFrom(EAccessModifier.Protected)]
		public float cooldown = 1f;

		// Token: 0x04001265 RID: 4709
		[PublicizedFrom(EAccessModifier.Protected)]
		public float range = 10f;

		// Token: 0x04001266 RID: 4710
		[PublicizedFrom(EAccessModifier.Protected)]
		public EntityAlive target;

		// Token: 0x04001267 RID: 4711
		[PublicizedFrom(EAccessModifier.Private)]
		public Action onFireComplete;

		// Token: 0x04001268 RID: 4712
		[PublicizedFrom(EAccessModifier.Private)]
		public float cooldownTimer;
	}

	// Token: 0x0200039C RID: 924
	[Preserve]
	public class HealBeamWeapon : DroneWeapons.Weapon
	{
		// Token: 0x06001B98 RID: 7064 RVA: 0x000ACA51 File Offset: 0x000AAC51
		public HealBeamWeapon(EntityAlive _entity) : base(_entity)
		{
		}

		// Token: 0x06001B99 RID: 7065 RVA: 0x000ACA84 File Offset: 0x000AAC84
		public override void Init()
		{
			this.WeaponJoint = this.entity.transform.FindInChilds("WristLeft", false);
			if (this.properties.Values.ContainsKey("HealCooldown"))
			{
				float.TryParse(this.properties.Values["HealCooldown"], out this.cooldown);
			}
			if (this.properties.Values.ContainsKey("HealActionTime"))
			{
				float.TryParse(this.properties.Values["HealActionTime"], out this.actionTime);
			}
			if (this.properties.Values.ContainsKey("HealDamageThreshold"))
			{
				float.TryParse(this.properties.Values["HealDamageThreshold"], out this.HealDamageThreshold);
			}
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x000ACB58 File Offset: 0x000AAD58
		public override void Fire(EntityAlive _target)
		{
			base.Fire(_target);
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				ItemStack healingItemStack = this.getHealingItemStack();
				if (healingItemStack == null)
				{
					base.InvokeFireComplete();
					return;
				}
				EntityDrone entityDrone = this.entity as EntityDrone;
				entityDrone.SendSyncData(8);
				this.entity.inventory.SetItem(0, healingItemStack);
				this.entity.inventory.SetHoldingItemIdx(0);
				ItemAction itemAction = this.entity.inventory.holdingItem.Actions[1];
				ItemActionData actionData = this.entity.inventory.holdingItemData.actionData[1];
				if (itemAction != null)
				{
					itemAction.ExecuteAction(actionData, true);
				}
				EntityAlive owner = entityDrone.Owner;
				if (owner)
				{
					owner.Buffs.AddBuff("buffJunkDroneHealCooldownEffect", -1, true, false, -1f);
				}
				ParticleEffect pe = new ParticleEffect("drone_heal_beam", Vector3.zero, Quaternion.LookRotation(_target.getHeadPosition() - this.entity.position), 1f, Color.clear, null, this.entity.transform);
				Transform transform = base.SpawnDroneParticleEffect(pe, this.entity.entityId, DroneWeapons.NetPackageDroneParticleEffect.cActionType.Heal);
				if (transform && !GameManager.IsDedicatedServer)
				{
					transform.GetComponent<DroneBeamParticle>().SetDisplayTime(this.actionTime);
				}
				ParticleEffect pe2 = new ParticleEffect("drone_heal_player", Vector3.zero, Quaternion.identity, 1f, Color.clear, null, _target.transform);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(pe2, _target.entityId, false, false), false, -1, -1, -1, null, 192, false);
					if (!GameManager.IsDedicatedServer)
					{
						GameManager.Instance.SpawnParticleEffectClient(pe2, _target.entityId, false, false);
					}
				}
			}
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x000ACD21 File Offset: 0x000AAF21
		public override bool canFire()
		{
			return base.canFire() && this.hasHealingItem();
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x000ACD33 File Offset: 0x000AAF33
		public bool hasHealingItem()
		{
			return this._hasItem(this._healTypeToString(DroneWeapons.HealBeamWeapon.HealItemType.Bandage)) || this._hasItem(this._healTypeToString(DroneWeapons.HealBeamWeapon.HealItemType.FirstAidKit));
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x000ACD53 File Offset: 0x000AAF53
		public bool targetCanBeHealed(EntityAlive _target)
		{
			return _target.IsAlive() && !_target.Buffs.HasBuff("buffHealHealth") && _target.Health < _target.GetMaxHealth();
		}

		// Token: 0x06001B9E RID: 7070 RVA: 0x000ACD80 File Offset: 0x000AAF80
		public bool targetNeedsHealing(EntityAlive _target)
		{
			float num = (float)_target.GetMaxHealth();
			float modifiedMax = _target.Stats.Health.ModifiedMax;
			return this.targetCanBeHealed(_target) && ((num == modifiedMax && (float)_target.Health < num - this.HealDamageThreshold) || (float)_target.Health < modifiedMax * 0.67f);
		}

		// Token: 0x06001B9F RID: 7071 RVA: 0x000ACDD7 File Offset: 0x000AAFD7
		[PublicizedFrom(EAccessModifier.Private)]
		public string _healTypeToString(DroneWeapons.HealBeamWeapon.HealItemType healType)
		{
			return this._supportedHealingItems[(int)healType];
		}

		// Token: 0x06001BA0 RID: 7072 RVA: 0x000ACDE4 File Offset: 0x000AAFE4
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemStack getHealingItemStack()
		{
			DroneWeapons.HealBeamWeapon.HealItemType healType = DroneWeapons.HealBeamWeapon.HealItemType.Bandage;
			int num = this._hasItem(this._healTypeToString(DroneWeapons.HealBeamWeapon.HealItemType.Bandage)) ? 1 : 0;
			bool flag = this._hasItem(this._healTypeToString(DroneWeapons.HealBeamWeapon.HealItemType.FirstAidKit));
			if (num == 0 && flag)
			{
				healType = DroneWeapons.HealBeamWeapon.HealItemType.FirstAidKit;
			}
			ItemStack[] array = this.entity.bag.GetSlots();
			if (this.entity.lootContainer != null)
			{
				array = this.entity.lootContainer.GetItems();
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].itemValue != null && array[i].itemValue.ItemClass != null && array[i].itemValue.ItemClass.HasAnyTags(DroneWeapons.healingItemTags) && array[i].count > 0 && this._isItem(array[i].itemValue, this._healTypeToString(healType)))
				{
					ItemValue itemValue = array[i].itemValue.Clone();
					array[i].count--;
					if (array[i].count == 0)
					{
						array[i] = ItemStack.Empty.Clone();
					}
					this.entity.bag.SetSlots(array);
					this.entity.bag.OnUpdate();
					this.entity.lootContainer.UpdateSlot(i, array[i]);
					return new ItemStack(itemValue, 1);
				}
			}
			return null;
		}

		// Token: 0x06001BA1 RID: 7073 RVA: 0x000ACF34 File Offset: 0x000AB134
		[PublicizedFrom(EAccessModifier.Private)]
		public bool _hasItem(string itemGroupOrName)
		{
			ItemStack[] array = this.entity.bag.GetSlots();
			if (this.entity.lootContainer != null)
			{
				array = this.entity.lootContainer.GetItems();
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].itemValue != null && array[i].itemValue.ItemClass != null && array[i].itemValue.ItemClass.HasAnyTags(DroneWeapons.healingItemTags) && array[i].itemValue.ItemClass.Name.ContainsCaseInsensitive(itemGroupOrName))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001BA2 RID: 7074 RVA: 0x000ACFD2 File Offset: 0x000AB1D2
		[PublicizedFrom(EAccessModifier.Private)]
		public bool _isItem(ItemValue iv, string itemGroupOrName)
		{
			return iv.ItemClass.Name.ContainsCaseInsensitive(itemGroupOrName);
		}

		// Token: 0x04001269 RID: 4713
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cHealBeam = "drone_heal_beam";

		// Token: 0x0400126A RID: 4714
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cHealPlayer = "drone_heal_player";

		// Token: 0x0400126B RID: 4715
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cIdxHealing = 0;

		// Token: 0x0400126C RID: 4716
		public float HealDamageThreshold = 35f;

		// Token: 0x0400126D RID: 4717
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cAbrasionInjury = "buffInjuryAbrasion";

		// Token: 0x0400126E RID: 4718
		[PublicizedFrom(EAccessModifier.Private)]
		public const string cHealingBuff = "buffHealHealth";

		// Token: 0x0400126F RID: 4719
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cModifiedHealthCutoff = 0.67f;

		// Token: 0x04001270 RID: 4720
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] _supportedHealingItems = new string[]
		{
			"bandage",
			"medicalFirstAidKit"
		};

		// Token: 0x0200039D RID: 925
		[PublicizedFrom(EAccessModifier.Private)]
		public enum HealItemType
		{
			// Token: 0x04001272 RID: 4722
			Bandage,
			// Token: 0x04001273 RID: 4723
			FirstAidKit
		}
	}

	// Token: 0x0200039E RID: 926
	[Preserve]
	public class StunBeamWeapon : DroneWeapons.Weapon
	{
		// Token: 0x06001BA3 RID: 7075 RVA: 0x000ACFEA File Offset: 0x000AB1EA
		public StunBeamWeapon(EntityAlive _entity) : base(_entity)
		{
		}

		// Token: 0x06001BA4 RID: 7076 RVA: 0x000ACFF4 File Offset: 0x000AB1F4
		public override void Init()
		{
			this.WeaponJoint = this.entity.transform.FindInChilds("WristRight", false);
			if (this.properties.Values.ContainsKey("StunCooldown"))
			{
				float.TryParse(this.properties.Values["StunCooldown"], out this.cooldown);
			}
			if (this.properties.Values.ContainsKey("StunActionTime"))
			{
				float.TryParse(this.properties.Values["StunActionTime"], out this.actionTime);
			}
		}

		// Token: 0x06001BA5 RID: 7077 RVA: 0x000AD090 File Offset: 0x000AB290
		public override void Fire(EntityAlive _target)
		{
			base.Fire(_target);
			base.TargetApplyBuff("buffShocked");
			Manager.Play(this.entity, "drone_attackeffect", 1f, false);
			ParticleEffect pe = new ParticleEffect("nozzleflashuzi", this.WeaponJoint.position + Origin.position, Quaternion.Euler(0f, 180f, 0f), 1f, Color.white, "Electricity/Turret/turret_fire", this.WeaponJoint);
			base.SpawnParticleEffect(pe, -1);
			float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(this.entity.position)) / 2f;
			ParticleEffect pe2 = new ParticleEffect("nozzlesmokeuzi", this.WeaponJoint.position + Origin.position, lightValue, new Color(1f, 1f, 1f, 0.3f), null, this.WeaponJoint, false);
			base.SpawnParticleEffect(pe2, -1);
		}
	}

	// Token: 0x0200039F RID: 927
	[Preserve]
	public class MachineGunWeapon : DroneWeapons.Weapon
	{
		// Token: 0x06001BA6 RID: 7078 RVA: 0x000AD187 File Offset: 0x000AB387
		public MachineGunWeapon(EntityAlive _entity) : base(_entity)
		{
		}

		// Token: 0x06001BA7 RID: 7079 RVA: 0x000AD1B0 File Offset: 0x000AB3B0
		public override void Init()
		{
			this.damageMultiplier = new DamageMultiplier(this.properties, null);
			this.WeaponJoint = this.entity.transform.FindInChilds("WristRight", false);
			if (this.properties.Values.ContainsKey("MaxDistance"))
			{
				this.range = StringParsers.ParseFloat(this.properties.Values["MaxDistance"], 0, -1, NumberStyles.Any);
			}
			this.spreadHorizontal = new Vector2(-1f, 1f);
			this.spreadVertical = new Vector2(-1f, 1f);
			if (this.properties.Values.ContainsKey("RaySpread"))
			{
				float num = StringParsers.ParseFloat(this.properties.Values["RaySpread"], 0, -1, NumberStyles.Any);
				num *= 0.5f;
				this.spreadHorizontal = new Vector2(-num, num);
				this.spreadVertical = new Vector2(-num, num);
			}
			if (this.properties.Values.ContainsKey("RayCount"))
			{
				this.RayCount = (float)int.Parse(this.properties.Values["RayCount"]);
			}
			if (this.properties.Values.ContainsKey("BurstRoundCount"))
			{
				this.burstRoundCountMax = int.Parse(this.properties.Values["BurstRoundCount"]);
			}
			if (this.properties.Values.ContainsKey("BurstFireRate"))
			{
				this.burstFireRate = Mathf.Max(StringParsers.ParseFloat(this.properties.Values["BurstFireRate"], 0, -1, NumberStyles.Any), 0.1f);
			}
			this.actionTime = this.burstFireRate * (float)this.burstRoundCountMax;
			if (this.properties.Values.ContainsKey("CooldownTime"))
			{
				this.cooldown = StringParsers.ParseFloat(this.properties.Values["CooldownTime"], 0, -1, NumberStyles.Any);
			}
			if (this.properties.Values.ContainsKey("EntityDamage"))
			{
				this.entityDamage = int.Parse(this.properties.Values["EntityDamage"]);
			}
			this.buffActions = new List<string>();
			if (this.properties.Values.ContainsKey("Buff"))
			{
				string[] collection = this.properties.Values["Buff"].Replace(" ", "").Split(',', StringSplitOptions.None);
				this.buffActions.AddRange(collection);
			}
		}

		// Token: 0x06001BA8 RID: 7080 RVA: 0x000AD44C File Offset: 0x000AB64C
		public override void Update()
		{
			base.Update();
			if (this.target != null && !this.target.IsDead() && this.burstRoundCount < this.burstRoundCountMax && base.TimeRemaning > 0f && base.TimeRemaning < base.TimeLength - this.burstFireRate * (float)this.burstRoundCount)
			{
				this._fireWeapon();
			}
		}

		// Token: 0x06001BA9 RID: 7081 RVA: 0x000AD4B8 File Offset: 0x000AB6B8
		public override void Fire(EntityAlive _target)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return;
			}
			base.Fire(_target);
			this._fireWeapon();
		}

		// Token: 0x06001BAA RID: 7082 RVA: 0x000AD4D4 File Offset: 0x000AB6D4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnFireComplete()
		{
			base.OnFireComplete();
			this.burstRoundCount = 0;
		}

		// Token: 0x06001BAB RID: 7083 RVA: 0x000AD4E4 File Offset: 0x000AB6E4
		[PublicizedFrom(EAccessModifier.Private)]
		public void _fireWeapon()
		{
			EntityDrone entityDrone = this.entity as EntityDrone;
			Vector3 position = this.WeaponJoint.transform.position;
			Vector3 a = this.target.getChestPosition() - Origin.position;
			EntityAlive entity = GameManager.Instance.World.GetEntity(entityDrone.belongsPlayerId) as EntityAlive;
			GameRandom gameRandom = GameManager.Instance.World.GetGameRandom();
			FastTags<TagGroup.Global> itemTags = entityDrone.OriginalItemValue.ItemClass.ItemTags;
			int num = (int)EffectManager.GetValue(PassiveEffects.RoundRayCount, entityDrone.OriginalItemValue, this.RayCount, entity, null, entityDrone.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
			float value = EffectManager.GetValue(PassiveEffects.MaxRange, entityDrone.OriginalItemValue, this.range, entity, null, entityDrone.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
			for (int i = 0; i < num; i++)
			{
				Vector3 vector = (a - position).normalized;
				vector = Quaternion.Euler(gameRandom.RandomRange(this.spreadHorizontal.x, this.spreadHorizontal.y), gameRandom.RandomRange(this.spreadVertical.x, this.spreadVertical.y), 0f) * vector;
				Ray ray = new Ray(position + Origin.position, vector);
				int num2 = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.EntityPenetrationCount, entityDrone.OriginalItemValue, 0f, entity, null, entityDrone.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false));
				num2++;
				int num3 = Mathf.FloorToInt(EffectManager.GetValue(PassiveEffects.BlockPenetrationFactor, entityDrone.OriginalItemValue, 1f, entity, null, entityDrone.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false));
				EntityAlive x = null;
				for (int j = 0; j < num2; j++)
				{
					if (Voxel.Raycast(GameManager.Instance.World, ray, value, -538750997, 8, 0f))
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
								goto IL_334;
							}
							x = entityAlive;
						}
						else
						{
							j += Mathf.FloorToInt((float)ItemActionAttack.GetBlockHit(GameManager.Instance.World, worldRayHitInfo).Block.MaxDamage / (float)num3);
						}
						ItemActionAttack.Hit(worldRayHitInfo, entityDrone.belongsPlayerId, EnumDamageTypes.Piercing, this.GetDamageBlock(entityDrone.OriginalItemValue, BlockValue.Air, GameManager.Instance.World.GetEntity(entityDrone.belongsPlayerId) as EntityAlive, 1), this.GetDamageEntity(entityDrone.OriginalItemValue, GameManager.Instance.World.GetEntity(entityDrone.belongsPlayerId) as EntityAlive, 1), 1f, entityDrone.OriginalItemValue.PercentUsesLeft, 0f, 0f, "bullet", this.damageMultiplier, this.buffActions, new ItemActionAttack.AttackHitInfo(), 1, 0, 0f, null, null, ItemActionAttack.EnumAttackMode.RealNoHarvesting, null, entityDrone.entityId, entityDrone.OriginalItemValue);
					}
					IL_334:;
				}
			}
			ParticleEffect pe = new ParticleEffect("nozzleflashuzi", this.WeaponJoint.position + Origin.position, Quaternion.Euler(0f, 180f, 0f), 1f, Color.white, "Electricity/Turret/turret_fire", this.WeaponJoint);
			base.SpawnParticleEffect(pe, -1);
			float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(this.entity.position)) / 2f;
			ParticleEffect pe2 = new ParticleEffect("nozzlesmokeuzi", this.WeaponJoint.position + Origin.position, lightValue, new Color(1f, 1f, 1f, 0.3f), null, this.WeaponJoint, false);
			base.SpawnParticleEffect(pe2, -1);
			this.burstRoundCount++;
			if ((int)EffectManager.GetValue(PassiveEffects.MagazineSize, entityDrone.OriginalItemValue, 0f, null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0)
			{
				EntityDrone entityDrone2 = entityDrone;
				int ammoCount = entityDrone2.AmmoCount;
				entityDrone2.AmmoCount = ammoCount - 1;
			}
			entityDrone.OriginalItemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, entityDrone.OriginalItemValue, 1f, entity, null, entityDrone.OriginalItemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
		}

		// Token: 0x06001BAC RID: 7084 RVA: 0x000AD994 File Offset: 0x000ABB94
		[PublicizedFrom(EAccessModifier.Private)]
		public float GetDamageEntity(ItemValue _itemValue, EntityAlive _holdingEntity = null, int actionIndex = 0)
		{
			return EffectManager.GetValue(PassiveEffects.EntityDamage, _itemValue, (float)this.entityDamage, _holdingEntity, null, _itemValue.ItemClass.ItemTags, true, false, true, true, true, 1, true, false);
		}

		// Token: 0x06001BAD RID: 7085 RVA: 0x000AD9C4 File Offset: 0x000ABBC4
		[PublicizedFrom(EAccessModifier.Private)]
		public float GetDamageBlock(ItemValue _itemValue, BlockValue _blockValue, EntityAlive _holdingEntity = null, int actionIndex = 0)
		{
			this.tmpTag = _itemValue.ItemClass.ItemTags;
			this.tmpTag |= _blockValue.Block.Tags;
			float value = EffectManager.GetValue(PassiveEffects.BlockDamage, _itemValue, (float)this.blockDamage, _holdingEntity, null, this.tmpTag, true, false, true, true, true, 1, true, false);
			return Utils.FastMin((float)_blockValue.Block.blockMaterial.MaxIncomingDamage, value);
		}

		// Token: 0x04001274 RID: 4724
		[PublicizedFrom(EAccessModifier.Private)]
		public float RayCount = 1f;

		// Token: 0x04001275 RID: 4725
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2 spreadHorizontal;

		// Token: 0x04001276 RID: 4726
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2 spreadVertical;

		// Token: 0x04001277 RID: 4727
		[PublicizedFrom(EAccessModifier.Private)]
		public int burstRoundCountMax = 1;

		// Token: 0x04001278 RID: 4728
		[PublicizedFrom(EAccessModifier.Private)]
		public int burstRoundCount;

		// Token: 0x04001279 RID: 4729
		[PublicizedFrom(EAccessModifier.Private)]
		public const float burstFireRateMax = 0.1f;

		// Token: 0x0400127A RID: 4730
		[PublicizedFrom(EAccessModifier.Private)]
		public float burstFireRate = 1f;

		// Token: 0x0400127B RID: 4731
		[PublicizedFrom(EAccessModifier.Private)]
		public int entityDamage;

		// Token: 0x0400127C RID: 4732
		[PublicizedFrom(EAccessModifier.Private)]
		public int blockDamage;

		// Token: 0x0400127D RID: 4733
		[PublicizedFrom(EAccessModifier.Private)]
		public DamageMultiplier damageMultiplier;

		// Token: 0x0400127E RID: 4734
		[PublicizedFrom(EAccessModifier.Private)]
		public List<string> buffActions;

		// Token: 0x0400127F RID: 4735
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> tmpTag;
	}

	// Token: 0x020003A0 RID: 928
	[Preserve]
	public class NetPackageDroneParticleEffect : NetPackage
	{
		// Token: 0x06001BAE RID: 7086 RVA: 0x000ADA38 File Offset: 0x000ABC38
		public DroneWeapons.NetPackageDroneParticleEffect Setup(ParticleEffect _pe, int _entityThatCausedIt, DroneWeapons.NetPackageDroneParticleEffect.cActionType _actionType)
		{
			this.pe = _pe;
			this.entityThatCausedIt = _entityThatCausedIt;
			this.actionType = _actionType;
			return this;
		}

		// Token: 0x06001BAF RID: 7087 RVA: 0x000ADA50 File Offset: 0x000ABC50
		public override void read(PooledBinaryReader _br)
		{
			this.pe = new ParticleEffect();
			this.pe.Read(_br);
			this.entityThatCausedIt = _br.ReadInt32();
		}

		// Token: 0x06001BB0 RID: 7088 RVA: 0x000ADA75 File Offset: 0x000ABC75
		public override void write(PooledBinaryWriter _bw)
		{
			base.write(_bw);
			this.pe.Write(_bw);
			_bw.Write(this.entityThatCausedIt);
		}

		// Token: 0x06001BB1 RID: 7089 RVA: 0x000ADA98 File Offset: 0x000ABC98
		public override void ProcessPackage(World _world, GameManager _callbacks)
		{
			if (_world == null)
			{
				return;
			}
			if (!_world.IsRemote())
			{
				_world.GetGameManager().SpawnParticleEffectServer(this.pe, this.entityThatCausedIt, false, false);
				return;
			}
			Transform transform = _world.GetGameManager().SpawnParticleEffectClientForceCreation(this.pe, this.entityThatCausedIt, false);
			if (transform != null)
			{
				EntityDrone entityDrone = _world.GetEntity(this.entityThatCausedIt) as EntityDrone;
				if (entityDrone != null)
				{
					DroneBeamParticle component = transform.GetComponent<DroneBeamParticle>();
					if (this.actionType == DroneWeapons.NetPackageDroneParticleEffect.cActionType.Attack)
					{
						transform.parent = entityDrone.stunWeapon.WeaponJoint;
						component.SetDisplayTime(entityDrone.AttackActionTime);
					}
					else if (this.actionType == DroneWeapons.NetPackageDroneParticleEffect.cActionType.Heal)
					{
						transform.parent = entityDrone.healWeapon.WeaponJoint;
						component.SetDisplayTime(entityDrone.HealActionTime);
					}
					transform.localPosition = Vector3.zero;
					transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x06001BB2 RID: 7090 RVA: 0x000ADB75 File Offset: 0x000ABD75
		public override int GetLength()
		{
			return 20;
		}

		// Token: 0x04001280 RID: 4736
		[PublicizedFrom(EAccessModifier.Private)]
		public ParticleEffect pe;

		// Token: 0x04001281 RID: 4737
		[PublicizedFrom(EAccessModifier.Private)]
		public int entityThatCausedIt;

		// Token: 0x04001282 RID: 4738
		[PublicizedFrom(EAccessModifier.Private)]
		public DroneWeapons.NetPackageDroneParticleEffect.cActionType actionType;

		// Token: 0x020003A1 RID: 929
		public enum cActionType
		{
			// Token: 0x04001284 RID: 4740
			Attack,
			// Token: 0x04001285 RID: 4741
			Heal
		}
	}
}
