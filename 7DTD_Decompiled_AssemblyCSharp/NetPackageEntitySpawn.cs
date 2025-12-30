using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000741 RID: 1857
[Preserve]
public class NetPackageEntitySpawn : NetPackage
{
	// Token: 0x1700057F RID: 1407
	// (get) Token: 0x06003648 RID: 13896 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003649 RID: 13897 RVA: 0x001667F1 File Offset: 0x001649F1
	public NetPackageEntitySpawn Setup(EntityCreationData _es)
	{
		this.es = _es;
		return this;
	}

	// Token: 0x0600364A RID: 13898 RVA: 0x001667FB File Offset: 0x001649FB
	public override void read(PooledBinaryReader _reader)
	{
		this.es = new EntityCreationData();
		this.es.read(_reader, true);
	}

	// Token: 0x0600364B RID: 13899 RVA: 0x00166815 File Offset: 0x00164A15
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		this.es.write(_writer, true);
	}

	// Token: 0x0600364C RID: 13900 RVA: 0x0016682C File Offset: 0x00164A2C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.es.clientEntityId != 0)
		{
			List<EntityPlayerLocal> localPlayers = _world.GetLocalPlayers();
			for (int i = 0; i < localPlayers.Count; i++)
			{
				if (localPlayers[i].entityId == this.es.belongsPlayerId)
				{
					_world.ChangeClientEntityIdToServer(this.es.clientEntityId, this.es.id);
					return;
				}
			}
		}
		Entity entity = EntityFactory.CreateEntity(this.es);
		_world.SpawnEntityInWorld(entity);
	}

	// Token: 0x0600364D RID: 13901 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002C15 RID: 11285
	public EntityCreationData es;
}
