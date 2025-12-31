using System;
using UnityEngine;

// Token: 0x02000285 RID: 645
public interface IConsoleServer
{
	// Token: 0x0600124F RID: 4687
	void Disconnect();

	// Token: 0x06001250 RID: 4688
	void SendLine(string _line);

	// Token: 0x06001251 RID: 4689
	void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime);
}
