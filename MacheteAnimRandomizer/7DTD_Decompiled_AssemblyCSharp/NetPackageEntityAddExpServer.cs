using System;
using UnityEngine.Scripting;

// Token: 0x0200072D RID: 1837
[Preserve]
public class NetPackageEntityAddExpServer : NetPackageEntityAddExpClient
{
	// Token: 0x17000572 RID: 1394
	// (get) Token: 0x060035CC RID: 13772 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060035CD RID: 13773 RVA: 0x00165024 File Offset: 0x00163224
	public NetPackageEntityAddExpServer Setup(int _entityId, int _experience)
	{
		base.Setup(_entityId, _experience, Progression.XPTypes.Other);
		return this;
	}

	// Token: 0x060035CE RID: 13774 RVA: 0x00165034 File Offset: 0x00163234
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayer entityPlayer = (EntityPlayer)_world.GetEntity(this.entityId);
		if (entityPlayer == null)
		{
			return;
		}
		if (entityPlayer.isEntityRemote)
		{
			entityPlayer.Progression.AddLevelExp(this.xp, "_xpOther", Progression.XPTypes.Other, true, true);
		}
	}
}
