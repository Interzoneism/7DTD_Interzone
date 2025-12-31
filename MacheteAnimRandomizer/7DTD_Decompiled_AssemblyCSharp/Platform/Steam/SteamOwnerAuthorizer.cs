using System;
using UnityEngine.Scripting;

namespace Platform.Steam
{
	// Token: 0x020018B2 RID: 6322
	[Preserve]
	public class SteamOwnerAuthorizer : AuthorizerAbs
	{
		// Token: 0x1700153E RID: 5438
		// (get) Token: 0x0600BAA1 RID: 47777 RVA: 0x00472011 File Offset: 0x00470211
		public override int Order
		{
			get
			{
				return 430;
			}
		}

		// Token: 0x1700153F RID: 5439
		// (get) Token: 0x0600BAA2 RID: 47778 RVA: 0x00472018 File Offset: 0x00470218
		public override string AuthorizerName
		{
			get
			{
				return "SteamFamily";
			}
		}

		// Token: 0x17001540 RID: 5440
		// (get) Token: 0x0600BAA3 RID: 47779 RVA: 0x00019766 File Offset: 0x00017966
		public override string StateLocalizationKey
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001541 RID: 5441
		// (get) Token: 0x0600BAA4 RID: 47780 RVA: 0x00075C39 File Offset: 0x00073E39
		public override EPlatformIdentifier PlatformRestriction
		{
			get
			{
				return EPlatformIdentifier.Steam;
			}
		}

		// Token: 0x0600BAA5 RID: 47781 RVA: 0x00472020 File Offset: 0x00470220
		public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
		{
			UserIdentifierSteam userIdentifierSteam = (UserIdentifierSteam)_clientInfo.PlatformId;
			UserIdentifierSteam ownerId = userIdentifierSteam.OwnerId;
			DateTime banUntil;
			string customReason;
			if (GameManager.Instance.adminTools != null && ownerId != null && !userIdentifierSteam.Equals(ownerId) && GameManager.Instance.adminTools.Blacklist.IsBanned(ownerId, out banUntil, out customReason))
			{
				return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.Banned, 0, banUntil, customReason)));
			}
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
	}
}
