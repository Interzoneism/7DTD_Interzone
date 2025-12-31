using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200064C RID: 1612
[Preserve]
public class MinEventActionSetDancing : MinEventActionTargetedBase
{
	// Token: 0x0600311C RID: 12572 RVA: 0x0014EF84 File Offset: 0x0014D184
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].SetDancing(this.enabled);
		}
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x0014EFBE File Offset: 0x0014D1BE
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "enabled")
		{
			this.enabled = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
		}
		return flag;
	}

	// Token: 0x0400277D RID: 10109
	[PublicizedFrom(EAccessModifier.Private)]
	public bool enabled;
}
