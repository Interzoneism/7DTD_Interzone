using System;

namespace XMLData.Formatters
{
	// Token: 0x020013B0 RID: 5040
	public abstract class ValueFormatter<TValue>
	{
		// Token: 0x06009DE2 RID: 40418
		public abstract string FormatValue(TValue _value);

		// Token: 0x06009DE3 RID: 40419 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Protected)]
		public ValueFormatter()
		{
		}
	}
}
