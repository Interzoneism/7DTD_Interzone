using System;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public abstract class EntityEnemy : EntityAlive
{
	// Token: 0x06002200 RID: 8704 RVA: 0x000D016C File Offset: 0x000CE36C
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x000D619E File Offset: 0x000D439E
	public override void InitFromPrefab(int _entityClass)
	{
		base.InitFromPrefab(_entityClass);
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x000D61A7 File Offset: 0x000D43A7
	public override void PostInit()
	{
		base.PostInit();
		if (!this.isEntityRemote)
		{
			this.IsBloodMoon = this.world.aiDirector.BloodMoonComponent.BloodMoonActive;
		}
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsDrawMapIcon()
	{
		return true;
	}

	// Token: 0x06002204 RID: 8708 RVA: 0x000D61D2 File Offset: 0x000D43D2
	public override Vector3 GetMapIconScale()
	{
		return new Vector3(0.75f, 0.75f, 1f);
	}

	// Token: 0x06002205 RID: 8709 RVA: 0x000D61E8 File Offset: 0x000D43E8
	public override bool IsSavedToFile()
	{
		return (base.GetSpawnerSource() != EnumSpawnerSource.Dynamic || this.IsDead()) && base.IsSavedToFile();
	}

	// Token: 0x06002206 RID: 8710 RVA: 0x000D6203 File Offset: 0x000D4403
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool canDespawn()
	{
		return (!this.IsHordeZombie || this.world.GetPlayers().Count == 0) && base.canDespawn();
	}

	// Token: 0x06002207 RID: 8711 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDetailedHeadBodyColliders()
	{
		return true;
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x000D6227 File Offset: 0x000D4427
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnEntityTargeted(EntityAlive target)
	{
		base.OnEntityTargeted(target);
		if (!this.isEntityRemote && base.GetSpawnerSource() != EnumSpawnerSource.Dynamic && target is EntityPlayer)
		{
			this.world.aiDirector.NotifyIntentToAttack(this, target as EntityPlayer);
		}
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x000CBB5D File Offset: 0x000C9D5D
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float _impulseScale)
	{
		return base.DamageEntity(_damageSource, _strength, _criticalHit, _impulseScale);
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x000CBB90 File Offset: 0x000C9D90
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityEnemy()
	{
	}

	// Token: 0x04001971 RID: 6513
	public bool IsHordeZombie;
}
