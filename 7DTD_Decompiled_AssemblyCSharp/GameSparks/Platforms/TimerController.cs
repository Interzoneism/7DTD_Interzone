using System;
using System.Collections.Generic;

namespace GameSparks.Platforms
{
	// Token: 0x02001997 RID: 6551
	public class TimerController
	{
		// Token: 0x0600C0CC RID: 49356 RVA: 0x00490240 File Offset: 0x0048E440
		public void Initialize()
		{
			this.timeOfLastUpdate = DateTime.UtcNow.Ticks;
		}

		// Token: 0x0600C0CD RID: 49357 RVA: 0x00490260 File Offset: 0x0048E460
		public void Update()
		{
			long num = DateTime.UtcNow.Ticks - this.timeOfLastUpdate;
			this.timeOfLastUpdate += num;
			foreach (IControlledTimer controlledTimer in this.timers)
			{
				controlledTimer.Update(num);
			}
		}

		// Token: 0x0600C0CE RID: 49358 RVA: 0x004902D4 File Offset: 0x0048E4D4
		public void AddTimer(IControlledTimer timer)
		{
			this.timers.Add(timer);
		}

		// Token: 0x0600C0CF RID: 49359 RVA: 0x004902E2 File Offset: 0x0048E4E2
		public void RemoveTimer(IControlledTimer timer)
		{
			this.timers.Remove(timer);
		}

		// Token: 0x0400964D RID: 38477
		[PublicizedFrom(EAccessModifier.Private)]
		public long timeOfLastUpdate;

		// Token: 0x0400964E RID: 38478
		[PublicizedFrom(EAccessModifier.Private)]
		public List<IControlledTimer> timers = new List<IControlledTimer>();
	}
}
