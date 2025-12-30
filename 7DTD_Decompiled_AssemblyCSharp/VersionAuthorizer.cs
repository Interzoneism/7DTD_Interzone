using System;
using UnityEngine.Scripting;

// Token: 0x020006DB RID: 1755
[Preserve]
public class VersionAuthorizer : AuthorizerAbs
{
	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x060033D0 RID: 13264 RVA: 0x0015DDFE File Offset: 0x0015BFFE
	public override int Order
	{
		get
		{
			return 70;
		}
	}

	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x060033D1 RID: 13265 RVA: 0x0015DE02 File Offset: 0x0015C002
	public override string AuthorizerName
	{
		get
		{
			return "VersionCheck";
		}
	}

	// Token: 0x17000518 RID: 1304
	// (get) Token: 0x060033D2 RID: 13266 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x0015DE0C File Offset: 0x0015C00C
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (!string.Equals(Constants.cVersionInformation.LongStringNoBuild, _clientInfo.compatibilityVersion, StringComparison.Ordinal))
		{
			EAuthorizerSyncResult item = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason = GameUtils.EKickReason.VersionMismatch;
			int apiResponseEnum = 0;
			string longStringNoBuild = Constants.cVersionInformation.LongStringNoBuild;
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), longStringNoBuild)));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
