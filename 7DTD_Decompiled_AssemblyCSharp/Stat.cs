using System;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x020001A9 RID: 425
public sealed class Stat
{
	// Token: 0x06000CF6 RID: 3318 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public Stat()
	{
	}

	// Token: 0x06000CF7 RID: 3319 RVA: 0x00057D64 File Offset: 0x00055F64
	public Stat(Stat.StatTypes _statType, EntityAlive _entity, float _value, float _baseMax)
	{
		this.StatType = _statType;
		this.Entity = _entity;
		this.m_value = _value;
		this.m_originalValue = _value;
		this.m_lastValue = _value;
		this.m_baseMax = _baseMax;
		this.m_originalBaseMax = _baseMax;
		this.m_maxModifier = 0f;
		this.m_changed = false;
	}

	// Token: 0x06000CF8 RID: 3320 RVA: 0x00057DBC File Offset: 0x00055FBC
	public void Tick(float dt)
	{
		if (this.MaxPassive != PassiveEffects.None)
		{
			this.BaseMax = EffectManager.GetValue(this.MaxPassive, null, this.m_originalBaseMax, this.Entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		if ((this.StatType == Stat.StatTypes.Health || this.StatType == Stat.StatTypes.Stamina) && Utils.FastAbs(this.m_lastValue - this.m_value) >= 1f)
		{
			if (this.m_value > this.m_lastValue && this.GainPassive != PassiveEffects.None)
			{
				this.m_value = Utils.FastClamp(this.m_lastValue + EffectManager.GetValue(this.GainPassive, null, this.m_value - this.m_lastValue, this.Entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false), 0f, this.m_baseMax);
			}
			else if (this.m_value < this.m_lastValue && this.LossPassive != PassiveEffects.None)
			{
				this.m_value = Utils.FastClamp(this.m_lastValue - EffectManager.GetValue(this.LossPassive, null, this.m_lastValue - this.m_value, this.Entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false), 0f, this.m_baseMax);
			}
		}
		if (this.m_value + this.regenAmount > this.ModifiedMax)
		{
			this.regenAmount = this.ModifiedMax - this.m_value;
		}
		this.RegenerationAmountUI = this.m_value - this.m_lastValue + this.regenAmount / dt;
		this.m_value += this.regenAmount;
		if (this.regenAmount > 0f)
		{
			if (this.StatType == Stat.StatTypes.Stamina)
			{
				this.Entity.Stats.Water.RegenerationAmount -= this.regenAmount * EffectManager.GetValue(PassiveEffects.WaterLossPerStaminaPointGained, null, 1f, this.Entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				this.Entity.Stats.Food.RegenerationAmount -= this.regenAmount * EffectManager.GetValue(PassiveEffects.FoodLossPerStaminaPointGained, null, 1f, this.Entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
			else if (this.StatType == Stat.StatTypes.Health)
			{
				this.Entity.Stats.Water.RegenerationAmount -= this.regenAmount * EffectManager.GetValue(PassiveEffects.WaterLossPerHealthPointGained, null, 1f, this.Entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
				this.Entity.Stats.Food.RegenerationAmount -= this.regenAmount * EffectManager.GetValue(PassiveEffects.FoodLossPerHealthPointGained, null, 1f, this.Entity, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			}
		}
		this.regenAmount = this.m_value - this.m_lastValue;
		this.SetChangedFlag(this.m_value, this.m_lastValue);
		this.m_lastValue = this.m_value;
	}

	// Token: 0x06000CF9 RID: 3321 RVA: 0x000580CF File Offset: 0x000562CF
	public void ResetValue()
	{
		this.m_value = this.m_originalValue;
		this.m_baseMax = this.m_originalBaseMax;
		this.m_maxModifier = 0f;
		this.m_changed = true;
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x000580FC File Offset: 0x000562FC
	public void CopyFrom(Stat _stat)
	{
		this.m_value = _stat.m_value;
		this.m_originalValue = _stat.m_originalValue;
		this.m_baseMax = _stat.m_baseMax;
		this.m_originalBaseMax = _stat.m_originalBaseMax;
		this.m_maxModifier = _stat.m_maxModifier;
		this.m_changed = false;
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x06000CFB RID: 3323 RVA: 0x0005814C File Offset: 0x0005634C
	// (set) Token: 0x06000CFC RID: 3324 RVA: 0x00058154 File Offset: 0x00056354
	public float RegenerationAmount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.regenAmount;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			this.regenAmount = value;
		}
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x06000CFD RID: 3325 RVA: 0x0005815D File Offset: 0x0005635D
	// (set) Token: 0x06000CFE RID: 3326 RVA: 0x00058165 File Offset: 0x00056365
	public float RegenerationAmountUI { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x06000CFF RID: 3327 RVA: 0x0005816E File Offset: 0x0005636E
	public float Max
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_baseMax;
		}
	}

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x06000D00 RID: 3328 RVA: 0x0005816E File Offset: 0x0005636E
	// (set) Token: 0x06000D01 RID: 3329 RVA: 0x00058176 File Offset: 0x00056376
	public float BaseMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_baseMax;
		}
		set
		{
			if (this.m_baseMax != value)
			{
				this.SetChangedFlag(this.m_baseMax, value);
				this.m_baseMax = value;
			}
		}
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x06000D02 RID: 3330 RVA: 0x00058195 File Offset: 0x00056395
	public float ModifiedMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_baseMax + this.m_maxModifier;
		}
	}

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x06000D03 RID: 3331 RVA: 0x000581A4 File Offset: 0x000563A4
	// (set) Token: 0x06000D04 RID: 3332 RVA: 0x000581CC File Offset: 0x000563CC
	public float Value
	{
		get
		{
			if (this.GodModeEntity())
			{
				return this.ModifiedMax;
			}
			return Utils.FastClamp(this.m_value, 0f, this.ModifiedMax);
		}
		set
		{
			if (this.m_value != value)
			{
				float value2 = this.m_value;
				this.m_value = Utils.FastClamp(value, 0f, this.ModifiedMax);
				this.SetChangedFlag(value2, value);
			}
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000D05 RID: 3333 RVA: 0x00058208 File Offset: 0x00056408
	public float ValuePercent
	{
		get
		{
			return Utils.FastClamp01(this.Value / this.ModifiedMax);
		}
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000D06 RID: 3334 RVA: 0x0005821C File Offset: 0x0005641C
	public float ValuePercentUI
	{
		get
		{
			return Utils.FastClamp01(this.Value / this.Max);
		}
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000D07 RID: 3335 RVA: 0x00058230 File Offset: 0x00056430
	public float ModifiedMaxPercent
	{
		get
		{
			return Utils.FastClamp01(this.ModifiedMax / this.Max);
		}
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000D08 RID: 3336 RVA: 0x00058244 File Offset: 0x00056444
	// (set) Token: 0x06000D09 RID: 3337 RVA: 0x0005824C File Offset: 0x0005644C
	public float MaxModifier
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_maxModifier;
		}
		set
		{
			this.m_maxModifier = Utils.FastClamp(value, -(this.Max * 0.75f), 0f);
		}
	}

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x06000D0A RID: 3338 RVA: 0x0005826C File Offset: 0x0005646C
	// (set) Token: 0x06000D0B RID: 3339 RVA: 0x00058274 File Offset: 0x00056474
	public float OriginalValue
	{
		get
		{
			return this.m_originalValue;
		}
		set
		{
			this.m_originalValue = value;
		}
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000D0C RID: 3340 RVA: 0x0005827D File Offset: 0x0005647D
	// (set) Token: 0x06000D0D RID: 3341 RVA: 0x00058285 File Offset: 0x00056485
	public float OriginalMax
	{
		get
		{
			return this.m_originalBaseMax;
		}
		set
		{
			this.m_originalBaseMax = value;
		}
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000D0E RID: 3342 RVA: 0x0005828E File Offset: 0x0005648E
	// (set) Token: 0x06000D0F RID: 3343 RVA: 0x00058296 File Offset: 0x00056496
	public bool Changed
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_changed;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			this.m_changed = value;
		}
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x000582A0 File Offset: 0x000564A0
	public void Write(BinaryWriter stream)
	{
		stream.Write(6);
		stream.Write(this.m_value);
		stream.Write(this.m_maxModifier);
		stream.Write(this.m_baseMax);
		stream.Write(this.m_originalBaseMax);
		stream.Write(this.m_originalValue);
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x000582F0 File Offset: 0x000564F0
	public void Read(BinaryReader stream)
	{
		int num = stream.ReadInt32();
		this.m_value = stream.ReadSingle();
		this.m_maxModifier = stream.ReadSingle();
		if (num <= 5)
		{
			stream.ReadSingle();
		}
		this.m_baseMax = stream.ReadSingle();
		this.m_originalBaseMax = stream.ReadSingle();
		this.m_originalValue = stream.ReadSingle();
		this.m_lastValue = this.m_value;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00058355 File Offset: 0x00056555
	[PublicizedFrom(EAccessModifier.Private)]
	public bool GodModeEntity()
	{
		return this.Entity && this.Entity.entityId == this.Entity.world.GetPrimaryPlayerId() && !GameStats.GetBool(EnumGameStats.IsPlayerDamageEnabled);
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x0005838D File Offset: 0x0005658D
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetChangedFlag(float newValue, float oldValue)
	{
		this.m_changed = (this.m_changed || Utils.Fastfloor(newValue) != Utils.Fastfloor(oldValue));
	}

	// Token: 0x04000AB8 RID: 2744
	public const int cBinaryVersion = 6;

	// Token: 0x04000AB9 RID: 2745
	public Stat.StatTypes StatType;

	// Token: 0x04000ABA RID: 2746
	public PassiveEffects GainPassive;

	// Token: 0x04000ABB RID: 2747
	public PassiveEffects LossPassive;

	// Token: 0x04000ABC RID: 2748
	public PassiveEffects MaxPassive;

	// Token: 0x04000ABD RID: 2749
	public EntityAlive Entity;

	// Token: 0x04000ABE RID: 2750
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_value;

	// Token: 0x04000ABF RID: 2751
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_originalValue;

	// Token: 0x04000AC0 RID: 2752
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_lastValue;

	// Token: 0x04000AC1 RID: 2753
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_baseMax;

	// Token: 0x04000AC2 RID: 2754
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_originalBaseMax;

	// Token: 0x04000AC3 RID: 2755
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_maxModifier;

	// Token: 0x04000AC4 RID: 2756
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_changed;

	// Token: 0x04000AC5 RID: 2757
	[PublicizedFrom(EAccessModifier.Private)]
	public float regenAmount;

	// Token: 0x020001AA RID: 426
	public enum StatTypes
	{
		// Token: 0x04000AC8 RID: 2760
		Health,
		// Token: 0x04000AC9 RID: 2761
		Stamina,
		// Token: 0x04000ACA RID: 2762
		Food,
		// Token: 0x04000ACB RID: 2763
		Water,
		// Token: 0x04000ACC RID: 2764
		CoreTemp
	}
}
