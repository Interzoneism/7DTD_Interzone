using System;
using System.IO;

// Token: 0x020011F3 RID: 4595
public class PooledExpandableMemoryStream : MemoryStream, IMemoryPoolableObject, IDisposable
{
	// Token: 0x06008FA3 RID: 36771 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Close()
	{
	}

	// Token: 0x06008FA4 RID: 36772 RVA: 0x001F6BA0 File Offset: 0x001F4DA0
	public void Reset()
	{
		this.SetLength(0L);
	}

	// Token: 0x06008FA5 RID: 36773 RVA: 0x00395C80 File Offset: 0x00393E80
	public void Cleanup()
	{
		this.Reset();
	}

	// Token: 0x06008FA6 RID: 36774 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Dispose(bool _disposing)
	{
	}

	// Token: 0x06008FA7 RID: 36775 RVA: 0x00395C88 File Offset: 0x00393E88
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose()
	{
		MemoryPools.poolMemoryStream.FreeSync(this);
	}
}
