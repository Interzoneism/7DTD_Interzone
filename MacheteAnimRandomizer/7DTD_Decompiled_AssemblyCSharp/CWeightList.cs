using System;
using System.Collections;
using UnityEngine;

// Token: 0x02001066 RID: 4198
[PublicizedFrom(EAccessModifier.Internal)]
public class CWeightList
{
	// Token: 0x060084AC RID: 33964 RVA: 0x0036108C File Offset: 0x0035F28C
	public CWeightList()
	{
		this.weights = new ArrayList();
	}

	// Token: 0x040066D3 RID: 26323
	public Transform transform;

	// Token: 0x040066D4 RID: 26324
	public ArrayList weights;
}
