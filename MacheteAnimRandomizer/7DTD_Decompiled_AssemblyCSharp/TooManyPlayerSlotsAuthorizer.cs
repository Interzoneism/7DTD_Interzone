using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020006DD RID: 1757
[Preserve]
public class TooManyPlayerSlotsAuthorizer : AuthorizerAbs
{
	// Token: 0x1700051C RID: 1308
	// (get) Token: 0x060033DA RID: 13274 RVA: 0x0015E010 File Offset: 0x0015C210
	public override int Order
	{
		get
		{
			return 81;
		}
	}

	// Token: 0x1700051D RID: 1309
	// (get) Token: 0x060033DB RID: 13275 RVA: 0x0015DE6C File Offset: 0x0015C06C
	public override string AuthorizerName
	{
		get
		{
			return "PlayerSlots";
		}
	}

	// Token: 0x1700051E RID: 1310
	// (get) Token: 0x060033DC RID: 13276 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060033DD RID: 13277 RVA: 0x0015E014 File Offset: 0x0015C214
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount);
		bool flag = true;
		EPlayGroup eplayGroup = _clientInfo.device.ToPlayGroup();
		if (eplayGroup == EPlayGroup.XBS || eplayGroup == EPlayGroup.PS5)
		{
			flag = (@int <= 8);
		}
		if (!flag)
		{
			EAuthorizerSyncResult item = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason = GameUtils.EKickReason.PlatformPlayerLimitExceeded;
			int apiResponseEnum = 0;
			string customReason = 8.ToString();
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason)));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
