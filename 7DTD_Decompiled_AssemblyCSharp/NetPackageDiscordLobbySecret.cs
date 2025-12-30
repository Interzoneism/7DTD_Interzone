using System;
using UnityEngine.Scripting;

// Token: 0x020002EE RID: 750
[Preserve]
public class NetPackageDiscordLobbySecret : NetPackage
{
	// Token: 0x0600155F RID: 5471 RVA: 0x0007E476 File Offset: 0x0007C676
	public NetPackageDiscordLobbySecret Setup(DiscordManager.ELobbyType _lobbyType, string _lobbySecret)
	{
		this.lobbyType = _lobbyType;
		this.lobbySecret = _lobbySecret;
		return this;
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001560 RID: 5472 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x0007E487 File Offset: 0x0007C687
	public override void read(PooledBinaryReader _reader)
	{
		this.lobbyType = (DiscordManager.ELobbyType)_reader.ReadByte();
		this.lobbySecret = StreamUtils.ReadString(_reader);
	}

	// Token: 0x06001562 RID: 5474 RVA: 0x0007E4A1 File Offset: 0x0007C6A1
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((byte)this.lobbyType);
		StreamUtils.Write(_writer, this.lobbySecret);
	}

	// Token: 0x06001563 RID: 5475 RVA: 0x0007E4C2 File Offset: 0x0007C6C2
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		DiscordManager.Instance.ReceivedLobbySecret(this.lobbyType, this.lobbySecret);
	}

	// Token: 0x06001564 RID: 5476 RVA: 0x0007E4DA File Offset: 0x0007C6DA
	public override int GetLength()
	{
		int num = 3;
		string text = this.lobbySecret;
		return num + ((text != null) ? text.Length : 0);
	}

	// Token: 0x04000DB1 RID: 3505
	[PublicizedFrom(EAccessModifier.Private)]
	public DiscordManager.ELobbyType lobbyType;

	// Token: 0x04000DB2 RID: 3506
	[PublicizedFrom(EAccessModifier.Private)]
	public string lobbySecret;
}
