using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000602 RID: 1538
[Preserve]
public class IsIndoors : TargetedCompareRequirementBase
{
	// Token: 0x06003009 RID: 12297 RVA: 0x001478C0 File Offset: 0x00145AC0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return this.target.Stats.AmountEnclosed > 0f;
		}
		return this.target.Stats.AmountEnclosed <= 0f;
	}

	// Token: 0x0600300A RID: 12298 RVA: 0x00147912 File Offset: 0x00145B12
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("{0}indoors", this.invert ? "NOT " : ""));
	}
}
