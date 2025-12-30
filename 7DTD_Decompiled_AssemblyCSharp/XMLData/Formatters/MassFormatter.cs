using System;

namespace XMLData.Formatters
{
	// Token: 0x020013AF RID: 5039
	public class MassFormatter : ValueFormatter<float>
	{
		// Token: 0x06009DE0 RID: 40416 RVA: 0x000FD701 File Offset: 0x000FB901
		public override string FormatValue(float _value)
		{
			return _value.ToCultureInvariantString();
		}
	}
}
