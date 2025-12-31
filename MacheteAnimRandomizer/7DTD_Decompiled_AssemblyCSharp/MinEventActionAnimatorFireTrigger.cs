using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000623 RID: 1571
[Preserve]
public class MinEventActionAnimatorFireTrigger : MinEventActionTargetedBase
{
	// Token: 0x06003089 RID: 12425 RVA: 0x0014B96C File Offset: 0x00149B6C
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].emodel != null && this.targets[i].emodel.avatarController != null)
			{
				this.targets[i].emodel.avatarController.TriggerEvent(this.property);
			}
		}
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x0014B9E8 File Offset: 0x00149BE8
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "property")
		{
			this.property = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002708 RID: 9992
	[PublicizedFrom(EAccessModifier.Protected)]
	public string property;

	// Token: 0x04002709 RID: 9993
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool value;
}
