using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// Token: 0x020007E5 RID: 2021
public class ServerInformationTcpProvider
{
	// Token: 0x170005DE RID: 1502
	// (get) Token: 0x06003A1E RID: 14878 RVA: 0x00176A0C File Offset: 0x00174C0C
	public static ServerInformationTcpProvider Instance
	{
		get
		{
			ServerInformationTcpProvider result;
			if ((result = ServerInformationTcpProvider.instance) == null)
			{
				result = (ServerInformationTcpProvider.instance = new ServerInformationTcpProvider());
			}
			return result;
		}
	}

	// Token: 0x06003A1F RID: 14879 RVA: 0x00176A22 File Offset: 0x00174C22
	[PublicizedFrom(EAccessModifier.Private)]
	public ServerInformationTcpProvider()
	{
		this.gameInfoProviderCallback = new AsyncCallback(this.AcceptTcpClient);
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x00176A58 File Offset: 0x00174C58
	public void StartServer()
	{
		object obj = this.gameInfoLock;
		lock (obj)
		{
			try
			{
				this.gameInfoProvider = new TcpListener(IPAddress.Any, GamePrefs.GetInt(EnumGamePrefs.ServerPort));
				this.gameInfoProvider.Start();
				this.gameInfoProvider.BeginAcceptTcpClient(this.gameInfoProviderCallback, null);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance != null && SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo != null)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedAny += this.updateServer;
				}
			}
			catch (Exception e)
			{
				Log.Exception(e);
			}
		}
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x00176B10 File Offset: 0x00174D10
	public void StopServer()
	{
		object obj = this.gameInfoLock;
		lock (obj)
		{
			if (this.gameInfoProvider != null)
			{
				if (SingletonMonoBehaviour<ConnectionManager>.Instance != null && SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo != null)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.OnChangedAny -= this.updateServer;
				}
				this.gameInfoProvider.Stop();
				this.bufferLength = 0;
				this.gameInfoProvider = null;
			}
		}
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x00176BA4 File Offset: 0x00174DA4
	public string GetServerPorts()
	{
		return GamePrefs.GetInt(EnumGamePrefs.ServerPort).ToString() + "/TCP";
	}

	// Token: 0x06003A23 RID: 14883 RVA: 0x00176BCC File Offset: 0x00174DCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateServer(GameServerInfo _gameServerInfo)
	{
		object obj = this.gameInfoLock;
		lock (obj)
		{
			if (this.gameInfoProvider != null)
			{
				this.bufferLength = 0;
			}
		}
	}

	// Token: 0x06003A24 RID: 14884 RVA: 0x00176C18 File Offset: 0x00174E18
	[PublicizedFrom(EAccessModifier.Private)]
	public void AcceptTcpClient(IAsyncResult _asyncResult)
	{
		object obj = this.gameInfoLock;
		lock (obj)
		{
			if (this.gameInfoProvider != null && this.gameInfoProvider.Server.IsBound)
			{
				TcpClient tcpClient = null;
				bool flag2 = false;
				try
				{
					tcpClient = this.gameInfoProvider.EndAcceptTcpClient(_asyncResult);
					this.gameInfoProvider.BeginAcceptTcpClient(this.gameInfoProviderCallback, null);
					flag2 = true;
				}
				catch (SocketException arg)
				{
					Log.Warning(string.Format("[NET] Info Provider exception while waiting for a client to connect: {0}", arg));
					return;
				}
				catch (ObjectDisposedException)
				{
					return;
				}
				finally
				{
					if (!flag2 && tcpClient != null)
					{
						tcpClient.Dispose();
					}
				}
				using (TcpClient tcpClient2 = tcpClient)
				{
					tcpClient2.SendTimeout = 50;
					tcpClient2.LingerState = new LingerOption(true, 1);
					NetworkStream stream = tcpClient2.GetStream();
					if (this.bufferLength <= 0)
					{
						string text = SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo.ToString(true);
						try
						{
							int byteCount = Encoding.UTF8.GetByteCount(text);
							if (byteCount > this.buffer.Length)
							{
								Log.Error(string.Format("[NET] Can not provide server information on the info port: Server info size ({0}) exceeds buffer size ({1}), probably due to ServerDescription and/or ServerLoginConfirmationText", byteCount, this.buffer.Length));
							}
							else
							{
								this.bufferLength = Encoding.UTF8.GetBytes(text, 0, text.Length, this.buffer, 0);
							}
						}
						catch (Exception e)
						{
							Log.Error("[NET] Could not provide server information on the info port:");
							Log.Exception(e);
						}
					}
					stream.WriteByte((byte)(this.bufferLength / 10000 % 10 + 48));
					stream.WriteByte((byte)(this.bufferLength / 1000 % 10 + 48));
					stream.WriteByte((byte)(this.bufferLength / 100 % 10 + 48));
					stream.WriteByte((byte)(this.bufferLength / 10 % 10 + 48));
					stream.WriteByte((byte)(this.bufferLength / 1 % 10 + 48));
					stream.WriteByte(13);
					stream.WriteByte(10);
					if (this.bufferLength > 0)
					{
						stream.Write(this.buffer, 0, this.bufferLength);
					}
					stream.Flush();
					stream.Close(100);
					tcpClient2.Close();
				}
			}
		}
	}

	// Token: 0x04002F01 RID: 12033
	public const int BufferSize = 32768;

	// Token: 0x04002F02 RID: 12034
	[PublicizedFrom(EAccessModifier.Private)]
	public static ServerInformationTcpProvider instance;

	// Token: 0x04002F03 RID: 12035
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object gameInfoLock = new object();

	// Token: 0x04002F04 RID: 12036
	[PublicizedFrom(EAccessModifier.Private)]
	public TcpListener gameInfoProvider;

	// Token: 0x04002F05 RID: 12037
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AsyncCallback gameInfoProviderCallback;

	// Token: 0x04002F06 RID: 12038
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly byte[] buffer = new byte[32768];

	// Token: 0x04002F07 RID: 12039
	[PublicizedFrom(EAccessModifier.Private)]
	public int bufferLength;
}
