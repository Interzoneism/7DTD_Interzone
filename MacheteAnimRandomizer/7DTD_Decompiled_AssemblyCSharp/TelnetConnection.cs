using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class TelnetConnection : ConsoleConnectionAbstract
{
	// Token: 0x170001F2 RID: 498
	// (get) Token: 0x06001266 RID: 4710 RVA: 0x0007285F File Offset: 0x00070A5F
	public bool IsClosed
	{
		get
		{
			return this.closed;
		}
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x06001267 RID: 4711 RVA: 0x00072869 File Offset: 0x00070A69
	public bool IsAuthenticated
	{
		get
		{
			return !this.authEnabled || this.authenticated;
		}
	}

	// Token: 0x170001F4 RID: 500
	// (get) Token: 0x06001268 RID: 4712 RVA: 0x0007287B File Offset: 0x00070A7B
	public int EndPointHash { get; }

	// Token: 0x06001269 RID: 4713 RVA: 0x00072884 File Offset: 0x00070A84
	public TelnetConnection(TelnetConsole _owner, TcpClient _client, int _addressHash, bool _authEnabled)
	{
		this.telnet = _owner;
		this.authEnabled = _authEnabled;
		this.client = _client;
		this.endpoint = _client.Client.RemoteEndPoint;
		this.EndPointHash = _addressHash;
		string str = "Telnet connection from: ";
		EndPoint endPoint = this.endpoint;
		Log.Out(str + ((endPoint != null) ? endPoint.ToString() : null));
		this.clientStream = _client.GetStream();
		this.reader = new StreamReader(this.clientStream, Encoding.UTF8);
		string str2 = "TelnetClient_";
		EndPoint endPoint2 = this.endpoint;
		ThreadManager.StartThread(str2 + ((endPoint2 != null) ? endPoint2.ToString() : null), null, new ThreadManager.ThreadFunctionLoopDelegate(this.HandlerThread), new ThreadManager.ThreadFunctionEndDelegate(this.ThreadEnd), null, null, false, false);
		if (_authEnabled)
		{
			this.toClientQueue.Enqueue("Please enter password:");
			return;
		}
		this.LoginMessage();
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x00072998 File Offset: 0x00070B98
	[PublicizedFrom(EAccessModifier.Private)]
	public void LoginMessage()
	{
		this.toClientQueue.Enqueue("*** Connected with 7DTD server.");
		this.toClientQueue.Enqueue("*** Server version: " + Constants.cVersionInformation.LongString + " Compatibility Version: " + Constants.cVersionInformation.LongStringNoBuild);
		this.toClientQueue.Enqueue(string.Empty);
		this.toClientQueue.Enqueue("Server IP:   " + (string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.ServerIP)) ? "Any" : GamePrefs.GetString(EnumGamePrefs.ServerIP)));
		this.toClientQueue.Enqueue("Server port: " + GamePrefs.GetInt(EnumGamePrefs.ServerPort).ToString());
		this.toClientQueue.Enqueue("Max players: " + GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount).ToString());
		this.toClientQueue.Enqueue("Game mode:   " + GamePrefs.GetString(EnumGamePrefs.GameMode));
		this.toClientQueue.Enqueue("World:       " + GamePrefs.GetString(EnumGamePrefs.GameWorld));
		this.toClientQueue.Enqueue("Game name:   " + GamePrefs.GetString(EnumGamePrefs.GameName));
		this.toClientQueue.Enqueue("Difficulty:  " + GamePrefs.GetInt(EnumGamePrefs.GameDifficulty).ToString());
		this.toClientQueue.Enqueue(string.Empty);
		this.toClientQueue.Enqueue("Press 'help' to get a list of all commands. Press 'exit' to end session.");
		this.toClientQueue.Enqueue(string.Empty);
	}

	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x0600126B RID: 4715 RVA: 0x00072B14 File Offset: 0x00070D14
	public bool ConnectionUsable
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.client.Connected && !this.closed;
		}
	}

	// Token: 0x0600126C RID: 4716 RVA: 0x00072B30 File Offset: 0x00070D30
	[PublicizedFrom(EAccessModifier.Private)]
	public int HandlerThread(ThreadManager.ThreadInfo _tInfo)
	{
		if (!this.ConnectionUsable || this.closeConnection)
		{
			return -1;
		}
		try
		{
			if (!this.handleReading())
			{
				return -1;
			}
			this.handleWriting();
		}
		catch (IOException ex)
		{
			SocketException ex2 = ex.InnerException as SocketException;
			if (ex2 != null && ex2.SocketErrorCode == SocketError.ConnectionAborted)
			{
				string str = "Connection closed by host in TelnetClient_";
				EndPoint endPoint = this.endpoint;
				Log.Warning(str + ((endPoint != null) ? endPoint.ToString() : null));
				return -1;
			}
			string str2 = "IOException in TelnetClient_";
			EndPoint endPoint2 = this.endpoint;
			Log.Error(str2 + ((endPoint2 != null) ? endPoint2.ToString() : null) + ": " + ex.Message);
			Log.Exception(ex);
			return -1;
		}
		return 25;
	}

	// Token: 0x0600126D RID: 4717 RVA: 0x00072BF4 File Offset: 0x00070DF4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool handleReading()
	{
		int num;
		while (this.ConnectionUsable && this.clientStream.CanRead && this.client.Available > 0 && (num = this.reader.Read(this.charBuffer, 0, this.charBuffer.Length)) > 0)
		{
			for (int i = 0; i < num; i++)
			{
				char c = this.charBuffer[i];
				if (c == '\r' || c == '\n')
				{
					if (!this.submitInput())
					{
						return false;
					}
				}
				else
				{
					this.readStringBuilder.Append(c);
				}
			}
		}
		return true;
	}

	// Token: 0x0600126E RID: 4718 RVA: 0x00072C7C File Offset: 0x00070E7C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool submitInput()
	{
		this.readStringBuilder.Trim();
		if (this.readStringBuilder.Length <= 0)
		{
			return true;
		}
		string text = this.readStringBuilder.ToString();
		if (!this.IsAuthenticated)
		{
			this.authenticate(text);
		}
		else
		{
			if (text.EqualsCaseInsensitive("exit"))
			{
				return false;
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteAsync(text, this);
		}
		this.readStringBuilder.Length = 0;
		return true;
	}

	// Token: 0x0600126F RID: 4719 RVA: 0x00072CEC File Offset: 0x00070EEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void authenticate(string _line)
	{
		if (_line.Equals(GamePrefs.GetString(EnumGamePrefs.TelnetPassword)))
		{
			this.authenticated = true;
			this.toClientQueue.Enqueue("Logon successful.");
			this.toClientQueue.Enqueue(string.Empty);
			this.toClientQueue.Enqueue(string.Empty);
			this.toClientQueue.Enqueue(string.Empty);
			this.LoginMessage();
			return;
		}
		if (this.telnet.RegisterFailedLogin(this))
		{
			this.toClientQueue.Enqueue("Password incorrect, please enter password:");
			return;
		}
		this.toClientQueue.Enqueue("Too many failed login attempts!");
		this.closeConnection = true;
	}

	// Token: 0x06001270 RID: 4720 RVA: 0x00072D8C File Offset: 0x00070F8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void handleWriting()
	{
		while (this.ConnectionUsable && this.clientStream.CanWrite && this.toClientQueue.HasData())
		{
			string text = this.toClientQueue.Dequeue();
			if (text == null)
			{
				this.clientStream.WriteByte(0);
			}
			else
			{
				int num;
				for (int i = 0; i < text.Length; i += num)
				{
					num = Math.Min(64, text.Length - i);
					int bytes = Encoding.UTF8.GetBytes(text, i, num, this.byteBuffer, 0);
					this.clientStream.Write(this.byteBuffer, 0, bytes);
				}
				this.clientStream.WriteByte(13);
				this.clientStream.WriteByte(10);
			}
		}
	}

	// Token: 0x06001271 RID: 4721 RVA: 0x00072E43 File Offset: 0x00071043
	[PublicizedFrom(EAccessModifier.Private)]
	public void ThreadEnd(ThreadManager.ThreadInfo _threadInfo, bool _exitForException)
	{
		this.Close(false);
	}

	// Token: 0x06001272 RID: 4722 RVA: 0x00072E4C File Offset: 0x0007104C
	public void Close(bool _kickedForLogins = false)
	{
		if (this.closed)
		{
			return;
		}
		this.closed = true;
		this.toClientQueue.Close();
		if (this.client.Connected)
		{
			this.client.GetStream().Close();
			this.client.Close();
		}
		this.telnet.ConnectionClosed(this);
		if (_kickedForLogins)
		{
			string str = "Telnet connection closed for too many login attempts: ";
			EndPoint endPoint = this.endpoint;
			Log.Out(str + ((endPoint != null) ? endPoint.ToString() : null));
			return;
		}
		string str2 = "Telnet connection closed: ";
		EndPoint endPoint2 = this.endpoint;
		Log.Out(str2 + ((endPoint2 != null) ? endPoint2.ToString() : null));
	}

	// Token: 0x06001273 RID: 4723 RVA: 0x00072EF2 File Offset: 0x000710F2
	public override void SendLine(string _line)
	{
		if (!this.closed && this.IsAuthenticated)
		{
			this.toClientQueue.Enqueue(_line);
			return;
		}
		this.toClientQueue.Enqueue(null);
	}

	// Token: 0x06001274 RID: 4724 RVA: 0x00072F20 File Offset: 0x00071120
	public override void SendLines(List<string> _output)
	{
		for (int i = 0; i < _output.Count; i++)
		{
			this.SendLine(_output[i]);
		}
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x00072F4B File Offset: 0x0007114B
	public override void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
	{
		if (base.IsLogLevelEnabled(_type))
		{
			this.SendLine(_formattedMessage);
		}
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x00072F60 File Offset: 0x00071160
	public override string GetDescription()
	{
		string result;
		if ((result = this.cachedDescription) == null)
		{
			string str = "Telnet from ";
			EndPoint endPoint = this.endpoint;
			result = (this.cachedDescription = str + ((endPoint != null) ? endPoint.ToString() : null));
		}
		return result;
	}

	// Token: 0x04000BF0 RID: 3056
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MAX_CHARS_PER_CONVERSION = 64;

	// Token: 0x04000BF1 RID: 3057
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly BlockingQueue<string> toClientQueue = new BlockingQueue<string>();

	// Token: 0x04000BF2 RID: 3058
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly TelnetConsole telnet;

	// Token: 0x04000BF3 RID: 3059
	[PublicizedFrom(EAccessModifier.Private)]
	public bool authenticated;

	// Token: 0x04000BF4 RID: 3060
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool authEnabled;

	// Token: 0x04000BF5 RID: 3061
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly TcpClient client;

	// Token: 0x04000BF6 RID: 3062
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly EndPoint endpoint;

	// Token: 0x04000BF7 RID: 3063
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile bool closed;

	// Token: 0x04000BF8 RID: 3064
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedDescription;

	// Token: 0x04000BF9 RID: 3065
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly NetworkStream clientStream;

	// Token: 0x04000BFA RID: 3066
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly StreamReader reader;

	// Token: 0x04000BFB RID: 3067
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly StringBuilder readStringBuilder = new StringBuilder();

	// Token: 0x04000BFC RID: 3068
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly char[] charBuffer = new char[64];

	// Token: 0x04000BFD RID: 3069
	[PublicizedFrom(EAccessModifier.Private)]
	public bool closeConnection;

	// Token: 0x04000BFE RID: 3070
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly byte[] byteBuffer = new byte[256];
}
