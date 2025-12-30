using System;
using System.Collections.Generic;

// Token: 0x02000195 RID: 405
public class BlockMaskRules<O, M>
{
	// Token: 0x06000C48 RID: 3144 RVA: 0x00053E9D File Offset: 0x0005209D
	public BlockMaskRules(M _wildcard)
	{
		this.blockRules = new List<BlockRule<O, M>>();
		this.wildcard = _wildcard;
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00053EB7 File Offset: 0x000520B7
	public void AddRule(BlockRule<O, M> blockRule)
	{
		if (!this.blockRules.Contains(blockRule))
		{
			this.blockRules.Add(blockRule);
			this.RotateLocalRangeYAxis(blockRule);
		}
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00053EDC File Offset: 0x000520DC
	public O GetOutput(M[] mask)
	{
		for (int i = 0; i < this.blockRules.Count; i++)
		{
			BlockRule<O, M> blockRule = this.blockRules[i];
			M[] mask2 = blockRule.Mask;
			bool flag = true;
			byte b = 0;
			while ((int)b < mask2.Length)
			{
				if (!mask2[(int)b].Equals(this.wildcard) && !mask2[(int)b].Equals(mask[(int)b]))
				{
					flag = false;
					break;
				}
				b += 1;
			}
			if (flag)
			{
				return blockRule.Output;
			}
		}
		return default(O);
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00053F8C File Offset: 0x0005218C
	[PublicizedFrom(EAccessModifier.Private)]
	public void RotateLocalRangeYAxis(BlockRule<O, M> blockRule)
	{
		M[] mask = blockRule.Mask;
		List<M> list = new List<M>
		{
			mask[0],
			mask[1],
			mask[2],
			mask[3],
			mask[4],
			mask[5],
			mask[6],
			mask[7],
			mask[8],
			mask[9],
			mask[10],
			mask[11],
			mask[12],
			mask[13],
			mask[14],
			mask[15],
			mask[16],
			mask[17],
			mask[18],
			mask[19],
			mask[20],
			mask[21],
			mask[22],
			mask[23],
			mask[24],
			mask[25],
			mask[26]
		};
		for (int i = 0; i < 3; i++)
		{
			list = new List<M>
			{
				list[6],
				list[3],
				list[0],
				list[7],
				list[4],
				list[1],
				list[8],
				list[5],
				list[2],
				list[15],
				list[12],
				list[9],
				list[16],
				list[13],
				list[10],
				list[17],
				list[14],
				list[11],
				list[24],
				list[21],
				list[18],
				list[25],
				list[22],
				list[19],
				list[26],
				list[23],
				list[20]
			};
			BlockRule<O, M> blockRule2 = new BlockRule<O, M>();
			blockRule2.Output = blockRule.Output;
			blockRule2.Mask = list.ToArray();
			if (!this.blockRules.Contains(blockRule2))
			{
				this.blockRules.Add(blockRule2);
			}
		}
	}

	// Token: 0x04000A4F RID: 2639
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockRule<O, M>> blockRules;

	// Token: 0x04000A50 RID: 2640
	[PublicizedFrom(EAccessModifier.Private)]
	public M wildcard;
}
