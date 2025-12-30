using System;

namespace MusicUtils
{
	// Token: 0x020016F9 RID: 5881
	public abstract class Curve
	{
		// Token: 0x0600B1E2 RID: 45538 RVA: 0x00454D0D File Offset: 0x00452F0D
		public Curve(float _start, float _end, float _startX, float _endX)
		{
			this.rate = (_end - _start) / (_endX - _startX);
			this.linearStart = _start;
			this.linearEnd = _end;
			this.startX = _startX;
		}

		// Token: 0x0600B1E3 RID: 45539
		public abstract float GetMixerValue(float _param);

		// Token: 0x04008B53 RID: 35667
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float rate;

		// Token: 0x04008B54 RID: 35668
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float linearStart;

		// Token: 0x04008B55 RID: 35669
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float linearEnd;

		// Token: 0x04008B56 RID: 35670
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly float startX;
	}
}
