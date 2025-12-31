using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200170D RID: 5901
	public abstract class AbstractMusicTimeTracker : AbstractFilter, IFilter<SectionType>
	{
		// Token: 0x0600B202 RID: 45570 RVA: 0x0045509B File Offset: 0x0045329B
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractMusicTimeTracker()
		{
		}

		// Token: 0x04008BA2 RID: 35746
		[PublicizedFrom(EAccessModifier.Protected)]
		public float dailyAllottedPlayTime;

		// Token: 0x04008BA3 RID: 35747
		[PublicizedFrom(EAccessModifier.Protected)]
		public float dailyPlayTimeUsed;

		// Token: 0x04008BA4 RID: 35748
		[PublicizedFrom(EAccessModifier.Protected)]
		public float musicStartTime;

		// Token: 0x04008BA5 RID: 35749
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool IsMusicPlaying;

		// Token: 0x04008BA6 RID: 35750
		[PublicizedFrom(EAccessModifier.Protected)]
		public float pauseDuration;

		// Token: 0x04008BA7 RID: 35751
		[PublicizedFrom(EAccessModifier.Protected)]
		public float pauseStartTime;
	}
}
