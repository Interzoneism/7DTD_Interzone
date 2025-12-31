using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000659 RID: 1625
[Preserve]
public class MinEventActionRemoveToolbeltMessage : MinEventActionBase
{
	// Token: 0x06003142 RID: 12610 RVA: 0x0014FB60 File Offset: 0x0014DD60
	public override void Execute(MinEventParams _params)
	{
		base.Execute(_params);
		EntityPlayerLocal entityPlayerLocal = _params.Self as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			GameManager.RemovePinnedTooltip(entityPlayerLocal, this.messageKey);
		}
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x0014FB8F File Offset: 0x0014DD8F
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "message_key")
		{
			this.messageKey = _attribute.Value;
		}
		return flag;
	}

	// Token: 0x04002796 RID: 10134
	[PublicizedFrom(EAccessModifier.Private)]
	public string messageKey = "";
}
