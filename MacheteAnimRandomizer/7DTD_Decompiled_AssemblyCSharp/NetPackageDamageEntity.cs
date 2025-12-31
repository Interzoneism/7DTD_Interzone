using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000714 RID: 1812
[Preserve]
public class NetPackageDamageEntity : NetPackage
{
	// Token: 0x0600354A RID: 13642 RVA: 0x00162F40 File Offset: 0x00161140
	public NetPackageDamageEntity Setup(int _targetEntityId, DamageResponse _dmResponse)
	{
		this.entityId = _targetEntityId;
		DamageSource source = _dmResponse.Source;
		this.damageSrc = source.GetSource();
		this.damageTyp = source.GetDamageType();
		this.attackingItem = source.AttackingItem;
		int num = _dmResponse.Strength;
		if (num > 65535)
		{
			num = 65535;
		}
		this.strength = (ushort)num;
		this.hitDirection = (int)_dmResponse.HitDirection;
		this.hitBodyPart = (short)_dmResponse.HitBodyPart;
		this.movementState = _dmResponse.MovementState;
		this.bPainHit = _dmResponse.PainHit;
		this.bFatal = _dmResponse.Fatal;
		this.bCritical = _dmResponse.Critical;
		this.attackerEntityId = source.getEntityId();
		this.dirV = source.getDirection();
		this.blockPos = source.BlockPosition;
		this.hitTransformName = (source.getHitTransformName() ?? string.Empty);
		this.hitTransformPosition = source.getHitTransformPosition();
		this.uvHit = source.getUVHit();
		this.damageMultiplier = source.DamageMultiplier;
		this.bonusDamageType = (byte)source.BonusDamageType;
		this.random = _dmResponse.Random;
		this.bIgnoreConsecutiveDamages = source.IsIgnoreConsecutiveDamages();
		this.bDismember = _dmResponse.Dismember;
		this.bCrippleLegs = _dmResponse.CrippleLegs;
		this.bTurnIntoCrawler = _dmResponse.TurnIntoCrawler;
		this.StunType = (byte)_dmResponse.Stun;
		this.StunDuration = _dmResponse.StunDuration;
		this.ArmorSlot = _dmResponse.ArmorSlot;
		this.ArmorSlotGroup = _dmResponse.ArmorSlotGroup;
		this.ArmorDamage = _dmResponse.ArmorDamage;
		this.bFromBuff = (source.BuffClass != null);
		return this;
	}

