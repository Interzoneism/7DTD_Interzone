using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class TelnetConsole : IConsoleServer
{
	// Token: 0x06001277 RID: 4727 RVA: 0x00072F9C File Offset: 0x0007119C
	public TelnetConsole()
	{
		try
		{
			int @int = GamePrefs.GetInt(EnumGamePrefs.TelnetPort);
			this.authEnabled = (GamePrefs.GetString(EnumGamePrefs.TelnetPassword).Length != 0);
			this.listener = new TcpListener(this.authEnabled ? IPAddress.Any : IPAddress.Loopback, @int);
			TelnetConsole.maxLoginAttempts = GamePrefs.GetInt(EnumGamePrefs.TelnetFailedLoginLimit);
			TelnetConsole.blockTimeSeconds = GamePrefs.GetInt(EnumGamePrefs.TelnetFailedLoginsBlocktime);
			this.listener.Start();
			this.listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptClient), null);
			Log.Out("Started Telnet on " + @int.ToString());
		}
		catch (Exception ex)
		{
			string str = "Error in Telnet.ctor: ";
			Exception ex2 = ex;
			Log.Out(str + ((ex2 != null) ? ex2.ToString() : null));
		}
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x0007308C File Offset: 0x0007128C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AcceptClient(IAsyncResult _asyncResult)
	{
		TcpListener tcpListener = this.listener;
		if (((tcpListener != null) ? tcpListener.Server : null) == null || !this.listener.Server.IsBound)
		{
			return;
		}
		TcpClient tcpClient = this.listener.EndAcceptTcpClient(_asyncResult);
		EndPoint remoteEndPoint = tcpClient.Client.RemoteEndPoint;
		IPEndPoint ipendPoint = remoteEndPoint as IPEndPoint;
		int hashCode;
		if (ipendPoint != null)
		{
			hashCode = ipendPoint.Address.GetHashCode();
		}
		else
		{
			hashCode = remoteEndPoint.GetHashCode();
			string str = "EndPoint is not an IPEndPoint but: ";
			Type type = remoteEndPoint.GetType();
			Log.Out(str + ((type != null) ? type.ToString() : null));
		}
		Dictionary<int, TelnetConsole.LoginAttempts> obj = this.loginAttemptsPerIP;
		lock (obj)
		{
			TelnetConsole.LoginAttempts loginAttempts;
			if (!this.loginAttemptsPerIP.TryGetValue(hashCode, out loginAttempts))
			{
				loginAttempts = new TelnetConsole.LoginAttempts();
				this.loginAttemptsPerIP[hashCode] = loginAttempts;
			}
			if (!loginAttempts.IsBanned())
			{
				TelnetConnection item = new TelnetConnection(this, tcpClient, hashCode, this.authEnabled);
				List<TelnetConnection> obj2 = this.connections;
				lock (obj2)
				{
					this.connections.Add(item);
					goto IL_131;
				}
			}
			tcpClient.Close();
			string str2 = "Telnet connection not accepted for too many login attempts: ";
			EndPoint endPoint = remoteEndPoint;
			Log.Out(str2 + ((endPoint != null) ? endPoint.ToString() : null));
		}
		IL_131:
		this.listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptClient), null);
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x00073200 File Offset: 0x00071400
	public bool RegisterFailedLogin(TelnetConnection _con)
	{
		Dictionary<int, TelnetConsole.LoginAttempts> obj = this.loginAttemptsPerIP;
		bool result;
		lock (obj)
		{
			result = this.loginAttemptsPerIP[_con.EndPointHash].LogAttempt();
		}
		return result;
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x00073254 File Offset: 0x00071454
	public void ConnectionClosed(TelnetConnection _con)
	{
		List<TelnetConnection> obj = this.connections;
		lock (obj)
		{
			this.connections.Remove(_con);
		}
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x0007329C File Offset: 0x0007149C
	public void Disconnect()
	{
		try
		{
			if (this.listener != null)
			{
				this.listener.Stop();
				this.listener = null;
			}
			List<TelnetConnection> obj = this.connections;
			List<TelnetConnection> list;
			lock (obj)
			{
				list = new List<TelnetConnection>(this.connections);
			}
			foreach (TelnetConnection telnetConnection in list)
			{
				telnetConnection.Close(false);
			}
		}
		catch (Exception ex)
		{
			string str = "Error in Telnet.Disconnect: ";
			Exception ex2 = ex;
			Log.Out(str + ((ex2 != null) ? ex2.ToString() : null));
		}
	}

	// Token: 0x0600127C RID: 4732 RVA: 0x00073368 File Offset: 0x00071568
	public void SendLine(string _line)
	{
		if (_line == null)
		{
			return;
		}
		List<TelnetConnection> obj = this.connections;
		lock (obj)
		{
			foreach (TelnetConnection telnetConnection in this.connections)
			{
				telnetConnection.SendLine(_line);
			}
		}
	}

	// Token: 0x0600127D RID: 4733 RVA: 0x000733E4 File Offset: 0x000715E4
	public void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
	{
		List<TelnetConnection> obj = this.connections;
		lock (obj)
		{
			foreach (TelnetConnection telnetConnection in this.connections)
			{
				telnetConnection.SendLog(_formattedMessage, _plainMessage, _trace, _type, _timestamp, _uptime);
			}
		}
	}

	// Token: 0x04000C00 RID: 3072
	[PublicizedFrom(EAccessModifier.Private)]
	public static int maxLoginAttempts;

	// Token: 0x04000C01 RID: 3073
	[PublicizedFrom(EAccessModifier.Private)]
	public static int blockTimeSeconds;

	// Token: 0x04000C02 RID: 3074
	[PublicizedFrom(EAccessModifier.Private)]
	public TcpListener listener;

	// Token: 0x04000C03 RID: 3075
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool authEnabled;

	// Token: 0x04000C04 RID: 3076
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<TelnetConnection> connections = new List<TelnetConnection>();

	// Token: 0x04000C05 RID: 3077
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<int, TelnetConsole.LoginAttempts> loginAttemptsPerIP = new Dictionary<int, TelnetConsole.LoginAttempts>();

	// Token: 0x0200028B RID: 651
	[PublicizedFrom(EAccessModifier.Private)]
	public class LoginAttempts
	{
		// Token: 0x0600127E RID: 4734 RVA: 0x00073464 File Offset: 0x00071664
		public bool LogAttempt()
		{
			this.lastAttempt = DateTime.Now;
			this.count++;
			return this.count < TelnetConsole.maxLoginAttempts;
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x0007348C File Offset: 0x0007168C
		public bool IsBanned()
		{
			if ((DateTime.Now - this.lastAttempt).TotalSeconds > (double)TelnetConsole.blockTimeSeconds)
			{
				this.count = 0;
			}
			return this.count >= TelnetConsole.maxLoginAttempts;
		}

		// Token: 0x04000C06 RID: 3078
		[PublicizedFrom(EAccessModifier.Private)]
		public int count;

		// Token: 0x04000C07 RID: 3079
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime lastAttempt = new DateTime(0L);
	}
}
