using System;
using UnityEngine.Scripting;

// Token: 0x02000730 RID: 1840
[Preserve]
public class NetPackageEntityAddScoreServer : NetPackageEntityAddScoreClient
{
	// Token: 0x17000575 RID: 1397
	// (get) Token: 0x060035DB RID: 13787 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060035DC RID: 13788 RVA: 0x001651F7 File Offset: 0x001633F7
	public new NetPackageEntityAddScoreServer Setup(int _entityId, int _zombieKills, int _playerKills, int _otherTeamNumber, int _conditions)
	{
		base.Setup(_entityId, _zombieKills, _playerKills, _otherTeamNumber, _conditions);
		return this;
	}

	// Token: 0x060035DD RID: 13789 RVA: 0x00165208 File Offset: 0x00163408
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.gameManager.AddScoreServer(this.entityId, this.zombieKills, this.playerKills, this.otherTeamNumber, this.conditions);
	}
}
