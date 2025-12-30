using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000672 RID: 1650
[Preserve]
public class MinEventActionAddChatMessage : MinEventActionTargetedBase
{
	// Token: 0x06003194 RID: 12692 RVA: 0x001522C8 File Offset: 0x001504C8
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityPlayerLocal entityPlayerLocal = this.targets[i] as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				XUiC_ChatOutput.AddMessage(LocalPlayerUI.GetUIForPlayer(entityPlayerLocal).xui, EnumGameMessages.PlainTextLocal, this.message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.Server, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
			}
		}
	}

	// Token: 0x06003195 RID: 12693 RVA: 0x0015231F File Offset: 0x0015051F
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && this.message != null;
	}

	// Token: 0x06003196 RID: 12694 RVA: 0x00152338 File Offset: 0x00150538
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "message")
			{
				if (this.message == null || this.message == "")
				{
					this.message = _attribute.Value;
				}
				return true;
			}
			if (localName == "message_key")
			{
				if (Localization.Exists(_attribute.Value, false))
				{
					this.message = Localization.Get(_attribute.Value, false);
				}
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002808 RID: 10248
	[PublicizedFrom(EAccessModifier.Private)]
	public string message;
}
