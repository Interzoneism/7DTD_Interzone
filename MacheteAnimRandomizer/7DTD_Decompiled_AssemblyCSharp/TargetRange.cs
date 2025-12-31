using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000604 RID: 1540
[Preserve]
public class TargetRange : TargetedCompareRequirementBase
{
	// Token: 0x0600300F RID: 12303 RVA: 0x001479A4 File Offset: 0x00145BA4
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (_params.ItemValue.IsEmpty())
		{
			return false;
		}
		if (!(_params.Self != null) || !(_params.Other != null))
		{
			return false;
		}
		if (!(_params.Self != _params.Other))
		{
			return false;
		}
		if (this.invert)
		{
			return !RequirementBase.compareValues(_params.Self.GetDistance(_params.Other), this.operation, this.value);
		}
		return RequirementBase.compareValues(_params.Self.GetDistance(_params.Other), this.operation, this.value);
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x00147A4E File Offset: 0x00145C4E
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("TargetRange: {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}
}
