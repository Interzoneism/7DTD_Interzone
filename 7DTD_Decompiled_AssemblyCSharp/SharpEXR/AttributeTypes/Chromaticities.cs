using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x0200141A RID: 5146
	public struct Chromaticities
	{
		// Token: 0x0600A03B RID: 41019 RVA: 0x003F6893 File Offset: 0x003F4A93
		public Chromaticities(float redX, float redY, float greenX, float greenY, float blueX, float blueY, float whiteX, float whiteY)
		{
			this.RedX = redX;
			this.RedY = redY;
			this.GreenX = greenX;
			this.GreenY = greenY;
			this.BlueX = blueX;
			this.BlueY = blueY;
			this.WhiteX = whiteX;
			this.WhiteY = whiteY;
		}

		// Token: 0x04007B0B RID: 31499
		public readonly float RedX;

		// Token: 0x04007B0C RID: 31500
		public readonly float RedY;

		// Token: 0x04007B0D RID: 31501
		public readonly float GreenX;

		// Token: 0x04007B0E RID: 31502
		public readonly float GreenY;

		// Token: 0x04007B0F RID: 31503
		public readonly float BlueX;

		// Token: 0x04007B10 RID: 31504
		public readonly float BlueY;

		// Token: 0x04007B11 RID: 31505
		public readonly float WhiteX;

		// Token: 0x04007B12 RID: 31506
		public readonly float WhiteY;
	}
}
