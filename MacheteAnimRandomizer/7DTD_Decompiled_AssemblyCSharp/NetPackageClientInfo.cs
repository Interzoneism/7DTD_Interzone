using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200070F RID: 1807
[Preserve]
public class NetPackageClientInfo : NetPackage
{
	// Token: 0x06003524 RID: 13604 RVA: 0x001629F8 File Offset: 0x00160BF8
	public NetPackageClientInfo Setup(WorldBase _world, IList<ClientInfo> _clients)
	{
		this.playerIds.Clear();
		this.pingTimes.Clear();
		this.admins.Clear();
		if (!GameManager.IsDedicatedServer)
		{
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer != null)
			{
				this.addPlayerEntry(primaryPlayer, null);
			}
		}
		for (int i = 0; i < _clients.Count; i++)
		{
			int entityId = _clients[i].entityId;
			if (entityId != -1)
			{
				EntityAlive ea = (EntityAlive)_world.GetEntity(entityId);
				this.addPlayerEntry(ea, _clients[i]);
			}
		}
		return this;
	}

	// Token: 0x06003525 RID: 13605 RVA: 0x00162A8C File Offset: 0x00160C8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void addPlayerEntry(EntityAlive _ea, ClientInfo _clientInfo)
	{
		if (_ea == null)
		{
			return;
		}
		EntityPlayer entityPlayer = _ea as EntityPlayer;
		_ea.pingToServer = ((_clientInfo != null) ? _clientInfo.ping : -1);
		this.playerIds.Add(_ea.entityId);
		this.pingTimes.Add(_ea.pingToServer);
		this.admins.Add(entityPlayer != null && entityPlayer.IsAdmin);
	}

	// Token: 0x06003526 RID: 13606 RVA: 0x00162AFC File Offset: 0x00160CFC
	public override void read(PooledBinaryReader _reader)
	{
		this.playerIds.Clear();
		this.pingTimes.Clear();
		this.admins.Clear();
		int num = (int)_reader.ReadUInt16();
		for (int i = 0; i < num; i++)
		{
			this.playerIds.Add(_reader.ReadInt32());
			this.pingTimes.Add((int)_reader.ReadInt16());
			this.admins.Add(_reader.ReadBoolean());
		}
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x00162B70 File Offset: 0x00160D70
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((ushort)this.playerIds.Count);
		for (int i = 0; i < this.playerIds.Count; i++)
		{
			_writer.Write(this.playerIds[i]);
			_writer.Write((short)this.pingTimes[i]);
			_writer.Write(this.admins[i]);
		}
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x00162BE4 File Offset: 0x00160DE4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		for (int i = 0; i < this.playerIds.Count; i++)
		{
			EntityAlive entityAlive = (EntityAlive)_world.GetEntity(this.playerIds[i]);
			if (entityAlive != null)
			{
				entityAlive.pingToServer = this.pingTimes[i];
				EntityPlayer entityPlayer = entityAlive as EntityPlayer;
				if (entityPlayer != null)
				{
					entityPlayer.IsAdmin = this.admins[i];
				}
			}
		}
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x00162999 File Offset: 0x00160B99
	public override int GetLength()
	{
		return 40;
	}

	// Token: 0x17000561 RID: 1377
	// (get) Token: 0x0600352A RID: 13610 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x04002B53 RID: 11091
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> playerIds = new List<int>();

	// Token: 0x04002B54 RID: 11092
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> pingTimes = new List<int>();

	// Token: 0x04002B55 RID: 11093
	[PublicizedFrom(EAccessModifier.Private)]
	public List<bool> admins = new List<bool>();
}
