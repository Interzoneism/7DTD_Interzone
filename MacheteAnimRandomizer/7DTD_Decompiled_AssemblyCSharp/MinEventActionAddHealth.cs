using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000657 RID: 1623
[Preserve]
public class MinEventActionAddHealth : MinEventActionTargetedBase
{
	// Token: 0x0600313C RID: 12604 RVA: 0x0014F9A0 File Offset: 0x0014DBA0
	public override void Execute(MinEventParams _params)
	{
		if (this.targets == null)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			int num = (!this.cvarRef) ? this.health : ((int)this.targets[i].Buffs.GetCustomVar(this.refCvarName));
			this.targets[i].AddHealth(num);
			if (this.showSplatter && num < 0)
			{
				EntityPlayerLocal entityPlayerLocal = this.targets[i] as EntityPlayerLocal;
				if (entityPlayerLocal != null)
				{
					entityPlayerLocal.ForceBloodSplatter();
				}
			}
		}
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x0014FA34 File Offset: 0x0014DC34
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "health"))
			{
				if (localName == "show_splatter")
				{
					this.showSplatter = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				}
			}
			else if (_attribute.Value.StartsWith("@"))
			{
				this.cvarRef = true;
				this.refCvarName = _attribute.Value.Substring(1);
			}
			else
			{
				this.health = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
			}
		}
		return flag;
	}

	// Token: 0x04002791 RID: 10129
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x04002792 RID: 10130
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName = "";

	// Token: 0x04002793 RID: 10131
	[PublicizedFrom(EAccessModifier.Private)]
	public int health;

	// Token: 0x04002794 RID: 10132
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showSplatter = true;
}
