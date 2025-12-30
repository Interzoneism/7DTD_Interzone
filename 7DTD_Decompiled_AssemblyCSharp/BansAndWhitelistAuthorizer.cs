using System;
using UnityEngine.Scripting;

// Token: 0x020006E2 RID: 1762
[Preserve]
public class BansAndWhitelistAuthorizer : AuthorizerAbs
{
	// Token: 0x1700052D RID: 1325
	// (get) Token: 0x060033FF RID: 13311 RVA: 0x0015E645 File Offset: 0x0015C845
	public override int Order
	{
		get
		{
			return 500;
		}
	}

	// Token: 0x1700052E RID: 1326
	// (get) Token: 0x06003400 RID: 13312 RVA: 0x0015E64C File Offset: 0x0015C84C
	public override string AuthorizerName
	{
		get
		{
			return "BansAndWhitelist";
		}
	}

	// Token: 0x1700052F RID: 1327
	// (get) Token: 0x06003401 RID: 13313 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000530 RID: 1328
	// (get) Token: 0x06003402 RID: 13314 RVA: 0x0015E653 File Offset: 0x0015C853
	public override bool AuthorizerActive
	{
		get
		{
			return GameManager.Instance.adminTools != null;
		}
	}

	// Token: 0x06003403 RID: 13315 RVA: 0x0015E664 File Offset: 0x0015C864
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		AdminTools adminTools = GameManager.Instance.adminTools;
		DateTime banUntil;
		string customReason;
		if (adminTools.Blacklist.IsBanned(_clientInfo.PlatformId, out banUntil, out customReason))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.Banned, 0, banUntil, customReason)));
		}
		DateTime banUntil2;
		string customReason2;
		if (_clientInfo.CrossplatformId != null && adminTools.Blacklist.IsBanned(_clientInfo.CrossplatformId, out banUntil2, out customReason2))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.Banned, 0, banUntil2, customReason2)));
		}
		if (adminTools.Whitelist.IsWhiteListEnabled() && !adminTools.Whitelist.IsWhitelisted(_clientInfo) && !adminTools.Users.HasEntry(_clientInfo))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.NotOnWhitelist, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
