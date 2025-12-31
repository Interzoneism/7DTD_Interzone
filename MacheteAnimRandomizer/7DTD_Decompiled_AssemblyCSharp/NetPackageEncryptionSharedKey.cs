using System;
using UnityEngine.Scripting;

// Token: 0x020006FD RID: 1789
[Preserve]
public class NetPackageEncryptionSharedKey : NetPackage
{
	// Token: 0x1700054F RID: 1359
	// (get) Token: 0x060034B5 RID: 13493 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x00161A40 File Offset: 0x0015FC40
	public NetPackageEncryptionSharedKey Setup(byte[] encryptionKey, byte[] integrityKey)
	{
		this.EncryptionKey = encryptionKey;
		this.IntegrityKey = integrityKey;
		return this;
	}

	// Token: 0x060034B7 RID: 13495 RVA: 0x00161A54 File Offset: 0x0015FC54
	public override void read(PooledBinaryReader _br)
	{
		this.EncryptionKey = new byte[_br.ReadInt32()];
		_br.Read(this.EncryptionKey, 0, this.EncryptionKey.Length);
		this.IntegrityKey = new byte[_br.ReadInt32()];
		_br.Read(this.IntegrityKey, 0, this.IntegrityKey.Length);
	}

	// Token: 0x060034B8 RID: 13496 RVA: 0x00161AAF File Offset: 0x0015FCAF
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.EncryptionKey.Length);
		_bw.Write(this.EncryptionKey);
		_bw.Write(this.IntegrityKey.Length);
		_bw.Write(this.IntegrityKey);
	}

	// Token: 0x060034B9 RID: 13497 RVA: 0x00161AEC File Offset: 0x0015FCEC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.AntiCheatEncryptionAuthClient.CompleteKeyExchange(this.EncryptionKey, this.IntegrityKey);
	}

	// Token: 0x060034BA RID: 13498 RVA: 0x00161B0C File Offset: 0x0015FD0C
	public override int GetLength()
	{
		byte[] encryptionKey = this.EncryptionKey;
		if (encryptionKey == null)
		{
			byte[] integrityKey = this.IntegrityKey;
			int? num = (integrityKey != null) ? new int?(integrityKey.Length) : null;
			return ((num != null) ? new int?(num.GetValueOrDefault()) : null).GetValueOrDefault();
		}
		return encryptionKey.Length;
	}

	// Token: 0x04002B1B RID: 11035
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] EncryptionKey;

	// Token: 0x04002B1C RID: 11036
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] IntegrityKey;
}
