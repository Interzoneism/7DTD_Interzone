using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005D5 RID: 1493
[Preserve]
public class StatComparePercModMaxToMax : StatCompareCurrent
{
	// Token: 0x06002F81 RID: 12161 RVA: 0x00145A7C File Offset: 0x00143C7C
	public override bool Compare(MinEventParams _params)
	{
		StatCompareCurrent.StatTypes stat = this.stat;
		if (stat != StatCompareCurrent.StatTypes.Health)
		{
			return stat == StatCompareCurrent.StatTypes.Stamina && this.invert != RequirementBase.compareValues(this.target.Stats.Stamina.ModifiedMaxPercent, this.operation, this.value);
		}
		return this.invert != RequirementBase.compareValues(this.target.Stats.Health.ModifiedMaxPercent, this.operation, this.value);
	}

	// Token: 0x06002F82 RID: 12162 RVA: 0x00145B00 File Offset: 0x00143D00
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("stat '{1}'% {0}{2} {3}", new object[]
		{
			this.invert ? "NOT " : "",
			this.stat.ToStringCached<StatCompareCurrent.StatTypes>(),
			this.operation.ToStringCached<RequirementBase.OperationTypes>(),
			this.value.ToCultureInvariantString()
		}));
	}
}
