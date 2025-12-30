using System;
using System.Collections.Generic;

// Token: 0x0200096A RID: 2410
public struct LevelRequirement
{
	// Token: 0x060048E0 RID: 18656 RVA: 0x001CD7E7 File Offset: 0x001CB9E7
	public LevelRequirement(int _level)
	{
		this.Level = _level;
		this.Requirements = null;
	}

	// Token: 0x060048E1 RID: 18657 RVA: 0x001CD7F7 File Offset: 0x001CB9F7
	public void AddRequirement(IRequirement _req)
	{
		if (this.Requirements == null)
		{
			this.Requirements = new List<IRequirement>();
		}
		this.Requirements.Add(_req);
	}

	// Token: 0x04003824 RID: 14372
	public readonly int Level;

	// Token: 0x04003825 RID: 14373
	public List<IRequirement> Requirements;
}
