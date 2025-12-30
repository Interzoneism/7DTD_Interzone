using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x0200141B RID: 5147
	public struct KeyCode
	{
		// Token: 0x0600A03C RID: 41020 RVA: 0x003F68D2 File Offset: 0x003F4AD2
		public KeyCode(int filmMfcCode, int filmType, int prefix, int count, int perfOffset, int perfsPerFrame, int perfsPerCount)
		{
			this.FilmMfcCode = filmMfcCode;
			this.FilmType = filmType;
			this.Prefix = prefix;
			this.Count = count;
			this.PerfOffset = perfOffset;
			this.PerfsPerFrame = perfsPerFrame;
			this.PerfsPerCount = perfsPerCount;
		}

		// Token: 0x04007B13 RID: 31507
		public readonly int FilmMfcCode;

		// Token: 0x04007B14 RID: 31508
		public readonly int FilmType;

		// Token: 0x04007B15 RID: 31509
		public readonly int Prefix;

		// Token: 0x04007B16 RID: 31510
		public readonly int Count;

		// Token: 0x04007B17 RID: 31511
		public readonly int PerfOffset;

		// Token: 0x04007B18 RID: 31512
		public readonly int PerfsPerFrame;

		// Token: 0x04007B19 RID: 31513
		public readonly int PerfsPerCount;
	}
}
