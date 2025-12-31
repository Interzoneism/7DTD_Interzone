using System;
using UnityEngine.Scripting;

// Token: 0x020006D8 RID: 1752
[Preserve]
public class HostMpAllowedAuthorizer : AuthorizerAbs
{
	// Token: 0x1700050D RID: 1293
	// (get) Token: 0x060033C1 RID: 13249 RVA: 0x0015DC92 File Offset: 0x0015BE92
	public override int Order
	{
		get
		{
			return 41;
		}
	}

	// Token: 0x1700050E RID: 1294
	// (get) Token: 0x060033C2 RID: 13250 RVA: 0x0015DC96 File Offset: 0x0015BE96
	public override string AuthorizerName
	{
		get
		{
			return "MpHostAllowed";
		}
	}

	// Token: 0x1700050F RID: 1295
	// (get) Token: 0x060033C3 RID: 13251 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060033C4 RID: 13252 RVA: 0x0015DCA0 File Offset: 0x0015BEA0
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (!PermissionsManager.IsMultiplayerAllowed() || !PermissionsManager.CanHostMultiplayer())
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.MultiplayerBlockedForHostAccount, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
