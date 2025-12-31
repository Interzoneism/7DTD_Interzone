using System;

// Token: 0x020006BE RID: 1726
public interface IAuthorizationResponses
{
	// Token: 0x060032B2 RID: 12978
	void AuthorizationDenied(IAuthorizer _authorizer, ClientInfo _clientInfo, GameUtils.KickPlayerData _kickPlayerData);

	// Token: 0x060032B3 RID: 12979
	void AuthorizationAccepted(IAuthorizer _authorizer, ClientInfo _clientInfo);
}
