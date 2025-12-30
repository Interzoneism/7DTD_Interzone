using System;
using UnityEngine;

// Token: 0x020004C3 RID: 1219
public class DamageSource
{
	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x060027D5 RID: 10197 RVA: 0x00102088 File Offset: 0x00100288
	public ItemClass ItemClass
	{
		get
		{
			if (this.AttackingItem != null)
			{
				return this.AttackingItem.ItemClass;
			}
			return null;
		}
	}

	// Token: 0x060027D6 RID: 10198 RVA: 0x0010209F File Offset: 0x0010029F
	public DamageSource(EnumDamageSource _dsn, EnumDamageTypes _damageType)
	{
		this.damageSource = _dsn;
		this.damageType = _damageType;
		this.DamageTypeTag = FastTags<TagGroup.Global>.Parse(_damageType.ToStringCached<EnumDamageTypes>());
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x001020E0 File Offset: 0x001002E0
	public DamageSource(EnumDamageSource _dsn, EnumDamageTypes _damageType, Vector3 _direction)
	{
		this.damageSource = _dsn;
		this.damageType = _damageType;
		this.direction = _direction;
		this.DamageTypeTag = FastTags<TagGroup.Global>.Parse(_damageType.ToStringCached<EnumDamageTypes>());
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x00102132 File Offset: 0x00100332
	public bool AffectedByArmor()
	{
		return this.damageSource == EnumDamageSource.External;
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x00102140 File Offset: 0x00100340
	public EquipmentSlots GetEntityDamageEquipmentSlot(Entity entity)
	{
		if (entity.emodel)
		{
			Transform hitTransform = entity.emodel.GetHitTransform(this);
			if (hitTransform)
			{
				string tag = hitTransform.tag;
				if ("E_BP_Head".Equals(tag))
				{
					return EquipmentSlots.Head;
				}
				if ("E_BP_Body".Equals(tag))
				{
					return EquipmentSlots.Chest;
				}
				if ("E_BP_LLeg".Equals(tag))
				{
					return EquipmentSlots.Chest;
				}
				if ("E_BP_RLeg".Equals(tag))
				{
					return EquipmentSlots.Chest;
				}
				if ("E_BP_LArm".Equals(tag))
				{
					return EquipmentSlots.Hands;
				}
				if ("E_BP_RArm".Equals(tag))
				{
					return EquipmentSlots.Hands;
				}
			}
		}
		return EquipmentSlots.Count;
	}

	// Token: 0x060027DA RID: 10202 RVA: 0x001021D4 File Offset: 0x001003D4
	public EquipmentSlotGroups GetEntityDamageEquipmentSlotGroup(Entity entity)
	{
		if (entity.emodel)
		{
			Transform hitTransform = entity.emodel.GetHitTransform(this);
			if (hitTransform)
			{
				string tag = hitTransform.tag;
				if ("E_BP_Head".Equals(tag))
				{
					return EquipmentSlotGroups.Head;
				}
				if ("E_BP_Body".Equals(tag))
				{
					return EquipmentSlotGroups.UpperBody;
				}
				if ("E_BP_LLeg".Equals(tag))
				{
					return EquipmentSlotGroups.LowerBody;
				}
				if ("E_BP_RLeg".Equals(tag))
				{
					return EquipmentSlotGroups.LowerBody;
				}
				if ("E_BP_LArm".Equals(tag))
				{
					return EquipmentSlotGroups.UpperBody;
				}
				"E_BP_RArm".Equals(tag);
				return EquipmentSlotGroups.UpperBody;
			}
		}
		return EquipmentSlotGroups.UpperBody;
	}

	// Token: 0x060027DB RID: 10203 RVA: 0x00102264 File Offset: 0x00100464
	public EnumBodyPartHit GetEntityDamageBodyPart(Entity entity)
	{
		if (this.bodyParts != EnumBodyPartHit.None)
		{
			return this.bodyParts;
		}
		if (entity.emodel)
		{
			Transform hitTransform = entity.emodel.GetHitTransform(this);
			if (hitTransform)
			{
				return DamageSource.TagToBodyPart(hitTransform.tag);
			}
		}
		return EnumBodyPartHit.None;
	}

	// Token: 0x060027DC RID: 10204 RVA: 0x001022B0 File Offset: 0x001004B0
	public static EnumBodyPartHit TagToBodyPart(string _name)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_name);
		if (num <= 1181961453U)
		{
			if (num <= 719580451U)
			{
				if (num != 265341418U)
				{
					if (num == 719580451U)
					{
						if (_name == "E_BP_Special")
						{
							return EnumBodyPartHit.Special;
						}
					}
				}
				else if (_name == "E_BP_Head")
				{
					return EnumBodyPartHit.Head;
				}
			}
			else if (num != 769391494U)
			{
				if (num != 1103658916U)
				{
					if (num == 1181961453U)
					{
						if (_name == "E_BP_RLowerLeg")
						{
							return EnumBodyPartHit.RightLowerLeg;
						}
					}
				}
				else if (_name == "E_BP_Body")
				{
					return EnumBodyPartHit.Torso;
				}
			}
			else if (_name == "E_BP_LArm")
			{
				return EnumBodyPartHit.LeftUpperArm;
			}
		}
		else if (num <= 2493191411U)
		{
			if (num != 1478707584U)
			{
				if (num != 2129509723U)
				{
					if (num == 2493191411U)
					{
						if (_name == "E_BP_RLowerArm")
						{
							return EnumBodyPartHit.RightLowerArm;
						}
					}
				}
				else if (_name == "E_BP_LLowerLeg")
				{
					return EnumBodyPartHit.LeftLowerLeg;
				}
			}
			else if (_name == "E_BP_RArm")
			{
				return EnumBodyPartHit.RightUpperArm;
			}
		}
		else if (num != 2661128377U)
		{
			if (num != 2886638712U)
			{
				if (num == 3661196970U)
				{
					if (_name == "E_BP_RLeg")
					{
						return EnumBodyPartHit.RightUpperLeg;
					}
				}
			}
			else if (_name == "E_BP_LLeg")
			{
				return EnumBodyPartHit.LeftUpperLeg;
			}
		}
		else if (_name == "E_BP_LLowerArm")
		{
			return EnumBodyPartHit.LeftLowerArm;
		}
		return EnumBodyPartHit.None;
	}

	// Token: 0x060027DD RID: 10205 RVA: 0x00102444 File Offset: 0x00100644
	public void GetEntityDamageBodyPartAndEquipmentSlot(Entity entity, out EnumBodyPartHit bodyPartHit, out EquipmentSlots damageSlot)
	{
		damageSlot = EquipmentSlots.Count;
		bodyPartHit = EnumBodyPartHit.None;
		if (entity.emodel)
		{
			Transform hitTransform = entity.emodel.GetHitTransform(this);
			if (hitTransform)
			{
				string tag = hitTransform.tag;
				if ("E_BP_Head".Equals(tag))
				{
					damageSlot = EquipmentSlots.Head;
					bodyPartHit = EnumBodyPartHit.Head;
					return;
				}
				if ("E_BP_Body".Equals(tag))
				{
					damageSlot = EquipmentSlots.Chest;
					bodyPartHit = EnumBodyPartHit.Torso;
					return;
				}
				if ("E_BP_LLeg".Equals(tag))
				{
					damageSlot = EquipmentSlots.Chest;
					bodyPartHit = EnumBodyPartHit.LeftUpperLeg;
					return;
				}
				if ("E_BP_LLowerLeg".Equals(tag))
				{
					damageSlot = EquipmentSlots.Feet;
					bodyPartHit = EnumBodyPartHit.LeftLowerLeg;
					return;
				}
				if ("E_BP_RLeg".Equals(tag))
				{
					damageSlot = EquipmentSlots.Chest;
					bodyPartHit = EnumBodyPartHit.RightUpperLeg;
					return;
				}
				if ("E_BP_RLowerLeg".Equals(tag))
				{
					damageSlot = EquipmentSlots.Feet;
					bodyPartHit = EnumBodyPartHit.RightLowerLeg;
					return;
				}
				if ("E_BP_LArm".Equals(tag))
				{
					damageSlot = EquipmentSlots.Hands;
					bodyPartHit = EnumBodyPartHit.LeftUpperArm;
					return;
				}
				if ("E_BP_LLowerArm".Equals(tag))
				{
					damageSlot = EquipmentSlots.Hands;
					bodyPartHit = EnumBodyPartHit.LeftLowerArm;
					return;
				}
				if ("E_BP_RArm".Equals(tag))
				{
					damageSlot = EquipmentSlots.Hands;
					bodyPartHit = EnumBodyPartHit.RightUpperArm;
					return;
				}
				if ("E_BP_RLowerArm".Equals(tag))
				{
					damageSlot = EquipmentSlots.Hands;
					bodyPartHit = EnumBodyPartHit.RightLowerArm;
					return;
				}
			}
		}
		else
		{
			if (this.damageType == EnumDamageTypes.Falling)
			{
				bodyPartHit = EnumBodyPartHit.RightLowerLeg;
				damageSlot = EquipmentSlots.Feet;
				return;
			}
			bodyPartHit = EnumBodyPartHit.Torso;
			damageSlot = EquipmentSlots.Chest;
		}
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x00102578 File Offset: 0x00100778
	public EquipmentSlots GetDamagedEquipmentSlot(Entity entity)
	{
		if (entity.emodel)
		{
			Transform hitTransform = entity.emodel.GetHitTransform(this);
			if (hitTransform)
			{
				string tag = hitTransform.tag;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(tag);
				if (num <= 1478707584U)
				{
					if (num <= 769391494U)
					{
						if (num != 265341418U)
						{
							if (num == 769391494U)
							{
								if (tag == "E_BP_LArm")
								{
									return EquipmentSlots.Chest;
								}
							}
						}
						else if (tag == "E_BP_Head")
						{
							return EquipmentSlots.Head;
						}
					}
					else if (num != 1103658916U)
					{
						if (num != 1181961453U)
						{
							if (num == 1478707584U)
							{
								if (tag == "E_BP_RArm")
								{
									return EquipmentSlots.Chest;
								}
							}
						}
						else if (tag == "E_BP_RLowerLeg")
						{
							return EquipmentSlots.Feet;
						}
					}
					else if (tag == "E_BP_Body")
					{
						return EquipmentSlots.Chest;
					}
				}
				else if (num <= 2493191411U)
				{
					if (num != 2129509723U)
					{
						if (num == 2493191411U)
						{
							if (tag == "E_BP_RLowerArm")
							{
								return EquipmentSlots.Hands;
							}
						}
					}
					else if (tag == "E_BP_LLowerLeg")
					{
						return EquipmentSlots.Feet;
					}
				}
				else if (num != 2661128377U)
				{
					if (num != 2886638712U)
					{
						if (num == 3661196970U)
						{
							if (tag == "E_BP_RLeg")
							{
								return EquipmentSlots.Chest;
							}
						}
					}
					else if (tag == "E_BP_LLeg")
					{
						return EquipmentSlots.Chest;
					}
				}
				else if (tag == "E_BP_LLowerArm")
				{
					return EquipmentSlots.Hands;
				}
				return EquipmentSlots.Chest;
			}
		}
		else if (this.damageType == EnumDamageTypes.Falling)
		{
			return EquipmentSlots.Feet;
		}
		return EquipmentSlots.Chest;
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x00102708 File Offset: 0x00100908
	public virtual Vector3 getDirection()
	{
		return this.direction;
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x00102710 File Offset: 0x00100910
	public virtual int getEntityId()
	{
		return this.ownerEntityId;
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x00019766 File Offset: 0x00017966
	public virtual string getHitTransformName()
	{
		return null;
	}

	// Token: 0x060027E2 RID: 10210 RVA: 0x000470CA File Offset: 0x000452CA
	public virtual Vector3 getHitTransformPosition()
	{
		return Vector3.zero;
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x00047136 File Offset: 0x00045336
	public virtual Vector2 getUVHit()
	{
		return Vector2.zero;
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x00102718 File Offset: 0x00100918
	public virtual EnumDamageSource GetSource()
	{
		return this.damageSource;
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x00102720 File Offset: 0x00100920
	public virtual EnumDamageTypes GetDamageType()
	{
		return this.damageType;
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x060027E6 RID: 10214 RVA: 0x00102728 File Offset: 0x00100928
	public bool CanStun
	{
		get
		{
			return this.damageType == EnumDamageTypes.Bashing || this.damageType == EnumDamageTypes.Heat || this.damageType == EnumDamageTypes.Piercing || this.damageType == EnumDamageTypes.Crushing || this.damageType == EnumDamageTypes.Falling;
		}
	}

	// Token: 0x060027E7 RID: 10215 RVA: 0x0010275A File Offset: 0x0010095A
	public void SetIgnoreConsecutiveDamages(bool _b)
	{
		this.bIgnoreConsecutiveDamages = _b;
	}

	// Token: 0x060027E8 RID: 10216 RVA: 0x00102763 File Offset: 0x00100963
	public virtual bool IsIgnoreConsecutiveDamages()
	{
		return this.bIgnoreConsecutiveDamages;
	}

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x060027E9 RID: 10217 RVA: 0x0010276B File Offset: 0x0010096B
	// (set) Token: 0x060027EA RID: 10218 RVA: 0x00102773 File Offset: 0x00100973
	public float DamageMultiplier
	{
		get
		{
			return this.damageMultiplier;
		}
		set
		{
			this.damageMultiplier = value;
		}
	}

	// Token: 0x04001E7C RID: 7804
	public static readonly DamageSource eat = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Slashing);

	// Token: 0x04001E7D RID: 7805
	public static readonly DamageSource fallingBlock = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Crushing);

	// Token: 0x04001E7E RID: 7806
	public static readonly DamageSource radiation = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Radiation);

	// Token: 0x04001E7F RID: 7807
	public static readonly DamageSource fall = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Falling);

	// Token: 0x04001E80 RID: 7808
	public static readonly DamageSource starve = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Starvation);

	// Token: 0x04001E81 RID: 7809
	public static readonly DamageSource dehydrate = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Dehydration);

	// Token: 0x04001E82 RID: 7810
	public static readonly DamageSource radiationSickness = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Radiation);

	// Token: 0x04001E83 RID: 7811
	public static readonly DamageSource disease = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Disease);

	// Token: 0x04001E84 RID: 7812
	public static readonly DamageSource suffocating = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suffocation);

