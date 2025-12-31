using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020006E0 RID: 1760
[Preserve]
public class FriendsAuthorizer : AuthorizerAbs
{
	// Token: 0x17000525 RID: 1317
	// (get) Token: 0x060033EE RID: 13294 RVA: 0x0015E325 File Offset: 0x0015C525
	public override int Order
	{
		get
		{
			return 450;
		}
	}

	// Token: 0x17000526 RID: 1318
	// (get) Token: 0x060033EF RID: 13295 RVA: 0x0015E32C File Offset: 0x0015C52C
	public override string AuthorizerName
	{
		get
		{
			return "Friends";
		}
	}

	// Token: 0x17000527 RID: 1319
	// (get) Token: 0x060033F0 RID: 13296 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000528 RID: 1320
	// (get) Token: 0x060033F1 RID: 13297 RVA: 0x0015E333 File Offset: 0x0015C533
	public override bool AuthorizerActive
	{
		get
		{
			return !GameManager.IsDedicatedServer && GamePrefs.GetInt(EnumGamePrefs.ServerVisibility) == 1;
		}
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x0015E34C File Offset: 0x0015C54C
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (PlatformManager.NativePlatform.User.IsFriend(_clientInfo.PlatformId))
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		DiscordManager.DiscordUser user;
		if (_clientInfo.DiscordUserId > 0UL && (user = DiscordManager.Instance.GetUser(_clientInfo.DiscordUserId)) != null && user.IsFriend)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.FriendsOnly, 0, default(DateTime), "")));
	}
}
