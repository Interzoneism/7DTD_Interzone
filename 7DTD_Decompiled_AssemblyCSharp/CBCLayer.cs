using System;

// Token: 0x020009A1 RID: 2465
public class CBCLayer : IMemoryPoolableObject
{
	// Token: 0x06004B12 RID: 19218 RVA: 0x001DAD40 File Offset: 0x001D8F40
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~CBCLayer()
	{
	}

	// Token: 0x06004B13 RID: 19219 RVA: 0x00002914 File Offset: 0x00000B14
	public void Reset()
	{
	}

	// Token: 0x06004B14 RID: 19220 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06004B15 RID: 19221 RVA: 0x001DAD68 File Offset: 0x001D8F68
	public void CopyFrom(CBCLayer _other)
	{
		Array.Copy(_other.data, this.data, this.data.Length);
	}

	// Token: 0x04003998 RID: 14744
	public readonly byte[] data = new byte[1024];

	// Token: 0x04003999 RID: 14745
	public static int InstanceCount;
}
