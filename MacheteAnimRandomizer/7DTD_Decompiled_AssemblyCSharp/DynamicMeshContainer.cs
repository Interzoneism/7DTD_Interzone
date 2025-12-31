using System;
using UnityEngine;

// Token: 0x0200032C RID: 812
public abstract class DynamicMeshContainer
{
	// Token: 0x060017A2 RID: 6050 RVA: 0x000903B1 File Offset: 0x0008E5B1
	public string ToDebugLocation()
	{
		return this.WorldPosition.x.ToString() + " " + this.WorldPosition.z.ToString();
	}

	// Token: 0x060017A3 RID: 6051
	public abstract GameObject GetGameObject();

	// Token: 0x060017A4 RID: 6052 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicMeshContainer()
	{
	}

	// Token: 0x04000EEE RID: 3822
	public Vector3i WorldPosition;

	// Token: 0x04000EEF RID: 3823
	public long Key;
}