	// Token: 0x04001E85 RID: 7813
	public BuffClass BuffClass;

	// Token: 0x04001E86 RID: 7814
	public ItemValue AttackingItem;

	// Token: 0x04001E87 RID: 7815
	public EnumDamageSource damageSource;

	// Token: 0x04001E88 RID: 7816
	public readonly EnumDamageTypes damageType;

	// Token: 0x04001E89 RID: 7817
	public EnumBodyPartHit bodyParts;

	// Token: 0x04001E8A RID: 7818
	public float DismemberChance;

	// Token: 0x04001E8B RID: 7819
	public EnumDamageBonusType BonusDamageType;

	// Token: 0x04001E8C RID: 7820
	public FastTags<TagGroup.Global> DamageTypeTag;

	// Token: 0x04001E8D RID: 7821
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bIgnoreConsecutiveDamages;

	// Token: 0x04001E8E RID: 7822
	[PublicizedFrom(EAccessModifier.Private)]
	public float damageMultiplier = 1f;

	// Token: 0x04001E8F RID: 7823
	[PublicizedFrom(EAccessModifier.Protected)]
	public int ownerEntityId = -1;

	// Token: 0x04001E90 RID: 7824
	public int CreatorEntityId = -1;

	// Token: 0x04001E91 RID: 7825
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 direction;

	// Token: 0x04001E92 RID: 7826
	public Vector3i BlockPosition;
}
