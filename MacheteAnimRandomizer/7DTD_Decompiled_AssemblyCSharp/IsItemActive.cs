using System;
using UnityEngine.Scripting;

// Token: 0x020005DB RID: 1499
[Preserve]
public class IsItemActive : TargetedCompareRequirementBase
{
	// Token: 0x06002F8F RID: 12175 RVA: 0x00145CF4 File Offset: 0x00143EF4
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (_params.ItemValue == null)
		{
			return false;
		}
		if (_params.ItemValue.Activated > 0)
		{
			return !this.invert;
		}
		return this.invert;
	}
}
