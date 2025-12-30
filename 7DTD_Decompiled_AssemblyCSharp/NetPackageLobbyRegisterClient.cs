using System;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000759 RID: 1881
[Preserve]
public class NetPackageLobbyRegisterClient : NetPackage
{
	// Token: 0x1700058E RID: 1422
	// (get) Token: 0x060036DA RID: 14042 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060036DB RID: 14043 RVA: 0x00168931 File Offset: 0x00166B31
	public NetPackageLobbyRegisterClient Setup(PlatformLobbyId lobbyId, bool overwriteExistingLobby)
	{
		this.lobbyId = lobbyId;
		this.overwriteExistingLobby = overwriteExistingLobby;
		return this;
	}

	// Token: 0x060036DC RID: 14044 RVA: 0x00168942 File Offset: 0x00166B42
	public override void read(PooledBinaryReader _br)
	{
		this.lobbyId = PlatformLobbyId.Read(_br);
		this.overwriteExistingLobby = _br.ReadBoolean();
	}

	// Token: 0x060036DD RID: 14045 RVA: 0x0016895C File Offset: 0x00166B5C
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		this.lobbyId.Write(_bw);
		_bw.Write(this.overwriteExistingLobby);
	}

	// Token: 0x060036DE RID: 14046 RVA: 0x00168980 File Offset: 0x00166B80
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (this.lobbyId.PlatformIdentifier == PlatformManager.NativePlatform.PlatformIdentifier)
		{
			return;
		}
		if (this.lobbyId.PlatformIdentifier != base.Sender.PlatformId.PlatformIdentifier)
		{
			Log.Warning(string.Format("Received {0} for lobby with platform {1} but client is from {2}. This is not permitted, lobby will not be registered", "NetPackageLobbyRegisterClient", this.lobbyId.PlatformIdentifier, base.Sender.PlatformId.PlatformIdentifier));
			return;
		}
		PlatformManager.ClientLobbyManager.RegisterLobbyClient(this.lobbyId, base.Sender, this.overwriteExistingLobby);
	}

	// Token: 0x060036DF RID: 14047 RVA: 0x00168A18 File Offset: 0x00166C18
	public override int GetLength()
	{
		Encoding utf = Encoding.UTF8;
		PlatformLobbyId platformLobbyId = this.lobbyId;
		if (platformLobbyId == null)
		{
			return 0;
		}
		return platformLobbyId.GetWriteLength(utf);
	}

	// Token: 0x04002C7F RID: 11391
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformLobbyId lobbyId;

	// Token: 0x04002C80 RID: 11392
	[PublicizedFrom(EAccessModifier.Private)]
	public bool overwriteExistingLobby;
}
