using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x0200141F RID: 5151
	public struct TileDesc
	{
		// Token: 0x0600A042 RID: 41026 RVA: 0x003F6A84 File Offset: 0x003F4C84
		public TileDesc(uint xSize, uint ySize, byte mode)
		{
			this.XSize = xSize;
			this.YSize = ySize;
			int roundingMode = (mode & 240) >> 4;
			int levelMode = (int)(mode & 15);
			this.RoundingMode = (RoundingMode)roundingMode;
			this.LevelMode = (LevelMode)levelMode;
		}

		// Token: 0x0600A043 RID: 41027 RVA: 0x003F6ABC File Offset: 0x003F4CBC
		public override string ToString()
		{
			return string.Format("{0}: XSize={1}, YSize={2}", base.GetType().Name, this.XSize, this.YSize);
		}

		// Token: 0x04007B1E RID: 31518
		public readonly uint XSize;

		// Token: 0x04007B1F RID: 31519
		public readonly uint YSize;

		// Token: 0x04007B20 RID: 31520
		public readonly LevelMode LevelMode;

		// Token: 0x04007B21 RID: 31521
		public readonly RoundingMode RoundingMode;
	}
}
