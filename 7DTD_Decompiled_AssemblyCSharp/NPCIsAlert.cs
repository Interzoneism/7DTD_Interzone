using System;
using System.Collections.Generic;

// Token: 0x020005EB RID: 1515
public class NPCIsAlert : TargetedCompareRequirementBase
{
	// Token: 0x06002FC3 RID: 12227 RVA: 0x00146A66 File Offset: 0x00144C66
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.target.IsAlive())
		{
			return false;
		}
		if (this.invert)
		{
			return !this.target.IsAlert;
		}
		return this.target.IsAlert;
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x00146AA4 File Offset: 0x00144CA4
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Entity {0}Alert", this.invert ? "NOT " : ""));
	}
}
