using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x02001423 RID: 5155
	public struct V3F
	{
		// Token: 0x0600A04D RID: 41037 RVA: 0x003F6BB1 File Offset: 0x003F4DB1
		public V3F(float v0, float v1, float v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		// Token: 0x0600A04E RID: 41038 RVA: 0x003F6BC8 File Offset: 0x003F4DC8
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}, {3}", new object[]
			{
				base.GetType().Name,
				this.V0,
				this.V1,
				this.V2
			});
		}

		// Token: 0x17001169 RID: 4457
		// (get) Token: 0x0600A04F RID: 41039 RVA: 0x003F6C27 File Offset: 0x003F4E27
		public float X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x1700116A RID: 4458
		// (get) Token: 0x0600A050 RID: 41040 RVA: 0x003F6C2F File Offset: 0x003F4E2F
		public float Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x1700116B RID: 4459
		// (get) Token: 0x0600A051 RID: 41041 RVA: 0x003F6C37 File Offset: 0x003F4E37
		public float Z
		{
			get
			{
				return this.V2;
			}
		}

		// Token: 0x04007B28 RID: 31528
		public float V0;

		// Token: 0x04007B29 RID: 31529
		public float V1;

		// Token: 0x04007B2A RID: 31530
		public float V2;
	}
}
