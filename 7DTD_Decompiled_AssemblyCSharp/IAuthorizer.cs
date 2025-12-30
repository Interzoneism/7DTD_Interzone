using System;
using Platform;

// Token: 0x020006D4 RID: 1748
public interface IAuthorizer
{
	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x060033A0 RID: 13216
	int Order { get; }

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x060033A1 RID: 13217
	string AuthorizerName { get; }

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x060033A2 RID: 13218
	string StateLocalizationKey { get; }

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x060033A3 RID: 13219
	EPlatformIdentifier PlatformRestriction { get; }

	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x060033A4 RID: 13220
	bool AuthorizerActive { get; }

	// Token: 0x060033A5 RID: 13221
	void Init(IAuthorizationResponses _authResponsesHandler);

	// Token: 0x060033A6 RID: 13222
	void Cleanup();

	// Token: 0x060033A7 RID: 13223
	void ServerStart();

	// Token: 0x060033A8 RID: 13224
	void ServerStop();

	// Token: 0x060033A9 RID: 13225
	ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo);

	// Token: 0x060033AA RID: 13226
	void Disconnect(ClientInfo _clientInfo);
}
