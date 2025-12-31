using System;
using UnityEngine.Scripting;

// Token: 0x020006FE RID: 1790
[Preserve]
public class NetPackageKeyExchangeComplete : NetPackage
{
	// Token: 0x17000550 RID: 1360
	// (get) Token: 0x060034BC RID: 13500 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060034BD RID: 13501 RVA: 0x00161B6B File Offset: 0x0015FD6B
	public NetPackageKeyExchangeComplete Setup(bool _wasSuccessful)
	{
		this.wasSuccessful = _wasSuccessful;
		return this;
	}

	// Token: 0x060034BE RID: 13502 RVA: 0x00161B75 File Offset: 0x0015FD75
	public override void read(PooledBinaryReader _br)
	{
		this.wasSuccessful = _br.ReadBoolean();
	}

	// Token: 0x060034BF RID: 13503 RVA: 0x00161B83 File Offset: 0x0015FD83
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.wasSuccessful);
	}

	// Token: 0x060034C0 RID: 13504 RVA: 0x00161B98 File Offset: 0x0015FD98
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.AntiCheatEncryptionAuthServer.CompleteKeyExchange(base.Sender, this.wasSuccessful);
	}

	// Token: 0x060034C1 RID: 13505 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int GetLength()
	{
		return 1;
	}

	// Token: 0x04002B1D RID: 11037
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasSuccessful;
}
