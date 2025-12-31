using System;
using UnityEngine.Scripting;

// Token: 0x020005D8 RID: 1496
[Preserve]
public class IsLocalPlayer : TargetedCompareRequirementBase
{
	// Token: 0x06002F89 RID: 12169 RVA: 0x00145C1F File Offset: 0x00143E1F
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return this.target as EntityPlayerLocal != null;
		}
		return !(this.target as EntityPlayerLocal != null);
	}
}
