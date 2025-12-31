using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200118B RID: 4491
public class Vector3EqualityComparer : IEqualityComparer<Vector3>
{
	// Token: 0x06008C40 RID: 35904 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3EqualityComparer()
	{
	}

	// Token: 0x06008C41 RID: 35905 RVA: 0x003877F2 File Offset: 0x003859F2
	public bool Equals(Vector3 _a, Vector3 _b)
	{
		return _a == _b;
	}

	// Token: 0x06008C42 RID: 35906 RVA: 0x003877FB File Offset: 0x003859FB
	public int GetHashCode(Vector3 _a)
	{
		return _a.GetHashCode();
	}

	// Token: 0x04006D42 RID: 27970
	public static readonly Vector3EqualityComparer Instance = new Vector3EqualityComparer();
}
