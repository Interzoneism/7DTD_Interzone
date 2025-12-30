using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000612 RID: 1554
[Preserve]
public class GameStatInt : TargetedCompareRequirementBase
{
	// Token: 0x06003041 RID: 12353 RVA: 0x00148518 File Offset: 0x00146718
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		int @int = GameStats.GetInt(this.GameStat);
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)@int, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)@int, this.operation, this.value);
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x00148570 File Offset: 0x00146770
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "gamestat")
		{
			this.GameStat = Enum.Parse<EnumGameStats>(_attribute.Value);
			return true;
		}
		return flag;
	}

	// Token: 0x040026CB RID: 9931
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumGameStats GameStat = EnumGameStats.AnimalCount;
}
