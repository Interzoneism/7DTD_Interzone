using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005F4 RID: 1524
[Preserve]
public class PlayerItemCountByTags : TargetedCompareRequirementBase
{
	// Token: 0x06002FDF RID: 12255 RVA: 0x00146F80 File Offset: 0x00145180
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		int num = this.target.inventory.GetItemCount(this.itemTags, -1, -1, true);
		num += this.target.bag.GetItemCount(this.itemTags, -1, -1, true);
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)num, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)num, this.operation, this.value);
	}

	// Token: 0x06002FE0 RID: 12256 RVA: 0x00147000 File Offset: 0x00145200
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "item_tags")
		{
			this.itemTags = FastTags<TagGroup.Global>.Parse(_attribute.Value);
			return true;
		}
		return flag;
	}

	// Token: 0x040026B7 RID: 9911
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> itemTags;
}
