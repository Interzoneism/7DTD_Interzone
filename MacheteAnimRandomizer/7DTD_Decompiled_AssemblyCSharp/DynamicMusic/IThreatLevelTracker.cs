using System;

namespace DynamicMusic
{
	// Token: 0x0200176E RID: 5998
	public interface IThreatLevelTracker : ICleanable
	{
		// Token: 0x17001417 RID: 5143
		// (get) Token: 0x0600B3DC RID: 46044
		IThreatLevel ThreatLevel { get; }

		// Token: 0x0600B3DD RID: 46045
		void Update(EntityPlayerLocal _player);
	}
}
