using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200060E RID: 1550
[Preserve]
public class ArmorGroupLowestQuality : TargetedCompareRequirementBase
{
	// Token: 0x06003033 RID: 12339 RVA: 0x001481F0 File Offset: 0x001463F0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		int armorGroupLowestQuality = this.target.equipment.GetArmorGroupLowestQuality(this.armorGroupName);
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)armorGroupLowestQuality, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)armorGroupLowestQuality, this.operation, this.value);
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x00148251 File Offset: 0x00146451
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("ArmorGroupLowestQuality: {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x00148290 File Offset: 0x00146490
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "group_name")
		{
			this.armorGroupName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x040026C7 RID: 9927
	[PublicizedFrom(EAccessModifier.Private)]
	public string armorGroupName;
}
