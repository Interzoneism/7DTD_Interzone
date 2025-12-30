using System;

// Token: 0x0200040B RID: 1035
public struct DamageResponse
{
	// Token: 0x06001EF5 RID: 7925 RVA: 0x000C08E8 File Offset: 0x000BEAE8
	public static DamageResponse New(bool _fatal)
	{
		return new DamageResponse
		{
			HitBodyPart = EnumBodyPartHit.Torso,
			Random = GameManager.Instance.World.GetGameRandom().RandomFloat,
			Fatal = _fatal,
			PainHit = !_fatal,
			ImpulseScale = 1f,
			ArmorSlot = EquipmentSlots.Count,
			ArmorDamage = 0
		};
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000C0950 File Offset: 0x000BEB50
	public static DamageResponse New(DamageSource _source, bool _fatal)
	{
		return new DamageResponse
		{
			HitBodyPart = EnumBodyPartHit.Torso,
			Random = GameManager.Instance.World.GetGameRandom().RandomFloat,
			Fatal = _fatal,
			PainHit = !_fatal,
			Source = _source,
			ImpulseScale = 1f,
			ArmorSlot = EquipmentSlots.Count,
			ArmorDamage = 0
		};
	}

	// Token: 0x0400158B RID: 5515
	public DamageSource Source;

	// Token: 0x0400158C RID: 5516
	public int Strength;

	// Token: 0x0400158D RID: 5517
	public int ModStrength;

	// Token: 0x0400158E RID: 5518
	public int MovementState;

	// Token: 0x0400158F RID: 5519
	public Utils.EnumHitDirection HitDirection;

	// Token: 0x04001590 RID: 5520
	public EnumBodyPartHit HitBodyPart;

	// Token: 0x04001591 RID: 5521
	public bool PainHit;

	// Token: 0x04001592 RID: 5522
	public bool Fatal;

	// Token: 0x04001593 RID: 5523
	public bool Critical;

	// Token: 0x04001594 RID: 5524
	public bool Dismember;

	// Token: 0x04001595 RID: 5525
	public bool CrippleLegs;

	// Token: 0x04001596 RID: 5526
	public bool TurnIntoCrawler;

	// Token: 0x04001597 RID: 5527
	public float Random;

	// Token: 0x04001598 RID: 5528
	public float ImpulseScale;

	// Token: 0x04001599 RID: 5529
	public EnumEntityStunType Stun;

	// Token: 0x0400159A RID: 5530
	public float StunDuration;

	// Token: 0x0400159B RID: 5531
	public EquipmentSlots ArmorSlot;

	// Token: 0x0400159C RID: 5532
	public EquipmentSlotGroups ArmorSlotGroup;

	// Token: 0x0400159D RID: 5533
	public int ArmorDamage;
}
