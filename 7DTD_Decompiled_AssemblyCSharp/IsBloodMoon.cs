using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005F9 RID: 1529
[Preserve]
public class IsBloodMoon : TargetedCompareRequirementBase
{
	// Token: 0x06002FF0 RID: 12272 RVA: 0x001472E7 File Offset: 0x001454E7
	public override bool IsValid(MinEventParams _params)
	{
		return base.IsValid(_params) && (SkyManager.IsBloodMoonVisible() ^ this.invert);
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x00147300 File Offset: 0x00145500
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0} blood moon", this.invert ? "NOT " : ""));
	}
}
