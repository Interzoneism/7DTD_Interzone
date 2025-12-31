using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using UnityEngine.Networking;

// Token: 0x020007CD RID: 1997
public class NetworkServerLiteNetLib : INetworkServer
{
	// Token: 0x06003992 RID: 14738 RVA: 0x00173C7F File Offset: 0x00171E7F
	public NetworkServerLiteNetLib(IProtocolManagerProtocolInterface _protoManager)
	{
		this.protoManager = _protoManager;
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x00173CA4 File Offset: 0x00171EA4
	public void Update()
	{
		ClientInfo clientInfo;
		do
		{
			clientInfo = null;
			List<ClientInfo> obj = this.dropClientsQueue;
			lock (obj)
			{
				int num = this.dropClientsQueue.Count - 1;
				if (num >= 0)
				{
					clientInfo = this.dropClientsQueue[num];
					this.dropClientsQueue.RemoveAt(num);
				}
			}
			if (clientInfo != null)
			{
				this.DropClient(clientInfo, false);
			}
		}
		while (clientInfo != null);
		NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer liteNetLibAuthWrapperServer = this.authWrapper;
		if (liteNetLibAuthWrapperServer == null)
		{
			return;
		}
		liteNetLibAuthWrapperServer.Update();
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x00002914 File Offset: 0x00000B14
	public void LateUpdate()
	{
	}

	// Token: 0x06003995 RID: 14741 RVA: 0x00173D2C File Offset: 0x00171F2C
	public NetworkConnectionError StartServer(int _basePort, string _password)
	{
		this.serverPassword = (string.IsNullOrEmpty(_password) ? "" : _password);
		EventBasedNetListener eventBasedNetListener = new EventBasedNetListener();
		this.server = new NetManager(eventBasedNetListener, null);
		this.authWrapper = new NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer(this);
		NetworkCommonLiteNetLib.InitConfig(this.server);
		eventBasedNetListener.ConnectionRequestEvent += this.authWrapper.ConnectionRequestCheck;
		eventBasedNetListener.PeerConnectedEvent += this.authWrapper.OnPeerConnectedEvent;
		eventBasedNetListener.PeerDisconnectedEvent += this.authWrapper.OnPeerDisconnectedEvent;
		eventBasedNetListener.NetworkReceiveEvent += this.authWrapper.OnNetworkReceiveEvent;
		eventBasedNetListener.NetworkErrorEvent += delegate(IPEndPoint _endpoint, SocketError _code)
		{
			Log.Error("NET: LiteNetLib: Network error: {0}", new object[]
			{
				_code
			});
		};
		if (this.server.Start(_basePort + 2))
		{
			Log.Out("NET: LiteNetLib server started");
			return NetworkConnectionError.NoError;
		}
		Log.Out("NET: LiteNetLib server could not be started");
		return NetworkConnectionError.CreateSocketOrThreadFailure;
	}

	// Token: 0x06003996 RID: 14742 RVA: 0x00173E23 File Offset: 0x00172023
	public void SetServerPassword(string _password)
	{
		this.serverPassword = (_password ?? "");
	}

	// Token: 0x06003997 RID: 14743 RVA: 0x00173E38 File Offset: 0x00172038
	public void StopServer()
	{
		NetManager netManager = this.server;
		if (netManager != null && netManager.IsRunning)
		{
			List<NetPeer> list = new List<NetPeer>();
			this.server.GetPeersNonAlloc(list, ConnectionState.Any);
			for (int i = 0; i < list.Count; i++)
			{
				this.server.DisconnectPeer(list[i], NetworkServerLiteNetLib.disconnectServerShutdown);
			}
			this.server.Stop();
			this.server = null;
			this.authWrapper = null;
		}
		Log.Out("NET: LiteNetLib server stopped");
	}

	// Token: 0x06003998 RID: 14744 RVA: 0x00173EB8 File Offset: 0x001720B8
	public void DropClient(ClientInfo _clientInfo, bool _clientDisconnect)
	{
		this.OnPlayerDisconnected(_clientInfo.litenetPeerConnectId);
		NetPeer peerByConnectId = this.GetPeerByConnectId(_clientInfo.litenetPeerConnectId);
		if (peerByConnectId != null)
		{
			this.server.DisconnectPeer(peerByConnectId, NetworkServerLiteNetLib.disconnectFromClientSide);
		}
	}

	// Token: 0x06003999 RID: 14745 RVA: 0x00173EF4 File Offset: 0x001720F4
	public NetworkError SendData(ClientInfo _cInfo, int _channel, ArrayListMP<byte> _data, bool _reliableDelivery = true)
	{
		NetPeer peerByConnectId = this.GetPeerByConnectId(_cInfo.litenetPeerConnectId);
		if (peerByConnectId == null)
		{
			Log.Warning("NET: LiteNetLib: SendData requested for unknown client {0}", new object[]
			{
				_cInfo.ToString()
			});
			List<ClientInfo> obj = this.dropClientsQueue;
			lock (obj)
			{
				if (!this.dropClientsQueue.Contains(_cInfo))
				{
					this.dropClientsQueue.Add(_cInfo);
				}
			}
			return NetworkError.WrongConnection;
		}
		_data[0] = (byte)_channel;
		if (ConnectionManager.VerboseNetLogging)
		{
			Log.Out("Sending data to peer {2}: ch={0}, size={1}", new object[]
			{
				_channel,
				_data.Count,
				_cInfo.InternalId.CombinedString
			});
		}
		peerByConnectId.Send(_data.Items, 0, _data.Count, _reliableDelivery ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable);
		return NetworkError.Ok;
	}

	// Token: 0x0600399A RID: 14746 RVA: 0x00173FD4 File Offset: 0x001721D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void PeerConnectedEvent(NetPeer _peer)
	{
		Log.Out(string.Format("NET: LiteNetLib: Connect from: {0} / {1}", _peer.EndPoint, _peer.Id));
		ClientInfo clientInfo = new ClientInfo
		{
			litenetPeerConnectId = (long)_peer.Id,
			network = this,
			netConnection = new INetConnection[2]
		};
		for (int i = 0; i < 2; i++)
		{
			clientInfo.netConnection[i] = new NetConnectionSimple(i, clientInfo, null, _peer.Id.ToString(), 1, 0);
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.AddClient(clientInfo);
		SingletonMonoBehaviour<ConnectionManager>.Instance.Net_PlayerConnected(clientInfo);
	}

	// Token: 0x0600399B RID: 14747 RVA: 0x0017406C File Offset: 0x0017226C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PeerDisconnectedEvent(NetPeer _peer, DisconnectInfo _info)
	{
		Log.Out(string.Format("NET: LiteNetLib: Client disconnect from: {0} / {1} ({2})", _peer.EndPoint, _peer.Id, _info.Reason.ToStringCached<DisconnectReason>()));
		if (_info.Reason == DisconnectReason.Timeout)
		{
			Log.Out(string.Format("NET: LiteNetLib: TimeSinceLastPacket: {0}", _peer.TimeSinceLastPacket));
		}
		ThreadManager.AddSingleTaskMainThread("PlayerDisconnectLiteNetLib", delegate(object _)
		{
			Log.Out(string.Format("NET: LiteNetLib: MT: Client disconnect from: {0} / {1} ({2})", _peer.EndPoint, _peer.Id, _info.Reason.ToStringCached<DisconnectReason>()));
			this.OnPlayerDisconnected((long)_peer.Id);
		}, null);
	}

	// Token: 0x0600399C RID: 14748 RVA: 0x00174114 File Offset: 0x00172314
	[PublicizedFrom(EAccessModifier.Private)]
	public void NetworkReceiveEvent(NetPeer _peer, NetPacketReader _reader, DeliveryMethod _deliveryMethod)
	{
		ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForLiteNetPeer((long)_peer.Id);
		if (clientInfo == null)
		{
			string str = "NET: LiteNetLib: Received package from an unknown client: ";
			IPEndPoint endPoint = _peer.EndPoint;
			Log.Out(str + ((endPoint != null) ? endPoint.ToString() : null));
			return;
		}
		if (_reader.AvailableBytes == 0)
		{
			string str2 = "NET: LiteNetLib: Received package with zero size from: ";
			ClientInfo clientInfo2 = clientInfo;
			Log.Out(str2 + ((clientInfo2 != null) ? clientInfo2.ToString() : null));
			return;
		}
		int availableBytes = _reader.AvailableBytes;
		byte[] array = MemoryPools.poolByte.Alloc(availableBytes);
		_reader.GetBytes(array, availableBytes);
		if (ConnectionManager.VerboseNetLogging)
		{
			Log.Out("Received data from peer {2}: ch={0}, size={1}", new object[]
			{
				array[0],
				availableBytes,
				clientInfo.InternalId.CombinedString
			});
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.Net_DataReceivedServer(clientInfo, (int)array[0], array, availableBytes);
	}

	// Token: 0x0600399D RID: 14749 RVA: 0x001741E8 File Offset: 0x001723E8
	public string GetIP(ClientInfo _cInfo)
	{
		NetPeer peerByConnectId = this.GetPeerByConnectId(_cInfo.litenetPeerConnectId);
		if (peerByConnectId == null)
		{
			Log.Warning("NET: LiteNetLib: IP requested for unknown client {0}", new object[]
			{
				_cInfo.ToString()
			});
			return string.Empty;
		}
		return peerByConnectId.EndPoint.Address.ToString();
	}

	// Token: 0x0600399E RID: 14750 RVA: 0x00174234 File Offset: 0x00172434
	public int GetPing(ClientInfo _cInfo)
	{
		NetPeer peerByConnectId = this.GetPeerByConnectId(_cInfo.litenetPeerConnectId);
		if (peerByConnectId == null)
		{
			Log.Warning("NET: LiteNetLib: Ping requested for unknown client {0}", new object[]
			{
				_cInfo.ToString()
			});
			return -1;
		}
		return peerByConnectId.Ping;
	}

	// Token: 0x0600399F RID: 14751 RVA: 0x00174274 File Offset: 0x00172474
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPlayerDisconnected(long _peerConnectId)
	{
		ClientInfo clientInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForLiteNetPeer(_peerConnectId);
		if (clientInfo != null)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.Net_PlayerDisconnected(clientInfo);
		}
	}

	// Token: 0x060039A0 RID: 14752 RVA: 0x001742A0 File Offset: 0x001724A0
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPeer GetPeerByConnectId(long _connectId)
	{
		if (this.server == null)
		{
			return null;
		}
		List<NetPeer> obj = this.getPeersList;
		lock (obj)
		{
			this.server.GetPeersNonAlloc(this.getPeersList, ConnectionState.Any);
			for (int i = 0; i < this.getPeersList.Count; i++)
			{
				if ((long)this.getPeersList[i].Id == _connectId)
				{
					return this.getPeersList[i];
				}
			}
		}
		return null;
	}

	// Token: 0x060039A1 RID: 14753 RVA: 0x00174334 File Offset: 0x00172534
	public int GetBadPacketCount(ClientInfo _cInfo)
	{
		NetPeer peerByConnectId = this.GetPeerByConnectId(_cInfo.litenetPeerConnectId);
		if (peerByConnectId != null)
		{
			return peerByConnectId.badPacketCount;
		}
		return 0;
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x0017435C File Offset: 0x0017255C
	public string GetServerPorts(int _basePort)
	{
		return (_basePort + 2).ToString() + "/UDP";
	}

	// Token: 0x060039A3 RID: 14755 RVA: 0x0017437E File Offset: 0x0017257E
	public void SetLatencySimulation(bool _enable, int _minLatency, int _maxLatency)
	{
		if (this.server != null)
		{
			this.server.SimulateLatency = _enable;
			this.server.SimulationMinLatency = _minLatency;
			this.server.SimulationMaxLatency = _maxLatency;
		}
	}

	// Token: 0x060039A4 RID: 14756 RVA: 0x001743AC File Offset: 0x001725AC
	public void SetPacketLossSimulation(bool _enable, int _chance)
	{
		if (this.server != null)
		{
			this.server.SimulatePacketLoss = _enable;
			this.server.SimulationPacketLossChance = _chance;
		}
	}

	// Token: 0x060039A5 RID: 14757 RVA: 0x001743CE File Offset: 0x001725CE
	public void EnableStatistics()
	{
		if (this.server != null)
		{
			this.server.EnableStatistics = true;
		}
	}

	// Token: 0x060039A6 RID: 14758 RVA: 0x001743E4 File Offset: 0x001725E4
	public void DisableStatistics()
	{
		if (this.server != null)
		{
			this.server.EnableStatistics = false;
		}
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x001743FA File Offset: 0x001725FA
	public string PrintNetworkStatistics()
	{
		if (this.server != null)
		{
			return this.server.Statistics.ToString();
		}
		return "no server!";
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x0017441A File Offset: 0x0017261A
	public void ResetNetworkStatistics()
	{
		if (this.server != null)
		{
			this.server.Statistics.Reset();
		}
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x00174434 File Offset: 0x00172634
	public int GetMaximumPacketSize(ClientInfo _cInfo, bool _reliable = false)
	{
		int result = -1;
		NetPeer peerByConnectId = this.GetPeerByConnectId(_cInfo.litenetPeerConnectId);
		if (peerByConnectId != null)
		{
			result = peerByConnectId.GetMaxSinglePacketSize(_reliable ? DeliveryMethod.ReliableOrdered : DeliveryMethod.Unreliable);
		}
		return result;
	}

	// Token: 0x060039AA RID: 14762 RVA: 0x00174464 File Offset: 0x00172664
	// Note: this type is marked as 'beforefieldinit'.
	[PublicizedFrom(EAccessModifier.Private)]
	static NetworkServerLiteNetLib()
	{
		byte[] array = new byte[2];
		array[0] = 1;
		NetworkServerLiteNetLib.rejectRateLimit = array;
		byte[] array2 = new byte[2];
		array2[0] = 2;
		NetworkServerLiteNetLib.rejectPendingConnection = array2;
		byte[] array3 = new byte[2];
		array3[0] = 3;
		NetworkServerLiteNetLib.disconnectServerShutdown = array3;
		byte[] array4 = new byte[2];
		array4[0] = 4;
		NetworkServerLiteNetLib.disconnectFromClientSide = array4;
	}

	// Token: 0x04002E86 RID: 11910
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly byte[] rejectInvalidPassword = new byte[2];

	// Token: 0x04002E87 RID: 11911
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly byte[] rejectRateLimit;

	// Token: 0x04002E88 RID: 11912
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly byte[] rejectPendingConnection;

	// Token: 0x04002E89 RID: 11913
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly byte[] disconnectServerShutdown;

	// Token: 0x04002E8A RID: 11914
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly byte[] disconnectFromClientSide;

	// Token: 0x04002E8B RID: 11915
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly IProtocolManagerProtocolInterface protoManager;

	// Token: 0x04002E8C RID: 11916
	[PublicizedFrom(EAccessModifier.Private)]
	public string serverPassword;

	// Token: 0x04002E8D RID: 11917
	[PublicizedFrom(EAccessModifier.Private)]
	public NetManager server;

	// Token: 0x04002E8E RID: 11918
	[PublicizedFrom(EAccessModifier.Private)]
	public NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer authWrapper;

	// Token: 0x04002E8F RID: 11919
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<ClientInfo> dropClientsQueue = new List<ClientInfo>();

	// Token: 0x04002E90 RID: 11920
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<NetPeer> getPeersList = new List<NetPeer>();

	// Token: 0x020007CE RID: 1998
	[PublicizedFrom(EAccessModifier.Private)]
	public class LiteNetLibAuthWrapperServer
	{
		// Token: 0x060039AB RID: 14763 RVA: 0x001744B8 File Offset: 0x001726B8
		public LiteNetLibAuthWrapperServer(NetworkServerLiteNetLib _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x060039AC RID: 14764 RVA: 0x001744E8 File Offset: 0x001726E8
		public void Update()
		{
			DateTime now = DateTime.Now;
			if (now < this.nextStateChecksRun)
			{
				return;
			}
			this.nextStateChecksRun = now + NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.ConnectionStateCheckInterval;
			Dictionary<int, NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState> obj = this.authStates;
			lock (obj)
			{
				this.clearList.Clear();
				foreach (KeyValuePair<int, NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState> keyValuePair in this.authStates)
				{
					int num;
					NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState lnlauthConnectionState;
					keyValuePair.Deconstruct(out num, out lnlauthConnectionState);
					int item = num;
					NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState lnlauthConnectionState2 = lnlauthConnectionState;
					if (lnlauthConnectionState2.State == NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState.EState.Authenticating && !(lnlauthConnectionState2.Age <= NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.MaxDurationInAuthState))
					{
						this.clearList.Add(item);
					}
				}
				foreach (int key in this.clearList)
				{
					this.authStates.Remove(key);
				}
			}
		}

		// Token: 0x060039AD RID: 14765 RVA: 0x00174614 File Offset: 0x00172814
		public void ConnectionRequestCheck(ConnectionRequest _request)
		{
			string text = _request.RemoteEndPoint.Address.ToString();
			DateTime now = DateTime.Now;
			DateTime d;
			this.lastConnectAttemptTimes.TryGetValue(text, out d);
			TimeSpan timeSpan = now - d;
			this.lastConnectAttemptTimes[text] = now;
			if (timeSpan.TotalMilliseconds < 500.0)
			{
				Log.Out("NET: Rejecting connection request from " + text + ": Limiting connect rate from that IP!");
				_request.Reject(NetworkServerLiteNetLib.rejectRateLimit);
				return;
			}
			foreach (ClientInfo clientInfo in SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.List)
			{
				if (!clientInfo.loginDone && clientInfo.ip == text)
				{
					Log.Out("NET: Rejecting connection request from " + text + ": A connection attempt from that IP is currently being processed!");
					_request.Reject(NetworkServerLiteNetLib.rejectPendingConnection);
					return;
				}
			}
			if (_request.Data.GetString() != this.owner.serverPassword)
			{
				_request.Reject(NetworkServerLiteNetLib.rejectInvalidPassword);
				return;
			}
			_request.Accept();
		}

		// Token: 0x060039AE RID: 14766 RVA: 0x00174744 File Offset: 0x00172944
		public void OnPeerConnectedEvent(NetPeer _peer)
		{
			Dictionary<int, NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState> obj = this.authStates;
			NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState lnlauthConnectionState;
			lock (obj)
			{
				if (this.authStates.TryGetValue(_peer.Id, out lnlauthConnectionState) && ConnectionManager.VerboseNetLogging)
				{
					Log.Out(string.Format("NET: LiteNetLib: New peer {0} with previously used peer id {1}", _peer.EndPoint, _peer.Id));
				}
				lnlauthConnectionState = new NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState();
				this.authStates[_peer.Id] = lnlauthConnectionState;
			}
			byte[] array = new byte[17];
			array[0] = 202;
			lnlauthConnectionState.Challenge.WriteToBuffer(array, 1);
			if (ConnectionManager.VerboseNetLogging)
			{
				Log.Out(string.Format("NET: LiteNetLib: Sending challenge {0} to new peer {1} / {2}", lnlauthConnectionState.Challenge, _peer.EndPoint, _peer.Id));
			}
			_peer.Send(array, 0, array.Length, DeliveryMethod.ReliableOrdered);
		}

		// Token: 0x060039AF RID: 14767 RVA: 0x00174830 File Offset: 0x00172A30
		public void OnPeerDisconnectedEvent(NetPeer _peer, DisconnectInfo _disconnectInfo)
		{
			Dictionary<int, NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState> obj = this.authStates;
			NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState lnlauthConnectionState;
			lock (obj)
			{
				if (!this.authStates.TryGetValue(_peer.Id, out lnlauthConnectionState))
				{
					return;
				}
			}
			obj = this.authStates;
			lock (obj)
			{
				this.authStates.Remove(_peer.Id);
			}
			if (lnlauthConnectionState.State == NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState.EState.Authenticating)
			{
				Log.Out(string.Format("NET: LiteNetLib: Peer disconnected in auth state: {0} / {1}", _peer.EndPoint, _peer.Id));
				return;
			}
			this.owner.PeerDisconnectedEvent(_peer, _disconnectInfo);
		}

		// Token: 0x060039B0 RID: 14768 RVA: 0x001748F4 File Offset: 0x00172AF4
		public void OnNetworkReceiveEvent(NetPeer _peer, NetPacketReader _reader, DeliveryMethod _deliveryMethod)
		{
			Dictionary<int, NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState> obj = this.authStates;
			NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState lnlauthConnectionState;
			lock (obj)
			{
				if (!this.authStates.TryGetValue(_peer.Id, out lnlauthConnectionState))
				{
					return;
				}
			}
			if (lnlauthConnectionState.State == NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState.EState.Authenticating)
			{
				int availableBytes = _reader.AvailableBytes;
				if (availableBytes != 17)
				{
					Log.Out(string.Format("NET: LiteNetLib: Received invalid package (length={0}) from an client in auth state: {1} / {2}", availableBytes, _peer.EndPoint, _peer.Id));
					_peer.Disconnect();
					return;
				}
				byte[] array = MemoryPools.poolByte.Alloc(availableBytes);
				_reader.GetBytes(array, availableBytes);
				if (array[0] != 202)
				{
					Log.Out(string.Format("NET: LiteNetLib: Received invalid package (channel={0}) from an client in auth state: {1} / {2}", array[0], _peer.EndPoint, _peer.Id));
					_peer.Disconnect();
					MemoryPools.poolByte.Free(array);
					return;
				}
				byte[] array2 = new byte[16];
				Array.Copy(array, 1, array2, 0, 16);
				Guid guid = new Guid(array2);
				if (guid != lnlauthConnectionState.Challenge)
				{
					Log.Out(string.Format("NET: LiteNetLib: Received invalid challenge {0} from an client in auth state: {1} / {2}", guid, _peer.EndPoint, _peer.Id));
					_peer.Disconnect();
					MemoryPools.poolByte.Free(array);
					return;
				}
				if (ConnectionManager.VerboseNetLogging)
				{
					Log.Out(string.Format("NET: LiteNetLib: Received correct challenge {0} from new peer {1} / {2}, accepting", guid, _peer.EndPoint, _peer.Id));
				}
				lnlauthConnectionState.State = NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState.EState.Connected;
				this.owner.PeerConnectedEvent(_peer);
				MemoryPools.poolByte.Free(array);
				return;
			}
			else
			{
				this.owner.NetworkReceiveEvent(_peer, _reader, _deliveryMethod);
			}
		}

		// Token: 0x04002E91 RID: 11921
		[PublicizedFrom(EAccessModifier.Private)]
		public const bool Enabled = true;

		// Token: 0x04002E92 RID: 11922
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan ConnectionStateCheckInterval = TimeSpan.FromSeconds(10.0);

		// Token: 0x04002E93 RID: 11923
		[PublicizedFrom(EAccessModifier.Private)]
		public const int ConnectionRateLimitMilliseconds = 500;

		// Token: 0x04002E94 RID: 11924
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan MaxDurationInAuthState = new TimeSpan(0, 0, 0, 10);

		// Token: 0x04002E95 RID: 11925
		[PublicizedFrom(EAccessModifier.Private)]
		public const int ChallengePackageSize = 17;

		// Token: 0x04002E96 RID: 11926
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly NetworkServerLiteNetLib owner;

		// Token: 0x04002E97 RID: 11927
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, DateTime> lastConnectAttemptTimes = new Dictionary<string, DateTime>();

		// Token: 0x04002E98 RID: 11928
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<int, NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState> authStates = new Dictionary<int, NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState>();

		// Token: 0x04002E99 RID: 11929
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<int> clearList = new List<int>();

		// Token: 0x04002E9A RID: 11930
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime nextStateChecksRun;

		// Token: 0x020007CF RID: 1999
		[PublicizedFrom(EAccessModifier.Private)]
		public class LNLAuthConnectionState
		{
			// Token: 0x170005CF RID: 1487
			// (get) Token: 0x060039B2 RID: 14770 RVA: 0x00174AD4 File Offset: 0x00172CD4
			public TimeSpan Age
			{
				get
				{
					return DateTime.Now - this.StartTime;
				}
			}

			// Token: 0x04002E9B RID: 11931
			public NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer.LNLAuthConnectionState.EState State;

			// Token: 0x04002E9C RID: 11932
			public readonly Guid Challenge = Guid.NewGuid();

			// Token: 0x04002E9D RID: 11933
			public readonly DateTime StartTime = DateTime.Now;

			// Token: 0x020007D0 RID: 2000
			public enum EState
			{
				// Token: 0x04002E9F RID: 11935
				Authenticating,
				// Token: 0x04002EA0 RID: 11936
				Connected
			}
		}
	}
}
