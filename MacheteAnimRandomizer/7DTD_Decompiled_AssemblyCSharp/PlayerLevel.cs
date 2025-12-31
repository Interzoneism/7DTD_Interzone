using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005EF RID: 1519
[Preserve]
public class PlayerLevel : TargetedCompareRequirementBase
{
	// Token: 0x06002FCF RID: 12239 RVA: 0x00146BE8 File Offset: 0x00144DE8
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.target.Progression == null)
		{
			return false;
		}
		int level = this.target.Progression.GetLevel();
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)level, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)level, this.operation, this.value);
	}

	// Token: 0x06002FD0 RID: 12240 RVA: 0x00146C52 File Offset: 0x00144E52
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Player level {0} {1} {2}", this.invert ? "NOT" : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}
}
