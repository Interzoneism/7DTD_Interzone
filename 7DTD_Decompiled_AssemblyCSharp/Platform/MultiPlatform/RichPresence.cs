using System;

namespace Platform.MultiPlatform
{
	// Token: 0x020018E3 RID: 6371
	public class RichPresence : IRichPresence
	{
		// Token: 0x0600BC50 RID: 48208 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600BC51 RID: 48209 RVA: 0x00477851 File Offset: 0x00475A51
		public void UpdateRichPresence(IRichPresence.PresenceStates _state)
		{
			IRichPresence richPresence = PlatformManager.NativePlatform.RichPresence;
			if (richPresence != null)
			{
				richPresence.UpdateRichPresence(_state);
			}
			IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
			if (crossplatformPlatform == null)
			{
				return;
			}
			IRichPresence richPresence2 = crossplatformPlatform.RichPresence;
			if (richPresence2 == null)
			{
				return;
			}
			richPresence2.UpdateRichPresence(_state);
		}
	}
}
