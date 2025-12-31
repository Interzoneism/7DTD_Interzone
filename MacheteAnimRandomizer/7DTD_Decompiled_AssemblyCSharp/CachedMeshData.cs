using System;
using UnityEngine;

// Token: 0x02001184 RID: 4484
[Serializable]
public class CachedMeshData
{
	// Token: 0x06008C1B RID: 35867 RVA: 0x00386EB0 File Offset: 0x003850B0
	public bool ApproximatelyEquals(CachedMeshData other)
	{
		return this.name.Equals(other.name) && Mathf.Abs(this.vertexCount - other.vertexCount) < 10 && Mathf.Abs(this.triCount - other.triCount) < 10;
	}

	// Token: 0x04006D32 RID: 27954
	public string name;

	// Token: 0x04006D33 RID: 27955
	public int vertexCount;

	// Token: 0x04006D34 RID: 27956
	public int triCount;
}
