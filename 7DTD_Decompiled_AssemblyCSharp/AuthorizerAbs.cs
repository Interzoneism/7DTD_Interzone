using System;
using Platform;

// Token: 0x020006D5 RID: 1749
public abstract class AuthorizerAbs : IAuthorizer
{
	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x060033AB RID: 13227
	public abstract int Order { get; }

	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x060033AC RID: 13228
	public abstract string AuthorizerName { get; }

	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x060033AD RID: 13229
	public abstract string StateLocalizationKey { get; }

	// Token: 0x17000505 RID: 1285
	// (get) Token: 0x060033AE RID: 13230 RVA: 0x000768E0 File Offset: 0x00074AE0
	public virtual EPlatformIdentifier PlatformRestriction
	{
		get
		{
			return EPlatformIdentifier.Count;
		}
	}

	// Token: 0x17000506 RID: 1286
	// (get) Token: 0x060033AF RID: 13231 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool AuthorizerActive
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x0015DB93 File Offset: 0x0015BD93
	public virtual void Init(IAuthorizationResponses _authResponsesHandler)
	{
		this.authResponsesHandler = _authResponsesHandler;
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Cleanup()
	{
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ServerStart()
	{
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ServerStop()
	{
	}

	// Token: 0x060033B4 RID: 13236
	public abstract ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo);

	// Token: 0x060033B5 RID: 13237 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Disconnect(ClientInfo _clientInfo)
	{
	}

	// Token: 0x060033B6 RID: 13238 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public AuthorizerAbs()
	{
	}

	// Token: 0x04002AB0 RID: 10928
	[PublicizedFrom(EAccessModifier.Protected)]
	public IAuthorizationResponses authResponsesHandler;
}
