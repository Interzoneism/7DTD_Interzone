using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005F8 RID: 1528
[Preserve]
public class IsNight : TargetedCompareRequirementBase
{
	// Token: 0x06002FED RID: 12269 RVA: 0x001472B0 File Offset: 0x001454B0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return !GameManager.Instance.World.IsDaytime();
		}
		return GameManager.Instance.World.IsDaytime();
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x00147289 File Offset: 0x00145489
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0} night time", this.invert ? "NOT " : ""));
	}
}
