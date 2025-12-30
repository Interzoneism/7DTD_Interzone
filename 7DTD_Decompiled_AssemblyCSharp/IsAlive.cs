using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005EC RID: 1516
[Preserve]
public class IsAlive : TargetedCompareRequirementBase
{
	// Token: 0x06002FC6 RID: 12230 RVA: 0x00146ACB File Offset: 0x00144CCB
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!(this.target != null))
		{
			return false;
		}
		if (this.invert)
		{
			return !this.target.IsAlive();
		}
		return this.target.IsAlive();
	}

	// Token: 0x06002FC7 RID: 12231 RVA: 0x00146B0A File Offset: 0x00144D0A
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Entity {0}Alive", this.invert ? "NOT " : ""));
	}
}
