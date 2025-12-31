using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005E5 RID: 1509
[Preserve]
public class NotHasBuff : TargetedCompareRequirementBase
{
	// Token: 0x06002FAF RID: 12207 RVA: 0x001464B4 File Offset: 0x001446B4
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return !this.target.Buffs.HasBuff(this.buffName);
		}
		return this.target.Buffs.HasBuff(this.buffName);
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x00146504 File Offset: 0x00144704
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

	// Token: 0x06002FB1 RID: 12209 RVA: 0x00146547 File Offset: 0x00144747
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Target does {0}have buff '{1}'", (!this.invert) ? "NOT " : "", this.buffName));
	}

	// Token: 0x040026A7 RID: 9895
	[PublicizedFrom(EAccessModifier.Private)]
	public string buffName;
}
