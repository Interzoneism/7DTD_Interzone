using System;
using UnityEngine.Scripting;

// Token: 0x0200077C RID: 1916
[Preserve]
public class NetPackagePlayerVendingMachine : NetPackage
{
	// Token: 0x060037B8 RID: 14264 RVA: 0x0016BCAD File Offset: 0x00169EAD
	public NetPackagePlayerVendingMachine Setup(PlatformUserIdentifierAbs _userId, Vector3i _position, bool _removing)
	{
		this.userId = _userId;
		this.position = _position;
		this.removing = _removing;
		return this;
	}

	// Token: 0x060037B9 RID: 14265 RVA: 0x0016BCC5 File Offset: 0x00169EC5
	public override void read(PooledBinaryReader _reader)
	{
		this.userId = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
		this.position = new Vector3i(_reader.ReadInt32(), _reader.ReadInt32(), _reader.ReadInt32());
		this.removing = _reader.ReadBoolean();
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x0016BD00 File Offset: 0x00169F00
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		this.userId.ToStream(_writer, false);
		_writer.Write(this.position.x);
		_writer.Write(this.position.y);
		_writer.Write(this.position.z);
		_writer.Write(this.removing);
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x0016BD60 File Offset: 0x00169F60
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		PersistentPlayerData persistentPlayerData2;
		if (this.removing)
		{
			PersistentPlayerData persistentPlayerData;
			if (_callbacks.persistentPlayers.Players.TryGetValue(this.userId, out persistentPlayerData))
			{
				persistentPlayerData.TryRemoveVendingMachinePosition(this.position);
				return;
			}
		}
		else if (_callbacks.persistentPlayers.Players.TryGetValue(this.userId, out persistentPlayerData2))
		{
			persistentPlayerData2.AddVendingMachinePosition(this.position);
		}
	}

	// Token: 0x060037BC RID: 14268 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002D13 RID: 11539
	public PlatformUserIdentifierAbs userId;

	// Token: 0x04002D14 RID: 11540
	public Vector3i position;

	// Token: 0x04002D15 RID: 11541
	public bool removing;
}
