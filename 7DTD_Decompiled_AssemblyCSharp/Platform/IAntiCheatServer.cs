using System;
using System.Runtime.CompilerServices;

namespace Platform
{
	// Token: 0x020017D9 RID: 6105
	public interface IAntiCheatServer : IAntiCheatEncryption, IEncryptionModule
	{
		// Token: 0x0600B665 RID: 46693
		void Init(IPlatform _owner);

		// Token: 0x0600B666 RID: 46694
		void Update();

		// Token: 0x0600B667 RID: 46695
		bool StartServer(AuthenticationSuccessfulCallbackDelegate _authSuccessfulDelegate, KickPlayerDelegate _kickPlayerDelegate);

		// Token: 0x0600B668 RID: 46696
		bool RegisterUser(ClientInfo _client);

		// Token: 0x0600B669 RID: 46697
		void FreeUser(ClientInfo _client);

		// Token: 0x0600B66A RID: 46698
		void HandleMessageFromClient(ClientInfo _cInfo, byte[] _data);

		// Token: 0x0600B66B RID: 46699
		void StopServer();

		// Token: 0x0600B66C RID: 46700
		void Destroy();

		// Token: 0x0600B66D RID: 46701
		bool ServerEacEnabled();

		// Token: 0x0600B66E RID: 46702
		bool ServerEacAvailable();

		// Token: 0x0600B66F RID: 46703
		bool GetHostUserIdAndToken([TupleElementNames(new string[]
		{
			"userId",
			"token"
		})] out ValueTuple<PlatformUserIdentifierAbs, string> _hostUserIdAndToken);
	}
}
