using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Platform;

// Token: 0x020002EB RID: 747
public class DiscordInviteListener : IJoinSessionGameInviteListener
{
	// Token: 0x1700026D RID: 621
	// (get) Token: 0x0600154B RID: 5451 RVA: 0x0007E0EB File Offset: 0x0007C2EB
	public static DiscordInviteListener ListenerInstance
	{
		get
		{
			DiscordInviteListener result;
			if ((result = DiscordInviteListener.listenerInstance) == null)
			{
				result = (DiscordInviteListener.listenerInstance = new DiscordInviteListener());
			}
			return result;
		}
	}

	// Token: 0x0600154C RID: 5452 RVA: 0x00002914 File Offset: 0x00000B14
	public void Init(IPlatform _owner)
	{
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x0007E101 File Offset: 0x0007C301
	public void SetPendingInvite(string _sessionId, string _password)
	{
		this.pendingActivityInvite = new ValueTuple<string, string>?(new ValueTuple<string, string>(_sessionId, _password));
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x0007E115 File Offset: 0x0007C315
	[return: TupleElementNames(new string[]
	{
		"invite",
		"password"
	})]
	public ValueTuple<string, string> TakePendingInvite()
	{
		if (this.pendingActivityInvite == null)
		{
			return new ValueTuple<string, string>(null, null);
		}
		ValueTuple<string, string> value = this.pendingActivityInvite.Value;
		this.pendingActivityInvite = null;
		return value;
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x0007E143 File Offset: 0x0007C343
	public IEnumerator ConnectToInvite(string _invite, string _password = null, Action<bool> _onFinished = null)
	{
		yield return InviteManager.HandleSessionIdInvite(_invite, _password, _onFinished);
		yield break;
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x0007E160 File Offset: 0x0007C360
	public string GetListenerIdentifier()
	{
		return "DCD";
	}

	// Token: 0x04000DA5 RID: 3493
	[PublicizedFrom(EAccessModifier.Private)]
	public static DiscordInviteListener listenerInstance;

	// Token: 0x04000DA6 RID: 3494
	[TupleElementNames(new string[]
	{
		"session",
		"password"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public ValueTuple<string, string>? pendingActivityInvite;
}
