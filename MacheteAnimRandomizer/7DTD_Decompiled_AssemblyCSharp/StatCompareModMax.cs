using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005D2 RID: 1490
[Preserve]
public class StatCompareModMax : StatCompareCurrent
{
	// Token: 0x06002F78 RID: 12152 RVA: 0x00145668 File Offset: 0x00143868
	public override bool Compare(MinEventParams _params)
	{
		float modifiedMax;
		switch (this.stat)
		{
		case StatCompareCurrent.StatTypes.Health:
			modifiedMax = this.target.Stats.Health.ModifiedMax;
			break;
		case StatCompareCurrent.StatTypes.Stamina:
			modifiedMax = this.target.Stats.Stamina.ModifiedMax;
			break;
		case StatCompareCurrent.StatTypes.Food:
			modifiedMax = this.target.Stats.Food.ModifiedMax;
			break;
		case StatCompareCurrent.StatTypes.Water:
			modifiedMax = this.target.Stats.Water.ModifiedMax;
			break;
		default:
			return false;
		}
		return this.invert != RequirementBase.compareValues(modifiedMax, this.operation, this.value);
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x00145718 File Offset: 0x00143918
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
