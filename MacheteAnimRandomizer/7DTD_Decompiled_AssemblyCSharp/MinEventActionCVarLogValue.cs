using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000630 RID: 1584
[Preserve]
public class MinEventActionCVarLogValue : MinEventActionTargetedBase
{
	// Token: 0x060030AD RID: 12461 RVA: 0x0014C66C File Offset: 0x0014A86C
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			Log.Out("CVarLogValue: {0} == {1}", new object[]
			{
				this.cvarName,
				this.targets[i].Buffs.GetCustomVar(this.cvarName).ToCultureInvariantString()
			});
		}
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x0014C6CC File Offset: 0x0014A8CC
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "cvar")
		{
			this.cvarName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x0400272B RID: 10027
	[PublicizedFrom(EAccessModifier.Private)]
	public string cvarName;
}
