using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005D6 RID: 1494
[Preserve]
public class IsMale : TargetedCompareRequirementBase
{
	// Token: 0x06002F84 RID: 12164 RVA: 0x00145B65 File Offset: 0x00143D65
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return this.target.IsMale;
		}
		return !this.target.IsMale;
	}

	// Token: 0x06002F85 RID: 12165 RVA: 0x00145B94 File Offset: 0x00143D94
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0}Male", this.invert ? "NOT " : ""));
	}
}
