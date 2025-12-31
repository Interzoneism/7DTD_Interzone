using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x0200019D RID: 413
public class EntityStats
{
	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000C9F RID: 3231 RVA: 0x000560BB File Offset: 0x000542BB
	public float AmountEnclosed
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_amountEnclosed;
		}
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x000560C3 File Offset: 0x000542C3
	public EntityStats()
	{
		this.Health = new Stat();
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x000560DE File Offset: 0x000542DE
	public EntityStats(EntityAlive _ea)
	{
		this.m_entity = _ea;
		this.Init();
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x000560FC File Offset: 0x000542FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Init()
	{
		int num = (int)EffectManager.GetValue(PassiveEffects.HealthMax, null, 100f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.Health = new Stat(Stat.StatTypes.Health, this.m_entity, (float)num, (float)num)
		{
			MaxPassive = PassiveEffects.HealthMax,
			GainPassive = PassiveEffects.HealthGain,
			LossPassive = PassiveEffects.HealthLoss
		};
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0005615E File Offset: 0x0005435E
	public virtual void CopyFrom(EntityStats _newStats)
	{
		this.Health.CopyFrom(_newStats.Health);
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x00056171 File Offset: 0x00054371
	public virtual EntityStats SimpleClone()
	{
		EntityStats entityStats = new EntityStats();
		entityStats.Health.CopyFrom(this.Health);
		return entityStats;
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void EntityBuffAdded(BuffValue _buff)
	{
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void EntityBuffRemoved(BuffValue _buff)
	{
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0005618C File Offset: 0x0005438C
	public void Tick(ulong worldTime)
	{
		if (this.m_entity.isEntityRemote || this.m_entity.IsDead())
		{
			return;
		}
		int num = this.waitTicks + 1;
		this.waitTicks = num;
		if (num >= 10)
		{
			this.waitTicks = 0;
		}
		this.TickWait(worldTime);
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x000561D8 File Offset: 0x000543D8
	public virtual void TickWait(ulong worldTime)
	{
		float dt = 0.5f;
		if (this.waitTicks == 1)
		{
			this.UpdateNPCStatsOverTime(dt);
			this.Health.Tick(dt);
		}
		if (this.waitTicks == 2 && this.Health.Changed)
		{
			this.SendStatChangePacket(NetPackageEntityStatChanged.EnumStat.Health);
			this.Health.Changed = false;
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

	// Token: 0x06000CA9 RID: 3241 RVA: 0x000562B0 File Offset: 0x000544B0
	public void UpdateNPCStatsOverTime(float dt)
	{
		List<EffectManager.ModifierValuesAndSources> valuesAndSources = EffectManager.GetValuesAndSources(PassiveEffects.HealthChangeOT, null, 0f, this.m_entity, null, default(FastTags<TagGroup.Global>), true, true);
		for (int i = 0; i < valuesAndSources.Count; i++)
		{
			EffectManager.ModifierValuesAndSources modifierValuesAndSources = valuesAndSources[i];
			if (modifierValuesAndSources.ParentType == MinEffectController.SourceParentType.BuffClass)
			{
				BuffValue buff = this.m_entity.Buffs.GetBuff((string)modifierValuesAndSources.Source);
				if (buff != null && buff.BuffClass != null)
				{
					BuffClass buffClass = buff.BuffClass;
					float num = 0f;
					float num2 = 1f;
					buffClass.ModifyValue(this.m_entity, PassiveEffects.HealthChangeOT, buff, ref num, ref num2, FastTags<TagGroup.Global>.none);
					float num3 = num * num2 * dt;
					if (num3 < 0f)
					{
						float num4 = -num3 + this.buffDamageRemainder;
						int num5 = (int)num4;
						this.buffDamageRemainder = num4 - (float)num5;
						if (num5 > 0)
						{
							DamageSource damageSource = new DamageSource(buffClass.DamageSource, buffClass.DamageType);
							damageSource.BuffClass = buffClass;
							this.m_entity.DamageEntity(damageSource, num5, false, 0f);
						}
					}
					else if (num3 > 0f)
					{
						this.Health.Value += num3;
					}
				}
			}
			else
			{
				this.Health.Value += modifierValuesAndSources.Value * dt;
			}
		}
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x00002914 File Offset: 0x00000B14
	public void ResetStats()
	{
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0005640C File Offset: 0x0005460C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendStatChangePacket(NetPackageEntityStatChanged.EnumStat enumStat)
	{
		int num;
		if (GameManager.IsDedicatedServer)
		{
			num = -1;
		}
		else
		{
			num = this.m_entity.world.GetPrimaryPlayer().entityId;
		}
		NetPackageEntityStatChanged package = NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(this.m_entity, num, enumStat);
		if (this.m_entity.world.IsRemote())
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		this.m_entity.world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(this.m_entity.entityId, num, package, enumStat > NetPackageEntityStatChanged.EnumStat.Health);
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x00056494 File Offset: 0x00054694
	public virtual void Read(BinaryReader stream)
	{
		int num = stream.ReadInt32();
		if (num < 10)
		{
			stream.ReadInt32();
		}
		this.Health.Read(stream);
		if (num < 10)
		{
			Stat stat = new Stat(Stat.StatTypes.Health, null, 0f, 0f);
			stat.Read(stream);
			stat.Read(stream);
			stat.Read(stream);
			stat.Read(stream);
			stream.ReadSingle();
		}
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x000564F6 File Offset: 0x000546F6
	public virtual void Write(BinaryWriter stream)
	{
		stream.Write(10);
		this.Health.Write(stream);
	}

	// Token: 0x04000A78 RID: 2680
	public const int cVersion = 10;

	// Token: 0x04000A79 RID: 2681
	public static bool WeatherSurvivalEnabled;

	// Token: 0x04000A7A RID: 2682
	public static bool NewWeatherSurvivalEnabled = true;

	// Token: 0x04000A7B RID: 2683
	public Stat Health;

	// Token: 0x04000A7C RID: 2684
	public Stat Stamina;

	// Token: 0x04000A7D RID: 2685
	public Stat Water;

	// Token: 0x04000A7E RID: 2686
	public Stat Food;

	// Token: 0x04000A7F RID: 2687
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAlive m_entity;

	// Token: 0x04000A80 RID: 2688
	[PublicizedFrom(EAccessModifier.Protected)]
	public float m_amountEnclosed;

	// Token: 0x04000A81 RID: 2689
	[PublicizedFrom(EAccessModifier.Private)]
	public float buffDamageRemainder;

	// Token: 0x04000A82 RID: 2690
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int cWaitTicks = 10;

	// Token: 0x04000A83 RID: 2691
	[PublicizedFrom(EAccessModifier.Protected)]
	public int waitTicks;

	// Token: 0x04000A84 RID: 2692
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int cNetSyncWaitTicks = 10;

	// Token: 0x04000A85 RID: 2693
	[PublicizedFrom(EAccessModifier.Protected)]
	public int netSyncWaitTicks = 10;
}
