using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000609 RID: 1545
[Preserve]
public class IsOnLadder : TargetedCompareRequirementBase
{
	// Token: 0x06003020 RID: 12320 RVA: 0x00147D43 File Offset: 0x00145F43
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return this.target.IsInElevator();
		}
		return !this.target.IsInElevator();
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x00147D72 File Offset: 0x00145F72
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0} On Ladder", this.invert ? "NOT " : ""));
	}
}
