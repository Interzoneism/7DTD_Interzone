using System;
using System.Xml.Linq;

// Token: 0x020005F1 RID: 1521
public class IsLookingAtBlock : RequirementBase
{
	// Token: 0x06002FD5 RID: 12245 RVA: 0x00146D66 File Offset: 0x00144F66
	public override bool IsValid(MinEventParams _params)
	{
		return base.IsValid(_params);
	}

	// Token: 0x06002FD6 RID: 12246 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void raycast()
	{
	}

	// Token: 0x06002FD7 RID: 12247 RVA: 0x00146D74 File Offset: 0x00144F74
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

	// Token: 0x040026B1 RID: 9905
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> tagsToCompare;

	// Token: 0x040026B2 RID: 9906
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
