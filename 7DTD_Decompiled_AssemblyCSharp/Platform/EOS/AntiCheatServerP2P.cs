using System;
using System.IO;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatClient;
using Epic.OnlineServices.AntiCheatCommon;

namespace Platform.EOS
{
	// Token: 0x02001908 RID: 6408
	public class AntiCheatServerP2P : IAntiCheatServer, IAntiCheatEncryption, IEncryptionModule
	{
		// Token: 0x0600BD47 RID: 48455 RVA: 0x0047B27C File Offset: 0x0047947C
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
			this.owner.Api.ClientApiInitialized += this.apiInitialized;
		}

		// Token: 0x0600BD48 RID: 48456 RVA: 0x0047B2A4 File Offset: 0x004794A4
		[PublicizedFrom(EAccessModifier.Private)]
		public void apiInitialized()
		{
			EosHelpers.AssertMainThread("ACSP2P.Init");
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.antiCheatInterface = ((Api)this.owner.Api).PlatformInterface.GetAntiCheatClientInterface();
			}
			if (this.antiCheatInterface == null)
			{
				Log.Out("[EAC] AntiCheatServerP2P initialized with null interface");
				return;
			}
		}

		// Token: 0x0600BD49 RID: 48457 RVA: 0x0047B320 File Offset: 0x00479520
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddCallbacks()
		{
			AddNotifyMessageToPeerOptions addNotifyMessageToPeerOptions = default(AddNotifyMessageToPeerOptions);
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handleMessageToPeerID = this.antiCheatInterface.AddNotifyMessageToPeer(ref addNotifyMessageToPeerOptions, null, new OnMessageToPeerCallback(this.handleMessageToPeer));
			}
			AddNotifyPeerAuthStatusChangedOptions addNotifyPeerAuthStatusChangedOptions = default(AddNotifyPeerAuthStatusChangedOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handlePeerAuthStateChangeID = this.antiCheatInterface.AddNotifyPeerAuthStatusChanged(ref addNotifyPeerAuthStatusChangedOptions, null, new OnPeerAuthStatusChangedCallback(this.handlePeerAuthStateChange));
			}
			AddNotifyPeerActionRequiredOptions addNotifyPeerActionRequiredOptions = default(AddNotifyPeerActionRequiredOptions);
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.handlePeerActionRequiredID = this.antiCheatInterface.AddNotifyPeerActionRequired(ref addNotifyPeerActionRequiredOptions, null, new OnPeerActionRequiredCallback(this.handlePeerActionRequired));
			}
		}

		// Token: 0x0600BD4A RID: 48458 RVA: 0x0047B428 File Offset: 0x00479628
		[PublicizedFrom(EAccessModifier.Private)]
		public void RemoveCallbacks()
		{
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.antiCheatInterface.RemoveNotifyMessageToPeer(this.handleMessageToPeerID);
				this.antiCheatInterface.RemoveNotifyPeerAuthStatusChanged(this.handlePeerAuthStateChangeID);
				this.antiCheatInterface.RemoveNotifyPeerActionRequired(this.handlePeerActionRequiredID);
			}
		}

		// Token: 0x0600BD4B RID: 48459 RVA: 0x0047B494 File Offset: 0x00479694
		public bool GetHostUserIdAndToken([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] out ValueTuple<PlatformUserIdentifierAbs, string> _hostUserIdAndToken)
		{
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			PlatformUserIdentifierAbs item;
			if (crossplatformPlatform == null)
			{
				item = null;
			}
			else
			{
				IUserClient user = crossplatformPlatform.User;
				item = ((user != null) ? user.PlatformUserId : null);
			}
			IPlatform crossplatformPlatform2 = PlatformManager.CrossplatformPlatform;
			string item2;
			if (crossplatformPlatform2 == null)
			{
				item2 = null;
			}
			else
			{
				IAuthenticationClient authenticationClient = crossplatformPlatform2.AuthenticationClient;
				item2 = ((authenticationClient != null) ? authenticationClient.GetAuthTicket() : null);
			}
			_hostUserIdAndToken = new ValueTuple<PlatformUserIdentifierAbs, string>(item, item2);
			return true;
		}

		// Token: 0x0600BD4C RID: 48460 RVA: 0x00002914 File Offset: 0x00000B14
		public void Update()
		{
		}

		// Token: 0x0600BD4D RID: 48461 RVA: 0x0047B4E8 File Offset: 0x004796E8
		public bool StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate)
		{
			if (this.ServerEacEnabled())
			{
				this.AddCallbacks();
				Log.Out("[EAC] Starting EAC peer to peer server");
				this.authSuccessfulDelegate = _authSuccessfulDelegate;
				this.kickPlayerDelegate = _kickPlayerDelegate;
				ProductUserId productUserId = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
				BeginSessionOptions beginSessionOptions = new BeginSessionOptions
				{
					LocalUserId = productUserId,
					Mode = AntiCheatClientMode.PeerToPeer
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.antiCheatInterface.BeginSession(ref beginSessionOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-ACSP2P] Starting module failed: " + result.ToStringCached<Result>());
				}
				else
				{
					this.serverRunning = true;
				}
				return result == Result.Success;
			}
			return true;
		}

		// Token: 0x0600BD4E RID: 48462 RVA: 0x0047B5BC File Offset: 0x004797BC
		public bool RegisterUser(ClientInfo _client)
		{
			if (!this.serverRunning)
			{
				return false;
			}
			Log.Out(string.Format("[EOS-ACSP2P] Registering user: {0}", _client));
			EosHelpers.AssertMainThread("ACSP2P.Reg");
			RegisterPeerOptions registerPeerOptions = new RegisterPeerOptions
			{
				PeerHandle = AntiCheatCommon.ClientInfoToIntPtr(_client),
				ClientPlatform = EosHelpers.DeviceTypeToAntiCheatPlatformMappings[_client.device],
				PeerProductUserId = ((UserIdentifierEos)_client.CrossplatformId).ProductUserId,
				ClientType = (_client.requiresAntiCheat ? AntiCheatCommonClientType.ProtectedClient : AntiCheatCommonClientType.UnprotectedClient),
				IpAddress = _client.ip,
				AuthenticationTimeout = 60U
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.RegisterPeer(ref registerPeerOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Failed registering user: " + result.ToStringCached<Result>());
				return false;
			}
			if (!_client.requiresAntiCheat)
			{
				this.authSuccessfulDelegate(_client);
			}
			return true;
		}

		// Token: 0x0600BD4F RID: 48463 RVA: 0x0047B6CC File Offset: 0x004798CC
		public void FreeUser(ClientInfo _client)
		{
			if (!this.serverRunning)
			{
				return;
			}
			EosHelpers.AssertMainThread("ACS.Free");
			Log.Out(string.Format("[EOS-ACSP2P] FreeUser: {0}", _client));
			UnregisterPeerOptions unregisterPeerOptions = new UnregisterPeerOptions
			{
				PeerHandle = AntiCheatCommon.ClientInfoToIntPtr(_client)
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.UnregisterPeer(ref unregisterPeerOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Failed unregistering user: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600BD50 RID: 48464 RVA: 0x0047B768 File Offset: 0x00479968
		public void HandleMessageFromClient(ClientInfo _cInfo, byte[] _data)
		{
			if (!this.serverRunning)
			{
				Log.Warning("[EOS-ACSP2P] Server: Received EAC package but EAC was not initialized");
				return;
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACSP2P] PushNetworkMessage (len={0}, from={1})", _data.Length, _cInfo.InternalId));
			}
			ReceiveMessageFromPeerOptions receiveMessageFromPeerOptions = new ReceiveMessageFromPeerOptions
			{
				Data = new ArraySegment<byte>(_data),
				PeerHandle = AntiCheatCommon.ClientInfoToIntPtr(_cInfo)
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.ReceiveMessageFromPeer(ref receiveMessageFromPeerOptions);
			}
			if (result != Result.AntiCheatPeerNotFound && result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Failed handling message: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600BD51 RID: 48465 RVA: 0x0047B82C File Offset: 0x00479A2C
		public void StopServer()
		{
			if (!this.serverRunning)
			{
				return;
			}
			this.RemoveCallbacks();
			EndSessionOptions endSessionOptions = default(EndSessionOptions);
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.antiCheatInterface.EndSession(ref endSessionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACSP2P] Stopping module failed: " + result.ToStringCached<Result>());
			}
			this.serverRunning = false;
			this.authSuccessfulDelegate = null;
			this.kickPlayerDelegate = null;
		}

		// Token: 0x0600BD52 RID: 48466 RVA: 0x0047B8B8 File Offset: 0x00479AB8
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleMessageToPeer(ref OnMessageToClientCallbackInfo _data)
		{
			if (!this.serverRunning)
			{
				return;
			}
			ClientInfo clientInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACSP2P] Got message for unknown client number: {0}");
			if (clientInfo == null)
			{
				Log.Out(string.Format("[EOS-ACSP2P] FreeUser: {0}", _data.ClientHandle));
				UnregisterPeerOptions unregisterPeerOptions = new UnregisterPeerOptions
				{
					PeerHandle = _data.ClientHandle
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.antiCheatInterface.UnregisterPeer(ref unregisterPeerOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-ACSP2P] Failed unregistering user: " + result.ToStringCached<Result>());
				}
			}
			if (clientInfo != null)
			{
				clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageEAC>().Setup(_data.MessageData.Count, _data.MessageData.Array));
			}
		}

		// Token: 0x0600BD53 RID: 48467 RVA: 0x0047B99C File Offset: 0x00479B9C
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerActionRequired(ref OnClientActionRequiredCallbackInfo _data)
		{
			if (!this.serverRunning)
			{
				return;
			}
			ClientInfo clientInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACSP2P] Got action for unknown client number: {0}");
			if (clientInfo == null)
			{
				return;
			}
			AntiCheatCommonClientAction clientAction = _data.ClientAction;
			AntiCheatCommonClientActionReason actionReasonCode = _data.ActionReasonCode;
			string text = _data.ActionReasonDetailsString;
			if (clientAction != AntiCheatCommonClientAction.RemovePlayer)
			{
				Log.Warning(string.Format("[EOS-ACSP2P] Got invalid action ({0}), reason='{1}', details={2}, client={3}", new object[]
				{
					clientAction.ToStringCached<AntiCheatCommonClientAction>(),
					actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(),
					text,
					clientInfo
				}));
				return;
			}
			Log.Out(string.Format("[EOS-ACSP2P] Kicking player. Reason={0}, details='{1}', client={2}", actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(), text, clientInfo));
			KickPlayerDelegate kickPlayerDelegate = this.kickPlayerDelegate;
			if (kickPlayerDelegate == null)
			{
				return;
			}
			ClientInfo cInfo = clientInfo;
			GameUtils.EKickReason kickReason = GameUtils.EKickReason.EosEacViolation;
			int apiResponseEnum = (int)actionReasonCode;
			string customReason = text;
			kickPlayerDelegate(cInfo, new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason));
		}

		// Token: 0x0600BD54 RID: 48468 RVA: 0x0047BA54 File Offset: 0x00479C54
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerAuthStateChange(ref OnClientAuthStatusChangedCallbackInfo _data)
		{
			if (!this.serverRunning)
			{
				return;
			}
			ClientInfo cInfo = AntiCheatCommon.IntPtrToClientInfo(_data.ClientHandle, "[EOS-ACSP2P] Got auth state change for unknown client number: {0}");
			if (_data.ClientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				Log.Out(string.Format("[EOS-ACSP2P] Remote Auth complete for client number {0}", _data.ClientHandle));
				AuthenticationSuccessfulCallbackDelegate authenticationSuccessfulCallbackDelegate = this.authSuccessfulDelegate;
				if (authenticationSuccessfulCallbackDelegate == null)
				{
					return;
				}
				authenticationSuccessfulCallbackDelegate(cInfo);
			}
		}

		// Token: 0x0600BD55 RID: 48469 RVA: 0x00002914 File Offset: 0x00000B14
		public void Destroy()
		{
		}

		// Token: 0x0600BD56 RID: 48470 RVA: 0x0047BAAF File Offset: 0x00479CAF
		public bool ServerEacEnabled()
		{
			return this.antiCheatInterface != null && GamePrefs.GetBool(EnumGamePrefs.ServerEACPeerToPeer);
		}

		// Token: 0x0600BD57 RID: 48471 RVA: 0x0047BAC8 File Offset: 0x00479CC8
		public bool ServerEacAvailable()
		{
			return this.antiCheatInterface != null;
		}

		// Token: 0x0600BD58 RID: 48472 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool EncryptionAvailable()
		{
			return false;
		}

		// Token: 0x0600BD59 RID: 48473 RVA: 0x0047BAD6 File Offset: 0x00479CD6
		public bool EncryptStream(ClientInfo _cInfo, MemoryStream _stream)
		{
			throw new NotImplementedException("Encryption is not supported for a Peer to Peer AntiCheatServer.");
		}

		// Token: 0x0600BD5A RID: 48474 RVA: 0x0047BAD6 File Offset: 0x00479CD6
		public bool DecryptStream(ClientInfo _cInfo, MemoryStream _stream)
		{
			throw new NotImplementedException("Encryption is not supported for a Peer to Peer AntiCheatServer.");
		}

		// Token: 0x04009368 RID: 37736
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x04009369 RID: 37737
		[PublicizedFrom(EAccessModifier.Private)]
		public AntiCheatClientInterface antiCheatInterface;

		// Token: 0x0400936A RID: 37738
		[PublicizedFrom(EAccessModifier.Private)]
		public bool serverRunning;

		// Token: 0x0400936B RID: 37739
		[PublicizedFrom(EAccessModifier.Private)]
		public AuthenticationSuccessfulCallbackDelegate authSuccessfulDelegate;

		// Token: 0x0400936C RID: 37740
		[PublicizedFrom(EAccessModifier.Private)]
		public KickPlayerDelegate kickPlayerDelegate;

		// Token: 0x0400936D RID: 37741
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handleMessageToPeerID;

		// Token: 0x0400936E RID: 37742
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerAuthStateChangeID;

		// Token: 0x0400936F RID: 37743
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerActionRequiredID;
	}
}
