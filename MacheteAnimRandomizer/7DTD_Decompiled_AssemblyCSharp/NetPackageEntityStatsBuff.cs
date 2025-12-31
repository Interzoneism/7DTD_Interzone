using System;
using UnityEngine.Scripting;

// Token: 0x020001A1 RID: 417
[Preserve]
public class NetPackageEntityStatsBuff : NetPackage
{
	// Token: 0x170000DD RID: 221
	// (get) Token: 0x06000CD2 RID: 3282 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool ReliableDelivery
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0005773C File Offset: 0x0005593C
	public NetPackageEntityStatsBuff Setup(EntityAlive entity, byte[] _data = null)
	{
		this.m_entityId = entity.entityId;
		if (_data == null)
		{
			using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(true))
				{
					pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
					entity.Buffs.Write(pooledBinaryWriter, true);
					this.data = pooledExpandableMemoryStream.ToArray();
					return this;
				}
			}
		}
		this.data = _data;
		return this;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x000577CC File Offset: 0x000559CC
	public override void read(PooledBinaryReader _reader)
	{
		this.m_entityId = _reader.ReadInt32();
		this.data = _reader.ReadBytes(_reader.ReadInt32());
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000577EC File Offset: 0x000559EC
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_entityId);
		_writer.Write(this.data.Length);
		_writer.Write(this.data);
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x0005781C File Offset: 0x00055A1C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.m_entityId) as EntityAlive;
		if (entityAlive != null)
		{
			if (entityAlive.isEntityRemote)
			{
				using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
				{
					pooledExpandableMemoryStream.Write(this.data, 0, this.data.Length);
					pooledExpandableMemoryStream.Position = 0L;
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
					{
						pooledBinaryReader.SetBaseStream(pooledExpandableMemoryStream);
						entityAlive.Buffs.Read(pooledBinaryReader);
					}
				}
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStatsBuff>().Setup(entityAlive, this.data), false, -1, entityAlive.entityId, -1, null, 192, false);
			}
		}
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x0005790C File Offset: 0x00055B0C
	public override int GetLength()
	{
		return this.data.Length + 4;
	}

	// Token: 0x04000AA1 RID: 2721
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_entityId;

	// Token: 0x04000AA2 RID: 2722
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;
}
