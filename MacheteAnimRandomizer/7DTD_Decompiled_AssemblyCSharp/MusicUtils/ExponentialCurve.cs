using System;

namespace MusicUtils
{
	// Token: 0x020016FB RID: 5883
	public class ExponentialCurve : LinearCurve
	{
		// Token: 0x0600B1E7 RID: 45543 RVA: 0x00454D78 File Offset: 0x00452F78
		public ExponentialCurve(double _base, float _start, float _end, float _startX, float _endX) : base((float)Math.Log((double)_start, _base), (float)Math.Log((double)_end, _base), _startX, _endX)
		{
			this.b = _base;
			if (_start < _end)
			{
				this.min = _start;
				this.max = _end;
				return;
			}
			this.min = _end;
			this.max = _start;
		}

		// Token: 0x0600B1E8 RID: 45544 RVA: 0x00454DC9 File Offset: 0x00452FC9
		public override float GetMixerValue(float _param)
		{
			return Utils.FastClamp((float)Math.Pow(this.b, (double)base.GetLine(_param)), this.min, this.max);
		}

		// Token: 0x04008B57 RID: 35671
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly double b;

		// Token: 0x04008B58 RID: 35672
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float min;

		// Token: 0x04008B59 RID: 35673
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly float max;
	}
}
