using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005DD RID: 1501
[Preserve]
public class CompareItemMetaFloat : TargetedCompareRequirementBase
{
	// Token: 0x06002F93 RID: 12179 RVA: 0x00145E04 File Offset: 0x00144004
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		ItemValue itemValue = _params.ItemValue;
		if (itemValue == null || string.IsNullOrEmpty(this.metaKey))
		{
			return false;
		}
		object metadata = itemValue.GetMetadata(this.metaKey);
		if (!(metadata is float))
		{
			return false;
		}
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)metadata, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)metadata, this.operation, this.value);
	}

	// Token: 0x06002F94 RID: 12180 RVA: 0x00145E88 File Offset: 0x00144088
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "key")
		{
			this.metaKey = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x0400269B RID: 9883
	[PublicizedFrom(EAccessModifier.Private)]
	public string metaKey;
}
