using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005E9 RID: 1513
[Preserve]
public class RequirementItemTier : RequirementBase
{
	// Token: 0x06002FBC RID: 12220 RVA: 0x00146860 File Offset: 0x00144A60
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (_params.ItemValue == null)
		{
			return false;
		}
		if (!this.invert)
		{
			return RequirementBase.compareValues((float)_params.ItemValue.Quality, this.operation, this.value);
		}
		return !RequirementBase.compareValues((float)_params.ItemValue.Quality, this.operation, this.value);
	}

	// Token: 0x06002FBD RID: 12221 RVA: 0x001468C8 File Offset: 0x00144AC8
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Item tier {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}
}
