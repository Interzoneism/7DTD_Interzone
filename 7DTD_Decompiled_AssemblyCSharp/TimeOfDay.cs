using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020005FA RID: 1530
[Preserve]
public class TimeOfDay : TargetedCompareRequirementBase
{
	// Token: 0x06002FF3 RID: 12275 RVA: 0x00147328 File Offset: 0x00145528
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.isSetup)
		{
			this.timeValue = GameUtils.DayTimeToWorldTime(1, (int)this.value / 100, (int)this.value % 100);
			this.isSetup = true;
		}
		ulong num = GameManager.Instance.World.worldTime % 24000UL;
		if (this.invert)
		{
			return !RequirementBase.compareValues(num, this.operation, this.timeValue);
		}
		return RequirementBase.compareValues(num, this.operation, this.timeValue);
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x001473BE File Offset: 0x001455BE
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("time of day {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}

	// Token: 0x040026BA RID: 9914
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isSetup;

	// Token: 0x040026BB RID: 9915
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong timeValue;
}
