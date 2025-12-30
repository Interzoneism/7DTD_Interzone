using System;

namespace Platform
{
	// Token: 0x0200181C RID: 6172
	public interface IRichPresence
	{
		// Token: 0x0600B7AF RID: 47023
		void Init(IPlatform _owner);

		// Token: 0x0600B7B0 RID: 47024
		void UpdateRichPresence(IRichPresence.PresenceStates _state);

		// Token: 0x0200181D RID: 6173
		public enum PresenceStates
		{
			// Token: 0x04008FF3 RID: 36851
			Menu,
			// Token: 0x04008FF4 RID: 36852
			Loading,
			// Token: 0x04008FF5 RID: 36853
			Connecting,
			// Token: 0x04008FF6 RID: 36854
			InGame
		}
	}
}
