using System;
using System.Collections.Generic;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020006E1 RID: 1761
[Preserve]
public class CrossplatformAuthorizer : AuthorizerAbs
{
	// Token: 0x17000529 RID: 1321
	// (get) Token: 0x060033F4 RID: 13300 RVA: 0x0015E3D7 File Offset: 0x0015C5D7
	public override int Order
	{
		get
		{
			return 490;
		}
	}

	// Token: 0x1700052A RID: 1322
	// (get) Token: 0x060033F5 RID: 13301 RVA: 0x0015E3DE File Offset: 0x0015C5DE
	public override string AuthorizerName
	{
		get
		{
			return "CrossplatformAuth";
		}
	}

	// Token: 0x1700052B RID: 1323
	// (get) Token: 0x060033F6 RID: 13302 RVA: 0x0015E3E5 File Offset: 0x0015C5E5
	public override string StateLocalizationKey
	{
		get
		{
			return "authstate_crossplatform";
		}
	}

	// Token: 0x1700052C RID: 1324
	// (get) Token: 0x060033F7 RID: 13303 RVA: 0x0015E3EC File Offset: 0x0015C5EC
	public override bool AuthorizerActive
	{
		get
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			return ((crossplatformPlatform != null) ? crossplatformPlatform.AuthenticationServer : null) != null;
		}
	}

	// Token: 0x060033F8 RID: 13304 RVA: 0x0015E404 File Offset: 0x0015C604
	public override void ServerStart()
	{
		base.ServerStart();
		foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.ServerPlatforms)
		{
			if (keyValuePair.Value.IsCrossplatform)
			{
				IAuthenticationServer authenticationServer = keyValuePair.Value.AuthenticationServer;
				if (authenticationServer != null)
				{
					authenticationServer.StartServer(new AuthenticationSuccessfulCallbackDelegate(this.authPlayerSteamSuccessfulCallback), new KickPlayerDelegate(this.kickPlayerCallback));
				}
			}
		}
	}

	// Token: 0x060033F9 RID: 13305 RVA: 0x0015E48C File Offset: 0x0015C68C
	public override void ServerStop()
	{
		base.ServerStop();
		foreach (KeyValuePair<EPlatformIdentifier, IPlatform> keyValuePair in PlatformManager.ServerPlatforms)
		{
			if (keyValuePair.Value.IsCrossplatform)
			{
				IAuthenticationServer authenticationServer = keyValuePair.Value.AuthenticationServer;
				if (authenticationServer != null)
				{
					authenticationServer.StopServer();
				}
			}
		}
	}

	// Token: 0x060033FA RID: 13306 RVA: 0x0015E4FC File Offset: 0x0015C6FC
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		if (_clientInfo.CrossplatformId == null)
		{
			EAuthorizerSyncResult item = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason = GameUtils.EKickReason.WrongCrossPlatform;
			int apiResponseEnum = 0;
			string customReason = EPlatformIdentifier.None.ToStringCached<EPlatformIdentifier>();
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason)));
		}
		EPlatformIdentifier platformIdentifier = _clientInfo.CrossplatformId.PlatformIdentifier;
		IPlatform platform = PlatformManager.InstanceForPlatformIdentifier(platformIdentifier);
		if (platform == null)
		{
			EAuthorizerSyncResult item2 = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason2 = GameUtils.EKickReason.UnsupportedPlatform;
			int apiResponseEnum2 = 0;
			string customReason = platformIdentifier.ToStringCached<EPlatformIdentifier>();
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item2, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason2, apiResponseEnum2, default(DateTime), customReason)));
		}
		if (platform.PlatformIdentifier != PlatformManager.CrossplatformPlatform.PlatformIdentifier)
		{
			EAuthorizerSyncResult item3 = EAuthorizerSyncResult.SyncDeny;
			GameUtils.EKickReason kickReason3 = GameUtils.EKickReason.WrongCrossPlatform;
			int apiResponseEnum3 = 0;
			string customReason = platformIdentifier.ToStringCached<EPlatformIdentifier>();
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(item3, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(kickReason3, apiResponseEnum3, default(DateTime), customReason)));
		}
		if (platform.AuthenticationServer == null)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncAllow, null);
		}
		EBeginUserAuthenticationResult ebeginUserAuthenticationResult = platform.AuthenticationServer.AuthenticateUser(_clientInfo);
		if (ebeginUserAuthenticationResult != EBeginUserAuthenticationResult.Ok)
		{
			return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.SyncDeny, new GameUtils.KickPlayerData?(new GameUtils.KickPlayerData(GameUtils.EKickReason.CrossPlatformAuthenticationBeginFailed, (int)ebeginUserAuthenticationResult, default(DateTime), "")));
		}
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
	}

	// Token: 0x060033FB RID: 13307 RVA: 0x0015E2C8 File Offset: 0x0015C4C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void authPlayerSteamSuccessfulCallback(ClientInfo _clientInfo)
	{
		this.authResponsesHandler.AuthorizationAccepted(this, _clientInfo);
	}

	// Token: 0x060033FC RID: 13308 RVA: 0x0015E2D7 File Offset: 0x0015C4D7
	[PublicizedFrom(EAccessModifier.Private)]
	public void kickPlayerCallback(ClientInfo _cInfo, GameUtils.KickPlayerData _kickData)
	{
		this.authResponsesHandler.AuthorizationDenied(this, _cInfo, _kickData);
	}

	// Token: 0x060033FD RID: 13309 RVA: 0x0015E607 File Offset: 0x0015C807
	public override void Disconnect(ClientInfo _clientInfo)
	{
		if (_clientInfo != null)
		{
			PlatformUserIdentifierAbs crossplatformId = _clientInfo.CrossplatformId;
			if (((crossplatformId != null) ? crossplatformId.ReadablePlatformUserIdentifier : null) != null)
			{
				IPlatform platform = PlatformManager.InstanceForPlatformIdentifier(_clientInfo.CrossplatformId.PlatformIdentifier);
				if (platform == null)
				{
					return;
				}
				IAuthenticationServer authenticationServer = platform.AuthenticationServer;
				if (authenticationServer == null)
				{
					return;
				}
				authenticationServer.RemoveUser(_clientInfo);
			}
		}
	}
}
