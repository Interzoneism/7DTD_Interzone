using System;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Sanctions;

namespace Platform.EOS
{
	// Token: 0x0200190D RID: 6413
	public class AuthServer : IAuthenticationServer
	{
		// Token: 0x170015B9 RID: 5561
		// (get) Token: 0x0600BD75 RID: 48501 RVA: 0x0047C8B5 File Offset: 0x0047AAB5
		public ConnectInterface connectInterface
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return ((Api)this.owner.Api).ConnectInterface;
			}
		}

		// Token: 0x170015BA RID: 5562
		// (get) Token: 0x0600BD76 RID: 48502 RVA: 0x0047C8CC File Offset: 0x0047AACC
		public SanctionsInterface sanctionsInterface
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return ((Api)this.owner.Api).SanctionsInterface;
			}
		}

		// Token: 0x0600BD77 RID: 48503 RVA: 0x0047C8E3 File Offset: 0x0047AAE3
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600BD78 RID: 48504 RVA: 0x0047C8EC File Offset: 0x0047AAEC
		public EBeginUserAuthenticationResult AuthenticateUser(ClientInfo _cInfo)
		{
			UserIdentifierEos identifierEos = (UserIdentifierEos)_cInfo.CrossplatformId;
			Log.Out("[EOS] Verifying token for " + identifierEos.ProductUserIdString);
			EosHelpers.AssertMainThread("ASe.Auth");
			IdToken value = new IdToken
			{
				JsonWebToken = identifierEos.Ticket,
				ProductUserId = identifierEos.ProductUserId
			};
			VerifyIdTokenOptions verifyIdTokenOptions = new VerifyIdTokenOptions
			{
				IdToken = new IdToken?(value)
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				Action<SanctionsCheckResult> <>9__1;
				this.connectInterface.VerifyIdToken(ref verifyIdTokenOptions, null, delegate(ref VerifyIdTokenCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS] VerifyIdToken failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						KickPlayerDelegate kickPlayerDelegate = this.kickPlayerDelegate;
						ClientInfo cInfo = _cInfo;
						GameUtils.EKickReason kickReason = GameUtils.EKickReason.CrossPlatformAuthenticationFailed;
						int apiResponseEnum = 50;
						string customReason = _callbackData.ResultCode.ToStringCached<Result>();
						kickPlayerDelegate(cInfo, new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason));
						return;
					}
					if (!_callbackData.IsAccountInfoPresent)
					{
						Log.Error("[EOS] VerifyIdToken failed: No account info");
						KickPlayerDelegate kickPlayerDelegate2 = this.kickPlayerDelegate;
						ClientInfo cInfo2 = _cInfo;
						GameUtils.EKickReason kickReason2 = GameUtils.EKickReason.CrossPlatformAuthenticationFailed;
						int apiResponseEnum2 = 50;
						string customReason = _callbackData.ResultCode.ToStringCached<Result>();
						kickPlayerDelegate2(cInfo2, new GameUtils.KickPlayerData(kickReason2, apiResponseEnum2, default(DateTime), customReason));
						return;
					}
					string text = _callbackData.Platform;
					string text2 = _callbackData.AccountId;
					string text3 = _callbackData.DeviceType;
					ExternalAccountType accountIdType = _callbackData.AccountIdType;
					ProductUserId productUserId = _callbackData.ProductUserId;
					EPlatformIdentifier eplatformIdentifier;
					if (!EosHelpers.AccountTypeMappings.TryGetValue(accountIdType, out eplatformIdentifier))
					{
						KickPlayerDelegate kickPlayerDelegate3 = this.kickPlayerDelegate;
						ClientInfo cInfo3 = _cInfo;
						GameUtils.EKickReason kickReason3 = GameUtils.EKickReason.CrossPlatformAuthenticationFailed;
						int apiResponseEnum3 = 50;
						string customReason = "UnsupportedAccountType " + accountIdType.ToStringCached<ExternalAccountType>();
						kickPlayerDelegate3(cInfo3, new GameUtils.KickPlayerData(kickReason3, apiResponseEnum3, default(DateTime), customReason));
						return;
					}
					if (eplatformIdentifier != _cInfo.PlatformId.PlatformIdentifier)
					{
						KickPlayerDelegate kickPlayerDelegate4 = this.kickPlayerDelegate;
						ClientInfo cInfo4 = _cInfo;
						GameUtils.EKickReason kickReason4 = GameUtils.EKickReason.CrossPlatformAuthenticationFailed;
						int apiResponseEnum4 = 50;
						string customReason = string.Concat(new string[]
						{
							"PlatformIdentifierMismatch (",
							eplatformIdentifier.ToStringCached<EPlatformIdentifier>(),
							" vs ",
							_cInfo.PlatformId.PlatformIdentifier.ToStringCached<EPlatformIdentifier>(),
							")"
						});
						kickPlayerDelegate4(cInfo4, new GameUtils.KickPlayerData(kickReason4, apiResponseEnum4, default(DateTime), customReason));
						return;
					}
					if (text2 != _cInfo.PlatformId.ReadablePlatformUserIdentifier)
					{
						KickPlayerDelegate kickPlayerDelegate5 = this.kickPlayerDelegate;
						ClientInfo cInfo5 = _cInfo;
						GameUtils.EKickReason kickReason5 = GameUtils.EKickReason.CrossPlatformAuthenticationFailed;
						int apiResponseEnum5 = 50;
						string customReason = "AccountIdMismatch (" + text2 + ")";
						kickPlayerDelegate5(cInfo5, new GameUtils.KickPlayerData(kickReason5, apiResponseEnum5, default(DateTime), customReason));
						return;
					}
					string text4 = productUserId.ToString();
					if (text4 != identifierEos.ProductUserIdString)
					{
						KickPlayerDelegate kickPlayerDelegate6 = this.kickPlayerDelegate;
						ClientInfo cInfo6 = _cInfo;
						GameUtils.EKickReason kickReason6 = GameUtils.EKickReason.CrossPlatformAuthenticationFailed;
						int apiResponseEnum6 = 50;
						string customReason = "PuidMismatch (" + text4 + ")";
						kickPlayerDelegate6(cInfo6, new GameUtils.KickPlayerData(kickReason6, apiResponseEnum6, default(DateTime), customReason));
						return;
					}
					Log.Out(string.Format("[EOS] Device={0}, Platform={1}, AccType={2}, AccId={3}, PUID={4}", new object[]
					{
						text3,
						text,
						accountIdType,
						text2,
						productUserId
					}));
					_cInfo.device = EosHelpers.GetDeviceTypeFromPlatform(text);
					_cInfo.requiresAntiCheat = _cInfo.device.RequiresAntiCheat();
					EPlayGroup eplayGroup = _cInfo.device.ToPlayGroup();
					if (eplayGroup != EPlayGroupExtensions.Current && !SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.AllowsCrossplay)
					{
						KickPlayerDelegate kickPlayerDelegate7 = this.kickPlayerDelegate;
						ClientInfo cInfo7 = _cInfo;
						GameUtils.EKickReason kickReason7 = GameUtils.EKickReason.CrossPlatformAuthenticationFailed;
						int apiResponseEnum7 = 50;
						string customReason = string.Format("NoCrossplay {0} <-> {1}", EPlayGroupExtensions.Current, eplayGroup);
						kickPlayerDelegate7(cInfo7, new GameUtils.KickPlayerData(kickReason7, apiResponseEnum7, default(DateTime), customReason));
						return;
					}
					ProductUserId productUserId2 = null;
					if (!GameManager.IsDedicatedServer)
					{
						UserIdentifierEos userIdentifierEos = this.owner.User.PlatformUserId as UserIdentifierEos;
						productUserId2 = ((userIdentifierEos != null) ? userIdentifierEos.ProductUserId : null);
					}
					if (GameManager.IsDedicatedServer && GamePrefs.GetBool(EnumGamePrefs.IgnoreEOSSanctions))
					{
						this.authSuccessfulDelegate(_cInfo);
						((SessionsHost)this.owner.ServerListAnnouncer).RegisterUser(_cInfo);
						return;
					}
					SanctionsCheck eosSanctionsCheck = ((Api)this.owner.Api).eosSanctionsCheck;
					SanctionsInterface sanctionsInterface = this.sanctionsInterface;
					ProductUserId productUserId3 = identifierEos.ProductUserId;
					ProductUserId localUser = productUserId2;
					Action<SanctionsCheckResult> callback;
					if ((callback = <>9__1) == null)
					{
						callback = (<>9__1 = delegate(SanctionsCheckResult result)
						{
							if (!result.Success || result.HasActiveSanctions)
							{
								this.kickPlayerDelegate(_cInfo, result.KickReason);
								return;
							}
							this.authSuccessfulDelegate(_cInfo);
							((SessionsHost)this.owner.ServerListAnnouncer).RegisterUser(_cInfo);
						});
					}
					eosSanctionsCheck.CheckSanctions(sanctionsInterface, productUserId3, localUser, callback);
				});
			}
			return EBeginUserAuthenticationResult.Ok;
		}

		// Token: 0x0600BD79 RID: 48505 RVA: 0x0047C9E0 File Offset: 0x0047ABE0
		public void RemoveUser(ClientInfo _cInfo)
		{
			((SessionsHost)this.owner.ServerListAnnouncer).UnregisterUser(_cInfo);
		}

		// Token: 0x0600BD7A RID: 48506 RVA: 0x0047C9F8 File Offset: 0x0047ABF8
		public void StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate)
		{
			this.authSuccessfulDelegate = _authSuccessfulDelegate;
			this.kickPlayerDelegate = _kickPlayerDelegate;
		}

		// Token: 0x0600BD7B RID: 48507 RVA: 0x000424BD File Offset: 0x000406BD
		public void StartServerSteamGroups(SteamGroupStatusResponse _groupStatusResponseDelegate)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600BD7C RID: 48508 RVA: 0x0047CA08 File Offset: 0x0047AC08
		public void StopServer()
		{
			this.authSuccessfulDelegate = null;
			this.kickPlayerDelegate = null;
		}

		// Token: 0x0600BD7D RID: 48509 RVA: 0x000424BD File Offset: 0x000406BD
		public bool RequestUserInGroupStatus(ClientInfo _cInfo, string _steamIdGroup)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04009386 RID: 37766
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x04009387 RID: 37767
		[PublicizedFrom(EAccessModifier.Private)]
		public AuthenticationSuccessfulCallbackDelegate authSuccessfulDelegate;

		// Token: 0x04009388 RID: 37768
		[PublicizedFrom(EAccessModifier.Private)]
		public KickPlayerDelegate kickPlayerDelegate;
	}
}
