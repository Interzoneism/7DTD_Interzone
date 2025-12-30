using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000651 RID: 1617
[Preserve]
public class MinEventActionSetPitch : MinEventActionTargetedBase
{
	// Token: 0x0600312B RID: 12587 RVA: 0x0014F2F4 File Offset: 0x0014D4F4
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].OverridePitch = this.pitch;
		}
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x0014F330 File Offset: 0x0014D530
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "pitch")
		{
			this.pitch = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
			return true;
		}
		return flag;
	}

	// Token: 0x04002784 RID: 10116
	[PublicizedFrom(EAccessModifier.Private)]
	public float pitch = 1f;
}
