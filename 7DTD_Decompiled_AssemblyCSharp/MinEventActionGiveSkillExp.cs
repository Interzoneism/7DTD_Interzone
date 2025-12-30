using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200063D RID: 1597
[Preserve]
public class MinEventActionGiveSkillExp : MinEventActionTargetedBase
{
	// Token: 0x060030D7 RID: 12503 RVA: 0x0014DB80 File Offset: 0x0014BD80
	public override void Execute(MinEventParams _params)
	{
		if (this.targets == null)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.exp != -1)
			{
				this.targets[i].Progression.AddLevelExp(this.exp, "_xpOther", Progression.XPTypes.Other, true, true);
				this.targets[i].Progression.bProgressionStatsChanged = !this.targets[i].isEntityRemote;
				this.targets[i].bPlayerStatsChanged |= !this.targets[i].isEntityRemote;
			}
			else if (this.level_percent != -1f)
			{
				this.targets[i].Progression.AddLevelExp(this.exp, "_xpOther", Progression.XPTypes.Other, true, true);
				this.targets[i].Progression.bProgressionStatsChanged = !this.targets[i].isEntityRemote;
				this.targets[i].bPlayerStatsChanged |= !this.targets[i].isEntityRemote;
			}
		}
	}

	// Token: 0x060030D8 RID: 12504 RVA: 0x0014DCC6 File Offset: 0x0014BEC6
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && this.skill != null && (this.exp != -1 || this.level_percent != -1f);
	}

	// Token: 0x060030D9 RID: 12505 RVA: 0x0014DCF8 File Offset: 0x0014BEF8
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "skill"))
			{
				if (!(localName == "experience"))
				{
					if (localName == "level_percentage")
					{
						this.level_percent = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
					}
				}
				else
				{
					this.exp = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
				}
			}
			else
			{
				this.skill = _attribute.Value;
			}
		}
		return flag;
	}

	// Token: 0x0400274F RID: 10063
	[PublicizedFrom(EAccessModifier.Private)]
	public string skill;

	// Token: 0x04002750 RID: 10064
	[PublicizedFrom(EAccessModifier.Private)]
	public int exp = -1;

	// Token: 0x04002751 RID: 10065
	[PublicizedFrom(EAccessModifier.Private)]
	public float level_percent = -1f;
}
