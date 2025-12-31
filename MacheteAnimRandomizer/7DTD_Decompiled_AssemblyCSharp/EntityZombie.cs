using System;
using UnityEngine.Scripting;

// Token: 0x0200047C RID: 1148
[Preserve]
public class EntityZombie : EntityHuman
{
	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x06002569 RID: 9577 RVA: 0x0000FB42 File Offset: 0x0000DD42
	// (set) Token: 0x0600256A RID: 9578 RVA: 0x00002914 File Offset: 0x00000B14
	public override bool AimingGun
	{
		get
		{
			return false;
		}
		set
		{
		}
	}
}
