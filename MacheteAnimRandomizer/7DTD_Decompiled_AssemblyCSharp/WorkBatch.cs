using System;
using System.Collections.Generic;

// Token: 0x0200109A RID: 4250
public class WorkBatch<T>
{
	// Token: 0x06008616 RID: 34326 RVA: 0x00367904 File Offset: 0x00365B04
	public WorkBatch()
	{
		this.queuingList = new List<T>();
		this.workingList = new List<T>();
		this.sync = new object();
	}

	// Token: 0x06008617 RID: 34327 RVA: 0x00367930 File Offset: 0x00365B30
	public int Count()
	{
		int num = this.workingList.Count;
		object obj = this.sync;
		lock (obj)
		{
			num += this.queuingList.Count;
		}
		return num;
	}

	// Token: 0x06008618 RID: 34328 RVA: 0x00367988 File Offset: 0x00365B88
	public void Clear()
	{
		object obj = this.sync;
		lock (obj)
		{
			this.queuingList.Clear();
		}
		this.workingList.Clear();
	}

	// Token: 0x06008619 RID: 34329 RVA: 0x003679D8 File Offset: 0x00365BD8
	public void DoWork(Action<T> _action)
	{
		this.FlipLists();
		this.workingList.ForEach(_action);
		this.workingList.Clear();
	}

	// Token: 0x0600861A RID: 34330 RVA: 0x003679F8 File Offset: 0x00365BF8
	public void Add(T _item)
	{
		object obj = this.sync;
		lock (obj)
		{
			this.queuingList.Add(_item);
		}
	}

	// Token: 0x0600861B RID: 34331 RVA: 0x00367A40 File Offset: 0x00365C40
	[PublicizedFrom(EAccessModifier.Private)]
	public void FlipLists()
	{
		object obj = this.sync;
		lock (obj)
		{
			List<T> list = this.workingList;
			this.workingList = this.queuingList;
			this.queuingList = list;
		}
	}

	// Token: 0x0400682E RID: 26670
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T> queuingList;

	// Token: 0x0400682F RID: 26671
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T> workingList;

	// Token: 0x04006830 RID: 26672
	[PublicizedFrom(EAccessModifier.Private)]
	public object sync;
}
