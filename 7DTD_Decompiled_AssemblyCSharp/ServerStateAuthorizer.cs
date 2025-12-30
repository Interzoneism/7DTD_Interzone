using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020006D7 RID: 1751
[Preserve]
public class ServerStateAuthorizer : AuthorizerAbs
{
	// Token: 0x1700050A RID: 1290
	// (get) Token: 0x060033BC RID: 13244 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int Order
	{
		get
		{
			return 30;
		}
	}

	// Token: 0x1700050B RID: 1291
	// (get) Token: 0x060033BD RID: 13245 RVA: 0x0015DBF6 File Offset: 0x0015BDF6
	public override string AuthorizerName
	{
		get
		{
			return "ServerState";
		}
	}

	// Token: 0x1700050C RID: 1292
	// (get) Token: 0x060033BE RID: 13246 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060033BF RID: 13247 RVA: 0x0015DC00 File Offset: 0x0015BE00
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		IMasterServerAnnouncer serverListAnnouncer = PlatformManager.MultiPlatform.ServerListAnnouncer;
		if ((serverListAnnouncer != null && !serverListAnnouncer.GameServerInitialized) || !GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.GameStillLoading, 0, default(DateTime), "")));
		}
		if (GameStats.GetInt(EnumGameStats.GameState) == 2)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.GamePaused, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
