using System;

namespace Platform
{
	// Token: 0x020017DF RID: 6111
	public sealed class ClientAuthenticateServerContext
	{
		// Token: 0x0600B685 RID: 46725 RVA: 0x004677CA File Offset: 0x004659CA
		public ClientAuthenticateServerContext(GameServerInfo _gameServerInfo, PlatformUserIdentifierAbs _platformUserId, PlatformUserIdentifierAbs _crossplatformUserId, ClientAuthenticateServerSuccessDelegate _success, ClientAuthenticateServerDisconnectDelegate _disconnect)
		{
			this.GameServerInfo = _gameServerInfo;
			this.PlatformUserId = _platformUserId;
			this.CrossplatformUserId = _crossplatformUserId;
			this.success = _success;
			this.disconnect = _disconnect;
		}

		// Token: 0x17001492 RID: 5266
		// (get) Token: 0x0600B686 RID: 46726 RVA: 0x004677F7 File Offset: 0x004659F7
		public GameServerInfo GameServerInfo { get; }

		// Token: 0x17001493 RID: 5267
		// (get) Token: 0x0600B687 RID: 46727 RVA: 0x004677FF File Offset: 0x004659FF
		public PlatformUserIdentifierAbs PlatformUserId { get; }

		// Token: 0x17001494 RID: 5268
		// (get) Token: 0x0600B688 RID: 46728 RVA: 0x00467807 File Offset: 0x00465A07
		public PlatformUserIdentifierAbs CrossplatformUserId { get; }

		// Token: 0x0600B689 RID: 46729 RVA: 0x0046780F File Offset: 0x00465A0F
		public void Success()
		{
			ClientAuthenticateServerSuccessDelegate clientAuthenticateServerSuccessDelegate = this.success;
			if (clientAuthenticateServerSuccessDelegate == null)
			{
				return;
			}
			clientAuthenticateServerSuccessDelegate();
		}

		// Token: 0x0600B68A RID: 46730 RVA: 0x00467824 File Offset: 0x00465A24
		public void DisconnectNoCrossplay()
		{
			string reason = PermissionsManager.GetPermissionDenyReason(EUserPerms.Crossplay, PermissionsManager.PermissionSources.All) ?? Localization.Get("auth_noCrossplay", false);
			ClientAuthenticateServerDisconnectDelegate clientAuthenticateServerDisconnectDelegate = this.disconnect;
			if (clientAuthenticateServerDisconnectDelegate == null)
			{
				return;
			}
			clientAuthenticateServerDisconnectDelegate(reason);
		}

		// Token: 0x0600B68B RID: 46731 RVA: 0x0046785A File Offset: 0x00465A5A
		public void DisconnectNoCrossplay(EPlayGroup otherPlayGroup)
		{
			ClientAuthenticateServerDisconnectDelegate clientAuthenticateServerDisconnectDelegate = this.disconnect;
			if (clientAuthenticateServerDisconnectDelegate == null)
			{
				return;
			}
			clientAuthenticateServerDisconnectDelegate(string.Format(Localization.Get("auth_noCrossplayBetween", false), EPlayGroupExtensions.Current, otherPlayGroup));
		}

		// Token: 0x04008F93 RID: 36755
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ClientAuthenticateServerSuccessDelegate success;

		// Token: 0x04008F94 RID: 36756
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ClientAuthenticateServerDisconnectDelegate disconnect;
	}
}
