using System;

// Token: 0x02000A57 RID: 2647
public class NoThreadingSemantics : IThreadingSemantics
{
	// Token: 0x060050B1 RID: 20657 RVA: 0x002014F0 File Offset: 0x001FF6F0
	public void Synchronize(AtomicActionDelegate _delegate)
	{
		_delegate();
	}

	// Token: 0x060050B2 RID: 20658 RVA: 0x002014F8 File Offset: 0x001FF6F8
	public T Synchronize<T>(Func<T> _delegate)
	{
		return _delegate();
	}

	// Token: 0x060050B3 RID: 20659 RVA: 0x00201500 File Offset: 0x001FF700
	public int InterlockedAdd(ref int _number, int _add)
	{
		_number += _add;
		return _number;
	}
}
