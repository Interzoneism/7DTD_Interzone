using System;
using UnityEngine.Scripting;

// Token: 0x020006D6 RID: 1750
[Preserve]
public class PlayerNameAuthorizer : AuthorizerAbs
{
	// Token: 0x17000507 RID: 1287
	// (get) Token: 0x060033B7 RID: 13239 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int Order
	{
		get
		{
			return 20;
		}
	}

	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x060033B8 RID: 13240 RVA: 0x0015DB9C File Offset: 0x0015BD9C
	public override string AuthorizerName
	{
		get
		{
			return "PlayerName";
		}
	}

	// Token: 0x17000509 RID: 1289
	// (get) Token: 0x060033B9 RID: 13241 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x0015DBA4 File Offset: 0x0015BDA4
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (string.IsNullOrEmpty(_clientInfo.playerName))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.EmptyNameOrPlayerID, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
