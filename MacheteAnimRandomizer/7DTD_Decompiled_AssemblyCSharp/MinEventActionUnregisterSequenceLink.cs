using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200064E RID: 1614
[Preserve]
public class MinEventActionUnregisterSequenceLink : MinEventActionTargetedBase
{
	// Token: 0x06003122 RID: 12578 RVA: 0x0014F11C File Offset: 0x0014D31C
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i] != null)
			{
				GameEventManager.Current.UnRegisterLink(_params.Self as EntityPlayer, this.sequenceLink);
			}
		}
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x0014F16E File Offset: 0x0014D36E
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "sequence_link")
		{
			this.sequenceLink = _attribute.Value;
		}
		return flag;
	}

	// Token: 0x04002781 RID: 10113
	[PublicizedFrom(EAccessModifier.Protected)]
	public string sequenceLink = "";
}
