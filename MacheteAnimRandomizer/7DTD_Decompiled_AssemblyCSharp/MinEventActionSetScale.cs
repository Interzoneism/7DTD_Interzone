using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200064F RID: 1615
[Preserve]
public class MinEventActionSetScale : MinEventActionTargetedBase
{
	// Token: 0x06003125 RID: 12581 RVA: 0x0014F1B0 File Offset: 0x0014D3B0
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityAlive entityAlive = this.targets[i];
			entityAlive.OverrideSize = this.scale;
			entityAlive.SetScale(this.scale);
		}
	}

	// Token: 0x06003126 RID: 12582 RVA: 0x0014F1F8 File Offset: 0x0014D3F8
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "scale")
		{
			this.scale = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
			return true;
		}
		return flag;
	}

	// Token: 0x04002782 RID: 10114
	[PublicizedFrom(EAccessModifier.Private)]
	public float scale = 1f;
}
