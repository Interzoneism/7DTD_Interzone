using System;
using UnityEngine;

// Token: 0x020004C4 RID: 1220
public class DamageSourceEntity : DamageSource
{
	// Token: 0x060027EC RID: 10220 RVA: 0x001027FA File Offset: 0x001009FA
	public DamageSourceEntity(EnumDamageSource _damageSource, EnumDamageTypes _damageType, int _damageSourceEntityId) : base(_damageSource, _damageType)
	{
		this.ownerEntityId = _damageSourceEntityId;
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x0010280B File Offset: 0x00100A0B
	public DamageSourceEntity(EnumDamageSource _damageSource, EnumDamageTypes _damageType, int _damageSourceEntityId, Vector3 _direction) : base(_damageSource, _damageType, _direction)
	{
		this.ownerEntityId = _damageSourceEntityId;
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x0010281E File Offset: 0x00100A1E
	public DamageSourceEntity(EnumDamageSource _damageSource, EnumDamageTypes _damageType, int _damageSourceEntityId, Vector3 _direction, string _hitTransformName, Vector3 _hitTransformPosition, Vector2 _uvHit) : this(_damageSource, _damageType, _damageSourceEntityId, _direction)
	{
		this.hitTransformName = _hitTransformName;
		this.hitTransformPosition = _hitTransformPosition;
		this.uvHit = _uvHit;
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x00102843 File Offset: 0x00100A43
	public override Vector3 getHitTransformPosition()
	{
		return this.hitTransformPosition;
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x0010284B File Offset: 0x00100A4B
	public override string getHitTransformName()
	{
		return this.hitTransformName;
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x00102853 File Offset: 0x00100A53
	public override Vector2 getUVHit()
	{
		return this.uvHit;
	}

	// Token: 0x04001E93 RID: 7827
	public Vector2 uvHit;

	// Token: 0x04001E94 RID: 7828
	public string hitTransformName;

	// Token: 0x04001E95 RID: 7829
	public Vector3 hitTransformPosition;
}
