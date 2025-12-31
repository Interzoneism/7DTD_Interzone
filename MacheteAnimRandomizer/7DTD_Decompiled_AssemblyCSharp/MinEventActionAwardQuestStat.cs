using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000640 RID: 1600
[Preserve]
public class MinEventActionAwardQuestStat : MinEventActionTargetedBase
{
	// Token: 0x060030E3 RID: 12515 RVA: 0x0014E028 File Offset: 0x0014C228
	public override void Execute(MinEventParams _params)
	{
		if (this.targets == null)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityPlayerLocal entityPlayerLocal = this.targets[i] as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				QuestEventManager.Current.QuestAwardCredited(this.stat, (!this.cvarRef) ? this.awardCount : ((int)entityPlayerLocal.Buffs.GetCustomVar(this.refCvarName)));
			}
		}
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x0014E09B File Offset: 0x0014C29B
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && (this.cvarRef || this.awardCount > 0);
	}

	// Token: 0x060030E5 RID: 12517 RVA: 0x0014E0BC File Offset: 0x0014C2BC
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "count"))
			{
				if (localName == "stat")
				{
					this.stat = _attribute.Value;
				}
			}
			else if (_attribute.Value.StartsWith("@"))
			{
				this.cvarRef = true;
				this.refCvarName = _attribute.Value.Substring(1);
			}
			else
			{
				this.awardCount = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
			}
		}
		return flag;
	}

	// Token: 0x04002759 RID: 10073
	[PublicizedFrom(EAccessModifier.Private)]
	public string stat;

	// Token: 0x0400275A RID: 10074
	[PublicizedFrom(EAccessModifier.Private)]
	public int awardCount = 1;

	// Token: 0x0400275B RID: 10075
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x0400275C RID: 10076
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName;
}
