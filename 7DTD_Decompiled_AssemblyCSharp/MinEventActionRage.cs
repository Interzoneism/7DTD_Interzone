using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000653 RID: 1619
[Preserve]
public class MinEventActionRage : MinEventActionTargetedBase
{
	// Token: 0x06003131 RID: 12593 RVA: 0x0014F490 File Offset: 0x0014D690
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityHuman entityHuman = this.targets[i] as EntityHuman;
			if (entityHuman != null)
			{
				if (this.enabled)
				{
					entityHuman.StartRage(this.speedPercent, this.rageTime + 1f);
				}
				else
				{
					entityHuman.StopRage();
				}
			}
		}
	}

	// Token: 0x06003132 RID: 12594 RVA: 0x0014F4F8 File Offset: 0x0014D6F8
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "speed")
			{
				this.speedPercent = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
			if (localName == "time")
			{
				this.rageTime = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
			if (localName == "enabled")
			{
				this.enabled = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
			}
		}
		return flag;
	}

	// Token: 0x04002786 RID: 10118
	[PublicizedFrom(EAccessModifier.Private)]
	public bool enabled;

	// Token: 0x04002787 RID: 10119
	[PublicizedFrom(EAccessModifier.Private)]
	public float speedPercent = 2f;

	// Token: 0x04002788 RID: 10120
	[PublicizedFrom(EAccessModifier.Private)]
	public float rageTime = 60f;
}
