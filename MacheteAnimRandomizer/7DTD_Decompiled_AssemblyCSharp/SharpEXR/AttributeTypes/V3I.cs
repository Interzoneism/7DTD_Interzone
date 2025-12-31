using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x02001424 RID: 5156
	public struct V3I
	{
		// Token: 0x0600A052 RID: 41042 RVA: 0x003F6C3F File Offset: 0x003F4E3F
		public V3I(int v0, int v1, int v2)
		{
			this.V0 = v0;
			this.V1 = v1;
			this.V2 = v2;
		}

		// Token: 0x0600A053 RID: 41043 RVA: 0x003F6C58 File Offset: 0x003F4E58
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

		// Token: 0x1700116C RID: 4460
		// (get) Token: 0x0600A054 RID: 41044 RVA: 0x003F6CB7 File Offset: 0x003F4EB7
		public int X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x1700116D RID: 4461
		// (get) Token: 0x0600A055 RID: 41045 RVA: 0x003F6CBF File Offset: 0x003F4EBF
		public int Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x1700116E RID: 4462
		// (get) Token: 0x0600A056 RID: 41046 RVA: 0x003F6CC7 File Offset: 0x003F4EC7
		public int Z
		{
			get
			{
				return this.V2;
			}
		}

		// Token: 0x04007B2B RID: 31531
		public int V0;

		// Token: 0x04007B2C RID: 31532
		public int V1;

		// Token: 0x04007B2D RID: 31533
		public int V2;
	}
}
