using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005E0 RID: 1504
[Preserve]
public class TriggerHasTags : TargetedCompareRequirementBase
{
	// Token: 0x06002F9E RID: 12190 RVA: 0x00146088 File Offset: 0x00144288
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		bool flag;
		if (!this.hasAllTags)
		{
			flag = _params.Tags.Test_AnySet(this.currentItemTags);
		}
		else
		{
			flag = _params.Tags.Test_AllSet(this.currentItemTags);
		}
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x06002F9F RID: 12191 RVA: 0x00002914 File Offset: 0x00000B14
	public override void GetInfoStrings(ref List<string> list)
	{
	}

	// Token: 0x06002FA0 RID: 12192 RVA: 0x001460E0 File Offset: 0x001442E0
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

	// Token: 0x040026A0 RID: 9888
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> currentItemTags;

	// Token: 0x040026A1 RID: 9889
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
