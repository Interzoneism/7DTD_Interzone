using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

// Token: 0x020011AD RID: 4525
[PublicizedFrom(EAccessModifier.Internal)]
public static class IEnumerableExtensions
{
	// Token: 0x06008D84 RID: 36228 RVA: 0x0038DC7D File Offset: 0x0038BE7D
	[Obsolete("Causes allocations, use Length or Count==0")]
	public static bool IsEmpty<a>(this IEnumerable<a> A_0)
	{
		return !A_0.Any<a>();
	}

	// Token: 0x06008D85 RID: 36229 RVA: 0x0038DC88 File Offset: 0x0038BE88
	public static string Join<T>(this IEnumerable<T> A_0)
	{
		return string.Join(", ", A_0.Select(new Func<T, string>(IEnumerableExtensions.Join<T>)).ToArray<string>());
	}

	// Token: 0x06008D86 RID: 36230 RVA: 0x0038DCAB File Offset: 0x0038BEAB
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public static string Join<T>(T A_0)
	{
		return A_0.ToString();
	}
}
