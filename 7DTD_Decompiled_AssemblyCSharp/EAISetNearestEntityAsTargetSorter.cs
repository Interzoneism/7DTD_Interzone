using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020003F2 RID: 1010
[Preserve]
public class EAISetNearestEntityAsTargetSorter : IComparer<Entity>
{
	// Token: 0x06001E8D RID: 7821 RVA: 0x000BECC3 File Offset: 0x000BCEC3
	public EAISetNearestEntityAsTargetSorter(Entity _entity)
	{
		this.theEntity = _entity;
	}

	// Token: 0x06001E8E RID: 7822 RVA: 0x000BECD4 File Offset: 0x000BCED4
	[PublicizedFrom(EAccessModifier.Private)]
	public int isNearer(Entity _e, Entity _other)
	{
		float distanceSq = this.theEntity.GetDistanceSq(_e);
		float distanceSq2 = this.theEntity.GetDistanceSq(_other);
		if (distanceSq < distanceSq2)
		{
			return -1;
		}
		if (distanceSq > distanceSq2)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06001E8F RID: 7823 RVA: 0x000BED08 File Offset: 0x000BCF08
	public int Compare(Entity _obj1, Entity _obj2)
	{
		return this.isNearer(_obj1, _obj2);
	}

	// Token: 0x04001516 RID: 5398
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity theEntity;
}
