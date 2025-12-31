using System;

namespace Platform
{
	// Token: 0x020017DE RID: 6110
	public interface IAuthenticationClient
	{
		// Token: 0x0600B681 RID: 46721
		void Init(IPlatform _owner);

		// Token: 0x0600B682 RID: 46722
		string GetAuthTicket();

		// Token: 0x0600B683 RID: 46723
		void AuthenticateServer(ClientAuthenticateServerContext _context);

		// Token: 0x0600B684 RID: 46724
		void Destroy();
	}
}
