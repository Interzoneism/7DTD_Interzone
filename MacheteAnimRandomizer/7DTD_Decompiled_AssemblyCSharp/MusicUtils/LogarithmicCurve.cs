using System;

namespace MusicUtils
{
	// Token: 0x020016FC RID: 5884
	public class LogarithmicCurve : LinearCurve
	{
		// Token: 0x0600B1E9 RID: 45545 RVA: 0x00454DF0 File Offset: 0x00452FF0
		public LogarithmicCurve(double _base, double _scale, float _start, float _end, float _startX, float _endX) : base((float)Math.Pow(_base, (double)_start / _scale), (float)Math.Pow(_base, (double)_end / _scale), _startX, _endX)
		{
			this.b = Math.Pow(_base, 1.0 / _scale);
			if (_start < _end)
			{
				this.min = _start;
				this.max = _end;
				return;
			}
			this.min = _end;
			this.max = _start;
		}

		// Token: 0x0600B1EA RID: 45546 RVA: 0x00454E59 File Offset: 0x00453059
		public override float GetMixerValue(float _param)
		{
			return Utils.FastClamp((float)Math.Log((double)Math.Max(base.GetLine(_param), 0f), this.b), this.min, this.max);
		}

		// Token: 0x04008B5A RID: 35674
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly double b;

		// Token: 0x04008B5B RID: 35675
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float min;

		// Token: 0x04008B5C RID: 35676
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float max;
	}
}
