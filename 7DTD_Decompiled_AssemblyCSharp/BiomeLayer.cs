using System;
using System.Collections.Generic;

// Token: 0x02000A86 RID: 2694
public class BiomeLayer
{
	// Token: 0x06005345 RID: 21317 RVA: 0x00215780 File Offset: 0x00213980
	public BiomeLayer(int _depth, BiomeBlockDecoration _bb)
	{
		this.m_Block = _bb;
		this.m_Depth = _depth;
		this.m_Resources = new List<BiomeBlockDecoration>();
		this.SumResourceProbs = new List<float>();
		this.MaxResourceProb = 0f;
	}

	// Token: 0x06005346 RID: 21318 RVA: 0x002157B8 File Offset: 0x002139B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~BiomeLayer()
	{
	}

	// Token: 0x06005347 RID: 21319 RVA: 0x002157E0 File Offset: 0x002139E0
	public void AddResource(BiomeBlockDecoration _res)
	{
		this.m_Resources.Add(_res);
		this.MaxResourceProb = Utils.FastMax(_res.prob, this.MaxResourceProb);
		int count = this.SumResourceProbs.Count;
		this.SumResourceProbs.Add((count > 0) ? (this.SumResourceProbs[count - 1] + _res.prob) : _res.prob);
	}

	// Token: 0x04003F6F RID: 16239
	public BiomeBlockDecoration m_Block;

	// Token: 0x04003F70 RID: 16240
	public int m_Depth;

	// Token: 0x04003F71 RID: 16241
	public List<BiomeBlockDecoration> m_Resources;

	// Token: 0x04003F72 RID: 16242
	public List<float> SumResourceProbs;

	// Token: 0x04003F73 RID: 16243
	public float MaxResourceProb;
}
