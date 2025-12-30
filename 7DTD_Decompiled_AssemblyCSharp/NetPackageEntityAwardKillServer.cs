using System;
using UnityEngine.Scripting;

// Token: 0x02000734 RID: 1844
[Preserve]
public class NetPackageEntityAwardKillServer : NetPackage
{
	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x060035F5 RID: 13813 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060035F6 RID: 13814 RVA: 0x001656C4 File Offset: 0x001638C4
	public NetPackageEntityAwardKillServer Setup(int _killerEntityId, int _killedEntityId)
	{
		this.EntityId = _killerEntityId;
		this.KilledEntityId = _killedEntityId;
		return this;
	}

	// Token: 0x060035F7 RID: 13815 RVA: 0x001656D5 File Offset: 0x001638D5
	public override void read(PooledBinaryReader _reader)
	{
		this.EntityId = _reader.ReadInt32();
		this.KilledEntityId = _reader.ReadInt32();
	}

	// Token: 0x060035F8 RID: 13816 RVA: 0x001656EF File Offset: 0x001638EF
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.EntityId);
		_writer.Write(this.KilledEntityId);
	}

	// Token: 0x060035F9 RID: 13817 RVA: 0x00165710 File Offset: 0x00163910
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayerLocal entityPlayerLocal = _world.GetEntity(this.EntityId) as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			EntityAlive entityAlive = _world.GetEntity(this.KilledEntityId) as EntityAlive;
			if (entityAlive != null)
			{
				QuestEventManager.Current.EntityKilled(entityPlayerLocal, entityAlive);
			}
		}
	}

	// Token: 0x060035FA RID: 13818 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002BE5 RID: 11237
	public int EntityId;

	// Token: 0x04002BE6 RID: 11238
	public int KilledEntityId;
}
