using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000620 RID: 1568
[Preserve]
public class MinEventActionAnimatorSetFloat : MinEventActionTargetedBase
{
	// Token: 0x06003080 RID: 12416 RVA: 0x0014B6A0 File Offset: 0x001498A0
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].emodel != null && this.targets[i].emodel.avatarController != null)
			{
				this.targets[i].emodel.avatarController.UpdateFloat(this.property, this.value, true);
			}
		}
	}

	// Token: 0x06003081 RID: 12417 RVA: 0x0014B724 File Offset: 0x00149924
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
				this.value = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002702 RID: 9986
	[PublicizedFrom(EAccessModifier.Protected)]
	public string property;

	// Token: 0x04002703 RID: 9987
	[PublicizedFrom(EAccessModifier.Protected)]
	public float value;
}
