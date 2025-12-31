using System;
using System.Collections;

// Token: 0x020012A8 RID: 4776
[PublicizedFrom(EAccessModifier.Internal)]
public static class vp_GlobalEventInternal
{
	// Token: 0x06009550 RID: 38224 RVA: 0x003B8085 File Offset: 0x003B6285
	public static vp_GlobalEventInternal.UnregisterException ShowUnregisterException(string name)
	{
		return new vp_GlobalEventInternal.UnregisterException(string.Format("Attempting to Unregister the event {0} but vp_GlobalEvent has not registered this event.", name));
	}

	// Token: 0x06009551 RID: 38225 RVA: 0x003B8097 File Offset: 0x003B6297
	public static vp_GlobalEventInternal.SendException ShowSendException(string name)
	{
		return new vp_GlobalEventInternal.SendException(string.Format("Attempting to Send the event {0} but vp_GlobalEvent has not registered this event.", name));
	}

	// Token: 0x040071F7 RID: 29175
	public static Hashtable Callbacks = new Hashtable();

	// Token: 0x020012A9 RID: 4777
	public class UnregisterException : Exception
	{
		// Token: 0x06009553 RID: 38227 RVA: 0x00384427 File Offset: 0x00382627
		public UnregisterException(string msg) : base(msg)
		{
		}
	}

	// Token: 0x020012AA RID: 4778
	public class SendException : Exception
	{
		// Token: 0x06009554 RID: 38228 RVA: 0x00384427 File Offset: 0x00382627
		public SendException(string msg) : base(msg)
		{
		}
	}
}
