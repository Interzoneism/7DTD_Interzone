using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x02001418 RID: 5144
	public struct Box2F
	{
		// Token: 0x0600A033 RID: 41011 RVA: 0x003F672B File Offset: 0x003F492B
		public Box2F(float xMin, float yMin, float xMax, float yMax)
		{
			this.XMin = xMin;
			this.YMin = yMin;
			this.XMax = xMax;
			this.YMax = yMax;
		}

		// Token: 0x0600A034 RID: 41012 RVA: 0x003F674C File Offset: 0x003F494C
		public override string ToString()
		{
			return string.Format("{0}: ({1}, {2})-({3}, {4})", new object[]
			{
				base.GetType().Name,
				this.XMin,
				this.YMin,
				this.XMax,
				this.YMax
			});
		}

		// Token: 0x17001160 RID: 4448
		// (get) Token: 0x0600A035 RID: 41013 RVA: 0x003F67B9 File Offset: 0x003F49B9
		public float Width
		{
			get
			{
				return this.XMax - this.XMin + 1f;
			}
		}

		// Token: 0x17001161 RID: 4449
		// (get) Token: 0x0600A036 RID: 41014 RVA: 0x003F67CE File Offset: 0x003F49CE
		public float Height
		{
			get
			{
				return this.YMax - this.YMin + 1f;
			}
		}

		// Token: 0x04007B03 RID: 31491
		public readonly float XMin;

		// Token: 0x04007B04 RID: 31492
		public readonly float YMin;

		// Token: 0x04007B05 RID: 31493
		public readonly float XMax;

		// Token: 0x04007B06 RID: 31494
		public readonly float YMax;
	}
}
