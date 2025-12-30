using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001456 RID: 5206
	public class Stamp
	{
		// Token: 0x17001185 RID: 4485
		// (get) Token: 0x0600A155 RID: 41301 RVA: 0x003FE8D0 File Offset: 0x003FCAD0
		public int imageHeight
		{
			get
			{
				return this.stamp.height;
			}
		}

		// Token: 0x17001186 RID: 4486
		// (get) Token: 0x0600A156 RID: 41302 RVA: 0x003FE8DD File Offset: 0x003FCADD
		public int imageWidth
		{
			get
			{
				return this.stamp.width;
			}
		}

		// Token: 0x0600A157 RID: 41303 RVA: 0x003FE8EC File Offset: 0x003FCAEC
		public Stamp(WorldBuilder _worldBuilder, RawStamp _stamp, TranslationData _transData, bool _isCustomColor = false, Color _customColor = default(Color), float _biomeAlphaCutoff = 0.1f, bool _isWater = false, string stampName = "")
		{
			this.worldBuilder = _worldBuilder;
			this.stamp = _stamp;
			this.transform = _transData;
			this.scale = this.transform.scale;
			this.isCustomColor = _isCustomColor;
			this.customColor = _customColor;
			this.biomeAlphaCutoff = _biomeAlphaCutoff;
			this.isWater = _isWater;
			this.Name = stampName;
			this.alpha = 1f;
			this.additive = false;
			int rotation = this.transform.rotation;
			int num = (int)((float)_stamp.width * this.scale * 1.4f);
			int num2 = (int)((float)_stamp.height * this.scale * 1.4f);
			int x = this.transform.x - num / 2;
			int x2 = this.transform.x + num / 2;
			int y = this.transform.y - num2 / 2;
			int y2 = this.transform.y + num2 / 2;
			int x3 = this.transform.x;
			int y3 = this.transform.y;
			Vector2i rotatedPoint = this.getRotatedPoint(x, y, x3, y3, rotation);
			Vector2i rotatedPoint2 = this.getRotatedPoint(x2, y, x3, y3, rotation);
			Vector2i rotatedPoint3 = this.getRotatedPoint(x, y2, x3, y3, rotation);
			Vector2i rotatedPoint4 = this.getRotatedPoint(x2, y2, x3, y3, rotation);
			Vector2 vector = new Vector2((float)Mathf.Min(Mathf.Min(rotatedPoint.x, rotatedPoint2.x), Mathf.Min(rotatedPoint3.x, rotatedPoint4.x)), (float)Mathf.Min(Mathf.Min(rotatedPoint.y, rotatedPoint2.y), Mathf.Min(rotatedPoint3.y, rotatedPoint4.y)));
			Vector2 a = new Vector2((float)Mathf.Max(Mathf.Max(rotatedPoint.x, rotatedPoint2.x), Mathf.Max(rotatedPoint3.x, rotatedPoint4.x)), (float)Mathf.Max(Mathf.Max(rotatedPoint.y, rotatedPoint2.y), Mathf.Max(rotatedPoint3.y, rotatedPoint4.y)));
			this.Area = new Rect(vector, a - vector);
			if (this.isWater)
			{
				if (this.worldBuilder.waterRects == null)
				{
					this.worldBuilder.waterRects = new List<Rect>();
				}
				this.worldBuilder.waterRects.Add(this.Area);
			}
		}

		// Token: 0x0600A158 RID: 41304 RVA: 0x003FEB50 File Offset: 0x003FCD50
		[PublicizedFrom(EAccessModifier.Private)]
		public Color[] rotateColorArray(Color[] src, float angle, int width, int height)
		{
			Color[] array = new Color[width * height];
			double num = Math.Sin(0.017453292519943295 * (double)angle);
			double num2 = Math.Cos(0.017453292519943295 * (double)angle);
			int num3 = width / 2;
			int num4 = height / 2;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					float num5 = (float)(num2 * (double)(j - num3) + num * (double)(i - num4) + (double)num3);
					float num6 = (float)(-(float)num * (double)(j - num3) + num2 * (double)(i - num4) + (double)num4);
					int num7 = (int)num5;
					int num8 = (int)num6;
					num5 -= (float)num7;
					num6 -= (float)num8;
					if (num7 >= 0 && num7 < width && num8 >= 0 && num8 < height)
					{
						Color color = src[num8 * width + num7];
						Color rightVal = color;
						Color upVal = color;
						Color upRightVal = color;
						if (num7 + 1 < width)
						{
							rightVal = src[num8 * width + num7 + 1];
						}
						if (num8 + 1 < height)
						{
							upVal = src[(num8 + 1) * width + num7];
						}
						if (num7 + 1 < width && num8 + 1 < height)
						{
							upRightVal = src[(num8 + 1) * width + num7 + 1];
						}
						array[i * width + j] = this.QuadLerpColor(color, rightVal, upRightVal, upVal, num5, num6);
					}
				}
			}
			return array;
		}

		// Token: 0x0600A159 RID: 41305 RVA: 0x003FECAC File Offset: 0x003FCEAC
		[PublicizedFrom(EAccessModifier.Private)]
		public Color QuadLerpColor(Color selfVal, Color rightVal, Color upRightVal, Color upVal, float horizontalPerc, float verticalPerc)
		{
			return Color.Lerp(Color.Lerp(selfVal, rightVal, horizontalPerc), Color.Lerp(upVal, upRightVal, horizontalPerc), verticalPerc);
		}

		// Token: 0x0600A15A RID: 41306 RVA: 0x003FECC8 File Offset: 0x003FCEC8
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector2i getRotatedPoint(int x, int y, int cx, int cy, int angle)
		{
			double num = Math.Cos((double)angle);
			double num2 = Math.Sin((double)angle);
			return new Vector2i(Mathf.RoundToInt((float)((double)(x - cx) * num - (double)(y - cy) * num2 + (double)cx)), Mathf.RoundToInt((float)((double)(x - cx) * num2 + (double)(y - cy) * num + (double)cy)));
		}

		// Token: 0x04007C4A RID: 31818
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly WorldBuilder worldBuilder;

		// Token: 0x04007C4B RID: 31819
		public TranslationData transform;

		// Token: 0x04007C4C RID: 31820
		public float alpha;

		// Token: 0x04007C4D RID: 31821
		public bool additive;

		// Token: 0x04007C4E RID: 31822
		public float scale;

		// Token: 0x04007C4F RID: 31823
		public bool isCustomColor;

		// Token: 0x04007C50 RID: 31824
		public Color customColor;

		// Token: 0x04007C51 RID: 31825
		public Rect Area;

		// Token: 0x04007C52 RID: 31826
		public float biomeAlphaCutoff;

		// Token: 0x04007C53 RID: 31827
		public bool isWater;

		// Token: 0x04007C54 RID: 31828
		public string Name = "";

		// Token: 0x04007C55 RID: 31829
		public RawStamp stamp;

		// Token: 0x04007C56 RID: 31830
		[PublicizedFrom(EAccessModifier.Private)]
		public const float oneByoneScale = 1.4f;
	}
}
