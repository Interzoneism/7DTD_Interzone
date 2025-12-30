using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005D1 RID: 1489
[Preserve]
public class StatCompareMax : StatCompareCurrent
{
	// Token: 0x06002F75 RID: 12149 RVA: 0x00145548 File Offset: 0x00143748
	public override bool Compare(MinEventParams _params)
	{
		float max;
		switch (this.stat)
		{
		case StatCompareCurrent.StatTypes.Health:
			max = this.target.Stats.Health.Max;
			break;
		case StatCompareCurrent.StatTypes.Stamina:
			max = this.target.Stats.Stamina.Max;
			break;
		case StatCompareCurrent.StatTypes.Food:
			max = this.target.Stats.Food.Max;
			break;
		case StatCompareCurrent.StatTypes.Water:
			max = this.target.Stats.Water.Max;
			break;
		default:
			return false;
		}
		return this.invert != RequirementBase.compareValues(max, this.operation, this.value);
	}

	// Token: 0x06002F76 RID: 12150 RVA: 0x001455F8 File Offset: 0x001437F8
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
