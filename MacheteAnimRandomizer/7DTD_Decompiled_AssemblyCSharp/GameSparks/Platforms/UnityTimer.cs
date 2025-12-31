using System;

namespace GameSparks.Platforms
{
	// Token: 0x02001998 RID: 6552
	public class UnityTimer : IControlledTimer, IGameSparksTimer
	{
		// Token: 0x0600C0D1 RID: 49361 RVA: 0x00490304 File Offset: 0x0048E504
		public void SetController(TimerController controller)
		{
			this.controller = controller;
			this.controller.AddTimer(this);
		}

		// Token: 0x0600C0D2 RID: 49362 RVA: 0x00490319 File Offset: 0x0048E519
		public void Initialize(int interval, Action callback)
		{
			this.callback = callback;
			this.interval = interval;
			this.running = true;
		}

		// Token: 0x0600C0D3 RID: 49363 RVA: 0x00002914 File Offset: 0x00000B14
		public void Trigger()
		{
		}

		// Token: 0x0600C0D4 RID: 49364 RVA: 0x00490330 File Offset: 0x0048E530
		public void Stop()
		{
			this.running = false;
			this.callback = null;
			this.controller.RemoveTimer(this);
		}

		// Token: 0x0600C0D5 RID: 49365 RVA: 0x0049034C File Offset: 0x0048E54C
		public void Update(long ticks)
		{
			if (this.running)
			{
				this.elapsedTicks += ticks;
				if (this.elapsedTicks > (long)this.interval)
				{
					this.elapsedTicks -= (long)this.interval;
					if (this.callback != null)
					{
						this.callback();
					}
				}
			}
		}

		// Token: 0x0400964F RID: 38479
		[PublicizedFrom(EAccessModifier.Private)]
		public Action callback;

		// Token: 0x04009650 RID: 38480
		[PublicizedFrom(EAccessModifier.Private)]
		public int interval;

		// Token: 0x04009651 RID: 38481
		[PublicizedFrom(EAccessModifier.Private)]
		public long elapsedTicks;

		// Token: 0x04009652 RID: 38482
		[PublicizedFrom(EAccessModifier.Private)]
		public bool running;

		// Token: 0x04009653 RID: 38483
		[PublicizedFrom(EAccessModifier.Private)]
		public TimerController controller;
	}
}
