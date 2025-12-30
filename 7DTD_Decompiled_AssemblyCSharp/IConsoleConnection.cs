using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000284 RID: 644
public interface IConsoleConnection
{
	// Token: 0x0600124A RID: 4682
	void SendLines(List<string> _output);

	// Token: 0x0600124B RID: 4683
	void SendLine(string _text);

	// Token: 0x0600124C RID: 4684
	void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime);

	// Token: 0x0600124D RID: 4685
	void EnableLogLevel(LogType _type, bool _enable);

	// Token: 0x0600124E RID: 4686
	string GetDescription();
}
