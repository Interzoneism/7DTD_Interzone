using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000607 RID: 1543
[Preserve]
public class HasParticle : TargetedCompareRequirementBase
{
	// Token: 0x0600301A RID: 12314 RVA: 0x00147C54 File Offset: 0x00145E54
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return _params.Self.HasParticle(this.particleName);
		}
		return !_params.Self.HasParticle(this.particleName);
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x00147C90 File Offset: 0x00145E90
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "particle")
		{
			this.particleName = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x040026C1 RID: 9921
	[PublicizedFrom(EAccessModifier.Private)]
	public string particleName = "";
}
