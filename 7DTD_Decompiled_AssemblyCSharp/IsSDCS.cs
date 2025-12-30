using System;
using UnityEngine.Scripting;

// Token: 0x020005DA RID: 1498
[Preserve]
public class IsSDCS : TargetedCompareRequirementBase
{
	// Token: 0x06002F8D RID: 12173 RVA: 0x00145CB0 File Offset: 0x00143EB0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		EntityAlive target = this.target;
		bool flag = ((target != null) ? target.emodel : null) as EModelSDCS != null;
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}
}
