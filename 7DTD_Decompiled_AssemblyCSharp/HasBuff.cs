using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005E4 RID: 1508
[Preserve]
public class HasBuff : TargetedCompareRequirementBase
{
	// Token: 0x06002FAB RID: 12203 RVA: 0x001463F4 File Offset: 0x001445F4
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return this.target.Buffs.HasBuff(this.buffName);
		}
		return !this.target.Buffs.HasBuff(this.buffName);
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x00146444 File Offset: 0x00144644
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "buff")
		{
			this.buffName = _attribute.Value.ToLower();
			return true;
		}
		return flag;
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x00146487 File Offset: 0x00144687
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Target does {0}have buff '{1}'", this.invert ? "NOT " : "", this.buffName));
	}

	// Token: 0x040026A6 RID: 9894
	[PublicizedFrom(EAccessModifier.Private)]
	public string buffName;
}
