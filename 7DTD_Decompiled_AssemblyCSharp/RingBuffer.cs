using System;

// Token: 0x02001203 RID: 4611
public class RingBuffer<T>
{
	// Token: 0x06008FF4 RID: 36852 RVA: 0x00397476 File Offset: 0x00395676
	public RingBuffer(int _count)
	{
		this.data = new T[_count];
	}

	// Token: 0x06008FF5 RID: 36853 RVA: 0x0039748C File Offset: 0x0039568C
	public void Add(T _el)
	{
		T[] array = this.data;
		int num = this.idx;
		this.idx = num + 1;
		array[num] = _el;
		if (this.idx >= this.data.Length)
		{
			this.idx = 0;
		}
		this.count++;
		if (this.count > this.data.Length)
		{
			this.count = this.data.Length;
		}
	}

	// Token: 0x17000EE7 RID: 3815
	// (get) Token: 0x06008FF6 RID: 36854 RVA: 0x003974F9 File Offset: 0x003956F9
	public int Count
	{
		get
		{
			return this.count;
		}
	}

	// Token: 0x06008FF7 RID: 36855 RVA: 0x00397501 File Offset: 0x00395701
	public void Clear()
	{
		this.count = 0;
		this.idx = 0;
	}

	// Token: 0x06008FF8 RID: 36856 RVA: 0x00397511 File Offset: 0x00395711
	public void SetToLast()
	{
		this.readIdx = this.idx - 1;
		if (this.readIdx < 0)
		{
			this.readIdx = this.data.Length - 1;
		}
	}

	// Token: 0x06008FF9 RID: 36857 RVA: 0x0039753A File Offset: 0x0039573A
	public T Peek()
	{
		return this.data[this.readIdx];
	}

	// Token: 0x06008FFA RID: 36858 RVA: 0x00397550 File Offset: 0x00395750
	public T GetPrev()
	{
		T[] array = this.data;
		int num = this.readIdx;
		this.readIdx = num - 1;
		T result = array[num];
		if (this.readIdx < 0)
		{
			this.readIdx = this.data.Length - 1;
		}
		return result;
	}

	// Token: 0x06008FFB RID: 36859 RVA: 0x00397594 File Offset: 0x00395794
	public T GetNext()
	{
		T[] array = this.data;
		int num = this.readIdx;
		this.readIdx = num + 1;
		T result = array[num];
		if (this.readIdx >= this.data.Length)
		{
			this.readIdx = 0;
		}
		return result;
	}

	// Token: 0x04006EE8 RID: 28392
	[PublicizedFrom(EAccessModifier.Private)]
	public T[] data;

	// Token: 0x04006EE9 RID: 28393
	[PublicizedFrom(EAccessModifier.Private)]
	public int idx;

	// Token: 0x04006EEA RID: 28394
	[PublicizedFrom(EAccessModifier.Private)]
	public int count;

	// Token: 0x04006EEB RID: 28395
	[PublicizedFrom(EAccessModifier.Private)]
	public int readIdx;
}
