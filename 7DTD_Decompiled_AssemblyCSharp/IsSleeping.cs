using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005FD RID: 1533
[Preserve]
public class IsSleeping : TargetedCompareRequirementBase
{
	// Token: 0x06002FFC RID: 12284 RVA: 0x00147524 File Offset: 0x00145724
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		EntityEnemy entityEnemy = this.target as EntityEnemy;
		if (entityEnemy == null)
		{
			return false;
		}
		if (!this.invert)
		{
			return entityEnemy.IsSleeping;
		}
		return !entityEnemy.IsSleeping;
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x0014756B File Offset: 0x0014576B
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("{0}sleeping", this.invert ? "NOT " : ""));
	}
}
