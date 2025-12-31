using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005FB RID: 1531
[Preserve]
public class InBiome : TargetedCompareRequirementBase
{
	// Token: 0x06002FF6 RID: 12278 RVA: 0x001473FC File Offset: 0x001455FC
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (_params.Biome == null)
		{
			return false;
		}
		if (!this.invert)
		{
			return this.biomeID == (int)_params.Biome.m_Id;
		}
		return this.biomeID != (int)_params.Biome.m_Id;
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x00147450 File Offset: 0x00145650
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("{0}in biome {1}", this.invert ? "NOT " : "", this.biomeID));
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x00147484 File Offset: 0x00145684
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "biome")
		{
			this.biomeID = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
			return true;
		}
		return flag;
	}

	// Token: 0x040026BC RID: 9916
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomeID;
}
