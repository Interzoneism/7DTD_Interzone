using System;
using System.Collections.Generic;

// Token: 0x020005F0 RID: 1520
public class CompareLightLevel : TargetedCompareRequirementBase
{
	// Token: 0x06002FD2 RID: 12242 RVA: 0x00146C90 File Offset: 0x00144E90
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
		if (!this.invert)
		{
			return RequirementBase.compareValues(this.target.GetLightBrightness(), this.operation, this.value);
		}
		return !RequirementBase.compareValues(this.target.GetLightBrightness(), this.operation, this.value);
	}

	// Token: 0x06002FD3 RID: 12243 RVA: 0x00146CFC File Offset: 0x00144EFC
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("light level '{0}'% {1}{2} {3}", new object[]
		{
			this.target.GetLightBrightness().ToCultureInvariantString(),
			this.invert ? "NOT " : "",
			this.operation.ToStringCached<RequirementBase.OperationTypes>(),
			this.value.ToCultureInvariantString()
		}));
	}
}
