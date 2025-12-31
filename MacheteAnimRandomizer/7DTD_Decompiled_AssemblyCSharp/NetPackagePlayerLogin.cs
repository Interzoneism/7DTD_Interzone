using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

// Token: 0x02000775 RID: 1909
[Preserve]
public class NetPackagePlayerLogin : NetPackage
{
	// Token: 0x170005A4 RID: 1444
	// (get) Token: 0x06003785 RID: 14213 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003786 RID: 14214 RVA: 0x0016B29E File Offset: 0x0016949E
	public NetPackagePlayerLogin Setup(string _playerName, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _platformUserAndToken, [TupleElementNames(new string[]
	{
		"userId",
		"token"
	})] ValueTuple<PlatformUserIdentifierAbs, string> _crossplatformUserAndToken, string _version, string _compVersion, ulong _discordUserId)
	{
		this.playerName = _playerName;
		this.platformUserAndToken = _platformUserAndToken;
		this.crossplatformUserAndToken = _crossplatformUserAndToken;
		this.version = _version;
		this.compVersion = _compVersion;
		this.discordUserId = _discordUserId;
		return this;
	}

	// Token: 0x06003787 RID: 14215 RVA: 0x0016B2D0 File Offset: 0x001694D0
	public override void read(PooledBinaryReader _br)
	{
		Log.Out("NPPL.Read");
		this.playerName = _br.ReadString();
		this.platformUserAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(PlatformUserIdentifierAbs.FromStream(_br, false, true), _br.ReadString());
		this.crossplatformUserAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(PlatformUserIdentifierAbs.FromStream(_br, false, true), _br.ReadString());
		this.version = _br.ReadString();
		this.compVersion = _br.ReadString();
		this.discordUserId = _br.ReadUInt64();
	}

	// Token: 0x06003788 RID: 14216 RVA: 0x0016B34C File Offset: 0x0016954C
	public override void write(PooledBinaryWriter _bw)
	{
		Log.Out("NPPL.Write");
		base.write(_bw);
		_bw.Write(this.playerName);
		this.platformUserAndToken.Item1.ToStream(_bw, true);
		_bw.Write(this.platformUserAndToken.Item2 ?? "");
		this.crossplatformUserAndToken.Item1.ToStream(_bw, true);
		_bw.Write(this.crossplatformUserAndToken.Item2 ?? "");
		_bw.Write(this.version);
		_bw.Write(this.compVersion);
		_bw.Write(this.discordUserId);
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x0016B3F2 File Offset: 0x001695F2
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		_callbacks.PlayerLoginRPC(base.Sender, this.playerName, this.platformUserAndToken, this.crossplatformUserAndToken, this.compVersion, this.discordUserId);
	}

	// Token: 0x170005A5 RID: 1445
	// (get) Token: 0x0600378A RID: 14218 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x0600378B RID: 14219 RVA: 0x0016B41E File Offset: 0x0016961E
	public override int GetLength()
	{
		return 120;
	}

	// Token: 0x04002CF8 RID: 11512
	[PublicizedFrom(EAccessModifier.Private)]
	public string playerName;

	// Token: 0x04002CF9 RID: 11513
	[TupleElementNames(new string[]
	{
		"userId",
		"token"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public ValueTuple<PlatformUserIdentifierAbs, string> platformUserAndToken;

	// Token: 0x04002CFA RID: 11514
	[TupleElementNames(new string[]
	{
		"userId",
		"token"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public ValueTuple<PlatformUserIdentifierAbs, string> crossplatformUserAndToken;

	// Token: 0x04002CFB RID: 11515
	[PublicizedFrom(EAccessModifier.Private)]
	public string version;

	// Token: 0x04002CFC RID: 11516
	[PublicizedFrom(EAccessModifier.Private)]
	public string compVersion;

	// Token: 0x04002CFD RID: 11517
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong discordUserId;
}
