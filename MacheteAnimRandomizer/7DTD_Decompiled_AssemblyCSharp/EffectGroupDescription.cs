using System;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x0200061B RID: 1563
public class EffectGroupDescription
{
	// Token: 0x170004A9 RID: 1193
	// (get) Token: 0x0600306E RID: 12398 RVA: 0x0014A004 File Offset: 0x00148204
	public string Description
	{
		get
		{
			if (Localization.Exists(this.DescriptionKey, false))
			{
				return Localization.Get(this.DescriptionKey, false);
			}
			return this.CustomDescription;
		}
	}

	// Token: 0x170004AA RID: 1194
	// (get) Token: 0x0600306F RID: 12399 RVA: 0x0014A027 File Offset: 0x00148227
	public string LongDescription
	{
		get
		{
			return Localization.Get(this.LongDescriptionKey, false);
		}
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x0014A035 File Offset: 0x00148235
	public EffectGroupDescription(int _minLevel, int _maxLevel, string _desc_key, string _description, string _long_desc_key)
	{
		this.MinLevel = _minLevel;
		this.MaxLevel = _maxLevel;
		this.DescriptionKey = _desc_key;
		this.CustomDescription = _description;
		this.LongDescriptionKey = _long_desc_key;
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x0014A064 File Offset: 0x00148264
	public static EffectGroupDescription ParseDescription(XElement _element)
	{
		if (!_element.HasAttribute("level") || (!_element.HasAttribute("desc_key") && !_element.HasAttribute("desc_base")))
		{
			return null;
		}
		int minLevel;
		int maxLevel;
		if (_element.GetAttribute("level").Contains(","))
		{
			string[] array = _element.GetAttribute("level").Split(',', StringSplitOptions.None);
			if (array.Length < 1)
			{
				return null;
			}
			if (array.Length == 1)
			{
				maxLevel = (minLevel = StringParsers.ParseSInt32(array[0], 0, -1, NumberStyles.Integer));
			}
			else
			{
				minLevel = StringParsers.ParseSInt32(array[0], 0, -1, NumberStyles.Integer);
				maxLevel = StringParsers.ParseSInt32(array[1], 0, -1, NumberStyles.Integer);
			}
		}
		else
		{
			maxLevel = (minLevel = StringParsers.ParseSInt32(_element.GetAttribute("level"), 0, -1, NumberStyles.Integer));
		}
		return new EffectGroupDescription(minLevel, maxLevel, _element.GetAttribute("desc_key"), _element.GetAttribute("desc_base"), _element.HasAttribute("long_desc_key") ? _element.GetAttribute("long_desc_key") : "");
	}

	// Token: 0x040026EB RID: 9963
	public readonly int MinLevel;

	// Token: 0x040026EC RID: 9964
	public readonly int MaxLevel;

	// Token: 0x040026ED RID: 9965
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string DescriptionKey;

	// Token: 0x040026EE RID: 9966
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string CustomDescription;

	// Token: 0x040026EF RID: 9967
	public readonly string LongDescriptionKey;
}
