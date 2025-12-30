using System;
using UnityEngine.Scripting;

// Token: 0x020005D7 RID: 1495
[Preserve]
public class IsFPV : TargetedCompareRequirementBase
{
	// Token: 0x06002F87 RID: 12167 RVA: 0x00145BBC File Offset: 0x00143DBC
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!(this.target as EntityPlayerLocal != null))
		{
			return this.invert;
		}
		if (!this.invert)
		{
			return (this.target as EntityPlayerLocal).bFirstPersonView;
		}
		return !(this.target as EntityPlayerLocal).bFirstPersonView;
	}
}
