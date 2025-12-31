using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x0200146B RID: 5227
	public class TranslationData
	{
		// Token: 0x0600A205 RID: 41477 RVA: 0x004064D8 File Offset: 0x004046D8
		public TranslationData(int _x, int _y, float _randomScaleMin = 0.5f, float _randomScaleMax = 1.5f, int _rotation = -1)
		{
			this.x = _x;
			this.y = _y;
			this.scale = Rand.Instance.Range(_randomScaleMin, _randomScaleMax);
			this.rotation = _rotation;
			if (_rotation < 0)
			{
				this.rotation = Rand.Instance.Range(0, 360);
			}
		}

		// Token: 0x0600A206 RID: 41478 RVA: 0x0040652F File Offset: 0x0040472F
		public TranslationData(int _x, int _y, float _scale, int _rotation)
		{
			this.x = _x;
			this.y = _y;
			this.scale = _scale;
			this.rotation = _rotation;
		}

		// Token: 0x04007CF0 RID: 31984
		public int x;

		// Token: 0x04007CF1 RID: 31985
		public int y;

		// Token: 0x04007CF2 RID: 31986
		public float scale;

		// Token: 0x04007CF3 RID: 31987
		public int rotation;
	}
}
