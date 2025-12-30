using System;
using System.Net;
using System.Net.Sockets;

namespace Platform.LAN
{
	// Token: 0x020018FD RID: 6397
	public class UdpClientReceiveHandler
	{
		// Token: 0x0600BCF6 RID: 48374 RVA: 0x00479198 File Offset: 0x00477398
		public UdpClientReceiveHandler(UdpClient _udpClient)
		{
			this.udpClient = _udpClient;
		}

		// Token: 0x0600BCF7 RID: 48375 RVA: 0x004791A8 File Offset: 0x004773A8
		public bool BeginReceive()
		{
			this.remoteEP = null;
			this.message = null;
			try
			{
				this.isComplete = false;
				this.udpClient.BeginReceive(new AsyncCallback(UdpClientReceiveHandler.CompleteReceiveAsync), this);
				return true;
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN receive handler unable to start receive. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.isComplete = true;
			return false;
		}

		// Token: 0x0600BCF8 RID: 48376 RVA: 0x00479238 File Offset: 0x00477438
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CompleteReceiveAsync(IAsyncResult _result)
		{
			((UdpClientReceiveHandler)_result.AsyncState).CompleteReceive(_result);
		}

		// Token: 0x0600BCF9 RID: 48377 RVA: 0x0047924C File Offset: 0x0047744C
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteReceive(IAsyncResult _result)
		{
			try
			{
				this.message = this.udpClient.EndReceive(_result, ref this.remoteEP);
				this.length = this.message.Length;
				this.isComplete = true;
				return;
			}
			catch (ObjectDisposedException)
			{
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN receive handler unable to complete receive. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.remoteEP = null;
			this.message = null;
			this.length = 0;
			this.isComplete = true;
		}

		// Token: 0x04009330 RID: 37680
		public readonly UdpClient udpClient;

		// Token: 0x04009331 RID: 37681
		public IPEndPoint remoteEP;

		// Token: 0x04009332 RID: 37682
		public byte[] message;

		// Token: 0x04009333 RID: 37683
		public int length;

		// Token: 0x04009334 RID: 37684
		public bool isComplete;
	}
}
