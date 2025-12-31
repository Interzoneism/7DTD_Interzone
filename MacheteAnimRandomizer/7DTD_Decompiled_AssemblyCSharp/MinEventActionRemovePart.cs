using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200063A RID: 1594
[Preserve]
public class MinEventActionRemovePart : MinEventActionTargetedBase
{
	// Token: 0x060030CB RID: 12491 RVA: 0x0014D726 File Offset: 0x0014B926
	public override void Execute(MinEventParams _params)
	{
		if (_params.Self == null)
		{
			return;
		}
		_params.Self.RemovePart(this.partName);
	}

	// Token: 0x060030CC RID: 12492 RVA: 0x0014D748 File Offset: 0x0014B948
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && _params.Self != null && this.partName != null;
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x0014D770 File Offset: 0x0014B970
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "part")
		{
			this.partName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002746 RID: 10054
	[PublicizedFrom(EAccessModifier.Private)]
	public string partName;
}
