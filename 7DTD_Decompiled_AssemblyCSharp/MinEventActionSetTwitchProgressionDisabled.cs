using System;
using System.Xml.Linq;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000647 RID: 1607
[Preserve]
public class MinEventActionSetTwitchProgressionDisabled : MinEventActionTargetedBase
{
	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x0600310A RID: 12554 RVA: 0x0014EBC5 File Offset: 0x0014CDC5
	// (set) Token: 0x0600310B RID: 12555 RVA: 0x0014EBCD File Offset: 0x0014CDCD
	public bool disabled { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x0600310C RID: 12556 RVA: 0x0014EBD8 File Offset: 0x0014CDD8
	public override void Execute(MinEventParams _params)
	{
		if (this.targets == null)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityPlayerLocal entityPlayerLocal = this.targets[i] as EntityPlayerLocal;
			if (entityPlayerLocal != null && entityPlayerLocal.TwitchEnabled)
			{
				TwitchManager.Current.OverrideProgession = this.disabled;
			}
		}
	}

	// Token: 0x0600310D RID: 12557 RVA: 0x0014EC31 File Offset: 0x0014CE31
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "disabled")
		{
			this.disabled = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
		}
		return flag;
	}
}
