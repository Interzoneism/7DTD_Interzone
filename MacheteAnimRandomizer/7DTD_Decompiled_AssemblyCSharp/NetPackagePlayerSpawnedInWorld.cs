using System;
using UnityEngine.Scripting;

// Token: 0x02000779 RID: 1913
[Preserve]
public class NetPackagePlayerSpawnedInWorld : NetPackage
{
	// Token: 0x060037A4 RID: 14244 RVA: 0x0016B862 File Offset: 0x00169A62
	public NetPackagePlayerSpawnedInWorld Setup(RespawnType _respawnReason, Vector3i _position, int _entityId)
	{
		this.respawnReason = _respawnReason;
		this.position = _position;
		this.entityId = _entityId;
		return this;
	}

	// Token: 0x170005AA RID: 1450
	// (get) Token: 0x060037A5 RID: 14245 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.Both;
		}
	}

	// Token: 0x060037A6 RID: 14246 RVA: 0x0016B87A File Offset: 0x00169A7A
	public override void read(PooledBinaryReader _reader)
	{
		this.respawnReason = (RespawnType)_reader.ReadInt32();
		this.position = StreamUtils.ReadVector3i(_reader);
		this.entityId = _reader.ReadInt32();
	}

	// Token: 0x060037A7 RID: 14247 RVA: 0x0016B8A1 File Offset: 0x00169AA1
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((int)this.respawnReason);
		StreamUtils.Write(_writer, this.position);
		_writer.Write(this.entityId);
	}

	// Token: 0x060037A8 RID: 14248 RVA: 0x0016B8D0 File Offset: 0x00169AD0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.entityId, false))
		{
			return;
		}
		GameManager.Instance.PlayerSpawnedInWorld(base.Sender, this.respawnReason, this.position, this.entityId);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerSpawnedInWorld>().Setup(this.respawnReason, new Vector3i(this.position), this.entityId), false, -1, base.Sender.entityId, -1, null, 192, false);
		}
	}

	// Token: 0x060037A9 RID: 14249 RVA: 0x00163F5F File Offset: 0x0016215F
	public override int GetLength()
	{
		return 16;
	}

	// Token: 0x04002D08 RID: 11528
	[PublicizedFrom(EAccessModifier.Private)]
	public RespawnType respawnReason;

	// Token: 0x04002D09 RID: 11529
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i position;

	// Token: 0x04002D0A RID: 11530
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;
}
