using System;

namespace DynamicMusic
{
	// Token: 0x0200170F RID: 5903
	public abstract class ContentPlayer : IPlayable
	{
		// Token: 0x170013B8 RID: 5048
		// (get) Token: 0x0600B20F RID: 45583 RVA: 0x00455147 File Offset: 0x00453347
		// (set) Token: 0x0600B210 RID: 45584 RVA: 0x0045514F File Offset: 0x0045334F
		public virtual float Volume { get; set; } = 1f;

		// Token: 0x170013B9 RID: 5049
		// (get) Token: 0x0600B211 RID: 45585 RVA: 0x00455158 File Offset: 0x00453358
		// (set) Token: 0x0600B212 RID: 45586 RVA: 0x00455160 File Offset: 0x00453360
		public virtual bool IsDone { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170013BA RID: 5050
		// (get) Token: 0x0600B213 RID: 45587 RVA: 0x00455169 File Offset: 0x00453369
		// (set) Token: 0x0600B214 RID: 45588 RVA: 0x00455171 File Offset: 0x00453371
		public virtual bool IsPaused { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170013BB RID: 5051
		// (get) Token: 0x0600B215 RID: 45589 RVA: 0x0045517A File Offset: 0x0045337A
		// (set) Token: 0x0600B216 RID: 45590 RVA: 0x00455182 File Offset: 0x00453382
		public virtual bool IsPlaying { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170013BC RID: 5052
		// (get) Token: 0x0600B217 RID: 45591 RVA: 0x0045518B File Offset: 0x0045338B
		// (set) Token: 0x0600B218 RID: 45592 RVA: 0x00455193 File Offset: 0x00453393
		public virtual bool IsReady { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600B219 RID: 45593
		public abstract void Init();

		// Token: 0x0600B21A RID: 45594 RVA: 0x0045519C File Offset: 0x0045339C
		public virtual void Play()
		{
			this.IsDone = false;
			this.IsPlaying = true;
			this.IsPaused = false;
		}

		// Token: 0x0600B21B RID: 45595 RVA: 0x004551B3 File Offset: 0x004533B3
		public virtual void Pause()
		{
			this.IsPlaying = false;
			this.IsPaused = true;
		}

		// Token: 0x0600B21C RID: 45596 RVA: 0x004551C3 File Offset: 0x004533C3
		public virtual void UnPause()
		{
			this.IsPlaying = true;
			this.IsPaused = false;
		}

		// Token: 0x0600B21D RID: 45597 RVA: 0x004551D3 File Offset: 0x004533D3
		public virtual void Stop()
		{
			this.IsDone = true;
			this.IsPlaying = false;
			this.IsPaused = false;
		}

		// Token: 0x0600B21E RID: 45598 RVA: 0x004551EA File Offset: 0x004533EA
		[PublicizedFrom(EAccessModifier.Protected)]
		public ContentPlayer()
		{
		}
	}
}
