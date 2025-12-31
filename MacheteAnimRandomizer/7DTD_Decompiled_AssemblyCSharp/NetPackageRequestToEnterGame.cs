using System;
using UnityEngine.Scripting;

// Token: 0x02000788 RID: 1928
[Preserve]
public class NetPackageRequestToEnterGame : NetPackage
{
	// Token: 0x170005AF RID: 1455
	// (get) Token: 0x06003801 RID: 14337 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003802 RID: 14338 RVA: 0x00002914 File Offset: 0x00000B14
	public override void read(PooledBinaryReader _reader)
	{
	}

	// Token: 0x06003803 RID: 14339 RVA: 0x00161C1F File Offset: 0x0015FE1F
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
	}

	// Token: 0x06003804 RID: 14340 RVA: 0x0016E44B File Offset: 0x0016C64B
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		ThreadManager.StartCoroutine(_callbacks.RequestToEnterGame(base.Sender));
	}

	// Token: 0x06003805 RID: 14341 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}
}
