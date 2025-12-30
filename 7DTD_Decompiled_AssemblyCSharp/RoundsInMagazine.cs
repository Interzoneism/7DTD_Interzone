using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000600 RID: 1536
[Preserve]
public class RoundsInMagazine : TargetedCompareRequirementBase
{
	// Token: 0x06003003 RID: 12291 RVA: 0x00147744 File Offset: 0x00145944
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (_params.ItemValue.IsEmpty() || !(_params.ItemValue.ItemClass.Actions[0] is ItemActionRanged))
		{
			return false;
		}
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)_params.ItemValue.Meta, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)_params.ItemValue.Meta, this.operation, this.value);
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x001477CA File Offset: 0x001459CA
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Rounds in Magazine: {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}
}
