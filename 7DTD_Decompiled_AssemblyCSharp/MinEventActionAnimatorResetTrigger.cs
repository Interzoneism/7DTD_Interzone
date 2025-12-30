using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000624 RID: 1572
[Preserve]
public class MinEventActionAnimatorResetTrigger : MinEventActionTargetedBase
{
	// Token: 0x0600308C RID: 12428 RVA: 0x0014BA28 File Offset: 0x00149C28
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].emodel != null && this.targets[i].emodel.avatarController != null)
			{
				this.targets[i].emodel.avatarController.CancelEvent(this.property);
			}
		}
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x0014BAA4 File Offset: 0x00149CA4
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

	// Token: 0x0400270A RID: 9994
	[PublicizedFrom(EAccessModifier.Protected)]
	public string property;

	// Token: 0x0400270B RID: 9995
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool value;
}
