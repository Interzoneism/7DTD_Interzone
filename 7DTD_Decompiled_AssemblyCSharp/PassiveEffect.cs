using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x0200061D RID: 1565
public class PassiveEffect
{
	// Token: 0x06003074 RID: 12404 RVA: 0x0014A1B0 File Offset: 0x001483B0
	public void ModifyValue(EntityAlive _ea, float _level, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>), int _stackEffectMultiplier = 1)
	{
		if (this.CVarValues != null)
		{
			for (int i = 0; i < this.CVarValues.Length; i++)
			{
				string text = this.CVarValues[i];
				if (text != null)
				{
					if (_ea.Buffs.HasCustomVar(text))
					{
						this.Values[i] = _ea.Buffs.GetCustomVar(text);
					}
					else
					{
						_ea.Buffs.AddCustomVar(text, 0f);
						this.Values[i] = 0f;
					}
				}
			}
		}
		PassiveEffect.ModValue(this.Modifier, _level, ref _base_value, ref _perc_value, this.Levels, this.Values, (float)_stackEffectMultiplier, 0);
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x0014A248 File Offset: 0x00148448
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSource, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, MinEffectController.SourceParentType _parentType, EntityAlive _ea, float _level, ref float _base_value, ref float _perc_value, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>), int _stackEffectMultiplier = 1, object _parentPointer = null)
	{
		float num = 0f;
		float num2 = 1f;
		if (this.CVarValues != null)
		{
			for (int i = 0; i < this.CVarValues.Length; i++)
			{
				string text = this.CVarValues[i];
				if (text != null)
				{
					if (_ea.Buffs.HasCustomVar(text))
					{
						this.Values[i] = _ea.Buffs.GetCustomVar(text);
					}
					else
					{
						Log.Out("PassiveEffects: CVar '{0}' was not found in custom variable dictionary for entity '{1}'", new object[]
						{
							text,
							_ea.EntityName
						});
					}
				}
			}
		}
		PassiveEffect.ModValue(this.Modifier, _level, ref num, ref num2, this.Levels, this.Values, (float)_stackEffectMultiplier, 0);
		if (num == 0f && num2 == 1f)
		{
			return;
		}
		EffectManager.ModifierValuesAndSources modifierValuesAndSources = new EffectManager.ModifierValuesAndSources
		{
			ValueSource = _sourceType,
			ParentType = _parentType,
			Source = _parentPointer,
			ModifierType = this.Modifier,
			Tags = this.Tags
		};
		if (this.Modifier.ToStringCached<PassiveEffect.ValueModifierTypes>().Contains("base"))
		{
			modifierValuesAndSources.Value = num;
		}
		else
		{
			modifierValuesAndSources.Value = num2;
		}
		_modValueSource.Add(modifierValuesAndSources);
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x0014A368 File Offset: 0x00148568
	public bool RequirementsMet(MinEventParams _params)
	{
		if (!this.hasMatchingTag(_params.Tags))
		{
			return false;
		}
		if (this.Requirements == null)
		{
			return true;
		}
		if (!this.OrCompare)
		{
			for (int i = 0; i < this.Requirements.Count; i++)
			{
				if (!this.Requirements[i].IsValid(_params))
				{
					return false;
				}
			}
			return true;
		}
		for (int j = 0; j < this.Requirements.Count; j++)
		{
			if (this.Requirements[j].IsValid(_params))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x0014A3F4 File Offset: 0x001485F4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasMatchingTag(FastTags<TagGroup.Global> _tags)
	{
		if (_tags.IsEmpty && !this.Tags.IsEmpty)
		{
			return false;
		}
		if (this.MatchAnyTags)
		{
			if (this.Tags.IsEmpty)
			{
				return !this.InvertTagCheck;
			}
			if (!this.InvertTagCheck)
			{
				return _tags.Test_AnySet(this.Tags);
			}
			return !_tags.Test_AnySet(this.Tags);
		}
		else
		{
			if (!this.InvertTagCheck)
			{
				return _tags.Test_AllSet(this.Tags);
			}
			return !_tags.Test_AllSet(this.Tags);
		}
	}

	// Token: 0x06003078 RID: 12408 RVA: 0x0014A488 File Offset: 0x00148688
	public static PassiveEffect ParsePassiveEffect(XElement _element)
	{
		string attribute = _element.GetAttribute("name");
		if (attribute.Length == 0)
		{
			return null;
		}
		string attribute2 = _element.GetAttribute("modifier");
		if (attribute2.Length == 0)
		{
			attribute2 = _element.GetAttribute("operation");
			if (attribute2.Length == 0)
			{
				return null;
			}
		}
		string text = _element.GetAttribute("value");
		if (text.Length == 0)
		{
			return null;
		}
		if (text[0] == '^')
		{
			text = EntityClassesFromXml.sReplacePassiveEffects[text];
		}
		PassiveEffect passiveEffect = new PassiveEffect();
		passiveEffect.Type = EnumUtils.Parse<PassiveEffects>(attribute, true);
		if (passiveEffect.Type == PassiveEffects.None)
		{
			return null;
		}
		string attribute3 = _element.GetAttribute("compare_type");
		if (attribute3.Length > 0 && attribute3.EqualsCaseInsensitive("or"))
		{
			passiveEffect.OrCompare = true;
		}
		if (text.Contains(","))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			passiveEffect.Values = new float[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				float num;
				if (array[i].StartsWith("@"))
				{
					if (passiveEffect.CVarValues == null)
					{
						passiveEffect.CVarValues = new string[array.Length];
					}
					passiveEffect.CVarValues[i] = array[i].Trim().Remove(0, 1);
				}
				else if (StringParsers.TryParseFloat(array[i], out num, 0, -1, NumberStyles.Any))
				{
					passiveEffect.Values[i] = num;
				}
			}
		}
		else if (text.StartsWith("@"))
		{
			passiveEffect.CVarValues = new string[]
			{
				text.Trim().Remove(0, 1)
			};
			passiveEffect.Values = new float[1];
		}
		else
		{
			passiveEffect.Values = new float[]
			{
				StringParsers.ParseFloat(text, 0, -1, NumberStyles.Any)
			};
		}
		if (passiveEffect.CVarValues != null)
		{
			for (int j = 0; j < passiveEffect.CVarValues.Length; j++)
			{
				string text2 = passiveEffect.CVarValues[j];
				if (text2 != null && text2.Contains("@"))
				{
					Log.Error("CVar reference contains an '@' symbol! This will break calls to it.");
				}
			}
		}
		string attribute4 = _element.GetAttribute("level");
		if (attribute4.Length > 0)
		{
			if (attribute4.Contains(","))
			{
				string[] array2 = attribute4.Split(',', StringSplitOptions.None);
				passiveEffect.Levels = new float[array2.Length];
				for (int k = 0; k < array2.Length; k++)
				{
					passiveEffect.Levels[k] = StringParsers.ParseFloat(array2[k], 0, -1, NumberStyles.Any);
				}
			}
			else
			{
				passiveEffect.Levels = new float[]
				{
					StringParsers.ParseFloat(attribute4, 0, -1, NumberStyles.Any)
				};
			}
		}
		else if ((attribute4 = _element.GetAttribute("tier")).Length > 0)
		{
			if (attribute4.Contains(","))
			{
				string[] array3 = attribute4.Split(',', StringSplitOptions.None);
				passiveEffect.Levels = new float[array3.Length];
				for (int l = 0; l < array3.Length; l++)
				{
					passiveEffect.Levels[l] = StringParsers.ParseFloat(array3[l], 0, -1, NumberStyles.Any);
				}
			}
			else
			{
				passiveEffect.Levels = new float[]
				{
					StringParsers.ParseFloat(attribute4, 0, -1, NumberStyles.Any)
				};
			}
		}
		else if ((attribute4 = _element.GetAttribute("duration")).Length > 0)
		{
			if (attribute4.Contains(","))
			{
				string[] array4 = attribute4.Split(',', StringSplitOptions.None);
				passiveEffect.Levels = new float[array4.Length];
				for (int m = 0; m < array4.Length; m++)
				{
					passiveEffect.Levels[m] = StringParsers.ParseFloat(array4[m], 0, -1, NumberStyles.Any);
				}
			}
			else
			{
				passiveEffect.Levels = new float[]
				{
					StringParsers.ParseFloat(attribute4, 0, -1, NumberStyles.Any)
				};
			}
		}
		string attribute5 = _element.GetAttribute("tags");
		if (attribute5.Length > 0)
		{
			passiveEffect.Tags = FastTags<TagGroup.Global>.Parse(attribute5);
		}
		else
		{
			attribute5 = _element.GetAttribute("tag");
			if (attribute5.Length > 0)
			{
				passiveEffect.Tags = FastTags<TagGroup.Global>.Parse(attribute5);
			}
		}
		if (_element.HasAttribute("match_all_tags"))
		{
			passiveEffect.MatchAnyTags = false;
		}
		if (_element.HasAttribute("invert_tag_check"))
		{
			passiveEffect.InvertTagCheck = true;
		}
		passiveEffect.Modifier = EnumUtils.Parse<PassiveEffect.ValueModifierTypes>(attribute2, false);
		foreach (XElement element in _element.Elements("requirement"))
		{
			IRequirement requirement = RequirementBase.ParseRequirement(element);
			if (requirement != null)
			{
				if (passiveEffect.Requirements == null)
				{
					passiveEffect.Requirements = new List<IRequirement>();
				}
				passiveEffect.Requirements.Add(requirement);
			}
		}
		return passiveEffect;
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x0014A968 File Offset: 0x00148B68
	public static PassiveEffect CreateEmptyPassiveEffect(PassiveEffects type)
	{
		return new PassiveEffect
		{
			Type = type,
			Modifier = PassiveEffect.ValueModifierTypes.perc_add,
			Values = new float[1]
		};
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x0014A998 File Offset: 0x00148B98
	public void AddColoredInfoStrings(ref List<string> _infoList, float _level = -1f)
	{
		if (this.Levels == null)
		{
			_infoList.Add(this.GetDisplayValue(0f, 0f, 1f, 1f));
			return;
		}
		if (_level == -1f)
		{
			for (int i = 0; i < this.Levels.Length; i++)
			{
				_infoList.Add(this.GetDisplayValue(this.Levels[i], 0f, 1f, 1f));
			}
			return;
		}
		_infoList.Add(this.GetDisplayValue(_level, 0f, 1f, 1f));
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x0014AA2C File Offset: 0x00148C2C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ModValue(PassiveEffect.ValueModifierTypes _modifier, float _level, ref float _base_value, ref float _perc_value, float[] _levels, float[] _values, float _multiplier = 1f, int _seed = 0)
	{
		if (_levels != null)
		{
			if (_values != null)
			{
				if (_values.Length == _levels.Length)
				{
					if (_levels.Length >= 2)
					{
						int i = _levels.Length - 1;
						while (i > 0)
						{
							if (PassiveEffect.InLevelRange(_level, _levels[i - 1], _levels[i]))
							{
								switch (_modifier)
								{
								case PassiveEffect.ValueModifierTypes.base_set:
									_base_value = Mathf.Lerp(_values[i - 1], _values[i], (_level - _levels[i - 1]) / (_levels[i] - _levels[i - 1]));
									return;
								case PassiveEffect.ValueModifierTypes.base_add:
									_base_value += Mathf.Lerp(_values[i - 1], _values[i], (_level - _levels[i - 1]) / (_levels[i] - _levels[i - 1]));
									return;
								case PassiveEffect.ValueModifierTypes.base_subtract:
									_base_value -= Mathf.Lerp(_values[i - 1], _values[i], (_level - _levels[i - 1]) / (_levels[i] - _levels[i - 1]));
									return;
								case PassiveEffect.ValueModifierTypes.perc_set:
									_perc_value = Mathf.Lerp(_values[i - 1], _values[i], (_level - _levels[i - 1]) / (_levels[i] - _levels[i - 1]));
									return;
								case PassiveEffect.ValueModifierTypes.perc_add:
									_perc_value += Mathf.Lerp(_values[i - 1], _values[i], (_level - _levels[i - 1]) / (_levels[i] - _levels[i - 1]));
									return;
								case PassiveEffect.ValueModifierTypes.perc_subtract:
									_perc_value -= Mathf.Lerp(_values[i - 1], _values[i], (_level - _levels[i - 1]) / (_levels[i] - _levels[i - 1]));
									return;
								default:
									return;
								}
							}
							else
							{
								i--;
							}
						}
						return;
					}
					if (_levels.Length >= 1 && Mathf.FloorToInt(_level) == Mathf.FloorToInt(_levels[0]))
					{
						switch (_modifier)
						{
						case PassiveEffect.ValueModifierTypes.base_set:
							_base_value = _values[0];
							return;
						case PassiveEffect.ValueModifierTypes.base_add:
							_base_value += _values[0];
							return;
						case PassiveEffect.ValueModifierTypes.base_subtract:
							_base_value -= _values[0];
							return;
						case PassiveEffect.ValueModifierTypes.perc_set:
							_perc_value = _values[0];
							return;
						case PassiveEffect.ValueModifierTypes.perc_add:
							_perc_value += _values[0];
							return;
						case PassiveEffect.ValueModifierTypes.perc_subtract:
							_perc_value -= _values[0];
							return;
						default:
							return;
						}
					}
				}
				else if (_levels.Length == 1 && _values.Length == 2 && Mathf.FloorToInt(_level) == Mathf.FloorToInt(_levels[0]))
				{
					if (MinEventParams.CachedEventParam.Seed == 0)
					{
						switch (_modifier)
						{
						case PassiveEffect.ValueModifierTypes.base_set:
							_base_value = (_values[0] + _values[1]) * 0.5f;
							return;
						case PassiveEffect.ValueModifierTypes.base_add:
							_base_value += (_values[0] + _values[1]) * 0.5f;
							return;
						case PassiveEffect.ValueModifierTypes.base_subtract:
							_base_value -= (_values[0] + _values[1]) * 0.5f;
							return;
						case PassiveEffect.ValueModifierTypes.perc_set:
							_perc_value = (_values[0] + _values[1]) * 0.5f;
							return;
						case PassiveEffect.ValueModifierTypes.perc_add:
							_perc_value += (_values[0] + _values[1]) * 0.5f;
							return;
						case PassiveEffect.ValueModifierTypes.perc_subtract:
							_perc_value -= (_values[0] + _values[1]) * 0.5f;
							return;
						default:
							return;
						}
					}
					else
					{
						GameRandom tempGameRandom = GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed);
						switch (_modifier)
						{
						case PassiveEffect.ValueModifierTypes.base_set:
							_base_value = tempGameRandom.RandomRange(_values[0], _values[1]);
							return;
						case PassiveEffect.ValueModifierTypes.base_add:
							_base_value += tempGameRandom.RandomRange(_values[0], _values[1]);
							return;
						case PassiveEffect.ValueModifierTypes.base_subtract:
							_base_value -= tempGameRandom.RandomRange(_values[0], _values[1]);
							return;
						case PassiveEffect.ValueModifierTypes.perc_set:
							_perc_value = tempGameRandom.RandomRange(_values[0], _values[1]);
							return;
						case PassiveEffect.ValueModifierTypes.perc_add:
							_perc_value += tempGameRandom.RandomRange(_values[0], _values[1]);
							return;
						case PassiveEffect.ValueModifierTypes.perc_subtract:
							_perc_value -= tempGameRandom.RandomRange(_values[0], _values[1]);
							return;
						default:
							return;
						}
					}
				}
				else if (_values.Length == 1 && _levels.Length == 2 && PassiveEffect.InLevelRange(_level, _levels[0], _levels[1]))
				{
					switch (_modifier)
					{
					case PassiveEffect.ValueModifierTypes.base_set:
						_base_value = _values[0];
						return;
					case PassiveEffect.ValueModifierTypes.base_add:
						_base_value += _values[0];
						return;
					case PassiveEffect.ValueModifierTypes.base_subtract:
						_base_value -= _values[0];
						return;
					case PassiveEffect.ValueModifierTypes.perc_set:
						_perc_value = _values[0];
						return;
					case PassiveEffect.ValueModifierTypes.perc_add:
						_perc_value += _values[0];
						return;
					case PassiveEffect.ValueModifierTypes.perc_subtract:
						_perc_value -= _values[0];
						return;
					default:
						return;
					}
				}
			}
		}
		else if (_values != null)
		{
			if (_values.Length == 1)
			{
				switch (_modifier)
				{
				case PassiveEffect.ValueModifierTypes.base_set:
					_base_value = _values[0];
					return;
				case PassiveEffect.ValueModifierTypes.base_add:
					_base_value += _values[0];
					return;
				case PassiveEffect.ValueModifierTypes.base_subtract:
					_base_value -= _values[0];
					return;
				case PassiveEffect.ValueModifierTypes.perc_set:
					_perc_value = _values[0];
					return;
				case PassiveEffect.ValueModifierTypes.perc_add:
					_perc_value += _values[0];
					return;
				case PassiveEffect.ValueModifierTypes.perc_subtract:
					_perc_value -= _values[0];
					return;
				default:
					return;
				}
			}
			else if (_values.Length == 2)
			{
				if (MinEventParams.CachedEventParam.Seed == 0)
				{
					switch (_modifier)
					{
					case PassiveEffect.ValueModifierTypes.base_set:
						_base_value = (_values[0] + _values[1]) * 0.5f;
						return;
					case PassiveEffect.ValueModifierTypes.base_add:
						_base_value += (_values[0] + _values[1]) * 0.5f;
						return;
					case PassiveEffect.ValueModifierTypes.base_subtract:
						_base_value -= (_values[0] + _values[1]) * 0.5f;
						return;
					case PassiveEffect.ValueModifierTypes.perc_set:
						_perc_value = (_values[0] + _values[1]) * 0.5f;
						return;
					case PassiveEffect.ValueModifierTypes.perc_add:
						_perc_value += (_values[0] + _values[1]) * 0.5f;
						return;
					case PassiveEffect.ValueModifierTypes.perc_subtract:
						_perc_value -= (_values[0] + _values[1]) * 0.5f;
						return;
					default:
						return;
					}
				}
				else
				{
					GameRandom tempGameRandom2 = GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed);
					switch (_modifier)
					{
					case PassiveEffect.ValueModifierTypes.base_set:
						_base_value = tempGameRandom2.RandomRange(_values[0], _values[1]);
						return;
					case PassiveEffect.ValueModifierTypes.base_add:
						_base_value += tempGameRandom2.RandomRange(_values[0], _values[1]);
						return;
					case PassiveEffect.ValueModifierTypes.base_subtract:
						_base_value -= tempGameRandom2.RandomRange(_values[0], _values[1]);
						return;
					case PassiveEffect.ValueModifierTypes.perc_set:
						_perc_value = tempGameRandom2.RandomRange(_values[0], _values[1]);
						return;
					case PassiveEffect.ValueModifierTypes.perc_add:
						_perc_value += tempGameRandom2.RandomRange(_values[0], _values[1]);
						return;
					case PassiveEffect.ValueModifierTypes.perc_subtract:
						_perc_value -= tempGameRandom2.RandomRange(_values[0], _values[1]);
						break;
					default:
						return;
					}
				}
			}
		}
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x00149C68 File Offset: 0x00147E68
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool InLevelRange(float _level, float _min, float _max)
	{
		return _level >= _min && _level <= _max;
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x0014AFB8 File Offset: 0x001491B8
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetDisplayValue(float _level, float _base_value = 0f, float _perc_value = 1f, float _multiplier = 1f)
	{
		if (this.Levels != null)
		{
			if (this.Values != null)
			{
				if (this.Values.Length == this.Levels.Length)
				{
					if (this.Levels.Length >= 2)
					{
						for (int i = 0; i < this.Levels.Length - 1; i += 2)
						{
							if (PassiveEffect.InLevelRange(_level, this.Levels[i], this.Levels[i + 1]))
							{
								PassiveEffect.ValueModifierTypes modifier = this.Modifier;
								if (modifier <= PassiveEffect.ValueModifierTypes.base_subtract)
								{
									_base_value = Mathf.Lerp(this.Values[i], this.Values[i + 1], (_level - this.Levels[i]) / (this.Levels[i + 1] - this.Levels[i]));
									return string.Format("{0}: [REPLACE_COLOR]{1}{2}[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_base_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.base_add) ? "+" : "", _base_value.ToCultureInvariantString("0.0"));
								}
								if (modifier - PassiveEffect.ValueModifierTypes.perc_set <= 2)
								{
									_perc_value = Mathf.Lerp(this.Values[i], this.Values[i + 1], (_level - this.Levels[i]) / (this.Levels[i + 1] - this.Levels[i]));
									return string.Format("{0}: [REPLACE_COLOR]{1}{2}%[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_perc_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.perc_add) ? "+" : "", (_perc_value * 100f).ToCultureInvariantString("0.0"));
								}
							}
						}
					}
					else if (this.Levels.Length >= 1 && _level == this.Levels[0])
					{
						PassiveEffect.ValueModifierTypes modifier = this.Modifier;
						if (modifier <= PassiveEffect.ValueModifierTypes.base_subtract)
						{
							_base_value = this.Values[0];
							return string.Format("{0}: [REPLACE_COLOR]{1}{2}[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_base_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.base_add) ? "+" : "", _base_value.ToCultureInvariantString("0.0"));
						}
						if (modifier - PassiveEffect.ValueModifierTypes.perc_set <= 2)
						{
							_perc_value = this.Values[0];
							return string.Format("{0}: [REPLACE_COLOR]{1}{2}%[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_perc_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.perc_add) ? "+" : "", (_perc_value * 100f).ToCultureInvariantString("0.0"));
						}
					}
				}
				else if (this.Values.Length == 2 && this.Levels.Length == 1)
				{
					GameRandom tempGameRandom = GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed);
					PassiveEffect.ValueModifierTypes modifier = this.Modifier;
					if (modifier <= PassiveEffect.ValueModifierTypes.base_subtract)
					{
						_base_value = ((MinEventParams.CachedEventParam.Seed != 0) ? tempGameRandom.RandomRange(this.Values[0], this.Values[1]) : ((this.Values[0] + this.Values[1]) * 0.5f));
						return string.Format("{0}: [REPLACE_COLOR]{1}{2}[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_base_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.base_add) ? "+" : "", _base_value.ToCultureInvariantString("0.0"));
					}
					if (modifier - PassiveEffect.ValueModifierTypes.perc_set <= 2)
					{
						_perc_value = ((MinEventParams.CachedEventParam.Seed != 0) ? tempGameRandom.RandomRange(this.Values[0], this.Values[1]) : ((this.Values[0] + this.Values[1]) * 0.5f));
						return string.Format("{0}: [REPLACE_COLOR]{1}{2}%[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_perc_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.perc_add) ? "+" : "", (_perc_value * 100f).ToCultureInvariantString("0.0"));
					}
				}
				else if (this.Values.Length == 1 && this.Levels.Length == 2 && PassiveEffect.InLevelRange(_level, this.Levels[0], this.Levels[1]))
				{
					PassiveEffect.ValueModifierTypes modifier = this.Modifier;
					if (modifier <= PassiveEffect.ValueModifierTypes.base_subtract)
					{
						_base_value = this.Values[0];
						return string.Format("{0}: [REPLACE_COLOR]{1}{2}[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_base_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.base_add) ? "+" : "", _base_value.ToCultureInvariantString("0.0"));
					}
					if (modifier - PassiveEffect.ValueModifierTypes.perc_set <= 2)
					{
						_perc_value = this.Values[0];
						return string.Format("{0}: [REPLACE_COLOR]{1}{2}%[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_perc_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.perc_add) ? "+" : "", (_perc_value * 100f).ToCultureInvariantString("0.0"));
					}
				}
			}
		}
		else if (this.Values != null)
		{
			if (this.Values.Length == 1)
			{
				PassiveEffect.ValueModifierTypes modifier = this.Modifier;
				if (modifier <= PassiveEffect.ValueModifierTypes.base_subtract)
				{
					_base_value = this.Values[0];
					return string.Format("{0}: [REPLACE_COLOR]{1}{2}[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_base_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.base_add) ? "+" : "", _base_value.ToCultureInvariantString("0.0"));
				}
				if (modifier - PassiveEffect.ValueModifierTypes.perc_set <= 2)
				{
					_perc_value = this.Values[0];
					return string.Format("{0}: [REPLACE_COLOR]{1}{2}%[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_perc_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.perc_add) ? "+" : "", (_perc_value * 100f).ToCultureInvariantString("0.0"));
				}
			}
			else if (this.Values.Length == 2)
			{
				GameRandom tempGameRandom2 = GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed);
				PassiveEffect.ValueModifierTypes modifier = this.Modifier;
				if (modifier <= PassiveEffect.ValueModifierTypes.base_subtract)
				{
					_base_value = tempGameRandom2.RandomRange(this.Values[0], this.Values[1]);
					return string.Format("{0}: [REPLACE_COLOR]{1}{2}[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_base_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.base_add) ? "+" : "", _base_value.ToCultureInvariantString("0.0"));
				}
				if (modifier - PassiveEffect.ValueModifierTypes.perc_set <= 2)
				{
					_perc_value = tempGameRandom2.RandomRange(this.Values[0], this.Values[1]);
					return string.Format("{0}: [REPLACE_COLOR]{1}{2}%[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_perc_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.perc_add) ? "+" : "", (_perc_value * 100f).ToCultureInvariantString("0.0"));
				}
			}
			else
			{
				PassiveEffect.ValueModifierTypes modifier = this.Modifier;
				if (modifier <= PassiveEffect.ValueModifierTypes.base_subtract)
				{
					_base_value = this.Values[0];
					return string.Format("{0}: [REPLACE_COLOR]{1}{2}[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_base_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.base_add) ? "+" : "", _base_value.ToCultureInvariantString("0.0"));
				}
				if (modifier - PassiveEffect.ValueModifierTypes.perc_set <= 2)
				{
					_perc_value = this.Values[0];
					return string.Format("{0}: [REPLACE_COLOR]{1}{2}%[-]\n", this.Type.ToStringCached<PassiveEffects>(), (_perc_value > 0f && this.Modifier == PassiveEffect.ValueModifierTypes.perc_add) ? "+" : "", (_perc_value * 100f).ToCultureInvariantString("0.0"));
				}
			}
		}
		return null;
	}

	// Token: 0x040026F0 RID: 9968
	public PassiveEffects Type;

	// Token: 0x040026F1 RID: 9969
	public PassiveEffect.ValueModifierTypes Modifier;

	// Token: 0x040026F2 RID: 9970
	public string[] CVarValues;

	// Token: 0x040026F3 RID: 9971
	public float[] Values;

	// Token: 0x040026F4 RID: 9972
	public float[] Levels;

	// Token: 0x040026F5 RID: 9973
	public bool OrCompare;

	// Token: 0x040026F6 RID: 9974
	public List<IRequirement> Requirements;

	// Token: 0x040026F7 RID: 9975
	public FastTags<TagGroup.Global> Tags;

	// Token: 0x040026F8 RID: 9976
	public bool MatchAnyTags = true;

	// Token: 0x040026F9 RID: 9977
	public bool InvertTagCheck;

	// Token: 0x0200061E RID: 1566
	public enum ValueModifierTypes
	{
		// Token: 0x040026FB RID: 9979
		base_set,
		// Token: 0x040026FC RID: 9980
		base_add,
		// Token: 0x040026FD RID: 9981
		base_subtract,
		// Token: 0x040026FE RID: 9982
		perc_set,
		// Token: 0x040026FF RID: 9983
		perc_add,
		// Token: 0x04002700 RID: 9984
		perc_subtract,
		// Token: 0x04002701 RID: 9985
		COUNT
	}
}
