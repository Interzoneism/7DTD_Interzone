using System;
using UnityEngine.Scripting;

// Token: 0x0200077A RID: 1914
[Preserve]
public class NetPackagePlayerStats : NetPackage
{
	// Token: 0x060037AB RID: 14251 RVA: 0x0016B96C File Offset: 0x00169B6C
	public NetPackagePlayerStats Setup(EntityAlive _entity)
	{
		this.entityId = _entity.entityId;
		this.entityNetworkStats.FillFromEntity(_entity);
		return this;
	}

	// Token: 0x060037AC RID: 14252 RVA: 0x0016B988 File Offset: 0x00169B88
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackagePlayerStats()
	{
	}

	// Token: 0x060037AD RID: 14253 RVA: 0x0016B9B0 File Offset: 0x00169BB0
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.entityNetworkStats.read(_reader);
	}

	// Token: 0x060037AE RID: 14254 RVA: 0x0016B9CA File Offset: 0x00169BCA
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		this.entityNetworkStats.write(_writer);
	}

	// Token: 0x060037AF RID: 14255 RVA: 0x0016B9EC File Offset: 0x00169BEC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.entityId) as EntityAlive;
		if (!entityAlive)
		{
			Log.Out("Discarding " + base.GetType().Name + " for entity Id=" + this.entityId.ToString());
			return;
		}
		if (!base.ValidEntityIdForSender(this.entityId, true))
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && entityAlive is EntityPlayer)
		{
			this.entityNetworkStats.SetName(base.Sender.playerName);
		}
		this.entityNetworkStats.ToEntity(entityAlive);
		entityAlive.EnqueueNetworkStats(this.entityNetworkStats);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(entityAlive), false, -1, base.Sender.entityId, -1, null, 192, false);
		}
	}

	// Token: 0x060037B0 RID: 14256 RVA: 0x0015DD3D File Offset: 0x0015BF3D
	public override int GetLength()
	{
		return 60;
	}

	// Token: 0x04002D0B RID: 11531
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002D0C RID: 11532
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive.EntityNetworkStats entityNetworkStats = new EntityAlive.EntityNetworkStats();
}
