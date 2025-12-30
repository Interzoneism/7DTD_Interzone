using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005F3 RID: 1523
[Preserve]
public class PlayerItemCount : TargetedCompareRequirementBase
{
	// Token: 0x06002FDB RID: 12251 RVA: 0x00146E54 File Offset: 0x00145054
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.item_name != null && this.item == null)
		{
			this.item = ItemClass.GetItem(this.item_name, true);
		}
		if (this.item == null)
		{
			return false;
		}
		int num = this.target.inventory.GetItemCount(this.item, false, -1, -1, true);
		num += this.target.bag.GetItemCount(this.item, -1, -1, true);
		if (this.invert)
		{
			return !RequirementBase.compareValues((float)num, this.operation, this.value);
		}
		return RequirementBase.compareValues((float)num, this.operation, this.value);
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x00146F01 File Offset: 0x00145101
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Item count {0}{1} {2}", this.invert ? "NOT " : "", this.operation.ToStringCached<RequirementBase.OperationTypes>(), this.value.ToCultureInvariantString()));
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x00146F40 File Offset: 0x00145140
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "item_name")
		{
			this.item_name = _attribute.Value;
			return true;
		}
		return flag;
	}

	// Token: 0x040026B5 RID: 9909
	[PublicizedFrom(EAccessModifier.Private)]
	public string item_name;

	// Token: 0x040026B6 RID: 9910
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue item;
}
