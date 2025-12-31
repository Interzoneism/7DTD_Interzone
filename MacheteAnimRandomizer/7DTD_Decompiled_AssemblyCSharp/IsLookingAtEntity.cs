using System;
using System.Xml.Linq;

// Token: 0x020005F2 RID: 1522
public class IsLookingAtEntity : IsLookingAtBlock
{
	// Token: 0x06002FD9 RID: 12249 RVA: 0x00146DE0 File Offset: 0x00144FE0
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

	// Token: 0x040026B3 RID: 9907
	[PublicizedFrom(EAccessModifier.Private)]
	public new FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x040026B4 RID: 9908
	[PublicizedFrom(EAccessModifier.Private)]
	public new bool hasAllTags;
}
