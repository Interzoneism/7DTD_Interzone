using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatClient;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.Connect;

namespace Platform.EOS
{
	// Token: 0x02001903 RID: 6403
	public class AntiCheatClientP2P
	{
		// Token: 0x14000128 RID: 296
		// (add) Token: 0x0600BD16 RID: 48406 RVA: 0x00479C54 File Offset: 0x00477E54
		// (remove) Token: 0x0600BD17 RID: 48407 RVA: 0x00479CB4 File Offset: 0x00477EB4
		public event Action OnRemoteAuthComplete
		{
			add
			{
				object obj = this.lockObject;
				lock (obj)
				{
					this.onRemoteAuthComplete = (Action)Delegate.Combine(this.onRemoteAuthComplete, value);
					if (this.clientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
					{
						value();
					}
				}
			}
			remove
			{
				object obj = this.lockObject;
				lock (obj)
				{
					this.onRemoteAuthComplete = (Action)Delegate.Remove(this.onRemoteAuthComplete, value);
				}
			}
		}

		// Token: 0x0600BD18 RID: 48408 RVA: 0x00479D08 File Offset: 0x00477F08
		public AntiCheatClientP2P(IPlatform _owner, AntiCheatClientInterface _antiCheatInterface)
		{
			this.owner = _owner;
			this.antiCheatInterface = _antiCheatInterface;
			this.connectInterface = ((Api)this.owner.Api).ConnectInterface;
		}

		// Token: 0x0600BD19 RID: 48409 RVA: 0x00479D68 File Offset: 0x00477F68
		public void Activate()
		{
			if (this.handleMessageToPeerID == 0UL)
			{
				AddNotifyMessageToPeerOptions addNotifyMessageToPeerOptions = default(AddNotifyMessageToPeerOptions);
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.handleMessageToPeerID = this.antiCheatInterface.AddNotifyMessageToPeer(ref addNotifyMessageToPeerOptions, null, new OnMessageToPeerCallback(this.handleMessageToPeer));
				}
			}
			if (this.handlePeerAuthStateChangeID == 0UL)
			{
				AddNotifyPeerAuthStatusChangedOptions addNotifyPeerAuthStatusChangedOptions = default(AddNotifyPeerAuthStatusChangedOptions);
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.handlePeerAuthStateChangeID = this.antiCheatInterface.AddNotifyPeerAuthStatusChanged(ref addNotifyPeerAuthStatusChangedOptions, null, new OnPeerAuthStatusChangedCallback(this.handlePeerAuthStateChange));
				}
			}
			if (this.handlePeerActionRequiredID == 0UL)
			{
				AddNotifyPeerActionRequiredOptions addNotifyPeerActionRequiredOptions = default(AddNotifyPeerActionRequiredOptions);
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.handlePeerActionRequiredID = this.antiCheatInterface.AddNotifyPeerActionRequired(ref addNotifyPeerActionRequiredOptions, null, new OnPeerActionRequiredCallback(this.handlePeerActionRequired));
				}
			}
		}

		// Token: 0x0600BD1A RID: 48410 RVA: 0x00479E84 File Offset: 0x00478084
		public void Deactivate()
		{
			object obj = AntiCheatCommon.LockObject;
			lock (obj)
			{
				if (this.handleMessageToPeerID != 0UL)
				{
					this.antiCheatInterface.RemoveNotifyMessageToPeer(this.handleMessageToPeerID);
					this.handleMessageToPeerID = 0UL;
				}
				if (this.handlePeerAuthStateChangeID != 0UL)
				{
					this.antiCheatInterface.RemoveNotifyPeerAuthStatusChanged(this.handlePeerAuthStateChangeID);
					this.handlePeerAuthStateChangeID = 0UL;
				}
				if (this.handlePeerActionRequiredID != 0UL)
				{
					this.antiCheatInterface.RemoveNotifyPeerActionRequired(this.handlePeerActionRequiredID);
					this.handlePeerActionRequiredID = 0UL;
				}
			}
		}

		// Token: 0x0600BD1B RID: 48411 RVA: 0x00479F20 File Offset: 0x00478120
		[PublicizedFrom(EAccessModifier.Private)]
		public bool BeginSession()
		{
			ProductUserId productUserId = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
			BeginSessionOptions beginSessionOptions = new BeginSessionOptions
			{
				LocalUserId = productUserId,
				Mode = AntiCheatClientMode.PeerToPeer
			};
			object obj = AntiCheatCommon.LockObject;
			Result result;
			lock (obj)
			{
				result = this.antiCheatInterface.BeginSession(ref beginSessionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACCP2P] Starting module failed: " + result.ToStringCached<Result>());
				return false;
			}
			return true;
		}

		// Token: 0x0600BD1C RID: 48412 RVA: 0x00479FBC File Offset: 0x004781BC
		public void ConnectToServer([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] ValueTuple<PlatformUserIdentifierAbs, string> _hostIdentifierAndToken, Action _onConnectionComplete, Action<string> _onConnectionFailed)
		{
			AntiCheatClientP2P.<>c__DisplayClass18_0 CS$<>8__locals1 = new AntiCheatClientP2P.<>c__DisplayClass18_0();
			CS$<>8__locals1._onConnectionFailed = _onConnectionFailed;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1._onConnectionComplete = _onConnectionComplete;
			PlatformUserIdentifierAbs item = _hostIdentifierAndToken.Item1;
			CS$<>8__locals1.identifierEos = (item as UserIdentifierEos);
			if (CS$<>8__locals1.identifierEos != null)
			{
				CS$<>8__locals1.identifierEos.DecodeTicket(_hostIdentifierAndToken.Item2);
				IdToken value = new IdToken
				{
					JsonWebToken = CS$<>8__locals1.identifierEos.Ticket,
					ProductUserId = CS$<>8__locals1.identifierEos.ProductUserId
				};
				VerifyIdTokenOptions verifyIdTokenOptions = new VerifyIdTokenOptions
				{
					IdToken = new IdToken?(value)
				};
				object obj = AntiCheatCommon.LockObject;
				lock (obj)
				{
					this.connectInterface.VerifyIdToken(ref verifyIdTokenOptions, null, new OnVerifyIdTokenCallback(CS$<>8__locals1.<ConnectToServer>g__VerifyIdTokenCallback|0));
				}
				return;
			}
			Log.Warning(string.Format("[EOS] [ACl.Auth] Expected EOS Crossplatform ID? But got: {0}", _hostIdentifierAndToken.Item1));
			Action<string> onConnectionFailed = CS$<>8__locals1._onConnectionFailed;
			if (onConnectionFailed == null)
			{
				return;
			}
			onConnectionFailed("Invalid EOS Crossplatform ID");
		}

		// Token: 0x0600BD1D RID: 48413 RVA: 0x0047A0D4 File Offset: 0x004782D4
		public bool IsServerAntiCheatProtected()
		{
			return this.serverDeviceType.RequiresAntiCheat();
		}

		// Token: 0x0600BD1E RID: 48414 RVA: 0x0047A0E4 File Offset: 0x004782E4
		public void HandleMessageFromPeer(byte[] _data)
		{
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
			{
				return;
			}
			if (AntiCheatCommon.DebugEacVerbose)
			{
				Log.Out(string.Format("[EOS-ACC] PushNetworkMessage (len={0})", _data.Length));
			}
			EosHelpers.AssertMainThread("ACC.HMFP");
			ReceiveMessageFromPeerOptions receiveMessageFromPeerOptions = new ReceiveMessageFromPeerOptions
			{
				Data = new ArraySegment<byte>(_data),
				PeerHandle = this.serverHandle
			};
			object obj = AntiCheatCommon.LockObject;
			Result result;
			lock (obj)
			{
				result = this.antiCheatInterface.ReceiveMessageFromPeer(ref receiveMessageFromPeerOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACC] Failed handling message: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600BD1F RID: 48415 RVA: 0x0047A1A0 File Offset: 0x004783A0
		public void DisconnectFromServer()
		{
			this.clientAuthStatus = AntiCheatCommonClientAuthStatus.Invalid;
			this.serverDeviceType = ClientInfo.EDeviceType.Unknown;
			this.onRemoteAuthComplete = null;
			this.EndSession();
			this.Deactivate();
			Log.Out("[EOS-ACC] Disconnected from game server");
		}

		// Token: 0x0600BD20 RID: 48416 RVA: 0x0047A1D0 File Offset: 0x004783D0
		[PublicizedFrom(EAccessModifier.Private)]
		public void EndSession()
		{
			EosHelpers.AssertMainThread("ACC.Disc");
			EndSessionOptions endSessionOptions = default(EndSessionOptions);
			object obj = AntiCheatCommon.LockObject;
			Result result;
			lock (obj)
			{
				result = this.antiCheatInterface.EndSession(ref endSessionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-ACC] Stopping module failed: " + result.ToStringCached<Result>());
			}
		}

		// Token: 0x0600BD21 RID: 48417 RVA: 0x0047A248 File Offset: 0x00478448
		[PublicizedFrom(EAccessModifier.Private)]
		public void handleMessageToPeer(ref OnMessageToClientCallbackInfo _data)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageEAC>().Setup(_data.MessageData.Count, _data.MessageData.Array), false);
		}

		// Token: 0x0600BD22 RID: 48418 RVA: 0x0047A288 File Offset: 0x00478488
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerActionRequired(ref OnClientActionRequiredCallbackInfo _data)
		{
			if (_data.ClientHandle != this.serverHandle)
			{
				Log.Error("[EOS-ACCP2P] Received Peer action for non-server peer as a client.");
				return;
			}
			AntiCheatCommonClientAction clientAction = _data.ClientAction;
			AntiCheatCommonClientActionReason actionReasonCode = _data.ActionReasonCode;
			string text = _data.ActionReasonDetailsString;
			if (clientAction == AntiCheatCommonClientAction.RemovePlayer)
			{
				Log.Out(string.Concat(new string[]
				{
					"[EOS-ACCP2P] Disconnecting from server. Reason=",
					actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(),
					", details='",
					text,
					"'"
				}));
				GameUtils.EKickReason kickReason = GameUtils.EKickReason.EosEacViolation;
				int apiResponseEnum = (int)actionReasonCode;
				string customReason = text;
				this.QueueDisconnectFromServer(new GameUtils.KickPlayerData(kickReason, apiResponseEnum, default(DateTime), customReason));
				return;
			}
			Log.Warning(string.Concat(new string[]
			{
				"[EOS-ACCP2P] Got invalid action (",
				clientAction.ToStringCached<AntiCheatCommonClientAction>(),
				"), reason='",
				actionReasonCode.ToStringCached<AntiCheatCommonClientActionReason>(),
				"', details='",
				text,
				"'"
			}));
		}

		// Token: 0x0600BD23 RID: 48419 RVA: 0x0047A368 File Offset: 0x00478568
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator DisconnectOnNextFrame(GameUtils.KickPlayerData _kickPlayerData)
		{
			yield return null;
			this.EndSession();
			this.Deactivate();
			SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
			GameManager.Instance.ShowMessagePlayerDenied(_kickPlayerData);
			yield break;
		}

		// Token: 0x0600BD24 RID: 48420 RVA: 0x0047A37E File Offset: 0x0047857E
		[PublicizedFrom(EAccessModifier.Private)]
		public void QueueDisconnectFromServer(GameUtils.KickPlayerData _kickPlayerData)
		{
			ThreadManager.StartCoroutine(this.DisconnectOnNextFrame(_kickPlayerData));
		}

		// Token: 0x0600BD25 RID: 48421 RVA: 0x0047A390 File Offset: 0x00478590
		[PublicizedFrom(EAccessModifier.Private)]
		public void handlePeerAuthStateChange(ref OnClientAuthStatusChangedCallbackInfo _data)
		{
			if (_data.ClientHandle != this.serverHandle)
			{
				Log.Error("[EOS-ACCP2P] Received Peer auth state change for non-server peer as a client.");
				return;
			}
			this.clientAuthStatus = _data.ClientAuthStatus;
			if (this.clientAuthStatus == AntiCheatCommonClientAuthStatus.RemoteAuthComplete)
			{
				Log.Out("[EOS-ACCP2P] Auth State Change for Server : " + this.clientAuthStatus.ToStringCached<AntiCheatCommonClientAuthStatus>());
				Action action = this.onRemoteAuthComplete;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x04009349 RID: 37705
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x0400934A RID: 37706
		[PublicizedFrom(EAccessModifier.Private)]
		public ConnectInterface connectInterface;

		// Token: 0x0400934B RID: 37707
		[PublicizedFrom(EAccessModifier.Private)]
		public AntiCheatClientInterface antiCheatInterface;

		// Token: 0x0400934C RID: 37708
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handleMessageToPeerID;

		// Token: 0x0400934D RID: 37709
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerAuthStateChangeID;

		// Token: 0x0400934E RID: 37710
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong handlePeerActionRequiredID;

		// Token: 0x0400934F RID: 37711
		[PublicizedFrom(EAccessModifier.Private)]
		public IntPtr serverHandle = new IntPtr(int.MaxValue);

		// Token: 0x04009350 RID: 37712
		[PublicizedFrom(EAccessModifier.Private)]
		public ClientInfo.EDeviceType serverDeviceType = ClientInfo.EDeviceType.Unknown;

		// Token: 0x04009351 RID: 37713
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly object lockObject = new object();

		// Token: 0x04009352 RID: 37714
		[PublicizedFrom(EAccessModifier.Private)]
		public AntiCheatCommonClientAuthStatus clientAuthStatus;

		// Token: 0x04009353 RID: 37715
		[PublicizedFrom(EAccessModifier.Private)]
		public Action onRemoteAuthComplete;
	}
}
