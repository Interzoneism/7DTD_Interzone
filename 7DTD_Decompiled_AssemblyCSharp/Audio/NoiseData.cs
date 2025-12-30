using System;

namespace Audio
{
	// Token: 0x0200179E RID: 6046
	public class NoiseData
	{
		// Token: 0x0600B4F1 RID: 46321 RVA: 0x0045D571 File Offset: 0x0045B771
		public NoiseData()
		{
			this.volume = 0f;
			this.time = 1f;
			this.heatMapStrength = 0f;
			this.heatMapTime = 100UL;
			this.crouchMuffle = 1f;
		}

		// Token: 0x04008DBB RID: 36283
		public float volume;

		// Token: 0x04008DBC RID: 36284
		public float time;

		// Token: 0x04008DBD RID: 36285
		public float heatMapStrength;

		// Token: 0x04008DBE RID: 36286
		public ulong heatMapTime;

		// Token: 0x04008DBF RID: 36287
		public float crouchMuffle;
	}
}
