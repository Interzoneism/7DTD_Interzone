using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200063F RID: 1599
[Preserve]
public class MinEventActionAwardChallenge : MinEventActionTargetedBase
{
	// Token: 0x060030DF RID: 12511 RVA: 0x0014DEF4 File Offset: 0x0014C0F4
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
				QuestEventManager.Current.ChallengeAwardCredited(this.challengeStat, (!this.cvarRef) ? this.challengeAwardCount : ((int)entityPlayerLocal.Buffs.GetCustomVar(this.refCvarName)));
			}
		}
	}

	// Token: 0x060030E0 RID: 12512 RVA: 0x0014DF67 File Offset: 0x0014C167
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && (this.cvarRef || this.challengeAwardCount > 0);
	}

	// Token: 0x060030E1 RID: 12513 RVA: 0x0014DF88 File Offset: 0x0014C188
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "count"))
			{
				if (localName == "challenge_stat")
				{
					this.challengeStat = _attribute.Value;
				}
			}
			else if (_attribute.Value.StartsWith("@"))
			{
				this.cvarRef = true;
				this.refCvarName = _attribute.Value.Substring(1);
			}
			else
			{
				this.challengeAwardCount = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
			}
		}
		return flag;
	}

	// Token: 0x04002755 RID: 10069
	[PublicizedFrom(EAccessModifier.Private)]
	public string challengeStat;

	// Token: 0x04002756 RID: 10070
	[PublicizedFrom(EAccessModifier.Private)]
	public int challengeAwardCount = 1;

	// Token: 0x04002757 RID: 10071
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x04002758 RID: 10072
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName;
}
