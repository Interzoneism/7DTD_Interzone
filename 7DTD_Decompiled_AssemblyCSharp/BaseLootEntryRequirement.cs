using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000589 RID: 1417
[Preserve]
public class BaseLootEntryRequirement
{
	// Token: 0x06002DB8 RID: 11704 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Init(XElement e)
	{
	}

	// Token: 0x06002DB9 RID: 11705 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CheckRequirement(EntityPlayer player)
	{
		return true;
	}
}
