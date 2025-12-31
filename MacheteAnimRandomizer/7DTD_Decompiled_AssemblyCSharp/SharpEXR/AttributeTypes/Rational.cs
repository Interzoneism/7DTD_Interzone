using System;

namespace SharpEXR.AttributeTypes
{
	// Token: 0x0200141E RID: 5150
	public struct Rational
	{
		// Token: 0x0600A03F RID: 41023 RVA: 0x003F6A3E File Offset: 0x003F4C3E
		public Rational(int numerator, uint denominator)
		{
			this.Numerator = numerator;
			this.Denominator = denominator;
		}

		// Token: 0x0600A040 RID: 41024 RVA: 0x003F6A4E File Offset: 0x003F4C4E
		public override string ToString()
		{
			return string.Format("{0}/{1}", this.Numerator, this.Denominator);
		}

		// Token: 0x17001164 RID: 4452
		// (get) Token: 0x0600A041 RID: 41025 RVA: 0x003F6A70 File Offset: 0x003F4C70
		public double Value
		{
			get
			{
				return (double)this.Numerator / this.Denominator;
			}
		}

		// Token: 0x04007B1C RID: 31516
		public readonly int Numerator;

		// Token: 0x04007B1D RID: 31517
		public readonly uint Denominator;
	}
}
