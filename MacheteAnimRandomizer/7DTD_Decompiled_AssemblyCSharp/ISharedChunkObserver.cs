using System;

// Token: 0x02000A39 RID: 2617
public interface ISharedChunkObserver : IDisposable
{
	// Token: 0x0600500B RID: 20491
	void Reference();

	// Token: 0x17000825 RID: 2085
	// (get) Token: 0x0600500C RID: 20492
	Vector2i ChunkPos { get; }

	// Token: 0x17000826 RID: 2086
	// (get) Token: 0x0600500D RID: 20493
	SharedChunkObserverCache Owner { get; }
}
