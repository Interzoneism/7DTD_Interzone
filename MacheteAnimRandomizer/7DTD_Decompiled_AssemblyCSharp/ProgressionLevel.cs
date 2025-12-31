using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005EA RID: 1514
[Preserve]
public class ProgressionLevel : TargetedCompareRequirementBase
{
	// Token: 0x06002FBF RID: 12223 RVA: 0x00146908 File Offset: 0x00144B08
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.target.Progression != null)
		{
			this.pv = this.target.Progression.GetProgressionValue(this.progressionId);
			if (this.pv != null)
			{
				if (this.invert)
				{
					return !RequirementBase.compareValues(this.pv.GetCalculatedLevel(this.target), this.operation, this.value);
				}
				return RequirementBase.compareValues(this.pv.GetCalculatedLevel(this.target), this.operation, this.value);
			}
		}
		return false;
	}

	// Token: 0x06002FC0 RID: 12224 RVA: 0x001469A4 File Offset: 0x00144BA4
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("'{1}' level {0} {2} {3}", new object[]
		{
			this.invert ? "NOT" : "",
			this.progressionName,
			this.operation.ToStringCached<RequirementBase.OperationTypes>(),
			this.value.ToCultureInvariantString()
		}));
	}

	// Token: 0x06002FC1 RID: 12225 RVA: 0x00146A04 File Offset: 0x00144C04
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "progression_name")
		{
			this.progressionName = _attribute.Value;
			this.progressionId = Progression.CalcId(this.progressionName);
			return true;
		}
		return flag;
	}

	// Token: 0x040026AE RID: 9902
	[PublicizedFrom(EAccessModifier.Private)]
	public string progressionName = string.Empty;

	// Token: 0x040026AF RID: 9903
	[PublicizedFrom(EAccessModifier.Private)]
	public int progressionId;

	// Token: 0x040026B0 RID: 9904
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionValue pv;
}
