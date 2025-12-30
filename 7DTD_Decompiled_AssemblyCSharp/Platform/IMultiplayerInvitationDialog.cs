using System;

namespace Platform
{
	// Token: 0x020017EE RID: 6126
	public interface IMultiplayerInvitationDialog
	{
		// Token: 0x17001499 RID: 5273
		// (get) Token: 0x0600B6C3 RID: 46787
		bool CanShow { get; }

		// Token: 0x0600B6C4 RID: 46788
		void Init(IPlatform owner);

		// Token: 0x0600B6C5 RID: 46789
		void ShowInviteDialog();
	}
}
