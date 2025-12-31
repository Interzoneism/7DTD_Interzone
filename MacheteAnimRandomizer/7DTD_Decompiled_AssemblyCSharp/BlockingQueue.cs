using System;
using System.Collections.Generic;
using System.Threading;

// Token: 0x0200113E RID: 4414
public class BlockingQueue<T>
{
	// Token: 0x06008A91 RID: 35473 RVA: 0x00380B40 File Offset: 0x0037ED40
	public void Enqueue(T item)
	{
		Queue<T> obj = this.queue;
		lock (obj)
		{
			this.queue.Enqueue(item);
			Monitor.PulseAll(this.queue);
		}
	}

	// Token: 0x06008A92 RID: 35474 RVA: 0x00380B94 File Offset: 0x0037ED94
	public T Dequeue()
	{
		Queue<T> obj = this.queue;
		T result;
		lock (obj)
		{
			while (this.queue.Count == 0)
			{
				if (this.closing)
				{
					result = default(T);
					return result;
				}
				Monitor.Wait(this.queue);
			}
			result = this.queue.Dequeue();
		}
		return result;
	}

	// Token: 0x06008A93 RID: 35475 RVA: 0x00380C0C File Offset: 0x0037EE0C
	public bool HasData()
	{
		Queue<T> obj = this.queue;
		bool result;
		lock (obj)
		{
			result = (this.queue.Count > 0);
		}
		return result;
	}

	// Token: 0x06008A94 RID: 35476 RVA: 0x00380C58 File Offset: 0x0037EE58
	public void Close()
	{
		Queue<T> obj = this.queue;
		lock (obj)
		{
			this.closing = true;
			Monitor.PulseAll(this.queue);
		}
	}

	// Token: 0x06008A95 RID: 35477 RVA: 0x00380CA4 File Offset: 0x0037EEA4
	public void Clear()
	{
		Queue<T> obj = this.queue;
		lock (obj)
		{
			this.queue.Clear();
		}
	}

	// Token: 0x04006C69 RID: 27753
	[PublicizedFrom(EAccessModifier.Private)]
	public bool closing;

	// Token: 0x04006C6A RID: 27754
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Queue<T> queue = new Queue<T>();
}
