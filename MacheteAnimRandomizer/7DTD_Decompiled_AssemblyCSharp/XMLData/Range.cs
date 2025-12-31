using System;

namespace XMLData
{
	// Token: 0x0200138C RID: 5004
	public class Range<TValue>
	{
		// Token: 0x06009C7F RID: 40063 RVA: 0x0000A7E3 File Offset: 0x000089E3
		public Range()
		{
		}

		// Token: 0x06009C80 RID: 40064 RVA: 0x003E1DD3 File Offset: 0x003DFFD3
		public Range(bool _hasMin, TValue _min, bool _hasMax, TValue _max)
		{
			this.hasMin = _hasMin;
			this.hasMax = _hasMax;
			this.min = _min;
			this.max = _max;
		}

		// Token: 0x06009C81 RID: 40065 RVA: 0x003E1DF8 File Offset: 0x003DFFF8
		public override string ToString()
		{
			return string.Format("{0}-{1}", this.hasMin ? this.min.ToString() : "*", this.hasMax ? this.max.ToString() : "*");
		}

		// Token: 0x040078F7 RID: 30967
		public bool hasMin;

		// Token: 0x040078F8 RID: 30968
		public bool hasMax;

		// Token: 0x040078F9 RID: 30969
		public TValue min;

		// Token: 0x040078FA RID: 30970
		public TValue max;
	}
}
