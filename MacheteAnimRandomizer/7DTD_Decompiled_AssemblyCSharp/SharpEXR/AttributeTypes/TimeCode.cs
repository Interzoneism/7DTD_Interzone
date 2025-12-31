using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x02001420 RID: 5152
	public struct TimeCode
	{
		// Token: 0x0600A044 RID: 41028 RVA: 0x003F6AF3 File Offset: 0x003F4CF3
		public TimeCode(uint timeAndFlags, uint userData)
		{
			this.TimeAndFlags = timeAndFlags;
			this.UserData = userData;
		}

		// Token: 0x04007B22 RID: 31522
		public readonly uint TimeAndFlags;

		// Token: 0x04007B23 RID: 31523
		public readonly uint UserData;
	}
}
