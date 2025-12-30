using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000636 RID: 1590
[Preserve]
public class MinEventActionRemoveParticleEffectFromEntity : MinEventActionTargetedBase
{
	// Token: 0x060030BF RID: 12479 RVA: 0x0014D1C2 File Offset: 0x0014B3C2
	public override void Execute(MinEventParams _params)
	{
		if (_params.Self == null)
		{
			return;
		}
		_params.Self.RemoveParticle(this.particleEffectName);
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x0014D1E5 File Offset: 0x0014B3E5
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && _params.Self != null && this.particleEffectName != null;
	}

	// Token: 0x060030C1 RID: 12481 RVA: 0x0014D20C File Offset: 0x0014B40C
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "particle")
		{
			this.particleEffectName = "Ptl_" + _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x0400273E RID: 10046
	[PublicizedFrom(EAccessModifier.Private)]
	public string particleEffectName;
}
