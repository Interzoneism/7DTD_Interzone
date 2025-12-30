using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000282 RID: 642
public abstract class ConsoleConnectionAbstract : IConsoleConnection
{
	// Token: 0x0600123A RID: 4666
	public abstract void SendLines(List<string> _output);

	// Token: 0x0600123B RID: 4667
	public abstract void SendLine(string _text);

	// Token: 0x0600123C RID: 4668
	public abstract void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime);

	// Token: 0x0600123D RID: 4669
	public abstract string GetDescription();

	// Token: 0x0600123E RID: 4670 RVA: 0x00071C1E File Offset: 0x0006FE1E
	public void EnableLogLevel(LogType _type, bool _enable)
	{
		if (_enable)
		{
			this.enabledLogLevels.Add(_type);
			return;
		}
		this.enabledLogLevels.Remove(_type);
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x00071C3E File Offset: 0x0006FE3E
	public bool IsLogLevelEnabled(LogType _type)
	{
		return this.enabledLogLevels.Contains(_type);
	}

	// Token: 0x06001240 RID: 4672 RVA: 0x00071C4C File Offset: 0x0006FE4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public ConsoleConnectionAbstract()
	{
	}

	// Token: 0x04000BE3 RID: 3043
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<LogType> enabledLogLevels = new HashSet<LogType>
	{
		LogType.Log,
		LogType.Warning,
		LogType.Error,
		LogType.Exception,
		LogType.Assert
	};
}
