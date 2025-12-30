using System;

// Token: 0x02000A55 RID: 2645
public interface IThreadingSemantics
{
	// Token: 0x060050A9 RID: 20649
	void Synchronize(AtomicActionDelegate _delegate);

	// Token: 0x060050AA RID: 20650
	T Synchronize<T>(Func<T> _delegate);

	// Token: 0x060050AB RID: 20651
	int InterlockedAdd(ref int _number, int _add);
}
