using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x02001422 RID: 5154
	public struct V2I
	{
		// Token: 0x0600A049 RID: 41033 RVA: 0x003F6B5A File Offset: 0x003F4D5A
		public V2I(int v0, int v1)
		{
			this.V0 = v0;
			this.V1 = v1;
		}

		// Token: 0x0600A04A RID: 41034 RVA: 0x003F6B6A File Offset: 0x003F4D6A
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}", base.GetType().Name, this.V0, this.V1);
		}

		// Token: 0x17001167 RID: 4455
		// (get) Token: 0x0600A04B RID: 41035 RVA: 0x003F6BA1 File Offset: 0x003F4DA1
		public int X
		{
			get
			{
				return this.V0;
			}
		}

		// Token: 0x17001168 RID: 4456
		// (get) Token: 0x0600A04C RID: 41036 RVA: 0x003F6BA9 File Offset: 0x003F4DA9
		public int Y
		{
			get
			{
				return this.V1;
			}
		}

		// Token: 0x04007B26 RID: 31526
		public int V0;

		// Token: 0x04007B27 RID: 31527
		public int V1;
	}
}
