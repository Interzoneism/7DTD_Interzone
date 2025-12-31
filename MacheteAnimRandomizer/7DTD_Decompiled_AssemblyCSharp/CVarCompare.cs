using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005CC RID: 1484
[Preserve]
public class CVarCompare : TargetedCompareRequirementBase
{
	// Token: 0x06002F69 RID: 12137 RVA: 0x00145194 File Offset: 0x00143394
	public override bool IsValid(MinEventParams _params)
	{
		return base.IsValid(_params) && this.invert != RequirementBase.compareValues(this.target.Buffs.GetCustomVar(this.cvarCompareName), this.operation, this.value);
	}

	// Token: 0x06002F6A RID: 12138 RVA: 0x001451D3 File Offset: 0x001433D3
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("cvar.{0} {1} {2}", this.cvarCompareName, this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x00145204 File Offset: 0x00143404
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "cvar")
		{
			this.cvarCompareName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x0400268B RID: 9867
	[PublicizedFrom(EAccessModifier.Protected)]
	public string cvarCompareName;
}
