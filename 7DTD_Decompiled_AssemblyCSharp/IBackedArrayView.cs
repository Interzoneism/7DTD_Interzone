using System;
using System.Runtime.CompilerServices;

// Token: 0x02001127 RID: 4391
public interface IBackedArrayView<[IsUnmanaged] T> : IDisposable where T : struct, ValueType
{
	// Token: 0x17000E63 RID: 3683
	// (get) Token: 0x060089FA RID: 35322
	int Length { get; }

	// Token: 0x17000E64 RID: 3684
	// (get) Token: 0x060089FB RID: 35323
	BackedArrayHandleMode Mode { get; }

	// Token: 0x17000E65 RID: 3685
	T this[int i]
	{
		get;
		set;
	}

	// Token: 0x060089FE RID: 35326
	void Flush();
}
