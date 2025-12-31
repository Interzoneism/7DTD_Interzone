using System;
using UnityEngine;

// Token: 0x02001065 RID: 4197
[PublicizedFrom(EAccessModifier.Internal)]
public class CVertexWeight
{
	// Token: 0x060084AB RID: 33963 RVA: 0x0036106F File Offset: 0x0035F26F
	public CVertexWeight(int i, Vector3 p, float w)
	{
		this.index = i;
		this.localPosition = p;
		this.weight = w;
	}

	// Token: 0x040066D0 RID: 26320
	public int index;

	// Token: 0x040066D1 RID: 26321
	public Vector3 localPosition;

	// Token: 0x040066D2 RID: 26322
	public float weight;
}
