using System;
using MusicUtils.Enums;

namespace DynamicMusic.Legacy.ObjectModel
{
	// Token: 0x02001783 RID: 6019
	public class ThreatLevel : EnumDictionary<LayerType, Layer>
	{
		// Token: 0x0600B480 RID: 46208 RVA: 0x0045BA18 File Offset: 0x00459C18
		public ThreatLevel(double _tempo, double _sigHi, double _sigLo)
		{
			this.Tempo = _tempo;
			this.SignatureHi = _sigHi;
			this.SignatureLo = _sigLo;
		}

		// Token: 0x04008CDD RID: 36061
		public readonly double Tempo;

		// Token: 0x04008CDE RID: 36062
		public readonly double SignatureHi;

		// Token: 0x04008CDF RID: 36063
		public readonly double SignatureLo;
	}
}
