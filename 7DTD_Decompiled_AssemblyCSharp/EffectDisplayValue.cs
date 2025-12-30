using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x0200061A RID: 1562
public class EffectDisplayValue
{
	// Token: 0x06003068 RID: 12392 RVA: 0x00149BAF File Offset: 0x00147DAF
	public EffectDisplayValue(string _name, float[] _value, float[] _levels, List<IRequirement> _requirements)
	{
		this.Name = _name;
		this.Values = _value;
		this.Levels = _levels;
		this.Requirements = _requirements;
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x00149BD4 File Offset: 0x00147DD4
	public bool IsValid(MinEventParams _params)
	{
		return this.canRun(_params);
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x00149BE0 File Offset: 0x00147DE0
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

	// Token: 0x0600306B RID: 12395 RVA: 0x00149C68 File Offset: 0x00147E68
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool InLevelRange(float _level, float _min, float _max)
	{
		return _level >= _min && _level <= _max;
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x00149C78 File Offset: 0x00147E78
	public float GetValue(int _level)
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
							if (EffectDisplayValue.InLevelRange((float)_level, this.Levels[i], this.Levels[i + 1]))
							{
								return Mathf.Lerp(this.Values[i], this.Values[i + 1], ((float)_level - this.Levels[i]) / (this.Levels[i + 1] - this.Levels[i]));
							}
						}
					}
					else if (this.Levels.Length >= 1 && (float)_level == this.Levels[0])
					{
						return this.Values[0];
					}
				}
				else if (this.Values.Length == 2 && this.Levels.Length == 1)
				{
					GameRandom tempGameRandom = GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed);
					if (MinEventParams.CachedEventParam.Seed == 0)
					{
						return (this.Values[0] + this.Values[1]) * 0.5f;
					}
					return tempGameRandom.RandomRange(this.Values[0], this.Values[1]);
				}
				else if (this.Values.Length == 1 && this.Levels.Length == 2 && EffectDisplayValue.InLevelRange((float)_level, this.Levels[0], this.Levels[1]))
				{
					return this.Values[0];
				}
			}
		}
		else if (this.Values != null)
		{
			if (this.Values.Length == 1)
			{
				return this.Values[0];
			}
			if (this.Values.Length == 2)
			{
				return GameRandomManager.Instance.GetTempGameRandom(MinEventParams.CachedEventParam.Seed).RandomRange(this.Values[0], this.Values[1]);
			}
			return this.Values[0];
		}
		return 0f;
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x00149E50 File Offset: 0x00148050
	public static EffectDisplayValue ParseDisplayValue(XElement _element)
	{
		if (!_element.HasAttribute("name") || !_element.HasAttribute("value"))
		{
			return null;
		}
		List<IRequirement> list = new List<IRequirement>();
		foreach (XElement element in _element.Elements("requirements"))
		{
			IRequirement requirement = RequirementBase.ParseRequirement(element);
			if (requirement != null)
			{
				list.Add(requirement);
			}
		}
		string attribute = _element.GetAttribute("value");
		float[] array = null;
		if (!string.IsNullOrEmpty(attribute))
		{
			if (attribute.Contains(","))
			{
				string[] array2 = attribute.Split(',', StringSplitOptions.None);
				array = new float[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					float num;
					if (StringParsers.TryParseFloat(array2[i], out num, 0, -1, NumberStyles.Any))
					{
						array[i] = num;
					}
				}
			}
			else
			{
				array = new float[]
				{
					StringParsers.ParseFloat(attribute, 0, -1, NumberStyles.Any)
				};
			}
		}
		string attribute2 = _element.GetAttribute("tier");
		float[] array3 = null;
		if (!string.IsNullOrEmpty(attribute2))
		{
			if (attribute2.Contains(","))
			{
				string[] array4 = attribute2.Split(',', StringSplitOptions.None);
				array3 = new float[array4.Length];
				for (int j = 0; j < array4.Length; j++)
				{
					array3[j] = StringParsers.ParseFloat(array4[j], 0, -1, NumberStyles.Any);
				}
			}
			else
			{
				array3 = new float[]
				{
					StringParsers.ParseFloat(attribute2, 0, -1, NumberStyles.Any)
				};
			}
		}
		return new EffectDisplayValue(_element.GetAttribute("name"), array, array3, list);
	}

	// Token: 0x040026E6 RID: 9958
	public string Name;

	// Token: 0x040026E7 RID: 9959
	public float[] Values;

	// Token: 0x040026E8 RID: 9960
	public float[] Levels;

	// Token: 0x040026E9 RID: 9961
	public List<IRequirement> Requirements;

	// Token: 0x040026EA RID: 9962
	public bool OrCompare;
}
