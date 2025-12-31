using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000622 RID: 1570
[Preserve]
public class MinEventActionAnimatorSetBool : MinEventActionTargetedBase
{
	// Token: 0x06003086 RID: 12422 RVA: 0x0014B880 File Offset: 0x00149A80
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].emodel != null && this.targets[i].emodel.avatarController != null)
			{
				this.targets[i].emodel.avatarController.UpdateBool(this.property, this.value, true);
			}
		}
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x0014B904 File Offset: 0x00149B04
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
				this.value = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002706 RID: 9990
	[PublicizedFrom(EAccessModifier.Protected)]
	public string property;

	// Token: 0x04002707 RID: 9991
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool value;
}
