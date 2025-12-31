using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005DF RID: 1503
[Preserve]
public class ItemHasTags : TargetedCompareRequirementBase
{
	// Token: 0x06002F9A RID: 12186 RVA: 0x00145FA0 File Offset: 0x001441A0
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		bool flag = false;
		if (!_params.ItemValue.IsEmpty() && _params.ItemValue.ItemClass != null)
		{
			if (!this.hasAllTags)
			{
				flag = _params.ItemValue.ItemClass.HasAnyTags(this.currentItemTags);
			}
			else
			{
				flag = _params.ItemValue.ItemClass.HasAllTags(this.currentItemTags);
			}
		}
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x00002914 File Offset: 0x00000B14
	public override void GetInfoStrings(ref List<string> list)
	{
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x0014601C File Offset: 0x0014421C
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "tags")
			{
				this.currentItemTags = FastTags<TagGroup.Global>.Parse(_attribute.Value);
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

	// Token: 0x0400269E RID: 9886
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> currentItemTags;

	// Token: 0x0400269F RID: 9887
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
