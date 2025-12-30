using System;

// Token: 0x020007F3 RID: 2035
public class TraderStageTemplate
{
	// Token: 0x06003A79 RID: 14969 RVA: 0x0017826D File Offset: 0x0017646D
	public bool IsWithin(int traderStage, int quality)
	{
		return (this.Min == -1 || this.Min <= traderStage) && (this.Max == -1 || this.Max >= traderStage) && (this.Quality == -1 || quality == this.Quality);
	}

	// Token: 0x04002F64 RID: 12132
	public int Min = -1;

	// Token: 0x04002F65 RID: 12133
	public int Max = -1;

	// Token: 0x04002F66 RID: 12134
	public int Quality = -1;
}
