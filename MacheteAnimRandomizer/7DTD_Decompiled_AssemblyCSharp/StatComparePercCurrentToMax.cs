using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005D3 RID: 1491
[Preserve]
public class StatComparePercCurrentToMax : StatCompareCurrent
{
	// Token: 0x06002F7B RID: 12155 RVA: 0x00145780 File Offset: 0x00143980
	public override bool Compare(MinEventParams _params)
	{
		switch (this.stat)
		{
		case StatCompareCurrent.StatTypes.Health:
		{
			float max = this.target.Stats.Health.Max;
			return max > 0f && this.invert != RequirementBase.compareValues((float)this.target.Health / max, this.operation, this.value);
		}
		case StatCompareCurrent.StatTypes.Stamina:
		{
			float max2 = this.target.Stats.Stamina.Max;
			return max2 > 0f && this.invert != RequirementBase.compareValues(this.target.Stamina / max2, this.operation, this.value);
		}
		case StatCompareCurrent.StatTypes.Food:
		{
			float max3 = this.target.Stats.Food.Max;
			return max3 > 0f && this.invert != RequirementBase.compareValues(this.target.Stats.Food.Value / max3, this.operation, this.value);
		}
		case StatCompareCurrent.StatTypes.Water:
		{
			float max4 = this.target.Stats.Water.Max;
			return max4 > 0f && this.invert != RequirementBase.compareValues(this.target.Stats.Water.Value / max4, this.operation, this.value);
		}
		default:
			return false;
		}
	}

	// Token: 0x06002F7C RID: 12156 RVA: 0x001458F4 File Offset: 0x00143AF4
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
