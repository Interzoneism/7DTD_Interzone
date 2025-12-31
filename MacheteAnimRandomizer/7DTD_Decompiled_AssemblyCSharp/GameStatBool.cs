using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000611 RID: 1553
[Preserve]
public class GameStatBool : RequirementBase
{
	// Token: 0x0600303E RID: 12350 RVA: 0x0014849A File Offset: 0x0014669A
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (GameStats.GetBool(this.GameStat))
		{
			return !this.invert;
		}
		return this.invert;
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x001484C4 File Offset: 0x001466C4
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

	// Token: 0x040026CA RID: 9930
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumGameStats GameStat = EnumGameStats.AnimalCount;
}
