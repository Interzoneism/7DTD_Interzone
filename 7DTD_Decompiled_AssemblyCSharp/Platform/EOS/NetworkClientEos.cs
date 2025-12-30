using System;
using System.Collections;
using System.Text;
using Epic.OnlineServices;
using Epic.OnlineServices.P2P;
using UnityEngine.Networking;

namespace Platform.EOS
{
	// Token: 0x0200191A RID: 6426
	public class NetworkClientEos : IPlatformNetworkClient, INetworkClient
	{
		// Token: 0x170015BC RID: 5564
		// (get) Token: 0x0600BDB1 RID: 48561 RVA: 0x0047E1CF File Offset: 0x0047C3CF
		public bool IsConnected
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.serverId != null && (this.protoManager.IsClient || this.connecting) && this.owner.User.UserStatus == EUserStatus.LoggedIn;
			}
		}

		// Token: 0x0600BDB2 RID: 48562 RVA: 0x0047E20C File Offset: 0x0047C40C
		public NetworkClientEos(IPlatform _owner, IProtocolManagerProtocolInterface _protoManager)
		{
			this.owner = _owner;
			this.protoManager = _protoManager;
			this.owner.Api.ClientApiInitialized += delegate()
			{
				if (!GameManager.IsDedicatedServer)
				{
					EosHelpers.AssertMainThread("P2P.Init");
					this.localUser = ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
					this.socketId = new SocketId
					{
						SocketName = "Game"
					};
					object lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface = ((Api)this.owner.Api).PlatformInterface.GetP2PInterface();
					}
					AddNotifyPeerConnectionRequestOptions addNotifyPeerConnectionRequestOptions = new AddNotifyPeerConnectionRequestOptions
					{
						LocalUserId = this.localUser,
						SocketId = new SocketId?(this.socketId)
					};
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyPeerConnectionRequest(ref addNotifyPeerConnectionRequestOptions, null, new OnIncomingConnectionRequestCallback(this.ConnectionRequestHandler));
					}
					AddNotifyPeerConnectionEstablishedOptions addNotifyPeerConnectionEstablishedOptions = new AddNotifyPeerConnectionEstablishedOptions
					{
						LocalUserId = this.localUser,
						SocketId = new SocketId?(this.socketId)
					};
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyPeerConnectionEstablished(ref addNotifyPeerConnectionEstablishedOptions, null, new OnPeerConnectionEstablishedCallback(this.ConnectionEstablishedHandler));
					}
					AddNotifyPeerConnectionClosedOptions addNotifyPeerConnectionClosedOptions = new AddNotifyPeerConnectionClosedOptions
					{
						LocalUserId = this.localUser,
						SocketId = new SocketId?(this.socketId)
					};
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyPeerConnectionClosed(ref addNotifyPeerConnectionClosedOptions, null, new OnRemoteConnectionClosedCallback(this.ConnectionClosedHandler));
					}
					AddNotifyIncomingPacketQueueFullOptions addNotifyIncomingPacketQueueFullOptions = default(AddNotifyIncomingPacketQueueFullOptions);
					lockObject = AntiCheatCommon.LockObject;
					lock (lockObject)
					{
						this.ptpInterface.AddNotifyIncomingPacketQueueFull(ref addNotifyIncomingPacketQueueFullOptions, null, new OnIncomingPacketQueueFullCallback(this.IncomingPacketQueueFullHandler));
					}
				}
			};
		}

		// Token: 0x0600BDB3 RID: 48563 RVA: 0x0047E26C File Offset: 0x0047C46C
		public void Connect(GameServerInfo _gsi)
		{
			this.disconnectEventReceived = false;
			Log.Out("[EOS-P2PC] Trying to connect to: " + _gsi.GetValue(GameInfoString.IP) + ":" + _gsi.GetValue(GameInfoInt.Port).ToString());
			if (string.IsNullOrEmpty(_gsi.GetValue(GameInfoString.CombinedPrimaryId)))
			{
				Log.Out("[EOS-P2PC] Resolving EOS ID for IP " + _gsi.GetValue(GameInfoString.IP) + ":" + _gsi.GetValue(GameInfoInt.Port).ToString());
				ServerInformationTcpClient.RequestRules(_gsi, false, new ServerInformationTcpClient.RulesRequestDone(this.RulesRequestTcpDone));
				this.connecting = true;
				return;
			}
			this.ConnectInternal(_gsi);
		}

		// Token: 0x0600BDB4 RID: 48564 RVA: 0x0047E305 File Offset: 0x0047C505
		[PublicizedFrom(EAccessModifier.Private)]
		public void RulesRequestTcpDone(bool _success, string _message, GameServerInfo _gsi)
		{
			if (_success && this.connecting)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo = _gsi;
				this.ConnectInternal(_gsi);
				return;
			}
			this.Disconnect();
			ThreadManager.StartCoroutine(this.connectionFailedLater(_message));
		}

		// Token: 0x0600BDB5 RID: 48565 RVA: 0x0047E338 File Offset: 0x0047C538
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectInternal(GameServerInfo _gsi)
		{
			string value = _gsi.GetValue(GameInfoString.CombinedPrimaryId);
			if (string.IsNullOrEmpty(value))
			{
				Log.Error("Server info does not have a CombinedPrimaryId");
				this.Disconnect();
				ThreadManager.StartCoroutine(this.connectionFailedLater(Localization.Get("netSteamNetworking_NoServerID", false)));
				return;
			}
			if (_gsi.AllowsCrossplay && !PermissionsManager.IsCrossplayAllowed())
			{
				this.Disconnect();
				this.protoManager.ConnectionFailedEv(Localization.Get("auth_noCrossplay", false));
				return;
			}
			UserIdentifierEos userIdentifierEos = PlatformUserIdentifierAbs.FromCombinedString(value, true) as UserIdentifierEos;
			if (userIdentifierEos == null)
			{
				this.Disconnect();
				ThreadManager.StartCoroutine(this.connectionFailedLater(Localization.Get("netSteamNetworking_NoServerID", false)));
				return;
			}
			string password = ServerInfoCache.Instance.GetPassword(_gsi);
			ArrayListMP<byte> arrayListMP;
			if (!string.IsNullOrEmpty(password))
			{
				int byteCount = Encoding.UTF8.GetByteCount(password);
				arrayListMP = new ArrayListMP<byte>(MemoryPools.poolByte, byteCount + 1)
				{
					Count = byteCount + 1
				};
				Encoding.UTF8.GetBytes(password, 0, password.Length, arrayListMP.Items, 1);
			}
			else
			{
				arrayListMP = new ArrayListMP<byte>(MemoryPools.poolByte, 1)
				{
					Count = 1
				};
			}
			EosHelpers.AssertMainThread("P2P.ConInt.PUID");
			this.serverId = userIdentifierEos.ProductUserId;
			string str = "[EOS-P2PC] Connecting to EOS ID ";
			ProductUserId productUserId = this.serverId;
			Log.Out(str + ((productUserId != null) ? productUserId.ToString() : null));
			this.connecting = true;
			this.SendData(50, arrayListMP);
		}

		// Token: 0x0600BDB6 RID: 48566 RVA: 0x0047E489 File Offset: 0x0047C689
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator connectionFailedLater(string _message)
		{
			yield return null;
			yield return null;
			this.protoManager.ConnectionFailedEv(_message);
			yield break;
		}

		// Token: 0x0600BDB7 RID: 48567 RVA: 0x0047E4A0 File Offset: 0x0047C6A0
		public void Disconnect()
		{
			this.connecting = false;
			this.sendBufs.Clear();
			EosHelpers.AssertMainThread("P2P.CloseCons");
			CloseConnectionsOptions closeConnectionsOptions = new CloseConnectionsOptions
			{
				SocketId = new SocketId?(this.socketId),
				LocalUserId = this.localUser
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.ptpInterface.CloseConnections(ref closeConnectionsOptions);
			}
			if (result != Result.Success)
			{
				Log.Error("[EOS-P2PC] Failed closing connections: " + result.ToStringCached<Result>());
			}
			this.serverId = null;
		}

		// Token: 0x0600BDB8 RID: 48568 RVA: 0x0047E550 File Offset: 0x0047C750
		public NetworkError SendData(int _channel, ArrayListMP<byte> _data)
		{
			if (this.IsConnected)
			{
				_data[0] = (byte)_channel;
				this.sendBufs.Enqueue(new NetworkCommonEos.SendInfo(null, _data));
			}
			else
			{
				Log.Warning("[EOS-P2PC] Tried to send a package while not connected to a server");
			}
			return NetworkError.Ok;
		}

		// Token: 0x0600BDB9 RID: 48569 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectionRequestHandler(ref OnIncomingConnectionRequestInfo _data)
		{
		}

		// Token: 0x0600BDBA RID: 48570 RVA: 0x0047E582 File Offset: 0x0047C782
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectionEstablishedHandler(ref OnPeerConnectionEstablishedInfo _data)
		{
			Log.Out(string.Format("[EOS-P2PC] Connection established: {0}", _data.RemoteUserId));
		}

		// Token: 0x0600BDBB RID: 48571 RVA: 0x0047E59C File Offset: 0x0047C79C
		[PublicizedFrom(EAccessModifier.Private)]
		public void ConnectionClosedHandler(ref OnRemoteConnectionClosedInfo _data)
		{
			ProductUserId remoteUserId = _data.RemoteUserId;
			if (this.connecting)
			{
				this.Disconnect();
				Log.Out(string.Format("[EOS-P2PC] P2PSessionConnectFail to: {0} - Error: {1}", _data.RemoteUserId, _data.Reason.ToStringCached<ConnectionClosedReason>()));
				string msg = Localization.Get("netSteamNetworkingSessionError_" + _data.Reason.ToStringCached<ConnectionClosedReason>(), false);
				this.protoManager.ConnectionFailedEv(msg);
				return;
			}
			if (!this.IsConnected)
			{
				return;
			}
			Log.Out(string.Format("[EOS-P2PC] Connection closed by {0}: ", remoteUserId) + _data.Reason.ToStringCached<ConnectionClosedReason>());
			if (_data.Reason == ConnectionClosedReason.ClosedByLocalUser)
			{
				return;
			}
			CloseConnectionOptions closeConnectionOptions = new CloseConnectionOptions
			{
				SocketId = new SocketId?(this.socketId),
				LocalUserId = this.localUser,
				RemoteUserId = remoteUserId
			};
			object lockObject = AntiCheatCommon.LockObject;
			Result result;
			lock (lockObject)
			{
				result = this.ptpInterface.CloseConnection(ref closeConnectionOptions);
			}
			if (result != Result.Success)
			{
				Log.Error(string.Format("[EOS-P2PC] Failed closing connection to {0}: {1}", remoteUserId, result.ToStringCached<Result>()));
			}
			this.OnDisconnectedFromServer();
		}

		// Token: 0x0600BDBC RID: 48572 RVA: 0x0047E6CC File Offset: 0x0047C8CC
		[PublicizedFrom(EAccessModifier.Private)]
		public void IncomingPacketQueueFullHandler(ref OnIncomingPacketQueueFullInfo _data)
		{
			if (!this.IsConnected)
			{
				return;
			}
			Log.Error(string.Format("[EOS-P2PC] Packet queue full: Chn={0}, IncSize={1}, Used={2}, Max={3}", new object[]
			{
				_data.OverflowPacketChannel,
				_data.OverflowPacketSizeBytes,
				_data.PacketQueueCurrentSizeBytes,
				_data.PacketQueueMaxSizeBytes
			}));
		}

		// Token: 0x0600BDBD RID: 48573 RVA: 0x0047E730 File Offset: 0x0047C930
		public void Update()
		{
			if (!this.IsConnected)
			{
				return;
			}
			Result result;
			for (;;)
			{
				ReceivePacketOptions receivePacketOptions = new ReceivePacketOptions
				{
					LocalUserId = this.localUser,
					MaxDataSizeBytes = 1170U
				};
				ProductUserId productUserId = new ProductUserId();
				SocketId socketId = default(SocketId);
				object lockObject = AntiCheatCommon.LockObject;
				uint num;
				lock (lockObject)
				{
					byte b;
					result = this.ptpInterface.ReceivePacket(ref receivePacketOptions, ref productUserId, ref socketId, out b, this.receiveBuffer, out num);
				}
				if (result != Result.Success)
				{
					break;
				}
				if (num > 0U)
				{
					NetworkCommonEos.ESteamNetChannels esteamNetChannels = (NetworkCommonEos.ESteamNetChannels)this.receiveBuffer.Array[0];
					if (esteamNetChannels > NetworkCommonEos.ESteamNetChannels.NetpackageChannel1)
					{
						if (esteamNetChannels != NetworkCommonEos.ESteamNetChannels.Authentication)
						{
							if (esteamNetChannels == NetworkCommonEos.ESteamNetChannels.Ping)
							{
								SendPacketOptions sendPacketOptions = new SendPacketOptions
								{
									SocketId = new SocketId?(this.socketId),
									LocalUserId = this.localUser,
									RemoteUserId = this.serverId,
									Channel = 0,
									Reliability = PacketReliability.ReliableOrdered,
									AllowDelayedDelivery = true,
									Data = this.receiveBuffer
								};
								lockObject = AntiCheatCommon.LockObject;
								Result result2;
								lock (lockObject)
								{
									result2 = this.ptpInterface.SendPacket(ref sendPacketOptions);
								}
								if (result2 != Result.Success)
								{
									Log.Error("[EOS-P2PC] Could not send ping package to server: " + result2.ToStringCached<Result>());
								}
							}
						}
						else
						{
							if (this.connecting)
							{
								this.connecting = false;
								Log.Out("[EOS-P2PC] Connection established");
							}
							if (this.receiveBuffer.Array[1] == 0)
							{
								Log.Out("[EOS-P2PC] Received invalid password package");
								ThreadManager.AddSingleTaskMainThread("SteamNetInvalidPassword", delegate(object _info)
								{
									this.protoManager.InvalidPasswordEv();
								}, null);
							}
							else
							{
								Log.Out("[EOS-P2PC] Password accepted");
								this.OnConnectedToServer();
							}
						}
					}
					else
					{
						byte[] array = MemoryPools.poolByte.Alloc((int)num);
						Array.Copy(this.receiveBuffer.Array, array, (long)((ulong)num));
						SingletonMonoBehaviour<ConnectionManager>.Instance.Net_DataReceivedClient((int)esteamNetChannels, array, (int)num);
					}
				}
			}
			if (result != Result.NotFound)
			{
				Log.Error("[EOS-P2PS] Error reading packages: " + result.ToStringCached<Result>());
				return;
			}
		}

		// Token: 0x0600BDBE RID: 48574 RVA: 0x0047E978 File Offset: 0x0047CB78
		public void LateUpdate()
		{
			if (!this.IsConnected)
			{
				return;
			}
			while (this.sendBufs.HasData())
			{
				NetworkCommonEos.SendInfo sendInfo = this.sendBufs.Dequeue();
				ProductUserId remoteUserId = this.serverId;
				SendPacketOptions sendPacketOptions = new SendPacketOptions
				{
					SocketId = new SocketId?(this.socketId),
					LocalUserId = this.localUser,
					RemoteUserId = remoteUserId,
					Channel = 0,
					Reliability = PacketReliability.ReliableOrdered,
					AllowDelayedDelivery = true,
					Data = new ArraySegment<byte>(sendInfo.Data.Items, 0, sendInfo.Data.Count)
				};
				object lockObject = AntiCheatCommon.LockObject;
				Result result;
				lock (lockObject)
				{
					result = this.ptpInterface.SendPacket(ref sendPacketOptions);
				}
				if (result != Result.Success)
				{
					Log.Error("[EOS-P2PC] Could not send package to server: " + result.ToStringCached<Result>());
				}
			}
		}

		// Token: 0x0600BDBF RID: 48575 RVA: 0x0047EA78 File Offset: 0x0047CC78
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnDisconnectedFromServer()
		{
			this.Disconnect();
			this.protoManager.DisconnectedFromServerEv(Localization.Get("netSteamNetworking_ConnectionClosedByServer", false));
		}

		// Token: 0x0600BDC0 RID: 48576 RVA: 0x0047EA98 File Offset: 0x0047CC98
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnConnectedToServer()
		{
			INetConnection[] array = new INetConnection[2];
			for (int i = 0; i < 2; i++)
			{
				array[i] = new NetConnectionSimple(i, null, this, null, 1, 1120);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SetConnectionToServer(array);
		}

		// Token: 0x0600BDC1 RID: 48577 RVA: 0x00002914 File Offset: 0x00000B14
		public void SetLatencySimulation(bool _enable, int _minLatency, int _maxLatency)
		{
		}

		// Token: 0x0600BDC2 RID: 48578 RVA: 0x00002914 File Offset: 0x00000B14
		public void SetPacketLossSimulation(bool _enable, int _chance)
		{
		}

		// Token: 0x0600BDC3 RID: 48579 RVA: 0x00002914 File Offset: 0x00000B14
		public void EnableStatistics()
		{
		}

		// Token: 0x0600BDC4 RID: 48580 RVA: 0x00002914 File Offset: 0x00000B14
		public void DisableStatistics()
		{
		}

		// Token: 0x0600BDC5 RID: 48581 RVA: 0x0002B133 File Offset: 0x00029333
		public string PrintNetworkStatistics()
		{
			return "";
		}

		// Token: 0x0600BDC6 RID: 48582 RVA: 0x00002914 File Offset: 0x00000B14
		public void ResetNetworkStatistics()
		{
		}

		// Token: 0x040093BE RID: 37822
		[PublicizedFrom(EAccessModifier.Private)]
		public const string socketName = "Game";

		// Token: 0x040093BF RID: 37823
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x040093C0 RID: 37824
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IProtocolManagerProtocolInterface protoManager;

		// Token: 0x040093C1 RID: 37825
		[PublicizedFrom(EAccessModifier.Private)]
		public P2PInterface ptpInterface;

		// Token: 0x040093C2 RID: 37826
		[PublicizedFrom(EAccessModifier.Private)]
		public ProductUserId localUser;

		// Token: 0x040093C3 RID: 37827
		[PublicizedFrom(EAccessModifier.Private)]
		public SocketId socketId;

		// Token: 0x040093C4 RID: 37828
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly BlockingQueue<NetworkCommonEos.SendInfo> sendBufs = new BlockingQueue<NetworkCommonEos.SendInfo>();

		// Token: 0x040093C5 RID: 37829
		[PublicizedFrom(EAccessModifier.Private)]
		public ProductUserId serverId;

		// Token: 0x040093C6 RID: 37830
		[PublicizedFrom(EAccessModifier.Private)]
		public bool connecting;

		// Token: 0x040093C7 RID: 37831
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disconnectEventReceived;

		// Token: 0x040093C8 RID: 37832
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(new byte[1170]);
	}
}
