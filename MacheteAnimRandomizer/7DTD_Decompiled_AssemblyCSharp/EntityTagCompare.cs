using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005E8 RID: 1512
[Preserve]
public class EntityTagCompare : TargetedCompareRequirementBase
{
	// Token: 0x06002FB9 RID: 12217 RVA: 0x00146774 File Offset: 0x00144974
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		if (this.hasAllTags)
		{
			if (!this.invert)
			{
				return this.target.HasAllTags(this.tagsToCompare);
			}
			return !this.target.HasAllTags(this.tagsToCompare);
		}
		else
		{
			if (!this.invert)
			{
				return this.target.HasAnyTags(this.tagsToCompare);
			}
			return !this.target.HasAnyTags(this.tagsToCompare);
		}
	}

	// Token: 0x06002FBA RID: 12218 RVA: 0x001467F4 File Offset: 0x001449F4
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "tags")
			{
				this.tagsToCompare = FastTags<TagGroup.Global>.Parse(_attribute.Value);
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

	// Token: 0x040026AC RID: 9900
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x040026AD RID: 9901
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
