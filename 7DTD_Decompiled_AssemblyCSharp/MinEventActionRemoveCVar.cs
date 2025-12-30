using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000631 RID: 1585
[Preserve]
public class MinEventActionRemoveCVar : MinEventActionTargetedBase
{
	// Token: 0x060030B0 RID: 12464 RVA: 0x0014C70C File Offset: 0x0014A90C
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			for (int j = 0; j < this.cvarNames.Length; j++)
			{
				this.targets[i].Buffs.SetCustomVar(this.cvarNames[j], 0f, (this.targets[i].isEntityRemote && !_params.Self.isEntityRemote) || _params.IsLocal, CVarOperation.set);
			}
		}
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x0014C790 File Offset: 0x0014A990
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "cvar")
		{
			this.cvarNames = _attribute.Value.Split(',', StringSplitOptions.None);
			return true;
		}
		return flag;
	}

	// Token: 0x0400272C RID: 10028
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] cvarNames;
}
