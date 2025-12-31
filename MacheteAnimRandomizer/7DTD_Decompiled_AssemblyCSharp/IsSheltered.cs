using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000603 RID: 1539
[Preserve]
public class IsSheltered : TargetedCompareRequirementBase
{
	// Token: 0x0600300C RID: 12300 RVA: 0x0014793C File Offset: 0x00145B3C
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		EntityPlayerLocal entityPlayerLocal = this.target as EntityPlayerLocal;
		return entityPlayerLocal != null && this.invert != entityPlayerLocal.shelterPercent > 0f;
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x0014797D File Offset: 0x00145B7D
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("{0}sheltered", this.invert ? "NOT " : ""));
	}
}
