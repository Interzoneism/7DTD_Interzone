using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000621 RID: 1569
[Preserve]
public class MinEventActionAnimatorSetInt : MinEventActionTargetedBase
{
	// Token: 0x06003083 RID: 12419 RVA: 0x0014B798 File Offset: 0x00149998
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].emodel != null && this.targets[i].emodel.avatarController != null)
			{
				this.targets[i].emodel.avatarController.UpdateInt(this.property, this.value, true);
			}
		}
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x0014B81C File Offset: 0x00149A1C
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "property")
			{
				this.property = _attribute.Value;
				return true;
			}
			if (localName == "value")
			{
				this.value = int.Parse(_attribute.Value);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002704 RID: 9988
	[PublicizedFrom(EAccessModifier.Protected)]
	public string property;

	// Token: 0x04002705 RID: 9989
	[PublicizedFrom(EAccessModifier.Protected)]
	public int value;
}
