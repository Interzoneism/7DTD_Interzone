using System;
using UnityEngine.Scripting;

// Token: 0x02000530 RID: 1328
[Preserve]
public class ItemActionProjectile : ItemActionAttack
{
	// Token: 0x06002AE8 RID: 10984 RVA: 0x0011B074 File Offset: 0x00119274
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		this.Explosion = new ExplosionData(this.Properties, this.item.Effects);
		this.Properties.ParseFloat("FlyTime", ref this.FlyTime);
		this.Properties.ParseFloat("LifeTime", ref this.LifeTime);
		this.Properties.ParseFloat("DeadTime", ref this.DeadTime);
		this.Properties.ParseFloat("Velocity", ref this.Velocity);
		this.Gravity = -9.81f;
		this.Properties.ParseFloat("Gravity", ref this.Gravity);
		this.Properties.ParseFloat("CollisionRadius", ref this.collisionRadius);
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x00002914 File Offset: 0x00000B14
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
	}

	// Token: 0x0400215D RID: 8541
	public new ExplosionData Explosion;

	// Token: 0x0400215E RID: 8542
	public new float Velocity;

	// Token: 0x0400215F RID: 8543
	public new float FlyTime;

	// Token: 0x04002160 RID: 8544
	public new float LifeTime;

	// Token: 0x04002161 RID: 8545
	public float DeadTime;

	// Token: 0x04002162 RID: 8546
	public float Gravity;

	// Token: 0x04002163 RID: 8547
	public float collisionRadius;
}
