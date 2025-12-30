using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x0200077B RID: 1915
[Preserve]
public class NetPackagePlayerTwitchStats : NetPackage
{
	// Token: 0x060037B2 RID: 14258 RVA: 0x0016BAE8 File Offset: 0x00169CE8
	public NetPackagePlayerTwitchStats Setup(EntityAlive _entity)
	{
		this.entityId = _entity.entityId;
		EntityPlayer entityPlayer = _entity as EntityPlayer;
		if (entityPlayer)
		{
			this.twitchEnabled = entityPlayer.TwitchEnabled;
			this.twitchSafe = entityPlayer.TwitchSafe;
			this.twitchVoteLock = entityPlayer.TwitchVoteLock;
			this.twitchVisionDisabled = entityPlayer.TwitchVisionDisabled;
			this.twitchActionsEnabled = entityPlayer.TwitchActionsEnabled;
		}
		return this;
	}

	// Token: 0x060037B3 RID: 14259 RVA: 0x0016BB50 File Offset: 0x00169D50
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.twitchEnabled = _reader.ReadBoolean();
		this.twitchSafe = _reader.ReadBoolean();
		this.twitchVoteLock = (TwitchVoteLockTypes)_reader.ReadByte();
		this.twitchVisionDisabled = _reader.ReadBoolean();
		this.twitchActionsEnabled = (EntityPlayer.TwitchActionsStates)_reader.ReadByte();
	}

	// Token: 0x060037B4 RID: 14260 RVA: 0x0016BBA8 File Offset: 0x00169DA8
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.twitchEnabled);
		_writer.Write(this.twitchSafe);
		_writer.Write((byte)this.twitchVoteLock);
		_writer.Write(this.twitchVisionDisabled);
		_writer.Write((byte)this.twitchActionsEnabled);
	}

	// Token: 0x060037B5 RID: 14261 RVA: 0x0016BC08 File Offset: 0x00169E08
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayer entityPlayer = _world.GetEntity(this.entityId) as EntityPlayer;
		if (entityPlayer)
		{
			entityPlayer.TwitchEnabled = this.twitchEnabled;
			entityPlayer.TwitchSafe = this.twitchSafe;
			entityPlayer.TwitchVoteLock = this.twitchVoteLock;
			entityPlayer.TwitchVisionDisabled = this.twitchVisionDisabled;
			entityPlayer.TwitchActionsEnabled = this.twitchActionsEnabled;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerTwitchStats>().Setup(entityPlayer), false, -1, base.Sender.entityId, -1, null, 192, false);
		}
	}

	// Token: 0x060037B6 RID: 14262 RVA: 0x0015DD3D File Offset: 0x0015BF3D
	public override int GetLength()
	{
		return 60;
	}

	// Token: 0x04002D0D RID: 11533
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002D0E RID: 11534
	[PublicizedFrom(EAccessModifier.Private)]
	public bool twitchEnabled;

	// Token: 0x04002D0F RID: 11535
	[PublicizedFrom(EAccessModifier.Private)]
	public bool twitchSafe;

	// Token: 0x04002D10 RID: 11536
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchVoteLockTypes twitchVoteLock;

	// Token: 0x04002D11 RID: 11537
	[PublicizedFrom(EAccessModifier.Private)]
	public bool twitchVisionDisabled;

	// Token: 0x04002D12 RID: 11538
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer.TwitchActionsStates twitchActionsEnabled;
}
