using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200064D RID: 1613
[Preserve]
public class MinEventActionCallGameEvent : MinEventActionTargetedBase
{
	// Token: 0x0600311F RID: 12575 RVA: 0x0014EFF8 File Offset: 0x0014D1F8
	public override void Execute(MinEventParams _params)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || this.allowClientCall)
		{
			for (int i = 0; i < this.targets.Count; i++)
			{
				if (this.targets[i] != null)
				{
					GameEventManager.Current.HandleAction(this.eventName, _params.Self as EntityPlayer, this.targets[i], false, "", "", false, true, this.sequenceLink, null);
				}
			}
		}
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x0014F080 File Offset: 0x0014D280
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "event"))
			{
				if (!(localName == "sequence_link"))
				{
					if (localName == "allow_client_call")
					{
						this.allowClientCall = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
					}
				}
				else
				{
					this.sequenceLink = _attribute.Value;
				}
			}
			else
			{
				this.eventName = _attribute.Value;
			}
		}
		return flag;
	}

	// Token: 0x0400277E RID: 10110
	[PublicizedFrom(EAccessModifier.Protected)]
	public string eventName = "";

	// Token: 0x0400277F RID: 10111
	[PublicizedFrom(EAccessModifier.Protected)]
	public string sequenceLink = "";

	// Token: 0x04002780 RID: 10112
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool allowClientCall;
}
