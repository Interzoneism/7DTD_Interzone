using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000613 RID: 1555
[Preserve]
public class GameStatFloat : TargetedCompareRequirementBase
{
	// Token: 0x06003044 RID: 12356 RVA: 0x001485C4 File Offset: 0x001467C4
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		float @float = GameStats.GetFloat(this.GameStat);
		if (this.invert)
		{
			return !RequirementBase.compareValues(@float, this.operation, this.value);
		}
		return RequirementBase.compareValues(@float, this.operation, this.value);
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x00148618 File Offset: 0x00146818
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

	// Token: 0x040026CC RID: 9932
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumGameStats GameStat = EnumGameStats.AnimalCount;
}
