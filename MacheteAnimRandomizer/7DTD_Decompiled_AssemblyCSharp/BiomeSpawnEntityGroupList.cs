using System;
using System.Collections.Generic;

// Token: 0x02000974 RID: 2420
public class BiomeSpawnEntityGroupList
{
	// Token: 0x06004935 RID: 18741 RVA: 0x001CF4D0 File Offset: 0x001CD6D0
	public BiomeSpawnEntityGroupData Find(int _idHash)
	{
		for (int i = 0; i < this.list.Count; i++)
		{
			if (this.list[i].idHash == _idHash)
			{
				return this.list[i];
			}
		}
		return null;
	}

	// Token: 0x0400385A RID: 14426
	public List<BiomeSpawnEntityGroupData> list = new List<BiomeSpawnEntityGroupData>();
}
