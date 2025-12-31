using System;

// Token: 0x02001166 RID: 4454
public class DebugWrapperException : Exception
{
	// Token: 0x06008B5C RID: 35676 RVA: 0x0038441F File Offset: 0x0038261F
	public DebugWrapperException()
	{
	}

	// Token: 0x06008B5D RID: 35677 RVA: 0x00384427 File Offset: 0x00382627
	public DebugWrapperException(string message) : base(message)
	{
	}

	// Token: 0x06008B5E RID: 35678 RVA: 0x00384430 File Offset: 0x00382630
	public DebugWrapperException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
