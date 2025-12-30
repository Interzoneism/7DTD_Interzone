using System;

// Token: 0x02000B6E RID: 2926
public struct ScopedChunkWriteAccess : IDisposable
{
	// Token: 0x17000945 RID: 2373
	// (get) Token: 0x06005ADB RID: 23259 RVA: 0x002470E8 File Offset: 0x002452E8
	// (set) Token: 0x06005ADC RID: 23260 RVA: 0x002470F0 File Offset: 0x002452F0
	public Chunk Chunk { readonly get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06005ADD RID: 23261 RVA: 0x002470F9 File Offset: 0x002452F9
	public ScopedChunkWriteAccess(Chunk chunk)
	{
		this.Chunk = chunk;
		Chunk chunk2 = this.Chunk;
		if (chunk2 == null)
		{
			return;
		}
		chunk2.EnterWriteLock();
	}

	// Token: 0x06005ADE RID: 23262 RVA: 0x00247112 File Offset: 0x00245312
	public void Dispose()
	{
		Chunk chunk = this.Chunk;
		if (chunk == null)
		{
			return;
		}
		chunk.ExitWriteLock();
	}
}
