using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005E6 RID: 1510
[Preserve]
public class EntityHasMovementTag : TargetedCompareRequirementBase
{
	// Token: 0x06002FB3 RID: 12211 RVA: 0x00146574 File Offset: 0x00144774
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
				return this.target.CurrentMovementTag.Test_AllSet(this.tagsToCompare);
			}
			return !this.target.CurrentMovementTag.Test_AllSet(this.tagsToCompare);
		}
		else
		{
			if (!this.invert)
			{
				return this.target.CurrentMovementTag.Test_AnySet(this.tagsToCompare);
			}
			return !this.target.CurrentMovementTag.Test_AnySet(this.tagsToCompare);
		}
	}

	// Token: 0x06002FB4 RID: 12212 RVA: 0x00146608 File Offset: 0x00144808
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

	// Token: 0x040026A8 RID: 9896
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x040026A9 RID: 9897
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
