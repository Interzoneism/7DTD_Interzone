using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200064B RID: 1611
[Preserve]
public class MinEventActionSetBigHead : MinEventActionTargetedBase
{
	// Token: 0x06003119 RID: 12569 RVA: 0x0014EF04 File Offset: 0x0014D104
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityAlive entityAlive = this.targets[i];
			if (this.enabled)
			{
				entityAlive.SetBigHead();
			}
			else
			{
				entityAlive.ResetHead();
			}
		}
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x0014EF4A File Offset: 0x0014D14A
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "enabled")
		{
			this.enabled = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
		}
		return flag;
	}

	// Token: 0x0400277C RID: 10108
	[PublicizedFrom(EAccessModifier.Private)]
	public bool enabled;
}
