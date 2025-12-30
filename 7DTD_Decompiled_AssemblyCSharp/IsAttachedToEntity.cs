using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000608 RID: 1544
[Preserve]
public class IsAttachedToEntity : TargetedCompareRequirementBase
{
	// Token: 0x0600301D RID: 12317 RVA: 0x00147CE1 File Offset: 0x00145EE1
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return this.target.AttachedToEntity != null;
		}
		return !(this.target.AttachedToEntity != null);
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x00147D1C File Offset: 0x00145F1C
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0}Attached To Entity", this.invert ? "NOT " : ""));
	}
}
