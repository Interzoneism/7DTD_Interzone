using System;
using System.Collections.Generic;

// Token: 0x020007F2 RID: 2034
public class TraderStageTemplateGroup
{
	// Token: 0x06003A77 RID: 14967 RVA: 0x00178214 File Offset: 0x00176414
	public bool IsWithin(int traderStage, int quality)
	{
		for (int i = 0; i < this.Templates.Count; i++)
		{
			if (this.Templates[i].IsWithin(traderStage, quality))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002F62 RID: 12130
	public string Name = "";

	// Token: 0x04002F63 RID: 12131
	public List<TraderStageTemplate> Templates = new List<TraderStageTemplate>();
}
