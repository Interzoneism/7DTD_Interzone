using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000606 RID: 1542
[Preserve]
public class HitLocation : TargetedCompareRequirementBase
{
	// Token: 0x06003016 RID: 12310 RVA: 0x00147B62 File Offset: 0x00145D62
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (!this.invert)
		{
			return (this.bodyParts & _params.DamageResponse.HitBodyPart) > EnumBodyPartHit.None;
		}
		return (this.bodyParts & _params.DamageResponse.HitBodyPart) == EnumBodyPartHit.None;
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x00147BA2 File Offset: 0x00145DA2
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("{0} hit location: ", this.invert ? "NOT " : "", this.bodyPartNames));
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x00147BD0 File Offset: 0x00145DD0
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "body_parts")
		{
			this.bodyPartNames = _attribute.Value;
			string[] array = this.bodyPartNames.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.bodyParts |= EnumUtils.Parse<EnumBodyPartHit>(array[i], true);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x040026BF RID: 9919
	[PublicizedFrom(EAccessModifier.Private)]
	public string bodyPartNames = "";

	// Token: 0x040026C0 RID: 9920
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumBodyPartHit bodyParts;
}
