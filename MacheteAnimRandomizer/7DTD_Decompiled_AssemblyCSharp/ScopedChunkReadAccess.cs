using System;

// Token: 0x02000B6D RID: 2925
public struct ScopedChunkReadAccess : IDisposable
{
	// Token: 0x17000944 RID: 2372
	// (get) Token: 0x06005AD7 RID: 23255 RVA: 0x002470AC File Offset: 0x002452AC
	// (set) Token: 0x06005AD8 RID: 23256 RVA: 0x002470B4 File Offset: 0x002452B4
	public Chunk Chunk { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06005AD9 RID: 23257 RVA: 0x002470BD File Offset: 0x002452BD
	public ScopedChunkReadAccess(Chunk chunk)
	{
		this.Chunk = chunk;
		Chunk chunk2 = this.Chunk;
		if (chunk2 == null)
		{
			return;
		}
		chunk2.EnterReadLock();
	}

	// Token: 0x06005ADA RID: 23258 RVA: 0x002470D6 File Offset: 0x002452D6
	public void Dispose()
	{
		Chunk chunk = this.Chunk;
		if (chunk == null)
		{
			return;
		}
		chunk.ExitReadLock();
	}
}
