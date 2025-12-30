using System;
using UnityEngine.Scripting;

// Token: 0x020005FC RID: 1532
[Preserve]
public class HoldingItemBroken : TargetedCompareRequirementBase
{
	// Token: 0x06002FFA RID: 12282 RVA: 0x001474CC File Offset: 0x001456CC
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.target == null)
		{
			return false;
		}
		bool flag = this.target.inventory.holdingItemItemValue.PercentUsesLeft <= 0f;
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}
}
