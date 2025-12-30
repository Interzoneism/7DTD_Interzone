using System;

// Token: 0x02000A88 RID: 2696
public class BiomePrefabDecoration
{
	// Token: 0x0600534A RID: 21322 RVA: 0x00215946 File Offset: 0x00213B46
	public BiomePrefabDecoration(string _prefabName, float _prob, bool _isDecorateOnSlopes, int _checkResource = 2147483647)
	{
		this.prefabName = _prefabName;
		this.prob = _prob;
		this.checkResourceOffsetY = _checkResource;
		this.isDecorateOnSlopes = _isDecorateOnSlopes;
	}

	// Token: 0x04003F79 RID: 16249
	public string prefabName;

	// Token: 0x04003F7A RID: 16250
	public float prob;

	// Token: 0x04003F7B RID: 16251
	public int checkResourceOffsetY;

	// Token: 0x04003F7C RID: 16252
	public bool isDecorateOnSlopes;
}
