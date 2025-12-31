using System;
using UnityEngine.Scripting;

// Token: 0x020006FB RID: 1787
[Preserve]
public class NetPackageEncryptionRequest : NetPackage
{
	// Token: 0x1700054D RID: 1357
	// (get) Token: 0x060034A8 RID: 13480 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060034A9 RID: 13481 RVA: 0x00112051 File Offset: 0x00110251
	public NetPackageEncryptionRequest Setup()
	{
		return this;
	}

	// Token: 0x060034AA RID: 13482 RVA: 0x00002914 File Offset: 0x00000B14
	public override void read(PooledBinaryReader _reader)
	{
	}

	// Token: 0x060034AB RID: 13483 RVA: 0x0016187A File Offset: 0x0015FA7A
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.AntiCheatEncryptionAuthClient.StartKeyExchange();
	}

	// Token: 0x060034AC RID: 13484 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}
}
