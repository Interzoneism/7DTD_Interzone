using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005CD RID: 1485
[Preserve]
public class IsStatAtMax : TargetedCompareRequirementBase
{
	// Token: 0x06002F6D RID: 12141 RVA: 0x00145244 File Offset: 0x00143444
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		switch (this.stat)
		{
		case IsStatAtMax.StatTypes.Health:
			if (this.target.Stats.Health.Max - this.target.Stats.Health.Value < 0.1f)
			{
				return !this.invert;
			}
			return this.invert;
		case IsStatAtMax.StatTypes.Stamina:
			if (this.target.Stats.Stamina.Max - this.target.Stats.Stamina.Value < 0.1f)
			{
				return !this.invert;
			}
			return this.invert;
		case IsStatAtMax.StatTypes.Food:
			if (this.target.Stats.Food.Max - this.target.Stats.Food.Value < 0.1f)
			{
				return !this.invert;
			}
			return this.invert;
		case IsStatAtMax.StatTypes.Water:
			if (this.target.Stats.Water.Max - this.target.Stats.Water.Value < 0.1f)
			{
				return !this.invert;
			}
			return this.invert;
		default:
			return false;
		}
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x00145390 File Offset: 0x00143590
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "stat")
		{
			this.stat = EnumUtils.Parse<IsStatAtMax.StatTypes>(_attribute.Value, true);
			return true;
		}
		return flag;
	}

	// Token: 0x0400268C RID: 9868
	[PublicizedFrom(EAccessModifier.Protected)]
	public IsStatAtMax.StatTypes stat;

	// Token: 0x020005CE RID: 1486
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum StatTypes
	{
		// Token: 0x0400268E RID: 9870
		None,
		// Token: 0x0400268F RID: 9871
		Health,
		// Token: 0x04002690 RID: 9872
		Stamina,
		// Token: 0x04002691 RID: 9873
		Food,
		// Token: 0x04002692 RID: 9874
		Water
	}
}
