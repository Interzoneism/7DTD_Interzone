using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x020017EA RID: 6122
	public interface IJoinSessionGameInviteListener
	{
		// Token: 0x0600B6B0 RID: 46768
		void Init(IPlatform _owner);

		// Token: 0x0600B6B1 RID: 46769
		[return: TupleElementNames(new string[]
		{
			"invite",
			"password"
		})]
		ValueTuple<string, string> TakePendingInvite();

		// Token: 0x0600B6B2 RID: 46770
		IEnumerator ConnectToInvite(string _invite, string _password = null, Action<bool> _onFinished = null);

		// Token: 0x0600B6B3 RID: 46771
		string GetListenerIdentifier();
	}
}
