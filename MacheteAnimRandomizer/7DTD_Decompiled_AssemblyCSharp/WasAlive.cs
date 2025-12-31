using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005ED RID: 1517
[Preserve]
public class WasAlive : TargetedCompareRequirementBase
{
	// Token: 0x06002FC9 RID: 12233 RVA: 0x00146B31 File Offset: 0x00144D31
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
			return !this.target.WasAlive();
		}
		return this.target.WasAlive();
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x00146B70 File Offset: 0x00144D70
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Entity Was {0}Alive", this.invert ? "NOT " : ""));
	}
}
