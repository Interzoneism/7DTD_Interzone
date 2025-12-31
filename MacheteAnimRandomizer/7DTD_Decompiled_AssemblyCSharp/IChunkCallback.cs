using System;

// Token: 0x02000A91 RID: 2705
public interface IChunkCallback
{
	// Token: 0x0600537B RID: 21371
	void OnChunkAdded(Chunk _c);

	// Token: 0x0600537C RID: 21372
	void OnChunkBeforeRemove(Chunk _c);

	// Token: 0x0600537D RID: 21373
	void OnChunkBeforeSave(Chunk _c);
}
