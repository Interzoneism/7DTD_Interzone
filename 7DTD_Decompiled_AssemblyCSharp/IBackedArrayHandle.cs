using System;

// Token: 0x02001126 RID: 4390
public interface IBackedArrayHandle : IDisposable
{
	// Token: 0x17000E62 RID: 3682
	// (get) Token: 0x060089F8 RID: 35320
	BackedArrayHandleMode Mode { get; }

	// Token: 0x060089F9 RID: 35321
	void Flush();
}