	// Token: 0x0600354B RID: 13643 RVA: 0x001630D8 File Offset: 0x001612D8
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.damageSrc = (EnumDamageSource)_reader.ReadByte();
		this.damageTyp = (EnumDamageTypes)_reader.ReadByte();
		this.strength = _reader.ReadUInt16();
		this.hitDirection = (int)_reader.ReadByte();
		this.hitBodyPart = _reader.ReadInt16();
		this.movementState = (int)_reader.ReadByte();
		this.bPainHit = _reader.ReadBoolean();
		this.bFatal = _reader.ReadBoolean();
		this.bCritical = _reader.ReadBoolean();
		this.attackerEntityId = _reader.ReadInt32();
		this.dirV.x = _reader.ReadSingle();
		this.dirV.y = _reader.ReadSingle();
		this.dirV.z = _reader.ReadSingle();
		this.blockPos = StreamUtils.ReadVector3i(_reader);
		this.hitTransformName = _reader.ReadString();
		this.hitTransformPosition.x = _reader.ReadSingle();
		this.hitTransformPosition.y = _reader.ReadSingle();
		this.hitTransformPosition.z = _reader.ReadSingle();
		this.uvHit.x = _reader.ReadSingle();
		this.uvHit.y = _reader.ReadSingle();
		this.damageMultiplier = _reader.ReadSingle();
		this.random = _reader.ReadSingle();
		this.bIgnoreConsecutiveDamages = _reader.ReadBoolean();
		this.bIsDamageTransfer = _reader.ReadBoolean();
		this.bDismember = _reader.ReadBoolean();
		this.bCrippleLegs = _reader.ReadBoolean();
		this.bTurnIntoCrawler = _reader.ReadBoolean();
		this.bonusDamageType = _reader.ReadByte();
		this.StunType = _reader.ReadByte();
		this.StunDuration = _reader.ReadSingle();
		this.bFromBuff = _reader.ReadBoolean();
		this.ArmorSlot = (EquipmentSlots)_reader.ReadByte();
		this.ArmorSlotGroup = (EquipmentSlotGroups)_reader.ReadByte();
		this.ArmorDamage = (int)_reader.ReadInt16();
		if (_reader.ReadBoolean())
		{
			this.attackingItem = new ItemValue();
			this.attackingItem.Read(_reader);
		}
	}

	// Token: 0x0600354C RID: 13644 RVA: 0x001632D0 File Offset: 0x001614D0
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write((byte)this.damageSrc);
		_writer.Write((byte)this.damageTyp);
		_writer.Write(this.strength);
		_writer.Write((byte)this.hitDirection);
		_writer.Write(this.hitBodyPart);
		_writer.Write((byte)this.movementState);
		_writer.Write(this.bPainHit);
		_writer.Write(this.bFatal);
		_writer.Write(this.bCritical);
		_writer.Write(this.attackerEntityId);
		_writer.Write(this.dirV.x);
		_writer.Write(this.dirV.y);
		_writer.Write(this.dirV.z);
		StreamUtils.Write(_writer, this.blockPos);
		_writer.Write(this.hitTransformName);
		_writer.Write(this.hitTransformPosition.x);
		_writer.Write(this.hitTransformPosition.y);
		_writer.Write(this.hitTransformPosition.z);
		_writer.Write(this.uvHit.x);
		_writer.Write(this.uvHit.y);
		_writer.Write(this.damageMultiplier);
		_writer.Write(this.random);
		_writer.Write(this.bIgnoreConsecutiveDamages);
		_writer.Write(this.bIsDamageTransfer);
		_writer.Write(this.bDismember);
		_writer.Write(this.bCrippleLegs);
		_writer.Write(this.bTurnIntoCrawler);
		_writer.Write(this.bonusDamageType);
		_writer.Write(this.StunType);
		_writer.Write(this.StunDuration);
		_writer.Write(this.bFromBuff);
		_writer.Write((byte)this.ArmorSlot);
		_writer.Write((byte)this.ArmorSlotGroup);
		_writer.Write((ushort)this.ArmorDamage);
		_writer.Write(this.attackingItem != null);
		if (this.attackingItem != null)
		{
			this.attackingItem.Write(_writer);
		}
	}

	// Token: 0x0600354D RID: 13645 RVA: 0x001634DC File Offset: 0x001616DC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (_world.GetPrimaryPlayer() != null && _world.GetPrimaryPlayer().entityId == this.entityId)
		{
			if (this.damageTyp == EnumDamageTypes.Falling)
			{
				return;
			}
			if (this.damageSrc == EnumDamageSource.External && (this.damageTyp == EnumDamageTypes.Piercing || this.damageTyp == EnumDamageTypes.BarbedWire) && this.attackerEntityId == -1)
			{
				return;
			}
		}
		Entity entity = _world.GetEntity(this.entityId);
		if (entity != null)
		{
			DamageSource damageSource = new DamageSourceEntity(this.damageSrc, this.damageTyp, this.attackerEntityId, this.dirV, this.hitTransformName, this.hitTransformPosition, this.uvHit);
			damageSource.SetIgnoreConsecutiveDamages(this.bIgnoreConsecutiveDamages);
			damageSource.DamageMultiplier = this.damageMultiplier;
			damageSource.BonusDamageType = (EnumDamageBonusType)this.bonusDamageType;
			damageSource.AttackingItem = this.attackingItem;
			damageSource.BlockPosition = this.blockPos;
			DamageResponse damageResponse = default(DamageResponse);
			damageResponse.Strength = (int)this.strength;
			damageResponse.ModStrength = 0;
			damageResponse.MovementState = this.movementState;
			damageResponse.HitDirection = (Utils.EnumHitDirection)this.hitDirection;
			damageResponse.HitBodyPart = (EnumBodyPartHit)this.hitBodyPart;
			damageResponse.PainHit = this.bPainHit;
			damageResponse.Fatal = this.bFatal;
			damageResponse.Critical = this.bCritical;
			damageResponse.Random = this.random;
			damageResponse.Source = damageSource;
			damageResponse.CrippleLegs = this.bCrippleLegs;
			damageResponse.Dismember = this.bDismember;
			damageResponse.TurnIntoCrawler = this.bTurnIntoCrawler;
			damageResponse.Stun = (EnumEntityStunType)this.StunType;
			damageResponse.StunDuration = this.StunDuration;
			damageResponse.ArmorSlot = this.ArmorSlot;
			damageResponse.ArmorSlotGroup = this.ArmorSlotGroup;
			damageResponse.ArmorDamage = this.ArmorDamage;
			if (this.bFromBuff)
			{
				damageResponse.Source.BuffClass = new BuffClass("");
			}
			entity.FireAttackedEvents(damageResponse);
			entity.ProcessDamageResponse(damageResponse);
		}
	}

	// Token: 0x0600354E RID: 13646 RVA: 0x0015DCEC File Offset: 0x0015BEEC
	public override int GetLength()
	{
		return 50;
	}

	// Token: 0x04002B5C RID: 11100
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002B5D RID: 11101
	[PublicizedFrom(EAccessModifier.Private)]
	public int attackerEntityId;

	// Token: 0x04002B5E RID: 11102
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumDamageSource damageSrc;

	// Token: 0x04002B5F RID: 11103
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumDamageTypes damageTyp;

	// Token: 0x04002B60 RID: 11104
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort strength;

	// Token: 0x04002B61 RID: 11105
	[PublicizedFrom(EAccessModifier.Private)]
	public int hitDirection;

	// Token: 0x04002B62 RID: 11106
	[PublicizedFrom(EAccessModifier.Private)]
	public short hitBodyPart;

	// Token: 0x04002B63 RID: 11107
	[PublicizedFrom(EAccessModifier.Private)]
	public int movementState;

	// Token: 0x04002B64 RID: 11108
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 dirV;

	// Token: 0x04002B65 RID: 11109
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockPos;

	// Token: 0x04002B66 RID: 11110
	[PublicizedFrom(EAccessModifier.Private)]
	public string hitTransformName;

	// Token: 0x04002B67 RID: 11111
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 hitTransformPosition;

	// Token: 0x04002B68 RID: 11112
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 uvHit;

	// Token: 0x04002B69 RID: 11113
	[PublicizedFrom(EAccessModifier.Private)]
	public float damageMultiplier;

	// Token: 0x04002B6A RID: 11114
	[PublicizedFrom(EAccessModifier.Private)]
	public float random;

	// Token: 0x04002B6B RID: 11115
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bIgnoreConsecutiveDamages;

	// Token: 0x04002B6C RID: 11116
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bPainHit;

	// Token: 0x04002B6D RID: 11117
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFatal;

	// Token: 0x04002B6E RID: 11118
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bCritical;

	// Token: 0x04002B6F RID: 11119
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bIsDamageTransfer;

	// Token: 0x04002B70 RID: 11120
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDismember;

	// Token: 0x04002B71 RID: 11121
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bCrippleLegs;

	// Token: 0x04002B72 RID: 11122
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bTurnIntoCrawler;

	// Token: 0x04002B73 RID: 11123
	[PublicizedFrom(EAccessModifier.Private)]
	public byte bonusDamageType;

	// Token: 0x04002B74 RID: 11124
	[PublicizedFrom(EAccessModifier.Private)]
	public byte StunType;

	// Token: 0x04002B75 RID: 11125
	[PublicizedFrom(EAccessModifier.Private)]
	public float StunDuration;

	// Token: 0x04002B76 RID: 11126
	[PublicizedFrom(EAccessModifier.Private)]
	public EquipmentSlots ArmorSlot;

	// Token: 0x04002B77 RID: 11127
	[PublicizedFrom(EAccessModifier.Private)]
	public EquipmentSlotGroups ArmorSlotGroup;

	// Token: 0x04002B78 RID: 11128
	[PublicizedFrom(EAccessModifier.Private)]
	public int ArmorDamage;

	// Token: 0x04002B79 RID: 11129
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue attackingItem;

	// Token: 0x04002B7A RID: 11130
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFromBuff;
}
