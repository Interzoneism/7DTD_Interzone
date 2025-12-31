using System;
using System.Collections.Generic;
using System.Xml.Linq;

// Token: 0x02000619 RID: 1561
public class MinEffectGroup
{
	// Token: 0x0600305D RID: 12381 RVA: 0x0014968C File Offset: 0x0014788C
	public MinEffectGroup()
	{
		this.Requirements = null;
		this.PassiveEffects = new List<PassiveEffect>();
		this.TriggeredEffects = new Dictionary<MinEventTypes, List<MinEventActionBase>>();
		this.EffectDescriptions = new List<EffectGroupDescription>();
		this.OwnerTiered = true;
		this.PassivesIndices = new List<PassiveEffects>();
		this.EffectDisplayValues = new CaseInsensitiveStringDictionary<EffectDisplayValue>();
	}

	// Token: 0x0600305E RID: 12382 RVA: 0x001496E4 File Offset: 0x001478E4
	public void ModifyValue(MinEventParams _params, EntityAlive _self, PassiveEffects _effect, ref float _base_value, ref float _perc_value, float level, FastTags<TagGroup.Global> _tags, int _multiplier = 1)
	{
		if (!this.canRun(_params))
		{
			return;
		}
		int count = this.PassiveEffects.Count;
		for (int i = 0; i < count; i++)
		{
			PassiveEffect passiveEffect = this.PassiveEffects[i];
			if (passiveEffect.Type == _effect && passiveEffect.RequirementsMet(_params))
			{
				passiveEffect.ModifyValue(_self, level, ref _base_value, ref _perc_value, _tags, _multiplier);
			}
		}
	}

