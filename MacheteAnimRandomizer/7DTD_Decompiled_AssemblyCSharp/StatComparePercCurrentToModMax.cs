using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005D4 RID: 1492
[Preserve]
public class StatComparePercCurrentToModMax : StatCompareCurrent
{
	// Token: 0x06002F7E RID: 12158 RVA: 0x0014595C File Offset: 0x00143B5C
	public override bool Compare(MinEventParams _params)
	{
		StatCompareCurrent.StatTypes stat = this.stat;
		if (stat == StatCompareCurrent.StatTypes.Health)
		{
			float modifiedMax = this.target.Stats.Health.ModifiedMax;
			return modifiedMax > 0f && this.invert != RequirementBase.compareValues((float)this.target.Health / modifiedMax, this.operation, this.value);
		}
		if (stat != StatCompareCurrent.StatTypes.Stamina)
		{
			return false;
		}
		float modifiedMax2 = this.target.Stats.Stamina.ModifiedMax;
		return modifiedMax2 > 0f && this.invert != RequirementBase.compareValues(this.target.Stamina / modifiedMax2, this.operation, this.value);
	}

	// Token: 0x06002F7F RID: 12159 RVA: 0x00145A14 File Offset: 0x00143C14
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
