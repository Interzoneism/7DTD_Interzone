using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200170A RID: 5898
	public abstract class AbstractDayTimeTracker : AbstractFilter, IFilter<SectionType>
	{
		// Token: 0x0600B1FD RID: 45565
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract int GetCurrentDay();

		// Token: 0x0600B1FE RID: 45566
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract float GetCurrentTime();

		// Token: 0x0600B1FF RID: 45567 RVA: 0x0045509B File Offset: 0x0045329B
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractDayTimeTracker()
		{
		}

		// Token: 0x04008B97 RID: 35735
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractDayTimeTracker.DayPeriodType dayPeriod;

		// Token: 0x04008B98 RID: 35736
		[PublicizedFrom(EAccessModifier.Protected)]
		public int currentDay;

		// Token: 0x04008B99 RID: 35737
		[PublicizedFrom(EAccessModifier.Protected)]
		public float currentTime;

		// Token: 0x04008B9A RID: 35738
		[PublicizedFrom(EAccessModifier.Protected)]
		public float dawnTime;

		// Token: 0x04008B9B RID: 35739
		[PublicizedFrom(EAccessModifier.Protected)]
		public float duskTime;

		// Token: 0x0200170B RID: 5899
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum DayPeriodType : byte
		{
			// Token: 0x04008B9D RID: 35741
			Morning,
			// Token: 0x04008B9E RID: 35742
			Day,
			// Token: 0x04008B9F RID: 35743
			Night,
			// Token: 0x04008BA0 RID: 35744
			Dusk,
			// Token: 0x04008BA1 RID: 35745
			Dawn
		}
	}
}
