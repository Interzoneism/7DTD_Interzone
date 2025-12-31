using System;
using UnityEngine.Scripting;

// Token: 0x02000700 RID: 1792
[Preserve]
public class NetPackageAuthConfirmation : NetPackage
{
	// Token: 0x17000552 RID: 1362
	// (get) Token: 0x060034CA RID: 13514 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool FlushQueue
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000553 RID: 1363
	// (get) Token: 0x060034CB RID: 13515 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060034CC RID: 13516 RVA: 0x00112051 File Offset: 0x00110251
	public NetPackageAuthConfirmation Setup()
	{
		return this;
	}

	// Token: 0x060034CD RID: 13517 RVA: 0x00002914 File Offset: 0x00000B14
	public override void read(PooledBinaryReader _reader)
	{
	}

	// Token: 0x060034CE RID: 13518 RVA: 0x00161C1F File Offset: 0x0015FE1F
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
	}

	// Token: 0x060034CF RID: 13519 RVA: 0x00161C28 File Offset: 0x0015FE28
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			AuthFinalizer.Instance.ReplyReceived(base.Sender);
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageAuthConfirmation>().Setup(), false);
		}
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x0011934C File Offset: 0x0011754C
	public override int GetLength()
	{
		return 9;
	}
}
