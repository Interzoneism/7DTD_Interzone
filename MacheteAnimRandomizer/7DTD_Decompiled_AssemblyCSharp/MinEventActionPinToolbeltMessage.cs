using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000658 RID: 1624
[Preserve]
public class MinEventActionPinToolbeltMessage : MinEventActionBase
{
	// Token: 0x0600313F RID: 12607 RVA: 0x0014FAE8 File Offset: 0x0014DCE8
	public override void Execute(MinEventParams _params)
	{
		base.Execute(_params);
		EntityPlayerLocal entityPlayerLocal = _params.Self as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			GameManager.ShowTooltip(entityPlayerLocal, this.messageKey, false, true, 0f);
		}
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x0014FB1E File Offset: 0x0014DD1E
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "message_key")
		{
			this.messageKey = _attribute.Value;
		}
		return flag;
	}

	// Token: 0x04002795 RID: 10133
	[PublicizedFrom(EAccessModifier.Private)]
	public string messageKey = "";
}
