using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x02001419 RID: 5145
	public struct Box2I
	{
		// Token: 0x0600A037 RID: 41015 RVA: 0x003F67E3 File Offset: 0x003F49E3
		public Box2I(int xMin, int yMin, int xMax, int yMax)
		{
			this.XMin = xMin;
			this.YMin = yMin;
			this.XMax = xMax;
			this.YMax = yMax;
		}

		// Token: 0x0600A038 RID: 41016 RVA: 0x003F6804 File Offset: 0x003F4A04
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

		// Token: 0x17001162 RID: 4450
		// (get) Token: 0x0600A039 RID: 41017 RVA: 0x003F6871 File Offset: 0x003F4A71
		public int Width
		{
			get
			{
				return this.XMax - this.XMin + 1;
			}
		}

		// Token: 0x17001163 RID: 4451
		// (get) Token: 0x0600A03A RID: 41018 RVA: 0x003F6882 File Offset: 0x003F4A82
		public int Height
		{
			get
			{
				return this.YMax - this.YMin + 1;
			}
		}

		// Token: 0x04007B07 RID: 31495
		public readonly int XMin;

		// Token: 0x04007B08 RID: 31496
		public readonly int YMin;

		// Token: 0x04007B09 RID: 31497
		public readonly int XMax;

		// Token: 0x04007B0A RID: 31498
		public readonly int YMax;
	}
}
