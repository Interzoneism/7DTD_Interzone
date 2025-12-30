using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x020005E1 RID: 1505
[Preserve]
public class BlockHasTags : TargetedCompareRequirementBase
{
	// Token: 0x06002FA2 RID: 12194 RVA: 0x0014614C File Offset: 0x0014434C
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		bool flag = false;
		if (!_params.BlockValue.isair && _params.BlockValue.Block != null)
		{
			if (!this.hasAllTags)
			{
				flag = _params.BlockValue.Block.Tags.Test_AnySet(this.currentBlockTags);
			}
			else
			{
				flag = _params.BlockValue.Block.Tags.Test_AllSet(this.currentBlockTags);
			}
		}
		if (!this.invert)
		{
			return flag;
		}
		return !flag;
	}

	// Token: 0x06002FA3 RID: 12195 RVA: 0x00002914 File Offset: 0x00000B14
	public override void GetInfoStrings(ref List<string> list)
	{
	}

	// Token: 0x06002FA4 RID: 12196 RVA: 0x001461D0 File Offset: 0x001443D0
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "tags")
			{
				this.currentBlockTags = FastTags<TagGroup.Global>.Parse(_attribute.Value);
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

	// Token: 0x040026A2 RID: 9890
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> currentBlockTags;

	// Token: 0x040026A3 RID: 9891
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAllTags;
}
