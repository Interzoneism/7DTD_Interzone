using System;

// Token: 0x02000A89 RID: 2697
public class BiomeBluffDecoration
{
	// Token: 0x0600534B RID: 21323 RVA: 0x0021596B File Offset: 0x00213B6B
	public BiomeBluffDecoration(string _name, float _prob, float _minScale, float _maxScale)
	{
		this.m_sName = _name;
		this.m_Prob = _prob;
		this.m_MinScale = _minScale;
		this.m_MaxScale = _maxScale;
	}

	// Token: 0x04003F7D RID: 16253
	public string m_sName;

	// Token: 0x04003F7E RID: 16254
	public float m_Prob;

	// Token: 0x04003F7F RID: 16255
	public float m_MinScale;

	// Token: 0x04003F80 RID: 16256
	public float m_MaxScale;
}
