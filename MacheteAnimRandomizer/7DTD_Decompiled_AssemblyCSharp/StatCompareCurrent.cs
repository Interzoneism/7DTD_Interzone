using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005CF RID: 1487
[Preserve]
public class StatCompareCurrent : TargetedCompareRequirementBase
{
	// Token: 0x06002F70 RID: 12144 RVA: 0x001453D4 File Offset: 0x001435D4
	public override bool IsValid(MinEventParams _params)
	{
		return base.IsValid(_params) && this.Compare(_params);
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x001453E8 File Offset: 0x001435E8
	public virtual bool Compare(MinEventParams _params)
	{
		float valueA;
		switch (this.stat)
		{
		case StatCompareCurrent.StatTypes.Health:
			valueA = (float)this.target.Health;
			break;
		case StatCompareCurrent.StatTypes.Stamina:
			valueA = this.target.Stamina;
			break;
		case StatCompareCurrent.StatTypes.Food:
			valueA = this.target.Stats.Food.Value;
			break;
		case StatCompareCurrent.StatTypes.Water:
			valueA = this.target.Stats.Water.Value;
			break;
		case StatCompareCurrent.StatTypes.Armor:
			valueA = this.target.equipment.CurrentLowestDurability;
			break;
		default:
			return false;
		}
		return this.invert != RequirementBase.compareValues(valueA, this.operation, this.value);
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x0014549C File Offset: 0x0014369C
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("stat '{1}' {0} {2} {3}", new object[]
		{
			this.invert ? "NOT" : "",
			this.stat.ToStringCached<StatCompareCurrent.StatTypes>(),
			this.operation.ToStringCached<RequirementBase.OperationTypes>(),
			this.value.ToCultureInvariantString()
		}));
	}

	// Token: 0x06002F73 RID: 12147 RVA: 0x00145504 File Offset: 0x00143704
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "stat")
		{
			this.stat = EnumUtils.Parse<StatCompareCurrent.StatTypes>(_attribute.Value, true);
			return true;
		}
		return flag;
	}

	// Token: 0x04002693 RID: 9875
	[PublicizedFrom(EAccessModifier.Protected)]
	public StatCompareCurrent.StatTypes stat;

	// Token: 0x020005D0 RID: 1488
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum StatTypes
	{
		// Token: 0x04002695 RID: 9877
		None,
		// Token: 0x04002696 RID: 9878
		Health,
		// Token: 0x04002697 RID: 9879
		Stamina,
		// Token: 0x04002698 RID: 9880
		Food,
		// Token: 0x04002699 RID: 9881
		Water,
		// Token: 0x0400269A RID: 9882
		Armor
	}
}
