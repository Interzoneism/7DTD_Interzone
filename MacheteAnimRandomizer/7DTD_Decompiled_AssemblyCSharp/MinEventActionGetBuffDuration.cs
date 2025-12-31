using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200062C RID: 1580
[Preserve]
public class MinEventActionGetBuffDuration : MinEventActionTargetedBase
{
	// Token: 0x060030A2 RID: 12450 RVA: 0x0014C118 File Offset: 0x0014A318
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		if (base.CanExecute(_eventType, _params))
		{
			BuffValue buff = _params.Buff;
			if (((buff != null) ? buff.BuffClass : null) != null)
			{
				return !string.IsNullOrEmpty(this.reference);
			}
		}
		return false;
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x0014C148 File Offset: 0x0014A348
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].Buffs.SetCustomVar(this.reference, _params.Buff.BuffClass.DurationMax, true, CVarOperation.set);
		}
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x0014C19C File Offset: 0x0014A39C
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "reference")
		{
			this.reference = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x04002717 RID: 10007
	[PublicizedFrom(EAccessModifier.Private)]
	public string reference;
}
