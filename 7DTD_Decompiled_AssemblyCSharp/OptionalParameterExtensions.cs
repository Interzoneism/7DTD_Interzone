using System;

// Token: 0x020011DF RID: 4575
public static class OptionalParameterExtensions
{
	// Token: 0x06008ED9 RID: 36569 RVA: 0x0039250F File Offset: 0x0039070F
	public static T OrIfNullThen<T>(this T element, T defaultValue) where T : class
	{
		if (element != null)
		{
			return element;
		}
		return defaultValue;
	}
}
