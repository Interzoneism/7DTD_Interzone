using System;

// Token: 0x0200019C RID: 412
public interface IEntityBuffsChanged
{
	// Token: 0x06000C9D RID: 3229
	void EntityBuffAdded(BuffValue _buff);

	// Token: 0x06000C9E RID: 3230
	void EntityBuffRemoved(BuffValue _buff);
}
