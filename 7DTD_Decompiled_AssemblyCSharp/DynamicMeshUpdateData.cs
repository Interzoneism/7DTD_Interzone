using System;

// Token: 0x02000340 RID: 832
public class DynamicMeshUpdateData
{
	// Token: 0x06001872 RID: 6258 RVA: 0x0009612B File Offset: 0x0009432B
	public string ToDebugLocation()
	{
		return this.ChunkPosition.x.ToString() + "," + this.ChunkPosition.z.ToString();
	}

	// Token: 0x04000F91 RID: 3985
	public Vector3i ChunkPosition;

	// Token: 0x04000F92 RID: 3986
	public long Key;

	// Token: 0x04000F93 RID: 3987
	public float MaxTime;

	// Token: 0x04000F94 RID: 3988
	public float UpdateTime;

	// Token: 0x04000F95 RID: 3989
	public bool IsUrgent;

	// Token: 0x04000F96 RID: 3990
	public bool AddToThread;
}
