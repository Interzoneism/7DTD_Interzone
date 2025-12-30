using System;

namespace Platform
{
	// Token: 0x020017E2 RID: 6114
	public interface IAuthenticationServer
	{
		// Token: 0x0600B694 RID: 46740
		void Init(IPlatform _owner);

		// Token: 0x0600B695 RID: 46741
		EBeginUserAuthenticationResult AuthenticateUser(ClientInfo _cInfo);

		// Token: 0x0600B696 RID: 46742
		void RemoveUser(ClientInfo _cInfo);

		// Token: 0x0600B697 RID: 46743
		void StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate);

		// Token: 0x0600B698 RID: 46744
		void StartServerSteamGroups(SteamGroupStatusResponse _groupStatusResponseDelegate);

		// Token: 0x0600B699 RID: 46745
		void StopServer();

		// Token: 0x0600B69A RID: 46746
		bool RequestUserInGroupStatus(ClientInfo _cInfo, string _steamIdGroup);
	}
}
