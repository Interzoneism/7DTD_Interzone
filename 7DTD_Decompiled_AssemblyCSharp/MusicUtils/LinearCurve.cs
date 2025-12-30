using System;

namespace MusicUtils
{
	// Token: 0x020016FA RID: 5882
	public class LinearCurve : Curve
	{
		// Token: 0x0600B1E4 RID: 45540 RVA: 0x00454D38 File Offset: 0x00452F38
		public LinearCurve(float _startY, float _endY, float _startX, float _endX) : base(_startY, _endY, _startX, _endX)
		{
		}

		// Token: 0x0600B1E5 RID: 45541 RVA: 0x00454D45 File Offset: 0x00452F45
		public override float GetMixerValue(float _param)
		{
			return Utils.FastClamp(this.GetLine(_param), this.linearStart, this.linearEnd);
		}

		// Token: 0x0600B1E6 RID: 45542 RVA: 0x00454D5F File Offset: 0x00452F5F
		[PublicizedFrom(EAccessModifier.Protected)]
		public float GetLine(float _param)
		{
			return this.rate * (_param - this.startX) + this.linearStart;
		}
	}
}