	// Token: 0x0600305F RID: 12383 RVA: 0x00149744 File Offset: 0x00147944
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSource, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, MinEffectController.SourceParentType _parentType, EntityAlive _self, PassiveEffects _effect, ref float _base_value, ref float _perc_value, float level, FastTags<TagGroup.Global> _tags, int _multiplier = 1, object _parentPointer = null)
	{
		MinEventParams minEventParams;
		if (_self == null)
		{
			minEventParams = MinEventParams.CachedEventParam;
			minEventParams.Self = null;
		}
		else
		{
			minEventParams = _self.MinEventContext;
		}
		minEventParams.Tags = _tags;
		if (!this.canRun(minEventParams))
		{
			return;
		}
		for (int i = 0; i < this.PassiveEffects.Count; i++)
		{
			PassiveEffect passiveEffect = this.PassiveEffects[i];
			if (passiveEffect.Type == _effect && passiveEffect.RequirementsMet(minEventParams))
			{
				passiveEffect.GetModifiedValueData(_modValueSource, _sourceType, _parentType, _self, level, ref _base_value, ref _perc_value, _tags, _multiplier, _parentPointer);
			}
		}
	}

	// Token: 0x06003060 RID: 12384 RVA: 0x001497D4 File Offset: 0x001479D4
	public IReadOnlyList<MinEventActionBase> GetTriggeredEffects(MinEventTypes _eventType)
	{
		List<MinEventActionBase> result;
		if (!this.TriggeredEffects.TryGetValue(_eventType, out result))
		{
			return Array.Empty<MinEventActionBase>();
		}
		return result;
	}

	// Token: 0x06003061 RID: 12385 RVA: 0x001497F8 File Offset: 0x001479F8
	public void AddTriggeredEffect(MinEventActionBase triggeredEffect)
	{
		MinEventTypes eventType = triggeredEffect.EventType;
		List<MinEventActionBase> list;
		if (!this.TriggeredEffects.TryGetValue(eventType, out list))
		{
			list = new List<MinEventActionBase>();
			this.TriggeredEffects.Add(eventType, list);
		}
		list.Add(triggeredEffect);
	}

	// Token: 0x06003062 RID: 12386 RVA: 0x00149836 File Offset: 0x00147A36
	public bool HasEvents()
	{
		return this.TriggeredEffects.Count > 0;
	}

	// Token: 0x06003063 RID: 12387 RVA: 0x00149848 File Offset: 0x00147A48
	public void FireEvent(MinEventTypes _eventType, MinEventParams _eventParms)
	{
		if (this.TriggeredEffects.Count <= 0)
		{
			return;
		}
		IReadOnlyList<MinEventActionBase> triggeredEffects = this.GetTriggeredEffects(_eventType);
		if (triggeredEffects.Count <= 0)
		{
			return;
		}
		if (!this.canRun(_eventParms))
		{
			return;
		}
		for (int i = 0; i < triggeredEffects.Count; i++)
		{
			MinEventActionBase minEventActionBase = triggeredEffects[i];
			if (minEventActionBase.CanExecute(_eventType, _eventParms))
			{
				minEventActionBase.Execute(_eventParms);
			}
		}
	}

	// Token: 0x06003064 RID: 12388 RVA: 0x001498AC File Offset: 0x00147AAC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canRun(MinEventParams _params)
	{
		if (this.Requirements == null)
		{
			return true;
		}
		if (this.OrCompareRequirements)
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

	// Token: 0x06003065 RID: 12389 RVA: 0x00149926 File Offset: 0x00147B26
	public bool HasTrigger(MinEventTypes _eventType)
	{
		return this.GetTriggeredEffects(_eventType).Count > 0;
	}

	// Token: 0x06003066 RID: 12390 RVA: 0x00149938 File Offset: 0x00147B38
	public static MinEffectGroup ParseXml(XElement _element)
	{
		MinEffectGroup minEffectGroup = new MinEffectGroup();
		if (_element.HasAttribute("compare_type"))
		{
			minEffectGroup.OrCompareRequirements = _element.GetAttribute("compare_type").EqualsCaseInsensitive("or");
		}
		if (_element.HasAttribute("tiered"))
		{
			minEffectGroup.OwnerTiered = StringParsers.ParseBool(_element.GetAttribute("tiered"), 0, -1, true);
		}
		foreach (XElement xelement in _element.Elements())
		{
			if (xelement.Name == "requirements")
			{
				if (xelement.HasAttribute("compare_type"))
				{
					minEffectGroup.OrCompareRequirements = xelement.GetAttribute("compare_type").EqualsCaseInsensitive("or");
				}
				List<IRequirement> list = RequirementBase.ParseRequirements(xelement);
				if (list.Count > 0)
				{
					if (minEffectGroup.Requirements == null)
					{
						minEffectGroup.Requirements = new List<IRequirement>();
					}
					minEffectGroup.Requirements.AddRange(list);
				}
			}
			else if (xelement.Name == "requirement")
			{
				IRequirement requirement = RequirementBase.ParseRequirement(xelement);
				if (requirement != null)
				{
					if (minEffectGroup.Requirements == null)
					{
						minEffectGroup.Requirements = new List<IRequirement>();
					}
					minEffectGroup.Requirements.Add(requirement);
				}
			}
			else if (xelement.Name == "passive_effect")
			{
				PassiveEffect passiveEffect = PassiveEffect.ParsePassiveEffect(xelement);
				if (passiveEffect != null)
				{
					MinEffectGroup.AddPassiveEffectToGroup(minEffectGroup, passiveEffect);
				}
			}
			else if (xelement.Name == "triggered_effect")
			{
				MinEventActionBase minEventActionBase = MinEventActionBase.ParseAction(xelement);
				if (minEventActionBase != null)
				{
					minEffectGroup.AddTriggeredEffect(minEventActionBase);
				}
			}
			else if (xelement.Name == "effect_description")
			{
				EffectGroupDescription effectGroupDescription = EffectGroupDescription.ParseDescription(xelement);
				if (effectGroupDescription != null)
				{
					minEffectGroup.EffectDescriptions.Add(effectGroupDescription);
				}
			}
			else if (xelement.Name == "display_value")
			{
				EffectDisplayValue effectDisplayValue = EffectDisplayValue.ParseDisplayValue(xelement);
				if (effectDisplayValue != null)
				{
					minEffectGroup.EffectDisplayValues.Add(effectDisplayValue.Name, effectDisplayValue);
				}
			}
		}
		return minEffectGroup;
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x00149B90 File Offset: 0x00147D90
	public static void AddPassiveEffectToGroup(MinEffectGroup _effectGroup, PassiveEffect _pe)
	{
		_effectGroup.PassivesIndices.Add(_pe.Type);
		_effectGroup.PassiveEffects.Add(_pe);
	}

	// Token: 0x040026DE RID: 9950
	public bool OrCompareRequirements;

	// Token: 0x040026DF RID: 9951
	public List<IRequirement> Requirements;

	// Token: 0x040026E0 RID: 9952
	public List<PassiveEffect> PassiveEffects;

	// Token: 0x040026E1 RID: 9953
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<MinEventTypes, List<MinEventActionBase>> TriggeredEffects;

	// Token: 0x040026E2 RID: 9954
	public List<EffectGroupDescription> EffectDescriptions;

	// Token: 0x040026E3 RID: 9955
	public CaseInsensitiveStringDictionary<EffectDisplayValue> EffectDisplayValues;

	// Token: 0x040026E4 RID: 9956
	public bool OwnerTiered;

	// Token: 0x040026E5 RID: 9957
	public List<PassiveEffects> PassivesIndices;
}
