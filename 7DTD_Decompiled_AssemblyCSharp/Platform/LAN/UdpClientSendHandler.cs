using System;
using System.Net;
using System.Net.Sockets;

namespace Platform.LAN
{
	// Token: 0x020018FC RID: 6396
	public class UdpClientSendHandler
	{
		// Token: 0x0600BCF2 RID: 48370 RVA: 0x00479077 File Offset: 0x00477277
		public UdpClientSendHandler(UdpClient _udpClient)
		{
			this.udpClient = _udpClient;
		}

		// Token: 0x0600BCF3 RID: 48371 RVA: 0x00479088 File Offset: 0x00477288
		public bool BeginSend(byte[] _message, int _length, IPEndPoint _endPoint)
		{
			try
			{
				this.isComplete = false;
				this.udpClient.BeginSend(_message, _length, _endPoint, new AsyncCallback(UdpClientSendHandler.CompleteSendAsync), this);
				return true;
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN send handler unable to start send. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.isComplete = true;
			return false;
		}

		// Token: 0x0600BCF4 RID: 48372 RVA: 0x0047910C File Offset: 0x0047730C
		[PublicizedFrom(EAccessModifier.Private)]
		public static void CompleteSendAsync(IAsyncResult _result)
		{
			((UdpClientSendHandler)_result.AsyncState).CompleteSend(_result);
		}

		// Token: 0x0600BCF5 RID: 48373 RVA: 0x00479120 File Offset: 0x00477320
		[PublicizedFrom(EAccessModifier.Private)]
		public void CompleteSend(IAsyncResult _result)
		{
			try
			{
				this.udpClient.EndSend(_result);
			}
			catch (ObjectDisposedException)
			{
			}
			catch (SocketException ex)
			{
				Log.Warning(string.Format("LAN send handler unable to complete send. {0} ErrorCode: {1}", "SocketException", ex.ErrorCode));
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
			this.isComplete = true;
		}

		// Token: 0x0400932E RID: 37678
		public readonly UdpClient udpClient;

		// Token: 0x0400932F RID: 37679
		public bool isComplete;
	}
}
