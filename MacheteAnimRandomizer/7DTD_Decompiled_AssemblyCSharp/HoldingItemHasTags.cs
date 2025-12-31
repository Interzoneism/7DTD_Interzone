using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005DE RID: 1502
[Preserve]
public class HoldingItemHasTags : TargetedCompareRequirementBase
{
	// Token: 0x06002F96 RID: 12182 RVA: 0x00145EC8 File Offset: 0x001440C8
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		bool flag;
		if (!this.hasAllTags)
		{
			flag = this.target.inventory.holdingItem.HasAnyTags(this.holdingItemTags);
		}
		else
		{
			flag = this.target.inventory.holdingItem.HasAllTags(this.holdingItemTags);
		}
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x06002F97 RID: 12183 RVA: 0x00145B94 File Offset: 0x00143D94
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Is {0}Male", this.invert ? "NOT " : ""));
	}

	// Token: 0x06002F98 RID: 12184 RVA: 0x00145F34 File Offset: 0x00144134
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "tags")
			{
				this.holdingItemTags = FastTags<TagGroup.Global>.Parse(_attribute.Value);
				return true;
			}
			if (localName == "has_all_tags")
			{
				this.hasAllTags = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x0400269C RID: 9884
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> holdingItemTags;

	// Token: 0x0400269D RID: 9885
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
