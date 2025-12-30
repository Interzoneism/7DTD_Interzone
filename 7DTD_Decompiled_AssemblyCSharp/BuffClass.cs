using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020005B6 RID: 1462
public class BuffClass
{
	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x06002F03 RID: 12035 RVA: 0x0014297F File Offset: 0x00140B7F
	// (set) Token: 0x06002F04 RID: 12036 RVA: 0x00142987 File Offset: 0x00140B87
	public float DurationMax
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.durationMax;
		}
		set
		{
			if (this.initialDurationMax == 0f && value > 0f)
			{
				this.initialDurationMax = value;
			}
			this.durationMax = value;
		}
	}

	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x06002F05 RID: 12037 RVA: 0x001429AC File Offset: 0x00140BAC
	public float InitialDurationMax
	{
		get
		{
			return this.initialDurationMax;
		}
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x001429B4 File Offset: 0x00140BB4
	public BuffClass(string _name = "")
	{
		this.Name = _name.ToLower();
		this.LocalizedName = string.Empty;
		this.DescriptionKey = string.Empty;
		this.TooltipKey = string.Empty;
		this.Icon = string.Empty;
		this.IconBlink = false;
		this.OrCompare = false;
		this.Requirements = new List<IRequirement>();
		this.Hidden = false;
		this.DamageType = EnumDamageTypes.None;
		this.StackType = BuffEffectStackTypes.Replace;
		this.durationMax = 0f;
		this.initialDurationMax = 0f;
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x00142A70 File Offset: 0x00140C70
	public void UpdateTimer(BuffValue _ev, float _deltaTime)
	{
		uint durationInTicks = _ev.DurationInTicks;
		_ev.DurationInTicks = durationInTicks + 1U;
		if (this.DurationMax > 0f && _ev.DurationInSeconds >= this.DurationMax)
		{
			_ev.Finished = true;
		}
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x00142AB0 File Offset: 0x00140CB0
	public void ModifyValue(EntityAlive _self, PassiveEffects _effect, BuffValue _bv, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags)
	{
		if (_bv.Remove)
		{
			return;
		}
		if (this.Requirements.Count > 0)
		{
			_self.MinEventContext.Tags |= _tags;
			if (!this.canRun(_self.MinEventContext))
			{
				return;
			}
		}
		if (this.Effects != null)
		{
			this.Effects.ModifyValue(_self, _effect, ref _base_value, ref _perc_value, _bv.DurationInSeconds, _tags, (this.StackType == BuffEffectStackTypes.Effect) ? _bv.StackEffectMultiplier : 1);
		}
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x00142B30 File Offset: 0x00140D30
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, EntityAlive _self, PassiveEffects _effect, BuffValue _bv, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags)
	{
		if (_bv.Remove)
		{
			return;
		}
		if (this.Requirements.Count > 0)
		{
			_self.MinEventContext.Tags |= _tags;
			if (!this.canRun(_self.MinEventContext))
			{
				return;
			}
		}
		if (this.Effects != null)
		{
			this.Effects.GetModifiedValueData(_modValueSources, _sourceType, _self, _effect, ref _base_value, ref _perc_value, _bv.DurationInSeconds, _tags, (this.StackType == BuffEffectStackTypes.Effect) ? _bv.StackEffectMultiplier : 1);
		}
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x00142BB4 File Offset: 0x00140DB4
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.Effects == null)
		{
			return;
		}
		if (!this.canRun(_params))
		{
			return;
		}
		this.Effects.FireEvent(_eventType, _params);
	}

	// Token: 0x06002F0B RID: 12043 RVA: 0x00142BD8 File Offset: 0x00140DD8
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canRun(MinEventParams _params)
	{
		if (this.Requirements == null || this.Requirements.Count <= 0)
		{
			return true;
		}
		if (this.OrCompare)
		{
			for (int i = 0; i < this.Requirements.Count; i++)
			{
				if (this.Requirements[i].IsValid(_params))
				{
					return true;
				}
			}
			return false;
		}
		for (int j = 0; j < this.Requirements.Count; j++)
		{
			if (!this.Requirements[j].IsValid(_params))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400251F RID: 9503
	public string Name;

	// Token: 0x04002520 RID: 9504
	public string LocalizedName;

	// Token: 0x04002521 RID: 9505
	public string Description;

	// Token: 0x04002522 RID: 9506
	public string DescriptionKey;

	// Token: 0x04002523 RID: 9507
	public string Tooltip;

	// Token: 0x04002524 RID: 9508
	public string TooltipKey;

	// Token: 0x04002525 RID: 9509
	public string Icon;

	// Token: 0x04002526 RID: 9510
	public string DisplayValueCVar;

	// Token: 0x04002527 RID: 9511
	public string DisplayValueKey;

	// Token: 0x04002528 RID: 9512
	public BuffClass.CVarDisplayFormat DisplayValueFormat;

	// Token: 0x04002529 RID: 9513
	public Color IconColor;

	// Token: 0x0400252A RID: 9514
	public bool IconBlink;

	// Token: 0x0400252B RID: 9515
	public EnumEntityUINotificationDisplayMode DisplayType;

	// Token: 0x0400252C RID: 9516
	public List<string> Cures;

	// Token: 0x0400252D RID: 9517
	public bool OrCompare;

	// Token: 0x0400252E RID: 9518
	public List<IRequirement> Requirements;

	// Token: 0x0400252F RID: 9519
	public MinEffectController Effects;

	// Token: 0x04002530 RID: 9520
	public bool Hidden;

	// Token: 0x04002531 RID: 9521
	public bool ShowOnHUD = true;

	// Token: 0x04002532 RID: 9522
	public bool AllowInEditor;

	// Token: 0x04002533 RID: 9523
	public EnumGameStats RequiredGameStat = EnumGameStats.Last;

	// Token: 0x04002534 RID: 9524
	[PublicizedFrom(EAccessModifier.Private)]
	public float durationMax;

	// Token: 0x04002535 RID: 9525
	[PublicizedFrom(EAccessModifier.Private)]
	public float initialDurationMax;

	// Token: 0x04002536 RID: 9526
	public float UpdateRate = 1f;

	// Token: 0x04002537 RID: 9527
	public EnumDamageTypes DamageType;

	// Token: 0x04002538 RID: 9528
	public EnumDamageSource DamageSource;

	// Token: 0x04002539 RID: 9529
	public BuffEffectStackTypes StackType;

	// Token: 0x0400253A RID: 9530
	public bool RemoveOnDeath = true;

	// Token: 0x0400253B RID: 9531
	public FastTags<TagGroup.Global> NameTag;

	// Token: 0x0400253C RID: 9532
	public FastTags<TagGroup.Global> Tags = FastTags<TagGroup.Global>.none;

	// Token: 0x020005B7 RID: 1463
	public enum CVarDisplayFormat
	{
		// Token: 0x0400253E RID: 9534
		None,
		// Token: 0x0400253F RID: 9535
		Float,
		// Token: 0x04002540 RID: 9536
		FlooredToInt,
		// Token: 0x04002541 RID: 9537
		RoundedToInt,
		// Token: 0x04002542 RID: 9538
		CeiledToInt,
		// Token: 0x04002543 RID: 9539
		Time,
		// Token: 0x04002544 RID: 9540
		Percentage
	}
}
