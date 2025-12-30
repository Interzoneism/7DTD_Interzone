using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000667 RID: 1639
[Preserve]
public class MinEventActionRefreshPerks : MinEventActionBase
{
	// Token: 0x06003175 RID: 12661 RVA: 0x001514CD File Offset: 0x0014F6CD
	public override void Execute(MinEventParams _params)
	{
		if (_params.Self.Progression == null)
		{
			return;
		}
		_params.Self.Progression.RefreshPerks(this.attribute);
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x001514F4 File Offset: 0x0014F6F4
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "attribute")
		{
			this.attribute = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x040027DC RID: 10204
	[PublicizedFrom(EAccessModifier.Private)]
	public string attribute = "";
}
