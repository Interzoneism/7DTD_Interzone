using System;
using Unity.Collections.LowLevel.Unsafe;

// Token: 0x02001190 RID: 4496
public static class FastEnumConverter<TEnum> where TEnum : struct, IConvertible
{
	// Token: 0x06008C82 RID: 35970 RVA: 0x00388789 File Offset: 0x00386989
	public static int ToInt(TEnum _enum)
	{
		return (int)((long)UnsafeUtility.EnumToInt<TEnum>(_enum) & FastEnumConverter<TEnum>.underlyingMask);
	}

	// Token: 0x04006D4B RID: 27979
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly int underlyingSize = UnsafeUtility.SizeOf(Enum.GetUnderlyingType(typeof(TEnum)));

	// Token: 0x04006D4C RID: 27980
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly long underlyingMask = (FastEnumConverter<TEnum>.underlyingSize >= 8) ? -1L : ((1L << FastEnumConverter<TEnum>.underlyingSize * 8) - 1L);
}
