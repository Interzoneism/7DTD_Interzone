using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005F7 RID: 1527
[Preserve]
public class IsDay : TargetedCompareRequirementBase
{
	// Token: 0x06002FEA RID: 12266 RVA: 0x00147252 File Offset: 0x00145452
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return GameManager.Instance.World.IsDaytime();
		}
		return !GameManager.Instance.World.IsDaytime();
	}

	// Token: 0x06002FEB RID: 12267 RVA: 0x00147289 File Offset: 0x00145489
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0} night time", this.invert ? "NOT " : ""));
	}
}
