using System;
using System.Collections.Generic;

// Token: 0x0200118F RID: 4495
public class FastEnumIntEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct, IConvertible
{
	// Token: 0x06008C7E RID: 35966 RVA: 0x00388766 File Offset: 0x00386966
	[PublicizedFrom(EAccessModifier.Private)]
	public int ToInt(TEnum _enum)
	{
		return EnumInt32ToInt.Convert<TEnum>(_enum);
	}

	// Token: 0x06008C7F RID: 35967 RVA: 0x0038876E File Offset: 0x0038696E
	public bool Equals(TEnum firstEnum, TEnum secondEnum)
	{
		return this.ToInt(firstEnum) == this.ToInt(secondEnum);
	}

	// Token: 0x06008C80 RID: 35968 RVA: 0x00388780 File Offset: 0x00386980
	public int GetHashCode(TEnum firstEnum)
	{
		return this.ToInt(firstEnum);
	}
}
