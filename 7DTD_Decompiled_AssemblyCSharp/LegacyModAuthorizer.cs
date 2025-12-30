using System;
using UnityEngine.Scripting;

// Token: 0x020006DE RID: 1758
[Preserve]
public class LegacyModAuthorizer : AuthorizerAbs
{
	// Token: 0x1700051F RID: 1311
	// (get) Token: 0x060033DF RID: 13279 RVA: 0x0015E084 File Offset: 0x0015C284
	public override int Order
	{
		get
		{
			return 150;
		}
	}

	// Token: 0x17000520 RID: 1312
	// (get) Token: 0x060033E0 RID: 13280 RVA: 0x0015E08B File Offset: 0x0015C28B
	public override string AuthorizerName
	{
		get
		{
			return "LegacyModAuthorizations";
		}
	}

	// Token: 0x17000521 RID: 1313
	// (get) Token: 0x060033E1 RID: 13281 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x060033E2 RID: 13282 RVA: 0x0015E094 File Offset: 0x0015C294
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		ModEvents.SPlayerLoginData splayerLoginData = new ModEvents.SPlayerLoginData(_clientInfo, _clientInfo.compatibilityVersion);
		ValueTuple<ModEvents.EModEventResult, Mod> valueTuple = ModEvents.PlayerLogin.Invoke(ref splayerLoginData);
		ModEvents.EModEventResult item = valueTuple.Item1;
		Mod item2 = valueTuple.Item2;
		if (item == ModEvents.EModEventResult.StopHandlersAndVanilla)
		{
			Log.Out("Denying login from mod: " + item2.Name);
			EAuthorizerSyncResult item3 = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason = GameUtils.EKickReason.ModDecision;
			int apiResponseEnum = 0;
			string customMessage = splayerLoginData.CustomMessage;
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item3, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customMessage)));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
