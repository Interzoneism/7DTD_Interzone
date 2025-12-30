using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x02001421 RID: 5153
	public struct V2F
	{
		// Token: 0x0600A045 RID: 41029 RVA: 0x003F6B03 File Offset: 0x003F4D03
		public V2F(float v0, float v1)
		{
			this.V0 = v0;
			this.V1 = v1;
		}

		// Token: 0x0600A046 RID: 41030 RVA: 0x003F6B13 File Offset: 0x003F4D13
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}", base.GetType().Name, this.V0, this.V1);
		}

		// Token: 0x17001165 RID: 4453
		// (get) Token: 0x0600A047 RID: 41031 RVA: 0x003F6B4A File Offset: 0x003F4D4A
		public float X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x17001166 RID: 4454
		// (get) Token: 0x0600A048 RID: 41032 RVA: 0x003F6B52 File Offset: 0x003F4D52
		public float Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x04007B24 RID: 31524
		public float V0;

		// Token: 0x04007B25 RID: 31525
		public float V1;
	}
}
