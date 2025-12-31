using System;
using System.Threading;

// Token: 0x02000A56 RID: 2646
public class ThreadSafeSemantics : IThreadingSemantics
{
	// Token: 0x060050AC RID: 20652 RVA: 0x0020143F File Offset: 0x001FF63F
	public ThreadSafeSemantics()
	{
		this.monitor = this;
	}

	// Token: 0x060050AD RID: 20653 RVA: 0x0020144E File Offset: 0x001FF64E
	public ThreadSafeSemantics(object _monitor)
	{
		this.monitor = _monitor;
	}

	// Token: 0x060050AE RID: 20654 RVA: 0x00201460 File Offset: 0x001FF660
	public void Synchronize(AtomicActionDelegate _delegate)
	{
		object obj = this.monitor;
		lock (obj)
		{
			_delegate();
		}
	}

	// Token: 0x060050AF RID: 20655 RVA: 0x002014A0 File Offset: 0x001FF6A0
	public int InterlockedAdd(ref int _number, int _add)
	{
		return Interlocked.Add(ref _number, _add);
	}

	// Token: 0x060050B0 RID: 20656 RVA: 0x002014AC File Offset: 0x001FF6AC
	public T Synchronize<T>(Func<T> _delegate)
	{
		object obj = this.monitor;
		T result;
		lock (obj)
		{
			result = _delegate();
		}
		return result;
	}

	// Token: 0x04003DD4 RID: 15828
	[PublicizedFrom(EAccessModifier.Private)]
	public object monitor;
}
