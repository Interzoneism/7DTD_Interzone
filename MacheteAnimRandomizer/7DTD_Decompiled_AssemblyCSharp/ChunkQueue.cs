using System;
using ConcurrentCollections;

// Token: 0x02000312 RID: 786
public class ChunkQueue
{
	// Token: 0x06001650 RID: 5712 RVA: 0x00081E80 File Offset: 0x00080080
	public void Add(long item)
	{
		object @lock = this._lock;
		lock (@lock)
		{
			this.KeyQueue.Add(item);
		}
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x00081EC8 File Offset: 0x000800C8
	public void Clear()
	{
		this.KeyQueue.Clear();
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x00081ED5 File Offset: 0x000800D5
	public bool Contains(long item)
	{
		return this.KeyQueue.Contains(item);
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x00081EE3 File Offset: 0x000800E3
	public void Remove(long item)
	{
		this.KeyQueue.TryRemove(item);
	}

	// Token: 0x04000E14 RID: 3604
	[PublicizedFrom(EAccessModifier.Private)]
	public object _lock = new object();

	// Token: 0x04000E15 RID: 3605
	[PublicizedFrom(EAccessModifier.Private)]
	public ConcurrentHashSet<long> KeyQueue = new ConcurrentHashSet<long>();
}
