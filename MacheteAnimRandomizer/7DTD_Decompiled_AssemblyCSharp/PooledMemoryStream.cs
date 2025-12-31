using System;
using System.IO;

// Token: 0x02000A29 RID: 2601
public class PooledMemoryStream : MemoryStream, IMemoryPoolableObject
{
	// Token: 0x06004F7E RID: 20350 RVA: 0x001F6B78 File Offset: 0x001F4D78
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~PooledMemoryStream()
	{
	}

	// Token: 0x06004F7F RID: 20351 RVA: 0x001F6BA0 File Offset: 0x001F4DA0
	public void Reset()
	{
		this.SetLength(0L);
	}

	// Token: 0x06004F80 RID: 20352 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x04003CBE RID: 15550
	public static int InstanceCount;
}
