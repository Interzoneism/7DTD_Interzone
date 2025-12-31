using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000420 RID: 1056
[Preserve]
public abstract class EntityAnimal : EntityAlive
{
	// Token: 0x060020A6 RID: 8358 RVA: 0x000CBB2D File Offset: 0x000C9D2D
	public override void OnUpdateLive()
	{
		base.GetEntitySenses().Clear();
		base.OnUpdateLive();
	}

	// Token: 0x060020A7 RID: 8359 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsDrawMapIcon()
	{
		return false;
	}

	// Token: 0x060020A8 RID: 8360 RVA: 0x000CBB40 File Offset: 0x000C9D40
	public override Color GetMapIconColor()
	{
		return new Color(1f, 0.8235294f, 0.34117648f);
	}

	// Token: 0x060020A9 RID: 8361 RVA: 0x000CBB56 File Offset: 0x000C9D56
	[PublicizedFrom(EAccessModifier.Protected)]
	public override float getNextStepSoundDistance()
	{
		return 0.8f;
	}

	// Token: 0x060020AA RID: 8362 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isGameMessageOnDeath()
	{
		return false;
	}

	// Token: 0x060020AB RID: 8363 RVA: 0x000CBB5D File Offset: 0x000C9D5D
	public override int DamageEntity(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		return base.DamageEntity(_damageSource, _strength, _criticalHit, impulseScale);
	}

	// Token: 0x060020AC RID: 8364 RVA: 0x000CBB6A File Offset: 0x000C9D6A
	public override void OnEntityDeath()
	{
		if (this.PhysicsTransform)
		{
			this.PhysicsTransform.gameObject.SetActive(false);
		}
		base.OnEntityDeath();
	}

	// Token: 0x060020AD RID: 8365 RVA: 0x000CBB90 File Offset: 0x000C9D90
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAnimal()
	{
	}
}
