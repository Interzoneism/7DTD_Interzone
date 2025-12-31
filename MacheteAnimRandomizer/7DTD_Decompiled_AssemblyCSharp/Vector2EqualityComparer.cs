using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200118D RID: 4493
public class Vector2EqualityComparer : IEqualityComparer<Vector2>
{
	// Token: 0x06008C49 RID: 35913 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2EqualityComparer()
	{
	}

	// Token: 0x06008C4A RID: 35914 RVA: 0x00387897 File Offset: 0x00385A97
	public bool Equals(Vector2 _a, Vector2 _b)
	{
		return _a == _b;
	}

	// Token: 0x06008C4B RID: 35915 RVA: 0x003878A0 File Offset: 0x00385AA0
	public int GetHashCode(Vector2 _a)
	{
		return _a.GetHashCode();
	}

	// Token: 0x04006D45 RID: 27973
	public static readonly Vector2EqualityComparer Instance = new Vector2EqualityComparer();
}
