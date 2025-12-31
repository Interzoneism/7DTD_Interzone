using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x020017D7 RID: 6103
	public interface IAntiCheatClient : IAntiCheatEncryption, IEncryptionModule
	{
		// Token: 0x0600B65C RID: 46684
		void Init(IPlatform _owner);

		// Token: 0x0600B65D RID: 46685
		bool ClientAntiCheatEnabled();

		// Token: 0x0600B65E RID: 46686
		bool GetUnhandledViolationMessage(out string _message);

		// Token: 0x0600B65F RID: 46687
		void WaitForRemoteAuth(Action onRemoteAuthSkippedOrComplete);

		// Token: 0x0600B660 RID: 46688
		void ConnectToServer([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] ValueTuple<PlatformUserIdentifierAbs, string> hostUserAndToken, Action onNoAntiCheatOrConnectionComplete, Action<string> onConnectionFailed);

		// Token: 0x0600B661 RID: 46689
		void HandleMessageFromServer(byte[] _data);

		// Token: 0x0600B662 RID: 46690
		void DisconnectFromServer();

		// Token: 0x0600B663 RID: 46691
		void Destroy();
	}
}
