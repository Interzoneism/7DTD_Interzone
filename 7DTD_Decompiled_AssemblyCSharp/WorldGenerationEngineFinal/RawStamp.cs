using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001454 RID: 5204
	public class RawStamp
	{
		// Token: 0x17001183 RID: 4483
		// (get) Token: 0x0600A144 RID: 41284 RVA: 0x003FE633 File Offset: 0x003FC833
		public bool hasWater
		{
			get
			{
				return this.waterPixels != null;
			}
		}

		// Token: 0x0600A145 RID: 41285 RVA: 0x003FE640 File Offset: 0x003FC840
		public void SmoothAlpha(int _boxSize)
		{
			float[] array = new float[this.alphaPixels.Length];
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					double num = 0.0;
					int num2 = 0;
					for (int k = -1; k < _boxSize; k++)
					{
						int num3 = i + k;
						if (num3 >= 0 && num3 < this.height)
						{
							for (int l = -1; l < _boxSize; l++)
							{
								int num4 = j + l;
								if (num4 >= 0 && num4 < this.width)
								{
									num += (double)this.alphaPixels[num4 + num3 * this.width];
									num2++;
								}
							}
						}
					}
					num /= (double)num2;
					array[j + i * this.width] = (float)num;
				}
			}
			this.alphaPixels = array;
		}

		// Token: 0x0600A146 RID: 41286 RVA: 0x003FE718 File Offset: 0x003FC918
		public void BoxAlpha()
		{
			for (int i = 0; i < this.height; i += 4)
			{
				for (int j = 0; j < this.width; j += 4)
				{
					int num = j + i * this.width;
					double num2 = 0.0;
					for (int k = 0; k < 4; k++)
					{
						for (int l = 0; l < 4; l++)
						{
							num2 += (double)this.alphaPixels[num + l + k * this.width];
						}
					}
					num2 /= 16.0;
					for (int m = 0; m < 4; m++)
					{
						for (int n = 0; n < 4; n++)
						{
							this.alphaPixels[num + n + m * this.width] = (float)num2;
						}
					}
				}
			}
		}

		// Token: 0x04007C40 RID: 31808
		public string name;

		// Token: 0x04007C41 RID: 31809
		public float heightConst;

		// Token: 0x04007C42 RID: 31810
		public float[] heightPixels;

		// Token: 0x04007C43 RID: 31811
		public float alphaConst;

		// Token: 0x04007C44 RID: 31812
		public float[] alphaPixels;

		// Token: 0x04007C45 RID: 31813
		public float[] waterPixels;

		// Token: 0x04007C46 RID: 31814
		public int width;

		// Token: 0x04007C47 RID: 31815
		public int height;
	}
}
