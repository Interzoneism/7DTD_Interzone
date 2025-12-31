using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200118C RID: 4492
public class Vector3ToFixedEqualityComparer : IEqualityComparer<Vector3>
{
	// Token: 0x06008C44 RID: 35908 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3ToFixedEqualityComparer()
	{
	}

	// Token: 0x06008C45 RID: 35909 RVA: 0x00387816 File Offset: 0x00385A16
	public bool Equals(Vector3 _a, Vector3 _b)
	{
		return this.GetHashCode(_a) == this.GetHashCode(_b);
	}

	// Token: 0x06008C46 RID: 35910 RVA: 0x00387828 File Offset: 0x00385A28
	public int GetHashCode(Vector3 _a)
	{
		return this.FloatToFixed(_a.x).GetHashCode() ^ this.FloatToFixed(_a.y).GetHashCode() << 2 ^ this.FloatToFixed(_a.z).GetHashCode() >> 2;
	}

	// Token: 0x06008C47 RID: 35911 RVA: 0x00387877 File Offset: 0x00385A77
	[PublicizedFrom(EAccessModifier.Private)]
	public int FloatToFixed(float a)
	{
		return (int)Math.Round((double)a * 32.0);
	}

	// Token: 0x04006D43 RID: 27971
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFixedPointFractionalBits = 5;

	// Token: 0x04006D44 RID: 27972
	public static readonly Vector3ToFixedEqualityComparer Instance = new Vector3ToFixedEqualityComparer();
}
