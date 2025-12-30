using System;
using UnityEngine.Scripting;

// Token: 0x020006FC RID: 1788
[Preserve]
public class NetPackageEncryptionPublicKey : NetPackage
{
	// Token: 0x1700054E RID: 1358
	// (get) Token: 0x060034AE RID: 13486 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060034AF RID: 13487 RVA: 0x0016188B File Offset: 0x0015FA8B
	public NetPackageEncryptionPublicKey Setup(string keyParamsXml, byte[] hash, byte[] signedHash)
	{
		this.ExchangePublicKeyParamsXml = keyParamsXml;
		this.Hash = hash;
		this.SignedHash = signedHash;
		return this;
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x001618A4 File Offset: 0x0015FAA4
	public override void read(PooledBinaryReader _br)
	{
		this.ExchangePublicKeyParamsXml = _br.ReadString();
		this.Hash = new byte[_br.ReadInt32()];
		_br.Read(this.Hash, 0, this.Hash.Length);
		this.SignedHash = new byte[_br.ReadInt32()];
		_br.Read(this.SignedHash, 0, this.SignedHash.Length);
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x0016190C File Offset: 0x0015FB0C
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.ExchangePublicKeyParamsXml);
		_bw.Write(this.Hash.Length);
		_bw.Write(this.Hash);
		_bw.Write(this.SignedHash.Length);
		_bw.Write(this.SignedHash);
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x00161960 File Offset: 0x0015FB60
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.AntiCheatEncryptionAuthServer.SendSharedKey(base.Sender, this.ExchangePublicKeyParamsXml, this.Hash, this.SignedHash);
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x0016198C File Offset: 0x0015FB8C
	public override int GetLength()
	{
		string exchangePublicKeyParamsXml = this.ExchangePublicKeyParamsXml;
		if (exchangePublicKeyParamsXml != null)
		{
			return exchangePublicKeyParamsXml.Length;
		}
		byte[] hash = this.Hash;
		int? num = (hash != null) ? new int?(hash.Length) : null;
		int? num2 = (num != null) ? new int?(num.GetValueOrDefault()) : null;
		if (num2 == null)
		{
			byte[] signedHash = this.SignedHash;
			num = ((signedHash != null) ? new int?(signedHash.Length) : null);
			return ((num != null) ? new int?(num.GetValueOrDefault()) : null).GetValueOrDefault();
		}
		return num2.GetValueOrDefault();
	}

	// Token: 0x04002B18 RID: 11032
	[PublicizedFrom(EAccessModifier.Private)]
	public string ExchangePublicKeyParamsXml;

	// Token: 0x04002B19 RID: 11033
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] Hash;

	// Token: 0x04002B1A RID: 11034
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] SignedHash;
}
