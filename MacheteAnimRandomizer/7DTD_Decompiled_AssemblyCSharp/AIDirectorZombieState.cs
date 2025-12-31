using System;
using UnityEngine.Scripting;

// Token: 0x020003CA RID: 970
[Preserve]
public class AIDirectorZombieState : IMemoryPoolableObject
{
	// Token: 0x06001D99 RID: 7577 RVA: 0x000B832F File Offset: 0x000B652F
	public AIDirectorZombieState Construct(EntityEnemy zombie)
	{
		this.m_zombie = zombie;
		return this;
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x000B8339 File Offset: 0x000B6539
	public void Reset()
	{
		this.m_zombie = null;
	}

	// Token: 0x06001D9B RID: 7579 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06001D9C RID: 7580 RVA: 0x000B8342 File Offset: 0x000B6542
	public EntityEnemy Zombie
	{
		get
		{
			return this.m_zombie;
		}
	}

	// Token: 0x0400143D RID: 5181
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityEnemy m_zombie;
}
