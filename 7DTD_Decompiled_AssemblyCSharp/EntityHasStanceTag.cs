using System;
using System.Xml.Linq;

// Token: 0x020005E7 RID: 1511
public class EntityHasStanceTag : TargetedCompareRequirementBase
{
	// Token: 0x06002FB6 RID: 12214 RVA: 0x00146674 File Offset: 0x00144874
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
				return this.target.CurrentStanceTag.Test_AllSet(this.tagsToCompare);
			}
			return !this.target.CurrentStanceTag.Test_AllSet(this.tagsToCompare);
		}
		else
		{
			if (!this.invert)
			{
				return this.target.CurrentStanceTag.Test_AnySet(this.tagsToCompare);
			}
			return !this.target.CurrentStanceTag.Test_AnySet(this.tagsToCompare);
		}
	}

	// Token: 0x06002FB7 RID: 12215 RVA: 0x00146708 File Offset: 0x00144908
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

	// Token: 0x040026AA RID: 9898
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x040026AB RID: 9899
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
