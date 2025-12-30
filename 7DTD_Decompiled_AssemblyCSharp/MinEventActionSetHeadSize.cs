using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000650 RID: 1616
[Preserve]
public class MinEventActionSetHeadSize : MinEventActionTargetedBase
{
	// Token: 0x06003128 RID: 12584 RVA: 0x0014F258 File Offset: 0x0014D458
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].SetHeadSize(this.standard);
		}
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x0014F294 File Offset: 0x0014D494
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "size")
		{
			this.standard = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
			return true;
		}
		return flag;
	}

	// Token: 0x04002783 RID: 10115
	[PublicizedFrom(EAccessModifier.Private)]
	public float standard = 1f;
}
