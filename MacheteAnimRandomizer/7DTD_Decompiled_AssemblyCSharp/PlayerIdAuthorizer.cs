using System;
using UnityEngine.Scripting;

// Token: 0x020006D9 RID: 1753
[Preserve]
public class PlayerIdAuthorizer : AuthorizerAbs
{
	// Token: 0x17000510 RID: 1296
	// (get) Token: 0x060033C6 RID: 13254 RVA: 0x0015DCEC File Offset: 0x0015BEEC
	public override int Order
	{
		get
		{
			return 50;
		}
	}

	// Token: 0x17000511 RID: 1297
	// (get) Token: 0x060033C7 RID: 13255 RVA: 0x0015DCF0 File Offset: 0x0015BEF0
	public override string AuthorizerName
	{
		get
		{
			return "PlayerId";
		}
	}

	// Token: 0x17000512 RID: 1298
	// (get) Token: 0x060033C8 RID: 13256 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060033C9 RID: 13257 RVA: 0x0015DCF8 File Offset: 0x0015BEF8
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (_clientInfo.PlatformId == null)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.EmptyNameOrPlayerID, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
