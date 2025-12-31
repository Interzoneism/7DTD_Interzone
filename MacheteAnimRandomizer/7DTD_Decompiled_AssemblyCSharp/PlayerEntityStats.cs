using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class PlayerEntityStats : EntityStats
{
	// Token: 0x06000CAF RID: 3247 RVA: 0x00056514 File Offset: 0x00054714
	public PlayerEntityStats()
	{
		this.Stamina = new Stat();
		this.Water = new Stat();
		this.Food = new Stat();
		this.CoreTemp = new Stat();
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x00056565 File Offset: 0x00054765
	public PlayerEntityStats(EntityAlive _ea) : base(_ea)
	{
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x00056580 File Offset: 0x00054780
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Init()
	{
		int num = (int)EffectManager.GetValue(PassiveEffects.HealthMax, null, 100f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Health = new Stat(Stat.StatTypes.Health, this.m_entity, (float)num, (float)num)
		{
			GainPassive = PassiveEffects.HealthGain,
			MaxPassive = PassiveEffects.HealthMax,
			LossPassive = PassiveEffects.HealthLoss
		};
		int num2 = (int)EffectManager.GetValue(PassiveEffects.StaminaMax, null, 100f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Stamina = new Stat(Stat.StatTypes.Stamina, this.m_entity, (float)num2, (float)num2)
		{
			GainPassive = PassiveEffects.StaminaGain,
			MaxPassive = PassiveEffects.StaminaMax,
			LossPassive = PassiveEffects.StaminaLoss
		};
		int num3 = (int)EffectManager.GetValue(PassiveEffects.WaterMax, null, 100f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Water = new Stat(Stat.StatTypes.Water, this.m_entity, (float)num3, (float)num3)
		{
			GainPassive = PassiveEffects.WaterGain,
			MaxPassive = PassiveEffects.WaterMax,
			LossPassive = PassiveEffects.WaterLoss
		};
		int num4 = (int)EffectManager.GetValue(PassiveEffects.FoodMax, null, 100f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Food = new Stat(Stat.StatTypes.Food, this.m_entity, (float)num4, (float)num4)
		{
			GainPassive = PassiveEffects.FoodGain,
			MaxPassive = PassiveEffects.FoodMax,
			LossPassive = PassiveEffects.FoodLoss
		};
		this.CoreTemp = new Stat(Stat.StatTypes.CoreTemp, this.m_entity, -200f, 200f);
		this.buffChangedDelegates = new List<IEntityBuffsChanged>();
		this.notificationChangedDelegates = new List<IEntityUINotificationChanged>();
		this.m_notifications = new List<EntityUINotification>();
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x00056724 File Offset: 0x00054924
	public override void CopyFrom(EntityStats _stats)
	{
		PlayerEntityStats playerEntityStats = (PlayerEntityStats)_stats;
		this.Health.CopyFrom(playerEntityStats.Health);
		this.Stamina.CopyFrom(playerEntityStats.Stamina);
		this.Water.CopyFrom(playerEntityStats.Water);
		this.Food.CopyFrom(playerEntityStats.Food);
		this.CoreTemp.CopyFrom(playerEntityStats.CoreTemp);
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x00056790 File Offset: 0x00054990
	public override EntityStats SimpleClone()
	{
		PlayerEntityStats playerEntityStats = new PlayerEntityStats();
		playerEntityStats.Health.CopyFrom(this.Health);
		playerEntityStats.Stamina.CopyFrom(this.Stamina);
		playerEntityStats.Water.CopyFrom(this.Water);
		playerEntityStats.Food.CopyFrom(this.Food);
		playerEntityStats.CoreTemp.CopyFrom(this.CoreTemp);
		return playerEntityStats;
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x000567F7 File Offset: 0x000549F7
	public void AddUINotificationChangedDelegate(IEntityUINotificationChanged _uiChangedDelegate)
	{
		if (!this.notificationChangedDelegates.Contains(_uiChangedDelegate))
		{
			this.notificationChangedDelegates.Add(_uiChangedDelegate);
		}
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x00056813 File Offset: 0x00054A13
	public void RemoveUINotificationChangedDelegate(IEntityUINotificationChanged _uiChangedDelegate)
	{
		this.notificationChangedDelegates.Remove(_uiChangedDelegate);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x00056822 File Offset: 0x00054A22
	public void AddBuffChangedDelegate(IEntityBuffsChanged _buffChangedDelegate)
	{
		if (!this.buffChangedDelegates.Contains(_buffChangedDelegate))
		{
			this.buffChangedDelegates.Add(_buffChangedDelegate);
		}
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0005683E File Offset: 0x00054A3E
	public void RemoveBuffChangedDelegate(IEntityBuffsChanged _buffChangedDelegate)
	{
		this.buffChangedDelegates.Remove(_buffChangedDelegate);
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x00056850 File Offset: 0x00054A50
	public override void EntityBuffAdded(BuffValue _buff)
	{
		for (int i = 0; i < this.buffChangedDelegates.Count; i++)
		{
			this.buffChangedDelegates[i].EntityBuffAdded(_buff);
		}
		BuffClass buffClass = _buff.BuffClass;
		if (!buffClass.Hidden && buffClass.Icon != null)
		{
			BuffEntityUINotification buffEntityUINotification = new BuffEntityUINotification(this.m_entity, _buff);
			this.m_notifications.Add(buffEntityUINotification);
			for (int j = 0; j < this.notificationChangedDelegates.Count; j++)
			{
				this.notificationChangedDelegates[j].EntityUINotificationAdded(buffEntityUINotification);
			}
		}
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x000568E0 File Offset: 0x00054AE0
	public override void EntityBuffRemoved(BuffValue _buff)
	{
		for (int i = 0; i < this.buffChangedDelegates.Count; i++)
		{
			this.buffChangedDelegates[i].EntityBuffRemoved(_buff);
		}
		int j = 0;
		while (j < this.m_notifications.Count)
		{
			EntityUINotification entityUINotification = this.m_notifications[j];
			if (entityUINotification.Buff == _buff)
			{
				this.m_notifications.RemoveAt(j);
				for (int k = 0; k < this.notificationChangedDelegates.Count; k++)
				{
					this.notificationChangedDelegates[k].EntityUINotificationRemoved(entityUINotification);
				}
			}
			else
			{
				j++;
			}
		}
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x06000CBA RID: 3258 RVA: 0x00056977 File Offset: 0x00054B77
	public List<EntityUINotification> Notifications
	{
		get
		{
			return this.m_notifications;
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x06000CBB RID: 3259 RVA: 0x00056980 File Offset: 0x00054B80
	public int LocalPlayerId
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_localPlayer == null)
			{
				this.m_localPlayer = this.m_entity.world.GetPrimaryPlayer();
				if (this.m_localPlayer != null)
				{
					this.m_localPlayerId = this.m_localPlayer.entityId;
				}
			}
			return this.m_localPlayerId;
		}
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x06000CBC RID: 3260 RVA: 0x000569D6 File Offset: 0x00054BD6
	// (set) Token: 0x06000CBD RID: 3261 RVA: 0x000569DE File Offset: 0x00054BDE
	public bool Shaded
	{
		get
		{
			return this.m_isInShade;
		}
		set
		{
			this.m_isInShade = value;
		}
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x000569E8 File Offset: 0x00054BE8
	public override void TickWait(ulong worldTime)
	{
		float dt = 0.5f;
		if (this.waitTicks == 1)
		{
			this.UpdateWeatherStats(dt, worldTime, this.m_entity.IsGodMode.Value);
		}
		if (this.waitTicks == 2)
		{
			this.UpdatePlayerFoodOT(dt);
			this.UpdatePlayerWaterOT(dt);
		}
		if (this.waitTicks == 3)
		{
			this.UpdatePlayerHealthOT(dt);
		}
		if (this.waitTicks == 4)
		{
			this.UpdatePlayerStaminaOT(dt);
		}
		if (this.waitTicks == 5)
		{
			if (this.Health.Changed)
			{
				this.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Health);
				this.Health.Changed = false;
			}
			if (this.Stamina.Changed)
			{
				this.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Stamina);
				this.Stamina.Changed = false;
			}
			if (this.Water.Changed)
			{
				this.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Water);
				this.Water.Changed = false;
			}
			if (this.Food.Changed)
			{
				this.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Food);
				this.Food.Changed = false;
			}
		}
		if (this.waitTicks == 6)
		{
			if (this.netSyncWaitTicks > 0)
			{
				this.netSyncWaitTicks--;
				return;
			}
			this.netSyncWaitTicks = 10;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStatsBuff>().Setup(this.m_entity, null), false, -1, -1, -1, null, 192, false);
				return;
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEntityStatsBuff>().Setup(this.m_entity, null), false);
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x00056B60 File Offset: 0x00054D60
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateWeatherStats(float dt, ulong worldTime, bool godMode)
	{
		this.m_amountEnclosed = this.m_entity.GetAmountEnclosed();
		this.LightInsidePer = this.m_amountEnclosed;
		EntityAlive entity = this.m_entity;
		EntityBuffs buffs = entity.Buffs;
		float wetnessRate = entity.GetWetnessRate();
		buffs.SetCustomVar("_wetnessrate", wetnessRate, true, CVarOperation.set);
		if (!EntityStats.WeatherSurvivalEnabled || WeatherManager.inWeatherGracePeriod || entity.IsGodMode.Value || entity.biomeStandingOn == null || buffs.HasBuff("god"))
		{
			buffs.SetCustomVar("_sheltered", this.m_amountEnclosed, true, CVarOperation.set);
			buffs.SetCustomVar("_shaded", (float)(this.IsShaded() ? 1 : 0), true, CVarOperation.set);
			buffs.SetCustomVar("_degreesabsorbed", 0f, true, CVarOperation.set);
			buffs.SetCustomVar("_coretemp", 0f, true, CVarOperation.set);
			return;
		}
		this.m_isInShade = this.IsShaded();
		float num = this.GetOutsideTemperature();
		num -= 10f * wetnessRate;
		float value;
		if (num < 70f)
		{
			value = EffectManager.GetValue(PassiveEffects.HypothermalResist, null, 0f, entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			num = Mathf.Min(70f, num + value);
		}
		else
		{
			value = EffectManager.GetValue(PassiveEffects.HyperthermalResist, null, 0f, entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			num = Mathf.Max(70f, num - value);
		}
		if ((int)this.lastCoreTemp < (int)num)
		{
			this.lastCoreTemp += 1f;
		}
		else if ((int)this.lastCoreTemp > (int)num)
		{
			this.lastCoreTemp -= 1f;
		}
		buffs.SetCustomVar("_degreesabsorbed", value, true, CVarOperation.set);
		buffs.SetCustomVar("_coretemp", this.lastCoreTemp - 70f, true, CVarOperation.set);
		buffs.SetCustomVar("_sheltered", this.m_amountEnclosed, true, CVarOperation.set);
		buffs.SetCustomVar("_shaded", (float)(this.m_isInShade ? 1 : 0), true, CVarOperation.set);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x00056D50 File Offset: 0x00054F50
	public void UpdatePlayerFoodOT(float dt)
	{
		this.Food.RegenerationAmount += EffectManager.GetValue(PassiveEffects.FoodChangeOT, null, 0f, this.m_entity, null, this.m_entity.CurrentMovementTag, true, true, true, true, true, 1, true, false) * dt;
		this.Food.MaxModifier = -EffectManager.GetValue(PassiveEffects.FoodMaxBlockage, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Food.Tick(dt);
		this.Food.RegenerationAmount = 0f;
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00056DE8 File Offset: 0x00054FE8
	public void UpdatePlayerWaterOT(float dt)
	{
		this.Water.RegenerationAmount += EffectManager.GetValue(PassiveEffects.WaterChangeOT, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * dt;
		this.Water.MaxModifier = -EffectManager.GetValue(PassiveEffects.WaterMaxBlockage, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Water.Tick(dt);
		this.Water.RegenerationAmount = 0f;
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x00056E80 File Offset: 0x00055080
	public void UpdatePlayerHealthOT(float dt)
	{
		float num = EffectManager.GetValue(PassiveEffects.HealthChangeOT, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (this.Health.ValuePercent < 1f && num > 0f)
		{
			float waterPercent = this.GetWaterPercent();
			this.Health.RegenerationAmount = num * waterPercent * dt;
		}
		else if (num < 0f)
		{
			List<EffectManager.ModifierValuesAndSources> valuesAndSources = EffectManager.GetValuesAndSources(PassiveEffects.HealthChangeOT, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true);
			float num2 = 0f;
			float num3 = 1f;
			for (int i = 0; i < valuesAndSources.Count; i++)
			{
				if (valuesAndSources[i].ParentType == MinEffectController.SourceParentType.BuffClass)
				{
					BuffValue buff = this.m_entity.Buffs.GetBuff((string)valuesAndSources[i].Source);
					if (buff != null && buff.BuffClass != null && !buff.Remove)
					{
						BuffClass buffClass = buff.BuffClass;
						num2 = 0f;
						num3 = 1f;
						buffClass.ModifyValue(this.m_entity, PassiveEffects.HealthChangeOT, buff, ref num2, ref num3, FastTags<TagGroup.Global>.none);
						num = num2 * num3 * dt;
						if (num < 0f)
						{
							DamageSourceEntity damageSourceEntity = new DamageSourceEntity(buffClass.DamageSource, buffClass.DamageType, buff.InstigatorId);
							damageSourceEntity.BuffClass = buffClass;
							this.m_entity.DamageEntity(damageSourceEntity, (int)(-num + 0.5f), false, 0f);
						}
					}
				}
				else
				{
					this.Health.RegenerationAmount = valuesAndSources[i].Value * dt;
				}
			}
		}
		this.Health.MaxModifier = -EffectManager.GetValue(PassiveEffects.HealthMaxBlockage, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Health.Tick(dt);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x00057070 File Offset: 0x00055270
	public void UpdatePlayerStaminaOT(float dt)
	{
		float value = EffectManager.GetValue(PassiveEffects.StaminaChangeOT, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		if (this.Stamina.ValuePercent < 1f && value > 0f)
		{
			this.Stamina.RegenerationAmount = value * dt;
		}
		else if (value < 0f)
		{
			this.Stamina.RegenerationAmount = value * dt;
		}
		this.Stamina.RegenerationAmount = EffectManager.GetValue(PassiveEffects.StaminaChangeOT, null, this.Stamina.RegenerationAmount, this.m_entity, null, this.m_entity.CurrentMovementTag | this.m_entity.CurrentStanceTag, true, true, true, true, true, 1, true, false) * dt;
		if (this.Stamina.RegenerationAmount > 0f)
		{
			float waterPercent = this.GetWaterPercent();
			this.Stamina.RegenerationAmount *= waterPercent;
		}
		this.Stamina.MaxModifier = -EffectManager.GetValue(PassiveEffects.StaminaMaxBlockage, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Stamina.Tick(dt);
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x00057198 File Offset: 0x00055398
	[PublicizedFrom(EAccessModifier.Private)]
	public float AdjustTemperatureFromEnclosure(float _temperature)
	{
		if (_temperature < 70f)
		{
			if (_temperature + 30f < 70f)
			{
				_temperature = (_temperature + 30f) * this.m_amountEnclosed + _temperature * (1f - this.m_amountEnclosed);
			}
			else
			{
				_temperature = 70f * this.m_amountEnclosed + _temperature * (1f - this.m_amountEnclosed);
			}
		}
		else if (_temperature - 30f > 70f)
		{
			_temperature = (_temperature - 30f) * this.m_amountEnclosed + _temperature * (1f - this.m_amountEnclosed);
		}
		else
		{
			_temperature = 70f * this.m_amountEnclosed + _temperature * (1f - this.m_amountEnclosed);
		}
		return _temperature;
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x00057248 File Offset: 0x00055448
	public float GetOutsideTemperature()
	{
		float num = WeatherManager.Instance.GetCurrentTemperatureValue();
		if (!this.m_isInShade)
		{
			if (WeatherManager.Instance.GetCurrentRainfallPercent() > 0.25f && num > 70f)
			{
				num -= 10f;
			}
			WeatherManager.Instance.GetCurrentCloudThicknessPercent();
		}
		else if (num > 70f)
		{
			num -= 8f;
		}
		else
		{
			num += 8f;
		}
		return this.AdjustTemperatureFromEnclosure(num);
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x000572B8 File Offset: 0x000554B8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsShaded()
	{
		Vector3 sunLightDirection = SkyManager.GetSunLightDirection();
		if (sunLightDirection.y > -0.25f)
		{
			return true;
		}
		Ray ray = new Ray(this.m_entity.getHeadPosition() - Origin.position + sunLightDirection * 0.5f, -sunLightDirection);
		bool result = false;
		RaycastHit raycastHit;
		if (Physics.SphereCast(ray, 0.5f, out raycastHit, float.PositiveInfinity, 65809))
		{
			result = (raycastHit.distance < float.PositiveInfinity);
		}
		return result;
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x00057334 File Offset: 0x00055534
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetWaterPercent()
	{
		float num = this.Water.ValuePercentUI * (this.Water.Max * 0.01f);
		if (num != 0f)
		{
			if (num < 0.25f)
			{
				num = 0.25f;
			}
			else if (num < 0.5f)
			{
				num = 0.5f;
			}
			else
			{
				num = 1f;
			}
		}
		return num;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x00057390 File Offset: 0x00055590
	[PublicizedFrom(EAccessModifier.Private)]
	public new void SendStatChangePacket(NetPackageEntityStatChanged.EnumStat enumStat)
	{
		NetPackageEntityStatChanged package = NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(this.m_entity, this.LocalPlayerId, enumStat);
		if (this.m_entity.world.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		this.m_entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.m_entity.entityId, -1, package, enumStat > NetPackageEntityStatChanged.EnumStat.Health);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x000573FC File Offset: 0x000555FC
	public override void Read(BinaryReader stream)
	{
		int num = stream.ReadInt32();
		if (num < 10)
		{
			stream.ReadInt32();
		}
		this.Health.Read(stream);
		this.Stamina.Read(stream);
		this.CoreTemp.Read(stream);
		this.Water.Read(stream);
		this.Food.Read(stream);
		if (num < 10)
		{
			stream.ReadSingle();
		}
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x00057464 File Offset: 0x00055664
	public override void Write(BinaryWriter stream)
	{
		stream.Write(10);
		this.Health.Write(stream);
		this.Stamina.Write(stream);
		this.CoreTemp.Write(stream);
		this.Water.Write(stream);
		this.Food.Write(stream);
	}

	// Token: 0x04000A86 RID: 2694
	[PublicizedFrom(EAccessModifier.Private)]
	public List<IEntityBuffsChanged> buffChangedDelegates;

	// Token: 0x04000A87 RID: 2695
	[PublicizedFrom(EAccessModifier.Private)]
	public List<IEntityUINotificationChanged> notificationChangedDelegates;

	// Token: 0x04000A88 RID: 2696
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityUINotification> m_notifications;

	// Token: 0x04000A89 RID: 2697
	public Stat CoreTemp;

	// Token: 0x04000A8A RID: 2698
	public float LightInsidePer;

	// Token: 0x04000A8B RID: 2699
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer m_localPlayer;

	// Token: 0x04000A8C RID: 2700
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_localPlayerId = -1;

	// Token: 0x04000A8D RID: 2701
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_isInShade;

	// Token: 0x04000A8E RID: 2702
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastCoreTemp = 70f;

	// Token: 0x04000A8F RID: 2703
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRaycastMask = 65809;
}
