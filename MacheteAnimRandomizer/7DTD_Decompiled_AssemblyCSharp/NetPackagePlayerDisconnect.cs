using System;
using UnityEngine.Scripting;

// Token: 0x02000770 RID: 1904
[Preserve]
public class NetPackagePlayerDisconnect : NetPackagePlayerData
{
	// Token: 0x170005A0 RID: 1440
	// (get) Token: 0x06003764 RID: 14180 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x0016ABC7 File Offset: 0x00168DC7
	public new NetPackagePlayerDisconnect Setup(EntityPlayer _player)
	{
		base.Setup(_player);
		return this;
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x0016ABD2 File Offset: 0x00168DD2
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		base.ProcessPackage(_world, _callbacks);
		_callbacks.PlayerDisconnected(base.Sender);
	}
}
