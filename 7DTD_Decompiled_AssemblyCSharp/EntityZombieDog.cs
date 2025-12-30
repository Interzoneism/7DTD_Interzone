using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200047E RID: 1150
[Preserve]
public class EntityZombieDog : EntityEnemyAnimal
{
	// Token: 0x06002578 RID: 9592 RVA: 0x000F28BC File Offset: 0x000F0ABC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		base.Awake();
		Transform transform = base.transform.Find("Graphics/BlobShadowProjector");
		if (transform)
		{
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002579 RID: 9593 RVA: 0x000F28F4 File Offset: 0x000F0AF4
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		this.timeToDie = this.world.worldTime + 1800UL + (ulong)(22000f * this.rand.RandomFloat);
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x000F2928 File Offset: 0x000F0B28
	public override void InitFromPrefab(int _entityClass)
	{
		base.InitFromPrefab(_entityClass);
		this.timeToDie = this.world.worldTime + 1800UL + (ulong)(22000f * this.rand.RandomFloat);
	}

	// Token: 0x0600257B RID: 9595 RVA: 0x000F295C File Offset: 0x000F0B5C
	public override void OnUpdateLive()
	{
		base.OnUpdateLive();
		if (this.world.worldTime >= this.timeToDie && !this.isEntityRemote)
		{
			this.Kill(DamageResponse.New(true));
		}
	}

	// Token: 0x0600257C RID: 9596 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x0600257D RID: 9597 RVA: 0x000F298B File Offset: 0x000F0B8B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override int GetMaxAttackTime()
	{
		return 30;
	}

	// Token: 0x0600257E RID: 9598 RVA: 0x000F298F File Offset: 0x000F0B8F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityTargeted(EntityAlive target)
	{
		base.OnEntityTargeted(target);
	}

	// Token: 0x04001C80 RID: 7296
	public ulong timeToDie;
}
