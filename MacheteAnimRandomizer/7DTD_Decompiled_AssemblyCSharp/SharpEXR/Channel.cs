using System;

namespace SharpEXR
{
	// Token: 0x020013F8 RID: 5112
	public class Channel
	{
		// Token: 0x1700113A RID: 4410
		// (get) Token: 0x06009F15 RID: 40725 RVA: 0x003F2123 File Offset: 0x003F0323
		// (set) Token: 0x06009F16 RID: 40726 RVA: 0x003F212B File Offset: 0x003F032B
		public string Name { get; set; }

		// Token: 0x1700113B RID: 4411
		// (get) Token: 0x06009F17 RID: 40727 RVA: 0x003F2134 File Offset: 0x003F0334
		// (set) Token: 0x06009F18 RID: 40728 RVA: 0x003F213C File Offset: 0x003F033C
		public PixelType Type { get; set; }

		// Token: 0x1700113C RID: 4412
		// (get) Token: 0x06009F19 RID: 40729 RVA: 0x003F2145 File Offset: 0x003F0345
		// (set) Token: 0x06009F1A RID: 40730 RVA: 0x003F214D File Offset: 0x003F034D
		public bool Linear { get; set; }

		// Token: 0x1700113D RID: 4413
		// (get) Token: 0x06009F1B RID: 40731 RVA: 0x003F2156 File Offset: 0x003F0356
		// (set) Token: 0x06009F1C RID: 40732 RVA: 0x003F215E File Offset: 0x003F035E
		public int XSampling { get; set; }

		// Token: 0x1700113E RID: 4414
		// (get) Token: 0x06009F1D RID: 40733 RVA: 0x003F2167 File Offset: 0x003F0367
		// (set) Token: 0x06009F1E RID: 40734 RVA: 0x003F216F File Offset: 0x003F036F
		public int YSampling { get; set; }

		// Token: 0x1700113F RID: 4415
		// (get) Token: 0x06009F1F RID: 40735 RVA: 0x003F2178 File Offset: 0x003F0378
		// (set) Token: 0x06009F20 RID: 40736 RVA: 0x003F2180 File Offset: 0x003F0380
		public byte[] Reserved { get; set; }

		// Token: 0x06009F21 RID: 40737 RVA: 0x003F218C File Offset: 0x003F038C
		public Channel(string name, PixelType type, bool linear, int xSampling, int ySampling) : this(name, type, linear, 0, 0, 0, xSampling, ySampling)
		{
		}

		// Token: 0x06009F22 RID: 40738 RVA: 0x003F21A9 File Offset: 0x003F03A9
		public Channel(string name, PixelType type, bool linear, byte reserved0, byte reserved1, byte reserved2, int xSampling, int ySampling)
		{
			this.Name = name;
			this.Type = type;
			this.Linear = linear;
			this.Reserved = new byte[]
			{
				reserved0,
				reserved1,
				reserved2
			};
		}

		// Token: 0x06009F23 RID: 40739 RVA: 0x003F21E1 File Offset: 0x003F03E1
		public override string ToString()
		{
			return string.Format("{0} {1} {2}", base.GetType().Name, this.Name, this.Type);
		}
	}
}
