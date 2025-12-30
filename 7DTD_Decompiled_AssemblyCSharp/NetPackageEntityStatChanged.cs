using System;
using UnityEngine.Scripting;

// Token: 0x0200019F RID: 415
[Preserve]
public class NetPackageEntityStatChanged : NetPackage
{
	// Token: 0x06000CCB RID: 3275 RVA: 0x000574B8 File Offset: 0x000556B8
	public NetPackageEntityStatChanged Setup(EntityAlive entity, int instigatorId, NetPackageEntityStatChanged.EnumStat Estat)
	{
		this.m_entityId = entity.entityId;
		this.m_instigatorId = instigatorId;
		this.m_enumStat = Estat;
		Stat stat = NetPackageEntityStatChanged.GetStat(entity, Estat);
		this.m_value = stat.Value;
		this.m_max = stat.BaseMax;
		this.m_maxModifier = stat.MaxModifier;
		return this;
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0005750C File Offset: 0x0005570C
	[PublicizedFrom(EAccessModifier.Private)]
	public static Stat GetStat(EntityAlive entity, NetPackageEntityStatChanged.EnumStat stat)
	{
		if (stat <= NetPackageEntityStatChanged.EnumStat.Stamina)
		{
			if (stat == NetPackageEntityStatChanged.EnumStat.Health)
			{
				return entity.Stats.Health;
			}
			if (stat == NetPackageEntityStatChanged.EnumStat.Stamina)
			{
				return entity.Stats.Stamina;
			}
		}
		else
		{
			if (stat == NetPackageEntityStatChanged.EnumStat.CoreTemp)
			{
				return ((EntityPlayer)entity).PlayerStats.CoreTemp;
			}
			if (stat == NetPackageEntityStatChanged.EnumStat.Water)
			{
				return entity.Stats.Water;
			}
		}
		return entity.Stats.Health;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x00057570 File Offset: 0x00055770
	public override void read(PooledBinaryReader _reader)
	{
		this.m_entityId = _reader.ReadInt32();
		this.m_instigatorId = _reader.ReadInt32();
		this.m_enumStat = (NetPackageEntityStatChanged.EnumStat)_reader.ReadByte();
		this.m_value = _reader.ReadSingle();
		this.m_max = _reader.ReadSingle();
		this.m_maxModifier = _reader.ReadSingle();
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x000575C8 File Offset: 0x000557C8
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_entityId);
		_writer.Write(this.m_instigatorId);
		_writer.Write((byte)this.m_enumStat);
		_writer.Write(this.m_value);
		_writer.Write(this.m_max);
		_writer.Write(this.m_maxModifier);
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x00057628 File Offset: 0x00055828
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (this.m_entityId == _world.GetPrimaryPlayerId() && this.m_entityId == this.m_instigatorId)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.m_instigatorId, false))
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.m_entityId) as EntityAlive;
		if (entityAlive != null)
		{
			Stat stat = NetPackageEntityStatChanged.GetStat(entityAlive, this.m_enumStat);
			stat.BaseMax = this.m_max;
			stat.MaxModifier = this.m_maxModifier;
			stat.Value = this.m_value;
			stat.Changed = false;
			if (!entityAlive.isEntityRemote && this.m_enumStat == NetPackageEntityStatChanged.EnumStat.Health)
			{
				entityAlive.MinEventContext.Other = (_world.GetEntity(this.m_instigatorId) as EntityAlive);
				entityAlive.FireEvent(MinEventTypes.onOtherHealedSelf, true);
			}
			if (!_world.IsRemote())
			{
				_world.entityDistributer.SendPacketToTrackedPlayersAndTrackedEntity(entityAlive.entityId, this.m_instigatorId, NetPackageManager.GetPackage<NetPackageEntityStatChanged>().Setup(entityAlive, this.m_instigatorId, this.m_enumStat), this.m_enumStat > NetPackageEntityStatChanged.EnumStat.Health);
			}
		}
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0005772E File Offset: 0x0005592E
	public override int GetLength()
	{
		return 21;
	}

	// Token: 0x04000A90 RID: 2704
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_entityId;

	// Token: 0x04000A91 RID: 2705
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_instigatorId;

	// Token: 0x04000A92 RID: 2706
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageEntityStatChanged.EnumStat m_enumStat;

	// Token: 0x04000A93 RID: 2707
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_value;

	// Token: 0x04000A94 RID: 2708
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_max;

	// Token: 0x04000A95 RID: 2709
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_maxModifier;

	// Token: 0x04000A96 RID: 2710
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_valueModifier;

	// Token: 0x020001A0 RID: 416
	public enum EnumStat
	{
		// Token: 0x04000A98 RID: 2712
		Health,
		// Token: 0x04000A99 RID: 2713
		Stamina,
		// Token: 0x04000A9A RID: 2714
		Sickness,
		// Token: 0x04000A9B RID: 2715
		Gassiness,
		// Token: 0x04000A9C RID: 2716
		SpeedModifier,
		// Token: 0x04000A9D RID: 2717
		Wellness,
		// Token: 0x04000A9E RID: 2718
		CoreTemp,
		// Token: 0x04000A9F RID: 2719
		Food,
		// Token: 0x04000AA0 RID: 2720
		Water
	}
}
