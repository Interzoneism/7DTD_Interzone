using System;

// Token: 0x02000344 RID: 836
public abstract class DynamicMeshServerData : NetPackage
{
	// Token: 0x0600187A RID: 6266
	public abstract bool Prechecks();

	// Token: 0x0600187B RID: 6267 RVA: 0x00057732 File Offset: 0x00055932
	[PublicizedFrom(EAccessModifier.Protected)]
	public DynamicMeshServerData()
	{
	}

	// Token: 0x04000FA5 RID: 4005
	public int X;

	// Token: 0x04000FA6 RID: 4006
	public int Z;
}
