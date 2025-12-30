using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020006E3 RID: 1763
[Preserve]
public class CrossplayAuthorizer : AuthorizerAbs
{
	// Token: 0x17000531 RID: 1329
	// (get) Token: 0x06003405 RID: 13317 RVA: 0x0015E738 File Offset: 0x0015C938
	public override int Order
	{
		get
		{
			return 550;
		}
	}

	// Token: 0x17000532 RID: 1330
	// (get) Token: 0x06003406 RID: 13318 RVA: 0x0015E73F File Offset: 0x0015C93F
	public override string AuthorizerName
	{
		get
		{
			return "Crossplay";
		}
	}

	// Token: 0x17000533 RID: 1331
	// (get) Token: 0x06003407 RID: 13319 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06003408 RID: 13320 RVA: 0x0015E748 File Offset: 0x0015C948
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.ServerAllowCrossplay);
		IUserClient user = PlatformManager.MultiPlatform.User;
		bool flag = user == null || user.Permissions.HasCrossplay();
		if (@bool && flag)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		if (_clientInfo.device.ToPlayGroup() != DeviceFlag.StandaloneWindows.ToPlayGroup())
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.CrossplayDisabled, 0, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
	}
}
