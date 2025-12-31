using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005EE RID: 1518
[Preserve]
public class IsCorpse : TargetedCompareRequirementBase
{
	// Token: 0x06002FCC RID: 12236 RVA: 0x00146B97 File Offset: 0x00144D97
	public override bool IsValid(MinEventParams _params)
	{
		if (base.IsValid(_params) && this.target.IsCorpse())
		{
			return !this.invert;
		}
		return this.invert;
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x00146BBF File Offset: 0x00144DBF
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Entity {0}IsCorpse", this.invert ? "NOT " : ""));
	}
}
