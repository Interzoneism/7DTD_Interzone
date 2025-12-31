using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Platform.LAN
{
	// Token: 0x020018F3 RID: 6387
	public class LANMasterServerAnnouncer : IMasterServerAnnouncer
	{
		// Token: 0x170015AC RID: 5548
		// (get) Token: 0x0600BCC0 RID: 48320 RVA: 0x000197A5 File Offset: 0x000179A5
		public bool GameServerInitialized
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600BCC1 RID: 48321 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600BCC2 RID: 48322 RVA: 0x00047178 File Offset: 0x00045378
		public string GetServerPorts()
		{
			return string.Empty;
		}

		// Token: 0x0600BCC3 RID: 48323 RVA: 0x004787F0 File Offset: 0x004769F0
		public void AdvertiseServer(Action _onServerRegistered)
		{
			int num = 11000;
			IPAddress multicastGroupIp = LANServerSearchConfig.MulticastGroupIp;
			try
			{
				this.udpClient = new UdpClient(num);
				this.udpClient.JoinMulticastGroup(multicastGroupIp);
				this.receiveHandler = new UdpClientReceiveHandler(this.udpClient);
				this.shouldAnnounce = true;
				this.replyCoroutine = ThreadManager.StartCoroutine(this.LANServerListReplyTask());
				Log.Out(string.Format("[{0}] listening on {1} and multicast group {2}", "LANMasterServerAnnouncer", num, multicastGroupIp));
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("[{0}] could not start LAN server search listening on port {1} and multicast group {2}. ErrorCode: {3}, Message: {4}", new object[]
				{
					"LANMasterServerAnnouncer",
					num,
					multicastGroupIp,
					ex.ErrorCode,
					ex.Message
				}));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			_onServerRegistered();
		}

		// Token: 0x0600BCC4 RID: 48324 RVA: 0x004788D4 File Offset: 0x00476AD4
		public IEnumerator LANServerListReplyTask()
		{
			while (this.shouldAnnounce)
			{
				if (!this.receiveHandler.BeginReceive())
				{
					Log.Error("[LANMasterServerAnnouncer] could not start receive");
					yield break;
				}
				while (!this.receiveHandler.isComplete)
				{
					yield return null;
				}
				IPEndPoint remoteEP = this.receiveHandler.remoteEP;
				byte[] message = this.receiveHandler.message;
				int length = this.receiveHandler.length;
				if (remoteEP != null && message != null && length == 0)
				{
					this.SendReply(remoteEP);
				}
			}
			yield break;
		}

		// Token: 0x0600BCC5 RID: 48325 RVA: 0x004788E4 File Offset: 0x00476AE4
		[PublicizedFrom(EAccessModifier.Private)]
		public void SendReply(IPEndPoint _remoteEP)
		{
			try
			{
				int value = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.GetValue(GameInfoInt.Port);
				int size = 0;
				StreamUtils.Write(LANMasterServerAnnouncer.replyBuffer, value, ref size);
				this.udpClient.Client.SendTo(LANMasterServerAnnouncer.replyBuffer, 0, size, SocketFlags.None, _remoteEP);
			}
			catch (Exception e)
			{
				Log.Error(string.Format("[{0}] could not send reply to {1}", "LANMasterServerAnnouncer", _remoteEP));
				Log.Exception(e);
			}
		}

		// Token: 0x0600BCC6 RID: 48326 RVA: 0x00002914 File Offset: 0x00000B14
		public void Update()
		{
		}

		// Token: 0x0600BCC7 RID: 48327 RVA: 0x0047895C File Offset: 0x00476B5C
		public void StopServer()
		{
			this.shouldAnnounce = false;
			if (this.replyCoroutine != null)
			{
				ThreadManager.StopCoroutine(this.replyCoroutine);
			}
			UdpClient udpClient = this.udpClient;
			if (udpClient != null)
			{
				udpClient.Dispose();
			}
			this.udpClient = null;
			this.receiveHandler = null;
		}

		// Token: 0x04009307 RID: 37639
		[PublicizedFrom(EAccessModifier.Private)]
		public bool shouldAnnounce = true;

		// Token: 0x04009308 RID: 37640
		[PublicizedFrom(EAccessModifier.Private)]
		public UdpClient udpClient;

		// Token: 0x04009309 RID: 37641
		[PublicizedFrom(EAccessModifier.Private)]
		public UdpClientReceiveHandler receiveHandler;

		// Token: 0x0400930A RID: 37642
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine replyCoroutine;

		// Token: 0x0400930B RID: 37643
		[PublicizedFrom(EAccessModifier.Private)]
		public static byte[] emptyMessage = new byte[0];

		// Token: 0x0400930C RID: 37644
		[PublicizedFrom(EAccessModifier.Private)]
		public static byte[] replyBuffer = new byte[4];
	}
}
