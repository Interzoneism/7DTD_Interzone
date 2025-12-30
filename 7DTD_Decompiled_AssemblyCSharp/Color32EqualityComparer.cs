using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200118A RID: 4490
public class Color32EqualityComparer : IEqualityComparer<Color32>
{
	// Token: 0x06008C3C RID: 35900 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32EqualityComparer()
	{
	}

	// Token: 0x06008C3D RID: 35901 RVA: 0x00387755 File Offset: 0x00385955
	public bool Equals(Color32 _a, Color32 _b)
	{
		return _a.r == _b.r && _a.g == _b.g && _a.b == _b.b && _a.a == _b.a;
	}

	// Token: 0x06008C3E RID: 35902 RVA: 0x00387794 File Offset: 0x00385994
	public int GetHashCode(Color32 _a)
	{
		return ((_a.r.GetHashCode() * 397 ^ _a.g.GetHashCode()) * 397 ^ _a.b.GetHashCode()) * 397 ^ _a.a.GetHashCode();
	}

	// Token: 0x04006D41 RID: 27969
	public static readonly Color32EqualityComparer Instance = new Color32EqualityComparer();
}
