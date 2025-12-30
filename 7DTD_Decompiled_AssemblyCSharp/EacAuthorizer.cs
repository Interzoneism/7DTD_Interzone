using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020006E4 RID: 1764
[Preserve]
public class EacAuthorizer : AuthorizerAbs
{
	// Token: 0x17000534 RID: 1332
	// (get) Token: 0x0600340A RID: 13322 RVA: 0x0015E7D0 File Offset: 0x0015C9D0
	public override int Order
	{
		get
		{
			return 600;
		}
	}

	// Token: 0x17000535 RID: 1333
	// (get) Token: 0x0600340B RID: 13323 RVA: 0x0015E7D7 File Offset: 0x0015C9D7
	public override string AuthorizerName
	{
		get
		{
			return "EAC";
		}
	}

	// Token: 0x17000536 RID: 1334
	// (get) Token: 0x0600340C RID: 13324 RVA: 0x0015E7DE File Offset: 0x0015C9DE
	public override string StateLocalizationKey
	{
		get
		{
			return "authstate_eac";
		}
	}

	// Token: 0x17000537 RID: 1335
	// (get) Token: 0x0600340D RID: 13325 RVA: 0x0015E7E5 File Offset: 0x0015C9E5
	public override bool AuthorizerActive
	{
		get
		{
			IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
			return antiCheatServer != null && antiCheatServer.ServerEacEnabled();
		}
	}

	// Token: 0x0600340E RID: 13326 RVA: 0x0015E7FC File Offset: 0x0015C9FC
	public override void ServerStart()
	{
		base.ServerStart();
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer == null)
		{
			return;
		}
		antiCheatServer.StartServer(new AuthenticationSuccessfulCallbackDelegate(this.authPlayerEacSuccessfulCallback), new KickPlayerDelegate(this.kickPlayerCallback));
	}

	// Token: 0x0600340F RID: 13327 RVA: 0x0015E831 File Offset: 0x0015CA31
	public override void ServerStop()
	{
		base.ServerStop();
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer == null)
		{
			return;
		}
		antiCheatServer.StopServer();
	}

	// Token: 0x06003410 RID: 13328 RVA: 0x0015E850 File Offset: 0x0015CA50
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer != null)
		{
			antiCheatServer.RegisterUser(_clientInfo);
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
	}

	// Token: 0x06003411 RID: 13329 RVA: 0x0015E883 File Offset: 0x0015CA83
	[PublicizedFrom(EAccessModifier.Private)]
	public void authPlayerEacSuccessfulCallback(ClientInfo _cInfo)
	{
		_cInfo.acAuthDone = true;
		this.authResponsesHandler.AuthorizationAccepted(this, _cInfo);
	}

	// Token: 0x06003412 RID: 13330 RVA: 0x0015E2D7 File Offset: 0x0015C4D7
	[PublicizedFrom(EAccessModifier.Private)]
	public void kickPlayerCallback(ClientInfo _cInfo, GameUtils.KickPlayerData _kickData)
	{
		this.authResponsesHandler.AuthorizationDenied(this, _cInfo, _kickData);
	}

	// Token: 0x06003413 RID: 13331 RVA: 0x0015E899 File Offset: 0x0015CA99
	public override void Disconnect(ClientInfo _clientInfo)
	{
		IAntiCheatServer antiCheatServer = PlatformManager.MultiPlatform.AntiCheatServer;
		if (antiCheatServer == null)
		{
			return;
		}
		antiCheatServer.FreeUser(_clientInfo);
	}
}
