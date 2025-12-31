using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020003EF RID: 1007
[Preserve]
public class EAISetNearestCorpseAsTarget : EAITarget
{
	// Token: 0x06001E78 RID: 7800 RVA: 0x000BE05C File Offset: 0x000BC25C
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity, 15f, true);
		this.executeDelay = 0.8f;
		this.rndTimeout = 0;
		this.MutexBits = 1;
		this.sorter = new EAISetNearestEntityAsTargetSorter(_theEntity);
	}

	// Token: 0x06001E79 RID: 7801 RVA: 0x000BE090 File Offset: 0x000BC290
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		string names;
		if (data.TryGetValue("flags", out names))
		{
			EntityClass.ParseEntityFlags(names, ref this.targetFlags);
		}
		base.GetData(data, "maxDistance2d", ref this.maxXZDistance);
	}

	// Token: 0x06001E7A RID: 7802 RVA: 0x000BE0D4 File Offset: 0x000BC2D4
	public override bool CanExecute()
	{
		if (this.theEntity.HasInvestigatePosition)
		{
			return false;
		}
		if (this.theEntity.IsSleeping)
		{
			return false;
		}
		if (this.rndTimeout > 0 && base.GetRandom(this.rndTimeout) != 0)
		{
			return false;
		}
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (attackTarget is EntityPlayer && attackTarget.IsAlive() && base.RandomFloat < 0.95f)
		{
			return false;
		}
		float radius = this.theEntity.IsSleeper ? 7f : this.maxXZDistance;
		this.theEntity.world.GetEntitiesAround(this.targetFlags, this.targetFlags, this.theEntity.position, radius, EAISetNearestCorpseAsTarget.entityList);
		EAISetNearestCorpseAsTarget.entityList.Sort(this.sorter);
		EntityAlive entityAlive = null;
		for (int i = 0; i < EAISetNearestCorpseAsTarget.entityList.Count; i++)
		{
			EntityAlive entityAlive2 = EAISetNearestCorpseAsTarget.entityList[i] as EntityAlive;
			if (entityAlive2 && entityAlive2.IsDead())
			{
				entityAlive = entityAlive2;
				break;
			}
		}
		EAISetNearestCorpseAsTarget.entityList.Clear();
		this.targetEntity = entityAlive;
		return this.targetEntity != null;
	}

	// Token: 0x06001E7B RID: 7803 RVA: 0x000BE1F8 File Offset: 0x000BC3F8
	public override void Start()
	{
		base.Start();
		this.theEntity.SetAttackTarget(this.targetEntity, 600);
	}

	// Token: 0x06001E7C RID: 7804 RVA: 0x000BE216 File Offset: 0x000BC416
	public override bool Continue()
	{
		return this.targetEntity && this.targetEntity.IsDead() && !(this.targetEntity != this.theEntity.GetAttackTarget());
	}

	// Token: 0x04001502 RID: 5378
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive targetEntity;

	// Token: 0x04001503 RID: 5379
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityFlags targetFlags;

	// Token: 0x04001504 RID: 5380
	[PublicizedFrom(EAccessModifier.Private)]
	public int rndTimeout;

	// Token: 0x04001505 RID: 5381
	[PublicizedFrom(EAccessModifier.Private)]
	public EAISetNearestEntityAsTargetSorter sorter;

	// Token: 0x04001506 RID: 5382
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> entityList = new List<Entity>();
}
